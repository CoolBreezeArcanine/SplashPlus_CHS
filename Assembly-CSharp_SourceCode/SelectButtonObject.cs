using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SelectButtonObject : MonoBehaviour
{
	[SerializeField]
	private Image symbolImage;

	[SerializeField]
	private Sprite activeSprite;

	[SerializeField]
	private Sprite normalSprite;

	[SerializeField]
	private Vector3 _activePosition;

	[SerializeField]
	private Vector3 _normalPosition;

	[SerializeField]
	private Color _activeColor = Color.white;

	[SerializeField]
	private Color _normalColor = Color.white;

	[SerializeField]
	private Sprite[] _buttonSprites;

	private Image buttonImage;

	private Animator _buttonAnimator;

	private void Awake()
	{
		buttonImage = GetComponent<Image>();
		_buttonAnimator = GetComponent<Animator>();
	}

	public void SetVisibleButton(bool isVisible)
	{
		if (base.gameObject.activeSelf != isVisible)
		{
			base.gameObject.SetActive(isVisible);
			if (isVisible && _buttonAnimator != null)
			{
				_buttonAnimator.SetTrigger("IntroductionTrigger");
			}
		}
	}

	public void SetAnimationActive(bool isActive)
	{
		if (isActive)
		{
			if (activeSprite != null)
			{
				buttonImage.sprite = activeSprite;
			}
			if (symbolImage != null)
			{
				symbolImage.transform.localPosition = _activePosition;
			}
			buttonImage.color = _activeColor;
		}
		else
		{
			if (normalSprite != null)
			{
				buttonImage.sprite = normalSprite;
			}
			if (symbolImage != null)
			{
				symbolImage.transform.localPosition = _normalPosition;
			}
			buttonImage.color = _normalColor;
		}
	}

	public void ChangeButtonImage(int index)
	{
		if (_buttonSprites.Length != 0 && index < _buttonSprites.Length)
		{
			buttonImage.sprite = _buttonSprites[index];
		}
	}
}
