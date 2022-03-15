using MAI2.Util;
using Manager;
using Process;
using UnityEngine;

namespace Monitor
{
	public class TouchHoldC : TouchNoteC
	{
		[SerializeField]
		protected SpriteRenderer HoldGaugeObject;

		protected double HoldReleaseTime;

		protected double HoldReleaseCorrectTime;

		protected NoteJudge.ETiming JudgeHeadResult;

		private const int PushOldFrameNum = 4;

		private readonly bool[] PushList = new bool[4];

		protected Material GaugeMaterial;

		protected bool TrigetOn;

		protected bool BodyOn;

		protected bool HeadJudged;

		protected const string ShaderAmountStr = "_Amount";

		protected bool LastHoldState;

		protected bool AllJudged;

		public NoteJudge.ETiming GetJudgeHeadResult()
		{
			return JudgeHeadResult;
		}

		protected override bool IsNoteCheckTimeHoldHeadIgnoreJudgeWait()
		{
			return NotesManager.GetCurrentMsec() >= AppearMsec + NoteJudge.JudgeTouchHoldHeadFrame;
		}

		protected override bool IsNoteCheckTimeHoldTailIgnoreJudgeWait()
		{
			return NotesManager.GetCurrentMsec() <= TailMsec - NoteJudge.JudgeTouchHoldTailFrame;
		}

		protected override bool IsJudgeNote()
		{
			return true;
		}

		protected override float GetBaseZPosition()
		{
			return -8f + (float)NoteIndex * 0.0001f;
		}

		protected override void SetEach(bool eachFlag)
		{
			if (eachFlag)
			{
				SpriteRender.sprite = GameNoteImageContainer.EachTouchPoint;
			}
			else
			{
				SpriteRender.sprite = GameNoteImageContainer.NormalTouchPoint;
			}
			EachFlag = eachFlag;
		}

		protected void HoldOn(bool on)
		{
			HoldGaugeObject.sprite = (on ? GameNoteImageContainer.TouchHoldGuide : GameNoteImageContainer.TouchHoldGuideOff);
		}

		protected override void Awake()
		{
			base.Awake();
			GaugeMaterial = HoldGaugeObject.GetComponent<SpriteRenderer>().material;
		}

		public override void Initialize(NoteData note)
		{
			base.Initialize(note);
			HoldGaugeObject.sortingOrder = -(NoteIndex + 1);
			StartPos = 0f;
			EndPos = 0f;
			NoteObj.transform.localPosition = new Vector3(0f, StartPos, GetBaseZPosition());
			JudgeType = NoteJudge.EJudgeType.Touch;
			JudgeHeadResult = NoteJudge.ETiming.End;
			HoldReleaseTime = 0.0;
			HoldReleaseCorrectTime = 0.0;
			TrigetOn = false;
			BodyOn = false;
			HeadJudged = false;
			LastHoldState = true;
			AllJudged = false;
			if (GaugeMaterial.HasProperty("_Amount"))
			{
				GaugeMaterial.SetFloat("_Amount", 0f);
			}
			for (int i = 0; i < 4; i++)
			{
				PushList[i] = false;
			}
			float num = TouchNoteB.NotesScale[(int)TouchSize];
			HoldGaugeObject.transform.localScale = new Vector3(num, num, 1f);
		}

		private void Start()
		{
			NoteKind = NoteTypeID.Def.TouchHold;
		}

		public override void Execute()
		{
			if (!EndFlag)
			{
				GetNoteYPosition();
				float holdAmount = GetHoldAmount();
				if (GaugeMaterial.HasProperty("_Amount"))
				{
					GaugeMaterial.SetFloat("_Amount", holdAmount);
				}
			}
			NoteCheck();
		}

