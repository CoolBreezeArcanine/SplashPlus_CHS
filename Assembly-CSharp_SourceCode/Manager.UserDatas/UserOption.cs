using System;
using DB;

namespace Manager.UserDatas
{
	public class UserOption
	{
		public class OptionData
		{
			public OptionCategoryID Category;

			public string Value;
		}

		private class UserOptionSimpleBase
		{
			public OptionNotespeedID noteSpeed { get; set; }

			public OptionTouchspeedID touchSpeed { get; set; }
		}

		private static readonly string DefaultTag = OptionRootID.DefaultColorTag.GetName();

		private static readonly string NormalTag = OptionRootID.NormalColorTag.GetName();

		public static Type[] OptionTypes = new Type[5]
		{
			typeof(OptionCateSpeedID),
			typeof(OptionCateGameID),
			typeof(OptionCateJudgeID),
			typeof(OptionCateDesignID),
			typeof(OptionCateSoundID)
		};

		private UserOptionSimpleBase[] OptionBase = new UserOptionSimpleBase[3]
		{
			new UserOptionSimpleBase
			{
				noteSpeed = OptionNotespeedID.Speed2_0,
				touchSpeed = OptionTouchspeedID.Speed2_0
			},
			new UserOptionSimpleBase
			{
				noteSpeed = OptionNotespeedID.Speed3_0,
				touchSpeed = OptionTouchspeedID.Speed3_0
			},
			new UserOptionSimpleBase
			{
				noteSpeed = OptionNotespeedID.Speed4_5,
				touchSpeed = OptionTouchspeedID.Speed4_5
			}
		};

		public const OptionKindID DEFAULT_OPTION_KIND = OptionKindID.Basic;

		public static string[][] Text = new string[5][]
		{
			new string[3]
			{
				OptionCateSpeedID.NoteSpeed.GetName(),
				OptionCateSpeedID.TouchSpeed.GetName(),
				OptionCateSpeedID.SlideSpeed.GetName()
			},
			new string[6]
			{
				OptionCateGameID.TrackSkip.GetName(),
				OptionCateGameID.Mirror.GetName(),
				OptionCateGameID.StarRotate.GetName(),
				OptionCateGameID.AdjustTiming.GetName(),
				OptionCateGameID.JudgeTiming.GetName(),
				OptionCateGameID.Brightness.GetName()
			},
			new string[8]
			{
				OptionCateJudgeID.DispCenter.GetName(),
				OptionCateJudgeID.DispJudge.GetName(),
				OptionCateJudgeID.DispJudgeTapPos.GetName(),
				OptionCateJudgeID.DispJudgeTouchPos.GetName(),
				OptionCateJudgeID.ChainDisp.GetName(),
				OptionCateJudgeID.SubMonitor_Achive.GetName(),
				OptionCateJudgeID.RatingDisp.GetName(),
				OptionCateJudgeID.SubMonitor_Appeal.GetName()
			},
			new string[5]
			{
				OptionCateDesignID.TapDesign.GetName(),
				OptionCateDesignID.HoldDesign.GetName(),
				OptionCateDesignID.SlideDesign.GetName(),
				OptionCateDesignID.StarDesign.GetName(),
				OptionCateDesignID.OutlineDesign.GetName()
			},
			new string[11]
			{
				OptionCateSoundID.Ans_Vol.GetName(),
				OptionCateSoundID.Tap_Se.GetName(),
				OptionCateSoundID.Tap_Vol.GetName(),
				OptionCateSoundID.Break_Se.GetName(),
				OptionCateSoundID.Break_Vol.GetName(),
				OptionCateSoundID.Ex_Se.GetName(),
				OptionCateSoundID.Ex_Vol.GetName(),
				OptionCateSoundID.Slide_Se.GetName(),
				OptionCateSoundID.Slide_Vol.GetName(),
				OptionCateSoundID.TouchHold_Vol.GetName(),
				OptionCateSoundID.DamageSe_Vol.GetName()
			}
		};

		public OptionKindID OptionKind { get; set; }

		public OptionNotespeedID NoteSpeed { get; set; }

		public OptionSlidespeedID SlideSpeed { get; set; }

		public OptionTouchspeedID TouchSpeed { get; set; }

		public OptionNotespeedID GetNoteSpeed
		{
			get
			{
				if (OptionKindID.Custom != OptionKind)
				{
					return OptionBase[(int)OptionKind].noteSpeed;
				}
				return NoteSpeed;
			}
		}

		public OptionSlidespeedID GetSlideSpeed => SlideSpeed;

		public OptionTouchspeedID GetTouchSpeed
		{
			get
			{
				if (OptionKindID.Custom != OptionKind)
				{
					return OptionBase[(int)OptionKind].touchSpeed;
				}
				return TouchSpeed;
			}
		}

		public OptionGametapID TapDesign { get; set; }

		public OptionGameholdID HoldDesign { get; set; }

		public OptionGameslideID SlideDesign { get; set; }

		public OptionStartypeID StarType { get; set; }

		public OptionGameoutlineID OutlineDesign { get; set; }

		public OptionNotesizeID NoteSize { get; set; }

		public OptionSlidesizeID SlideSize { get; set; }

		public OptionTouchsizeID TouchSize { get; set; }

		public OptionStarrotateID StarRotate { get; set; }

		public OptionCenterdisplayID DispCenter { get; set; }

		public OptionDispchainID DispChain { get; set; }

		public OptionDisprateID DispRate { get; set; }

		public OptionDispbarlineID BarDisp { get; set; }

		public OptionToucheffectID TouchEffect { get; set; }

		public OptionSubmonitorID SubmonitorAnimation { get; set; }

		public OptionSubmonAchiveID SubmonitorAchive { get; set; }

		public OptionAppealID SubmonitorAppeal { get; set; }

		public OptionMatchingID Matching { get; set; }

		public OptionTrackskipID TrackSkip { get; set; }

		public OptionMoviebrightnessID Brightness { get; set; }

		public OptionMirrorID MirrorMode { get; set; }

		public OptionDispjudgeID DispJudge { get; set; }

		public OptionDispjudgeposID DispJudgePos { get; set; }

		public OptionDispjudgetouchposID DispJudgeTouchPos { get; set; }

		public OptionJudgetimingID AdjustTiming { get; set; }

		public OptionJudgetimingID JudgeTiming { get; set; }

		public OptionVolumeID AnsVolume { get; set; }

		public OptionVolumeID TapHoldVolume { get; set; }

