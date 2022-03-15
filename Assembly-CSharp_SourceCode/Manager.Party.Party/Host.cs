using System;
using System.Collections.Generic;
using System.Linq;
using DB;
using MAI2;
using PartyLink;

namespace Manager.Party.Party
{
	public class Host : StateMachine<Host, PartyPartyHostStateID>, IDisposable
	{
		private ListenSocket _listenSocket;

		private BroadcastSocket _broadcastSocket;

		private MemberList _memberList;

		private RecruitInfo _recruitInfo;

		private DateTime _refreshRecruitTime;

		private PartyPlayInfo _partyPlayInfo;

		private string _message;

		private PartyMemberPing _partyMemberPing;

		private readonly byte[] _rankCalcBuf;

		private readonly int[] _scoreCalcBuf;

		private bool _alreadyDisposed;

		private const int MockID = -1;

		private Queue<ChainHistory> _chainHistory;

		private int _sendPartyPlayInfoCount;

		private const uint ChainLowestMemberNum = 2u;

		public Host(string name)
		{
			_listenSocket = new ListenSocket(name + "Listen", -1);
			_broadcastSocket = new BroadcastSocket(name + "BroadCast", -1);
			_memberList = new MemberList();
			_recruitInfo = new RecruitInfo();
			_refreshRecruitTime = DateTime.MinValue;
			_partyPlayInfo = new PartyPlayInfo();
			_message = "";
			_partyMemberPing = new PartyMemberPing();
			_rankCalcBuf = new byte[PartyLink.Party.c_maxMecha * 2];
			_scoreCalcBuf = new int[PartyLink.Party.c_maxMecha * 2];
			_chainHistory = new Queue<ChainHistory>();
			SetCurrentStateID(PartyPartyHostStateID.First);
		}

		~Host()
		{
			DisposeImpl(disposing: false);
		}

		public void Dispose()
		{
			DisposeImpl(disposing: true);
		}

		private void DisposeImpl(bool disposing)
		{
			if (!_alreadyDisposed)
			{
				if (disposing)
				{
					PartyLink.Util.SafeDispose(ref _listenSocket);
					PartyLink.Util.SafeDispose(ref _broadcastSocket);
					PartyLink.Util.SafeDispose(_memberList);
					_memberList = null;
					_recruitInfo = null;
					_partyPlayInfo = null;
					_partyMemberPing = null;
					GC.SuppressFinalize(this);
				}
				_alreadyDisposed = true;
			}
		}

		public void Update()
		{
			updateState(-1f);
			ExecuteAccept();
			foreach (Member member in _memberList)
			{
				member.Update();
			}
			UpdateMemberState();
			UpdateUserInfo();
			UpdatePartyMemberPing();
		}

		public void Initialize()
		{
			SetCurrentStateID(PartyPartyHostStateID.First);
		}

		public void Terminate()
		{
			SetCurrentStateID(PartyPartyHostStateID.Finish);
		}

		public void Start()
		{
			SetCurrentStateID(PartyPartyHostStateID.Setup);
		}

		public void Wait()
		{
			if (IsNormal())
			{
				SetCurrentStateID(PartyPartyHostStateID.Wait);
			}
		}

		public void SelectMusic()
		{
			SetCurrentStateID(PartyPartyHostStateID.Wait);
		}

		public void StartRecruit(MechaInfo info, RecruitStance stanceID)
		{
			_recruitInfo = new RecruitInfo(Party.Get().GetGroup(), Party.Get().GetEventMode(), info, DateTime.Now, stanceID);
			SetCurrentStateID(PartyPartyHostStateID.Recruit);
		}

		public void CancelRecruit()
		{
			foreach (Member member in _memberList)
			{
				member.KickByCancel();
			}
			SetCurrentStateID(PartyPartyHostStateID.Wait);
		}

		public void FinishRecruit()
		{
			if (IsRecruit())
			{
				SetCurrentStateID(PartyPartyHostStateID.FinishRecruit);
			}
		}

