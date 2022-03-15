using System;

namespace Net.VO.Mai2
{
	[Serializable]
	public class GameSettingResponseVO : VOSerializer
	{
		public bool isAouAccession;

		public GameSetting gameSetting;
	}
}
