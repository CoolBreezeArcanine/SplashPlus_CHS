using System;

namespace MAI2.Util
{
	public class SingletonStateMachine<TClass, TState> : StateMachine<TClass, TState> where TClass : class, new()where TState : struct, IConvertible
	{
		private static readonly TClass _instance = new TClass();

		public static TClass Instance => _instance;

		protected SingletonStateMachine()
		{
		}
	}
}
