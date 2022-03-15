using UnityEngine;

namespace FX
{
	public class FX_CopyMat_SetColor : MonoBehaviour
	{
		[SerializeField]
		private string valueName = "_TintColor";

		[SerializeField]
		private string valueName2 = "_Color";

		[SerializeField]
		private Color color = new Color(1f, 1f, 1f, 1f);

		private void Start()
		{
			Renderer component = GetComponent<Renderer>();
			if (component != null && component.material != null)
			{
				component.material.SetColor(valueName, color);
				component.material.SetColor(valueName2, color);
			}
		}
	}
}
