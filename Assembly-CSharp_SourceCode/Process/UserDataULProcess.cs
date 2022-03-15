using MAI2.Util;
using Manager;
using Net;
using Net.Packet;
using Process.MapCore;
using Process.UserDataNet.State;
using Process.UserDataNet.State.UserDataULState;

namespace Process
{
	public class UserDataULProcess : MapProcess
	{
		public enum StateType
		{
			UserGuest,
			UserAime,
			UserLogout
		}

		private ProcessContext<UserDataULProcess> _context;

		private int _userIndex;

		private readonly bool[] _isUpsertError = new bool[2];

		public bool IsDone { get; private set; }

		public bool IsError { get; private set; }

		public UserDataULProcess(ProcessDataContainer container)
			: base(container, ProcessType.NetworkProcess)
		{
		}

		public override void OnStart()
		{
			_context = new ProcessContext<UserDataULProcess>(this);
			_context.AddState(0, new StateULUserGuest());
			_context.AddState(1, new StateULUserAime());
			_context.AddState(2, new StateULUserLogout());
			State = StateUserDataCheck;
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
				NextUserIndex();
			}
			else if (UserID.IsGuest(userData.Detail.UserID))
			{
				SetStateTerminate();
				_context.SetState(0, _userIndex);
			}
			else
			{
				SetStateTerminate();
				_context.SetState(1, _userIndex);
			}
		}

		public void NextUserIndex()
		{
			if (++_userIndex >= 2)
			{
				TerminateProcess();
			}
			else
			{
				State = StateUserDataCheck;
			}
		}

		public void NetworkError(PacketStatus err, int index)
		{
			TerminateProcess();
			IsDone = false;
			IsError = true;
		}

		public void UpsertError(int index)
		{
			_isUpsertError[index] = true;
		}

		private void TerminateProcess()
		{
			SetStateTerminate();
			IsDone = true;
		}

		public void Kill()
		{
			container.processManager.ReleaseProcess(this);
		}

		public bool GetUpsertError(int index)
		{
			return _isUpsertError[index];
		}
	}
}
