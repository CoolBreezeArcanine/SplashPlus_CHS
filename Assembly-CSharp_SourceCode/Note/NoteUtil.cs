using UnityEngine;

namespace Note
{
	internal static class NoteUtil
	{
		private const float EPSILON = 1E-06f;

		public static bool isNearZero(float src, float epsilon = 1E-06f)
		{
			return Mathf.Abs(src) < epsilon;
		}

		public static bool isLessZero(float src, float epsilon = 1E-06f)
		{
			return src < epsilon;
		}

		public static bool isMoreZero(float src, float epsilon = 1E-06f)
		{
			return src > 0f - epsilon;
		}

		public static bool isNear(float l, float r, float epsilon = 1E-06f)
		{
			return Mathf.Abs(l - r) < epsilon;
		}

		public static bool isMore(float l, float r, float epsilon = 1E-06f)
		{
			return l - r >= epsilon;
		}

		public static bool isLess(float l, float r, float epsilon = 1E-06f)
		{
			return l - r <= 0f - epsilon;
		}

		public static bool isNearMore(float l, float r, float epsilon = 1E-06f)
		{
			return l - r > 0f - epsilon;
		}

		public static bool isNearLess(float l, float r, float epsilon = 1E-06f)
		{
			return l - r < epsilon;
		}

		public static int compare(float l, float r, float epsilon = 1E-06f)
		{
			if (isLess(l, r, epsilon))
			{
				return -1;
			}
			if (isMore(l, r, epsilon))
			{
				return 1;
			}
			return 0;
		}

		public static float calcRate(float from, float to, float mid, float minimum = 0.01f)
		{
			float num;
			if (to > from)
			{
				num = Mathf.Max(to - from, minimum);
			}
			else
			{
				if (!(to < from))
				{
					return from;
				}
				num = Mathf.Min(to - from, 0f - minimum);
			}
			return (mid - from) / num;
		}

		public static float getRate(float from, float to, float rate)
		{
			return from * (1f - rate) + to * rate;
		}

		public static void safeDestroy(ref GameObject go)
		{
			if (!(go == null))
			{
				Object.Destroy(go);
				go = null;
			}
		}

		public static void setRenderQueue(GameObject go, int rq)
		{
			Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				if (!(renderer.material == null))
				{
					renderer.material.renderQueue = rq;
					rq++;
				}
			}
		}

		public static bool tempResult2Result(JudgeTiming result, out Judge judge, out Timing timing)
		{
			JudgeTimingParam judgeTimingParam = Static.judgeTimingParam[(int)result];
			judge = judgeTimingParam.result;
			timing = judgeTimingParam.timing;
			return judgeTimingParam.isFinish;
		}

		public static JudgeTiming result2TempResult(Judge judge, Timing timing)
		{
			JudgeTiming result = JudgeTiming.Begin;
			bool flag = timing == Timing.Begin;
			switch (judge)
			{
			case Judge.Perfect:
				result = JudgeTiming.Perfect;
				break;
			case Judge.Great:
				result = (flag ? JudgeTiming.FastGreat : JudgeTiming.LateGreat);
				break;
			case Judge.Good:
				result = (flag ? JudgeTiming.FastGood : JudgeTiming.LateGood);
				break;
			case Judge.Begin:
				result = JudgeTiming.TooLate;
				break;
			}
			return result;
		}

		public static bool isTempResultJudged(JudgeTiming result)
		{
			return Static.judgeTimingParam[(int)result].isFinish;
		}

		public static bool isCross(float ax, float ay, float bx, float by, float cx, float cy, float dx, float dy)
		{
			float num = (cx - dx) * (ay - cy) + (cy - dy) * (cx - ax);
			float num2 = (cx - dx) * (by - cy) + (cy - dy) * (cx - bx);
			float num3 = (ax - bx) * (cy - ay) + (ay - by) * (ax - cx);
			float num4 = (ax - bx) * (dy - ay) + (ay - by) * (ax - dx);
			if (num3 * num4 <= 0f)
			{
				return num * num2 <= 0f;
			}
			return false;
		}

		public static Color calcRateColor(float rate, Color colFrom, Color colTo)
		{
			Color result = default(Color);
			result.a = colTo.a * rate + colFrom.a * (1f - rate);
			result.r = colTo.r * rate + colFrom.r * (1f - rate);
			result.g = colTo.g * rate + colFrom.g * (1f - rate);
			result.b = colTo.b * rate + colFrom.b * (1f - rate);
			return result;
		}
	}
}
