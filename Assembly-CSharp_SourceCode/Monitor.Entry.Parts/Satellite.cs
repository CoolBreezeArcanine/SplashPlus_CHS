using Monitor.MapCore;
using TMPro;
using UnityEngine;

namespace Monitor.Entry.Parts
{
	public class Satellite : MapBehaviour
	{
		[SerializeField]
		private Animator _animator;

		[SerializeField]
		private GameObject _face;

		[SerializeField]
		private Animator _animatorName;

		[SerializeField]
		private TextMeshProUGUI _textName;

		public void Open(SatelliteEntryType type, string text)
		{
			_animator.Play("In");
			switch (type)
			{
			case SatelliteEntryType.None:
				_face.SetActive(value: false);
				break;
			case SatelliteEntryType.Aime:
			case SatelliteEntryType.Guest:
				_face.SetActive(value: true);
				if (!string.IsNullOrEmpty(text))
				{
					_animatorName.Play("In");
					_animatorName.GetComponent<CanvasGroup>().alpha = 1f;
					_textName.text = text;
				}
				break;
			}
		}

		public bool IsLoopState()
		{
			return _animator.GetCurrentAnimatorStateInfo(_animator.GetLayerIndex("Base Layer")).IsName("Base Layer.Loop");
		}

		public void SyncLoopAnimation()
		{
			_animator.PlayInFixedTime("Loop", -1, 0f);
			_animatorName.PlayInFixedTime("Loop", -1, 0f);
		}
	}
}
