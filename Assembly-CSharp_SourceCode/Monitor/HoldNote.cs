using DB;
using MAI2.Util;
using Manager;
using Process;
using UnityEngine;

namespace Monitor
{
	public class HoldNote : NoteBase
	{
		[SerializeField]
		protected GameObject EndPointObj;

		protected SpriteRenderer EndPointSprite;

		[SerializeField]
		protected SpriteRenderer EffectSprite;

		protected NoteJudge.ETiming JudgeHeadResult;

		private const int PushOldFrameNum = 4;

		private readonly bool[] PushList = new bool[4];

		protected const float DefaultHeight = 140f;

		protected double HoldReleaseTime;

		protected double HoldReleaseCorrectTime;

		protected bool TrigetOn;

		protected bool BodyOn;

		protected bool HeadJudged;

		protected bool EachOn;

		protected bool LastHoldState;

		protected bool AllJudged;

		protected bool HoldBodyOnFlg;

		public NoteJudge.ETiming GetJudgeHeadResult()
		{
			return JudgeHeadResult;
		}

		protected override void SetEach(bool eachFlag)
		{
			EachOn = eachFlag;
			if (eachFlag)
			{
				SpriteRender.sprite = GameNoteImageContainer.EachHold[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.HoldDesign];
				EffectSprite.sprite = GameNoteImageContainer.EachHoldOn[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.HoldDesign];
				EndPointSprite.sprite = GameNoteImageContainer.EachHoldEnd;
				if (GuideObj != null)
				{
					GuideObj.SetColor(NoteGuide.Color.Each);
				}
				SpriteRenderEx.color = CommonScriptable.GetNotesEffectSetting().EachColor;
			}
			else
			{
				SpriteRender.sprite = GameNoteImageContainer.NormalHold[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.HoldDesign];
				EffectSprite.sprite = GameNoteImageContainer.NormalHoldOn[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.HoldDesign];
				EndPointSprite.sprite = GameNoteImageContainer.NormalHoldEnd;
				if (GuideObj != null)
				{
					GuideObj.SetColor(NoteGuide.Color.Normal);
					SpriteRenderEx.color = CommonScriptable.GetNotesEffectSetting().TapColor;
				}
			}
			SpriteRender.size = new Vector2(SpriteRender.size.x, 140f);
			EachFlag = eachFlag;
		}

		protected void HoldOn(bool on)
		{
			if (EachOn)
			{
				SpriteRender.sprite = (on ? GameNoteImageContainer.EachHoldOn[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.HoldDesign] : GameNoteImageContainer.HoldOff[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.HoldDesign]);
			}
			else
			{
				SpriteRender.sprite = (on ? GameNoteImageContainer.NormalHoldOn[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.HoldDesign] : GameNoteImageContainer.HoldOff[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.HoldDesign]);
			}
			HoldBodyOnFlg = on;
		}

		protected override void Awake()
		{
			base.Awake();
			GuideStop = true;
			EndPointSprite = EndPointObj.GetComponent<SpriteRenderer>();
			SpriteRenderEx = ExObj.GetComponent<SpriteRenderer>();
		}

		public override void Initialize(NoteData note)
		{
			base.Initialize(note);
			EndPointObj.SetActive(value: false);
			EndPointObj.transform.localPosition = new Vector3(0f, StartPos, GetBaseZPosition());
			EndPointSprite.sortingOrder = -(NoteIndex * 10);
			EffectSprite.sortingOrder = -(NoteIndex * 10) + 1;
			JudgeHeadResult = NoteJudge.ETiming.End;
			HoldReleaseTime = 0.0;
			HoldReleaseCorrectTime = 0.0;
			TrigetOn = false;
			BodyOn = false;
			HeadJudged = false;
			LastHoldState = true;
			AllJudged = false;
			SpriteRender.size = new Vector2(SpriteRender.size.x, 140f);
			SpriteRenderEx.sprite = GameNoteImageContainer.ExHold[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.HoldDesign];
			ExObj.SetActive(base.ExNote);
			JudgeType = (base.ExNote ? NoteJudge.EJudgeType.ExTap : NoteJudge.EJudgeType.Tap);
			HoldBodyOnFlg = false;
			for (int i = 0; i < 4; i++)
			{
				PushList[i] = false;
			}
		}

