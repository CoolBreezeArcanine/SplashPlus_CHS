using System;
using System.Collections.Generic;
using System.Linq;
using DB;
using PartyLink;

namespace Manager.Party.Party
{
	public class Member : IMember, IDisposable
	{
		private readonly Host _host;

		private bool _isJoin;

		private PartyPartyClientStateID _state;

		private bool _stateChanged;

		private MechaInfo _mechaInfo;

		private bool _userInfoChanged;

		private HeartBeat _heartBeat;

		private ConnectSocket _socket;

		private Queue<ClientPlayInfo> _clientPlayInfo;

		private ClientPlayInfo _clientPlayInfoLast;

		private int _clientPlayInfoRecv;

		private long _measureSendUsec;

		private long _measureResponsedUsec;

		private long _measureDiffUsec;

		private bool _isResponsedMeasure;

		private PartyTick _measureTick;

		private FinishNews _finishNews;

		private bool _alreadyDisposed;

		private ParamPool<ClientPlayInfo> _clientPlayInfoPool;

		public Member(string name, Host host, NFSocket socket)
		{
			_host = host;
			_socket = new ConnectSocket(name + "_Socket", socket);
			_mechaInfo = new MechaInfo();
			_userInfoChanged = false;
			_isJoin = false;
			_heartBeat = new HeartBeat(_socket, PartyLink.Party.c_heartBeatInterval, PartyLink.Party.c_heartBeatTimeout);
			_state = PartyPartyClientStateID.First;
			_clientPlayInfo = new Queue<ClientPlayInfo>();
			_clientPlayInfoLast = new ClientPlayInfo();
			_clientPlayInfoRecv = 0;
			_stateChanged = false;
			_measureSendUsec = 0L;
			_measureResponsedUsec = 0L;
			_measureDiffUsec = 0L;
			_isResponsedMeasure = false;
			_measureTick = new PartyTick();
			_finishNews = new FinishNews();
			_clientPlayInfoPool = new ParamPool<ClientPlayInfo>(32);
			_socket.registCommand(Command.RequestJoin, RecvRequestJoin);
			_socket.registCommand(Command.ClientState, RecvClientState);
			_socket.registCommand(Command.HeartBeatRequest, RecvHeartBeatRequest);
			_socket.registCommand(Command.HeartBeatResponse, RecvHeartBeatResponse);
			_socket.registCommand(Command.ClientPlayInfo, RecvClientPlayInfo);
			_socket.registCommand(Command.UpdateMechaInfo, RecvMechaInfo);
			_socket.registCommand(Command.ResponseMeasure, RecvResponseMeasure);
			_socket.registCommand(Command.FinishNews, RecvFinishNews);
			_socket.active();
		}

