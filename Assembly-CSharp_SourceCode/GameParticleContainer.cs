using UnityEngine;

public class GameParticleContainer
{
	private static GameObject _touchEffect;

	private static GameObject[] _centerBackEffect = new GameObject[5];

	private static GameObject[] _tapEffect = new GameObject[5];

	private static GameObject[] _tapExEffect = new GameObject[5];

	private static GameObject _centerEffect;

	private static GameObject[] _holdEffect = new GameObject[2];

	private static GameObject[] _holdReleaseEffect = new GameObject[2];

	private static GameObject[] _breakEffect = new GameObject[5];

	private const string GameTouchDirectoryPath = "CMN_Touch/FX_CMN_Touch";

	private const string GameEffectDirectoryPath = "GAM_NotesDecides/GAM_Tap/";

	private const string TypeTapPrefix = "FX_GAM_Notes_Tap_";

	private const string TypeExTapPrefix = "FX_GAM_Notes_EXTap_";

	private const string TypeCenterPrefix = "FX_GAM_Notes_Touch_";

	private const string TypeHoldPrefix = "FX_GAM_Notes_Hold_";

	private const string TypeHoldReleasePrefix = "FX_GAM_Notes_Hold_Release_";

	private const string TypeBreakPrefix = "FX_GAM_Notes_Break_";

	private const string TypeCenterBackPrefix = "FX_GAM_Notes_Center_Back_";

	public static GameObject TouchEffect => _touchEffect;

	public static GameObject[] CenterBackEffect => _centerBackEffect;

	public static GameObject[] TapEffect => _tapEffect;

	public static GameObject[] TapExEffect => _tapExEffect;

	public static GameObject CenterEffect => _centerEffect;

	public static GameObject[] HoldEffect => _holdEffect;

	public static GameObject[] HoldReleaseEffect => _holdReleaseEffect;

	public static GameObject[] BreakEffect => _breakEffect;

	public static void Initialize()
	{
		_touchEffect = Resources.Load<GameObject>("CMN_Touch/FX_CMN_Touch");
		_centerEffect = Resources.Load<GameObject>("GAM_NotesDecides/GAM_Tap/FX_GAM_Notes_Touch_00");
		for (int i = 0; i < 5; i++)
		{
			string text = $"{i:D02}";
			_tapEffect[i] = Resources.Load<GameObject>("GAM_NotesDecides/GAM_Tap/FX_GAM_Notes_Tap_" + text);
			_tapExEffect[i] = Resources.Load<GameObject>("GAM_NotesDecides/GAM_Tap/FX_GAM_Notes_EXTap_" + text);
			_breakEffect[i] = Resources.Load<GameObject>("GAM_NotesDecides/GAM_Tap/FX_GAM_Notes_Break_" + text);
			_centerBackEffect[i] = Resources.Load<GameObject>("GAM_NotesDecides/GAM_Tap/FX_GAM_Notes_Center_Back_" + text);
		}
		for (int j = 0; j < 2; j++)
		{
			string text2 = $"{j:D02}";
			_holdEffect[j] = Resources.Load<GameObject>("GAM_NotesDecides/GAM_Tap/FX_GAM_Notes_Hold_" + text2);
			_holdReleaseEffect[j] = Resources.Load<GameObject>("GAM_NotesDecides/GAM_Tap/FX_GAM_Notes_Hold_Release_" + text2);
		}
	}
}
