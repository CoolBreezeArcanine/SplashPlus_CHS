using System;

namespace PartyLink
{
	[Serializable]
	public class HeartBeatRequest : ICommandParam
	{
		public Command getCommand()
		{
			return Command.HeartBeatRequest;
		}
	}
}
