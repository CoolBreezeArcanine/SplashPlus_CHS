using System;
using System.Collections.Generic;
using System.Linq;
using PartyLink;

namespace Manager.Party.Party
{
	public class Recruit
	{
		private readonly string _name;

		private readonly RecruitList _list;

		public Recruit(string name)
		{
			_name = name;
			_list = new RecruitList();
		}

		public void Add(RecruitInfo addInfo)
		{
			int num = _list.FindIndex((RecruitInfo info) => info.IpU32 == addInfo.IpU32);
			if (num >= 0)
			{
				_list[num] = addInfo;
			}
			else
			{
				_list.Add(addInfo);
			}
		}

		public void Remove(RecruitInfo reomveInfo)
		{
			RecruitInfo recruitInfo = _list.Find((RecruitInfo info) => info.IpU32 == reomveInfo.IpU32);
			if (recruitInfo != null && recruitInfo.StartTime == reomveInfo.StartTime)
			{
				_list.Remove(recruitInfo);
			}
		}

		public void Update(DateTime now)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < _list.Count; i++)
			{
				if (_list[i].RecvTime.AddSeconds(60.0) <= now)
				{
					list.Add(i);
				}
			}
			for (int num = list.Count - 1; num >= 0; num--)
			{
				_list.RemoveAt(num);
			}
		}

		public RecruitList GetList()
		{
			return _list;
		}

		public RecruitList GetListWithoutMe(int mockID = 0)
		{
			uint myIp = PartyLink.Util.MyIpAddress(mockID).ToNetworkByteOrderU32();
			return new RecruitList(_list.Where((RecruitInfo info) => myIp != info.IpU32));
		}

		public void Info(ref string os)
		{
			os = os + _name + "\n";
			os += _list;
		}
	}
}
