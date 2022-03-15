using DB;
using Monitor.Common;

namespace Monitor.Entry.Parts.Screens
{
	public class DisplayPleaseWait : EntryScreen
	{
		public override void Open(params object[] args)
		{
			base.Open(args);
			OpenCommonWindow(WindowMessageID.EntryDisplayPleaseWait);
			OpenOperationInformation(OperationInformationController.InformationType.Hide);
			EntryMonitor.OpenPromotion((!EntryMonitor.IsShowInfoWindow()) ? PromotionType.Normal : PromotionType.None);
		}
	}
}
