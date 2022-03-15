using System.Collections.Generic;
using UnityEngine;

public class CourseCardSelectController : CourseControllerBase
{
	[SerializeField]
	private Transform _mainCardTransform;

	[SerializeField]
	private Transform _mainModeCardTransform;

	[SerializeField]
	private GameObject _miniSelectCardOrigin;

	[SerializeField]
	private GameObject _miniModeSelectCardOrigin;

	[SerializeField]
	private GameObject _mainSelectCardOrigin;

	[SerializeField]
	private GameObject _mainModeSelectCardOrigin;

	[SerializeField]
	private GameObject _separatorOrigin;

	private CourseCard _mainSelectCard;

	private CourseModeCard _mainModeSelectCard;

	private CourseCard[] _leftSelectCard;

	private CourseCard[] _rightSelectCard;

	private CourseModeCard[] _leftModeSelectCard;

	private CourseModeCard[] _rightModeSelectCard;

	private CourseSeparator[] _leftSeparator;

	private CourseSeparator[] _rightSeparator;

	private List<CourseCardData> _cardDatas = new List<CourseCardData>();

	private List<CourseModeCardData> _modeCardDatas = new List<CourseModeCardData>();

	private void Awake()
	{
		_leftSelectCard = new CourseCard[_leftTransforms.Length];
		_rightSelectCard = new CourseCard[_rightTransforms.Length];
		_leftModeSelectCard = new CourseModeCard[_leftTransforms.Length];
		_rightModeSelectCard = new CourseModeCard[_rightTransforms.Length];
		_leftSeparator = new CourseSeparator[_leftTransforms.Length];
		_rightSeparator = new CourseSeparator[_leftTransforms.Length];
		for (int i = 0; i < _leftTransforms.Length; i++)
		{
			_leftSelectCard[i] = Object.Instantiate(_miniSelectCardOrigin, _leftTransforms[i]).GetComponent<CourseCard>();
			_leftSelectCard[i].Initialize();
			_leftModeSelectCard[i] = Object.Instantiate(_miniModeSelectCardOrigin, _leftTransforms[i]).GetComponent<CourseModeCard>();
			_leftSeparator[i] = Object.Instantiate(_separatorOrigin, _leftTransforms[i]).GetComponent<CourseSeparator>();
		}
		for (int j = 0; j < _rightTransforms.Length; j++)
		{
			_rightSelectCard[j] = Object.Instantiate(_miniSelectCardOrigin, _rightTransforms[j]).GetComponent<CourseCard>();
			_rightSelectCard[j].Initialize();
			_rightModeSelectCard[j] = Object.Instantiate(_miniModeSelectCardOrigin, _rightTransforms[j]).GetComponent<CourseModeCard>();
			_rightSeparator[j] = Object.Instantiate(_separatorOrigin, _rightTransforms[j]).GetComponent<CourseSeparator>();
		}
		_mainSelectCard = Object.Instantiate(_mainSelectCardOrigin, _mainCardTransform).GetComponent<CourseCard>();
		_mainSelectCard.Initialize();
		_mainModeSelectCard = Object.Instantiate(_mainModeSelectCardOrigin, _mainModeCardTransform).GetComponent<CourseModeCard>();
		_tabButtons = new TabButton[_tabButtonTrans.Length];
		for (int k = 0; k < _tabButtons.Length; k++)
		{
			_tabButtons[k] = Object.Instantiate(_tabButtonObj, _tabButtonTrans[k]).GetComponent<TabButton>();
		}
	}

	public void PrepareModeSelectMainPanel(CourseModeCardData modeCardData)
	{
		_mainModeSelectCard.Prepare(modeCardData);
	}

	public void SetModeSelectData(List<CourseModeCardData> modeCardDatas)
	{
		_modeCardDatas = modeCardDatas;
		_isAnimation = true;
	}

	public void SetSelectData(List<CourseCardData> carDatas)
	{
		_cardDatas = carDatas;
		_isAnimation = true;
	}

