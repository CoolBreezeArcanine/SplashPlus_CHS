using UI.DaisyChainList;

public class StringChainList : DaisyChainList
{
	private INameEntryProcess _process;

	private int _monitorId;

	public virtual void AdvancedInitialize(INameEntryProcess process, int monitorId)
	{
		_process = process;
		_monitorId = monitorId;
	}

	private void SetChainData(StringChainCard card, int index)
	{
		string stringByAdjustIndex = _process.GetStringByAdjustIndex(_monitorId, index);
		card.Prepare(stringByAdjustIndex);
	}

	public override void Deploy()
	{
		RemoveAll();
		int num = -4;
		for (int i = 0; i < 9; i++)
		{
			StringChainCard chain = GetChain<StringChainCard>();
			SetChainData(chain, num);
			chain.ChangeSize(i == 4);
			SetSpot(i, chain);
			num++;
		}
		base.Deploy();
	}

	public void DeployEndOnly()
	{
		RemoveAll();
		int index = -4;
		StringChainCard chain = GetChain<StringChainCard>();
		SetChainData(chain, index);
		chain.ChangeSize(isMainActive: true);
		SetSpot(0, chain);
		base.Deploy();
	}

	protected override void Next(int targetIndex, Direction direction)
	{
		int index = ((direction != Direction.Right) ? 8 : 0);
		StringChainCard chain = GetChain<StringChainCard>();
		SetChainData(chain, targetIndex);
		chain.ChangeSize(isMainActive: false);
		SetSpot(index, chain);
	}

	public override void SetScrollCard(bool isVisible)
	{
		if (isVisible)
		{
			StringChainCard stringChainCard = (StringChainCard)ScrollChainCard;
			stringChainCard.ChangeSize(isMainActive: true);
			SetChainData(stringChainCard, 0);
		}
		base.SetScrollCard(isVisible);
	}
}
