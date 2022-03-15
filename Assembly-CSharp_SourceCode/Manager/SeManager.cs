using System.Collections.Generic;
using Mai2.Mai2Cue;
using MAI2.Util;

namespace Manager
{
	public class SeManager : Singleton<SeManager>
	{
		private readonly Dictionary<Cue, SoundManager.PlayerID>[] _cache = new Dictionary<Cue, SoundManager.PlayerID>[2]
		{
			new Dictionary<Cue, SoundManager.PlayerID>(),
			new Dictionary<Cue, SoundManager.PlayerID>()
		};

		public void Initialize()
		{
		}

		public void PlaySE(int cueIndex, int target)
		{
			PlaySE((Cue)cueIndex, target);
		}

		public void PlaySE(Cue cueIndex, int target)
		{
			switch (target)
			{
			case 0:
			case 1:
				PlaySE_Impl(cueIndex, target);
				break;
			case 2:
				PlaySE_Impl(cueIndex, 0);
				PlaySE_Impl(cueIndex, 1);
				break;
			}
		}

		private void PlaySE_Impl(Cue cue, int index)
		{
			if (cue != (Cue)(-1))
			{
				if (_cache[index].TryGetValue(cue, out var value))
				{
					SoundManager.StopSE(value);
				}
				_cache[index][cue] = SoundManager.PlaySE(cue, index);
			}
		}
	}
}
