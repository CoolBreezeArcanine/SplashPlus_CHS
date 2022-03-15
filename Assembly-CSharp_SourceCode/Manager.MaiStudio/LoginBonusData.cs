using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class LoginBonusData : AccessorBase
	{
		public StringID name { get; private set; }

		public LoginBonusType BonusType { get; private set; }

		public StringID PartnerId { get; private set; }

		public StringID CharacterId { get; private set; }

		public StringID MusicId { get; private set; }

		public StringID TitleId { get; private set; }

		public StringID PlateId { get; private set; }

		public StringID IconId { get; private set; }

		public bool IsCollabo { get; private set; }

		public StringID OpenEventId { get; private set; }

		public StringID netOpenName { get; private set; }

		public string dataName { get; private set; }

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

		public void Init(Manager.MaiStudio.Serialize.LoginBonusData sz)
		{
			name = (StringID)sz.name;
			BonusType = sz.BonusType;
			PartnerId = (StringID)sz.PartnerId;
			CharacterId = (StringID)sz.CharacterId;
			MusicId = (StringID)sz.MusicId;
			TitleId = (StringID)sz.TitleId;
			PlateId = (StringID)sz.PlateId;
			IconId = (StringID)sz.IconId;
			IsCollabo = sz.IsCollabo;
			OpenEventId = (StringID)sz.OpenEventId;
			netOpenName = (StringID)sz.netOpenName;
			dataName = sz.dataName;
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
