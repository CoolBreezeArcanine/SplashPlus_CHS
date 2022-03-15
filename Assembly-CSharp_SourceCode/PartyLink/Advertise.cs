using System;
using System.Collections.Generic;
using DB;
using MAI2;

namespace PartyLink
{
	public static class Advertise
	{
		public abstract class Advertise_Base : ICommandParam
		{
			public uint _ipAddress;

			public int _group;

			public int _kind;

			public abstract Command getCommand();

			public Advertise_Base()
			{
				_ipAddress = 0u;
				_group = 1;
				_kind = 0;
			}
		}

		[Serializable]
		public class AdvertiseRequest : Advertise_Base
		{
			public override Command getCommand()
			{
				return Command.AdvertiseRequest;
			}

			public AdvertiseRequest()
			{
				_ipAddress = 0u;
				_group = 1;
				_kind = 0;
			}

			public AdvertiseRequest(uint ipAddress, MachineGroupID group, Kind kind)
			{
				_ipAddress = ipAddress;
				_group = (int)group;
				_kind = (int)kind;
			}

			public override string ToString()
			{
				return string.Concat(string.Concat(string.Concat(string.Concat("", getCommand(), " "), _ipAddress.convIpString(), " "), _group, " "), _kind, " ");
			}
		}

		[Serializable]
		public class AdvertiseResponse : Advertise_Base
		{
			public override Command getCommand()
			{
				return Command.AdvertiseResponse;
			}

			public AdvertiseResponse()
			{
				_ipAddress = 0u;
				_group = 1;
				_kind = 0;
			}

			public AdvertiseResponse(uint ipAddress, MachineGroupID group, Kind kind)
			{
				_ipAddress = ipAddress;
				_group = (int)group;
				_kind = (int)kind;
			}

			public override string ToString()
			{
				return string.Concat(string.Concat(string.Concat(string.Concat("", getCommand(), " "), _ipAddress.convIpString(), " "), _group, " "), _kind, " ");
			}
		}

		[Serializable]
		public class AdvertiseGo : Advertise_Base
		{
			public long _maxUsec;

			public long _myUsec;

			public override Command getCommand()
			{
				return Command.AdvertiseGo;
			}

			public AdvertiseGo()
			{
				clear();
			}

			public AdvertiseGo(uint ipAddress, MachineGroupID group, Kind kind, long maxUsec, long myUsec)
			{
				_ipAddress = ipAddress;
				_group = (int)group;
				_kind = (int)kind;
				_maxUsec = maxUsec;
				_myUsec = myUsec;
			}

			public void clear()
			{
				_ipAddress = 0u;
				_group = 1;
				_kind = 0;
				_maxUsec = 0L;
				_myUsec = 0L;
			}

			public override string ToString()
			{
				return string.Concat(getCommand(), " address ", _ipAddress.convIpString(), " group ", _group, " kind ", _kind, " maxUsec ", _maxUsec, " myUsec ", _myUsec);
			}
		}

		public class Parameter
		{
			public ushort _portNumber;

			public Parameter()
			{
				clear();
			}

			public Parameter(Parameter arg)
			{
				_portNumber = arg._portNumber;
			}

			private void clear()
			{
				_portNumber = CPortNumber;
			}

			public void info(ref string os)
			{
				os += "(";
				os = os + " _portNumber " + _portNumber;
				os += ")\n";
			}
		}

		public enum Kind
		{
			None,
			Advertise,
			Ranking,
			Max
		}

		public interface IManager
		{
			void execute();

			void initialize(MachineGroupID group);

			void terminate();

			void start(Kind kind, int second);

			void exit();

			bool isGo();

			bool isStop();

			void info(ref string os);

			void infoDetail(ref string os);
		}

		private class Member
		{
			public uint _ipAddress;

			public long _measureUsec;

			public Member()
			{
				clear();
			}

			public void clear()
			{
				_ipAddress = 0u;
				_measureUsec = 0L;
			}
		}

		private class Manager : StateMachine<Manager, PartyAdvertiseStateID>, IManager, IDisposable
		{
			private BroadcastSocket _broadcastSocket;

			private UdpRecvSocket _udpSocket;

			private DateTime _startTime;

			private PartyTick _tick;

			private AdvertiseGo _goInfo;

			private int _waitSecond;

			private long _sendUsec;

			private Dictionary<uint, Member> _members;

			private uint _ignoreCount;

			private uint _requestCount;

			private uint _responseCount;

			private uint _goCount;

			private MachineGroupID _group;

			private Kind _kind;

			private bool _alreadyDisposed;

			private const int MockID = -1;

			private Manager(Manager other)
			{
			}

			public Manager()
				: this(0)
			{
			}