		public OptionCriticalID CriticalSe { get; set; }

		public OptionBreakseID BreakSe { get; set; }

		public OptionVolumeID BreakVolume { get; set; }

		public OptionExseID ExSe { get; set; }

		public OptionVolumeID ExVolume { get; set; }

		public OptionSlideseID SlideSe { get; set; }

		public OptionVolumeID SlideVolume { get; set; }

		public OptionVolumeID TouchHoldVolume { get; set; }

		public OptionVolumeID DamageSeVolume { get; set; }

		public OptionHeadphonevolumeID HeadPhoneVolume { get; set; }

		public string GetOptionName(OptionCategoryID category, int optionIndex)
		{
			string result = "";
			switch (category)
			{
			case OptionCategoryID.GameSetting:
				if (optionIndex < 7)
				{
					result = ((OptionCateGameID)optionIndex).GetName();
				}
				break;
			case OptionCategoryID.SoundSetting:
				if (optionIndex < 11)
				{
					result = ((OptionCateSoundID)optionIndex).GetName();
				}
				break;
			case OptionCategoryID.JudgeSetting:
				if (optionIndex < 8)
				{
					result = ((OptionCateJudgeID)optionIndex).GetName();
				}
				break;
			case OptionCategoryID.DesignSetting:
				if (optionIndex < 5)
				{
					result = ((OptionCateDesignID)optionIndex).GetName();
				}
				break;
			case OptionCategoryID.SpeedSetting:
				if (optionIndex < 3)
				{
					result = ((OptionCateSpeedID)optionIndex).GetName();
				}
				break;
			}
			return result;
		}

		public string GetOptionDetail(OptionCategoryID category, int optionIndex)
		{
			string result = "";
			switch (category)
			{
			case OptionCategoryID.GameSetting:
				if (optionIndex < 7)
				{
					result = ((OptionCateGameID)optionIndex).GetDetail();
				}
				break;
			case OptionCategoryID.SoundSetting:
				if (optionIndex < 11)
				{
					result = ((OptionCateSoundID)optionIndex).GetDetail();
				}
				break;
			case OptionCategoryID.SpeedSetting:
				if (optionIndex < 3)
				{
					result = ((OptionCateSpeedID)optionIndex).GetDetail();
				}
				break;
			case OptionCategoryID.DesignSetting:
				if (optionIndex < 5)
				{
					result = ((OptionCateDesignID)optionIndex).GetDetail();
				}
				break;
			case OptionCategoryID.JudgeSetting:
				if (optionIndex < 8)
				{
					result = ((OptionCateJudgeID)optionIndex).GetDetail();
				}
				break;
			}
			return result;
		}

		public string GetOptionValue(OptionCategoryID category, int optionIndex)
		{
			string result = "";
			switch (category)
			{
			case OptionCategoryID.SpeedSetting:
				switch (optionIndex)
				{
				case 0:
					result = (NoteSpeed.IsDefault() ? DefaultTag : NormalTag) + NoteSpeed.GetName();
					break;
				case 1:
					result = (TouchSpeed.IsDefault() ? DefaultTag : NormalTag) + TouchSpeed.GetName();
					break;
				case 2:
					result = (SlideSpeed.IsDefault() ? DefaultTag : NormalTag) + SlideSpeed.GetName();
					break;
				}
				break;
			case OptionCategoryID.DesignSetting:
				switch (optionIndex)
				{
				case 0:
					result = (TapDesign.IsDefault() ? DefaultTag : NormalTag) + TapDesign.GetName();
					break;
				case 1:
					result = (HoldDesign.IsDefault() ? DefaultTag : NormalTag) + HoldDesign.GetName();
					break;
				case 2:
					result = (SlideDesign.IsDefault() ? DefaultTag : NormalTag) + SlideDesign.GetName();
					break;
				case 3:
					result = (StarType.IsDefault() ? DefaultTag : NormalTag) + StarType.GetName();
					break;
				case 4:
					result = (OutlineDesign.IsDefault() ? DefaultTag : NormalTag) + OutlineDesign.GetName();
					break;
				}
				break;
			case OptionCategoryID.GameSetting:
				switch (optionIndex)
				{
				case 0:
					result = (TrackSkip.IsDefault() ? DefaultTag : NormalTag) + TrackSkip.GetName();
					break;
				case 1:
					result = (MirrorMode.IsDefault() ? DefaultTag : NormalTag) + MirrorMode.GetName();
					break;
				case 3:
					result = (AdjustTiming.IsDefault() ? DefaultTag : NormalTag) + AdjustTiming.GetName();
					break;
				case 4:
					result = (JudgeTiming.IsDefault() ? DefaultTag : NormalTag) + JudgeTiming.GetName();
					break;
				case 5:
					result = (Brightness.IsDefault() ? DefaultTag : NormalTag) + Brightness.GetName();
					break;
				case 6:
					result = (TouchEffect.IsDefault() ? DefaultTag : NormalTag) + TouchEffect.GetName();
					break;
				case 2:
					result = (StarRotate.IsDefault() ? DefaultTag : NormalTag) + StarRotate.GetName();
					break;
				}
				break;
			case OptionCategoryID.SoundSetting:
				switch (optionIndex)
				{
				case 0:
					result = (AnsVolume.IsDefault() ? DefaultTag : NormalTag) + AnsVolume.GetName();
					break;
				case 2:
					result = (TapHoldVolume.IsDefault() ? DefaultTag : NormalTag) + TapHoldVolume.GetName();
					break;
				case 1:
					result = (CriticalSe.IsDefault() ? DefaultTag : NormalTag) + CriticalSe.GetName();
					break;
				case 3:
					result = (BreakSe.IsDefault() ? DefaultTag : NormalTag) + BreakSe.GetName();
					break;
				case 4:
					result = (BreakVolume.IsDefault() ? DefaultTag : NormalTag) + BreakVolume.GetName();
					break;
				case 5:
					result = (ExSe.IsDefault() ? DefaultTag : NormalTag) + ExSe.GetName();
					break;
				case 6:
					result = (ExVolume.IsDefault() ? DefaultTag : NormalTag) + ExVolume.GetName();
					break;
				case 7:
					result = (SlideSe.IsDefault() ? DefaultTag : NormalTag) + SlideSe.GetName();
					break;
				case 8:
					result = (SlideVolume.IsDefault() ? DefaultTag : NormalTag) + SlideVolume.GetName();
					break;
				case 9:
					result = (TouchHoldVolume.IsDefault() ? DefaultTag : NormalTag) + TouchHoldVolume.GetName();
					break;
				case 10:
					result = (DamageSeVolume.IsDefault() ? DefaultTag : NormalTag) + DamageSeVolume.GetName();
					break;
				}
				break;
			case OptionCategoryID.JudgeSetting:
				switch (optionIndex)
				{
				case 1:
					result = (DispJudge.IsDefault() ? DefaultTag : NormalTag) + DispJudge.GetName();
					break;
				case 2:
					result = (DispJudgePos.IsDefault() ? DefaultTag : NormalTag) + DispJudgePos.GetName();
					break;
				case 3:
					result = (DispJudgeTouchPos.IsDefault() ? DefaultTag : NormalTag) + DispJudgeTouchPos.GetName();
					break;
				case 0:
					result = (DispCenter.IsDefault() ? DefaultTag : NormalTag) + DispCenter.GetName();
					break;
				case 4:
					result = (DispChain.IsDefault() ? DefaultTag : NormalTag) + DispChain.GetName();
					break;
				case 6:
					result = (DispRate.IsDefault() ? DefaultTag : NormalTag) + DispRate.GetName();
					break;
				case 5:
					result = (SubmonitorAchive.IsDefault() ? DefaultTag : NormalTag) + SubmonitorAchive.GetName();
					break;
				case 7:
					result = (SubmonitorAppeal.IsDefault() ? DefaultTag : NormalTag) + SubmonitorAppeal.GetName();
					break;
				}
				break;
			}
			return result;
		}

