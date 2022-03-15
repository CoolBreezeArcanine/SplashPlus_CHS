using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using Mai2.Voice_Partner_000001;

namespace Manager
{
	public static class SoundManager
	{
		public enum PlayerID
		{
			Begin = 0,
			Test = 0,
			Error = 1,
			Music = 2,
			SystemStart = 3,
			SystemEnd = 4,
			Sound_Sepalate_Start = 5,
			Sound_Sepalate_Left = 5,
			Bgm = 5,
			Voice = 6,
			PartnerVoice = 7,
			Jingle = 8,
			SeStart = 9,
			Se1 = 9,
			Se2 = 10,
			Se3 = 11,
			Se4 = 12,
			SeEnd = 12,
			LoopSeStart = 13,
			LoopSe = 13,
			LoopSe2 = 14,
			LoopSeEnd = 14,
			GameSeStart = 15,
			GameSe1 = 15,
			GameSe2 = 16,
			GameSe3 = 17,
			GameSe4 = 18,
			GameSe5 = 19,
			GameSe6 = 20,
			GameSe7 = 21,
			GameSe8 = 22,
			GameSe9 = 23,
			GameSe10 = 24,
			GameSe11 = 25,
			GameSe12 = 26,
			GameSe13 = 27,
			GameSe14 = 28,
			GameSe15 = 29,
			GameSe16 = 30,
			GameSeEnd = 30,
			BreakSe = 31,
			SlideSe = 32,
			ExSe = 33,
			CenterSe = 34,
			Chear = 35,
			TouchHoldLoop = 36,
			Sound_Sepalate_End = 37,
			Sound_Sepalate_Right = 38,
			Sound_Sepalate_Finish = 69,
			End = 70,
			Sound_Sepalate_Num = 33,
			SePlayerNum = 4,
			GameSePlayerNum = 16
		}

		public struct PlayCueInfo
		{
			public enum PausePhase
			{
				Pause,
				Play,
				None
			}

			public bool IsEmpty;

			public int AcbID;

			public int PlayerID;

			public int CueID;

			public bool Prepare;

			public int Target;

			public int StartTime;

			public float Volume;

			public PausePhase Pause;

			public PlayCueInfo(int acbID, int playerID, int cueID, bool prepare, int target, int startTime, float volume)
			{
				IsEmpty = false;
				AcbID = acbID;
				PlayerID = playerID;
				CueID = cueID;
				Prepare = prepare;
				Target = target;
				StartTime = startTime;
				Volume = volume;
				Pause = PausePhase.None;
			}

			public void SetPause(bool flg)
			{
				Pause = ((!flg) ? PausePhase.Play : PausePhase.Pause);
			}

			public void Clear()
			{
				IsEmpty = true;
				CueID = -1;
				Target = -1;
				Pause = PausePhase.None;
			}
		}

		public enum AcbID
		{
			Default,
			Music,
			Voice_1P,
			Voice_2P,
			Partner_Voice_1P,
			Partner_Voice_2P,
			Max
		}

		public static PlayCueInfo[] PlayingCueInfoList = new PlayCueInfo[70];

		private static bool _initialized = false;

		public static int[] PlayingCueBgm = new int[2];

		private static readonly int[] SeCueRotateIndex = new int[2];

		private static readonly int[] GameSeCueRotateIndex = new int[2];

		private static readonly SoundCtrl.PlaySetting Setting = new SoundCtrl.PlaySetting();

		public static string GetMusicCueName(int musicID)
		{
			return Singleton<OptionDataManager>.Instance.GetSoundDataPath($"music{musicID:000000}");
		}

		public static string GetVoiceCueName(int voiceID)
		{
			return Singleton<OptionDataManager>.Instance.GetSoundDataPath($"Voice_{voiceID:000000}");
		}

		public static string GetPartnerVoiceCueName(int voiceID)
		{
			return Singleton<OptionDataManager>.Instance.GetSoundDataPath($"Voice_Partner_{voiceID:000000}");
		}

		public static void PlaySystemSE(Mai2.Mai2Cue.Cue cueIndex)
		{
			Play(AcbID.Default, PlayerID.SystemStart, (int)cueIndex, prepare: false, 2, 0, 1f);
		}