		private void Start()
		{
			NoteKind = NoteTypeID.Def.Hold;
		}

		public override void Execute()
		{
			if (!EndFlag)
			{
				float num = GetNoteYPosition();
				if (num >= EndPos)
				{
					num = EndPos;
				}
				if (GetNoteStatus() == NoteStatus.Scale)
				{
					SpriteRender.size = new Vector2(SpriteRender.size.x, 140f);
					NoteObj.transform.localPosition = new Vector3(0f, num, GetBaseZPosition());
					EndPointObj.transform.localPosition = new Vector3(0f, num, GetBaseZPosition());
				}
				else
				{
					float num2 = TailMsec - GetMaiBugAdjustMSec();
					if (num2 - DefaultMsec <= NotesManager.GetCurrentMsec())
					{
						if (!EndPointObj.activeSelf)
						{
							EndPointObj.SetActive(value: true);
						}
						if (TailMsec > NotesManager.GetCurrentMsec())
						{
							if (num2 < NotesManager.GetCurrentMsec())
							{
								num2 = NotesManager.GetCurrentMsec();
							}
							float num3 = num2 - NotesManager.GetCurrentMsec();
							float num4 = 1f * (1f - num3 / DefaultMsec);
							float num5 = (EndPos - StartPos) * num4;
							float num6 = num - StartPos - num5;
							SpriteRender.size = new Vector2(SpriteRender.size.x, num6 + 140f);
							NoteObj.transform.localPosition = ((AppearMsec > NotesManager.GetCurrentMsec()) ? new Vector3(0f, num - num6 / 2f, GetBaseZPosition()) : new Vector3(0f, EndPos - num6 / 2f, GetBaseZPosition()));
							NoteObj.transform.localPosition = new Vector3(0f, num - num6 / 2f, GetBaseZPosition());
							EndPointObj.transform.localPosition = new Vector3(0f, num - num6, GetBaseZPosition());
						}
						else
						{
							NoteObj.transform.localPosition = new Vector3(0f, EndPos, GetBaseZPosition());
							SpriteRender.size = new Vector2(SpriteRender.size.x, 140f);
						}
					}
					else
					{
						float num6 = num - StartPos;
						SpriteRender.size = new Vector2(SpriteRender.size.x, num6 + 140f);
						NoteObj.transform.localPosition = new Vector3(0f, num - num6 / 2f, GetBaseZPosition());
						EndPointObj.transform.localPosition = new Vector3(0f, num - num6, GetBaseZPosition());
					}
				}
				SpriteRenderEx.size = SpriteRender.size;
				EffectSprite.size = SpriteRender.size;
			}
			NoteCheck();
		}

