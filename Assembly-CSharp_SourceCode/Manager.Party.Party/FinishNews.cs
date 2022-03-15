using System;
using PartyLink;

namespace Manager.Party.Party
{
	[Serializable]
	public class FinishNews : ICommandParam
	{
		public uint IpAddress;

		public bool[] IsValids = new bool[2];

		public bool[] GaugeClears = new bool[2];

		public uint[] GaugeStockNums = new uint[2];

		public Command getCommand()
		{
			return Command.FinishNews;
		}

		public FinishNews()
		{
			Clear();
		}

		public FinishNews(uint ipAddress, bool gaugeClear0, uint gaugeStockNum0, bool gaugeClear1, uint gaugeStockNum1)
		{
			IpAddress = ipAddress;
			IsValids[0] = true;
			IsValids[1] = true;
			GaugeClears[0] = gaugeClear0;
			GaugeClears[1] = gaugeClear1;
			GaugeStockNums[0] = gaugeStockNum0;
			GaugeStockNums[1] = gaugeStockNum1;
		}

		public void Clear()
		{
			IpAddress = 0u;
			for (int i = 0; i < 2; i++)
			{
				IsValids[i] = false;
				GaugeClears[i] = false;
				GaugeStockNums[i] = 0u;
			}
		}
	}
}
