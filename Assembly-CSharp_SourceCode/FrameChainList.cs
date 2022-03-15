using UI.DaisyChainList;
using UnityEngine;

public class FrameChainList : DaisyChainList
{
	private IPhotoShootProcess _process;

	private int _monitorId;

	public virtual void AdvancedInitialize(IPhotoShootProcess process, int monitorId)
	{
		_process = process;
		_monitorId = monitorId;
	}

	private void SetChainData(FrameChainCard card, int index)
	{
		Sprite frameSpriteByAdjustIndex = _process.GetFrameSpriteByAdjustIndex(_monitorId, index);
		card.Prepare(frameSpriteByAdjustIndex);
	}

	public override void Deploy()
	{
		RemoveAll();
		int num = -4;
		for (int i = 0; i < 9; i++)
		{
			FrameChainCard chain = GetChain<FrameChainCard>();
			SetChainData(chain, num);
			chain.ChangeSize(i == 4);
			SetSpot(i, chain);
			num++;
		}
		Positioning(isImmediate: true, isAnimation: true);
		base.IsListEnable = true;
	}

	protected override void Next(int targetIndex, Direction direction)
	{
		int index = ((direction != Direction.Right) ? 8 : 0);
		FrameChainCard chain = GetChain<FrameChainCard>();
		SetChainData(chain, targetIndex);
		chain.ChangeSize(isMainActive: false);
		SetSpot(index, chain);
	}

	public override void SetScrollCard(bool isVisible)
	{
		if (isVisible)
		{
			FrameChainCard frameChainCard = (FrameChainCard)ScrollChainCard;
			frameChainCard.ChangeSize(isMainActive: true);
			SetChainData(frameChainCard, 0);
		}
		base.SetScrollCard(isVisible);
	}
}
