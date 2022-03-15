using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniTabPanel : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	private Image _bgImage;

	[SerializeField]
	private TextMeshProUGUI _categoryNameText;

	public void Prepare(Color bgColor, string categoryName)
	{
		_bgImage.color = bgColor;
		_categoryNameText.text = categoryName;
	}

	public void SetVisible(bool isActive)
	{
		_canvasGroup.alpha = (isActive ? 1 : 0);
	}
}
