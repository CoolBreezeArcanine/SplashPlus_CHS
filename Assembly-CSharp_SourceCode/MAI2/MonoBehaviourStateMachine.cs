using System;
using RD1.SSS;
using UnityEngine;

namespace MAI2
{
	public class MonoBehaviourStateMachine<TClass, TState> : MonoBehaviour where TClass : class where TState : struct, IConvertible
	{
		private RD1.SSS.StateMachine<TClass, TState> _stateMachine;

		public string stateString => _stateMachine.getStateName();

		protected void Awake()
		{
			_stateMachine = new RD1.SSS.StateMachine<TClass, TState>(this, "_");
		}

		protected void Update()
		{
			_stateMachine.updateState(Time.deltaTime);
		}

		public TState getCurrentState()
		{
			return _stateMachine.State;
		}

		public TState? getNextState()
		{
			return _stateMachine.NextState;
		}

		public bool isStateEnd()
		{
			return _stateMachine.isStateEnd();
		}

		public void setNextState(TState state)
		{
			_stateMachine.GoNext(state);
		}

		public void setStateEnd()
		{
			_stateMachine.GoExit();
		}
	}
}
