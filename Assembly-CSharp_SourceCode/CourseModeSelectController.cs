using System.Collections.Generic;
using UnityEngine;

public class CourseModeSelectController : CourseControllerBase
{
	[SerializeField]
	private CourseModeCard _miniCardOrigin;

	[SerializeField]
	private CourseModeCard _main;

	private CourseModeCard[] _leftPanels;

	private CourseModeCard[] _rightPanels;

	private List<CourseModeCardData> _cardDatas = new List<CourseModeCardData>();

	private void Awake()
	{
		_leftPanels = new CourseModeCard[_leftTransforms.Length];
		_rightPanels = new CourseModeCard[_rightTransforms.Length];
		for (int i = 0; i < _leftTransforms.Length; i++)
		{
			_leftPanels[i] = Object.Instantiate(_miniCardOrigin, _leftTransforms[i]).GetComponent<CourseModeCard>();
			_rightPanels[i] = Object.Instantiate(_miniCardOrigin, _rightTransforms[i]).GetComponent<CourseModeCard>();
		}
		_tabButtons = new TabButton[_tabButtonTrans.Length];
		for (int j = 0; j < _tabButtons.Length; j++)
		{
			_tabButtons[j] = Object.Instantiate(_tabButtonObj, _tabButtonTrans[j]).GetComponent<TabButton>();
		}
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
		int num = currentCategoryId - 1;
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
		num = currentCategoryId + 1;
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
		CourseModeCardData data = _cardDatas[currentCategoryId];
		_main.Prepare(data);
	}

	public void SetVisibleMiniTab(bool isActive)
	{
		for (int i = 0; i < _leftPanels.Length; i++)
		{
			_leftPanels[i].gameObject.SetActive(isActive);
			_rightPanels[i].gameObject.SetActive(isActive);
		}
	}

	public new void SetVisibleParts(bool isActive)
	{
		SetVisibleMiniTab(isActive);
		for (int i = 0; i < _tabButtons.Length; i++)
		{
			_tabButtons[i].SetActiveButton(isActive);
		}
	}
}
