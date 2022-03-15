using DB;
using TMPro;
using UnityEngine;

namespace Monitor.MusicSelect.OtherParts
{
	public class GhostJumpInfo : MonoBehaviour
	{
		public enum Anim
		{
			Idle,
			In,
			Loop
		}

		[SerializeField]
		private Animator _BGAnim;

		[SerializeField]
		private TextMeshProUGUI _jumpText;

		[SerializeField]
		private TextMeshProUGUI _returnText;

		public void Initialize()
		{
			_jumpText.SetText(CommonMessageID.GhostJumpText.GetName());
			_returnText.SetText(CommonMessageID.GhostReturnText.GetName());
		}

		public void SetAnim(Anim _anim, bool isReturn)
		{
			if (!isReturn)
			{
				switch (_anim)
				{
				case Anim.Idle:
					_BGAnim.Play("Idle");
					break;
				case Anim.In:
					_BGAnim.Play("In_Info01");
					break;
				case Anim.Loop:
					_BGAnim.Play("Loop_Info01");
					break;
				}
			}
			else
			{
				switch (_anim)
				{
				case Anim.Idle:
					_BGAnim.Play("Idle");
					break;
				case Anim.In:
					_BGAnim.Play("In_Info02");
					break;
				case Anim.Loop:
					_BGAnim.Play("Loop_Info02");
					break;
				}
			}
		}
	}
}
