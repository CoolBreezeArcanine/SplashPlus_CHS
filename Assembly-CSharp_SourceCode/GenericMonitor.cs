using System.Collections.Generic;
using DB;
using Manager;
using Process;
using UnityEngine;

public class GenericMonitor : MonitorBase, IMessageMonitor
{
	private const int MAX_WINDOW_NUM = 3;

	[SerializeField]
	[Header("各種Windowの元")]
	private CommonWindow _small;

	[SerializeField]
	private CommonWindow _large_V;

	[SerializeField]
	private CommonWindow _large_H;

	[SerializeField]
	private Transform _windowRoot;

	[SerializeField]
	[Header("座標")]
	private Transform UpperPos;

	[SerializeField]
	private Transform MiddlePos;

	[SerializeField]
	private Transform LowerPos;

	private CommonWindow[] _smalls;

	[SerializeField]
	private WarningWindow _warningWindow;

	[SerializeField]
	[Header("Attention")]
	private Sprite _attentionWindow;

	[SerializeField]
	private Sprite _attentionTitle;

	[SerializeField]
	[Header("Default(Normal)")]
	private Sprite _defaultWindow;

	[SerializeField]
	private Sprite _defaultTitle;

	[SerializeField]
	[Header("タイマー")]
	private GameObject _timerParent;

	private CommonTimer _timer;

	private WindowSizeID _currentSelectKind;

	private int _monitorId;

	private List<CommonWindow> _windowList;

	public Sprite AttentionWindow => _attentionWindow;

	public Sprite AttentionTitle => _attentionTitle;

	public Sprite DefaultWindow => _defaultWindow;

	public Sprite DefaultTitle => _defaultTitle;

	public int MonitorId => _monitorId;

	public override void Initialize(int monIndex, bool active)
	{
		base.Initialize(monIndex, active);
		_monitorId = monIndex;
		PrepareWindow();
		_warningWindow.SetVisible(isActive: false);
	}

	private void PrepareWindow()
	{
		_smalls = new CommonWindow[3];
		for (int i = 0; i < 3; i++)
		{
			_smalls[i] = Object.Instantiate(_small, _windowRoot);
			_smalls[i].SetVisible(isActive: false);
		}
		_large_H.SetVisible(isActive: false);
		_large_V.SetVisible(isActive: false);
		_windowList = new List<CommonWindow>();
	}

	public void SetMessageWindow(GenericProcess.WindowInfo info)
	{
		WindowMessageID messageId = info.MessageId;
		WindowPositionID positionId = info.PositionId;
		WindowSizeID size = messageId.GetSize();
		Vector3 position = GetPosition(size, positionId);
		switch (size)
		{
		case WindowSizeID.LargeVertical:
			_large_V.Prepare(this, messageId, positionId, position);
			_windowList.Add(_large_V);
			break;
		case WindowSizeID.LargeHorizontal:
			_large_H.Prepare(this, messageId, positionId, position);
			_windowList.Add(_large_H);
			break;
		case WindowSizeID.Large:
		case WindowSizeID.Middle:
		case WindowSizeID.Small:
			SetSmallWindow(messageId, positionId, position);
			break;
		}
	}

	private Vector3 GetPosition(WindowSizeID sizeId, WindowPositionID positionId)
	{
		return positionId switch
		{
			WindowPositionID.Middle => MiddlePos.localPosition, 
			WindowPositionID.Upper => UpperPos.localPosition, 
			WindowPositionID.Lower => LowerPos.localPosition, 
			_ => Vector3.zero, 
		};
	}

	private void SetSmallWindow(WindowMessageID recordId, WindowPositionID positionId, Vector3 position)
	{
		for (int i = 0; i < 3; i++)
		{
			if (!_smalls[i].IsOpening)
			{
				_smalls[i].Prepare(this, recordId, positionId, position);
				_windowList.Add(_smalls[i]);
				break;
			}
		}
	}

	public void SetWarningWindow(WarningWindowInfo info, Vector3 position)
	{
		_warningWindow.Prepare(info, position);
	}

	public void AllInterrupt()
	{
		for (int i = 0; i < _windowList.Count; i++)
		{
			_windowList[i].Interrupt();
		}
		_windowList.Clear();
	}

	public void Interrupt(WindowPositionID positionId)
	{
		for (int i = 0; i < _windowList.Count; i++)
		{
			if (_windowList[i].PositionID == positionId)
			{
				_windowList[i].Interrupt();
				break;
			}
		}
	}

	public override void ViewUpdate()
	{
		UpdateWarningWindow(GameManager.GetGameMSecAdd());
		if (0 < _windowList.Count)
		{
			RemoveWindowFromList();
			for (int i = 0; i < _windowList.Count; i++)
			{
				_windowList[i].UpdateView(GameManager.GetGameMSecAdd());
			}
		}
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

	public void AllClose()
	{
		for (int i = 0; i < _windowList.Count; i++)
		{
			_windowList[i].Close();
		}
	}

	public void ForcedAllClose()
	{
		for (int i = 0; i < _windowList.Count; i++)
		{
			_windowList[i].ForcedClose();
		}
	}

	public void Close(WindowPositionID positionId)
	{
		for (int i = 0; i < _windowList.Count; i++)
		{
			if (_windowList[i].PositionID == positionId)
			{
				_windowList[i].Close();
				break;
			}
		}
	}

	public bool IsOpening(WindowPositionID positionId)
	{
		bool result = false;
		for (int i = 0; i < _windowList.Count; i++)
		{
			if (_windowList[i].PositionID == positionId)
			{
				result = _windowList[i].IsOpening;
				break;
			}
		}
		return result;
	}

	public void WarningInterrupt()
	{
		_warningWindow.Interrupt();
	}

	public void CloseWarnigWindow()
	{
		_warningWindow.Close();
	}

	public bool IsOpeningWarningWindow()
	{
		return _warningWindow.IsOpening;
	}

	private void UpdateWarningWindow(long gameMSecAdd)
	{
		_warningWindow.UpdateView(gameMSecAdd);
	}

	public void SetVisibleTimer(bool isActive)
	{
		if (_timer != null)
		{
			_timer.SetVisible(isActive);
		}
	}

	public void CreateTimer()
	{
		_timer = Object.Instantiate(CommonPrefab.GetCommonTimerPrefab(), _timerParent.transform);
		_timer.MonitorIndex = base.MonitorIndex;
		SetVisibleTimer(isActive: false);
	}

	public void SetTimerSecond(uint second, bool isInfinity = false)
	{
		if (_timer != null)
		{
			_timer.SetTimerCount(second, isInfinity);
		}
	}
}
