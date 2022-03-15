using UI.DaisyChainList;

public class EndChainList : DaisyChainList
{
	private const string END_CHAR = "终";

	public override void Deploy()
	{
		RemoveAll();
		StringChainCard chain = GetChain<StringChainCard>();
		chain.Prepare("终");
		chain.ChangeSize(isMainActive: true);
		SetSpot(4, chain);
		base.Deploy();
	}
}
