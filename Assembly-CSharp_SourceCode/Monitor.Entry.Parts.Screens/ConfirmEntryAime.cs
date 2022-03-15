using System.Collections.Generic;
using System.Linq;
using DB;
using Manager;
using Monitor.Common;
using UnityEngine;

namespace Monitor.Entry.Parts.Screens
{
	public class ConfirmEntryAime : EntryScreen
	{
		[SerializeField]
		private GameObject _userEntryPrefab;

		private GameObject _userEntry;

		protected UserEntryPlate UserEntryPlate;

		[SerializeField]
		private GameObject _effMokuPrefab;

		private GameObject _effMoku;

		protected List<Animator> EffMokuAnimators;

		protected bool UpdateOperationInformation;

		public override void Open(params object[] args)
		{
			bool num = (bool)args[0];
			WindowMessageID messageId = (WindowMessageID)args[1];
			PromotionType promotionType = (PromotionType)args[2];
			base.Open(args);
			BuildUserEntry();
			OpenOperationInformation(OperationInformationController.InformationType.Hide);
			EntryMonitor.OpenPromotion(PromotionType.None);
			EntryMonitor.HideInfoWindow();
			CreateButton(ButtonType.Entry, delegate
			{
				EntryMonitor.ResponseYes();
			}, () => true);
			CreateButton(ButtonType.No, delegate
			{
				EntryMonitor.ResponseNo();
			});
			CreateButton(ButtonType.AccessCode, delegate
			{
				EntryMonitor.ResponseAccessCode();
			});
			if (!num)
			{
				_ = GameManager.IsEventMode;
			}
			Delay.StartDelay(0.35f, delegate
			{
				UserEntryPlate.AnimIn();
				EffMokuAnimators.ForEach(delegate(Animator e)
				{
					e.Play("In");
				});
				Delay.StartDelay(1.05f, delegate
				{
					OpenCommonWindow(messageId);
					UpdateOperationInformation = true;
					ActivateButtons();
					Delay.StartDelay(0.2f, delegate
					{
						EntryMonitor.OpenPromotion(promotionType);
					});
				});
			});
		}

		protected override void StateUpdate(float deltaTime)
		{
			if (UpdateOperationInformation)
			{
				OpenOperationInformation(isAimeUser: true, isFreedom: false);
			}
		}

		public override void Close()
		{
			base.Close();
			UpdateOperationInformation = false;
			UserEntryPlate.AnimOut();
			Delay.Stop();
		}

		protected void BuildUserEntry()
		{
			if (_userEntry != null)
			{
				Object.Destroy(_userEntry);
			}
			_userEntry = Object.Instantiate(_userEntryPrefab, base.transform, worldPositionStays: false);
			UserEntryPlate = _userEntry.GetComponent<UserEntryPlate>();
			UserEntryPlate.Initialize(base.Monitor.MonitorIndex);
			UserEntryPlate.SetUserData(EntryMonitor.Process.Container.assetManager.GetIconTexture2D(base.Monitor.MonitorIndex, EntryMonitor.UserIcon), EntryMonitor.UserName, EntryMonitor.UserRating, EntryMonitor.UserDispRate, EntryMonitor.UserTotalAwake);
			if (_effMoku != null)
			{
				Object.Destroy(_effMoku);
			}
			_effMoku = Object.Instantiate(_effMokuPrefab, base.transform, worldPositionStays: false);
			EffMokuAnimators = _effMoku.GetComponentsInChildren<Animator>().ToList();
		}
	}
}
