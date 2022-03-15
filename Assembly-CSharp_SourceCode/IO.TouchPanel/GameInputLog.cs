using Manager;

namespace IO.TouchPanel
{
	internal class GameInputLog
	{
		private enum TpInputGeneretion
		{
			Now,
			Last,
			Old,
			End
		}

		private const int MaxLog = 16;

		private readonly ulong[,] _tpData = new ulong[2, 3];

		private readonly bool[] _tpTrigerFrame = new bool[2];

		public void Execute(int index, ulong now, bool update = true)
		{
			_tpTrigerFrame[index] = false;
			if (update)
			{
				for (int num = 2; num > 0; num--)
				{
					_tpData[index, num] = _tpData[index, num - 1];
				}
				_tpData[index, 0] = now;
				_tpTrigerFrame[index] = true;
				for (int i = 0; i < 35; i++)
				{
					KillOnOffOn((uint)index, (InputManager.TouchPanelArea)i);
				}
			}
		}

		public bool StateOn(uint side, InputManager.TouchPanelArea type)
		{
			return Now(side, type);
		}

		public bool StateOff(uint side, InputManager.TouchPanelArea type)
		{
			return !Now(side, type);
		}

		public bool TriggerOn(uint side, InputManager.TouchPanelArea type)
		{
			if (!Last(side, type) && Now(side, type))
			{
				return _tpTrigerFrame[side];
			}
			return false;
		}

		public bool Now(uint side, InputManager.TouchPanelArea type)
		{
			return ((_tpData[side, 0] >> (int)type) & 1) != 0;
		}

		public bool Last(uint side, InputManager.TouchPanelArea type)
		{
			return ((_tpData[side, 1] >> (int)type) & 1) != 0;
		}

		public bool Old(uint side, InputManager.TouchPanelArea type)
		{
			return ((_tpData[side, 2] >> (int)type) & 1) != 0;
		}

		public void KillOnOffOn(uint side, InputManager.TouchPanelArea type)
		{
			if (Old(side, type) && !Last(side, type) && Now(side, type))
			{
				ulong num = (ulong)(~(1L << (int)type));
				_tpData[side, 0] &= num;
			}
		}
	}
}
