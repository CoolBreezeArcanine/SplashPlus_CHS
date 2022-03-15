using Process.CodeRead;
using TMPro;
using UnityEngine;

namespace Monitor.CodeRead.Controller
{
	public class ReadMessageWindow : FixedWindow
	{
		[SerializeField]
		[Header("カード生成先")]
		private RectTransform _readCard;

		[SerializeField]
		[Header("オリジナルカード")]
		private ReadCodeController _originalCodeController;

		[SerializeField]
		[Header("効果ダウン")]
		private RectTransform _parentEffectDown;

		[SerializeField]
		private EffectDownController _originalEffectDownController;

		[SerializeField]
		[Header("拡張メッセージ部")]
		private TextMeshProUGUI _useMessageText;

		[SerializeField]
		private TextMeshProUGUI _boostMessage;

		private Animator _animator;

		private ReadCodeController _readCodeController;

		private EffectDownController _downController;

		public void Initialize()
		{
			_animator = GetComponent<Animator>();
			_readCodeController = Object.Instantiate(_originalCodeController, _readCard).GetComponent<ReadCodeController>();
			_downController = Object.Instantiate(_originalEffectDownController, _parentEffectDown).GetComponent<EffectDownController>();
			_downController.gameObject.SetActive(value: false);
		}

		public void SetData(string message, string subMessage, string boost, CodeReadProcess.CardStatus status)
		{
			_messageText.text = message;
			if (string.IsNullOrEmpty(boost))
			{
				_boostMessage.gameObject.SetActive(value: false);
			}
			else
			{
				_boostMessage.gameObject.SetActive(value: true);
				_boostMessage.text = boost;
			}
			_useMessageText.text = subMessage;
			if ((int)status > 0)
			{
				_downController.gameObject.SetActive(value: true);
				_downController.SetData(status);
			}
			else
			{
				_downController.gameObject.SetActive(value: false);
			}
		}

		public void SetCardData(CodeReadProcess.ReadCard card)
		{
			_readCodeController.SetData(card);
		}

		public void PlayIntroduction()
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			_animator.Play(Animator.StringToHash("In"));
		}

		public void PlayOutroduction()
		{
			if (base.gameObject.activeSelf && base.gameObject.activeInHierarchy)
			{
				_animator.Play(Animator.StringToHash("Out"));
			}
		}
	}
}
