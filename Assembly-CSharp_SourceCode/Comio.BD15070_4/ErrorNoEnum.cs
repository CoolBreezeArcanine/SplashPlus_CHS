namespace Comio.BD15070_4
{
	public static class ErrorNoEnum
	{
		private static readonly ErrorNoTableRecord[] Records = new ErrorNoTableRecord[8]
		{
			new ErrorNoTableRecord(0, "None", isNeedFirmUpdate: false),
			new ErrorNoTableRecord(0, "ComTimeout", isNeedFirmUpdate: false),
			new ErrorNoTableRecord(0, "SumError", isNeedFirmUpdate: false),
			new ErrorNoTableRecord(0, "ComResponse_ParamError", isNeedFirmUpdate: false),
			new ErrorNoTableRecord(0, "ComResponse_Invalid", isNeedFirmUpdate: false),
			new ErrorNoTableRecord(0, "FirmError", isNeedFirmUpdate: true),
			new ErrorNoTableRecord(0, "FirmVersionError", isNeedFirmUpdate: true),
			new ErrorNoTableRecord(0, "EEPRomWriteError", isNeedFirmUpdate: false)
		};

		public static bool IsValid(this ErrorNo self)
		{
			if (self >= ErrorNo.Begin)
			{
				return self < ErrorNo.End;
			}
			return false;
		}

		public static string GetString(this ErrorNo self)
		{
			if (self.IsValid())
			{
				return Records[(int)self].String;
			}
			return "";
		}

		public static bool IsNeedFirmUpdate(this ErrorNo self)
		{
			if (self.IsValid())
			{
				return Records[(int)self].IsNeedFirmUpdate;
			}
			return false;
		}
	}
}
