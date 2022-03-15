using Monitor.Common;

namespace Monitor.Entry.Parts.Screens
{
	public class ConfirmFelica : EntryScreen
	{
		public override void Open(params object[] args)
		{
			base.Open(args);
			OpenOperationInformation(OperationInformationController.InformationType.Hide);
			EntryMonitor.OpenPromotion(PromotionType.None);
			CreateButton(ButtonType.Agree, delegate
			{
				EntryMonitor.ResponseYes();
			});
			CreateButton(ButtonType.Disagree, delegate
			{
				EntryMonitor.ResponseNo();
			});
			ActivateButtons();
		}
	}
}