		public void Execute_old()
		{
			if (!EndFlag)
			{
				float num = GetNoteYPosition();
				if (num >= EndPos)
				{
					num = EndPos;
				}
				if (GetNoteStatus() == NoteStatus.Scale)
				{
					SpriteRender.size = new Vector2(SpriteRender.size.x, 140f);
					NoteObj.transform.localPosition = new Vector3(0f, num, GetBaseZPosition());
					EndPointObj.transform.localPosition = new Vector3(0f, num, GetBaseZPosition());
				}
				else if (TailMsec - DefaultMsec <= NotesManager.GetCurrentMsec())
				{
					if (TailMsec > NotesManager.GetCurrentMsec())
					{
						float num2 = TailMsec - NotesManager.GetCurrentMsec();
						float num3 = 1f * (1f - num2 / DefaultMsec);
						float num4 = (EndPos - StartPos) * num3;
						float num5 = num - StartPos - num4;
						SpriteRender.size = new Vector2(SpriteRender.size.x, num5 + 140f);
						NoteObj.transform.localPosition = ((AppearMsec > NotesManager.GetCurrentMsec()) ? new Vector3(0f, num - num5 / 2f, GetBaseZPosition()) : new Vector3(0f, EndPos - num5 / 2f, GetBaseZPosition()));
						NoteObj.transform.localPosition = new Vector3(0f, num - num5 / 2f, GetBaseZPosition());
						EndPointObj.transform.localPosition = new Vector3(0f, num - num5, GetBaseZPosition());
					}
					else
					{
						NoteObj.transform.localPosition = new Vector3(0f, EndPos, GetBaseZPosition());
						SpriteRender.size = new Vector2(SpriteRender.size.x, 140f);
					}
				}
				else
				{
					float num5 = num - StartPos;
					SpriteRender.size = new Vector2(SpriteRender.size.x, num5 + 140f);
					NoteObj.transform.localPosition = new Vector3(0f, num - num5 / 2f, GetBaseZPosition());
					EndPointObj.transform.localPosition = new Vector3(0f, num - num5, GetBaseZPosition());
				}
				SpriteRenderEx.size = SpriteRender.size;
				EffectSprite.size = SpriteRender.size;
			}
			NoteCheck();
		}

		protected override void NoteCheck()
		{
			float currentMsec = NotesManager.GetCurrentMsec();
			if (IsNoteCheckTimeStart(Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.GetJudgeTimingFrame()))
			{
				SetAutoPlayJudge();
				if ((!GameManager.IsAutoPlay() && InputManager.InGameButtonDown(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId)) || InputManager.InGameTouchPanelAreaDown(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId))
				{
					TrigetOn = true;
					if (IsJudgeNote() && GetJudgeHeadResult() == NoteJudge.ETiming.End && !InputManager.IsUsedThisFrame(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId))
					{
						JudgeHoldHead();
						JudgeEffectObject.InitializeHold(GetJudgeHeadResult());
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
				}
			}
			for (int num = 3; num > 0; num--)
			{
				PushList[num] = PushList[num - 1];
			}
			bool flag = false;
			if ((!GameManager.IsAutoPlay() && InputManager.GetButtonPush(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId)) || InputManager.GetTouchPanelAreaPush(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId))
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
					JudgeEffectObject.InitializeHold(GetJudgeHeadResult());
				}
				else if (!flag2 && LastHoldState)
				{
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
				return;
			}
			float num2 = StartMsec + DefaultMsec - GetMaiBugAdjustMSec();
			float num3 = ((num2 <= NotesManager.GetCurrentMsec()) ? 1f : (1f - (num2 - NotesManager.GetCurrentMsec()) / DefaultMsec));
			if ((double)num3 < 0.0)
			{
				num3 = 0f;
			}
			NoteObj.transform.localScale = new Vector3(num3 * Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.NoteSize.GetValue(), num3, 0f);
			EndPointObj.transform.localScale = new Vector3(num3, num3, 1f);
		}

