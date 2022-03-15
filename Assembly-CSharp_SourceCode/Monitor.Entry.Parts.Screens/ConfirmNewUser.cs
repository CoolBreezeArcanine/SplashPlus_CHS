using DB;

namespace Monitor.Entry.Parts.Screens
{
	public class ConfirmNewUser : EntryScreen
	{
		public override void Open(params object[] args)
		{
			base.Open(args);
			OpenCommonWindow(WindowMessageID.EntryConfirmNewUser);
			EntryMonitor.OpenPromotion(PromotionType.None);
			CreateButton(ButtonType.Entry, delegate
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
