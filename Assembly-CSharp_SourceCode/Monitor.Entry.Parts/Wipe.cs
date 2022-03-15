using System;
using System.Linq;
using Mai2.Mai2Cue;
using Mai2.Voice_000001;
using Monitor.MapCore;
using Monitor.MapCore.Component;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.Entry.Parts
{
	public class Wipe : MapBehaviour
	{
		[SerializeField]
		private GameObject _wipePrefab;

		private Animator _wipeAnimator;

		private PlayendComponent _playend;

		private DelayComponent _delay;

		public void Initialize()
		{
			_wipeAnimator = UnityEngine.Object.Instantiate(_wipePrefab, base.transform, worldPositionStays: false).GetComponentInChildren<Animator>();
			_wipeAnimator.GetComponent<CanvasGroup>().alpha = 1f;
			_playend = base.gameObject.AddComponent<PlayendComponent>();
			_delay = base.gameObject.AddComponent<DelayComponent>();
			if (base.Monitor.MonitorIndex == 1)
			{
				Vector3 localScale = base.transform.localScale;
				localScale.x *= -1f;
				base.transform.localScale = localScale;
				Image image = GetComponentsInChildren<Image>(includeInactive: true).FirstOrDefault((Image i) => i.name == "PlanetImage");
				if (image != null)
				{
					localScale = image.transform.localScale;
					localScale.x *= -1f;
					image.transform.localScale = localScale;
				}
			}
		}

		public void Show(Action onDone)
		{
			PlaySE(Mai2.Mai2Cue.Cue.SE_ENTRY_TRANSITION_01);
			PlayVoice(Mai2.Voice_000001.Cue.VO_000173);
			_wipeAnimator.Play("In");
			_playend.StartWatching(_wipeAnimator, "Base Layer", "In", onDone);
			_delay.StartDelay(2f, delegate
			{
				EntryMonitor entryMonitor = base.Monitor as EntryMonitor;
				if (entryMonitor != null)
				{
					entryMonitor.HideMachine();
				}
			});
		}
	}
}
