using System.Collections.Generic;
using DB;
using MAI2.Util;
using Manager;
using Manager.Party.Party;
using Manager.UserDatas;
using Process;
using UI.DaisyChainList;
using UnityEngine;
using Util;

namespace Monitor.MusicSelect.ChainList
{
	public class MenuChainList : MusicSelectDaisyChainList
	{
		[SerializeField]
		private MusicSelectChainList _musicSelectChainList;

		public void ChangeDifficulty(MusicDifficultyID difficulty)
		{
			MusicSelectProcess.MusicSelectData music = SelectProcess.GetMusic(0);
			MusicSelectProcess.CombineMusicSelectData combineMusic = SelectProcess.GetCombineMusic(0);
			MenuCardObject menuCardObject = (MenuCardObject)SpotArray[4];
			UserScore userScore = SelectProcess.GetUserScore(MonitorIndex, (int)difficulty, music.MusicData.name.id);
			int num = -1;
			uint dxScore = 0u;
			uint dxScoreMax = 0u;
			MusicClearrankID musicClearrankID = MusicClearrankID.Invalid;
			PlayComboflagID scoreAchivment = PlayComboflagID.None;
			PlaySyncflagID sync = PlaySyncflagID.None;
			if (userScore != null)
			{
				num = (int)userScore.achivement;
				musicClearrankID = GameManager.GetClearRank(num);
				scoreAchivment = userScore.combo;
				sync = userScore.sync;
				dxScore = userScore.deluxscore;
				dxScoreMax = (uint)(music.ScoreData[(int)difficulty].maxNotes * 3);
			}
			string thumbnailName = music.MusicData.thumbnailName;
			menuCardObject.MusicCardObject.SetMusicData(music.MusicData.name.str, music.MusicData.artistName.str, music.ScoreData[(int)difficulty].notesDesigner.str, music.MusicData.bpm, AssetManager.GetJacketThumbTexture2D(thumbnailName), (int)difficulty);
			menuCardObject.MusicCardObject.SetScoreAchivement(musicClearrankID, num, dxScore, dxScoreMax, scoreAchivment, sync);
			int level = music.ScoreData[(int)difficulty].level;
			menuCardObject.MusicCardObject.SetLevel(level, (MusicLevelID)music.ScoreData[(int)difficulty].musicLevelID, (int)difficulty);
			menuCardObject.MusicCardObject.SetDifficulty(_musicSelectChainList.GetDifficultyBackSprite((int)difficulty), _musicSelectChainList.GetDifficultyTagDeluxeSprite((int)difficulty), _musicSelectChainList.GetDifficultyTagStandardSprite((int)difficulty), _musicSelectChainList.GetDifficultyMiniBackSprite((int)difficulty));
			menuCardObject.MusicCardObject.SetTagAddRank(_musicSelectChainList.GetRankTagAddDeluxeSprite((int)musicClearrankID), _musicSelectChainList.GetRankTagAddStandardSprite((int)musicClearrankID));
			int id = music.MusicData.name.id;
			bool flag = id >= 10000 && id < 20000;
			bool deluxeExist = combineMusic.existDeluxeScore;
			bool standardExist = combineMusic.existStandardScore;
			if (flag)
			{
				standardExist = false;
			}
			else
			{
				deluxeExist = false;
			}
			menuCardObject.MusicCardObject.SetScoreKind(flag, deluxeExist, standardExist, isAnimation: false);
			NotesWrapper notesWrapper = Singleton<NotesListManager>.Instance.GetNotesList()[music.MusicData.GetID()];
			bool isExist = notesWrapper.IsEnable[4];
			bool isUnlock = notesWrapper.IsUnlock[4];
			_ = notesWrapper.Ranking;
			int id2 = music.MusicData.genreName.id;
			Color bgColor = Utility.ConvertColor(Singleton<DataManager>.Instance.GetMusicGenre(id2).Color);
			if (difficulty == MusicDifficultyID.Master)
			{
				menuCardObject.MusicCardObject.SetRemasterExistIcon(isExist, isUnlock);
			}
			else
			{
				menuCardObject.MusicCardObject.SetRemasterExistIcon(isExist: false, isUnlock: false);
			}
			bool flag2 = SelectProcess.GetMusicCategoryNameFromMusicIndex(0).Contains(Singleton<DataManager>.Instance.GetMusicGenre(2).genreName);
			menuCardObject.MusicCardObject.SetRanking(flag2 ? _musicSelectChainList.GetRankingGenreSprite(id2) : null, bgColor);
		}

