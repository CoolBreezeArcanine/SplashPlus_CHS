using System;
using System.Reflection;
using System.Text;

namespace MAI2.Util
{
	public class StringEx
	{
		private const int StringBuilderMaxCacheSize = 1024;

		private const int StringBuilderMaxCacheTsSize = 32;

		private static char[] clearArray;

		private static StringBuilder Sb;

		private static char[] sbCharArray;

		private static StringBuilder SbTs;

		private static char[] sbCharArrayTs;

		private static readonly char[] IntToChar;

		private static readonly char MinusPaddingChar;

		static StringEx()
		{
			clearArray = new char[1024];
			Sb = new StringBuilder(1024, 1024);
			SbTs = new StringBuilder(32, 32);
			IntToChar = new char[10] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
			MinusPaddingChar = '-';
			for (int i = 0; i < clearArray.Length; i++)
			{
				clearArray[i] = ' ';
			}
			sbCharArray = (char[])Sb.GetType().GetField("m_ChunkChars", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Sb);
			sbCharArrayTs = (char[])SbTs.GetType().GetField("m_ChunkChars", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(SbTs);
		}

		public static void ClearString()
		{
			Sb.Clear();
			Array.Copy(clearArray, sbCharArray, sbCharArray.Length);
		}

		public static void AddString(string addString)
		{
			Sb.Append(addString);
		}

		public static string GetString()
		{
			return Sb.ToString();
		}

		public static string ToStringInt(int num, char padding, int keta, char addTail = ' ')
		{
			SbTs.Clear();
			Array.Copy(clearArray, sbCharArrayTs, sbCharArrayTs.Length);
			int num2 = 0;
			int num3 = Math.Abs(num);
			bool flag = num < 0;
			do
			{
				num3 /= 10;
				num2++;
			}
			while (num3 > 0);
			if (flag)
			{
				num2++;
			}
			if (num2 > keta)
			{
				if (flag)
				{
					SbTs.Append(MinusPaddingChar);
					for (int i = 1; i < keta; i++)
					{
						SbTs.Append(IntToChar[9]);
					}
				}
				else
				{
					for (int j = 0; j < keta; j++)
					{
						SbTs.Append(IntToChar[9]);
					}
				}
			}
			else
			{
				num3 = Math.Abs(num);
				for (int k = 0; k < keta; k++)
				{
					if (keta - k > num2)
					{
						if (flag && k == keta - num2)
						{
							SbTs.Append(MinusPaddingChar);
						}
						else
						{
							SbTs.Append(padding);
						}
					}
					else if (flag && k == keta - num2)
					{
						SbTs.Append(MinusPaddingChar);
					}
					else
					{
						int num4 = keta - (k + 1);
						SbTs.Append(IntToChar[num3 / (int)Math.Pow(10.0, num4)]);
						num3 %= (int)Math.Pow(10.0, num4);
					}
				}
			}
			if (addTail != ' ')
			{
				SbTs.Append(addTail);
			}
			return SbTs.ToString();
		}

		private unsafe static void CopyCharArrayToString(string destString, char[] charArray, int length)
		{
			fixed (char* ptr = destString)
			{
				for (int i = 0; i < length; i++)
				{
					ptr[i] = charArray[i];
				}
			}
		}
	}
}
