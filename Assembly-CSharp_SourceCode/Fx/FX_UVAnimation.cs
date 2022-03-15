using UnityEngine;

namespace FX
{
	public class FX_UVAnimation : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("アニメーション開始時のUVオフセット")]
		private Vector2 _startUVOffset;

		[SerializeField]
		[Tooltip("アニメーション終了時のUVオフセット")]
		private Vector2 _goalUVOffset;

		[SerializeField]
		[Tooltip("アニメーション開始までのウェイト（秒数）")]
		private float _timeToStart;

		[SerializeField]
		[Tooltip("アニメーション開始から終了までの長さ（秒数）")]
		private float _timeToGoal;

		private Vector2 resultUV;

		private Material thisMaterial;

		private float timeCount;

		private float rate;

		public void Start()
		{
			thisMaterial = GetComponent<Renderer>().material;
		}

		public void Update()
		{
			rate = (timeCount - _timeToStart) / _timeToGoal;
			rate = Mathf.Clamp(rate, 0f, 1f);
			resultUV = _startUVOffset + (_goalUVOffset - _startUVOffset) * rate;
			thisMaterial.SetTextureOffset("_MainTex", resultUV);
			timeCount += Time.deltaTime;
		}
	}
}
