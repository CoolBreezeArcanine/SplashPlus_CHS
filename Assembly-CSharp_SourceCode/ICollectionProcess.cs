using System.Collections.Generic;
using Manager;

public interface ICollectionProcess : ICollectionGenreIDList, ITitleList, IIconList, INamePlateList, IPartnerList, IFrameList
{
	CollectionGenreID CurrentColletionType(int monitorId);

	CollectionProcess.SubSequence CurrentSubSequence(int monitorId);

	int GetTotalCollectionNum(int monitorId);

	string CategoryName(int monitorId, int diff);

	int GetCurrentCategoryMax(int monitorId);

	string CurrentElementText(int monitorId);

	int CurrentCollectionIndex(int monitorId);

	int CurrentCategoryCollectionNum(int monitorId);

	int CurrentCategoryIndex(int monitorId);

	List<CollectionTabData> GetTabDatas(int monitorId);

	void SetSubSequence(int monitorId, CollectionProcess.SubSequence subSequence);

	CollectionGenreID ConvertToCollectionGenreID(CollectionProcess.SubSequence subSequence);
}