		~Member()
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
					PartyLink.Util.SafeDispose(ref _socket);
					_mechaInfo = null;
					_clientPlayInfo = null;
					_clientPlayInfoLast = null;
					_heartBeat = null;
					_measureTick = null;
					_finishNews = null;
					_clientPlayInfoPool = null;
					GC.SuppressFinalize(this);
				}
				_alreadyDisposed = true;
			}
		}

		public void Update()
		{
			_socket.update();
			_heartBeat.update();
			if (GetCurrentStateID() != PartyPartyClientStateID.Disconnected && !IsActive())
			{
				SetCurrentStateID(PartyPartyClientStateID.Disconnected);
			}
		}

		public void Join(MechaInfo info)
		{
			_mechaInfo = info;
			_isJoin = true;
		}

		public void KickByCancel()
		{
			_isJoin = false;
			_socket.sendClass(new Kick(KickBy.Cancel));
		}

		public void KickByStart()
		{
			_isJoin = false;
			_socket.sendClass(new Kick(KickBy.Start));
		}

		public void KickByClientDisconnect()
		{
			_isJoin = false;
			_socket.sendClass(new Kick(KickBy.Disconnect));
		}

		public void StartPlay(long maxMeasure, long myMeasure)
		{
			_socket.sendClass(new StartPlay(maxMeasure, myMeasure));
		}

		public void StartState(PartyPartyClientStateID state)
		{
			_socket.sendClass(new StartClientState(state));
		}

		public void SendPartyMemberInfo(PartyMemberInfo info)
		{
			_socket.sendClass(info);
		}

		public void SendPartyMemberState(PartyMemberState info)
		{
			_socket.sendClass(info);
		}

		public void RequestMeasure()
		{
			_measureTick.reset();
			_measureSendUsec = _measureTick.getUsec();
			_measureResponsedUsec = 0L;
			_measureDiffUsec = 0L;
			_isResponsedMeasure = false;
			_socket.sendClass(new RequestMeasure());
		}

		public bool IsResponsedMeasure()
		{
			return _isResponsedMeasure;
		}

		public long GetMeasureDiffUsec()
		{
			return _measureDiffUsec;
		}

		public void NotifyPartyPlayInfo(PartyPlayInfo i)
		{
			_socket.sendClass(i);
		}

		public ConnectSocket GetSocket()
		{
			return _socket;
		}

		public bool IsJoin()
		{
			return _isJoin;
		}

		public bool IsFinishSetting()
		{
			if (IsJoin())
			{
				return GetCurrentStateID() == PartyPartyClientStateID.FinishSetting;
			}
			return false;
		}

		public bool IsActive()
		{
			if (_socket != null)
			{
				return _socket.isActive();
			}
			return false;
		}

		public bool IsFinishPlay()
		{
			return GetCurrentStateID() == PartyPartyClientStateID.FinishPlay;
		}

		public PartyPartyClientStateID GetCurrentStateID()
		{
			return _state;
		}

		public uint GetRecvCount(Command command)
		{
			return _socket.getRecvCount(command);
		}

		public MechaInfo GetMechaInfo()
		{
			return _mechaInfo;
		}

		public IpAddress GetAddress()
		{
			return _socket.getDestinationAddress();
		}

		public void InfoBase(ref string os)
		{
			os += (IsActive() ? " Connect    " : " Disconnect ");
			os += $"J {_isJoin} ";
			os += (_state.IsValid() ? (_state.GetName() + " ") : " ");
			os += $"So {_socket} ";
			os += $"PngF {GetPingFrame()} ";
			os += $"PngU {GetPingUsec()}\n";
		}

		public void InfoUser(ref string os)
		{
			os += $"{_mechaInfo} {_socket.getDestinationAddress()}\n";
		}

		public void InfoPlay(ref string os)
		{
			os += $"recv {_clientPlayInfoRecv} ";
			os += $"size {_clientPlayInfo.Count} ";
			os += $"Info({_clientPlayInfoLast}) ";
			os += _socket.getDestinationAddress();
		}

		public void InfoMeasure(ref string os)
		{
			os += " Measure ";
			os += $"isResponsed {_isResponsedMeasure} ";
			os += $"SendUsec {_measureSendUsec} ";
			os += $"ResponsedUsec {_measureResponsedUsec} ";
			os += $"DiffUsec {_measureDiffUsec}";
		}

		public Queue<ClientPlayInfo> GetClientPlayInfo()
		{
			return _clientPlayInfo;
		}

		public ClientPlayInfo GetLastClientPlayInfo()
		{
			return _clientPlayInfoLast;
		}

		public FinishNews GetFinishNews()
		{
			return _finishNews;
		}

		public void PopClientPlayInfo(int current)
		{
			Queue<ClientPlayInfo> clientPlayInfo = _clientPlayInfo;
			if (clientPlayInfo.Any() && clientPlayInfo.Peek().Count == current)
			{
				ClientPlayInfo t = clientPlayInfo.Dequeue();
				_clientPlayInfoPool.Return(t);
			}
		}

		public void UpdateLastClientPlayInfo(int current)
		{
			Queue<ClientPlayInfo> clientPlayInfo = _clientPlayInfo;
			if (clientPlayInfo.Any() && clientPlayInfo.Peek().Count == current)
			{
				_clientPlayInfoLast = new ClientPlayInfo(_clientPlayInfo.Peek());
			}
		}

		public void SetCurrentStateID(PartyPartyClientStateID state)
		{
			_state = state;
			_stateChanged = true;
		}

		public void SetStateChanged(bool stateChanged)
		{
			_stateChanged = stateChanged;
		}

		public bool IsStateChanged()
		{
			return _stateChanged;
		}

		public void SetUserInfoChanged(bool userInfoChanged)
		{
			_userInfoChanged = userInfoChanged;
		}

		public bool IsUserInfoChanged()
		{
			return _userInfoChanged;
		}

		public long GetPingFrame()
		{
			return _heartBeat.getPingFrame();
		}

		public long GetPingUsec()
		{
			return _heartBeat.getPingUsec();
		}

		private void RecvRequestJoin(Packet packet)
		{
			RequestJoin param = packet.getParam<RequestJoin>();
			JoinResult info = new JoinResult(_host.RequestJoin(this, param));
			_socket.sendClass(info);
		}

		private void RecvClientState(Packet packet)
		{
			ClientState param = packet.getParam<ClientState>();
			SetCurrentStateID(param.GetState());
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

		private void RecvClientPlayInfo(Packet packet)
		{
			ClientPlayInfo param = packet.getParam<ClientPlayInfo>();
			ClientPlayInfo clientPlayInfo = _clientPlayInfoPool.Get();
			if (clientPlayInfo != null)
			{
				clientPlayInfo.CopyFrom(param);
				_clientPlayInfo.Enqueue(clientPlayInfo);
			}
			_clientPlayInfoRecv++;
		}

		private void RecvMechaInfo(Packet packet)
		{
			UpdateMechaInfo param = packet.getParam<UpdateMechaInfo>();
			_mechaInfo = new MechaInfo(param.MechaInfo);
			_userInfoChanged = true;
		}

		private void RecvFinishNews(Packet packet)
		{
			_finishNews = packet.getParam<FinishNews>();
		}

		private void RecvResponseMeasure(Packet packet)
		{
			if (_measureTick != null)
			{
				_measureResponsedUsec = _measureTick.getUsec();
				_measureDiffUsec = _measureResponsedUsec - _measureSendUsec;
				_measureTick.reset();
			}
			else
			{
				_measureDiffUsec = 0L;
			}
			_isResponsedMeasure = true;
		}
	}
}
