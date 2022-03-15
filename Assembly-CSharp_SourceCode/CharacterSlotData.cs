using Manager.MaiStudio;
using UnityEngine;

public class CharacterSlotData
{
	private const string CharaRootPath = "Common/Sprites/CharacterColor/";

	private uint _id;

	private Sprite _base;

	private Sprite _frame;

	private Sprite _leaderFrame;

	private Sprite _smallAwakeStar;

	private Sprite _awakeStarBase;

	private Sprite _awakeStarFrame;

	private Color24 _color;

	private Color24 _subColor;

	public uint ID => _id;

	public Sprite Base => _base;

	public Sprite Frame => _frame;

	public Sprite LeaderFrame => _leaderFrame;

	public Sprite SmallAwakeStar => _smallAwakeStar;

	public Sprite AwakeStarBase => _awakeStarBase;

	public Sprite AwakeStarFrame => _awakeStarFrame;

	public Color24 Color => _color;

	public Color24 SubColor => _subColor;

	public CharacterSlotData(int colorKey, Color24 color, Color24 subColor)
	{
		Load(colorKey);
		_color = color;
		_subColor = subColor;
	}

	private void Load(int colorKey)
	{
		if (_base == null)
		{
			_base = AssetManager.Instance().GetMapBgSprite(colorKey, "UI_Chara_RBase");
			_frame = AssetManager.Instance().GetMapBgSprite(colorKey, "UI_Chara_RFrame");
			_leaderFrame = AssetManager.Instance().GetMapBgSprite(colorKey, "UI_Chara_LFrame");
			_smallAwakeStar = AssetManager.Instance().GetMapBgSprite(colorKey, "UI_Chara_Star_S");
			_awakeStarBase = AssetManager.Instance().GetMapBgSprite(colorKey, "UI_Chara_Star");
			_awakeStarFrame = Resources.Load<Sprite>("Common/Sprites/Map/StarGauge/UI_CMN_Chara_Star_Big_Gauge01");
		}
	}
}
