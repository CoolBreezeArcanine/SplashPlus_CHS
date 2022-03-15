using UnityEngine;

namespace Monitor.MusicSelect.OtherParts
{
	public class ChallengeBG : MonoBehaviour
	{
		public enum Anim
		{
			Idle,
			In,
			Loop,
			Out
		}

		[SerializeField]
		private Animator _BGAnim;

		public void SetAnim(Anim _anim)
		{
			switch (_anim)
			{
			case Anim.Idle:
				_BGAnim.Play("Idle");
				break;
			case Anim.In:
				_BGAnim.Play("In");
				break;
			case Anim.Loop:
				_BGAnim.Play("Loop");
				break;
			case Anim.Out:
				_BGAnim.Play("Out");
				break;
			}
		}
	}
}
