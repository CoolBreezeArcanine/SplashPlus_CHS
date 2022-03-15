using DB;
using MAI2.Util;
using Manager;
using Monitor.Game;
using UnityEngine;

namespace Monitor
{
	public abstract class NoteBase : MonoBehaviour
	{
		public enum NoteStatus
		{
			Init,
			Scale,
			Move,
			Check,
			End,
			Max
		}

		[SerializeField]
		protected GameObject NoteObj;

		protected NoteGuide GuideObj;

		[SerializeField]
		protected bool NeedGuide;

		[SerializeField]
		protected GameObject ExObj;

		protected NoteStatus NoteStat;

		protected NoteTypeID.Def NoteKind = NoteTypeID.Def.End;

		protected int NoteIndex;

		protected const float NoteIndexZposDiff = 0.0001f;

		protected const float ClassicDelayFlame = 4f;

		protected const float CurrentDelayFlame = 1f;

		protected int MonitorIndex;

		protected float StartPos;

		protected float EndPos;

		protected bool EndFlag;

		protected float DefaultMsec;

		protected float AppearMsec;

		protected float TailMsec;

		protected float StartMsec;

		protected bool EachFlag;

		protected NoteJudge.EJudgeType JudgeType;

		protected NoteJudge.ETiming JudgeResult;

		protected float JudgeTimingDiffMsec;

		protected SpriteRenderer SpriteRender;

		protected SpriteRenderer SpriteRenderEx;

		protected Transform NoteGuideTrans;

		protected bool GuideStop;

		protected JudgeGrade JudgeGradeObject;

		protected TouchEffect JudgeEffectObject;

		protected bool ShotJudgeSound;

		protected bool IsExNote;

		protected NotesManager NoteMng;

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

		protected int ButtonId { get; set; }

		public Transform ParentTransform { get; set; }

		public bool ExNote
		{
			get
			{
				return IsExNote;
			}
			set
			{
				IsExNote = value;
			}
		}

		public NoteStatus GetNoteStatus()
		{
			return NoteStat;
		}

		protected virtual float GetBaseZPosition()
		{
			return -6f + (float)NoteIndex * 0.0001f;
		}

		public int GetNoteIndex()
		{
			return NoteIndex;
		}

		protected int GetGuideZPosition()
		{
			return -2;
		}

		public bool IsEnd()
		{
			return EndFlag;
		}

		public virtual float GetJudgeStartMsec()
		{
			return NoteJudge.GetNoteCheckStart(JudgeType);
		}

		public virtual float GetJudgeEndMsec()
		{
			return NoteJudge.GetNoteCheckEnd(JudgeType);
		}

		public NoteJudge.ETiming GetJudgeResult()
		{
			return JudgeResult;
		}

		protected bool IsNoteCheckTimeStart(float judgeTimingFrame)
		{
			return NotesManager.GetCurrentMsec() - judgeTimingFrame * 16.666666f >= AppearMsec + GetJudgeStartMsec();
		}

		protected virtual bool IsNoteCheckTimeHoldHeadIgnoreJudgeWait()
		{
			return NotesManager.GetCurrentMsec() >= AppearMsec + NoteJudge.JudgeHoldHeadFrame;
		}

		protected virtual bool IsNoteCheckTimeHoldTailIgnoreJudgeWait()
		{
			return NotesManager.GetCurrentMsec() <= TailMsec - NoteJudge.JudgeHoldTailFrame;
		}

		protected virtual void ReserveTapJudgeSe(NoteJudge.JudgeBox judge)
		{
			Singleton<GameSingleCueCtrl>.Instance.ReserveTapJudgeSe(MonitorIndex, judge);
		}

		protected void ReserveExJudgeSe(NoteJudge.JudgeBox judge)
		{
			Singleton<GameSingleCueCtrl>.Instance.ReserveExSe(MonitorIndex, judge);
		}

		protected virtual bool IsJudgeNote()
		{
			return base.transform.parent.childCount - 3 == base.transform.GetSiblingIndex();
		}

		protected virtual void Awake()
		{
			SpriteRender = NoteObj.GetComponent<SpriteRenderer>();
			NoteStat = NoteStatus.Init;
		}

