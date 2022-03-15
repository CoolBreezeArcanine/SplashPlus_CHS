using System;

namespace Manager
{
	public static class FumenUtil
	{
		private static string[] tagTotal = new string[46]
		{
			"TT_REC_TAP", "TT_REC_BRK", "TT_REC_HLD", "TT_REC_STR", "TT_REC_BST", "TT_REC_SLD", "TT_REC_TCT", "TT_REC_TCH", "TT_REC_EXT", "TT_REC_EXH",
			"TT_REC_EXS", "TT_REC_ALL", "TT_NOTE_TAP", "TT_NOTE_BRK", "TT_NOTE_HLD", "TT_NOTE_STR", "TT_NOTE_BST", "TT_NOTE_SLD", "TT_NOTE_TCT", "TT_NOTE_TCH",
			"TT_NOTE_EXT", "TT_NOTE_EXH", "TT_NOTE_EXS", "TT_NOTE_ALL", "TT_JUDGE_TAP", "TT_JUDGE_BRK", "TT_JUDGE_HLD", "TT_JUDGE_STR", "TT_JUDGE_BST", "TT_JUDGE_SLD",
			"TT_JUDGE_TCT", "TT_JUDGE_TCH", "TT_JUDGE_EXT", "TT_JUDGE_EXH", "TT_JUDGE_EXS", "TT_JUDGE_ALL", "TT_NUM_TAP", "TT_NUM_BRK", "TT_NUM_HLD", "TT_NUM_SLD",
			"TT_NUM_TCT", "TT_NUM_ALL", "TT_EACHPAIRS", "TT_SCR_NOTE", "TT_SCR_BRK", "TT_SCR_ALL"
		};

		public static M2sfileParamID getParamIDFromRec(M2sfileRecordID rec, int index)
		{
			M2sfileParamID result = new M2sfileParamID(M2sfileParamID.Def.Invalid);
			if (!rec.isValid())
			{
				return result;
			}
			if (index < 0 || index >= rec.getParamNum())
			{
				return result;
			}
			switch (index)
			{
			case 0:
				result = new M2sfileParamID(M2sfileParamID.Def.Begin);
				break;
			case 1:
				return rec.getParam2();
			case 2:
				return rec.getParam3();
			case 3:
				return rec.getParam4();
			case 4:
				return rec.getParam5();
			case 5:
				return rec.getParam6();
			case 6:
				return rec.getParam7();
			case 7:
				return rec.getParam8();
			}
			return result;
		}

		public static M2sfileRecordID tag2Rec(string tag)
		{
			return M2sfileRecordID.findID(tag);
		}

		public static M2sfileNotetypeID tag2Type(string tag)
		{
			M2sfileNotetypeID m2sfileNotetypeID = M2sfileNotetypeID.findID(tag);
			if (m2sfileNotetypeID.isValid())
			{
				return m2sfileNotetypeID;
			}
			return null;
		}

		public static bool tag2Check(string tag)
		{
			return M2sfileNotetypeID.findID(tag).isValid();
		}

		public static string m2s_VERSION(int rom_major, int rom_minor, int rom_release, int conv_major, int conv_minor, int conv_release)
		{
			return string.Format("VERSION\t{0}.{1:00}.{2:00}\t{3}.{4:00}.{5:00}" + Environment.NewLine, rom_major, rom_minor, rom_release, conv_major, conv_minor, conv_release);
		}

		public static string m2s_MUSIC(int musicID)
		{
			return string.Format("MUSIC\t{0}" + Environment.NewLine, musicID);
		}

		public static string m2s_SEQUENCEID(int sequenceID)
		{
			return string.Format("SEQUENCEID\t{0}" + Environment.NewLine, sequenceID);
		}

		public static string m2s_DIFFICULT(string difficulty)
		{
			return string.Format("DIFFICULT\t{0}" + Environment.NewLine, difficulty);
		}

		public static string m2s_LEVEL(float level)
		{
			return string.Format("LEVEL\t{0:f1}" + Environment.NewLine, level);
		}

		public static string m2s_CREATOR(string creator)
		{
			return string.Format("CREATOR\t{0}" + Environment.NewLine, creator);
		}

		public static string m2s_BPM_DEF(float firstBPM, float defaultBPM, float maxBPM, float minBPM)
		{
			return string.Format("BPM_DEF\t{0:f3}\t{1:f3}\t{2:f3}\t{3:f3}" + Environment.NewLine, firstBPM, defaultBPM, maxBPM, minBPM);
		}

		public static string m2s_MET_DEF(uint denomi, uint num)
		{
			return string.Format("MET_DEF\t{0}\t{1}" + Environment.NewLine, denomi, num);
		}

