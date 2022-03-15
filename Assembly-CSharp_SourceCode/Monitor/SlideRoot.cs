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
	public class SlideRoot : MonoBehaviour
	{
		[SerializeField]
		private GameObject _arrowPrefub;

		[SerializeField]
		protected GameObject _starPrefub;

		protected int NoteIndex;

		protected int SlideIndex;

		protected const float NoteIndexZposDiff = 0.0001f;

		private readonly List<GameObject> _arrowList = new List<GameObject>();

		private GameObject _starNote;

		protected SlideJudge JudgeObj;

		protected const float StarScale = 1.5f;

		protected int MonitorIndex;

		private readonly List<SpriteRenderer> _spriteRenders = new List<SpriteRenderer>();

		protected SpriteRenderer SpriteRender;

		protected bool PlaySlideSe;

		protected bool EndFlag;

		protected bool DispJudge;

		protected float StartPos;

		protected float EndPos;

		protected bool EachFlag;

		protected NoteJudge.EJudgeType JudgeType = NoteJudge.EJudgeType.SlideOut;

		protected float DefaultMsec;

		protected float AppearMsec;

		protected float StarLaunchMsec;

		protected float StarArriveMsec;

		protected float TailMsec;

		protected float StartMsec;

		protected NoteJudge.ETiming JudgeResult;

		protected float JudgeTimingDiffMsec;

		private List<Vector4> _slideVecList;

		private List<SlideManager.HitArea> _hitAreaList;

		private int _hitIndex;

		private bool _hitIn;

		private int _hitSubIndex;

		protected bool ShotJudgeSound;

		protected float lastWaitTime;

		protected float lastWaitTimeForJudge;

		protected SlideType SlideType;

		protected NotesManager NoteMng;

		private int _dispLaneNum;

		public Transform ParentTransform { get; set; }

		public int ButtonId { get; set; }

		public int EndButtonId { get; set; }

		public int MonitorId
		{
			get
			{
				return MonitorIndex;
			}
			set
			{
				MonitorIndex = value;
			}
		}

		protected float GetLaneZPosition()
		{
			return -3f + (float)NoteIndex * 0.0001f;
		}

		protected float GetStarZPosition()
		{
			return -4f + (float)NoteIndex * 0.0001f;
		}

		protected float GetJudgeZPosition()
		{
			return -4f;
		}

		public List<GameObject> GetArrowObjList()
		{
			return _arrowList;
		}

		public bool IsEnd()
		{
			return EndFlag;
		}

		public NoteJudge.ETiming GetJudgeResult()
		{
			return JudgeResult;
		}

		public float GetJudgeStartMsec()
		{
			return NoteJudge.GetNoteCheckStart(JudgeType);
		}

		public float GetJudgeEndMsec()
		{
			return NoteJudge.GetNoteCheckEnd(JudgeType);
		}

		protected bool IsNoteCheckTimeStart(float judgeTimingFrame)
		{
			return NotesManager.GetCurrentMsec() - judgeTimingFrame * 16.666666f >= StarLaunchMsec + GetJudgeStartMsec();
		}

		protected bool IsNoteCheckTimeStartIgnoreJudgeWait()
		{
			return NotesManager.GetCurrentMsec() + NoteJudge.JudgeAdjustMs >= AppearMsec;
		}

		protected void ReserveSlideSe()
		{
			Singleton<GameSingleCueCtrl>.Instance.ReserveSlideSe(MonitorIndex);
		}

		private void Awake()
		{
			for (int i = 0; i < SlideManager.MaxSlideLane; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(_arrowPrefub);
				gameObject.transform.SetParent(base.transform);
				_arrowList.Add(gameObject);
				_spriteRenders.Add(gameObject.GetComponent<SpriteRenderer>());
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(_starPrefub);
			gameObject2.transform.SetParent(base.transform);
			_starNote = gameObject2;
			SpriteRender = gameObject2.GetComponent<SpriteRenderer>();
		}

		public void SetJudgeObject(SlideJudge slideJudge)
		{
			JudgeObj = slideJudge;
		}

		public virtual void SetEach(bool eachFlag)
		{
			foreach (SpriteRenderer spriteRender in _spriteRenders)
			{
				spriteRender.sprite = (eachFlag ? GameNoteImageContainer.EachlSlide[(int)Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.SlideDesign] : GameNoteImageContainer.NormalSlide[(int)Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.SlideDesign]);
			}
			SpriteRender.sprite = (eachFlag ? GameNoteImageContainer.EachStar[(int)Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.SlideDesign] : GameNoteImageContainer.NormalStar[(int)Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.SlideDesign, (int)Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.StarType]);
			EachFlag = eachFlag;
		}

		public virtual void Initialize(NoteData note)
		{
			NoteMng = NotesManager.Instance(MonitorId);
			SlideType = note.slideData.type;
			StartPos = GameCtrl.NoteStartPos();
			EndPos = GameCtrl.NoteEndPos();
			AppearMsec = note.time.msec;
			TailMsec = note.end.msec;
			NoteIndex = note.indexNote;
			SlideIndex = note.indexSlide;
			ButtonId = note.startButtonPos;
			EndButtonId = note.slideData.targetNote;
			SetEach(note.isEach);
			JudgeResult = NoteJudge.ETiming.End;
			JudgeTimingDiffMsec = 0f;
			EndFlag = false;
			DispJudge = false;
			PlaySlideSe = false;
			ShotJudgeSound = false;
			DefaultMsec = (float)((double)GameManager.GetNoteSpeedForBeat((int)Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.GetNoteSpeed) * 4.0);
			StartMsec = AppearMsec - DefaultMsec * 2f;
			StarLaunchMsec = note.slideData.shoot.time.msec;
			StarArriveMsec = note.slideData.arrive.time.msec;
			_slideVecList = Singleton<SlideManager>.Instance.GetSlidePath(note.slideData.type, note.startButtonPos, note.slideData.targetNote);
			_hitAreaList = Singleton<SlideManager>.Instance.GetSlideHitArea(note.slideData.type, note.startButtonPos, note.slideData.targetNote);
			_hitIndex = 0;
			_hitSubIndex = 0;
			_hitIn = false;
			base.transform.localRotation = Quaternion.Euler(0f, 0f, -45f * (float)ButtonId);
			float value = Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.SlideSize.GetValue();
			_dispLaneNum = _slideVecList.Count - 2;
			if (_slideVecList[_slideVecList.Count - 2].z + 23.556f < _slideVecList[_slideVecList.Count - 1].z)
			{
				_dispLaneNum++;
			}
			for (int i = 0; i < SlideManager.MaxSlideLane; i++)
			{
				int num = i + 1;
				if (_dispLaneNum > num)
				{
					Vector4 vector = _slideVecList[num];
					_arrowList[i].SetActive(value: true);
					_arrowList[i].transform.localPosition = new Vector3(vector.x, vector.y, GetLaneZPosition() + 0.01f * (1f - (float)i / (float)SlideManager.MaxSlideLane));
					_arrowList[i].transform.localRotation = Quaternion.Euler(0f, 0f, vector.w);
					_arrowList[i].transform.localScale = new Vector3(1f, value, 1f);
					_spriteRenders[i].color = new Color(1f, 1f, 1f, 1f);
					_spriteRenders[i].sortingOrder = -(SlideIndex * SlideManager.MaxSlideLane + i);
				}
				else
				{
					_arrowList[i].SetActive(value: false);
				}
			}
			SpriteRender.color = new Color(1f, 1f, 1f, 0f);
			SpriteRender.sortingOrder = 32001 - SlideIndex * SlideManager.MaxSlideLane;
			_starNote.SetActive(value: true);
			_starNote.transform.localScale = new Vector3(0.75f, 0.75f, 1f);
			_starNote.transform.localPosition = new Vector3(_slideVecList[0].x, _slideVecList[0].y, GetStarZPosition());
			_starNote.transform.localScale = new Vector3(value, value, 1f);
			if (null != JudgeObj)
			{
				Vector4 vector2 = _slideVecList[_slideVecList.Count - 1];
				JudgeObj.transform.localPosition = new Vector3((float)((double)vector2.x * Math.Cos(Math.PI / 180.0 * (double)(-45f * (float)ButtonId)) - (double)vector2.y * Math.Sin(Math.PI / 180.0 * (double)(-45f * (float)ButtonId))), (float)((double)vector2.x * Math.Sin(Math.PI / 180.0 * (double)(-45f * (float)ButtonId)) + (double)vector2.y * Math.Cos(Math.PI / 180.0 * (double)(-45f * (float)ButtonId))), GetJudgeZPosition());
				JudgeObj.transform.localRotation = Quaternion.Euler(0f, 0f, vector2.w + 180f + -45f * (float)ButtonId);
				if (note.slideData.type == SlideType.Slide_Circle_L)
				{
					JudgeObj.SetJudgeType(SlideJudge.SlideJudgeType.Circle);
					JudgeObj.transform.Rotate(0f, 0f, 158f);
					JudgeObj.Flip(flip: true);
				}
				else if (note.slideData.type == SlideType.Slide_Circle_R)
				{
					JudgeObj.SetJudgeType(SlideJudge.SlideJudgeType.Circle);
					JudgeObj.transform.Rotate(0f, 0f, 25f);
					JudgeObj.Flip(flip: false);
				}
				else
				{
					JudgeObj.SetJudgeType(SlideJudge.SlideJudgeType.Normal);
					int endButtonId = EndButtonId;
					if ((uint)endButtonId <= 3u)
					{
						JudgeObj.Flip(flip: false);
					}
					else
					{
						JudgeObj.Flip(flip: true);
						JudgeObj.transform.Rotate(0f, 0f, 180f);
					}
				}
			}
			float num2 = StarArriveMsec - StarLaunchMsec;
			SlideManager.HitArea hitArea = _hitAreaList[_hitAreaList.Count - 1];
			lastWaitTime = num2 * (float)(hitArea.PushDistance / hitArea.ReleaseDistance);
			lastWaitTimeForJudge = lastWaitTime;
		}

		public void Execute()
		{
			if (!EndFlag)
			{
				UpdateAlpha();
				MoveStarLane();
			}
			NoteCheck();
		}

		protected virtual void UpdateAlpha()
		{
			float currentMsec = NotesManager.GetCurrentMsec();
			float num = (AppearMsec - StartMsec) / 21f * (float)Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.SlideSpeed;
			float num2 = StartMsec + num;
			float num3 = AppearMsec - currentMsec;
			Color color = new Color(1f, 1f, 1f, 1f);
			if (num2 <= currentMsec && currentMsec < AppearMsec)
			{
				float num4 = 0f;
				if (AppearMsec - num2 < 200f)
				{
					num4 = 0.5f * (1f - num3 / (currentMsec - num2));
				}
				else
				{
					num4 = 0.5f * ((currentMsec - num2) / 200f);
					if (num4 > 0.5f)
					{
						num4 = 0.5f;
					}
				}
				color.a = num4;
			}
			else if (AppearMsec <= currentMsec)
			{
				color.a = 1f;
			}
			else
			{
				color.a = 0f;
			}
			for (int i = 0; i < _dispLaneNum; i++)
			{
				_spriteRenders[i].color = color;
			}
		}

		protected virtual void MoveStarLane()
		{
			float currentMsec = NotesManager.GetCurrentMsec();
			float value = Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.SlideSize.GetValue();
			if (AppearMsec >= currentMsec)
			{
				SpriteRender.color = new Color(1f, 1f, 1f, 0f);
				_starNote.transform.localScale = new Vector3(0.75f * value, 0.75f * value, 1f);
			}
			else if (StarLaunchMsec >= currentMsec)
			{
				float num = currentMsec - AppearMsec;
				float num2 = StarLaunchMsec - AppearMsec;
				float num3 = num / num2;
				float a = 1f * num3;
				float num4 = (0.5f + 1f * num3) * value;
				Vector4 vector = _slideVecList[0];
				SpriteRender.color = new Color(1f, 1f, 1f, a);
				_starNote.transform.localScale = new Vector3(num4, num4, 1f);
				_starNote.transform.transform.localRotation = Quaternion.Euler(0f, 0f, vector.w + 90f);
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
					float num8 = _slideVecList[_slideVecList.Count - 1].z * num7;
					int num9 = 0;
					float t = 0f;
					for (int i = 0; i < _slideVecList.Count; i++)
					{
						if (_slideVecList[i].z <= num8 && num8 <= _slideVecList[i + 1].z)
						{
							float num10 = _slideVecList[i + 1].z - _slideVecList[i].z;
							float num11 = num8 - _slideVecList[i].z;
							num9 = i;
							t = num11 / num10;
							break;
						}
					}
					Vector4 vector2 = Vector4.LerpUnclamped(_slideVecList[num9], _slideVecList[num9 + 1], t);
					float num12 = Mathf.LerpAngle(_slideVecList[num9].w, _slideVecList[num9 + 1].w, t);
					SpriteRender.color = new Color(1f, 1f, 1f, 1f);
					_starNote.transform.localScale = new Vector3(1.5f * value, 1.5f * value, 1f);
					_starNote.transform.localPosition = new Vector3(vector2.x, vector2.y, GetStarZPosition());
					_starNote.transform.transform.localRotation = Quaternion.Euler(0f, 0f, num12 + 90f);
				}
				else if (num7 > 1f)
				{
					Vector4 vector3 = _slideVecList[_slideVecList.Count - 1];
					SpriteRender.color = new Color(1f, 1f, 1f, 1f);
					_starNote.transform.localScale = new Vector3(1.5f * value, 1.5f * value, 1f);
					_starNote.transform.localPosition = new Vector3(vector3.x, vector3.y, GetStarZPosition());
					_starNote.transform.transform.localRotation = Quaternion.Euler(0f, 0f, vector3.w + 90f);
				}
			}
		}

		protected virtual void NoteCheck()
		{
			float currentMsec = NotesManager.GetCurrentMsec();
			if (IsNoteCheckTimeStartIgnoreJudgeWait() && _hitIndex < _hitAreaList.Count)
			{
				if (!GameManager.IsAutoPlay())
				{
					bool flag = false;
					bool flag2 = false;
					do
					{
						flag = CheckSlideTouch(_hitIndex, _hitIn);
						if (!flag && NextCheck())
						{
							int num = _hitIndex + 1;
							if (num < _hitAreaList.Count)
							{
								flag = CheckSlideTouch(num, In: false);
							}
						}
						if (flag)
						{
							double deleteArrowDistance = GetDeleteArrowDistance();
							int num2 = (int)((double)_dispLaneNum * deleteArrowDistance);
							for (int i = 0; i < _slideVecList.Count && num2 > i; i++)
							{
								_arrowList[i].SetActive(value: false);
							}
							if (!PlaySlideSe)
							{
								PlaySlideSe = true;
								ReserveSlideSe();
							}
						}
						else if (!flag2)
						{
							flag2 = true;
						}
						if (_hitIndex >= _hitAreaList.Count)
						{
							flag = false;
						}
					}
					while (flag);
				}
				else
				{
					float num3 = currentMsec - StarLaunchMsec;
					float num4 = StarArriveMsec - StarLaunchMsec - lastWaitTime;
					float num5 = num3 / num4;
					_hitIndex = (int)((float)_hitAreaList.Count * num5);
					if (_hitIndex >= _hitAreaList.Count)
					{
						_hitIndex = _hitAreaList.Count - 1;
					}
					if (_hitIndex < 0)
					{
						_hitIndex = 0;
					}
					double deleteArrowDistance2 = GetDeleteArrowDistance();
					int num6 = (int)((double)_dispLaneNum * deleteArrowDistance2);
					for (int j = 0; j < _slideVecList.Count && num6 > j; j++)
					{
						_arrowList[j].SetActive(value: false);
						if (!PlaySlideSe)
						{
							PlaySlideSe = true;
							ReserveSlideSe();
						}
					}
					if (num5 >= 1f)
					{
						_hitIndex = _hitAreaList.Count;
					}
				}
			}
			if (_hitIndex >= _hitAreaList.Count)
			{
				Judge();
			}
			if (GetJudgeResult() == NoteJudge.ETiming.End && !JudgeToolate())
			{
				return;
			}
			if (lastWaitTime <= 0f || GetJudgeResult() == NoteJudge.ETiming.TooLate)
			{
				if (GetJudgeResult() == NoteJudge.ETiming.TooLate && _hitIndex >= _hitAreaList.Count - 1)
				{
					JudgeResult = NoteJudge.ETiming.LateGood;
				}
				PlayJudgeSe();
				EndFlag = true;
				_starNote.SetActive(value: false);
				for (int k = 0; k < SlideManager.MaxSlideLane; k++)
				{
					_arrowList[k].SetActive(value: false);
				}
				JudgeObj.Initialize(JudgeResult, JudgeTimingDiffMsec);
				DispJudge = true;
				base.gameObject.SetActive(value: false);
				base.transform.SetParent(ParentTransform, worldPositionStays: false);
				SetPlayResult();
			}
			lastWaitTime -= GameManager.GetGameMSecAdd();
		}

		protected virtual double GetDeleteArrowDistance()
		{
			double num = 0.0;
			if (_hitIndex != 0)
			{
				for (int i = 0; i < _hitIndex; i++)
				{
					int num2 = _hitIndex - 1;
					num += _hitAreaList[i].PushDistance;
					if (_hitIn || num2 > i)
					{
						num += _hitAreaList[i].ReleaseDistance;
					}
				}
			}
			return num / _hitAreaList[_hitAreaList.Count - 1].ReleaseDistance;
		}

		protected bool CheckSlideTouch(int index, bool In)
		{
			bool result = false;
			if (!In)
			{
				int num = 0;
				{
					foreach (InputManager.TouchPanelArea hitPoint in _hitAreaList[index].HitPoints)
					{
						if (InputManager.ConvertTouchPanelRotatePush(MonitorIndex, hitPoint, ButtonId))
						{
							_hitIndex = index;
							_hitSubIndex = num;
							result = true;
							_hitIn = true;
							if (index == _hitAreaList.Count - 1)
							{
								_hitIndex = index + 1;
								return result;
							}
							return result;
						}
						num++;
					}
					return result;
				}
			}
			if (!InputManager.ConvertTouchPanelRotatePush(MonitorIndex, _hitAreaList[index].HitPoints[_hitSubIndex], ButtonId))
			{
				_hitIndex = index + 1;
				_hitSubIndex = 0;
				result = true;
				_hitIn = false;
			}
			return result;
		}

		protected virtual void SetPlayResult()
		{
			Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).SetResult(NoteIndex, NoteScore.EScoreType.Slide, GetJudgeResult());
		}

		public virtual void SetForcePlayResult(int monitorId, NoteData note, NoteJudge.ETiming timing)
		{
			Singleton<GamePlayManager>.Instance.GetGameScore(monitorId).SetResult(note.indexNote, NoteScore.EScoreType.Slide, timing);
		}

		protected bool Judge()
		{
			if (IsNoteCheckTimeStart(Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.GetJudgeTimingFrame()))
			{
				JudgeTimingDiffMsec = NotesManager.GetCurrentMsec() - TailMsec + lastWaitTime;
				JudgeResult = (GameManager.IsAutoPlay() ? GameManager.AutoJudge() : NoteJudge.GetSlideJudgeTiming(ref JudgeTimingDiffMsec, Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.GetJudgeTimingFrame(), JudgeType, lastWaitTimeForJudge));
				if (JudgeResult != NoteJudge.ETiming.End)
				{
					if (JudgeResult == NoteJudge.ETiming.TooFast)
					{
						JudgeResult = NoteJudge.ETiming.FastGood;
					}
					return true;
				}
				return false;
			}
			return false;
		}

		protected bool JudgeToolate()
		{
			if (GetJudgeResult() == NoteJudge.ETiming.TooLate)
			{
				return true;
			}
			if (GetJudgeResult() != NoteJudge.ETiming.End)
			{
				return false;
			}
			float _fMsec = NotesManager.GetCurrentMsec() - TailMsec;
			if (NoteJudge.GetSlideJudgeTiming(ref _fMsec, Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.GetJudgeTimingFrame(), JudgeType, lastWaitTime) == NoteJudge.ETiming.TooLate)
			{
				JudgeResult = NoteJudge.ETiming.TooLate;
				JudgeTimingDiffMsec = _fMsec;
			}
			return GetJudgeResult() == NoteJudge.ETiming.TooLate;
		}

		protected void PlayJudgeSe()
		{
			ShotJudgeSound = true;
		}

		private bool NextCheck()
		{
			bool flag = false;
			if (IsExtraSlideForNextCheck())
			{
				return IsExtraSlideCheck();
			}
			return IsUnderThreePointSlideCheck();
		}

		private bool IsUnderThreePointSlideCheck()
		{
			bool result = false;
			if (_hitAreaList.Count > 3)
			{
				result = true;
			}
			else if (_hitAreaList.Count <= 3 && _hitAreaList.Count - 1 != _hitIndex + 1)
			{
				result = true;
			}
			else if (_hitAreaList.Count <= 3 && _hitAreaList.Count - 1 == _hitIndex + 1 && _hitIn)
			{
				result = true;
			}
			return result;
		}

		private bool IsExtraSlideForNextCheck()
		{
			bool flag = false;
			SlideType slideType = SlideType;
			if ((uint)(slideType - 11) <= 1u)
			{
				return true;
			}
			return false;
		}

		private bool IsExtraSlideCheck()
		{
			bool flag = false;
			int slideTypeLength = Singleton<SlideManager>.Instance.GetSlideTypeLength(SlideType, ButtonId, EndButtonId);
			SlideType slideType = SlideType;
			if ((uint)(slideType - 11) <= 1u)
			{
				switch (slideTypeLength)
				{
				case 1:
				case 2:
				case 3:
					return ExtraHitAreaCheck(1);
				case 4:
					if (!ExtraHitAreaCheck(1) || !ExtraHitAreaCheck(3))
					{
						return false;
					}
					return true;
				default:
					return true;
				}
			}
			return true;
		}

		private bool ExtraHitAreaCheck(int hitCount)
		{
			if (_hitIndex == hitCount)
			{
				return _hitIn;
			}
			return true;
		}
	}
}
