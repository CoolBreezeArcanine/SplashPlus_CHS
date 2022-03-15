namespace Manager
{
	public interface IBackup
	{
		object getRecord();

		bool getDirty();

		void resetDirty();

		bool verify();

		void clear();
	}
}
