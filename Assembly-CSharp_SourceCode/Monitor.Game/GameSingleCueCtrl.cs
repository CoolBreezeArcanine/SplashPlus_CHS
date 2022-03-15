using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager;

namespace Monitor.Game
{
	public class GameSingleCueCtrl : Singleton<GameSingleCueCtrl>
	{
		private readonly OptionVolumeID[] SeVolume = new OptionVolumeID[2];

		private readonly OptionVolumeID[] AnsVolume = new OptionVolumeID[2];

		private readonly OptionVolumeID[] BreakVolume = new OptionVolumeID[2];

		private readonly OptionVolumeID[] SlideVolume = new OptionVolumeID[2];

		private readonly OptionVolumeID[] ExVolume = new OptionVolumeID[2];

		private readonly OptionVolumeID[] TouchHoldVolume = new OptionVolumeID[2];

		private static readonly bool[] AnsSE = new bool[2];

		private static readonly bool[] SlideSE = new bool[2];

		private static readonly NoteJudge.JudgeBox[] ExSE = new NoteJudge.JudgeBox[2];

		private static readonly NoteJudge.JudgeBox[] CenterEffectSE = new NoteJudge.JudgeBox[2];

		private static readonly NoteJudge.JudgeBox[] JudgeTapSE = new NoteJudge.JudgeBox[2];

		private static readonly NoteJudge.JudgeBox[] JudgeTouchSE = new NoteJudge.JudgeBox[2];

		private static readonly NoteJudge.JudgeBox[] JudgeBreakSE = new NoteJudge.JudgeBox[2];

		private static readonly NoteJudge.JudgeBox[] JudgeTouchHoldLoopSE = new NoteJudge.JudgeBox[2];

		private readonly Cue[] SlideSe = new Cue[2];

		private readonly Cue[] BreakGoodSe = new Cue[2];

		private readonly Cue[] BreakBadSe = new Cue[2];

		private readonly Cue[] ExSe = new Cue[2];

		private readonly bool[] DisableTouchHoldLoop = new bool[2];

		private readonly bool[] StopTouchSe = new bool[2];

		public void Initialize(int index)
		{
			SeVolume[index] = Singleton<GamePlayManager>.Instance.GetGameScore(index).UserOption.TapHoldVolume;
			BreakVolume[index] = Singleton<GamePlayManager>.Instance.GetGameScore(index).UserOption.BreakVolume;
			SlideVolume[index] = Singleton<GamePlayManager>.Instance.GetGameScore(index).UserOption.SlideVolume;
			ExVolume[index] = Singleton<GamePlayManager>.Instance.GetGameScore(index).UserOption.ExVolume;
			AnsVolume[index] = Singleton<GamePlayManager>.Instance.GetGameScore(index).UserOption.AnsVolume;
			TouchHoldVolume[index] = Singleton<GamePlayManager>.Instance.GetGameScore(index).UserOption.TouchHoldVolume;
			SlideSe[index] = Singleton<GamePlayManager>.Instance.GetGameScore(index).UserOption.SlideSe.GetSlideCue();
			BreakGoodSe[index] = Singleton<GamePlayManager>.Instance.GetGameScore(index).UserOption.BreakSe.GetBreakGoodCue();
			BreakBadSe[index] = Singleton<GamePlayManager>.Instance.GetGameScore(index).UserOption.BreakSe.GetBreakBadCue();
			ExSe[index] = Singleton<GamePlayManager>.Instance.GetGameScore(index).UserOption.ExSe.GetExCue();
			DisableTouchHoldLoop[index] = true;
			StopTouchSe[index] = false;
			ClearCue(index);
		}

		public void ReserveAnswerSe(int index)
		{
			AnsSE[index] = true;
		}

		public void ReserveTouchJudgeSe(int index, NoteJudge.JudgeBox judge)
		{
			if (JudgeTouchSE[index] == NoteJudge.JudgeBox.End || JudgeTouchSE[index] < judge)
			{
				JudgeTouchSE[index] = judge;
			}
		}

		public void ReserveTapJudgeSe(int index, NoteJudge.JudgeBox judge)
		{
			if (JudgeTapSE[index] == NoteJudge.JudgeBox.End || JudgeTapSE[index] < judge)
			{
				JudgeTapSE[index] = judge;
			}
		}

