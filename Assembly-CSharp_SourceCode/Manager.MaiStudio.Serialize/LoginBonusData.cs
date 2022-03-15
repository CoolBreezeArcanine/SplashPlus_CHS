using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class LoginBonusData : SerializeBase, ISerialize
	{
		public StringID name;

		public LoginBonusType BonusType;

		public StringID PartnerId;

		public StringID CharacterId;

		public StringID MusicId;

		public StringID TitleId;

		public StringID PlateId;

		public StringID IconId;

		public bool IsCollabo;

		public StringID OpenEventId;

		public StringID netOpenName;

		public string dataName;

		public LoginBonusData()
		{
			name = new StringID();
			BonusType = LoginBonusType.Partner;
			PartnerId = new StringID();
			CharacterId = new StringID();
			MusicId = new StringID();
			TitleId = new StringID();
			PlateId = new StringID();
			IconId = new StringID();
			IsCollabo = false;
			OpenEventId = new StringID();
			netOpenName = new StringID();
			dataName = "";
		}

		public static explicit operator Manager.MaiStudio.LoginBonusData(LoginBonusData sz)
		{
			Manager.MaiStudio.LoginBonusData loginBonusData = new Manager.MaiStudio.LoginBonusData();
			loginBonusData.Init(sz);
			return loginBonusData;
		}

		public override void AddPath(string parentPath)
		{
			name.AddPath(parentPath);
			PartnerId.AddPath(parentPath);
			CharacterId.AddPath(parentPath);
			MusicId.AddPath(parentPath);
			TitleId.AddPath(parentPath);
			PlateId.AddPath(parentPath);
			IconId.AddPath(parentPath);
			OpenEventId.AddPath(parentPath);
			netOpenName.AddPath(parentPath);
		}

		public int GetID()
		{
			return name.id;
		}

		public void SetPriority(int pri)
		{
		}

		public bool IsDisable()
		{
			return false;
		}
	}
}