		public void AddOption(OptionCategoryID category, int currentOptionIndex)
		{
			switch (category)
			{
			case OptionCategoryID.SpeedSetting:
				switch (currentOptionIndex)
				{
				case 0:
					NoteSpeed = ((NoteSpeed + 1 >= OptionNotespeedID.End) ? OptionNotespeedID.Speed_Sonic : (NoteSpeed + 1));
					break;
				case 1:
					TouchSpeed = ((TouchSpeed + 1 >= OptionTouchspeedID.End) ? OptionTouchspeedID.Speed_Sonic : (TouchSpeed + 1));
					break;
				case 2:
					SlideSpeed = ((SlideSpeed + 1 >= OptionSlidespeedID.End) ? OptionSlidespeedID.Late1_0 : (SlideSpeed + 1));
					break;
				}
				break;
			case OptionCategoryID.DesignSetting:
				switch (currentOptionIndex)
				{
				case 0:
					TapDesign = ((TapDesign + 1 >= OptionGametapID.End) ? OptionGametapID.Any : (TapDesign + 1));
					break;
				case 1:
					HoldDesign = ((HoldDesign + 1 >= OptionGameholdID.End) ? OptionGameholdID.Legacy : (HoldDesign + 1));
					break;
				case 2:
					SlideDesign = ((SlideDesign + 1 >= OptionGameslideID.End) ? OptionGameslideID.Legacy : (SlideDesign + 1));
					break;
				case 3:
					StarType = ((StarType + 1 >= OptionStartypeID.End) ? OptionStartypeID.Pink : (StarType + 1));
					break;
				case 4:
					OutlineDesign = ((OutlineDesign + 1 >= OptionGameoutlineID.End) ? OptionGameoutlineID.Splash : (OutlineDesign + 1));
					break;
				}
				break;
			case OptionCategoryID.GameSetting:
				switch (currentOptionIndex)
				{
				case 0:
					TrackSkip = ((TrackSkip + 1 >= OptionTrackskipID.End) ? OptionTrackskipID.AutoBest : (TrackSkip + 1));
					break;
				case 1:
					MirrorMode = ((MirrorMode + 1 >= OptionMirrorID.End) ? OptionMirrorID.UDLR : (MirrorMode + 1));
					break;
				case 3:
					AdjustTiming = ((AdjustTiming + 1 >= OptionJudgetimingID.End) ? OptionJudgetimingID.Late2_0 : (AdjustTiming + 1));
					break;
				case 4:
					JudgeTiming = ((JudgeTiming + 1 >= OptionJudgetimingID.End) ? OptionJudgetimingID.Late2_0 : (JudgeTiming + 1));
					break;
				case 5:
					Brightness = ((Brightness + 1 >= OptionMoviebrightnessID.End) ? OptionMoviebrightnessID.Bright_3 : (Brightness + 1));
					break;
				case 6:
					TouchEffect = ((TouchEffect + 1 >= OptionToucheffectID.End) ? OptionToucheffectID.On : (TouchEffect + 1));
					break;
				case 2:
					StarRotate = ((StarRotate + 1 >= OptionStarrotateID.End) ? OptionStarrotateID.On : (StarRotate + 1));
					break;
				}
				break;
			case OptionCategoryID.SoundSetting:
				switch (currentOptionIndex)
				{
				case 0:
					AnsVolume = ((AnsVolume + 1 >= OptionVolumeID.End) ? OptionVolumeID.Vol5 : (AnsVolume + 1));
					break;
				case 1:
					CriticalSe = ((CriticalSe + 1 >= OptionCriticalID.End) ? OptionCriticalID.NotPerfect : (CriticalSe + 1));
					break;
				case 2:
					TapHoldVolume = ((TapHoldVolume + 1 >= OptionVolumeID.End) ? OptionVolumeID.Vol5 : (TapHoldVolume + 1));
					break;
				case 3:
					BreakSe = ((BreakSe + 1 >= OptionBreakseID.End) ? OptionBreakseID.Se31 : (BreakSe + 1));
					break;
				case 4:
					BreakVolume = ((BreakVolume + 1 >= OptionVolumeID.End) ? OptionVolumeID.Vol5 : (BreakVolume + 1));
					break;
				case 5:
					ExSe = ((ExSe + 1 >= OptionExseID.End) ? OptionExseID.SE7 : (ExSe + 1));
					break;
				case 6:
					ExVolume = ((ExVolume + 1 >= OptionVolumeID.End) ? OptionVolumeID.Vol5 : (ExVolume + 1));
					break;
				case 7:
					SlideSe = ((SlideSe + 1 >= OptionSlideseID.End) ? OptionSlideseID.Se31 : (SlideSe + 1));
					break;
				case 8:
					SlideVolume = ((SlideVolume + 1 >= OptionVolumeID.End) ? OptionVolumeID.Vol5 : (SlideVolume + 1));
					break;
				case 9:
					TouchHoldVolume = ((TouchHoldVolume + 1 >= OptionVolumeID.End) ? OptionVolumeID.Vol5 : (TouchHoldVolume + 1));
					break;
				case 10:
					DamageSeVolume = ((DamageSeVolume + 1 >= OptionVolumeID.End) ? OptionVolumeID.Vol5 : (DamageSeVolume + 1));
					break;
				}
				break;
			case OptionCategoryID.JudgeSetting:
				switch (currentOptionIndex)
				{
				case 1:
					DispJudge = ((DispJudge + 1 >= OptionDispjudgeID.End) ? OptionDispjudgeID.Type3D : (DispJudge + 1));
					break;
				case 2:
					DispJudgePos = ((DispJudgePos + 1 >= OptionDispjudgeposID.End) ? OptionDispjudgeposID.Out : (DispJudgePos + 1));
					break;
				case 3:
					DispJudgeTouchPos = ((DispJudgeTouchPos + 1 >= OptionDispjudgetouchposID.End) ? OptionDispjudgetouchposID.Out : (DispJudgeTouchPos + 1));
					break;
				case 0:
					DispCenter = ((DispCenter + 1 >= OptionCenterdisplayID.End) ? OptionCenterdisplayID.BoarderBest : (DispCenter + 1));
					break;
				case 4:
					DispChain = ((DispChain + 1 >= OptionDispchainID.End) ? OptionDispchainID.Sync : (DispChain + 1));
					break;
				case 6:
					DispRate = ((DispRate + 1 >= OptionDisprateID.End) ? OptionDisprateID.Hide : (DispRate + 1));
					break;
				case 5:
					SubmonitorAchive = ((SubmonitorAchive + 1 >= OptionSubmonAchiveID.End) ? OptionSubmonAchiveID.AchiveMinus : (SubmonitorAchive + 1));
					break;
				case 7:
					SubmonitorAppeal = ((SubmonitorAppeal + 1 >= OptionAppealID.End) ? OptionAppealID.AllPlay : (SubmonitorAppeal + 1));
					break;
				}
				break;
			}
		}