		public void ReserveBreakJudgeSe(int index, NoteJudge.JudgeBox judge)
		{
			if (JudgeBreakSE[index] == NoteJudge.JudgeBox.End || JudgeBreakSE[index] < judge)
			{
				JudgeBreakSE[index] = judge;
			}
		}

		public void ReserveSlideSe(int index)
		{
			SlideSE[index] = true;
		}

		public void ReserveExSe(int index, NoteJudge.JudgeBox judge)
		{
			if (ExSE[index] == NoteJudge.JudgeBox.End || ExSE[index] < judge)
			{
				ExSE[index] = judge;
			}
		}

		public void ReserveCenterEffectSe(int index, NoteJudge.JudgeBox judge)
		{
			if (CenterEffectSE[index] == NoteJudge.JudgeBox.End || CenterEffectSE[index] < judge)
			{
				CenterEffectSE[index] = judge;
			}
		}

		public void ReserveTouchHoldLoopSe(int index, NoteJudge.JudgeBox judge, bool loopDisable)
		{
			if (JudgeTouchHoldLoopSE[index] == NoteJudge.JudgeBox.End || JudgeTouchHoldLoopSE[index] < judge)
			{
				JudgeTouchHoldLoopSE[index] = judge;
				DisableTouchHoldLoop[index] = loopDisable;
			}
		}