		public void SetJudgeObject(JudgeGrade judgeGrade, TouchEffect judgeEffect)
		{
			JudgeGradeObject = judgeGrade;
			JudgeEffectObject = judgeEffect;
		}

		public void SetGuideObject(NoteGuide guideObj)
		{
			GuideObj = guideObj;
			GuideObj.gameObject.SetActive(value: true);
		}

		public virtual void Initialize(NoteData note)
		{
			if (GameManager.ForceHideNote(MonitorId))
			{
				NeedGuide = false;
				ExNote = false;
				NoteObj.SetActive(value: false);
			}
			NoteStat = NoteStatus.Init;
			NoteMng = NotesManager.Instance(MonitorId);
			StartPos = GameCtrl.NoteStartPos();
			EndPos = GameCtrl.NoteEndPos();
			AppearMsec = note.time.msec;
			TailMsec = note.end.msec;
			ButtonId = note.startButtonPos;
			NoteIndex = note.indexNote;
			JudgeResult = NoteJudge.ETiming.End;
			JudgeTimingDiffMsec = 0f;
			EndFlag = false;
			ShotJudgeSound = false;
			DefaultMsec = (float)((double)GameManager.GetNoteSpeedForBeat((int)Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.GetNoteSpeed) * 4.0);
			StartMsec = AppearMsec - DefaultMsec * 2f;
			NoteObj.transform.localScale = new Vector3(0f, 0f, 0f);
			NoteObj.transform.localPosition = new Vector3(0f, StartPos, GetBaseZPosition());
			SpriteRender.sortingOrder = -(NoteIndex * 10);
			if (NeedGuide)
			{
				NoteGuideTrans = GuideObj.transform;
				GuideObj.Initialize(GetEachAngle(note), note.indexEach);
				GuideObj.gameObject.SetActive(value: true);
				GuideObj.transform.SetParent(base.transform);
				NoteGuideTrans.localScale = new Vector3(0f, 0f, 1f);
				NoteGuideTrans.localPosition = new Vector3(0f, 0f, GetGuideZPosition());
				NoteGuideTrans.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
			if (null != ExObj)
			{
				ExObj.SetActive(IsExNote);
				SpriteRenderEx.sortingOrder = SpriteRender.sortingOrder + 1;
				NoteObj.transform.localScale = new Vector3(0f, 0f, 0f);
				NoteObj.transform.localPosition = new Vector3(0f, StartPos, GetBaseZPosition());
			}
			SetEach(note.isEach);
		}

		protected int GetEachAngle(NoteData note)
		{
			int num = 0;
			int num2 = 4;
			int num3 = -note.startButtonPos;
			foreach (NoteData item in note.eachChild)
			{
				int num4 = item.startButtonPos + num3;
				if (num4 < 0)
				{
					num4 += 8;
				}
				int num5 = 0;
				num5 = ((num4 <= num2) ? (45 * num4) : (45 * (8 - num4) * -1));
				if (Mathf.Abs(num5) > Mathf.Abs(num))
				{
					num = num5;
				}
			}
			return num;
		}

		protected abstract void SetEach(bool eachFlag);

		protected virtual void SetAutoPlayJudge()
		{
			if (NotesManager.GetCurrentMsec() > AppearMsec - 4.16666651f && GameManager.IsAutoPlay())
			{
				JudgeResult = GameManager.AutoJudge();
				PlayJudgeSe();
			}
		}

		protected virtual void NoteCheck()
		{
			SetAutoPlayJudge();
			if (((!GameManager.IsAutoPlay() && InputManager.InGameButtonDown(MonitorId, (InputManager.ButtonSetting)ButtonId)) || InputManager.InGameTouchPanelAreaDown(MonitorId, (InputManager.ButtonSetting)ButtonId)) && IsJudgeNote() && !InputManager.IsUsedThisFrame(MonitorId, (InputManager.ButtonSetting)ButtonId))
			{
				Judge();
			}
			if (GetJudgeResult() != NoteJudge.ETiming.End || JudgeToolate())
			{
				EndNote();
			}
			if (!EndFlag)
			{
				float num = StartMsec + DefaultMsec - GetMaiBugAdjustMSec();
				float num2 = ((num <= NotesManager.GetCurrentMsec()) ? 1f : (1f - (num - NotesManager.GetCurrentMsec()) / DefaultMsec));
				if ((double)num2 < 0.0)
				{
					num2 = 0f;
				}
				num2 *= Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.NoteSize.GetValue();
				NoteObj.transform.localScale = new Vector3(num2, num2, 0f);
			}
		}

		protected virtual void NoteCheck_old()
		{
			SetAutoPlayJudge();
			if (((!GameManager.IsAutoPlay() && InputManager.InGameButtonDown(MonitorId, (InputManager.ButtonSetting)ButtonId)) || InputManager.InGameTouchPanelAreaDown(MonitorId, (InputManager.ButtonSetting)ButtonId)) && IsJudgeNote() && !InputManager.IsUsedThisFrame(MonitorId, (InputManager.ButtonSetting)ButtonId))
			{
				Judge();
			}
			if (GetJudgeResult() != NoteJudge.ETiming.End || JudgeToolate())
			{
				EndNote();
			}
			if (!EndFlag)
			{
				float num = StartMsec + DefaultMsec;
				float num2 = ((num <= NotesManager.GetCurrentMsec()) ? 1f : (1f - (num - NotesManager.GetCurrentMsec()) / DefaultMsec));
				if ((double)num2 < 0.0)
				{
					num2 = 0f;
				}
				num2 *= Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.NoteSize.GetValue();
				NoteObj.transform.localScale = new Vector3(num2, num2, 0f);
			}
		}

		protected virtual void EndNote()
		{
			EndFlag = true;
			NoteObj.transform.localScale = new Vector3(0f, 0f, 1f);
			NoteStat = NoteStatus.End;
			JudgeGradeObject.Initialize(GetJudgeResult(), JudgeTimingDiffMsec, JudgeType);
			if (IsExNote)
			{
				JudgeEffectObject.InitializeEx(GetJudgeResult());
			}
			else
			{
				JudgeEffectObject.Initialize(GetJudgeResult());
			}
			SetPlayResult();
			base.gameObject.SetActive(value: false);
			base.transform.SetParent(ParentTransform, worldPositionStays: false);
			if (NeedGuide)
			{
				GuideObj.ReturnToBase();
			}
		}

		protected virtual void SetPlayResult()
		{
			Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).SetResult(NoteIndex, NoteScore.EScoreType.Tap, GetJudgeResult());
		}

