using System;
using System.Collections.Generic;
using System.Linq;
using MAI2.Util;
using Manager;
using UnityEngine;

namespace Monitor.MapCore
{
	public class MapMonitor : MonitorBase
	{
		private class BehaviourNode
		{
			public MapBehaviour Behaviour;

			public Action OnStart;

			public Action<float> OnUpdate;

			public Action<float> OnLateUpdate;
		}

		private readonly List<BehaviourNode> _behaviours = new List<BehaviourNode>();

		protected Action<float> State;

		private float _deltaRatio = 1f;

		public bool _isEntry;

		public bool _isGuest;

		public float DeltaRatio
		{
			get
			{
				return _deltaRatio;
			}
			set
			{
				if (!(Mathf.Abs(_deltaRatio - value) < float.Epsilon))
				{
					_deltaRatio = value;
					_behaviours.ForEach(delegate(BehaviourNode b)
					{
						b.Behaviour.OnChangeDeltaRatio(_deltaRatio);
					});
				}
			}
		}

		public void AddEvents(MapBehaviour behaviour, Action start, Action<float> update, Action<float> lateUpdate)
		{
			if (!_behaviours.Any((BehaviourNode b) => b.Behaviour == behaviour))
			{
				_behaviours.Add(new BehaviourNode
				{
					Behaviour = behaviour,
					OnStart = start,
					OnUpdate = update,
					OnLateUpdate = lateUpdate
				});
				behaviour.OnChangeDeltaRatio(DeltaRatio);
			}
		}

		public void RemoveEvents(MonoBehaviour mono)
		{
			BehaviourNode behaviourNode = _behaviours.FirstOrDefault((BehaviourNode b) => b.Behaviour == mono);
			if (behaviourNode != null)
			{
				_behaviours.Remove(behaviourNode);
			}
		}

		public virtual void OnUpdate(float deltaTime)
		{
			deltaTime *= DeltaRatio;
			State?.Invoke(deltaTime);
			foreach (BehaviourNode item in _behaviours.Where((BehaviourNode b) => b.Behaviour.enabled && b.Behaviour.gameObject.activeSelf).ToList())
			{
				if (item.OnStart != null)
				{
					item.OnStart();
					item.OnStart = null;
				}
				else
				{
					item.OnUpdate(deltaTime);
				}
			}
		}

		public virtual void OnLateUpdate(float deltaTime)
		{
			deltaTime *= DeltaRatio;
			foreach (BehaviourNode item in _behaviours.Where((BehaviourNode b) => b.Behaviour.enabled && b.Behaviour.gameObject.activeSelf).ToList())
			{
				item.OnLateUpdate(deltaTime);
			}
		}

		public override void ViewUpdate()
		{
		}

		public UserData GetUserData()
		{
			return Singleton<UserDataManager>.Instance.GetUserData(monitorIndex);
		}

		protected void SetStateTerminate()
		{
			State = null;
		}
	}
}
