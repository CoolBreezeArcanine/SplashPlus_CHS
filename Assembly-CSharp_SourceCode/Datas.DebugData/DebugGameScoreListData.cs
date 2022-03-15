using System;
using Game;
using Manager;
using Manager.UserDatas;
using UnityEngine;

namespace Datas.DebugData
{
	[Serializable]
	public class DebugGameScoreListData
	{
		[SerializeField]
		private UserScore _score;

		[SerializeField]
		private MusicDifficultyID _difficulty;

		[SerializeField]
		public DebugNoteScore Tap;

		[SerializeField]
		public DebugNoteScore Hold;

		[SerializeField]
		public DebugNoteScore Slide;

		[SerializeField]
		public DebugNoteScore Touch;

		[SerializeField]
		public DebugNoteScore Break;

		[SerializeField]
		public uint DxScore;

		[SerializeField]
		public uint Combo;

		[SerializeField]
		public uint TotalCombo;

		[SerializeField]
		public uint Chain;

		[SerializeField]
		public uint Fast;

		[SerializeField]
		public uint Late;

		public UserScore Score => _score;

		public MusicDifficultyID Difficulty => _difficulty;

		public uint Perfect => Tap.GetScore(NoteJudge.JudgeBox.Perfect) + Hold.GetScore(NoteJudge.JudgeBox.Perfect) + Slide.GetScore(NoteJudge.JudgeBox.Perfect) + Touch.GetScore(NoteJudge.JudgeBox.Perfect) + Break.GetScore(NoteJudge.JudgeBox.Perfect);

		public uint Great => Tap.GetScore(NoteJudge.JudgeBox.Great) + Hold.GetScore(NoteJudge.JudgeBox.Great) + Slide.GetScore(NoteJudge.JudgeBox.Great) + Touch.GetScore(NoteJudge.JudgeBox.Great) + Break.GetScore(NoteJudge.JudgeBox.Great);

		public uint Good => Tap.GetScore(NoteJudge.JudgeBox.Great) + Hold.GetScore(NoteJudge.JudgeBox.Good) + Slide.GetScore(NoteJudge.JudgeBox.Good) + Touch.GetScore(NoteJudge.JudgeBox.Good) + Break.GetScore(NoteJudge.JudgeBox.Good);

		public uint Miss => Tap.GetScore(NoteJudge.JudgeBox.Great) + Hold.GetScore(NoteJudge.JudgeBox.Miss) + Slide.GetScore(NoteJudge.JudgeBox.Miss) + Touch.GetScore(NoteJudge.JudgeBox.Miss) + Break.GetScore(NoteJudge.JudgeBox.Miss);

		public uint Critical => Tap.GetScore(NoteJudge.JudgeBox.Critical) + Hold.GetScore(NoteJudge.JudgeBox.Critical) + Slide.GetScore(NoteJudge.JudgeBox.Critical) + Touch.GetScore(NoteJudge.JudgeBox.Critical) + Break.GetScore(NoteJudge.JudgeBox.Critical);

		public float GetNoteScore(NoteScore.EScoreType type, NoteJudge.ETiming timing)
		{
			float result = 0f;
			switch (type)
			{
			case NoteScore.EScoreType.Tap:
				result = Tap.Get(timing);
				break;
			case NoteScore.EScoreType.Hold:
				result = Hold.Get(timing);
				break;
			case NoteScore.EScoreType.Slide:
				result = Slide.Get(timing);
				break;
			case NoteScore.EScoreType.Touch:
				result = Touch.Get(timing);
				break;
			case NoteScore.EScoreType.Break:
				result = Break.Get(timing);
				break;
			}
			return result;
		}

		public uint GetScore(NoteScore.EScoreType type, NoteJudge.JudgeBox judge)
		{
			return type switch
			{
				NoteScore.EScoreType.Tap => Tap.GetScore(judge), 
				NoteScore.EScoreType.Hold => Hold.GetScore(judge), 
				NoteScore.EScoreType.Slide => Slide.GetScore(judge), 
				NoteScore.EScoreType.Touch => Touch.GetScore(judge), 
				NoteScore.EScoreType.Break => Break.GetScore(judge), 
				_ => 0u, 
			};
		}

		public uint GetCount(NoteScore.EScoreType type)
		{
			uint result = 0u;
			switch (type)
			{
			case NoteScore.EScoreType.Tap:
				result = Tap.Count;
				break;
			case NoteScore.EScoreType.Hold:
				result = Hold.Count;
				break;
			case NoteScore.EScoreType.Slide:
				result = Slide.Count;
				break;
			case NoteScore.EScoreType.Touch:
				result = Touch.Count;
				break;
			case NoteScore.EScoreType.Break:
				result = Break.Count;
				break;
			}
			return result;
		}

		public GameScoreList GetGameScoreList(int monitorIndex)
		{
			GameScoreList gameScoreList = new GameScoreList(monitorIndex);
			gameScoreList.Initialize(monitorIndex, isParty: false);
			gameScoreList.OverridePartyScore((int)_score.achivement, 0, 0, 0, 0, trackSkip: false);
			gameScoreList.SessionInfo = new SessionInfo
			{
				difficulty = (int)Difficulty,
				musicId = _score.id,
				isTutorial = false,
				isAdvDemo = false
			};
			gameScoreList.CalcUpdate();
			return gameScoreList;
		}
	}
}
