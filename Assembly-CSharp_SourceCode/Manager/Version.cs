namespace Manager
{
	public class Version
	{
		public string str;

		public int major;

		public int minor;

		public int release;

		public Version()
		{
			init();
		}

		public void init()
		{
			str = "";
			major = 0;
			minor = 0;
			release = 0;
		}
	}
}
