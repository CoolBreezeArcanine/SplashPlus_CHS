using System.Collections.Generic;
using System.Linq;
using Datas.DebugData;
using DB;
using Game;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager.MaiStudio;
using Manager.Party.Party;
using Manager.UserDatas;

namespace Manager
{
	public class GameScoreList
	{
		public bool IsEnable;

		public ulong UserID;

		public string UserName;

		public UserData.UserIDType UserType;

		public uint TheoryChain;

		public bool IsChallenge;

		public bool IsCourse;

		public bool IsCourseForceFail;

		public bool IsLifeZero;

		private uint _missPoints = 1u;

		private uint _prevCriticalNum;

		private uint _prevPerfectNum;

		private uint _prevGreatNum;

		private uint _prevGoodNum;

		private uint _prevMissNum;

		private uint _nowCriticalNum;

		private uint _nowPerfectNum;

		private uint _nowGreatNum;

		private uint _nowGoodNum;

		private uint _nowMissNum;

		private readonly int _monitorIndex;

		private JudgeResultSt[] _judgeResultList;

		private readonly uint[,] _resultList = new uint[6, 15];

		private readonly uint[] _totalJudgeList = new uint[6];

		public NotesTotal ScoreTotal;

		public SessionInfo SessionInfo;

		public bool IsTrackSkip;

		public bool IsClear;

		public uint VsRank;

		public UserOption UserOption;

		public int[] CharaSlot = new int[5];

		public uint[] CharaLevel = new uint[5];

		public uint[] CharaAwake = new uint[5];

		public List<RivalData> Rival = new List<RivalData>();

		public long UnixTime { get; private set; }

		public int TrackNo { get; private set; }

		public int PlayerIndex { get; private set; }

		public uint CriticalNum { get; private set; }

		public uint PerfectNum { get; private set; }

		public uint GreatNum { get; private set; }

		public uint GoodNum { get; private set; }

		public uint MissNum { get; private set; }

		public uint TrueCriticalNum { get; private set; }

		public uint TruePerfectNum { get; private set; }

		public decimal TheoryIgnoreBreakAchivement { get; private set; }

		public decimal TheoryBreakAchivement { get; private set; }

		public decimal AchivementIgnoreBreak { get; private set; }

		public decimal AchivementBreakOnly { get; private set; }

		public uint TotalScore { get; private set; }

		public uint TheoryScore { get; private set; }

		public bool ResetCombo { get; private set; }

		public uint Combo { get; private set; }

		public uint MaxCombo { get; private set; }

		public uint TheoryCombo { get; private set; }

		public PlayComboflagID ComboType { get; private set; }

		public PlaySyncflagID SyncType { get; set; }

		public uint ComboAddThisFrame { get; private set; }

		public uint Chain { get; private set; }

		public uint MaxChain { get; private set; }

		public uint Fast { get; private set; }

		public uint Late { get; private set; }

		public uint Life { get; private set; }

		public uint StartLife { get; private set; }

		public decimal PreAchivement { get; private set; }

		public decimal Achivement { get; private set; }

		public uint PreDxScore { get; private set; }

		public uint DxScore { get; private set; }

		public uint TheoryDxScore { get; private set; }

		public uint PreMusicRate { get; private set; }

		public uint MusicRate { get; private set; }

		public uint PreDanRate { get; private set; }

		public uint DanRate { get; private set; }

		public UdemaeID PreDan { get; private set; }

		public UdemaeID Dan { get; private set; }

		public UdemaeID MaxDan { get; private set; }

		public uint PreUdemaeValue { get; private set; }

		public uint PreClassValue { get; private set; }

		public uint ClassValue { get; private set; }

		public bool IsHuman()
		{
			return UserData.IsHuman(UserType);
		}

		public bool IsGhost()
		{
			return UserData.IsGhost(UserType);
		}

		public bool IsNpc()
		{
			return UserData.IsNpc(UserType);
		}

		public bool IsBoss()
		{
			return UserData.IsBoss(UserType);
		}

		public uint PreRating()
		{
			return PreDanRate + PreMusicRate;
		}

		public uint Rating()
		{
			return DanRate + MusicRate;
		}

		public GameScoreList(int index)
		{
			_monitorIndex = index;
		}

		public bool CheckResultNum(int resultNum)
		{
			return NotesManager.Instance(_monitorIndex).getReader().GetNoteList()
				.Count == resultNum;
		}

