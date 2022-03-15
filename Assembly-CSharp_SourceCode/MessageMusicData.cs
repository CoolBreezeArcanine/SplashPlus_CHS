using MAI2System;
using Manager;
using UnityEngine;

public class MessageMusicData
{
	public Texture2D Jacket { get; private set; }

	public string Name { get; private set; }

	public int MusicDifficultyID { get; private set; }

	public int LevelID { get; private set; }

	public ConstParameter.ScoreKind Kind { get; private set; }

	public MessageMusicData(Texture2D jacket, string name, int difficulty, int level, ConstParameter.ScoreKind kind)
	{
		SetData(jacket, name, difficulty, level, kind);
	}

	public void SetData(Texture2D jacket, string name, int difficulty, int level, ConstParameter.ScoreKind kind)
	{
		Jacket = jacket;
		Name = name;
		MusicDifficultyID = difficulty;
		LevelID = level;
		Kind = kind;
	}

	public void SetData(MusicDifficultyID difficulty, MusicLevelID levelId)
	{
		MusicDifficultyID = (int)difficulty;
		LevelID = (int)levelId;
	}
}
