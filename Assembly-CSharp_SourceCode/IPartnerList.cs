public interface IPartnerList
{
	int CurrentPartnerIndex(int monitorId);

	bool IsPartnerBoundary(int monitorId, int diffIndex, out int overCount);

	CollectionData GetPartnerById(int monitorId, int targetId);

	CollectionData GetPartner(int monitorId, int diffIndex);

	int GetAllPartnerNum(int monitorId);
}
