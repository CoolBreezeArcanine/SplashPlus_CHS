using UnityEngine;

namespace Timeline
{
	public abstract class TimeControlBaseObject : MonoBehaviour
	{
		public virtual void OnGraphStart()
		{
		}

		public virtual void OnGraphStop()
		{
		}

		public virtual void OnBehaviourPlay()
		{
		}

		public virtual void OnBehaviourPause()
		{
		}

		public virtual void OnClipPlay()
		{
		}

		public virtual void OnClipTailEnd()
		{
		}

		public virtual void OnClipHeadEnd()
		{
		}

		public virtual void PrepareFrame(double normalizeTime)
		{
		}
	}
}
