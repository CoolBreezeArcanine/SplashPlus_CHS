namespace DB
{
	public static class PartyAdvertiseStateIDEnum
	{
		private static readonly PartyAdvertiseStateTableRecord[] records = new PartyAdvertiseStateTableRecord[5]
		{
			new PartyAdvertiseStateTableRecord(0, "First", "First", 0, 0),
			new PartyAdvertiseStateTableRecord(1, "Wait", "待機", 1, 0),
			new PartyAdvertiseStateTableRecord(2, "Requested", "返答待ち", 1, 0),
			new PartyAdvertiseStateTableRecord(3, "Ready", "指定時間待つ", 1, 0),
			new PartyAdvertiseStateTableRecord(4, "Go", "出発", 1, 1)
		};

		public static bool IsActive(this PartyAdvertiseStateID self)
		{
			if (self >= PartyAdvertiseStateID.First && self < PartyAdvertiseStateID.End)
			{
				return self != PartyAdvertiseStateID.First;
			}
			return false;
		}

		public static bool IsValid(this PartyAdvertiseStateID self)
		{
			if (self >= PartyAdvertiseStateID.First)
			{
				return self < PartyAdvertiseStateID.End;
			}
			return false;
		}

		public static void Clamp(this PartyAdvertiseStateID self)
		{
			if (self < PartyAdvertiseStateID.First)
			{
				self = PartyAdvertiseStateID.First;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PartyAdvertiseStateID)GetEnd();
			}
		}

		public static int GetEnd(this PartyAdvertiseStateID self)
		{
			return GetEnd();
		}

		public static PartyAdvertiseStateID FindID(string enumName)
		{
			for (PartyAdvertiseStateID partyAdvertiseStateID = PartyAdvertiseStateID.First; partyAdvertiseStateID < PartyAdvertiseStateID.End; partyAdvertiseStateID++)
			{
				if (partyAdvertiseStateID.GetEnumName() == enumName)
				{
					return partyAdvertiseStateID;
				}
			}
			return PartyAdvertiseStateID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PartyAdvertiseStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PartyAdvertiseStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PartyAdvertiseStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static bool IsNormal(this PartyAdvertiseStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isNormal;
			}
			return false;
		}

		public static bool IsGo(this PartyAdvertiseStateID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].isGo;
			}
			return false;
		}
	}
}
