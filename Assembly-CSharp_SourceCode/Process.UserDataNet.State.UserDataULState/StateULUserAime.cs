using MAI2.Util;
using Manager;
using Net.Packet;
using Net.Packet.Helper;
using Net.Packet.Mai2;
using UnityEngine;

namespace Process.UserDataNet.State.UserDataULState
{
	public class StateULUserAime : ProcessState<UserDataULProcess>
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
				RequestUploadUserPortraitData();
			}
		}

		private void RequestUploadUserPortraitData()
		{
			if (GameManager.IsTakeFaceIcon[_index] && GameManager.FaceIconTexture[_index] != null)
			{
				ulong userID = _userData.Detail.UserID;
				string fname = $"{userID}.jpg";
				byte[] source = GameManager.FaceIconTexture[_index].EncodeToJPG(50);
				PacketHelper.StartPacket(new PacketUploadUserPortrait(userID, fname, source, delegate
				{
					RequestUploadUserPhotoData();
				}, delegate(PacketStatus err)
				{
					Process.NetworkError(err, _index);
				}));
			}
			else
			{
				RequestUploadUserPhotoData();
			}
		}

		private void RequestUploadUserPhotoData()
		{
			if (GameManager.IsUploadPhoto[_index] && GameManager.PhotoTrackNo[_index] != 0 && GameManager.PhotoTexture[_index] != null)
			{
				ulong userID = _userData.Detail.UserID;
				byte[] source = GameManager.PhotoTexture[_index].EncodeToJPG(100);
				PacketHelper.StartPacket(new PacketUploadUserPhoto(_index, userID, GameManager.PhotoTrackNo[_index], source, delegate
				{
					RequestUpsertUserAll();
				}, delegate(PacketStatus err)
				{
					Process.NetworkError(err, _index);
				}));
			}
			else
			{
				RequestUpsertUserAll();
			}
		}

		private void RequestUpsertUserAll()
		{
			PacketHelper.StartPacket(new PacketUpsertUserAll(_index, _userData, delegate(int code)
			{
				if (code == 1)
				{
					RequestUserLogout();
				}
				else
				{
					Process.UpsertError(_index);
					Process.NextUserIndex();
				}
			}, delegate(PacketStatus err)
			{
				Process.NetworkError(err, _index);
			}));
		}

		private void RequestUserLogout()
		{
			Context.SetState(2, _index);
		}
	}
}
