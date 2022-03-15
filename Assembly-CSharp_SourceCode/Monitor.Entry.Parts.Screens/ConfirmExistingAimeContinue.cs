using DB;
using MAI2.Util;
using Manager;
using Monitor.Common;
using UnityEngine;

namespace Monitor.Entry.Parts.Screens
{
	public class ConfirmExistingAimeContinue : ConfirmEntryAime
	{
		public override void Open(params object[] args)
		{
			InitAnimation();
			BuildUserEntry();
			OpenOperationInformation(OperationInformationController.InformationType.Hide);
			EntryMonitor.OpenPromotion(PromotionType.None);
			EntryMonitor.HideInfoWindow();
			CreateButton(ButtonType.Entry, delegate
			{
				EntryMonitor.ResponseYes();
			}, () => !EntryMonitor.OtherMonitor.IsLoginProcessing && SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsGameCostEnough(), delegate
			{
				EntryMonitor.IsLoginProcessing = true;
			});
			CreateButton(ButtonType.No, delegate
			{
				EntryMonitor.ResponseNo();
			});
			CreateButton(ButtonType.AccessCode, delegate
			{
				EntryMonitor.ResponseAccessCode();
			});
			Delay.StartDelay(0.35f, delegate
			{
				UserEntryPlate.AnimIn();
				EffMokuAnimators.ForEach(delegate(Animator e)
				{
					e.Play("In");
				});
				Delay.StartDelay(1.05f, delegate
				{
					OpenCommonWindow(WindowMessageID.EntryConfirmExistingAime);
					UpdateOperationInformation = true;
					ActivateButtons();
				});
			});
		}

		private void InitAnimation()
		{
			base.gameObject.SetActive(value: true);
			Animator.Play("In");
			Playend.StartWatching(Animator, "Base Layer", "In", delegate
			{
				State = StateUpdate;
			});
		}
	}
}
