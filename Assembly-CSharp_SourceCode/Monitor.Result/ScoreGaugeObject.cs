using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.Result
{
	public class ScoreGaugeObject : MonoBehaviour
	{
		[SerializeField]
		[Header("パーフェクト")]
		private MultipleImage _perfectGauge;

		[SerializeField]
		[Header("クリティカル")]
		private MultipleImage _criticalPerfectGauge;

		[SerializeField]
		[Header("グレート")]
		private Image _greatGauge;

		[SerializeField]
		[Header("グッド")]
		private Image _goodGauge;

		[SerializeField]
		[Header("その他")]
		private TextMeshProUGUI _normalTotalCount;

		[SerializeField]
		private TextMeshProUGUI _activeTotalCount;

		[SerializeField]
		private Image _titleImage;

		[SerializeField]
		private Outline _titleOutline;

		[SerializeField]
		private Outline _outline;

		[SerializeField]
		private Image _background;

		[SerializeField]
		private Image _criticalBackImage;

		[SerializeField]
		private Image _perfectBackImage;

		[SerializeField]
		private Image _decalImage;

		[SerializeField]
		private Image _decal02Image;

		[SerializeField]
		private RectTransform _gaugeParent;

		private Animator _animator;

		private string _animationTriggerKey = "Normal";

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_activeTotalCount.gameObject.SetActive(value: false);
			_perfectBackImage.gameObject.SetActive(value: false);
			_criticalBackImage.gameObject.SetActive(value: false);
			_decalImage.gameObject.SetActive(value: false);
			_decal02Image.gameObject.SetActive(value: false);
			_titleOutline.enabled = false;
		}

		public void SetActiveCriticalPerfectGuage(bool isActive)
		{
			_criticalPerfectGauge.gameObject.SetActive(isActive);
		}

		public void SetMaxCount(string count)
		{
			_normalTotalCount.text = count;
			_activeTotalCount.text = count;
		}

		public void SetPerfectGauge(float rate)
		{
			_perfectGauge.fillAmount = rate;
			if (rate >= 1f)
			{
				_perfectGauge.ChangeSprite(1);
				_perfectGauge.color = Color.white;
			}
		}

		public void SetCriticalPerfectGuage(float rate)
		{
			_criticalPerfectGauge.fillAmount = rate;
		}

		public void SetColor(Color baseColor, Color outlineColor, ScoreGaugeController.ScoreGaugeType decaleIndex)
		{
			_background.color = baseColor;
			_outline.effectColor = outlineColor;
			_activeTotalCount.gameObject.SetActive(value: false);
			switch (decaleIndex)
			{
			case ScoreGaugeController.ScoreGaugeType.Normal:
				_animationTriggerKey = "Normal";
				_titleOutline.enabled = false;
				_normalTotalCount.gameObject.SetActive(value: true);
				break;
			case ScoreGaugeController.ScoreGaugeType.Perfect:
			case ScoreGaugeController.ScoreGaugeType.Critical:
			{
				_titleOutline.enabled = true;
				_activeTotalCount.gameObject.SetActive(value: true);
				_normalTotalCount.gameObject.SetActive(value: false);
				bool flag = decaleIndex == ScoreGaugeController.ScoreGaugeType.Perfect;
				_animationTriggerKey = (flag ? "Perfect" : "Critical");
				break;
			}
			case ScoreGaugeController.ScoreGaugeType.Disable:
				_animationTriggerKey = "Disable";
				break;
			}
		}

		public void Play()
		{
			if (!string.IsNullOrEmpty(_animationTriggerKey))
			{
				_animator.Rebind();
				_animator.SetTrigger(_animationTriggerKey);
			}
		}

		public void Show()
		{
			if (!(_animator == null) && !string.IsNullOrEmpty(_animationTriggerKey))
			{
				_animator.Rebind();
				if (_animationTriggerKey == "Normal")
				{
					_animator.SetTrigger("Show");
				}
				else
				{
					_animator.SetTrigger(_animationTriggerKey);
				}
				int shortNameHash = _animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
				_animator.Play(shortNameHash, 0, 0f);
				_animator.speed = 0f;
			}
		}

		public void SetGauge(float perfect, float critical, float great, float good)
		{
			SetPerfectGauge(perfect);
			SetCriticalPerfectGuage(critical);
			_greatGauge.fillAmount = great;
			_goodGauge.fillAmount = good;
		}

		public void ResetGauge()
		{
			if (_animationTriggerKey == "Normal")
			{
				_gaugeParent.localScale = new Vector3(0f, 1f, 1f);
			}
		}
	}
}
