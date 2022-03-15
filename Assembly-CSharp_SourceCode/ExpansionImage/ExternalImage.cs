using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ExpansionImage
{
	[ExecuteAlways]
	public class ExternalImage : UIBehaviour
	{
		protected Image _image;

		protected override void Awake()
		{
			base.Awake();
			if (_image != null)
			{
				_image.material = null;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			SetMaterialParamaters();
		}

		protected virtual void SetMaterialParamaters()
		{
		}

		protected override void OnDidApplyAnimationProperties()
		{
			base.OnDidApplyAnimationProperties();
			SetMaterialParamaters();
		}
	}
}