		public void SubOption(OptionCategoryID category, int currentOptionIndex)
		{
			switch (category)
			{
			case OptionCategoryID.SpeedSetting:
				switch (currentOptionIndex)
				{
				case 0:
					NoteSpeed = ((NoteSpeed - 1 >= OptionNotespeedID.Speed1_0) ? (NoteSpeed - 1) : OptionNotespeedID.Speed1_0);
					break;
				case 1:
					TouchSpeed = ((TouchSpeed - 1 >= OptionTouchspeedID.Speed1_0) ? (TouchSpeed - 1) : OptionTouchspeedID.Speed1_0);
					break;
				case 2:
					SlideSpeed = ((SlideSpeed - 1 >= OptionSlidespeedID.Fast1_0) ? (SlideSpeed - 1) : OptionSlidespeedID.Fast1_0);
					break;
				}
				break;
			case OptionCategoryID.DesignSetting:
				switch (currentOptionIndex)
				{
				case 0:
					TapDesign = ((TapDesign - 1 >= OptionGametapID.Default) ? (TapDesign - 1) : OptionGametapID.Default);
					break;
				case 1:
					HoldDesign = ((HoldDesign - 1 >= OptionGameholdID.Cute) ? (HoldDesign - 1) : OptionGameholdID.Cute);
					break;
				case 2:
					SlideDesign = ((SlideDesign - 1 >= OptionGameslideID.Cute) ? (SlideDesign - 1) : OptionGameslideID.Cute);
					break;
				case 3:
					StarType = ((StarType - 1 >= OptionStartypeID.Blue) ? (StarType - 1) : OptionStartypeID.Blue);
					break;
				case 4:
					OutlineDesign = ((OutlineDesign - 1 >= OptionGameoutlineID.Hide) ? (OutlineDesign - 1) : OptionGameoutlineID.Hide);
					break;
				}
				break;
			case OptionCategoryID.GameSetting:
				switch (currentOptionIndex)
				{
				case 0:
					TrackSkip = ((TrackSkip - 1 >= OptionTrackskipID.Off) ? (TrackSkip - 1) : OptionTrackskipID.Off);
					break;
				case 1:
					MirrorMode = ((MirrorMode - 1 >= OptionMirrorID.Normal) ? (MirrorMode - 1) : OptionMirrorID.Normal);
					break;
				case 3:
					AdjustTiming = ((AdjustTiming - 1 >= OptionJudgetimingID.Fast2_0) ? (AdjustTiming - 1) : OptionJudgetimingID.Fast2_0);
					break;
				case 4:
					JudgeTiming = ((JudgeTiming - 1 >= OptionJudgetimingID.Fast2_0) ? (JudgeTiming - 1) : OptionJudgetimingID.Fast2_0);
					break;
				case 5:
					Brightness = ((Brightness - 1 >= OptionMoviebrightnessID.Bright_0) ? (Brightness - 1) : OptionMoviebrightnessID.Bright_0);
					break;
				case 6:
					TouchEffect = ((TouchEffect - 1 >= OptionToucheffectID.Off) ? (TouchEffect - 1) : OptionToucheffectID.Off);
					break;
				case 2:
					StarRotate = ((StarRotate - 1 >= OptionStarrotateID.Off) ? (StarRotate - 1) : OptionStarrotateID.Off);
					break;
				}
				break;
			case OptionCategoryID.SoundSetting:
				switch (currentOptionIndex)
				{
				case 0:
					AnsVolume = ((AnsVolume - 1 >= OptionVolumeID.Mute) ? (AnsVolume - 1) : OptionVolumeID.Mute);
					break;
				case 1:
					CriticalSe = ((CriticalSe - 1 >= OptionCriticalID.Default) ? (CriticalSe - 1) : OptionCriticalID.Default);
					break;
				case 2:
					TapHoldVolume = ((TapHoldVolume - 1 >= OptionVolumeID.Mute) ? (TapHoldVolume - 1) : OptionVolumeID.Mute);
					break;
				case 3:
					BreakSe = ((BreakSe - 1 >= OptionBreakseID.Se1) ? (BreakSe - 1) : OptionBreakseID.Se1);
					break;
				case 4:
					BreakVolume = ((BreakVolume - 1 >= OptionVolumeID.Mute) ? (BreakVolume - 1) : OptionVolumeID.Mute);
					break;
				case 5:
					ExSe = ((ExSe - 1 >= OptionExseID.Se1) ? (ExSe - 1) : OptionExseID.Se1);
					break;
				case 6:
					ExVolume = ((ExVolume - 1 >= OptionVolumeID.Mute) ? (ExVolume - 1) : OptionVolumeID.Mute);
					break;
				case 7:
					SlideSe = ((SlideSe - 1 >= OptionSlideseID.Se1) ? (SlideSe - 1) : OptionSlideseID.Se1);
					break;
				case 8:
					SlideVolume = ((SlideVolume - 1 >= OptionVolumeID.Mute) ? (SlideVolume - 1) : OptionVolumeID.Mute);
					break;
				case 9:
					TouchHoldVolume = ((TouchHoldVolume - 1 >= OptionVolumeID.Mute) ? (TouchHoldVolume - 1) : OptionVolumeID.Mute);
					break;
				case 10:
					DamageSeVolume = ((DamageSeVolume - 1 >= OptionVolumeID.Mute) ? (DamageSeVolume - 1) : OptionVolumeID.Mute);
					break;
				}
				break;
			case OptionCategoryID.JudgeSetting:
				switch (currentOptionIndex)
				{
				case 1:
					DispJudge = ((DispJudge - 1 >= OptionDispjudgeID.Type1A) ? (DispJudge - 1) : OptionDispjudgeID.Type1A);
					break;
				case 2:
					DispJudgePos = ((DispJudgePos - 1 >= OptionDispjudgeposID.Off) ? (DispJudgePos - 1) : OptionDispjudgeposID.Off);
					break;
				case 3:
					DispJudgeTouchPos = ((DispJudgeTouchPos - 1 >= OptionDispjudgetouchposID.Off) ? (DispJudgeTouchPos - 1) : OptionDispjudgetouchposID.Off);
					break;
				case 0:
					DispCenter = ((DispCenter - 1 >= OptionCenterdisplayID.Off) ? (DispCenter - 1) : OptionCenterdisplayID.Off);
					break;
				case 4:
					DispChain = ((DispChain - 1 >= OptionDispchainID.Off) ? (DispChain - 1) : OptionDispchainID.Off);
					break;
				case 6:
					DispRate = ((DispRate - 1 >= OptionDisprateID.AllDisp) ? (DispRate - 1) : OptionDisprateID.AllDisp);
					break;
				case 5:
					SubmonitorAchive = ((SubmonitorAchive - 1 >= OptionSubmonAchiveID.AchivePlus) ? (SubmonitorAchive - 1) : OptionSubmonAchiveID.AchivePlus);
					break;
				case 7:
					SubmonitorAppeal = ((SubmonitorAppeal - 1 >= OptionAppealID.OFF) ? (SubmonitorAppeal - 1) : OptionAppealID.OFF);
					break;
				}
				break;
			}
		}

