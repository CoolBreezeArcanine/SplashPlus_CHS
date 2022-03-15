using IO;
using UnityEngine;

namespace DB
{
	public static class LedBlock
	{
		public static void SetColor(this LedBlockID self, Color color)
		{
			if (!self.IsValid())
			{
				return;
			}
			if (self.IsJvs())
			{
				if (self.GetLedbdID() == -1)
				{
					MechaManager.Jvs?.SetPwmOutput((byte)self.GetPlayerindex(), color);
				}
				else
				{
					MechaManager.Jvs?.SetOutput((JvsOutputID)self.GetLedbdID(), color.r > 0f);
				}
			}
			else if (self.IsFet())
			{
				MechaManager.LedIf[self.GetPlayerindex()].SetColorFet((byte)self.GetLedbdID(), (byte)(color.r * 255f));
			}
			else
			{
				MechaManager.LedIf[self.GetPlayerindex()].SetColor((byte)self.GetLedbdID(), color);
			}
		}
	}
}
