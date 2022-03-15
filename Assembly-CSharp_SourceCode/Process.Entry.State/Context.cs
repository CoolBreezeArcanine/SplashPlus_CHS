using System.Collections.Generic;
using System.Linq;

namespace Process.Entry.State
{
	public class Context
	{
		private readonly Dictionary<int, State> _table;

		private State _current;

		private int _currentKey;

		public Context()
		{
			_table = new Dictionary<int, State>();
		}

		public void AddState(int key, State state)
		{
			state.Setup(this);
			_table[key] = state;
		}

		public void SetState(int key, params object[] args)
		{
			_current?.Term();
			if (_table.TryGetValue(key, out var value))
			{
				_current = value;
				_currentKey = key;
				_current.Init(args);
			}
			else
			{
				_current = null;
				_currentKey = 0;
			}
		}

		public void NextState(params object[] args)
		{
			SetState(_currentKey + 1, args);
		}

		public void Execute(float deltaTime)
		{
			_current?.Exec(deltaTime);
		}

		public void GetCurrentState(out int key, out State state)
		{
			if (_current == null)
			{
				key = -1;
				state = null;
				return;
			}
			key = _table.FirstOrDefault((KeyValuePair<int, State> i) => i.Value == _current).Key;
			state = _current;
		}

		public override string ToString()
		{
			return _current?.ToString() ?? "";
		}
	}
}
