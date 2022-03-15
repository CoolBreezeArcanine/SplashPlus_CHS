using System;
using Manager;

public class SlideScrollController
{
	private const float SlideTime = 11.1111107f;

	private Action<int> _leftScroll;

	private Action<int> _rightScroll;

	private int _autoScrollCount;

	private float _timer;

	private int _monitorId;

	private bool _isSlideToRight;

	public void SetAction(int monitorId, Action<int> left, Action<int> right)
	{
		_leftScroll = left;
		_rightScroll = right;
		_monitorId = monitorId;
		Refresh();
	}

	public void StartSlideScroll(bool isToRight, int slideScrollNum)
	{
		_isSlideToRight = isToRight;
		_autoScrollCount = slideScrollNum;
		Invoke(_isSlideToRight);
	}

	public void UpdateAutoScroll()
	{
		if (_autoScrollCount >= 0)
		{
			_timer += GameManager.GetGameMSecAdd();
			if (11.1111107f < _timer)
			{
				Invoke(_isSlideToRight);
			}
		}
	}

	public void Refresh()
	{
		_timer = 0f;
		_autoScrollCount = -1;
	}

	private void Invoke(bool isToRight)
	{
		if (_isSlideToRight)
		{
			_rightScroll(_monitorId);
		}
		else
		{
			_leftScroll(_monitorId);
		}
		_autoScrollCount--;
		_timer = 0f;
	}
}
