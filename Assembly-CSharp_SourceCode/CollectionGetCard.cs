using UI;
using UnityEngine;
using UnityEngine.UI;

public class CollectionGetCard : MonoBehaviour
{
	public enum CardType
	{
		Blank,
		NamePlate,
		Icon,
		Title,
		Partner,
		Frame,
		Ticket
	}

	[SerializeField]
	private MultipleImage _baseImage;

	[SerializeField]
	private RectMask2D _mask;

	[SerializeField]
	private Image _mainImage;

	[SerializeField]
	private GameObject _plateSetObj;

	[SerializeField]
	private Image _frameImage;

	[SerializeField]
	private MultiImage _multiImage;

	private Animator _animator;

	private CardType _cardType;

	public bool IsBlank => _cardType == CardType.Blank;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
	}

	public void SetCard(CardType cardType, Sprite sprite)
	{
		_cardType = cardType;
		ChangeBase(_cardType);
		if (_cardType == CardType.NamePlate)
		{
			_multiImage.Image2 = sprite;
			_mainImage.gameObject.SetActive(value: false);
			_frameImage.gameObject.SetActive(value: false);
		}
		else if (_cardType == CardType.Frame)
		{
			_frameImage.sprite = sprite;
			_mainImage.gameObject.SetActive(value: false);
			_plateSetObj.SetActive(value: false);
		}
		else
		{
			_mainImage.sprite = sprite;
			_mainImage.SetNativeSize();
			_plateSetObj.SetActive(value: false);
			_frameImage.gameObject.SetActive(value: false);
			_mask.enabled = false;
		}
		_animator.Play("Idle");
	}

	public void BlankSet()
	{
		ChangeBase(CardType.Blank);
		_plateSetObj.SetActive(value: false);
		_frameImage.gameObject.SetActive(value: false);
		_mainImage.gameObject.SetActive(value: false);
	}

	public void Play()
	{
		_animator.Play("In");
	}

	private void ChangeBase(CardType cardType)
	{
		_baseImage.ChangeSprite((int)cardType);
	}
}
