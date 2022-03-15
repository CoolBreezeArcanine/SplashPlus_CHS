using Process.CodeRead;
using UI;
using UnityEngine;

namespace Monitor.CodeRead.Controller
{
	public class EffectDownController : CodeReadControllerBase
	{
		[SerializeField]
		private MultipleImage _downTypeImage;

		public override void Play(AnimationType type)
		{
		}

		public void SetData(CodeReadProcess.CardStatus status)
		{
			_downTypeImage.ChangeSprite((int)(status - 1));
		}
	}
}
