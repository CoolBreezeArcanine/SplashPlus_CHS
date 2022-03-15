using System;
using DB;
using Manager.UserDatas;

namespace Datas.DebugData
{
	[Serializable]
	public class DebugUserOption
	{
		public OptionKindID OptionKind;

		public OptionNotespeedID NoteSpeed = OptionNotespeedID.Speed2_0;

		public OptionSlidespeedID SlideSpeed = OptionSlidespeedID.Normal;

		public OptionTouchspeedID TouchSpeed = OptionTouchspeedID.Speed4_0;

		public OptionStarrotateID StarRotate = OptionStarrotateID.On;

		public OptionJudgetimingID AdjustTiming = OptionJudgetimingID.Normal;

		public OptionJudgetimingID JudgeTiming = OptionJudgetimingID.Normal;

		public OptionDispjudgeID DispJudge;

		public OptionMoviebrightnessID Brightness;

		public OptionSubmonitorID SubmonitorAnimation;

		public OptionMirrorID MirrorMode;

		public OptionVolumeID SeVolume = OptionVolumeID.Vol4;

		public OptionVolumeID BreakVolume = OptionVolumeID.Vol4;

		public OptionVolumeID SlideVolume = OptionVolumeID.Vol4;

		public OptionVolumeID AnsVolume = OptionVolumeID.Vol4;

		public OptionVolumeID TapHoldVolume = OptionVolumeID.Vol4;

		public OptionHeadphonevolumeID HeadPhoneVolume = OptionHeadphonevolumeID.Vol10;

		public UserOption GetUserOption()
		{
			UserOption userOption = new UserOption();
			userOption.Initialize();
			userOption.OptionKind = OptionKind;
			userOption.NoteSpeed = NoteSpeed;
			userOption.SlideSpeed = SlideSpeed;
			userOption.TouchSpeed = TouchSpeed;
			userOption.StarRotate = StarRotate;
			userOption.AdjustTiming = AdjustTiming;
			userOption.JudgeTiming = JudgeTiming;
			userOption.DispJudge = DispJudge;
			userOption.Brightness = Brightness;
			userOption.SubmonitorAnimation = SubmonitorAnimation;
			userOption.BreakVolume = BreakVolume;
			userOption.SlideVolume = SlideVolume;
			userOption.AnsVolume = AnsVolume;
			userOption.TapHoldVolume = TapHoldVolume;
			userOption.HeadPhoneVolume = HeadPhoneVolume;
			return userOption;
		}
	}
}
