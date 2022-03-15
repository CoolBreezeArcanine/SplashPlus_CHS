using UnityEngine;
using UnityEngine.UI;

public class CustomSlider : MonoBehaviour
{
	private const float Margin = 5f;

	private const float MinWidth = 20f;

	[SerializeField]
	private Image _bgImage;

	[SerializeField]
	private Image _sliderImage;

	private float baseSize;

	public void Prepare(int num, bool changeSliderSize)
	{
		_sliderImage.transform.localPosition = Vector3.zero;
		float num2 = 10f;
		float x = _bgImage.rectTransform.sizeDelta.x;
		if (changeSliderSize)
		{
			if (num != 0)
			{
				baseSize = (x - num2) / (float)num;
			}
			float num3 = ((baseSize < 20f) ? 20f : baseSize);
			if (num > 1)
			{
				baseSize = (x - num2 - num3) / (float)(num - 1);
			}
			_sliderImage.rectTransform.sizeDelta = new Vector2(num3, _sliderImage.rectTransform.sizeDelta.y);
			_sliderImage.rectTransform.anchoredPosition = new Vector2(5f, _sliderImage.transform.localPosition.y);
		}
		else
		{
			float x2 = _sliderImage.rectTransform.sizeDelta.x;
			baseSize = ((num > 1) ? ((x - num2 - x2) / (float)(num - 1)) : 0f);
		}
	}

	public void MoveSlider(int index)
	{
		float num = 5f;
		num = 5f + baseSize * (float)index;
		float num2 = _bgImage.rectTransform.sizeDelta.x * _sliderImage.rectTransform.pivot.x;
		float num3 = _sliderImage.rectTransform.sizeDelta.x * _sliderImage.rectTransform.pivot.x;
		num += 0f - num2 + num3;
		_sliderImage.rectTransform.anchoredPosition = new Vector2(num, _sliderImage.transform.localPosition.y);
	}

	private void SetBgColor(Color color)
	{
		_bgImage.color = color;
	}

	private void SetSliderColor(Color color)
	{
		_sliderImage.color = color;
	}
}
