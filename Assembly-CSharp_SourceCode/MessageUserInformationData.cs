using DB;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Manager.UserDatas;
using UnityEngine;

public class MessageUserInformationData
{
	public Texture2D Icon { get; private set; }

	public Texture2D Plate { get; private set; }

	public Texture2D Frame { get; private set; }

	public bool IsKiraFrame { get; private set; }

	public Texture2D KiraFrame { get; private set; }

	public Texture2D KiraFrameMask { get; private set; }

	public string Name { get; private set; }

	public uint Rateing { get; private set; }

	public UdemaeID ClassID { get; private set; }

	public uint DaniID { get; private set; }

	public string Title { get; private set; }

	public Sprite TitleBg { get; private set; }

	public Sprite Pass { get; private set; }

	public OptionDisprateID DispRate { get; private set; }

	public int TotalAwake { get; private set; }

	public Material PassMaterial { get; private set; }

	public Color BaseColor { get; private set; }

	public bool IsSubMonitor { get; private set; }

	public MessageUserInformationData(int playerId, AssetManager manager, UserDetail data, OptionDisprateID dispRate, bool isSubMonitor)
	{
		Texture2D iconTexture2D = manager.GetIconTexture2D(playerId, data.EquipIconID);
		Texture2D plateTexture2D = manager.GetPlateTexture2D(data.EquipPlateID);
		Texture2D frameTexture2D = manager.GetFrameTexture2D(data.EquipFrameID);
		bool flag = false;
		Texture2D texture2D = null;
		Texture2D texture2D2 = null;
		FrameData frame = Singleton<DataManager>.Instance.GetFrame(data.EquipFrameID);
		if (frame != null && frame.isEffect)
		{
			flag = true;
			texture2D = manager.GetFrameTexture2D(data.EquipFrameID);
			texture2D2 = manager.GetFrameMaskTexture2D(data.EquipFrameID);
		}
		else
		{
			flag = false;
			texture2D = null;
			texture2D2 = null;
		}
		TitleData title = Singleton<DataManager>.Instance.GetTitle(data.EquipTitleID);
		string title2 = title?.name.str ?? "";
		Sprite titleBg = Resources.Load<Sprite>("Process/Common/Sprites/UpperMonitor/UI_CMN_Shougou_" + (title?.rareType ?? TrophyRareType.Normal));
		IsSubMonitor = isSubMonitor;
		Sprite pass = null;
		Material passMaterial = null;
		if (IsSubMonitor)
		{
			int cardType = data.CardType;
			string text = "";
			string text2 = "";
			Color baseColor = Color.clear;
			switch (cardType)
			{
			case 2:
				text = "Bronze";
				text2 = "FX_CMN_Card_Bronze";
				baseColor = new Color32(156, 70, 28, byte.MaxValue);
				break;
			case 3:
				text = "Silver";
				text2 = "FX_CMN_Card_Silver_Pattern";
				baseColor = new Color32(160, 162, 177, byte.MaxValue);
				break;
			case 4:
				text = "Gold";
				text2 = "FX_CMN_Card_Gold_Pattern";
				baseColor = new Color32(240, 96, 0, byte.MaxValue);
				break;
			case 5:
				text = "Platinum";
				text2 = "FX_CMN_Card_Platinum_Pattern";
				baseColor = new Color32(135, 99, 168, byte.MaxValue);
				break;
			case 6:
				text = "Freedom";
				text2 = "FX_CMN_Card_Freedom_Pattern";
				baseColor = new Color32(49, 115, 180, byte.MaxValue);
				break;
			}
			pass = Resources.Load<Sprite>("Process/Common/Sprites/UpperMonitor/UI_CMN_DXPass_" + text);
			passMaterial = Resources.Load<Material>("CMN_Card/" + text2);
			BaseColor = baseColor;
		}
		Icon = iconTexture2D;
		Plate = plateTexture2D;
		Name = data.UserName;
		Rateing = data.Rating;
		Title = title2;
		TitleBg = titleBg;
		Pass = pass;
		DispRate = dispRate;
		ClassID = (UdemaeID)data.ClassRank;
		DaniID = data.CourseRank;
		PassMaterial = passMaterial;
		Frame = frameTexture2D;
		IsKiraFrame = flag;
		KiraFrame = texture2D;
		KiraFrameMask = texture2D2;
	}

	public MessageUserInformationData(AssetManager manager, UserGhost data)
	{
		Texture2D silhouetteTexture2D = manager.GetSilhouetteTexture2D(data.IconId);
		Texture2D plateTexture2D = manager.GetPlateTexture2D(data.PlateId);
		Texture2D frameTexture2D = manager.GetFrameTexture2D(0);
		string str = Singleton<DataManager>.Instance.GetMapTitleData(data.TitleId).name.str;
		Sprite titleBg = Resources.Load<Sprite>("Process/Common/Sprites/UpperMonitor/UI_CMN_Shougou_" + TrophyRareType.Normal);
		Sprite pass = Resources.Load<Sprite>("Process/Common/Sprites/UpperMonitor/UI_CMN_DXPass_Platinum");
		Icon = silhouetteTexture2D;
		Plate = plateTexture2D;
		Frame = frameTexture2D;
		Name = data.Name;
		Rateing = (uint)data.Rate;
		Title = str;
		TitleBg = titleBg;
		Pass = pass;
		ClassID = UserUdemae.GetRateToUdemaeID(data.ClassValue);
		DaniID = data.CourseRank;
		DispRate = OptionDisprateID.DispDan;
		IsKiraFrame = false;
		KiraFrame = null;
		KiraFrameMask = null;
	}

	public MessageUserInformationData(AssetManager manager, UserDetail data)
	{
		Texture2D silhouetteTexture2D = manager.GetSilhouetteTexture2D(data.EquipIconID);
		Texture2D plateTexture2D = manager.GetPlateTexture2D(data.EquipPlateID);
		Texture2D frameTexture2D = manager.GetFrameTexture2D(0);
		string title = Singleton<DataManager>.Instance.GetTitle(data.EquipTitleID)?.name.str ?? "";
		Sprite titleBg = Resources.Load<Sprite>("Process/Common/Sprites/UpperMonitor/UI_CMN_Shougou_" + TrophyRareType.Normal);
		Sprite pass = Resources.Load<Sprite>("Process/Common/Sprites/UpperMonitor/UI_CMN_DXPass_Platinum");
		Icon = silhouetteTexture2D;
		Plate = plateTexture2D;
		Frame = frameTexture2D;
		Name = data.UserName;
		Rateing = data.Rating;
		Title = title;
		TitleBg = titleBg;
		Pass = pass;
		ClassID = (UdemaeID)data.ClassRank;
		DaniID = data.CourseRank;
		DispRate = OptionDisprateID.AllDisp;
		IsKiraFrame = false;
		KiraFrame = null;
		KiraFrameMask = null;
	}

	public void OverrideRateAndGrade(uint rate, UdemaeID classID)
	{
		Rateing = rate;
		ClassID = classID;
	}
}
