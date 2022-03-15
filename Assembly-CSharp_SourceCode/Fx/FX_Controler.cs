using UnityEngine;

namespace Fx
{
	public class FX_Controler : MonoBehaviour
	{
		protected GameObject _particleRoot;

		protected ParticleSystem[] _particles;

		private void Awake()
		{
			Initialize();
		}

		protected void Initialize()
		{
			_particleRoot = base.transform.Find("Root").gameObject;
			if (_particleRoot != null)
			{
				_particles = _particleRoot.GetComponentsInChildren<ParticleSystem>();
			}
			SetTransform(base.transform.position);
			Stop();
		}

		public void SetTransform(Vector3 position)
		{
			if (_particleRoot != null)
			{
				base.transform.position = position;
			}
		}

		public void Play()
		{
			if (_particleRoot != null)
			{
				_particleRoot.SetActive(value: false);
				_particleRoot.SetActive(value: true);
			}
			ParticleSystem[] particles = _particles;
			foreach (ParticleSystem obj in particles)
			{
				obj.Clear();
				obj.Simulate(0f);
				obj.Play();
			}
		}

		public void Stop()
		{
			if (_particleRoot != null)
			{
				_particleRoot.SetActive(value: false);
			}
			ParticleSystem[] particles = _particles;
			for (int i = 0; i < particles.Length; i++)
			{
				particles[i].Stop();
			}
		}

		private void OnEnable()
		{
			Stop();
		}

		public bool IsStop()
		{
			return !_particleRoot.activeSelf;
		}

		private void Update()
		{
			if (!(_particleRoot != null))
			{
				return;
			}
			bool flag = true;
			ParticleSystem[] particles = _particles;
			for (int i = 0; i < particles.Length; i++)
			{
				if (particles[i].isPlaying)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				_particleRoot.SetActive(value: false);
			}
		}
	}
}
