using System.Collections.Generic;
using MAI2.Util;
using Manager;
using Manager.UserDatas;
using Monitor.GetMusic;
using Net.VO.Mai2;
using Process.MusicSelectInfo;
using UnityEngine;

namespace Process
{
	public class GetMusicProcess : ProcessBase
	{
		private enum GetMusicState : byte
		{
			Wait,
			GotoEnd,
			Disp,
			Released
		}

		private GetMusicMonitor[] _monitors;

		private GetMusicState _state;

		private readonly List<int>[] infoList = new List<int>[2];

		public GetMusicProcess(ProcessDataContainer dataContainer)
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
			GameObject prefs = Resources.Load<GameObject>("Process/GetMusic/GetMusicProcess");
			_monitors = new GetMusicMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<GetMusicMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<GetMusicMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].Initialize(i, active: true);
				_monitors[i].SetAssetManager(container.assetManager);
				infoList[i] = new List<int>();
				infoList[i].Clear();
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
				if (!userData.IsEntry)
				{
					continue;
				}
				foreach (Manager.UserDatas.UserItem present in userData.PresentList)
				{
					if (present.stock <= 0 || !present.isValid)
					{
						continue;
					}
					ItemKind itemType = (ItemKind)0;
					int itemId = 0;
					if (Singleton<DataManager>.Instance.ConvertPresentID2Item(present.itemId, out itemId, out itemType))
					{
						switch (itemType)
						{
						case ItemKind.Music:
							infoList[i].Add(itemId);
							userData.AddUnlockMusic(UserData.MusicUnlock.Base, itemId);
							break;
						case ItemKind.Icon:
							userData.AddCollections(UserData.Collection.Icon, itemId, addNewGet: true);
							break;
						case ItemKind.Plate:
							userData.AddCollections(UserData.Collection.Plate, itemId, addNewGet: true);
							break;
						case ItemKind.Title:
							userData.AddCollections(UserData.Collection.Title, itemId, addNewGet: true);
							break;
						case ItemKind.Partner:
							userData.AddCollections(UserData.Collection.Partner, itemId, addNewGet: true);
							break;
						case ItemKind.Frame:
							userData.AddCollections(UserData.Collection.Frame, itemId, addNewGet: true);
							break;
						case ItemKind.Ticket:
							userData.AddCollections(UserData.Collection.Ticket, itemId, addNewGet: true);
							break;
						case ItemKind.Character:
							userData.AddCollections(UserData.Collection.Chara, itemId, addNewGet: true);
							break;
						}
						present.stock = 0;
					}
				}
				_monitors[i].SetInfoList(infoList[i]);
			}
			if (infoList[0].Count == 0 && infoList[1].Count == 0)
			{
				_state = GetMusicState.GotoEnd;
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
			case GetMusicState.GotoEnd:
				_state = GetMusicState.Released;
				if (!Singleton<UserDataManager>.Instance.IsPlayCountEnouth(3))
				{
					container.processManager.AddProcess(new TutorialSelectProces(container), 50);
				}
				else
				{
					container.processManager.AddProcess(new MusicSelectInfoProcess(container), 50);
				}
				container.processManager.ReleaseProcess(this);
				break;
			case GetMusicState.Wait:
				_state = GetMusicState.Disp;
				break;
			case GetMusicState.Disp:
				if (_monitors[0].IsState(GetMusicMonitor.DispState.End) && _monitors[1].IsState(GetMusicMonitor.DispState.End))
				{
					_state = GetMusicState.Released;
					container.processManager.AddProcess(new FadeProcess(container, this, new MusicSelectInfoProcess(container)), 50);
				}
				break;
			}
			GetMusicMonitor[] monitors = _monitors;
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
					return false;
				}
			}
			return false;
		}
	}
}
