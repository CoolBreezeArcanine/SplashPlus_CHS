using System;
using System.Collections.Generic;
using DB;
using MAI2.Util;
using Manager;
using Monitor.Game;
using Process;
using UnityEngine;

namespace Monitor
{
	public class SlideFan : SlideRoot
	{
		public const int ArrowMax = 11;

		public const int StarMax = 3;

		[SerializeField]
		private GameObject[] _arrowPrefubs = new GameObject[11];

		private bool[] _hitIns = new bool[3];

		[SerializeField]
		private Color[] _laneColor = new Color[2];

		private Color _defaultColor;

		protected int[] GoalButtonId = new int[3];

		private readonly SpriteRenderer[] _spriteLines = new SpriteRenderer[22];

		private readonly GameObject[] _starObjs = new GameObject[3];

		private readonly SpriteRenderer[] _spriteStars = new SpriteRenderer[3];

		private readonly List<Vector4>[] _slideVecList = new List<Vector4>[3];

		private readonly List<SlideManager.HitArea>[] _hitAreaList = new List<SlideManager.HitArea>[3];

		private readonly int[] _hitIndex = new int[3];

		private readonly int[] _hitSubIndex = new int[3];

		private void Awake()
		{
			int num = 0;
			for (int i = 0; i < 11; i++)
			{
				_spriteLines[num++] = _arrowPrefubs[i].GetComponent<SpriteRenderer>();
				_spriteLines[num++] = _arrowPrefubs[i].transform.GetChild(0).GetComponent<SpriteRenderer>();
			}
			for (int j = 0; j < 3; j++)
			{
				_starObjs[j] = UnityEngine.Object.Instantiate(_starPrefub);
				_starObjs[j].transform.SetParent(base.transform);
				_spriteStars[j] = _starObjs[j].GetComponent<SpriteRenderer>();
			}
		}

