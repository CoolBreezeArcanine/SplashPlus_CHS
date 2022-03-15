using UnityEngine;

public class MessageCharactorInfomationData
{
	public int Index { get; private set; }

	public Texture2D Character { get; private set; }

	public uint Level { get; private set; }

	public uint Awakening { get; private set; }

	public float AwakeRate { get; private set; }

	public Color RegionColor { get; private set; }

	public int MapKey { get; private set; }

	public MessageCharactorInfomationData(int index, int mapKey, Texture2D character, uint level, uint awakening, float awakeRate, Color regionColor)
	{
		SetData(index, mapKey, character, level, awakening, awakeRate, regionColor);
	}

	public void SetData(int index, int mapKey, Texture2D character, uint level, uint awakening, float awakeRate, Color regionColor)
	{
		Index = index;
		Character = character;
		Level = level;
		Awakening = awakening;
		AwakeRate = awakeRate;
		MapKey = mapKey;
		RegionColor = regionColor;
	}
}