		public void SetForceAchivement_Battle(float achivement)
		{
			decimal num = (decimal)achivement;
			NoteJudge.ETiming[] array = new NoteJudge.ETiming[8]
			{
				NoteJudge.ETiming.Critical,
				NoteJudge.ETiming.LatePerfect,
				NoteJudge.ETiming.LatePerfect2nd,
				NoteJudge.ETiming.LateGreat,
				NoteJudge.ETiming.LateGreat2nd,
				NoteJudge.ETiming.LateGreat3rd,
				NoteJudge.ETiming.LateGood,
				NoteJudge.ETiming.TooLate
			};
			uint num2 = ScoreTotal._totalData[37];
			uint num3 = ScoreTotal._totalData[39];
			uint num4 = ScoreTotal._totalData[33];
			uint num5 = ScoreTotal._totalData[32] + ScoreTotal._totalData[34];
			uint num6 = ScoreTotal._totalData[38] - num4;
			uint num7 = ScoreTotal._totalData[36] + ScoreTotal._totalData[40] - num5;
			NoteTypeID.Def downerKind = NoteTypeID.Def.Break;
			if (num7 != 0)
			{
				downerKind = NoteTypeID.Def.Begin;
			}
			else if (num6 != 0)
			{
				downerKind = NoteTypeID.Def.Hold;
			}
			else if (num5 != 0)
			{
				downerKind = NoteTypeID.Def.ExTap;
			}
			else if (num4 != 0)
			{
				downerKind = NoteTypeID.Def.ExHold;
			}
			else if (num3 != 0)
			{
				downerKind = NoteTypeID.Def.Slide;
			}
			uint[] judgeNum = new uint[array.Length];
			uint[] useJudgeNum = new uint[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				judgeNum[i] = 0u;
				useJudgeNum[i] = 0u;
			}
			uint normalScore = 0u;
			uint num8 = 0u;
			uint normalTheoryScore = 0u;
			uint num9 = 0u;
			for (int j = 0; j < num2; j++)
			{
				normalTheoryScore += NoteScore.GetJudgeScore(array[0], NoteScore.EScoreType.Break);
				num9 += NoteScore.GetJudgeScore(array[0], NoteScore.EScoreType.BreakBonus);
				int num10 = 0;
				foreach (NoteJudge.ETiming item in array.Reverse())
				{
					uint num11 = normalScore + NoteScore.GetJudgeScore(item, NoteScore.EScoreType.Break);
					uint num12 = num8 + NoteScore.GetJudgeScore(item, NoteScore.EScoreType.BreakBonus);
					decimal num13 = 0.0m;
					num13 = ((num9 != 0) ? ((decimal)num11 / (decimal)normalTheoryScore * 100.0m + (decimal)num12 / (decimal)num9) : ((decimal)num11 / (decimal)normalTheoryScore * 100.0m));
					if (num13 >= num || item == NoteJudge.ETiming.Critical)
					{
						normalScore = num11;
						num8 = num12;
						judgeNum[array.Length - num10 - 1]++;
						break;
					}
					num10++;
				}
			}
			decimal bonusAchieve = default(decimal);
			if (num9 != 0)
			{
				bonusAchieve = (decimal)num8 / (decimal)num9;
			}
			uint[] judgeNum2 = new uint[array.Length];
			uint[] useJudgeNum2 = new uint[array.Length];
			for (int k = 0; k < array.Length; k++)
			{
				judgeNum2[k] = 0u;
				useJudgeNum2[k] = 0u;
			}
			ProcessCalcJudge(ref normalTheoryScore, ref normalScore, ref judgeNum2, bonusAchieve, num3, num, array, downerKind, NoteTypeID.Def.Slide);
			uint[] judgeNum3 = new uint[array.Length];
			uint[] useJudgeNum3 = new uint[array.Length];
			for (int l = 0; l < array.Length; l++)
			{
				judgeNum3[l] = 0u;
				useJudgeNum3[l] = 0u;
			}
			ProcessCalcJudge(ref normalTheoryScore, ref normalScore, ref judgeNum3, bonusAchieve, num4, num, array, downerKind, NoteTypeID.Def.ExHold);
			uint[] judgeNum4 = new uint[array.Length];
			uint[] useJudgeNum4 = new uint[array.Length];
			for (int m = 0; m < array.Length; m++)
			{
				judgeNum4[m] = 0u;
				useJudgeNum4[m] = 0u;
			}
			ProcessCalcJudge(ref normalTheoryScore, ref normalScore, ref judgeNum4, bonusAchieve, num5, num, array, downerKind, NoteTypeID.Def.ExTap);
			uint[] judgeNum5 = new uint[array.Length];
			uint[] useJudgeNum5 = new uint[array.Length];
			for (int n = 0; n < array.Length; n++)
			{
				judgeNum5[n] = 0u;
				useJudgeNum5[n] = 0u;
			}
			ProcessCalcJudge(ref normalTheoryScore, ref normalScore, ref judgeNum5, bonusAchieve, num6, num, array, downerKind, NoteTypeID.Def.Hold);
			uint[] judgeNum6 = new uint[array.Length];
			uint[] useJudgeNum6 = new uint[array.Length];
			for (int num14 = 0; num14 < array.Length; num14++)
			{
				judgeNum6[num14] = 0u;
				useJudgeNum6[num14] = 0u;
			}
			ProcessCalcJudge(ref normalTheoryScore, ref normalScore, ref judgeNum6, bonusAchieve, num7, num, array, downerKind, NoteTypeID.Def.Begin);
			NoteDataList noteList = NotesManager.Instance(_monitorIndex).getReader().GetNoteList();
			foreach (NoteData item2 in noteList)
			{
				if (item2.type == NoteTypeID.Def.Break || item2.type == NoteTypeID.Def.BreakStar)
				{
					NoteJudge.ETiming timing = distoributionNotes(ref judgeNum, ref useJudgeNum, array);
					NoteScore.EScoreType kind = GamePlayManager.NoteType2ScoreType(item2.type.getEnum());
					SetResult(item2.indexNote, kind, timing);
				}
			}
			foreach (NoteData item3 in noteList)
			{
				if (item3.type == NoteTypeID.Def.Slide)
				{
					NoteJudge.ETiming timing2 = distoributionNotes(ref judgeNum2, ref useJudgeNum2, array);
					NoteScore.EScoreType kind2 = GamePlayManager.NoteType2ScoreType(item3.type.getEnum());
					SetResult(item3.indexNote, kind2, timing2);
				}
			}
			foreach (NoteData item4 in noteList)
			{
				if (item4.type == NoteTypeID.Def.ExHold)
				{
					NoteJudge.ETiming timing3 = distoributionNotes(ref judgeNum3, ref useJudgeNum3, array);
					NoteScore.EScoreType kind3 = GamePlayManager.NoteType2ScoreType(item4.type.getEnum());
					SetResult(item4.indexNote, kind3, timing3);
				}
			}
			foreach (NoteData item5 in noteList)
			{
				if (item5.type == NoteTypeID.Def.ExTap || item5.type == NoteTypeID.Def.ExStar)
				{
					NoteJudge.ETiming timing4 = distoributionNotes(ref judgeNum4, ref useJudgeNum4, array);
					NoteScore.EScoreType kind4 = GamePlayManager.NoteType2ScoreType(item5.type.getEnum());
					SetResult(item5.indexNote, kind4, timing4);
				}
			}
			foreach (NoteData item6 in noteList)
			{
				if (item6.type == NoteTypeID.Def.Hold || item6.type == NoteTypeID.Def.TouchHold)
				{
					NoteJudge.ETiming timing5 = distoributionNotes(ref judgeNum5, ref useJudgeNum5, array);
					NoteScore.EScoreType kind5 = GamePlayManager.NoteType2ScoreType(item6.type.getEnum());
					SetResult(item6.indexNote, kind5, timing5);
				}
			}
			foreach (NoteData item7 in noteList)
			{
				if (item7.type == NoteTypeID.Def.Begin || item7.type == NoteTypeID.Def.Star || item7.type == NoteTypeID.Def.TouchTap)
				{
					NoteJudge.ETiming timing6 = distoributionNotes(ref judgeNum6, ref useJudgeNum6, array);
					NoteScore.EScoreType kind6 = GamePlayManager.NoteType2ScoreType(item7.type.getEnum());
					SetResult(item7.indexNote, kind6, timing6);
				}
			}
		}

		private void ProcessCalcJudge(ref uint normalTheoryScore, ref uint normalScore, ref uint[] judgeNum, decimal bonusAchieve, uint noteNum, decimal targetAchieve, NoteJudge.ETiming[] scoreTable, NoteTypeID.Def downerKind, NoteTypeID.Def type)
		{
			for (int i = 0; i < noteNum; i++)
			{
				normalTheoryScore += NoteScore.GetJudgeScore(scoreTable[0], convertType(type));
				int num = 0;
				if (downerKind == type)
				{
					foreach (NoteJudge.ETiming eTiming in scoreTable)
					{
						if (!IsEx(type) || eTiming == NoteJudge.ETiming.Critical || eTiming == NoteJudge.ETiming.TooLate)
						{
							uint num2 = normalScore + NoteScore.GetJudgeScore(eTiming, convertType(type));
							if ((decimal)num2 / (decimal)normalTheoryScore * 100.0m + bonusAchieve < targetAchieve || eTiming == NoteJudge.ETiming.TooLate)
							{
								normalScore = num2;
								judgeNum[num]++;
								break;
							}
						}
						num++;
					}
					continue;
				}
				foreach (NoteJudge.ETiming item in scoreTable.Reverse())
				{
					if (!IsEx(type) || item == NoteJudge.ETiming.Critical || item == NoteJudge.ETiming.TooLate)
					{
						uint num3 = normalScore + NoteScore.GetJudgeScore(item, convertType(type));
						if ((decimal)num3 / (decimal)normalTheoryScore * 100.0m + bonusAchieve >= targetAchieve || item == NoteJudge.ETiming.Critical)
						{
							normalScore = num3;
							judgeNum[scoreTable.Length - num - 1]++;
							break;
						}
					}
					num++;
				}
			}
		}

