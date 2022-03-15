using System;
using UnityEngine;

namespace MAI2.Util
{
	public class SingletonMonoBehaviourStateMachine<TClass, TState> : MonoBehaviourStateMachine<TClass, TState> where TClass : MonoBehaviour where TState : struct, IConvertible
	{
		[SerializeField]
		protected bool _dontDestroyOnLoad;

		private static TClass _instance;

		public static TClass Instance => _instance;

		protected SingletonMonoBehaviourStateMachine()
		{
		}

		protected new void Awake()
		{
			base.Awake();
			_instance = base.gameObject.GetComponent<TClass>();
			if (_dontDestroyOnLoad)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.transform.gameObject);
			}
		}

		private void OnDestroy()
		{
			_instance = null;
		}
	}
}
