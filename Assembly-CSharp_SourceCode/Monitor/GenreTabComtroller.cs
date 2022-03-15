using TMPro;
using UI.Common;
using UnityEngine;

namespace Monitor
{
	public class GenreTabComtroller : TabControllerBase
	{
		[SerializeField]
		[Header("ベースのスプライト")]
		private Sprite _baseSprite;

		[SerializeField]
		[Header("ベースのカラー")]
		private Color _baseColor;

		[SerializeField]
		[Header("タブのnullポインタ座標リスト")]
		private Transform[] _leftTabPositions;

		[SerializeField]
		private Transform[] _rightTabPositions;

		[SerializeField]
		[Header("ジャンル名テキスト")]
		private TextMeshProUGUI _genreName;

		[SerializeField]
		[Header("ボタンのnullポインタ座標")]
		private Transform _leftButtonPositions;

		[SerializeField]
		private Transform _rightButtonPositions;

		private void Start()
		{
		}

		private void Update()
		{
		}
	}
}