		public static string m2s_RESOLUTION(uint resolution)
		{
			return string.Format("RESOLUTION\t{0}" + Environment.NewLine, resolution);
		}

		public static string m2s_CLK_DEF(uint clickFirst)
		{
			return string.Format("CLK_DEF\t{0}" + Environment.NewLine, clickFirst);
		}

		public static string m2s_PROGJUDGE_BPM(float bpm)
		{
			return string.Format("PROGJUDGE_BPM\t{0:f3}" + Environment.NewLine, bpm);
		}

		public static string m2s_PROGJUDGE_AER(float rate)
		{
			return string.Format("PROGJUDGE_AER\t{0:f3}" + Environment.NewLine, rate);
		}

		public static string m2s_TUTORIAL(bool isTutorial)
		{
			return string.Format("TUTORIAL\t{0}" + Environment.NewLine, isTutorial ? 1 : 0);
		}

		public static string m2s_FES_MODE(bool isFesMode)
		{
			return string.Format("FES\t{0}" + Environment.NewLine, isFesMode ? 1 : 0);
		}

		public static string m2s_BPM(int bar, int grid, float bpm)
		{
			return string.Format("BPM\t{0}\t{1}\t{2:f3}" + Environment.NewLine, bar, grid, bpm);
		}

		public static string m2s_MET(int bar, int beat, uint denomi, uint num)
		{
			return string.Format("MET\t{0}\t{1}\t{2}\t{3}" + Environment.NewLine, bar, beat, denomi, num);
		}

		public static string m2s_STP(int bar, int beat, int grid)
		{
			return string.Format("STP\t{0}\t{1}\t{2}" + Environment.NewLine, bar, beat, grid);
		}

		public static string m2s_SFL(int bar, int beat, int grid, float speed)
		{
			return string.Format("SFL\t{0}\t{1}\t{2}\t{3:f6}" + Environment.NewLine, bar, beat, grid, speed);
		}

		public static string m2s_DCM(int bar, int beat, int grid, float speed)
		{
			return string.Format("DCM\t{0}\t{1}\t{2}\t{3:f6}" + Environment.NewLine, bar, beat, grid, speed);
		}

		public static string m2s_CLK(int bar, int beat)
		{
			return string.Format("CLK\t{0}\t{1}" + Environment.NewLine, bar, beat);
		}

		public static string m2s_TAP(int bar, int beat, int pos, uint width)
		{
			return string.Format("TAP\t{0}\t{1}\t{2}\t{3}" + Environment.NewLine, bar, beat, pos, width);
		}

		public static string m2s_STR(int bar, int beat, int pos)
		{
			return string.Format("STR\t{0}\t{1}\t{2}\t{3}" + Environment.NewLine, bar, beat, pos);
		}

		public static string m2s_BST(int bar, int beat, int pos)
		{
			return string.Format("BST\t{0}\t{1}\t{2}\t{3}" + Environment.NewLine, bar, beat, pos);
		}

		public static string m2s_HLD(int bar, int beat, int pos, uint width, int len)
		{
			return string.Format("HLD\t{0}\t{1}\t{2}\t{3}\t{4}" + Environment.NewLine, bar, beat, pos, width, len);
		}

		public static string m2s_SLD(int bar, int beat, int pos, uint waitLen, int shootLen, int targetNote, M2sfileRecordID type)
		{
			if (type < M2sfileRecordID.Def.SI_ || type > M2sfileRecordID.Def.SF_)
			{
				return "";
			}
			return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}" + Environment.NewLine, type.getEnumName(), bar, beat, pos, waitLen, shootLen, targetNote);
		}

		public static string m2s_FLK(int bar, int beat, int pos, uint width, string dirName)
		{
			return string.Format("FLK\t{0}\t{1}\t{2}\t{3}\t{4}" + Environment.NewLine, bar, beat, pos, width, dirName);
		}

		public static string m2s_TCH(int bar, int beat, int pos, string touchArea)
		{
			return string.Format("TCH\t{0}\t{1}\t{2}\t{3}\t{4}" + Environment.NewLine, bar, beat, pos, touchArea);
		}

		public static string m2s_TCS(int bar, int beat, int pos, uint width, string dirName)
		{
			return string.Format("TCS\t{0}\t{1}\t{2}\t{3}\t{4}" + Environment.NewLine, bar, beat, pos, width, dirName);
		}

		public static string m2s_TCF(int bar, int beat, int pos, uint width, string dirName)
		{
			return string.Format("TCF\t{0}\t{1}\t{2}\t{3}\t{4}" + Environment.NewLine, bar, beat, pos, width, dirName);
		}

		public static string m2s_TOTAL(TotalType kind, uint data)
		{
			return string.Format("{0}\t{1}" + Environment.NewLine, tagTotal[(int)kind], data);
		}
	}
}