			public Manager(int mockID)
			{
				_broadcastSocket = new BroadcastSocket("Advertise::ServerBroadcast", -1);
				_udpSocket = new UdpRecvSocket("Advertise::ClientUdp", -1);
				_startTime = DateTime.MinValue;
				_tick = new PartyTick();
				_goInfo = new AdvertiseGo();
				_waitSecond = 0;
				_sendUsec = 0L;
				_members = new Dictionary<uint, Member>();
				_ignoreCount = 0u;
				_requestCount = 0u;
				_responseCount = 0u;
				_goCount = 0u;
				_group = MachineGroupID.OFF;
				_kind = Kind.None;
				_udpSocket.registCommand(Command.AdvertiseRequest, recvAdvertiseRequest);
				_udpSocket.registCommand(Command.AdvertiseResponse, recvAdvertiseResponse);
				_udpSocket.registCommand(Command.AdvertiseGo, recvAdvertiseGo);
			}

			~Manager()
			{
				dispose(disposing: false);
			}

			public void Dispose()
			{
				dispose(disposing: true);
			}

			protected virtual void dispose(bool disposing)
			{
				if (!_alreadyDisposed)
				{
					if (disposing)
					{
						Util.SafeDispose(ref _broadcastSocket);
						Util.SafeDispose(ref _udpSocket);
						_goInfo = null;
						_members = null;
						GC.SuppressFinalize(this);
					}
					_alreadyDisposed = true;
				}
			}

			public void initialize(MachineGroupID group)
			{
				clear();
				_group = group;
				setCurrentStateID(PartyAdvertiseStateID.Wait);
			}

			public void terminate()
			{
				clear();
				setCurrentStateID(PartyAdvertiseStateID.First);
			}

			public void clear()
			{
				_broadcastSocket.close();
				_udpSocket.close();
				_members.Clear();
			}

			public void start(Kind kind, int second)
			{
				_kind = kind;
				_waitSecond = second;
				start();
			}

			public void start()
			{
				clear();
				if (_group == MachineGroupID.OFF)
				{
					setCurrentStateID(PartyAdvertiseStateID.Wait);
					return;
				}
				if (!_broadcastSocket.open(new IpAddress(Util.BroadcastAddress()), getParam()._portNumber))
				{
					setCurrentStateID(PartyAdvertiseStateID.Wait);
					return;
				}
				IpAddress ipAddress = new IpAddress(Util.MyIpAddress(-1));
				if (ipAddress == IpAddress.Zero)
				{
					setCurrentStateID(PartyAdvertiseStateID.Wait);
					return;
				}
				_udpSocket.open(ipAddress, getParam()._portNumber);
				_udpSocket.isValid();
				setCurrentStateID(PartyAdvertiseStateID.Wait);
			}

			public void exit()
			{
				setCurrentStateID(PartyAdvertiseStateID.Go);
			}

			public void execute()
			{
				updateState();
				_udpSocket.recv();
			}

			public void updateState()
			{
				updateState(-1f);
			}

			public PartyAdvertiseStateID getState()
			{
				return getCurrentStateID();
			}

			public bool isGo()
			{
				return getState().IsGo();
			}

			public bool isStop()
			{
				return getState() == PartyAdvertiseStateID.First;
			}

			public bool isNormal()
			{
				return getState().IsNormal();
			}

			public void infoDetail(ref string os)
			{
				os += "Param ";
				getParam().info(ref os);
				os = os + "State " + getCurrentStateID().GetEnumName() + "\n";
				int num = (int)Math.Floor(Math.Abs(_startTime.Subtract(DateTime.Now).TotalSeconds));
				os = os + "_startTime remain " + (_waitSecond - num) + "\n";
				os = string.Concat(os, "Group ", _group, "\n");
				os = string.Concat(os, "goInfo ", _goInfo, "\n");
				os += "\n";
				os = os + "ignoreCount " + _ignoreCount + "\n";
				os = os + "requestCount " + _requestCount + "\n";
				os = os + "responseCount " + _responseCount + "\n";
				os = os + "goCount " + _goCount + "\n";
				os += "\n";
				os = string.Concat(os, "broadcastSocket ", _broadcastSocket, "\n");
				os = string.Concat(os, "udpSocket ", _udpSocket, "\n");
				os += "\n";
			}

			public void info(ref string os)
			{
				os = string.Concat(os, "Group ", _group, " ");
				os = os + "ignoreCount " + _ignoreCount + " ";
				os = os + "State " + getCurrentStateID().GetEnumName() + "\n";
				int num = (int)Math.Floor(Math.Abs(_startTime.Subtract(DateTime.Now).TotalSeconds));
				os = os + "_startTime remain " + (_waitSecond - num) + " ";
				os = string.Concat(os, "goInfo ", _goInfo, "\n");
				os = string.Concat(os, "udpSocket ", _udpSocket, "\n");
			}

			public bool isIgnore(Advertise_Base info)
			{
				if (Util.isMyIP(info._ipAddress, -1))
				{
					return true;
				}
				if (info._group != (int)_group)
				{
					return true;
				}
				if (info._kind != (int)_kind)
				{
					return true;
				}
				return false;
			}