		public string GetFilePath(OptionCategoryID category, int currentOptionIndex)
		{
			string result = "";
			switch (category)
			{
			case OptionCategoryID.SpeedSetting:
				switch (currentOptionIndex)
				{
				case 0:
					result = NoteSpeed.GetFilePath();
					break;
				case 2:
					result = SlideSpeed.GetFilePath();
					break;
				case 1:
					result = TouchSpeed.GetFilePath();
					break;
				}
				break;
			case OptionCategoryID.DesignSetting:
				switch (currentOptionIndex)
				{
				case 0:
					result = TapDesign.GetFilePath();
					break;
				case 1:
					result = HoldDesign.GetFilePath();
					break;
				case 2:
					result = SlideDesign.GetFilePath();
					break;
				case 3:
					result = StarType.GetFilePath();
					break;
				case 4:
					result = OutlineDesign.GetFilePath();
					break;
				}
				break;
			case OptionCategoryID.GameSetting:
				switch (currentOptionIndex)
				{
				case 0:
					result = TrackSkip.GetFilePath();
					break;
				case 1:
					result = MirrorMode.GetFilePath();
					break;
				case 3:
					result = AdjustTiming.GetFilePath();
					break;
				case 4:
					result = JudgeTiming.GetFilePath();
					break;
				case 5:
					result = Brightness.GetFilePath();
					break;
				case 6:
					result = TouchEffect.GetFilePath();
					break;
				case 2:
					result = StarRotate.GetFilePath();
					break;
				}
				break;
			case OptionCategoryID.SoundSetting:
				switch (currentOptionIndex)
				{
				case 0:
					result = AnsVolume.GetFilePath();
					break;
				case 1:
					result = CriticalSe.GetFilePath();
					break;
				case 2:
					result = TapHoldVolume.GetFilePath();
					break;
				case 3:
					result = BreakSe.GetFilePath();
					break;
				case 4:
					result = BreakVolume.GetFilePath();
					break;
				case 5:
					result = ExSe.GetFilePath();
					break;
				case 6:
					result = ExVolume.GetFilePath();
					break;
				case 7:
					result = SlideSe.GetFilePath();
					break;
				case 8:
					result = SlideVolume.GetFilePath();
					break;
				case 9:
					result = TouchHoldVolume.GetFilePath();
					break;
				case 10:
					result = DamageSeVolume.GetFilePath();
					break;
				}
				break;
			case OptionCategoryID.JudgeSetting:
				switch (currentOptionIndex)
				{
				case 1:
					result = DispJudge.GetFilePath();
					break;
				case 2:
					result = DispJudgePos.GetFilePath();
					break;
				case 3:
					result = DispJudgeTouchPos.GetFilePath();
					break;
				case 0:
					result = DispCenter.GetFilePath();
					break;
				case 4:
					result = DispChain.GetFilePath();
					break;
				case 6:
					result = DispRate.GetFilePath();
					break;
				case 5:
					result = SubmonitorAchive.GetFilePath();
					break;
				case 7:
					result = SubmonitorAppeal.GetFilePath();
					break;
				}
				break;
			}
			return result;
		}

