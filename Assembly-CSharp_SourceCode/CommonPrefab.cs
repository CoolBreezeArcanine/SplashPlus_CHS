using Manager;
using UI;
using UnityEngine;

public static class CommonPrefab
{
	private static bool _initialized;

	private static CommonTimer _timerPrefab;

	private static CommonButtonObject _circleButtonPrefab;

	private static CommonButtonObject _flatButtonPrefab;

	private static MovieController _moviePrefab;

	private static CommonCharaObject _naviCharaPrefab;

	private static CommonCharaObject _naviCharaTatePrefab;

	private static MusicDifficultySheet _musicLevelSheetTable;

	private static AchieveNumSheet _achieveSheetTable;

	public static CommonTimer GetCommonTimerPrefab()
	{
		CreatePrefab();
		return _timerPrefab;
	}

	public static CommonButtonObject GetCirclebuButtonObject()
	{
		CreatePrefab();
		return _circleButtonPrefab;
	}

	public static CommonButtonObject GetFlatButtonObject()
	{
		CreatePrefab();
		return _flatButtonPrefab;
	}

	public static MovieController GetMovieCtrlObject()
	{
		CreatePrefab();
		return _moviePrefab;
	}

	public static CommonCharaObject GetNaviCharaObject()
	{
		CreatePrefab();
		return _naviCharaPrefab;
	}

	public static CommonCharaObject GetNaviCharaTateObject()
	{
		CreatePrefab();
		return _naviCharaTatePrefab;
	}

	public static Sprite[] GetMusicLevelSprites(int difficulty)
	{
		CreatePrefab();
		return _musicLevelSheetTable.MusicLevelSprites[difficulty].Sheet;
	}

	public static Sprite[] GetAchieveIntSprites(int difficulty)
	{
		CreatePrefab();
		return _achieveSheetTable.AchieveIntSprites[difficulty].Sheet;
	}

	public static Sprite[] GetAchieveDecimalSprites(int difficulty)
	{
		CreatePrefab();
		return _achieveSheetTable.AchieveDecimalSprites[difficulty].Sheet;
	}

	public static void CreatePrefab()
	{
		if (!_initialized)
		{
			_timerPrefab = Resources.Load<CommonTimer>("Common/Prefabs/CommonTimer");
			_circleButtonPrefab = Resources.Load<CommonButtonObject>("Common/Prefabs/CircleButton");
			_flatButtonPrefab = Resources.Load<CommonButtonObject>("Common/Prefabs/FlatButton");
			_moviePrefab = Resources.Load<MovieController>("Common/Prefabs/MovieController");
			_naviCharaPrefab = Resources.Load<CommonCharaObject>("Common/Prefabs/NaviCharaExtend");
			_naviCharaTatePrefab = Resources.Load<CommonCharaObject>("Common/Prefabs/NaviCharaTateExtend");
			_musicLevelSheetTable = Resources.Load<MusicDifficultySheet>("Common/Table/MusicDifficultyTable");
			_achieveSheetTable = Resources.Load<AchieveNumSheet>("Common/Table/AhieveNumTable");
			_initialized = true;
			_timerPrefab.name = _timerPrefab.name.Replace("(Clone)", "");
		}
	}

	public static void DestroyPrefab()
	{
		if (_initialized)
		{
			_initialized = false;
		}
	}
}
