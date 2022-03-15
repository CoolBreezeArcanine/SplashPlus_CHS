namespace DB
{
	public static class PartySettingErrorIDEnum
	{
		private static readonly PartySettingErrorTableRecord[] records = new PartySettingErrorTableRecord[6]
		{
			new PartySettingErrorTableRecord(0, "ConnectFailed", "接続失敗"),
			new PartySettingErrorTableRecord(1, "Disconnected", "切断された"),
			new PartySettingErrorTableRecord(2, "ForceError", "強制エラー"),
			new PartySettingErrorTableRecord(3, "GroupOff", "グループがOFF"),
			new PartySettingErrorTableRecord(4, "DuplicateHost", "ホストが重複"),
			new PartySettingErrorTableRecord(5, "SocketOpenFailed", "ソケットの生成失敗")
		};

		public static bool IsActive(this PartySettingErrorID self)
		{
			if (self >= PartySettingErrorID.ConnectFailed && self < PartySettingErrorID.End)
			{
				return self != PartySettingErrorID.ConnectFailed;
			}
			return false;
		}

		public static bool IsValid(this PartySettingErrorID self)
		{
			if (self >= PartySettingErrorID.ConnectFailed)
			{
				return self < PartySettingErrorID.End;
			}
			return false;
		}

		public static void Clamp(this PartySettingErrorID self)
		{
			if (self < PartySettingErrorID.ConnectFailed)
			{
				self = PartySettingErrorID.ConnectFailed;
			}
			else if ((int)self >= GetEnd())
			{
				self = (PartySettingErrorID)GetEnd();
			}
		}

		public static int GetEnd(this PartySettingErrorID self)
		{
			return GetEnd();
		}

		public static PartySettingErrorID FindID(string enumName)
		{
			for (PartySettingErrorID partySettingErrorID = PartySettingErrorID.ConnectFailed; partySettingErrorID < PartySettingErrorID.End; partySettingErrorID++)
			{
				if (partySettingErrorID.GetEnumName() == enumName)
				{
					return partySettingErrorID;
				}
			}
			return PartySettingErrorID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this PartySettingErrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this PartySettingErrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this PartySettingErrorID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
