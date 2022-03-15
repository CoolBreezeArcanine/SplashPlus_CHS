using System.Runtime.InteropServices;
using UnityEngine;

namespace IO
{
	public class QRImage
	{
		public static class ARCFOUR
		{
			public const int StateSize = 256;

			public const int StateMask = 255;

			private static void swap(ref byte x0, ref byte x1)
			{
				byte b = x1;
				x1 = x0;
				x0 = b;
			}

			public static void encrypt(byte[] data, byte[] key)
			{
				byte[] array = new byte[256];
				int i;
				for (i = 0; i < 256; i++)
				{
					array[i] = (byte)i;
				}
				int num = 0;
				for (i = 0; i < 256; i++)
				{
					num = (num + array[i] + key[i % key.Length]) & 0xFF;
					swap(ref array[i], ref array[num]);
				}
				i = 0;
				num = 0;
				for (int j = 0; j < data.Length; j++)
				{
					i = (i + 1) & 0xFF;
					num = (num + array[i]) & 0xFF;
					swap(ref array[i], ref array[num]);
					data[j] ^= array[(array[i] + array[num]) & 0xFF];
				}
			}

			public static void decrypt(byte[] cipher, byte[] key)
			{
				encrypt(cipher, key);
			}
		}

		public const string PluginName = "QR_Image";

		private static readonly byte[] Key = new byte[16]
		{
			144, 95, 51, 167, 195, 243, 253, 226, 84, 194,
			239, 80, 177, 205, 41, 78
		};

		[DllImport("QR_Image")]
		public static extern int QRGetSize(int version);

		[DllImport("QR_Image")]
		public static extern int QREncode(byte[] dst, int errorCorrectLevel, int version, int srcSize, byte[] src);

		[DllImport("QR_Image")]
		public static extern int QREncodeString(byte[] dst, int errorCorrectLevel, int version, string src);

		public static int getSize(int version)
		{
			return 21 + (version - 1) * 4;
		}

		public static int encode(byte[] dst, int errorCorrectLevel, int version, int srcSize, byte[] src)
		{
			return QREncode(dst, errorCorrectLevel, version, srcSize, src);
		}

		private static Color32[] encode(out int padded, int size, byte[] src, int cellSize, int margin)
		{
			padded = (size + (margin << 1)) * cellSize;
			int num = padded * padded;
			Color32[] array = new Color32[num];
			Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			for (int i = 0; i < num; i++)
			{
				array[i] = color;
			}
			for (int j = 0; j < size; j++)
			{
				for (int k = 0; k < size; k++)
				{
					if (src[(size - j - 1) * size + k] == 0)
					{
						continue;
					}
					int num2 = (k + margin) * cellSize;
					int num3 = (j + margin) * cellSize;
					for (int l = 0; l < cellSize; l++)
					{
						for (int m = 0; m < cellSize; m++)
						{
							array[(num3 + l) * padded + (num2 + m)] = new Color32(0, 0, 0, byte.MaxValue);
						}
					}
				}
			}
			return array;
		}

		public static Color32[] encode(out int size, byte[] src, int cellSize, int margin, int errorCorrectLevel = 1, int version = 1)
		{
			size = 0;
			int size2 = getSize(version);
			byte[] array = new byte[size2 * size2];
			int num = QREncode(array, errorCorrectLevel, version, src.Length, src);
			if (num < 0)
			{
				return null;
			}
			return encode(out size, num, array, cellSize, margin);
		}

		public static Color32[] encode(out int size, string src, int cellSize, int margin, int errorCorrectLevel = 1, int version = 1)
		{
			size = 0;
			int size2 = getSize(version);
			byte[] array = new byte[size2 * size2];
			int num = QREncodeString(array, errorCorrectLevel, version, src);
			if (num < 0)
			{
				return null;
			}
			return encode(out size, num, array, cellSize, margin);
		}

		public static Texture2D encode(byte[] src, int cellSize, int margin, int errorCorrectLevel = 1, int version = 1)
		{
			int size;
			Color32[] array = encode(out size, src, cellSize, margin, errorCorrectLevel, version);
			if (array == null)
			{
				return null;
			}
			Texture2D texture2D = new Texture2D(size, size, TextureFormat.RGB24, mipChain: false, linear: true);
			texture2D.filterMode = FilterMode.Point;
			texture2D.SetPixels32(array);
			texture2D.Apply();
			return texture2D;
		}

		public static Texture2D encode(string src, int cellSize, int margin, int errorCorrectLevel = 1, int version = 1)
		{
			int size;
			Color32[] array = encode(out size, src, cellSize, margin, errorCorrectLevel, version);
			if (array == null)
			{
				return null;
			}
			Texture2D texture2D = new Texture2D(size, size, TextureFormat.RGB24, mipChain: false, linear: true);
			texture2D.filterMode = FilterMode.Point;
			texture2D.SetPixels32(array);
			texture2D.Apply();
			return texture2D;
		}

		public static byte[] createBytes(long playerID, int cardID, int serial, uint gameID)
		{
			byte[] array = new byte[14];
			createBytes(array, playerID, cardID, serial, gameID);
			return array;
		}

		public static void createBytes(byte[] bytes, long playerID, int cardID, int param, uint gameID)
		{
			bytes[0] = (byte)(playerID & 0xFF);
			bytes[1] = (byte)((playerID >> 8) & 0xFF);
			bytes[2] = (byte)((playerID >> 16) & 0xFF);
			bytes[3] = (byte)((playerID >> 24) & 0xFF);
			bytes[4] = (byte)((uint)cardID & 0xFFu);
			bytes[5] = (byte)((uint)(cardID >> 8) & 0xFFu);
			bytes[6] = (byte)((uint)(cardID >> 16) & 0xFFu);
			bytes[7] = (byte)((uint)param & 0xFFu);
			bytes[8] = (byte)((uint)(param >> 8) & 0xFFu);
			bytes[9] = (byte)((uint)(param >> 16) & 0xFFu);
			bytes[10] = (byte)((uint)(param >> 24) & 0xFFu);
			bytes[11] = (byte)(gameID & 0xFFu);
			bytes[12] = (byte)((gameID >> 8) & 0xFFu);
			bytes[13] = (byte)((gameID >> 16) & 0xFFu);
		}

		public static void encrypt(byte[] bytes)
		{
			ARCFOUR.encrypt(bytes, Key);
		}

		public static void decrypt(byte[] bytes)
		{
			ARCFOUR.decrypt(bytes, Key);
		}
	}
}
