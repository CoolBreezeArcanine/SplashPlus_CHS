public interface IFrameList
{
	int CurrentFrameIndex(int monitorId);

	bool IsFrameBoundary(int monitorId, int diffIndex, out int overCount);

	CollectionData GetFrameById(int monitorId, int targetId);

	CollectionData GetFrame(int monitorId, int diffIndex);

	int GetAllFrameNum(int monitorId);
}
