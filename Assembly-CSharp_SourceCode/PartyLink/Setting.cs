using System;
using System.Linq;
using DB;
using MAI2;
using MAI2.Memory;

namespace PartyLink
{
	public static class Setting
	{
		public class Client : StateMachine<Client, PartySettingClientStateID>, IDisposable
		{
			private IManager _manager;

			private ConnectSocket _tcpSocket;

			private UdpRecvSocket _udpSocket;

			private HeartBeat _heartBeat;

			private string _errorMessage;

			private DateTime _errorTime;

			private SettingHostAddress _hostAddress;

			private DateTime _hostAddressRecvTime;

			private Data _data;

			private DateTime _dataRecvTime;

			private bool _alreadyDisposed;

			private const int MockID = -1;

			private Client()
			{
			}

			private Client(Client other)
			{
			}

			public Client(IManager manager, int mockID)
			{
				_tcpSocket = new ConnectSocket("Setting::Client::TCP", -1);
				_udpSocket = new UdpRecvSocket("Setting::Client::UDP", -1);
				_manager = manager;
				_heartBeat = new HeartBeat(_tcpSocket, getParam()._heartBeatInterval, getParam()._heartBeatTimeout);
				_errorMessage = "";
				_errorTime = DateTime.MinValue;
				_hostAddress = new SettingHostAddress();
				_hostAddressRecvTime = DateTime.MinValue;
				_data = new Data();
				_dataRecvTime = DateTime.MinValue;
				clear();
				_tcpSocket.registCommand(Command.SettingResponse, recvSettingResponse);
				_tcpSocket.registCommand(Command.HeartBeatRequest, recvHeartBeatRequest);
				_tcpSocket.registCommand(Command.HeartBeatResponse, recvHeartBeatResponse);
				_udpSocket.registCommand(Command.SettingHostAddress, recvSettingHostAddress);
				setCurrentStateID(PartySettingClientStateID.First);
			}

