using System.Collections.Generic;
using UnityEngine;

public class CourseTabController : CourseControllerBase
{
	[SerializeField]
	private CourseModeCard _miniCardOrigin;

	[SerializeField]
	private CourseModeCard _main;

	[SerializeField]
	private GameObject _daniBG;

	[SerializeField]
	private GameObject _sinDaniBG;

	[SerializeField]
	private GameObject _randomBG;

	private CourseModeCard[] _leftPanels;

	private CourseModeCard[] _rightPanels;

	private List<CourseModeCardData> _cardDatas = new List<CourseModeCardData>();

	private void Awake()
	{
	}

	public new void Initialize(int monitorIndex)
	{
		_monitorIndex = monitorIndex;
		_leftPanels = new CourseModeCard[_leftTransforms.Length];
		_rightPanels = new CourseModeCard[_rightTransforms.Length];
		for (int i = 0; i < _leftTransforms.Length; i++)
		{
			_leftPanels[i] = Object.Instantiate(_miniCardOrigin, _leftTransforms[i]).GetComponent<CourseModeCard>();
		}
		for (int j = 0; j < _rightTransforms.Length; j++)
		{
			_rightPanels[j] = Object.Instantiate(_miniCardOrigin, _rightTransforms[j]).GetComponent<CourseModeCard>();
		}
		_tabButtons = new TabButton[_tabButtonTrans.Length];
		for (int k = 0; k < _tabButtons.Length; k++)
		{
			_tabButtons[k] = Object.Instantiate(_tabButtonObj, _tabButtonTrans[k]).GetComponent<TabButton>();
			_tabButtons[k].Initialize(_monitorIndex);
		}
		_tabButtons[0].UseRightArrow();
		_tabButtons[1].UseLeftArrow();
	}

	public void PrepareMainPanel(CourseModeCardData cardData)
	{
		_main.Prepare(cardData);
	}

	public void SetData(List<CourseModeCardData> datas)
	{
		_cardDatas = datas;
		_isAnimation = true;
	}

	public void UpdateTab(int currentCategoryId)
	{
		if (!_isAnimation)
		{
			return;
		}
		int num = currentCategoryId;
		int num2 = _leftPanels.Length - 1;
		while (-1 < num2)
		{
			if (num < 0)
			{
				num = _cardDatas.Count - 1;
			}
			if (0 <= num)
			{
				_leftPanels[num2].Prepare(_cardDatas[num]);
				num--;
			}
			num2--;
		}
		num = currentCategoryId;
		for (int i = 0; i < _rightPanels.Length; i++)
		{
			if (_cardDatas.Count <= num)
			{
				num = 0;
			}
			if (num < _cardDatas.Count)
			{
				_rightPanels[i].Prepare(_cardDatas[num]);
				num++;
			}
		}
		CourseModeCardData courseModeCardData = _cardDatas[currentCategoryId];
		_main.Prepare(courseModeCardData);
		SetCourseBG(courseModeCardData._courseMode);
	}

	public void SetVisibleTab(bool isActive)
	{
		for (int i = 0; i < _leftPanels.Length; i++)
		{
			_leftPanels[i].gameObject.SetActive(isActive);
			_rightPanels[i].gameObject.SetActive(isActive);
		}
		_main.gameObject.SetActive(isActive);
		if (!isActive)
		{
			_daniBG?.gameObject.SetActive(isActive);
			_sinDaniBG?.gameObject.SetActive(isActive);
			_randomBG?.gameObject.SetActive(isActive);
		}
	}

	public new void SetVisibleParts(bool isActive)
	{
		SetVisibleTab(isActive);
		for (int i = 0; i < _tabButtons.Length; i++)
		{
			_tabButtons[i].SetActiveButton(isActive);
		}
	}

	public void SetCourseBG(int modeId)
	{
		switch (modeId)
		{
		case 1:
			_daniBG?.gameObject.SetActive(value: true);
			_sinDaniBG?.gameObject.SetActive(value: false);
			_randomBG?.gameObject.SetActive(value: false);
			break;
		case 2:
			_daniBG?.gameObject.SetActive(value: false);
			_sinDaniBG?.gameObject.SetActive(value: true);
			_randomBG?.gameObject.SetActive(value: false);
			break;
		case 3:
			_daniBG?.gameObject.SetActive(value: false);
			_sinDaniBG?.gameObject.SetActive(value: false);
			_randomBG?.gameObject.SetActive(value: true);
			break;
		}
	}
}
