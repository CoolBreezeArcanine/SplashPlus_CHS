using System.Collections.Generic;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Monitor.UnlockMusic;
using UnityEngine;

namespace Process
{
	public class UnlockMusicProcess : ProcessBase
	{
		private enum UnlockMusicState : byte
		{
			Wait,
			GotoEnd,
			Disp,
			Released
		}

		private UnlockMusicMonitor[] _monitors;

		private UnlockMusicState _state;

		private List<UnlockMusicMonitor.InfoParam>[] infoList = new List<UnlockMusicMonitor.InfoParam>[2];

		public UnlockMusicProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				Object.Destroy(_monitors[i].gameObject);
			}
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/UnlockMusic/UnlockMusicProcess");
			_monitors = new UnlockMusicMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<UnlockMusicMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<UnlockMusicMonitor>()
			};
			bool flag = false;
			int num = 0;
			for (int i = 0; i < Singleton<GamePlayManager>.Instance.GetScoreListCount(); i++)
			{
				GameScoreList[] gameScoresAllMember = Singleton<GamePlayManager>.Instance.GetGameScoresAllMember(i);
				if (!IsAnyKaiden(gameScoresAllMember))
				{
					continue;
				}
				for (int j = 0; j < 4; j++)
				{
					if (gameScoresAllMember[j].IsEnable && gameScoresAllMember[j].IsHuman())
					{
						MusicData music = Singleton<DataManager>.Instance.GetMusic(gameScoresAllMember[j].SessionInfo.musicId);
						if (music.lockType == MusicLockType.Transmission)
						{
							flag = true;
							num = music.name.id;
							break;
						}
					}
				}
				if (flag)
				{
					break;
				}
			}
			UnlockMusicMonitor.InfoParam item = default(UnlockMusicMonitor.InfoParam);
			UnlockMusicMonitor.InfoParam item2 = default(UnlockMusicMonitor.InfoParam);
			for (int k = 0; k < _monitors.Length; k++)
			{
				_monitors[k].Initialize(k, active: true);
				_monitors[k].SetAssetManager(container.assetManager);
				infoList[k] = new List<UnlockMusicMonitor.InfoParam>();
				infoList[k].Clear();
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(k);
				if (!userData.IsEntry)
				{
					continue;
				}
				if (flag && !userData.IsUnlockMusic(UserData.MusicUnlock.Base, num))
				{
					item.musicID = num;
					item.infoKind = UnlockMusicMonitor.InfoKind.Transmission;
					infoList[k].Add(item);
					userData.AddUnlockMusic(UserData.MusicUnlock.Base, num);
					userData.Activity.TransmissionMusicGet(num);
				}
				foreach (int unlockMusic in Singleton<ScoreRankingManager>.Instance.GetUnlockMusicList(k))
				{
					if (!userData.IsUnlockMusic(UserData.MusicUnlock.Base, unlockMusic))
					{
						item2.musicID = unlockMusic;
						item2.infoKind = UnlockMusicMonitor.InfoKind.ScoreRanking;
						infoList[k].Add(item2);
						userData.AddUnlockMusic(UserData.MusicUnlock.Base, unlockMusic);
					}
				}
				_monitors[k].SetInfoList(infoList[k]);
			}
			if (infoList[0].Count == 0 && infoList[1].Count == 0)
			{
				_state = UnlockMusicState.GotoEnd;
			}
			else
			{
				container.processManager.NotificationFadeIn();
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (_state)
			{
			case UnlockMusicState.GotoEnd:
				_state = UnlockMusicState.Released;
				container.processManager.AddProcess(new CollectionGetProcess(container), 50);
				container.processManager.ReleaseProcess(this);
				break;
			case UnlockMusicState.Wait:
				_state = UnlockMusicState.Disp;
				break;
			case UnlockMusicState.Disp:
				if (_monitors[0].IsState(UnlockMusicMonitor.DispState.End) && _monitors[1].IsState(UnlockMusicMonitor.DispState.End))
				{
					_state = UnlockMusicState.Released;
					container.processManager.AddProcess(new FadeProcess(container, this, new CollectionGetProcess(container)), 50);
				}
				break;
			}
			UnlockMusicMonitor[] monitors = _monitors;
			for (int i = 0; i < monitors.Length; i++)
			{
				monitors[i].ViewUpdate();
			}
		}

		protected override void UpdateInput(int monitorId)
		{
		}

		public override void OnLateUpdate()
		{
		}

		public bool IsAnyKaiden(GameScoreList[] list)
		{
			foreach (GameScoreList gameScoreList in list)
			{
				if (gameScoreList.IsEnable && gameScoreList.IsHuman() && UdemaeID.Class_LEGEND == gameScoreList.MaxDan)
				{
					return true;
				}
			}
			return false;
		}
	}
}
