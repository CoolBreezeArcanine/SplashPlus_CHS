using UnityEngine;
using UnityEngine.UI;

public class CustomNum : MonoBehaviour
{
	[SerializeField]
	private Image _bgImage;

	[SerializeField]
	private Sprite[] _numberSprites;

	[SerializeField]
	private Image[] _leftImages;

	[SerializeField]
	private Image[] _rightImages;

	public void Prepare(int numerator, int denominator)
	{
		Image[] leftImages = _leftImages;
		for (int i = 0; i < leftImages.Length; i++)
		{
			leftImages[i].gameObject.SetActive(value: true);
		}
		leftImages = _rightImages;
		for (int i = 0; i < leftImages.Length; i++)
		{
			leftImages[i].gameObject.SetActive(value: true);
		}
		SetNumerator(numerator);
		SetDenominator(denominator);
	}

	private void SetBgColor(Color color)
	{
		_bgImage.color = color;
	}

	public void SetNumerator(int number)
	{
		number = Mathf.Abs(number);
		string text = number.ToString();
		for (int i = 0; i < _leftImages.Length; i++)
		{
			if (i < text.Length)
			{
				_leftImages[i].gameObject.SetActive(value: true);
				_leftImages[i].sprite = _numberSprites[text[i] - 48];
			}
			else
			{
				_leftImages[i].gameObject.SetActive(value: false);
			}
		}
	}

	private void SetDenominator(int number)
	{
		number = Mathf.Abs(number);
		string text = number.ToString();
		for (int i = 0; i < _leftImages.Length; i++)
		{
			if (i < text.Length)
			{
				_rightImages[i].gameObject.SetActive(value: true);
				_rightImages[i].sprite = _numberSprites[text[i] - 48];
			}
			else
			{
				_rightImages[i].gameObject.SetActive(value: false);
			}
		}
	}
}
