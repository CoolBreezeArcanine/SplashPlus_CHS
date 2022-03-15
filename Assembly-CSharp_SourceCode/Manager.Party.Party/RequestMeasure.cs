using System;
using PartyLink;

namespace Manager.Party.Party
{
	[Serializable]
	public class RequestMeasure : ICommandParam
	{
		public Command getCommand()
		{
			return Command.RequestMeasure;
		}
	}
}
