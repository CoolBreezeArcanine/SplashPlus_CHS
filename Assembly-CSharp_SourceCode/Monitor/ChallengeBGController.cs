using Monitor.MusicSelect.OtherParts;
using UnityEngine;

namespace Monitor
{
	public class ChallengeBGController : MonoBehaviour
	{
		[SerializeField]
		[Header("オリジナルデコレーション背景データ")]
		private ChallengeDecoBG _originalDecoBGObj;

		[SerializeField]
		[Header("デコレーション背景のnullポインタ座標")]
		private Transform _decoBGPosition;

		private ChallengeDecoBG _decoBGObj;

		public ChallengeDecoBG DecoObj => _decoBGObj;

		public void Initialize()
		{
			_decoBGObj = Object.Instantiate(_originalDecoBGObj, _decoBGPosition);
			_decoBGObj.gameObject.SetActive(value: false);
		}
	}
}
