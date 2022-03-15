using Process.CodeRead;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.CodeRead.Controller
{
	public class ReadCodeController : CodeReadControllerBase
	{
		[SerializeField]
		private Image _background;

		[SerializeField]
		private Image _character;

		[SerializeField]
		private Image _frame;

		public override void Play(AnimationType type)
		{
		}

		public void SetData(Sprite background, Sprite character, Sprite frame)
		{
			_background.gameObject.SetActive(background != null);
			_background.sprite = background;
			_character.gameObject.SetActive(character != null);
			_character.sprite = character;
			_frame.gameObject.SetActive(frame != null);
			_frame.sprite = frame;
		}

		public void SetData(CodeReadProcess.ReadCard card)
		{
			SetData(card.Background, card.Character, card.Frame);
		}
	}
}
