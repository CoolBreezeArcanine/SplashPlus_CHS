using System.Collections.Generic;
using System.Linq;
using Datas.DebugData;
using DB;
using MAI2.Util;
using Manager.Party.Party;
using Manager.UserDatas;

namespace Manager
{
	public class GamePlayManager : Singleton<GamePlayManager>
	{
		private class ScoreLogClass
		{
			public bool IsGhostVs;

			public bool IsPartyHost;

			public readonly GameScoreList[] _score = new GameScoreList[4]
			{
				new GameScoreList(0),
				new GameScoreList(1),
				new GameScoreList(2),
				new GameScoreList(3)
			};
		}

		private const long TrackSkipTime = 3000L;

		public const GameManager.PlayerID GhostListIndex = GameManager.PlayerID.OtherLeft;

		private List<ScoreLogClass> _scoreLists = new List<ScoreLogClass>();

		private uint _chainCounter;

		private int[] _nowAchiveDiff = new int[4];

		private int[] _nowRank = new int[4];

		private readonly DebugGameScoreList[] _debugGameScoreLists = new DebugGameScoreList[4];

		public int GetScoreListCount()
		{
			return _scoreLists.Count;
		}

		public bool IsEmpty()
		{
			return GetScoreListCount() == 0;
		}

		public GameScoreList[] GetGameScores(int index)
		{
			return _scoreLists.Select((ScoreLogClass i) => i._score[index]).ToArray();
		}

		public GameScoreList[] GetGameScoresAllMember(int trackNo)
		{
			return _scoreLists[trackNo]._score;
		}

		public void SetTrackSkipFrag(int index, int trackNo = -1)
		{
			GetGameScore(index, trackNo).SetTrackSkip();
		}

