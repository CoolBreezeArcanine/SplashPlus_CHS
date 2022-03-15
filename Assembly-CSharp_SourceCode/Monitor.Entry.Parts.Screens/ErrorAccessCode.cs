using DB;
using Mai2.Mai2Cue;
using Monitor.Common;

namespace Monitor.Entry.Parts.Screens
{
	public class ErrorAccessCode : EntryScreen
	{
		public override void Open(params object[] args)
		{
			base.Open(args);
			OpenCommonWindow(WindowMessageID.EntryErrorAccessCodeRegistration);
			OpenOperationInformation(OperationInformationController.InformationType.Hide);
			EntryMonitor.OpenPromotion(PromotionType.None);
			PlaySE(Cue.SE_ENTRY_AIME_ERROR);
			Delay.StartDelay(3f, delegate
			{
				EntryMonitor.ResponseYes();
			});
		}
	}
}
