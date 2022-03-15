public interface IIconList
{
	int CurrentIconIndex(int monitorId);

	bool IsIconBoundary(int monitorId, int diffIndex, out int overCount);

	CollectionData GetIconById(int monitorId, int targetId);

	CollectionData GetIcon(int monitorId, int diffIndex);

	int GetAllIconNum(int monitorId);
}
