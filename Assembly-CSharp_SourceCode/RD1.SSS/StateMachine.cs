using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace RD1.SSS
{
	public class StateMachine<TClass, TState> : IStateMachine where TClass : class where TState : struct, IConvertible
	{
		private struct MethodSet
		{
			public bool IsValid { get; set; }

			public Action<TClass> Enter { get; set; }

			public Action<TClass> Execute { get; set; }

			public Action<TClass> Leave { get; set; }
		}

		private Dictionary<TState, Type> _childStateMap = new Dictionary<TState, Type>();

		private IStateMachine _childState;

		private static readonly TState[] States = (TState[])Enum.GetValues(typeof(TState));

		private int stateFrame;

		private float stateDeltaTime;

		public TState State { get; private set; }

		public bool IsExited { get; private set; }

		public int stateResult { get; protected set; }

		public IStateMachine childState => _childState;

		public bool isChildStateEnd
		{
			get
			{
				if (_childState != null)
				{
					return _childState.isStateEnd();
				}
				return true;
			}
		}

		public int childStateResult
		{
			get
			{
				if (_childState != null)
				{
					return _childState.getStateResult();
				}
				return -1;
			}
		}

		private TClass MethodHolder { get; set; }

		private MethodSet[] MethodSets { get; set; }

		private MethodSet CurrentMethodSet { get; set; }

		private IEnumerator CurrentCoRoutine { get; set; }

		public TState? NextState { get; private set; }

		private bool NextExit { get; set; }

		public int StateFrame => stateFrame;

		public float StateDeltaTime => stateDeltaTime;

		private static Action<TClass> getMethod(Type type, string name)
		{
			MethodInfo method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (null == method)
			{
				return null;
			}
			return Delegate.CreateDelegate(typeof(Action<TClass>), method) as Action<TClass>;
		}

		public StateMachine(object methodHolder, string statePrefix, TState firstState)
		{
			if (methodHolder == null)
			{
				methodHolder = this;
			}
			if (!States.Contains(firstState))
			{
				throw new ArgumentException("`firstState` is invalid value.", "firstState");
			}
			if (!(methodHolder is TClass))
			{
				throw new ArgumentException("The type of `methodHolder' is not TClass", "firstState");
			}
			int num = 0;
			for (int i = 0; i < States.Length; i++)
			{
				int b = Convert.ToInt32(States[i]);
				num = Mathf.Max(num, b);
			}
			State = firstState;
			IsExited = false;
			MethodHolder = methodHolder as TClass;
			MethodSets = new MethodSet[num + 1];
			CurrentMethodSet = default(MethodSet);
			CurrentCoRoutine = null;
			NextState = firstState;
			NextExit = false;
			if (statePrefix == null)
			{
				statePrefix = string.Empty;
			}
			Type type = methodHolder.GetType();
			StringBuilder stringBuilder = new StringBuilder();
			for (int j = 0; j < States.Length; j++)
			{
				int num2 = Convert.ToInt32(States[j]);
				if (num2 >= 0 && !MethodSets[num2].IsValid)
				{
					stringBuilder.Length = 0;
					string text = stringBuilder.Append(statePrefix).Append(States[j].ToString()).ToString();
					stringBuilder.Length = 0;
					Action<TClass> method = getMethod(type, stringBuilder.Append("Enter").Append(text).ToString());
					stringBuilder.Length = 0;
					Action<TClass> method2 = getMethod(type, stringBuilder.Append("Execute").Append(text).ToString());
					if (method2 == null)
					{
						throw new ArgumentException("`Execute" + text + "()` method is not found in `methodHolder`.", "methodHolder");
					}
					stringBuilder.Length = 0;
					Action<TClass> method3 = getMethod(type, stringBuilder.Append("Leave").Append(text).ToString());
					MethodSets[num2] = default(MethodSet);
					MethodSets[num2].Enter = method;
					MethodSets[num2].Execute = method2;
					MethodSets[num2].Leave = method3;
					MethodSets[num2].IsValid = true;
				}
			}
		}

		public StateMachine(object methodHolder, TState firstState)
			: this(methodHolder, typeof(TState).Name, firstState)
		{
		}

		public StateMachine(object methodHolder, string statePrefix)
			: this(methodHolder, statePrefix, States[0])
		{
		}

		public StateMachine(object methodHolder)
			: this(methodHolder, (string)null)
		{
		}

		public void GoNext(TState state)
		{
			if (!States.Contains(state))
			{
				throw new ArgumentException("`state` is invalid value.", "state");
			}
			NextState = state;
		}

		public void GoExit()
		{
			NextExit = true;
		}

		public virtual bool updateState(float deltaTime)
		{
			if (IsExited)
			{
				return false;
			}
			if (NextExit)
			{
				if (CurrentMethodSet.Leave != null)
				{
					CurrentMethodSet.Leave(MethodHolder);
				}
				IsExited = true;
				NextState = null;
				NextExit = false;
				stateDeltaTime = 0f;
				stateFrame = 0;
				return false;
			}
			if (NextState.HasValue)
			{
				if (CurrentMethodSet.Leave != null)
				{
					CurrentMethodSet.Leave(MethodHolder);
				}
				State = NextState.Value;
				CurrentMethodSet = MethodSets[Convert.ToInt32(State)];
				NextState = null;
				stateFrame = 0;
				stateDeltaTime = 0f;
				createChildState(State);
				if (CurrentMethodSet.Enter != null)
				{
					CurrentMethodSet.Enter(MethodHolder);
				}
			}
			updateChildState(deltaTime);
			if (CurrentMethodSet.Execute != null)
			{
				stateDeltaTime += deltaTime;
				CurrentMethodSet.Execute(MethodHolder);
				stateFrame++;
			}
			return true;
		}

		public bool isStateEnd()
		{
			return IsExited;
		}

		public string getStateName()
		{
			string text = ToString() + ":" + State;
			text = ((!IsExited) ? (text + "[" + stateFrame + "]") : (text + "[EXIT]"));
			if (_childState != null)
			{
				text = text + "\n" + _childState.getStateName();
			}
			return text;
		}

		public void getStateName(StringBuilder stringBuilder)
		{
			stringBuilder.Append(ToString()).Append(':').Append(State.ToString());
			if (IsExited)
			{
				stringBuilder.Append("[EXIT]");
			}
			else
			{
				stringBuilder.Append('[').Append(stateFrame).Append(']');
			}
			if (_childState != null)
			{
				stringBuilder.AppendLine();
				_childState.getStateName(stringBuilder);
			}
		}

		public int getStateResult()
		{
			return stateResult;
		}

		public void addChildState<AAA>(TState state) where AAA : IStateMachine, new()
		{
			_childStateMap.Add(state, typeof(AAA));
		}

		private void createChildState(TState state)
		{
			_childState = null;
			if (_childStateMap.TryGetValue(state, out var value))
			{
				_childState = (IStateMachine)Activator.CreateInstance(value);
			}
		}

		private void updateChildState(float deltaTime)
		{
			if (_childState != null)
			{
				_childState.updateState(deltaTime);
			}
		}
	}
}
