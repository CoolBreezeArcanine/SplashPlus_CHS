using UnityEngine;

namespace Process
{
	public static class GameNoteImageContainer
	{
		private static Sprite[] _normalTap = new Sprite[5];

		private static Sprite[] _eachlTap = new Sprite[5];

		private static Sprite[] _exTap = new Sprite[5];

		private static Sprite[] _normalBreak = new Sprite[5];

		private static Sprite[] _normalBreakEff = new Sprite[5];

		private static Sprite[] _normalHold = new Sprite[2];

		private static Sprite[] _normalHoldOn = new Sprite[2];

		private static Sprite _normalHoldEnd;

		private static Sprite[] _eachHold = new Sprite[2];

		private static Sprite[] _eachHoldOn = new Sprite[2];

		private static Sprite _eachHoldEnd;

		private static Sprite[] _exHold = new Sprite[2];

		private static Sprite[] _holdOff = new Sprite[2];

		private static Sprite[,] _normalStar = new Sprite[2, 2];

		private static Sprite[,] _normalDoubleStar = new Sprite[2, 2];

		private static Sprite[] _eachStar = new Sprite[2];

		private static Sprite[] _eachDoubleStar = new Sprite[2];

		private static Sprite[] _exStar = new Sprite[2];

		private static Sprite[] _exDoubleStar = new Sprite[2];

		private static Sprite[] _breakStar = new Sprite[2];

		private static Sprite[] _breakDoubleStar = new Sprite[2];

		private static Sprite[] _breakStarEff = new Sprite[2];

		private static Sprite[] _breakDoubleStarEff = new Sprite[2];

		private static Sprite[] _normalSlide = new Sprite[2];

		private static Sprite[] _eachSlide = new Sprite[2];

		private static Sprite _normalTouch;

		private static Sprite _normalTouchPoint;

		private static Sprite _eachTouch;

		private static Sprite _eachTouchPoint;

		private static Sprite _touchHoldGuide;

		private static Sprite _touchHoldGuideOff;

		private static Sprite[] _guide = new Sprite[5];

		private static Sprite _judgeCritical;

		private static Sprite _judgePerfect;

		private static Sprite _judgeFastPerfect;

		private static Sprite _judgeLatePerfect;

		private static Sprite _judgeGreat;

		private static Sprite _judgeFastGreat;

		private static Sprite _judgeLateGreat;

		private static Sprite _judgeGood;

		private static Sprite _judgeFastGood;

		private static Sprite _judgeLateGood;

		private static Sprite _judgeMiss;

		private static Sprite _judgeFast;

		private static Sprite _judgeLate;

		private static Sprite _judgeCriticalBreakAdd;

		private static Sprite _judgePerfectBreakAdd;

		private static Sprite[,] _judgeSlideTooFast = new Sprite[3, 2];

		private static Sprite[,] _judgeSlideFastGood = new Sprite[3, 2];

		private static Sprite[,] _judgeSlideFastGoodCol = new Sprite[3, 2];

		private static Sprite[,] _judgeSlideFastGreat = new Sprite[3, 2];

		private static Sprite[,] _judgeSlideFastGreatCol = new Sprite[3, 2];

		private static Sprite[,] _judgeSlideFastPerfect = new Sprite[3, 2];

		private static Sprite[,] _judgeSlideFastPerfectCol = new Sprite[3, 2];

		private static Sprite[,] _judgeSlidePerfect = new Sprite[3, 2];

		private static Sprite[,] _judgeSlideCritical = new Sprite[3, 2];

		private static Sprite[,] _judgeSlideLatePerfect = new Sprite[3, 2];

		private static Sprite[,] _judgeSlideLatePerfectCol = new Sprite[3, 2];

		private static Sprite[,] _judgeSlideLateGreat = new Sprite[3, 2];

		private static Sprite[,] _judgeSlideLateGreatCol = new Sprite[3, 2];

		private static Sprite[,] _judgeSlideLateGood = new Sprite[3, 2];

		private static Sprite[,] _judgeSlideLateGoodCol = new Sprite[3, 2];

