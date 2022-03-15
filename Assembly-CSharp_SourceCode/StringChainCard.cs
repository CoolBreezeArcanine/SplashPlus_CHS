using TMPro;
using UI.DaisyChainList;
using UnityEngine;

public class StringChainCard : ChainObject
{
	[SerializeField]
	[Header("StringCard")]
	private TextMeshProUGUI _stringText;

	[SerializeField]
	private TextMeshProUGUI _stringShadowText;

	[SerializeField]
	private CanvasGroup _miniCardGroup;

	[SerializeField]
	private CanvasGroup _mainCardGroup;

	[SerializeField]
	private StringChainCard _miniCard;

	private const string PREFIX_TAG = "<size=190%>";

	private const string SUFFIX_TAG = "</size>";

	public void Prepare(string text)
	{
		_stringText.text = text;
		_stringShadowText.text = text;
		if (_miniCard != null)
		{
			string text2 = "<size=190%>" + text + "</size>";
			_miniCard.Prepare(text2);
		}
	}

	public void ChangeSize(bool isMainActive)
	{
		if (_miniCard != null)
		{
			_miniCardGroup.alpha = ((!isMainActive) ? 1 : 0);
			_mainCardGroup.alpha = (isMainActive ? 1 : 0);
		}
	}

	public override void OnCenterIn()
	{
		ChangeSize(isMainActive: true);
	}

	public override void OnCenterOut()
	{
		ChangeSize(isMainActive: false);
	}
}
