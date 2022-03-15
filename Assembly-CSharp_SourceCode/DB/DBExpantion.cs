using System;
using Mai2.Mai2Cue;

namespace DB
{
	public static class DBExpantion
	{
		public static OptionNotespeedID GetDefault(this OptionNotespeedID self)
		{
			OptionNotespeedID[] array = (OptionNotespeedID[])Enum.GetValues(typeof(OptionNotespeedID));
			foreach (OptionNotespeedID optionNotespeedID in array)
			{
				if (optionNotespeedID.IsDefault())
				{
					return optionNotespeedID;
				}
			}
			return OptionNotespeedID.Invalid;
		}

		public static OptionSlidespeedID GetDefault(this OptionSlidespeedID self)
		{
			OptionSlidespeedID[] array = (OptionSlidespeedID[])Enum.GetValues(typeof(OptionSlidespeedID));
			foreach (OptionSlidespeedID optionSlidespeedID in array)
			{
				if (optionSlidespeedID.IsDefault())
				{
					return optionSlidespeedID;
				}
			}
			return OptionSlidespeedID.Invalid;
		}

		public static OptionTouchspeedID GetDefault(this OptionTouchspeedID self)
		{
			OptionTouchspeedID[] array = (OptionTouchspeedID[])Enum.GetValues(typeof(OptionTouchspeedID));
			foreach (OptionTouchspeedID optionTouchspeedID in array)
			{
				if (optionTouchspeedID.IsDefault())
				{
					return optionTouchspeedID;
				}
			}
			return OptionTouchspeedID.Invalid;
		}

		public static OptionStarrotateID GetDefault(this OptionStarrotateID self)
		{
			OptionStarrotateID[] array = (OptionStarrotateID[])Enum.GetValues(typeof(OptionStarrotateID));
			foreach (OptionStarrotateID optionStarrotateID in array)
			{
				if (optionStarrotateID.IsDefault())
				{
					return optionStarrotateID;
				}
			}
			return OptionStarrotateID.Invalid;
		}

		public static OptionGameoutlineID GetDefault(this OptionGameoutlineID self)
		{
			OptionGameoutlineID[] array = (OptionGameoutlineID[])Enum.GetValues(typeof(OptionGameoutlineID));
			foreach (OptionGameoutlineID optionGameoutlineID in array)
			{
				if (optionGameoutlineID.IsDefault())
				{
					return optionGameoutlineID;
				}
			}
			return OptionGameoutlineID.Invalid;
		}

		public static OptionVolumeID GetDefault(this OptionVolumeID self)
		{
			OptionVolumeID[] array = (OptionVolumeID[])Enum.GetValues(typeof(OptionVolumeID));
			foreach (OptionVolumeID optionVolumeID in array)
			{
				if (optionVolumeID.IsDefault())
				{
					return optionVolumeID;
				}
			}
			return OptionVolumeID.Invalid;
		}

		public static OptionCenterdisplayID GetDefault(this OptionCenterdisplayID self)
		{
			OptionCenterdisplayID[] array = (OptionCenterdisplayID[])Enum.GetValues(typeof(OptionCenterdisplayID));
			foreach (OptionCenterdisplayID optionCenterdisplayID in array)
			{
				if (optionCenterdisplayID.IsDefault())
				{
					return optionCenterdisplayID;
				}
			}
			return OptionCenterdisplayID.Invalid;
		}

		public static OptionDispjudgeID GetDefault(this OptionDispjudgeID self)
		{
			OptionDispjudgeID[] array = (OptionDispjudgeID[])Enum.GetValues(typeof(OptionDispjudgeID));
			foreach (OptionDispjudgeID optionDispjudgeID in array)
			{
				if (optionDispjudgeID.IsDefault())
				{
					return optionDispjudgeID;
				}
			}
			return OptionDispjudgeID.Invalid;
		}

