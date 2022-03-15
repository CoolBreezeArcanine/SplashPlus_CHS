using UnityEngine;
using UnityEngine.UI;

namespace CustomUI
{
	[DisallowMultipleComponent]
	public class NumImageInt : NumImageBase
	{
		[SerializeField]
		private int counter_;

		public const int MaxUpperDigits = 4;

		[SerializeField]
		[Range(1f, 4f)]
		private int upperNumDigits_ = 1;

		[SerializeField]
		private Vector2 upperScale_ = Vector2.one;

		[SerializeField]
		private float upperCharSpacing_;

		[SerializeField]
		private float upperYOffset_;

		[SerializeField]
		private float upperLowerSpace_;

		public int Counter
		{
			get
			{
				return counter_;
			}
			set
			{
				if (value != counter_)
				{
					counter_ = value;
					updatePosition();
					SetVerticesDirty();
				}
			}
		}

		public void setForceDirty()
		{
			updatePosition();
			SetVerticesDirty();
		}

		protected void calcNumFiguresInt(int value)
		{
			numFigures_ = 0;
			int num = 0;
			bool flag = base.isDispCamma;
			if (value == 0)
			{
				pushFigure(0);
				num = 1;
			}
			else
			{
				int num2 = Mathf.Abs(value);
				while (0 < num2)
				{
					int num3 = num2 / 10;
					int num4 = num2 - num3 * 10;
					pushFigure((byte)num4);
					num2 = num3;
					num++;
					if (flag && num % 3 == 0 && 0 < num2)
					{
						pushFigure(11);
					}
				}
			}
			bool flag2 = false;
			if (base.ZeroPadding && num < numDigits_)
			{
				flag2 = true;
				pushFigure(16);
				int num5 = numDigits_ - num - 1;
				while (0 <= num5)
				{
					pushFigure(0);
					num++;
					if (flag && num % 3 == 0 && 0 < num5)
					{
						pushFigure(11);
					}
					num5--;
				}
			}
			if (value < 0)
			{
				pushFigure(12);
			}
			else if (base.isDispPlus)
			{
				if (value == 0)
				{
					if (!base.isEraseZeroSign)
					{
						pushFigure(10);
					}
				}
				else
				{
					pushFigure(10);
				}
			}
			if (flag2)
			{
				pushFigure(15);
			}
		}

		private Vector2 calcTotalSize()
		{
			float num = Mathf.Max(size_.y, signSize_.y);
			Vector2 vector;
			float num2;
			if (base.UseUpper)
			{
				vector = upperScale_;
				num2 = (base.UpperUseIntegerSpacing ? charSpacing_ : upperCharSpacing_);
				num = Mathf.Max(upperScale_.y, num);
			}
			else
			{
				vector = size_;
				num2 = charSpacing_;
			}
			Vector2 result = new Vector2(0f, num);
			int num3 = 0;
			int num4 = numFigures_ - 1;
			while (0 <= num4)
			{
				switch (figures_[num4])
				{
				case 10:
				case 12:
					result.x += signSize_.x;
					result.x += num2;
					break;
				case 11:
					result.x += signSize_.x;
					result.x += 2f * cammaSidePadding_;
					result.x += num2;
					break;
				default:
					if (base.UseUpper)
					{
						if (num3 == upperNumDigits_)
						{
							vector = size_;
							num2 = charSpacing_;
							result.x += upperLowerSpace_;
						}
						num3++;
					}
					result.x += vector.x;
					if (0 < num4)
					{
						result.x += num2;
					}
					break;
				case 15:
				case 16:
					break;
				}
				num4--;
			}
			return result;
		}

		private void updatePosition()
		{
			if (!(null == base.sprite))
			{
				calcNumFiguresInt(counter_);
				Rect rect = base.rectTransform.rect;
				Vector2 anchorPivot = NumImageBase.getAnchorPivot(align_);
				Vector2 b = calcTotalSize();
				Vector2 vector = Vector2.Scale(base.rectTransform.pivot, rect.size) - rect.size * 0.5f;
				Vector2 vector2 = Vector2.Scale(anchorPivot, b);
				vector += vector2;
				float num = b.x - vector.x;
				if ((bool)rightObject_)
				{
					float x = 0.5f * rightObject_.rect.width + num + rightOffset_;
					rightObject_.anchoredPosition = new Vector2(x, rightObject_.anchoredPosition.y);
				}
				if ((bool)leftObject_)
				{
					float x2 = -0.5f * leftObject_.rect.width - vector.x - leftOffset_;
					leftObject_.anchoredPosition = new Vector2(x2, leftObject_.anchoredPosition.y);
				}
			}
		}