		public void BeginPlay()
		{
			SetCurrentStateID(PartyPartyHostStateID.BeginPlay);
		}

		public void Ready()
		{
			SetCurrentStateID(PartyPartyHostStateID.Ready);
		}

		private void NotifyPartyPlayInfo(PartyPlayInfo info)
		{
			PartyPlayInfo i = new PartyPlayInfo(info);
			foreach (Member member in _memberList)
			{
				if (member.IsJoin())
				{
					member.NotifyPartyPlayInfo(i);
				}
			}
		}

		public PartyPartyJoinResultID RequestJoin(Member member, RequestJoin info)
		{
			if (!IsRecruit())
			{
				_message = "requestJoin refused because NoRecruit";
				return PartyPartyJoinResultID.NoRecruit;
			}
			if (PartyLink.Party.c_maxMecha <= GetEntryNumber())
			{
				_message = "requestJoin refused because Full.";
				return PartyPartyJoinResultID.Full;
			}
			if (_recruitInfo.GroupID != info.GroupID)
			{
				_message = "requestJoin refused because Different Group.";
				return PartyPartyJoinResultID.DifferentGroup;
			}
			if (_recruitInfo.EventModeID != info.EventModeID)
			{
				_message = "requestJoin refused because Different Event.";
				return PartyPartyJoinResultID.DifferentEventMode;
			}
			if (_recruitInfo.MusicID != info.MechaInfo.MusicID)
			{
				_message = "requestJoin refused because Different Music ID.";
				return PartyPartyJoinResultID.DifferentMusic;
			}
			if (member.IsJoin())
			{
				_message = "requestJoin refused because AlreadyJoined.";
				return PartyPartyJoinResultID.AlreadyJoined;
			}
			member.Join(info.MechaInfo);
			SendPartyMemberInfo();
			SendRecruitInfo();
			return PartyPartyJoinResultID.Success;
		}

		public bool IsNormal()
		{
			return GetCurrentStateID().IsNormal();
		}

		public bool IsError()
		{
			return GetCurrentStateID() == PartyPartyHostStateID.Error;
		}

		public bool IsWorking()
		{
			return GetCurrentStateID().IsWorking();
		}

		public bool IsPlay()
		{
			return GetCurrentStateID().IsPlay();
		}

		public bool IsRecruit()
		{
			return GetCurrentStateID().IsRecruit();
		}

		public int GetEntryNumber()
		{
			return _memberList.Count((Member c) => c.IsJoin());
		}

		public bool IsMemberState(PartyPartyClientStateID id)
		{
			if (GetEntryNumber() == 0)
			{
				return false;
			}
			foreach (Member member in _memberList)
			{
				if (member.IsJoin() && member.IsActive() && member.GetCurrentStateID() != id)
				{
					return false;
				}
			}
			return true;
		}

		public uint GetRecvCount(int memberNo, Command command)
		{
			_ = _memberList.Count;
			return _memberList[memberNo].GetRecvCount(command);
		}

		public MemberList GetMemberList()
		{
			return _memberList;
		}

		public void Info(ref string os)
		{
			os = os + "HostState " + base.stateString + "\n";
			os = os + "message " + _message + "\n";
			os += $"listenSocket {_listenSocket}\n";
			os += $"broadcastSocket {_broadcastSocket}\n";
			os += "recruitInfo=\n";
			os += _recruitInfo;
			os += "_partyPlayInfo\n";
			_partyPlayInfo.Info(ref os);
			if (_memberList.Count == 0)
			{
				os += "member empty\n";
				return;
			}
			foreach (Member member in _memberList)
			{
				os += "member.base ";
				member.InfoBase(ref os);
			}
			foreach (Member member2 in _memberList)
			{
				os += "member.user   ";
				member2.InfoUser(ref os);
			}
			foreach (Member member3 in _memberList)
			{
				os += "member.play   ";
				member3.InfoPlay(ref os);
				os += "\n";
			}
			foreach (Member member4 in _memberList)
			{
				os += "member.measure ";
				member4.InfoMeasure(ref os);
				os += "\n";
			}
		}

