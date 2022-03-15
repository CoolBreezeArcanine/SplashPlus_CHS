using Fx;
using Manager;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.Entry.Util
{
	public class EntryButtonUI : CommonButtonObject
	{
		private Image _image;

		public Animator Animator => ButtonAnimator;

		public InputManager.ButtonSetting ButtonSetting => ButtonIndex;

		public LedColors LedColor => _ledColor;

		private void Awake()
		{
			ButtonAnimator = GetComponent<Animator>();
			_image = GetComponentInChildren<Image>();
			ParticleControler = base.transform.parent.GetComponentInChildren<FX_Controler>();
			EffectTransform = base.transform;
		}

		public override void SetActiveButton(bool isActive)
		{
			if (isActive)
			{
				ButtonAnimator.Play("In");
			}
			else
			{
				ButtonAnimator.SetTrigger("Out");
			}
		}

		public void SetImageColor(Color color)
		{
			_image.color = color;
		}
	}
}