		private NoteJudge.ETiming distoributionNotes(ref uint[] judgeNum, ref uint[] useJudgeNum, NoteJudge.ETiming[] scoreTable)
		{
			NoteJudge.ETiming result = NoteJudge.ETiming.TooLate;
			float num = 1000f;
			int num2 = 0;
			for (int i = 0; i < useJudgeNum.Length; i++)
			{
				if (judgeNum[i] != 0)
				{
					float num3 = (float)useJudgeNum[i] / (float)judgeNum[i];
					float num4 = (float)(useJudgeNum[i] + 1) / (float)judgeNum[i];
					float num5 = num3 + num4;
					if (num5 < num)
					{
						num = num5;
						result = scoreTable[i];
						num2 = i;
					}
				}
			}
			useJudgeNum[num2]++;
			return result;
		}

		private NoteScore.EScoreType convertType(NoteTypeID.Def type)
		{
			NoteScore.EScoreType result = NoteScore.EScoreType.End;
			switch (type)
			{
			case NoteTypeID.Def.Begin:
			case NoteTypeID.Def.Star:
			case NoteTypeID.Def.ExTap:
			case NoteTypeID.Def.ExStar:
				result = NoteScore.EScoreType.Tap;
				break;
			case NoteTypeID.Def.TouchTap:
				result = NoteScore.EScoreType.Touch;
				break;
			case NoteTypeID.Def.Hold:
			case NoteTypeID.Def.TouchHold:
			case NoteTypeID.Def.ExHold:
				result = NoteScore.EScoreType.Hold;
				break;
			case NoteTypeID.Def.Slide:
				result = NoteScore.EScoreType.Slide;
				break;
			case NoteTypeID.Def.Break:
			case NoteTypeID.Def.BreakStar:
				result = NoteScore.EScoreType.Break;
				break;
			}
			return result;
		}

		private bool IsEx(NoteTypeID.Def type)
		{
			if (type != NoteTypeID.Def.ExTap && type != NoteTypeID.Def.ExHold)
			{
				return type == NoteTypeID.Def.ExStar;
			}
			return true;
		}

		private bool IsEx(TotalType type)
		{
			if (type != TotalType.TT_NUM_EXT && type != TotalType.TT_NUM_EXH)
			{
				return type == TotalType.TT_NUM_EXS;
			}
			return true;
		}

		public void SetForceAchivementLowAP()
		{
			foreach (NoteData note in NotesManager.Instance(_monitorIndex).getReader().GetNoteList())
			{
				NoteJudge.ETiming timing = NoteJudge.ETiming.LatePerfect2nd;
				NoteScore.EScoreType kind = GamePlayManager.NoteType2ScoreType(note.type.getEnum());
				SetResult(note.indexNote, kind, timing);
			}
		}

