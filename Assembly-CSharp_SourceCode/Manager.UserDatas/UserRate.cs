using System;
using DB;
using MAI2.Util;
using Manager.MaiStudio;

namespace Manager.UserDatas
{
	public struct UserRate : IComparable<UserRate>
	{
		public int MusicId { get; private set; }

		public int Level { get; private set; }

		public uint Achievement { get; private set; }

		public uint RomVersion { get; private set; }

		public int ScoreRate { get; private set; }

		public uint SingleRate { get; private set; }

		public bool OldFlag { get; private set; }

		public UserRate(int musicid, int level, uint achive, int rate, uint singleRate, bool oldFlag, uint romVersion)
		{
			MusicId = musicid;
			Level = level;
			Achievement = achive;
			ScoreRate = rate;
			SingleRate = singleRate;
			OldFlag = oldFlag;
			RomVersion = romVersion;
		}

		public UserRate(int musicid, int level, uint achive, uint romVersion)
		{
			MusicId = musicid;
			Level = level;
			Achievement = achive;
			ScoreRate = 0;
			SingleRate = 0u;
			OldFlag = true;
			RomVersion = romVersion;
			Notes notes = null;
			if (level <= 4)
			{
				notes = Singleton<DataManager>.Instance.GetMusic(musicid)?.notesData[level];
			}
			int num = 0;
			int num2 = (((int)Achievement >= RatingTableID.Rate_22.GetAchive()) ? RatingTableID.Rate_22.GetAchive() : ((int)Achievement));
			for (int num3 = 22; num3 >= 0; num3--)
			{
				if (((RatingTableID)num3).GetAchive() <= num2)
				{
					num = ((RatingTableID)num3).GetOffset();
					break;
				}
			}
			if (notes != null)
			{
				ScoreRate = notes.level * 10 + notes.levelDecimal;
				SingleRate = (uint)((long)ScoreRate * (long)num2 * num / 100000000);
				OldFlag = romVersion < 21500;
			}
		}

		public void Clear()
		{
			MusicId = 0;
			Level = 0;
			Achievement = 0u;
			ScoreRate = 0;
			SingleRate = 0u;
			RomVersion = 0u;
		}

		public static bool operator <(UserRate l, UserRate r)
		{
			return l.SingleRate < r.SingleRate;
		}

		public static bool operator <=(UserRate l, UserRate r)
		{
			return l.SingleRate <= r.SingleRate;
		}

		public static bool operator >(UserRate l, UserRate r)
		{
			return l.SingleRate > r.SingleRate;
		}

		public static bool operator >=(UserRate l, UserRate r)
		{
			return l.SingleRate >= r.SingleRate;
		}

		public static bool operator ==(UserRate a, UserRate b)
		{
			if (a.MusicId == b.MusicId)
			{
				return a.Level == b.Level;
			}
			return false;
		}

		public static bool operator !=(UserRate a, UserRate b)
		{
			if (a.MusicId == b.MusicId)
			{
				return a.Level != b.Level;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			UserRate userRate = this;
			UserRate userRate2 = (UserRate)obj;
			return userRate == userRate2;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public int CompareTo(UserRate other)
		{
			int num = SingleRate.CompareTo(other.SingleRate);
			switch (num)
			{
			case -1:
			case 1:
				return num;
			case 0:
				return Achievement.CompareTo(other.Achievement);
			default:
				return 0;
			}
		}

		public int CompareTo(object obj)
		{
			UserRate userRate = this;
			UserRate other = (UserRate)obj;
			return userRate.CompareTo(other);
		}
	}
}
