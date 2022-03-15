using UnityEngine;
using UnityEngine.UI;

public class KiraFrameImage : Image
{
	[SerializeField]
	private Texture2D _subTexture;

	private readonly int SubTextureID = Shader.PropertyToID("_SubTex");

	private void UpdateKira()
	{
		if (material != null)
		{
			material.SetTexture(SubTextureID, _subTexture);
		}
	}

	public void SetSourceImage(Texture2D tex)
	{
		if (tex != null)
		{
			base.sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), Vector2.zero);
		}
	}

	public void SetSubTexImage(Texture2D tex)
	{
		if (tex != null)
		{
			_subTexture = tex;
			UpdateKira();
		}
	}
}