		public void SetVolume(OptionHeadphonevolumeID volume, float amount)
		{
			((MenuCardObject)SpotArray[4]).SetVolume(volume, amount);
		}

		public void HideUnUseCard()
		{
			for (int i = 0; i < GetCardListCount(); i++)
			{
				if (i >= 4)
				{
					MenuCardObject card = GetCard<MenuCardObject>(i);
					if (card != null)
					{
						card.gameObject.SetActive(value: false);
					}
				}
			}
		}

		public override void Deploy()
		{
			RemoveAll();
			int currentMenu = SelectProcess.GetCurrentMenu(MonitorIndex);
			MusicSelectProcess.MenuType menuType = MusicSelectProcess.MenuType.Matching;
			int scoreAttackRankingId;
			for (int i = 0; i < 9; i++)
			{
				if (i < 4 - currentMenu)
				{
					continue;
				}
				MenuCardObject chain = GetChain<MenuCardObject>();
				chain.ResetChain();
				switch (menuType)
				{
				case MusicSelectProcess.MenuType.GameStart:
				{
					chain.MusicCardObject.gameObject.SetActive(value: true);
					chain.MusicCardObject.ChangeSize(isMainActive: true);
					MusicSelectProcess.CombineMusicSelectData combineMusic = SelectProcess.GetCombineMusic(0);
					MusicSelectProcess.MusicSelectData musicSelectData = combineMusic.musicSelectData[(int)SelectProcess.ScoreType];
					MusicSelectProcess.MusicSelectDetailData msDetailData = combineMusic.msDetailData;
					bool flag3 = SelectProcess.IsGhostFolder(0);
					bool flag4 = SelectProcess.IsRatingFolder(0);
					bool flag5 = SelectProcess.IsMapTaskFolder(0);
					bool flag6 = SelectProcess.IsChallengeFolder(0);
					bool flag7 = SelectProcess.IsTournamentFolder(0);
					bool flag8 = false;
					scoreAttackRankingId = SelectProcess.GetTournamentRankingID(0);
					bool isRanking = false;
					int score = 0;
					int ranking = -1;
					if (flag7)
					{
						int count = Singleton<ScoreRankingManager>.Instance.getSrDataForMS(scoreAttackRankingId).MusicInfoList.Count;
						MusicSelectProcess.GenreSelectData genreSelectDataForMusicIndex = SelectProcess.GetGenreSelectDataForMusicIndex(MonitorIndex, 0);
						if (genreSelectDataForMusicIndex != null && count == genreSelectDataForMusicIndex.totalMusicNum)
						{
							isRanking = true;
							UserScoreRanking userScoreRanking = Singleton<UserDataManager>.Instance.GetUserData(MonitorIndex).ScoreRankingList.Find((UserScoreRanking scoreData) => scoreData.tournamentId == scoreAttackRankingId);
							if (userScoreRanking != null)
							{
								score = (int)userScoreRanking.totalScore;
								ranking = userScoreRanking.ranking;
							}
						}
					}
					int num5 = SelectProcess.DifficultySelectIndex[MonitorIndex];
					if (num5 == -1)
					{
						num5 = 0;
					}
					UserScore userScore = SelectProcess.GetUserScore(MonitorIndex, num5, musicSelectData.MusicData.name.id);
					NotesWrapper notesWrapper = Singleton<NotesListManager>.Instance.GetNotesList()[combineMusic.GetID(SelectProcess.ScoreType)];
					int num6 = -1;
					uint dxScore = 0u;
					uint dxScoreMax = 0u;
					MusicClearrankID musicClearrankID = MusicClearrankID.Invalid;
					PlayComboflagID scoreAchivment = PlayComboflagID.None;
					PlaySyncflagID sync = PlaySyncflagID.None;
					if (userScore != null)
					{
						num6 = (int)userScore.achivement;
						musicClearrankID = GameManager.GetClearRank(num6);
						scoreAchivment = userScore.combo;
						sync = userScore.sync;
						dxScore = userScore.deluxscore;
						dxScoreMax = (uint)(musicSelectData.ScoreData[num5].maxNotes * 3);
					}
					string thumbnailName = musicSelectData.MusicData.thumbnailName;
					chain.MusicCardObject.SetMusicData(musicSelectData.MusicData.name.str, musicSelectData.MusicData.artistName.str, musicSelectData.ScoreData[num5].notesDesigner.str, musicSelectData.MusicData.bpm, AssetManager.GetJacketThumbTexture2D(thumbnailName), num5);
					chain.MusicCardObject.SetScoreAchivement(musicClearrankID, num6, dxScore, dxScoreMax, scoreAchivment, sync);
					int level = musicSelectData.ScoreData[num5].level;
					chain.MusicCardObject.SetCopyright((string.Empty != musicSelectData.MusicData.rightFile) ? AssetManager.GetRightTexture2D(musicSelectData.MusicData.rightFile) : null);
					chain.MusicCardObject.SetScoreAttackRankingInfo(flag7, isRanking, score, ranking);
					chain.MusicCardObject.SetLevel(level, (MusicLevelID)musicSelectData.ScoreData[num5].musicLevelID, num5);
					chain.MusicCardObject.SetDifficulty(_musicSelectChainList.GetDifficultyBackSprite(num5), _musicSelectChainList.GetDifficultyTagDeluxeSprite(num5), _musicSelectChainList.GetDifficultyTagStandardSprite(num5), _musicSelectChainList.GetDifficultyMiniBackSprite(num5));
					chain.MusicCardObject.SetTagAddRank(_musicSelectChainList.GetRankTagAddDeluxeSprite((int)musicClearrankID), _musicSelectChainList.GetRankTagAddStandardSprite((int)musicClearrankID));
					int id = musicSelectData.MusicData.name.id;
					bool flag9 = false;
					bool isUnlock = notesWrapper.IsNeedUnlock[MonitorIndex][num5];
					bool isRating = notesWrapper.IsRating[MonitorIndex][num5];
					bool flag10 = id >= 10000 && id < 20000;
					bool deluxeExist = combineMusic.existDeluxeScore;
					bool standardExist = combineMusic.existStandardScore;
					_ = notesWrapper.Ranking;
					int id2 = musicSelectData.MusicData.genreName.id;
					Color bgColor = Utility.ConvertColor(Singleton<DataManager>.Instance.GetMusicGenre(id2).Color);
					for (int l = 0; l < notesWrapper.IsMapBonus.Count; l++)
					{
						flag9 |= notesWrapper.IsMapBonus[l];
					}
					if (flag10)
					{
						standardExist = false;
					}
					else
					{
						deluxeExist = false;
					}
					chain.MusicCardObject.SetScoreKind(flag10, deluxeExist, standardExist, isAnimation: false);
					bool isExist = notesWrapper.IsEnable[4];
					bool isUnlock2 = notesWrapper.IsUnlock[4];
					if (num5 == 3)
					{
						chain.MusicCardObject.SetRemasterExistIcon(isExist, isUnlock2);
					}
					else
					{
						chain.MusicCardObject.SetRemasterExistIcon(isExist: false, isUnlock: false);
					}
					bool flag11 = SelectProcess.GetMusicCategoryNameFromMusicIndex(0).Contains(Singleton<DataManager>.Instance.GetMusicGenre(2).genreName);
					chain.MusicCardObject.SetRanking(flag11 ? _musicSelectChainList.GetRankingGenreSprite(id2) : null, bgColor);
					chain.MusicCardObject.SetNewLable(notesWrapper.IsNewMusic);
					chain.MusicCardObject.SetBonus(flag9);
					chain.MusicCardObject.SetLabel(isUnlock, isRating);
					if (flag6)
					{
						chain.MusicCardObject.SetChallengeView(active: true);
						chain.MusicCardObject.SetChallengeLife(msDetailData.startLife);
					}
					else
					{
						chain.MusicCardObject.SetChallengeView(active: false);
					}
					if (flag3)
					{
						int extraPlayer = SelectProcess.GetExtraPlayer(0);
						GhostManager.GhostType ghostType = SelectProcess.GetGhostType(0);
						bool flag12 = SelectProcess.GetCombineMusic(0).musicSelectData[(int)SelectProcess.ScoreType].Difficulty <= num5;
						chain.MusicCardObject.SetGhost(flag12, (int)ghostType);
						chain.MusicCardObject.SetPlayerPlate(flag12, extraPlayer);
						UserGhost ghostToEnum = Singleton<GhostManager>.Instance.GetGhostToEnum(musicSelectData.GhostTarget);
						if (ghostToEnum != null)
						{
							if (ghostType == GhostManager.GhostType.Map && flag12)
							{
								int iconId = ghostToEnum.IconId;
								chain.MusicCardObject.SetGhost(Resources.Load<Sprite>("Process/MusicSelect/Sprites/MusicSelect/MusicPlate/NPC/UI_MSS_GhostB_Icon_" + iconId.ToString("000000")));
							}
							MusicClearrankID clearRank = GameManager.GetClearRank(ghostToEnum.Achievement);
							chain.MusicCardObject.SetGhostRank(flag12, clearRank);
						}
						else
						{
							chain.MusicCardObject.SetGhostRank(isVisisble: false, MusicClearrankID.Invalid);
						}
					}
					else if (flag4)
					{
						chain.MusicCardObject.SetGhost(isVisisble: false, 0);
						chain.MusicCardObject.SetPlayerPlate(num5 == musicSelectData.Difficulty, musicSelectData.RatingPlayer);
						chain.MusicCardObject.SetGhostRank(isVisisble: false, MusicClearrankID.Invalid);
					}
					else if (flag5 || flag6)
					{
						chain.MusicCardObject.SetGhost(isVisisble: false, 0);
						chain.MusicCardObject.SetGhostRank(isVisisble: false, MusicClearrankID.Invalid);
						int playerIndex = 0;
						int playerIndex2 = -1;
						if (flag5)
						{
							if (notesWrapper.IsMapTaskMusic[0])
							{
								if (notesWrapper.IsMapTaskMusic[1])
								{
									playerIndex2 = 1;
								}
							}
							else
							{
								playerIndex = 1;
							}
						}
						else if (flag6)
						{
							if (notesWrapper.ChallengeDetail[0].isEnable)
							{
								if (notesWrapper.ChallengeDetail[1].isEnable)
								{
									playerIndex2 = 1;
								}
							}
							else
							{
								playerIndex = 1;
							}
						}
						chain.MusicCardObject.SetPlayerPlate(isVisible: true, playerIndex, playerIndex2);
						if (notesWrapper.ChallengeDetail[MonitorIndex].isEnable)
						{
							flag8 = true;
						}
					}
					else
					{
						chain.MusicCardObject.SetGhost(isVisisble: false, 0);
						chain.MusicCardObject.SetPlayerPlate(isVisible: false, 0);
						chain.MusicCardObject.SetGhostRank(isVisisble: false, MusicClearrankID.Invalid);
					}
					if (flag6 && flag8)
					{
						chain.MusicCardObject.SetLockScore(5);
					}
					if (SelectProcess.GetCurrentMenu(MonitorIndex) != 1)
					{
						chain.MusicCardObject.ChangeSize(isMainActive: false);
					}
					break;
				}
				case MusicSelectProcess.MenuType.Volume:
				{
					OptionHeadphonevolumeID headPhoneVolume = Singleton<UserDataManager>.Instance.GetUserData(MonitorIndex).Option.HeadPhoneVolume;
					chain.SetVolume(headPhoneVolume, SelectProcess.GetVolumeAmount(MonitorIndex));
					break;
				}
				case MusicSelectProcess.MenuType.Matching:
				{
					chain.MatchingCardObject.gameObject.SetActive(value: true);
					bool flag = SelectProcess.IsGhostFolder(0);
					bool flag2 = SelectProcess.IsChallengeFolder(0);
					if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.MachineGroupID != MachineGroupID.OFF)
					{
						if (GameManager.IsFreedomMode)
						{
							chain.MatchingCardObject.SetMatchingInfo(MatchingChainCardObject.MatchingPattern.Disable);
							chain.MatchingCardObject.SetNgReason(CommonMessageID.MusicSelectConnectNG_Freedom.GetName());
						}
						else if (GameManager.IsCourseMode)
						{
							chain.MatchingCardObject.SetMatchingInfo(MatchingChainCardObject.MatchingPattern.Disable);
							chain.MatchingCardObject.SetNgReason(CommonMessageID.MusicSelectConnectNG_Course.GetName());
						}
						else if (flag)
						{
							chain.MatchingCardObject.SetMatchingInfo(MatchingChainCardObject.MatchingPattern.Disable);
							chain.MatchingCardObject.SetNgReason(CommonMessageID.MusicSelectConnectNG_Ghost.GetName());
						}
						else if (flag2)
						{
							chain.MatchingCardObject.SetMatchingInfo(MatchingChainCardObject.MatchingPattern.Disable);
							chain.MatchingCardObject.SetNgReason(CommonMessageID.MusicSelectConnectNG_Challenge.GetName());
						}
						else if (Party.Get().IsHost() || Party.Get().IsClient())
						{
							chain.MatchingCardObject.SetMatchingInfo(MatchingChainCardObject.MatchingPattern.Matching);
							IEnumerable<MechaInfo> joinMembers = Party.Get().GetPartyMemberInfo().GetJoinMembers();
							int num = 0;
							int num2 = 0;
							int num3 = 4;
							foreach (MechaInfo item in joinMembers)
							{
								MatchingPlayerObject.PlayerState playerState = MatchingPlayerObject.PlayerState.Entry;
								if (SelectProcess.IsActiveJoined() || Party.Get().IsClient())
								{
									playerState = MatchingPlayerObject.PlayerState.Setup;
								}
								if (Party.Get().IsHost() && num2 >= 1 && SelectProcess.IsClientFinishSetting())
								{
									playerState = MatchingPlayerObject.PlayerState.Ready;
								}
								for (int j = 0; j < item.Entrys.Length; j++)
								{
									if (item.Entrys[j])
									{
										int difficulty = item.FumenDifs[j];
										string userName = item.UserNames[j];
										int num4 = j;
										if ((Party.Get().IsHost() && num2 >= 1) || (Party.Get().IsClient() && num2 < 1))
										{
											num4 += 2;
										}
										Texture2D iconTexture2D = AssetManager.GetIconTexture2D(num4, item.IconIDs[j]);
										chain.MatchingCardObject.SetPlayerInfo(num, difficulty, playerState, userName, iconTexture2D);
									}
									else
									{
										chain.MatchingCardObject.SetPlayerInfo(num, 0, MatchingPlayerObject.PlayerState.None, null, null);
									}
									num++;
								}
								num2++;
							}
							for (int k = num; k < num3; k++)
							{
								chain.MatchingCardObject.SetPlayerInfo(k, 0, MatchingPlayerObject.PlayerState.None, null, null);
							}
						}
						else
						{
							chain.MatchingCardObject.SetMatchingInfo(MatchingChainCardObject.MatchingPattern.Enable);
						}
					}
					else
					{
						chain.MatchingCardObject.SetMatchingInfo(MatchingChainCardObject.MatchingPattern.Disable);
						chain.MatchingCardObject.SetNgReason(CommonMessageID.MusicSelectConnectNG_Single.GetName());
					}
					break;
				}
				}
				chain.Initialize(menuType);
				chain.ChangeOptionCard(Singleton<UserDataManager>.Instance.GetUserData(MonitorIndex).Option.OptionKind);
				if (i == 4)
				{
					chain.OnCenterIn();
					chain.MatchingCardObject.SetRecruitBaseSize(isBig: true);
				}
				else
				{
					chain.OnCenterOut();
					chain.MatchingCardObject.SetRecruitBaseSize(isBig: false);
				}
				SetSpot(i, chain);
				menuType++;
			}
			base.IsListEnable = true;
		}

