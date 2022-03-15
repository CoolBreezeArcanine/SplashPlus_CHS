namespace Process
{
	public struct PhotoTiming
	{
		public long TakeTime;

		public bool Took;

		public PhotoTiming(long time)
		{
			TakeTime = time;
			Took = false;
		}
	}
}
