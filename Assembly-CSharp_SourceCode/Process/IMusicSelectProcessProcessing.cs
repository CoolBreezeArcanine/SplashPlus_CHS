using System.Collections.Generic;
using System.Collections.ObjectModel;
using DB;
using MAI2System;
using Manager;
using Manager.Party.Party;
using Monitor;
using UnityEngine;

namespace Process
{
	public interface IMusicSelectProcessProcessing
	{
		MusicDifficultyID[] CurrentDifficulty { get; set; }

		MusicSelectMonitor[] MonitorArray { get; }

		MusicSelectProcess.MenuType[] CurrentSelectMenu { get; set; }

		bool[] IsPreparationCompletes { get; set; }

		bool IsConnectingMusic { get; set; }

		bool SharedIsInputLock { get; set; }

		bool SharedSlideScrollToRight { get; set; }

		float SharedSlideScrollTime { get; set; }

		int SharedSlideScrollCount { get; set; }

		List<ReadOnlyCollection<MusicSelectProcess.CombineMusicSelectData>> CombineMusicDataList { get; }

		ConstParameter.ScoreKind ScoreType { get; set; }

		int SortDecidePlayer { get; set; }

		bool IsScoreSort { get; }

		bool IsConnectCategoryEnable { get; }

		List<string> CategoryNameList { get; }

		SortRootID CurrentSortRootID { get; set; }

		int CurrentMusicSelect { get; set; }

		int CurrentCategorySelect { get; set; }

		int PlayingScoreID { get; set; }

		int[] DifficultySelectIndex { get; set; }

		bool[] IsForceMusicBackConfirm { get; }

		bool IsForceMusicBack { get; set; }

		int IsForceMusicBackPlayer { get; }

		RecruitInfo RecruitData { get; }

		bool JoinActive { get; set; }

		bool RecruitActive { get; set; }

		bool RecruitCancel { get; set; }

		bool PrepareFinish { get; set; }

		bool BackSetting { get; set; }

		bool ConnectMusicAllDecide { get; set; }

		void ExecuteSort(int player);

		MusicSelectProcess.LevelCategoryData GetLevelToListPositoin(int musicId, int difficulty);

		MusicSelectProcess.MusicSelectData GetMusic(int diffIndex);

		MusicSelectProcess.CombineMusicSelectData GetCombineMusic(int diffIndex);

		bool[] IsPlayableMusic(int musicID);

		bool IsMusicBoundary(int diffIndex, out int overCount);

		bool IsGenreBoundary(int diffIndex, out int overCount);

		bool IsEntry(int playerIndex);

		bool IsExtraFolder(int diffIndex);

		bool IsGhostFolder(int diffIndex);

		bool IsConnectionFolder(int index = 0);

		bool IsMapTaskFolder(int index = 0);

		bool IsChallengeFolder(int index = 0);

		bool IsTournamentFolder(int index = 0);

		int GetTournamentRankingID(int diffIndex);

		bool IsMaiList(int musicIndex = 0);

		bool IsMaiListFromCategoryIndex(int categoryIndex = 0);

		bool IsLevelTab(int musicIndex = 0);

		bool IsRemasterEnable(int index = 0);

		int GetDifficulty(int playerIndex, int musicIndex = -1);

		int GetCurrentDifficulty(int index);

		bool ScoreKindMove();

		MusicSelectProcess.SubSequence GetBeforeSubSeq(int playerIndex = -1);

		void CallWaitMessage(int playIndex);

		void CallCancelMessage(int playerIndex);

		void CallMessage(int playerIndex, WindowMessageID id);

		void CloseWindow(int playerIndex = -1);

		void ReCalcGenreSelectData();

		void ReCalcLevelSortData();

		void SetGhostJumpIndex();

		Sprite GetOptionValueSprite(string key);

		Sprite GetSortValueSprite(SortRootID root);

		void SetInputLockInfo(int monitorId, float time);

		bool IsInputLocking(int monitorId);

		bool IsUseCharacterSelect(int monitorId);

		void ChangeBGM();

		int GetSortValueIndex(SortRootID root);

		int GetSortMax(SortRootID root);

		void AddSort(SortRootID root);

		void SubSort(SortRootID root);

		void CharacterSelectReset(int playerIndex);

		void NotificationCharacterSelectProcess();

		void ChangeUserInfo();

		bool IsClientFinishSetting();
	}
}
