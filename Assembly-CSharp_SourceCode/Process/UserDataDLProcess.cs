using System;
using MAI2.Util;
using Manager;
using Net.Packet;
using Process.MapCore;
using Process.UserDataNet.State;
using Process.UserDataNet.State.UserDataDLState.Exist;
using Process.UserDataNet.State.UserDataDLState.Inherit;

namespace Process
{
	public class UserDataDLProcess : MapProcess
	{
		public enum MessageID
		{
			Kill = 100,
			IsDone = 200,
			IsError = 300
		}

		private enum StateType
		{
			ExistUserDownload = 10,
			ExistUserStore = 11,
			InheritMusicDownload = 20,
			InheritMusicStore = 21
		}

		private readonly ProcessContext<UserDataDLProcess> _context;

		private int _userIndex;

		private bool _isDone;

		private bool _isError;

		private readonly Action _state;

		public UserDataDLProcess(ProcessDataContainer container, bool isInherit = false)
			: base(container, ProcessType.NetworkProcess)
		{
			_context = new ProcessContext<UserDataDLProcess>(this);
			_context.AddState(10, new StateUserDownload());
			_context.AddState(11, new StateUserStore());
			_context.AddState(20, new StateInheritDownload());
			_context.AddState(21, new StateUserInheritStore());
			_state = StateUserDataCheck;
		}

		public override void OnStart()
		{
			if (!Singleton<OperationManager>.Instance.IsAliveServer)
			{
				TerminateProcess();
			}
			else
			{
				State = _state;
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			_context?.Execute((float)GameManager.GetGameMSecAdd() / 1000f);
		}

		private void StateUserDataCheck()
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(_userIndex);
			if (!userData.IsEntry)
			{
				ToNextUser();
				return;
			}
			switch (userData.UserType)
			{
			case UserData.UserIDType.Guest:
			case UserData.UserIDType.New:
				ToNextUser();
				break;
			case UserData.UserIDType.Exist:
				SetStateTerminate();
				_context.SetState(10, _userIndex);
				break;
			case UserData.UserIDType.Inherit:
				SetStateTerminate();
				_context.SetState(20, _userIndex);
				break;
			default:
				TerminateProcess();
				break;
			}
		}

		public void ToNextUser()
		{
			if (++_userIndex >= 2)
			{
				TerminateProcess();
			}
			else
			{
				State = _state;
			}
		}

		public void OnError(PacketStatus err)
		{
			TerminateProcess();
			_isDone = false;
			_isError = true;
		}

		private void TerminateProcess()
		{
			SetStateTerminate();
			_isDone = true;
		}

		public override object HandleMessage(Message message)
		{
			switch (message.Id)
			{
			case 100:
				container.processManager.ReleaseProcess(this);
				break;
			case 200:
				return _isDone;
			case 300:
				return _isError;
			}
			return null;
		}
	}
}