		public static OptionMirrorID GetDefault(this OptionMirrorID self)
		{
			OptionMirrorID[] array = (OptionMirrorID[])Enum.GetValues(typeof(OptionMirrorID));
			foreach (OptionMirrorID optionMirrorID in array)
			{
				if (optionMirrorID.IsDefault())
				{
					return optionMirrorID;
				}
			}
			return OptionMirrorID.Invalid;
		}

		public static OptionTrackskipID GetDefault(this OptionTrackskipID self)
		{
			OptionTrackskipID[] array = (OptionTrackskipID[])Enum.GetValues(typeof(OptionTrackskipID));
			foreach (OptionTrackskipID optionTrackskipID in array)
			{
				if (optionTrackskipID.IsDefault())
				{
					return optionTrackskipID;
				}
			}
			return OptionTrackskipID.Invalid;
		}

		public static OptionJudgetimingID GetDefault(this OptionJudgetimingID self)
		{
			OptionJudgetimingID[] array = (OptionJudgetimingID[])Enum.GetValues(typeof(OptionJudgetimingID));
			foreach (OptionJudgetimingID optionJudgetimingID in array)
			{
				if (optionJudgetimingID.IsDefault())
				{
					return optionJudgetimingID;
				}
			}
			return OptionJudgetimingID.Invalid;
		}

		public static OptionBreakseID GetDefault(this OptionBreakseID self)
		{
			OptionBreakseID[] array = (OptionBreakseID[])Enum.GetValues(typeof(OptionBreakseID));
			foreach (OptionBreakseID optionBreakseID in array)
			{
				if (optionBreakseID.IsDefault())
				{
					return optionBreakseID;
				}
			}
			return OptionBreakseID.Invalid;
		}

		public static OptionSlideseID GetDefault(this OptionSlideseID self)
		{
			OptionSlideseID[] array = (OptionSlideseID[])Enum.GetValues(typeof(OptionSlideseID));
			foreach (OptionSlideseID optionSlideseID in array)
			{
				if (optionSlideseID.IsDefault())
				{
					return optionSlideseID;
				}
			}
			return OptionSlideseID.Invalid;
		}

		public static OptionExseID GetDefault(this OptionExseID self)
		{
			OptionExseID[] array = (OptionExseID[])Enum.GetValues(typeof(OptionExseID));
			foreach (OptionExseID optionExseID in array)
			{
				if (optionExseID.IsDefault())
				{
					return optionExseID;
				}
			}
			return OptionExseID.Invalid;
		}

		public static OptionCriticalID GetDefault(this OptionCriticalID self)
		{
			OptionCriticalID[] array = (OptionCriticalID[])Enum.GetValues(typeof(OptionCriticalID));
			foreach (OptionCriticalID optionCriticalID in array)
			{
				if (optionCriticalID.IsDefault())
				{
					return optionCriticalID;
				}
			}
			return OptionCriticalID.Invalid;
		}

		public static OptionMoviebrightnessID GetDefault(this OptionMoviebrightnessID self)
		{
			OptionMoviebrightnessID[] array = (OptionMoviebrightnessID[])Enum.GetValues(typeof(OptionMoviebrightnessID));
			foreach (OptionMoviebrightnessID optionMoviebrightnessID in array)
			{
				if (optionMoviebrightnessID.IsDefault())
				{
					return optionMoviebrightnessID;
				}
			}
			return OptionMoviebrightnessID.Invalid;
		}

		public static OptionNotesizeID GetDefault(this OptionNotesizeID self)
		{
			OptionNotesizeID[] array = (OptionNotesizeID[])Enum.GetValues(typeof(OptionNotesizeID));
			foreach (OptionNotesizeID optionNotesizeID in array)
			{
				if (optionNotesizeID.IsDefault())
				{
					return optionNotesizeID;
				}
			}
			return OptionNotesizeID.Invalid;
		}

		public static OptionSlidesizeID GetDefault(this OptionSlidesizeID self)
		{
			OptionSlidesizeID[] array = (OptionSlidesizeID[])Enum.GetValues(typeof(OptionSlidesizeID));
			foreach (OptionSlidesizeID optionSlidesizeID in array)
			{
				if (optionSlidesizeID.IsDefault())
				{
					return optionSlidesizeID;
				}
			}
			return OptionSlidesizeID.Invalid;
		}

