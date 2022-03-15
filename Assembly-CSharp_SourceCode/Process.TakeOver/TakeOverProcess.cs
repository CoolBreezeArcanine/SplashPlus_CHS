using DB;
using MAI2.Util;
using Manager;
using Monitor.TakeOver;
using UnityEngine;

namespace Process.TakeOver
{
	public class TakeOverProcess : ProcessBase
	{
		private enum TakeOverState : byte
		{
			Wait,
			UserWait,
			InheritMusicWait,
			Released
		}

		private TakeOverMonitor[] _monitors;

		private TakeOverState _state;

		public uint[] _rom_version = new uint[2];

		public TakeOverMonitor.MajorRomVersion[] _major_version = new TakeOverMonitor.MajorRomVersion[2];

		public bool[] _isActive = new bool[2];

		public bool _isArgument;

		public bool[] _isMajor = new bool[2];

		public bool[] _isMinor = new bool[2];

		public TakeOverProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public TakeOverProcess(ProcessDataContainer dataContainer, params object[] args)
			: base(dataContainer)
		{
			if (args == null)
			{
				return;
			}
			uint num = (uint)args.Length;
			switch (num)
			{
			case 2u:
			{
				for (int j = 0; j < 2; j++)
				{
					_rom_version[j] = (uint)args[j];
					TakeOverMajorVersion takeOverMajorVersion2 = new TakeOverMajorVersion();
					_major_version[j] = takeOverMajorVersion2.GetMajorRomVersion(_rom_version[j]);
				}
				_isArgument = true;
				break;
			}
			case 6u:
			{
				for (int i = 0; i < 2; i++)
				{
					uint num2 = (uint)i * (num / 2u);
					_rom_version[i] = (uint)args[num2];
					TakeOverMajorVersion takeOverMajorVersion = new TakeOverMajorVersion();
					_major_version[i] = takeOverMajorVersion.GetMajorRomVersion(_rom_version[i]);
					_isMajor[i] = (bool)args[num2 + 1];
					_isMinor[i] = (bool)args[num2 + 2];
				}
				_isArgument = true;
				break;
			}
			}
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
			GameObject prefs = Resources.Load<GameObject>("Process/TakeOver/TakeOverProcess");
			_monitors = new TakeOverMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<TakeOverMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<TakeOverMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
				bool flag = false;
				flag = userData.IsEntry && (userData.UserType == UserData.UserIDType.Inherit || _rom_version[i] != 0);
				if (flag && userData.IsEntry && userData.UserType == UserData.UserIDType.Exist && !_isMajor[i])
				{
					flag = false;
				}
				_monitors[i].Initialize(i, flag);
				_monitors[i].SetProcessManager(container.processManager);
				_monitors[i]._rom_version = _rom_version[i];
				_monitors[i]._major_version = _major_version[i];
				_isActive[i] = flag;
			}
			container.processManager.NotificationFadeIn();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			switch (_state)
			{
			case TakeOverState.Wait:
				_state = TakeOverState.UserWait;
				break;
			case TakeOverState.UserWait:
				if (_monitors[0].IsState(TakeOverMonitor.DispState.End) && _monitors[1].IsState(TakeOverMonitor.DispState.End))
				{
					_state = TakeOverState.InheritMusicWait;
				}
				break;
			case TakeOverState.InheritMusicWait:
			{
				bool flag = false;
				for (int i = 0; i < _monitors.Length; i++)
				{
					UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
					if (userData.IsEntry && !userData.IsGuest() && (userData.UserType == UserData.UserIDType.Inherit || userData.UserType == UserData.UserIDType.New))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					bool num = (bool)container.processManager.SendMessage(new Message(ProcessType.NetworkProcess, 200));
					bool flag2 = (bool)container.processManager.SendMessage(new Message(ProcessType.NetworkProcess, 300));
					if (num || flag2)
					{
						_state = TakeOverState.Released;
						if (!_isArgument)
						{
							container.processManager.AddProcess(new FadeProcess(container, this, new NameEntryProcess(container)), 50);
						}
						else
						{
							container.processManager.AddProcess(new FadeProcess(container, this, new NameEntryProcess(container, _rom_version[0], _rom_version[1])), 50);
						}
						container.processManager.SetVisibleTimers(isVisible: false);
					}
				}
				else
				{
					_state = TakeOverState.Released;
					if (!_isArgument)
					{
						container.processManager.AddProcess(new FadeProcess(container, this, new PlInformationProcess(container)), 50);
					}
					else
					{
						container.processManager.AddProcess(new FadeProcess(container, this, new PlInformationProcess(container, _rom_version[0], _rom_version[1])), 50);
					}
					container.processManager.SetVisibleTimers(isVisible: false);
				}
				break;
			}
			}
			for (int j = 0; j < _monitors.Length; j++)
			{
				if (!_monitors[j].IsState(TakeOverMonitor.DispState.End))
				{
					_monitors[j].ViewUpdate();
				}
			}
		}

		protected override void UpdateInput(int monitorId)
		{
		}

		public override void OnLateUpdate()
		{
			if (Singleton<UserDataManager>.Instance.IsSingleUser())
			{
				return;
			}
			for (int i = 0; i < _monitors.Length; i++)
			{
				if (_monitors[i].IsState(TakeOverMonitor.DispState.EndWait))
				{
					container.processManager.EnqueueMessage(i, WindowMessageID.PlayPreparationWait, WindowPositionID.Middle);
				}
			}
		}
	}
}
