using UnityEngine;

namespace FX
{
	public class FX_CopyMat_SetColor_Child : MonoBehaviour
	{
		[SerializeField]
		private string valueName = "_TintColor";

		[SerializeField]
		private string valueName2 = "_Color";

		[SerializeField]
		private Color color = new Color(1f, 1f, 1f, 1f);

		private void Start()
		{
			int propertyID_ValueName1 = Shader.PropertyToID(valueName);
			int propertyID_ValueName2 = Shader.PropertyToID(valueName2);
			GetAllChildren.MapComponentsInChildren(base.transform, delegate(Renderer renderer)
			{
				if (!(null == renderer))
				{
					Material[] materials = renderer.materials;
					for (int i = 0; i < materials.Length; i++)
					{
						materials[i].SetColor(propertyID_ValueName1, color);
						materials[i].SetColor(propertyID_ValueName2, color);
					}
					renderer.materials = materials;
				}
			});
		}
	}
}