		public static PlayerID PlaySE(Mai2.Mai2Cue.Cue cueIndex, int target)
		{
			if (target == 2)
			{
				for (int i = 0; i < 2; i++)
				{
					PlaySE(cueIndex, i);
				}
				return PlayerID.End;
			}
			PlayerID playerID = GetPlayerID(PlayerID.SeStart, target);
			Play(AcbID.Default, playerID, (int)cueIndex, prepare: false, target);
			return playerID;
		}

		public static PlayerID PlayLoopSE(Mai2.Mai2Cue.Cue cueIndex, int target, int id = 0)
		{
			if (id > 1)
			{
				id = 0;
			}
			PlayerID playerID = GetPlayerID((PlayerID)(13 + id), target);
			Play(AcbID.Default, playerID, (int)cueIndex, prepare: false, target);
			return playerID;
		}

		public static PlayerID PlayGameSE(Mai2.Mai2Cue.Cue cueIndex, int target, float volume)
		{
			PlayerID playerID = GetPlayerID(PlayerID.GameSeStart, target);
			Play(AcbID.Default, playerID, (int)cueIndex, prepare: false, target, 0, volume);
			return playerID;
		}

		public static PlayerID PlayGameSingleSe(Mai2.Mai2Cue.Cue cueIndex, int target, PlayerID player, float volume)
		{
			PlayerID playerID = GetPlayerID(player, target);
			Stop(playerID);
			Play(AcbID.Default, playerID, (int)cueIndex, prepare: false, target, 0, volume);
			return playerID;
		}

		public static void StopGameSingleSe(int target, PlayerID playID)
		{
			Stop(GetPlayerID(playID, target));
		}

		public static void StopSE(PlayerID playID)
		{
			Stop(playID);
		}

		public static void StopLoopSe(int target, int id = 0)
		{
			if (id > 1)
			{
				id = 0;
			}
			Stop(GetPlayerID((PlayerID)(13 + id), target));
		}

		public static PlayerID PlayVoice(Mai2.Voice_000001.Cue cueIndex, int target)
		{
			if (target < 3)
			{
				_ = 0;
			}
			if (target == 2)
			{
				for (int i = 0; i < 2; i++)
				{
					PlayVoice(cueIndex, i);
				}
				return PlayerID.End;
			}
			PlayerID playerID = GetPlayerID(PlayerID.Voice, target);
			Play((target == 0) ? AcbID.Voice_1P : AcbID.Voice_2P, playerID, (int)cueIndex, prepare: false, target);
			return playerID;
		}

		public static PlayerID PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue cueIndex, int target)
		{
			if (target < 3)
			{
				_ = 0;
			}
			if (target == 2)
			{
				for (int i = 0; i < 2; i++)
				{
					PlayPartnerVoice(cueIndex, i);
				}
				return PlayerID.End;
			}
			PlayerID playerID = GetPlayerID(PlayerID.Voice, target);
			Play((target == 0) ? AcbID.Partner_Voice_1P : AcbID.Partner_Voice_2P, playerID, (int)cueIndex, prepare: false, target);
			return playerID;
		}

		public static void StopVoice(int target)
		{
			Stop(GetPlayerID(PlayerID.Voice, target));
		}

		public static PlayerID PlayJingle(Mai2.Mai2Cue.Cue cueIndex, int target)
		{
			PlayerID playerID = GetPlayerID(PlayerID.Jingle, target);
			Play(AcbID.Default, playerID, (int)cueIndex, prepare: false, target);
			return playerID;
		}

		public static void StopJingle(int target)
		{
			Stop(GetPlayerID(PlayerID.Jingle, target));
		}

		public static void PlayBGM(Mai2.Mai2Cue.Cue cueIndex, int target)
		{
			if (target == 2)
			{
				for (int i = 0; i < 2; i++)
				{
					if (PlayingCueBgm[i] != (int)cueIndex)
					{
						Play(AcbID.Default, GetPlayerID(PlayerID.Sound_Sepalate_Start, i), (int)cueIndex, prepare: false, i);
						PlayingCueBgm[i] = (int)cueIndex;
					}
				}
			}
			else if (PlayingCueBgm[target] != (int)cueIndex)
			{
				Play(AcbID.Default, GetPlayerID(PlayerID.Sound_Sepalate_Start, target), (int)cueIndex, prepare: false, target);
				PlayingCueBgm[target] = (int)cueIndex;
			}
		}

