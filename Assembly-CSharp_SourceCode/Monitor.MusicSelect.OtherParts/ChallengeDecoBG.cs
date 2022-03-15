using UnityEngine;

namespace Monitor.MusicSelect.OtherParts
{
	public class ChallengeDecoBG : MonoBehaviour
	{
		public enum Anim
		{
			Idle,
			In,
			Loop,
			DiffIn,
			DiffLoop,
			Out
		}

		[SerializeField]
		private Animator _BGAnim;

		[SerializeField]
		private Animator _BGPartsLeftAnim;

		[SerializeField]
		private Animator _BGPartsRightAnim1;

		public void SetAnim(Anim _anim)
		{
			switch (_anim)
			{
			case Anim.Idle:
				_BGAnim.Play("Idle");
				_BGPartsLeftAnim.Play("Idle");
				_BGPartsRightAnim1.Play("Idle");
				break;
			case Anim.In:
				_BGAnim.Play("In");
				_BGPartsLeftAnim.Play("In");
				_BGPartsRightAnim1.Play("In");
				break;
			case Anim.Loop:
				_BGAnim.Play("Loop");
				_BGPartsLeftAnim.Play("Loop");
				_BGPartsRightAnim1.Play("Loop");
				break;
			case Anim.DiffIn:
				_BGAnim.Play("In_Select");
				break;
			case Anim.DiffLoop:
				_BGAnim.Play("Loop_Select");
				break;
			case Anim.Out:
				_BGAnim.Play("Out");
				_BGPartsLeftAnim.Play("Out");
				_BGPartsRightAnim1.Play("Out");
				break;
			}
		}
	}
}
