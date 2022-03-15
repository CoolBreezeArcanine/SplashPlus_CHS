using Manager;
using UnityEngine;

namespace Note
{
	public class KeepChecker
	{
		private float _frameStartUpdate;

		private float _frameStartJudge;

		private float _frameEnd;

		private float[] _frameQualifyList = new float[3];

		private bool _isTouch;

		private float _frameLastKeep;

		private float _frameLastCheck;

		private float _frameWorstNoKeep;

		public KeepChecker()
		{
			clear();
		}

		public void init(float length, float fP, float fGr, float fGd, float offset = 0f, float startUpdate = 0f)
		{
			clear();
			_frameStartUpdate = startUpdate + offset;
			_frameStartJudge = offset;
			_frameEnd = length + offset;
			_frameQualifyList[0] = fP;
			_frameQualifyList[1] = fGr;
			_frameQualifyList[2] = fGd;
		}

		public void init(NoteData noteData, float startUpdate = 0f)
		{
			float num = -0.5f;
			init(noteData.end.frame - noteData.time.frame + num, 3f, 6f, 9f, num, startUpdate + num);
		}

		private void clear()
		{
			_frameStartUpdate = 0f;
			_frameStartJudge = 1f;
			_frameEnd = 100f;
			_frameQualifyList[0] = 1f;
			_frameQualifyList[1] = 1f;
			_frameQualifyList[2] = 1f;
			_isTouch = false;
			_frameLastKeep = 0f;
			_frameLastCheck = 0f;
			_frameWorstNoKeep = 0f;
		}

		public int update(float curFrame, bool isKeep)
		{
			if (curFrame < _frameStartUpdate)
			{
				return -1;
			}
			_isTouch |= isKeep;
			if (curFrame > _frameEnd)
			{
				curFrame = _frameEnd;
			}
			_frameLastCheck = curFrame;
			if (isKeep)
			{
				_frameLastKeep = curFrame;
			}
			float currentNoKeepFrame = getCurrentNoKeepFrame();
			_frameWorstNoKeep = Mathf.Max(_frameWorstNoKeep, currentNoKeepFrame);
			return calcResultFromNoKeepFrame(currentNoKeepFrame);
		}

		public int getWorstJudge()
		{
			if (_frameLastCheck < _frameStartJudge)
			{
				return -1;
			}
			return calcResultFromNoKeepFrame(_frameWorstNoKeep);
		}

		public void resetWorstJudge()
		{
			_frameWorstNoKeep = getCurrentNoKeepFrame();
		}

		public float getCurrentNoKeepFrame()
		{
			return _frameLastCheck - Mathf.Max(_frameLastKeep, _frameStartJudge);
		}

		public float getWorstNoKeepFrame()
		{
			return _frameWorstNoKeep;
		}

		public int calcResultFromNoKeepFrame(float frameNoKeep)
		{
			int num = 3;
			if (!_isTouch)
			{
				return num;
			}
			for (int i = 0; i < num; i++)
			{
				if (frameNoKeep <= _frameQualifyList[i])
				{
					return i;
				}
			}
			return num;
		}
	}
}
