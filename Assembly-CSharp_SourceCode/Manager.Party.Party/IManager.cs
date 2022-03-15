using DB;
using PartyLink;

namespace Manager.Party.Party
{
	public interface IManager
	{
		void Initialize();

		void Execute();

		void Terminate();

		void Start(MachineGroupID groupID);

		void Wait();

		void SelectMusic();

		bool StartRecruit(MechaInfo mechaInfo, RecruitStance stanceID);

		void FinishRecruit();

		void GoToTrackStart();

		bool StartJoin(IpAddress hostAddress, MechaInfo mechaInfo);

		void SendMechaInfo(MechaInfo mechaInfo);

		void CancelBothRecruitJoin();

		bool FinishSetting();

		bool BackToSetting();

		void BeginPlay();

		void Ready();

		void FinishPlay();

		void FinishNews(bool gaugeClear0, uint gaugeStockNum0, bool gaugeClear1, uint gaugeStockNum1);

		void SendClientPlayInfo(ClientPlayInfo info);

		ClientPlayInfo GetLastSendClientPlayInfo();

		PartyPlayInfo GetPartyPlayInfo();

		RecruitList GetRecruitList();

		RecruitList GetRecruitListForMusicList();

		RecruitList GetRecruitListWithoutMe();

		ushort GetPortNumber();

		bool IsHost();

		bool IsClient();

		bool IsNormal();

		bool IsJoin();

		bool IsRequest();

		bool IsConnect();

		bool IsJoinAndActive();

		bool IsError();

		bool IsClientJoinActive();

		bool IsWaitSetting();

		bool IsWaitSettingAsClient();

		bool IsWaitSettingAsHost();

		bool IsClientDisconnected();

		KickBy GetClientLastKickReason();

		bool IsClientToReady();

		PartyMemberInfo GetPartyMemberInfo();

		PartyMemberState GetPartyMemberState();

		bool IsAllBeginPlay();

		bool IsPlay();

		bool IsNews();

		bool IsResult();

		void Info(ref string os);

		PartyPartyManagerStateID GetCurrentStateID();

		uint GetClientRecvCount(Command command);

		uint GetHostRecvCount(int no, Command command);

		PartyPartyClientStateID GetClientStateID();

		PartyPartyHostStateID GetHostStateID();

		MachineGroupID GetGroup();

		int GetEntryNumber();

		int GetStartOkNumber();

		bool GetEventMode();

		int GetMyIndex();

		long GetPingToHostFrame();

		PartyMemberPing GetPartyMemberPing();
	}
}
