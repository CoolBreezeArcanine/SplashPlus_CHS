using System.Collections.Generic;
using DB;
using UnityEngine;

public class MessageController : MonoBehaviour, IMessageController
{
	public struct MessageInfo
	{
		public WindowKindID KindId;

		public WindowSizeID SizeId;

		public WindowPositionID PositionId;

		public Vector3 Position;

		public string Title;

		public string Message;

		public bool IsWarning;
	}

	[SerializeField]
	private CustomWindow _origin;

	[SerializeField]
	private CustomWindow _largeV;

	[SerializeField]
	private CustomWindow _largeH;

	[SerializeField]
	private Transform _windowRoot;

	[SerializeField]
	private Transform _upper;

	[SerializeField]
	private Transform _middle;

	[SerializeField]
	private Transform _lower;

	[SerializeField]
	private WarningWindow _warningWindow;

	[SerializeField]
	private Sprite _attentionSp;

	[SerializeField]
	private Sprite _attentionTitleSp;

	[SerializeField]
	private Sprite _defaultSp;

	[SerializeField]
	private Sprite _defaultTitleSp;

	private List<CustomWindow> _windowList = new List<CustomWindow>();

	public Sprite AttentionSp()
	{
		return _attentionSp;
	}

	public Sprite AttentionTitleSp()
	{
		return _attentionTitleSp;
	}

	public Sprite DefaultSp()
	{
		return _defaultSp;
	}

	public Sprite DefaultTitleSp()
	{
		return _defaultTitleSp;
	}

	private void Awake()
	{
		_origin.SetVisible(isActive: false);
		_largeH.SetVisible(isActive: false);
		_largeV.SetVisible(isActive: false);
		_warningWindow.SetVisible(isActive: false);
	}

	public void SetMessageWindow(MessageInfo info)
	{
		for (int i = 0; i < _windowList.Count; i++)
		{
			if (_windowList[i].IsOpening)
			{
				_windowList[i].ForcedClose();
			}
		}
		if (_warningWindow.IsOpening)
		{
			_warningWindow.ForcedClose();
		}
		if (info.IsWarning)
		{
			WarningWindowInfo info2 = new WarningWindowInfo(info.Title, info.Message, -1f, 0);
			_warningWindow.Prepare(info2, Vector3.zero);
			return;
		}
		WindowPositionID positionId = info.PositionId;
		WindowSizeID sizeId = info.SizeId;
		Vector3 vector = (info.Position = GetPosition(sizeId, positionId));
		switch (sizeId)
		{
		case WindowSizeID.LargeVertical:
			_largeV.Prepare(info, this);
			_windowList.Add(_largeV);
			break;
		case WindowSizeID.LargeHorizontal:
			_largeH.Prepare(info, this);
			_windowList.Add(_largeH);
			break;
		case WindowSizeID.Large:
		case WindowSizeID.Middle:
		case WindowSizeID.Small:
			SetSmallWindow(info);
			break;
		}
	}

	private Vector3 GetPosition(WindowSizeID sizeId, WindowPositionID positionId)
	{
		return positionId switch
		{
			WindowPositionID.Middle => _middle.localPosition, 
			WindowPositionID.Upper => _upper.localPosition, 
			WindowPositionID.Lower => _lower.localPosition, 
			_ => Vector3.zero, 
		};
	}

	private void SetSmallWindow(MessageInfo info)
	{
		_origin.Prepare(info, this);
		_windowList.Add(_origin);
	}

	public void SetWarningWindow(WarningWindowInfo info, Vector3 position)
	{
		_warningWindow.Prepare(info, position);
	}

	public void ViewUpdate()
	{
		long num = (long)(Time.deltaTime * 1000f);
		UpdateWarningWindow(num);
		if (0 < _windowList.Count)
		{
			RemoveWindowFromList();
			for (int i = 0; i < _windowList.Count; i++)
			{
				_windowList[i].UpdateView(num);
			}
		}
		_warningWindow.UpdateView(num);
	}

	private void RemoveWindowFromList()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < _windowList.Count; i++)
		{
			if (!_windowList[i].IsOpening)
			{
				list.Add(i);
			}
		}
		int num = 0;
		for (int j = 0; j < list.Count; j++)
		{
			_windowList.RemoveAt(list[j] - num);
			num++;
		}
	}

	private void UpdateWarningWindow(long gameMSecAdd)
	{
		_warningWindow.UpdateView(gameMSecAdd);
	}
}
