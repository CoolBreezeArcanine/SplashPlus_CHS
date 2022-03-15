using System.Collections.Generic;
using UnityEngine;

public class NoteJudge
{
	public enum EJudgeType
	{
		Tap,
		HoldOut,
		SlideOut,
		Touch,
		ExTap,
		Break,
		End
	}

	public enum ETiming
	{
		TooFast,
		FastGood,
		FastGreat3rd,
		FastGreat2nd,
		FastGreat,
		FastPerfect2nd,
		FastPerfect,
		Critical,
		LatePerfect,
		LatePerfect2nd,
		LateGreat,
		LateGreat2nd,
		LateGreat3rd,
		LateGood,
		TooLate,
		End
	}

	public enum JudgeBox
	{
		Miss,
		Good,
		Great,
		Perfect,
		Critical,
		End
	}

	private class JuggeTiming
	{
		private float[] judgeFlameList = new float[15];

		public float this[int i]
		{
			get
			{
				return judgeFlameList[i];
			}
			set
			{
				judgeFlameList[i] = value;
			}
		}

		public JuggeTiming(float[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				judgeFlameList[i] = array[i] * 16.666666f + JudgeAdjustMs;
			}
		}
	}

	private enum HoldCheckPoint
	{
		Critical,
		Perfect,
		Great,
		Good,
		Miss,
		End
	}

	public static float JudgeAdjustMs;

	public static float JudgeHoldHeadFrame;

	public static float JudgeHoldTailFrame;

	public static float JudgeTouchHoldHeadFrame;

	public static float JudgeTouchHoldTailFrame;

	private static readonly List<JuggeTiming> judgeParamTbl;

	private static readonly int[] holdJudgePercent;

	private static readonly ETiming[][] holdJudgeParam;

	public static float GetNoteCheckStart(EJudgeType type)
	{
		return judgeParamTbl[(int)type][0];
	}

	public static float GetNoteCheckEnd(EJudgeType type)
	{
		return judgeParamTbl[(int)type][14];
	}

	static NoteJudge()
	{
		JudgeAdjustMs = 50f;
		JudgeHoldHeadFrame = 50f + JudgeAdjustMs;
		JudgeHoldTailFrame = 150f + JudgeAdjustMs;
		JudgeTouchHoldHeadFrame = 200f + JudgeAdjustMs;
		JudgeTouchHoldTailFrame = 150f + JudgeAdjustMs;
		judgeParamTbl = new List<JuggeTiming>
		{
			new JuggeTiming(new float[15]
			{
				-9f, -6f, -5f, -4f, -3f, -2f, -1f, 0f, 1f, 2f,
				3f, 4f, 5f, 6f, 9f
			}),
			new JuggeTiming(new float[15]
			{
				-9f, -6f, -5f, -4f, -3f, -2f, -1f, 0f, 1f, 4f,
				8f, 9f, 10f, 11f, 14f
			}),
			new JuggeTiming(new float[15]
			{
				-36f, -26f, -22f, -18f, -14f, -14f, -14f, 0f, 14f, 14f,
				14f, 16f, 22f, 26f, 36f
			}),
			new JuggeTiming(new float[15]
			{
				-9f, -9f, -9f, -9f, -9f, -9f, -9f, 0f, 9f, 10.5f,
				12f, 13f, 14f, 15f, 18f
			}),
			new JuggeTiming(new float[15]
			{
				-9f, -9f, -9f, -9f, -9f, -9f, -9f, 0f, 9f, 9f,
				9f, 9f, 9f, 9f, 9f
			}),
			new JuggeTiming(new float[15]
			{
				-9f, -6f, -5f, -4f, -3f, -2f, -1f, 0f, 1f, 2f,
				3f, 4f, 5f, 6f, 9f
			})
		};
		holdJudgePercent = new int[5] { 0, 33, 67, 95, 100 };
		holdJudgeParam = new ETiming[5][]
		{
			new ETiming[15]
			{
				ETiming.FastGood,
				ETiming.FastGreat,
				ETiming.FastGreat,
				ETiming.FastGreat,
				ETiming.FastGreat,
				ETiming.FastPerfect,
				ETiming.FastPerfect,
				ETiming.Critical,
				ETiming.LatePerfect,
				ETiming.LatePerfect,
				ETiming.LateGreat,
				ETiming.LateGreat,
				ETiming.LateGreat,
				ETiming.LateGreat,
				ETiming.LateGood
			},
			new ETiming[15]
			{
				ETiming.FastGood,
				ETiming.FastGreat,
				ETiming.FastGreat,
				ETiming.FastGreat,
				ETiming.FastGreat,
				ETiming.FastPerfect,
				ETiming.FastPerfect,
				ETiming.LatePerfect,
				ETiming.LatePerfect,
				ETiming.LatePerfect,
				ETiming.LateGreat,
				ETiming.LateGreat,
				ETiming.LateGreat,
				ETiming.LateGreat,
				ETiming.LateGood
			},
			new ETiming[15]
			{
				ETiming.FastGood,
				ETiming.FastGood,
				ETiming.FastGreat,
				ETiming.FastGreat,
				ETiming.FastGreat,
				ETiming.FastGreat,
				ETiming.FastGreat,
				ETiming.LateGreat,
				ETiming.LateGreat,
				ETiming.LateGreat,
				ETiming.LateGreat,
				ETiming.LateGreat,
				ETiming.LateGreat,
				ETiming.LateGood,
				ETiming.LateGood
			},
			new ETiming[15]
			{
				ETiming.FastGood,
				ETiming.FastGood,
				ETiming.FastGood,
				ETiming.FastGood,
				ETiming.FastGood,
				ETiming.FastGood,
				ETiming.FastGood,
				ETiming.LateGood,
				ETiming.LateGood,
				ETiming.LateGood,
				ETiming.LateGood,
				ETiming.LateGood,
				ETiming.LateGood,
				ETiming.LateGood,
				ETiming.LateGood
			},
			new ETiming[15]
			{
				ETiming.TooFast,
				ETiming.FastGood,
				ETiming.FastGood,
				ETiming.FastGood,
				ETiming.FastGood,
				ETiming.FastGood,
				ETiming.FastGood,
				ETiming.LateGood,
				ETiming.LateGood,
				ETiming.LateGood,
				ETiming.LateGood,
				ETiming.LateGood,
				ETiming.LateGood,
				ETiming.LateGood,
				ETiming.TooLate
			}
		};
	}

