using System;
using DB;
using MAI2.Util;
using Manager;
using Process;
using UnityEngine;

namespace Monitor
{
	public class StarNote : NoteBase
	{
		protected bool MultiSlide;

		public float StarRotateSpeed { get; set; }

		protected override void SetEach(bool eachFlag)
		{
		}

		public void SetEach(bool eachFlag, bool multiSlide)
		{
			if (eachFlag)
			{
				SpriteRender.sprite = (multiSlide ? GameNoteImageContainer.EachDoubleStar[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.SlideDesign] : GameNoteImageContainer.EachStar[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.SlideDesign]);
				if (GuideObj != null)
				{
					GuideObj.SetColor(NoteGuide.Color.Each);
				}
				SpriteRenderEx.color = CommonScriptable.GetNotesEffectSetting().EachColor;
			}
			else
			{
				if (multiSlide)
				{
					SpriteRender.sprite = GameNoteImageContainer.NormalDoubleStar[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.SlideDesign, (int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.StarType];
				}
				else
				{
					SpriteRender.sprite = GameNoteImageContainer.NormalStar[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.SlideDesign, (int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.StarType];
				}
				OptionStartypeID starType = Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.StarType;
				if (starType == OptionStartypeID.Blue || starType != OptionStartypeID.Pink)
				{
					SpriteRenderEx.color = CommonScriptable.GetNotesEffectSetting().StarColor;
				}
				else
				{
					SpriteRenderEx.color = CommonScriptable.GetNotesEffectSetting().TapColor;
				}
				if (GuideObj != null)
				{
					GuideObj.SetColor(NoteGuide.Color.Slide);
				}
			}
			MultiSlide = multiSlide;
			EachFlag = eachFlag;
		}

		protected override void Awake()
		{
			base.Awake();
			SpriteRenderEx = ExObj.GetComponent<SpriteRenderer>();
		}

		public override void Initialize(NoteData note)
		{
			base.Initialize(note);
			float num = 0f;
			if (note.child.Count > 0)
			{
				float num2 = Singleton<SlideManager>.Instance.GetSlideLength(note.child[0].slideData.type, note.startButtonPos, note.child[0].slideData.targetNote) / (float)Math.PI;
				float num3 = note.child[0].slideData.arrive.time.msec - note.child[0].slideData.shoot.time.msec;
				num = num2 / num3 * 15f;
			}
			if (num >= 18f)
			{
				num = 18f;
			}
			StarRotateSpeed = -1f * num;
			if (Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.StarRotate == OptionStarrotateID.Off)
			{
				StarRotateSpeed = 0f;
			}
			SetEach(note.isEach, note.child.Count >= 2);
			SpriteRenderEx.sprite = (MultiSlide ? GameNoteImageContainer.ExDoubleStar[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.SlideDesign] : GameNoteImageContainer.ExStar[(int)Singleton<GamePlayManager>.Instance.GetGameScore(base.MonitorId).UserOption.SlideDesign]);
			ExObj.SetActive(base.ExNote);
			JudgeType = (base.ExNote ? NoteJudge.EJudgeType.ExTap : NoteJudge.EJudgeType.Tap);
		}

		private void Start()
		{
			NoteKind = NoteTypeID.Def.Star;
		}

		public override void Execute()
		{
			if (!EndFlag)
			{
				NoteObj.transform.localPosition = new Vector3(0f, GetNoteYPosition(), GetBaseZPosition());
				NoteObj.transform.Rotate(0f, 0f, StarRotateSpeed * GameManager.GetGameFrameAdd());
			}
			NoteCheck();
		}
	}
}
