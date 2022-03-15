using DB;
using Monitor.Common;

namespace Monitor.Entry.Parts.Screens
{
	public class ConfirmFelicaSite : EntryScreen
	{
		public override void Open(params object[] args)
		{
			base.Open(args);
			OpenCommonWindow(WindowMessageID.EntryConfirmFelicaSite);
			OpenOperationInformation(OperationInformationController.InformationType.Hide);
			EntryMonitor.OpenPromotion(PromotionType.None);
			CreateButton(ButtonType.Yes, delegate
			{
				EntryMonitor.ResponseYes();
			});
			CreateButton(ButtonType.No, delegate
			{
				EntryMonitor.ResponseNo();
			});
			ActivateButtons();
		}
	}
}
