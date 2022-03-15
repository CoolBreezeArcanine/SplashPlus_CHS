using UnityEngine;

public class AwakeIconController : MonoBehaviour
{
	private const int MaxIconNum = 5;

	[SerializeField]
	private AwakeIcon[] _awakeIcons;

	[SerializeField]
	private AwakeCenterIcon _centerIcon;

	public void Prepare(Sprite small, Sprite large, float gaugeAmount, int awakeNum)
	{
		for (int i = 0; i < _awakeIcons.Length; i++)
		{
			_awakeIcons[i].Prepare(small, i < awakeNum);
			if (awakeNum >= 5)
			{
				if (awakeNum >= 6)
				{
					_awakeIcons[i].SetStarType(isCounterStop: true);
				}
				else
				{
					_awakeIcons[i].SetStarType(isCounterStop: false);
				}
			}
		}
		_centerIcon.Prepare(large, gaugeAmount);
		if (awakeNum >= 5)
		{
			if (awakeNum >= 6)
			{
				_centerIcon.SetStarType(isCounterStop: true);
			}
			else
			{
				_centerIcon.SetStarType(isCounterStop: false);
			}
			_centerIcon.SetVisibleStar(isActive: true);
		}
		else
		{
			_centerIcon.SetStarType(isCounterStop: false);
		}
	}

	public void AwakePrepare(Sprite small, Sprite large, float gaugeAmount, int awakeNum)
	{
		for (int i = 0; i < _awakeIcons.Length; i++)
		{
			_awakeIcons[i].AwakePrepare(small, i < awakeNum);
		}
		_centerIcon.AwakePrepare(large, gaugeAmount);
		if (awakeNum >= 5)
		{
			if (awakeNum >= 6)
			{
				_centerIcon.SetStarType(isCounterStop: true);
			}
			else
			{
				_centerIcon.SetStarType(isCounterStop: false);
			}
			_centerIcon.SetVisibleStar(isActive: true);
		}
		else
		{
			_centerIcon.SetStarType(isCounterStop: false);
		}
	}

	public void OnCenterIn()
	{
		AwakeIcon[] awakeIcons = _awakeIcons;
		for (int i = 0; i < awakeIcons.Length; i++)
		{
			awakeIcons[i].OnCenterIn();
		}
		_centerIcon.OnCenterIn();
	}

	public void OnCenterOut()
	{
		AwakeIcon[] awakeIcons = _awakeIcons;
		for (int i = 0; i < awakeIcons.Length; i++)
		{
			awakeIcons[i].OnCenterOut();
		}
		_centerIcon.OnCenterOut();
	}

	public void AnimCenterSpark()
	{
		_centerIcon.AnimCenterSpark();
	}

	public void AnimStarGet(int awakeNum)
	{
		if (awakeNum >= 4)
		{
			_centerIcon.AwakeStar();
		}
		else
		{
			_awakeIcons[awakeNum].AwakeStar();
		}
	}

	public void PutStar(float gaugeAmount)
	{
		_centerIcon.AwakeStarDisp(gaugeAmount);
	}
}
