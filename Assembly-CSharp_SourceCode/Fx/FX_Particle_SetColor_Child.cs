using UnityEngine;

namespace FX
{
	public class FX_Particle_SetColor_Child : MonoBehaviour
	{
		[SerializeField]
		private Color color = new Color(1f, 1f, 1f, 1f);

		private void Start()
		{
			GetAllChildren.MapComponentsInChildren(base.transform, delegate(ParticleSystem particle)
			{
				if (!(null == particle))
				{
					ParticleSystem.MainModule main = particle.main;
					main.startColor = color;
				}
			});
		}
	}
}
