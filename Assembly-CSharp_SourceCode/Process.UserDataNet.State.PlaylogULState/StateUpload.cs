using MAI2.Util;
using Manager;
using Net.Packet.Helper;
using Net.Packet.Mai2;

namespace Process.UserDataNet.State.PlaylogULState
{
	public class StateUpload : ProcessState<PlaylogULProcess>
	{
		public override void Init(params object[] args)
		{
			int num = (int)args[0];
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(num);
			PacketHelper.StartPacket(new PacketUploadUserPlaylog(num, userData, -1, delegate
			{
				Process.NextUserIndex();
			}, Process.OnError));
		}
	}
}
