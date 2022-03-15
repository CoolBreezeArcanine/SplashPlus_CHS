using System;
using System.Text;
using MAI2.Util;

namespace MAI2System
{
	public struct VersionNo : IEquatable<VersionNo>, IComparable<VersionNo>
	{
		public static readonly VersionNo MaxValue = new VersionNo(255u, 255u, 255u);

		private byte _majorNo;

		private byte _minorNo;

		private byte _releaseNo;

		public byte majorNo => _majorNo;

		public byte minorNo => _minorNo;

		public byte releaseNo => _releaseNo;

		public uint versionCode => calcVersionCode(_majorNo, _minorNo, _releaseNo);

		public uint versionCodeReleaseNo0 => calcVersionCode(_majorNo, _minorNo, 0);

		public uint versionCodeNoRelease => calcVersionCodeNoRelease(_majorNo, _minorNo);

		public string releaseNoAlphabet
		{
			get
			{
				StringBuilder stringBuilder = Singleton<SystemConfig>.Instance.getStringBuilder();
				int num = releaseNo;
				if (num > 0)
				{
					do
					{
						string value = stringBuilder.ToString();
						stringBuilder.Length = 0;
						num--;
						int num2 = num % 26;
						stringBuilder.Append(((char)(65 + num2)).ToString());
						stringBuilder.Append(value);
						num /= 26;
					}
					while (num > 0);
				}
				return stringBuilder.ToString();
			}
		}

		public string versionString => $"{_majorNo}.{_minorNo:00}.{_releaseNo:00}";

		public string versionShotString => $"{_majorNo}.{_minorNo:00}";

		public VersionNo(uint majorNo, uint minorNo, uint releaseNo)
		{
			_majorNo = (byte)majorNo;
			_minorNo = (byte)minorNo;
			_releaseNo = (byte)releaseNo;
		}

		public void set(uint majorNo, uint minorNo, uint releaseNo)
		{
			_majorNo = (byte)majorNo;
			_minorNo = (byte)minorNo;
			_releaseNo = (byte)releaseNo;
		}

		public bool tryParse(string s, bool setZeroIfFailed)
		{
			if (setZeroIfFailed)
			{
				_majorNo = 0;
				_minorNo = 0;
				_releaseNo = 0;
			}
			char[] separator = new char[1] { '.' };
			string[] array = s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length < 3)
			{
				return false;
			}
			if (!byte.TryParse(array[0], out var result))
			{
				return false;
			}
			if (!byte.TryParse(array[1], out var result2))
			{
				return false;
			}
			if (!byte.TryParse(array[2], out var result3))
			{
				return false;
			}
			_majorNo = result;
			_minorNo = result2;
			_releaseNo = result3;
			return true;
		}

		public bool tryParseNoRelease(string s, bool setZeroIfFailed)
		{
			if (setZeroIfFailed)
			{
				_majorNo = 0;
				_minorNo = 0;
				_releaseNo = 0;
			}
			char[] separator = new char[1] { '.' };
			string[] array = s.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length < 2)
			{
				return false;
			}
			if (!byte.TryParse(array[0], out var result))
			{
				return false;
			}
			if (!byte.TryParse(array[1], out var result2))
			{
				return false;
			}
			_majorNo = result;
			_minorNo = result2;
			_releaseNo = 0;
			return true;
		}

		public static uint calcVersionCode(byte majorNo, byte minorNo, byte releaseNo)
		{
			return (uint)(0 + majorNo * 1000 * 1000 + minorNo * 1000 + releaseNo);
		}

		public static uint calcVersionCodeNoRelease(uint majorNo, uint minorNo)
		{
			return 0 + majorNo * 1000 + minorNo;
		}

		public static uint calcShortVersionCode(byte majorNo, byte minorNo)
		{
			return (uint)(0 + majorNo * 100 + minorNo);
		}

		public bool Equals(VersionNo other)
		{
			return versionCode.Equals(other.versionCode);
		}

		public int CompareTo(VersionNo other)
		{
			return versionCode.CompareTo(other.versionCode);
		}
	}
}
