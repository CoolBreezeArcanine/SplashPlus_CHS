using System.Collections.Generic;
using UI.DaisyChainList;
using UnityEngine;

public class TabController : MonoBehaviour
{
	[SerializeField]
	private GameObject _tabObj;

	[SerializeField]
	private Transform _tabTransform;

	private SelectorTab _tab;

	private void Awake()
	{
		_tab = Object.Instantiate(_tabObj, _tabTransform).GetComponent<SelectorTab>();
	}

	public virtual void Initialize(int monitorId)
	{
		_tab.Initialize(monitorId);
	}

	public void Set(List<TabDataBase> tabDatas, int categoryIndex)
	{
		_tab.SetVisibleParts(isActive: true);
		_tab.SetData(tabDatas);
		_tab.UpdateTab(categoryIndex);
		_tab.PlayInAnimation();
	}

	public void SetOnlyMainPanel(TabDataBase data)
	{
		_tab.PrepareMainPanel(data);
		_tab.PlayChangeAnimation();
		_tab.SetVisibleParts(isActive: false);
	}

	public void SetActiveMiniPanels(int categoryIndex)
	{
		_tab.SetVisibleParts(isActive: true);
		_tab.UpdateTab(categoryIndex);
		_tab.PlayInAnimation();
	}

	public void Change(int currentCategoryId, Direction direction)
	{
		_tab.UpdateTab(currentCategoryId);
		switch (direction)
		{
		case Direction.Right:
			_tab.PlayMoveRightAnimation();
			break;
		case Direction.Left:
			_tab.PlayMoveLeftAnimation();
			break;
		}
	}

	public void Change(int currentCategoryId)
	{
		_tab.UpdateTab(currentCategoryId);
	}

	public void UpdateButtonAnimation()
	{
		_tab.UpdateButtonView();
	}

	public void PressedTabButton(bool isRight)
	{
		_tab.PressedTabButton(isRight);
	}

	public void PlayInAnimation()
	{
		_tab.PlayInAnimation();
	}

	public void SetVisibleMiniTab(bool isActive)
	{
		_tab.SetVisibleMiniTab(isActive);
	}

	public void PlayOutAnimation()
	{
		_tab.PlayOutAnimation();
	}

	public void SetLeftIcon(Sprite sprite)
	{
		_tab.SetLeftIcon(sprite);
	}

	public void SetRightIcon(Sprite sprite)
	{
		_tab.SetRightIcon(sprite);
	}
}