		public PartyPlayInfo GetPartyPlayInfo()
		{
			return _partyPlayInfo;
		}

		public void UpdateRecruitMechaInfo(MechaInfo info)
		{
			_recruitInfo.MechaInfo = new MechaInfo(info);
			if (IsRecruit())
			{
				SendRecruitInfo();
			}
		}

		public PartyMemberPing GetPartyMemberPing()
		{
			return _partyMemberPing;
		}

		private void Clear()
		{
			_listenSocket.close();
			_broadcastSocket.close();
			_memberList.ClearWithDispose();
			_partyPlayInfo.Clear();
			_chainHistory.Clear();
		}

		private void ExecuteAccept()
		{
			IpAddress outAddress;
			NFSocket nFSocket = _listenSocket.acceptClient(out outAddress);
			if (nFSocket != null)
			{
				string text = "Party::Member";
				text += _memberList.Count;
				_memberList.erase_if((Member m) => m.GetAddress() == outAddress);
				_memberList.Add(new Member(text, this, nFSocket));
			}
		}

		private void SendPartyMemberInfo()
		{
			PartyMemberInfo partyMemberInfo = new PartyMemberInfo();
			int num = 0;
			foreach (Member member in _memberList)
			{
				if (member.IsJoin())
				{
					partyMemberInfo.MechaInfo[num] = new MechaInfo(member.GetMechaInfo());
					num++;
				}
			}
			foreach (Member member2 in _memberList)
			{
				if (member2.IsJoin())
				{
					member2.SendPartyMemberInfo(partyMemberInfo);
				}
			}
		}

		private void SendPartyMemberState()
		{
			PartyMemberState partyMemberState = new PartyMemberState();
			int num = 0;
			foreach (Member member in _memberList)
			{
				member.SetStateChanged(stateChanged: false);
				if (member.IsJoin())
				{
					partyMemberState.SetState(num, member.GetCurrentStateID());
					member.SetStateChanged(stateChanged: false);
					num++;
				}
			}
			foreach (Member member2 in _memberList)
			{
				if (member2.IsJoin())
				{
					member2.SendPartyMemberState(partyMemberState);
				}
			}
		}

		private void SendRecruitInfo()
		{
			_recruitInfo.JoinNumber = GetEntryNumber();
			_broadcastSocket.sendClass(new StartRecruit(_recruitInfo));
			_refreshRecruitTime = DateTime.Now;
		}

		private void UpdateMemberState()
		{
			bool flag = false;
			foreach (Member member in _memberList)
			{
				if (member.IsStateChanged())
				{
					flag = true;
				}
			}
			if (flag)
			{
				SendPartyMemberState();
			}
		}

		private void UpdateUserInfo()
		{
			bool flag = false;
			foreach (Member member in _memberList)
			{
				if (member.IsUserInfoChanged())
				{
					member.SetUserInfoChanged(userInfoChanged: false);
					flag = true;
				}
			}
			if (flag)
			{
				SendPartyMemberInfo();
			}
		}

		private void UpdatePartyMemberPing()
		{
			int num = 0;
			_partyMemberPing.Clear();
			foreach (Member member in _memberList)
			{
				if (member.IsJoin())
				{
					if (member.IsActive())
					{
						_partyMemberPing.Ping[num] = member.GetPingFrame();
					}
					num++;
				}
			}
		}

		private void EraseDisconnectMember(bool isAutoSend)
		{
			int count = _memberList.Count;
			_memberList.erase_if((Member m) => m == null || !m.IsActive());
			if (count == _memberList.Count)
			{
				return;
			}
			foreach (Member member in _memberList)
			{
				if (member.IsJoin())
				{
					member.SetStateChanged(stateChanged: true);
				}
			}
			if (isAutoSend)
			{
				SendPartyMemberInfo();
				SendPartyMemberState();
				SendRecruitInfo();
			}
		}

