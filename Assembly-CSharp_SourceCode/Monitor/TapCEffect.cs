using Fx;
using MAI2.Util;
using Manager;
using UnityEngine;

namespace Monitor
{
	public class TapCEffect : MonoBehaviour
	{
		private GameObject _particleRootObject;

		private FX_Mai2_Note_Color _particleControler;

		private int initializeCounter;

		public void Intialize()
		{
			base.gameObject.SetActive(value: true);
			_particleRootObject.SetActive(value: true);
			_particleControler?.Play();
		}

		public void SetUpParticle(int monitorIndex)
		{
			_particleRootObject = Object.Instantiate(GameParticleContainer.CenterBackEffect[(int)Singleton<GamePlayManager>.Instance.GetGameScore(monitorIndex).UserOption.TapDesign], base.transform);
			_particleRootObject.SetActive(value: false);
			_particleControler = _particleRootObject.GetComponent<FX_Mai2_Note_Color>();
			_particleControler?.Play();
		}

		public void PreLoad()
		{
			base.gameObject.SetActive(value: true);
			_particleControler?.Play();
		}

		public void Execute()
		{
			if (_particleControler.IsStop())
			{
				base.gameObject.SetActive(value: false);
				_particleRootObject.SetActive(value: false);
			}
		}

		public bool IsStop()
		{
			initializeCounter++;
			if (initializeCounter > 10)
			{
				_particleControler.Stop();
			}
			return _particleControler.IsStop();
		}

		public void Stop()
		{
			base.gameObject.SetActive(value: false);
			_particleControler.Stop();
		}
	}
}
