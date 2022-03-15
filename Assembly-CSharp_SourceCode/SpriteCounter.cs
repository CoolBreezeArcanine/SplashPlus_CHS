using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class SpriteCounter : MaskableGraphic
{
	private readonly StringBuilder sb = new StringBuilder();

	private readonly UIVertex[] tempArray = new UIVertex[4];

	[SerializeField]
	private Sprite[] spriteSheet;

	[SerializeField]
	private string mainText;

	[SerializeField]
	private string formatText;

	[SerializeField]
	private List<SpriteCounterData> frameList = new List<SpriteCounterData>();

	[SerializeField]
	private List<SpriteCounterBetweenData> betweenList = new List<SpriteCounterBetweenData>();

	[SerializeField]
	private float _posYMagnification = 10f;

	[SerializeField]
	private float _scaleMagnification = 1f;

	private RectTransform getRectTransform;

	public float Timer;

	public List<SpriteCounterData> FrameList => frameList;

	public List<SpriteCounterBetweenData> BetweenList => betweenList;

	private RectTransform GetRectTransform
	{
		get
		{
			if (!(getRectTransform != null))
			{
				return getRectTransform = GetComponent<RectTransform>();
			}
			return getRectTransform;
		}
	}

	public override Texture mainTexture
	{
		get
		{
			if (spriteSheet != null && spriteSheet.Length != 0 && spriteSheet[0] != null)
			{
				return spriteSheet[0].texture;
			}
			return base.mainTexture;
		}
	}

	public void SetSpriteSheet(Sprite[] sprites)
	{
		spriteSheet = sprites;
		SetAllDirty();
	}

	private void Update()
	{
		for (int i = 0; i < frameList.Count; i++)
		{
			if (frameList[i].IsAnimated)
			{
				float num = frameList[i].PosYCurve.Evaluate(Timer / 60f);
				frameList[i].AnimationYPos = num * _posYMagnification;
				float num2 = frameList[i].ScaleCurve.Evaluate(Timer / 60f);
				frameList[i].AnimationScale = num2 * _scaleMagnification;
			}
			else
			{
				frameList[i].AnimationYPos = 0f;
				frameList[i].AnimationScale = 0f;
			}
		}
	}

	public void AddFormatFream()
	{
		if (spriteSheet != null && spriteSheet.Length != 0)
		{
			Sprite sprite = spriteSheet[0];
			Vector4 drawingDimensions = GetDrawingDimensions(sprite);
			Vector4 outerUV = DataUtility.GetOuterUV(sprite);
			SpriteCounterData spriteCounterData = new SpriteCounterData
			{
				Text = "0"
			};
			spriteCounterData.UiVertexs[0] = new CustomUIVertex
			{
				Position = new Vector3(drawingDimensions.x, drawingDimensions.y),
				Color = color,
				UV = new Vector2(outerUV.x, outerUV.y)
			};
			spriteCounterData.UiVertexs[1] = new CustomUIVertex
			{
				Position = new Vector3(drawingDimensions.x, drawingDimensions.w),
				Color = color,
				UV = new Vector2(outerUV.x, outerUV.w)
			};
			spriteCounterData.UiVertexs[2] = new CustomUIVertex
			{
				Position = new Vector3(drawingDimensions.z, drawingDimensions.w),
				Color = color,
				UV = new Vector2(outerUV.z, outerUV.w)
			};
			spriteCounterData.UiVertexs[3] = new CustomUIVertex
			{
				Position = new Vector3(drawingDimensions.z, drawingDimensions.y),
				Color = color,
				UV = new Vector2(outerUV.z, outerUV.y)
			};
			spriteCounterData.DefaultScale = sprite.rect.size;
			spriteCounterData.Scale = 1f;
			frameList.Add(spriteCounterData);
			if (frameList.Count > 1)
			{
				betweenList.Add(new SpriteCounterBetweenData
				{
					Size = new Vector2(sprite.rect.size.x, 0f)
				});
			}
			GetRectTransform.sizeDelta = new Vector2(frameList[0].DefaultScale.x * (float)frameList.Count, frameList[0].DefaultScale.y);
			mainText += "0";
			SetAllDirty();
		}
	}

	public void RemoveFormatFrame()
	{
		if (frameList.Count > 0)
		{
			GetRectTransform.sizeDelta = new Vector2(frameList[0].DefaultScale.x * (float)(frameList.Count - 1), frameList[0].DefaultScale.y);
			frameList.RemoveAt(frameList.Count - 1);
			if (betweenList.Count > 0)
			{
				betweenList.RemoveAt(betweenList.Count - 1);
			}
			if (mainText.Length > 0)
			{
				mainText = mainText.Remove(mainText.Length - 1);
			}
			if (formatText.Length > 0)
			{
				formatText = formatText.Remove(formatText.Length - 1);
			}
			SetAllDirty();
		}
	}

	public override Material GetModifiedMaterial(Material baseMaterial)
	{
		for (int i = 0; i < frameList.Count; i++)
		{
			for (int j = 0; j < frameList[i].UiVertexs.Length; j++)
			{
				frameList[i].UiVertexs[j].Color = color;
			}
		}
		return base.GetModifiedMaterial(baseMaterial);
	}

	public void ChangeText(string text, string format)
	{
		sb.Length = 0;
		int num = 0;
		for (int i = 0; i < format.Length; i++)
		{
			switch (format[i])
			{
			case '#':
				if (text[num] == '0')
				{
					sb.Append(" ");
				}
				else
				{
					sb.Append(text[num]);
				}
				break;
			default:
				sb.Append(text[num]);
				break;
			}
			num++;
		}
		ChangeText(sb.ToString());
	}

	public void ChangeText(string text)
	{
		int num = 0;
		foreach (char c in text)
		{
			int num2 = (int)char.GetNumericValue(c);
			bool flag = false;
			if (num2 < 0)
			{
				switch (c)
				{
				case ' ':
					num2 = 0;
					flag = true;
					break;
				case '+':
					num2 = 10;
					break;
				case '-':
					num2 = 11;
					break;
				case ',':
					num2 = 12;
					break;
				case '.':
					num2 = 13;
					break;
				case '%':
					num2 = 14;
					break;
				default:
					num2 = -1;
					return;
				}
			}
			if (spriteSheet.Length <= num2)
			{
				return;
			}
			Sprite sprite = spriteSheet[num2];
			Vector4 drawingDimensions = GetDrawingDimensions(sprite);
			Vector4 vector = DataUtility.GetOuterUV(sprite);
			if (num >= frameList.Count)
			{
				break;
			}
			frameList[num].UiVertexs[0].Position = new Vector3(drawingDimensions.x, drawingDimensions.y);
			frameList[num].UiVertexs[1].Position = new Vector3(drawingDimensions.x, drawingDimensions.w);
			frameList[num].UiVertexs[2].Position = new Vector3(drawingDimensions.z, drawingDimensions.w);
			frameList[num].UiVertexs[3].Position = new Vector3(drawingDimensions.z, drawingDimensions.y);
			if (flag)
			{
				vector = Vector4.zero;
			}
			frameList[num].UiVertexs[0].UV = new Vector2(vector.x, vector.y);
			frameList[num].UiVertexs[1].UV = new Vector2(vector.x, vector.w);
			frameList[num].UiVertexs[2].UV = new Vector2(vector.z, vector.w);
			frameList[num].UiVertexs[3].UV = new Vector2(vector.z, vector.y);
			num++;
		}
		SetAllDirty();
		mainText = text;
	}

	private static Vector4 GetDrawingDimensions(Sprite sprite)
	{
		return new Vector4(0f - sprite.rect.width / 2f, 0f - sprite.rect.height / 2f, sprite.rect.width / 2f, sprite.rect.height / 2f);
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		if (spriteSheet == null || spriteSheet.Length < 1)
		{
			return;
		}
		for (int i = 0; i < frameList.Count; i++)
		{
			Vector2 zero = Vector2.zero;
			if (frameList.Count > 1)
			{
				zero.y = 0f;
				zero.x = 0f - frameList[i].DefaultScale.x * (float)frameList.Count / 2f + frameList[i].DefaultScale.x * (float)i + frameList[i].DefaultScale.x / 2f;
			}
			for (int j = 0; j < 4; j++)
			{
				tempArray[j] = frameList[i].UiVertexs[j].Get();
				tempArray[j].position *= frameList[i].Scale + frameList[i].AnimationScale;
				tempArray[j].position.x += zero.x + frameList[i].RelativePosition.x;
				tempArray[j].position.y += zero.y + frameList[i].RelativePosition.y + frameList[i].AnimationYPos;
			}
			vh.AddUIVertexQuad(tempArray);
		}
	}

	public void SetColor(Color setColor)
	{
		color = setColor;
		for (int i = 0; i < frameList.Count; i++)
		{
			for (int j = 0; j < frameList[i].UiVertexs.Length; j++)
			{
				frameList[i].UiVertexs[j].Color = color;
			}
		}
		SetAllDirty();
	}
}
