using System;
using PartyLink;

namespace Manager.Party.Party
{
	[Serializable]
	public class ResponseMeasure : ICommandParam
	{
		public Command getCommand()
		{
			return Command.ResponseMeasure;
		}
	}
}
