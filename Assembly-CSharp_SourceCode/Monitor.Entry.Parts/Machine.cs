using Monitor.MapCore;
using UnityEngine;

namespace Monitor.Entry.Parts
{
	public class Machine : MapBehaviour
	{
		[SerializeField]
		private Satellite[] _satellites;

		private SatelliteEntryType[] _types;

		private Animator _animator;

		public bool IsShow { get; private set; }

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_types = new SatelliteEntryType[_satellites.Length];
		}

		public void Show(int index, SatelliteEntryType type, string text)
		{
			IsShow = true;
			_animator.Play("In");
			_satellites[index].Open(type, text);
			_types[index] = type;
		}

		public void Hide()
		{
			_animator.Play("Out");
			SetStateTerminate();
		}

		public bool IsSatellitesLoopState()
		{
			if (_satellites[0].IsLoopState())
			{
				return _satellites[1].IsLoopState();
			}
			return false;
		}

		public void SyncSatellitesLoopAnimation()
		{
			_satellites[0].SyncLoopAnimation();
			_satellites[1].SyncLoopAnimation();
		}
	}
}
