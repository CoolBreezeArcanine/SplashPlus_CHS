public interface ITitleList
{
	int CurrentTitleIndex(int monitorId);

	bool IsTitleBoundary(int monitorId, int diffIndex, out int overCount);

	CollectionData GetTitleById(int monitorId, int targetId);

	CollectionData GetTitle(int monitorId, int diffIndex);

	int GetAllTitleNum(int monitorId);
}
