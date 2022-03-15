using System;
using MAI2.Util;
using Manager;
using Monitor.MapCore;
using Monitor.MapCore.Component;
using UnityEngine;

namespace Monitor.Entry.Parts
{
	public class InfoWindow : MapBehaviour
	{
		public enum Kind
		{
			AimeTouch,
			GuestOnly,
			Credit,
			MoreCredit
		}

		private class Node
		{
			public readonly Animator Animator;

			public readonly CanvasGroup CanvasGroup;

			public Node(GameObject go)
			{
				Animator = go.GetComponent<Animator>();
				CanvasGroup = go.GetComponentInChildren<CanvasGroup>();
			}
		}

		[SerializeField]
		private GameObject _creditInfoPrefab;

		private Node _info;

		private DelayComponent _delay;

		private Kind _current;

		public bool IsShow { get; private set; }

		private void Awake()
		{
			_info = new Node(UnityEngine.Object.Instantiate(_creditInfoPrefab, base.transform, worldPositionStays: false));
			_delay = base.gameObject.AddComponent<DelayComponent>();
		}

		public void Initialize()
		{
			State = StateUpdate;
		}

		private void StateUpdate(float deltaTime)
		{
			if (IsShow)
			{
				Kind currentKind = GetCurrentKind();
				if (currentKind != _current)
				{
					ShowImmediate(currentKind, null);
				}
			}
		}

		public void Show(Action onDone)
		{
			if (IsShow)
			{
				_delay.StartDelay(0.016f, onDone);
			}
			else
			{
				ShowImmediate(GetCurrentKind(), onDone);
			}
		}

		public void ShowImmediate(Kind kind, Action onDone)
		{
			Hide();
			_current = kind;
			_info.Animator.Play("In");
			_info.Animator.SetBool("Credit", kind == Kind.Credit);
			_info.Animator.SetBool("MoreCredit", kind == Kind.MoreCredit);
			_info.Animator.SetBool("AimeUnavailable", !Singleton<OperationManager>.Instance.IsAliveAime);
			_info.CanvasGroup.alpha = 1f;
			IsShow = true;
			_delay.StartDelay(0.5f, onDone);
		}

		public void ShowImmediateHide(Kind kind)
		{
			Hide();
			_current = kind;
			_info.Animator.Play("In");
			_info.Animator.SetBool("Credit", kind == Kind.Credit);
			_info.Animator.SetBool("MoreCredit", kind == Kind.MoreCredit);
			_info.Animator.SetBool("AimeUnavailable", !Singleton<OperationManager>.Instance.IsAliveAime);
			_info.CanvasGroup.alpha = 1f;
		}

		public void SetIsShowFlag(bool flag)
		{
			IsShow = flag;
		}

		public void Hide()
		{
			IsShow = false;
			_info.CanvasGroup.alpha = 0f;
		}

		public static Kind GetCurrentKind()
		{
			if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsGameCostEnough())
			{
				if (!Singleton<OperationManager>.Instance.IsAliveAime)
				{
					return Kind.GuestOnly;
				}
				return Kind.AimeTouch;
			}
			if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.Remain != 0 || SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.NowCredit != 0)
			{
				return Kind.MoreCredit;
			}
			return Kind.Credit;
		}
	}
}