		private void SendPartyPlayInfo(bool checkFullChain)
		{
			int num = 0;
			foreach (Member member2 in _memberList)
			{
				if (member2.IsJoin())
				{
					_partyPlayInfo.Member[num] = (MemberPlayInfo)member2.GetLastClientPlayInfo().GetMemberPlayInfo().Clone();
					_partyPlayInfo.Member[num].IsActive = member2.IsActive();
					_partyPlayInfo.Member[num].IpAddress = member2.GetMechaInfo().IpAddress;
					num++;
				}
			}
			num = 0;
			MemberPlayInfo[] member = _partyPlayInfo.Member;
			foreach (MemberPlayInfo memberPlayInfo in member)
			{
				_scoreCalcBuf[num++] = memberPlayInfo.Achieves[0];
				_scoreCalcBuf[num++] = memberPlayInfo.Achieves[1];
			}
			GetRank(_scoreCalcBuf, _rankCalcBuf);
			num = 0;
			member = _partyPlayInfo.Member;
			foreach (MemberPlayInfo obj in member)
			{
				obj.Rankings[0] = _rankCalcBuf[num++];
				obj.Rankings[1] = _rankCalcBuf[num++];
			}
			ChainHistory[] chainHistory = _partyPlayInfo.ChainHistory;
			for (int i = 0; i < chainHistory.Length; i++)
			{
				chainHistory[i].Clear();
			}
			int num2 = 0;
			foreach (ChainHistory item in _chainHistory)
			{
				if (num2 < PartyLink.Party.c_chainHistorySize)
				{
					_partyPlayInfo.ChainHistory[num2] = item;
					num2++;
					continue;
				}
				break;
			}
			if (checkFullChain)
			{
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				foreach (Member member3 in _memberList)
				{
					ClientPlayInfo lastClientPlayInfo = member3.GetLastClientPlayInfo();
					if (!member3.IsJoin())
					{
						continue;
					}
					for (int j = 0; j < lastClientPlayInfo.GameOvers.Length; j++)
					{
						if (lastClientPlayInfo.GameOvers[j])
						{
							num5++;
						}
					}
					for (int k = 0; k < lastClientPlayInfo.MissNums.Length; k++)
					{
						if (lastClientPlayInfo.MissNums[k] > 0)
						{
							num6++;
						}
					}
					if (!member3.IsActive())
					{
						continue;
					}
					num3++;
					for (int l = 0; l < 2; l++)
					{
						if (!lastClientPlayInfo.GameOvers[l] && lastClientPlayInfo.FullCombos[l] != 0 && lastClientPlayInfo.MissNums[l] == 0)
						{
							num4++;
						}
					}
				}
				_partyPlayInfo.IsFullChain = false;
				if ((long)num3 >= 2L && _partyPlayInfo.ChainMiss == 0 && num5 == 0 && num6 == 0 && num3 == num4)
				{
					_partyPlayInfo.IsFullChain = true;
				}
				_partyPlayInfo.CalcStatus = 1;
			}
			NotifyPartyPlayInfo(_partyPlayInfo);
		}

		public void GetRank(int[] scoreList, byte[] outRankList)
		{
			if (scoreList == null || outRankList == null || scoreList.Length != outRankList.Length)
			{
				return;
			}
			int num = scoreList.Length;
			for (int i = 0; i < num; i++)
			{
				int num2 = scoreList[i];
				int num3 = 0;
				for (int j = 0; j < num; j++)
				{
					if (scoreList[j] > num2)
					{
						num3++;
					}
				}
				outRankList[i] = (byte)(num3 + 1);
			}
		}

		public void Enter_First()
		{
			Clear();
		}

		public void Execute_First()
		{
		}

		public void Enter_Setup()
		{
			Clear();
		}

		public void Execute_Setup()
		{
			_listenSocket.open(PartyLink.Party.CPortNumber);
			SetCurrentStateID(_listenSocket.isValid() ? PartyPartyHostStateID.Wait : PartyPartyHostStateID.Error);
		}

