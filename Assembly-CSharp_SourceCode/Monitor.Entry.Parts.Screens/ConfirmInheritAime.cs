using DB;

namespace Monitor.Entry.Parts.Screens
{
	public class ConfirmInheritAime : ConfirmEntryAime
	{
		public override void Open(params object[] args)
		{
			base.Open(args[0], (object)WindowMessageID.EntryConfirmInheritAime, (object)PromotionType.None);
		}
	}
}
