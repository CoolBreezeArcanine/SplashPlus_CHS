using System;
using System.Collections.Generic;
using Manager.UserDatas;

namespace Datas.DebugData
{
	[Serializable]
	public class DebugUserScore
	{
		public List<UserScore> Basic = new List<UserScore>();

		public List<UserScore> Advanced = new List<UserScore>();

		public List<UserScore> Expart = new List<UserScore>();

		public List<UserScore> Master = new List<UserScore>();

		public List<UserScore> ReMaster = new List<UserScore>();
	}
}
