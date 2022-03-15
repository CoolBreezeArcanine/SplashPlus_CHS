using DB;
using Monitor.Common;

namespace Monitor.Entry.Parts.Screens
{
	public class NoticeFelicaRegistration : EntryScreen
	{
		public override void Open(params object[] args)
		{
			base.Open(args);
			OpenCommonWindow(WindowMessageID.EntryNoticeFelicaRegistration);
			OpenOperationInformation(OperationInformationController.InformationType.Hide);
			EntryMonitor.OpenPromotion(PromotionType.None);
			Delay.StartDelay(3f, delegate
			{
				EntryMonitor.ResponseYes();
			});
		}
	}
}