		protected override void NoteCheck()
		{
			float currentMsec = NotesManager.GetCurrentMsec();
			bool loopDisable = currentMsec >= TailMsec;
			if (!IsNoteCheckTimeStart(Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.GetJudgeTimingFrame()))
			{
				return;
			}
			SetAutoPlayJudge();
			if (!GameManager.IsAutoPlay() && InputManager.InGameTouchPanelArea_C_Down(base.MonitorId))
			{
				TrigetOn = true;
				if (IsJudgeNote() && GetJudgeHeadResult() == NoteJudge.ETiming.End && !InputManager.IsUsedThisFrame(base.MonitorId, (InputManager.TouchPanelArea)(base.ButtonId + 16)))
				{
					JudgeHoldHead();
					JudgeEffectObject.InitializeHold(GetJudgeHeadResult());
					ReserveTouchHoldLoopSe(NoteJudge.ConvertJudge(GetJudgeHeadResult()), loopDisable);
					HeadJudged = true;
					BodyOn = true;
				}
			}
			if (JudgeToolate() && !HeadJudged)
			{
				HeadJudged = true;
			}
			if (GameManager.IsAutoPlay() && GetJudgeHeadResult() != NoteJudge.ETiming.End && !HeadJudged)
			{
				HeadJudged = true;
				TrigetOn = true;
				BodyOn = true;
				JudgeEffectObject.InitializeHold(GetJudgeHeadResult());
				ReserveTouchHoldLoopSe(NoteJudge.ConvertJudge(GetJudgeHeadResult()), loopDisable);
			}
			for (int num = 3; num > 0; num--)
			{
				PushList[num] = PushList[num - 1];
			}
			bool flag = false;
			if (InputManager.InGameTouchPanelArea_C_Push(base.MonitorId))
			{
				flag = true;
			}
			if (GameManager.IsAutoPlay())
			{
				flag = true;
			}
			PushList[0] = flag;
			if (IsNoteCheckTimeHoldHeadIgnoreJudgeWait() && IsNoteCheckTimeHoldTailIgnoreJudgeWait())
			{
				bool flag2 = false;
				if (HeadJudged)
				{
					if (flag)
					{
						flag2 = true;
						HoldReleaseCorrectTime = 0.0;
					}
					else
					{
						bool flag3 = false;
						for (int i = 1; i < 3; i++)
						{
							if (PushList[i])
							{
								flag3 = true;
							}
						}
						if (flag3)
						{
							HoldReleaseCorrectTime += GameManager.GetGameMSecAddD();
							flag2 = true;
						}
					}
				}
				if (!flag2)
				{
					HoldReleaseTime += GameManager.GetGameMSecAddD();
					if (PushList[3])
					{
						HoldReleaseTime += HoldReleaseCorrectTime;
					}
					HoldReleaseCorrectTime = 0.0;
				}
				if (flag2 && !LastHoldState)
				{
					ReserveTouchHoldLoopSe(NoteJudge.ConvertJudge(GetJudgeHeadResult()), loopDisable);
					JudgeEffectObject.InitializeHold(GetJudgeHeadResult());
				}
				else if (!flag2 && LastHoldState)
				{
					SoundManager.StopGameSingleSe(base.MonitorId, SoundManager.PlayerID.TouchHoldLoop);
					JudgeEffectObject.StopHoldPlay();
				}
				LastHoldState = flag2;
				if (flag2)
				{
					BodyOn = true;
				}
				HoldOn(LastHoldState);
			}
			if (currentMsec >= TailMsec && HeadJudged)
			{
				EndNote();
			}
		}

		protected float GetHoldAmount()
		{
			float currentMsec = NotesManager.GetCurrentMsec();
			NoteStat = NoteStatus.Move;
			if (AppearMsec >= currentMsec)
			{
				return 0f;
			}
			NoteStat = NoteStatus.Move;
			float num = (currentMsec - AppearMsec) / (TailMsec - AppearMsec);
			if (num > 1f)
			{
				num = 1f;
			}
			return num;
		}

		protected override void SetPlayResult()
		{
			Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).SetResult(NoteIndex, NoteScore.EScoreType.Hold, GetJudgeResult());
		}

