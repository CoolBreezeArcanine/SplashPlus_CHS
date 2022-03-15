namespace DB
{
	public class WindowMessageTableRecord
	{
		public int EnumValue;

		public string EnumName;

		public WindowKindID Kind;

		public WindowPositionID Position;

		public WindowSizeID Size;

		public string Title;

		public string TitleEx;

		public string Name;

		public string NameEx;

		public int Lifetime;

		public string FileName;

		public WindowMessageTableRecord()
		{
		}

		public WindowMessageTableRecord(int EnumValue, string EnumName, int Kind, int Position, int Size, string Title, string TitleEx, string Name, string NameEx, int Lifetime, string FileName)
		{
			this.EnumValue = EnumValue;
			this.EnumName = EnumName;
			this.Kind = (WindowKindID)Kind;
			this.Position = (WindowPositionID)Position;
			this.Size = (WindowSizeID)Size;
			this.Title = Title;
			this.TitleEx = TitleEx;
			this.Name = Name;
			this.NameEx = NameEx;
			this.Lifetime = Lifetime;
			this.FileName = FileName;
		}
	}
}
