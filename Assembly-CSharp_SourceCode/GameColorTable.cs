using UnityEngine;

[CreateAssetMenu(menuName = "Mai2Data/GameColorTable", fileName = "ParameterTable")]
public class GameColorTable : ScriptableObject
{
	[SerializeField]
	[Header("リザルト：達成率表示差分色")]
	private Color _riseColor;

	[SerializeField]
	private Color _declineColor;

	[SerializeField]
	[Header("ゲーム中：達成率表示差分色")]
	private Color[] _gameAchiveColor;

	[SerializeField]
	[Header("ゲーム中：ゲージ色")]
	private Color[] _gameGaugeColor = new Color[6];

	[SerializeField]
	[Header("ゲーム中：ゲージ数値色")]
	private Color[] _gameGaugeNumColor = new Color[3];

	[SerializeField]
	[Header("ゲーム中：中央表示色")]
	private Color[] _gameCenterNumColor = new Color[11];

	[SerializeField]
	[Header("おきにいり色")]
	private Color _favoriteColor;

	public Color RiseColor
	{
		get
		{
			return _riseColor;
		}
		private set
		{
		}
	}

	public Color DeclineColor
	{
		get
		{
			return _declineColor;
		}
		private set
		{
		}
	}

	public Color[] GameAchiveColor
	{
		get
		{
			return _gameAchiveColor;
		}
		private set
		{
		}
	}

	public Color[] GameGaugeColor
	{
		get
		{
			return _gameGaugeColor;
		}
		private set
		{
		}
	}

	public Color[] GameGaugeNumColor
	{
		get
		{
			return _gameGaugeNumColor;
		}
		private set
		{
		}
	}

	public Color[] GameCenterNumColor
	{
		get
		{
			return _gameCenterNumColor;
		}
		private set
		{
		}
	}

	public Color FavoriteColor
	{
		get
		{
			return _favoriteColor;
		}
		private set
		{
		}
	}
}
