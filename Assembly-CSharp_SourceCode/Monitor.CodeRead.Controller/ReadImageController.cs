using System.Collections;
using Process.CodeRead;
using UI.DaisyChainList;
using UnityEngine;

namespace Monitor.CodeRead.Controller
{
	public class ReadImageController : CodeReadControllerBase
	{
		private const string LoopAnimationName = "Read_Image_Loop";

		protected readonly int ReadImageLoop = Animator.StringToHash("Read_Image_Loop");

		protected readonly int Insert = Animator.StringToHash("Read_Image_Insert");

		protected readonly int Problem = Animator.StringToHash("Read_Image_Problem");

		[SerializeField]
		private GameObject _leftObject;

		[SerializeField]
		private GameObject _rightObject;

		private CodeReadProcess.CodeReaderState _readerState;

		public void Initialize(int playerIndex, Direction direction)
		{
			Initialize(playerIndex);
			switch (direction)
			{
			case Direction.Left:
				_leftObject.gameObject.SetActive(value: false);
				break;
			case Direction.Right:
				_rightObject.gameObject.SetActive(value: false);
				break;
			default:
				_leftObject.gameObject.SetActive(value: false);
				_rightObject.gameObject.SetActive(value: false);
				break;
			}
		}

		public override void Play(AnimationType type)
		{
			if (IsAnimationActive())
			{
				switch (type)
				{
				case AnimationType.In:
					StartCoroutine(PlayCoroutine());
					break;
				case AnimationType.Loop:
					MainAnimator.Play(ReadImageLoop);
					break;
				case AnimationType.Out:
					MainAnimator.Play(Out);
					break;
				}
			}
		}

		public void PlayReadImage(CodeReadProcess.CodeReaderState state)
		{
			_readerState = state;
			if (IsAnimationActive())
			{
				switch (state)
				{
				case CodeReadProcess.CodeReaderState.Insert:
					MainAnimator.Play(Insert);
					break;
				case CodeReadProcess.CodeReaderState.Error:
					MainAnimator.Play(Problem);
					break;
				case CodeReadProcess.CodeReaderState.Normal:
					MainAnimator.Play(ReadImageLoop);
					break;
				}
			}
		}

		private IEnumerator PlayCoroutine()
		{
			MainAnimator.Play(Animator.StringToHash("In"));
			yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(MainAnimator.GetCurrentAnimatorStateInfo(0).length);
			MainAnimator.Play(Animator.StringToHash("Read_Image_Loop"));
			yield return new WaitForEndOfFrame();
			PlayReadImage(_readerState);
		}
	}
}