		public static OptionTouchsizeID GetDefault(this OptionTouchsizeID self)
		{
			OptionTouchsizeID[] array = (OptionTouchsizeID[])Enum.GetValues(typeof(OptionTouchsizeID));
			foreach (OptionTouchsizeID optionTouchsizeID in array)
			{
				if (optionTouchsizeID.IsDefault())
				{
					return optionTouchsizeID;
				}
			}
			return OptionTouchsizeID.Invalid;
		}

		public static OptionDispchainID GetDefault(this OptionDispchainID self)
		{
			OptionDispchainID[] array = (OptionDispchainID[])Enum.GetValues(typeof(OptionDispchainID));
			foreach (OptionDispchainID optionDispchainID in array)
			{
				if (optionDispchainID.IsDefault())
				{
					return optionDispchainID;
				}
			}
			return OptionDispchainID.Invalid;
		}

		public static OptionGametapID GetDefault(this OptionGametapID self)
		{
			OptionGametapID[] array = (OptionGametapID[])Enum.GetValues(typeof(OptionGametapID));
			foreach (OptionGametapID optionGametapID in array)
			{
				if (optionGametapID.IsDefault())
				{
					return optionGametapID;
				}
			}
			return OptionGametapID.Invalid;
		}

		public static OptionGameholdID GetDefault(this OptionGameholdID self)
		{
			OptionGameholdID[] array = (OptionGameholdID[])Enum.GetValues(typeof(OptionGameholdID));
			foreach (OptionGameholdID optionGameholdID in array)
			{
				if (optionGameholdID.IsDefault())
				{
					return optionGameholdID;
				}
			}
			return OptionGameholdID.Invalid;
		}

		public static OptionGameslideID GetDefault(this OptionGameslideID self)
		{
			OptionGameslideID[] array = (OptionGameslideID[])Enum.GetValues(typeof(OptionGameslideID));
			foreach (OptionGameslideID optionGameslideID in array)
			{
				if (optionGameslideID.IsDefault())
				{
					return optionGameslideID;
				}
			}
			return OptionGameslideID.Invalid;
		}

		public static OptionDisprateID GetDefault(this OptionDisprateID self)
		{
			OptionDisprateID[] array = (OptionDisprateID[])Enum.GetValues(typeof(OptionDisprateID));
			foreach (OptionDisprateID optionDisprateID in array)
			{
				if (optionDisprateID.IsDefault())
				{
					return optionDisprateID;
				}
			}
			return OptionDisprateID.Invalid;
		}

		public static OptionStartypeID GetDefault(this OptionStartypeID self)
		{
			OptionStartypeID[] array = (OptionStartypeID[])Enum.GetValues(typeof(OptionStartypeID));
			foreach (OptionStartypeID optionStartypeID in array)
			{
				if (optionStartypeID.IsDefault())
				{
					return optionStartypeID;
				}
			}
			return OptionStartypeID.Invalid;
		}

		public static OptionDispbarlineID GetDefault(this OptionDispbarlineID self)
		{
			OptionDispbarlineID[] array = (OptionDispbarlineID[])Enum.GetValues(typeof(OptionDispbarlineID));
			foreach (OptionDispbarlineID optionDispbarlineID in array)
			{
				if (optionDispbarlineID.IsDefault())
				{
					return optionDispbarlineID;
				}
			}
			return OptionDispbarlineID.Invalid;
		}

		public static OptionDispjudgeposID GetDefault(this OptionDispjudgeposID self)
		{
			OptionDispjudgeposID[] array = (OptionDispjudgeposID[])Enum.GetValues(typeof(OptionDispjudgeposID));
			foreach (OptionDispjudgeposID optionDispjudgeposID in array)
			{
				if (optionDispjudgeposID.IsDefault())
				{
					return optionDispjudgeposID;
				}
			}
			return OptionDispjudgeposID.Invalid;
		}

