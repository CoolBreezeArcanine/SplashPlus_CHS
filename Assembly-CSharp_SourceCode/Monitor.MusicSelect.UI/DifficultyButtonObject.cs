using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.MusicSelect.UI
{
	public class DifficultyButtonObject : CommonButtonObject
	{
		[SerializeField]
		[Header("AdvancedSettings")]
		private Image _baseImage;

		[SerializeField]
		private Animator _mainAnim;

		public void ChangeDifficulty(Sprite symbolSprite, Sprite typeASprite, bool isFlip, int diff)
		{
			SetSymbolSprite(symbolSprite, isFlip);
			_baseImage.sprite = typeASprite;
			if (base.gameObject.activeInHierarchy && base.gameObject.activeSelf)
			{
				_mainAnim.SetTrigger("Loop");
			}
		}

		public override void Pressed()
		{
			if (base.gameObject.activeInHierarchy && base.gameObject.activeSelf)
			{
				_mainAnim.SetTrigger("Pressed");
			}
			if (base.gameObject.activeInHierarchy)
			{
				ParticleControler?.SetTransform(EffectTransform.position);
				ParticleControler?.Play();
			}
		}
	}
}