		protected override void NoteCheck_old()
		{
			float currentMsec = NotesManager.GetCurrentMsec();
			if (IsNoteCheckTimeStart(Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.GetJudgeTimingFrame()))
			{
				SetAutoPlayJudge();
				if ((!GameManager.IsAutoPlay() && InputManager.InGameButtonDown(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId)) || InputManager.InGameTouchPanelAreaDown(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId))
				{
					TrigetOn = true;
					if (IsJudgeNote() && GetJudgeHeadResult() == NoteJudge.ETiming.End && !InputManager.IsUsedThisFrame(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId))
					{
						JudgeHoldHead();
						JudgeEffectObject.InitializeHold(GetJudgeHeadResult());
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
				}
			}
			if (IsNoteCheckTimeHoldHeadIgnoreJudgeWait() && IsNoteCheckTimeHoldTailIgnoreJudgeWait())
			{
				bool flag = false;
				if (HeadJudged && ((!GameManager.IsAutoPlay() && InputManager.GetButtonPush(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId)) || InputManager.GetTouchPanelAreaPush(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId)))
				{
					flag = true;
				}
				if (GameManager.IsAutoPlay())
				{
					flag = true;
				}
				if (!flag)
				{
					HoldReleaseTime += GameManager.GetGameMSecAddD();
				}
				if (flag && !LastHoldState)
				{
					JudgeEffectObject.InitializeHold(GetJudgeHeadResult());
				}
				else if (!flag && LastHoldState)
				{
					JudgeEffectObject.StopHoldPlay();
				}
				LastHoldState = flag;
				if (flag)
				{
					BodyOn = true;
				}
				HoldOn(LastHoldState);
			}
			if (currentMsec >= TailMsec && HeadJudged)
			{
				EndNote();
				return;
			}
			float num = StartMsec + DefaultMsec;
			float num2 = ((num <= NotesManager.GetCurrentMsec()) ? 1f : (1f - (num - NotesManager.GetCurrentMsec()) / DefaultMsec));
			if ((double)num2 < 0.0)
			{
				num2 = 0f;
			}
			NoteObj.transform.localScale = new Vector3(num2 * Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.NoteSize.GetValue(), num2, 0f);
			EndPointObj.transform.localScale = new Vector3(num2, num2, 1f);
		}

		protected override void SetPlayResult()
		{
			Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).SetResult(NoteIndex, NoteScore.EScoreType.Hold, GetJudgeResult());
		}

		public override void SetForcePlayResult(int monitorId, NoteData note, NoteJudge.ETiming timing)
		{
			Singleton<GamePlayManager>.Instance.GetGameScore(monitorId).SetResult(note.indexNote, NoteScore.EScoreType.Hold, timing);
		}

		protected override void SetAutoPlayJudge()
		{
			if (NotesManager.GetCurrentMsec() > AppearMsec - 4.16666651f && GameManager.IsAutoPlay() && !HeadJudged)
			{
				JudgeHeadResult = GameManager.AutoJudge();
				PlayJudgeHeadSe();
			}
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
					if (GetJudgeResult() != NoteJudge.ETiming.TooLate)
					{
						InputManager.SetUsedThisFrame(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId);
					}
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
				JudgeResult = NoteJudge.JudgeHoldTotal(TailMsec - AppearMsec, (float)HoldReleaseTime, JudgeHeadResult, BodyOn, isTouchHold: false);
			}
		}

		protected void PlayJudgeHeadSe()
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

		protected override void EndNote()
		{
			EndFlag = true;
			JudgeTotalResult();
			JudgeEffectObject.FinishHold(GetJudgeResult());
			JudgeGradeObject.Initialize(GetJudgeResult(), 0f, NoteJudge.EJudgeType.HoldOut);
			PlayJudgeSe();
			SetPlayResult();
			base.gameObject.SetActive(value: false);
			base.transform.SetParent(base.ParentTransform, worldPositionStays: false);
			if (NeedGuide)
			{
				GuideObj.ReturnToBase();
			}
		}

		protected override void PlayJudgeSe()
		{
			if (!ShotJudgeSound)
			{
				ReserveTapJudgeSe(NoteJudge.ConvertJudge(GetJudgeResult()));
			}
			ShotJudgeSound = true;
		}

		protected override float GetNoteYPosition()
		{
			if (HoldBodyOnFlg)
			{
				EffectSprite.color = new Color(1f, 1f, 1f, Mathf.Sin(GameManager.GetGameFrame() * 0.4f) * 0.25f + 0.25f);
			}
			else
			{
				EffectSprite.color = new Color(1f, 1f, 1f, 0f);
			}
			return base.GetNoteYPosition();
		}
	}
}
