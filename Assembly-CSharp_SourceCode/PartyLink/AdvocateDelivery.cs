using System;
using MAI2.Memory;

namespace PartyLink
{
	[Serializable]
	public class AdvocateDelivery : ICommandParam
	{
		public uint _address;

		public Command getCommand()
		{
			return Command.AdvocateDelivery;
		}

		public int serialize(int pos, Chunk chunk)
		{
			pos = chunk.writeU32(pos, _address);
			return pos;
		}

		public AdvocateDelivery deserialize(ref int pos, Chunk chunk)
		{
			_address = chunk.readU32(ref pos);
			return this;
		}

		public AdvocateDelivery()
		{
			_address = IpAddress.Any.ToNetworkByteOrderU32();
		}

		public AdvocateDelivery(IpAddress address)
		{
			_address = address.ToNetworkByteOrderU32();
		}
	}
}
