using System;
using PartyLink;

namespace Manager.Party.Party
{
	[Serializable]
	public class Kick : ICommandParam
	{
		public RecruitInfo RecruitInfo;

		public KickBy KickBy { get; }

		public Command getCommand()
		{
			return Command.Kick;
		}

		private Kick()
			: this(KickBy.None)
		{
		}

		public Kick(KickBy arg)
		{
			KickBy = arg;
			RecruitInfo = new RecruitInfo();
		}

		public Kick(KickBy arg, RecruitInfo recruitInfo)
		{
			KickBy = arg;
			RecruitInfo = new RecruitInfo(recruitInfo);
		}
	}
}
