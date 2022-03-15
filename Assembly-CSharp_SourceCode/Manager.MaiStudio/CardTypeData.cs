using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class CardTypeData : AccessorBase
	{
		public StringID name { get; private set; }

		public string title { get; private set; }

		public int paramId { get; private set; }

		public bool disable { get; private set; }

		public string effectText { get; private set; }

		public CardKind kind { get; private set; }

		public StringID releaseId { get; private set; }

		public CardMapBonus mapBonus { get; private set; }

		public string imageFile { get; private set; }

		public string thumbnailName { get; private set; }

		public string frameImageFile { get; private set; }

		public string frameThumbnailName { get; private set; }

		public int priority { get; private set; }

		public int extendBitParameter { get; private set; }

		public string dataName { get; private set; }

		public CardTypeData()
		{
			name = new StringID();
			title = "";
			paramId = 0;
			disable = false;
			effectText = "";
			kind = CardKind.Pass;
			releaseId = new StringID();
			mapBonus = CardMapBonus.None;
			imageFile = "";
			thumbnailName = "";
			frameImageFile = "";
			frameThumbnailName = "";
			priority = 0;
			extendBitParameter = 0;
			dataName = "";
		}

		public void Init(Manager.MaiStudio.Serialize.CardTypeData sz)
		{
			name = (StringID)sz.name;
			title = sz.title;
			paramId = sz.paramId;
			disable = sz.disable;
			effectText = sz.effectText;
			kind = sz.kind;
			releaseId = (StringID)sz.releaseId;
			mapBonus = sz.mapBonus;
			imageFile = sz.imageFile;
			thumbnailName = sz.thumbnailName;
			frameImageFile = sz.frameImageFile;
			frameThumbnailName = sz.frameThumbnailName;
			priority = sz.priority;
			extendBitParameter = sz.extendBitParameter;
			dataName = sz.dataName;
		}

		public int GetID()
		{
			return name.id;
		}

		public void SetPriority(int pri)
		{
			priority = pri;
		}

		public bool IsDisable()
		{
			return disable;
		}
	}
}
