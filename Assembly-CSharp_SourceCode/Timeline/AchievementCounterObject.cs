using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Timeline
{
	public class AchievementCounterObject : TimeControlBaseObject
	{
		[SerializeField]
		private float _fromAchievement;

		[SerializeField]
		private float _toAchievement;

		[SerializeField]
		[Header("達成率")]
		[Tooltip("達成率\u3000整数部")]
		private SpriteCounter[] _rateIntegerCounter;

		[SerializeField]
		[Tooltip("達成率\u3000少数1位２位")]
		private SpriteCounter[] _rateDecimal12Counter;

		[SerializeField]
		[Tooltip("達成率\u3000少数３位４位")]
		private SpriteCounter[] _rateDecimal34Counter;

		[SerializeField]
		[Header("数字")]
		private List<Sprite[]> _achievementTypeSprites;

		[SerializeField]
		private AchievementFontData[] _fontDatas;

		[SerializeField]
		private Animator _activeAnimator;

		[SerializeField]
		[Header("パーセント")]
		private MultipleImage _parsentImage;

		[SerializeField]
		private Image _parsentShadowImage;

		private int _state = -1;

		private static readonly int Active = Animator.StringToHash("Active");

		private void OnValidate()
		{
		}

		public override void OnBehaviourPlay()
		{
			UpdateScore(0f);
		}

		public override void OnClipPlay()
		{
			UpdateScore(0f);
		}

		public override void OnClipTailEnd()
		{
			UpdateScore(1f);
		}

		public override void OnClipHeadEnd()
		{
			UpdateScore(0f);
		}

		public override void OnGraphStop()
		{
			UpdateScore(0f);
		}

		public override void PrepareFrame(double normalizeTime)
		{
			UpdateScore((float)normalizeTime);
		}

		public void SetAchievement(uint from, uint to)
		{
			_fromAchievement = from;
			_toAchievement = to;
		}

		private void UpdateScore(float normalize)
		{
			uint num = (uint)Mathf.Lerp(_fromAchievement, _toAchievement, normalize);
			if (num < 800000)
			{
				if (_state != 0)
				{
					_state = 0;
					ChangeCounter();
				}
			}
			else if (num < 970000)
			{
				if (_state != 1)
				{
					_state = 1;
					ChangeCounter();
					if (Application.isPlaying && _activeAnimator != null)
					{
						_activeAnimator.SetTrigger(Active);
					}
				}
			}
			else if (_state != 2)
			{
				_state = 2;
				ChangeCounter();
				if (Application.isPlaying && _activeAnimator != null)
				{
					_activeAnimator.SetTrigger(Active);
				}
			}
			string text = Convert.ToString(num).PadLeft(7, '0');
			string text2 = string.Format("{0,3}", num / 10000u) + ".";
			string text3 = text.Substring(3, 2);
			string text4 = text.Substring(5, 2);
			SpriteCounter[] rateIntegerCounter = _rateIntegerCounter;
			for (int i = 0; i < rateIntegerCounter.Length; i++)
			{
				rateIntegerCounter[i]?.ChangeText(text2);
			}
			rateIntegerCounter = _rateDecimal12Counter;
			for (int i = 0; i < rateIntegerCounter.Length; i++)
			{
				rateIntegerCounter[i]?.ChangeText(text3);
			}
			rateIntegerCounter = _rateDecimal34Counter;
			for (int i = 0; i < rateIntegerCounter.Length; i++)
			{
				rateIntegerCounter[i]?.ChangeText(text4);
			}
		}

		private void ChangeCounter()
		{
			_parsentImage.ChangeSprite(_state);
			if (_parsentShadowImage != null)
			{
				_parsentShadowImage.color = _fontDatas[_state].ShadowColor;
			}
			_rateIntegerCounter[0]?.SetColor(_fontDatas[_state].ShadowColor);
			_rateIntegerCounter[1]?.SetColor(_fontDatas[_state].Base01Color);
			_rateIntegerCounter[2]?.SetColor(_fontDatas[_state].Base02Color);
			_rateIntegerCounter[3]?.SetSpriteSheet(CommonPrefab.GetAchieveIntSprites(_state));
			_rateDecimal12Counter[0]?.SetColor(_fontDatas[_state].ShadowColor);
			_rateDecimal12Counter[1]?.SetColor(_fontDatas[_state].Base01Color);
			_rateDecimal12Counter[2]?.SetColor(_fontDatas[_state].Base02Color);
			_rateDecimal12Counter[3]?.SetSpriteSheet(CommonPrefab.GetAchieveDecimalSprites(_state));
			_rateDecimal34Counter[0]?.SetColor(_fontDatas[_state].ShadowColor);
			_rateDecimal34Counter[1]?.SetColor(_fontDatas[_state].Base01Color);
			_rateDecimal34Counter[2]?.SetColor(_fontDatas[_state].Base02Color);
			_rateDecimal34Counter[3]?.SetSpriteSheet(CommonPrefab.GetAchieveDecimalSprites(_state));
		}
	}
}
