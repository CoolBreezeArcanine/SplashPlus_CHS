using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionalList : MonoBehaviour
{
	private enum ScrollState
	{
		None,
		Lineup,
		InitLayout,
		InitPosition,
		Update
	}

	[SerializeField]
	private AnimationCurve _curve;

	private const float _scrollDuration = 0.2f;

	[SerializeField]
	private Transform _root;

	private AssetManager _assetManager;

	private List<RegionalListNode> _nodeList;

	private HorizontalLayoutGroup _layoutGroup;

	private ContentSizeFitter _contentSize;

	private Vector3 _fromPos;

	private Vector3 _toPos;

	private float _timer;

	private int _scrollTargetIndex;

	private int _prevID = -1;

	private ScrollState _state;

	public void Initialize(int monitorIndex, List<UserMapData> mapDataList, AssetManager assetManager)
	{
		_layoutGroup = _root.GetComponent<HorizontalLayoutGroup>();
		_contentSize = _root.GetComponent<ContentSizeFitter>();
		_assetManager = assetManager;
		_nodeList = new List<RegionalListNode>();
		GenerateSeparator("Left");
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		foreach (UserMapData mapData in mapDataList)
		{
			if (mapData.IsEvent)
			{
				flag = true;
			}
			else if (!mapData.IsDeluxe)
			{
				if (flag && !flag3)
				{
					GenerateSeparator("Event2Normal");
					flag3 = true;
				}
				flag2 = true;
			}
			else if (flag2 && !flag4)
			{
				flag4 = true;
			}
			GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Process/RegionSelect/Prefabs/UI_ListNode"), _root);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;
			obj.name = obj.name.Replace("(Clone)", "_" + mapData.Name);
			RegionalListNode component = obj.GetComponent<RegionalListNode>();
			RegionalListNode.RegionNodeState regionNodeState = (mapData.IsLock ? RegionalListNode.RegionNodeState.Lock : (mapData.IsComplete ? RegionalListNode.RegionNodeState.Complete : RegionalListNode.RegionNodeState.Open));
			string text = ((regionNodeState == RegionalListNode.RegionNodeState.Lock) ? "Lock" : "Open");
			Sprite baseSprite = Resources.Load<Sprite>("Process/RegionSelect/Sprites/mapselect/UI_CHS_SelectorBase_" + text + "_S");
			text = ((regionNodeState == RegionalListNode.RegionNodeState.Lock) ? "UI_Island_Icon_Lock" : "UI_Island_Icon");
			Sprite mapBgSprite = _assetManager.GetMapBgSprite(mapData.ID, text);
			component.Initialize();
			component.SetData(regionNodeState, (regionNodeState == RegionalListNode.RegionNodeState.Lock) ? "???" : mapData.Name, baseSprite, mapBgSprite);
			_nodeList.Add(component);
		}
		GenerateSeparator("Right");
		_scrollTargetIndex = 0;
		_state = ScrollState.None;
		_layoutGroup.SetLayoutHorizontal();
		_layoutGroup.CalculateLayoutInputHorizontal();
		StartCoroutine(Init());
	}

	private IEnumerator Init()
	{
		yield return new WaitForEndOfFrame();
		_contentSize.enabled = false;
		_layoutGroup.enabled = false;
	}

	public int GetListCount()
	{
		return _nodeList.Count;
	}

	public void ViewUpdate(float deltaTime)
	{
		if (_nodeList == null)
		{
			return;
		}
		foreach (RegionalListNode node in _nodeList)
		{
			node.ViewUpdate();
		}
		switch (_state)
		{
		case ScrollState.Lineup:
			_state = ScrollState.InitLayout;
			break;
		case ScrollState.InitLayout:
			ScrollInitLayout();
			_state = ScrollState.InitPosition;
			break;
		case ScrollState.InitPosition:
			ScrollInitPosition();
			_state = ScrollState.Update;
			break;
		case ScrollState.Update:
			if (_timer > 0f)
			{
				float t = _curve.Evaluate((0.2f - _timer) / 0.2f);
				_root.localPosition = Vector2.Lerp(_fromPos, _toPos, t);
				_timer -= deltaTime;
			}
			else
			{
				float t2 = _curve.Evaluate(1f);
				_root.localPosition = Vector2.Lerp(_fromPos, _toPos, t2);
				_state = ScrollState.None;
			}
			break;
		}
	}

	public void ScrollExecute(int targetID)
	{
		_scrollTargetIndex = targetID;
		_state = ScrollState.InitLayout;
	}

	private void ScrollInitLayout()
	{
		_nodeList[_scrollTargetIndex].SetNodeType(isMain: true);
		if (_prevID >= 0 && _scrollTargetIndex != _prevID)
		{
			_nodeList[_prevID].SetNodeType(isMain: false);
		}
		_prevID = _scrollTargetIndex;
	}

	private void ScrollInitPosition()
	{
		_fromPos = _root.localPosition;
		_toPos = _nodeList[_scrollTargetIndex].transform.localPosition;
		_toPos = new Vector3(_toPos.x * -1f, _toPos.y, _toPos.z);
		_timer = 0.2f;
	}

	private void GenerateSeparator(string separateName = "")
	{
		GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Process/RegionSelect/Prefabs/UI_ListNode"), _root);
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.identity;
		obj.name = obj.name.Replace("(Clone)", "_" + separateName + "Separator");
		RegionalListNode component = obj.GetComponent<RegionalListNode>();
		component.Initialize();
		component.InitializeSeparator();
	}
}
