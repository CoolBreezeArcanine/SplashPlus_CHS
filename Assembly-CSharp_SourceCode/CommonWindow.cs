using DB;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommonWindow : MonoBehaviour
{
	public enum WindowState
	{
		Idle,
		Prepare,
		StartAnim,
		LifeCounting,
		EndAnim
	}

	private const string MESSAGE_WINDOW_DIRECTORY_PATH = "Process/Generic/Sprites/";

	protected const string TYPE_SPRITE_PREFIX = "UI_WND_Type_";

	protected const string TYPE_TITLE_SPRITE_PREFIX = "UI_WND_Title_Type_";

	private static readonly Color AttentionColor = new Color(1f, 1f, 1f, 1f);

	private static readonly Color NormalColor = new Color(0f, 0f, 0f, 1f);

	private const float MIN_VERTICAL_PADDING_L = 60f;

	private const float MIN_WINDOW_WIDTH = 650f;

	private const float MIN_WINDOW_HEIGHT = 124f;

	private const int BORDER_LINE_NUM = 3;

	private static readonly Vector2Int NormalVerticalPadding = new Vector2Int(32, 42);

	private static readonly Vector2Int WideVerticalPadding = new Vector2Int(42, 52);

	private static readonly Vector2 PivotUpper = new Vector2(0.5f, 0.5f);

	private static readonly Vector2 PivotMiddle = new Vector2(0.5f, 0.5f);

	private static readonly Vector2 PivotLower = new Vector2(0.5f, 0.5f);

	[SerializeField]
	protected Image _titleImage;

	[SerializeField]
	protected Image _windowImage;

	[SerializeField]
	[Header("タイトルテキスト")]
	protected TextMeshProUGUI _titleText;

	[SerializeField]
	[Header("メッセージ本体")]
	protected TextMeshProUGUI _messageText;

	[SerializeField]
	private VerticalLayoutGroup _verticalLayoutGroup;

	[SerializeField]
	private HorizontalLayoutGroup _horizontalLayoutGroup;

	[SerializeField]
	[Tooltip("ウィンドウ用")]
	private CanvasGroup _windowGroup;

	[SerializeField]
	[Tooltip("タイトルグループ")]
	protected CanvasGroup _titleGroup;

	[SerializeField]
	[Header("個別パーツ")]
	private Image _iconImage;

	protected WindowState _state;

	private WindowPositionID _positionID;

	protected float LifeTime;

	protected float _lifeCounter;

	protected bool _isOpening;

	private const float START_ANIM_TIME = 350f;

	private const float END_ANIM_TIME = 350f;

	private readonly Vector3 ADJUST_POS = new Vector3(0f, 50f, 0f);

	private Vector3 _startPos = Vector3.zero;

	private Vector3 _endPos = Vector3.zero;

	private float _animationCounter;

	public WindowPositionID PositionID => _positionID;

	public bool IsOpening => _isOpening;

	public void Prepare(IMessageMonitor monitor, WindowMessageID id, WindowPositionID positionId, Vector3 position)
	{
		SetSprites(monitor, id.GetKind(), id.GetSize(), id.GetFileName());
		CommonPrepare(id, positionId);
		SetPosition(position);
	}

	public void Close()
	{
		if (_isOpening && _state == WindowState.LifeCounting)
		{
			_state = WindowState.EndAnim;
			_startPos = base.transform.localPosition;
			_endPos = base.transform.localPosition - ADJUST_POS;
			_lifeCounter = 0f;
		}
	}

	public void ForcedClose()
	{
		_state = WindowState.EndAnim;
		_startPos = base.transform.localPosition;
		_endPos = base.transform.localPosition - ADJUST_POS;
		_lifeCounter = 0f;
	}

	protected void CommonPrepare(WindowMessageID id, WindowPositionID positionId)
	{
		SetPivot(positionId);
		string text = id.GetName();
		_messageText.text = text;
		_messageText.color = ((id.GetKind() == WindowKindID.Attention) ? AttentionColor : NormalColor);
		if (id.GetSize() == WindowSizeID.LargeHorizontal || id.GetSize() == WindowSizeID.LargeVertical)
		{
			SetPadding();
		}
		else
		{
			SetHeight();
		}
		_titleGroup.alpha = ((!(id.GetTitle() == "")) ? 1 : 0);
		_titleText.text = id.GetTitle();
		_lifeCounter = 0f;
		LifeTime = id.GetLifetime();
		_state = WindowState.Prepare;
		_positionID = positionId;
		_isOpening = true;
	}

	private bool IsOverLineNum(string message)
	{
		int num = 0;
		for (int i = 0; i < message.Length; i++)
		{
			if (message[i].ToString() == "\n")
			{
				num++;
			}
			if (3 <= num)
			{
				return true;
			}
		}
		return false;
	}

	protected void SetPadding()
	{
		float preferredWidth = _messageText.preferredWidth;
		if (preferredWidth < 650f)
		{
			float num = 650f - preferredWidth;
			if (_horizontalLayoutGroup != null)
			{
				num -= _horizontalLayoutGroup.spacing;
				if (_iconImage != null)
				{
					num -= _iconImage.preferredWidth;
				}
			}
			int num2 = Mathf.FloorToInt(num * 0.5f);
			if ((float)num2 < 60f)
			{
				num2 = 60;
			}
			SetPadding(num2);
		}
		else
		{
			SetPadding(60);
		}
	}

	protected void SetHeight()
	{
		float preferredHeight = _messageText.preferredHeight;
		float num = NormalVerticalPadding.x + NormalVerticalPadding.y;
		float y = Mathf.Max(preferredHeight + num, 124f);
		_windowImage.rectTransform.sizeDelta = new Vector2(650f, y);
	}

	private void SetPadding(int side)
	{
		Vector2Int vector2Int = NormalVerticalPadding;
		if (IsOverLineNum(_messageText.text))
		{
			vector2Int = WideVerticalPadding;
		}
		if (_verticalLayoutGroup != null)
		{
			_verticalLayoutGroup.padding = new RectOffset(side, side, vector2Int.x, vector2Int.y);
		}
		if (_horizontalLayoutGroup != null)
		{
			_horizontalLayoutGroup.padding = new RectOffset(side, side, vector2Int.x, vector2Int.y);
		}
	}

	private void SetSprites(IMessageMonitor monitor, WindowKindID kind, WindowSizeID sizeId, string fileName)
	{
		switch (kind)
		{
		case WindowKindID.Attention:
			_windowImage.sprite = monitor.AttentionWindow;
			_titleImage.sprite = monitor.AttentionTitle;
			break;
		case WindowKindID.Common:
			_windowImage.sprite = monitor.DefaultWindow;
			_titleImage.sprite = monitor.DefaultTitle;
			break;
		}
		if (sizeId == WindowSizeID.LargeHorizontal || sizeId == WindowSizeID.LargeVertical)
		{
			Sprite iconSprite = Resources.Load<Sprite>("Process/Generic/Sprites/" + fileName);
			SetIconSprite(iconSprite);
		}
	}

	protected void SetPivot(WindowPositionID positionId)
	{
		switch (positionId)
		{
		case WindowPositionID.Upper:
			_titleImage.rectTransform.pivot = PivotUpper;
			break;
		case WindowPositionID.Middle:
			_titleImage.rectTransform.pivot = PivotMiddle;
			break;
		case WindowPositionID.Lower:
			_titleImage.rectTransform.pivot = PivotLower;
			break;
		}
	}

	protected void SetIconSprite(Sprite sprite)
	{
		if (_iconImage != null)
		{
			_iconImage.sprite = sprite;
			_iconImage.SetNativeSize();
		}
	}

	protected void SetPosition(Vector3 position)
	{
		base.transform.localPosition = position;
	}

	public void Interrupt()
	{
		_state = WindowState.Idle;
		_lifeCounter = 0f;
		_isOpening = false;
		SetVisible(isActive: false);
	}

	public void SetVisible(bool isActive)
	{
		_windowGroup.alpha = (isActive ? 1 : 0);
		if (_titleGroup != null && _titleText.text != "")
		{
			_titleGroup.alpha = (isActive ? 1 : 0);
		}
	}

	public void UpdateView(long gameMSec)
	{
		switch (_state)
		{
		case WindowState.Prepare:
			PrepareAnimation();
			break;
		case WindowState.StartAnim:
			StartAnimation(gameMSec);
			break;
		case WindowState.LifeCounting:
			CountLifeTime(gameMSec);
			break;
		case WindowState.EndAnim:
			EndAnimation(gameMSec);
			break;
		case WindowState.Idle:
			break;
		}
	}

	private void PrepareAnimation()
	{
		SetColor(0f);
		base.transform.localPosition -= ADJUST_POS;
		_startPos = base.transform.localPosition;
		_endPos = base.transform.localPosition + ADJUST_POS;
		_animationCounter = 0f;
		_state = WindowState.StartAnim;
	}

	private void CountLifeTime(long gameMSec)
	{
		if (LifeTime != -1f)
		{
			if (LifeTime < _lifeCounter)
			{
				_lifeCounter = 0f;
				_state = WindowState.EndAnim;
				_startPos = base.transform.localPosition;
				_endPos = base.transform.localPosition - ADJUST_POS;
			}
			_lifeCounter += gameMSec;
		}
	}

	private void StartAnimation(long gameMSec)
	{
		float num = _animationCounter / 350f;
		base.transform.localPosition = CalculatePosition(_startPos, _endPos, num);
		SetColor(num);
		if (350f <= _animationCounter)
		{
			_state = WindowState.LifeCounting;
			_isOpening = true;
			_animationCounter = 0f;
			ResetPosition(_endPos);
			SetColor(1f);
		}
		_animationCounter += gameMSec;
	}

	private void ResetPosition(Vector3 position)
	{
		base.transform.localPosition = position;
	}

	private void SetColor(float weight)
	{
		_windowGroup.alpha = weight;
		if (_titleText.text != "")
		{
			_titleGroup.alpha = weight;
		}
	}

	private void EndAnimation(long gameMSec)
	{
		float num = _animationCounter / 350f;
		base.transform.localPosition = CalculatePosition(_startPos, _endPos, num);
		SetColor(1f - num);
		if (350f <= _animationCounter)
		{
			Refresh();
			_state = WindowState.Idle;
		}
		_animationCounter += gameMSec;
	}

	private void Refresh()
	{
		SetVisible(isActive: false);
		ResetPosition(_startPos);
		_isOpening = false;
		_animationCounter = 0f;
	}

	private Vector3 CalculatePosition(Vector3 start, Vector3 end, float weight)
	{
		float x = CalculateCurrentValue(start.x, end.x, weight);
		float y = CalculateCurrentValue(start.y, end.y, weight);
		float z = CalculateCurrentValue(start.z, end.z, weight);
		return new Vector3(x, y, z);
	}

	private float CalculateCurrentValue(float start, float end, float weight)
	{
		return start * (1f - weight) + end * weight;
	}
}
