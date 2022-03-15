using DB;
using Monitor.Common;

namespace Monitor.Entry.Parts.Screens
{
	public class ConfirmAccessCode : EntryScreen
	{
		public override void Open(params object[] args)
		{
			base.Open(args);
			OpenCommonWindow(WindowMessageID.EntryConfirmAccessCode);
			OpenOperationInformation(OperationInformationController.InformationType.Hide);
			EntryMonitor.OpenPromotion(PromotionType.None);
			CreateButton(ButtonType.Cancel, delegate
			{
				EntryMonitor.ResponseNo();
			});
			ActivateButtons();
		}
	}
}
