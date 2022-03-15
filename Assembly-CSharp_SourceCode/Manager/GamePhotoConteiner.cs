using System;
using UnityEngine;

namespace Manager
{
	public struct GamePhotoConteiner
	{
		public bool Enable { get; private set; }

		public Color32[] Colors { get; private set; }

		public GamePhotoConteiner(int height, int width)
		{
			Enable = false;
			Colors = new Color32[height * width];
		}

		public void ClearBuffer()
		{
			Enable = false;
			Array.Clear(Colors, 0, Colors.Length);
		}

		public void CopyColor(WebCamTexture texture)
		{
			Enable = true;
			texture.GetPixels32(Colors);
		}
	}
}
