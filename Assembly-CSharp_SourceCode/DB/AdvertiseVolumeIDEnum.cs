namespace DB
{
	public static class AdvertiseVolumeIDEnum
	{
		private static readonly AdvertiseVolumeTableRecord[] records = new AdvertiseVolumeTableRecord[5]
		{
			new AdvertiseVolumeTableRecord(0, "_0", "0%", 0),
			new AdvertiseVolumeTableRecord(1, "_25", "25%", 25),
			new AdvertiseVolumeTableRecord(2, "_50", "50%", 50),
			new AdvertiseVolumeTableRecord(3, "_75", "75%", 75),
			new AdvertiseVolumeTableRecord(4, "_100", "100%", 100)
		};

		public static bool IsActive(this AdvertiseVolumeID self)
		{
			if (self >= AdvertiseVolumeID._0 && self < AdvertiseVolumeID.End)
			{
				return self != AdvertiseVolumeID._0;
			}
			return false;
		}

		public static bool IsValid(this AdvertiseVolumeID self)
		{
			if (self >= AdvertiseVolumeID._0)
			{
				return self < AdvertiseVolumeID.End;
			}
			return false;
		}

		public static void Clamp(this AdvertiseVolumeID self)
		{
			if (self < AdvertiseVolumeID._0)
			{
				self = AdvertiseVolumeID._0;
			}
			else if ((int)self >= GetEnd())
			{
				self = (AdvertiseVolumeID)GetEnd();
			}
		}

		public static int GetEnd(this AdvertiseVolumeID self)
		{
			return GetEnd();
		}

		public static AdvertiseVolumeID FindID(string enumName)
		{
			for (AdvertiseVolumeID advertiseVolumeID = AdvertiseVolumeID._0; advertiseVolumeID < AdvertiseVolumeID.End; advertiseVolumeID++)
			{
				if (advertiseVolumeID.GetEnumName() == enumName)
				{
					return advertiseVolumeID;
				}
			}
			return AdvertiseVolumeID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this AdvertiseVolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this AdvertiseVolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this AdvertiseVolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static int GetVolume(this AdvertiseVolumeID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Volume;
			}
			return 0;
		}
	}
}
