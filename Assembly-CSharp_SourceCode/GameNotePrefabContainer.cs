using Monitor;
using UnityEngine;

public class GameNotePrefabContainer
{
	private static TapNote _tap;

	private static BreakNote _break;

	private static HoldNote _hold;

	private static StarNote _star;

	private static BreakStarNote _breakStar;

	private static SlideRoot _slide;

	private static SlideFan _slideFan;

	private static SlideJudge _slideJudge;

	private static SlideJudge _slideCircleJudge;

	private static SlideJudge _slideFanJudge;

	private static TouchNoteB _touchTapB;

	private static TouchNoteC _touchTapC;

	private static TouchHoldC _touchHoldC;

	private static NoteGuide _guide;

	private static BarGuide _barGuide;

	private static GameObject _noteLauncher;

	private static GameObject _slideLauncher;

	private static GameObject _touchBLauncher;

	private static GameObject _touchCLauncher;

	private static GameObject _touchELauncher;

	private static TouchEffect _touchEffect;

	private static JudgeGrade _judgeGrade;

	private static JudgeTouchGrade _touchJudgeGrade;

	private static TouchReserve _touchReserve;

	private static TrackSkip _trackSkip;

	private const string GamenoteDirectoryPath = "Process/Game/Prefubs/";

	private const string TypeNotePrefix = "Notes/";

	private const string TypeSlidePrefix = "Slide/";

	private const string TypeEffectPrefix = "Effect/";

	public static TapNote Tap => _tap;

	public static BreakNote Break => _break;

	public static HoldNote Hold => _hold;

	public static StarNote Star => _star;

	public static BreakStarNote BreakStar => _breakStar;

	public static SlideRoot Slide => _slide;

	public static SlideFan SlideFan => _slideFan;

	public static SlideJudge SlideJudge => _slideJudge;

	public static SlideJudge SlideCircleJudge => _slideCircleJudge;

	public static SlideJudge SlideFanJudge => _slideFanJudge;

	public static TouchNoteB TouchTapB => _touchTapB;

	public static TouchNoteC TouchTapC => _touchTapC;

	public static TouchHoldC TouchHoldC => _touchHoldC;

	public static NoteGuide Guide => _guide;

	public static BarGuide BarGuide => _barGuide;

	public static GameObject NoteLauncher => _noteLauncher;

	public static GameObject SlideLauncher => _slideLauncher;

	public static GameObject TouchBLauncher => _touchBLauncher;

	public static GameObject TouchCLauncher => _touchCLauncher;

	public static GameObject TouchELauncher => _touchELauncher;

	public static TouchEffect TouchEffect => _touchEffect;

	public static JudgeGrade JudgeGrade => _judgeGrade;

	public static JudgeTouchGrade TouchJudgeGrade => _touchJudgeGrade;

	public static TouchReserve TouchReserve => _touchReserve;

	public static TrackSkip TrackSkip => _trackSkip;

	public static void Initialize()
	{
		_tap = Resources.Load<TapNote>("Process/Game/Prefubs/Notes/Tap");
		_break = Resources.Load<BreakNote>("Process/Game/Prefubs/Notes/Break");
		_hold = Resources.Load<HoldNote>("Process/Game/Prefubs/Notes/Hold");
		_star = Resources.Load<StarNote>("Process/Game/Prefubs/Notes/Star");
		_breakStar = Resources.Load<BreakStarNote>("Process/Game/Prefubs/Notes/BreakStar");
		_slide = Resources.Load<SlideRoot>("Process/Game/Prefubs/Slide/SlideLane");
		_slideFan = Resources.Load<SlideFan>("Process/Game/Prefubs/Slide/SlideFan");
		_slideJudge = Resources.Load<SlideJudge>("Process/Game/Prefubs/Slide/SlideJudge");
		_slideCircleJudge = Resources.Load<SlideJudge>("Process/Game/Prefubs/Slide/SlideCircleJudge");
		_slideFanJudge = Resources.Load<SlideJudge>("Process/Game/Prefubs/Slide/SlideFanJudge");
		_touchTapB = Resources.Load<TouchNoteB>("Process/Game/Prefubs/Notes/TouchNote_B");
		_touchTapC = Resources.Load<TouchNoteC>("Process/Game/Prefubs/Notes/TouchNote_C");
		_touchHoldC = Resources.Load<TouchHoldC>("Process/Game/Prefubs/Notes/TouchHold_C");
		_guide = Resources.Load<NoteGuide>("Process/Game/Prefubs/Notes/NoteGuide");
		_barGuide = Resources.Load<BarGuide>("Process/Game/Prefubs/Notes/BarGuide");
		_noteLauncher = Resources.Load<GameObject>("Process/Game/Prefubs/NoteLauncher");
		_slideLauncher = Resources.Load<GameObject>("Process/Game/Prefubs/SlideLauncher");
		_touchBLauncher = Resources.Load<GameObject>("Process/Game/Prefubs/TouchB_Launcher");
		_touchCLauncher = Resources.Load<GameObject>("Process/Game/Prefubs/TouchC_Launcher");
		_touchELauncher = Resources.Load<GameObject>("Process/Game/Prefubs/TouchE_Launcher");
		_touchEffect = Resources.Load<TouchEffect>("Process/Game/Prefubs/Effect/TouchParticle");
		_judgeGrade = Resources.Load<JudgeGrade>("Process/Game/Prefubs/Effect/JudgeGrade");
		_touchJudgeGrade = Resources.Load<JudgeTouchGrade>("Process/Game/Prefubs/Effect/TouchJudgeGrade");
		_touchReserve = Resources.Load<TouchReserve>("Process/Game/Prefubs/Effect/TouchReserve");
		_trackSkip = Resources.Load<TrackSkip>("Process/Game/Prefubs/UI_GAM_TrackSkip");
	}
}
