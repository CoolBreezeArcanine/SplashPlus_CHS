using UnityEngine;

namespace ExpansionImage
{
	public class MaskingImage : ExpansionImage
	{
		[SerializeField]
		private float _whScale = 1f;

		private float _prefScale = 1f;

		protected override void SetMaterialParamaters()
		{
			base.SetMaterialParamaters();
			if (base.ExpansionSprite != null)
			{
				material.EnableKeyword("_MODE_MASK");
				material.DisableKeyword("_MODE_DECAL");
				material.DisableKeyword("_MODE_DEBUG");
			}
		}

		public void UVScaleCalculate()
		{
			Rect uV1Scale = base.UV1Scale;
			float num = base.rectTransform.rect.width * _whScale;
			float num2 = base.rectTransform.rect.height * _whScale;
			uV1Scale.width = base.rectTransform.rect.width / num;
			uV1Scale.height = base.rectTransform.rect.height / num2;
			uV1Scale.x = (0f - (base.rectTransform.rect.width / 2f - num / 2f)) / num;
			uV1Scale.y = (0f - (base.rectTransform.rect.height / 2f - num2 / 2f)) / num2;
			base.UV1Scale = uV1Scale;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
			float num = base.rectTransform.rect.width / base.UV1Scale.width;
			float num2 = base.rectTransform.rect.height / base.UV1Scale.height;
			Vector3 vector = new Vector3((0f - base.rectTransform.rect.width) / 2f - base.UV1Scale.x * num + num / 2f, (0f - base.rectTransform.rect.height) / 2f - base.UV1Scale.y * num2 + num2 / 2f, 0f);
			Gizmos.DrawCube(base.transform.position + vector, new Vector3(num, num2, 100f));
		}

		protected override void OnDidApplyAnimationProperties()
		{
			base.OnDidApplyAnimationProperties();
			if (_prefScale != _whScale)
			{
				UVScaleCalculate();
				_prefScale = _whScale;
			}
		}
	}
}
