using TMPro;
using UnityEngine;

namespace Monitor.Common
{
	public class MaintenanceInformationLayerObject : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _text;

		private Animator _animator;

		private string _message;

		private int _param;

		public void Initialize(string message)
		{
			_animator = GetComponent<Animator>();
			_animator.PlayInFixedTime("Out", -1, float.MaxValue);
			_message = message;
			_text.text = _message;
		}

		public void SetParameter(int param)
		{
			if (_param != param)
			{
				_param = param;
				_text.text = string.Format(_message, _param);
			}
		}

		public void SetText(string text)
		{
			if (_message != text)
			{
				_message = text;
				_text.text = _message;
				_param = 0;
			}
		}

		public void Play(string stateName)
		{
			_animator.Play(stateName);
		}

		public void Play(int hash)
		{
			_animator.Play(hash);
		}
	}
}