		public void Enter_Wait()
		{
			_recruitInfo = new RecruitInfo();
			_partyPlayInfo = new PartyPlayInfo();
			_memberList.ClearWithDispose();
		}

		public void Execute_Wait()
		{
		}

		public void Enter_Recruit()
		{
			_broadcastSocket.close();
			_broadcastSocket.open(new IpAddress(PartyLink.Util.BroadcastAddress()), PartyLink.Party.CPortNumber);
			_broadcastSocket.isValid();
			_partyPlayInfo = new PartyPlayInfo();
			SendRecruitInfo();
		}

		public void Execute_Recruit()
		{
			DateTime now = DateTime.Now;
			if (_refreshRecruitTime.AddSeconds(PartyLink.Party.c_recruitRefresh) < now)
			{
				SendRecruitInfo();
			}
			EraseDisconnectMember(isAutoSend: true);
		}

		public void Leave_Recruit()
		{
			FinishRecruit info = new FinishRecruit(_recruitInfo);
			_broadcastSocket.sendClass(info);
		}

		public void Enter_FinishRecruit()
		{
			foreach (Member member in _memberList)
			{
				if (member.IsJoin())
				{
					member.StartState(PartyPartyClientStateID.ToReady);
				}
			}
		}

		public void Execute_FinishRecruit()
		{
		}

		public void Execute_BeginPlay()
		{
			if (IsMemberState(PartyPartyClientStateID.BeginPlay))
			{
				SetCurrentStateID(PartyPartyHostStateID.AllBeginPlay);
			}
		}

		public void Enter_AllBeginPlay()
		{
			EraseDisconnectMember(isAutoSend: false);
			SendPartyMemberInfo();
			foreach (Member member in _memberList)
			{
				if (member.IsJoin())
				{
					member.StartState(PartyPartyClientStateID.AllBeginPlay);
				}
			}
		}

		public void Execute_AllBeginPlay()
		{
		}

		public void Execute_Ready()
		{
			if (IsMemberState(PartyPartyClientStateID.Ready))
			{
				SetCurrentStateID(PartyPartyHostStateID.Sync);
			}
		}

		public void Enter_Sync()
		{
			foreach (Member member in _memberList)
			{
				if (member.IsJoin())
				{
					member.RequestMeasure();
				}
			}
		}

		public void Execute_Sync()
		{
			uint num = 0u;
			uint num2 = 0u;
			foreach (Member member in _memberList)
			{
				if (member.IsJoin() && member.IsActive())
				{
					num2++;
				}
				if (member.IsResponsedMeasure())
				{
					num++;
				}
			}
			if (num == num2)
			{
				SetCurrentStateID(PartyPartyHostStateID.Play);
			}
		}

		public void Enter_Play()
		{
			long num = 0L;
			foreach (Member member in _memberList)
			{
				if (member.IsResponsedMeasure() && member.IsJoin() && member.IsActive())
				{
					num = Math.Max(num, member.GetMeasureDiffUsec());
				}
			}
			foreach (Member member2 in _memberList)
			{
				if (member2.IsJoin())
				{
					member2.StartPlay(num, member2.GetMeasureDiffUsec());
				}
			}
			_partyPlayInfo.Clear();
		}