		protected override void Next(int targetIndex, Direction direction)
		{
		}

		public void SetConnectMember()
		{
			MenuCardObject menuCardObject = (MenuCardObject)SpotArray[4];
			if ((bool)menuCardObject)
			{
				menuCardObject.MatchingCardObject.SetMatchingInfo(MatchingChainCardObject.MatchingPattern.Matching);
			}
		}

		public void SetChangeConnectCardSize()
		{
			for (int i = 0; i < 9; i++)
			{
				if (i == 4)
				{
					((MenuCardObject)SpotArray[i])?.MatchingCardObject.SetRecruitBaseSize(isBig: true);
				}
				else
				{
					((MenuCardObject)SpotArray[i])?.MatchingCardObject.SetRecruitBaseSize(isBig: false);
				}
			}
		}

		public void PressedButton(Direction direction)
		{
			MenuCardObject menuCardObject = (MenuCardObject)SpotArray[4];
			if ((bool)menuCardObject)
			{
				menuCardObject.PressedButton(direction);
			}
		}

		public void ChangeOptionCard(OptionKindID kind)
		{
			for (int i = 0; i < 9; i++)
			{
				if (SpotArray[i] != null)
				{
					((MenuCardObject)SpotArray[i]).ChangeOptionCard(kind);
				}
			}
		}

		public void SetSpeed(string speed)
		{
			foreach (MenuCardObject chainObject in ChainObjectList)
			{
				chainObject.SetSpeed(speed);
			}
		}

		public void SetDetils(string mirror, string trackSkip)
		{
			foreach (MenuCardObject chainObject in ChainObjectList)
			{
				chainObject.SetCustomDetils(mirror, trackSkip);
			}
		}

		public void SetCharacterSelectMessage(bool isActive, string message)
		{
			foreach (MenuCardObject chainObject in ChainObjectList)
			{
				chainObject.SetCharacterSelectMessage(isActive, message);
			}
		}
	}
}
