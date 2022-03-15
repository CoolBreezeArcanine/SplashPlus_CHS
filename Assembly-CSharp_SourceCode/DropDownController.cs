using System;
using System.Collections.Generic;
using DB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropDownController : MonoBehaviour
{
	private const string BeginStr = "Begin";

	private const string EndStr = "End";

	[SerializeField]
	private CustomDropDown _kind;

	[SerializeField]
	private CustomDropDown _position;

	[SerializeField]
	private CustomDropDown _size;

	[SerializeField]
	private Toggle _isWarning;

	private void Awake()
	{
		Prepare();
	}

	private void Prepare()
	{
		List<TMP_Dropdown.OptionData> list = new List<TMP_Dropdown.OptionData>();
		string[] names = Enum.GetNames(typeof(WindowKindID));
		foreach (string text in names)
		{
			if (text == "End")
			{
				break;
			}
			if (!(text == "Begin"))
			{
				list.Add(new TMP_Dropdown.OptionData(text));
			}
		}
		_kind.Prepare("WindowKindID", list);
		list.Clear();
		names = Enum.GetNames(typeof(WindowPositionID));
		foreach (string text2 in names)
		{
			if (text2 == "End")
			{
				break;
			}
			if (!(text2 == "Begin"))
			{
				list.Add(new TMP_Dropdown.OptionData(text2));
			}
		}
		_position.Prepare("WindowPositionID", list);
		list.Clear();
		names = Enum.GetNames(typeof(WindowSizeID));
		foreach (string text3 in names)
		{
			if (text3 == "End")
			{
				break;
			}
			if (!(text3 == "Begin"))
			{
				list.Add(new TMP_Dropdown.OptionData(text3));
			}
		}
		_size.Prepare("WindowSizeID", list);
		list.Clear();
		_isWarning.isOn = false;
	}

	public WindowKindID GetWindowKind()
	{
		string text = _kind.GetCurrentOption().text;
		return (WindowKindID)Enum.Parse(typeof(WindowKindID), text);
	}

	public WindowPositionID GetPosition()
	{
		string text = _position.GetCurrentOption().text;
		return (WindowPositionID)Enum.Parse(typeof(WindowPositionID), text);
	}

	public WindowSizeID GetSize()
	{
		string text = _size.GetCurrentOption().text;
		return (WindowSizeID)Enum.Parse(typeof(WindowSizeID), text);
	}

	public bool IsWarning()
	{
		return _isWarning.isOn;
	}
}
