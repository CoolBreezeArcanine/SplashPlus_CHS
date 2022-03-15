using System.Collections.Generic;
using MAI2.Util;

namespace Manager
{
	public class SoundCtrl : Singleton<SoundCtrl>
	{
		public class InitParam
		{
			public string AcfFilePath;

			public int PlayerNum = 16;

			public Dictionary<int, int> UseSyncTimerPlayerIDs;

			public void AddUseSyncTimerPlayerID(int playerID)
			{
				if (UseSyncTimerPlayerIDs == null)
				{
					UseSyncTimerPlayerIDs = new Dictionary<int, int>();
				}
				UseSyncTimerPlayerIDs[playerID] = 1;
			}

			public InitParam Clone()
			{
				InitParam initParam = (InitParam)MemberwiseClone();
				if (UseSyncTimerPlayerIDs != null)
				{
					initParam.UseSyncTimerPlayerIDs = new Dictionary<int, int>(UseSyncTimerPlayerIDs);
				}
				return initParam;
			}
		}

		private class AcbObj
		{
			public CriAtomExAcb Acb;

			public void Dispose()
			{
				if (Acb != null)
				{
					Acb.Dispose();
				}
			}
		}

		private class AisacObj
		{
			public bool Enable;

			public float Value;
		}

		private class PlayerObj
		{
			public SoundTarget TargetID;

			public CriAtomExPlayer Player;

			public float Volume = 1f;

			public bool NeedUpdate;

			public readonly Dictionary<int, AisacObj> Aisacs = new Dictionary<int, AisacObj>();

			public CriAtomExPlayback LastPlayBack;

			public void Create(bool enableAudioSyncedTimer)
			{
				Player = new CriAtomExPlayer(enableAudioSyncedTimer);
			}

			public bool IsReady()
			{
				return Player != null;
			}

			public void Update()
			{
				if (IsReady() && NeedUpdate)
				{
					Player.UpdateAll();
					NeedUpdate = false;
				}
			}

			public void Dispose()
			{
				if (IsReady())
				{
					Player.Dispose();
				}
			}

			public void SetTarget(SoundTarget target)
			{
				TargetID = target;
			}

			public void SetAisac(int aisacID, float value)
			{
				if (IsReady())
				{
					if (!Aisacs.ContainsKey(aisacID))
					{
						Aisacs[aisacID] = new AisacObj();
					}
					Aisacs[aisacID].Enable = true;
					Aisacs[aisacID].Value = value;
					Player.SetAisacControl((uint)aisacID, value);
					NeedUpdate = true;
				}
			}

			public void ClearAisac(int aisacID)
			{
				if (Aisacs.ContainsKey(aisacID))
				{
					Aisacs[aisacID].Enable = false;
				}
			}

			public void ClearAisacAll()
			{
				foreach (KeyValuePair<int, AisacObj> aisac in Aisacs)
				{
					ClearAisac(aisac.Key);
				}
			}
		}

		public class PlaySetting
		{
			public int PlayerID;

			public int AcbID;

			public int CueIndex;

			public bool Prepare;

			public int StartTime;

			public bool IsFader;

			public int FadeInTime;

			public int FadeOutTime;

			public int SpekerTarget;

			public float Volume = -1f;

			public const float InvalidVol = -1f;
		}

		private InitParam _initParam;

		private Dictionary<int, AcbObj> _acbs = new Dictionary<int, AcbObj>();

		private Dictionary<int, PlayerObj> _players = new Dictionary<int, PlayerObj>();

		public float _masterVolume = 1f;

		private float[] _headPhoneVolume = new float[2];