	public void UpdateSelectCard(int currentCategoryId, int currentCardId)
	{
		if (!_isAnimation)
		{
			return;
		}
		int num = currentCardId;
		int num2 = currentCategoryId;
		int num3 = _leftSelectCard.Length - 1;
		while (-1 < num3)
		{
			if (num < 0)
			{
				_leftSelectCard[num3].gameObject.SetActive(value: false);
				if (num == -1)
				{
					num2 = (num2 + _modeCardDatas.Count - 1) % _modeCardDatas.Count;
					_leftSeparator[num3].SetShow(_modeCardDatas[num2]._courseMode, isRight: false);
					_leftSeparator[num3].gameObject.SetActive(value: true);
				}
				else
				{
					_leftSeparator[num3].gameObject.SetActive(value: false);
				}
			}
			if (0 <= num)
			{
				_leftSelectCard[num3].gameObject.SetActive(value: true);
				_leftSelectCard[num3].Prepare(_cardDatas[num], isMain: false);
				_leftSelectCard[num3].SetPlayAnim("Loop");
				num--;
				_leftSeparator[num3].gameObject.SetActive(value: false);
			}
			num3--;
		}
		num = currentCardId;
		num2 = currentCategoryId;
		for (int i = 0; i < _rightSelectCard.Length; i++)
		{
			if (_cardDatas.Count <= num)
			{
				_rightSelectCard[i].gameObject.SetActive(value: false);
				if (_cardDatas.Count == num)
				{
					num2 = (num2 + 1) % _modeCardDatas.Count;
					_rightSeparator[i].SetShow(_modeCardDatas[num2]._courseMode, isRight: true);
					_rightSeparator[i].gameObject.SetActive(value: true);
				}
				else
				{
					_rightSeparator[i].gameObject.SetActive(value: false);
				}
			}
			if (num < _cardDatas.Count)
			{
				_rightSelectCard[i].gameObject.SetActive(value: true);
				_rightSelectCard[i].Prepare(_cardDatas[num], isMain: false);
				_rightSelectCard[i].SetPlayAnim("Loop");
				num++;
				_rightSeparator[i].gameObject.SetActive(value: false);
			}
		}
		CourseCardData data = _cardDatas[currentCardId];
		_mainSelectCard.gameObject.SetActive(value: true);
		_mainSelectCard.Prepare(data, isMain: true);
		_mainSelectCard.SetPlayAnim("Loop");
	}

	public void UpdateModeSelectCard(int currentCategoryId)
	{
		if (!_isAnimation)
		{
			return;
		}
		int num = currentCategoryId;
		int num2 = _leftModeSelectCard.Length - 1;
		while (-1 < num2)
		{
			if (num < 0)
			{
				_leftModeSelectCard[num2].gameObject.SetActive(value: false);
			}
			if (0 <= num)
			{
				_leftModeSelectCard[num2].gameObject.SetActive(value: true);
				_leftModeSelectCard[num2].Prepare(_modeCardDatas[num]);
				_leftModeSelectCard[num2].SetPlayAnim("Loop");
				num--;
			}
			num2--;
		}
		num = currentCategoryId;
		for (int i = 0; i < _rightModeSelectCard.Length; i++)
		{
			if (_modeCardDatas.Count <= num)
			{
				_rightModeSelectCard[i].gameObject.SetActive(value: false);
			}
			if (num < _modeCardDatas.Count)
			{
				_rightModeSelectCard[i].gameObject.SetActive(value: true);
				_rightModeSelectCard[i].Prepare(_modeCardDatas[num]);
				_rightModeSelectCard[i].SetPlayAnim("Loop");
				num++;
			}
		}
		CourseModeCardData data = _modeCardDatas[currentCategoryId];
		_mainModeSelectCard.gameObject.SetActive(value: true);
		_mainModeSelectCard.Prepare(data);
		_mainModeSelectCard.SetPlayAnim("Loop");
	}

	public void SetHideSelectCard()
	{
		for (int i = 0; i < _leftSelectCard.Length; i++)
		{
			_leftSelectCard[i].gameObject.SetActive(value: false);
			_leftSeparator[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < _rightSelectCard.Length; j++)
		{
			_rightSelectCard[j].gameObject.SetActive(value: false);
			_rightSeparator[j].gameObject.SetActive(value: false);
		}
		_mainSelectCard.gameObject.SetActive(value: false);
	}

	public void SetHideModeSelectCard()
	{
		for (int i = 0; i < _leftModeSelectCard.Length; i++)
		{
			_leftModeSelectCard[i].gameObject.SetActive(value: false);
		}
		for (int j = 0; j < _rightModeSelectCard.Length; j++)
		{
			_rightModeSelectCard[j].gameObject.SetActive(value: false);
		}
		_mainModeSelectCard.gameObject.SetActive(value: false);
	}

	public void SetVisiblSelecteParts(bool isActive)
	{
		for (int i = 0; i < _tabButtons.Length; i++)
		{
			_tabButtons[i].SetActiveButton(isActive);
		}
	}

	public void SetVisibleModeSelectParts(bool isActive)
	{
		for (int i = 0; i < _tabButtons.Length; i++)
		{
			_tabButtons[i].SetActiveButton(isActive);
		}
	}
}
