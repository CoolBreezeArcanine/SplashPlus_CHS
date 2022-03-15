using TMPro;
using UI;
using UnityEngine;

public class CategorytabButtonObject : CommonButtonObject
{
	[SerializeField]
	private TextMeshProUGUI _tabNameText;

	[SerializeField]
	private TextMeshProUGUI _tabCountText;

	public void SetTabName(string tabName, string count)
	{
		_tabNameText.text = tabName;
	}

	public void SetTabColor(Color color)
	{
		_tabNameText.color = color;
	}
}
