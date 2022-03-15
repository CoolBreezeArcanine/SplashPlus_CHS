using SplitFlapDisplay;
using UnityEngine;

namespace Monitor.Result
{
	public class DXRatingCounterDisplay : MonoBehaviour
	{
		[SerializeField]
		private SplitFlapDisplayImageUnit[] _counters;

		private int _firstDigit;

		private int _secondDigit;

		private int _thirdDigit;

		private int _fourthDigit;

		private int _fifthDigit;

		public void Initialize(int initial)
		{
			int initialIndex = initial % 10;
			int initialIndex2 = initial / 10 % 10;
			int initialIndex3 = initial / 100 % 10;
			int initialIndex4 = initial / 1000 % 10;
			int initialIndex5 = initial / 10000 % 10;
			_counters[0].Initialize(initialIndex);
			_counters[1].Initialize(initialIndex2);
			_counters[2].Initialize(initialIndex3);
			_counters[3].Initialize(initialIndex4);
			_counters[4].Initialize(initialIndex5);
		}

		public void DisplayUpdate()
		{
			SplitFlapDisplayImageUnit[] counters = _counters;
			for (int i = 0; i < counters.Length; i++)
			{
				counters[i].UnitUpdate();
			}
		}

		public void Play(int target)
		{
			_firstDigit = target % 10;
			_secondDigit = target / 10 % 10;
			_thirdDigit = target / 100 % 10;
			_fourthDigit = target / 1000 % 10;
			_fifthDigit = target / 10000 % 10;
			if (_fifthDigit == 0)
			{
				_fifthDigit = 10;
				if (_fourthDigit == 0)
				{
					_fourthDigit = 10;
					if (_thirdDigit == 0)
					{
						_thirdDigit = 10;
						if (_secondDigit == 0)
						{
							_thirdDigit = 10;
						}
					}
				}
			}
			_counters[0].Play(-1);
			_counters[1].Play(-1);
			_counters[2].Play(-1);
			_counters[3].Play(-1);
			_counters[4].Play(-1);
		}

		public void Stop()
		{
			_counters[0].Play(_firstDigit);
			_counters[1].Play(_secondDigit);
			_counters[2].Play(_thirdDigit);
			_counters[3].Play(_fourthDigit);
			_counters[4].Play(_fifthDigit);
		}
	}
}
