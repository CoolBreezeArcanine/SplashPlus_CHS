public interface ICollectionMonitor
{
	bool ScrollCollectionListLeft(bool isLongTap);

	void SetScrollCard(bool isVisible);

	bool ScrollCollectionListRight(bool isLongTap);

	void ScrollCategoryRight();

	void ScrollCategoryLeft();

	void SetVisibleButton(bool isActive, params int[] indexs);

	void ChangeSubSequence(CollectionProcess.SubSequence subSequence);
}
