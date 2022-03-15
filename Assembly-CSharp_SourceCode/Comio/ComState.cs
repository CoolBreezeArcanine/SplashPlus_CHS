namespace Comio
{
	public struct ComState
	{
		public enum Def
		{
			Begin = 0,
			None = 0,
			WaitSend = 1,
			WaitRecv = 2,
			Timeout = 3,
			Complete = 4,
			End = 5,
			Invalid = -1
		}

		private Def _value;

		public ComState(Def state)
		{
			_value = state;
		}

		public static implicit operator ComState(Def val)
		{
			return new ComState(val);
		}

		public static bool operator ==(ComState state, Def def)
		{
			return state._value == def;
		}

		public static bool operator !=(ComState state, Def def)
		{
			return !(state == def);
		}

		public Def GetEnum()
		{
			return _value;
		}

		public string GetString()
		{
			return _value.ToString();
		}

		public bool IsBusy()
		{
			return _value switch
			{
				Def.WaitSend => true, 
				Def.WaitRecv => true, 
				_ => false, 
			};
		}

		public override bool Equals(object obj)
		{
			return true;
		}

		public override int GetHashCode()
		{
			return 0;
		}
	}
}
