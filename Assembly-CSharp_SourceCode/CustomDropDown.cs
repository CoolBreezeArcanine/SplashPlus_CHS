using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CustomDropDown : MonoBehaviour
{
	[SerializeField]
	private TMP_Dropdown _dropDown;

	public TMP_Dropdown.OptionData GetCurrentOption()
	{
		int value = _dropDown.value;
		return _dropDown.options[value];
	}

	public void Prepare(string captionText, List<TMP_Dropdown.OptionData> optionData)
	{
		_dropDown.captionText.text = captionText;
		_dropDown.ClearOptions();
		_dropDown.AddOptions(optionData);
		_dropDown.value = 0;
	}

	public void AddListener(UnityAction<int> action)
	{
		_dropDown.onValueChanged.RemoveAllListeners();
		_dropDown.onValueChanged.AddListener(action);
	}
}
