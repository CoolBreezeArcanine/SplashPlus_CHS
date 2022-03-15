namespace Process.Entry.State
{
	public abstract class State
	{
		protected Context Context;

		public abstract void Init(params object[] args);

		public abstract void Exec(float deltaTime);

		public abstract void Term();

		public void Setup(Context context)
		{
			Context = context;
		}

		public override string ToString()
		{
			return GetType().Name;
		}
	}
}
