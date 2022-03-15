using UnityEngine;
using UnityEngine.UI;

public class CommonCharaObject : MonoBehaviour
{
	public enum CharaType
	{
		Red,
		Blue,
		Max
	}

	public enum MouthType
	{
		Open,
		Close,
		Max
	}

	[SerializeField]
	private Image _noseObj;

	[SerializeField]
	private Image _bodyObj;

	[SerializeField]
	private Image _mouthObj;

	[SerializeField]
	private Sprite[] _noseImage = new Sprite[2];

	[SerializeField]
	private Sprite[] _bodyImage = new Sprite[2];

	[SerializeField]
	private Sprite[] _mouthImage = new Sprite[2];

	private CharaType _charaType;

	private MouthType _mouth;

	private Animator _charaAnimator;

	private void Awake()
	{
		_charaAnimator = GetComponent<Animator>();
	}

	public void Initialize(CharaType chara)
	{
		_charaType = chara;
		UpdateCharaType();
	}

	public void SetFace(MouthType mouth)
	{
		_mouth = mouth;
		UpdateChara();
	}

	public void UpdateCharaType()
	{
		_noseObj.sprite = _noseImage[(int)_charaType];
		_bodyObj.sprite = _bodyImage[(int)_charaType];
	}

	public void UpdateChara()
	{
		_mouthObj.sprite = _mouthImage[(int)_mouth];
	}

	public void PlayIdle()
	{
		if (base.gameObject.activeInHierarchy)
		{
			_charaAnimator.SetTrigger("Pressed");
		}
	}
}
