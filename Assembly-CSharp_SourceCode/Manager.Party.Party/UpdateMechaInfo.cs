using System;
using PartyLink;

namespace Manager.Party.Party
{
	[Serializable]
	public class UpdateMechaInfo : ICommandParam
	{
		public MechaInfo MechaInfo;

		public Command getCommand()
		{
			return Command.UpdateMechaInfo;
		}

		public UpdateMechaInfo()
		{
			MechaInfo = new MechaInfo();
			Clear();
		}

		public UpdateMechaInfo(MechaInfo info)
		{
			MechaInfo = new MechaInfo(info);
		}

		public void Info(ref string os)
		{
			os = string.Concat(os, MechaInfo, "\n");
		}

		public void Clear()
		{
			MechaInfo.Clear();
		}
	}
}
