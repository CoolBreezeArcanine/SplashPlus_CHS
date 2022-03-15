using Monitor.MapCore;
using UnityEngine;

namespace Monitor.Entry.Parts
{
	public class PromotionManager : MapBehaviour
	{
		[SerializeField]
		private Promotion[] _promotions;

		private Promotion _current;

		public void Initialize()
		{
			Promotion[] promotions = _promotions;
			for (int i = 0; i < promotions.Length; i++)
			{
				promotions[i]?.Initialize();
			}
		}

		public void Open(PromotionType type)
		{
		}

		public void Close()
		{
			_current?.Close();
			_current = null;
		}
	}
}
