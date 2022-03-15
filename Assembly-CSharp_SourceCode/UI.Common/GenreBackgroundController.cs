using UnityEngine;
using UnityEngine.UI;

namespace UI.Common
{
	public class GenreBackgroundController : MonoBehaviour
	{
		public enum AnimationType
		{
			Idle,
			In,
			Loop,
			Out,
			ChangeIn,
			ChangeOut
		}

		private enum AnimState
		{
			Fade,
			Loop,
			Change
		}

		private const string Idle = "Idle";

		private const string In = "In";

		private const string Loop = "Loop";

		private const string Out = "Out";

		private const string ChangeIn = "Change_Out";

		private const string ChangeOut = "Change_In";

		private const string InSelect = "In_Select";

		private const string OutSelect = "Out_Select";

		private const string OutBack = "Out_Back";

		private const string Disabled = "Disabled";

		[SerializeField]
		[Header("文字スクロール")]
		private CustomTextScroll _scrollText;

		[SerializeField]
		[Header("上方サンプル画像")]
		private RawImage[] _sampleSprite;

		[SerializeField]
		[Header("アニメーター")]
		private Animator _animator;

		[SerializeField]
		private Animator _bgAnimator;

		[SerializeField]
		[Header("熊インジケーター")]
		private CustomSlider _Indicator;

		private bool _prepareChangeJacket;

		private bool _loopStart;

		private bool _isAnimation;

		public int LoopCount { get; set; }

		public void AnimPlay(AnimationType type)
		{
			string value = "";
			int layer = 0;
			switch (type)
			{
			default:
				return;
			case AnimationType.Idle:
				value = "Idle";
				base.gameObject.SetActive(value: true);
				break;
			case AnimationType.In:
				if (!_isAnimation)
				{
					base.gameObject.SetActive(value: true);
					LoopCount = 0;
					value = "In";
					_isAnimation = true;
				}
				break;
			case AnimationType.Loop:
				base.gameObject.SetActive(value: true);
				value = "Loop";
				layer = 1;
				break;
			case AnimationType.Out:
				if (_isAnimation)
				{
					base.gameObject.SetActive(value: true);
					value = "Out";
					_isAnimation = false;
				}
				break;
			case AnimationType.ChangeIn:
				base.gameObject.SetActive(value: true);
				LoopCount = 0;
				value = "Change_Out";
				layer = 2;
				_prepareChangeJacket = false;
				break;
			case AnimationType.ChangeOut:
				base.gameObject.SetActive(value: true);
				value = "Change_In";
				layer = 2;
				_prepareChangeJacket = true;
				break;
			}
			if (!string.IsNullOrEmpty(value))
			{
				_animator.Play(Animator.StringToHash(value), layer, 0f);
			}
		}

		public void InitIndicator(int maxNum)
		{
			_Indicator?.Prepare(maxNum, changeSliderSize: false);
		}

		public void MoveIndicator(int num)
		{
			_Indicator?.MoveSlider(num);
		}

		public int GetSpriteLength()
		{
			return _sampleSprite.Length;
		}

		public void SetSampleImage(Texture2D[] texture)
		{
			for (int i = 0; i < _sampleSprite.Length; i++)
			{
				if (_sampleSprite[i] != null && texture[i % texture.Length] != null)
				{
					_sampleSprite[i].texture = texture[i % texture.Length];
				}
			}
		}

		public void SetNextSampleImage(Texture2D texture)
		{
			for (int i = 0; i < _sampleSprite.Length - 1; i++)
			{
				if (_sampleSprite[i] != null && _sampleSprite[i + 1] != null)
				{
					_sampleSprite[i].texture = _sampleSprite[i + 1].texture;
				}
			}
			if (_sampleSprite[_sampleSprite.Length - 1] != null)
			{
				_sampleSprite[_sampleSprite.Length - 1].texture = texture;
			}
			AnimPlay(AnimationType.Loop);
			_loopStart = true;
		}

		public bool GetLoopEnd()
		{
			if (!_loopStart)
			{
				if (_animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 1f)
				{
					return true;
				}
			}
			else
			{
				_loopStart = false;
			}
			return false;
		}

		public bool GetChangeEnd()
		{
			if (_prepareChangeJacket && _animator.GetCurrentAnimatorStateInfo(2).normalizedTime >= 1f)
			{
				return true;
			}
			return false;
		}

		public void SetScrollMessage(string message)
		{
			if (_scrollText != null)
			{
				_scrollText.SetData(message);
				_scrollText.ResetPosition();
			}
		}

		public void UpdateScroll()
		{
			if (_scrollText != null)
			{
				_scrollText.ViewUpdate();
			}
		}
	}
}
