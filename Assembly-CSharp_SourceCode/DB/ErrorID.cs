namespace DB
{
	public enum ErrorID
	{
		LedBoard_Left_InitError = 0,
		LedBoard_Left_Timeout = 1,
		LedBoard_Left_ResponseError = 2,
		LedBoard_Left_FirmError = 3,
		LedBoard_Left_FirmVerError = 4,
		LedBoard_Right_InitError = 5,
		LedBoard_Right_Timeout = 6,
		LedBoard_Right_ResponseError = 7,
		LedBoard_Right_FirmError = 8,
		LedBoard_Right_FirmVerError = 9,
		Begin = 0,
		End = 10,
		Invalid = -1
	}
}
