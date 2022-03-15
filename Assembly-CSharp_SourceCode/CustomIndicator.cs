using UnityEngine;

public class CustomIndicator : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	private CustomNum _number;

	[SerializeField]
	private CustomSlider _slider;

	public void SetVisible(bool isActive)
	{
		base.gameObject.SetActive(isActive);
	}

	public void Preapre(int numerator, int denominator)
	{
		_number.Prepare(numerator, denominator);
		_slider.Prepare(denominator, changeSliderSize: true);
	}

	public void ViewUpdate(int index, int number)
	{
		MoveSlider(index);
		SetNumerator(number);
	}

	private void MoveSlider(int index)
	{
		_slider.MoveSlider(index);
	}

	private void SetNumerator(int number)
	{
		_number.SetNumerator(number);
	}
}
