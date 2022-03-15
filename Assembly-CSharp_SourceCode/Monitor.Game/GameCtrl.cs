using System.Collections.Generic;
using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager;
using Manager.UserDatas;
using UnityEngine;

namespace Monitor.Game
{
	public class GameCtrl : MonoBehaviour
	{
		[SerializeField]
		[Header("ノーツなどの親")]
		private GameObject _NoteRoot;

		protected bool _debugSkipNow;

		private const int ButtonNum = 8;

		private const int CenterNum = 1;

		private const int TapMaxNum = 64;

		private const int HoldMaxNum = 64;

		private const int StarMaxNum = 64;

		private const int BreakMaxNum = 64;

		private const int BreakStarMaxNum = 64;

		private const int TouchBMaxNum = 32;

		private const int TouchCMaxNum = 8;

		private const int SlideMaxNum = 24;

		private const int FanSlideMaxNum = 4;

		private const int SlideJudgeNum = 16;

		private const int GuideMaxNum = 128;

		private const int BarGuideMaxNum = 12;

		private static float _noteStartPos;

		private static float _noteEndPos;

		private static readonly float[] TouchStartPosY = new float[4];

		private int _slideJudgeIndex;

		[SerializeField]
		[Header("ランチャーオブジェクト群")]
		private GameObject _noteLauncherLayer;

		[SerializeField]
		private GameObject _touchBLauncherLayer;

		[SerializeField]
		private GameObject _touchELauncherLayer;

		[SerializeField]
		private GameObject _touchCLauncherLayer;

		[SerializeField]
		private GameObject _barGuideLayer;

		[SerializeField]
		private GameObject _slideLaneLayer;

		[SerializeField]
		private GameObject _trackSkipLaneLayer;

		[SerializeField]
		[Header("オブジェクトプール群")]
		private GameObject _NotePool;

		private GameObject _tapListParent;

		private GameObject _holdListParent;

		private GameObject _starListParent;

		private GameObject _breakStarListParent;

		private GameObject _breakListParent;

		private GameObject _touchListParent;

		private GameObject _touchCTapListParent;

		private GameObject _touchCHoldListParent;

		private GameObject _barGuideListParent;

		private GameObject _slideListParent;

		private GameObject _fanSlideListParent;

		private GameObject _guideListParent;

		private GameObject _slideJudgeListParent;

		[SerializeField]
		[Header("ムービーマスク")]
		private GameObject _movieMaskObj;

		private SpriteRenderer _movieMaskSprite;

		private float _movieMaskAlpha;

		[SerializeField]
		private SpriteRenderer _movieSprite;

		[SerializeField]
		private GameObject _tapCEffectObject;

		[SerializeField]
		private GameObject _guideEndPointObj;

		[SerializeField]
		[Header("テクスチャプリローダー")]
		private GameSpritePreLoader _TexturePreLoader;

		private double debugTimer;

		private long preCount;

		private long diffCount;

		private List<GameObject> _launcherObjectList;

		private List<GameObject> _touchBLauncherObjectList;

		private List<GameObject> _touchELauncherObjectList;

		private List<GameObject> _touchCLauncherObjectList;

		private List<GameObject> _slideLauncherObjectList;

		private List<TapNote> _tapObjectList;

		private List<HoldNote> _holdObjectList;

		private List<StarNote> _starObjectList;

		private List<BreakStarNote> _breakStarObjectList;

		private List<BreakNote> _breakObjectList;

		private List<TouchNoteB> _touchBObjectList;

		private List<TouchNoteC> _touchCTapObjectList;

		private List<TouchHoldC> _touchCHoldObjectList;

		private List<SlideRoot> _slideObjectList;

		private List<SlideFan> _fanSlideObjectList;

		private List<NoteGuide> _guideObjectList;

		private List<BarGuide> _barGuideObjectList;

		private List<TouchEffect> _touchEffectObjectList;

		private List<JudgeGrade> _judgeGradeObjectList;

		private List<JudgeTouchGrade> _judgeGradeTouchBObjectList;

		private List<TouchEffect> _touchEffectBObjectList;

		private List<TouchReserve> _touchReserveBObjectList;

		private List<JudgeTouchGrade> _judgeGradeTouchEObjectList;

		private List<TouchEffect> _touchEffectEObjectList;

		private List<TouchReserve> _touchReserveEObjectList;

		private List<JudgeTouchGrade> _judgeGradeTouchCObjectList;

		private List<TouchEffect> _touchEffectCObjectList;

		private List<TouchReserve> _touchReserveCObjectList;

		private List<SlideJudge> _judgeSlideObjectList;

		private TapCEffect _tapCEffectObj;

		private TrackSkip _trackSkipObject;

		private List<NoteBase> _activeNoteList = new List<NoteBase>(192);

		private List<SlideRoot> _activeSlideList = new List<SlideRoot>(28);

		private const string _noteStartName = "NoteStart";

		private const string _noteEndName = "NoteEnd";

		protected float _barTime;

		protected int monitorIndex;

		protected int preLoadedFrame;

		protected const int PreLoadWaitFrame = 10;

		private NotesManager NoteMng;

		private GameMonitor.GameModeEnum GameMode;

		private bool isActive;

		private List<TouchReserve.ReserveData> _targetIndexs = new List<TouchReserve.ReserveData>();

		private float guideSpeed4BeatTap;

		private float guideSpeed4BeatTouch;

		private float apperMsecTap;

		private float apperMsecTouch;

		public int MonitorIndex => monitorIndex;

		public static float NoteStartPos()
		{
			return _noteStartPos;
		}

		public static float NoteEndPos()
		{
			return _noteEndPos;
		}

		public static float TouchStartPos(int touchArea)
		{
			return TouchStartPosY[touchArea];
		}

		private GameCtrl()
		{
			_noteLauncherLayer = null;
			_touchELauncherLayer = null;
			_barGuideLayer = null;
			_slideLaneLayer = null;
			NoteMng = null;
			isActive = false;
		}

		public void SetNoteManager(NotesManager noteMng)
		{
			NoteMng = noteMng;
		}

		public void Initialize(int monIndex, GameMonitor.GameModeEnum gameMode)
		{
			monitorIndex = monIndex;
			GameMode = gameMode;
			preLoadedFrame = 0;
			if (Singleton<UserDataManager>.Instance.GetUserData(monIndex).IsEntry || GameMode == GameMonitor.GameModeEnum.Advertise)
			{
				isActive = true;
				CreateNoteLuncher();
				CreateNotePool();
				Singleton<GameSingleCueCtrl>.Instance.Initialize(monIndex);
				UserOption userOption = Singleton<GamePlayManager>.Instance.GetGameScore(monIndex).UserOption;
				_guideEndPointObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Process/Game/Sprites/Outline/Outline_" + $"{(int)userOption.OutlineDesign:D2}");
				_movieMaskAlpha = userOption.GetBgBrightness();
				if (!GameManager.IsNoteCheckMode)
				{
					_tapCEffectObj.PreLoad();
					foreach (TouchEffect touchEffectObject in _touchEffectObjectList)
					{
						touchEffectObject.PreLoad();
					}
					foreach (TouchEffect touchEffectBObject in _touchEffectBObjectList)
					{
						touchEffectBObject.PreLoad();
					}
					foreach (TouchEffect touchEffectEObject in _touchEffectEObjectList)
					{
						touchEffectEObject.PreLoad();
					}
					foreach (TouchEffect touchEffectCObject in _touchEffectCObjectList)
					{
						touchEffectCObject.PreLoad();
					}
					foreach (JudgeGrade judgeGradeObject in _judgeGradeObjectList)
					{
						judgeGradeObject.PreLoad();
					}
					foreach (JudgeTouchGrade judgeGradeTouchBObject in _judgeGradeTouchBObjectList)
					{
						judgeGradeTouchBObject.PreLoad();
					}
					foreach (JudgeTouchGrade judgeGradeTouchEObject in _judgeGradeTouchEObjectList)
					{
						judgeGradeTouchEObject.PreLoad();
					}
					foreach (JudgeTouchGrade judgeGradeTouchCObject in _judgeGradeTouchCObjectList)
					{
						judgeGradeTouchCObject.PreLoad();
					}
					foreach (SlideJudge judgeSlideObject in _judgeSlideObjectList)
					{
						judgeSlideObject.PreLoad();
					}
				}
				guideSpeed4BeatTap = GameManager.GetNoteSpeedForBeat((int)userOption.GetNoteSpeed);
				guideSpeed4BeatTouch = GameManager.GetTouchSpeedForBeat((int)userOption.GetTouchSpeed);
				apperMsecTap = guideSpeed4BeatTap * 8f;
				apperMsecTouch = guideSpeed4BeatTouch * 5f;
			}
			else
			{
				isActive = false;
				_guideEndPointObj.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f);
			}
			_movieMaskSprite = _movieMaskObj.GetComponent<SpriteRenderer>();
			_movieMaskSprite.color = new Color(0f, 0f, 0f, 1f);
			if (monIndex == 0 && !GameManager.IsNoteCheckMode)
			{
				Object.Instantiate(_TexturePreLoader, base.transform);
			}
			NoteJudge.Initialize();
		}

