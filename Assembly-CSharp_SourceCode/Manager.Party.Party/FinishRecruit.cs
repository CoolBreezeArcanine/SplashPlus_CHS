using System;
using PartyLink;

namespace Manager.Party.Party
{
	[Serializable]
	public class FinishRecruit : ICommandParam
	{
		public RecruitInfo RecruitInfo;

		public Command getCommand()
		{
			return Command.FinishRecruit;
		}

		public FinishRecruit()
		{
			RecruitInfo = new RecruitInfo();
		}

		public FinishRecruit(RecruitInfo recruitInfo)
		{
			RecruitInfo = new RecruitInfo(recruitInfo);
		}
	}
}
