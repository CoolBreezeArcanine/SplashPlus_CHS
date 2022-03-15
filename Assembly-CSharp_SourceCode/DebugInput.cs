using MAI2.Util;
using MAI2System;
using UnityEngine;

public static class DebugInput
{
	public static bool GetKey(KeyCode name)
	{
		if (Singleton<SystemConfig>.Instance.config.IsDebug)
		{
			return Input.GetKey(name);
		}
		return false;
	}

	public static bool GetKeyDown(KeyCode name)
	{
		if (Singleton<SystemConfig>.Instance.config.IsDebug)
		{
			return Input.GetKeyDown(name);
		}
		return false;
	}

	public static bool GetKeyUp(KeyCode name)
	{
		if (Singleton<SystemConfig>.Instance.config.IsDebug)
		{
			return Input.GetKeyUp(name);
		}
		return false;
	}

	public static bool GetButton(string name)
	{
		if (Singleton<SystemConfig>.Instance.config.IsDebug)
		{
			return Input.GetButton(name);
		}
		return false;
	}

	public static bool GetButtonDown(string name)
	{
		if (Singleton<SystemConfig>.Instance.config.IsDebug)
		{
			return Input.GetButtonDown(name);
		}
		return false;
	}

	public static bool GetButtonUp(string name)
	{
		if (Singleton<SystemConfig>.Instance.config.IsDebug)
		{
			return Input.GetButtonUp(name);
		}
		return false;
	}

	public static bool GetMouseButton(int button)
	{
		if (Singleton<SystemConfig>.Instance.config.IsDebug)
		{
			return Input.GetMouseButton(button);
		}
		return false;
	}

	public static bool GetMouseButtonDown(int button)
	{
		if (Singleton<SystemConfig>.Instance.config.IsDebug)
		{
			return Input.GetMouseButtonDown(button);
		}
		return false;
	}

	public static bool GetMouseButtonUp(int button)
	{
		if (Singleton<SystemConfig>.Instance.config.IsDebug)
		{
			return Input.GetMouseButtonUp(button);
		}
		return false;
	}
}
