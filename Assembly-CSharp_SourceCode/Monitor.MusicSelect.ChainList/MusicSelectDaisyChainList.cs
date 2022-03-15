using Process;
using UI.DaisyChainList;

namespace Monitor.MusicSelect.ChainList
{
	public class MusicSelectDaisyChainList : DaisyChainList
	{
		protected AssetManager AssetManager;

		protected IMusicSelectProcess SelectProcess;

		protected int MonitorIndex;

		public virtual void AdvancedInitialize(IMusicSelectProcess selectProcess, AssetManager manager, int index)
		{
			SelectProcess = selectProcess;
			MonitorIndex = index;
			AssetManager = manager;
		}
	}
}