		public override void SetEach(bool eachFlag)
		{
			for (int i = 0; i < 3; i++)
			{
				_spriteStars[i].sprite = (eachFlag ? GameNoteImageContainer.EachStar[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.SlideDesign] : GameNoteImageContainer.NormalStar[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.SlideDesign, (int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.StarType]);
			}
			EachFlag = eachFlag;
			_defaultColor = _laneColor[EachFlag ? 1 : 0];
		}

		public override void Initialize(NoteData note)
		{
			NoteMng = NotesManager.Instance(base.MonitorId);
			StartPos = GameCtrl.NoteStartPos();
			EndPos = GameCtrl.NoteEndPos();
			AppearMsec = note.time.msec;
			TailMsec = note.end.msec;
			NoteIndex = note.indexNote;
			SlideIndex = note.indexSlide;
			base.ButtonId = note.startButtonPos;
			JudgeResult = NoteJudge.ETiming.End;
			JudgeTimingDiffMsec = 0f;
			EndFlag = false;
			DispJudge = false;
			PlaySlideSe = false;
			ShotJudgeSound = false;
			SetEach(note.isEach);
			DefaultMsec = (float)((double)GameManager.GetNoteSpeedForBeat((int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.GetNoteSpeed) * 4.0);
			StartMsec = AppearMsec - DefaultMsec * 2f;
			StarLaunchMsec = note.slideData.shoot.time.msec;
			StarArriveMsec = note.slideData.arrive.time.msec;
			GoalButtonId[0] = ((note.slideData.targetNote != 0) ? (note.slideData.targetNote - 1) : 7);
			GoalButtonId[1] = note.slideData.targetNote;
			GoalButtonId[2] = ((note.slideData.targetNote != 7) ? (note.slideData.targetNote + 1) : 0);
			base.transform.localRotation = Quaternion.Euler(0f, 0f, -45f * (float)base.ButtonId);
			for (int i = 0; i < 11; i++)
			{
				_arrowPrefubs[i].SetActive(value: true);
			}
			for (int j = 0; j < _spriteLines.Length; j++)
			{
				_spriteLines[j].color = new Color(_defaultColor.r, _defaultColor.g, _defaultColor.b, 0f);
				_spriteLines[j].sortingOrder = 32001 - (SlideIndex * SlideManager.MaxSlideLane + j);
			}
			for (int k = 0; k < 3; k++)
			{
				_slideVecList[k] = Singleton<SlideManager>.Instance.GetSlidePath(SlideType.Slide_Fan, note.startButtonPos, GoalButtonId[k]);
				_hitAreaList[k] = Singleton<SlideManager>.Instance.GetSlideHitArea(SlideType.Slide_Fan, note.startButtonPos, GoalButtonId[k]);
				_hitIndex[k] = 0;
				_hitSubIndex[k] = 0;
				_spriteStars[k].color = new Color(1f, 1f, 1f, 0f);
				_spriteStars[k].sortingOrder = 32001 - (SlideIndex * SlideManager.MaxSlideLane + k);
				_starObjs[k].SetActive(value: true);
				_starObjs[k].transform.localScale = new Vector3(0.75f, 0.75f, 1f);
				_starObjs[k].transform.localPosition = new Vector3(_slideVecList[k][0].x, _slideVecList[k][0].y, GetStarZPosition());
				_hitIns[k] = false;
				if (k == 1)
				{
					JudgeObj.transform.localPosition = new Vector3((float)((double)_slideVecList[k][_slideVecList[k].Count - 1].x * Math.Cos(Math.PI / 180.0 * (double)(-45f * (float)base.ButtonId)) - (double)_slideVecList[k][_slideVecList[k].Count - 1].y * Math.Sin(Math.PI / 180.0 * (double)(-45f * (float)base.ButtonId))), (float)((double)_slideVecList[k][_slideVecList[k].Count - 1].x * Math.Sin(Math.PI / 180.0 * (double)(-45f * (float)base.ButtonId)) + (double)_slideVecList[k][_slideVecList[k].Count - 1].y * Math.Cos(Math.PI / 180.0 * (double)(-45f * (float)base.ButtonId))), GetJudgeZPosition());
					JudgeObj.transform.localRotation = Quaternion.Euler(0f, 0f, _slideVecList[k][_slideVecList[k].Count - 1].w + 90f + -45f * (float)base.ButtonId);
					JudgeObj.SetJudgeType(SlideJudge.SlideJudgeType.Fan);
					int endButtonId = base.EndButtonId;
					if ((uint)(endButtonId - 2) <= 3u)
					{
						JudgeObj.Flip(flip: false);
						JudgeObj.transform.Rotate(0f, 0f, 180f);
					}
					else
					{
						JudgeObj.Flip(flip: true);
					}
				}
			}
			float num = StarArriveMsec - StarLaunchMsec;
			SlideManager.HitArea hitArea = _hitAreaList[1][_hitAreaList[1].Count - 1];
			lastWaitTime = num * (float)(hitArea.PushDistance / hitArea.ReleaseDistance);
			lastWaitTimeForJudge = lastWaitTime;
		}

		protected override void UpdateAlpha()
		{
			float currentMsec = NotesManager.GetCurrentMsec();
			float num = (AppearMsec - StartMsec) / 21f * (float)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.SlideSpeed;
			float num2 = StartMsec + num;
			float num3 = AppearMsec - currentMsec;
			Color defaultColor = _defaultColor;
			if (num2 <= currentMsec && num2 + 200f >= currentMsec)
			{
				float num4 = (defaultColor.a = 0.5f * ((currentMsec - num2) / 200f));
			}
			else if (AppearMsec <= currentMsec)
			{
				defaultColor.a = _defaultColor.a;
			}
			else if (num3 + 200f <= currentMsec)
			{
				defaultColor.a = 0.5f;
			}
			if (num2 >= NotesManager.GetCurrentMsec())
			{
				defaultColor.a = 0f;
			}
			SpriteRenderer[] spriteLines = _spriteLines;
			for (int i = 0; i < spriteLines.Length; i++)
			{
				spriteLines[i].color = defaultColor;
			}
		}

		protected override void MoveStarLane()
		{
			float currentMsec = NotesManager.GetCurrentMsec();
			float value = Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.SlideSize.GetValue();
			if (AppearMsec >= currentMsec)
			{
				Color color = new Color(1f, 1f, 1f, 0f);
				Vector3 localScale = new Vector3(0.75f * value, 0.75f * value, 1f);
				for (int i = 0; i < 3; i++)
				{
					_spriteStars[i].color = color;
					_starObjs[i].transform.localScale = localScale;
				}
			}
			else if (StarLaunchMsec >= currentMsec)
			{
				float num = currentMsec - AppearMsec;
				float num2 = StarLaunchMsec - AppearMsec;
				float num3 = num / num2;
				float a = 1f * num3;
				float num4 = (0.5f + 1f * num3) * value;
				for (int j = 0; j < 3; j++)
				{
					Vector4 vector = _slideVecList[j][0];
					_spriteStars[j].color = new Color(1f, 1f, 1f, a);
					_starObjs[j].transform.localScale = new Vector3(num4, num4, 1f);
					_starObjs[j].transform.transform.localRotation = Quaternion.Euler(0f, 0f, vector.w + 90f);
				}
			}
			else
			{
				if (!(StarLaunchMsec < currentMsec))
				{
					return;
				}
				float num5 = currentMsec - StarLaunchMsec;
				float num6 = StarArriveMsec - StarLaunchMsec;
				float num7 = num5 / num6;
				if (num7 >= 0f && num7 <= 1f)
				{
					for (int k = 0; k < 3; k++)
					{
						List<Vector4> list = _slideVecList[k];
						float num8 = list[list.Count - 1].z * num7;
						int num9 = 0;
						float t = 0f;
						for (int l = 0; l < list.Count; l++)
						{
							if (list[l].z <= num8 && num8 <= list[l + 1].z)
							{
								float num10 = list[l + 1].z - list[l].z;
								float num11 = num8 - list[l].z;
								num9 = l;
								t = num11 / num10;
								break;
							}
						}
						Vector4 vector2 = Vector4.LerpUnclamped(list[num9], list[num9 + 1], t);
						_spriteStars[k].color = new Color(1f, 1f, 1f, 1f);
						_starObjs[k].transform.localScale = new Vector3(1.5f * value, 1.5f * value, 1f);
						_starObjs[k].transform.localPosition = new Vector3(vector2.x, vector2.y, GetStarZPosition());
						_starObjs[k].transform.transform.localRotation = Quaternion.Euler(0f, 0f, vector2.w + 90f);
					}
				}
				else if (num7 > 1f)
				{
					for (int m = 0; m < 3; m++)
					{
						List<Vector4> obj = _slideVecList[m];
						Vector4 vector3 = obj[obj.Count - 1];
						_spriteStars[m].color = new Color(1f, 1f, 1f, 1f);
						_starObjs[m].transform.localScale = new Vector3(1.5f * value, 1.5f * value, 1f);
						_starObjs[m].transform.localPosition = new Vector3(vector3.x, vector3.y, GetStarZPosition());
						_starObjs[m].transform.transform.localRotation = Quaternion.Euler(0f, 0f, vector3.w + 90f);
					}
				}
			}
		}

		protected override void NoteCheck()
		{
			float currentMsec = NotesManager.GetCurrentMsec();
			if (IsNoteCheckTimeStartIgnoreJudgeWait())
			{
				int num = 0;
				float num2 = 10f;
				if (!GameManager.IsAutoPlay())
				{
					for (int i = 0; i < 3; i++)
					{
						if (_hitIndex[i] < _hitAreaList[i].Count)
						{
							bool flag = false;
							do
							{
								flag = CheckSlideTouch(_hitIndex[i], i, _hitIns[i]);
								if (!flag)
								{
									int num3 = _hitIndex[i] + 1;
									if (num3 < _hitAreaList[i].Count)
									{
										flag = CheckSlideTouch(num3, i, In: false);
									}
								}
								if (_hitIndex[i] >= _hitAreaList[i].Count)
								{
									flag = false;
								}
							}
							while (flag);
						}
						else
						{
							num++;
						}
						float num4 = (float)_hitIndex[i] / (float)_hitAreaList[i].Count;
						if (num2 >= num4)
						{
							num2 = num4;
						}
					}
					for (int j = 0; j < _arrowPrefubs.Length && (float)j < num2 * 11f; j++)
					{
						_arrowPrefubs[j].SetActive(value: false);
						if (!PlaySlideSe)
						{
							PlaySlideSe = true;
							ReserveSlideSe();
						}
					}
				}
				else
				{
					float num5 = currentMsec - StarLaunchMsec;
					float num6 = StarArriveMsec - StarLaunchMsec - lastWaitTime;
					float num7 = num5 / num6;
					for (int k = 0; k < _arrowPrefubs.Length && (float)k < num7 * 1f; k++)
					{
						_arrowPrefubs[k].SetActive(value: false);
						if (!PlaySlideSe)
						{
							PlaySlideSe = true;
							ReserveSlideSe();
						}
					}
					if (num7 > 1f)
					{
						num = 3;
					}
				}
				if (num >= 3)
				{
					Judge();
				}
			}
			if (GetJudgeResult() == NoteJudge.ETiming.End && !JudgeToolate())
			{
				return;
			}
			if (lastWaitTime <= 0f || GetJudgeResult() == NoteJudge.ETiming.TooLate)
			{
				if (GetJudgeResult() == NoteJudge.ETiming.TooLate)
				{
					for (int l = 0; l < 3 && _hitIndex[l] >= _hitAreaList[l].Count - 1; l++)
					{
						if (l == 2)
						{
							JudgeResult = NoteJudge.ETiming.LateGood;
						}
					}
				}
				PlayJudgeSe();
				EndFlag = true;
				base.gameObject.SetActive(value: false);
				base.transform.SetParent(base.ParentTransform, worldPositionStays: false);
				JudgeObj.Initialize(JudgeResult, JudgeTimingDiffMsec);
				DispJudge = true;
				SetPlayResult();
			}
			lastWaitTime -= GameManager.GetGameMSecAdd();
		}

		protected bool CheckSlideTouch(int index, int lane, bool In)
		{
			bool result = false;
			if (!In)
			{
				if (1 != lane || index < _hitAreaList[lane].Count - 2)
				{
					int num = 0;
					{
						foreach (InputManager.TouchPanelArea hitPoint in _hitAreaList[lane][index].HitPoints)
						{
							if (InputManager.ConvertTouchPanelRotatePush(MonitorIndex, hitPoint, base.ButtonId))
							{
								_hitIndex[lane] = index;
								_hitSubIndex[lane] = num;
								result = true;
								_hitIns[lane] = true;
								if (index == _hitAreaList[lane].Count - 1)
								{
									_hitIndex[lane] = index + 1;
								}
							}
						}
						return result;
					}
				}
				if (InputManager.ConvertTouchPanelRotatePush(MonitorIndex, _hitAreaList[lane][_hitAreaList[lane].Count - 1].HitPoints[0], base.ButtonId) || InputManager.ConvertTouchPanelRotatePush(MonitorIndex, _hitAreaList[lane][_hitAreaList[lane].Count - 2].HitPoints[0], base.ButtonId))
				{
					_hitIndex[lane] = index;
					result = true;
					_hitIns[lane] = true;
					if (index == _hitAreaList[lane].Count - 1)
					{
						_hitIndex[lane] = index + 1;
					}
				}
			}
			else if (!InputManager.ConvertTouchPanelRotatePush(MonitorIndex, _hitAreaList[lane][index].HitPoints[_hitSubIndex[lane]], base.ButtonId))
			{
				_hitIndex[lane] = index + 1;
				_hitSubIndex[lane] = 0;
				result = true;
				_hitIns[lane] = false;
			}
			return result;
		}
	}
}
