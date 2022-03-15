using UI;

namespace Monitor.PhotoEdit.UI
{
	public class PhotoChangeButton : CommonButtonObject
	{
		public void SetPressedOutFlag(bool isVisible)
		{
			ButtonAnimator.SetBool("ToOut", isVisible);
		}
	}
}