		public override void SetForcePlayResult(int monitorId, NoteData note, NoteJudge.ETiming timing)
		{
			Singleton<GamePlayManager>.Instance.GetGameScore(monitorId).SetResult(note.indexNote, NoteScore.EScoreType.Hold, timing);
		}

		protected override bool JudgeToolate()
		{
			if (GetJudgeHeadResult() == NoteJudge.ETiming.TooLate)
			{
				return true;
			}
			if (GetJudgeHeadResult() != NoteJudge.ETiming.End)
			{
				return false;
			}
			float _fMsec = NotesManager.GetCurrentMsec() - AppearMsec;
			if (NoteJudge.GetJudgeTiming(ref _fMsec, Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.GetJudgeTimingFrame(), JudgeType) == NoteJudge.ETiming.TooLate)
			{
				JudgeHeadResult = NoteJudge.ETiming.TooLate;
				JudgeTimingDiffMsec = _fMsec;
			}
			return GetJudgeHeadResult() == NoteJudge.ETiming.TooLate;
		}

		protected bool JudgeHoldHead()
		{
			if (IsNoteCheckTimeStart(Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.GetJudgeTimingFrame()))
			{
				JudgeTimingDiffMsec = NotesManager.GetCurrentMsec() - AppearMsec;
				JudgeHeadResult = NoteJudge.GetJudgeTiming(ref JudgeTimingDiffMsec, Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.GetJudgeTimingFrame(), JudgeType);
				if (GetJudgeHeadResult() != NoteJudge.ETiming.End)
				{
					PlayJudgeHeadSe();
					InputManager.SetUsedThisFrame(base.MonitorId, InputManager.TouchPanelArea.C1);
					InputManager.SetUsedThisFrame(base.MonitorId, InputManager.TouchPanelArea.C2);
					return true;
				}
				return false;
			}
			return false;
		}

		protected void JudgeTotalResult()
		{
			if (GameManager.IsAutoPlay())
			{
				JudgeResult = GameManager.AutoJudge();
			}
			else
			{
				JudgeResult = NoteJudge.JudgeHoldTotal(TailMsec - AppearMsec, (float)HoldReleaseTime, JudgeHeadResult, BodyOn, isTouchHold: true);
			}
		}

		protected virtual void PlayJudgeHeadSe()
		{
			if (base.ExNote)
			{
				ReserveExJudgeSe(NoteJudge.ConvertJudge(GetJudgeHeadResult()));
			}
			else
			{
				ReserveTapJudgeSe(NoteJudge.ConvertJudge(GetJudgeHeadResult()));
			}
		}

		protected override void SetAutoPlayJudge()
		{
			if (NotesManager.GetCurrentMsec() > AppearMsec - 4.16666651f && GameManager.IsAutoPlay() && !HeadJudged)
			{
				bool loopDisable = NotesManager.GetCurrentMsec() >= TailMsec;
				JudgeHeadResult = GameManager.AutoJudge();
				PlayJudgeHeadSe();
				ReserveTouchHoldLoopSe(NoteJudge.ConvertJudge(GetJudgeHeadResult()), loopDisable);
			}
		}

		protected override void EndNote()
		{
			EndFlag = true;
			JudgeTotalResult();
			JudgeEffectObject.FinishHold(GetJudgeResult());
			JudgeGradeObject.Initialize(GetJudgeResult(), JudgeTimingDiffMsec, NoteJudge.EJudgeType.Touch);
			PlayJudgeSe();
			SetPlayResult();
			SoundManager.StopGameSingleSe(base.MonitorId, SoundManager.PlayerID.TouchHoldLoop);
			TouchResreveObject.DeathNote();
			base.gameObject.SetActive(value: false);
			base.transform.SetParent(base.ParentTransform, worldPositionStays: false);
			if (NeedGuide)
			{
				GuideObj.ReturnToBase();
			}
		}
	}
}
