using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameField : MonoBehaviour
{
	[SerializeField]
	private Image _fieldBgImage;

	[SerializeField]
	private TextMeshProUGUI _stringText;

	public void Prepare(int index)
	{
		_stringText.text = "";
	}

	public void SetString(string text)
	{
		_stringText.text = ((text == "␣") ? "\u3000" : text);
	}

	public string GetStr()
	{
		return _stringText.text;
	}

	public void SetFieldBgSprite(Sprite sp)
	{
		_fieldBgImage.sprite = sp;
	}
}
