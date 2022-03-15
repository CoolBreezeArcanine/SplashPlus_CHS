using System;

namespace Process.Entry.State
{
	public class ProcEnum<TEnum>
	{
		private TEnum _mode;

		private readonly Action _onSet;

		private readonly Action _onGet;

		public TEnum Mode
		{
			get
			{
				_onGet?.Invoke();
				return _mode;
			}
			set
			{
				_onSet?.Invoke();
				_mode = value;
			}
		}

		public ProcEnum(Action onSet, Action onGet = null)
		{
			_onSet = onSet;
			_onGet = onGet;
		}
	}
}
