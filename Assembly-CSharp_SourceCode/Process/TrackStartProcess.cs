using DB;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Manager.Party.Party;
using Monitor;
using UnityEngine;

namespace Process
{
	public class TrackStartProcess : ProcessBase
	{
		public enum TrackStartSequence
		{
			Init,
			Wait,
			Disp,
			DispEnd,
			Release
		}

		private TrackStartSequence _state = TrackStartSequence.Wait;

		private TrackStartMonitor[] _monitors;

		private float _timeCounter;

		public TrackStartProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/TrackStart/TrackStartProcess");
			_monitors = new TrackStartMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<TrackStartMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<TrackStartMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].Initialize(i, Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry);
				_monitors[i].SetAssetManager(container.assetManager);
				if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
				{
					int num = GameManager.SelectMusicID[i];
					int num2 = GameManager.SelectDifficultyID[i];
					MusicData music = Singleton<DataManager>.Instance.GetMusic(num);
					Notes notes = null;
					MusicDifficultyID musicDifficultyID = (MusicDifficultyID)num2;
					MessageMusicData messageMusicData = new MessageMusicData(level: (((uint)musicDifficultyID > 4u) ? Singleton<DataManager>.Instance.GetMusic(num).notesData[0] : Singleton<DataManager>.Instance.GetMusic(num).notesData[num2]).musicLevelID, jacket: container.assetManager.GetJacketTexture2D(music.jacketFile), name: music.name.str, difficulty: num2, kind: GameManager.GetScoreKind(num));
					container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 20002, i, messageMusicData));
				}
			}
			container.processManager.SendMessage(new Message(ProcessType.CommonProcess, 30001));
			if (IsPartyPlay())
			{
				Party.Get().BeginPlay();
			}
		}

		public override void OnUpdate()
		{
			switch (_state)
			{
			case TrackStartSequence.Init:
				_state = TrackStartSequence.Wait;
				break;
			case TrackStartSequence.Wait:
			{
				_timeCounter += Time.deltaTime;
				if (!(_timeCounter >= 1f))
				{
					break;
				}
				_timeCounter = 0f;
				_state = TrackStartSequence.Disp;
				for (int i = 0; i < _monitors.Length; i++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
					{
						if (_monitors[i]._isChallenge)
						{
							_monitors[i].SetTrackStart(TrackStartMonitor.TrackStartType.Challenge);
						}
						else if (_monitors[i]._isCourse)
						{
							_monitors[i].SetTrackStart(TrackStartMonitor.TrackStartType.Course);
						}
						else if (GameManager.SelectGhostID[i] == GhostManager.GhostTarget.BossGhost_1P || GameManager.SelectGhostID[i] == GhostManager.GhostTarget.BossGhost_2P)
						{
							_monitors[i].SetTrackStart(TrackStartMonitor.TrackStartType.BossBattle);
						}
						else if (GameManager.SelectGhostID[i] != GhostManager.GhostTarget.End)
						{
							_monitors[i].SetTrackStart(TrackStartMonitor.TrackStartType.Versus);
						}
						else
						{
							_monitors[i].SetTrackStart(TrackStartMonitor.TrackStartType.Normal);
						}
					}
				}
				container.processManager.NotificationFadeIn();
				break;
			}
			case TrackStartSequence.Disp:
				if (!_monitors[0].IsEnd() || !_monitors[1].IsEnd())
				{
					break;
				}
				if (IsPartyPlay())
				{
					if (Party.Get().IsAllBeginPlay())
					{
						_state = TrackStartSequence.DispEnd;
					}
				}
				else
				{
					_state = TrackStartSequence.DispEnd;
				}
				break;
			case TrackStartSequence.DispEnd:
				container.processManager.AddProcess(new FadeProcess(container, this, new GameProcess(container), FadeProcess.FadeType.Type3), 50);
				if (GameManager.IsFreedomMode)
				{
					container.processManager.SendMessage(new Message(ProcessType.PleaseWaitProcess, 20003, false));
				}
				_state = TrackStartSequence.Release;
				break;
			case TrackStartSequence.Release:
				return;
			}
			TrackStartMonitor[] monitors = _monitors;
			for (int j = 0; j < monitors.Length; j++)
			{
				monitors[j].ViewUpdate();
			}
		}

		public override void OnLateUpdate()
		{
		}

		public override void OnRelease()
		{
			for (int i = 0; i < _monitors.Length; i++)
			{
				Object.Destroy(_monitors[i].gameObject);
			}
		}

		private bool IsPartyPlay()
		{
			IManager manager = Party.Get();
			if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.MachineGroupID != MachineGroupID.OFF && manager != null && manager.IsJoinAndActive())
			{
				return true;
			}
			return false;
		}
	}
}
