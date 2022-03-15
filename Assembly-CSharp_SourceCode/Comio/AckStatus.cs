namespace Comio
{
	public enum AckStatus
	{
		Ok = 1,
		SumError = 2,
		ParityError = 3,
		FramingError = 4,
		AckStatusOverRunError = 5,
		AckStatusRecvBfOverFlow = 6,
		AckStatusInvalid = 255
	}
}