		public void Initialize(InitParam param)
		{
			_initParam = param.Clone();
			CriAtomEx.RegisterAcf(null, _initParam.AcfFilePath);
			for (int i = 0; i < param.PlayerNum; i++)
			{
				PlayerObj playerObj = new PlayerObj();
				_players[i] = playerObj;
				bool enableAudioSyncedTimer = false;
				if (param.UseSyncTimerPlayerIDs != null && param.UseSyncTimerPlayerIDs.ContainsKey(i))
				{
					enableAudioSyncedTimer = true;
				}
				playerObj.Create(enableAudioSyncedTimer);
				if (i <= 4)
				{
					playerObj.SetTarget(SoundTarget.All);
					playerObj.SetAisac(4, _masterVolume);
					playerObj.SetAisac(5, _masterVolume);
					playerObj.SetAisac(2, _headPhoneVolume[0]);
					playerObj.SetAisac(3, _headPhoneVolume[1]);
				}
				else if (i <= 37)
				{
					playerObj.SetTarget(SoundTarget.Left);
					playerObj.SetAisac(4, _masterVolume);
					playerObj.SetAisac(5, 0f);
					playerObj.SetAisac(2, _headPhoneVolume[0]);
					playerObj.SetAisac(3, 0f);
				}
				else if (i <= 69)
				{
					playerObj.SetTarget(SoundTarget.Right);
					playerObj.SetAisac(4, 0f);
					playerObj.SetAisac(5, _masterVolume);
					playerObj.SetAisac(2, 0f);
					playerObj.SetAisac(3, _headPhoneVolume[1]);
				}
			}
		}

		public void Execute()
		{
			foreach (KeyValuePair<int, PlayerObj> player in _players)
			{
				player.Value.Update();
			}
		}

		public void Terminate()
		{
			foreach (KeyValuePair<int, PlayerObj> player in _players)
			{
				player.Value.Dispose();
			}
			UnloadCueSheetAll();
		}

		public bool LoadCueSheet(int id, string filePath)
		{
			bool result = false;
			UnloadCueSheet(id);
			CriAtomExAcb criAtomExAcb = CriAtomExAcb.LoadAcbFile(null, filePath + ".acb", filePath + ".awb");
			if (criAtomExAcb != null)
			{
				_acbs[id] = new AcbObj();
				_acbs[id].Acb = criAtomExAcb;
				result = true;
			}
			return result;
		}

		public void UnloadCueSheet(int id)
		{
			if (_acbs.ContainsKey(id))
			{
				_acbs[id].Dispose();
				_acbs.Remove(id);
			}
		}

		public void UnloadCueSheetAll()
		{
			foreach (KeyValuePair<int, PlayerObj> player in _players)
			{
				player.Value.Dispose();
			}
			_acbs.Clear();
		}

		private CriAtomExAcb GetCueSheet(int id)
		{
			CriAtomExAcb result = null;
			if (_acbs.ContainsKey(id))
			{
				result = _acbs[id].Acb;
			}
			return result;
		}

		public void Seek(int playerID, long seekMsec)
		{
			PlayerObj playerObj = _players[playerID];
			if (playerObj.IsReady())
			{
				playerObj.Player.SetStartTime(seekMsec);
				playerObj.Player.UpdateAll();
			}
		}

		public void Play(PlaySetting setting)
		{
			if (!_players.ContainsKey(setting.PlayerID) || !_acbs.ContainsKey(setting.AcbID))
			{
				return;
			}
			PlayerObj playerObj = _players[setting.PlayerID];
			if (!playerObj.IsReady())
			{
				return;
			}
			float num = 0f;
			num = ((setting.PlayerID == 3) ? setting.Volume : ((setting.Volume == -1f) ? _masterVolume : Adjust0_1(_masterVolume * setting.Volume)));
			switch (setting.SpekerTarget)
			{
			case 0:
				playerObj.SetAisac(4, Adjust0_1(num));
				break;
			case 1:
				playerObj.SetAisac(5, Adjust0_1(num));
				break;
			case 2:
				playerObj.SetAisac(4, Adjust0_1(num));
				playerObj.SetAisac(5, Adjust0_1(num));
				break;
			}
			playerObj.NeedUpdate = true;
			playerObj.Player.ResetParameters();
			playerObj.Player.SetCue(_acbs[setting.AcbID].Acb, setting.CueIndex);
			playerObj.Player.SetStartTime(setting.StartTime);
			foreach (KeyValuePair<int, AisacObj> aisac in playerObj.Aisacs)
			{
				if (aisac.Value.Enable)
				{
					playerObj.Player.SetAisacControl((uint)aisac.Key, aisac.Value.Value);
				}
			}
			if (setting.IsFader)
			{
				playerObj.Player.AttachFader();
				playerObj.Player.SetFadeInTime(setting.FadeInTime);
				playerObj.Player.SetFadeInTime(setting.FadeOutTime);
			}
			playerObj.NeedUpdate = false;
			if (setting.Prepare)
			{
				playerObj.Player.Pause();
			}
			playerObj.LastPlayBack = playerObj.Player.Start();
		}

