using System;
using DB;
using PartyLink;
using UnityEngine;

namespace Manager.Party.Party
{
	[Serializable]
	public class RecruitInfo
	{
		public MechaInfo MechaInfo;

		public int MusicID;

		public int GroupID;

		public bool EventModeID;

		public int JoinNumber;

		public int PartyStance;

		[SerializeField]
		private long _startTimeTicks;

		[SerializeField]
		private long _recvTimeTicks;

		public DateTime StartTime
		{
			get
			{
				return new DateTime(_startTimeTicks);
			}
			set
			{
				_startTimeTicks = value.Ticks;
			}
		}

		public DateTime RecvTime
		{
			get
			{
				return new DateTime(_recvTimeTicks);
			}
			set
			{
				_recvTimeTicks = value.Ticks;
			}
		}

		public uint IpU32 => MechaInfo?.IpAddress ?? 0;

		public IpAddress IpAddress => new IpAddress(IpU32);

		public RecruitInfo()
		{
			MechaInfo = new MechaInfo();
			MusicID = 0;
			GroupID = 1;
			EventModeID = false;
			JoinNumber = 0;
			PartyStance = 0;
			StartTime = DateTime.MinValue;
			RecvTime = DateTime.MinValue;
		}

		public RecruitInfo(RecruitInfo src)
		{
			MechaInfo = new MechaInfo(src.MechaInfo);
			MusicID = src.MusicID;
			GroupID = src.GroupID;
			EventModeID = src.EventModeID;
			JoinNumber = src.JoinNumber;
			PartyStance = src.PartyStance;
			StartTime = src.StartTime;
			RecvTime = src.RecvTime;
		}

		public RecruitInfo(MachineGroupID groupID, bool eventModeID, MechaInfo mechaInfo, DateTime startTime, RecruitStance stanceID)
		{
			MechaInfo = (MechaInfo)mechaInfo.Clone();
			GroupID = (int)groupID;
			EventModeID = eventModeID;
			MusicID = MechaInfo.MusicID;
			StartTime = startTime;
			JoinNumber = 0;
			PartyStance = (int)stanceID;
			RecvTime = DateTime.MinValue;
		}

		public bool IsFull()
		{
			return PartyLink.Party.c_maxMecha <= JoinNumber;
		}

		public override string ToString()
		{
			return string.Concat("musicID=", MusicID, ", groupID=", GroupID, ", stanceID=", PartyStance, ", eventModeID=", EventModeID.ToString(), ", startTime=", StartTime, ", recvTime=", RecvTime, ", joinNumber=", JoinNumber, ", ", MechaInfo, "\n");
		}
	}
}