		public void SetForceAchivement(int achivement, int dxscore)
		{
			decimal num = achivement;
			long num2;
			long num3;
			if (num > 100.0m)
			{
				num2 = (long)((decimal)ScoreTotal._totalData[43] * (num - 1.0m) * 0.01m);
				num3 = ScoreTotal._totalData[44];
			}
			else
			{
				num2 = (long)((decimal)ScoreTotal._totalData[43] * (num * 0.99m * 0.01m));
				num3 = (long)((decimal)ScoreTotal._totalData[44] * num * 0.01m);
			}
			NoteJudge.ETiming[] array = new NoteJudge.ETiming[7]
			{
				NoteJudge.ETiming.Critical,
				NoteJudge.ETiming.LatePerfect,
				NoteJudge.ETiming.LatePerfect2nd,
				NoteJudge.ETiming.LateGreat,
				NoteJudge.ETiming.LateGreat2nd,
				NoteJudge.ETiming.LateGreat3rd,
				NoteJudge.ETiming.LateGood
			};
			NoteDataList noteList = NotesManager.Instance(_monitorIndex).getReader().GetNoteList();
			foreach (NoteData item in noteList)
			{
				if (!(item.type == NoteTypeID.Def.Break) && !(item.type == NoteTypeID.Def.BreakStar))
				{
					continue;
				}
				bool flag = false;
				NoteJudge.ETiming[] array2 = array;
				foreach (NoteJudge.ETiming eTiming in array2)
				{
					NoteScore.EScoreType eScoreType = GamePlayManager.NoteType2ScoreType(item.type.getEnum());
					if (0m <= (decimal)(num2 - NoteScore.GetJudgeScore(eTiming, NoteScore.EScoreType.Break)) && 0m <= (decimal)(num3 - NoteScore.GetJudgeScore(eTiming, NoteScore.EScoreType.BreakBonus)))
					{
						num2 -= NoteScore.GetJudgeScore(eTiming, eScoreType);
						num3 -= NoteScore.GetJudgeScore(eTiming, NoteScore.EScoreType.BreakBonus);
						SetResult(item.indexNote, eScoreType, eTiming);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					SetResult(item.indexNote, NoteScore.EScoreType.Break, NoteJudge.ETiming.TooFast);
				}
			}
			int num4 = 0;
			int num5 = 0;
			long num6 = 0L;
			for (int j = 0; j < array.Length; j++)
			{
				long num7 = num2;
				long num8 = 0L;
				num8 += ScoreTotal._totalData[36] * NoteScore.GetJudgeScore(array[j]);
				num8 += ScoreTotal._totalData[38] * NoteScore.GetJudgeScore(array[j], NoteScore.EScoreType.Hold);
				num8 += ScoreTotal._totalData[39] * NoteScore.GetJudgeScore(array[j], NoteScore.EScoreType.Slide);
				num8 += ScoreTotal._totalData[40] * NoteScore.GetJudgeScore(array[j], NoteScore.EScoreType.Touch);
				if (num8 <= num7)
				{
					num6 = num7 - num8;
					num5 = ((num4 != 0) ? (num4 - 1) : 0);
					break;
				}
				num4++;
			}
			if (num4 >= array.Length)
			{
				num4 = array.Length - 1;
			}
			foreach (NoteData item2 in noteList)
			{
				if (item2.type == NoteTypeID.Def.Slide)
				{
					NoteScore.EScoreType eScoreType2 = GamePlayManager.NoteType2ScoreType(item2.type.getEnum());
					NoteJudge.ETiming eTiming2 = array[num4];
					NoteJudge.ETiming eTiming3 = array[num5];
					if (0m <= (decimal)num6 && 0m <= (decimal)(num2 - NoteScore.GetJudgeScore(eTiming3, eScoreType2)))
					{
						num6 -= NoteScore.GetJudgeScore(eTiming3, eScoreType2) - NoteScore.GetJudgeScore(eTiming2, eScoreType2);
						num2 -= NoteScore.GetJudgeScore(eTiming3, eScoreType2);
						SetResult(item2.indexNote, eScoreType2, eTiming3);
					}
					else if (0m <= (decimal)(num2 - NoteScore.GetJudgeScore(eTiming2, eScoreType2)))
					{
						num2 -= NoteScore.GetJudgeScore(eTiming2, eScoreType2);
						SetResult(item2.indexNote, eScoreType2, eTiming2);
					}
					else
					{
						SetResult(item2.indexNote, eScoreType2, NoteJudge.ETiming.TooFast);
					}
				}
			}
			foreach (NoteData item3 in noteList)
			{
				if (item3.type == NoteTypeID.Def.Hold || item3.type == NoteTypeID.Def.TouchHold || item3.type == NoteTypeID.Def.ExHold)
				{
					NoteScore.EScoreType eScoreType3 = GamePlayManager.NoteType2ScoreType(item3.type.getEnum());
					NoteJudge.ETiming eTiming4 = array[num4];
					NoteJudge.ETiming eTiming5 = array[num5];
					if (0m <= (decimal)num6 && 0m <= (decimal)(num2 - NoteScore.GetJudgeScore(eTiming5, eScoreType3)))
					{
						num6 -= NoteScore.GetJudgeScore(eTiming5, eScoreType3) - NoteScore.GetJudgeScore(eTiming4, eScoreType3);
						num2 -= NoteScore.GetJudgeScore(eTiming5, eScoreType3);
						SetResult(item3.indexNote, eScoreType3, eTiming5);
					}
					else if (0m <= (decimal)(num2 - NoteScore.GetJudgeScore(eTiming4, eScoreType3)))
					{
						num2 -= NoteScore.GetJudgeScore(eTiming4, eScoreType3);
						SetResult(item3.indexNote, eScoreType3, eTiming4);
					}
					else
					{
						SetResult(item3.indexNote, eScoreType3, NoteJudge.ETiming.TooFast);
					}
				}
			}
			foreach (NoteData item4 in noteList)
			{
				if (item4.type == NoteTypeID.Def.Begin || item4.type == NoteTypeID.Def.ExTap || item4.type == NoteTypeID.Def.Star || item4.type == NoteTypeID.Def.ExStar || item4.type == NoteTypeID.Def.TouchTap)
				{
					NoteScore.EScoreType eScoreType4 = GamePlayManager.NoteType2ScoreType(item4.type.getEnum());
					NoteJudge.ETiming eTiming6 = array[num4];
					NoteJudge.ETiming eTiming7 = array[num5];
					if (0m < (decimal)num6 && 0m <= (decimal)(num2 - NoteScore.GetJudgeScore(eTiming7, eScoreType4)))
					{
						num6 -= NoteScore.GetJudgeScore(eTiming7, eScoreType4) - NoteScore.GetJudgeScore(eTiming6, eScoreType4);
						num2 -= NoteScore.GetJudgeScore(eTiming7, eScoreType4);
						SetResult(item4.indexNote, eScoreType4, eTiming7);
					}
					else if (0m <= (decimal)(num2 - NoteScore.GetJudgeScore(eTiming6, eScoreType4)))
					{
						num2 -= NoteScore.GetJudgeScore(eTiming6, eScoreType4);
						SetResult(item4.indexNote, eScoreType4, eTiming6);
					}
					else if (0m < (decimal)num2)
					{
						num2 -= NoteScore.GetJudgeScore(eTiming6, eScoreType4);
						SetResult(item4.indexNote, eScoreType4, eTiming6);
					}
					else
					{
						SetResult(item4.indexNote, eScoreType4, NoteJudge.ETiming.TooFast);
					}
				}
			}
		}

		public void SetGhostData(UserGhost ghost)
		{
			NoteDataList noteList = NotesManager.Instance(_monitorIndex).getReader().GetNoteList();
			IsEnable = true;
			UserID = ghost.Id;
			UserName = ghost.Name;
			UserType = UserData.UserIDType.Ghost;
			for (int i = 0; i < noteList.Count; i++)
			{
				NoteScore.EScoreType kind = NoteScore.EScoreType.Tap;
				switch (noteList[i].type.getEnum())
				{
				case NoteTypeID.Def.Begin:
				case NoteTypeID.Def.Star:
				case NoteTypeID.Def.ExTap:
				case NoteTypeID.Def.ExStar:
					kind = NoteScore.EScoreType.Tap;
					break;
				case NoteTypeID.Def.TouchTap:
					kind = NoteScore.EScoreType.Touch;
					break;
				case NoteTypeID.Def.Hold:
				case NoteTypeID.Def.TouchHold:
				case NoteTypeID.Def.ExHold:
					kind = NoteScore.EScoreType.Hold;
					break;
				case NoteTypeID.Def.Slide:
					kind = NoteScore.EScoreType.Slide;
					break;
				case NoteTypeID.Def.Break:
				case NoteTypeID.Def.BreakStar:
					kind = NoteScore.EScoreType.Break;
					break;
				}
				SetResult(noteList[i].indexNote, kind, (NoteJudge.ETiming)ghost.GetResultIndexTo(i));
			}
		}

		public void SetResult(int index, NoteScore.EScoreType kind, NoteJudge.ETiming timing)
		{
			if (!IsEnable)
			{
				return;
			}
			NotesReader reader = NotesManager.Instance(_monitorIndex).getReader();
			NoteData noteData = reader.GetNoteList()[index];
			if (noteData.isJudged)
			{
				return;
			}
			noteData.isJudged = true;
			if (IsTrackSkip)
			{
				timing = NoteJudge.ETiming.TooLate;
				if (IsChallenge)
				{
					Life = 0u;
				}
			}
			if (kind == NoteScore.EScoreType.Touch && noteData.indexTouchGroup != -1)
			{
				List<TouchChainList> touchChainList = reader.GetTouchChainList();
				touchChainList[noteData.indexTouchGroup].EndCount++;
				touchChainList[noteData.indexTouchGroup].ChainJudge = timing;
			}
			_judgeResultList[index].UpdateScore(_monitorIndex, kind, timing);
			if (timing == NoteJudge.ETiming.TooFast || timing == NoteJudge.ETiming.TooLate)
			{
				Combo = 0u;
				ResetCombo = true;
				ComboAddThisFrame = 0u;
			}
			else
			{
				Combo++;
				ComboAddThisFrame++;
			}
			if (MaxCombo < Combo)
			{
				MaxCombo = Combo;
			}
			switch (UserOption.DispJudge)
			{
			case OptionDispjudgeID.Type3B:
			case OptionDispjudgeID.Type3C:
			case OptionDispjudgeID.Type3D:
				switch (timing)
				{
				case NoteJudge.ETiming.FastGood:
				case NoteJudge.ETiming.FastGreat3rd:
				case NoteJudge.ETiming.FastGreat2nd:
				case NoteJudge.ETiming.FastGreat:
				case NoteJudge.ETiming.FastPerfect2nd:
				case NoteJudge.ETiming.FastPerfect:
					Fast++;
					break;
				case NoteJudge.ETiming.LatePerfect:
				case NoteJudge.ETiming.LatePerfect2nd:
				case NoteJudge.ETiming.LateGreat:
				case NoteJudge.ETiming.LateGreat2nd:
				case NoteJudge.ETiming.LateGreat3rd:
				case NoteJudge.ETiming.LateGood:
					Late++;
					break;
				}
				break;
			case OptionDispjudgeID.Type1E:
			case OptionDispjudgeID.Type2E:
				switch (timing)
				{
				case NoteJudge.ETiming.FastGood:
				case NoteJudge.ETiming.FastGreat3rd:
				case NoteJudge.ETiming.FastGreat2nd:
				case NoteJudge.ETiming.FastGreat:
					Fast++;
					break;
				case NoteJudge.ETiming.LateGreat:
				case NoteJudge.ETiming.LateGreat2nd:
				case NoteJudge.ETiming.LateGreat3rd:
				case NoteJudge.ETiming.LateGood:
					Late++;
					break;
				case NoteJudge.ETiming.FastPerfect2nd:
				case NoteJudge.ETiming.FastPerfect:
					if (kind == NoteScore.EScoreType.Break)
					{
						Fast++;
					}
					break;
				case NoteJudge.ETiming.LatePerfect:
				case NoteJudge.ETiming.LatePerfect2nd:
					if (kind == NoteScore.EScoreType.Break)
					{
						Late++;
					}
					break;
				case NoteJudge.ETiming.Critical:
					break;
				}
				break;
			default:
				switch (timing)
				{
				case NoteJudge.ETiming.FastGood:
				case NoteJudge.ETiming.FastGreat3rd:
				case NoteJudge.ETiming.FastGreat2nd:
				case NoteJudge.ETiming.FastGreat:
					Fast++;
					break;
				case NoteJudge.ETiming.LateGreat:
				case NoteJudge.ETiming.LateGreat2nd:
				case NoteJudge.ETiming.LateGreat3rd:
				case NoteJudge.ETiming.LateGood:
					Late++;
					break;
				}
				break;
			}
		}

		public uint GetJudgeNumOfJudgeType(NoteJudge.ETiming timing)
		{
			if (!IsEnable)
			{
				return 0u;
			}
			uint num = 0u;
			JudgeResultSt[] judgeResultList = _judgeResultList;
			for (int i = 0; i < judgeResultList.Length; i++)
			{
				JudgeResultSt judgeResultSt = judgeResultList[i];
				if (judgeResultSt.Judged && judgeResultSt.Timing == (int)timing)
				{
					num++;
				}
			}
			return num;
		}

		public uint GetDeluxeScoreAll()
		{
			if (!IsEnable)
			{
				return 0u;
			}
			return DxScore;
		}

		public int GetDeluxeScoreMinusAll()
		{
			if (!IsEnable)
			{
				return 0;
			}
			return (int)(DxScore - TheoryDxScore);
		}

		public decimal GetAchivement()
		{
			if (!IsEnable)
			{
				return 0m;
			}
			return Achivement;
		}

		public decimal GetDecAchivement()
		{
			if (!IsEnable)
			{
				return 0m;
			}
			return 100.0m - GetTheoryAchivementIgnoreBreakDiff() + GetAchivementBreak();
		}

		public decimal GetDecTheoryAchivement()
		{
			if (!IsEnable)
			{
				return 0m;
			}
			return 101.0m - GetTheoryAchivementDiff();
		}

		public decimal GetAchivementIgnoreBreak()
		{
			if (!IsEnable)
			{
				return 0m;
			}
			return AchivementIgnoreBreak;
		}

		public decimal GetAchivementBreak()
		{
			if (!IsEnable)
			{
				return 0m;
			}
			return AchivementBreakOnly;
		}

		public decimal GetTheoryAchivement()
		{
			if (!IsEnable)
			{
				return 0m;
			}
			return TheoryIgnoreBreakAchivement + TheoryBreakAchivement;
		}

		public decimal GetTheoryAchivementIgnoreBreak()
		{
			if (!IsEnable)
			{
				return 0m;
			}
			return TheoryIgnoreBreakAchivement;
		}

		public decimal GetTheoryAchivementBreak()
		{
			if (!IsEnable)
			{
				return 0m;
			}
			return TheoryBreakAchivement;
		}

		public decimal GetTheoryAchivementDiff()
		{
			if (!IsEnable)
			{
				return 0m;
			}
			return GetTheoryAchivement() - GetAchivement();
		}

		public decimal GetTheoryAchivementIgnoreBreakDiff()
		{
			if (!IsEnable)
			{
				return 0m;
			}
			return GetTheoryAchivementIgnoreBreak() - GetAchivementIgnoreBreak();
		}

		public decimal GetTheoryAchivementBreakDiff()
		{
			if (!IsEnable)
			{
				return 0m;
			}
			return GetAchivementBreak() - GetAchivementBreak();
		}

		public JudgeResultSt GetScoreAt(int index)
		{
			return _judgeResultList[index];
		}

		public int GetScoreLength()
		{
			return _judgeResultList.Length;
		}

		public void Initialize(int monitorIndex, bool isParty)
		{
			NotesManager notesManager = NotesManager.Instance(monitorIndex);
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorIndex);
			IsEnable = true;
			UserID = userData.Detail.UserID;
			UserName = userData.Detail.UserName;
			UserType = userData.UserType;
			Achivement = 0m;
			AchivementIgnoreBreak = 0m;
			AchivementBreakOnly = 0m;
			TheoryIgnoreBreakAchivement = 0m;
			TheoryBreakAchivement = 0m;
			TotalScore = 0u;
			TheoryScore = 0u;
			DxScore = 0u;
			TheoryDxScore = 0u;
			VsRank = 0u;
			ResetCombo = true;
			ComboAddThisFrame = 0u;
			Combo = 0u;
			MaxCombo = 0u;
			ComboType = PlayComboflagID.None;
			SyncType = PlaySyncflagID.None;
			Chain = 0u;
			MaxChain = 0u;
			Life = 0u;
			IsChallenge = false;
			IsCourse = false;
			IsCourseForceFail = false;
			IsLifeZero = false;
			_prevCriticalNum = 0u;
			_prevPerfectNum = 0u;
			_prevGreatNum = 0u;
			_prevGoodNum = 0u;
			_prevMissNum = 0u;
			_nowCriticalNum = 0u;
			_nowPerfectNum = 0u;
			_nowGreatNum = 0u;
			_nowGoodNum = 0u;
			_nowMissNum = 0u;
			MusicRate = 0u;
			DanRate = 0u;
			UserOption = new UserOption(userData.Option);
			userData.Detail.CharaSlot.CopyTo(CharaSlot, 0);
			int i;
			for (i = 0; i < CharaSlot.Length; i++)
			{
				CharaLevel[i] = 0u;
				CharaAwake[i] = 0u;
				if (CharaSlot[i] == 0)
				{
					continue;
				}
				try
				{
					UserChara userChara = userData.CharaList.FirstOrDefault((UserChara x) => x.ID == CharaSlot[i]);
					CharaLevel[i] = userChara.Level;
					CharaAwake[i] = userChara.Awakening;
				}
				catch
				{
				}
			}
			if (GameManager.IsTutorial)
			{
				UserOption.InitializeTutorial();
			}
			notesManager.initialize();
			MusicDifficultyID musicDifficultyID = (MusicDifficultyID)GameManager.SelectDifficultyID[monitorIndex];
			if ((uint)musicDifficultyID <= 4u)
			{
				SessionInfo = new SessionInfo
				{
					musicId = GameManager.SelectMusicID[monitorIndex],
					notesData = ((4 >= GameManager.SelectMusicID[monitorIndex]) ? null : Singleton<DataManager>.Instance.GetMusic(GameManager.SelectMusicID[monitorIndex]).notesData[GameManager.SelectDifficultyID[monitorIndex]]),
					difficulty = GameManager.SelectDifficultyID[monitorIndex],
					isTutorial = GameManager.IsTutorial,
					isAdvDemo = GameManager.IsAdvDemo
				};
			}
			TrackNo = (int)GameManager.MusicTrackNumber;
			notesManager.loadScore(SessionInfo);
			ScoreTotal = notesManager.getReader().GetTotal();
			PreAchivement = 0m;
			PreDxScore = 0u;
			UserScore userScore = userData.ScoreList[SessionInfo.difficulty].Find((UserScore item) => item.id == SessionInfo.musicId);
			if (userScore != null)
			{
				PreAchivement = (int)userScore.achivement;
				PreDxScore = userScore.deluxscore;
			}
			PreUdemaeValue = (uint)userData.RatingList.Udemae.ClassValue;
			PreClassValue = (uint)userData.RatingList.Udemae.ClassValue;
			PreDan = userData.RatingList.Udemae.ClassID;
			MaxDan = userData.RatingList.Udemae.MaxClassID;
			PreMusicRate = userData.Detail.MusicRating;
			PreDanRate = userData.Detail.GradeRating;
			if (!IsHuman())
			{
				ClassValue = (uint)userData.RatingList.Udemae.ClassValue;
				MusicRate = userData.Detail.Rating;
				DanRate = 0u;
			}
			IsTrackSkip = false;
			_judgeResultList = new JudgeResultSt[ScoreTotal._totalData[41]];
			for (int j = 0; j < _judgeResultList.Length; j++)
			{
				_judgeResultList[j].Judged = false;
			}
			for (int k = 0; k < 6; k++)
			{
				for (int l = 0; l < 15; l++)
				{
					_resultList[k, l] = 0u;
				}
			}
			_totalJudgeList[0] = ScoreTotal._totalData[36];
			_totalJudgeList[3] = ScoreTotal._totalData[37];
			_totalJudgeList[1] = ScoreTotal._totalData[38];
			_totalJudgeList[2] = ScoreTotal._totalData[39];
			_totalJudgeList[4] = ScoreTotal._totalData[40];
			TheoryCombo = ScoreTotal._totalData[41];
			TheoryScore = ScoreTotal._totalData[45];
			UnixTime = TimeManager.GetNowUnixTime();
			if (isParty)
			{
				if (Manager.Party.Party.Party.Get().IsHost())
				{
					PlayerIndex = monitorIndex;
				}
				else if (monitorIndex < 2)
				{
					PlayerIndex = monitorIndex + 2;
				}
				else
				{
					PlayerIndex = monitorIndex - 2;
				}
			}
			else
			{
				PlayerIndex = monitorIndex;
			}
		}

