using System;

namespace PartyLink
{
	[Serializable]
	public class HeartBeatResponse : ICommandParam
	{
		public Command getCommand()
		{
			return Command.HeartBeatResponse;
		}
	}
}
