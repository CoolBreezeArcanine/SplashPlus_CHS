using System;
using UnityEngine;

namespace Monitor.MapCore.Component
{
	public class PlayendComponent : MapBehaviour
	{
		private Animator _animator;

		private int _layer;

		private int _hullParhHash;

		private Action _onDone;

		private bool _request;

		public void StartWatching(Animator animator, int layer, string fullPath, Action onDone)
		{
			_animator = animator;
			_layer = layer;
			_hullParhHash = Animator.StringToHash(fullPath);
			_onDone = onDone;
			_request = true;
		}

		public void StartWatching(Animator animator, string layerName, string stateName, Action onDone)
		{
			StartWatching(animator, animator.GetLayerIndex(layerName), layerName + "." + stateName, onDone);
		}

		private void StateUpdate(float deltaTime)
		{
			_animator.PlayendAction(_layer, _hullParhHash, delegate
			{
				_onDone?.Invoke();
				SetStateTerminate();
			});
		}

		protected override void OnLateUpdate(float deltaTime)
		{
			if (_request)
			{
				State = StateUpdate;
				_request = false;
			}
		}
	}
}