		public static void StopBGM(int target)
		{
			if (target == 2)
			{
				for (int i = 0; i < 2; i++)
				{
					Stop(GetPlayerID(PlayerID.Sound_Sepalate_Start, i));
					PlayingCueBgm[i] = -1;
				}
			}
			else
			{
				Stop(GetPlayerID(PlayerID.Sound_Sepalate_Start, target));
				PlayingCueBgm[target] = -1;
			}
		}

		public static void SetVoiceCue(int target, int voiceID)
		{
			SoundCtrl instance = Singleton<SoundCtrl>.Instance;
			instance.Stop(6);
			instance.LoadCueSheet((target == 0) ? 2 : 3, GetVoiceCueName(voiceID));
		}

		public static void SetPartnerVoiceCue(int target, int voiceID)
		{
			SoundCtrl instance = Singleton<SoundCtrl>.Instance;
			instance.Stop(6);
			instance.LoadCueSheet((target == 0) ? 4 : 5, GetPartnerVoiceCueName(voiceID));
		}

		public static void PreviewPlay(int cueId)
		{
			SoundCtrl instance = Singleton<SoundCtrl>.Instance;
			instance.Stop(2);
			instance.Pause(2, onOff: false);
			instance.LoadCueSheet(1, GetMusicCueName(cueId));
			Play(AcbID.Music, PlayerID.Music, 1, prepare: false, 2);
		}

		public static void PreviewEnd()
		{
			Stop(PlayerID.Music);
		}

		public static void MusicPrepare(int musicID, bool prepare = true)
		{
			SoundCtrl instance = Singleton<SoundCtrl>.Instance;
			instance.Stop(2);
			instance.Pause(2, onOff: false);
			instance.LoadCueSheet(1, GetMusicCueName(musicID));
			Play(AcbID.Music, PlayerID.Music, 0, prepare, 2);
		}

		public static bool IsMusicPrepare()
		{
			return Singleton<SoundCtrl>.Instance.IsPlaying(2);
		}

		public static void StartMusic()
		{
			Pause(PlayerID.Music, onOff: false);
		}

		public static bool IsEndMusic()
		{
			return !IsPlaying(PlayerID.Music);
		}

		public static void PauseMusic(bool pause)
		{
			Pause(PlayerID.Music, pause);
		}

		public static void SeekMusic(int seekMsec)
		{
			Seek(seekMsec);
		}

		public static void StopMusic()
		{
			Stop(PlayerID.Music);
		}

		private static PlayerID GetPlayerID(PlayerID playerID, int monitorIndex)
		{
			PlayerID playerID2 = ((monitorIndex == 0) ? playerID : (playerID + 33));
			switch (playerID)
			{
			case PlayerID.Begin:
			case PlayerID.Error:
			case PlayerID.Music:
			case PlayerID.SystemStart:
			case PlayerID.SystemEnd:
				return playerID;
			case PlayerID.SeStart:
				playerID2 += SeCueRotateIndex[monitorIndex];
				SeCueRotateIndex[monitorIndex] = (SeCueRotateIndex[monitorIndex] + 1) % 4;
				return playerID2;
			case PlayerID.GameSeStart:
				playerID2 += GameSeCueRotateIndex[monitorIndex];
				GameSeCueRotateIndex[monitorIndex] = (GameSeCueRotateIndex[monitorIndex] + 1) % 16;
				return playerID2;
			default:
				if (monitorIndex != 0)
				{
					return playerID + 33;
				}
				return playerID;
			}
		}

		public static void Initialize()
		{
			SoundCtrl instance = Singleton<SoundCtrl>.Instance;
			SoundCtrl.InitParam param = new SoundCtrl.InitParam
			{
				AcfFilePath = Singleton<OptionDataManager>.Instance.GetSoundDataPath("Mai2") + ".acf",
				PlayerNum = 70
			};
			instance.Initialize(param);
			instance.LoadCueSheet(0, Singleton<OptionDataManager>.Instance.GetSoundDataPath("Mai2Cue"));
			SetVoiceCue(0, 1);
			SetVoiceCue(1, 1);
			SetPartnerVoiceCue(0, 17);
			SetPartnerVoiceCue(1, 17);
			for (int i = 0; i < 2; i++)
			{
				SeCueRotateIndex[i] = 0;
				GameSeCueRotateIndex[i] = 0;
				PlayingCueBgm[i] = -1;
			}
			for (int j = 0; j < PlayingCueInfoList.Length; j++)
			{
				PlayingCueInfoList[j].Clear();
			}
			_initialized = true;
		}

