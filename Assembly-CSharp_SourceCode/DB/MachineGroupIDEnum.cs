namespace DB
{
	public static class MachineGroupIDEnum
	{
		private static readonly MachineGroupTableRecord[] records = new MachineGroupTableRecord[2]
		{
			new MachineGroupTableRecord(0, "ON", "ON"),
			new MachineGroupTableRecord(1, "OFF", "OFF")
		};

		public static bool IsActive(this MachineGroupID self)
		{
			if (self >= MachineGroupID.ON && self < MachineGroupID.End)
			{
				return self != MachineGroupID.ON;
			}
			return false;
		}

		public static bool IsValid(this MachineGroupID self)
		{
			if (self >= MachineGroupID.ON)
			{
				return self < MachineGroupID.End;
			}
			return false;
		}

		public static void Clamp(this MachineGroupID self)
		{
			if (self < MachineGroupID.ON)
			{
				self = MachineGroupID.ON;
			}
			else if ((int)self >= GetEnd())
			{
				self = (MachineGroupID)GetEnd();
			}
		}

		public static int GetEnd(this MachineGroupID self)
		{
			return GetEnd();
		}

		public static MachineGroupID FindID(string enumName)
		{
			for (MachineGroupID machineGroupID = MachineGroupID.ON; machineGroupID < MachineGroupID.End; machineGroupID++)
			{
				if (machineGroupID.GetEnumName() == enumName)
				{
					return machineGroupID;
				}
			}
			return MachineGroupID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this MachineGroupID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this MachineGroupID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this MachineGroupID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