		private static Sprite[,] _judgeSlideTooLate = new Sprite[3, 2];

		private const string GamenoteDirectoryPath = "Process/Game/Sprites/";

		private const string TypeTapPrefix = "Tap/";

		private const string TypeHoldPrefix = "Hold/";

		private const string TypeStarPrefix = "Star/";

		private const string TypeBreakPrefix = "Break/";

		private const string TypeBreakstarPrefix = "BreakStar/";

		private const string TypeTouchTapPrefix = "TouchTap/";

		private const string TypeTouchHoldPrefix = "TouchHold/";

		private const string TypeSlidePrefix = "Slide/";

		private const string TypeGuidePrefix = "Guide/";

		private const string TypeJudgePrefix = "Judge/";

		private const string TypeSlideJudgePrefix = "SlideJudge/";

		private const string TypeSlideCircleJudgePrefix = "SlideCircleJudge/";

		private const string TypeSlideFanJudgePrefix = "SlideFanJudge/";

		public static Sprite[] NormalTap => _normalTap;

		public static Sprite[] EachTap => _eachlTap;

		public static Sprite[] ExTap => _exTap;

		public static Sprite[] NormalBreak => _normalBreak;

		public static Sprite[] NormalBreakEff => _normalBreakEff;

		public static Sprite[] NormalHold => _normalHold;

		public static Sprite[] NormalHoldOn => _normalHoldOn;

		public static Sprite NormalHoldEnd => _normalHoldEnd;

		public static Sprite[] EachHold => _eachHold;

		public static Sprite[] EachHoldOn => _eachHoldOn;

		public static Sprite EachHoldEnd => _eachHoldEnd;

		public static Sprite[] ExHold => _exHold;

		public static Sprite[] HoldOff => _holdOff;

		public static Sprite[,] NormalStar => _normalStar;

		public static Sprite[,] NormalDoubleStar => _normalDoubleStar;

		public static Sprite[] EachStar => _eachStar;

		public static Sprite[] EachDoubleStar => _eachDoubleStar;

		public static Sprite[] ExStar => _exStar;

		public static Sprite[] ExDoubleStar => _exDoubleStar;

		public static Sprite[] BreakStar => _breakStar;

		public static Sprite[] BreakDoubleStar => _breakDoubleStar;

		public static Sprite[] BreakStarEff => _breakStarEff;

		public static Sprite[] BreakDoubleStarEff => _breakDoubleStarEff;

		public static Sprite[] NormalSlide => _normalSlide;

		public static Sprite[] EachlSlide => _eachSlide;

		public static Sprite NormalTouch => _normalTouch;

		public static Sprite NormalTouchPoint => _normalTouchPoint;

		public static Sprite EachTouch => _eachTouch;

		public static Sprite EachTouchPoint => _eachTouchPoint;

		public static Sprite TouchHoldGuide => _touchHoldGuide;

		public static Sprite TouchHoldGuideOff => _touchHoldGuideOff;

		public static Sprite[] Guide => _guide;

		public static Sprite JudgeCritical => _judgeCritical;

		public static Sprite JudgePerfect => _judgePerfect;

		public static Sprite JudgeFastPerfect => _judgeFastPerfect;

		public static Sprite JudgeLatePerfect => _judgeLatePerfect;

		public static Sprite JudgeGreat => _judgeGreat;

		public static Sprite JudgeFastGreat => _judgeFastGreat;

		public static Sprite JudgeLateGreat => _judgeLateGreat;

		public static Sprite JudgeGood => _judgeGood;

		public static Sprite JudgeFastGood => _judgeFastGood;

		public static Sprite JudgeLateGood => _judgeLateGood;

		public static Sprite JudgeMiss => _judgeMiss;

		public static Sprite JudgeFast => _judgeFast;

		public static Sprite JudgeLate => _judgeLate;

		public static Sprite JudgeCriticalBreak => _judgeCriticalBreakAdd;

		public static Sprite JudgePerfectBreak => _judgePerfectBreakAdd;

		public static Sprite[,] JudgeSlideTooFast => _judgeSlideTooFast;