		public virtual void SetForcePlayResult(int monitorId, NoteData note, NoteJudge.ETiming timing)
		{
			Singleton<GamePlayManager>.Instance.GetGameScore(monitorId).SetResult(note.indexNote, NoteScore.EScoreType.Tap, timing);
		}

		protected virtual float GetNoteYPosition()
		{
			float currentMsec = NotesManager.GetCurrentMsec();
			float maiBugAdjustMSec = GetMaiBugAdjustMSec();
			float num = AppearMsec - DefaultMsec * 2f - maiBugAdjustMSec;
			if (num > currentMsec)
			{
				return StartPos;
			}
			if (num + DefaultMsec >= currentMsec)
			{
				NoteStat = NoteStatus.Scale;
				if (NoteGuideTrans != null)
				{
					float num2 = currentMsec - num;
					NoteGuideTrans.localScale = new Vector3(0.25f, 0.25f, 1f);
					GuideObj.SetAlpha(num2 / DefaultMsec);
				}
				return StartPos;
			}
			NoteStat = NoteStatus.Move;
			float num3 = StartMsec + DefaultMsec * 2f - currentMsec;
			float num4 = 0.75f * (1f - num3 / DefaultMsec);
			float num5 = 1f * (1f - num3 / DefaultMsec);
			float num6 = StartPos + (EndPos - StartPos) * num5;
			float num7 = -1f / 120f;
			float num8 = Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.GetNoteSpeed.GetValue() / 150f;
			float num9 = (EndPos - StartPos) * num7 * (num8 - 1f);
			float num10 = num7 * (num8 - 1f) * 0.75f;
			float result = num6 + num9;
			num4 += num10;
			if (NoteGuideTrans != null)
			{
				NoteGuideTrans.localScale = ((!GuideStop) ? new Vector3(0.25f + num4, 0.25f + num4, 1f) : ((0.25f + num4 <= 1f) ? new Vector3(0.25f + num4, 0.25f + num4, 1f) : new Vector3(1f, 1f, 1f)));
				GuideObj.SetAlpha(1f);
			}
			return result;
		}

