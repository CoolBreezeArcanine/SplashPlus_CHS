using TMPro;
using UnityEngine;

public class SettingIndicator : MonoBehaviour
{
	[SerializeField]
	private GameObject[] _offSwitchArray;

	[SerializeField]
	private TextMeshProUGUI _headPhoneVolume;

	public void SetSettingState(int index, bool state)
	{
		_offSwitchArray[index].SetActive(!state);
	}

	public void SetVolume(string volume)
	{
		_headPhoneVolume.text = volume;
	}
}