		public static Sprite[,] JudgeSlideFastGood => _judgeSlideFastGood;

		public static Sprite[,] JudgeSlideFastGoodCol => _judgeSlideFastGoodCol;

		public static Sprite[,] JudgeSlideFastGreat => _judgeSlideFastGreat;

		public static Sprite[,] JudgeSlideFastGreatCol => _judgeSlideFastGreatCol;

		public static Sprite[,] JudgeSlideFastPerfect => _judgeSlideFastPerfect;

		public static Sprite[,] JudgeSlideFastPerfectCol => _judgeSlideFastPerfectCol;

		public static Sprite[,] JudgeSlidePerfect => _judgeSlidePerfect;

		public static Sprite[,] JudgeSlideCritical => _judgeSlideCritical;

		public static Sprite[,] JudgeSlideLatePerfect => _judgeSlideLatePerfect;

		public static Sprite[,] JudgeSlideLatePerfectCol => _judgeSlideLatePerfectCol;

		public static Sprite[,] JudgeSlideLateGreat => _judgeSlideLateGreat;

		public static Sprite[,] JudgeSlideLateGreatCol => _judgeSlideLateGreatCol;

		public static Sprite[,] JudgeSlideLateGood => _judgeSlideLateGood;

		public static Sprite[,] JudgeSlideLateGoodCol => _judgeSlideLateGoodCol;

		public static Sprite[,] JudgeSlideTooLate => _judgeSlideTooLate;