			~Client()
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
						clear();
						_manager = null;
						Util.SafeDispose(ref _tcpSocket);
						Util.SafeDispose(ref _udpSocket);
						_heartBeat = null;
						_hostAddress = null;
						_data = null;
						GC.SuppressFinalize(this);
					}
					_alreadyDisposed = true;
				}
			}

			public void initialize()
			{
				setCurrentStateID(PartySettingClientStateID.First);
			}

			public void terminate()
			{
				setCurrentStateID(PartySettingClientStateID.Finish);
			}

			public void update()
			{
				updateState(-1f);
				_tcpSocket.update();
				_udpSocket.recv();
				_heartBeat.update();
			}

			public void start()
			{
				if (_manager.getGroup() == MachineGroupID.OFF)
				{
					error("Group OFF");
				}
				else
				{
					setCurrentStateID(PartySettingClientStateID.Search);
				}
			}

			public bool request()
			{
				if (getCurrentStateID() == PartySettingClientStateID.Idle)
				{
					setCurrentStateID(PartySettingClientStateID.Request);
					return true;
				}
				return false;
			}

			public void info(ref string os)
			{
				os = string.Concat(os, "state ", getCurrentStateID(), "\n");
				os = os + "isNormal " + isNormal() + " ";
				os = os + "isError " + isError() + "\n";
				os = string.Concat(os, "_data ", _data, "\n");
				os = string.Concat(os, _hostAddressRecvTime, " hostAddress ", _hostAddress, "\n");
				os = string.Concat(os, _dataRecvTime, " data ", _data, "\n");
				os = string.Concat(os, "udpSocket ", _udpSocket, "\n");
				os = string.Concat(os, "heartBeat ", _heartBeat, "\n");
				os = string.Concat(os, "tcpSocket ", _tcpSocket, "\n");
			}

			public bool isNormal()
			{
				return getCurrentStateID().IsNormal();
			}

			public bool isError()
			{
				return getCurrentStateID().IsError();
			}

			public bool isBusy()
			{
				return getCurrentStateID().IsBusy();
			}

			public DateTime getErrorTime()
			{
				return _errorTime;
			}

			public void setData(Data data)
			{
				_data = new Data(data);
			}

			public Data getData()
			{
				return _data;
			}

			public string getErrorMessage()
			{
				return _errorMessage;
			}

			public string getInitializeMessage()
			{
				return getCurrentStateID().GetName() + "\n";
			}

			public IpAddress getHostAddress()
			{
				return new IpAddress(_hostAddress._address);
			}

			public void forceError()
			{
				error("ForceError");
			}

			private void recvSettingResponse(Packet packet)
			{
				SettingResponse param = packet.getParam<SettingResponse>();
				if (_manager.getGroup() == (MachineGroupID)param._group && getCurrentStateID() == PartySettingClientStateID.Request)
				{
					_data = new Data(param.getData());
					_dataRecvTime = DateTime.Now;
				}
			}

			private void recvSettingHostAddress(Packet packet)
			{
				SettingHostAddress param = packet.getParam<SettingHostAddress>();
				if (getCurrentStateID() == PartySettingClientStateID.Search && _manager.getGroup() == (MachineGroupID)param._group)
				{
					_hostAddress = new SettingHostAddress(param);
					_hostAddressRecvTime = DateTime.Now;
				}
			}

			private void recvHeartBeatRequest(Packet packet)
			{
				HeartBeatRequest param = packet.getParam<HeartBeatRequest>();
				_heartBeat.recvRequest(param);
			}

			private void recvHeartBeatResponse(Packet packet)
			{
				HeartBeatResponse param = packet.getParam<HeartBeatResponse>();
				_heartBeat.recvResponse(param);
			}

			private void error(string message)
			{
				_errorMessage = message;
				setCurrentStateID(PartySettingClientStateID.Error);
			}

			private void clear()
			{
				_hostAddress.clear();
				_tcpSocket.close();
				_udpSocket.close();
			}

			public void Enter_First()
			{
				clear();
			}

			public void Execute_First()
			{
			}

			public void Enter_Search()
			{
				clear();
			}

			public void Execute_Search()
			{
				if (!_udpSocket.isValid())
				{
					IpAddress ipAddress = new IpAddress(Util.MyIpAddress(-1));
					if (ipAddress != IpAddress.Zero)
					{
						_udpSocket.open(ipAddress, getParam()._portNumber);
					}
				}
				if (!(getHostAddress() == IpAddress.Any))
				{
					setCurrentStateID(PartySettingClientStateID.Connect);
				}
			}

			public void Enter_Connect()
			{
				_tcpSocket.close();
				_tcpSocket.connect(getHostAddress(), getParam()._portNumber);
			}

			public void Execute_Connect()
			{
				if (_tcpSocket.isActive())
				{
					setCurrentStateID(PartySettingClientStateID.Request);
				}
				else if (_tcpSocket.isError())
				{
					error("Connect failed.");
				}
			}

			public void Enter_Request()
			{
				SettingRequest settingRequest = new SettingRequest(_manager.getGroup());
				_tcpSocket.sendClass(settingRequest);
				_data.clear();
			}

			public void Execute_Request()
			{
				if (_data.isValid())
				{
					setCurrentStateID(PartySettingClientStateID.Idle);
				}
				else if (_tcpSocket.isClose())
				{
					error("Disconnected");
				}
			}

			public void Execute_Idle()
			{
				if (_tcpSocket.isClose())
				{
					error("Disconnected");
				}
			}

			public void Enter_Error()
			{
				_tcpSocket.close();
				_errorTime = DateTime.Now;
			}

			public void Execute_Error()
			{
			}

			public void Enter_Finish()
			{
				_tcpSocket.close();
				_udpSocket.close();
			}

			public void Execute_Finish()
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

			private PartySettingClientStateID getCurrentStateID()
			{
				return getCurrentState();
			}

			private void setCurrentStateID(PartySettingClientStateID nextState)
			{
				setNextState(nextState);
				updateState(-1f);
			}
		}

		[Serializable]
		public class SettingHostAddress : ICommandParam
		{
			public uint _address;

			public int _group;

			public Command getCommand()
			{
				return Command.SettingHostAddress;
			}

			public int serialize(int pos, Chunk chunk)
			{
				pos = chunk.writeU32(pos, _address);
				pos = chunk.writeS32(pos, _group);
				return pos;
			}

			public SettingHostAddress deserialize(ref int pos, Chunk chunk)
			{
				_address = chunk.readU32(ref pos);
				_group = chunk.readS32(ref pos);
				return this;
			}

			public SettingHostAddress()
			{
				clear();
			}

			public SettingHostAddress(MachineGroupID group, IpAddress address)
			{
				_address = address.ToNetworkByteOrderU32();
				_group = (int)group;
			}

			public SettingHostAddress(SettingHostAddress arg)
			{
				_address = arg._address;
				_group = arg._group;
			}

			public void clear()
			{
				_group = 1;
				_address = IpAddress.Any.ToNetworkByteOrderU32();
			}

			public override string ToString()
			{
				return string.Concat("( command ", getCommand(), " address ", _address.convIpString(), " group ", (MachineGroupID)_group, ")");
			}
		}

		[Serializable]
		public class SettingRequest : ICommandParam
		{
			public int _group;

			public Command getCommand()
			{
				return Command.SettingRequest;
			}

			public SettingRequest()
			{
				_group = 1;
			}

			public SettingRequest(MachineGroupID group)
			{
				_group = (int)group;
			}

			public override string ToString()
			{
				return string.Concat(getCommand(), " ", _group);
			}
		}

		[Serializable]
		public class SettingResponse : ICommandParam
		{
			public Data _data;

			public int _group;

			public Command getCommand()
			{
				return Command.SettingResponse;
			}

			public SettingResponse()
			{
				_group = 1;
				_data = new Data();
			}

			public SettingResponse(MachineGroupID group, Data data)
			{
				_group = (int)group;
				_data = new Data(data);
			}

			public Data getData()
			{
				return _data;
			}

			public override string ToString()
			{
				return string.Concat(getCommand(), " group ", _group, " data ", _data);
			}
		}

		[Serializable]
		public class Data
		{
			public bool _valid;

			public int _dummy;

			public bool _isEventMode;

			public int _eventModeMusicCount;

			public int _memberNumber;

			public bool isEventModeSettingAvailable => _isEventMode;

			public int eventModeMusicCount => _eventModeMusicCount;

			public Data()
			{
				clear();
			}

			public Data(Data arg)
				: this()
			{
				_valid = arg._valid;
				_dummy = arg._dummy;
				_isEventMode = arg._isEventMode;
				_eventModeMusicCount = arg._eventModeMusicCount;
				_memberNumber = arg._memberNumber;
			}

			public void clear()
			{
				_valid = false;
				_dummy = 0;
				_isEventMode = false;
				_eventModeMusicCount = 0;
				_memberNumber = 0;
			}

			public void set(bool isEventMode, int eventTrackNum)
			{
				_valid = true;
				_isEventMode = isEventMode;
				_eventModeMusicCount = eventTrackNum;
			}

			public bool isValid()
			{
				return _valid;
			}

			public override string ToString()
			{
				return "( _valid " + _valid.ToString() + " _dummy " + _dummy + " _isEventMode " + _isEventMode.ToString() + " _eventModeMusicCount " + _eventModeMusicCount + " _memberNumber " + _memberNumber + ")";
			}
		}

		public class Host : StateMachine<Host, PartySettingHostStateID>, IDisposable
		{
			private DateTime _sendAddressTime;

			private ListenSocket _listenSocket;

			private BroadcastSocket _broadcastSocket;

			private UdpRecvSocket _udpSocket;

			private MemberList _members;

			private readonly IManager _manager;

			private Data _data;

			private int _eraseCount;

			private int _acceptCount;

			private int _differentAddressCount;

			private int _sameAddressCount;

			private DateTime _errorTime;

			private string _errorMessage;

			private bool _alreadyDisposed;

			private const int MockID = -1;

			private Host()
			{
			}

			private Host(Host other)
			{
			}

			public Host(IManager manager, int mockID)
			{
				_sendAddressTime = DateTime.MinValue;
				_listenSocket = new ListenSocket("Setting::Host::Listen", -1);
				_broadcastSocket = new BroadcastSocket("Setting::Host::BroadCast", -1);
				_udpSocket = new UdpRecvSocket("Setting::Host::UDP", -1);
				_members = new MemberList();
				_manager = manager;
				_data = new Data();
				_eraseCount = 0;
				_acceptCount = 0;
				_differentAddressCount = 0;
				_sameAddressCount = 0;
				clear();
				_udpSocket.registCommand(Command.SettingHostAddress, recvSettingHostAddress);
				setCurrentStateID(PartySettingHostStateID.First);
			}

			~Host()
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
						Util.SafeDispose(ref _listenSocket);
						Util.SafeDispose(ref _broadcastSocket);
						Util.SafeDispose(ref _udpSocket);
						Util.SafeDispose(_members);
						_members = null;
						_data = null;
						GC.SuppressFinalize(this);
					}
					_alreadyDisposed = true;
				}
			}

			public void initialize()
			{
				setCurrentStateID(PartySettingHostStateID.First);
			}

			public void terminate()
			{
				setCurrentStateID(PartySettingHostStateID.Finish);
			}

			public void start()
			{
				if (_manager.getGroup() == MachineGroupID.OFF)
				{
					error(PartySettingErrorID.GroupOff, "Group Off");
				}
				else
				{
					setCurrentStateID(PartySettingHostStateID.Setup);
				}
			}

			public void update()
			{
				updateState(-1f);
				executeBroadcast();
				executeAccept();
				_udpSocket.recv();
				foreach (Member member in _members)
				{
					member.update();
				}
				int count = _members.Count;
				_members.erase_if((Member a) => !a.isAlive());
				_eraseCount += count - _members.Count;
			}

			public int getMemberNumber()
			{
				return _members.Count((Member a) => a.isJoin()) + 1;
			}

			public void info(ref string os)
			{
				os = os + "state " + base.stateString + "\n";
				os = string.Concat(os, "listenSocket ", _listenSocket, "\n");
				os = string.Concat(os, "broadcastSocket", _broadcastSocket, "\n");
				os = string.Concat(os, "sendAddressTime ", _sendAddressTime, "\n");
				os = os + "acceptCount " + _acceptCount + "\n";
				os = os + "eraseCount " + _eraseCount + "\n";
				os = os + "sameAddressCount " + _sameAddressCount + "\n";
				os = os + "differentAddressCount " + _differentAddressCount + "\n";
				os = os + "getMemberNumber() " + getMemberNumber() + "\n";
				os = os + "members.size() " + _members.Count + "\n";
				os = string.Concat(os, "listenSocket ", _listenSocket, "\n");
				os = string.Concat(os, "broadcastSocket ", _broadcastSocket, "\n");
				os = string.Concat(os, "udpSocket ", _udpSocket, "\n");
				int num = 0;
				foreach (Member member in _members)
				{
					os = os + num + " ";
					member.info(ref os);
				}
			}

			public bool isNormal()
			{
				return getCurrentStateID().IsNormal();
			}

			public bool isError()
			{
				return getCurrentStateID().IsError();
			}

			public bool isDuplicate()
			{
				return 0 < _differentAddressCount;
			}

			public MachineGroupID getGroup()
			{
				return _manager.getGroup();
			}

			public void setData(Data data)
			{
				_data = new Data(data);
			}

			public Data getData()
			{
				return _data;
			}

			public string getErrorMessage()
			{
				return _errorMessage;
			}

			public string getInitializeMessage()
			{
				return getCurrentStateID().GetName() + "\n";
			}

			public void forceError()
			{
				error(PartySettingErrorID.ForceError, "ForceError");
			}

			public DateTime getErrorTime()
			{
				return _errorTime;
			}

			private void clear()
			{
				_differentAddressCount = 0;
				_sameAddressCount = 0;
				_eraseCount = 0;
				_acceptCount = 0;
				_data.clear();
				_members.ClearWithDispose();
				_listenSocket.close();
				_udpSocket.close();
			}

			private void executeBroadcast()
			{
				DateTime now = DateTime.Now;
				DateTime dateTime = _sendAddressTime.AddSeconds(getParam()._hostInfoSendInterval);
				if (!(now < dateTime))
				{
					if (!_broadcastSocket.isValid())
					{
						IpAddress addrTo = new IpAddress(Util.BroadcastAddress());
						_broadcastSocket.open(addrTo, getParam()._portNumber);
					}
					SettingHostAddress settingHostAddress = new SettingHostAddress(_manager.getGroup(), new IpAddress(Util.MyIpAddress(-1)));
					_broadcastSocket.sendClass(settingHostAddress);
					_sendAddressTime = DateTime.Now;
				}
			}

			private void executeAccept()
			{
				IpAddress outAddress;
				NFSocket nFSocket = _listenSocket.acceptClient(out outAddress);
				if (nFSocket != null)
				{
					_acceptCount++;
					string name = "member" + _members.Count;
					_members.Add(new Member(name, this, nFSocket));
				}
			}

			private void error(PartySettingErrorID id, string message)
			{
				_errorMessage = message;
				setCurrentStateID(PartySettingHostStateID.Error);
			}

			private void recvSettingHostAddress(Packet packet)
			{
				SettingHostAddress param = packet.getParam<SettingHostAddress>();
				if (_manager.getGroup() == (MachineGroupID)param._group)
				{
					if (!Util.isMyIP(param._address, -1))
					{
						_differentAddressCount++;
					}
					else
					{
						_sameAddressCount++;
					}
				}
			}

			public void Execute_First()
			{
			}

			public void Enter_Setup()
			{
				clear();
			}

			public void Execute_Setup()
			{
				if (!_listenSocket.open(getParam()._portNumber))
				{
					error(PartySettingErrorID.SocketOpenFailed, "_listenSocket.open faild.");
				}
				else
				{
					setCurrentStateID(PartySettingHostStateID.Active);
				}
			}

			public void Execute_Active()
			{
				if (!_udpSocket.isValid())
				{
					IpAddress ipAddress = new IpAddress(Util.MyIpAddress(-1));
					if (ipAddress != IpAddress.Zero)
					{
						_udpSocket.open(ipAddress, getParam()._portNumber);
					}
				}
			}

			public void Enter_Error()
			{
				clear();
				_errorTime = DateTime.Now;
			}

			public void Execute_Error()
			{
			}

			public void Enter_Finish()
			{
				clear();
			}

			public void Execute_Finish()
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

			private PartySettingHostStateID getCurrentStateID()
			{
				return getCurrentState();
			}

			private void setCurrentStateID(PartySettingHostStateID nextState)
			{
				setNextState(nextState);
				updateState(-1f);
			}
		}

		public class Parameter
		{
			public ushort _portNumber;

			public int _hostInfoSendInterval;

			public int _revivalInterval;

			public int _heartBeatInterval;

			public int _heartBeatTimeout;

			public Parameter()
			{
				clear();
			}

			public Parameter(Parameter arg)
			{
				_portNumber = arg._portNumber;
				_revivalInterval = arg._revivalInterval;
				_hostInfoSendInterval = arg._hostInfoSendInterval;
				_heartBeatInterval = arg._heartBeatInterval;
				_heartBeatTimeout = arg._heartBeatTimeout;
			}

			public void clear()
			{
				_portNumber = CPortNumber;
				_revivalInterval = 5;
				_hostInfoSendInterval = 3;
				_heartBeatInterval = 3;
				_heartBeatTimeout = _heartBeatInterval * 2;
			}

			public void info(ref string os)
			{
				if (os != null)
				{
					os += "(";
					os = os + " _portNumber " + _portNumber;
					os = os + " _hostInfoSendInterval " + _hostInfoSendInterval;
					os = os + " _revivalInterval " + _revivalInterval;
					os = os + " _heartBeatInterval " + _heartBeatInterval;
					os = os + " _heartBeatTimeout " + _heartBeatTimeout;
					os += ")\n";
				}
			}
		}

		public interface IManager
		{
			bool isStandardSettingMachine { get; }

			void execute();

			void initialize();

			void start(bool isStandardSettingMachine_arg, MachineGroupID id);

			void retry();

			void terminate();

			void setData(Data data);

			bool requestData();

			MachineGroupID getGroup();

			bool isNormal();

			bool isBusy();

			bool isError();

			bool isDuplicate();

			bool isDataValid();

			Data getData();

			int getMemberNumber();

			void info(ref string os);

			string getInitializeMessage();

			string getErrorMessage();

			bool isRetryEnable();

			void setRetryEnable(bool isRetry);
		}

		private class Manager : IManager, IDisposable
		{
			private MachineGroupID _group;

			private bool _isRetryEnable;

			private Host _host;

			private Client _client;

			private bool _alreadyDisposed;

			private const int MockID = -1;

			public bool isStandardSettingMachine { get; private set; }

			public Manager()
				: this(0)
			{
			}

			public Manager(int mockID)
			{
				_group = MachineGroupID.OFF;
				_host = new Host(this, -1);
				_client = new Client(this, -1);
				_isRetryEnable = true;
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
						Util.SafeDispose(ref _host);
						Util.SafeDispose(ref _client);
						GC.SuppressFinalize(this);
					}
					_alreadyDisposed = true;
				}
			}

			public void initialize()
			{
				_host.initialize();
				_client.initialize();
			}

			public void terminate()
			{
				_host.terminate();
				_client.terminate();
			}

			public void start(bool isStandardSettingMachine_arg, MachineGroupID group)
			{
				terminate();
				isStandardSettingMachine = isStandardSettingMachine_arg;
				_group = group;
				retry();
			}

			public void retry()
			{
				if (isHost())
				{
					_host.start();
				}
				else
				{
					_client.start();
				}
			}

			public void execute()
			{
				if (isError() && isRetryEnable())
				{
					DateTime dateTime = getErrorTime().AddSeconds(getParam()._revivalInterval);
					DateTime now = DateTime.Now;
					if (dateTime <= now)
					{
						retry();
					}
				}
				if (isHost())
				{
					_host.update();
				}
				else
				{
					_client.update();
				}
			}

			public void info(ref string os)
			{
				DateTime now = DateTime.Now;
				os = string.Concat(os, "Now ", now, "\n");
				os = string.Concat(os, "Group ", getGroup(), " ");
				os = os + "Role " + (isStandardSettingMachine ? "Parent" : "Child") + "\n";
				os = os + "isNormal " + isNormal() + " ";
				os = os + "isError " + isError() + " ";
				os = os + "isDataValid " + isDataValid() + "\n";
				os += "Param ";
				getParam().info(ref os);
				os = string.Concat(os, "getData ", getData(), "\n");
				os = os + "getMemberNumber " + getMemberNumber() + "\n";
				os = os + "isRetryEnable " + isRetryEnable() + " ";
				os = string.Concat(os, "errorTime ", getErrorTime(), "\n");
				os = os + "getInitializeMessage " + getInitializeMessage() + "\n";
				os = os + "getErrorMessage " + getErrorMessage() + "\n";
				os += "\n";
				if (isHost())
				{
					os += "Host\n";
					_host.info(ref os);
				}
				else
				{
					os += "Client\n";
					_client.info(ref os);
				}
			}

			public bool isNormal()
			{
				if (isHost())
				{
					return _host.isNormal();
				}
				return _client.isNormal();
			}

			public DateTime getErrorTime()
			{
				if (isHost())
				{
					return _host.getErrorTime();
				}
				return _client.getErrorTime();
			}

			public bool isDuplicate()
			{
				if (isHost())
				{
					return _host.isDuplicate();
				}
				return false;
			}

			public virtual bool isError()
			{
				if (isHost())
				{
					return _host.isError();
				}
				return _client.isError();
			}

			public virtual int getMemberNumber()
			{
				return _host.getMemberNumber();
			}

			public virtual bool isDataValid()
			{
				if (!isNormal())
				{
					return false;
				}
				return getData().isValid();
			}

			public void setData(Data data)
			{
				if (isHost())
				{
					_host.setData(data);
				}
				else
				{
					_client.setData(data);
				}
			}

			public Data getData()
			{
				if (isHost())
				{
					return _host.getData();
				}
				return _client.getData();
			}

			public MachineGroupID getGroup()
			{
				return _group;
			}

			public bool requestData()
			{
				if (isHost())
				{
					return false;
				}
				return _client.request();
			}

			public bool isHost()
			{
				return isStandardSettingMachine;
			}

			public string getInitializeMessage()
			{
				string text = "";
				if (isHost())
				{
					return text + _host.getInitializeMessage();
				}
				return text + _client.getInitializeMessage();
			}

			public string getErrorMessage()
			{
				if (isHost())
				{
					return _host.getErrorMessage();
				}
				return _client.getErrorMessage();
			}

			public bool isRetryEnable()
			{
				return _isRetryEnable;
			}

			public void setRetryEnable(bool isRetryEnable)
			{
				_isRetryEnable = isRetryEnable;
			}

			public bool isBusy()
			{
				if (isHost())
				{
					return false;
				}
				return _client.isBusy();
			}

			public void forceError()
			{
				_host.forceError();
				_client.forceError();
			}
		}

		public class MemberList : MemberList_Base<Member>
		{
		}

		public class Member : IDisposable
		{
			private ConnectSocket _socket;

			private bool _join;

			private readonly Host _host;

			private HeartBeat _heartBeat;

			private int _recvRequestCount;

			private int _ignoreRequestCount;

			private bool _alreadyDisposed;

			public Member(string name, Host host, NFSocket socket)
			{
				_host = host;
				_join = false;
				_socket = new ConnectSocket(name + "_Socket", socket);
				_heartBeat = new HeartBeat(_socket, getParam()._heartBeatInterval, getParam()._heartBeatTimeout);
				_recvRequestCount = 0;
				_ignoreRequestCount = 0;
				_socket.registCommand(Command.SettingRequest, recvSettingRequest);
				_socket.registCommand(Command.HeartBeatRequest, recvHeartBeatRequest);
				_socket.registCommand(Command.HeartBeatResponse, recvHeartBeatResponse);
				_socket.active();
			}

			~Member()
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
						Util.SafeDispose(ref _socket);
						_heartBeat = null;
						GC.SuppressFinalize(this);
					}
					_alreadyDisposed = true;
				}
			}

			public void update()
			{
				_socket.update();
				_heartBeat.update();
			}

			public bool isAlive()
			{
				return _socket.isActive();
			}

			public bool isJoin()
			{
				return _join;
			}

			public IpAddress getDestinationAddress()
			{
				return _socket.getDestinationAddress();
			}

			public void info(ref string os)
			{
				os = string.Concat(os, " active ", _socket.isActive() ? "true " : "false ", " recvEnd ", _socket.isRecvEnd() ? "true" : "false ", " heartBeat ", _heartBeat, " recvRequestCount ", _recvRequestCount, " ignoreRequestCount ", _ignoreRequestCount, "\n");
			}

			private void recvSettingRequest(Packet packet)
			{
				_recvRequestCount++;
				SettingRequest param = packet.getParam<SettingRequest>();
				SettingResponse settingResponse = new SettingResponse(_host.getGroup(), _host.getData());
				_socket.sendClass(settingResponse);
				if (_host.getGroup() == (MachineGroupID)param._group)
				{
					_join = true;
					return;
				}
				_ignoreRequestCount++;
				_socket.shutdown();
			}

			private void recvHeartBeatRequest(Packet packet)
			{
				HeartBeatRequest param = packet.getParam<HeartBeatRequest>();
				_heartBeat.recvRequest(param);
			}

			private void recvHeartBeatResponse(Packet packet)
			{
				HeartBeatResponse param = packet.getParam<HeartBeatResponse>();
				_heartBeat.recvResponse(param);
			}
		}

		public static readonly ushort CPortNumber = 50101;

		private static Manager s_pMgr = null;

		private static Parameter s_param = new Parameter();

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
