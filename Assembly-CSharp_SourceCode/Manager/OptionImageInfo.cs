using System;

namespace Manager
{
	public class OptionImageInfo
	{
		public DateTime CreationTime { get; private set; }

		public string Name { get; private set; }

		public OptionImageInfo(DateTime creationTime, string name)
		{
			CreationTime = creationTime;
			Name = name;
		}
	}
}
