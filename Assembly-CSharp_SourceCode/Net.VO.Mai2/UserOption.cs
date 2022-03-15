using System;
using DB;

namespace Net.VO.Mai2
{
	[Serializable]
	public struct UserOption
	{
		public OptionKindID optionKind;

		public OptionNotespeedID noteSpeed;

		public OptionSlidespeedID slideSpeed;

		public OptionTouchspeedID touchSpeed;

		public OptionGametapID tapDesign;

		public OptionGameholdID holdDesign;

		public OptionGameslideID slideDesign;

		public OptionStartypeID starType;

		public OptionGameoutlineID outlineDesign;

		public OptionNotesizeID noteSize;

		public OptionSlidesizeID slideSize;

		public OptionTouchsizeID touchSize;

		public OptionStarrotateID starRotate;

		public OptionCenterdisplayID dispCenter;

		public OptionDispchainID dispChain;

		public OptionDisprateID dispRate;

		public OptionDispbarlineID dispBar;

		public OptionToucheffectID touchEffect;

		public OptionSubmonitorID submonitorAnimation;

		public OptionSubmonAchiveID submonitorAchive;

		public OptionAppealID submonitorAppeal;

		public OptionMatchingID matching;

		public OptionTrackskipID trackSkip;

		public OptionMoviebrightnessID brightness;

		public OptionMirrorID mirrorMode;

		public OptionDispjudgeID dispJudge;

		public OptionDispjudgeposID dispJudgePos;

		public OptionDispjudgetouchposID dispJudgeTouchPos;

		public OptionJudgetimingID adjustTiming;

		public OptionJudgetimingID judgeTiming;

		public OptionVolumeID ansVolume;

		public OptionVolumeID tapHoldVolume;

		public OptionCriticalID criticalSe;

		public OptionBreakseID breakSe;

		public OptionVolumeID breakVolume;

		public OptionExseID exSe;

		public OptionVolumeID exVolume;

		public OptionSlideseID slideSe;

		public OptionVolumeID slideVolume;

		public OptionVolumeID touchHoldVolume;

		public OptionVolumeID damageSeVolume;

		public OptionHeadphonevolumeID headPhoneVolume;

		public SortTabID sortTab;

		public SortMusicID sortMusic;
	}
}
