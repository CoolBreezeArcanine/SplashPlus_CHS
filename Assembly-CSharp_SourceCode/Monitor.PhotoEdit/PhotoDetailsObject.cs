using DB;
using MAI2System;
using Manager;
using Manager.UserDatas;
using UnityEngine;

namespace Monitor.PhotoEdit
{
	[RequireComponent(typeof(RectTransform))]
	public class PhotoDetailsObject : MonoBehaviour
	{
		[SerializeField]
		[Header("シングル")]
		private SingleResultCardController _singleResult;

		[SerializeField]
		[Header("マルチ")]
		private MultiResultCardController _multiResult;

		public void Initialize()
		{
			_singleResult.Initialize();
			_multiResult.Initialize();
		}

		public void ChangeLayout(PhotoeditLayoutID number)
		{
			switch (number)
			{
			case PhotoeditLayoutID.Normal:
			case PhotoeditLayoutID.Character:
				_singleResult.SetVisible(isVisible: true);
				_multiResult.SetVisible(isVisible: false);
				break;
			case PhotoeditLayoutID.AllPhoto:
				_singleResult.SetVisible(isVisible: false);
				_multiResult.SetVisible(isVisible: true);
				break;
			}
		}

		public void ViewUpdate()
		{
			_singleResult.ViewUpdate();
			_multiResult.ViewUpdate();
		}

		public void SetBasicData(string shopName, string date)
		{
			_singleResult.SetBasicData(shopName, date);
			_multiResult.SetBasicData(shopName, date);
		}

		public void SetDetailsData(string musicName, bool isClear)
		{
			_singleResult.SetMusicTitle(musicName);
			_singleResult.SetVisibleClear(isClear);
			_multiResult.SetMusicTitle(musicName);
		}

		public void SetPhotoData(Texture2D texture, int shiftPosition)
		{
			_singleResult.SetPhotoData(texture, shiftPosition);
			_multiResult.SetPhotoData(texture, 0);
		}

		public void SetDifficulty(GameManager.TargetID player, MusicDifficultyID difficultyIndex, Color difficultyFrontColor, Color difficultyBackColor, bool isThis)
		{
			if (isThis)
			{
				_singleResult.SetDifficulty(difficultyIndex, difficultyFrontColor, difficultyBackColor);
			}
			_multiResult.SetDifficulty(player, difficultyIndex);
		}

		public void SetClearRank(GameManager.TargetID player, MusicClearrankID rank, bool isMyself)
		{
			if (isMyself)
			{
				_singleResult.SetClearRank(rank);
			}
			_multiResult.SetClearRank(player, rank);
		}

		public void SetAchievement(GameManager.TargetID player, uint score, bool isMyself)
		{
			if (isMyself)
			{
				_singleResult.SetAchievement(score);
			}
			_multiResult.SetAchievement(player, score);
		}

		public void SetMyBestRecord(bool isNewRecord, int best, int diff)
		{
			_singleResult.SetMyBestRecord(isNewRecord, best, diff);
		}

		public void SetFastLate(uint fast, uint late)
		{
			_singleResult.SetFastLate(fast, late);
		}

		public void SetVisibleFastLate(bool isVisible)
		{
			_singleResult.SetVisibleFastLate(isVisible);
		}

		public void SetMaxComboData(uint maxConbo)
		{
			_singleResult.SetMaxComboData(maxConbo);
		}

		public void SetMaxSyncData(uint maxSync)
		{
			_singleResult.SetMaxSyncData(maxSync);
			_multiResult.SetMaxSync(maxSync);
		}

		public void SetVisibleSync(bool isVisible)
		{
			_singleResult.SetVisibleSync(isVisible);
		}

		public void SetVisiblePlayerRank(bool isVisible)
		{
			_singleResult.SetVisiblePlayerRank(isVisible);
		}

		public void SetPlayerRank(GameManager.TargetID player, uint rankOrder, bool isMyself)
		{
			if (isMyself)
			{
				_singleResult.SetPlayerRank(rankOrder);
			}
			_multiResult.SetPlayerRank(player, (int)rankOrder);
		}