		public void SetSyncResult()
		{
			_ = ComboType;
		}

		public void CalcUpdate()
		{
			if (!IsEnable || (Singleton<UserDataManager>.Instance.GetUserData(_monitorIndex).IsHuman() && _monitorIndex >= 2))
			{
				return;
			}
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 15; j++)
				{
					_resultList[i, j] = 0u;
				}
			}
			uint num = 0u;
			uint num2 = 0u;
			uint num3 = 0u;
			uint num4 = 0u;
			uint num5 = 0u;
			JudgeResultSt[] judgeResultList = _judgeResultList;
			for (int k = 0; k < judgeResultList.Length; k++)
			{
				JudgeResultSt judgeResultSt = judgeResultList[k];
				_resultList[(int)judgeResultSt.Type, judgeResultSt.Timing]++;
				num += judgeResultSt.Score;
				num3 += judgeResultSt.TheoryScore;
				num2 += judgeResultSt.ScoreBonus;
				num4 += judgeResultSt.TheoryScoreBonus;
				num5 += judgeResultSt.Deluxe;
			}
			ComboType = PlayComboflagID.None;
			CriticalNum = GetJudgeNumOfJudgeType(NoteJudge.ETiming.Critical);
			TrueCriticalNum = CriticalNum;
			PerfectNum = GetJudgeNumOfJudgeType(NoteJudge.ETiming.FastPerfect2nd) + GetJudgeNumOfJudgeType(NoteJudge.ETiming.FastPerfect) + GetJudgeNumOfJudgeType(NoteJudge.ETiming.LatePerfect) + GetJudgeNumOfJudgeType(NoteJudge.ETiming.LatePerfect2nd);
			TruePerfectNum = PerfectNum;
			GreatNum = GetJudgeNumOfJudgeType(NoteJudge.ETiming.FastGreat3rd) + GetJudgeNumOfJudgeType(NoteJudge.ETiming.FastGreat2nd) + GetJudgeNumOfJudgeType(NoteJudge.ETiming.FastGreat) + GetJudgeNumOfJudgeType(NoteJudge.ETiming.LateGreat3rd) + GetJudgeNumOfJudgeType(NoteJudge.ETiming.LateGreat2nd) + GetJudgeNumOfJudgeType(NoteJudge.ETiming.LateGreat);
			GoodNum = GetJudgeNumOfJudgeType(NoteJudge.ETiming.FastGood) + GetJudgeNumOfJudgeType(NoteJudge.ETiming.LateGood);
			MissNum = GetJudgeNumOfJudgeType(NoteJudge.ETiming.TooFast) + GetJudgeNumOfJudgeType(NoteJudge.ETiming.TooLate);
			uint num6 = CriticalNum + PerfectNum + GreatNum + GoodNum + MissNum;
			if (!UserOption.DispJudge.IsCritical())
			{
				PerfectNum += CriticalNum;
				CriticalNum = 0u;
			}
			AchivementIgnoreBreak = 0m;
			AchivementBreakOnly = 0m;
			TheoryIgnoreBreakAchivement = 0m;
			TheoryBreakAchivement = 0m;
			if (ScoreTotal._totalData[43] != 0)
			{
				if (ScoreTotal._totalData[44] == 0)
				{
					AchivementIgnoreBreak = (decimal)num / (decimal)ScoreTotal._totalData[43] * 100.0m;
					TheoryIgnoreBreakAchivement = (decimal)num3 / (decimal)ScoreTotal._totalData[43] * 100.0m;
				}
				else
				{
					AchivementIgnoreBreak = (decimal)num / (decimal)ScoreTotal._totalData[43] * 100.0m;
					AchivementBreakOnly = (decimal)num2 / (decimal)ScoreTotal._totalData[44];
					TheoryIgnoreBreakAchivement = (decimal)num3 / (decimal)ScoreTotal._totalData[43] * 100.0m;
					TheoryBreakAchivement = (decimal)num4 / (decimal)ScoreTotal._totalData[44];
				}
			}
			Achivement = AchivementIgnoreBreak + AchivementBreakOnly;
			TotalScore = num + num2;
			TheoryScore = num3 + num4;
			DxScore = num5;
			TheoryDxScore = num6 * 3;
			if (MissNum == 0 && TheoryCombo != 0)
			{
				if (GetAchivement() == 101.0m || (ScoreTotal._totalData[44] == 0 && GetAchivement() == 100.0m))
				{
					ComboType = PlayComboflagID.AllPerfectPlus;
				}
				else if (TheoryCombo == PerfectNum + CriticalNum)
				{
					ComboType = PlayComboflagID.AllPerfect;
				}
				else if (TheoryCombo == PerfectNum + CriticalNum + GreatNum)
				{
					ComboType = PlayComboflagID.Gold;
				}
				else if (TheoryCombo == PerfectNum + CriticalNum + GreatNum + GoodNum)
				{
					ComboType = PlayComboflagID.Silver;
				}
			}
			CourseData courseData = Singleton<CourseManager>.Instance.GetCourseData();
			if (IsCourse && courseData != null)
			{
				_prevCriticalNum = _nowCriticalNum;
				_prevPerfectNum = _nowPerfectNum;
				_prevGreatNum = _nowGreatNum;
				_prevGoodNum = _nowGoodNum;
				_prevMissNum = _nowMissNum;
				_nowCriticalNum = CriticalNum;
				_nowPerfectNum = PerfectNum;
				_nowGreatNum = GreatNum;
				_nowGoodNum = GoodNum;
				_nowMissNum = MissNum;
				uint num7 = 0u;
				uint num8 = 0u;
				uint num9 = 0u;
				uint num10 = 0u;
				if (_nowCriticalNum > _prevCriticalNum)
				{
					_ = _nowCriticalNum;
					_ = _prevCriticalNum;
				}
				if (_nowPerfectNum > _prevPerfectNum)
				{
					num7 = _nowPerfectNum - _prevPerfectNum;
					num7 *= (uint)courseData.perfectDamage;
				}
				if (_nowGreatNum > _prevGreatNum)
				{
					num8 = _nowGreatNum - _prevGreatNum;
					num8 *= (uint)courseData.greatDamage;
				}
				if (_nowGoodNum > _prevGoodNum)
				{
					num9 = _nowGoodNum - _prevGoodNum;
					num9 *= (uint)courseData.goodDamage;
				}
				if (_nowMissNum > _prevMissNum)
				{
					num10 = _nowMissNum - _prevMissNum;
					num10 *= (uint)courseData.missDamage;
				}
				uint num11 = num7 + num8 + num9 + num10;
				if (num11 != 0)
				{
					if (num11 > Life)
					{
						Life = 0u;
					}
					else
					{
						Life -= num11;
					}
					int num12 = 0;
					num12 = ((PlayerIndex % 2 != 0) ? 1 : 0);
					if (!IsLifeZero)
					{
						OptionVolumeID damageSeVolume = Singleton<GamePlayManager>.Instance.GetGameScore(num12).UserOption.DamageSeVolume;
						SoundManager.PlayGameSE(Cue.SE_GAME_DAMAGE_01, num12, damageSeVolume.GetValue());
					}
					if (!IsLifeZero && Life == 0)
					{
						IsLifeZero = true;
					}
				}
			}
			if (IsChallenge)
			{
				_prevCriticalNum = _nowCriticalNum;
				_prevPerfectNum = _nowPerfectNum;
				_prevGreatNum = _nowGreatNum;
				_prevGoodNum = _nowGoodNum;
				_prevMissNum = _nowMissNum;
				_nowCriticalNum = CriticalNum;
				_nowPerfectNum = PerfectNum;
				_nowGreatNum = GreatNum;
				_nowGoodNum = GoodNum;
				_nowMissNum = MissNum;
				uint num13 = 0u;
				uint num14 = 0u;
				uint num15 = 0u;
				if (_nowCriticalNum > _prevCriticalNum)
				{
					_ = _nowCriticalNum;
					_ = _prevCriticalNum;
				}
				if (_nowPerfectNum > _prevPerfectNum)
				{
					_ = _nowPerfectNum;
					_ = _prevPerfectNum;
				}
				if (_nowGreatNum > _prevGreatNum)
				{
					num13 = _nowGreatNum - _prevGreatNum;
				}
				if (_nowGoodNum > _prevGoodNum)
				{
					num14 = _nowGoodNum - _prevGoodNum;
				}
				if (_nowMissNum > _prevMissNum)
				{
					num15 = _nowMissNum - _prevMissNum;
				}
				uint num16 = (num13 + num14 + num15) * _missPoints;
				if (num16 != 0)
				{
					if (num16 > Life)
					{
						Life = 0u;
					}
					else
					{
						Life -= num16;
					}
					int num17 = 0;
					num17 = ((PlayerIndex % 2 != 0) ? 1 : 0);
					if (!IsLifeZero)
					{
						OptionVolumeID damageSeVolume2 = Singleton<GamePlayManager>.Instance.GetGameScore(num17).UserOption.DamageSeVolume;
						SoundManager.PlayGameSE(Cue.SE_GAME_DAMAGE_01, num17, damageSeVolume2.GetValue());
					}
					if (!IsLifeZero && Life == 0)
					{
						IsLifeZero = true;
					}
				}
			}
			IsClear = GameManager.GetClearRank(GetAchivement()) >= MusicClearrankID.Rank_A;
		}

		public void UpdateAfter()
		{
			if (IsEnable)
			{
				ComboAddThisFrame = 0u;
				ResetCombo = false;
			}
		}

		public uint GetJudgeNum(NoteScore.EScoreType kind, NoteJudge.ETiming timing)
		{
			if (!IsEnable)
			{
				return 0u;
			}
			return _resultList[(int)kind, (int)timing];
		}

		public uint GetJudgeTotalNum(NoteScore.EScoreType kind)
		{
			if (!IsEnable)
			{
				return 0u;
			}
			return _totalJudgeList[(int)kind];
		}

		public uint GetJudgeNum(NoteScore.EScoreType kind, NoteJudge.JudgeBox timing)
		{
			if (!IsEnable)
			{
				return 0u;
			}
			switch (timing)
			{
			case NoteJudge.JudgeBox.Miss:
				return _resultList[(int)kind, 0] + _resultList[(int)kind, 14];
			case NoteJudge.JudgeBox.Good:
				return _resultList[(int)kind, 1] + _resultList[(int)kind, 13];
			case NoteJudge.JudgeBox.Great:
				return _resultList[(int)kind, 2] + _resultList[(int)kind, 3] + _resultList[(int)kind, 4] + _resultList[(int)kind, 10] + _resultList[(int)kind, 11] + _resultList[(int)kind, 12];
			case NoteJudge.JudgeBox.Perfect:
				if (UserOption.DispJudge.IsCritical())
				{
					return _resultList[(int)kind, 5] + _resultList[(int)kind, 6] + _resultList[(int)kind, 8] + _resultList[(int)kind, 9];
				}
				return _resultList[(int)kind, 5] + _resultList[(int)kind, 6] + _resultList[(int)kind, 8] + _resultList[(int)kind, 9] + _resultList[(int)kind, 7];
			case NoteJudge.JudgeBox.Critical:
				if (UserOption.DispJudge.IsCritical())
				{
					return _resultList[(int)kind, 7];
				}
				return 0u;
			default:
				return 0u;
			}
		}

		public uint GetJudgeBreakNum(NoteJudge.JudgeBox timing)
		{
			NoteScore.EScoreType eScoreType = NoteScore.EScoreType.Break;
			if (!IsEnable)
			{
				return 0u;
			}
			return timing switch
			{
				NoteJudge.JudgeBox.Miss => _resultList[(int)eScoreType, 0] + _resultList[(int)eScoreType, 14], 
				NoteJudge.JudgeBox.Good => _resultList[(int)eScoreType, 1] + _resultList[(int)eScoreType, 13], 
				NoteJudge.JudgeBox.Great => _resultList[(int)eScoreType, 2] + _resultList[(int)eScoreType, 3] + _resultList[(int)eScoreType, 4] + _resultList[(int)eScoreType, 10] + _resultList[(int)eScoreType, 11] + _resultList[(int)eScoreType, 12], 
				NoteJudge.JudgeBox.Perfect => _resultList[(int)eScoreType, 5] + _resultList[(int)eScoreType, 6] + _resultList[(int)eScoreType, 8] + _resultList[(int)eScoreType, 9], 
				NoteJudge.JudgeBox.Critical => _resultList[(int)eScoreType, 7], 
				_ => 0u, 
			};
		}

		public void FinishPlay()
		{
			if (!IsEnable)
			{
				return;
			}
			foreach (NoteData note in NotesManager.Instance(_monitorIndex).getReader().GetNoteList())
			{
				if (!note.isJudged)
				{
					NoteScore.EScoreType kind = NoteScore.EScoreType.Tap;
					switch (note.type.getEnum())
					{
					case NoteTypeID.Def.Begin:
					case NoteTypeID.Def.Star:
					case NoteTypeID.Def.ExTap:
					case NoteTypeID.Def.ExStar:
						kind = NoteScore.EScoreType.Tap;
						break;
					case NoteTypeID.Def.TouchTap:
						kind = NoteScore.EScoreType.Touch;
						break;
					case NoteTypeID.Def.Hold:
					case NoteTypeID.Def.TouchHold:
					case NoteTypeID.Def.ExHold:
						kind = NoteScore.EScoreType.Hold;
						break;
					case NoteTypeID.Def.Slide:
						kind = NoteScore.EScoreType.Slide;
						break;
					case NoteTypeID.Def.Break:
					case NoteTypeID.Def.BreakStar:
						kind = NoteScore.EScoreType.Break;
						break;
					}
					SetResult(note.indexNote, kind, NoteJudge.ETiming.TooLate);
				}
			}
			CalcUpdate();
		}

		public void FinishPlayGhost()
		{
			if (!IsEnable || IsHuman())
			{
				return;
			}
			NotesReader reader = NotesManager.Instance(_monitorIndex).getReader();
			UserGhost myGhost = Singleton<UserDataManager>.Instance.GetUserData(2L).MyGhost;
			foreach (NoteData note in reader.GetNoteList())
			{
				if (!note.isJudged)
				{
					NoteScore.EScoreType kind = NoteScore.EScoreType.Tap;
					switch (note.type.getEnum())
					{
					case NoteTypeID.Def.Begin:
					case NoteTypeID.Def.Star:
					case NoteTypeID.Def.ExTap:
					case NoteTypeID.Def.ExStar:
						kind = NoteScore.EScoreType.Tap;
						break;
					case NoteTypeID.Def.TouchTap:
						kind = NoteScore.EScoreType.Touch;
						break;
					case NoteTypeID.Def.Hold:
					case NoteTypeID.Def.TouchHold:
					case NoteTypeID.Def.ExHold:
						kind = NoteScore.EScoreType.Hold;
						break;
					case NoteTypeID.Def.Slide:
						kind = NoteScore.EScoreType.Slide;
						break;
					case NoteTypeID.Def.Break:
					case NoteTypeID.Def.BreakStar:
						kind = NoteScore.EScoreType.Break;
						break;
					}
					SetResult(note.indexNote, kind, (NoteJudge.ETiming)myGhost.GetResultIndexTo(note.indexNote));
				}
			}
			CalcUpdate();
			for (int i = 0; i < 2; i++)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry && GameManager.SelectGhostID[i] != GhostManager.GhostTarget.End)
				{
					UserGhost ghostToEnum = Singleton<GhostManager>.Instance.GetGhostToEnum(GameManager.SelectGhostID[i]);
					if (ghostToEnum != null)
					{
						ghostToEnum.Achievement = GameManager.ConvAchiveDecimalToInt(Achivement);
					}
					break;
				}
			}
		}

		public bool IsAllJudged()
		{
			if (!IsEnable)
			{
				return true;
			}
			int num = 0;
			JudgeResultSt[] judgeResultList = _judgeResultList;
			for (int i = 0; i < judgeResultList.Length; i++)
			{
				if (judgeResultList[i].Judged)
				{
					num++;
				}
			}
			return num == _judgeResultList.Length;
		}

		public bool SetChain(uint value)
		{
			Chain = value;
			if (MaxChain < Chain)
			{
				MaxChain = Chain;
			}
			return true;
		}

		public bool SetLife(uint value)
		{
			Life = value;
			return true;
		}

		public bool SetStartLife(uint value)
		{
			StartLife = value;
			if (StartLife == 0)
			{
				IsLifeZero = true;
			}
			return true;
		}

		public void SetPlayAfterRate(int musicRate, int danRate, UdemaeID dan, int classValue)
		{
			MusicRate = (uint)musicRate;
			DanRate = (uint)danRate;
			Dan = dan;
			ClassValue = (uint)classValue;
			if (MaxDan < Dan)
			{
				MaxDan = Dan;
			}
		}

		public void AddRival(int rivalID, string name, int difficulty)
		{
			Rival.Add(new RivalData(rivalID, name, difficulty));
		}

		public void SetTrackSkip()
		{
			IsTrackSkip = true;
		}

		public void OverridePartyScore(int achieve, int rank, int chain, int maxChain, int comboType, bool trackSkip)
		{
			Achivement = GameManager.ConvAchiveIntToDecimal(achieve);
			VsRank = (uint)rank;
			Chain = (uint)chain;
			MaxChain = (uint)maxChain;
			ComboType = (PlayComboflagID)comboType;
			IsTrackSkip = trackSkip;
		}

		public void DebugSetData(int playerIndex, int trackIndex, DebugGameScoreList scoreList, bool isParty)
		{
			Initialize(playerIndex, isParty);
			DebugGameScoreListData debugGameScoreListData = scoreList.GameScoreData[trackIndex];
			PerfectNum = debugGameScoreListData.Perfect;
			GreatNum = debugGameScoreListData.Great;
			GoodNum = debugGameScoreListData.Good;
			MissNum = debugGameScoreListData.Miss;
			Late = debugGameScoreListData.Late;
			Fast = debugGameScoreListData.Fast;
			Combo = debugGameScoreListData.Combo;
			DxScore = debugGameScoreListData.DxScore;
			MaxCombo = debugGameScoreListData.Combo;
			MaxChain = debugGameScoreListData.Chain;
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(playerIndex);
			UserOption = new UserOption(userData.Option);
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < 15; j++)
				{
					_resultList[i, j] = 0u;
				}
			}
		}
	}
}