		public bool IsReady()
		{
			preLoadedFrame++;
			if (preLoadedFrame > 10)
			{
				_tapCEffectObj.Stop();
				foreach (TouchEffect touchEffectObject in _touchEffectObjectList)
				{
					touchEffectObject.StopAll();
				}
				foreach (TouchEffect touchEffectBObject in _touchEffectBObjectList)
				{
					touchEffectBObject.StopAll();
				}
				foreach (TouchEffect touchEffectEObject in _touchEffectEObjectList)
				{
					touchEffectEObject.StopAll();
				}
				foreach (TouchEffect touchEffectCObject in _touchEffectCObjectList)
				{
					touchEffectCObject.StopAll();
				}
				foreach (JudgeGrade judgeGradeObject in _judgeGradeObjectList)
				{
					judgeGradeObject.PreLoadStop();
				}
				foreach (JudgeTouchGrade judgeGradeTouchBObject in _judgeGradeTouchBObjectList)
				{
					judgeGradeTouchBObject.PreLoadStop();
				}
				foreach (JudgeTouchGrade judgeGradeTouchEObject in _judgeGradeTouchEObjectList)
				{
					judgeGradeTouchEObject.PreLoadStop();
				}
				foreach (JudgeTouchGrade judgeGradeTouchCObject in _judgeGradeTouchCObjectList)
				{
					judgeGradeTouchCObject.PreLoadStop();
				}
				foreach (SlideJudge judgeSlideObject in _judgeSlideObjectList)
				{
					judgeSlideObject.PreLoadStop();
				}
				return true;
			}
			return false;
		}

		public void SetMovieSize(uint height, uint width)
		{
			_movieSprite.size = new Vector2(width, height);
		}

		public void UpdateMovie()
		{
			float currentMsec = NotesManager.GetCurrentMsec();
			if ((double)currentMsec <= 500.0)
			{
				_movieMaskSprite.color = new Color(0f, 0f, 0f, Mathf.Lerp(1f, _movieMaskAlpha, currentMsec / 500f));
			}
			else
			{
				_movieMaskSprite.color = new Color(0f, 0f, 0f, _movieMaskAlpha);
			}
		}

		public bool UpdateCtrl()
		{
			bool result = false;
			if (!_NoteRoot.activeSelf)
			{
				return result;
			}
			UserOption userOption = Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption;
			if (!Singleton<GamePlayManager>.Instance.GetGameScore(monitorIndex).IsTrackSkip)
			{
				float num = 0f;
				foreach (NoteData note in NoteMng.getReader().GetNoteList())
				{
					if (note == null)
					{
						continue;
					}
					if (note.type.getEnum() != NoteTypeID.Def.Slide)
					{
						if (NotesManager.GetCurrentMsec() - 33f > note.time.msec && !note.playAnsSoundHead)
						{
							Singleton<GameSingleCueCtrl>.Instance.ReserveAnswerSe(MonitorIndex);
							note.playAnsSoundHead = true;
						}
						if ((note.type.getEnum() == NoteTypeID.Def.Hold || note.type.getEnum() == NoteTypeID.Def.ExHold || note.type.getEnum() == NoteTypeID.Def.TouchHold) && NotesManager.GetCurrentMsec() - 33f > note.end.msec && !note.playAnsSoundTail)
						{
							Singleton<GameSingleCueCtrl>.Instance.ReserveAnswerSe(MonitorIndex);
							note.playAnsSoundTail = true;
						}
					}
					if (note.isUsed)
					{
						continue;
					}
					switch (note.type.getEnum())
					{
					case NoteTypeID.Def.Begin:
					case NoteTypeID.Def.Hold:
					case NoteTypeID.Def.Star:
					case NoteTypeID.Def.Break:
					case NoteTypeID.Def.BreakStar:
					case NoteTypeID.Def.ExTap:
					case NoteTypeID.Def.ExHold:
					case NoteTypeID.Def.ExStar:
						num = apperMsecTap;
						break;
					case NoteTypeID.Def.TouchTap:
					case NoteTypeID.Def.TouchHold:
						num = apperMsecTouch;
						break;
					case NoteTypeID.Def.Slide:
						num = apperMsecTap;
						break;
					}
					if (!(NotesManager.GetCurrentMsec() >= note.time.msec - num))
					{
						continue;
					}
					if (_debugSkipNow)
					{
						if (NotesManager.GetCurrentMsec() <= note.end.msec + 150f)
						{
							if (!RegistNote(note))
							{
								break;
							}
							note.isUsed = true;
						}
						else
						{
							SkipRegistNote(note);
							note.isUsed = true;
						}
					}
					else
					{
						if (!RegistNote(note))
						{
							break;
						}
						note.isUsed = true;
					}
				}
				_debugSkipNow = false;
			}
			else
			{
				foreach (NoteData note2 in NoteMng.getReader().GetNoteList())
				{
					if (note2 != null && !note2.isUsed)
					{
						note2.isUsed = SkipRegistNote(note2);
					}
				}
			}
			if (userOption.BarDisp == OptionDispbarlineID.On)
			{
				BarData barNextData = NoteMng.getReader().getBarNextData(BarType.GAMEBAR, _barTime);
				if (barNextData != null)
				{
					float num2 = guideSpeed4BeatTap * 5.33333349f;
					if (NotesManager.GetCurrentMsec() + num2 >= barNextData.time.msec)
					{
						RegistBarGuide(barNextData);
						_barTime = barNextData.time.msec;
					}
				}
			}
			for (int i = 0; i < 8; i++)
			{
				if ((InputManager.InGameButtonDown(MonitorIndex, (InputManager.ButtonSetting)i) || InputManager.InGameTouchPanelAreaDown(MonitorIndex, (InputManager.ButtonSetting)i)) && userOption.TouchEffect != 0)
				{
					_touchEffectObjectList[i].Initialize();
				}
				if (InputManager.InGameTouchPanelArea_B_Down(MonitorIndex, (InputManager.ButtonSetting)i) && userOption.TouchEffect == OptionToucheffectID.On)
				{
					_touchEffectBObjectList[i].Initialize();
				}
				if (InputManager.InGameTouchPanelArea_E_Down(MonitorIndex, (InputManager.ButtonSetting)i) && userOption.TouchEffect == OptionToucheffectID.On)
				{
					_touchEffectEObjectList[i].Initialize();
				}
			}
			if (InputManager.InGameTouchPanelArea_C_Down(MonitorIndex) && userOption.TouchEffect == OptionToucheffectID.On)
			{
				_touchEffectCObjectList[0].Initialize();
			}
			ClickDataList clickSeList = NoteMng.getReader().GetCompositioin()._clickSeList;
			bool flag = false;
			if (NoteMng.IsPlaying())
			{
				foreach (ClickData item in clickSeList)
				{
					if (NotesManager.GetCurrentMsec() >= item.time.msec && !item.Played)
					{
						item.Played = true;
						if (!_debugSkipNow && !flag)
						{
							flag = true;
							SoundManager.PlayGameSE(Cue.SE_GAME_CLICK, MonitorIndex, 1f);
						}
					}
				}
			}
			if (GameManager.IsAutoPlay())
			{
				for (int j = 0; j < 35; j++)
				{
					InputManager.SetUsedThisFrame(monitorIndex, (InputManager.TouchPanelArea)j);
				}
			}
			UpdateNotes();
			if (isActive)
			{
				Singleton<GameSingleCueCtrl>.Instance.PlayJudgeSe(monitorIndex);
			}
			if (!Singleton<GamePlayManager>.Instance.GetGameScore(monitorIndex).IsAllJudged())
			{
				result = true;
			}
			return result;
		}

