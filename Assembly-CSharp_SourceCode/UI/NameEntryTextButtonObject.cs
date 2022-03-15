using IO;

namespace UI
{
	public class NameEntryTextButtonObject : CommonButtonObject
	{
		protected override void SetTrigger(string trigger)
		{
			ButtonAnimator.SetTrigger(trigger);
			if (_ledColor != 0)
			{
				_ = Initialized;
				switch (trigger)
				{
				case "In":
				case "Activated":
				case "Loop":
					MechaManager.LedIf[MonitorIndex].SetColorButton((byte)ButtonIndex, CommonButtonObject.LedColors32(_ledColor));
					break;
				case "Out":
				case "Disabled":
				case "NonActive":
				case "Pressed":
				case "Hold":
					MechaManager.LedIf[MonitorIndex].SetColorButton((byte)ButtonIndex, CommonButtonObject.LedColors32(LedColors.Black));
					break;
				}
			}
		}
	}
}
