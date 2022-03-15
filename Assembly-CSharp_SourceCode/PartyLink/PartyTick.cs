using System;

namespace PartyLink
{
	public class PartyTick
	{
		private long _resetTick;

		public PartyTick()
		{
			reset();
		}

		public void reset()
		{
			_resetTick = DateTime.Now.Ticks;
		}

		public long getUsec()
		{
			return (DateTime.Now.Ticks - _resetTick) / 10;
		}
	}
}
