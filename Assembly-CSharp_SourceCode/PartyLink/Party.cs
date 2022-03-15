using System;
using System.Collections.Generic;
using System.Linq;
using DB;
using MAI2;
using MAI2.Memory;
using MAI2.Util;
using Manager;
using UnityEngine;

namespace PartyLink
{
	public static class Party
	{
		public class Client : StateMachine<Client, PartyPartyClientStateID>, IDisposable
		{
			private readonly InitParam _initParam;

			private UserInfo _userInfo;

			private ConnectSocket _tcpSocket;

			private UdpRecvSocket _udpSocket;

			private IpAddress _hostAddress;

			private Recruit _recruit;

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

			private Client()
			{
			}

			private Client(Client other)
			{
			}

			public Client(string name, InitParam initParam, int mockID)
			{
				_initParam = initParam;
				_userInfo = new UserInfo();
				_tcpSocket = new ConnectSocket(name + "_TCP", -1);
				_udpSocket = new UdpRecvSocket(name + "_UDP", -1);
				_recruit = new Recruit(name + "_Recruit");
				_heartBeat = new HeartBeat(_tcpSocket, c_heartBeatInterval, c_heartBeatTimeout);
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
				_tcpSocket.registCommand(Command.JoinResult, recvJoinResult);
				_tcpSocket.registCommand(Command.RequestMeasure, recvRequestMeasure);
				_tcpSocket.registCommand(Command.StartPlay, recvStartPlay);
				_tcpSocket.registCommand(Command.PartyPlayInfo, recvPartyPlayInfo);
				_tcpSocket.registCommand(Command.PartyMemberInfo, recvPartyMemberInfo);
				_tcpSocket.registCommand(Command.PartyMemberState, recvPartyMemberState);
				_tcpSocket.registCommand(Command.StartClientState, recvStartClientState);
				_tcpSocket.registCommand(Command.Kick, recvKick);
				_tcpSocket.registCommand(Command.HeartBeatRequest, recvHeartBeatRequest);
				_tcpSocket.registCommand(Command.HeartBeatResponse, recvHeartBeatResponse);
				_udpSocket.registCommand(Command.StartRecruit, recvStartRecruit);
				_udpSocket.registCommand(Command.FinishRecruit, recvFinishRecruit);
				setCurrentStateID(PartyPartyClientStateID.First);
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
						_userInfo = null;
						Util.SafeDispose(ref _tcpSocket);
						Util.SafeDispose(ref _udpSocket);
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

			public void update()
			{
				if (getCurrentStateID().IsConnect() && _tcpSocket.isClose())
				{
					_message = "Disconnected";
					setCurrentStateID(PartyPartyClientStateID.Disconnected);
				}
				updateState(-1f);
				_tcpSocket.update();
				_udpSocket.recv();
				_recruit.update(DateTime.Now);
				_heartBeat.update();
			}

			public void initialize()
			{
				setCurrentStateID(PartyPartyClientStateID.First);
			}

			public void terminate()
			{
				setCurrentStateID(PartyPartyClientStateID.Finish);
			}

			public void start()
			{
				setCurrentStateID(PartyPartyClientStateID.Setup);
			}

			public void wait()
			{
				setCurrentStateID(PartyPartyClientStateID.Wait);
			}

			public void selectMusic()
			{
				setCurrentStateID(PartyPartyClientStateID.Wait);
			}

			public void requestJoin(uint hostAddress, UserInfo userInfo)
			{
				_lastKickReason = KickBy.None;
				_hostAddress = new IpAddress(hostAddress);
				_userInfo = (UserInfo)userInfo.Clone();
				_userInfo._ipAddress = Util.MyIpAddress(-1).ToNetworkByteOrderU32();
				setCurrentStateID(PartyPartyClientStateID.Connect);
			}

			public void cancelJoin()
			{
				isJoin();
				setCurrentStateID(PartyPartyClientStateID.Wait);
			}

			public void beginPlay()
			{
				setCurrentStateID(PartyPartyClientStateID.BeginPlay);
			}

			public void ready()
			{
				setCurrentStateID(PartyPartyClientStateID.Ready);
			}

			public void finishPlay()
			{
				setCurrentStateID(PartyPartyClientStateID.FinishPlay);
			}

			public void finishNews(bool gaugeClear, uint guageNumber)
			{
				FinishNews finishNews = new FinishNews(Util.MyIpAddress(-1).ToNetworkByteOrderU32(), gaugeClear, guageNumber);
				_tcpSocket.sendClass(finishNews);
				setCurrentStateID(PartyPartyClientStateID.NewsEnd);
			}

			public void sendClientPlayInfo(ClientPlayInfo info)
			{
				if (_tcpSocket.isActive())
				{
					isPlay();
					_lastSendClientPlayInfo.CopyFrom(info);
					_tcpSocket.sendClass(info);
				}
			}

			public ClientPlayInfo getLastSendClientPlayInfo()
			{
				return _lastSendClientPlayInfo;
			}

			public void sendUserInfo(UserInfo info)
			{
				if (_tcpSocket.isActive())
				{
					_userInfo = (UserInfo)info.Clone();
					UpdateUserInfo updateUserInfo = new UpdateUserInfo(_userInfo);
					_tcpSocket.sendClass(updateUserInfo);
				}
			}

			public bool isNormal()
			{
				PartyPartyClientStateID currentStateID = getCurrentStateID();
				if (!currentStateID.IsValid())
				{
					return false;
				}
				return currentStateID.IsNormal();
			}

			public bool isPlay()
			{
				return getCurrentStateID() == PartyPartyClientStateID.Play;
			}

			public bool isDisconnected()
			{
				return getCurrentStateID() == PartyPartyClientStateID.Disconnected;
			}

			public KickBy getLastKickReason()
			{
				return _lastKickReason;
			}

			public bool isError()
			{
				return getCurrentStateID() == PartyPartyClientStateID.Error;
			}

			public bool isJoin()
			{
				return _isJoin;
			}

			public bool isRequest()
			{
				PartyPartyClientStateID currentStateID = getCurrentStateID();
				if (!currentStateID.IsValid())
				{
					return false;
				}
				return currentStateID.IsRequest();
			}

			public DateTime getToReadyStartTime()
			{
				return _toReadyStartTime;
			}

			public RecruitList getRecruitList()
			{
				return _recruit.getList();
			}

			public RecruitList getRecruitListWithoutMe()
			{
				return _recruit.getListWithoutMe(-1);
			}

			public uint getRecvCount(Command command)
			{
				if (_tcpSocket.isRegisted(command))
				{
					return _tcpSocket.getRecvCount(command);
				}
				return _udpSocket.getRecvCount(command);
			}

			public PartyPlayInfo getPartyPlayInfo()
			{
				return _partyPlayInfo;
			}

			public PartyMemberInfo getPartyMemberInfo()
			{
				return _partyMemberInfo;
			}

			public PartyMemberState getPartyMemberState()
			{
				return _partyMemberState;
			}

			public PartyPartyClientStateID getPartyMemberMeState()
			{
				for (int i = 0; i < _partyMemberInfo._userInfo.Length; i++)
				{
					if (_partyMemberInfo._userInfo[i].IsMe(-1))
					{
						return (PartyPartyClientStateID)_partyMemberState._stateList[i];
					}
				}
				return PartyPartyClientStateID.Invalid;
			}

			public void info(ref string os)
			{
				os = os + "ClientState " + base.stateString + "\n";
				os = string.Concat(os, "tcpSocket ", _tcpSocket, "\n");
				os = string.Concat(os, "udpSocket ", _udpSocket, "\n");
				os = string.Concat(os, "hostAddress ", _hostAddress, "\n");
				os = string.Concat(os, "userInfo ", _userInfo, "\n");
				os = string.Concat(os, "heartBeat ", _heartBeat, "\n");
				os = string.Concat(os, "partyPlayInfo ", _partyPlayInfo, "\n");
				os = string.Concat(os, _partyMemberInfo, "\n");
				os = string.Concat(os, _partyMemberState, "\n");
				os = os + "toReadyStartTime " + _toReadyStartTime;
				os = os + "message " + _message + "\n";
				os += "recruit.info ";
				_recruit.info(ref os);
				os += "\n";
			}

			public void setCurrentStateID(PartyPartyClientStateID id)
			{
				if (_tcpSocket.isActive())
				{
					_tcpSocket.sendClass(new ClientState(id));
				}
				setNextState(id);
				updateState(-1f);
			}

			public long getPingUsec()
			{
				return _heartBeat.getPingUsec();
			}

			public long getPingFrame()
			{
				return _heartBeat.getPingFrame();
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

			private void recvStartRecruit(Packet packet)
			{
				StartRecruit param = packet.getParam<StartRecruit>();
				IManager manager = get();
				if (manager.getGroup() == (MachineGroupID)param._recruitInfo._groupID && manager.getEventMode() == param._recruitInfo._eventModeID)
				{
					param._recruitInfo._recvTime = DateTime.Now;
					_recruit.add(param._recruitInfo);
				}
			}

			private void recvFinishRecruit(Packet packet)
			{
				RecruitInfo recruitInfo = new RecruitInfo(packet.getParam<FinishRecruit>()._recruitInfo);
				IManager manager = get();
				if (manager.getGroup() == (MachineGroupID)recruitInfo._groupID && manager.getEventMode() == recruitInfo._eventModeID)
				{
					_recruit.remove(recruitInfo);
				}
			}

			private void recvJoinResult(Packet packet)
			{
				if (getCurrentStateID() == PartyPartyClientStateID.Request)
				{
					if (packet.getParam<JoinResult>().IsSuccess())
					{
						setCurrentStateID(PartyPartyClientStateID.Joined);
					}
					else
					{
						setCurrentStateID(PartyPartyClientStateID.Wait);
					}
				}
			}

			private void recvKick(Packet packet)
			{
				execKicked(packet.getParam<Kick>()?.kickBy ?? KickBy.None);
			}

			private void execKicked(KickBy reason)
			{
				setCurrentStateID(PartyPartyClientStateID.Disconnected);
				_lastKickReason = reason;
			}

			private void recvRequestMeasure(Packet packet)
			{
				ResponseMeasure responseMeasure = new ResponseMeasure();
				_tcpSocket.sendClass(responseMeasure);
			}

			private void recvStartPlay(Packet packet)
			{
				_startPlayCommand = packet.getParam<StartPlay>();
				setCurrentStateID(PartyPartyClientStateID.Sync);
			}

			private void recvStartClientState(Packet packet)
			{
				if (!isError() && isJoin())
				{
					StartClientState param = packet.getParam<StartClientState>();
					bool flag = false;
					PartyPartyClientStateID state = param.GetState();
					if (state != PartyPartyClientStateID.ToReady || true)
					{
						setCurrentStateID(param.GetState());
					}
				}
			}

			private void recvPartyPlayInfo(Packet packet)
			{
				if (_partyPlayInfo != null)
				{
					PartyPlayInfo param = packet.getParam<PartyPlayInfo>();
					_partyPlayInfo.CopyFrom(param);
				}
			}

			private void recvPartyMemberInfo(Packet packet)
			{
				if (_partyMemberInfo != null)
				{
					PartyMemberInfo param = packet.getParam<PartyMemberInfo>();
					_partyMemberInfo.CopyFrom(param);
				}
			}

			private void recvPartyMemberState(Packet packet)
			{
				_partyMemberState = packet.getParam<PartyMemberState>();
			}

			private void clear()
			{
				_udpSocket.close();
				_tcpSocket.close();
				_isJoin = false;
				_lastKickReason = KickBy.None;
			}

			private void error(string message)
			{
				_message = "error" + message;
				setCurrentStateID(PartyPartyClientStateID.Error);
			}

			public void Enter_First()
			{
				clear();
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
				setCurrentStateID(PartyPartyClientStateID.Wait);
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
					IpAddress ipAddress = new IpAddress(Util.MyIpAddress(-1));
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
				MemberPlayInfo[] member = _partyPlayInfo._member;
				foreach (MemberPlayInfo memberPlayInfo in member)
				{
					if (memberPlayInfo._ipAddress == 0)
					{
						memberPlayInfo._ipAddress = _partyMemberInfo._userInfo[num]._ipAddress;
					}
					memberPlayInfo._isActive = false;
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
				_tcpSocket.connect(_hostAddress, get().getPortNumber());
			}

			public void Execute_Connect()
			{
				if (_tcpSocket.isActive())
				{
					setCurrentStateID(PartyPartyClientStateID.Request);
				}
				else if (c_connectTimeOut < base.StateFrame)
				{
					_message = "Connect failed Time Out";
					setCurrentStateID(PartyPartyClientStateID.Wait);
				}
			}

			public void Enter_Request()
			{
				IManager manager = get();
				RequestJoin requestJoin = new RequestJoin(manager.getGroup(), manager.getEventMode(), _userInfo);
				_tcpSocket.sendClass(requestJoin);
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
				if ((_startPlayCommand._maxMeasure - _startPlayCommand._myMeasure) / 2 <= _tick.getUsec())
				{
					setCurrentStateID(PartyPartyClientStateID.Play);
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
				clear();
			}

			public void Execute_Finish()
			{
			}

			public void Enter_Error()
			{
				clear();
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

			public PartyPartyClientStateID getCurrentStateID()
			{
				return getCurrentState();
			}
		}

		public enum RecruitStance
		{
			OnlyFriend,
			EveryOne,
			MAX,
			Default
		}

		public enum PlayStatus
		{
			None,
			Lose,
			Win,
			Draw,
			MAX
		}

		public enum KickBy
		{
			None,
			Cancel,
			Start,
			MAX
		}

		[Serializable]
		public class UserInfo : ICloneable
		{
			public bool _isJoin;

			public uint _ipAddress;

			public long _userID;

			public string _playerName;

			public int _cardID;

			public int _trophyID;

			public int _playerRating;

			public int _musicID;

			public int _fumenDif;

			public int _optRatingID;

			public void CopyFrom(UserInfo src)
			{
				_isJoin = src._isJoin;
				_ipAddress = src._ipAddress;
				_userID = src._userID;
				_playerName = src._playerName;
				_cardID = src._cardID;
				_trophyID = src._trophyID;
				_playerRating = src._playerRating;
				_musicID = src._musicID;
				_fumenDif = src._fumenDif;
				_optRatingID = src._optRatingID;
			}

			public int serialize(int pos, Chunk chunk)
			{
				pos = chunk.writeBool(pos, _isJoin);
				pos = chunk.writeU32(pos, _ipAddress);
				pos = chunk.writeS64(pos, _userID);
				pos = chunk.writeString(pos, _playerName);
				pos = chunk.writeS32(pos, _cardID);
				pos = chunk.writeS32(pos, _trophyID);
				pos = chunk.writeS32(pos, _playerRating);
				pos = chunk.writeS32(pos, _musicID);
				pos = chunk.writeS32(pos, _fumenDif);
				pos = chunk.writeS32(pos, _optRatingID);
				return pos;
			}

			public UserInfo deserialize(ref int pos, Chunk chunk)
			{
				_isJoin = chunk.readBool(ref pos);
				_ipAddress = chunk.readU32(ref pos);
				_userID = chunk.readS64(ref pos);
				_playerName = chunk.readString(ref pos);
				_cardID = chunk.readS32(ref pos);
				_trophyID = chunk.readS32(ref pos);
				_playerRating = chunk.readS32(ref pos);
				_musicID = chunk.readS32(ref pos);
				_fumenDif = chunk.readS32(ref pos);
				_optRatingID = chunk.readS32(ref pos);
				return this;
			}

			public UserInfo()
			{
				Clear();
			}

			public UserInfo(UserInfo arg)
			{
				SetAll(arg);
			}

			public object Clone()
			{
				return new UserInfo(this);
			}

			public UserInfo(long userID, UserData userData, int musicId, MusicDifficultyID fumenDif)
				: this(userID, userData, musicId, fumenDif, 0)
			{
			}

			public UserInfo(long userID, UserData userData, int musicId, MusicDifficultyID fumenDif, int mockID)
				: this()
			{
				if (userData != null)
				{
					_isJoin = true;
					_ipAddress = Util.MyIpAddress(mockID).ToNetworkByteOrderU32();
					_userID = userID;
					_playerName = userData.Detail.UserName;
					_cardID = 0;
					_trophyID = userData.Detail.EquipTitleID;
					_playerRating = (int)userData.Detail.Rating;
					_musicID = musicId;
					_fumenDif = (int)fumenDif;
					_optRatingID = (int)userData.Option.DispRate;
				}
			}

			public void SetAll(UserInfo arg)
			{
				if (arg == null)
				{
					Clear();
					return;
				}
				_isJoin = arg._isJoin;
				_ipAddress = arg._ipAddress;
				_userID = arg._userID;
				_playerName = arg._playerName;
				_cardID = arg._cardID;
				_trophyID = arg._trophyID;
				_playerRating = arg._playerRating;
				_musicID = arg._musicID;
				_fumenDif = arg._fumenDif;
				_optRatingID = arg._optRatingID;
			}

			public void Clear()
			{
				_isJoin = false;
				_ipAddress = 0u;
				_userID = 0L;
				_playerName = "";
				_cardID = 0;
				_trophyID = 0;
				_playerRating = 0;
				_musicID = 0;
				_fumenDif = 0;
				_optRatingID = 0;
			}

			public string GetIpAddress()
			{
				return _ipAddress.convIpString();
			}

			public bool IsMe()
			{
				return IsMe(0);
			}

			public bool IsMe(int mockID)
			{
				return Util.isMyIP(_ipAddress, mockID);
			}

			public MusicDifficultyID GetMusicDifID()
			{
				return (MusicDifficultyID)_fumenDif;
			}

			public OptionDisprateID GetOptRatingID()
			{
				return (OptionDisprateID)_optRatingID;
			}

			public override string ToString()
			{
				return string.Concat("UserInfo[ isJoin=", _isJoin.ToString(), ", ip=", GetIpAddress(), ", userID=", _userID, ", music=", _musicID, ", dif=", GetMusicDifID(), ", name=", _playerName, ", card=", _cardID, ", trophyID=", _trophyID, ", rating=", _playerRating, ", optRating=", GetOptRatingID(), " ]");
			}
		}

		[Serializable]
		public class UpdateUserInfo : ICommandParam
		{
			public UserInfo _userInfo;

			public Command getCommand()
			{
				return Command.UpdateMechaInfo;
			}

			public UpdateUserInfo()
			{
				_userInfo = new UserInfo();
				clear();
			}

			public UpdateUserInfo(UserInfo info)
			{
				_userInfo = new UserInfo(info);
			}

			public void info(ref string os)
			{
				os = string.Concat(os, _userInfo, "\n");
			}

			public void clear()
			{
				_userInfo.Clear();
			}
		}

		[Serializable]
		public class PartyMemberInfo : ICommandParam, ICloneable
		{
			public UserInfo[] _userInfo;

			public Command getCommand()
			{
				return Command.PartyMemberInfo;
			}

			public void CopyFrom(PartyMemberInfo src)
			{
				for (int i = 0; i < _userInfo.Length; i++)
				{
					if (i < src._userInfo.Length)
					{
						_userInfo[i].CopyFrom(src._userInfo[i]);
					}
				}
			}

			public int Serialize(int pos, Chunk chunk)
			{
				UserInfo[] userInfo = _userInfo;
				for (int i = 0; i < userInfo.Length; i++)
				{
					pos = userInfo[i].serialize(pos, chunk);
				}
				return pos;
			}

			public PartyMemberInfo Deserialize(ref int pos, Chunk chunk)
			{
				UserInfo[] userInfo = _userInfo;
				for (int i = 0; i < userInfo.Length; i++)
				{
					userInfo[i].deserialize(ref pos, chunk);
				}
				return this;
			}

			public PartyMemberInfo()
			{
				_userInfo = new UserInfo[c_maxMember];
				for (int i = 0; i < _userInfo.Length; i++)
				{
					_userInfo[i] = new UserInfo();
				}
			}

			public int GetEntryNumber()
			{
				return _userInfo.Count((UserInfo i) => i._isJoin);
			}

			public IEnumerable<UserInfo> GetJoinMembers()
			{
				return _userInfo.Where((UserInfo member) => member._isJoin);
			}

			public IEnumerable<UserInfo> GetJoinMembersWithoutMe(int mockID = 0)
			{
				uint myIP = Util.MyIpAddress(mockID).ToNetworkByteOrderU32();
				return _userInfo.Where((UserInfo member) => member._isJoin && member._ipAddress != myIP);
			}

			public void Info(ref string os)
			{
				int num = 0;
				UserInfo[] userInfo = _userInfo;
				foreach (UserInfo userInfo2 in userInfo)
				{
					os = string.Concat(os, num, " ", userInfo2, "\n");
					num++;
				}
			}

			public void Clear()
			{
				UserInfo[] userInfo = _userInfo;
				for (int i = 0; i < userInfo.Length; i++)
				{
					userInfo[i].Clear();
				}
			}

			public object Clone()
			{
				PartyMemberInfo partyMemberInfo = new PartyMemberInfo();
				for (int i = 0; i < _userInfo.Length; i++)
				{
					partyMemberInfo._userInfo[i].SetAll(_userInfo[i]);
				}
				return partyMemberInfo;
			}

			public override string ToString()
			{
				string os = "";
				Info(ref os);
				return os;
			}
		}

		[Serializable]
		public class PartyMemberState : ICommandParam, ICloneable
		{
			public int[] _stateList = new int[c_maxMember];

			public Command getCommand()
			{
				return Command.PartyMemberState;
			}

			public PartyMemberState()
			{
				Clear();
			}

			public void Clear()
			{
				for (int i = 0; i < _stateList.Length; i++)
				{
					_stateList[i] = 0;
				}
			}

			public object Clone()
			{
				return new PartyMemberState
				{
					_stateList = (int[])_stateList.Clone()
				};
			}

			public void SetState(int n, PartyPartyClientStateID id)
			{
				if (0 <= n && n < _stateList.Length)
				{
					_stateList[n] = (int)id;
				}
			}

			public PartyPartyClientStateID GetState(int n)
			{
				return (PartyPartyClientStateID)_stateList[n];
			}

			public void Info(ref string os)
			{
				int num;
				for (num = 0; num < c_maxMember; num++)
				{
					PartyPartyClientStateID state = GetState(num);
					os = os + num + " " + (state.IsValid() ? state.GetName() : "") + "\n";
					num++;
				}
			}

			public override string ToString()
			{
				string os = "";
				Info(ref os);
				return os;
			}
		}

		[Serializable]
		public class PartyMemberPing
		{
			public long[] _ping = new long[c_maxMember];

			public PartyMemberPing()
			{
				Clear();
			}

			public void Clear()
			{
				for (int i = 0; i < _ping.Length; i++)
				{
					_ping[i] = 0L;
				}
			}
		}

		[Serializable]
		public class MemberPlayInfo : ICloneable
		{
			public uint _ipAddress;

			public bool _isActive;

			public byte _ranking;

			public int _achieve;

			public int _combo;

			public bool _miss;

			public int _missNum;

			public bool _gameOver;

			public bool _fullCombo;

			public void CopyFrom(MemberPlayInfo src)
			{
				_ipAddress = src._ipAddress;
				_isActive = src._isActive;
				_ranking = src._ranking;
				_achieve = src._achieve;
				_combo = src._combo;
				_missNum = src._missNum;
				_miss = src._miss;
				_gameOver = src._gameOver;
				_fullCombo = src._fullCombo;
			}

			public int Serialize(int pos, Chunk chunk)
			{
				pos = chunk.writeU32(pos, _ipAddress);
				pos = chunk.writeBool(pos, _isActive);
				pos = chunk.writeU8(pos, _ranking);
				pos = chunk.writeS32(pos, _achieve);
				pos = chunk.writeS32(pos, _combo);
				pos = chunk.writeS32(pos, _missNum);
				pos = chunk.writeBool(pos, _miss);
				pos = chunk.writeBool(pos, _gameOver);
				pos = chunk.writeBool(pos, _fullCombo);
				return pos;
			}

			public MemberPlayInfo Deserialize(ref int pos, Chunk chunk)
			{
				_ipAddress = chunk.readU32(ref pos);
				_isActive = chunk.readBool(ref pos);
				_ranking = chunk.readU8(ref pos);
				_achieve = chunk.readS32(ref pos);
				_combo = chunk.readS32(ref pos);
				_missNum = chunk.readS32(ref pos);
				_miss = chunk.readBool(ref pos);
				_gameOver = chunk.readBool(ref pos);
				_fullCombo = chunk.readBool(ref pos);
				return this;
			}

			public MemberPlayInfo()
			{
				Clear();
			}

			public MemberPlayInfo(MemberPlayInfo arg)
			{
				_ipAddress = arg._ipAddress;
				_isActive = arg._isActive;
				_ranking = arg._ranking;
				_achieve = arg._achieve;
				_combo = arg._combo;
				_missNum = arg._missNum;
				_miss = arg._miss;
				_gameOver = arg._gameOver;
				_fullCombo = arg._fullCombo;
			}

			public MemberPlayInfo(uint ipAddress, int achieve, int combo, int missNum, bool miss, bool gameOver, bool fullCombo)
				: this()
			{
				_ipAddress = ipAddress;
				_achieve = achieve;
				_combo = combo;
				_missNum = missNum;
				_miss = miss;
				_gameOver = gameOver;
				_fullCombo = fullCombo;
			}

			public void Clear()
			{
				_ipAddress = 0u;
				_isActive = false;
				_ranking = 0;
				_achieve = 0;
				_combo = 0;
				_missNum = 0;
				_miss = false;
				_gameOver = false;
				_fullCombo = false;
			}

			public object Clone()
			{
				return new MemberPlayInfo(this);
			}

			public bool IsError()
			{
				return !_isActive;
			}

			public bool IsExist()
			{
				return _ipAddress != 0;
			}

			public string GetIpAddress()
			{
				return _ipAddress.convIpString();
			}

			public bool IsMe()
			{
				return IsMe(0);
			}

			public bool IsMe(int mockID)
			{
				return Util.isMyIP(_ipAddress, mockID);
			}

			public void Info(ref string os)
			{
				os = os + (_isActive ? "O" : "X") + ",err=" + IsError().ToString() + ",rank=" + (int)_ranking + ",achieve=" + _achieve + ",combo=" + _combo + ",miss=" + _miss.ToString() + ",missNum=" + _missNum + ",gameOver=" + _gameOver.ToString() + ",fullCombo=" + _fullCombo.ToString() + ",ip=" + GetIpAddress() + "\n";
			}
		}

		[Serializable]
		public class PartyPlayInfo : ICommandParam
		{
			public MemberPlayInfo[] _member;

			public Command getCommand()
			{
				return Command.PartyPlayInfo;
			}

			public void CopyFrom(PartyPlayInfo src)
			{
				for (int i = 0; i < _member.Length; i++)
				{
					if (i < src._member.Length)
					{
						_member[i].CopyFrom(src._member[i]);
					}
				}
			}

			public int Serialize(int pos, Chunk chunk)
			{
				MemberPlayInfo[] member = _member;
				for (int i = 0; i < member.Length; i++)
				{
					pos = member[i].Serialize(pos, chunk);
				}
				return pos;
			}

			public PartyPlayInfo Deserialize(ref int pos, Chunk chunk)
			{
				MemberPlayInfo[] member = _member;
				for (int i = 0; i < member.Length; i++)
				{
					member[i].Deserialize(ref pos, chunk);
				}
				return this;
			}

			public PartyPlayInfo()
			{
				_member = new MemberPlayInfo[c_maxMember];
				for (int i = 0; i < _member.Length; i++)
				{
					_member[i] = new MemberPlayInfo();
				}
				Clear();
			}

			public PartyPlayInfo(PartyPlayInfo arg)
			{
				_member = new MemberPlayInfo[c_maxMember];
				for (int i = 0; i < _member.Length; i++)
				{
					_member[i] = new MemberPlayInfo(arg._member[i]);
				}
			}

			public void Clear()
			{
				MemberPlayInfo[] member = _member;
				for (int i = 0; i < member.Length; i++)
				{
					member[i].Clear();
				}
			}

			public uint GetActiveNumber()
			{
				return (uint)_member.Count((MemberPlayInfo i) => i._isActive);
			}

			public int CountExistMembers()
			{
				if (_member == null)
				{
					return 0;
				}
				int num = 0;
				MemberPlayInfo[] member = _member;
				for (int i = 0; i < member.Length; i++)
				{
					if (member[i].IsExist())
					{
						num++;
					}
				}
				return num;
			}

			public IEnumerable<MemberPlayInfo> GetJoinMembers()
			{
				return _member.Where((MemberPlayInfo member) => member.IsExist());
			}

			public IEnumerable<MemberPlayInfo> GetJoinMembersWithoutMe(int mockID = 0)
			{
				uint myIP = Util.MyIpAddress(mockID).ToNetworkByteOrderU32();
				return _member.Where((MemberPlayInfo member) => member.IsExist() && member._ipAddress != myIP);
			}

			public MemberPlayInfo GetJoinMemberMe(int mockID = 0)
			{
				uint myIP = Util.MyIpAddress(mockID).ToNetworkByteOrderU32();
				return _member.Where((MemberPlayInfo member) => member.IsExist() && member._ipAddress == myIP).FirstOrDefault();
			}

			public void Info(ref string os)
			{
				os += "PartyPlayInfo{\n";
				int num = 0;
				MemberPlayInfo[] member = _member;
				foreach (MemberPlayInfo memberPlayInfo in member)
				{
					os = os + "[" + num + "]";
					memberPlayInfo.Info(ref os);
					num++;
				}
				os += "}\n";
			}

			public override string ToString()
			{
				string os = "";
				Info(ref os);
				return os;
			}
		}

		[Serializable]
		public class ClientPlayInfo : ICommandParam, ICloneable
		{
			public uint _ipAddress;

			public bool _isValid;

			public int _count;

			public int _achieve;

			public int _combo;

			public bool _miss;

			public int _missNum;

			public bool _gameOver;

			public bool _fullCombo;

			public Command getCommand()
			{
				return Command.ClientPlayInfo;
			}

			public void CopyFrom(ClientPlayInfo src)
			{
				_ipAddress = src._ipAddress;
				_isValid = src._isValid;
				_count = src._count;
				_achieve = src._achieve;
				_combo = src._combo;
				_missNum = src._missNum;
				_miss = src._miss;
				_gameOver = src._gameOver;
				_fullCombo = src._fullCombo;
			}

			public int Serialize(int pos, Chunk chunk)
			{
				pos = chunk.writeU32(pos, _ipAddress);
				pos = chunk.writeBool(pos, _isValid);
				pos = chunk.writeS32(pos, _count);
				pos = chunk.writeS32(pos, _achieve);
				pos = chunk.writeS32(pos, _combo);
				pos = chunk.writeS32(pos, _missNum);
				pos = chunk.writeBool(pos, _miss);
				pos = chunk.writeBool(pos, _gameOver);
				pos = chunk.writeBool(pos, _fullCombo);
				return pos;
			}

			public ClientPlayInfo Deserialize(ref int pos, Chunk chunk)
			{
				_ipAddress = chunk.readU32(ref pos);
				_isValid = chunk.readBool(ref pos);
				_count = chunk.readS32(ref pos);
				_achieve = chunk.readS32(ref pos);
				_combo = chunk.readS32(ref pos);
				_missNum = chunk.readS32(ref pos);
				_miss = chunk.readBool(ref pos);
				_gameOver = chunk.readBool(ref pos);
				_fullCombo = chunk.readBool(ref pos);
				return this;
			}

			public ClientPlayInfo()
			{
				Clear();
			}

			public ClientPlayInfo(ClientPlayInfo arg)
			{
				_ipAddress = arg._ipAddress;
				_isValid = arg._isValid;
				_count = arg._count;
				_achieve = arg._achieve;
				_combo = arg._combo;
				_missNum = arg._missNum;
				_miss = arg._miss;
				_gameOver = arg._gameOver;
				_fullCombo = arg._fullCombo;
			}

			public void Clear()
			{
				_ipAddress = 0u;
				_isValid = false;
				_count = 0;
				_achieve = 0;
				_combo = 0;
				_missNum = 0;
				_miss = false;
				_gameOver = false;
				_fullCombo = false;
			}

			public object Clone()
			{
				return new ClientPlayInfo(this);
			}

			public bool IsAlive()
			{
				return !_gameOver;
			}

			public MemberPlayInfo GetMemberPlayInfo()
			{
				return new MemberPlayInfo(_ipAddress, _achieve, _combo, _missNum, _miss, _gameOver, _fullCombo);
			}

			public string GetIpAddress()
			{
				return _ipAddress.convIpString();
			}

			public void Info(ref string os)
			{
				os = os + " isValid=" + _isValid + ", ";
				os = os + " count=" + _count + ", ";
				os = os + " achieve=" + _achieve + ", ";
				os = os + " combo=" + _combo + ", ";
				os = os + " missNum=" + _missNum + ", ";
				os = os + " miss=" + _miss + ", ";
				os = os + " gameOver=" + _gameOver + ", ";
				os = os + " fullCombo=" + _fullCombo + ", ";
				os = os + "ip=" + GetIpAddress();
			}

			public override string ToString()
			{
				string os = "";
				Info(ref os);
				return os;
			}
		}

		[Serializable]
		public class FinishNews : ICommandParam
		{
			public uint _ipAddress;

			public bool _isValid;

			public bool _gaugeClear;

			public uint _gaugeStockNum;

			public Command getCommand()
			{
				return Command.FinishNews;
			}

			public FinishNews()
			{
				Clear();
			}

			public FinishNews(uint ipAddress, bool gaugeClear, uint gaugeStockNum)
			{
				_ipAddress = ipAddress;
				_isValid = true;
				_gaugeClear = gaugeClear;
				_gaugeStockNum = gaugeStockNum;
			}

			public void Clear()
			{
				_ipAddress = 0u;
				_isValid = false;
				_gaugeClear = false;
				_gaugeStockNum = 0u;
			}
		}

		[Serializable]
		public class RequestJoin : ICommandParam
		{
			public UserInfo _userInfo;

			public int _groupID;

			public bool _eventModeID;

			public Command getCommand()
			{
				return Command.RequestJoin;
			}

			public RequestJoin()
			{
				_userInfo = new UserInfo();
				_groupID = 1;
				_eventModeID = false;
			}

			public RequestJoin(MachineGroupID groupID, bool eventModeID, UserInfo userInfo)
			{
				_userInfo = new UserInfo(userInfo);
				_groupID = (int)groupID;
				_eventModeID = eventModeID;
			}
		}

		[Serializable]
		public class RecruitInfo
		{
			public UserInfo _userInfo;

			public int _musicID;

			public int _groupID;

			public bool _eventModeID;

			public int _joinNumber;

			public int _partyStance;

			[SerializeField]
			private long _startTimeTicks;

			[SerializeField]
			private long _recvTimeTicks;

			public DateTime _startTime
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

			public DateTime _recvTime
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

			public uint ipU32
			{
				get
				{
					if (_userInfo != null)
					{
						return _userInfo._ipAddress;
					}
					return 0u;
				}
			}

			public IpAddress ipAddress => new IpAddress(ipU32);

			public RecruitInfo()
			{
				_userInfo = new UserInfo();
				_musicID = 0;
				_groupID = 1;
				_eventModeID = false;
				_joinNumber = 0;
				_partyStance = 0;
				_startTime = DateTime.MinValue;
				_recvTime = DateTime.MinValue;
			}

			public RecruitInfo(RecruitInfo arg)
			{
				_userInfo = new UserInfo(arg._userInfo);
				_musicID = arg._musicID;
				_groupID = arg._groupID;
				_eventModeID = arg._eventModeID;
				_joinNumber = arg._joinNumber;
				_partyStance = arg._partyStance;
				_startTime = arg._startTime;
				_recvTime = arg._recvTime;
			}

			public RecruitInfo(MachineGroupID groupID, bool eventModeID, UserInfo userInfo, DateTime startTime, RecruitStance stanceID, int mockID)
			{
				_userInfo = (UserInfo)userInfo.Clone();
				_groupID = (int)groupID;
				_eventModeID = eventModeID;
				_musicID = _userInfo._musicID;
				_startTime = startTime;
				_joinNumber = 0;
				_partyStance = (int)stanceID;
				_recvTime = DateTime.MinValue;
			}

			public bool IsFull()
			{
				return c_maxMember <= _joinNumber;
			}

			public override string ToString()
			{
				return string.Concat("musicID=", _musicID, ", groupID=", _groupID, ", stanceID=", _partyStance, ", eventModeID=", _eventModeID.ToString(), ", startTime=", _startTime, ", recvTime=", _recvTime, ", joinNumber=", _joinNumber, ", ", _userInfo, "\n");
			}
		}

		[Serializable]
		public class StartRecruit : ICommandParam
		{
			public RecruitInfo _recruitInfo;

			public Command getCommand()
			{
				return Command.StartRecruit;
			}

			public StartRecruit()
			{
				_recruitInfo = new RecruitInfo();
			}

			public StartRecruit(RecruitInfo recruitInfo)
			{
				_recruitInfo = new RecruitInfo(recruitInfo);
			}
		}

		[Serializable]
		public class FinishRecruit : ICommandParam
		{
			public RecruitInfo _recruitInfo;

			public Command getCommand()
			{
				return Command.FinishRecruit;
			}

			public FinishRecruit()
			{
				_recruitInfo = new RecruitInfo();
			}

			public FinishRecruit(RecruitInfo recruitInfo)
			{
				_recruitInfo = new RecruitInfo(recruitInfo);
			}
		}

		[Serializable]
		public class Kick : ICommandParam
		{
			public RecruitInfo _recruitInfo;

			public int _kickBy;

			public KickBy kickBy => (KickBy)_kickBy;

			public Command getCommand()
			{
				return Command.Kick;
			}

			private Kick()
				: this(KickBy.None)
			{
			}

			public Kick(KickBy kickBy_arg)
			{
				_kickBy = (int)kickBy_arg;
				_recruitInfo = new RecruitInfo();
			}

			public Kick(KickBy kickBy_arg, RecruitInfo recruitInfo)
			{
				_kickBy = (int)kickBy_arg;
				_recruitInfo = new RecruitInfo(recruitInfo);
			}
		}

		[Serializable]
		public class JoinResult : ICommandParam
		{
			public int _resultID;

			public Command getCommand()
			{
				return Command.JoinResult;
			}

			public JoinResult()
			{
				_resultID = 0;
			}

			public JoinResult(PartyPartyJoinResultID id)
			{
				_resultID = (int)id;
			}

			public PartyPartyJoinResultID getResult()
			{
				return (PartyPartyJoinResultID)_resultID;
			}

			public bool IsSuccess()
			{
				return 1 == _resultID;
			}

			public override string ToString()
			{
				return getResult().GetName();
			}
		}

		[Serializable]
		public class RequestMeasure : ICommandParam
		{
			public Command getCommand()
			{
				return Command.RequestMeasure;
			}
		}

		[Serializable]
		public class ResponseMeasure : ICommandParam
		{
			public Command getCommand()
			{
				return Command.ResponseMeasure;
			}
		}

		[Serializable]
		public class StartPlay : ICommandParam
		{
			public long _maxMeasure;

			public long _myMeasure;

			public Command getCommand()
			{
				return Command.StartPlay;
			}

			public StartPlay(long maxMeasure, long myMeasure)
			{
				_maxMeasure = maxMeasure;
				_myMeasure = myMeasure;
			}

			public StartPlay()
			{
				Clear();
			}

			public void Clear()
			{
				_maxMeasure = 0L;
				_myMeasure = 0L;
			}

			public override string ToString()
			{
				return string.Concat(string.Concat("" + getCommand(), _maxMeasure), _myMeasure);
			}
		}

		[Serializable]
		public class StartClientState : ICommandParam
		{
			public int _clientStateID;

			public Command getCommand()
			{
				return Command.StartClientState;
			}

			public StartClientState()
			{
				_clientStateID = 0;
			}

			public StartClientState(PartyPartyClientStateID id)
			{
				_clientStateID = (int)id;
			}

			public PartyPartyClientStateID GetState()
			{
				return (PartyPartyClientStateID)_clientStateID;
			}

			public override string ToString()
			{
				string text = "";
				PartyPartyClientStateID state = GetState();
				return string.Concat(text, getCommand(), " ", state.IsValid() ? state.GetName() : "INVALID", "\n");
			}
		}

		[Serializable]
		public class ClientState : ICommandParam
		{
			public int _state;

			public Command getCommand()
			{
				return Command.ClientState;
			}

			public ClientState(PartyPartyClientStateID state)
			{
				_state = (int)state;
			}

			public ClientState()
			{
				_state = 0;
			}

			public PartyPartyClientStateID GetState()
			{
				return (PartyPartyClientStateID)_state;
			}
		}

		[Serializable]
		public class CancelJoin : ICommandParam
		{
			public Command getCommand()
			{
				return Command.CancelJoin;
			}
		}

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

			private byte[] _rankCalcBuf;

			private int[] _scoreCalcBuf;

			private bool _alreadyDisposed;

			private const int MockID = -1;

			private Host()
			{
			}

			private Host(Host other)
			{
			}

			public Host(string name, InitParam initParam, int mockID)
			{
				_listenSocket = new ListenSocket(name + "Listen", -1);
				_broadcastSocket = new BroadcastSocket(name + "BroadCast", -1);
				_memberList = new MemberList();
				_recruitInfo = new RecruitInfo();
				_refreshRecruitTime = DateTime.MinValue;
				_partyPlayInfo = new PartyPlayInfo();
				_message = "";
				_partyMemberPing = new PartyMemberPing();
				_rankCalcBuf = new byte[c_maxMember];
				_scoreCalcBuf = new int[c_maxMember];
				setCurrentStateID(PartyPartyHostStateID.First);
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
						Util.SafeDispose(_memberList);
						_memberList = null;
						_recruitInfo = null;
						_partyPlayInfo = null;
						_partyMemberPing = null;
						GC.SuppressFinalize(this);
					}
					_alreadyDisposed = true;
				}
			}

			public void update()
			{
				updateState(-1f);
				executeAccept();
				foreach (Member member in _memberList)
				{
					member.update();
				}
				updateMemberState();
				updateUserInfo();
				updatePartyMemberPing();
			}

			public void initialize()
			{
				setCurrentStateID(PartyPartyHostStateID.First);
			}

			public void terminate()
			{
				setCurrentStateID(PartyPartyHostStateID.Finish);
			}

			public void start()
			{
				setCurrentStateID(PartyPartyHostStateID.Setup);
			}

			public void wait()
			{
				if (isNormal())
				{
					setCurrentStateID(PartyPartyHostStateID.Wait);
				}
			}

			public void selectMusic()
			{
				setCurrentStateID(PartyPartyHostStateID.Wait);
			}

			public void startRecruit(UserInfo userInfo, RecruitStance stanceID)
			{
				_recruitInfo = new RecruitInfo(get().getGroup(), get().getEventMode(), userInfo, DateTime.Now, stanceID, -1);
				setCurrentStateID(PartyPartyHostStateID.Recruit);
			}

			public void cancelRecruit()
			{
				foreach (Member member in _memberList)
				{
					member.kickByCancel();
				}
				setCurrentStateID(PartyPartyHostStateID.Wait);
			}

			public void finishRecruit()
			{
				if (isRecruit())
				{
					setCurrentStateID(PartyPartyHostStateID.FinishRecruit);
				}
			}

			public void beginPlay()
			{
				setCurrentStateID(PartyPartyHostStateID.BeginPlay);
			}

			public void ready()
			{
				setCurrentStateID(PartyPartyHostStateID.Ready);
			}

			private void notifyPartyPlayInfo(PartyPlayInfo info)
			{
				PartyPlayInfo i = new PartyPlayInfo(info);
				foreach (Member member in _memberList)
				{
					if (member.isJoin())
					{
						member.notifyPartyPlayInfo(i);
					}
				}
			}

			public PartyPartyJoinResultID requestJoin(Member member, RequestJoin info)
			{
				if (!isRecruit())
				{
					_message = "requestJoin refused because NoRecruit";
					return PartyPartyJoinResultID.NoRecruit;
				}
				if (c_maxMember <= getEntryNumber())
				{
					_message = "requestJoin refused because Full.";
					return PartyPartyJoinResultID.Full;
				}
				if (_recruitInfo._groupID != info._groupID)
				{
					_message = "requestJoin refused because Different Group.";
					return PartyPartyJoinResultID.DifferentGroup;
				}
				if (_recruitInfo._eventModeID != info._eventModeID)
				{
					_message = "requestJoin refused because Different Event.";
					return PartyPartyJoinResultID.DifferentEventMode;
				}
				if (_recruitInfo._musicID != info._userInfo._musicID)
				{
					_message = "requestJoin refused because Different Music ID.";
					return PartyPartyJoinResultID.DifferentMusic;
				}
				if (member.isJoin())
				{
					_message = "requestJoin refused because AlreadyJoined.";
					return PartyPartyJoinResultID.AlreadyJoined;
				}
				member.join(info._userInfo);
				sendPartyMemberInfo();
				sendRecruitInfo();
				return PartyPartyJoinResultID.Success;
			}

			public bool isNormal()
			{
				return getCurrentStateID().IsNormal();
			}

			public bool isError()
			{
				return getCurrentStateID() == PartyPartyHostStateID.Error;
			}

			public bool isWorking()
			{
				return getCurrentStateID().IsWorking();
			}

			public bool isPlay()
			{
				return getCurrentStateID().IsPlay();
			}

			public bool isRecruit()
			{
				return getCurrentStateID().IsRecruit();
			}

			public int getEntryNumber()
			{
				return _memberList.Count((Member c) => c.isJoin());
			}

			public bool isMemberState(PartyPartyClientStateID id)
			{
				if (getEntryNumber() == 0)
				{
					return false;
				}
				foreach (Member member in _memberList)
				{
					if (member.isJoin() && member.isActive() && member.getCurrentStateID() != id)
					{
						return false;
					}
				}
				return true;
			}

			public uint getRecvCount(int memberNo, Command command)
			{
				_memberList.Count();
				return _memberList[memberNo].getRecvCount(command);
			}

			public MemberList getMemberList()
			{
				return _memberList;
			}

			public void info(ref string os)
			{
				os = os + "HostState " + base.stateString + "\n";
				os = os + "message " + _message + "\n";
				os = string.Concat(os, "listenSocket ", _listenSocket, "\n");
				os = string.Concat(os, "broadcastSocket", _broadcastSocket, "\n");
				os += "recruitInfo=\n";
				os += _recruitInfo;
				os += "_partyPlayInfo\n";
				_partyPlayInfo.Info(ref os);
				if (_memberList.Count() == 0)
				{
					os += "member empty\n";
					return;
				}
				foreach (Member member in _memberList)
				{
					os += "member.base ";
					member.infoBase(ref os);
				}
				foreach (Member member2 in _memberList)
				{
					os += "member.user   ";
					member2.infoUser(ref os);
				}
				foreach (Member member3 in _memberList)
				{
					os += "member.play   ";
					member3.infoPlay(ref os);
					os += "\n";
				}
				foreach (Member member4 in _memberList)
				{
					os += "member.measure ";
					member4.infoMeasure(ref os);
					os += "\n";
				}
			}

			public PartyPlayInfo getPartyPlayInfo()
			{
				return _partyPlayInfo;
			}

			public void updateRecruitUserInfo(UserInfo userInfo)
			{
				_recruitInfo._userInfo = new UserInfo(userInfo);
				if (isRecruit())
				{
					sendRecruitInfo();
				}
			}

			public PartyMemberPing getPartyMemberPing()
			{
				return _partyMemberPing;
			}

			private void clear()
			{
				_listenSocket.close();
				_broadcastSocket.close();
				_memberList.ClearWithDispose();
				_partyPlayInfo.Clear();
			}

			private void executeAccept()
			{
				IpAddress outAddress;
				NFSocket nFSocket = _listenSocket.acceptClient(out outAddress);
				if (nFSocket != null)
				{
					string text = "Party::Member";
					text += _memberList.Count();
					_memberList.erase_if((Member m) => m.getAddress() == outAddress);
					_memberList.Add(new Member(text, this, nFSocket));
				}
			}

			private void sendPartyMemberInfo()
			{
				PartyMemberInfo partyMemberInfo = new PartyMemberInfo();
				int num = 0;
				foreach (Member member in _memberList)
				{
					if (member.isJoin())
					{
						partyMemberInfo._userInfo[num] = new UserInfo(member.getUserInfo());
						num++;
					}
				}
				foreach (Member member2 in _memberList)
				{
					if (member2.isJoin())
					{
						member2.sendPartyMemberInfo(partyMemberInfo);
					}
				}
			}

			private void sendPartyMemberState()
			{
				PartyMemberState partyMemberState = new PartyMemberState();
				int num = 0;
				foreach (Member member in _memberList)
				{
					if (member.isJoin())
					{
						partyMemberState.SetState(num, member.getCurrentStateID());
						member.setStateChanged(stateChanged: false);
						num++;
					}
				}
				foreach (Member member2 in _memberList)
				{
					if (member2.isJoin())
					{
						member2.sendPartyMemberState(partyMemberState);
					}
				}
			}

			private void sendRecruitInfo()
			{
				_recruitInfo._joinNumber = getEntryNumber();
				_broadcastSocket.sendClass(new StartRecruit(_recruitInfo));
				_refreshRecruitTime = DateTime.Now;
			}

			private void updateMemberState()
			{
				bool flag = false;
				foreach (Member member in _memberList)
				{
					if (member.isStateChanged())
					{
						flag = true;
					}
				}
				if (flag)
				{
					sendPartyMemberState();
				}
			}

			private void updateUserInfo()
			{
				bool flag = false;
				foreach (Member member in _memberList)
				{
					if (member.isUserInfoChanged())
					{
						member.setUserInfoChanged(userInfoChanged: false);
						flag = true;
					}
				}
				if (flag)
				{
					sendPartyMemberInfo();
				}
			}

			private void updatePartyMemberPing()
			{
				int num = 0;
				_partyMemberPing.Clear();
				foreach (Member member in _memberList)
				{
					if (member.isJoin())
					{
						if (member.isActive())
						{
							_partyMemberPing._ping[num] = member.getPingFrame();
						}
						num++;
					}
				}
			}

			private void eraseDisconnectMember(bool isAutoSend)
			{
				int num = _memberList.Count();
				_memberList.erase_if((Member m) => m == null || !m.isActive());
				if (num == _memberList.Count())
				{
					return;
				}
				foreach (Member member in _memberList)
				{
					if (member.isJoin())
					{
						member.setStateChanged(stateChanged: true);
					}
				}
				if (isAutoSend)
				{
					sendPartyMemberInfo();
					sendPartyMemberState();
					sendRecruitInfo();
				}
			}

			private void sendPartyPlayInfo()
			{
				int num = 0;
				foreach (Member member in _memberList)
				{
					if (member.isJoin())
					{
						_partyPlayInfo._member[num] = (MemberPlayInfo)member.getLastClientPlayInfo().GetMemberPlayInfo().Clone();
						_partyPlayInfo._member[num]._isActive = member.isActive();
						_partyPlayInfo._member[num]._ipAddress = member.getUserInfo()._ipAddress;
						num++;
					}
				}
				for (int i = 0; i < _scoreCalcBuf.Length; i++)
				{
					_scoreCalcBuf[i] = _partyPlayInfo._member[i]._achieve;
				}
				getRank(_scoreCalcBuf, _rankCalcBuf);
				for (int j = 0; j < _partyPlayInfo._member.Length; j++)
				{
					if (j < _rankCalcBuf.Length)
					{
						_partyPlayInfo._member[j]._ranking = _rankCalcBuf[j];
					}
				}
				notifyPartyPlayInfo(_partyPlayInfo);
			}

			public void Enter_First()
			{
				clear();
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
				_listenSocket.open(CPortNumber);
				if (!_listenSocket.isValid())
				{
					setCurrentStateID(PartyPartyHostStateID.Error);
				}
				else
				{
					setCurrentStateID(PartyPartyHostStateID.Wait);
				}
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
				_broadcastSocket.open(new IpAddress(Util.BroadcastAddress()), CPortNumber);
				_broadcastSocket.isValid();
				_partyPlayInfo = new PartyPlayInfo();
				sendRecruitInfo();
			}

			public void Execute_Recruit()
			{
				DateTime now = DateTime.Now;
				if (_refreshRecruitTime.AddSeconds(c_recruitRefresh) < now)
				{
					sendRecruitInfo();
				}
				eraseDisconnectMember(isAutoSend: true);
			}

			public void Leave_Recruit()
			{
				FinishRecruit finishRecruit = new FinishRecruit(_recruitInfo);
				_broadcastSocket.sendClass(finishRecruit);
			}

			public void Enter_FinishRecruit()
			{
				foreach (Member member in _memberList)
				{
					if (member.isJoin())
					{
						member.startState(PartyPartyClientStateID.ToReady);
					}
				}
			}

			public void Execute_FinishRecruit()
			{
			}

			public void Execute_BeginPlay()
			{
				if (isMemberState(PartyPartyClientStateID.BeginPlay))
				{
					setCurrentStateID(PartyPartyHostStateID.AllBeginPlay);
				}
			}

			public void Enter_AllBeginPlay()
			{
				eraseDisconnectMember(isAutoSend: false);
				sendPartyMemberInfo();
				foreach (Member member in _memberList)
				{
					if (member.isJoin())
					{
						member.startState(PartyPartyClientStateID.AllBeginPlay);
					}
				}
			}

			public void Execute_AllBeginPlay()
			{
			}

			public void Execute_Ready()
			{
				if (isMemberState(PartyPartyClientStateID.Ready))
				{
					setCurrentStateID(PartyPartyHostStateID.Sync);
				}
			}

			public void Enter_Sync()
			{
				foreach (Member member in _memberList)
				{
					if (member.isJoin())
					{
						member.requestMeasure();
					}
				}
			}

			public void Execute_Sync()
			{
				uint num = 0u;
				uint num2 = 0u;
				foreach (Member member in _memberList)
				{
					if (member.isJoin() && member.isActive())
					{
						num2++;
					}
					if (member.isResponsedMeasure())
					{
						num++;
					}
				}
				if (num == num2)
				{
					setCurrentStateID(PartyPartyHostStateID.Play);
				}
			}

			public void Enter_Play()
			{
				long num = 0L;
				foreach (Member member in _memberList)
				{
					if (member.isResponsedMeasure() && member.isJoin() && member.isActive())
					{
						num = Math.Max(num, member.getMeasureDiffUsec());
					}
				}
				foreach (Member member2 in _memberList)
				{
					if (member2.isJoin())
					{
						member2.startPlay(num, member2.getMeasureDiffUsec());
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
					if (!member.isJoin() || !member.isActive())
					{
						continue;
					}
					Queue<ClientPlayInfo> clientPlayInfo = member.getClientPlayInfo();
					if (!member.isFinishPlay() || clientPlayInfo.Count != 0)
					{
						num2++;
						if (clientPlayInfo.Count == 0)
						{
							return;
						}
						ClientPlayInfo clientPlayInfo2 = clientPlayInfo.Peek();
						if (clientPlayInfo2._count < num)
						{
							num = clientPlayInfo2._count;
						}
					}
				}
				if (num2 == 0)
				{
					sendPartyPlayInfo();
					setCurrentStateID(PartyPartyHostStateID.News);
				}
				else
				{
					if (num == int.MaxValue)
					{
						return;
					}
					foreach (Member member2 in _memberList)
					{
						if (member2.isJoin() && member2.isActive())
						{
							member2.updateLastClientPlayInfo(num);
						}
					}
					int num3 = 0;
					foreach (Member member3 in _memberList)
					{
						if (member3.isJoin() && member3.isActive() && member3.getLastClientPlayInfo().IsAlive())
						{
							num3++;
						}
					}
					foreach (Member member4 in _memberList)
					{
						if (member4.isJoin() && member4.isActive())
						{
							member4.popClientPlayInfo(num);
						}
					}
					sendPartyPlayInfo();
				}
			}

			public void Enter_News()
			{
				foreach (Member member in _memberList)
				{
					if (member.isJoin())
					{
						member.startState(PartyPartyClientStateID.News);
					}
				}
			}

			public void Execute_News()
			{
				if (isMemberState(PartyPartyClientStateID.NewsEnd))
				{
					setCurrentStateID(PartyPartyHostStateID.Result);
				}
			}

			public void Enter_Result()
			{
				notifyPartyPlayInfo(_partyPlayInfo);
				foreach (Member member in _memberList)
				{
					if (member.isJoin())
					{
						member.startState(PartyPartyClientStateID.Result);
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
				clear();
			}

			public void Execute_Finish()
			{
			}

			public void Enter_Error()
			{
				clear();
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

			public PartyPartyHostStateID getCurrentStateID()
			{
				return getCurrentState();
			}

			private void setCurrentStateID(PartyPartyHostStateID nextState)
			{
				setNextState(nextState);
				updateState(-1f);
			}
		}

		public interface IMember
		{
			UserInfo getUserInfo();

			IpAddress getAddress();

			bool isJoin();

			bool isActive();

			Queue<ClientPlayInfo> getClientPlayInfo();

			void infoBase(ref string os);

			void infoUser(ref string os);

			void infoPlay(ref string os);

			void infoMeasure(ref string os);
		}

		public class MemberList : MemberList_Base<Member>
		{
		}

		public class Member : IMember, IDisposable
		{
			private readonly Host _host;

			private bool _isJoin;

			private PartyPartyClientStateID _state;

			private bool _stateChanged;

			private UserInfo _userInfo;

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
				_userInfo = new UserInfo();
				_userInfoChanged = false;
				_isJoin = false;
				_heartBeat = new HeartBeat(_socket, c_heartBeatInterval, c_heartBeatTimeout);
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
				_socket.registCommand(Command.RequestJoin, recvRequestJoin);
				_socket.registCommand(Command.ClientState, recvClientState);
				_socket.registCommand(Command.HeartBeatRequest, recvHeartBeatRequest);
				_socket.registCommand(Command.HeartBeatResponse, recvHeartBeatResponse);
				_socket.registCommand(Command.ClientPlayInfo, recvClientPlayInfo);
				_socket.registCommand(Command.UpdateMechaInfo, recvUserInfo);
				_socket.registCommand(Command.ResponseMeasure, recvResponseMeasure);
				_socket.registCommand(Command.FinishNews, recvFinishNews);
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
						_userInfo = null;
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

			public void update()
			{
				_socket.update();
				_heartBeat.update();
				if (getCurrentStateID() != PartyPartyClientStateID.Disconnected && !isActive())
				{
					setCurrentStateID(PartyPartyClientStateID.Disconnected);
				}
			}

			public void join(UserInfo userInfo)
			{
				_userInfo = userInfo;
				_isJoin = true;
			}

			public void kickByCancel()
			{
				_isJoin = false;
				_socket.sendClass(new Kick(KickBy.Cancel));
			}

			public void kickByStart()
			{
				_isJoin = false;
				_socket.sendClass(new Kick(KickBy.Start));
			}

			public void startPlay(long maxMeasure, long myMeasure)
			{
				_socket.sendClass(new StartPlay(maxMeasure, myMeasure));
			}

			public void startState(PartyPartyClientStateID state)
			{
				_socket.sendClass(new StartClientState(state));
			}

			public void sendPartyMemberInfo(PartyMemberInfo info)
			{
				_socket.sendClass(info);
			}

			public void sendPartyMemberState(PartyMemberState info)
			{
				_socket.sendClass(info);
			}

			public void requestMeasure()
			{
				_measureTick.reset();
				_measureSendUsec = _measureTick.getUsec();
				_measureResponsedUsec = 0L;
				_measureDiffUsec = 0L;
				_isResponsedMeasure = false;
				_socket.sendClass(new RequestMeasure());
			}

			public bool isResponsedMeasure()
			{
				return _isResponsedMeasure;
			}

			public long getMeasureDiffUsec()
			{
				return _measureDiffUsec;
			}

			public void notifyPartyPlayInfo(PartyPlayInfo i)
			{
				_socket.sendClass(i);
			}

			public ConnectSocket getSocket()
			{
				return _socket;
			}

			public bool isJoin()
			{
				return _isJoin;
			}

			public bool isFinishSetting()
			{
				if (isJoin())
				{
					return getCurrentStateID() == PartyPartyClientStateID.FinishSetting;
				}
				return false;
			}

			public bool isActive()
			{
				if (_socket != null)
				{
					return _socket.isActive();
				}
				return false;
			}

			public bool isFinishPlay()
			{
				return getCurrentStateID() == PartyPartyClientStateID.FinishPlay;
			}

			public PartyPartyClientStateID getCurrentStateID()
			{
				return _state;
			}

			public uint getRecvCount(Command command)
			{
				return _socket.getRecvCount(command);
			}

			public UserInfo getUserInfo()
			{
				return _userInfo;
			}

			public IpAddress getAddress()
			{
				return _socket.getDestinationAddress();
			}

			public void infoBase(ref string os)
			{
				if (isActive())
				{
					os += " Connect    ";
				}
				else
				{
					os += " Disconnect ";
				}
				os = os + " J " + _isJoin;
				os = os + " " + (_state.IsValid() ? _state.GetName() : "");
				os = os + " So " + _socket;
				os = os + " PngF " + getPingFrame();
				os = os + " PngU " + getPingUsec();
				os += "\n";
			}

			public void infoUser(ref string os)
			{
				os = string.Concat(os, _userInfo, " ", _socket.getDestinationAddress(), "\n");
			}

			public void infoPlay(ref string os)
			{
				os = os + "recv " + _clientPlayInfoRecv + " ";
				os = os + "size " + _clientPlayInfo.Count + " ";
				os = string.Concat(os, "Info( ", _clientPlayInfoLast, ") ");
				os += _socket.getDestinationAddress();
			}

			public void infoMeasure(ref string os)
			{
				os += " Measure ";
				os = os + " isResponsed " + _isResponsedMeasure;
				os = os + " SendUsec " + _measureSendUsec;
				os = os + " ResponsedUsec " + _measureResponsedUsec;
				os = os + " DiffUsec " + _measureDiffUsec;
			}

			public Queue<ClientPlayInfo> getClientPlayInfo()
			{
				return _clientPlayInfo;
			}

			public ClientPlayInfo getLastClientPlayInfo()
			{
				return _clientPlayInfoLast;
			}

			public FinishNews getFinishNews()
			{
				return _finishNews;
			}

			public void popClientPlayInfo(int current)
			{
				Queue<ClientPlayInfo> clientPlayInfo = _clientPlayInfo;
				if (clientPlayInfo.Count != 0 && clientPlayInfo.Peek()._count == current)
				{
					ClientPlayInfo t = clientPlayInfo.Dequeue();
					_clientPlayInfoPool.Return(t);
				}
			}

			public void updateLastClientPlayInfo(int current)
			{
				Queue<ClientPlayInfo> clientPlayInfo = _clientPlayInfo;
				if (clientPlayInfo.Count != 0 && clientPlayInfo.Peek()._count == current)
				{
					_clientPlayInfoLast = new ClientPlayInfo(_clientPlayInfo.Peek());
				}
			}

			public void setCurrentStateID(PartyPartyClientStateID state)
			{
				_state = state;
				_stateChanged = true;
			}

			public void setStateChanged(bool stateChanged)
			{
				_stateChanged = stateChanged;
			}

			public bool isStateChanged()
			{
				return _stateChanged;
			}

			public void setUserInfoChanged(bool userInfoChanged)
			{
				_userInfoChanged = userInfoChanged;
			}

			public bool isUserInfoChanged()
			{
				return _userInfoChanged;
			}

			public long getPingFrame()
			{
				return _heartBeat.getPingFrame();
			}

			public long getPingUsec()
			{
				return _heartBeat.getPingUsec();
			}

			private void recvRequestJoin(Packet packet)
			{
				RequestJoin param = packet.getParam<RequestJoin>();
				JoinResult info = new JoinResult(_host.requestJoin(this, param));
				_socket.sendClass(info);
			}

			private void recvClientState(Packet packet)
			{
				ClientState param = packet.getParam<ClientState>();
				setCurrentStateID(param.GetState());
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

			private void recvClientPlayInfo(Packet packet)
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

			private void recvUserInfo(Packet packet)
			{
				UpdateUserInfo param = packet.getParam<UpdateUserInfo>();
				_userInfo = new UserInfo(param._userInfo);
				_userInfoChanged = true;
			}

			private void recvFinishNews(Packet packet)
			{
				_finishNews = packet.getParam<FinishNews>();
			}

			private void recvResponseMeasure(Packet packet)
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

		public class InitParam
		{
			public ushort _portNumber;

			public float _startRecruitInterval;

			public int _heartBeatInterval;

			public InitParam()
			{
				_portNumber = CPortNumber;
				_startRecruitInterval = c_recruitRefresh;
				_heartBeatInterval = c_heartBeatInterval;
			}

			public InitParam(InitParam arg)
			{
				_portNumber = arg._portNumber;
				_startRecruitInterval = arg._startRecruitInterval;
				_heartBeatInterval = arg._heartBeatInterval;
			}
		}

		public class RecruitList : List<RecruitInfo>
		{
			public RecruitList()
			{
			}

			public RecruitList(IEnumerable<RecruitInfo> list)
				: base(list)
			{
			}

			public override string ToString()
			{
				string text = "RecruitList {\n";
				using (Enumerator enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						RecruitInfo current = enumerator.Current;
						text = string.Concat(text, current, "\n");
					}
				}
				return text + "}\n";
			}
		}

		public class Recruit
		{
			private string _name;

			private RecruitList _list;

			public Recruit(string name)
			{
				_name = name;
				_list = new RecruitList();
			}

			public void add(RecruitInfo addInfo)
			{
				int num = _list.FindIndex((RecruitInfo info) => info.ipU32 == addInfo.ipU32);
				if (num >= 0)
				{
					_list[num] = addInfo;
				}
				else
				{
					_list.Add(addInfo);
				}
			}

			public void remove(RecruitInfo reomveInfo)
			{
				RecruitInfo recruitInfo = _list.Find((RecruitInfo info) => info.ipU32 == reomveInfo.ipU32);
				if (recruitInfo != null && recruitInfo._startTime == reomveInfo._startTime)
				{
					_list.Remove(recruitInfo);
				}
			}

			public void update(DateTime now)
			{
				List<int> list = new List<int>();
				for (int i = 0; i < _list.Count(); i++)
				{
					if (_list[i]._recvTime.AddSeconds(c_recruitTimeout) <= now)
					{
						list.Add(i);
					}
				}
				for (int num = list.Count - 1; num >= 0; num--)
				{
					_list.RemoveAt(num);
				}
			}

			public RecruitList getList()
			{
				return _list;
			}

			public RecruitList getListWithoutMe(int mockID = 0)
			{
				uint myIp = Util.MyIpAddress(mockID).ToNetworkByteOrderU32();
				return new RecruitList(_list.Where((RecruitInfo info) => myIp != info.ipU32));
			}

			public void info(ref string os)
			{
				os = os + _name + "\n";
				os += _list;
			}
		}

		public interface IManager
		{
			void execute();

			void initialize();

			void terminate();

			void start(MachineGroupID id);

			void wait();

			void selectMusic();

			bool startRecruit(UserInfo userInfo, RecruitStance stanceID);

			void finishRecruit();

			void battleStart();

			bool startJoin(IpAddress hostAddress, UserInfo userInfo);

			void sendUserInfo(UserInfo userInfo);

			void cancelBothRecruitJoin();

			bool finishSetting();

			bool backToSetting();

			void beginPlay();

			void ready();

			void finishPlay();

			void finishNews(bool _gaugeClear, uint _gaugeStockNum);

			void sendClientPlayInfo(ClientPlayInfo info);

			ClientPlayInfo getLastSendClientPlayInfo();

			PartyPlayInfo getPartyPlayInfo();

			RecruitList getRecruitList();

			RecruitList getRecruitListForMusicList();

			RecruitList getRecruitListWithoutMe();

			ushort getPortNumber();

			bool isHost();

			bool isClient();

			bool isNormal();

			bool isJoin();

			bool isJoinAndActive();

			bool isError();

			bool isClientJoinActive();

			bool isWaitSetting();

			bool isWaitSettingAsClient();

			bool isWaitSettingAsHost();

			bool isClientDisconnected();

			KickBy getClientLastKickReason();

			bool isClientToReady();

			PartyMemberInfo getPartyMemberInfo();

			PartyMemberState getPartyMemberState();

			bool isAllBeginPlay();

			bool isPlay();

			bool isNews();

			bool isResult();

			void info(ref string os);

			PartyPartyManagerStateID getCurrentStateID();

			uint getClientRecvCount(Command command);

			uint getHostRecvCount(int no, Command command);

			PartyPartyClientStateID getClientStateID();

			PartyPartyHostStateID getHostStateID();

			MachineGroupID getGroup();

			int getEntryNumber();

			int getStartOKNumber();

			bool getEventMode();

			int getMyIndex();

			long getPingToHostFrame();

			PartyMemberPing getPartyMemberPing();
		}

		private class Manager : StateMachine<Manager, PartyPartyManagerStateID>, IManager, IDisposable
		{
			private InitParam _initParam;

			private Host _host;

			private Client _client;

			private string _message;

			private MachineGroupID _groupID;

			private bool _alreadyDisposed;

			private const int MockID = -1;

			private Manager()
			{
			}

			private Manager(Manager other)
			{
			}

			public Manager(InitParam param)
				: this(param, 0)
			{
			}

			public Manager(InitParam param, int mockID)
			{
				_initParam = new InitParam(param);
				_host = new Host("Party::Host::", _initParam, mockID);
				_client = new Client("Party::Client::", _initParam, mockID);
				_message = "";
				_groupID = MachineGroupID.OFF;
				setCurrentStateID(PartyPartyManagerStateID.First);
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
						_initParam = null;
						GC.SuppressFinalize(this);
					}
					_alreadyDisposed = true;
				}
			}

			public void initialize()
			{
				terminate();
				_groupID = MachineGroupID.OFF;
				setCurrentStateID(PartyPartyManagerStateID.First);
			}

			public void terminate()
			{
				setCurrentStateID(PartyPartyManagerStateID.Finish);
			}

			public void execute()
			{
				if (!isError())
				{
					if (_host.isError())
					{
						error("Manager::execute Host error");
					}
					else if (_client.isError())
					{
						error("Manager::execute Client error");
					}
					updateState(-1f);
					_host.update();
					_client.update();
				}
				else if (Setting.get().isRetryEnable())
				{
					start(_groupID);
				}
			}

			public void start(MachineGroupID groupID)
			{
				_groupID = groupID;
				setCurrentStateID(PartyPartyManagerStateID.Setup);
			}

			public void wait()
			{
				if (!isError())
				{
					setCurrentStateID(PartyPartyManagerStateID.Wait);
				}
			}

			public void selectMusic()
			{
				if (!isError() && getCurrentState() != PartyPartyManagerStateID.SelectMusic)
				{
					setCurrentStateID(PartyPartyManagerStateID.SelectMusic);
				}
			}

			public bool startRecruit(UserInfo userInfo, RecruitStance stanceID)
			{
				if (isError())
				{
					return false;
				}
				_host.startRecruit(userInfo, stanceID);
				_client.requestJoin(Util.LoopbackAddress(-1).ToNetworkByteOrderU32(), userInfo);
				setCurrentStateID(PartyPartyManagerStateID.Host);
				return true;
			}

			private void cancelRecruit()
			{
				if (!isError())
				{
					_host.cancelRecruit();
					setCurrentStateID(PartyPartyManagerStateID.SelectMusic);
				}
			}

			public void finishRecruit()
			{
				if (!isError() && isHost())
				{
					_host.finishRecruit();
				}
			}

			public void battleStart()
			{
				if (isHost())
				{
					finishRecruit();
					if (1 >= getStartOKNumber())
					{
						wait();
					}
				}
			}

			public void ready()
			{
				if (!isError() && isJoin())
				{
					if (isHost())
					{
						_host.ready();
					}
					_client.ready();
				}
			}

			public void beginPlay()
			{
				if (!isError() && isJoin())
				{
					if (isHost())
					{
						_host.beginPlay();
					}
					_client.beginPlay();
				}
			}

			public void finishPlay()
			{
				if (!isError() && isJoin())
				{
					_client.finishPlay();
				}
			}

			public void finishNews(bool gaugeClear, uint gaugeStockNum)
			{
				_client.finishNews(gaugeClear, gaugeStockNum);
			}

			public bool finishSetting()
			{
				if (_client.getCurrentStateID() == PartyPartyClientStateID.Joined)
				{
					_client.setCurrentStateID(PartyPartyClientStateID.FinishSetting);
					return true;
				}
				return false;
			}

			public bool backToSetting()
			{
				if (_client.getCurrentStateID() == PartyPartyClientStateID.FinishSetting)
				{
					_client.setCurrentStateID(PartyPartyClientStateID.Joined);
					return true;
				}
				return false;
			}

			public bool startJoin(IpAddress hostAddress, UserInfo userInfo)
			{
				if (isError())
				{
					return false;
				}
				if (getCurrentStateID() != PartyPartyManagerStateID.SelectMusic)
				{
					return false;
				}
				_client.requestJoin(hostAddress.ToNetworkByteOrderU32(), userInfo);
				setCurrentStateID(PartyPartyManagerStateID.Client);
				return true;
			}

			private void cancelJoin()
			{
				if (!isError())
				{
					_client.cancelJoin();
					setCurrentStateID(PartyPartyManagerStateID.SelectMusic);
				}
			}

			public void cancelBothRecruitJoin()
			{
				if (isHost())
				{
					cancelRecruit();
				}
				else
				{
					cancelJoin();
				}
			}

			public void sendUserInfo(UserInfo info)
			{
				if (!isError())
				{
					if (isHost())
					{
						_host.updateRecruitUserInfo(info);
					}
					_client.sendUserInfo(info);
				}
			}

			public void sendClientPlayInfo(ClientPlayInfo info)
			{
				if (!isError())
				{
					_client.sendClientPlayInfo(info);
				}
			}

			public ClientPlayInfo getLastSendClientPlayInfo()
			{
				return _client.getLastSendClientPlayInfo();
			}

			public void clear()
			{
				_client.terminate();
				_host.terminate();
			}

			public void error(string message)
			{
				_message = message;
				setCurrentStateID(PartyPartyManagerStateID.Error);
			}

			public RecruitList getRecruitList()
			{
				return _client.getRecruitList();
			}

			public RecruitList getRecruitListForMusicList()
			{
				return getRecruitListWithoutMe();
			}

			public RecruitList getRecruitListWithoutMe()
			{
				return _client.getRecruitListWithoutMe();
			}

			public ushort getPortNumber()
			{
				return _initParam._portNumber;
			}

			public bool isHost()
			{
				return getCurrentStateID().IsHost();
			}

			public bool isClient()
			{
				return getCurrentStateID().IsClient();
			}

			public bool isNormal()
			{
				return getCurrentStateID().IsNormal();
			}

			public bool isError()
			{
				return getCurrentStateID() == PartyPartyManagerStateID.Error;
			}

			public bool isJoin()
			{
				return _client.isJoin();
			}

			public bool isJoinAndActive()
			{
				if (_client.isJoin())
				{
					return !_client.isDisconnected();
				}
				return false;
			}

			public bool isPlay()
			{
				return _client.isPlay();
			}

			public bool isNews()
			{
				return _client.getCurrentStateID() == PartyPartyClientStateID.News;
			}

			public bool isResult()
			{
				return _client.getCurrentStateID() == PartyPartyClientStateID.Result;
			}

			public bool isClientJoinActive()
			{
				if (isJoinAndActive())
				{
					return isClient();
				}
				return false;
			}

			public bool isWaitSetting()
			{
				return _client.getPartyMemberMeState() == PartyPartyClientStateID.Joined;
			}

			public bool isWaitSettingAsClient()
			{
				if (isClient())
				{
					return isWaitSetting();
				}
				return false;
			}

			public bool isWaitSettingAsHost()
			{
				if (isHost())
				{
					return isWaitSetting();
				}
				return false;
			}

			public bool isClientDisconnected()
			{
				return _client.isDisconnected();
			}

			public KickBy getClientLastKickReason()
			{
				return _client.getLastKickReason();
			}

			public bool isClientToReady()
			{
				return _client.getCurrentStateID() == PartyPartyClientStateID.ToReady;
			}

			public bool isAllBeginPlay()
			{
				return _client.getCurrentStateID() == PartyPartyClientStateID.AllBeginPlay;
			}

			public MachineGroupID getGroup()
			{
				return _groupID;
			}

			public int getEntryNumber()
			{
				return _host.getEntryNumber();
			}

			public int getStartOKNumber()
			{
				int num = 0;
				PartyMemberInfo partyMemberInfo = getPartyMemberInfo();
				PartyMemberState partyMemberState = getPartyMemberState();
				if (partyMemberInfo == null || partyMemberState == null)
				{
					return 0;
				}
				for (int i = 0; i < partyMemberInfo._userInfo.Length; i++)
				{
					UserInfo obj = partyMemberInfo._userInfo[i];
					PartyPartyClientStateID state = partyMemberState.GetState(i);
					if (obj._isJoin && state.IsWaitPlay())
					{
						num++;
					}
				}
				return num;
			}

			public void info(ref string os)
			{
				os = string.Concat(os, "Ip ", Util.MyIpAddress(-1), " ");
				os = string.Concat(os, "Group ", getGroup(), "\n");
				os = os + "EventMode " + getEventMode() + "\n";
				os = os + "Mes " + _message + "\n";
				os = os + "State " + base.stateString + "\n";
				os = os + "isNormal " + isNormal() + " ";
				os = os + "isHost" + isHost() + " ";
				os = os + "isClient" + isClient() + " ";
				os = os + "isJoin" + isJoin() + " ";
				os = os + "isPlay" + isPlay() + " ";
				os = os + "isError" + isError() + "\n";
				os = os + "HostMemberEntryNumber " + getEntryNumber() + "\n";
				for (int i = 0; i < getEntryNumber(); i++)
				{
					os = string.Concat(os, "Member ", i, " ", getMember(i).getUserInfo(), "\n");
				}
				os += "\n---Host\n";
				_host.info(ref os);
				os += "\n---Client\n";
				_client.info(ref os);
			}

			public uint getClientRecvCount(Command command)
			{
				return _client.getRecvCount(command);
			}

			public uint getHostRecvCount(int no, Command command)
			{
				return _host.getRecvCount(no, command);
			}

			public PartyPartyManagerStateID getCurrentStateID()
			{
				return getCurrentState();
			}

			public PartyPartyClientStateID getClientStateID()
			{
				return _client.getCurrentStateID();
			}

			public PartyPartyHostStateID getHostStateID()
			{
				return _host.getCurrentStateID();
			}

			public PartyMemberState getPartyMemberState()
			{
				return _client.getPartyMemberState();
			}

			public PartyMemberInfo getPartyMemberInfo()
			{
				return _client.getPartyMemberInfo();
			}

			public PartyPlayInfo getPartyPlayInfo()
			{
				return _client.getPartyPlayInfo();
			}

			public IMember getMember(int no)
			{
				return _host.getMemberList()[no];
			}

			public bool getEventMode()
			{
				return SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.IsEventMode;
			}

			public int getMyIndex()
			{
				PartyMemberInfo partyMemberInfo = getPartyMemberInfo();
				uint num = Util.MyIpAddress(-1).ToNetworkByteOrderU32();
				for (int i = 0; i < c_maxMember; i++)
				{
					if (partyMemberInfo._userInfo[i]._ipAddress == num)
					{
						return i;
					}
				}
				return -1;
			}

			public long getPingToHostFrame()
			{
				return _client.getPingFrame();
			}

			public PartyMemberPing getPartyMemberPing()
			{
				return _host.getPartyMemberPing();
			}

			public void Enter_First()
			{
				_client.initialize();
				_host.initialize();
			}

			public void Execute_First()
			{
			}

			public void Enter_Setup()
			{
				_client.start();
				_host.start();
			}

			public void Execute_Setup()
			{
				if (getGroup() == MachineGroupID.OFF)
				{
					error("Group OFF");
				}
				else if (_host.isNormal() && _client.isNormal())
				{
					setCurrentStateID(PartyPartyManagerStateID.Wait);
				}
			}

			public void Enter_Wait()
			{
				_host.wait();
				_client.wait();
			}

			public void Execute_Wait()
			{
			}

			public void Enter_SelectMusic()
			{
				_host.selectMusic();
				_client.selectMusic();
			}

			public void Execute_SelectMusic()
			{
			}

			public void Enter_Client()
			{
				_client.getCurrentStateID();
				_ = 3;
			}

			public void Execute_Client()
			{
			}

			public void Execute_Host()
			{
			}

			public void Enter_Finish()
			{
				clear();
			}

			public void Execute_Finish()
			{
			}

			public void Enter_Error()
			{
				clear();
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

			private void setCurrentStateID(PartyPartyManagerStateID nextState)
			{
				setNextState(nextState);
				updateState(-1f);
			}
		}

		public static readonly ushort CPortNumber = 50100;

		private static readonly int c_connectTimeOut = 60;

		public static readonly int c_maxMecha = 2;

		public static readonly int c_maxMember = 4;

		public static readonly float c_recruitRefresh = 5f;

		public static readonly uint c_recruitTimeout = 10u;

		public static readonly int c_heartBeatInterval = 3;

		public static readonly int c_heartBeatTimeout = 6;

		public static readonly int c_chainHistorySize = 10;

		public static readonly int c_clientSendInfoIntervalMSec = 250;

		private static Manager s_pMgr = null;

		public static void getRank(int[] scoreList, byte[] outRankList)
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

		public static void createManager(InitParam param)
		{
			Util.SafeDispose(ref s_pMgr);
			s_pMgr = new Manager(param);
		}

		public static void destroyManager()
		{
			Util.SafeDispose(ref s_pMgr);
		}

		public static IManager get()
		{
			return s_pMgr;
		}

		public static bool isExist()
		{
			return s_pMgr != null;
		}
	}
}
