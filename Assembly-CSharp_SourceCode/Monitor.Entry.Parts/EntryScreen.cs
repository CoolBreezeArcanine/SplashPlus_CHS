using System;
using System.Collections.Generic;
using DB;
using MAI2.Util;
using Manager;
using Monitor.Common;
using Monitor.Entry.Util;
using Monitor.MapCore;
using Monitor.MapCore.Component;
using UnityEngine;

namespace Monitor.Entry.Parts
{
	public class EntryScreen : MapBehaviour
	{
		[SerializeField]
		public bool UseBlurBg;

		protected EntryMonitor EntryMonitor;

		protected Animator Animator;

		protected DelayComponent Delay;

		protected PlayendComponent Playend;

		protected readonly List<EntryButton> Buttons = new List<EntryButton>();

		protected virtual void Awake()
		{
			EntryMonitor = base.Monitor as EntryMonitor;
			Animator = GetComponent<Animator>();
			Delay = base.gameObject.AddComponent<DelayComponent>();
			Playend = base.gameObject.AddComponent<PlayendComponent>();
		}

		public virtual void Open(params object[] args)
		{
			base.gameObject.SetActive(value: true);
			Animator.Play("In");
			Playend.StartWatching(Animator, "Base Layer", "In", delegate
			{
				State = StateUpdate;
			});
		}

		public virtual void Close()
		{
			Animator.Play("Out");
			Playend.StartWatching(Animator, "Base Layer", "Out", delegate
			{
				base.gameObject.SetActive(value: false);
			});
			CloseCommonWindow();
			DeactivateButtons();
			SetStateTerminate();
		}

		protected virtual void StateUpdate(float deltaTime)
		{
		}

		protected EntryButton CreateButton(ButtonType type, Action<EntryButton> onPush, Func<bool> isValid = null, Action<EntryButton> onPress = null)
		{
			bool flag = false;
			flag = isValid != null && ((!isValid()) ? true : false);
			EntryButton entryButton = EntryMonitor.CreateCommonButton(type, flag);
			entryButton.OnPush = onPush;
			entryButton.OnPress = onPress ?? ((Action<EntryButton>)delegate
			{
				LockButtons();
			});
			entryButton.IsValid = isValid ?? ((Func<bool>)(() => true));
			entryButton.gameObject.SetActive(value: false);
			Buttons.Add(entryButton);
			return entryButton;
		}

		protected void ActivateButtons()
		{
			foreach (EntryButton button in Buttons)
			{
				button.gameObject.SetActive(value: true);
				button.Activate();
			}
		}

		protected void DeactivateButtons()
		{
			Buttons.ForEach(delegate(EntryButton b)
			{
				b.Deactivate();
			});
			Buttons.Clear();
		}

		public void LockButtons(bool flag = true)
		{
			Buttons.ForEach(delegate(EntryButton b)
			{
				b.IsLock = flag;
			});
		}

		protected void OpenCommonWindow(WindowMessageID windowMessageID)
		{
			EntryMonitor.Process.ProcessManager.EnqueueMessage(base.Monitor.MonitorIndex, windowMessageID);
		}

		protected void CloseCommonWindow()
		{
			EntryMonitor.Process.ProcessManager.CloseWindow(EntryMonitor.MonitorIndex);
		}

		protected void OpenOperationInformation(OperationInformationController.InformationType type)
		{
			EntryMonitor.Process.ProcessManager.SendMessage(new Message(ProcessType.CommonProcess, 40000, base.Monitor.MonitorIndex, type));
		}

		protected void OpenOperationInformation(bool isAimeUser, bool isFreedom)
		{
			bool isEnoughCredit = ((!isFreedom) ? SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsGameCostEnough() : SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsGameCostEnoughFreedom());
			bool isCoinExist = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.Remain != 0 || SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.NowCredit != 0;
			OpenOperationInformation(isEnoughCredit, isCoinExist, isAimeUser, Singleton<OperationManager>.Instance.IsAimeOffline());
		}

		protected void OpenOperationInformation(bool isEnoughCredit, bool isCoinExist, bool isAimeUser, bool isAimeUnavailable)
		{
			EntryMonitor.Process.ProcessManager.SendMessage(new Message(ProcessType.CommonProcess, 40001, base.Monitor.MonitorIndex, isEnoughCredit, isCoinExist, isAimeUser, isAimeUnavailable, false, true));
		}
	}
}