		public void Execute_Play()
		{
			int num = int.MaxValue;
			int num2 = 0;
			foreach (Member member in _memberList)
			{
				if (!member.IsJoin() || !member.IsActive())
				{
					continue;
				}
				Queue<ClientPlayInfo> clientPlayInfo = member.GetClientPlayInfo();
				if (!member.IsFinishPlay() || clientPlayInfo.Count != 0)
				{
					num2++;
					if (clientPlayInfo.Count == 0)
					{
						return;
					}
					ClientPlayInfo clientPlayInfo2 = clientPlayInfo.Peek();
					if (clientPlayInfo2.Count < num)
					{
						num = clientPlayInfo2.Count;
					}
				}
			}
			if (_memberList.Any((Member x) => !x.IsActive()))
			{
				foreach (Member member2 in _memberList)
				{
					member2.KickByClientDisconnect();
					num2 = 0;
				}
			}
			if (num2 == 0)
			{
				SendPartyPlayInfo(checkFullChain: true);
				SetCurrentStateID(PartyPartyHostStateID.News);
			}
			else
			{
				if (num == int.MaxValue)
				{
					return;
				}
				foreach (Member member3 in _memberList)
				{
					if (member3.IsJoin() && member3.IsActive())
					{
						member3.UpdateLastClientPlayInfo(num);
					}
				}
				int num3 = 0;
				foreach (Member member4 in _memberList)
				{
					if (member4.IsJoin() && member4.IsActive() && member4.GetLastClientPlayInfo().IsAlive())
					{
						num3 += member4.GetLastClientPlayInfo().GetAliveNum();
					}
				}
				if (2L <= (long)num3)
				{
					foreach (Member member5 in _memberList)
					{
						if (!member5.IsJoin() || !member5.IsActive())
						{
							continue;
						}
						Queue<ClientPlayInfo> clientPlayInfo3 = member5.GetClientPlayInfo();
						if (clientPlayInfo3.Count == 0)
						{
							continue;
						}
						ClientPlayInfo clientPlayInfo4 = clientPlayInfo3.Peek();
						if (clientPlayInfo4.Count != num)
						{
							continue;
						}
						for (int i = 0; i < 2; i++)
						{
							if (clientPlayInfo4.IsValids[i])
							{
								if (clientPlayInfo4.Miss[i])
								{
									_partyPlayInfo.ChainMiss++;
									_partyPlayInfo.Chain = 0;
									_partyPlayInfo.IsFullChain = false;
								}
								_partyPlayInfo.Chain += clientPlayInfo4.Combos[i];
								if (_partyPlayInfo.MaxChain < _partyPlayInfo.Chain)
								{
									_partyPlayInfo.MaxChain = _partyPlayInfo.Chain;
								}
							}
						}
					}
				}
				else
				{
					_partyPlayInfo.Chain = 0;
					_partyPlayInfo.IsFullChain = false;
				}
				foreach (Member member6 in _memberList)
				{
					if (member6.IsJoin() && member6.IsActive())
					{
						member6.PopClientPlayInfo(num);
					}
				}
				_sendPartyPlayInfoCount++;
				_chainHistory.Enqueue(new ChainHistory(_sendPartyPlayInfoCount, _partyPlayInfo.Chain));
				while (PartyLink.Party.c_chainHistorySize < _chainHistory.Count)
				{
					_chainHistory.Dequeue();
				}
				SendPartyPlayInfo(checkFullChain: false);
			}
		}

		public void Enter_News()
		{
			foreach (Member member in _memberList)
			{
				if (member.IsJoin())
				{
					member.StartState(PartyPartyClientStateID.News);
				}
			}
		}

		public void Execute_News()
		{
			if (IsMemberState(PartyPartyClientStateID.NewsEnd))
			{
				SetCurrentStateID(PartyPartyHostStateID.Result);
			}
		}

		public void Enter_Result()
		{
			NotifyPartyPlayInfo(_partyPlayInfo);
			foreach (Member member in _memberList)
			{
				if (member.IsJoin())
				{
					member.StartState(PartyPartyClientStateID.Result);
				}
			}
		}

		public void Execute_Result()
		{
		}

		public void Leave_Result()
		{
			_partyPlayInfo = new PartyPlayInfo();
		}

		public void Enter_Finish()
		{
			Clear();
		}

		public void Execute_Finish()
		{
		}

		public void Enter_Error()
		{
			Clear();
		}

		public void Execute_Error()
		{
		}

		public void Execute_End()
		{
		}

		public void Execute_Begin()
		{
		}

		public void Execute_Invalid()
		{
		}

		public PartyPartyHostStateID GetCurrentStateID()
		{
			return getCurrentState();
		}

		private void SetCurrentStateID(PartyPartyHostStateID nextState)
		{
			setNextState(nextState);
			updateState(-1f);
		}
	}
}