		protected float GetMaiBugAdjustMSec()
		{
			float num = -0.5f;
			float num2 = Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.GetNoteSpeed.GetValue() / 150f;
			return (num2 - 1f) * (num / num2) * 1.6f * 1000f / 60f;
		}

		protected virtual float GetNoteYPosition_old()
		{
			float currentMsec = NotesManager.GetCurrentMsec();
			if (StartMsec > currentMsec)
			{
				return StartPos;
			}
			if (StartMsec + DefaultMsec >= currentMsec)
			{
				NoteStat = NoteStatus.Scale;
				if (NoteGuideTrans != null)
				{
					float num = currentMsec - StartMsec;
					NoteGuideTrans.localScale = new Vector3(0.25f, 0.25f, 1f);
					GuideObj.SetAlpha(num / DefaultMsec);
				}
				return StartPos;
			}
			NoteStat = NoteStatus.Move;
			float num2 = StartMsec + DefaultMsec * 2f - currentMsec;
			float num3 = 0.75f * (1f - num2 / DefaultMsec);
			float num4 = 1f * (1f - num2 / DefaultMsec);
			float result = StartPos + (EndPos - StartPos) * num4;
			if (NoteGuideTrans != null)
			{
				NoteGuideTrans.localScale = ((0.25f + num3 <= 1f) ? new Vector3(0.25f + num3, 0.25f + num3, 1f) : new Vector3(0f, 0f, 1f));
				GuideObj.SetAlpha(1f);
			}
			return result;
		}

		protected virtual bool Judge()
		{
			if (IsNoteCheckTimeStart(Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.GetJudgeTimingFrame()))
			{
				JudgeTimingDiffMsec = NotesManager.GetCurrentMsec() - AppearMsec;
				JudgeResult = NoteJudge.GetJudgeTiming(ref JudgeTimingDiffMsec, Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.GetJudgeTimingFrame(), JudgeType);
				if (GetJudgeResult() != NoteJudge.ETiming.End)
				{
					PlayJudgeSe();
					if (GetJudgeResult() != NoteJudge.ETiming.TooLate)
					{
						InputManager.SetUsedThisFrame(MonitorId, (InputManager.ButtonSetting)ButtonId);
					}
					return true;
				}
				return false;
			}
			return false;
		}

		protected virtual bool JudgeToolate()
		{
			if (GetJudgeResult() == NoteJudge.ETiming.TooLate)
			{
				return true;
			}
			if (GetJudgeResult() != NoteJudge.ETiming.End)
			{
				return false;
			}
			float _fMsec = NotesManager.GetCurrentMsec() - AppearMsec;
			if (NoteJudge.GetJudgeTiming(ref _fMsec, Singleton<GamePlayManager>.Instance.GetGameScore(MonitorId).UserOption.GetJudgeTimingFrame(), JudgeType) == NoteJudge.ETiming.TooLate)
			{
				JudgeResult = NoteJudge.ETiming.TooLate;
				JudgeTimingDiffMsec = _fMsec;
				PlayJudgeSe();
			}
			return GetJudgeResult() == NoteJudge.ETiming.TooLate;
		}

		public virtual void Execute()
		{
		}

		protected virtual void PlayJudgeSe()
		{
			if (!ShotJudgeSound)
			{
				if (ExNote)
				{
					ReserveExJudgeSe(NoteJudge.ConvertJudge(GetJudgeResult()));
				}
				else
				{
					ReserveTapJudgeSe(NoteJudge.ConvertJudge(GetJudgeResult()));
				}
			}
			ShotJudgeSound = true;
		}
	}
}
