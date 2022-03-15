using DB;
using Mai2.Mai2Cue;

namespace Monitor.Entry.Parts.Screens
{
	public class WindowGeneral : EntryScreen
	{
		public override void Open(params object[] args)
		{
			WindowMessageID windowMessageID = (WindowMessageID)args[0];
			bool num = args.Length >= 2 && (bool)args[1];
			base.Open(args);
			OpenCommonWindow(windowMessageID);
			EntryMonitor.OpenPromotion(PromotionType.None);
			if (num)
			{
				PlaySE(Cue.SE_ENTRY_AIME_ERROR);
			}
			CreateButton(ButtonType.Skip, delegate
			{
				EntryMonitor.ResponseYes();
			});
			ActivateButtons();
		}
	}
}
