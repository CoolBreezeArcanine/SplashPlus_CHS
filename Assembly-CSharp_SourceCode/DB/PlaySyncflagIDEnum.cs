namespace DB
{
	public static class PlaySyncflagIDEnum
	{
		private static readonly PlaySyncflagTableRecord[] records = new PlaySyncflagTableRecord[5]
		{
			new PlaySyncflagTableRecord(0, "None", "なし", 0),
			new PlaySyncflagTableRecord(1, "ChainLow", "FullChain下位", 2),
			new PlaySyncflagTableRecord(2, "ChainHi", "FullChain上位", 5),
			new PlaySyncflagTableRecord(3, "SyncLow", "FullSync下位", 10),
			new PlaySyncflagTableRecord(4, "SyncHi", "FullSync上位", 12)
		};

		public static bool IsActive(this PlaySyncflagID self)
		{
			if (self >= PlaySyncflagID.None && self < PlaySyncflagID.End)
			{
				return self != PlaySyncflagID.None;
			}
			return false;
		}

		public static bool IsValid(this PlaySyncflagID self)
		{
			if (self >= PlaySyncflagID.None)
			{
				return self < PlaySyncflagID.End;
			}
			return false;
		}

		public static void Clamp(this PlaySyncflagID self)
		{
			if (self < PlaySyncflagID.None)
			{
				self = PlaySyncflagID.None;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PlaySyncflagID)GetEnd();
			}
		}

		public static int GetEnd(this PlaySyncflagID self)
		{
			return GetEnd();
		}

		public static PlaySyncflagID FindID(string enumName)
		{
			for (PlaySyncflagID playSyncflagID = PlaySyncflagID.None; playSyncflagID < PlaySyncflagID.End; playSyncflagID++)
			{
				if (playSyncflagID.GetEnumName() == enumName)
				{
					return playSyncflagID;
				}
			}
			return PlaySyncflagID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PlaySyncflagID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PlaySyncflagID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PlaySyncflagID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static int GetPoint(this PlaySyncflagID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Point;
			}
			return 0;
		}
	}
}
