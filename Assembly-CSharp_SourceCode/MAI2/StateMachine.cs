using System;
using RD1.SSS;
using UnityEngine;

namespace MAI2
{
	public class StateMachine<TClass, TState> : RD1.SSS.StateMachine<TClass, TState> where TClass : class where TState : struct, IConvertible
	{
		public string stateString => getStateName();

		public StateMachine()
			: base((object)null, "_")
		{
		}

		public TState getCurrentState()
		{
			return base.State;
		}

		public void setNextState(TState state)
		{
			GoNext(state);
		}

		public void setStateEnd()
		{
			GoExit();
		}

		public override bool updateState(float deltaTime = -1f)
		{
			if (deltaTime < 0f)
			{
				deltaTime = Time.deltaTime;
			}
			return base.updateState(deltaTime);
		}
	}
}