		public static bool IsInitialized()
		{
			return _initialized;
		}

		public static void Execute()
		{
			PlayExecute();
			Singleton<SoundCtrl>.Instance.Execute();
		}

		public static void Terminate()
		{
			Singleton<SoundCtrl>.Instance.Terminate();
		}

		public static void Play(AcbID acbID, PlayerID playerID, int cueID, bool prepare, int target, int startTime = 0, float volume = -1f)
		{
			PlayCueInfo[] playingCueInfoList = PlayingCueInfoList;
			for (int i = 0; i < playingCueInfoList.Length; i++)
			{
				PlayCueInfo playCueInfo = playingCueInfoList[i];
				if (!playCueInfo.IsEmpty && playCueInfo.Target == target && playCueInfo.CueID == cueID)
				{
					return;
				}
			}
			PlayingCueInfoList[(int)playerID] = new PlayCueInfo((int)acbID, (int)playerID, cueID, prepare, target, startTime, volume);
		}

		public static void PlayExecute()
		{
			SoundCtrl instance = Singleton<SoundCtrl>.Instance;
			for (int i = 0; i < PlayingCueInfoList.Length; i++)
			{
				PlayCueInfo playCueInfo = PlayingCueInfoList[i];
				if (!playCueInfo.IsEmpty && playCueInfo.Target != -1 && playCueInfo.CueID != -1)
				{
					instance.Stop(playCueInfo.PlayerID);
					Setting.AcbID = playCueInfo.AcbID;
					Setting.PlayerID = playCueInfo.PlayerID;
					Setting.CueIndex = playCueInfo.CueID;
					Setting.Prepare = playCueInfo.Prepare;
					Setting.StartTime = playCueInfo.StartTime;
					Setting.IsFader = false;
					Setting.FadeInTime = 0;
					Setting.FadeOutTime = 0;
					Setting.SpekerTarget = playCueInfo.Target;
					Setting.Volume = playCueInfo.Volume;
					instance.Play(Setting);
					if (Setting.Prepare && playCueInfo.Pause != PlayCueInfo.PausePhase.None)
					{
						instance.Pause(Setting.PlayerID, playCueInfo.Pause == PlayCueInfo.PausePhase.Pause);
					}
					PlayingCueInfoList[i].Clear();
				}
			}
		}

		public static bool IsPlaying(PlayerID playerID)
		{
			return Singleton<SoundCtrl>.Instance.IsPlaying((int)playerID);
		}

		public static void Stop(PlayerID playerID)
		{
			Singleton<SoundCtrl>.Instance.Stop((int)playerID);
			PlayingCueInfoList[(int)playerID].Clear();
		}

		public static void Seek(int seekMsec)
		{
			Singleton<SoundCtrl>.Instance.Stop(2);
			Play(AcbID.Music, PlayerID.Music, 0, prepare: true, 2, seekMsec);
		}

		public static void StopAll()
		{
			Singleton<SoundCtrl>.Instance.StopAll();
			for (int i = 0; i < PlayingCueInfoList.Length; i++)
			{
				PlayingCueInfoList[i].Clear();
			}
		}

		public static void Pause(PlayerID playerID, bool onOff)
		{
			SoundCtrl instance = Singleton<SoundCtrl>.Instance;
			if (!PlayingCueInfoList[(int)playerID].IsEmpty)
			{
				PlayingCueInfoList[(int)playerID].SetPause(onOff);
			}
			instance.Pause((int)playerID, onOff);
		}

		public static void SetHeadPhoneVolume(int playerID, float volume)
		{
			Singleton<SoundCtrl>.Instance.SetHeadPhoneVolume(playerID, volume);
		}

		public static void SetMasterVolume(int volume)
		{
			Singleton<SoundCtrl>.Instance.SetMasterVolume((float)volume / 100f);
		}

		public static void ResetMasterVolume()
		{
			Singleton<SoundCtrl>.Instance.ResetMasterVolume();
		}
	}
}
