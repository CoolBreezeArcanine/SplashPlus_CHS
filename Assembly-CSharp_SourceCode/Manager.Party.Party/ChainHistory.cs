using System;
using MAI2.Memory;

namespace Manager.Party.Party
{
	[Serializable]
	public class ChainHistory
	{
		public int PacketNo;

		public int Chain;

		public ChainHistory(int packetNo, int chain)
		{
			PacketNo = packetNo;
			Chain = chain;
		}

		public ChainHistory()
		{
			Clear();
		}

		public ChainHistory(ChainHistory src)
		{
			CopyFrom(src);
		}

		public void CopyFrom(ChainHistory src)
		{
			PacketNo = src.PacketNo;
			Chain = src.Chain;
		}

		public void Clear()
		{
			PacketNo = 0;
			Chain = 0;
		}

		public int Serialize(int pos, Chunk chunk)
		{
			pos = chunk.writeS32(pos, PacketNo);
			pos = chunk.writeS32(pos, Chain);
			return pos;
		}

		public ChainHistory Deserialize(ref int pos, Chunk chunk)
		{
			PacketNo = chunk.readS32(ref pos);
			Chain = chunk.readS32(ref pos);
			return this;
		}

		public void Info(ref string os)
		{
			os += "ChainHistory{\n";
			os = os + PacketNo + ":" + Chain;
			os += "}\n";
		}

		public override string ToString()
		{
			string os = "";
			Info(ref os);
			return os;
		}
	}
}