		public static void Initialize()
		{
			for (int i = 0; i < 5; i++)
			{
				string text = $"{i:D02}";
				_normalTap[i] = Resources.Load<Sprite>("Process/Game/Sprites/Tap/Tap_" + text);
				_eachlTap[i] = Resources.Load<Sprite>("Process/Game/Sprites/Tap/Tap_Each_" + text);
				_exTap[i] = Resources.Load<Sprite>("Process/Game/Sprites/Tap/Tap_Ex_" + text);
				_normalBreak[i] = Resources.Load<Sprite>("Process/Game/Sprites/Break/Break_" + text);
				_normalBreakEff[i] = Resources.Load<Sprite>("Process/Game/Sprites/Break/Break_EFF_" + text);
			}
			for (int j = 0; j < 2; j++)
			{
				string text2 = $"{j:D02}";
				_normalHold[j] = Resources.Load<Sprite>("Process/Game/Sprites/Hold/Hold_" + text2);
				_normalHoldOn[j] = Resources.Load<Sprite>("Process/Game/Sprites/Hold/Hold_On_" + text2);
				_normalHoldEnd = Resources.Load<Sprite>("Process/Game/Sprites/Hold/Hold_End");
				_eachHold[j] = Resources.Load<Sprite>("Process/Game/Sprites/Hold/Hold_Each_" + text2);
				_eachHoldOn[j] = Resources.Load<Sprite>("Process/Game/Sprites/Hold/Hold_Each_On_" + text2);
				_eachHoldEnd = Resources.Load<Sprite>("Process/Game/Sprites/Hold/Hold_Each_End");
				_exHold[j] = Resources.Load<Sprite>("Process/Game/Sprites/Hold/Hold_Ex_" + text2);
				_holdOff[j] = Resources.Load<Sprite>("Process/Game/Sprites/Hold/Miss_" + text2);
			}
			for (int k = 0; k < 2; k++)
			{
				string text3 = $"{k:D02}";
				_normalStar[k, 0] = Resources.Load<Sprite>("Process/Game/Sprites/Star/Star_" + text3);
				_normalStar[k, 1] = Resources.Load<Sprite>("Process/Game/Sprites/Star/Star_Pink_" + text3);
				_normalDoubleStar[k, 0] = Resources.Load<Sprite>("Process/Game/Sprites/Star/Star_Double_" + text3);
				_normalDoubleStar[k, 1] = Resources.Load<Sprite>("Process/Game/Sprites/Star/Star_Pink_Double_" + text3);
				_eachStar[k] = Resources.Load<Sprite>("Process/Game/Sprites/Star/Star_Each_" + text3);
				_eachDoubleStar[k] = Resources.Load<Sprite>("Process/Game/Sprites/Star/Star_Each_Double_" + text3);
				_exStar[k] = Resources.Load<Sprite>("Process/Game/Sprites/Star/Star_Ex_" + text3);
				_exDoubleStar[k] = Resources.Load<Sprite>("Process/Game/Sprites/Star/Star_Ex_Double_" + text3);
				_breakStar[k] = Resources.Load<Sprite>("Process/Game/Sprites/BreakStar/BreakStar_" + text3);
				_breakDoubleStar[k] = Resources.Load<Sprite>("Process/Game/Sprites/BreakStar/BreakStar_Double_" + text3);
				_breakStarEff[k] = Resources.Load<Sprite>("Process/Game/Sprites/BreakStar/BreakStar_EFF_" + text3);
				_breakDoubleStarEff[k] = Resources.Load<Sprite>("Process/Game/Sprites/BreakStar/BreakStar_Double_EFF_" + text3);
			}
			for (int l = 0; l < 2; l++)
			{
				string text4 = $"{l:D02}";
				_normalSlide[l] = Resources.Load<Sprite>("Process/Game/Sprites/Slide/Slide_" + text4);
				_eachSlide[l] = Resources.Load<Sprite>("Process/Game/Sprites/Slide/Slide_Each_" + text4);
			}
			_normalTouch = Resources.Load<Sprite>("Process/Game/Sprites/TouchTap/UI_NOTES_Touch_01");
			_eachTouch = Resources.Load<Sprite>("Process/Game/Sprites/TouchTap/UI_NOTES_Touch_Each_01");
			_normalTouchPoint = Resources.Load<Sprite>("Process/Game/Sprites/TouchTap/UI_NOTES_Touch_Point");
			_eachTouchPoint = Resources.Load<Sprite>("Process/Game/Sprites/TouchTap/UI_NOTES_Touch_Each_Point");
			_touchHoldGuide = Resources.Load<Sprite>("Process/Game/Sprites/TouchHold/UI_NOTES_Touch_Hold_hold");
			_touchHoldGuideOff = Resources.Load<Sprite>("Process/Game/Sprites/TouchHold/UI_NOTES_Touch_Hold_hold_Miss");
			_guide[0] = Resources.Load<Sprite>("Process/Game/Sprites/Guide/Normal");
			_guide[1] = Resources.Load<Sprite>("Process/Game/Sprites/Guide/Each");
			_guide[2] = Resources.Load<Sprite>("Process/Game/Sprites/Guide/Slide");
			_guide[3] = Resources.Load<Sprite>("Process/Game/Sprites/Guide/Break");
			_guide[4] = Resources.Load<Sprite>("Process/Game/Sprites/Guide/Ex");
			_judgeCritical = Resources.Load<Sprite>("Process/Game/Sprites/Judge/UI_GAM_Critical");
			_judgePerfect = Resources.Load<Sprite>("Process/Game/Sprites/Judge/UI_GAM_Perfect");
			_judgeFastPerfect = Resources.Load<Sprite>("Process/Game/Sprites/Judge/UI_GAM_FastPerfect");
			_judgeLatePerfect = Resources.Load<Sprite>("Process/Game/Sprites/Judge/UI_GAM_LatePerfect");
			_judgeGreat = Resources.Load<Sprite>("Process/Game/Sprites/Judge/UI_GAM_Great");
			_judgeFastGreat = Resources.Load<Sprite>("Process/Game/Sprites/Judge/UI_GAM_FastGreat");
			_judgeLateGreat = Resources.Load<Sprite>("Process/Game/Sprites/Judge/UI_GAM_LateGreat");
			_judgeGood = Resources.Load<Sprite>("Process/Game/Sprites/Judge/UI_GAM_Good");
			_judgeFastGood = Resources.Load<Sprite>("Process/Game/Sprites/Judge/UI_GAM_FastGood");
			_judgeLateGood = Resources.Load<Sprite>("Process/Game/Sprites/Judge/UI_GAM_LateGood");
			_judgeMiss = Resources.Load<Sprite>("Process/Game/Sprites/Judge/UI_GAM_Miss");
			_judgeFast = Resources.Load<Sprite>("Process/Game/Sprites/Judge/UI_GAM_Fast");
			_judgeLate = Resources.Load<Sprite>("Process/Game/Sprites/Judge/UI_GAM_Late");
			_judgeCriticalBreakAdd = Resources.Load<Sprite>("Process/Game/Sprites/Judge/UI_GAM_Critical_Break");
			_judgePerfectBreakAdd = Resources.Load<Sprite>("Process/Game/Sprites/Judge/UI_GAM_Perfect_Break");
			string[] array = new string[3] { "SlideJudge/", "SlideCircleJudge/", "SlideFanJudge/" };
			string[] array2 = new string[3] { "UI_GAM_Slide_", "UI_GAM_SlideCircle_", "UI_GAM_SlideFan_" };
			string[,] array3 = new string[3, 2]
			{
				{ "L_", "R_" },
				{ "L_", "R_" },
				{ "U_", "D_" }
			};
			for (int m = 0; m < 3; m++)
			{
				for (int n = 0; n < 2; n++)
				{
					_judgeSlideTooFast[m, n] = Resources.Load<Sprite>("Process/Game/Sprites/" + array[m] + array2[m] + array3[m, n] + "Fast");
					_judgeSlideFastGood[m, n] = Resources.Load<Sprite>("Process/Game/Sprites/" + array[m] + array2[m] + array3[m, n] + "FastGood_01");
					_judgeSlideFastGoodCol[m, n] = Resources.Load<Sprite>("Process/Game/Sprites/" + array[m] + array2[m] + array3[m, n] + "FastGood_02");
					_judgeSlideFastGreat[m, n] = Resources.Load<Sprite>("Process/Game/Sprites/" + array[m] + array2[m] + array3[m, n] + "FastGreat_01");
					_judgeSlideFastGreatCol[m, n] = Resources.Load<Sprite>("Process/Game/Sprites/" + array[m] + array2[m] + array3[m, n] + "FastGreat_02");
					_judgeSlideFastPerfect[m, n] = Resources.Load<Sprite>("Process/Game/Sprites/" + array[m] + array2[m] + array3[m, n] + "FastPerfect_01");
					_judgeSlideFastPerfectCol[m, n] = Resources.Load<Sprite>("Process/Game/Sprites/" + array[m] + array2[m] + array3[m, n] + "FastPerfect_02");
					_judgeSlidePerfect[m, n] = Resources.Load<Sprite>("Process/Game/Sprites/" + array[m] + array2[m] + array3[m, n] + "Perfect");
					_judgeSlideCritical[m, n] = Resources.Load<Sprite>("Process/Game/Sprites/" + array[m] + array2[m] + array3[m, n] + "Critical");
					_judgeSlideLatePerfect[m, n] = Resources.Load<Sprite>("Process/Game/Sprites/" + array[m] + array2[m] + array3[m, n] + "LatePerfect_01");
					_judgeSlideLatePerfectCol[m, n] = Resources.Load<Sprite>("Process/Game/Sprites/" + array[m] + array2[m] + array3[m, n] + "LatePerfect_02");
					_judgeSlideLateGreat[m, n] = Resources.Load<Sprite>("Process/Game/Sprites/" + array[m] + array2[m] + array3[m, n] + "LateGreat_01");
					_judgeSlideLateGreatCol[m, n] = Resources.Load<Sprite>("Process/Game/Sprites/" + array[m] + array2[m] + array3[m, n] + "LateGreat_02");
					_judgeSlideLateGood[m, n] = Resources.Load<Sprite>("Process/Game/Sprites/" + array[m] + array2[m] + array3[m, n] + "LateGood_01");
					_judgeSlideLateGoodCol[m, n] = Resources.Load<Sprite>("Process/Game/Sprites/" + array[m] + array2[m] + array3[m, n] + "LateGood_02");
					_judgeSlideTooLate[m, n] = Resources.Load<Sprite>("Process/Game/Sprites/" + array[m] + array2[m] + array3[m, n] + "Late");
				}
			}
		}
	}
}
