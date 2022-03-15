using UnityEngine;

public class FramePart
{
	private string _name;

	private Sprite _sprite;

	private Texture2D _tex;

	public string Name => _name;

	public Sprite Sprite => _sprite;

	public Texture2D Tex => _tex;

	public FramePart(string name, Sprite sprite, Texture2D tex)
	{
		_name = name;
		_sprite = sprite;
		_tex = ReplaceCustomTexture(tex);
	}

	private Texture2D ReplaceCustomTexture(Texture2D targetTexture)
	{
		Texture2D texture = new Texture2D(0, 0);
		Color[] colors = CustomGetPixels(targetTexture);
		SetColorsToTexture(targetTexture.width, targetTexture.height, colors, ref texture);
		return texture;
	}

	private Color[] CustomGetPixels(Texture2D texture)
	{
		RenderTexture temporary = RenderTexture.GetTemporary(texture.width, texture.height);
		Graphics.Blit(texture, temporary);
		Texture2D texture2D = new Texture2D(texture.width, texture.height);
		texture2D.ReadPixels(new Rect(0f, 0f, texture.width, texture.height), 0, 0);
		RenderTexture.ReleaseTemporary(temporary);
		return texture2D.GetPixels();
	}

	private void SetColorsToTexture(int fractionX, int fractionY, Color[] colors, ref Texture2D texture)
	{
		texture = new Texture2D(fractionX, fractionY, TextureFormat.ARGB32, mipChain: false);
		texture.filterMode = FilterMode.Point;
		texture.SetPixels(0, 0, fractionX, fractionY, colors);
		texture.Apply();
	}
}
