using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class CustomTypewriter : UIBehaviour
{
	private string _message;

	[SerializeField]
	[Range(0f, 1f)]
	private float _interval = 0.125f;

	[SerializeField]
	private bool _isPositionAnime;

	[SerializeField]
	private AnimationCurve _positionCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

	[SerializeField]
	private bool _isScaleAnime;

	[SerializeField]
	private AnimationCurve _scaleCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	[SerializeField]
	private bool _isRotatAnime;

	[SerializeField]
	private AnimationCurve _rotatoinCurve = AnimationCurve.Linear(0f, 0f, 1f, 0f);

	[SerializeField]
	private bool _isColorAnime;

	[SerializeField]
	private Gradient _colorGradient;

	private readonly TMP_Text _tmpText;

	private readonly RectTransform _rectTransform;

	private TextMoveEffect _textMoveEffect;

	private Coroutine _coroutine;

	private float _animationTime;

	private bool _isAnimation;

	public TMP_Text TmpText => _tmpText ?? GetComponent<TMP_Text>();

	public RectTransform RectTransform => _rectTransform ?? GetComponent<RectTransform>();

	protected override void Awake()
	{
		_animationTime = 0f;
		_textMoveEffect = GetComponent<TextMoveEffect>();
		_message = TmpText.text;
		TmpText.text = string.Empty;
		base.Awake();
	}

	private void Update()
	{
		UpdateAnimation();
	}

	public void StartTypewriter()
	{
		if (_coroutine == null)
		{
			_coroutine = StartCoroutine(Show());
		}
	}

	public void StartTypewriter(string text)
	{
		ResetTypewirter();
		_message = text;
		StartTypewriter();
	}

	public void ResetTypewirter()
	{
		if (_coroutine != null)
		{
			StopCoroutine(_coroutine);
			_coroutine = null;
		}
		TmpText.text = string.Empty;
	}

	private void UpdateAnimation()
	{
		if (!_isAnimation)
		{
			return;
		}
		UpdateMesh();
		_animationTime += Time.deltaTime;
		if (_animationTime > _interval)
		{
			_animationTime = 1f;
			UpdateMesh();
			_animationTime = 0f;
			_isAnimation = false;
			if (_textMoveEffect != null)
			{
				_textMoveEffect.IsTypewriter = false;
			}
			TmpText.ForceMeshUpdate();
		}
	}

	public void UpdateMesh()
	{
		if (!(_interval <= 0f))
		{
			UpdateMesh(_animationTime / _interval);
		}
	}

	public void UpdateTypewriter(float rate)
	{
		TmpText.text = GetCurrentText(rate);
		UpdateMesh(rate);
	}

	public void SetMessage(string message)
	{
		_message = message;
	}

	private string GetCurrentText(float rate)
	{
		string text = "";
		if (_message == null)
		{
			return "";
		}
		int num = (int)((float)_message.Length * rate);
		for (int i = 0; i < num; i++)
		{
			text += _message[i];
		}
		return text;
	}

	public void UpdateMesh(float rate)
	{
		if (_interval <= 0f)
		{
			return;
		}
		if (!_textMoveEffect || !_textMoveEffect.enabled)
		{
			TmpText.ForceMeshUpdate();
		}
		TMP_TextInfo textInfo = TmpText.textInfo;
		int num = textInfo.characterCount - 1;
		if (num < 0)
		{
			return;
		}
		int vertexIndex = textInfo.characterInfo[num].vertexIndex;
		if (num <= 0 || vertexIndex != 0)
		{
			int materialReferenceIndex = textInfo.characterInfo[num].materialReferenceIndex;
			Vector3[] vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
			float y = 0f;
			float num2 = 1f;
			float z = 0f;
			if (_isPositionAnime)
			{
				y = _positionCurve.Evaluate(rate) * 10f;
			}
			if (_isScaleAnime)
			{
				num2 = _scaleCurve.Evaluate(rate);
			}
			if (_isRotatAnime)
			{
				z = _rotatoinCurve.Evaluate(rate);
			}
			if (_isColorAnime)
			{
				Color32 color = _colorGradient.Evaluate(rate);
				Color32[] colors = textInfo.meshInfo[materialReferenceIndex].colors32;
				colors[vertexIndex] = color;
				colors[vertexIndex + 1] = color;
				colors[vertexIndex + 2] = color;
				colors[vertexIndex + 3] = color;
			}
			float y2 = textInfo.characterInfo[num].baseLine;
			int lineCount = textInfo.lineCount;
			float lineHeight = textInfo.lineInfo[lineCount - 1].lineHeight;
			switch (TmpText.alignment)
			{
			case TextAlignmentOptions.TopLeft:
			case TextAlignmentOptions.Top:
			case TextAlignmentOptions.TopRight:
			case TextAlignmentOptions.TopJustified:
			case TextAlignmentOptions.TopFlush:
			case TextAlignmentOptions.TopGeoAligned:
				y2 = RectTransform.sizeDelta.y / 2f - TmpText.margin.y - lineHeight / 2f - lineHeight * (float)(lineCount - 1);
				break;
			case TextAlignmentOptions.Left:
			case TextAlignmentOptions.Center:
			case TextAlignmentOptions.Right:
			case TextAlignmentOptions.MidlineLeft:
			case TextAlignmentOptions.Midline:
			case TextAlignmentOptions.MidlineRight:
			case TextAlignmentOptions.MidlineJustified:
			case TextAlignmentOptions.MidlineFlush:
			case TextAlignmentOptions.MidlineGeoAligned:
				y2 = lineHeight / 2f * (float)(-(lineCount - 1));
				break;
			case TextAlignmentOptions.BottomLeft:
			case TextAlignmentOptions.Bottom:
			case TextAlignmentOptions.BottomRight:
			case TextAlignmentOptions.BottomJustified:
			case TextAlignmentOptions.BottomFlush:
			case TextAlignmentOptions.BottomGeoAligned:
				y2 = 0f - (RectTransform.sizeDelta.y / 2f - TmpText.margin.w) + lineHeight / 2f;
				break;
			}
			Vector3 vector = new Vector2((vertices[vertexIndex].x + vertices[vertexIndex + 2].x) / 2f, y2);
			Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(0f, y, 0f), Quaternion.Euler(new Vector3(0f, 0f, z)), new Vector3(num2, num2, 1f));
			for (int i = 0; i < 4; i++)
			{
				vertices[vertexIndex + i] += -vector;
				vertices[vertexIndex + i] = matrix4x.MultiplyPoint3x4(vertices[vertexIndex + i]);
				vertices[vertexIndex + i] += vector;
			}
			if ((bool)_textMoveEffect && _textMoveEffect.enabled)
			{
				_textMoveEffect.TextMove(vertexIndex, ref vertices);
			}
			TmpText.UpdateVertexData();
		}
	}

	private IEnumerator Show()
	{
		TmpText.text = "";
		string message = _message;
		foreach (char c in message)
		{
			TmpText.text += c;
			_isAnimation = true;
			if (_textMoveEffect != null)
			{
				_textMoveEffect.IsTypewriter = true;
			}
			UpdateAnimation();
			yield return new WaitForSeconds(_interval + 0.1f);
		}
		_coroutine = null;
	}
}
