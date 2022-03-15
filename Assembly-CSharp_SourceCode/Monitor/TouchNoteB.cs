using MAI2.Util;
using Manager;
using Monitor.Game;
using Process;
using UnityEngine;

namespace Monitor
{
	public class TouchNoteB : NoteBase
	{
		[SerializeField]
		protected SpriteRenderer[] ColorsObject;

		protected Vector3[] DefaultCorlsPos;

		[SerializeField]
		protected GameObject NoticeObject;

		protected SpriteRenderer NoticeSprite;

		protected const float DispNoticeFlame = 3f;

		protected readonly int[] TargetSortDiff = new int[4] { 1, 1, 0, 0 };

		[SerializeField]
		protected float DispAdjustFlame;

		protected float AppearGrid;

		protected NoteTypeID.TouchArea TouchArea;

		protected int AddEffect;

		protected NoteTypeID.TouchSize TouchSize;

		private const int TouchPlateNum = 4;

		protected Material[] NotePlateMaterial = new Material[4];

		protected SpriteRenderer[] NotePlateSprite = new SpriteRenderer[4];

		protected TouchReserve TouchResreveObject;

		public static readonly float[] NotesScale = new float[2] { 1f, 1.3f };

		public const float TouchDispParam = 0.25f;

		private float GetTouchDispTimes()
		{
			return DefaultMsec * 0.25f;
		}

		protected void ReserveCenterEffectJudgeSe(NoteJudge.JudgeBox judge)
		{
			Singleton<GameSingleCueCtrl>.Instance.ReserveCenterEffectSe(MonitorIndex, judge);
		}

		protected void ReserveTouchHoldLoopSe(NoteJudge.JudgeBox judge, bool loopDisable)
		{
			Singleton<GameSingleCueCtrl>.Instance.ReserveTouchHoldLoopSe(MonitorIndex, judge, loopDisable);
		}

		protected override void ReserveTapJudgeSe(NoteJudge.JudgeBox judge)
		{
			Singleton<GameSingleCueCtrl>.Instance.ReserveTouchJudgeSe(MonitorIndex, judge);
		}

		protected override float GetBaseZPosition()
		{
			return -7f + (float)NoteIndex * 0.0001f;
		}

		public void SetReserveObject(TouchReserve touchReserve)
		{
			TouchResreveObject = touchReserve;
		}

		protected override void SetEach(bool eachFlag)
		{
			if (eachFlag)
			{
				for (int i = 0; i < DefaultCorlsPos.Length; i++)
				{
					NotePlateSprite[i].sprite = GameNoteImageContainer.EachTouch;
				}
				SpriteRender.sprite = GameNoteImageContainer.EachTouchPoint;
			}
			else
			{
				for (int j = 0; j < DefaultCorlsPos.Length; j++)
				{
					NotePlateSprite[j].sprite = GameNoteImageContainer.NormalTouch;
				}
				SpriteRender.sprite = GameNoteImageContainer.NormalTouchPoint;
			}
			EachFlag = eachFlag;
		}

		protected override bool IsJudgeNote()
		{
			return base.transform.parent.childCount - 3 == base.transform.GetSiblingIndex();
		}

		protected override void Awake()
		{
			SpriteRender = NoteObj.GetComponent<SpriteRenderer>();
			NoteStat = NoteStatus.Init;
			if (null != NoticeObject)
			{
				NoticeSprite = NoticeObject.GetComponent<SpriteRenderer>();
			}
			DefaultCorlsPos = new Vector3[ColorsObject.Length];
			for (int i = 0; i < DefaultCorlsPos.Length; i++)
			{
				DefaultCorlsPos[i] = ColorsObject[i].transform.localPosition;
				NotePlateSprite[i] = ColorsObject[i].GetComponent<SpriteRenderer>();
				NotePlateMaterial[i] = NotePlateSprite[i].material;
			}
		}

