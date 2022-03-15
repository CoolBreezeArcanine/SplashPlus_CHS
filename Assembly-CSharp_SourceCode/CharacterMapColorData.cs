using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using UnityEngine;
using Util;

public class CharacterMapColorData
{
	private readonly int _colorKey;

	[SerializeField]
	private uint _id;

	[SerializeField]
	private Sprite _base;

	[SerializeField]
	private Sprite _smallBase;

	[SerializeField]
	private Sprite _level;

	[SerializeField]
	private Sprite _frame;

	[SerializeField]
	private Sprite _smallFrame;

	[SerializeField]
	private Sprite _smallAwakeStar;

	[SerializeField]
	private Sprite _awakeStarBase;

	private int _colorID;

	public uint ID => _id;

	public Sprite Base => _base;

	public Sprite SmallBase => _smallBase;

	public Sprite Level => _level;

	public Sprite Frame => _frame;

	public Sprite SmallFrame => _smallFrame;

	public Sprite SmallAwakeStar => _smallAwakeStar;

	public Sprite AwakeStarBase => _awakeStarBase;

	public Color ShadowColor { get; private set; }

	public CharacterMapColorData(int colorKey, int colorID)
	{
		_colorKey = colorKey;
		_colorID = colorID;
	}

	public void Load()
	{
		if (_base == null)
		{
			_base = AssetManager.Instance().GetMapBgSprite(_colorKey, "UI_Chara_Base");
			_smallBase = AssetManager.Instance().GetMapBgSprite(_colorKey, "UI_Chara_Base_S");
			_frame = AssetManager.Instance().GetMapBgSprite(_colorKey, "UI_Chara_Frame");
			_smallFrame = AssetManager.Instance().GetMapBgSprite(_colorKey, "UI_Chara_Frame_S");
			_level = AssetManager.Instance().GetMapBgSprite(_colorKey, "UI_Chara_level_S");
			_smallAwakeStar = AssetManager.Instance().GetMapBgSprite(_colorKey, "UI_Chara_Star_S");
			_awakeStarBase = AssetManager.Instance().GetMapBgSprite(_colorKey, "UI_Chara_Star");
			MapColorData mapColorData = Singleton<DataManager>.Instance.GetMapColorData(_colorID);
			ShadowColor = Utility.ConvertColor(mapColorData.ColorDark);
		}
	}
}