		public void UpdateNotes()
		{
			NoteDataList noteList = NoteMng.getReader().GetNoteList();
			List<TouchChainList> touchChainList = NoteMng.getReader().GetTouchChainList();
			for (int i = 0; i < _activeNoteList.Count; i++)
			{
				_activeNoteList[i].Execute();
				if (!_activeNoteList[i].gameObject.activeSelf)
				{
					_activeNoteList.RemoveAt(i);
					i--;
				}
			}
			for (int j = 0; j < _activeSlideList.Count; j++)
			{
				_activeSlideList[j].Execute();
				if (!_activeSlideList[j].gameObject.activeSelf)
				{
					_activeSlideList.RemoveAt(j);
					j--;
				}
			}
			foreach (TouchNoteB touchBObject in _touchBObjectList)
			{
				if (!touchBObject.gameObject.activeSelf)
				{
					continue;
				}
				int noteIndex = touchBObject.GetNoteIndex();
				NoteData noteData = noteList[noteIndex];
				if (!noteData.isJudged && noteData.indexTouchGroup != -1)
				{
					TouchChainList touchChainList2 = touchChainList[noteData.indexTouchGroup];
					if (touchChainList2.IsChainJudge())
					{
						touchBObject.SetChainPlayResult(monitorIndex, noteData, touchChainList2.ChainJudge);
					}
				}
			}
			foreach (TouchNoteC touchCTapObject in _touchCTapObjectList)
			{
				if (!touchCTapObject.gameObject.activeSelf)
				{
					continue;
				}
				int noteIndex2 = touchCTapObject.GetNoteIndex();
				NoteData noteData2 = noteList[noteIndex2];
				if (!noteData2.isJudged && noteData2.indexTouchGroup != -1)
				{
					TouchChainList touchChainList3 = touchChainList[noteData2.indexTouchGroup];
					if (touchChainList3.IsChainJudge())
					{
						touchCTapObject.SetChainPlayResult(monitorIndex, noteData2, touchChainList3.ChainJudge);
					}
				}
			}
			foreach (JudgeGrade judgeGradeObject in _judgeGradeObjectList)
			{
				if (judgeGradeObject.gameObject.activeSelf)
				{
					judgeGradeObject.Execute();
				}
			}
			foreach (SlideJudge judgeSlideObject in _judgeSlideObjectList)
			{
				if (judgeSlideObject.gameObject.activeSelf)
				{
					judgeSlideObject.Execute();
				}
			}
			foreach (JudgeTouchGrade judgeGradeTouchBObject in _judgeGradeTouchBObjectList)
			{
				if (judgeGradeTouchBObject.gameObject.activeSelf)
				{
					judgeGradeTouchBObject.Execute();
				}
			}
			foreach (JudgeTouchGrade judgeGradeTouchCObject in _judgeGradeTouchCObjectList)
			{
				if (judgeGradeTouchCObject.gameObject.activeSelf)
				{
					judgeGradeTouchCObject.Execute();
				}
			}
			foreach (JudgeTouchGrade judgeGradeTouchEObject in _judgeGradeTouchEObjectList)
			{
				if (judgeGradeTouchEObject.gameObject.activeSelf)
				{
					judgeGradeTouchEObject.Execute();
				}
			}
			foreach (BarGuide barGuideObject in _barGuideObjectList)
			{
				if (barGuideObject.gameObject.activeSelf)
				{
					barGuideObject.Execute();
				}
			}
			foreach (TouchEffect touchEffectObject in _touchEffectObjectList)
			{
				if (touchEffectObject.gameObject.activeSelf)
				{
					touchEffectObject.Execute();
				}
			}
			foreach (TouchEffect touchEffectBObject in _touchEffectBObjectList)
			{
				if (touchEffectBObject.gameObject.activeSelf)
				{
					touchEffectBObject.Execute();
				}
			}
			foreach (TouchEffect touchEffectCObject in _touchEffectCObjectList)
			{
				if (touchEffectCObject.gameObject.activeSelf)
				{
					touchEffectCObject.Execute();
				}
			}
			foreach (TouchEffect touchEffectEObject in _touchEffectEObjectList)
			{
				if (touchEffectEObject.gameObject.activeSelf)
				{
					touchEffectEObject.Execute();
				}
			}
			foreach (NoteGuide guideObject in _guideObjectList)
			{
				if (!guideObject.gameObject.activeSelf || -1 == guideObject.EachIndex)
				{
					continue;
				}
				foreach (NoteData item in noteList)
				{
					if (item.isJudged && item.indexEach == guideObject.EachIndex)
					{
						guideObject.HideEachGuide();
					}
				}
			}
			if (_tapCEffectObj.gameObject.activeSelf)
			{
				_tapCEffectObj.Execute();
			}
			if (null != _trackSkipObject)
			{
				_trackSkipObject.Execute();
			}
		}

		public void ForceAchivement(int achivement, int dxscore)
		{
			ForceNoteCollect();
			Singleton<GamePlayManager>.Instance.GetGameScore(monitorIndex).SetForceAchivement(achivement, dxscore);
		}

		public void ForceAchivementLowAP()
		{
			ForceNoteCollect();
			Singleton<GamePlayManager>.Instance.GetGameScore(monitorIndex).SetForceAchivementLowAP();
		}

