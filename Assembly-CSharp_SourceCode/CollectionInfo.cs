using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class CollectionInfo
{
	private List<ReadOnlyCollection<int>> _collectionIdList;

	private ReadOnlyCollection<CollectionData> _collectionData;

	private int _index;

	private int _categoryIndex;

	private List<CollectionTabData> _tabData = new List<CollectionTabData>();

	private ICollectionProcess _process;

	private ICollectionMonitor _monitor;

	public int Index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = value;
		}
	}

	public int CategoryIndex
	{
		get
		{
			return _categoryIndex;
		}
		set
		{
			_categoryIndex = value;
		}
	}

	public List<CollectionTabData> TabData
	{
		get
		{
			return _tabData;
		}
		set
		{
			_tabData = value;
		}
	}

	private CollectionData GetData(int category, int index)
	{
		int targetId = _collectionIdList[category][index];
		return GetData(targetId);
	}

	private CollectionData GetData(int targetId)
	{
		return _collectionData.FirstOrDefault((CollectionData c) => c.GetID() == targetId);
	}

	public string CategoryName(int categoryIndex)
	{
		return _tabData[categoryIndex].Title;
	}

	public void Initialize(List<ReadOnlyCollection<int>> collectionIdList, List<CollectionData> data, List<CollectionTabData> tabDatas, ICollectionProcess process, ICollectionMonitor monitor, bool newGet, int setItemId)
	{
		_index = 0;
		_categoryIndex = 0;
		_collectionIdList = new List<ReadOnlyCollection<int>>();
		_process = process;
		_monitor = monitor;
		_tabData = tabDatas;
		_collectionIdList = collectionIdList;
		_collectionData = data.AsReadOnly();
		if (newGet)
		{
			return;
		}
		for (int i = 0; i < _collectionIdList.Count; i++)
		{
			for (int j = 0; j < _collectionIdList[i].Count; j++)
			{
				if (_collectionIdList[i][j] == setItemId)
				{
					_categoryIndex = i;
					_index = j;
					break;
				}
			}
		}
	}

	public void RefreshIndex()
	{
	}

	private void ScrollRight()
	{
		if (_index + 1 < GetIndexNum())
		{
			_index++;
			return;
		}
		_categoryIndex = ((_categoryIndex + 1 < GetCategoryNum()) ? (_categoryIndex + 1) : (_categoryIndex = 0));
		_index = 0;
	}

	private void ScrollLeft()
	{
		if (0 <= _index - 1)
		{
			_index--;
			return;
		}
		_categoryIndex = ((_categoryIndex - 1 < 0) ? (GetCategoryNum() - 1) : (_categoryIndex - 1));
		_index = GetIndexNum() - 1;
	}

	private void ChangeCategoryRight()
	{
		if (_categoryIndex + 1 < GetCategoryNum())
		{
			_categoryIndex++;
		}
		else
		{
			_categoryIndex = 0;
		}
		_index = 0;
	}

	private void ChangeCategoryLeft()
	{
		if (_categoryIndex - 1 < 0)
		{
			_categoryIndex = GetCategoryNum() - 1;
		}
		else
		{
			_categoryIndex--;
		}
		_index = GetIndexNum() - 1;
	}

	public int GetAllCollectionNum()
	{
		return _collectionData.Count((CollectionData c) => c.IsHave);
	}

	private int GetCategoryNum()
	{
		return _collectionIdList.Count;
	}

	public int GetCurrentIndex()
	{
		int num = 0;
		for (int i = 0; i < _categoryIndex; i++)
		{
			num += _collectionIdList[i].Count;
		}
		return _index + num;
	}

	public int GetIndexNum()
	{
		return _collectionIdList[CategoryIndex].Count;
	}

	public bool IsHaveNewIcon()
	{
		return _collectionData.Any((CollectionData data) => data.IsNew);
	}

	public CollectionData GetCurrentCenterCollectionData()
	{
		return GetData(CategoryIndex, Index);
	}

	public CollectionData GetCollectionById(int targetId)
	{
		return GetData(targetId);
	}

	public CollectionData GetCollection(int diffIndex)
	{
		int num = _index + diffIndex;
		int num2 = _categoryIndex;
		while (num >= _collectionIdList[num2].Count)
		{
			num -= _collectionIdList[num2].Count;
			num2 = ((num2 + 1 < _collectionIdList.Count) ? (num2 + 1) : 0);
		}
		while (num < 0)
		{
			num2 = ((num2 - 1 >= 0) ? (num2 - 1) : (_collectionIdList.Count - 1));
			num = _collectionIdList[num2].Count + num;
		}
		return GetData(num2, num);
	}

	public bool IsCollectionBoundary(int diffIndex, out int overCount)
	{
		int num = _index + diffIndex;
		int num2 = _categoryIndex;
		overCount = 0;
		if (num >= _collectionIdList[num2].Count)
		{
			while (num >= _collectionIdList[num2].Count)
			{
				num = num - _collectionIdList[num2].Count - 1;
				num2 = ((num2 + 1 < _collectionIdList.Count) ? (num2 + 1) : 0);
				overCount++;
			}
		}
		else if (num < -1)
		{
			while (num < -1)
			{
				overCount--;
				num2 = ((num2 - 1 > 0) ? (num2 - 1) : (_collectionIdList.Count - 1));
				num = _collectionIdList[num2].Count + 1 + num;
			}
		}
		if (num != _collectionIdList[num2].Count)
		{
			return num == -1;
		}
		return true;
	}

	public bool CollectionListLeft(bool isLongTap)
	{
		ScrollRight();
		return _monitor.ScrollCollectionListLeft(isLongTap);
	}

	public bool CollectionListRight(bool isLongTap)
	{
		ScrollLeft();
		return _monitor.ScrollCollectionListRight(isLongTap);
	}

	public void ShiftCategoryRight()
	{
		ChangeCategoryLeft();
		_monitor.ScrollCategoryLeft();
	}

	public void ShiftCategoryLeft()
	{
		ChangeCategoryRight();
		_monitor.ScrollCategoryRight();
	}

	public void CheckSetButton(int equipItemId)
	{
		CollectionData currentCenterCollectionData = GetCurrentCenterCollectionData();
		if ((currentCenterCollectionData != null && !currentCenterCollectionData.IsHave) || equipItemId == currentCenterCollectionData.ID)
		{
			_monitor.SetVisibleButton(false, 2);
		}
		else
		{
			_monitor.SetVisibleButton(true, 2);
		}
	}
}
