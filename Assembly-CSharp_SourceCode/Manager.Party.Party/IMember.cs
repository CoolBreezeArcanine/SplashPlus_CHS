using System.Collections.Generic;
using PartyLink;

namespace Manager.Party.Party
{
	public interface IMember
	{
		MechaInfo GetMechaInfo();

		IpAddress GetAddress();

		bool IsJoin();

		bool IsActive();

		Queue<ClientPlayInfo> GetClientPlayInfo();

		void InfoBase(ref string os);

		void InfoUser(ref string os);

		void InfoPlay(ref string os);

		void InfoMeasure(ref string os);
	}
}