			public void recvAdvertiseRequest(Packet packet)
			{
				AdvertiseRequest param = packet.getParam<AdvertiseRequest>();
				if (isIgnore(param))
				{
					_ignoreCount++;
					return;
				}
				if (getCurrentStateID() != PartyAdvertiseStateID.Wait)
				{
					_ignoreCount++;
					return;
				}
				_requestCount++;
				AdvertiseResponse advertiseResponse = new AdvertiseResponse(Util.MyIpAddress(-1).ToNetworkByteOrderU32(), _group, _kind);
				_broadcastSocket.sendToClass(new IpAddress(param._ipAddress), advertiseResponse);
			}

			public void recvAdvertiseResponse(Packet packet)
			{
				AdvertiseResponse param = packet.getParam<AdvertiseResponse>();
				if (isIgnore(param))
				{
					_ignoreCount++;
					return;
				}
				if (getCurrentStateID() != PartyAdvertiseStateID.Requested)
				{
					_ignoreCount++;
					return;
				}
				_responseCount++;
				if (!_members.ContainsKey(param._ipAddress))
				{
					Member member = new Member();
					member._ipAddress = param._ipAddress;
					if (_tick != null)
					{
						long usec = _tick.getUsec();
						member._measureUsec = usec - _sendUsec;
					}
					else
					{
						member._measureUsec = 0L;
					}
					_members.Add(member._ipAddress, member);
				}
			}

			public void recvAdvertiseGo(Packet packet)
			{
				AdvertiseGo param = packet.getParam<AdvertiseGo>();
				if (isIgnore(param))
				{
					_ignoreCount++;
				}
				else if (getCurrentStateID() == PartyAdvertiseStateID.Wait || getCurrentStateID() == PartyAdvertiseStateID.Requested)
				{
					_goCount++;
					_goInfo = param;
					setCurrentStateID(PartyAdvertiseStateID.Ready);
				}
				else
				{
					_ignoreCount++;
				}
			}

			public void Execute_First()
			{
			}

			public void Enter_Wait()
			{
				_startTime = DateTime.Now;
			}

			public void Execute_Wait()
			{
				DateTime now = DateTime.Now;
				if (_startTime.AddSeconds(_waitSecond) < now)
				{
					setCurrentStateID(PartyAdvertiseStateID.Requested);
				}
			}

			public void Enter_Requested()
			{
				_members.Clear();
				AdvertiseRequest advertiseRequest = new AdvertiseRequest(Util.MyIpAddress(-1).ToNetworkByteOrderU32(), _group, _kind);
				_broadcastSocket.sendClass(advertiseRequest);
				_tick.reset();
				_sendUsec = _tick.getUsec();
			}

			public void Execute_Requested()
			{
				if (_tick.getUsec() - _sendUsec < 1000000)
				{
					return;
				}
				long num = 0L;
				foreach (KeyValuePair<uint, Member> member in _members)
				{
					num = Math.Max(num, member.Value._measureUsec);
				}
				foreach (KeyValuePair<uint, Member> member2 in _members)
				{
					AdvertiseGo advertiseGo = new AdvertiseGo(Util.MyIpAddress(-1).ToNetworkByteOrderU32(), _group, _kind, num, member2.Value._measureUsec);
					_broadcastSocket.sendToClass(new IpAddress(member2.Key), advertiseGo);
				}
				_goInfo = new AdvertiseGo(Util.MyIpAddress(-1).ToNetworkByteOrderU32(), _group, _kind, num, 0L);
				setCurrentStateID(PartyAdvertiseStateID.Ready);
			}

			public void Leave_Requested()
			{
				_members.Clear();
			}

			public void Enter_Ready()
			{
				_tick.reset();
			}

			public void Execute_Ready()
			{
				if ((_goInfo._maxUsec - _goInfo._myUsec) / 2 <= _tick.getUsec())
				{
					setCurrentStateID(PartyAdvertiseStateID.Go);
				}
			}

			public void Execute_Go()
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

			private PartyAdvertiseStateID getCurrentStateID()
			{
				return getCurrentState();
			}

			private void setCurrentStateID(PartyAdvertiseStateID nextState)
			{
				setNextState(nextState);
				updateState();
			}
		}

		private static Manager s_pMgr = null;

		private static Parameter s_param = new Parameter();

		public static readonly ushort CPortNumber = 50102;

		public static void createManager(Parameter param)
		{
			s_param = new Parameter(param);
			Util.SafeDispose(ref s_pMgr);
			s_pMgr = new Manager();
		}

		public static void destroyManager()
		{
			Util.SafeDispose(ref s_pMgr);
		}

		public static bool isExist()
		{
			return s_pMgr != null;
		}

		public static IManager get()
		{
			return s_pMgr;
		}

		public static Parameter getParam()
		{
			return s_param;
		}
	}
}
