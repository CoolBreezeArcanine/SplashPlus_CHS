namespace Manager.MaiStudio
{
	public class Config
	{
		private RomConfig _romConfig;

		private DataConfig _dataConfig;

		private SupportConfig _supportConfig;

		public Config()
		{
		}

		public Config(RomConfig rom, DataConfig data, SupportConfig support)
		{
			_romConfig = rom;
			_dataConfig = data;
		}

		public Version GetRomVersion()
		{
			if (_romConfig != null)
			{
				return _romConfig.version;
			}
			return new Version();
		}

		public Version GetDataVersion()
		{
			if (_dataConfig != null)
			{
				return _dataConfig.version;
			}
			return new Version();
		}

		public Version GetCardMakerVersion()
		{
			if (_dataConfig != null)
			{
				return _dataConfig.cardMakerVersion;
			}
			return new Version();
		}

		public bool IsSupport()
		{
			if (_supportConfig != null)
			{
				return true;
			}
			return false;
		}

		public bool IsEmulate()
		{
			return false;
		}

		public bool IsWindow()
		{
			return false;
		}
	}
}
