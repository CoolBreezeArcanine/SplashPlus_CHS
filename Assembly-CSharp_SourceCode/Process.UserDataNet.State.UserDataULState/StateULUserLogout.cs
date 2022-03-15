using MAI2.Util;
using Manager;
using Net.Packet.Helper;
using Net.Packet.Mai2;

namespace Process.UserDataNet.State.UserDataULState
{
	public class StateULUserLogout : ProcessState<UserDataULProcess>
	{
		private ulong _userId;

		public override void Init(params object[] args)
		{
			int num = (int)args[0];
			_userId = Singleton<UserDataManager>.Instance.GetUserData(num).Detail.UserID;
			RequestUserLogout();
		}

		private void RequestUserLogout()
		{
			PacketHelper.StartPacket(new PacketUserLogout(_userId, delegate
			{
				Process.NextUserIndex();
			}));
		}
	}
}
