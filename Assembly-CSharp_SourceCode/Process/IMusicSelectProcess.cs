using System.Collections.Generic;
using DB;
using MAI2System;
using Manager;
using Manager.MaiStudio;
using Manager.Party.Party;
using Manager.UserDatas;
using Monitor;
using UnityEngine;

namespace Process
{
	public interface IMusicSelectProcess
	{
		List<MusicSelectProcess.GenreSelectData>[] GenreSelectDataList { get; }

		bool IsLevelCategory { get; }

		bool IsVersionCategory { get; }

		bool IsScoreSort { get; }

		bool IsConnectCategoryEnable { get; }

		ConstParameter.ScoreKind ScoreType { get; set; }

		int CurrentCategorySelect { get; set; }

		int[] DifficultySelectIndex { get; set; }

		SortTabID CategorySortSetting { get; }

		RecruitInfo RecruitData { get; }

		void SendMessage(Message message);

		MusicSelectProcess.MusicSelectData GetMusic(int diffIndex);

		MusicSelectProcess.CombineMusicSelectData GetCombineMusic(int diffIndex);

		MusicSelectProcess.GenreSelectData GetGenreSelectData(int player, int categoryIndex);

		MusicSelectProcess.GenreSelectData GetGenreSelectDataForMusicIndex(int player, int musicIndex);

		SortData GetSortData(SortRootID root);

		UserData GetUserData(int playerIndex);

		UserScore GetUserScore(int playerIndex, int difficulty, int musicId);

		MusicData GetMusicNotes(int musicID);

		Sprite GetTabSprite(MusicSelectProcess.GenreSelectData data);

		Sprite[] GetCompIconSprite(MusicClearrankID rank, MusicSelectMonitor.FcapIconEnum fcap);

		int GetDifficulty(int playerIndex, int musicIndex = -1);

		int GetDifficultyByLevel(int diffIndex);

		string GetCategoryName(int playerIndex, int diff);

		MusicSelectProcess.MedalData GetCategoryMedalData(int playerIndex, int diff);

		int GetCategoryGenruColor(int playerIndex, int diff);

		string GetOptionCategoryName(int playerIndex, int diffIndex);

		string GetOptionName(int playerIndex, int diffIndex, out OptionCategoryID category, out string value, out string detail, out string valueDetails, out string spriteKey, out bool isLeftButtonActive, out bool isRightButtonActive);

		string GetMusicCategoryNameFromMusicIndex(int musicIndex);

		int GetCurrentListIndex(int playerIndex);

		int GetCurrentCategoryMax(int playerIndex);

		int GetCurrentMenu(int playerIndex);

		int GetCurrentDifficulty(int index);

		int GetExtraPlayer(int diffIndex);

		GhostManager.GhostType GetGhostType(int music);

		bool IsExtraFolder(int diffIndex);

		bool IsGhostFolder(int diffIndex);

		bool IsRatingFolder(int diffIndex);

		bool IsConnectionFolder(int index = 0);

		bool IsMapTaskFolder(int diffIndex);

		bool IsChallengeFolder(int diffIndex);

		bool IsTournamentFolder(int diffIndex);

		int GetTournamentRankingID(int diffIndex);

		bool IsMusicBoundary(int diffIndex, out int overCount);

		bool IsGenreBoundary(int diffIndex, out int overCount);

		bool IsOptionBoundary(int playerIndex, int diffIndex, out int overCount);

		bool IsMaiList(int musicIndex = 0);

		bool IsLevelTab(int musicIndex = 0);

		bool IsRemasterEnable(int index = 0);

		bool IsMusicSeq(int player = -1);

		float GetVolumeAmount(int playerIndex);

		Sprite GetOptionValueSprite(string key);

		Sprite GetSortValueSprite(SortRootID root);

		Texture2D[] GetGenreTextureList(int index = 0);

		Texture2D GetGenreTexture(int index);

		bool IsActiveJoined();

		bool IsClientFinishSetting();
	}
}
