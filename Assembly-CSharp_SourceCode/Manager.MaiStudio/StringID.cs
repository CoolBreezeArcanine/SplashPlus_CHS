using Manager.MaiStudio.Serialize;

namespace Manager.MaiStudio
{
	public class StringID : AccessorBase
	{
		public const int InvalidID = -1;

		public int id { get; private set; }

		public string str { get; private set; }

		public StringID()
		{
			Clear();
		}

		public void Init(Manager.MaiStudio.Serialize.StringID sz)
		{
			id = sz.id;
			str = sz.str;
		}

		private void Clear()
		{
			id = -1;
			str = "";
		}

		public bool IsValid()
		{
			bool result = false;
			if (id > -1)
			{
				result = true;
			}
			return result;
		}
	}
}