	public static void Initialize()
	{
	}

	public static ETiming GetJudgeTiming(ref float _fMsec, float optionJudgeTiming, EJudgeType type)
	{
		_fMsec -= optionJudgeTiming * 16.666666f;
		if (type == EJudgeType.SlideOut)
		{
			_fMsec += JudgeAdjustMs;
		}
		ETiming result = ((!(_fMsec < judgeParamTbl[(int)type][0])) ? ((_fMsec < judgeParamTbl[(int)type][1]) ? ETiming.FastGood : ((_fMsec < judgeParamTbl[(int)type][2]) ? ETiming.FastGreat3rd : ((_fMsec < judgeParamTbl[(int)type][3]) ? ETiming.FastGreat2nd : ((_fMsec < judgeParamTbl[(int)type][4]) ? ETiming.FastGreat : ((_fMsec < judgeParamTbl[(int)type][5]) ? ETiming.FastPerfect2nd : ((_fMsec < judgeParamTbl[(int)type][6]) ? ETiming.FastPerfect : ((_fMsec <= judgeParamTbl[(int)type][8]) ? ETiming.Critical : ((_fMsec <= judgeParamTbl[(int)type][9]) ? ETiming.LatePerfect : ((_fMsec <= judgeParamTbl[(int)type][10]) ? ETiming.LatePerfect2nd : ((_fMsec <= judgeParamTbl[(int)type][11]) ? ETiming.LateGreat : ((_fMsec <= judgeParamTbl[(int)type][12]) ? ETiming.LateGreat2nd : ((_fMsec <= judgeParamTbl[(int)type][13]) ? ETiming.LateGreat3rd : ((!(_fMsec <= judgeParamTbl[(int)type][14])) ? ETiming.TooLate : ETiming.LateGood))))))))))))) : ETiming.TooFast);
		_fMsec -= JudgeAdjustMs;
		return result;
	}

	public static ETiming GetSlideJudgeTiming(ref float _fMsec, float optionJudgeTiming, EJudgeType type, float lastWaitTime)
	{
		float num = lastWaitTime / 4f;
		float num2 = judgeParamTbl[2][6] - num;
		float num3 = judgeParamTbl[2][8] + num;
		float num4 = judgeParamTbl[2][0];
		float num5 = judgeParamTbl[2][14];
		float num6 = _fMsec - optionJudgeTiming * 16.666666f;
		if (type != EJudgeType.SlideOut)
		{
			return GetJudgeTiming(ref _fMsec, optionJudgeTiming, EJudgeType.SlideOut);
		}
		if (num2 <= num6 && num6 <= num3 && num4 <= num6 && num6 <= num5)
		{
			return ETiming.Critical;
		}
		return GetJudgeTiming(ref _fMsec, optionJudgeTiming, EJudgeType.SlideOut);
	}

	public static JudgeBox ConvertJudge(ETiming timing)
	{
		switch (timing)
		{
		case ETiming.TooFast:
		case ETiming.TooLate:
			return JudgeBox.Miss;
		case ETiming.FastGood:
		case ETiming.LateGood:
			return JudgeBox.Good;
		case ETiming.FastGreat3rd:
		case ETiming.FastGreat2nd:
		case ETiming.FastGreat:
		case ETiming.LateGreat:
		case ETiming.LateGreat2nd:
		case ETiming.LateGreat3rd:
			return JudgeBox.Great;
		case ETiming.FastPerfect2nd:
		case ETiming.FastPerfect:
		case ETiming.LatePerfect:
		case ETiming.LatePerfect2nd:
			return JudgeBox.Perfect;
		case ETiming.Critical:
			return JudgeBox.Critical;
		default:
			return JudgeBox.Miss;
		}
	}

	public static ETiming JudgeHoldTotal(float holdTime, float holdReleaseTime, ETiming timing, bool bodyOn, bool isTouchHold)
	{
		float num = (isTouchHold ? JudgeTouchHoldHeadFrame : JudgeHoldHeadFrame);
		if (holdTime <= num + JudgeHoldTailFrame)
		{
			return timing;
		}
		holdTime -= num + JudgeHoldTailFrame;
		if (!bodyOn)
		{
			return holdJudgeParam[4][(int)timing];
		}
		int num2 = Mathf.CeilToInt(holdReleaseTime / holdTime * 100f);
		if (num2 > 100)
		{
			num2 = 100;
		}
		for (int i = 0; i < holdJudgePercent.Length; i++)
		{
			if (holdJudgePercent[i] >= num2)
			{
				return holdJudgeParam[i][(int)timing];
			}
		}
		return holdJudgeParam[4][(int)timing];
	}
}