		public void SetMedal(GameManager.TargetID player, PlayComboflagID comboType, PlaySyncflagID syncType, bool isMyself)
		{
			if (isMyself)
			{
				_singleResult.SetMedal(comboType, syncType);
			}
			_multiResult.SetMedal(player, comboType, syncType);
		}

		public void SetDxScore(uint dxScore, uint maximum, int dxStarCount)
		{
			_singleResult.SetDxScore(dxScore, maximum, dxStarCount);
		}

		public void SetLevel(GameManager.TargetID player, int level, MusicLevelID levelID, MusicDifficultyID difficulty, bool isThis)
		{
			if (isThis)
			{
				_singleResult.SetLevel(level, levelID, (int)difficulty);
			}
			_multiResult.SetLevel(player, level, levelID, difficulty);
		}

		public void SetUserData(GameManager.TargetID player, AssetManager manager, UserDetail data, UserOption option, bool isMyself)
		{
			if (isMyself)
			{
				_singleResult.SetUserData((int)player, manager, data, option);
			}
			_multiResult.SetUserData(player, manager, data, option);
		}

		public void SetScore(uint critical, uint perfect, uint great, uint good, uint miss)
		{
			_singleResult.SetScore(critical, perfect, great, good, miss);
		}

		public void SetScoreGauge(NoteScore.EScoreType type, float perfect, float critical, float great, float good, uint max)
		{
			_singleResult.SetScoreGauge(type, perfect, critical, great, good, max);
		}

		public void SetCharacter(GameManager.TargetID player, Sprite leftCharacter, Sprite rightCharacter)
		{
			_singleResult.SetCharacter((player == GameManager.TargetID.Left) ? leftCharacter : rightCharacter);
			_multiResult.SetCharacters(leftCharacter, rightCharacter);
		}

		public void SetCharacter(GameManager.TargetID player, Sprite character, bool isMyself)
		{
			if (isMyself)
			{
				_singleResult.SetCharacter(character);
			}
			_multiResult.SetCharacter(player, character);
		}

		public void SetFaceMasks(Rect[] faces, Sprite mask)
		{
			_singleResult.SetFaceMask(faces, mask);
		}

		public void RemoveMasks()
		{
			_singleResult.RemoveMasks();
		}

		public void ChangeStamp(PhotoeditStampID stampIndex)
		{
			_singleResult.ChangeStamp(stampIndex);
			_multiResult.ChangeStamp(stampIndex);
		}

		public void SetVisibleStamp(bool isVisible)
		{
			_singleResult.SetVisibleStamp(isVisible);
			_multiResult.SetVisibleStamp(isVisible);
		}

		public void SetGameScoreType(GameManager.TargetID player, ConstParameter.ScoreKind kind, bool isMyself)
		{
			if (isMyself)
			{
				_singleResult.SetGameScoreType(kind);
			}
			_multiResult.SetGameScoreType(player, kind);
		}

		public void SetPerfectChallenge(GameManager.TargetID player, bool isActive, int life, bool isClear, bool isMyself)
		{
			if (isMyself)
			{
				_singleResult.SetPerfectChallenge(isActive, life, isClear);
			}
			_multiResult.SetPerfectChallenge(player, isActive, life, isClear);
		}

		public void SetVisibleCritical(bool isVisible)
		{
			_singleResult.SetVisibleCritical(isVisible);
		}

		public void SetVisibleUserInfomation(bool isVisible)
		{
			_singleResult.SetVisibleUserInformation(isVisible);
			_multiResult.SetVisibleUserInformations(isVisible);
		}

		public void SetVisibleShootingDate(bool isVisible)
		{
			_singleResult.SetVisibleShootingDate(isVisible);
			_multiResult.SetVisibleShootingDate(isVisible);
		}

		public void SetVisibleStoreName(bool isVisible)
		{
			_singleResult.SetVisibleStoreName(isVisible);
			_multiResult.SetVisibleStoreName(isVisible);
		}

		public void SetSwitchPhotoCharacter(bool isPhoto)
		{
			_singleResult.SetSwitchPhotoCharacter(isPhoto);
		}
	}
}
