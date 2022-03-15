using UnityEngine;

namespace Sxc.Unity.App
{
	public static class UIShaderPropertyID
	{
		public static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");

		public static readonly int DstBlend = Shader.PropertyToID("_DstBlend");

		public static readonly int Color = Shader.PropertyToID("_Color");

		public static readonly int On = Shader.PropertyToID("_GradationOn");
	}
}