		public override void Initialize(NoteData note)
		{
			base.Initialize(note);
			NoteStat = NoteStatus.Init;
			NoteMng = NotesManager.Instance(base.MonitorId);
			AppearMsec = note.time.msec;
			TailMsec = note.end.msec;
			base.ButtonId = note.startButtonPos;
			NoteIndex = note.indexNote;
			AppearGrid = note.time.grid;
			TouchArea = note.touchArea;
			StartPos = GameCtrl.TouchStartPos((int)TouchArea);
			EndPos = GameCtrl.TouchStartPos((int)TouchArea);
			JudgeResult = NoteJudge.ETiming.End;
			JudgeTimingDiffMsec = 0f;
			EndFlag = false;
			AddEffect = note.effect;
			TouchSize = note.touchSize;
			DefaultMsec = (float)((double)GameManager.GetTouchSpeedForBeat((int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.GetTouchSpeed) * 4.0);
			StartMsec = AppearMsec - (DefaultMsec + GetTouchDispTimes());
			NoteObj.transform.localPosition = new Vector3(0f, StartPos, GetBaseZPosition());
			SpriteRender.transform.localPosition = new Vector3(0f, StartPos, GetBaseZPosition());
			float num = NotesScale[(int)TouchSize];
			NoteObj.transform.eulerAngles = new Vector3(0f, 0f, 0f);
			NoteObj.transform.localScale = new Vector3(num, num, 1f);
			SpriteRender.transform.eulerAngles = new Vector3(0f, 0f, 0f);
			SpriteRender.transform.localScale = new Vector3(num, num, 1f);
			SpriteRender.sortingOrder = -NoteIndex + 3;
			SpriteRender.color = new Color(1f, 1f, 1f, 0f);
			for (int i = 0; i < DefaultCorlsPos.Length; i++)
			{
				ColorsObject[i].transform.localPosition = DefaultCorlsPos[i];
				ColorsObject[i].color = new Color(1f, 1f, 1f, 0f);
				ColorsObject[i].sortingOrder = -NoteIndex + 1 + TargetSortDiff[i];
			}
			ShotJudgeSound = false;
			SetEach(note.isEach);
			JudgeType = NoteJudge.EJudgeType.Touch;
			if (null != NoticeObject)
			{
				NoticeObject.SetActive(value: false);
				NoticeSprite.sortingOrder = -NoteIndex + 4;
			}
		}

		private void Start()
		{
			NoteKind = NoteTypeID.Def.TouchTap;
		}

		public override void Execute()
		{
			if (!EndFlag)
			{
				GetNoteYPosition();
			}
			NoteCheck();
		}

		protected override float GetNoteYPosition()
		{
			float currentMsec = NotesManager.GetCurrentMsec();
			NoteStat = NoteStatus.Move;
			if (StartMsec >= currentMsec)
			{
				NoteStat = NoteStatus.Init;
				SpriteRender.color = new Color(1f, 1f, 1f, 0f);
				for (int i = 0; i < DefaultCorlsPos.Length; i++)
				{
					ColorsObject[i].color = new Color(1f, 1f, 1f, 0f);
				}
				return 0f;
			}
			if (StartMsec + GetTouchDispTimes() >= currentMsec)
			{
				NoteStat = NoteStatus.Scale;
				float num = (currentMsec - StartMsec) / GetTouchDispTimes();
				if (num > 1f)
				{
					num = 1f;
				}
				SpriteRender.color = new Color(1f, 1f, 1f, 1f);
				for (int j = 0; j < DefaultCorlsPos.Length; j++)
				{
					ColorsObject[j].color = new Color(1f, 1f, 1f, num);
				}
				return num;
			}
			NoteStat = NoteStatus.Move;
			float f = (currentMsec - (StartMsec + GetTouchDispTimes()) + DispAdjustFlame * 16.666666f) / DefaultMsec;
			f = 3.5f * Mathf.Pow(f, 4f) - 3.75f * Mathf.Pow(f, 3f) + 1.45f * Mathf.Pow(f, 2f) - 0.05f * Mathf.Pow(f, 1f) + 0.0005f;
			if (f > 1f)
			{
				f = 1f;
			}
			SpriteRender.color = new Color(1f, 1f, 1f, 1f);
			for (int k = 0; k < DefaultCorlsPos.Length; k++)
			{
				ColorsObject[k].transform.localPosition = Vector3.Lerp(DefaultCorlsPos[k], Vector3.zero, f);
				ColorsObject[k].color = new Color(1f, 1f, 1f, 1f);
			}
			if (null != NoticeObject)
			{
				NoticeObject.SetActive(AppearMsec <= currentMsec);
			}
			return 1f;
		}

		protected override void NoteCheck()
		{
			bool flag = false;
			if (TouchArea == NoteTypeID.TouchArea.B)
			{
				flag = !GameManager.IsAutoPlay() && InputManager.InGameTouchPanelArea_B_Down(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId) && !InputManager.IsUsedThisFrame(base.MonitorId, (InputManager.TouchPanelArea)(base.ButtonId + 8));
			}
			else if (TouchArea == NoteTypeID.TouchArea.E)
			{
				flag = !GameManager.IsAutoPlay() && InputManager.InGameTouchPanelArea_E_Down(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId) && !InputManager.IsUsedThisFrame(base.MonitorId, (InputManager.TouchPanelArea)(base.ButtonId + 26));
			}
			else if (TouchArea == NoteTypeID.TouchArea.C)
			{
				flag = !GameManager.IsAutoPlay() && InputManager.InGameTouchPanelArea_C_Down(base.MonitorId) && !InputManager.IsUsedThisFrame(base.MonitorId, (InputManager.TouchPanelArea)(base.ButtonId + 16));
			}
			if (flag && IsJudgeNote())
			{
				Judge();
			}
			if (GetJudgeResult() != NoteJudge.ETiming.End || JudgeToolate())
			{
				EndNote();
			}
			SetAutoPlayJudge();
		}

		public override void SetForcePlayResult(int monitorId, NoteData note, NoteJudge.ETiming timing)
		{
			Singleton<GamePlayManager>.Instance.GetGameScore(monitorId).SetResult(note.indexNote, NoteScore.EScoreType.Touch, timing);
		}

		protected override void EndNote()
		{
			EndFlag = true;
			SpriteRender.color = new Color(1f, 1f, 1f, 0f);
			NoteStat = NoteStatus.End;
			JudgeEffectObject.InitializeCenter(GetJudgeResult());
			JudgeGradeObject.Initialize(GetJudgeResult(), JudgeTimingDiffMsec, NoteJudge.EJudgeType.Touch);
			Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).SetResult(NoteIndex, NoteScore.EScoreType.Touch, GetJudgeResult());
			TouchResreveObject.DeathNote();
			SetPlayResult();
			base.gameObject.SetActive(value: false);
			base.transform.SetParent(base.ParentTransform, worldPositionStays: false);
			if (NeedGuide)
			{
				GuideObj.ReturnToBase();
			}
		}

