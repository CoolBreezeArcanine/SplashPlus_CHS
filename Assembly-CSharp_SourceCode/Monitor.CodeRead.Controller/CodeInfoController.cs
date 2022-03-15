using Process.CodeRead;
using TMPro;
using UI;
using UnityEngine;

namespace Monitor.CodeRead.Controller
{
	public class CodeInfoController : CodeReadControllerBase
	{
		[SerializeField]
		private MultipleImage _backgroundImage;

		[SerializeField]
		private TextMeshProUGUI _titleText;

		[SerializeField]
		private TextMeshProUGUI _messageText;

		[SerializeField]
		private TextMeshProUGUI _boostText;

		public void SetData(int playerIndex, string title, string message, string boost)
		{
			_backgroundImage.ChangeSprite(playerIndex);
			_titleText.text = title;
			_messageText.text = message;
			_boostText.gameObject.SetActive(!string.IsNullOrEmpty(boost));
			_boostText.text = boost;
		}

		public override void Play(AnimationType type)
		{
			if (IsAnimationActive())
			{
				switch (type)
				{
				case AnimationType.In:
					MainAnimator.Play(In);
					break;
				case AnimationType.Out:
					MainAnimator.Play(Out);
					break;
				}
			}
		}

		public void PlayChoiceIn()
		{
			if (IsAnimationActive())
			{
				MainAnimator.Play(Animator.StringToHash("Choice_in"), 0, 0f);
			}
		}
	}
}
