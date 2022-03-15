using UnityEngine;

namespace Fx
{
	public class FX_Mai2_Note_Color : FX_Controler
	{
		public enum FxColor
		{
			NoEffect,
			Good,
			Great,
			Perfect,
			White,
			Max
		}

		private static Color _whiteColor = new Color(1f, 1f, 1f, 1f);

		private static Color _perfectColor = new Color(1f, 1f, 0f, 1f);

		private static Color _greatColor = new Color(1f, 0.33f, 0.33f, 1f);

		private static Color _goodColor = new Color(0.25f, 1f, 0.25f, 1f);

		[SerializeField]
		[Header("パーティクルの色")]
		private FxColor colorType;

		private ParticleSystem.MainModule[] _particlemodules;

		private void Awake()
		{
			Initialize();
			if (_particles != null)
			{
				_particlemodules = new ParticleSystem.MainModule[_particles.Length];
				for (int i = 0; i < _particles.Length; i++)
				{
					_particlemodules[i] = _particles[i].main;
				}
			}
		}

		public void ChangeColor(FxColor colorId)
		{
			colorType = colorId;
			for (int i = 0; i < _particlemodules.Length; i++)
			{
				switch (colorType)
				{
				case FxColor.Good:
					_particlemodules[i].startColor = _goodColor;
					break;
				case FxColor.Great:
					_particlemodules[i].startColor = _greatColor;
					break;
				case FxColor.Perfect:
					_particlemodules[i].startColor = _perfectColor;
					break;
				case FxColor.White:
					_particlemodules[i].startColor = _whiteColor;
					break;
				}
			}
		}
	}
}
