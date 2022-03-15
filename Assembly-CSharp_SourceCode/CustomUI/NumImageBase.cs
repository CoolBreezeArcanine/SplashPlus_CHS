using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace CustomUI
{
	public abstract class NumImageBase : Image
	{
		public enum Align
		{
			Center,
			Left,
			Right
		}

		[SerializeField]
		protected Align align_;

		[SerializeField]
		protected Sprite[] sprites_;

		[SerializeField]
		protected int spriteIndex_;

		[SerializeField]
		protected Vector2 size_ = new Vector2(50f, 50f);

		[SerializeField]
		protected Vector2 signSize_ = new Vector2(50f, 50f);

		[SerializeField]
		[Range(1f, 9f)]
		protected int numDigits_ = 1;

		[SerializeField]
		protected float charSpacing_;

		[SerializeField]
		protected float cammaYOffset_;

		[SerializeField]
		protected float cammaSidePadding_;

		[SerializeField]
		protected int flags_;

		[SerializeField]
		protected RectTransform leftObject_;

		[SerializeField]
		protected float leftOffset_;

		[SerializeField]
		protected RectTransform rightObject_;

		[SerializeField]
		protected float rightOffset_;

		public const int Flag_DispPlus = 1;

		public const int Flag_DispCamma = 2;

		public const int Flag_ZeroPadding = 4;

		public const int Flag_Decimal = 8;

		public const int Flag_DecimalZeroPadding = 16;

		public const int Flag_DecimalUseIntegerSpacing = 32;

		public const int Flag_ZeroPaddingOtherUV = 64;

		public const int Flag_EraseZeroSign = 128;

		public const int Flag_PaddingIncludeZero = 256;

		public const int Flag_Upper = 512;

		public const int Flag_UpperUseIntegerSpacing = 1024;

		public const byte FigurePlus = 10;

		public const byte FigureComma = 11;

		public const byte FigureMinus = 12;

		public const byte FigureDot = 13;

		public const byte FigureSpace = 14;

		public const byte FigureBeginZeroPadding = 15;

		public const byte FigureEndZeroPadding = 16;

		protected int numFigures_;

		protected byte[] figures_ = new byte[28];

		protected UIVertex[] tempVertices_ = new UIVertex[4];

		public int SpriteCount
		{
			get
			{
				if (sprites_ == null)
				{
					return 0;
				}
				return sprites_.Length;
			}
		}

		public int SpriteIndex
		{
			get
			{
				return spriteIndex_;
			}
			set
			{
				if (value != spriteIndex_)
				{
					spriteIndex_ = value;
					resetSprite();
				}
			}
		}

		public bool isDispPlus
		{
			get
			{
				return (flags_ & 1) != 0;
			}
			set
			{
				if (value)
				{
					flags_ |= 1;
				}
				else
				{
					flags_ &= -2;
				}
			}
		}

		public bool isDispCamma
		{
			get
			{
				return (flags_ & 2) != 0;
			}
			set
			{
				if (value)
				{
					flags_ |= 2;
				}
				else
				{
					flags_ &= -3;
				}
			}
		}

		public bool isEraseZeroSign
		{
			get
			{
				return (flags_ & 0x80) != 0;
			}
			set
			{
				if (value)
				{
					flags_ |= 128;
				}
				else
				{
					flags_ &= -129;
				}
			}
		}

		public bool ZeroPadding
		{
			get
			{
				return (flags_ & 4) != 0;
			}
			set
			{
				if (value)
				{
					flags_ |= 4;
				}
				else
				{
					flags_ &= -5;
				}
			}
		}

		public bool ZeroPaddingOtherUV
		{
			get
			{
				return (flags_ & 0x40) != 0;
			}
			set
			{
				if (value)
				{
					flags_ |= 64;
				}
				else
				{
					flags_ &= -65;
				}
			}
		}

		public bool isDecimal
		{
			get
			{
				return (flags_ & 8) != 0;
			}
			set
			{
				if (value)
				{
					flags_ |= 8;
				}
				else
				{
					flags_ &= -9;
				}
			}
		}

		public bool DecimalZeroPadding
		{
			get
			{
				return (flags_ & 0x10) != 0;
			}
			set
			{
				if (value)
				{
					flags_ |= 16;
				}
				else
				{
					flags_ &= -17;
				}
			}
		}

		public bool DecimalUseIntegerSpacing
		{
			get
			{
				return (flags_ & 0x20) != 0;
			}
			set
			{
				if (value)
				{
					flags_ |= 32;
				}
				else
				{
					flags_ &= -33;
				}
			}
		}

		public bool PaddingIncludeZero
		{
			get
			{
				return (flags_ & 0x100) != 0;
			}
			set
			{
				if (value)
				{
					flags_ |= 256;
				}
				else
				{
					flags_ &= -257;
				}
			}
		}

		public bool UseUpper
		{
			get
			{
				return (flags_ & 0x200) != 0;
			}
			set
			{
				if (value)
				{
					flags_ |= 512;
				}
				else
				{
					flags_ &= -513;
				}
			}
		}

		public bool UpperUseIntegerSpacing
		{
			get
			{
				return (flags_ & 0x400) != 0;
			}
			set
			{
				if (value)
				{
					flags_ |= 1024;
				}
				else
				{
					flags_ &= -1025;
				}
			}
		}

		public void resetSprite()
		{
			if (sprites_ != null && spriteIndex_ < sprites_.Length)
			{
				base.sprite = sprites_[spriteIndex_];
			}
		}

		public static Vector2 getAnchorPivot(Align align)
		{
			return align switch
			{
				Align.Center => new Vector2(0.5f, 0.5f), 
				Align.Left => new Vector2(0f, 0.5f), 
				Align.Right => new Vector2(1f, 0.5f), 
				_ => Vector2.zero, 
			};
		}

		public static Vector4 toLocalUV(Vector4 spriteUV, Vector4 normalizedUV, float iw, float ih)
		{
			float num = spriteUV.z - spriteUV.x;
			float num2 = spriteUV.w - spriteUV.y;
			float num3 = 0.5f * iw;
			float b = 1f - num3;
			float num4 = 0.5f * ih;
			float b2 = 1f - num4;
			Vector4 result = default(Vector4);
			result.x = Mathf.Max(spriteUV.x + num * normalizedUV.x, num3);
			result.y = Mathf.Max(spriteUV.y + num2 * normalizedUV.y, num4);
			result.z = Mathf.Min(spriteUV.x + num * normalizedUV.z, b);
			result.w = Mathf.Min(spriteUV.y + num2 * normalizedUV.w, b2);
			return result;
		}

		public static Vector4 GetNoPaddingOuterUV(Sprite sprite, float iw, float ih)
		{
			Rect textureRect = sprite.textureRect;
			Vector4 padding = DataUtility.GetPadding(sprite);
			Vector4 result = default(Vector4);
			result.x = (textureRect.xMin - padding.x) * iw;
			result.y = (textureRect.yMin - padding.y) * ih;
			result.z = (textureRect.xMax + padding.z) * iw;
			result.w = (textureRect.yMax + padding.w) * ih;
			return result;
		}

		public static Vector4 getNormalizedUV(byte figure, bool zeropadding)
		{
			Vector4 result = default(Vector4);
			switch (figure)
			{
			case 10:
				result = new Vector4(0.5f, 0.25f, 0.75f, 0.5f);
				break;
			case 12:
				result = new Vector4(0.75f, 0.25f, 1f, 0.5f);
				break;
			case 13:
				result = new Vector4(0f, 0f, 0.25f, 0.25f);
				break;
			case 11:
				result = new Vector4(0.25f, 0f, 0.5f, 0.25f);
				break;
			case 14:
				return Vector4.zero;
			default:
			{
				if (zeropadding)
				{
					result = new Vector4(0.75f, 0f, 1f, 0.25f);
					break;
				}
				int num = figure >> 2;
				int num2 = figure - (num << 2);
				result.x = 0.25f * (float)num2;
				result.z = result.x + 0.25f;
				result.w = 1f - 0.25f * (float)num;
				result.y = result.w - 0.25f;
				break;
			}
			}
			return result;
		}

		protected void pushFigure(byte c)
		{
			figures_[numFigures_] = c;
			numFigures_++;
		}
	}
}
