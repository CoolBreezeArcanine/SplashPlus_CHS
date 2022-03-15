using UI;
using UnityEngine;

namespace Monitor.CharacterSelect
{
	public class CharacterSelectSubItemObject : MonoBehaviour
	{
		[SerializeField]
		private GameObject _setBaseObject;

		[SerializeField]
		private GameObject _setTitleIObject;

		[SerializeField]
		private MultipleImage _weakPointImage;

		[SerializeField]
		private InstantiateGenerator _characterGenerator;

		private CharaParts _charaParts;

		public void Initialize()
		{
			_charaParts = _characterGenerator.Instantiate<CharaParts>();
			_charaParts.SetBlank();
		}

		public void SetInformation(bool isParty, WeakPoint weak)
		{
			_setBaseObject.SetActive(isParty);
			_setTitleIObject.SetActive(isParty);
			_weakPointImage.ChangeSprite((int)weak);
		}

		public void SetData(int level, float amount, int awakeNum, Sprite backBase, Sprite face, Sprite levelBase, Sprite starSmallBase, Sprite starLargeBase, Sprite starLargeFrame, Color shadowColor)
		{
			_charaParts.SetParts(backBase, face, levelBase, starSmallBase, starLargeBase, starLargeFrame, amount, level, awakeNum, shadowColor);
			_charaParts.PlayJoinParty(OnPlayJoinPartyNext);
		}

		private void OnPlayJoinPartyNext()
		{
			_charaParts.SetVisibleDecorate(isVisible: true);
		}
	}
}
