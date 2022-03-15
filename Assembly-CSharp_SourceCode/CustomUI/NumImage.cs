using System;
using UnityEngine;
using UnityEngine.UI;

namespace CustomUI
{
	[DisallowMultipleComponent]
	public class NumImage : NumImageBase
	{
		[SerializeField]
		private double counter_;

		public const int MaxDecimalDigits = 5;

		[SerializeField]
		[Range(1f, 5f)]
		private int decimalNumDigits_ = 1;

		[SerializeField]
		private Vector2 decimalScale_ = Vector2.one;

		[SerializeField]
		private float decimalCharSpacing_;

		[SerializeField]
		private float decimalYOffset_;

		[SerializeField]
		private float decimalDotYOffset_;

		[SerializeField]
		private float decimalDotSidePadding_;

		public double Counter
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

		public int CounterAsInt
		{
			get
			{
				return (int)counter_;
			}
			set
			{
				Counter = value;
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

		protected void calcNumFiguresFloat(double value)
		{
			numFigures_ = 0;
			double num = ((value < 0.0) ? (0.0 - value) : value);
			int num2 = (int)num;
			int num3 = (int)Math.Ceiling((num - (double)num2) * (double)Mathf.Pow(10f, decimalNumDigits_ + 1));
			if (num3 == 0 && !base.DecimalZeroPadding)
			{
				pushFigure(0);
			}
			else
			{
				num3 /= 10;
				bool flag = false;
				for (int i = 0; i < decimalNumDigits_; i++)
				{
					int num4 = num3 / 10;
					int num5 = num3 - num4 * 10;
					if (!flag && num5 != 0)
					{
						flag = true;
					}
					if (base.DecimalZeroPadding || flag)
					{
						pushFigure((byte)num5);
					}
					num3 = num4;
				}
			}
			pushFigure(13);
			int num6 = 0;
			bool flag2 = base.isDispCamma;
			bool flag3 = false;
			if (num2 == 0)
			{
				if (base.ZeroPadding && base.PaddingIncludeZero)
				{
					flag3 = true;
					pushFigure(16);
				}
				pushFigure(0);
				num6 = 1;
			}
			else
			{
				int num7 = num2;
				while (0 < num7)
				{
					int num8 = num7 / 10;
					int num9 = num7 - num8 * 10;
					pushFigure((byte)num9);
					num7 = num8;
					num6++;
					if (flag2 && num6 % 3 == 0 && 0 < num7)
					{
						pushFigure(11);
					}
				}
			}
			bool flag4 = false;
			if (base.ZeroPadding && (num6 < numDigits_ || (base.PaddingIncludeZero && num6 <= numDigits_)))
			{
				flag4 = true;
				if (!flag3)
				{
					pushFigure(16);
				}
				int num10 = numDigits_ - num6 - 1;
				while (0 <= num10)
				{
					pushFigure(0);
					num6++;
					if (flag2 && num6 % 3 == 0 && 0 < num10)
					{
						pushFigure(11);
					}
					num10--;
				}
			}
			if (value < 0.0)
			{
				pushFigure(12);
			}
			else if (base.isDispPlus)
			{
				if (num < 1E-10)
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
			if (flag4)
			{
				pushFigure(15);
			}
		}

		private Vector2 calcTotalSize()
		{
			Vector2 vector = size_;
			Vector2 result = new Vector2(0f, Mathf.Max(vector.y, signSize_.y));
			float num = charSpacing_;
			int num2 = numFigures_ - 1;
			while (0 <= num2)
			{
				switch (figures_[num2])
				{
				case 10:
				case 12:
					result.x += signSize_.x;
					result.x += num;
					break;
				case 13:
					result.x += signSize_.x;
					result.x += 2f * decimalDotSidePadding_;
					vector = Vector2.Scale(decimalScale_, size_);
					if (!base.DecimalUseIntegerSpacing)
					{
						num = decimalCharSpacing_;
					}
					result.x += num;
					break;
				case 11:
					result.x += signSize_.x;
					result.x += 2f * cammaSidePadding_;
					result.x += num;
					break;
				default:
					result.x += vector.x;
					if (0 < num2)
					{
						result.x += num;
					}
					break;
				case 15:
				case 16:
					break;
				}
				num2--;
			}
			return result;
		}

		private void updatePosition()
		{
			if (!(null == base.sprite))
			{
				if (base.isDecimal)
				{
					calcNumFiguresFloat(counter_);
				}
				else
				{
					calcNumFiguresInt((int)counter_);
				}
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
			float num7 = 0f;
			Color color = this.color;
			bool zeropadding = false;
			for (int num8 = numFigures_ - 1; 0 <= num8; num8--)
			{
				float num9 = 0f;
				float y;
				float y2;
				switch (figures_[num8])
				{
				case 10:
				case 12:
					num7 = num + signSize_.x;
					y = signSize_.y + num4;
					y2 = num4;
					break;
				case 13:
					num += decimalDotSidePadding_;
					num9 = decimalDotSidePadding_;
					num7 = num + signSize_.x;
					y = signSize_.y + num4;
					y2 = num4 + decimalDotYOffset_;
					vector3 = Vector2.Scale(decimalScale_, size_);
					num5 = decimalYOffset_;
					if (!base.DecimalUseIntegerSpacing)
					{
						num6 = decimalCharSpacing_;
					}
					break;
				case 11:
					num += cammaSidePadding_;
					num9 = cammaSidePadding_;
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
					num7 = num + vector3.x;
					y = vector3.y + num3 + num5;
					y2 = num3 + num5;
					break;
				}
				Vector4 vector4 = NumImageBase.toLocalUV(noPaddingOuterUV, NumImageBase.getNormalizedUV(figures_[num8], zeropadding), iw, ih);
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
				num = num7 + num6 + num9;
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
