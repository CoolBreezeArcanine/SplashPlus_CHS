using UnityEngine;

namespace FX
{
	public class FX_Mai2_Tap_Color : MonoBehaviour
	{
		private static Color _perfectColor = new Color(1f, 1f, 0f, 1f);

		private static Color _greatColor = new Color(1f, 0.33f, 0.33f, 1f);

		private static Color _goodColor = new Color(0.25f, 1f, 0.25f, 1f);

		private static Color _defaultColor = new Color(1f, 1f, 1f, 1f);

		[SerializeField]
		[Range(0f, 3f)]
		private int switchGrade;

		private void Start()
		{
			GetAllChildren.MapComponentsInChildren(base.transform, delegate(ParticleSystem particle)
			{
				if (!(null == particle))
				{
					ParticleSystem.MainModule main = particle.main;
					switch (switchGrade)
					{
					case 0:
						main.startColor = _goodColor;
						break;
					case 1:
						main.startColor = _greatColor;
						break;
					case 2:
						main.startColor = _perfectColor;
						break;
					default:
						main.startColor = _defaultColor;
						break;
					}
				}
			});
		}
	}
}
