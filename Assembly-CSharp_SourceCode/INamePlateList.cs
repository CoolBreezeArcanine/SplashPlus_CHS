public interface INamePlateList
{
	int CurrentPlateIndex(int monitorId);

	bool IsPlateBoundary(int monitorId, int diffIndex, out int overCount);

	CollectionData GetPlateById(int monitorId, int targetId);

	CollectionData GetPlate(int monitorId, int diffIndex);

	int GetAllPlateNum(int monitorId);
}
