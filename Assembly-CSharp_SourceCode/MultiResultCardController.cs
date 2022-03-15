using DB;
using MAI2System;
using Manager;
using Manager.UserDatas;
using Timeline;
using UI;
using UnityEngine;

public class MultiResultCardController : ResultCardBaseController
{
	[SerializeField]
	[Header("左ユーザー")]
	private TotalResultPlayer _leftPlayer;

	[SerializeField]
	[Header("右ユーザー")]
	private TotalResultPlayer _rightPlayer;

	[SerializeField]
	[Header("MaxSync")]
	private CounterObject _maxSyncCounter;

	[SerializeField]
	[Header("順位")]
	private MultipleImage _leftRankImage;

	[SerializeField]
	[Header("順位")]
	private MultipleImage _rightRankImage;

	public void SetDifficulty(GameManager.TargetID player, MusicDifficultyID difficultyIndex)
	{
		switch (player)
		{
		case GameManager.TargetID.Left:
			_leftPlayer.SetDifficulty(difficultyIndex);
			break;
		case GameManager.TargetID.Right:
			_rightPlayer.SetDifficulty(difficultyIndex);
			break;
		}
	}

	public void SetLevel(GameManager.TargetID player, int level, MusicLevelID levelId, MusicDifficultyID difficultyIndex)
	{
		switch (player)
		{
		case GameManager.TargetID.Left:
			_leftPlayer.SetLevel(level, levelId, (int)difficultyIndex);
			break;
		case GameManager.TargetID.Right:
			_rightPlayer.SetLevel(level, levelId, (int)difficultyIndex);
			break;
		}
	}

	public void SetAchievement(GameManager.TargetID player, uint achievment)
	{
		switch (player)
		{
		case GameManager.TargetID.Left:
			_leftPlayer.SetAchievement(achievment);
			break;
		case GameManager.TargetID.Right:
			_rightPlayer.SetAchievement(achievment);
			break;
		}
	}

	public void SetUserData(GameManager.TargetID player, AssetManager manager, UserDetail data, UserOption option)
	{
		switch (player)
		{
		case GameManager.TargetID.Left:
			_leftPlayer.SetUserData((int)player, manager, data, option);
			break;
		case GameManager.TargetID.Right:
			_rightPlayer.SetUserData((int)player, manager, data, option);
			break;
		}
	}

	public void SetMaxSync(uint maxSync)
	{
		_maxSyncCounter.SetCountData(0u, maxSync);
		_maxSyncCounter.OnClipTailEnd();
	}

	public void SetCharacters(Sprite left, Sprite right)
	{
		_leftPlayer.SetCharacer(left);
		_rightPlayer.SetCharacer(right);
	}

	public void SetCharacter(GameManager.TargetID player, Sprite character)
	{
		switch (player)
		{
		case GameManager.TargetID.Left:
			_leftPlayer.SetCharacer(character);
			break;
		case GameManager.TargetID.Right:
			_rightPlayer.SetCharacer(character);
			break;
		}
	}

	public void SetVisibleUserInformations(bool isVisible)
	{
		_leftPlayer.SetVisibleUserInformation(isVisible);
		_rightPlayer.SetVisibleUserInformation(isVisible);
	}

	public void SetClearRank(GameManager.TargetID player, MusicClearrankID rank)
	{
		switch (player)
		{
		case GameManager.TargetID.Left:
			_leftPlayer.SetClearRank(rank);
			break;
		case GameManager.TargetID.Right:
			_rightPlayer.SetClearRank(rank);
			break;
		}
	}

	public void SetPlayerRank(GameManager.TargetID player, int vsRank)
	{
		switch (player)
		{
		case GameManager.TargetID.Left:
			_leftRankImage.ChangeSprite(vsRank);
			break;
		case GameManager.TargetID.Right:
			_rightRankImage.ChangeSprite(vsRank);
			break;
		}
	}

	public void SetMedal(GameManager.TargetID player, PlayComboflagID combo, PlaySyncflagID sync)
	{
		switch (player)
		{
		case GameManager.TargetID.Left:
			_leftPlayer.SetMedal(combo, sync);
			break;
		case GameManager.TargetID.Right:
			_rightPlayer.SetMedal(combo, sync);
			break;
		}
	}

	public void SetGameScoreType(GameManager.TargetID player, ConstParameter.ScoreKind kind)
	{
		switch (player)
		{
		case GameManager.TargetID.Left:
			_leftPlayer.SetGameScoreType(kind);
			break;
		case GameManager.TargetID.Right:
			_rightPlayer.SetGameScoreType(kind);
			break;
		}
	}

	public void SetPerfectChallenge(GameManager.TargetID player, bool isActive, int life, bool isClear)
	{
		switch (player)
		{
		case GameManager.TargetID.Left:
			_leftPlayer.SetPerfectChallenge(isActive, life, isClear);
			break;
		case GameManager.TargetID.Right:
			_rightPlayer.SetPerfectChallenge(isActive, life, isClear);
			break;
		}
	}
}
