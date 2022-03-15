namespace DB
{
	public class JvsButtonTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public string Name;

		public int JvsPlayer;

		public string InputIDName;

		public KeyCodeID SubstituteKey;

		public int Invert;

		public int System;

		public JvsButtonTableRecord()
		{
		}

		public JvsButtonTableRecord(int EnumValue, string EnumName, string Name, int JvsPlayer, string InputIDName, int SubstituteKey, int Invert, int System)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Name = Name;
			this.JvsPlayer = JvsPlayer;
			this.InputIDName = InputIDName;
			this.SubstituteKey = (KeyCodeID)SubstituteKey;
			this.Invert = Invert;
			this.System = System;
		}
	}
}
