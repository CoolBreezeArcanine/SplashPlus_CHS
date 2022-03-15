namespace DB
{
	public static class PartyDeliveryCheckerErrorIDEnum
	{
		private static readonly PartyDeliveryCheckerErrorTableRecord[] records = new PartyDeliveryCheckerErrorTableRecord[3]
		{
			new PartyDeliveryCheckerErrorTableRecord(0, "None", "None"),
			new PartyDeliveryCheckerErrorTableRecord(1, "Socket", "Socket"),
			new PartyDeliveryCheckerErrorTableRecord(2, "Duplicate", "Duplicate")
		};

		public static bool IsActive(this PartyDeliveryCheckerErrorID self)
		{
			if (self >= PartyDeliveryCheckerErrorID.None && self < PartyDeliveryCheckerErrorID.End)
			{
				return self != PartyDeliveryCheckerErrorID.None;
			}
			return false;
		}

		public static bool IsValid(this PartyDeliveryCheckerErrorID self)
		{
			if (self >= PartyDeliveryCheckerErrorID.None)
			{
				return self < PartyDeliveryCheckerErrorID.End;
			}
			return false;
		}

		public static void Clamp(this PartyDeliveryCheckerErrorID self)
		{
			if (self < PartyDeliveryCheckerErrorID.None)
			{
				self = PartyDeliveryCheckerErrorID.None;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PartyDeliveryCheckerErrorID)GetEnd();
			}
		}

		public static int GetEnd(this PartyDeliveryCheckerErrorID self)
		{
			return GetEnd();
		}

		public static PartyDeliveryCheckerErrorID FindID(string enumName)
		{
			for (PartyDeliveryCheckerErrorID partyDeliveryCheckerErrorID = PartyDeliveryCheckerErrorID.None; partyDeliveryCheckerErrorID < PartyDeliveryCheckerErrorID.End; partyDeliveryCheckerErrorID++)
			{
				if (partyDeliveryCheckerErrorID.GetEnumName() == enumName)
				{
					return partyDeliveryCheckerErrorID;
				}
			}
			return PartyDeliveryCheckerErrorID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PartyDeliveryCheckerErrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PartyDeliveryCheckerErrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PartyDeliveryCheckerErrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
