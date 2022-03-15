namespace DB
{
	public static class ExtendContentBitIDEnum
	{
		private static readonly ExtendContentBitTableRecord[] records = new ExtendContentBitTableRecord[4]
		{
			new ExtendContentBitTableRecord(0, "PhotoAgree", "写真撮影許可フラグ"),
			new ExtendContentBitTableRecord(1, "GotoCodeRead", "カード読み込みシーケンス移行フラグ"),
			new ExtendContentBitTableRecord(2, "GotoCharaSelect", "キャラセレクトシーケンス移行フラグ"),
			new ExtendContentBitTableRecord(3, "GotoIconPhotoShoot", "アイコン撮影シーケンス移行フラグ")
		};

		public static bool IsActive(this ExtendContentBitID self)
		{
			if (self >= ExtendContentBitID.PhotoAgree && self < ExtendContentBitID.End)
			{
				return self != ExtendContentBitID.PhotoAgree;
			}
			return false;
		}

		public static bool IsValid(this ExtendContentBitID self)
		{
			if (self >= ExtendContentBitID.PhotoAgree)
			{
				return self < ExtendContentBitID.End;
			}
			return false;
		}

		public static void Clamp(this ExtendContentBitID self)
		{
			if (self < ExtendContentBitID.PhotoAgree)
			{
				self = ExtendContentBitID.PhotoAgree;
			}
			else if ((int)self >= GetEnd())
			{
				self = (ExtendContentBitID)GetEnd();
			}
		}

		public static int GetEnd(this ExtendContentBitID self)
		{
			return GetEnd();
		}

		public static ExtendContentBitID FindID(string enumName)
		{
			for (ExtendContentBitID extendContentBitID = ExtendContentBitID.PhotoAgree; extendContentBitID < ExtendContentBitID.End; extendContentBitID++)
			{
				if (extendContentBitID.GetEnumName() == enumName)
				{
					return extendContentBitID;
				}
			}
			return ExtendContentBitID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this ExtendContentBitID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this ExtendContentBitID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this ExtendContentBitID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}
	}
}