		public static OptionDispjudgetouchposID GetDefault(this OptionDispjudgetouchposID self)
		{
			OptionDispjudgetouchposID[] array = (OptionDispjudgetouchposID[])Enum.GetValues(typeof(OptionDispjudgetouchposID));
			foreach (OptionDispjudgetouchposID optionDispjudgetouchposID in array)
			{
				if (optionDispjudgetouchposID.IsDefault())
				{
					return optionDispjudgetouchposID;
				}
			}
			return OptionDispjudgetouchposID.Invalid;
		}

		public static OptionToucheffectID GetDefault(this OptionToucheffectID self)
		{
			OptionToucheffectID[] array = (OptionToucheffectID[])Enum.GetValues(typeof(OptionToucheffectID));
			foreach (OptionToucheffectID optionToucheffectID in array)
			{
				if (optionToucheffectID.IsDefault())
				{
					return optionToucheffectID;
				}
			}
			return OptionToucheffectID.Invalid;
		}

		public static OptionSubmonitorID GetDefault(this OptionSubmonitorID self)
		{
			OptionSubmonitorID[] array = (OptionSubmonitorID[])Enum.GetValues(typeof(OptionSubmonitorID));
			foreach (OptionSubmonitorID optionSubmonitorID in array)
			{
				if (optionSubmonitorID.IsDefault())
				{
					return optionSubmonitorID;
				}
			}
			return OptionSubmonitorID.Invalid;
		}

		public static OptionSubmonAchiveID GetDefault(this OptionSubmonAchiveID self)
		{
			OptionSubmonAchiveID[] array = (OptionSubmonAchiveID[])Enum.GetValues(typeof(OptionSubmonAchiveID));
			foreach (OptionSubmonAchiveID optionSubmonAchiveID in array)
			{
				if (optionSubmonAchiveID.IsDefault())
				{
					return optionSubmonAchiveID;
				}
			}
			return OptionSubmonAchiveID.Invalid;
		}

		public static OptionAppealID GetDefault(this OptionAppealID self)
		{
			OptionAppealID[] array = (OptionAppealID[])Enum.GetValues(typeof(OptionAppealID));
			foreach (OptionAppealID optionAppealID in array)
			{
				if (optionAppealID.IsDefault())
				{
					return optionAppealID;
				}
			}
			return OptionAppealID.Invalid;
		}

		public static OptionHeadphonevolumeID GetDefault(this OptionHeadphonevolumeID self)
		{
			OptionHeadphonevolumeID[] array = (OptionHeadphonevolumeID[])Enum.GetValues(typeof(OptionHeadphonevolumeID));
			foreach (OptionHeadphonevolumeID optionHeadphonevolumeID in array)
			{
				if (optionHeadphonevolumeID.IsDefault())
				{
					return optionHeadphonevolumeID;
				}
			}
			return OptionHeadphonevolumeID.Invalid;
		}

		public static OptionBodybrightnessID GetDefault(this OptionBodybrightnessID self)
		{
			OptionBodybrightnessID[] array = (OptionBodybrightnessID[])Enum.GetValues(typeof(OptionBodybrightnessID));
			foreach (OptionBodybrightnessID optionBodybrightnessID in array)
			{
				if (optionBodybrightnessID.IsDefault())
				{
					return optionBodybrightnessID;
				}
			}
			return OptionBodybrightnessID.Invalid;
		}

		public static Cue GetBreakGoodCue(this OptionBreakseID self)
		{
			return (Cue)Enum.Parse(typeof(Cue), self.GetSeGoodEnum());
		}

		public static Cue GetBreakBadCue(this OptionBreakseID self)
		{
			return (Cue)Enum.Parse(typeof(Cue), self.GetSeBadEnum());
		}

		public static Cue GetSlideCue(this OptionSlideseID self)
		{
			return (Cue)Enum.Parse(typeof(Cue), self.GetSeEnum());
		}

		public static Cue GetExCue(this OptionExseID self)
		{
			return (Cue)Enum.Parse(typeof(Cue), self.GetSeEnum());
		}
	}
}
