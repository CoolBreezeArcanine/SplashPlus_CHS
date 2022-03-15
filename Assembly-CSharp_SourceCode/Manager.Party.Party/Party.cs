using System;
using System.Diagnostics;
using DB;
using MAI2;
using MAI2.Util;
using PartyLink;
using UnityEngine;

namespace Manager.Party.Party
{
	public class Party
	{
		private class Manager : StateMachine<Manager, PartyPartyManagerStateID>, IManager, IDisposable
		{
			private PartyLink.Party.InitParam _initParam;

			private Host _host;

			private Client _client;

			private string _message;

			private MachineGroupID _groupID;

			private bool _alreadyDisposed;

			private const int MockID = -1;

			public Manager(PartyLink.Party.InitParam param)
				: this(param, 0)
			{
			}

			private Manager(PartyLink.Party.InitParam param, int mockID)
			{
				_initParam = new PartyLink.Party.InitParam(param);
				_host = new Host("Party::Host::");
				_client = new Client("Party::Client::", _initParam);
				_message = "";
				_groupID = MachineGroupID.OFF;
				SetCurrentStateID(PartyPartyManagerStateID.First);
			}

			~Manager()
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
						PartyLink.Util.SafeDispose(ref _host);
						PartyLink.Util.SafeDispose(ref _client);
						_initParam = null;
						GC.SuppressFinalize(this);
					}
					_alreadyDisposed = true;
				}
			}

			public void Initialize()
			{
				Terminate();
				_groupID = MachineGroupID.OFF;
				SetCurrentStateID(PartyPartyManagerStateID.First);
			}

			public void Execute()
			{
				if (!IsError())
				{
					if (_host.IsError())
					{
						Error("Manager::execute Host error");
					}
					else if (_client.IsError())
					{
						Error("Manager::execute Client error");
					}
					updateState(-1f);
					_host.Update();
					_client.Update();
				}
				else if (Setting.get().isRetryEnable())
				{
					Start(_groupID);
				}
			}

			public void Terminate()
			{
				SetCurrentStateID(PartyPartyManagerStateID.Finish);
			}

			public void Start(MachineGroupID groupID)
			{
				_groupID = groupID;
				SetCurrentStateID(PartyPartyManagerStateID.Setup);
			}

			public void Wait()
			{
				if (!IsError())
				{
					SetCurrentStateID(PartyPartyManagerStateID.Wait);
				}
			}

			public void SelectMusic()
			{
				if (!IsError() && getCurrentState() != PartyPartyManagerStateID.SelectMusic)
				{
					SetCurrentStateID(PartyPartyManagerStateID.SelectMusic);
				}
			}

			public bool StartRecruit(MechaInfo mechaInfo, RecruitStance stanceID)
			{
				if (IsError())
				{
					return false;
				}
				_host.StartRecruit(mechaInfo, stanceID);
				_client.RequestJoin(PartyLink.Util.LoopbackAddress(-1).ToNetworkByteOrderU32(), mechaInfo);
				SetCurrentStateID(PartyPartyManagerStateID.Host);
				return true;
			}

			private void CancelRecruit()
			{
				if (!IsError())
				{
					_host.CancelRecruit();
					SetCurrentStateID(PartyPartyManagerStateID.SelectMusic);
				}
			}

			public void FinishRecruit()
			{
				if (!IsError() && IsHost())
				{
					_host.FinishRecruit();
				}
			}

			public void GoToTrackStart()
			{
				if (IsHost())
				{
					FinishRecruit();
					if (1 >= GetEntryNumber())
					{
						Wait();
					}
				}
			}

			public bool StartJoin(IpAddress hostAddress, MechaInfo mechaInfo)
			{
				if (IsError())
				{
					return false;
				}
				if (GetCurrentStateID() != PartyPartyManagerStateID.SelectMusic)
				{
					return false;
				}
				_client.RequestJoin(hostAddress.ToNetworkByteOrderU32(), mechaInfo);
				SetCurrentStateID(PartyPartyManagerStateID.Client);
				return true;
			}

			private void CancelJoin()
			{
				if (!IsError())
				{
					_client.CancelJoin();
					SetCurrentStateID(PartyPartyManagerStateID.SelectMusic);
				}
			}

			public void SendMechaInfo(MechaInfo info)
			{
				if (!IsError())
				{
					if (IsHost())
					{
						_host.UpdateRecruitMechaInfo(info);
					}
					_client.SendMechaInfo(info);
				}
			}

			public void CancelBothRecruitJoin()
			{
				if (IsHost())
				{
					CancelRecruit();
				}
				else
				{
					CancelJoin();
				}
			}

			public bool FinishSetting()
			{
				if (_client.GetCurrentStateID() == PartyPartyClientStateID.Joined)
				{
					_client.SetCurrentStateID(PartyPartyClientStateID.FinishSetting);
					return true;
				}
				return false;
			}

			public bool BackToSetting()
			{
				if (_client.GetCurrentStateID() == PartyPartyClientStateID.FinishSetting)
				{
					_client.SetCurrentStateID(PartyPartyClientStateID.Joined);
					return true;
				}
				return false;
			}

			public void BeginPlay()
			{
				if (!IsError() && IsJoin())
				{
					if (IsHost())
					{
						_host.BeginPlay();
					}
					_client.BeginPlay();
				}
			}

			public void Ready()
			{
				if (!IsError() && IsJoin())
				{
					if (IsHost())
					{
						_host.Ready();
					}
					_client.Ready();
				}
			}

			public void FinishPlay()
			{
				if (!IsError() && IsJoin())
				{
					_client.FinishPlay();
				}
			}

			public void FinishNews(bool gaugeClear0, uint gaugeStockNum0, bool gaugeClear1, uint gaugeStockNum1)
			{
				_client.FinishNews(gaugeClear0, gaugeStockNum0, gaugeClear1, gaugeStockNum1);
			}

			public void SendClientPlayInfo(ClientPlayInfo info)
			{
				if (!IsError())
				{
					_client.SendClientPlayInfo(info);
				}
			}

			public ClientPlayInfo GetLastSendClientPlayInfo()
			{
				return _client.GetLastSendClientPlayInfo();
			}

			public PartyPlayInfo GetPartyPlayInfo()
			{
				return _client.GetPartyPlayInfo();
			}

			public RecruitList GetRecruitList()
			{
				return _client.GetRecruitList();
			}

			public RecruitList GetRecruitListForMusicList()
			{
				return GetRecruitListWithoutMe();
			}

			public RecruitList GetRecruitListWithoutMe()
			{
				return _client.GetRecruitListWithoutMe();
			}

			public ushort GetPortNumber()
			{
				return _initParam._portNumber;
			}

			public bool IsHost()
			{
				return GetCurrentStateID().IsHost();
			}

			public bool IsClient()
			{
				return GetCurrentStateID().IsClient();
			}

			public bool IsNormal()
			{
				return GetCurrentStateID().IsNormal();
			}

			public bool IsJoin()
			{
				return _client.IsJoin();
			}

			public bool IsRequest()
			{
				return _client.IsRequest();
			}

			public bool IsConnect()
			{
				return _client.IsConnect();
			}

			public bool IsJoinAndActive()
			{
				if (_client.IsJoin())
				{
					return !_client.IsDisconnected();
				}
				return false;
			}

			public bool IsError()
			{
				return GetCurrentStateID() == PartyPartyManagerStateID.Error;
			}

			public bool IsClientJoinActive()
			{
				if (IsJoinAndActive())
				{
					return IsClient();
				}
				return false;
			}

			public bool IsWaitSetting()
			{
				return _client.GetPartyMemberMeState() == PartyPartyClientStateID.Joined;
			}

			public bool IsWaitSettingAsClient()
			{
				if (IsClient())
				{
					return IsWaitSetting();
				}
				return false;
			}

			public bool IsWaitSettingAsHost()
			{
				if (IsHost())
				{
					return IsWaitSetting();
				}
				return false;
			}

			public bool IsClientDisconnected()
			{
				return _client.IsDisconnected();
			}

			public KickBy GetClientLastKickReason()
			{
				return _client.GetLastKickReason();
			}

			public bool IsClientToReady()
			{
				return _client.GetCurrentStateID() == PartyPartyClientStateID.ToReady;
			}

			public PartyMemberInfo GetPartyMemberInfo()
			{
				return _client.GetPartyMemberInfo();
			}

			public PartyMemberState GetPartyMemberState()
			{
				return _client.GetPartyMemberState();
			}

			private IMember GetMember(int no)
			{
				return _host.GetMemberList()[no];
			}

			public bool IsAllBeginPlay()
			{
				return _client.GetCurrentStateID() == PartyPartyClientStateID.AllBeginPlay;
			}

			public bool IsPlay()
			{
				return _client.IsPlay();
			}

			public bool IsNews()
			{
				return _client.GetCurrentStateID() == PartyPartyClientStateID.News;
			}

			public bool IsResult()
			{
				return _client.GetCurrentStateID() == PartyPartyClientStateID.Result;
			}

			public void Info(ref string os)
			{
				os += $"Ip {PartyLink.Util.MyIpAddress(-1)} ";
				os += $"Group {GetGroup()}\n";
				os += $"EventMode {GetEventMode()}\n";
				os = os + "Mes " + _message + "\n";
				os = os + "State " + base.stateString + "\n";
				os += $"isNormal{IsNormal()} ";
				os += $"isHost{IsHost()} ";
				os += $"isClient{IsClient()} ";
				os += $"isJoin{IsJoin()} ";
				os += $"isPlay{IsPlay()} ";
				os += $"isError{IsError()}\n";
				os += $"HostMemberEntryNumber {GetEntryNumber()}\n";
				for (int i = 0; i < GetEntryNumber(); i++)
				{
					os += $"Member {i} {GetMember(i).GetMechaInfo()}\n";
				}
				os += "\n---Host\n";
				_host.Info(ref os);
				os += "\n---Client\n";
				_client.Info(ref os);
			}

			public PartyPartyManagerStateID GetCurrentStateID()
			{
				return getCurrentState();
			}

			public uint GetClientRecvCount(Command command)
			{
				return _client.GetRecvCount(command);
			}

			public uint GetHostRecvCount(int no, Command command)
			{
				return _host.GetRecvCount(no, command);
			}

			public PartyPartyClientStateID GetClientStateID()
			{
				return _client.GetCurrentStateID();
			}

			public PartyPartyHostStateID GetHostStateID()
			{
				return _host.GetCurrentStateID();
			}

			public MachineGroupID GetGroup()
			{
				return _groupID;
			}

			public int GetEntryNumber()
			{
				return _host.GetEntryNumber();
			}

			public int GetStartOkNumber()
			{
				int num = 0;
				PartyMemberInfo partyMemberInfo = GetPartyMemberInfo();
				PartyMemberState partyMemberState = GetPartyMemberState();
				if (partyMemberInfo == null || partyMemberState == null)
				{
					return 0;
				}
				for (int i = 0; i < partyMemberInfo.MechaInfo.Length; i++)
				{
					MechaInfo obj = partyMemberInfo.MechaInfo[i];
					PartyPartyClientStateID state = partyMemberState.GetState(i);
					if (obj.IsJoin && state.IsWaitPlay())
					{
						num++;
					}
				}
				return num;
			}

			public bool GetEventMode()
			{
				return SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.IsEventMode;
			}

			public int GetMyIndex()
			{
				PartyMemberInfo partyMemberInfo = GetPartyMemberInfo();
				uint num = PartyLink.Util.MyIpAddress(-1).ToNetworkByteOrderU32();
				for (int i = 0; i < partyMemberInfo.MechaInfo.Length; i++)
				{
					if (partyMemberInfo.MechaInfo[i].IpAddress == num)
					{
						return i;
					}
				}
				return -1;
			}

			public long GetPingToHostFrame()
			{
				return _client.GetPingFrame();
			}

			public PartyMemberPing GetPartyMemberPing()
			{
				return _host.GetPartyMemberPing();
			}

			private void Clear()
			{
				_client.Terminate();
				_host.Terminate();
			}

			private void Error(string message)
			{
				_message = message;
				SetCurrentStateID(PartyPartyManagerStateID.Error);
			}

			public void Enter_First()
			{
				_client.Initialize();
				_host.Initialize();
			}

			public void Execute_First()
			{
			}

			public void Enter_Setup()
			{
				_client.Start();
				_host.Start();
			}

			public void Execute_Setup()
			{
				if (GetGroup() == MachineGroupID.OFF)
				{
					Error("Group OFF");
				}
				else if (_host.IsNormal() && _client.IsNormal())
				{
					SetCurrentStateID(PartyPartyManagerStateID.Wait);
				}
			}

			public void Enter_Wait()
			{
				_host.Wait();
				_client.Wait();
			}

			public void Execute_Wait()
			{
			}

			public void Enter_SelectMusic()
			{
				_host.SelectMusic();
				_client.SelectMusic();
			}

			public void Execute_SelectMusic()
			{
			}

			public void Enter_Client()
			{
				_client.GetCurrentStateID();
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

			private void SetCurrentStateID(PartyPartyManagerStateID nextState)
			{
				setNextState(nextState);
				updateState(-1f);
			}
		}

		public const int ConnectTimeOut = 60;

		private static Manager _manager;

		public static void CreateManager(PartyLink.Party.InitParam param)
		{
			PartyLink.Util.SafeDispose(ref _manager);
			_manager = new Manager(param);
		}

		public static void DestroyManager()
		{
			PartyLink.Util.SafeDispose(ref _manager);
			_manager = null;
		}

		public static IManager Get()
		{
			return _manager;
		}

		public static bool IsExist()
		{
			return _manager != null;
		}

		[Conditional("_NET_DEBUG")]
		public static void Log(string message)
		{
			UnityEngine.Debug.Log(message);
		}

		[Conditional("_NET_DEBUG")]
		public static void LogWarning(string message)
		{
			UnityEngine.Debug.LogWarning(message);
		}

		[Conditional("_NET_DEBUG")]
		public static void LogError(string message)
		{
			UnityEngine.Debug.LogError(message);
		}
	}
}