		public string GetValueDetails(OptionCategoryID category, int currentOptionIndex)
		{
			string result = "";
			switch (category)
			{
			case OptionCategoryID.SpeedSetting:
				switch (currentOptionIndex)
				{
				case 0:
					result = NoteSpeed.GetDetail();
					break;
				case 2:
					result = SlideSpeed.GetDetail();
					break;
				case 1:
					result = TouchSpeed.GetDetail();
					break;
				}
				break;
			case OptionCategoryID.DesignSetting:
				switch (currentOptionIndex)
				{
				case 0:
					result = TapDesign.GetDetail();
					break;
				case 1:
					result = HoldDesign.GetDetail();
					break;
				case 2:
					result = SlideDesign.GetDetail();
					break;
				case 3:
					result = StarType.GetDetail();
					break;
				case 4:
					result = OutlineDesign.GetDetail();
					break;
				}
				break;
			case OptionCategoryID.GameSetting:
				switch (currentOptionIndex)
				{
				case 0:
					result = TrackSkip.GetDetail();
					break;
				case 1:
					result = MirrorMode.GetDetail();
					break;
				case 3:
					result = AdjustTiming.GetDetail();
					break;
				case 4:
					result = JudgeTiming.GetDetail();
					break;
				case 5:
					result = Brightness.GetDetail();
					break;
				case 6:
					result = TouchEffect.GetDetail();
					break;
				case 2:
					result = StarRotate.GetDetail();
					break;
				}
				break;
			case OptionCategoryID.SoundSetting:
				switch (currentOptionIndex)
				{
				case 0:
					result = AnsVolume.GetDetail();
					break;
				case 1:
					result = CriticalSe.GetDetail();
					break;
				case 2:
					result = TapHoldVolume.GetDetail();
					break;
				case 3:
					result = BreakSe.GetDetail();
					break;
				case 4:
					result = BreakVolume.GetDetail();
					break;
				case 5:
					result = ExSe.GetDetail();
					break;
				case 6:
					result = ExVolume.GetDetail();
					break;
				case 7:
					result = SlideSe.GetDetail();
					break;
				case 8:
					result = SlideVolume.GetDetail();
					break;
				case 9:
					result = TouchHoldVolume.GetDetail();
					break;
				case 10:
					result = DamageSeVolume.GetDetail();
					break;
				}
				break;
			case OptionCategoryID.JudgeSetting:
				switch (currentOptionIndex)
				{
				case 1:
					result = DispJudge.GetDetail();
					break;
				case 2:
					result = DispJudgePos.GetDetail();
					break;
				case 3:
					result = DispJudgeTouchPos.GetDetail();
					break;
				case 0:
					result = DispCenter.GetDetail();
					break;
				case 4:
					result = DispChain.GetDetail();
					break;
				case 6:
					result = DispRate.GetDetail();
					break;
				case 5:
					result = SubmonitorAchive.GetDetail();
					break;
				case 7:
					result = SubmonitorAppeal.GetDetail();
					break;
				}
				break;
			}
			return result;
		}

		public int GetOptionValueIndex(OptionCategoryID category, int currentOptionIndex)
		{
			int result = -1;
			switch (category)
			{
			case OptionCategoryID.SpeedSetting:
				switch (currentOptionIndex)
				{
				case 0:
					result = (int)NoteSpeed;
					break;
				case 2:
					result = (int)SlideSpeed;
					break;
				case 1:
					result = (int)TouchSpeed;
					break;
				}
				break;
			case OptionCategoryID.DesignSetting:
				switch (currentOptionIndex)
				{
				case 0:
					result = (int)TapDesign;
					break;
				case 1:
					result = (int)HoldDesign;
					break;
				case 2:
					result = (int)SlideDesign;
					break;
				case 3:
					result = (int)StarType;
					break;
				case 4:
					result = (int)OutlineDesign;
					break;
				}
				break;
			case OptionCategoryID.GameSetting:
				switch (currentOptionIndex)
				{
				case 0:
					result = (int)TrackSkip;
					break;
				case 1:
					result = (int)MirrorMode;
					break;
				case 3:
					result = (int)AdjustTiming;
					break;
				case 4:
					result = (int)JudgeTiming;
					break;
				case 5:
					result = (int)Brightness;
					break;
				case 6:
					result = (int)TouchEffect;
					break;
				case 2:
					result = (int)StarRotate;
					break;
				}
				break;
			case OptionCategoryID.SoundSetting:
				switch (currentOptionIndex)
				{
				case 0:
					result = (int)AnsVolume;
					break;
				case 1:
					result = (int)CriticalSe;
					break;
				case 2:
					result = (int)TapHoldVolume;
					break;
				case 3:
					result = (int)BreakSe;
					break;
				case 4:
					result = (int)BreakVolume;
					break;
				case 5:
					result = (int)ExSe;
					break;
				case 6:
					result = (int)ExVolume;
					break;
				case 7:
					result = (int)SlideSe;
					break;
				case 8:
					result = (int)SlideVolume;
					break;
				case 9:
					result = (int)TouchHoldVolume;
					break;
				case 10:
					result = (int)DamageSeVolume;
					break;
				}
				break;
			case OptionCategoryID.JudgeSetting:
				switch (currentOptionIndex)
				{
				case 1:
					result = (int)DispJudge;
					break;
				case 2:
					result = (int)DispJudgePos;
					break;
				case 3:
					result = (int)DispJudgeTouchPos;
					break;
				case 0:
					result = (int)DispCenter;
					break;
				case 4:
					result = (int)DispChain;
					break;
				case 6:
					result = (int)DispRate;
					break;
				case 5:
					result = (int)SubmonitorAchive;
					break;
				case 7:
					result = (int)SubmonitorAppeal;
					break;
				}
				break;
			}
			return result;
		}

