using System;
using DB;
using MAI2;
using PartyLink;

namespace Manager.Party.Party
{
	public class Client : StateMachine<Client, PartyPartyClientStateID>, IDisposable
	{
		private readonly PartyLink.Party.InitParam _initParam;

		private MechaInfo _mechaInfo;

		private ConnectSocket _tcpSocket;

		private UdpRecvSocket _udpSocket;

		private IpAddress _hostAddress;

		private readonly Recruit _recruit;

		private HeartBeat _heartBeat;

		private PartyPlayInfo _partyPlayInfo;

		private PartyMemberInfo _partyMemberInfo;

		private PartyMemberState _partyMemberState;

		private string _message;

		private DateTime _toReadyStartTime;

		private ClientPlayInfo _lastSendClientPlayInfo;

		private PartyTick _tick;

		private StartPlay _startPlayCommand;

		private bool _isJoin;

		private KickBy _lastKickReason;

		private bool _alreadyDisposed;

		private const int MockID = -1;

		public Client(string name, PartyLink.Party.InitParam initParam)
		{
			_initParam = initParam;
			_mechaInfo = new MechaInfo();
			_tcpSocket = new ConnectSocket(name + "_TCP", -1);
			_udpSocket = new UdpRecvSocket(name + "_UDP", -1);
			_recruit = new Recruit(name + "_Recruit");
			_heartBeat = new HeartBeat(_tcpSocket, PartyLink.Party.c_heartBeatInterval, PartyLink.Party.c_heartBeatTimeout);
			_partyPlayInfo = new PartyPlayInfo();
			_partyMemberInfo = new PartyMemberInfo();
			_partyMemberState = new PartyMemberState();
			_message = "";
			_toReadyStartTime = DateTime.MinValue;
			_lastSendClientPlayInfo = new ClientPlayInfo();
			_tick = new PartyTick();
			_startPlayCommand = new StartPlay();
			_hostAddress = IpAddress.Zero;
			_isJoin = false;
			_lastKickReason = KickBy.None;
			_tcpSocket.registCommand(Command.JoinResult, RecvJoinResult);
			_tcpSocket.registCommand(Command.RequestMeasure, RecvRequestMeasure);
			_tcpSocket.registCommand(Command.StartPlay, RecvStartPlay);
			_tcpSocket.registCommand(Command.PartyPlayInfo, RecvPartyPlayInfo);
			_tcpSocket.registCommand(Command.PartyMemberInfo, RecvPartyMemberInfo);
			_tcpSocket.registCommand(Command.PartyMemberState, RecvPartyMemberState);
			_tcpSocket.registCommand(Command.StartClientState, RecvStartClientState);
			_tcpSocket.registCommand(Command.Kick, RecvKick);
			_tcpSocket.registCommand(Command.HeartBeatRequest, RecvHeartBeatRequest);
			_tcpSocket.registCommand(Command.HeartBeatResponse, RecvHeartBeatResponse);
			_udpSocket.registCommand(Command.StartRecruit, RecvStartRecruit);
			_udpSocket.registCommand(Command.FinishRecruit, RecvFinishRecruit);
			SetCurrentStateID(PartyPartyClientStateID.First);
		}