		protected override bool Judge()
		{
			if (IsNoteCheckTimeStart(Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.GetJudgeTimingFrame()))
			{
				JudgeTimingDiffMsec = NotesManager.GetCurrentMsec() - AppearMsec;
				JudgeResult = NoteJudge.GetJudgeTiming(ref JudgeTimingDiffMsec, Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.GetJudgeTimingFrame(), JudgeType);
				if (JudgeResult != NoteJudge.ETiming.End)
				{
					PlayJudgeSe();
					if (TouchArea == NoteTypeID.TouchArea.B)
					{
						InputManager.SetUsedThisFrame(base.MonitorId, (InputManager.TouchPanelArea)(8 + base.ButtonId));
					}
					else if (TouchArea == NoteTypeID.TouchArea.E)
					{
						InputManager.SetUsedThisFrame(base.MonitorId, (InputManager.TouchPanelArea)(26 + base.ButtonId));
					}
					else if (TouchArea == NoteTypeID.TouchArea.C)
					{
						InputManager.SetUsedThisFrame(base.MonitorId, InputManager.TouchPanelArea.C1);
						InputManager.SetUsedThisFrame(base.MonitorId, InputManager.TouchPanelArea.C2);
					}
					return true;
				}
				return false;
			}
			return false;
		}

		public virtual void SetChainPlayResult(int monitorId, NoteData note, NoteJudge.ETiming timing)
		{
			Judge();
			EndNote();
		}
	}
}
