using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class FilePath : AccessorBase
	{
		public string path { get; private set; }

		public FilePath()
		{
			path = "";
		}

		public void Init(Manager.MaiStudio.Serialize.FilePath sz)
		{
			path = sz.path;
		}
	}
}
