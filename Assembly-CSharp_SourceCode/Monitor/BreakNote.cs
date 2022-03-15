using DB;
using MAI2.Util;
using Manager;
using Monitor.Game;
using Process;
using UnityEngine;

namespace Monitor
{
	public class BreakNote : NoteBase
	{
		[SerializeField]
		protected SpriteRenderer EffectSprite;

		protected override void ReserveTapJudgeSe(NoteJudge.JudgeBox judge)
		{
			Singleton<GameSingleCueCtrl>.Instance.ReserveBreakJudgeSe(MonitorIndex, judge);
		}

		protected override void SetEach(bool eachFlag)
		{
			SpriteRender.sprite = GameNoteImageContainer.NormalBreak[(int)Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.TapDesign];
			EffectSprite.sprite = GameNoteImageContainer.NormalBreakEff[(int)Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.TapDesign];
			if (GuideObj != null)
			{
				GuideObj.SetColor(NoteGuide.Color.Break);
			}
			EachFlag = eachFlag;
		}

		public override void Initialize(NoteData note)
		{
			base.Initialize(note);
			EffectSprite.sortingOrder = -(NoteIndex * 10) + 1;
		}

		private void Start()
		{
			NoteKind = NoteTypeID.Def.Break;
		}

		public override void Execute()
		{
			if (!EndFlag)
			{
				NoteObj.transform.localPosition = new Vector3(0f, GetNoteYPosition(), GetBaseZPosition());
			}
			NoteCheck();
		}

		protected override void NoteCheck()
		{
			if (((!GameManager.IsAutoPlay() && InputManager.InGameButtonDown(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId)) || InputManager.InGameTouchPanelAreaDown(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId)) && IsJudgeNote() && !InputManager.IsUsedThisFrame(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId))
			{
				Judge();
			}
			if (GetJudgeResult() != NoteJudge.ETiming.End || JudgeToolate())
			{
				EndNote();
			}
			SetAutoPlayJudge();
			if (!EndFlag)
			{
				float num = StartMsec + DefaultMsec - GetMaiBugAdjustMSec();
				float num2 = ((num <= NotesManager.GetCurrentMsec()) ? 1f : (1f - (num - NotesManager.GetCurrentMsec()) / DefaultMsec));
				if ((double)num2 < 0.0)
				{
					num2 = 0f;
				}
				num2 *= Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.NoteSize.GetValue();
				NoteObj.transform.localScale = new Vector3(num2, num2, 0f);
			}
		}

		protected override void NoteCheck_old()
		{
			if (((!GameManager.IsAutoPlay() && InputManager.InGameButtonDown(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId)) || InputManager.InGameTouchPanelAreaDown(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId)) && IsJudgeNote() && !InputManager.IsUsedThisFrame(base.MonitorId, (InputManager.ButtonSetting)base.ButtonId))
			{
				Judge();
			}
			if (GetJudgeResult() != NoteJudge.ETiming.End || JudgeToolate())
			{
				EndNote();
			}
			SetAutoPlayJudge();
			if (!EndFlag)
			{
				float num = StartMsec + DefaultMsec;
				float num2 = ((num <= NotesManager.GetCurrentMsec()) ? 1f : (1f - (num - NotesManager.GetCurrentMsec()) / DefaultMsec));
				if ((double)num2 < 0.0)
				{
					num2 = 0f;
				}
				num2 *= Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.NoteSize.GetValue();
				NoteObj.transform.localScale = new Vector3(num2, num2, 0f);
			}
		}

		protected override void SetPlayResult()
		{
			Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).SetResult(NoteIndex, NoteScore.EScoreType.Break, GetJudgeResult());
		}

		public override void SetForcePlayResult(int monitorId, NoteData note, NoteJudge.ETiming timing)
		{
			Singleton<GamePlayManager>.Instance.GetGameScore(monitorId).SetResult(note.indexNote, NoteScore.EScoreType.Break, timing);
		}

		protected override void EndNote()
		{
			EndFlag = true;
			NoteObj.transform.localScale = new Vector3(0f, 0f, 1f);
			NoteStat = NoteStatus.End;
			JudgeGradeObject.InitializeBreak(GetJudgeResult(), JudgeTimingDiffMsec, NoteJudge.EJudgeType.Break);
			JudgeEffectObject.InitializeBreak(GetJudgeResult());
			SetPlayResult();
			base.gameObject.SetActive(value: false);
			base.transform.SetParent(base.ParentTransform, worldPositionStays: false);
			if (NeedGuide)
			{
				GuideObj.ReturnToBase();
			}
		}

		protected override float GetNoteYPosition()
		{
			EffectSprite.color = new Color(1f, 1f, 1f, Mathf.Sin(GameManager.GetGameFrame() * 0.199995011f) * 0.5f);
			return base.GetNoteYPosition();
		}
	}
}