		public int GetPlayerNum(int trackNo = -1)
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				if (GetGameScore(i, trackNo).IsEnable)
				{
					num++;
				}
			}
			return num;
		}

		public int GetPlayerIgnoreNpcNum(int trackNo = -1)
		{
			int num = 0;
			for (int i = 0; i < 4; i++)
			{
				if (GetGameScore(i, trackNo).IsEnable && GetGameScore(i, trackNo).IsHuman())
				{
					num++;
				}
			}
			return num;
		}

		public void PlayHeadUpdate()
		{
		}

		public void PlayLastUpdate()
		{
			for (int i = 0; i < 4; i++)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
				{
					GetGameScore(i).CalcUpdate();
				}
			}
			IManager manager = Manager.Party.Party.Party.Get();
			if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.MachineGroupID != MachineGroupID.OFF && manager != null && manager.IsJoinAndActive())
			{
				foreach (MemberPlayInfo joinMember in manager.GetPartyPlayInfo().GetJoinMembers())
				{
					for (int j = 0; j < 2; j++)
					{
						int num = (joinMember.IsMe() ? j : (j + 2));
						if (Singleton<UserDataManager>.Instance.GetUserData(num).IsEntry)
						{
							if (!GetGameScore(num).IsTrackSkip)
							{
								GetGameScore(num).SetChain((uint)manager.GetPartyPlayInfo().Chain);
							}
							GetGameScore(num).VsRank = joinMember.Rankings[j];
						}
					}
				}
			}
			else
			{
				for (int k = 0; k < 4; k++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(k).IsEntry && Singleton<UserDataManager>.Instance.GetUserData(k).IsHuman())
					{
						if (GetGameScore(k).ResetCombo)
						{
							_chainCounter = 0u;
							break;
						}
						_chainCounter += GetGameScore(k).ComboAddThisFrame;
					}
				}
				for (int l = 0; l < 4; l++)
				{
					if (!Singleton<UserDataManager>.Instance.GetUserData(l).IsEntry || !Singleton<UserDataManager>.Instance.GetUserData(l).IsHuman())
					{
						continue;
					}
					GetGameScore(l).SetChain(_chainCounter);
					uint num2 = 1u;
					for (int m = 0; m < 4; m++)
					{
						if (Singleton<UserDataManager>.Instance.GetUserData(m).IsEntry && Singleton<UserDataManager>.Instance.GetUserData(m).IsHuman() && l != m && GetGameScore(l).SessionInfo.musicId == GetGameScore(m).SessionInfo.musicId && GameManager.ConvAchiveDecimalToInt(GetGameScore(m).GetAchivement()) > GameManager.ConvAchiveDecimalToInt(GetGameScore(l).GetAchivement()))
						{
							num2++;
						}
					}
					GetGameScore(l).VsRank = num2;
				}
			}
			GameScoreList gameScore = GetGameScore(2);
			if (gameScore.IsEnable && !gameScore.IsHuman())
			{
				NoteDataList noteList = NotesManager.Instance(2).getReader().GetNoteList();
				UserGhost myGhost = Singleton<UserDataManager>.Instance.GetUserData(2L).MyGhost;
				foreach (NoteData item in noteList)
				{
					if (item != null)
					{
						NoteScore.EScoreType kind = NoteType2ScoreType(item.type.getEnum());
						float msec = item.end.msec;
						if (NotesManager.GetCurrentMsec() >= msec && !item.isUsed)
						{
							item.isUsed = true;
							GetGameScore(2).SetResult(item.indexNote, kind, (NoteJudge.ETiming)myGhost.GetResultIndexTo(item.indexNote));
						}
					}
				}
				for (int n = 0; n < 2; n++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(n).IsEntry)
					{
						_nowAchiveDiff[n] = GameManager.ConvAchiveDecimalToInt(GetGameScore(n).GetAchivement()) - GameManager.ConvAchiveDecimalToInt(GetGameScore(2).GetAchivement());
					}
				}
			}
			for (int num3 = 0; num3 < 4; num3++)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(num3).IsEntry)
				{
					GetGameScore(num3).UpdateAfter();
				}
			}
		}

		public GameScoreList GetGameScore(int index, int trackNo = -1)
		{
			trackNo = ((trackNo != -1) ? trackNo : ((int)(GameManager.MusicTrackNumber - 1)));
			return _scoreLists[trackNo]._score[index];
		}

		public void SetGhostFlag(int trackNo = -1)
		{
			trackNo = ((trackNo != -1) ? trackNo : ((int)(GameManager.MusicTrackNumber - 1)));
			_scoreLists[trackNo].IsGhostVs = true;
		}

		public void SetPartyHostFlag(bool isHost, int trackNo = -1)
		{
			trackNo = ((trackNo != -1) ? trackNo : ((int)(GameManager.MusicTrackNumber - 1)));
			_scoreLists[trackNo].IsPartyHost = isHost;
		}

		public bool IsGhostFlag(int trackNo = -1)
		{
			trackNo = ((trackNo != -1) ? trackNo : ((int)(GameManager.MusicTrackNumber - 1)));
			return _scoreLists[trackNo].IsGhostVs;
		}

		public bool IsPartyHostFlag(int trackNo = -1)
		{
			trackNo = ((trackNo != -1) ? trackNo : ((int)(GameManager.MusicTrackNumber - 1)));
			return _scoreLists[trackNo].IsPartyHost;
		}

		public DebugGameScoreList GetDebugGameScore(int index)
		{
			return _debugGameScoreLists[index];
		}

		public void DebugSetGameScore(int index, DebugGameScoreList scoreList)
		{
			_debugGameScoreLists[index] = scoreList;
		}

		public void DebugSetGameScore(int index, GameScoreList score, int trackNo = -1)
		{
			trackNo = ((trackNo != -1) ? trackNo : ((int)(GameManager.MusicTrackNumber - 1)));
			_scoreLists[trackNo]._score[index] = score;
		}

		public PlayComboflagID GetComboType(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return PlayComboflagID.None;
			}
			return GetGameScore(monitorIndex, trackNo).ComboType;
		}

		public uint GetCombo(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return 0u;
			}
			return GetGameScore(monitorIndex, trackNo).Combo;
		}

		public uint GetMaxCombo(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return 0u;
			}
			return GetGameScore(monitorIndex, trackNo).MaxCombo;
		}

		public PlaySyncflagID GetSyncType(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return PlaySyncflagID.None;
			}
			return GetGameScore(monitorIndex, trackNo).SyncType;
		}

		public uint GetChainCount(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return 0u;
			}
			return GetGameScore(monitorIndex, trackNo).Chain;
		}

		public uint GetMaxChainCount(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return 0u;
			}
			return GetGameScore(monitorIndex, trackNo).MaxChain;
		}

		public int GetGhostVsDiff(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return 1;
			}
			return _nowAchiveDiff[monitorIndex];
		}

		public uint GetVsRank(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return 1u;
			}
			if (GetGameScore(monitorIndex, trackNo).VsRank != 0)
			{
				return GetGameScore(monitorIndex, trackNo).VsRank - 1;
			}
			return 0u;
		}

		public decimal GetVsAchieve(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return 1m;
			}
			decimal achivement = GetGameScore(monitorIndex, trackNo).GetAchivement();
			decimal num = default(decimal);
			for (int i = 0; i < 4; i++)
			{
				if (monitorIndex == i || !GetGameScore(i, trackNo).IsHuman())
				{
					continue;
				}
				if (num < GetGameScore(i, trackNo).GetAchivement())
				{
					if (num <= achivement)
					{
						num = GetGameScore(i, trackNo).GetAchivement();
					}
				}
				else if (GetGameScore(i, trackNo).GetAchivement() > achivement)
				{
					num = GetGameScore(i, trackNo).GetAchivement();
				}
			}
			int num2 = GameManager.ConvAchiveDecimalToInt(achivement);
			int num3 = GameManager.ConvAchiveDecimalToInt(num);
			return GameManager.ConvAchiveIntToDecimal(num2 - num3);
		}

		public decimal GetGhostAchieve(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return 1m;
			}
			decimal achivement = GetGameScore(monitorIndex, trackNo).GetAchivement();
			decimal num = default(decimal);
			for (int i = 0; i < 4; i++)
			{
				if (monitorIndex != i && !GetGameScore(i, trackNo).IsHuman() && num < GetGameScore(i, trackNo).GetAchivement())
				{
					num = GetGameScore(i, trackNo).GetAchivement();
				}
			}
			return achivement - num;
		}

		public void SetSyncResult(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return;
			}
			PlaySyncflagID playSyncflagID = PlaySyncflagID.None;
			PlayComboflagID comboType = GetGameScore(monitorIndex, trackNo).ComboType;
			if (comboType != 0)
			{
				int difficulty = GetGameScore(monitorIndex, trackNo).SessionInfo.difficulty;
				int musicId = GetGameScore(monitorIndex, trackNo).SessionInfo.musicId;
				for (int i = 0; i < 4; i++)
				{
					GameScoreList gameScore = GetGameScore(i, trackNo);
					if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry && Singleton<UserDataManager>.Instance.GetUserData(i).IsHuman() && i != monitorIndex && musicId == gameScore.SessionInfo.musicId && gameScore.ComboType != 0)
					{
						PlaySyncflagID playSyncflagID2 = PlaySyncflagID.None;
						playSyncflagID2 = ((difficulty > gameScore.SessionInfo.difficulty) ? PlaySyncflagID.ChainLow : ((difficulty > gameScore.SessionInfo.difficulty) ? PlaySyncflagID.ChainHi : ((gameScore.ComboType >= PlayComboflagID.AllPerfect && comboType >= PlayComboflagID.AllPerfect) ? PlaySyncflagID.SyncHi : ((gameScore.ComboType < PlayComboflagID.Gold || comboType < PlayComboflagID.Gold) ? PlaySyncflagID.ChainHi : PlaySyncflagID.SyncLow))));
						playSyncflagID = ((playSyncflagID < playSyncflagID2) ? playSyncflagID2 : playSyncflagID);
					}
				}
			}
			GetGameScore(monitorIndex, trackNo).SyncType = playSyncflagID;
		}

		public bool GetVsGhostWin(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return false;
			}
			GameScoreList gameScore = GetGameScore(2, trackNo);
			if (!gameScore.IsEnable || gameScore.IsHuman())
			{
				return false;
			}
			return GetGameScore(monitorIndex, trackNo).GetAchivement() >= gameScore.GetAchivement();
		}

		public bool IsMapNpcWin(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return false;
			}
			GameScoreList gameScore = GetGameScore(2, trackNo);
			if (!gameScore.IsEnable || gameScore.UserType != UserData.UserIDType.Npc)
			{
				return false;
			}
			return GetGameScore(monitorIndex, trackNo).GetAchivement() >= gameScore.GetAchivement();
		}

		public bool IsMapNpcLose(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return false;
			}
			GameScoreList gameScore = GetGameScore(2, trackNo);
			if (!gameScore.IsEnable || gameScore.UserType != UserData.UserIDType.Npc)
			{
				return false;
			}
			return GetGameScore(monitorIndex, trackNo).GetAchivement() < gameScore.GetAchivement();
		}

		public uint GetDeluxeScore(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return 0u;
			}
			return GetGameScore(monitorIndex, trackNo).GetDeluxeScoreAll();
		}

		public uint GetAchivement(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return 0u;
			}
			return (uint)GameManager.ConvAchiveDecimalToInt(GetGameScore(monitorIndex, trackNo).GetAchivement());
		}

		public MusicClearrankID GetClearRank(int monitorIndex, int trackNo = -1)
		{
			return GameManager.GetClearRank((int)GetAchivement(monitorIndex, trackNo));
		}

		public uint GetDecAchivement(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return 0u;
			}
			return (uint)GameManager.ConvAchiveDecimalToInt(GetGameScore(monitorIndex, trackNo).GetDecAchivement());
		}

		public uint GetDecTheoryAchivement(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return 0u;
			}
			return (uint)GameManager.ConvAchiveDecimalToInt(GetGameScore(monitorIndex, trackNo).GetDecTheoryAchivement());
		}

		public uint GetGhostAchivement(int trackNo = -1)
		{
			GameScoreList gameScore = GetGameScore(2, trackNo);
			if (!gameScore.IsEnable || gameScore.IsHuman())
			{
				return 0u;
			}
			return (uint)GameManager.ConvAchiveDecimalToInt(GetGameScore(2, trackNo).GetAchivement());
		}

		public GameScoreList GetGhostScore(int trackNo = -1)
		{
			GameScoreList gameScore = GetGameScore(2, trackNo);
			if (!gameScore.IsEnable || gameScore.IsHuman())
			{
				return null;
			}
			return GetGameScore(2, trackNo);
		}

		public ulong GetTotalTheoryScore(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return 0uL;
			}
			return GetGameScore(monitorIndex, trackNo).ScoreTotal._totalData[45];
		}

		public ulong GetTotalNoteScore(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return 0uL;
			}
			return GetGameScore(monitorIndex, trackNo).ScoreTotal._totalData[43];
		}

		public ulong GetTotalBreakScore(int monitorIndex, int trackNo = -1)
		{
			if (!Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).IsEntry)
			{
				return 0uL;
			}
			return GetGameScore(monitorIndex, trackNo).ScoreTotal._totalData[44];
		}

		public void AddPlayLog()
		{
			_scoreLists.Add(new ScoreLogClass());
		}

		public void ClaerLog()
		{
			_scoreLists.Clear();
		}

		public bool IsPlayLog()
		{
			return _scoreLists.Count != 0;
		}

		public void Initialize(bool partyPlay = false)
		{
			int trackNo = -1;
			uint num = 0u;
			_chainCounter = 0u;
			for (int i = 0; i < 2; i++)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry && Singleton<UserDataManager>.Instance.GetUserData(i).IsHuman())
				{
					num += CreateResult(i, partyPlay);
				}
			}
			if (partyPlay)
			{
				IEnumerable<MechaInfo> joinMembersWithoutMe = Manager.Party.Party.Party.Get().GetPartyMemberInfo().GetJoinMembersWithoutMe();
				if (joinMembersWithoutMe.Any())
				{
					foreach (MechaInfo item in joinMembersWithoutMe)
					{
						for (int j = 0; j < 2; j++)
						{
							if (item.Entrys[j])
							{
								int num2 = 2 + j;
								GameManager.SelectMusicID[num2] = item.MusicID;
								GameManager.SelectDifficultyID[num2] = item.FumenDifs[j];
								Singleton<UserDataManager>.Instance.GetUserData(num2).PartyEntry((ulong)item.UserIDs[j], item.UserNames[j], item.IconIDs[j], item.Rateing[j], item.ClassValue[j], item.MaxClassValue[j]);
								num += CreateResult(num2, isParty: true);
							}
						}
					}
				}
				SetPartyHostFlag(Manager.Party.Party.Party.Get().IsHost());
			}
			else
			{
				SetPartyHostFlag(isHost: true);
			}
			for (int k = 0; k < 4; k++)
			{
				_nowAchiveDiff[k] = 0;
				_nowRank[k] = 0;
				GetGameScore(k, trackNo).TheoryChain = num;
			}
			for (int l = 0; l < 2; l++)
			{
				if (!Singleton<UserDataManager>.Instance.GetUserData(l).IsEntry || GameManager.SelectGhostID[l] == GhostManager.GhostTarget.End)
				{
					continue;
				}
				UserGhost ghostToEnum = Singleton<GhostManager>.Instance.GetGhostToEnum(GameManager.SelectGhostID[l]);
				GameManager.SelectMusicID[2] = GameManager.SelectMusicID[l];
				GameManager.SelectDifficultyID[2] = GameManager.SelectDifficultyID[l];
				Singleton<UserDataManager>.Instance.GetUserData(2L).GhostEntry(ghostToEnum);
				CreateResult(2, isParty: false);
				if (ghostToEnum.Type == UserGhost.GhostType.MapNpc)
				{
					GetGameScore(2, trackNo).SetForceAchivement_Battle((float)GameManager.ConvAchiveIntToDecimal(ghostToEnum.Achievement));
					Singleton<UserDataManager>.Instance.GetUserData(2L).CreateGhost(2);
					CreateResult(2, isParty: false);
				}
				else if (ghostToEnum.Type == UserGhost.GhostType.Player)
				{
					if (!GetGameScore(2, trackNo).CheckResultNum(ghostToEnum.ResultNum))
					{
						GetGameScore(2, trackNo).SetForceAchivement_Battle((float)GameManager.ConvAchiveIntToDecimal(ghostToEnum.Achievement));
					}
					else
					{
						GetGameScore(2, trackNo).SetGhostData(ghostToEnum);
					}
					Singleton<UserDataManager>.Instance.GetUserData(2L).CreateGhost(2);
					CreateResult(2, isParty: false);
				}
				else if (ghostToEnum.Type == UserGhost.GhostType.Boss)
				{
					GetGameScore(2, trackNo).SetForceAchivement_Battle((float)GameManager.ConvAchiveIntToDecimal(ghostToEnum.Achievement));
					Singleton<UserDataManager>.Instance.GetUserData(2L).CreateGhost(2);
					CreateResult(2, isParty: false);
				}
				SetGhostFlag();
				break;
			}
		}

		public void InitializeAdvertise()
		{
			for (int i = 0; i < 2; i++)
			{
				CreateResult(i, isParty: false);
			}
		}

		public static NoteScore.EScoreType NoteType2ScoreType(NoteTypeID.Def noteDef)
		{
			switch (noteDef)
			{
			case NoteTypeID.Def.Break:
			case NoteTypeID.Def.BreakStar:
				return NoteScore.EScoreType.Break;
			case NoteTypeID.Def.Slide:
				return NoteScore.EScoreType.Slide;
			case NoteTypeID.Def.Hold:
			case NoteTypeID.Def.TouchHold:
			case NoteTypeID.Def.ExHold:
				return NoteScore.EScoreType.Hold;
			case NoteTypeID.Def.Begin:
			case NoteTypeID.Def.Star:
			case NoteTypeID.Def.ExTap:
			case NoteTypeID.Def.ExStar:
				return NoteScore.EScoreType.Tap;
			case NoteTypeID.Def.TouchTap:
				return NoteScore.EScoreType.Touch;
			default:
				return NoteScore.EScoreType.End;
			}
		}

		private uint CreateResult(int index, bool isParty)
		{
			int trackNo = -1;
			GetGameScore(index, trackNo).Initialize(index, isParty);
			uint theoryCombo = GetGameScore(index, trackNo).TheoryCombo;
			foreach (NoteData note in NotesManager.Instance(index).getReader().GetNoteList())
			{
				JudgeResultSt scoreAt = GetGameScore(index, trackNo).GetScoreAt(note.indexNote);
				if (note.type == NoteTypeID.Def.Hold || note.type == NoteTypeID.Def.ExHold || note.type == NoteTypeID.Def.TouchHold)
				{
					scoreAt.Type = NoteScore.EScoreType.Hold;
				}
				else if (note.type == NoteTypeID.Def.Slide)
				{
					scoreAt.Type = NoteScore.EScoreType.Slide;
				}
				else if (note.type == NoteTypeID.Def.Break || note.type == NoteTypeID.Def.BreakStar)
				{
					scoreAt.Type = NoteScore.EScoreType.Break;
				}
				else
				{
					scoreAt.Type = NoteScore.EScoreType.Tap;
				}
				scoreAt.Timing = 0;
				scoreAt.Judged = false;
			}
			return theoryCombo;
		}
	}
}
