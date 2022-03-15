using DB;
using Mai2.Voice_000001;

namespace Monitor.Entry.Parts.Screens
{
	public class ConfirmGuest : EntryScreen
	{
		public override void Open(params object[] args)
		{
			base.Open(args);
			OpenCommonWindow(WindowMessageID.EntryConfirmGuest);
			EntryMonitor.OpenPromotion(PromotionType.None);
			PlayVoice(Cue.VO_000009);
			CreateButton(ButtonType.Entry, delegate
			{
				EntryMonitor.ResponseYes();
			}, () => true);
			CreateButton(ButtonType.No, delegate
			{
				EntryMonitor.ResponseNo();
			});
			ActivateButtons();
		}
	}
}
