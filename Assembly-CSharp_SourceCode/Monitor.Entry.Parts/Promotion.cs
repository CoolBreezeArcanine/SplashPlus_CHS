using Monitor.MapCore;
using Monitor.MapCore.Component;
using UnityEngine;

namespace Monitor.Entry.Parts
{
	public class Promotion : MapBehaviour
	{
		private Animator _animator;

		private PlayendComponent _playend;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_playend = base.gameObject.AddComponent<PlayendComponent>();
		}

		public void Initialize()
		{
			base.gameObject.SetActive(value: false);
		}

		public void Open()
		{
			base.gameObject.SetActive(value: true);
			_animator.Play("In");
		}

		public void Close()
		{
			_animator.Play("Out");
			_playend.StartWatching(_animator, 0, "Out", delegate
			{
				base.gameObject.SetActive(value: false);
			});
		}
	}
}
