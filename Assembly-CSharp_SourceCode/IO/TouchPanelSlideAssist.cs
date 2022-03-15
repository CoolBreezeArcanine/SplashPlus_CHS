using IO.TouchPanel;
using Manager;

namespace IO
{
	internal class TouchPanelSlideAssist
	{
		private enum SlideStep
		{
			None = 0,
			CheckC1 = 1,
			CheckC2 = 2,
			CheckC1ReleaseWait = 3,
			CheckC2ReleaseWait = 4,
			End = 5,
			Invalid = -1
		}

		public enum SlideThroughSpeed
		{
			None,
			Fast1,
			Fast2,
			Fast3,
			Fast4,
			Fast5,
			End
		}

		private const int SensorNum = 2;

		private const int TimeoutTotalMsec = 200;

		private const int TimeoutStatMsec = 200;

		private readonly int[] _slideLevel = new int[6] { 0, 20, 40, 60, 80, 201 };

		private readonly SlideStep[] _eStat = new SlideStep[2];

		private readonly double[] _dCountTotal = new double[2];

		private readonly double[] _dCountStat = new double[2];

		private readonly bool[] _bMoveLr = new bool[2];

		private readonly bool[] _bMoveRl = new bool[2];

		private readonly uint[] _uMoveLrLevel = new uint[2];

		private readonly uint[] _uMoveRlLevel = new uint[2];

		private readonly bool[] _bState = new bool[2];

		private readonly bool[] _bOnly = new bool[2];

		private readonly InputManager.TouchPanelArea[] _eSwitchList = new InputManager.TouchPanelArea[2]
		{
			InputManager.TouchPanelArea.C1,
			InputManager.TouchPanelArea.C2
		};

		public void Execute(GameInputLog tpLog)
		{
			for (uint num = 0u; num < 2; num++)
			{
				_bMoveLr[num] = false;
				_bMoveRl[num] = false;
				for (int i = 0; i < 2; i++)
				{
					_bState[i] = tpLog.StateOn(num, _eSwitchList[i]);
				}
				for (int j = 0; j < 2; j++)
				{
					_bOnly[j] = _bState[j];
					for (int k = 0; k < 2; k++)
					{
						if (j != k && _bState[k])
						{
							_bOnly[j] = false;
							break;
						}
					}
				}
				switch (_eStat[num])
				{
				case SlideStep.None:
					_dCountTotal[num] = 0.0;
					if (_bOnly[0])
					{
						SetStat(num, SlideStep.CheckC2);
					}
					else if (_bOnly[1])
					{
						SetStat(num, SlideStep.CheckC1);
					}
					break;
				case SlideStep.CheckC1:
					if (_bOnly[0])
					{
						_bMoveLr[num] = true;
						SetThroughLrLevel(num);
						SetStat(num, SlideStep.CheckC1ReleaseWait);
					}
					else
					{
						CheckCount(num);
					}
					break;
				case SlideStep.CheckC2:
					if (_bOnly[1])
					{
						_bMoveRl[num] = true;
						SetThroughRlLevel(num);
						SetStat(num, SlideStep.CheckC2ReleaseWait);
					}
					else
					{
						CheckCount(num);
					}
					break;
				case SlideStep.CheckC1ReleaseWait:
					if (!tpLog.StateOn(num, _eSwitchList[0]))
					{
						SetStat(num, SlideStep.None);
					}
					break;
				case SlideStep.CheckC2ReleaseWait:
					if (!tpLog.StateOn(num, _eSwitchList[1]))
					{
						SetStat(num, SlideStep.None);
					}
					break;
				}
			}
		}

		private void ClearJump(uint playerID)
		{
			SetStat(playerID, SlideStep.None);
			_dCountTotal[playerID] = 0.0;
			_bMoveLr[playerID] = false;
			_bMoveRl[playerID] = false;
			_uMoveLrLevel[playerID] = 0u;
			_uMoveRlLevel[playerID] = 0u;
		}

		public bool IsMoveLr(uint playerID)
		{
			return _bMoveLr[playerID];
		}

		public uint IsMoveLrLevel(uint playerID)
		{
			return _uMoveLrLevel[playerID];
		}

		public bool IsMoveRl(uint playerID)
		{
			return _bMoveRl[playerID];
		}

		public uint IsMoveRlLevel(uint playerID)
		{
			return _uMoveRlLevel[playerID];
		}

		private void SetStat(uint playerID, SlideStep step)
		{
			_eStat[playerID] = step;
			_dCountStat[playerID] = 0.0;
		}

		private void SetThroughLrLevel(uint playerID)
		{
			for (int i = 0; i < _slideLevel.Length; i++)
			{
				if (_dCountTotal[playerID] < (double)_slideLevel[i])
				{
					_uMoveLrLevel[playerID] = (uint)i;
					break;
				}
			}
		}

		private void SetThroughRlLevel(uint playerID)
		{
			for (int i = 0; i < _slideLevel.Length; i++)
			{
				if (_dCountTotal[playerID] < (double)_slideLevel[i])
				{
					_uMoveRlLevel[playerID] = (uint)i;
					break;
				}
			}
		}

		private void CheckCount(uint playerID)
		{
			bool flag = false;
			_dCountStat[playerID] += GameManager.GetGameMSecAddD();
			if (_dCountStat[playerID] >= 200.0)
			{
				flag = true;
			}
			_dCountTotal[playerID] += GameManager.GetGameMSecAddD();
			if (_dCountTotal[playerID] >= 200.0)
			{
				flag = true;
			}
			if (flag)
			{
				ClearJump(playerID);
			}
		}
	}
}
