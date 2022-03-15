using System;
using PartyLink;

namespace Manager.Party.Party
{
	[Serializable]
	public class StartPlay : ICommandParam
	{
		public long MaxMeasure;

		public long MyMeasure;

		public Command getCommand()
		{
			return Command.StartPlay;
		}

		public StartPlay(long maxMeasure, long myMeasure)
		{
			MaxMeasure = maxMeasure;
			MyMeasure = myMeasure;
		}

		public StartPlay()
		{
			Clear();
		}

		public void Clear()
		{
			MaxMeasure = 0L;
			MyMeasure = 0L;
		}

		public override string ToString()
		{
			return string.Concat(string.Concat("" + getCommand(), MaxMeasure), MyMeasure);
		}
	}
}