		public void PlayJudgeSe(int index)
		{
			bool flag = false;
			if (AnsSE[index] && AnsVolume[index] != 0)
			{
				SoundManager.PlayGameSE(Cue.SE_GAME_ANSWER, index, AnsVolume[index].GetValue());
			}
			if (JudgeTapSE[index] != NoteJudge.JudgeBox.End && SeVolume[index] != 0)
			{
				switch (Singleton<GamePlayManager>.Instance.GetGameScore(index).UserOption.CriticalSe)
				{
				case OptionCriticalID.Default:
					switch (JudgeTapSE[index])
					{
					case NoteJudge.JudgeBox.Good:
						SoundManager.PlayGameSE(Cue.SE_GAME_GOOD, index, SeVolume[index].GetValue());
						break;
					case NoteJudge.JudgeBox.Great:
						SoundManager.PlayGameSE(Cue.SE_GAME_GREAT, index, SeVolume[index].GetValue());
						break;
					case NoteJudge.JudgeBox.Perfect:
						SoundManager.PlayGameSE(Cue.SE_GAME_PERFECT, index, SeVolume[index].GetValue());
						break;
					case NoteJudge.JudgeBox.Critical:
						SoundManager.PlayGameSE(Cue.SE_GAME_PERFECT, index, SeVolume[index].GetValue());
						break;
					}
					break;
				case OptionCriticalID.CriticalOn:
					switch (JudgeTapSE[index])
					{
					case NoteJudge.JudgeBox.Good:
						SoundManager.PlayGameSE(Cue.SE_GAME_GOOD, index, SeVolume[index].GetValue());
						break;
					case NoteJudge.JudgeBox.Great:
						SoundManager.PlayGameSE(Cue.SE_GAME_GREAT, index, SeVolume[index].GetValue());
						break;
					case NoteJudge.JudgeBox.Perfect:
						SoundManager.PlayGameSE(Cue.SE_GAME_PERFECT, index, SeVolume[index].GetValue());
						break;
					case NoteJudge.JudgeBox.Critical:
						SoundManager.PlayGameSE(Cue.SE_GAME_CRITICAL_PERFECT, index, SeVolume[index].GetValue());
						break;
					}
					break;
				case OptionCriticalID.CriticalOnly:
				{
					NoteJudge.JudgeBox judgeBox = JudgeTapSE[index];
					if (judgeBox == NoteJudge.JudgeBox.Critical)
					{
						SoundManager.PlayGameSE(Cue.SE_GAME_CRITICAL_PERFECT, index, SeVolume[index].GetValue());
					}
					break;
				}
				case OptionCriticalID.NotPerfect:
				{
					NoteJudge.JudgeBox judgeBox = JudgeTapSE[index];
					if ((uint)judgeBox <= 3u)
					{
						SoundManager.PlayGameSE(Cue.SE_GAME_NORMAL, index, SeVolume[index].GetValue());
						flag = true;
					}
					break;
				}
				}
			}
			if (JudgeTouchSE[index] != NoteJudge.JudgeBox.End && SeVolume[index] != 0)
			{
				switch (Singleton<GamePlayManager>.Instance.GetGameScore(index).UserOption.CriticalSe)
				{
				case OptionCriticalID.Default:
					switch (JudgeTouchSE[index])
					{
					case NoteJudge.JudgeBox.Good:
						SoundManager.PlayGameSE(Cue.SE_GAME_SCR_TOUCH_GOOD, index, SeVolume[index].GetValue());
						break;
					case NoteJudge.JudgeBox.Great:
						SoundManager.PlayGameSE(Cue.SE_GAME_SCR_TOUCH_GREAT, index, SeVolume[index].GetValue());
						break;
					case NoteJudge.JudgeBox.Perfect:
					case NoteJudge.JudgeBox.Critical:
						SoundManager.PlayGameSE(Cue.SE_GAME_SCR_TOUCH_PERFECT, index, SeVolume[index].GetValue());
						break;
					}
					break;
				case OptionCriticalID.CriticalOn:
					switch (JudgeTouchSE[index])
					{
					case NoteJudge.JudgeBox.Good:
						SoundManager.PlayGameSE(Cue.SE_GAME_SCR_TOUCH_GOOD, index, SeVolume[index].GetValue());
						break;
					case NoteJudge.JudgeBox.Great:
						SoundManager.PlayGameSE(Cue.SE_GAME_SCR_TOUCH_GREAT, index, SeVolume[index].GetValue());
						break;
					case NoteJudge.JudgeBox.Perfect:
					case NoteJudge.JudgeBox.Critical:
						SoundManager.PlayGameSE(Cue.SE_GAME_SCR_TOUCH_PERFECT, index, SeVolume[index].GetValue());
						break;
					}
					break;
				case OptionCriticalID.CriticalOnly:
				{
					NoteJudge.JudgeBox judgeBox = JudgeTouchSE[index];
					if (judgeBox == NoteJudge.JudgeBox.Critical)
					{
						SoundManager.PlayGameSE(Cue.SE_GAME_SCR_TOUCH_PERFECT, index, SeVolume[index].GetValue());
					}
					break;
				}
				case OptionCriticalID.NotPerfect:
				{
					NoteJudge.JudgeBox judgeBox = JudgeTouchSE[index];
					if ((uint)judgeBox <= 3u && !flag)
					{
						SoundManager.PlayGameSE(Cue.SE_GAME_NORMAL, index, SeVolume[index].GetValue());
					}
					break;
				}
				}
			}
			if (JudgeBreakSE[index] != NoteJudge.JudgeBox.End && BreakVolume[index] != 0)
			{
				switch (Singleton<GamePlayManager>.Instance.GetGameScore(index).UserOption.CriticalSe)
				{
				case OptionCriticalID.CriticalOnly:
				{
					NoteJudge.JudgeBox judgeBox = JudgeBreakSE[index];
					if (judgeBox == NoteJudge.JudgeBox.Critical)
					{
						SoundManager.PlayGameSingleSe(BreakGoodSe[index], index, SoundManager.PlayerID.BreakSe, BreakVolume[index].GetValue());
						SoundManager.PlayGameSingleSe(Cue.SE_GAME_CHEER, index, SoundManager.PlayerID.Chear, BreakVolume[index].GetValue());
					}
					break;
				}
				case OptionCriticalID.NotPerfect:
				{
					NoteJudge.JudgeBox judgeBox = JudgeBreakSE[index];
					if ((uint)judgeBox <= 3u && !flag)
					{
						SoundManager.PlayGameSE(Cue.SE_GAME_NORMAL, index, SeVolume[index].GetValue());
					}
					break;
				}
				default:
					switch (JudgeBreakSE[index])
					{
					case NoteJudge.JudgeBox.Good:
						SoundManager.PlayGameSingleSe(BreakBadSe[index], index, SoundManager.PlayerID.BreakSe, BreakVolume[index].GetValue());
						break;
					case NoteJudge.JudgeBox.Great:
						SoundManager.PlayGameSingleSe(BreakBadSe[index], index, SoundManager.PlayerID.BreakSe, BreakVolume[index].GetValue());
						break;
					case NoteJudge.JudgeBox.Perfect:
						SoundManager.PlayGameSingleSe(BreakGoodSe[index], index, SoundManager.PlayerID.BreakSe, BreakVolume[index].GetValue());
						break;
					case NoteJudge.JudgeBox.Critical:
						SoundManager.PlayGameSingleSe(BreakGoodSe[index], index, SoundManager.PlayerID.BreakSe, BreakVolume[index].GetValue());
						SoundManager.PlayGameSingleSe(Cue.SE_GAME_CHEER, index, SoundManager.PlayerID.Chear, BreakVolume[index].GetValue());
						break;
					}
					break;
				}
			}
			if (SlideSE[index] && SlideVolume[index] != 0)
			{
				SoundManager.PlayGameSingleSe(SlideSe[index], index, SoundManager.PlayerID.SlideSe, SlideVolume[index].GetValue());
			}
			if (ExSE[index] != NoteJudge.JudgeBox.End && ExVolume[index] != 0)
			{
				OptionCriticalID criticalSe = Singleton<GamePlayManager>.Instance.GetGameScore(index).UserOption.CriticalSe;
				if (criticalSe == OptionCriticalID.NotPerfect)
				{
					NoteJudge.JudgeBox judgeBox = ExSE[index];
					if ((uint)judgeBox <= 3u && !flag)
					{
						SoundManager.PlayGameSE(Cue.SE_GAME_NORMAL, index, SeVolume[index].GetValue());
					}
				}
				else
				{
					NoteJudge.JudgeBox judgeBox = ExSE[index];
					if ((uint)(judgeBox - 1) <= 3u)
					{
						SoundManager.PlayGameSingleSe(ExSe[index], index, SoundManager.PlayerID.ExSe, ExVolume[index].GetValue());
					}
				}
			}
			if (CenterEffectSE[index] != NoteJudge.JudgeBox.End && SeVolume[index] != 0)
			{
				NoteJudge.JudgeBox judgeBox = CenterEffectSE[index];
				if ((uint)(judgeBox - 1) <= 3u)
				{
					SoundManager.PlayGameSingleSe(Cue.SE_GAME_SCR_TOUCH_CENTER, index, SoundManager.PlayerID.CenterSe, TouchHoldVolume[index].GetValue());
				}
			}
			if (StopTouchSe[index])
			{
				SoundManager.StopGameSingleSe(index, SoundManager.PlayerID.TouchHoldLoop);
				StopTouchSe[index] = false;
			}
			if (JudgeTouchHoldLoopSE[index] != NoteJudge.JudgeBox.End && TouchHoldVolume[index] != 0)
			{
				switch (JudgeTouchHoldLoopSE[index])
				{
				case NoteJudge.JudgeBox.Miss:
					SoundManager.PlayGameSingleSe(Cue.SE_GAME_TOUCH_HOLD_MISS, index, SoundManager.PlayerID.TouchHoldLoop, TouchHoldVolume[index].GetValue());
					break;
				case NoteJudge.JudgeBox.Good:
					SoundManager.PlayGameSingleSe(Cue.SE_GAME_TOUCH_HOLD_GOOD, index, SoundManager.PlayerID.TouchHoldLoop, TouchHoldVolume[index].GetValue());
					break;
				case NoteJudge.JudgeBox.Great:
					SoundManager.PlayGameSingleSe(Cue.SE_GAME_TOUCH_HOLD_GREAT, index, SoundManager.PlayerID.TouchHoldLoop, TouchHoldVolume[index].GetValue());
					break;
				case NoteJudge.JudgeBox.Perfect:
				case NoteJudge.JudgeBox.Critical:
					SoundManager.PlayGameSingleSe(Cue.SE_GAME_TOUCH_HOLD_PERFECT, index, SoundManager.PlayerID.TouchHoldLoop, TouchHoldVolume[index].GetValue());
					break;
				}
				if (DisableTouchHoldLoop[index])
				{
					SoundManager.StopGameSingleSe(index, SoundManager.PlayerID.TouchHoldLoop);
					StopTouchSe[index] = true;
				}
			}
			ClearCue(index);
		}

		private void ClearCue(int index)
		{
			AnsSE[index] = false;
			SlideSE[index] = false;
			ExSE[index] = NoteJudge.JudgeBox.End;
			CenterEffectSE[index] = NoteJudge.JudgeBox.End;
			JudgeTapSE[index] = NoteJudge.JudgeBox.End;
			JudgeTouchSE[index] = NoteJudge.JudgeBox.End;
			JudgeBreakSE[index] = NoteJudge.JudgeBox.End;
			JudgeTouchHoldLoopSE[index] = NoteJudge.JudgeBox.End;
		}
	}
}
