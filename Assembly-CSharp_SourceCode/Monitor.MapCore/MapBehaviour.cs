using System;
using System.Collections.Generic;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using Manager;
using UnityEngine;

namespace Monitor.MapCore
{
	public class MapBehaviour : MonoBehaviour
	{
		private class ParticleNode
		{
			public float Speed;

			public ParticleSystem Particle;
		}

		private MapMonitor _monitor;

		protected Action<float> State;

		private readonly List<Animator> _animators = new List<Animator>();

		private readonly List<ParticleNode> _particleNodes = new List<ParticleNode>();

		protected MapMonitor Monitor => _monitor ?? (_monitor = GetComponentInParent<MapMonitor>());

		protected virtual void Start()
		{
			ConnectMonitor();
		}

		protected virtual void OnDestroy()
		{
			if (!(_monitor == null))
			{
				_monitor.RemoveEvents(this);
			}
		}

		protected void ConnectMonitor()
		{
			if (!(Monitor == null))
			{
				Monitor.AddEvents(this, OnStart, OnUpdate, OnLateUpdate);
			}
		}

		protected virtual void OnStart()
		{
		}

		protected virtual void OnUpdate(float deltaTime)
		{
			State?.Invoke(deltaTime);
		}

		protected virtual void OnLateUpdate(float deltaTime)
		{
		}

		protected void SetStateTerminate()
		{
			State = null;
		}

		protected void RegisterAnimator(Animator animator)
		{
			if (!(animator == null))
			{
				_animators.Add(animator);
			}
		}

		protected void RegisterParticle(ParticleSystem particle)
		{
			if (!(particle == null))
			{
				_particleNodes.Add(new ParticleNode
				{
					Speed = particle.main.simulationSpeed,
					Particle = particle
				});
			}
		}

		public virtual void OnChangeDeltaRatio(float ratio)
		{
			_animators.ForEach(delegate(Animator a)
			{
				a.speed = ratio;
			});
			_particleNodes.ForEach(delegate(ParticleNode p)
			{
				ParticleSystem.MainModule main = p.Particle.main;
				main.simulationSpeed = p.Speed * ratio;
			});
		}

		public UserData GetUserData()
		{
			return Singleton<UserDataManager>.Instance.GetUserData(Monitor.MonitorIndex);
		}

		protected bool GetInputDown(InputManager.ButtonSetting button, InputManager.TouchPanelArea panelArea)
		{
			return InputManager.GetInputDown(Monitor.MonitorIndex, button, panelArea);
		}

		protected bool GetInputLongPush(InputManager.ButtonSetting button, InputManager.TouchPanelArea panelArea)
		{
			return InputManager.GetInputLongPush(Monitor.MonitorIndex, button, panelArea, 1000L);
		}

		protected bool SlideAreaLr()
		{
			return InputManager.SlideAreaLr(Monitor.MonitorIndex);
		}

		protected uint SlideAreaLrLevel()
		{
			return InputManager.SlideAreaLrLevel(Monitor.MonitorIndex);
		}

		protected bool SlideAreaRr()
		{
			return InputManager.SlideAreaRl(Monitor.MonitorIndex);
		}

		protected uint SlideAreaRlLevel()
		{
			return InputManager.SlideAreaRlLevel(Monitor.MonitorIndex);
		}

		protected SoundManager.PlayerID PlaySE(Mai2.Mai2Cue.Cue cueIndex)
		{
			return SoundManager.PlaySE(cueIndex, Monitor.MonitorIndex);
		}

		protected void StopSE(SoundManager.PlayerID id)
		{
			SoundManager.StopSE(id);
		}

		protected SoundManager.PlayerID PlayVoice(Mai2.Voice_000001.Cue cueIndex)
		{
			return SoundManager.PlayVoice(cueIndex, Monitor.MonitorIndex);
		}
	}
}