		public void CreateNoteLuncher()
		{
			_launcherObjectList = new List<GameObject>(8);
			_touchBLauncherObjectList = new List<GameObject>(8);
			_touchELauncherObjectList = new List<GameObject>(8);
			_touchCLauncherObjectList = new List<GameObject>(1);
			_slideLauncherObjectList = new List<GameObject>(1);
			_touchEffectObjectList = new List<TouchEffect>(8);
			_touchEffectBObjectList = new List<TouchEffect>(8);
			_touchEffectCObjectList = new List<TouchEffect>(1);
			_touchEffectEObjectList = new List<TouchEffect>(8);
			_judgeGradeObjectList = new List<JudgeGrade>(8);
			_touchReserveBObjectList = new List<TouchReserve>(8);
			_judgeGradeTouchBObjectList = new List<JudgeTouchGrade>(8);
			_judgeGradeTouchEObjectList = new List<JudgeTouchGrade>(8);
			_touchReserveEObjectList = new List<TouchReserve>(8);
			_judgeGradeTouchCObjectList = new List<JudgeTouchGrade>(1);
			_touchReserveCObjectList = new List<TouchReserve>(1);
			_judgeSlideObjectList = new List<SlideJudge>(16);
			UserOption userOption = Singleton<GamePlayManager>.Instance.GetGameScore(monitorIndex).UserOption;
			if (GameManager.IsRhythmTestMusicID(monitorIndex))
			{
				userOption.DispJudge = OptionDispjudgeID.Type2A;
			}
			for (int i = 0; i < 8; i++)
			{
				float z = -22.5f + -45f * (float)i;
				float z2 = -45f * (float)i;
				GameObject gameObject = Object.Instantiate(GameNotePrefabContainer.NoteLauncher, _noteLauncherLayer.transform);
				gameObject.transform.rotation = Quaternion.Euler(0f, 0f, z);
				if (GameMode == GameMonitor.GameModeEnum.Tutorial)
				{
					gameObject.gameObject.transform.Find("NoteStart").gameObject.SetActive(value: true);
				}
				_launcherObjectList.Add(gameObject);
				GameObject gameObject2 = Object.Instantiate(GameNotePrefabContainer.TouchBLauncher, _touchBLauncherLayer.transform);
				gameObject2.transform.rotation = Quaternion.Euler(0f, 0f, z);
				_touchBLauncherObjectList.Add(gameObject2);
				GameObject gameObject3 = Object.Instantiate(GameNotePrefabContainer.TouchELauncher, _touchELauncherLayer.transform);
				gameObject3.transform.rotation = Quaternion.Euler(0f, 0f, z2);
				_touchELauncherObjectList.Add(gameObject3);
				GameObject gameObject4 = gameObject.gameObject.transform.Find("NoteEnd").gameObject;
				TouchEffect touchEffect = Object.Instantiate(GameNotePrefabContainer.TouchEffect, gameObject4.transform);
				touchEffect.SetUpParticle(monitorIndex);
				_touchEffectObjectList.Add(touchEffect);
				JudgeGrade judgeGrade = Object.Instantiate(GameNotePrefabContainer.JudgeGrade, gameObject4.transform);
				judgeGrade.gameObject.SetActive(value: false);
				judgeGrade.SetLedSetting(i, monitorIndex);
				judgeGrade.SetOption(userOption.DispJudge, (int)userOption.DispJudgePos);
				_judgeGradeObjectList.Add(judgeGrade);
			}
			_tapCEffectObj = _tapCEffectObject.GetComponent<TapCEffect>();
			_tapCEffectObj.SetUpParticle(monitorIndex);
			for (int j = 0; j < 8; j++)
			{
				float z3 = -67.5f + -45f * (float)j;
				GameObject gameObject5 = _touchBLauncherObjectList[j].gameObject.transform.Find("NoteEnd").gameObject;
				TouchEffect touchEffect2 = Object.Instantiate(GameNotePrefabContainer.TouchEffect, gameObject5.transform);
				touchEffect2.SetUpParticle(monitorIndex);
				_touchEffectBObjectList.Add(touchEffect2);
				JudgeTouchGrade judgeTouchGrade = Object.Instantiate(GameNotePrefabContainer.TouchJudgeGrade, gameObject5.transform);
				judgeTouchGrade.gameObject.SetActive(value: false);
				judgeTouchGrade.SetOption(userOption.DispJudge, (int)userOption.DispJudgeTouchPos);
				_judgeGradeTouchBObjectList.Add(judgeTouchGrade);
				TouchReserve touchReserve = Object.Instantiate(GameNotePrefabContainer.TouchReserve, gameObject5.transform);
				touchReserve.transform.localRotation = Quaternion.Euler(0f, 0f, z3);
				touchReserve.Initialize(monitorIndex);
				_touchReserveBObjectList.Add(touchReserve);
			}
			for (int k = 0; k < 8; k++)
			{
				float z4 = -90f + -45f * (float)k;
				GameObject gameObject6 = _touchELauncherObjectList[k].gameObject.transform.Find("NoteEnd").gameObject;
				TouchEffect touchEffect3 = Object.Instantiate(GameNotePrefabContainer.TouchEffect, gameObject6.transform);
				touchEffect3.SetUpParticle(monitorIndex);
				_touchEffectEObjectList.Add(touchEffect3);
				JudgeTouchGrade judgeTouchGrade2 = Object.Instantiate(GameNotePrefabContainer.TouchJudgeGrade, gameObject6.transform);
				judgeTouchGrade2.gameObject.SetActive(value: false);
				judgeTouchGrade2.SetOption(userOption.DispJudge, (int)userOption.DispJudgeTouchPos);
				_judgeGradeTouchEObjectList.Add(judgeTouchGrade2);
				TouchReserve touchReserve2 = Object.Instantiate(GameNotePrefabContainer.TouchReserve, gameObject6.transform);
				touchReserve2.transform.localRotation = Quaternion.Euler(0f, 0f, z4);
				touchReserve2.Initialize(monitorIndex);
				_touchReserveEObjectList.Add(touchReserve2);
			}
			for (int l = 0; l < 1; l++)
			{
				GameObject gameObject7 = Object.Instantiate(GameNotePrefabContainer.TouchCLauncher, _touchCLauncherLayer.transform);
				GameObject gameObject8 = gameObject7.gameObject.transform.Find("NoteEnd").gameObject;
				_touchCLauncherObjectList.Add(gameObject7);
				TouchEffect touchEffect4 = Object.Instantiate(GameNotePrefabContainer.TouchEffect, gameObject8.transform);
				touchEffect4.SetUpParticle(monitorIndex);
				_touchEffectCObjectList.Add(touchEffect4);
				JudgeTouchGrade judgeTouchGrade3 = Object.Instantiate(GameNotePrefabContainer.TouchJudgeGrade, gameObject8.transform);
				judgeTouchGrade3.gameObject.SetActive(value: false);
				judgeTouchGrade3.SetOption(userOption.DispJudge, (int)userOption.DispJudgeTouchPos);
				_judgeGradeTouchCObjectList.Add(judgeTouchGrade3);
				TouchReserve touchReserve3 = Object.Instantiate(GameNotePrefabContainer.TouchReserve, gameObject8.transform);
				touchReserve3.Initialize(monitorIndex);
				_touchReserveCObjectList.Add(touchReserve3);
			}
			for (int m = 0; m < 1; m++)
			{
				GameObject item2 = Object.Instantiate(GameNotePrefabContainer.SlideLauncher, _slideLaneLayer.transform);
				_slideLauncherObjectList.Add(item2);
			}
			if (null != _trackSkipLaneLayer)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorIndex);
				int difficulty = Singleton<GamePlayManager>.Instance.GetGameScore(monitorIndex).SessionInfo.difficulty;
				int musicID = Singleton<GamePlayManager>.Instance.GetGameScore(monitorIndex).SessionInfo.musicId;
				int myBest = 0;
				UserScore userScore = userData.ScoreList[difficulty].Find((UserScore item) => item.id == musicID);
				if (userScore != null)
				{
					myBest = (int)userScore.achivement;
				}
				_trackSkipObject = Object.Instantiate(GameNotePrefabContainer.TrackSkip, _trackSkipLaneLayer.transform);
				_trackSkipObject.Initialize(monitorIndex, myBest);
			}
			_noteStartPos = _launcherObjectList[0].transform.Find("NoteStart").transform.localPosition.y;
			_noteEndPos = _launcherObjectList[0].transform.Find("NoteEnd").transform.localPosition.y;
			TouchStartPosY[1] = _touchBLauncherObjectList[0].transform.Find("NoteStart").transform.localPosition.y;
			TouchStartPosY[2] = 0f;
			TouchStartPosY[3] = _touchELauncherObjectList[0].transform.Find("NoteStart").transform.localPosition.y;
			_tapObjectList = new List<TapNote>(64);
			_holdObjectList = new List<HoldNote>(64);
			_starObjectList = new List<StarNote>(64);
			_breakObjectList = new List<BreakNote>(64);
			_breakStarObjectList = new List<BreakStarNote>(64);
			_barGuideObjectList = new List<BarGuide>(12);
			_touchBObjectList = new List<TouchNoteB>(32);
			_touchCTapObjectList = new List<TouchNoteC>(8);
			_touchCHoldObjectList = new List<TouchHoldC>(8);
			_slideObjectList = new List<SlideRoot>(24);
			_guideObjectList = new List<NoteGuide>(128);
			_fanSlideObjectList = new List<SlideFan>(4);
		}

		public void CreateNotePool()
		{
			_tapListParent = new GameObject("TapPool");
			_tapListParent.transform.parent = _NotePool.transform;
			_holdListParent = new GameObject("HoldPool");
			_holdListParent.transform.parent = _NotePool.transform;
			_starListParent = new GameObject("StarPool");
			_starListParent.transform.parent = _NotePool.transform;
			_breakStarListParent = new GameObject("BreakStarPool");
			_breakStarListParent.transform.parent = _NotePool.transform;
			_breakListParent = new GameObject("BreakPool");
			_breakListParent.transform.parent = _NotePool.transform;
			_touchListParent = new GameObject("TouchPool");
			_touchListParent.transform.parent = _NotePool.transform;
			_touchCTapListParent = new GameObject("TouchCPool");
			_touchCTapListParent.transform.parent = _NotePool.transform;
			_touchCHoldListParent = new GameObject("TouchHoldPool");
			_touchCHoldListParent.transform.parent = _NotePool.transform;
			_barGuideListParent = new GameObject("BarGuidePool");
			_barGuideListParent.transform.parent = _NotePool.transform;
			_slideListParent = new GameObject("SlidePool");
			_slideListParent.transform.parent = _NotePool.transform;
			_fanSlideListParent = new GameObject("SlideFanPool");
			_fanSlideListParent.transform.parent = _NotePool.transform;
			_guideListParent = new GameObject("NoteGuidePool");
			_guideListParent.transform.parent = _NotePool.transform;
			_slideJudgeListParent = new GameObject("SlideJudgePool");
			_slideJudgeListParent.transform.parent = _NotePool.transform;
			for (int i = 0; i < 64; i++)
			{
				TapNote tapNote = Object.Instantiate(GameNotePrefabContainer.Tap, _tapListParent.transform);
				tapNote.gameObject.SetActive(value: false);
				tapNote.ParentTransform = _tapListParent.transform;
				_tapObjectList.Add(tapNote);
			}
			for (int j = 0; j < 64; j++)
			{
				HoldNote holdNote = Object.Instantiate(GameNotePrefabContainer.Hold, _holdListParent.transform);
				holdNote.gameObject.SetActive(value: false);
				holdNote.ParentTransform = _holdListParent.transform;
				_holdObjectList.Add(holdNote);
			}
			for (int k = 0; k < 64; k++)
			{
				StarNote starNote = Object.Instantiate(GameNotePrefabContainer.Star, _starListParent.transform);
				starNote.gameObject.SetActive(value: false);
				starNote.ParentTransform = _starListParent.transform;
				_starObjectList.Add(starNote);
			}
			for (int l = 0; l < 64; l++)
			{
				BreakStarNote breakStarNote = Object.Instantiate(GameNotePrefabContainer.BreakStar, _breakStarListParent.transform);
				breakStarNote.gameObject.SetActive(value: false);
				breakStarNote.ParentTransform = _breakStarListParent.transform;
				_breakStarObjectList.Add(breakStarNote);
			}
			for (int m = 0; m < 64; m++)
			{
				BreakNote breakNote = Object.Instantiate(GameNotePrefabContainer.Break, _breakListParent.transform);
				breakNote.gameObject.SetActive(value: false);
				breakNote.ParentTransform = _breakListParent.transform;
				_breakObjectList.Add(breakNote);
			}
			for (int n = 0; n < 32; n++)
			{
				TouchNoteB touchNoteB = Object.Instantiate(GameNotePrefabContainer.TouchTapB, _touchListParent.transform);
				touchNoteB.gameObject.SetActive(value: false);
				touchNoteB.ParentTransform = _touchListParent.transform;
				_touchBObjectList.Add(touchNoteB);
			}
			for (int num = 0; num < 8; num++)
			{
				TouchNoteC touchNoteC = Object.Instantiate(GameNotePrefabContainer.TouchTapC, _touchCTapListParent.transform);
				touchNoteC.gameObject.SetActive(value: false);
				touchNoteC.ParentTransform = _touchCTapListParent.transform;
				_touchCTapObjectList.Add(touchNoteC);
			}
			for (int num2 = 0; num2 < 8; num2++)
			{
				TouchHoldC touchHoldC = Object.Instantiate(GameNotePrefabContainer.TouchHoldC, _touchCHoldListParent.transform);
				touchHoldC.gameObject.SetActive(value: false);
				touchHoldC.ParentTransform = _touchCHoldListParent.transform;
				_touchCHoldObjectList.Add(touchHoldC);
			}
			for (int num3 = 0; num3 < 24; num3++)
			{
				SlideRoot slideRoot = Object.Instantiate(GameNotePrefabContainer.Slide, _slideListParent.transform);
				slideRoot.gameObject.SetActive(value: false);
				slideRoot.ParentTransform = _slideListParent.transform;
				_slideObjectList.Add(slideRoot);
			}
			for (int num4 = 0; num4 < 4; num4++)
			{
				SlideFan slideFan = Object.Instantiate(GameNotePrefabContainer.SlideFan, _fanSlideListParent.transform);
				slideFan.gameObject.SetActive(value: false);
				slideFan.ParentTransform = _fanSlideListParent.transform;
				_fanSlideObjectList.Add(slideFan);
			}
			for (int num5 = 0; num5 < 16; num5++)
			{
				SlideJudge slideJudge = Object.Instantiate(GameNotePrefabContainer.SlideJudge, _slideJudgeListParent.transform);
				slideJudge.gameObject.SetActive(value: false);
				slideJudge.ParentTransform = _slideJudgeListParent.transform;
				slideJudge.SetOption(Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.DispJudge);
				_judgeSlideObjectList.Add(slideJudge);
			}
			for (int num6 = 0; num6 < 128; num6++)
			{
				NoteGuide noteGuide = Object.Instantiate(GameNotePrefabContainer.Guide, _guideListParent.transform);
				noteGuide.gameObject.SetActive(value: false);
				noteGuide.ParentTransform = _guideListParent.transform;
				_guideObjectList.Add(noteGuide);
			}
			for (int num7 = 0; num7 < 12; num7++)
			{
				BarGuide barGuide = Object.Instantiate(GameNotePrefabContainer.BarGuide, _barGuideListParent.transform);
				barGuide.gameObject.SetActive(value: false);
				barGuide.ParentTransform = _barGuideListParent.transform;
				_barGuideObjectList.Add(barGuide);
			}
		}

		public bool RegistBarGuide(BarData barData)
		{
			float msec = barData.time.msec;
			foreach (BarGuide barGuideObject in _barGuideObjectList)
			{
				if (!barGuideObject.gameObject.activeSelf)
				{
					barGuideObject.transform.SetParent(_barGuideLayer.transform, worldPositionStays: false);
					barGuideObject.transform.SetAsFirstSibling();
					barGuideObject.gameObject.SetActive(value: true);
					barGuideObject.MonitorId = MonitorIndex;
					barGuideObject.Initialize(msec);
					return true;
				}
			}
			return false;
		}

		public bool SkipRegistNote(NoteData note)
		{
			float _fMsec = NotesManager.GetCurrentMsec() - note.end.msec;
			if (_fMsec < 0f)
			{
				return false;
			}
			NoteJudge.ETiming eTiming = NoteJudge.ETiming.TooLate;
			if (note.type == NoteTypeID.Def.Begin)
			{
				if (NoteJudge.GetJudgeTiming(ref _fMsec, Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.GetJudgeTimingFrame(), NoteJudge.EJudgeType.Tap) == eTiming)
				{
					_tapObjectList[0].SetForcePlayResult(MonitorIndex, note, eTiming);
					return true;
				}
			}
			else if (note.type == NoteTypeID.Def.ExTap)
			{
				if (NoteJudge.GetJudgeTiming(ref _fMsec, Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.GetJudgeTimingFrame(), NoteJudge.EJudgeType.ExTap) == eTiming)
				{
					_tapObjectList[0].SetForcePlayResult(MonitorIndex, note, eTiming);
					return true;
				}
			}
			else if (note.type == NoteTypeID.Def.Hold || note.type == NoteTypeID.Def.ExHold)
			{
				if (NoteJudge.GetJudgeTiming(ref _fMsec, Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.GetJudgeTimingFrame(), NoteJudge.EJudgeType.HoldOut) == eTiming)
				{
					_holdObjectList[0].SetForcePlayResult(MonitorIndex, note, eTiming);
					return true;
				}
			}
			else if (note.type == NoteTypeID.Def.Star)
			{
				if (NoteJudge.GetJudgeTiming(ref _fMsec, Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.GetJudgeTimingFrame(), NoteJudge.EJudgeType.Tap) == eTiming)
				{
					_starObjectList[0].SetForcePlayResult(MonitorIndex, note, eTiming);
					return true;
				}
			}
			else if (note.type == NoteTypeID.Def.ExStar)
			{
				if (NoteJudge.GetJudgeTiming(ref _fMsec, Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.GetJudgeTimingFrame(), NoteJudge.EJudgeType.ExTap) == eTiming)
				{
					_starObjectList[0].SetForcePlayResult(MonitorIndex, note, eTiming);
					return true;
				}
			}
			else if (note.type == NoteTypeID.Def.BreakStar)
			{
				if (NoteJudge.GetJudgeTiming(ref _fMsec, Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.GetJudgeTimingFrame(), NoteJudge.EJudgeType.Break) == eTiming)
				{
					_breakStarObjectList[0].SetForcePlayResult(MonitorIndex, note, eTiming);
					return true;
				}
			}
			else if (note.type == NoteTypeID.Def.Break)
			{
				if (NoteJudge.GetJudgeTiming(ref _fMsec, Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.GetJudgeTimingFrame(), NoteJudge.EJudgeType.Break) == eTiming)
				{
					_breakObjectList[0].SetForcePlayResult(MonitorIndex, note, eTiming);
					return true;
				}
			}
			else if (note.type == NoteTypeID.Def.Slide)
			{
				if (NoteJudge.GetJudgeTiming(ref _fMsec, Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.GetJudgeTimingFrame(), NoteJudge.EJudgeType.SlideOut) == eTiming)
				{
					if (note.slideData.type == SlideType.Slide_Fan)
					{
						_fanSlideObjectList[0].SetForcePlayResult(MonitorIndex, note, eTiming);
					}
					else
					{
						_slideObjectList[0].SetForcePlayResult(MonitorIndex, note, eTiming);
					}
					return true;
				}
			}
			else if (note.type == NoteTypeID.Def.TouchTap)
			{
				if (NoteJudge.GetJudgeTiming(ref _fMsec, Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.GetJudgeTimingFrame(), NoteJudge.EJudgeType.Touch) == eTiming)
				{
					if (note.startButtonPos <= 7)
					{
						_touchBObjectList[0].SetForcePlayResult(MonitorIndex, note, eTiming);
					}
					else
					{
						_touchCTapObjectList[0].SetForcePlayResult(MonitorIndex, note, eTiming);
					}
					return true;
				}
			}
			else if (note.type == NoteTypeID.Def.TouchHold && NoteJudge.GetJudgeTiming(ref _fMsec, Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.GetJudgeTimingFrame(), NoteJudge.EJudgeType.HoldOut) == eTiming)
			{
				_touchCHoldObjectList[0].SetForcePlayResult(MonitorIndex, note, eTiming);
				return true;
			}
			return false;
		}

		public bool RegistNote(NoteData note)
		{
			int startButtonPos = note.startButtonPos;
			if (note.type == NoteTypeID.Def.Begin || note.type == NoteTypeID.Def.ExTap)
			{
				foreach (TapNote tapObject in _tapObjectList)
				{
					if (tapObject.gameObject.activeSelf)
					{
						continue;
					}
					tapObject.transform.SetParent(_launcherObjectList[startButtonPos].transform, worldPositionStays: false);
					tapObject.transform.SetAsFirstSibling();
					tapObject.gameObject.SetActive(value: true);
					tapObject.MonitorId = MonitorIndex;
					foreach (NoteGuide guideObject in _guideObjectList)
					{
						if (!guideObject.gameObject.activeSelf)
						{
							tapObject.SetGuideObject(guideObject);
							break;
						}
					}
					tapObject.ExNote = note.type == NoteTypeID.Def.ExTap;
					tapObject.Initialize(note);
					tapObject.SetJudgeObject(_judgeGradeObjectList[startButtonPos], _touchEffectObjectList[startButtonPos]);
					_activeNoteList.Add(tapObject);
					return true;
				}
			}
			else if (note.type == NoteTypeID.Def.Hold || note.type == NoteTypeID.Def.ExHold)
			{
				foreach (HoldNote holdObject in _holdObjectList)
				{
					if (holdObject.gameObject.activeSelf)
					{
						continue;
					}
					holdObject.transform.SetParent(_launcherObjectList[startButtonPos].transform, worldPositionStays: false);
					holdObject.transform.SetAsFirstSibling();
					holdObject.gameObject.SetActive(value: true);
					holdObject.MonitorId = MonitorIndex;
					foreach (NoteGuide guideObject2 in _guideObjectList)
					{
						if (!guideObject2.gameObject.activeSelf)
						{
							holdObject.SetGuideObject(guideObject2);
							break;
						}
					}
					holdObject.ExNote = note.type == NoteTypeID.Def.ExHold;
					holdObject.Initialize(note);
					holdObject.SetJudgeObject(_judgeGradeObjectList[startButtonPos], _touchEffectObjectList[startButtonPos]);
					_activeNoteList.Add(holdObject);
					return true;
				}
			}
			else if (note.type == NoteTypeID.Def.Star || note.type == NoteTypeID.Def.ExStar)
			{
				foreach (StarNote starObject in _starObjectList)
				{
					if (starObject.gameObject.activeSelf)
					{
						continue;
					}
					starObject.transform.SetParent(_launcherObjectList[startButtonPos].transform, worldPositionStays: false);
					starObject.transform.SetAsFirstSibling();
					starObject.gameObject.SetActive(value: true);
					starObject.MonitorId = MonitorIndex;
					foreach (NoteGuide guideObject3 in _guideObjectList)
					{
						if (!guideObject3.gameObject.activeSelf)
						{
							starObject.SetGuideObject(guideObject3);
							break;
						}
					}
					starObject.ExNote = note.type == NoteTypeID.Def.ExStar;
					starObject.Initialize(note);
					starObject.SetJudgeObject(_judgeGradeObjectList[startButtonPos], _touchEffectObjectList[startButtonPos]);
					_activeNoteList.Add(starObject);
					return true;
				}
			}
			else if (note.type == NoteTypeID.Def.BreakStar)
			{
				foreach (BreakStarNote breakStarObject in _breakStarObjectList)
				{
					if (breakStarObject.gameObject.activeSelf)
					{
						continue;
					}
					breakStarObject.transform.SetParent(_launcherObjectList[startButtonPos].transform, worldPositionStays: false);
					breakStarObject.transform.SetAsFirstSibling();
					breakStarObject.gameObject.SetActive(value: true);
					breakStarObject.MonitorId = MonitorIndex;
					foreach (NoteGuide guideObject4 in _guideObjectList)
					{
						if (!guideObject4.gameObject.activeSelf)
						{
							breakStarObject.SetGuideObject(guideObject4);
							break;
						}
					}
					breakStarObject.ExNote = false;
					breakStarObject.Initialize(note);
					breakStarObject.SetJudgeObject(_judgeGradeObjectList[startButtonPos], _touchEffectObjectList[startButtonPos]);
					_activeNoteList.Add(breakStarObject);
					return true;
				}
			}
			else if (note.type == NoteTypeID.Def.Break)
			{
				foreach (BreakNote breakObject in _breakObjectList)
				{
					if (breakObject.gameObject.activeSelf)
					{
						continue;
					}
					breakObject.transform.SetParent(_launcherObjectList[startButtonPos].transform, worldPositionStays: false);
					breakObject.transform.SetAsFirstSibling();
					breakObject.gameObject.SetActive(value: true);
					breakObject.MonitorId = MonitorIndex;
					foreach (NoteGuide guideObject5 in _guideObjectList)
					{
						if (!guideObject5.gameObject.activeSelf)
						{
							breakObject.SetGuideObject(guideObject5);
							break;
						}
					}
					breakObject.ExNote = false;
					breakObject.Initialize(note);
					breakObject.SetJudgeObject(_judgeGradeObjectList[startButtonPos], _touchEffectObjectList[startButtonPos]);
					_activeNoteList.Add(breakObject);
					return true;
				}
			}
			else if (note.type == NoteTypeID.Def.Slide)
			{
				if (note.slideData.type == SlideType.Slide_Fan)
				{
					foreach (SlideFan fanSlideObject in _fanSlideObjectList)
					{
						if (!fanSlideObject.gameObject.activeSelf)
						{
							fanSlideObject.transform.SetParent(_slideLauncherObjectList[0].transform, worldPositionStays: false);
							fanSlideObject.transform.SetAsFirstSibling();
							fanSlideObject.gameObject.SetActive(value: true);
							fanSlideObject.MonitorId = MonitorIndex;
							fanSlideObject.ButtonId = note.startButtonPos;
							_judgeSlideObjectList[_slideJudgeIndex].transform.SetParent(_slideLauncherObjectList[0].transform, worldPositionStays: false);
							fanSlideObject.SetJudgeObject(_judgeSlideObjectList[_slideJudgeIndex]);
							_slideJudgeIndex = (_slideJudgeIndex + 1) % _judgeSlideObjectList.Count;
							fanSlideObject.Initialize(note);
							_activeSlideList.Add(fanSlideObject);
							return true;
						}
					}
				}
				else
				{
					foreach (SlideRoot slideObject in _slideObjectList)
					{
						if (!slideObject.gameObject.activeSelf)
						{
							slideObject.transform.SetParent(_slideLauncherObjectList[0].transform, worldPositionStays: false);
							slideObject.transform.SetAsFirstSibling();
							slideObject.gameObject.SetActive(value: true);
							slideObject.MonitorId = MonitorIndex;
							slideObject.ButtonId = note.startButtonPos;
							_judgeSlideObjectList[_slideJudgeIndex].transform.SetParent(_slideLauncherObjectList[0].transform, worldPositionStays: false);
							slideObject.SetJudgeObject(_judgeSlideObjectList[_slideJudgeIndex]);
							_slideJudgeIndex = (_slideJudgeIndex + 1) % _judgeSlideObjectList.Count;
							slideObject.Initialize(note);
							_activeSlideList.Add(slideObject);
							return true;
						}
					}
				}
			}
			else if (note.type == NoteTypeID.Def.TouchTap)
			{
				if (note.touchArea == NoteTypeID.TouchArea.B)
				{
					foreach (TouchNoteB touchBObject in _touchBObjectList)
					{
						if (!touchBObject.gameObject.activeSelf)
						{
							touchBObject.transform.SetParent(_touchBLauncherObjectList[startButtonPos].transform, worldPositionStays: false);
							touchBObject.transform.SetAsFirstSibling();
							touchBObject.gameObject.SetActive(value: true);
							touchBObject.MonitorId = MonitorIndex;
							touchBObject.ExNote = false;
							touchBObject.Initialize(note);
							touchBObject.SetJudgeObject(_judgeGradeTouchBObjectList[startButtonPos], _touchEffectBObjectList[startButtonPos]);
							touchBObject.SetReserveObject(_touchReserveBObjectList[startButtonPos]);
							_touchReserveBObjectList[startButtonPos].gameObject.SetActive(value: true);
							GetTouchReserve(note, ref _targetIndexs);
							_touchReserveBObjectList[startButtonPos].SetReserveCount(_targetIndexs);
							_activeNoteList.Add(touchBObject);
							return true;
						}
					}
				}
				else if (note.touchArea == NoteTypeID.TouchArea.E)
				{
					foreach (TouchNoteB touchBObject2 in _touchBObjectList)
					{
						if (!touchBObject2.gameObject.activeSelf)
						{
							touchBObject2.transform.SetParent(_touchELauncherObjectList[startButtonPos].transform, worldPositionStays: false);
							touchBObject2.transform.SetAsFirstSibling();
							touchBObject2.gameObject.SetActive(value: true);
							touchBObject2.MonitorId = MonitorIndex;
							touchBObject2.ExNote = false;
							touchBObject2.Initialize(note);
							touchBObject2.SetJudgeObject(_judgeGradeTouchEObjectList[startButtonPos], _touchEffectEObjectList[startButtonPos]);
							touchBObject2.SetReserveObject(_touchReserveEObjectList[startButtonPos]);
							_touchReserveEObjectList[startButtonPos].gameObject.SetActive(value: true);
							GetTouchReserve(note, ref _targetIndexs);
							_touchReserveEObjectList[startButtonPos].SetReserveCount(_targetIndexs);
							_activeNoteList.Add(touchBObject2);
							return true;
						}
					}
				}
				else if (note.touchArea == NoteTypeID.TouchArea.C)
				{
					foreach (TouchNoteC touchCTapObject in _touchCTapObjectList)
					{
						if (!touchCTapObject.gameObject.activeSelf)
						{
							touchCTapObject.transform.SetParent(_touchCLauncherObjectList[0].transform, worldPositionStays: false);
							touchCTapObject.transform.SetAsFirstSibling();
							touchCTapObject.gameObject.SetActive(value: true);
							touchCTapObject.MonitorId = MonitorIndex;
							touchCTapObject.ExNote = false;
							touchCTapObject.Initialize(note);
							touchCTapObject.SetJudgeObject(_judgeGradeTouchCObjectList[0], _touchEffectCObjectList[0]);
							touchCTapObject.SetEffectObject(_tapCEffectObj);
							touchCTapObject.SetReserveObject(_touchReserveCObjectList[startButtonPos]);
							_touchReserveCObjectList[startButtonPos].gameObject.SetActive(value: true);
							GetTouchReserve(note, ref _targetIndexs);
							_touchReserveCObjectList[startButtonPos].SetReserveCount(_targetIndexs);
							_activeNoteList.Add(touchCTapObject);
							return true;
						}
					}
				}
			}
			else if (note.type == NoteTypeID.Def.TouchHold)
			{
				foreach (TouchHoldC touchCHoldObject in _touchCHoldObjectList)
				{
					if (!touchCHoldObject.gameObject.activeSelf)
					{
						touchCHoldObject.transform.SetParent(_touchCLauncherObjectList[0].transform, worldPositionStays: false);
						touchCHoldObject.transform.SetAsFirstSibling();
						touchCHoldObject.gameObject.SetActive(value: true);
						touchCHoldObject.MonitorId = MonitorIndex;
						touchCHoldObject.ExNote = false;
						touchCHoldObject.Initialize(note);
						touchCHoldObject.SetJudgeObject(_judgeGradeTouchCObjectList[startButtonPos], _touchEffectCObjectList[startButtonPos]);
						touchCHoldObject.SetEffectObject(_tapCEffectObj);
						touchCHoldObject.SetReserveObject(_touchReserveCObjectList[startButtonPos]);
						_touchReserveCObjectList[startButtonPos].gameObject.SetActive(value: true);
						GetTouchReserve(note, ref _targetIndexs);
						_touchReserveCObjectList[startButtonPos].SetReserveCount(_targetIndexs);
						_activeNoteList.Add(touchCHoldObject);
						return true;
					}
				}
			}
			return false;
		}

		public void SetDebugSkipFlame()
		{
			_debugSkipNow = true;
		}

		public void ForceNoteCollect()
		{
			foreach (TapNote tapObject in _tapObjectList)
			{
				tapObject.gameObject.SetActive(value: false);
				tapObject.transform.SetParent(_tapListParent.transform, worldPositionStays: false);
			}
			foreach (HoldNote holdObject in _holdObjectList)
			{
				holdObject.gameObject.SetActive(value: false);
				holdObject.transform.SetParent(_holdListParent.transform, worldPositionStays: false);
			}
			foreach (StarNote starObject in _starObjectList)
			{
				starObject.gameObject.SetActive(value: false);
				starObject.transform.SetParent(_starListParent.transform, worldPositionStays: false);
			}
			foreach (BreakNote breakObject in _breakObjectList)
			{
				breakObject.gameObject.SetActive(value: false);
				breakObject.transform.SetParent(_breakListParent.transform, worldPositionStays: false);
			}
			foreach (BreakStarNote breakStarObject in _breakStarObjectList)
			{
				breakStarObject.gameObject.SetActive(value: false);
				breakStarObject.transform.SetParent(_breakStarListParent.transform, worldPositionStays: false);
			}
			foreach (TouchNoteB touchBObject in _touchBObjectList)
			{
				touchBObject.gameObject.SetActive(value: false);
				touchBObject.transform.SetParent(_touchListParent.transform, worldPositionStays: false);
			}
			foreach (TouchNoteC touchCTapObject in _touchCTapObjectList)
			{
				touchCTapObject.gameObject.SetActive(value: false);
				touchCTapObject.transform.SetParent(_touchCTapListParent.transform, worldPositionStays: false);
			}
			foreach (TouchHoldC touchCHoldObject in _touchCHoldObjectList)
			{
				touchCHoldObject.gameObject.SetActive(value: false);
				touchCHoldObject.transform.SetParent(_touchCHoldListParent.transform, worldPositionStays: false);
			}
			foreach (TouchReserve touchReserveBObject in _touchReserveBObjectList)
			{
				touchReserveBObject.gameObject.SetActive(value: false);
				touchReserveBObject.Initialize(monitorIndex);
			}
			foreach (TouchReserve touchReserveCObject in _touchReserveCObjectList)
			{
				touchReserveCObject.gameObject.SetActive(value: false);
				touchReserveCObject.Initialize(monitorIndex);
			}
			foreach (TouchReserve touchReserveEObject in _touchReserveEObjectList)
			{
				touchReserveEObject.gameObject.SetActive(value: false);
				touchReserveEObject.Initialize(monitorIndex);
			}
			foreach (SlideRoot slideObject in _slideObjectList)
			{
				slideObject.gameObject.SetActive(value: false);
				slideObject.transform.SetParent(_slideListParent.transform, worldPositionStays: false);
			}
			foreach (SlideFan fanSlideObject in _fanSlideObjectList)
			{
				fanSlideObject.gameObject.SetActive(value: false);
				fanSlideObject.transform.SetParent(_fanSlideListParent.transform, worldPositionStays: false);
			}
			foreach (SlideJudge judgeSlideObject in _judgeSlideObjectList)
			{
				judgeSlideObject.gameObject.SetActive(value: false);
				judgeSlideObject.transform.SetParent(_slideJudgeListParent.transform, worldPositionStays: false);
			}
			foreach (BarGuide barGuideObject in _barGuideObjectList)
			{
				barGuideObject.gameObject.SetActive(value: false);
				barGuideObject.transform.SetParent(_barGuideListParent.transform, worldPositionStays: false);
			}
			foreach (NoteGuide guideObject in _guideObjectList)
			{
				guideObject.gameObject.SetActive(value: false);
				guideObject.transform.SetParent(_guideListParent.transform, worldPositionStays: false);
			}
			foreach (TouchEffect touchEffectObject in _touchEffectObjectList)
			{
				touchEffectObject.gameObject.SetActive(value: false);
				touchEffectObject.StopAll();
			}
			foreach (TouchEffect touchEffectBObject in _touchEffectBObjectList)
			{
				touchEffectBObject.gameObject.SetActive(value: false);
				touchEffectBObject.StopAll();
			}
			foreach (TouchEffect touchEffectCObject in _touchEffectCObjectList)
			{
				touchEffectCObject.gameObject.SetActive(value: false);
				touchEffectCObject.StopAll();
			}
			foreach (TouchEffect touchEffectEObject in _touchEffectEObjectList)
			{
				touchEffectEObject.gameObject.SetActive(value: false);
				touchEffectEObject.StopAll();
			}
			foreach (NoteData note in NoteMng.getReader().GetNoteList())
			{
				if (note != null)
				{
					note.isUsed = false;
					note.isJudged = false;
					note.playAnsSoundHead = false;
					note.playAnsSoundTail = false;
				}
			}
			foreach (TouchChainList touchChain in NoteMng.getReader().GetTouchChainList())
			{
				if (touchChain != null)
				{
					touchChain.EndCount = 0;
					touchChain.ChainJudge = NoteJudge.ETiming.TooLate;
				}
			}
			foreach (ClickData clickSe in NoteMng.getReader().GetCompositioin()._clickSeList)
			{
				clickSe.Played = false;
			}
			SoundManager.StopGameSingleSe(MonitorIndex, SoundManager.PlayerID.TouchHoldLoop);
		}

		private void GetTouchReserve(NoteData target, ref List<TouchReserve.ReserveData> touchNoteIndexs)
		{
			touchNoteIndexs.Clear();
			float num = guideSpeed4BeatTouch * 4f;
			foreach (NoteData note in NoteMng.getReader().GetNoteList())
			{
				if (note != null && !note.isUsed && note.touchArea == target.touchArea && note.startButtonPos == target.startButtonPos && target != note)
				{
					if (!(target.time.msec <= note.time.msec) || !(target.time.msec + num >= note.time.msec))
					{
						break;
					}
					touchNoteIndexs.Add(new TouchReserve.ReserveData(note.indexNote, note.isEach));
				}
			}
		}

		public void SetActiveNotesField(bool flg)
		{
			_NoteRoot.SetActive(flg);
		}

		public int GetPushPhase()
		{
			return _trackSkipObject.GetPushPhase();
		}

		public void ResetOptionSpeed()
		{
			guideSpeed4BeatTap = GameManager.GetNoteSpeedForBeat((int)Singleton<GamePlayManager>.Instance.GetGameScore(monitorIndex).UserOption.GetNoteSpeed);
			guideSpeed4BeatTouch = GameManager.GetTouchSpeedForBeat((int)Singleton<GamePlayManager>.Instance.GetGameScore(monitorIndex).UserOption.GetTouchSpeed);
			apperMsecTap = guideSpeed4BeatTap * 8f;
			apperMsecTouch = guideSpeed4BeatTouch * 5f;
		}
	}
}