		public void Stop(int playerID, bool forceStop = false)
		{
			if (_players.ContainsKey(playerID))
			{
				PlayerObj playerObj = _players[playerID];
				if (playerObj.IsReady())
				{
					playerObj.Player.Stop(forceStop);
				}
			}
		}

		public long GetPlayTiming(int playerID)
		{
			if (_players.ContainsKey(playerID))
			{
				PlayerObj playerObj = _players[playerID];
				if (playerObj.IsReady())
				{
					return playerObj.Player.GetTime();
				}
			}
			return -1L;
		}

		public void StopAll()
		{
			foreach (KeyValuePair<int, PlayerObj> player in _players)
			{
				Stop(player.Key, forceStop: true);
			}
		}

		public void Pause(int playerID, bool onOff)
		{
			if (_players.ContainsKey(playerID))
			{
				PlayerObj playerObj = _players[playerID];
				if (playerObj.IsReady())
				{
					playerObj.Player.Pause(onOff);
				}
			}
		}

		public void SetVolume(int playerID, float volume)
		{
			if (_players.ContainsKey(playerID))
			{
				PlayerObj playerObj = _players[playerID];
				if (playerObj.IsReady())
				{
					playerObj.Volume = Adjust0_1(volume);
					playerObj.Player.SetVolume(playerObj.Volume);
					playerObj.NeedUpdate = true;
				}
			}
		}

		public float Adjust0_1(float volume)
		{
			float num = volume;
			if (0f > num)
			{
				num = 0f;
			}
			if (num > 1f)
			{
				num = 1f;
			}
			return num;
		}

		public void SetAisac(int playerID, int aisacID, float value)
		{
			if (_players.ContainsKey(playerID))
			{
				PlayerObj playerObj = _players[playerID];
				if (playerObj.IsReady())
				{
					playerObj.SetAisac(aisacID, value);
					playerObj.NeedUpdate = true;
				}
			}
		}

		public void SetAisacAll(int aisacID, float value)
		{
			foreach (KeyValuePair<int, PlayerObj> player in _players)
			{
				if (player.Value.IsReady())
				{
					player.Value.SetAisac(aisacID, value);
					player.Value.NeedUpdate = true;
				}
			}
		}

		public void ClearAisac(int playerID, int aisacID)
		{
			if (_players.ContainsKey(playerID))
			{
				PlayerObj playerObj = _players[playerID];
				if (playerObj.IsReady())
				{
					playerObj.ClearAisac(aisacID);
				}
			}
		}

		public void ClearAisacAll(int playerID)
		{
			if (_players.ContainsKey(playerID))
			{
				PlayerObj playerObj = _players[playerID];
				if (playerObj.IsReady())
				{
					playerObj.ClearAisacAll();
				}
			}
		}

		public void SetMasterVolume(float volume)
		{
			_masterVolume = volume;
		}

		public void ResetMasterVolume()
		{
			_masterVolume = 1f;
		}

		public bool IsPlaying(int playerID)
		{
			bool result = false;
			if (_players.ContainsKey(playerID))
			{
				PlayerObj playerObj = _players[playerID];
				if (playerObj.IsReady() && CriAtomExPlayer.Status.Playing == playerObj.Player.GetStatus())
				{
					result = true;
				}
			}
			return result;
		}

		public bool IsPlayEnd(int playerID)
		{
			bool result = false;
			if (_players.ContainsKey(playerID))
			{
				PlayerObj playerObj = _players[playerID];
				if (playerObj.IsReady() && CriAtomExPlayer.Status.PlayEnd == playerObj.Player.GetStatus())
				{
					result = true;
				}
			}
			return result;
		}

		public void SetHeadPhoneVolume(int targerID, float volume)
		{
			_headPhoneVolume[targerID] = Adjust0_1(volume);
			foreach (KeyValuePair<int, PlayerObj> player in _players)
			{
				if (player.Value.IsReady() && (player.Value.TargetID == (SoundTarget)targerID || player.Value.TargetID == SoundTarget.All))
				{
					switch (targerID)
					{
					case 0:
						player.Value.SetAisac(2, volume);
						break;
					case 1:
						player.Value.SetAisac(3, volume);
						break;
					}
					player.Value.NeedUpdate = true;
				}
			}
		}
	}
}
