namespace Note
{
	public class FrameArea
	{
		public float st;

		public float ed;

		public FrameArea()
		{
			st = 0f;
			ed = 0f;
		}

		public void set(float s, float e)
		{
			st = s;
			ed = e;
		}

		public bool isStart(float frame)
		{
			return frame >= st;
		}

		public bool isEnd(float frame)
		{
			return frame >= ed;
		}

		public bool isIn(float frame)
		{
			if (isStart(frame))
			{
				return !isEnd(frame);
			}
			return false;
		}

		public bool isOut(float frame)
		{
			if (isStart(frame))
			{
				return isEnd(frame);
			}
			return true;
		}
	}
}
