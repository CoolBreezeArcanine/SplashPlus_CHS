using UnityEngine;

namespace Comio.BD15070_4
{
	public class LedData
	{
		private Color32[] color = new Color32[11];

		public LedData()
		{
			SetOff();
		}

		public void SetOff()
		{
			for (int i = 0; i < color.Length; i++)
			{
				color[i] = Color.black;
			}
		}

		public void SetColorGrb(byte ledPos, byte green, byte red, byte blue)
		{
			if (11 > ledPos)
			{
				color[ledPos].r = AdjustColorElement(green);
				color[ledPos].g = AdjustColorElement(red);
				color[ledPos].b = AdjustColorElement(blue);
			}
		}

		public void SetColorRgb(byte ledPos, byte red, byte green, byte blue)
		{
			if (11 > ledPos)
			{
				color[ledPos].r = AdjustColorElement(red);
				color[ledPos].g = AdjustColorElement(green);
				color[ledPos].b = AdjustColorElement(blue);
			}
		}

		public void SetColorRgb(byte ledPos, byte colorBase)
		{
			if (11 > ledPos)
			{
				color[ledPos].r = AdjustColorElement(colorBase);
				color[ledPos].g = AdjustColorElement(colorBase);
				color[ledPos].b = AdjustColorElement(colorBase);
			}
		}

		public Color32 GetColor(byte ledPos)
		{
			if (11 > ledPos)
			{
				return color[ledPos];
			}
			return color[0];
		}

		public void CopyFrom(LedData src)
		{
			for (int i = 0; i < 11; i++)
			{
				color[i] = src.color[i];
			}
		}

		private static byte AdjustColorElement(byte element)
		{
			byte b = element;
			if (224 == b)
			{
				b = (byte)(b + 1);
			}
			if (208 == b)
			{
				b = (byte)(b + 1);
			}
			return b;
		}
	}
}
