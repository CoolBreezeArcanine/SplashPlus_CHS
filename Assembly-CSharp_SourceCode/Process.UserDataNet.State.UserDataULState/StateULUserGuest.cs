using MAI2.Util;
using Manager;
using Net.Packet;
using Net.Packet.Helper;
using Net.Packet.Mai2;

namespace Process.UserDataNet.State.UserDataULState
{
	public class StateULUserGuest : ProcessState<UserDataULProcess>
	{
		private int _index;

		private UserData _userData;

		private int _trackNo;

		public override void Init(params object[] args)
		{
			_index = (int)args[0];
			_userData = Singleton<UserDataManager>.Instance.GetUserData(_index);
			_trackNo = 0;
			RequestUploadUserPlayLogData();
		}

		private void RequestUploadUserPlayLogData()
		{
			if (_userData.IsEntry && !Singleton<GamePlayManager>.Instance.IsEmpty() && Singleton<GamePlayManager>.Instance.GetScoreListCount() > _trackNo)
			{
				PacketHelper.StartPacket(new PacketUploadUserPlaylog(_index, _userData, _trackNo, delegate
				{
					RequestUploadUserPlayLogData();
				}, delegate(PacketStatus err)
				{
					Process.NetworkError(err, _index);
				}));
				_trackNo++;
			}
			else
			{
				RequestUpsertUserAll();
			}
		}

		private void RequestUpsertUserAll()
		{
			PacketHelper.StartPacket(new PacketUpsertUserAll(_index, _userData, delegate
			{
				Process.NextUserIndex();
			}, delegate(PacketStatus err)
			{
				Process.NetworkError(err, _index);
			}));
		}
	}
}
