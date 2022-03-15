using UnityEngine;

namespace FX
{
	public class FX_CopyMat_SetColor2_Pulse : MonoBehaviour
	{
		[SerializeField]
		private string valueName = "_TintColor";

		[SerializeField]
		private Color _color1 = new Color(1f, 1f, 1f, 1f);

		[SerializeField]
		private Color _color2 = new Color(1f, 1f, 1f, 1f);

		[SerializeField]
		private int _changeFrames = 1;

		private Renderer rendererTarget = new Renderer();

		private Color incolor;

		private int switching;

		private int count = 500;

		private int _propertyID_ValueName;

		private void Start()
		{
			rendererTarget = GetComponent<Renderer>();
			_propertyID_ValueName = Shader.PropertyToID(valueName);
		}

		private void Update()
		{
			if (count >= _changeFrames)
			{
				count = 0;
				switching = ((switching == 0) ? 1 : 0);
				if (switching == 0)
				{
					rendererTarget.material.SetColor(_propertyID_ValueName, _color1);
				}
				else
				{
					rendererTarget.material.SetColor(_propertyID_ValueName, _color2);
				}
			}
			count++;
		}
	}
}
