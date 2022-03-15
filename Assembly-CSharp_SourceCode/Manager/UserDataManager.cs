using System.Collections.Generic;
using System.Linq;
using Datas.DebugData;
using DB;
using MAI2.Util;
using Manager.MaiStudio;
using Manager.UserDatas;
using Util;

namespace Manager
{
	public class UserDataManager : Singleton<UserDataManager>
	{
		private readonly UserData[] _userData = new UserData[4];

		public UserData GetUserData(long index)
		{
			if (index >= _userData.Length)
			{
				return null;
			}
			return _userData[index];
		}

		public void SetPartyUserData(int index, long userId, string userName)
		{
			index = 2 + index;
			if (index < _userData.Length)
			{
				_userData[index].Detail.UserID = (ulong)userId;
				_userData[index].Detail.UserName = userName;
			}
		}

		public void SetGuestContentBit(long index)
		{
			if (index < 4)
			{
				for (int i = 0; i < 24; i++)
				{
					_userData[index].Detail.ContentBit.SetFlag((ContentBitID)i, ((ContentBitID)i).IsGuestIgnore());
				}
			}
		}

		public void SetDefault(long index)
		{
			if (index >= 4)
			{
				return;
			}
			UserDetail detail = GetUserData(index).Detail;
			detail.EquipIconID = 1;
			detail.EquipTitleID = 1;
			detail.EquipPlateID = 1;
			detail.EquipPartnerID = 17;
			detail.EquipFrameID = 1;
			detail.SelectMapID = 1;
			UserExtend extend = GetUserData(index).Extend;
			extend.ExtendContendBit.SetFlag(ExtendContentBitID.PhotoAgree, flag: true);
			extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoCodeRead, flag: true);
			extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoCharaSelect, flag: true);
			extend.ExtendContendBit.SetFlag(ExtendContentBitID.GotoIconPhotoShoot, flag: true);
			foreach (KeyValuePair<int, IconData> item in from x in Singleton<DataManager>.Instance.GetIcons()
				where x.Value.isDefault
				select x)
			{
				GetUserData(index).AddCollections(UserData.Collection.Icon, item.Value.GetID());
			}
			foreach (KeyValuePair<int, PlateData> item2 in from x in Singleton<DataManager>.Instance.GetPlates()
				where x.Value.isDefault
				select x)
			{
				GetUserData(index).AddCollections(UserData.Collection.Plate, item2.Value.GetID());
			}
			foreach (KeyValuePair<int, TitleData> item3 in from x in Singleton<DataManager>.Instance.GetTitles()
				where x.Value.isDefault
				select x)
			{
				GetUserData(index).AddCollections(UserData.Collection.Title, item3.Value.GetID());
			}
			foreach (KeyValuePair<int, PartnerData> item4 in from x in Singleton<DataManager>.Instance.GetPartners()
				where x.Value.isDefault
				select x)
			{
				GetUserData(index).AddCollections(UserData.Collection.Partner, item4.Value.GetID());
			}
			foreach (KeyValuePair<int, FrameData> item5 in from x in Singleton<DataManager>.Instance.GetFrames()
				where x.Value.isDefault
				select x)
			{
				GetUserData(index).AddCollections(UserData.Collection.Frame, item5.Value.GetID());
			}
			SetDefault_SPLASH(index);
		}

		public void SetDefault_SPLASH(long index)
		{
			UserOption option = GetUserData(index).Option;
			OptionVolumeID optionVolumeID = (option.DamageSeVolume = option.DamageSeVolume.GetDefault());
		}

		public void SetDefault_SPLASH_PLUS(long index)
		{
			UserOption option = GetUserData(index).Option;
			OptionDisprateID optionDisprateID = (option.DispRate = option.DispRate.GetDefault());
		}

		public UserDataManager()
		{
			for (int i = 0; i < 4; i++)
			{
				_userData[i] = new UserData();
			}
		}

		public void SetDebugUserData(long index, DebugUserData data)
		{
			if (index < _userData.Length)
			{
				_userData[index] = data.GetUserData();
				Singleton<MapMaster>.Instance.SetDebugData((int)index, data.mapList);
			}
		}

		public bool IsPlayCountEnouth(int count)
		{
			for (int i = 0; i < 2; i++)
			{
				if (!IsPlayCountEnouth(i, count))
				{
					return false;
				}
			}
			return true;
		}

		public bool IsPlayCountEnouth(int index, int count)
		{
			if (GetUserData(index).IsEntry && GetUserData(index).Detail.PlayCount < count)
			{
				return false;
			}
			return true;
		}

		public bool IsSingleUser()
		{
			int num = 0;
			for (int i = 0; i < 2; i++)
			{
				if (GetUserData(i).IsEntry)
				{
					num++;
				}
			}
			return num == 1;
		}
	}
}
