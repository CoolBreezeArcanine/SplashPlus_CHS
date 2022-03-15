using System.Collections.Generic;
using AMDaemon;
using MAI2;

namespace Manager
{
	public class Delivery
	{
		private const int OptionImageInfoCountPerPage = 15;

		private ListList<OptionImageInfo> _optionImageInfos;

		public int testModeOptionPageCount => _optionImageInfos.ListCount;

		public void initialize()
		{
			_optionImageInfos = new ListList<OptionImageInfo>();
			_optionImageInfos.AddList(new List<OptionImageInfo>());
			LazyCollection<AMDaemon.OptionImageInfo> optionInfos = AppImage.OptionInfos;
			for (int i = 0; i < AppImage.OptionCount; i++)
			{
				int num = i / 15;
				while (num >= _optionImageInfos.ListCount)
				{
					_optionImageInfos.AddList(new List<OptionImageInfo>());
				}
				OptionImageInfo item = new OptionImageInfo(optionInfos[i].CreationTime, optionInfos[i].Name);
				_optionImageInfos.GetList(num).Add(item);
			}
		}

		public void execute()
		{
		}

		public void terminate()
		{
			_optionImageInfos = null;
		}

		public List<OptionImageInfo> getTestModeOptionImageInfo(int page)
		{
			if (page >= _optionImageInfos.ListCount)
			{
				return null;
			}
			return _optionImageInfos.GetList(page);
		}
	}
}
