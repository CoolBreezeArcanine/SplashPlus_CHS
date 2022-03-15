using System.Collections.Generic;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager;

namespace Process
{
	public class PhotoManager
	{
		public readonly List<PhotoTiming> _pictureTimingList = new List<PhotoTiming>();

		private int _takeIndex;

		public void Initialize(List<long> takeTimeList)
		{
			_takeIndex = 0;
			for (int i = 0; i < takeTimeList.Count; i++)
			{
				_pictureTimingList.Add(new PhotoTiming(takeTimeList[i]));
			}
		}

		public void Update(long now)
		{
			if (_takeIndex >= _pictureTimingList.Count)
			{
				return;
			}
			PhotoTiming photoTiming = _pictureTimingList[_takeIndex];
			if (photoTiming.TakeTime >= now || photoTiming.Took || !GameManager.IsPhotoAgree || !WebCamManager.IsAvailableCamera() || !WebCamManager.TakeGamePicture(_takeIndex))
			{
				return;
			}
			photoTiming.Took = true;
			_takeIndex++;
			for (int i = 0; i < 2; i++)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(i).IsEntry)
				{
					SoundManager.PlaySE(Cue.SE_CAMERA, i);
				}
			}
		}
	}
}