		public int GetOptionMax(OptionCategoryID category, int currentOptionIndex)
		{
			int result = -1;
			switch (category)
			{
			case OptionCategoryID.SpeedSetting:
				switch (currentOptionIndex)
				{
				case 0:
					result = OptionNotespeedID.End.GetEnd();
					break;
				case 2:
					result = OptionSlidespeedID.End.GetEnd();
					break;
				case 1:
					result = OptionTouchspeedID.End.GetEnd();
					break;
				}
				break;
			case OptionCategoryID.DesignSetting:
				switch (currentOptionIndex)
				{
				case 0:
					result = OptionGametapID.End.GetEnd();
					break;
				case 1:
					result = OptionGameholdID.End.GetEnd();
					break;
				case 2:
					result = OptionGameslideID.End.GetEnd();
					break;
				case 3:
					result = OptionStartypeID.End.GetEnd();
					break;
				case 4:
					result = OptionGameoutlineID.End.GetEnd();
					break;
				}
				break;
			case OptionCategoryID.GameSetting:
				switch (currentOptionIndex)
				{
				case 0:
					result = OptionTrackskipID.End.GetEnd();
					break;
				case 1:
					result = OptionMirrorID.End.GetEnd();
					break;
				case 3:
					result = OptionJudgetimingID.End.GetEnd();
					break;
				case 4:
					result = OptionJudgetimingID.End.GetEnd();
					break;
				case 5:
					result = OptionMoviebrightnessID.End.GetEnd();
					break;
				case 6:
					result = OptionToucheffectID.End.GetEnd();
					break;
				case 2:
					result = OptionStarrotateID.End.GetEnd();
					break;
				}
				break;
			case OptionCategoryID.SoundSetting:
				switch (currentOptionIndex)
				{
				case 0:
					result = OptionVolumeID.End.GetEnd();
					break;
				case 1:
					result = OptionCriticalID.End.GetEnd();
					break;
				case 2:
					result = OptionVolumeID.End.GetEnd();
					break;
				case 3:
					result = OptionBreakseID.End.GetEnd();
					break;
				case 4:
					result = OptionVolumeID.End.GetEnd();
					break;
				case 5:
					result = OptionExseID.End.GetEnd();
					break;
				case 6:
					result = OptionVolumeID.End.GetEnd();
					break;
				case 7:
					result = OptionSlideseID.End.GetEnd();
					break;
				case 8:
					result = OptionVolumeID.End.GetEnd();
					break;
				case 9:
					result = OptionVolumeID.End.GetEnd();
					break;
				case 10:
					result = OptionVolumeID.End.GetEnd();
					break;
				}
				break;
			case OptionCategoryID.JudgeSetting:
				switch (currentOptionIndex)
				{
				case 1:
					result = OptionDispjudgeID.End.GetEnd();
					break;
				case 2:
					result = OptionDispjudgeposID.End.GetEnd();
					break;
				case 3:
					result = OptionDispjudgetouchposID.End.GetEnd();
					break;
				case 0:
					result = OptionCenterdisplayID.End.GetEnd();
					break;
				case 4:
					result = OptionDispchainID.End.GetEnd();
					break;
				case 6:
					result = OptionDisprateID.End.GetEnd();
					break;
				case 5:
					result = OptionSubmonAchiveID.End.GetEnd();
					break;
				case 7:
					result = OptionAppealID.End.GetEnd();
					break;
				}
				break;
			}
			return result;
		}

		public float GetBgBrightness()
		{
			return Brightness.GetValue();
		}

		public float GetAdjustMSec()
		{
			return (float)(AdjustTiming - 20 + 36) / 10f * 16.666666f;
		}

		public float GetJudgeTimingFrame()
		{
			return (float)(JudgeTiming - 20) / 10f;
		}

		public void Initialize()
		{
			InitSpeed();
			InitGame();
			InitJudge();
			InitDesign();
			InitSound();
			InitSpecial();
			InitOther();
		}

		public void InitSpeed()
		{
			OptionKind = OptionKindID.Basic;
			NoteSpeed = NoteSpeed.GetDefault();
			SlideSpeed = SlideSpeed.GetDefault();
			TouchSpeed = TouchSpeed.GetDefault();
		}

		public void InitGame()
		{
			TrackSkip = TrackSkip.GetDefault();
			MirrorMode = MirrorMode.GetDefault();
			AdjustTiming = AdjustTiming.GetDefault();
			JudgeTiming = JudgeTiming.GetDefault();
			Brightness = Brightness.GetDefault();
			TouchEffect = TouchEffect.GetDefault();
			StarRotate = StarRotate.GetDefault();
		}

		public void InitJudge()
		{
			DispJudge = DispJudge.GetDefault();
			DispJudgePos = DispJudgePos.GetDefault();
			DispJudgeTouchPos = DispJudgeTouchPos.GetDefault();
			DispCenter = DispCenter.GetDefault();
			DispChain = DispChain.GetDefault();
			DispRate = DispRate.GetDefault();
			SubmonitorAchive = SubmonitorAchive.GetDefault();
			SubmonitorAppeal = SubmonitorAppeal.GetDefault();
		}

		public void InitDesign()
		{
			TapDesign = TapDesign.GetDefault();
			HoldDesign = HoldDesign.GetDefault();
			SlideDesign = SlideDesign.GetDefault();
			StarType = StarType.GetDefault();
			OutlineDesign = OutlineDesign.GetDefault();
		}

		public void InitSound()
		{
			AnsVolume = AnsVolume.GetDefault();
			CriticalSe = CriticalSe.GetDefault();
			TapHoldVolume = TapHoldVolume.GetDefault();
			BreakSe = BreakSe.GetDefault();
			BreakVolume = BreakVolume.GetDefault();
			ExSe = ExSe.GetDefault();
			ExVolume = ExVolume.GetDefault();
			SlideSe = SlideSe.GetDefault();
			SlideVolume = SlideVolume.GetDefault();
			DamageSeVolume = DamageSeVolume.GetDefault();
		}

		public void InitSpecial()
		{
			HeadPhoneVolume = HeadPhoneVolume.GetDefault();
		}

		public void InitOther()
		{
			TouchHoldVolume = TouchHoldVolume.GetDefault();
			NoteSize = NoteSize.GetDefault();
			SlideSize = SlideSize.GetDefault();
			TouchSize = TouchSize.GetDefault();
			Matching = OptionMatchingID.On;
			BarDisp = BarDisp.GetDefault();
			SubmonitorAnimation = SubmonitorAnimation.GetDefault();
		}

		public void InitializeTutorial()
		{
			Initialize();
		}

		public UserOption()
		{
		}

		public UserOption(UserOption option)
		{
			OptionKind = option.OptionKind;
			NoteSpeed = option.NoteSpeed;
			SlideSpeed = option.SlideSpeed;
			TouchSpeed = option.TouchSpeed;
			NoteSize = option.NoteSize;
			SlideSize = option.SlideSize;
			TouchSize = option.TouchSize;
			TapDesign = option.TapDesign;
			HoldDesign = option.HoldDesign;
			SlideDesign = option.SlideDesign;
			StarType = option.StarType;
			StarRotate = option.StarRotate;
			AdjustTiming = option.AdjustTiming;
			JudgeTiming = option.JudgeTiming;
			MirrorMode = option.MirrorMode;
			AnsVolume = option.AnsVolume;
			TapHoldVolume = option.TapHoldVolume;
			BreakSe = option.BreakSe;
			BreakVolume = option.BreakVolume;
			ExSe = option.ExSe;
			ExVolume = option.ExVolume;
			SlideSe = option.SlideSe;
			SlideVolume = option.SlideVolume;
			CriticalSe = option.CriticalSe;
			TouchHoldVolume = option.TouchHoldVolume;
			DamageSeVolume = option.DamageSeVolume;
			HeadPhoneVolume = option.HeadPhoneVolume;
			Matching = option.Matching;
			Brightness = option.Brightness;
			DispCenter = option.DispCenter;
			DispChain = option.DispChain;
			DispJudge = option.DispJudge;
			DispJudgePos = option.DispJudgePos;
			DispJudgeTouchPos = option.DispJudgeTouchPos;
			TrackSkip = option.TrackSkip;
			OutlineDesign = option.OutlineDesign;
			TouchEffect = option.TouchEffect;
			BarDisp = option.BarDisp;
			DispRate = option.DispRate;
			SubmonitorAppeal = option.SubmonitorAppeal;
			SubmonitorAchive = option.SubmonitorAchive;
			SubmonitorAnimation = option.SubmonitorAnimation;
		}

