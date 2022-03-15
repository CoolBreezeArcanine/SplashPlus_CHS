using System;

namespace Manager.MaiStudio.Serialize
{
	[Serializable]
	public class FilePath : SerializeBase
	{
		public string path;

		public FilePath()
		{
			path = "";
		}

		public static explicit operator Manager.MaiStudio.FilePath(FilePath sz)
		{
			Manager.MaiStudio.FilePath filePath = new Manager.MaiStudio.FilePath();
			filePath.Init(sz);
			return filePath;
		}

		public override void AddPath(string parentPath)
		{
			if (!string.IsNullOrEmpty(path))
			{
				path = parentPath + path;
			}
		}
	}
}
