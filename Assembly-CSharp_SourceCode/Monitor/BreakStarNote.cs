using System;
using DB;
using MAI2.Util;
using Manager;
using Process;
using UnityEngine;

namespace Monitor
{
	public class BreakStarNote : BreakNote
	{
		public float StarRotateSpeed { get; set; }

		protected override void SetEach(bool eachFlag)
		{
			EachFlag = eachFlag;
		}

		public void SetMulti(bool multiFlag, bool eachFlag)
		{
			SpriteRender.sprite = (multiFlag ? GameNoteImageContainer.BreakDoubleStar[(int)Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.SlideDesign] : GameNoteImageContainer.BreakStar[(int)Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.SlideDesign]);
			EffectSprite.sprite = (multiFlag ? GameNoteImageContainer.BreakDoubleStarEff[(int)Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.SlideDesign] : GameNoteImageContainer.BreakStarEff[(int)Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.SlideDesign]);
			if (GuideObj != null)
			{
				GuideObj.SetColor(NoteGuide.Color.Break);
			}
			EachFlag = eachFlag;
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
			if (StarRotateSpeed >= 18f)
			{
				StarRotateSpeed = 18f;
			}
			StarRotateSpeed = -1f * num;
			if (Singleton<GamePlayManager>.Instance.GetGameScore(MonitorIndex).UserOption.StarRotate == OptionStarrotateID.Off)
			{
				StarRotateSpeed = 0f;
			}
			SetMulti(note.child.Count >= 2, EachFlag);
		}

		private void Start()
		{
			NoteKind = NoteTypeID.Def.BreakStar;
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
