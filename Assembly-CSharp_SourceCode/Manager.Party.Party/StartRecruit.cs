using System;
using PartyLink;

namespace Manager.Party.Party
{
	[Serializable]
	public class StartRecruit : ICommandParam
	{
		public RecruitInfo RecruitInfo;

		public Command getCommand()
		{
			return Command.StartRecruit;
		}

		public StartRecruit()
		{
			RecruitInfo = new RecruitInfo();
		}

		public StartRecruit(RecruitInfo recruitInfo)
		{
			RecruitInfo = new RecruitInfo(recruitInfo);
		}
	}
}