		~Client()
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
					_mechaInfo = null;
					PartyLink.Util.SafeDispose(ref _tcpSocket);
					PartyLink.Util.SafeDispose(ref _udpSocket);
					_heartBeat = null;
					_partyPlayInfo = null;
					_partyMemberInfo = null;
					_partyMemberState = null;
					_lastSendClientPlayInfo = null;
					_tick = null;
					_startPlayCommand = null;
					GC.SuppressFinalize(this);
				}
				_alreadyDisposed = true;
			}
		}

		public void Update()
		{
			if (GetCurrentStateID().IsConnect() && _tcpSocket.isClose())
			{
				_message = "Disconnected";
				SetCurrentStateID(PartyPartyClientStateID.Disconnected);
			}
			updateState(-1f);
			_tcpSocket.update();
			_udpSocket.recv();
			_recruit.Update(DateTime.Now);
			_heartBeat.update();
		}

		public void Initialize()
		{
			SetCurrentStateID(PartyPartyClientStateID.First);
		}

		public void Terminate()
		{
			SetCurrentStateID(PartyPartyClientStateID.Finish);
		}

		public void Start()
		{
			SetCurrentStateID(PartyPartyClientStateID.Setup);
		}

		public void Wait()
		{
			SetCurrentStateID(PartyPartyClientStateID.Wait);
		}

		public void SelectMusic()
		{
			SetCurrentStateID(PartyPartyClientStateID.Wait);
		}

		public void RequestJoin(uint hostAddress, MechaInfo info)
		{
			_lastKickReason = KickBy.None;
			_hostAddress = new IpAddress(hostAddress);
			_mechaInfo = (MechaInfo)info.Clone();
			_mechaInfo.IpAddress = PartyLink.Util.MyIpAddress(-1).ToNetworkByteOrderU32();
			SetCurrentStateID(PartyPartyClientStateID.Connect);
		}

		public void CancelJoin()
		{
			IsJoin();
			SetCurrentStateID(PartyPartyClientStateID.Wait);
		}

		public void BeginPlay()
		{
			SetCurrentStateID(PartyPartyClientStateID.BeginPlay);
		}

		public void Ready()
		{
			SetCurrentStateID(PartyPartyClientStateID.Ready);
		}

		public void FinishPlay()
		{
			SetCurrentStateID(PartyPartyClientStateID.FinishPlay);
		}

		public void FinishNews(bool gaugeClear0, uint guageNumber0, bool gaugeClear1, uint guageNumber1)
		{
			FinishNews info = new FinishNews(PartyLink.Util.MyIpAddress(-1).ToNetworkByteOrderU32(), gaugeClear0, guageNumber0, gaugeClear1, guageNumber1);
			_tcpSocket.sendClass(info);
			SetCurrentStateID(PartyPartyClientStateID.NewsEnd);
		}

		public void SendClientPlayInfo(ClientPlayInfo info)
		{
			if (_tcpSocket.isActive())
			{
				IsPlay();
				_lastSendClientPlayInfo.CopyFrom(info);
				_tcpSocket.sendClass(info);
			}
		}

		public ClientPlayInfo GetLastSendClientPlayInfo()
		{
			return _lastSendClientPlayInfo;
		}

		public void SendMechaInfo(MechaInfo info)
		{
			if (_tcpSocket.isActive())
			{
				_mechaInfo = (MechaInfo)info.Clone();
				UpdateMechaInfo info2 = new UpdateMechaInfo(_mechaInfo);
				_tcpSocket.sendClass(info2);
			}
		}

		public bool IsNormal()
		{
			PartyPartyClientStateID currentStateID = GetCurrentStateID();
			if (currentStateID.IsValid())
			{
				return currentStateID.IsNormal();
			}
			return false;
		}

		public bool IsPlay()
		{
			return GetCurrentStateID() == PartyPartyClientStateID.Play;
		}

		public bool IsDisconnected()
		{
			return GetCurrentStateID() == PartyPartyClientStateID.Disconnected;
		}

		public KickBy GetLastKickReason()
		{
			return _lastKickReason;
		}

		public bool IsError()
		{
			return GetCurrentStateID() == PartyPartyClientStateID.Error;
		}

		public bool IsJoin()
		{
			return _isJoin;
		}

		public bool IsRequest()
		{
			PartyPartyClientStateID currentStateID = GetCurrentStateID();
			if (currentStateID.IsValid())
			{
				return currentStateID.IsRequest();
			}
			return false;
		}

		public bool IsConnect()
		{
			PartyPartyClientStateID currentStateID = GetCurrentStateID();
			if (currentStateID.IsValid())
			{
				return currentStateID.IsConnect();
			}
			return false;
		}

		public DateTime GetToReadyStartTime()
		{
			return _toReadyStartTime;
		}

		public RecruitList GetRecruitList()
		{
			return _recruit.GetList();
		}

		public RecruitList GetRecruitListWithoutMe()
		{
			return _recruit.GetListWithoutMe(-1);
		}

		public uint GetRecvCount(Command command)
		{
			if (!_tcpSocket.isRegisted(command))
			{
				return _udpSocket.getRecvCount(command);
			}
			return _tcpSocket.getRecvCount(command);
		}

		public PartyPlayInfo GetPartyPlayInfo()
		{
			return _partyPlayInfo;
		}

		public PartyMemberInfo GetPartyMemberInfo()
		{
			return _partyMemberInfo;
		}

		public PartyMemberState GetPartyMemberState()
		{
			return _partyMemberState;
		}

		public PartyPartyClientStateID GetPartyMemberMeState()
		{
			for (int i = 0; i < _partyMemberInfo.MechaInfo.Length; i++)
			{
				if (_partyMemberInfo.MechaInfo[i].IsMe(-1))
				{
					return (PartyPartyClientStateID)_partyMemberState.StateList[i];
				}
			}
			return PartyPartyClientStateID.Invalid;
		}

		public void Info(ref string os)
		{
			os = os + "ClientState " + base.stateString + "\n";
			os += $"tcpSocket {_tcpSocket}\n";
			os += $"udpSocket {_udpSocket}\n";
			os += $"hostAddress {_hostAddress}\n";
			os += $"mechaInfo {_mechaInfo}\n";
			os += $"heartBeat {_heartBeat}\n";
			os += $"partyPlayInfo {_partyPlayInfo}\n";
			os += $"{_partyMemberInfo}\n";
			os += $"{_partyMemberState}\n";
			os += $"toReadyStartTime {_toReadyStartTime}";
			os = os + "message " + _message + "\n";
			os += "recruit.info ";
			_recruit.Info(ref os);
			os += "\n";
		}

		public long GetPingUsec()
		{
			return _heartBeat.getPingUsec();
		}

		public long GetPingFrame()
		{
			return _heartBeat.getPingFrame();
		}

		private void RecvHeartBeatRequest(Packet packet)
		{
			HeartBeatRequest param = packet.getParam<HeartBeatRequest>();
			_heartBeat.recvRequest(param);
		}

		private void RecvHeartBeatResponse(Packet packet)
		{
			HeartBeatResponse param = packet.getParam<HeartBeatResponse>();
			_heartBeat.recvResponse(param);
		}

		private void RecvStartRecruit(Packet packet)
		{
			RecruitInfo recruitInfo = packet.getParam<StartRecruit>().RecruitInfo;
			IManager manager = Party.Get();
			if (manager.GetGroup() == (MachineGroupID)recruitInfo.GroupID && manager.GetEventMode() == recruitInfo.EventModeID)
			{
				recruitInfo.RecvTime = DateTime.Now;
				_recruit.Add(recruitInfo);
			}
		}

		private void RecvFinishRecruit(Packet packet)
		{
			RecruitInfo recruitInfo = new RecruitInfo(packet.getParam<FinishRecruit>().RecruitInfo);
			IManager manager = Party.Get();
			if (manager.GetGroup() == (MachineGroupID)recruitInfo.GroupID && manager.GetEventMode() == recruitInfo.EventModeID)
			{
				_recruit.Remove(recruitInfo);
			}
		}

		private void RecvJoinResult(Packet packet)
		{
			if (GetCurrentStateID() == PartyPartyClientStateID.Request)
			{
				if (packet.getParam<JoinResult>().IsSuccess())
				{
					SetCurrentStateID(PartyPartyClientStateID.Joined);
				}
				else
				{
					SetCurrentStateID(PartyPartyClientStateID.Wait);
				}
			}
		}

		private void RecvKick(Packet packet)
		{
			ExecKicked(packet.getParam<Kick>()?.KickBy ?? KickBy.None);
		}

		private void ExecKicked(KickBy reason)
		{
			SetCurrentStateID(PartyPartyClientStateID.Disconnected);
			_lastKickReason = reason;
		}

		private void RecvRequestMeasure(Packet packet)
		{
			_tcpSocket.sendClass(new ResponseMeasure());
		}

		private void RecvStartPlay(Packet packet)
		{
			_startPlayCommand = packet.getParam<StartPlay>();
			SetCurrentStateID(PartyPartyClientStateID.Sync);
		}

		private void RecvStartClientState(Packet packet)
		{
			if (!IsError() && IsJoin())
			{
				StartClientState param = packet.getParam<StartClientState>();
				bool flag = false;
				PartyPartyClientStateID state = param.GetState();
				if (state != PartyPartyClientStateID.ToReady || true)
				{
					SetCurrentStateID(param.GetState());
				}
			}
		}

		private void RecvPartyPlayInfo(Packet packet)
		{
			if (_partyPlayInfo != null)
			{
				PartyPlayInfo param = packet.getParam<PartyPlayInfo>();
				_partyPlayInfo.CopyFrom(param);
			}
		}

		private void RecvPartyMemberInfo(Packet packet)
		{
			if (_partyMemberInfo != null)
			{
				PartyMemberInfo param = packet.getParam<PartyMemberInfo>();
				_partyMemberInfo.CopyFrom(param);
			}
		}

		private void RecvPartyMemberState(Packet packet)
		{
			_partyMemberState = packet.getParam<PartyMemberState>();
		}

		private void Clear()
		{
			_udpSocket.close();
			_tcpSocket.close();
			_isJoin = false;
			_lastKickReason = KickBy.None;
		}

		private void Error(string message)
		{
			_message = "error" + message;
			SetCurrentStateID(PartyPartyClientStateID.Error);
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
			SetCurrentStateID(PartyPartyClientStateID.Wait);
		}

		public void Enter_Wait()
		{
			_tcpSocket.close();
			_partyMemberInfo.Clear();
			_partyPlayInfo.Clear();
			_udpSocket.close();
			_isJoin = false;
		}

		public void Execute_Wait()
		{
			if (!_udpSocket.isValid())
			{
				IpAddress ipAddress = new IpAddress(PartyLink.Util.MyIpAddress(-1));
				if (ipAddress != IpAddress.Zero)
				{
					_udpSocket.open(ipAddress, _initParam._portNumber);
				}
			}
		}

		public void Enter_Disconnected()
		{
			_tcpSocket.close();
			uint num = 0u;
			MemberPlayInfo[] member = _partyPlayInfo.Member;
			foreach (MemberPlayInfo memberPlayInfo in member)
			{
				if (memberPlayInfo.IpAddress == 0)
				{
					memberPlayInfo.IpAddress = _partyMemberInfo.MechaInfo[num].IpAddress;
				}
				memberPlayInfo.IsActive = false;
				num++;
			}
		}

		public void Execute_Disconnected()
		{
		}

		public void Enter_Connect()
		{
			_partyPlayInfo.Clear();
			_tcpSocket.close();
			_tcpSocket.connect(_hostAddress, Party.Get().GetPortNumber());
		}

		public void Execute_Connect()
		{
			if (_tcpSocket.isActive())
			{
				SetCurrentStateID(PartyPartyClientStateID.Request);
			}
			else if (60 < base.StateFrame)
			{
				_message = "Connect failed Time Out";
				SetCurrentStateID(PartyPartyClientStateID.Wait);
			}
		}

		public void Enter_Request()
		{
			IManager manager = Party.Get();
			RequestJoin info = new RequestJoin(manager.GetGroup(), manager.GetEventMode(), _mechaInfo);
			_tcpSocket.sendClass(info);
		}

		public void Execute_Request()
		{
		}

		public void Enter_Joined()
		{
			_isJoin = true;
		}

		public void Execute_Joined()
		{
		}

		public void Execute_FinishSetting()
		{
		}

		public void Enter_ToReady()
		{
			_toReadyStartTime = DateTime.Now;
		}

		public void Execute_ToReady()
		{
		}

		public void Enter_BeginPlay()
		{
		}

		public void Execute_BeginPlay()
		{
		}

		public void Enter_AllBeginPlay()
		{
		}

		public void Execute_AllBeginPlay()
		{
		}

		public void Enter_Ready()
		{
		}

		public void Execute_Ready()
		{
		}

		public void Enter_Sync()
		{
			_tick.reset();
		}

		public void Execute_Sync()
		{
			if ((_startPlayCommand.MaxMeasure - _startPlayCommand.MyMeasure) / 2 <= _tick.getUsec())
			{
				SetCurrentStateID(PartyPartyClientStateID.Play);
			}
		}

		public void Execute_Play()
		{
		}

		public void Execute_FinishPlay()
		{
		}

		public void Execute_News()
		{
		}

		public void Execute_NewsEnd()
		{
		}

		public void Execute_Result()
		{
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

		public PartyPartyClientStateID GetCurrentStateID()
		{
			return getCurrentState();
		}

		public void SetCurrentStateID(PartyPartyClientStateID id)
		{
			if (_tcpSocket.isActive())
			{
				_tcpSocket.sendClass(new ClientState(id));
			}
			setNextState(id);
			updateState(-1f);
		}
	}
}
