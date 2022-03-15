namespace Manager
{
	public class NotesTotal
	{
		public uint[] _totalData = new uint[46];

		public NotesTotal()
		{
			clear();
		}

		public void clear()
		{
			for (int i = 0; i < 46; i++)
			{
				_totalData[i] = 0u;
			}
		}
	}
}
