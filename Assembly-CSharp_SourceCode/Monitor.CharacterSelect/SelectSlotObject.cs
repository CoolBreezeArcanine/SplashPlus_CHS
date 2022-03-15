using UI;
using UnityEngine;

namespace Monitor.CharacterSelect
{
	public class SelectSlotObject : MonoBehaviour
	{
		[SerializeField]
		private MultipleImage _stateImage;

		[SerializeField]
		private MultipleImage _textImage;

		[SerializeField]
		private OdoSpriteTexts _distanceText;

		[SerializeField]
		private ImageColorGroup _colorGroup;

		[SerializeField]
		private Animator _weakPointStateAnimator;

		[SerializeField]
		[Header("タッチメッセージオブジェクト")]
		private GameObject _touchObject;

		[SerializeField]
		[Header("キャラクター")]
		private InstantiateGenerator _character;

		[SerializeField]
		[Header("エフェクト")]
		private InstantiateGenerator _emotionIcon;

		[SerializeField]
		[Header("ActiveRing")]
		private GameObject _activeRing;

		private Animator _animator;

		private Animator _instanceEffectAnimator;

		private bool _isPlayingLoop;

		public CharaParts InstanceCharaParts { get; private set; }

		public void Initialize()
		{
			_animator = GetComponent<Animator>();
			InstanceCharaParts = _character.Instantiate<CharaParts>();
			_instanceEffectAnimator = _emotionIcon.Instantiate<Animator>();
			SetBlank();
			_isPlayingLoop = true;
		}

		public void ViewUpdate(float normalizeTime)
		{
			if (_isPlayingLoop)
			{
				_animator.SetFloat("NormalizedTime", normalizeTime);
			}
		}

		public void Play()
		{
			_animator.Play(Animator.StringToHash("In"));
		}

		public void SetBlank()
		{
			_touchObject.SetActive(value: false);
			_instanceEffectAnimator.gameObject.SetActive(value: false);
			_stateImage.gameObject.SetActive(value: false);
			_textImage.gameObject.SetActive(value: false);
			InstanceCharaParts.SetBlank();
		}

		public void SetActiveNewcomerMode(bool isActive)
		{
			_animator.SetLayerWeight(1, isActive ? 1f : 0f);
			_touchObject.SetActive(isActive);
		}

		public void SetVisibleTouchMessageObject(bool isVisible)
		{
			_touchObject.SetActive(isVisible);
		}

		public void SetForteWeak(int distance, WeakPoint targetPoint)
		{
			string text = ((targetPoint == WeakPoint.Forte) ? "Loop_UP" : "Loop_DOWN");
			_distanceText.SetDistance(distance);
			if (!_stateImage.gameObject.activeSelf)
			{
				_stateImage.gameObject.SetActive(value: true);
			}
			if (!_textImage.gameObject.activeSelf)
			{
				_textImage.gameObject.SetActive(value: true);
			}
			if (targetPoint == WeakPoint.Leader)
			{
				text = "Loop_UP";
				_stateImage.ChangeSprite(0);
				_colorGroup.SetColor(0);
				_textImage.ChangeSprite(2);
			}
			else
			{
				_stateImage.ChangeSprite((int)targetPoint);
				_textImage.ChangeSprite((int)targetPoint);
				_colorGroup.SetColor((int)targetPoint);
			}
			if (!_instanceEffectAnimator.gameObject.activeSelf)
			{
				_instanceEffectAnimator.gameObject.SetActive(value: true);
			}
			_instanceEffectAnimator.Play(Animator.StringToHash(text));
			_weakPointStateAnimator.Play(Animator.StringToHash("Loop"), 0, 0f);
		}

		public void PlayJoinParty()
		{
			_animator.Play(Animator.StringToHash("ActiveRing"));
			InstanceCharaParts.SetVisibleDecorate(isVisible: true);
		}

		public void SetActiveRing(bool isActive)
		{
			_isPlayingLoop = isActive;
			_activeRing.SetActive(isActive);
		}
	}
}
