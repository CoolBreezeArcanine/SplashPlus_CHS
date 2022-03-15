using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorViewer : MonoBehaviour, IColorViewer
{
	[SerializeField]
	private Image _viewer;

	[SerializeField]
	private TMP_InputField _inputField;

	[SerializeField]
	private Button _button;

	private void Awake()
	{
		_button.onClick.RemoveAllListeners();
		_button.onClick.AddListener(ChangeColor);
	}

	public void ChangeColor()
	{
		ColorUtility.TryParseHtmlString(_inputField.text, out var color);
		_viewer.color = color;
	}

	public string GetColorCode()
	{
		return _inputField.text;
	}
}
