namespace Manager.MaiStudio.Serialize
{
	public interface ISerialize
	{
		int GetID();

		void SetPriority(int pri);

		bool IsDisable();
	}
}
