using DB;
using Mai2.Mai2Cue;
using Monitor.Common;

namespace Monitor.Entry.Parts.Screens
{
	public class ErrorAime : EntryScreen
	{
		public const int ErrorAimeLogin = 256;

		public const int ErrorAimeOverlap = 512;

		public override void Open(params object[] args)
		{
			WindowMessageID windowMessageID = (int)args[0] switch
			{
				1 => WindowMessageID.EntryErrorAimeUnknown, 
				2 => WindowMessageID.EntryErrorAimeNetwork, 
				256 => WindowMessageID.EntryErrorAimeLogin, 
				512 => WindowMessageID.EntryErrorAimeOverlap, 
				_ => WindowMessageID.EntryErrorAimeFatal, 
			};
			base.Open(args);
			OpenCommonWindow(windowMessageID);
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