		protected override void OnPopulateMesh(VertexHelper toFill)
		{
			toFill.Clear();
			if (null == base.sprite)
			{
				return;
			}
			Rect rect = base.rectTransform.rect;
			Vector2 anchorPivot = NumImageBase.getAnchorPivot(align_);
			Vector2 b = calcTotalSize();
			Vector2 vector = Vector2.Scale(base.rectTransform.pivot, rect.size) - rect.size * 0.5f;
			Vector2 vector2 = Vector2.Scale(anchorPivot, b);
			vector += vector2;
			float iw = 1f / (float)base.sprite.texture.width;
			float ih = 1f / (float)base.sprite.texture.height;
			Vector4 noPaddingOuterUV = NumImageBase.GetNoPaddingOuterUV(base.sprite, iw, ih);
			float num = 0f - vector.x;
			float num2 = size_.y - signSize_.y;
			float num3;
			float num4;
			if (0f <= num2)
			{
				num3 = 0f - vector.y;
				num4 = 0f - vector.y + 0.5f * num2;
			}
			else
			{
				num3 = 0f - vector.y - 0.5f * num2;
				num4 = 0f - vector.y;
			}
			Vector2 vector3 = size_;
			float num5 = 0f;
			float num6 = charSpacing_;
			if (base.UseUpper)
			{
				vector3 = upperScale_;
				num5 = upperYOffset_;
				num6 = (base.UpperUseIntegerSpacing ? charSpacing_ : upperCharSpacing_);
			}
			else
			{
				vector3 = size_;
				num5 = 0f;
				num6 = charSpacing_;
			}
			float num7 = 0f;
			Color color = this.color;
			bool zeropadding = false;
			int num8 = 0;
			for (int num9 = numFigures_ - 1; 0 <= num9; num9--)
			{
				float num10 = 0f;
				float y;
				float y2;
				switch (figures_[num9])
				{
				case 10:
				case 12:
					num7 = num + signSize_.x;
					y = signSize_.y + num4;
					y2 = num4;
					break;
				case 11:
					num += cammaSidePadding_;
					num10 = cammaSidePadding_;
					num7 = num + signSize_.x;
					y = signSize_.y + num4 + cammaYOffset_;
					y2 = num4 + cammaYOffset_;
					break;
				case 14:
					num = num + vector3.x + num6;
					continue;
				case 15:
					zeropadding = base.ZeroPaddingOtherUV;
					continue;
				case 16:
					zeropadding = false;
					continue;
				default:
					if (base.UseUpper)
					{
						if (num8 == upperNumDigits_)
						{
							vector3 = size_;
							num6 = charSpacing_;
							num5 = 0f;
							num += upperLowerSpace_;
						}
						num8++;
					}
					num7 = num + vector3.x;
					y = vector3.y + num3 + num5;
					y2 = num3 + num5;
					break;
				}
				Vector4 vector4 = NumImageBase.toLocalUV(noPaddingOuterUV, NumImageBase.getNormalizedUV(figures_[num9], zeropadding), iw, ih);
				tempVertices_[0].position = new Vector3(num, y);
				tempVertices_[0].uv0 = new Vector2(vector4.x, vector4.w);
				tempVertices_[0].color = color;
				tempVertices_[1].position = new Vector3(num7, y);
				tempVertices_[1].uv0 = new Vector2(vector4.z, vector4.w);
				tempVertices_[1].color = color;
				tempVertices_[2].position = new Vector3(num7, y2);
				tempVertices_[2].uv0 = new Vector2(vector4.z, vector4.y);
				tempVertices_[2].color = color;
				tempVertices_[3].position = new Vector3(num, y2);
				tempVertices_[3].uv0 = new Vector2(vector4.x, vector4.y);
				tempVertices_[3].color = color;
				toFill.AddUIVertexQuad(tempVertices_);
				num = num7 + num6 + num10;
			}
		}

		protected override void Start()
		{
			resetSprite();
			updatePosition();
			base.Start();
		}
	}
}
