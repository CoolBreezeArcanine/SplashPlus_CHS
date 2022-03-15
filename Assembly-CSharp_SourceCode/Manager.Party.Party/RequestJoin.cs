using System;
using DB;
using PartyLink;

namespace Manager.Party.Party
{
	[Serializable]
	public class RequestJoin : ICommandParam
	{
		public MechaInfo MechaInfo;

		public int GroupID;

		public bool EventModeID;

		public Command getCommand()
		{
			return Command.RequestJoin;
		}

		public RequestJoin()
		{
			MechaInfo = new MechaInfo();
			GroupID = 1;
			EventModeID = false;
		}

		public RequestJoin(MachineGroupID groupID, bool eventModeID, MechaInfo mechaInfo)
		{
			MechaInfo = new MechaInfo(mechaInfo);
			GroupID = (int)groupID;
			EventModeID = eventModeID;
		}
	}
}
