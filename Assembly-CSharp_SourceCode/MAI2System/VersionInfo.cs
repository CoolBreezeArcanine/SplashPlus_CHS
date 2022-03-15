using System.Runtime.InteropServices;

namespace MAI2System
{
	public class VersionInfo
	{
		private const int ProjectNameLength = 63;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
		private string _projectName = "";

		private VersionNo _versionNo;

		public string projectName => _projectName;

		public VersionNo versionNo => _versionNo;

		public void set(string projectName, byte majorNo, byte minorNo, byte releaseNo)
		{
			_projectName = projectName;
			_versionNo.set(majorNo, minorNo, releaseNo);
		}

		public void set(string projectName, VersionNo versionNo)
		{
			_projectName = projectName;
			_versionNo = versionNo;
		}
	}
}