		public void CheckOverParam()
		{
			if (OptionKind >= OptionKindID.End)
			{
				OptionKind = OptionKindID.Custom;
			}
			if (NoteSpeed >= OptionNotespeedID.End)
			{
				NoteSpeed = OptionNotespeedID.Speed_Sonic;
			}
			if (SlideSpeed >= OptionSlidespeedID.End)
			{
				SlideSpeed = OptionSlidespeedID.Late1_0;
			}
			if (TouchSpeed >= OptionTouchspeedID.End)
			{
				TouchSpeed = OptionTouchspeedID.Speed_Sonic;
			}
			if (NoteSize >= OptionNotesizeID.End)
			{
				NoteSize = OptionNotesizeID.Big;
			}
			if (SlideSize >= OptionSlidesizeID.End)
			{
				SlideSize = OptionSlidesizeID.Big;
			}
			if (TouchSize >= OptionTouchsizeID.End)
			{
				TouchSize = OptionTouchsizeID.Middle;
			}
			if (TapDesign >= OptionGametapID.End)
			{
				TapDesign = OptionGametapID.Any;
			}
			if (HoldDesign >= OptionGameholdID.End)
			{
				HoldDesign = OptionGameholdID.Legacy;
			}
			if (SlideDesign >= OptionGameslideID.End)
			{
				SlideDesign = OptionGameslideID.Legacy;
			}
			if (StarType >= OptionStartypeID.End)
			{
				StarType = OptionStartypeID.Pink;
			}
			if (StarRotate >= OptionStarrotateID.End)
			{
				StarRotate = OptionStarrotateID.On;
			}
			if (AdjustTiming >= OptionJudgetimingID.End)
			{
				AdjustTiming = OptionJudgetimingID.Late2_0;
			}
			if (JudgeTiming >= OptionJudgetimingID.End)
			{
				JudgeTiming = OptionJudgetimingID.Late2_0;
			}
			if (MirrorMode >= OptionMirrorID.End)
			{
				MirrorMode = OptionMirrorID.UDLR;
			}
			if (AnsVolume >= OptionVolumeID.End)
			{
				AnsVolume = OptionVolumeID.Vol5;
			}
			if (TapHoldVolume >= OptionVolumeID.End)
			{
				TapHoldVolume = OptionVolumeID.Vol5;
			}
			if (BreakSe >= OptionBreakseID.End)
			{
				BreakSe = OptionBreakseID.Se31;
			}
			if (BreakVolume >= OptionVolumeID.End)
			{
				BreakVolume = OptionVolumeID.Vol5;
			}
			if (ExSe >= OptionExseID.End)
			{
				ExSe = OptionExseID.SE7;
			}
			if (ExVolume >= OptionVolumeID.End)
			{
				ExVolume = OptionVolumeID.Vol5;
			}
			if (SlideSe >= OptionSlideseID.End)
			{
				SlideSe = OptionSlideseID.Se31;
			}
			if (SlideVolume >= OptionVolumeID.End)
			{
				SlideVolume = OptionVolumeID.Vol5;
			}
			if (CriticalSe >= OptionCriticalID.End)
			{
				CriticalSe = OptionCriticalID.NotPerfect;
			}
			if (TouchHoldVolume >= OptionVolumeID.End)
			{
				TouchHoldVolume = OptionVolumeID.Vol5;
			}
			if (DamageSeVolume >= OptionVolumeID.End)
			{
				DamageSeVolume = OptionVolumeID.Vol5;
			}
			if (HeadPhoneVolume >= OptionHeadphonevolumeID.End)
			{
				HeadPhoneVolume = OptionHeadphonevolumeID.Vol20;
			}
			if (Matching >= OptionMatchingID.End)
			{
				Matching = OptionMatchingID.On;
			}
			if (Brightness >= OptionMoviebrightnessID.End)
			{
				Brightness = OptionMoviebrightnessID.Bright_3;
			}
			if (DispCenter >= OptionCenterdisplayID.End)
			{
				DispCenter = OptionCenterdisplayID.BoarderBest;
			}
			if (DispChain >= OptionDispchainID.End)
			{
				DispChain = OptionDispchainID.Sync;
			}
			if (DispJudge >= OptionDispjudgeID.End)
			{
				DispJudge = OptionDispjudgeID.Type3D;
			}
			if (DispJudgePos >= OptionDispjudgeposID.End)
			{
				DispJudgePos = OptionDispjudgeposID.Out;
			}
			if (DispJudgeTouchPos >= OptionDispjudgetouchposID.End)
			{
				DispJudgeTouchPos = OptionDispjudgetouchposID.Out;
			}
			if (TrackSkip >= OptionTrackskipID.End)
			{
				TrackSkip = OptionTrackskipID.AutoBest;
			}
			if (OutlineDesign >= OptionGameoutlineID.End)
			{
				OutlineDesign = OptionGameoutlineID.Splash;
			}
			if (TouchEffect >= OptionToucheffectID.End)
			{
				TouchEffect = OptionToucheffectID.On;
			}
			if (BarDisp >= OptionDispbarlineID.End)
			{
				BarDisp = OptionDispbarlineID.On;
			}
			if (DispRate >= OptionDisprateID.End)
			{
				DispRate = OptionDisprateID.Hide;
			}
			if (SubmonitorAppeal >= OptionAppealID.End)
			{
				SubmonitorAppeal = OptionAppealID.AllPlay;
			}
			if (SubmonitorAchive >= OptionSubmonAchiveID.End)
			{
				SubmonitorAchive = OptionSubmonAchiveID.AchiveMinus;
			}
			if (SubmonitorAnimation >= OptionSubmonitorID.End)
			{
				SubmonitorAnimation = OptionSubmonitorID.achievement_only;
			}
		}
	}
}
