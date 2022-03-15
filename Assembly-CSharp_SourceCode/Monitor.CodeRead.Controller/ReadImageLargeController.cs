using System.Collections;
using Process.CodeRead;
using UnityEngine;

namespace Monitor.CodeRead.Controller
{
	public class ReadImageLargeController : CodeReadControllerBase
	{
		[SerializeField]
		private RectTransform _derakkuma;

		[SerializeField]
		private GameObject _derakkumaOriginal;

		[SerializeField]
		private CanvasGroup _canvasGroup;

		private Animator _derakkumaAnimator;

		private CardReadPlayer _player;

		private int _layer1P;

		private int _layer2P;

		private int _layer1P2P;

		private int _layerError;

		public override void Initialize(int playerIndex)
		{
			base.Initialize(playerIndex);
			_derakkumaAnimator = Object.Instantiate(_derakkumaOriginal, _derakkuma).GetComponent<Animator>();
			_layer1P = MainAnimator.GetLayerIndex("1P");
			_layer2P = MainAnimator.GetLayerIndex("2P");
			_layer1P2P = MainAnimator.GetLayerIndex("1P2P");
			_layerError = MainAnimator.GetLayerIndex("Error");
			MainAnimator.SetLayerWeight(_layer1P, 0f);
			MainAnimator.SetLayerWeight(_layer2P, 0f);
			MainAnimator.SetLayerWeight(_layer1P2P, 0f);
			MainAnimator.SetLayerWeight(_layerError, 0f);
			_canvasGroup.alpha = 0f;
		}

		public void SetData(CardReadPlayer player)
		{
			_player = player;
		}

		public Coroutine PlayIntroduction(bool isInit)
		{
			return StartCoroutine(PlayIntroductionCoroutine(isInit));
		}

		private IEnumerator PlayIntroductionCoroutine(bool isInit)
		{
			MainAnimator.SetLayerWeight((int)(_player + 2), 1f);
			yield return new WaitForEndOfFrame();
			_canvasGroup.alpha = 1f;
			_derakkumaAnimator.Play("fun_01_Loop");
			if (isInit)
			{
				MainAnimator.Play("In");
			}
			yield return new WaitForSeconds(MainAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
		}

		public override void Play(AnimationType type)
		{
			switch (type)
			{
			case AnimationType.Loop:
				PlayLoop();
				break;
			case AnimationType.Out:
				PlayOutroduction();
				break;
			case AnimationType.In:
				break;
			}
		}

		private void PlayLoop()
		{
			StartCoroutine(PlayLoopCoroutine());
		}

		private IEnumerator PlayLoopCoroutine()
		{
			_derakkumaAnimator.SetLayerWeight(_derakkumaAnimator.GetLayerIndex("Result"), 0f);
			yield return new WaitForEndOfFrame();
			MainAnimator.Play(Animator.StringToHash("Read_Image_Large_Loop"));
			_derakkumaAnimator.Play(Animator.StringToHash("fun_01_Loop"));
		}

		public void PlayAbnormality()
		{
			if (IsAnimationActive())
			{
				MainAnimator.SetLayerWeight(_layer1P, 0f);
				MainAnimator.SetLayerWeight(_layer2P, 0f);
				MainAnimator.SetLayerWeight(_layer1P2P, 0f);
				MainAnimator.SetLayerWeight(_layerError, 1f);
			}
		}

		private void PlayOutroduction()
		{
			MainAnimator.Play(Animator.StringToHash("Out"));
		}
	}
}
