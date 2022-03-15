using UnityEngine;

public static class CommonScriptable
{
	private static bool _initialized;

	private static LedColorTable _ledSetting;

	private static GameColorTable _gameSetting;

	private static NotesEffectColorTable _notesEffectSetting;

	public static LedColorTable GetLedSetting()
	{
		CreateScriptable();
		return _ledSetting;
	}

	public static GameColorTable GetColorSetting()
	{
		CreateScriptable();
		return _gameSetting;
	}

	public static NotesEffectColorTable GetNotesEffectSetting()
	{
		CreateScriptable();
		return _notesEffectSetting;
	}

	public static void CreateScriptable()
	{
		if (!_initialized)
		{
			_ledSetting = Resources.Load<LedColorTable>("Common/Table/LedColorTable");
			_gameSetting = Resources.Load<GameColorTable>("Common/Table/GameColorTable");
			_notesEffectSetting = Resources.Load<NotesEffectColorTable>("Process/Game/Table/NotesEffectTable");
			_initialized = true;
		}
	}

	public static void DestroyScriptable()
	{
		if (_initialized)
		{
			_initialized = false;
		}
	}
}
