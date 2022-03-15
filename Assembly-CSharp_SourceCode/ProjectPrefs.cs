using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectPrefs : ScriptableObject
{
	[Serializable]
	private class PrefsStringData
	{
		public string key;

		public string value;
	}

	[Serializable]
	private class PrefsIntegerData
	{
		public string key;

		public int value;
	}

	private static ProjectPrefs instance;

	[SerializeField]
	private List<PrefsStringData> stringData;

	[SerializeField]
	private List<PrefsIntegerData> intData;

	public static ProjectPrefs Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Resources.Load<ProjectPrefs>("_ProjectPrefs");
			}
			return instance;
		}
	}

	public static string GetString(string key)
	{
		return GetString(key, "");
	}

	public static string GetString(string key, string defaultValue)
	{
		PrefsStringData prefsStringData = Instance.stringData.Find((PrefsStringData d) => d.key == key);
		if (prefsStringData == null)
		{
			return defaultValue;
		}
		return prefsStringData.value;
	}

	public static int GetInt(string key, int defaultValue)
	{
		return Instance.intData.Find((PrefsIntegerData d) => d.key == key)?.value ?? defaultValue;
	}
}
