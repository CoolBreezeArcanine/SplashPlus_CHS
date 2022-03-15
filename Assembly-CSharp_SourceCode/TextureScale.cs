using UnityEngine;

public class TextureScale
{
	private static Color[] texColors;

	private static Color[] newColors;

	private static float ratioX;

	private static float ratioY;

	private static int srcWidth;

	private static int dstWidth;

	public static void Scale(Texture2D tex, int newWidth, int newHeight)
	{
		texColors = tex.GetPixels();
		newColors = new Color[newWidth * newHeight];
		ratioX = 1f / ((float)newWidth / (float)(tex.width - 1));
		ratioY = 1f / ((float)newHeight / (float)(tex.height - 1));
		srcWidth = tex.width;
		dstWidth = newWidth;
		BilinearScale(0, newHeight);
		tex.Resize(newWidth, newHeight);
		tex.SetPixels(newColors);
		tex.Apply();
	}

	private static void BilinearScale(int top, int bottom)
	{
		for (int i = top; i < bottom; i++)
		{
			int num = (int)Mathf.Floor((float)i * ratioY);
			int num2 = num * srcWidth;
			int num3 = (num + 1) * srcWidth;
			int num4 = i * dstWidth;
			for (int j = 0; j < dstWidth; j++)
			{
				int num5 = (int)Mathf.Floor((float)j * ratioX);
				float leapValue = (float)j * ratioX - (float)num5;
				newColors[num4 + j] = LerpColor(LerpColor(texColors[num2 + num5], texColors[num2 + num5 + 1], leapValue), LerpColor(texColors[num3 + num5], texColors[num3 + num5 + 1], leapValue), (float)i * ratioY - (float)num);
			}
		}
	}

	private static Color LerpColor(Color color1, Color color2, float leapValue)
	{
		return new Color(color1.r + (color2.r - color1.r) * leapValue, color1.g + (color2.g - color1.g) * leapValue, color1.b + (color2.b - color1.b) * leapValue, color1.a + (color2.a - color1.a) * leapValue);
	}
}
