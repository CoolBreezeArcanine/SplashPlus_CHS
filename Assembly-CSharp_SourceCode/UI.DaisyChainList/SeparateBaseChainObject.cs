namespace UI.DaisyChainList
{
	public abstract class SeparateBaseChainObject : ChainObject
	{
		public abstract void SetData(string left, string right);

		public abstract void SetTitle(string left, string right);

		public abstract void SetVisible(bool isVisible, Direction direction);

		public abstract void SetScrollDirection(Direction direction);
	}
}
