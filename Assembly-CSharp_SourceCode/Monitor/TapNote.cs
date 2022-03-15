using MAI2.Util;
using Manager;
using Process;
using UnityEngine;

namespace Monitor
{
	public class TapNote : NoteBase
	{
		protected override void SetEach(bool eachFlag)
		{
			if (eachFlag)
			{
				SpriteRender.sprite = GameNoteImageContainer.EachTap[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.TapDesign];
				if (GuideObj != null)
				{
					GuideObj.SetColor(NoteGuide.Color.Each);
				}
				SpriteRenderEx.color = CommonScriptable.GetNotesEffectSetting().EachColor;
			}
			else
			{
				SpriteRender.sprite = GameNoteImageContainer.NormalTap[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.TapDesign];
				if (GuideObj != null)
				{
					GuideObj.SetColor(NoteGuide.Color.Normal);
				}
				SpriteRenderEx.color = CommonScriptable.GetNotesEffectSetting().TapColor;
			}
			EachFlag = eachFlag;
		}

		protected override void Awake()
		{
			base.Awake();
			SpriteRenderEx = ExObj.GetComponent<SpriteRenderer>();
		}

		public override void Initialize(NoteData note)
		{
			NoteKind = NoteTypeID.Def.Begin;
			base.Initialize(note);
			SpriteRenderEx.sprite = GameNoteImageContainer.ExTap[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.TapDesign];
			ExObj.SetActive(base.ExNote);
			JudgeType = (base.ExNote ? NoteJudge.EJudgeType.ExTap : NoteJudge.EJudgeType.Tap);
		}

		public override void Execute()
		{
			if (!EndFlag)
			{
				NoteObj.transform.localPosition = new Vector3(0f, GetNoteYPosition(), GetBaseZPosition());
			}
			NoteCheck();
		}
	}
}
