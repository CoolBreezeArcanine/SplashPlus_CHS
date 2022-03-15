using MAI2.Util;
using Manager;
using UnityEngine;

namespace Monitor.Entry.Parts.Screens
{
	public class ConfirmFreedom : EntryScreen
	{
		[SerializeField]
		private GameObject _derakkumaRoot;

		[SerializeField]
		private GameObject _derakkumaPrefub;

		private Animator _derakkumaAnim;

		protected override void Awake()
		{
			base.Awake();
			_derakkumaAnim = Object.Instantiate(_derakkumaPrefub, _derakkumaRoot.transform).GetComponent<Animator>();
		}

		public override void Open(params object[] args)
		{
			base.Open(args);
			EntryMonitor.OpenPromotion(PromotionType.None);
			CreateButton(ButtonType.Entry, delegate
			{
				EntryMonitor.ResponseYes();
			}, () => SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsGameCostEnoughFreedom());
			CreateButton(ButtonType.No, delegate
			{
				EntryMonitor.ResponseNo();
			});
			ActivateButtons();
			_derakkumaAnim?.Play("Default");
			State = StateUpdate;
		}

		protected override void StateUpdate(float deltaTime)
		{
			OpenOperationInformation(isAimeUser: true, isFreedom: true);
		}
	}
}
