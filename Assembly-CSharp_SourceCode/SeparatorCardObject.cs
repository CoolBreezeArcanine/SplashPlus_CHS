using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SeparatorCardObject : SelectCardObject
{
	[SerializeField]
	private Image arrowImage;

	[SerializeField]
	private TextMeshProUGUI categoryNameText;

	[SerializeField]
	private Sprite left;

	[SerializeField]
	private Sprite right;

	public void ModeSwitch(bool isLeft, string name)
	{
		categoryNameText.text = name;
		if (isLeft)
		{
			arrowImage.sprite = left;
			categoryNameText.rectTransform.localPosition = new Vector3(0f, 12.5f, 0f);
		}
		else
		{
			arrowImage.sprite = right;
			categoryNameText.rectTransform.localPosition = new Vector3(0f, 12.5f, 0f);
		}
	}
}
