using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace UI
{
	public class MultipleImage : Image
	{
		public enum MultipleBlendMode
		{
			None,
			Blend
		}

		private const string ShaderName = "MAI2/UI/MultiImage";

		private const string SrcPropertyName = "_SrcBlend";

		private const string DstPropertyName = "_DstBlend";

		private const string OpPropertyName = "_BlendOp";

		private static readonly Vector2[] s_VertScratch = new Vector2[4];

		private static readonly Vector2[] s_UVScratch = new Vector2[4];

		private static readonly Vector3[] s_Xy = new Vector3[4];

		private static readonly Vector3[] s_Uv = new Vector3[4];

		[SerializeField]
		private int _selectSpriteIndex;

		[SerializeField]
		private Rect _uvRect = new Rect(0f, 0f, 0f, 0f);

		[SerializeField]
		private MultiImageBlendType _blendType;

		[SerializeField]
		private MultipleBlendMode _blendMode;

		public List<Sprite> MultiSprites = new List<Sprite>();

		public MultipleBlendMode BlendMode
		{
			get
			{
				return _blendMode;
			}
			set
			{
				if (_blendMode != value)
				{
					_blendMode = value;
					CustomDirty();
				}
			}
		}

		public Rect UVRect
		{
			get
			{
				return _uvRect;
			}
			set
			{
				if (!(_uvRect == value))
				{
					_uvRect = value;
					SetVerticesDirty();
				}
			}
		}

		public MultiImageBlendType BlendType
		{
			get
			{
				return _blendType;
			}
			set
			{
				if (_blendType != value)
				{
					_blendType = value;
					UpdateParameters();
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			material = null;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			UpdateParameters();
		}

		public void SetSprites(Sprite[] sprites)
		{
			MultiSprites.Clear();
			AddSprites(sprites);
		}

		public void AddSprites(Sprite[] sprites)
		{
			for (int i = 0; i < sprites.Length; i++)
			{
				MultiSprites.Add(sprites[i]);
			}
		}

		public void UpdateParameters()
		{
			CustomDirty();
			switch (_blendType)
			{
			case MultiImageBlendType.Normal:
				material.SetInt("_SrcBlend", 5);
				material.SetInt("_DstBlend", 10);
				material.SetInt("_BlendOp", 0);
				break;
			case MultiImageBlendType.Add:
				material.SetInt("_SrcBlend", 5);
				material.SetInt("_DstBlend", 1);
				material.SetInt("_BlendOp", 0);
				break;
			case MultiImageBlendType.Subtract:
				material.SetInt("_SrcBlend", 5);
				material.SetInt("_DstBlend", 1);
				material.SetInt("_BlendOp", 2);
				break;
			}
		}

		public void CustomDirty()
		{
			switch (_blendMode)
			{
			case MultipleBlendMode.None:
				_blendType = MultiImageBlendType.Normal;
				material = null;
				break;
			case MultipleBlendMode.Blend:
				if (material == null || material.shader.name != "MAI2/UI/MultiImage")
				{
					material = new Material(Shader.Find("MAI2/UI/MultiImage"));
				}
				break;
			}
		}

		public void ChangeSprite(int index)
		{
			if (index >= 0 && MultiSprites.Count > index)
			{
				_selectSpriteIndex = index;
				base.sprite = MultiSprites[_selectSpriteIndex];
			}
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			if (base.overrideSprite == null)
			{
				base.OnPopulateMesh(vh);
				return;
			}
			switch (base.type)
			{
			case Type.Simple:
				GenerateSimpleSprite(vh, base.preserveAspect);
				break;
			case Type.Sliced:
				GenerateSlicedSprite(vh);
				break;
			case Type.Tiled:
				GenerateTiledSprite(vh);
				break;
			case Type.Filled:
				GenerateFilledSprite(vh, base.preserveAspect);
				break;
			}
		}

		private void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect)
		{
			Vector4 drawingDimensions = GetDrawingDimensions(lPreserveAspect);
			Vector4 vector = ((!(base.overrideSprite != null)) ? Vector4.zero : DataUtility.GetOuterUV(base.overrideSprite));
			Color color = this.color;
			vh.Clear();
			vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.y), color, new Vector2(vector.x + _uvRect.xMin, vector.y + _uvRect.yMin));
			vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.w), color, new Vector2(vector.x + _uvRect.xMin, vector.w + _uvRect.yMax));
			vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.w), color, new Vector2(vector.z + _uvRect.xMax, vector.w + _uvRect.yMax));
			vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.y), color, new Vector2(vector.z + _uvRect.xMax, vector.y + _uvRect.yMin));
			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}

		private void GenerateSlicedSprite(VertexHelper toFill)
		{
			if (!base.hasBorder)
			{
				GenerateSimpleSprite(toFill, lPreserveAspect: false);
				return;
			}
			Vector4 vector;
			Vector4 vector2;
			Vector4 vector3;
			Vector4 vector4;
			if (base.overrideSprite != null)
			{
				vector = DataUtility.GetOuterUV(base.overrideSprite);
				vector2 = DataUtility.GetInnerUV(base.overrideSprite);
				vector3 = DataUtility.GetPadding(base.overrideSprite);
				vector4 = base.overrideSprite.border;
			}
			else
			{
				vector = Vector4.zero;
				vector2 = Vector4.zero;
				vector3 = Vector4.zero;
				vector4 = Vector4.zero;
			}
			Rect pixelAdjustedRect = GetPixelAdjustedRect();
			Vector4 adjustedBorders = GetAdjustedBorders(vector4 / base.pixelsPerUnit, pixelAdjustedRect);
			Vector4 vector5 = vector3 / base.pixelsPerUnit;
			s_VertScratch[0] = new Vector2(vector5.x, vector5.y);
			s_VertScratch[3] = new Vector2(pixelAdjustedRect.width - vector5.z, pixelAdjustedRect.height - vector5.w);
			s_VertScratch[1].x = adjustedBorders.x;
			s_VertScratch[1].y = adjustedBorders.y;
			s_VertScratch[2].x = pixelAdjustedRect.width - adjustedBorders.z;
			s_VertScratch[2].y = pixelAdjustedRect.height - adjustedBorders.w;
			for (int i = 0; i < 4; i++)
			{
				s_VertScratch[i].x += pixelAdjustedRect.x;
				s_VertScratch[i].y += pixelAdjustedRect.y;
			}
			s_UVScratch[0] = new Vector2(vector.x, vector.y);
			s_UVScratch[1] = new Vector2(vector2.x, vector2.y);
			s_UVScratch[2] = new Vector2(vector2.z, vector2.w);
			s_UVScratch[3] = new Vector2(vector.z, vector.w);
			toFill.Clear();
			for (int j = 0; j < 3; j++)
			{
				int num = j + 1;
				for (int k = 0; k < 3; k++)
				{
					if (base.fillCenter || j != 1 || k != 1)
					{
						int num2 = k + 1;
						AddQuad(toFill, new Vector2(s_VertScratch[j].x, s_VertScratch[k].y), new Vector2(s_VertScratch[num].x, s_VertScratch[num2].y), color, new Vector2(s_UVScratch[j].x, s_UVScratch[k].y), new Vector2(s_UVScratch[num].x, s_UVScratch[num2].y));
					}
				}
			}
		}

		private void GenerateTiledSprite(VertexHelper toFill)
		{
			Vector4 vector;
			Vector4 vector2;
			Vector2 vector4;
			Vector4 vector3;
			if (base.overrideSprite != null)
			{
				vector = DataUtility.GetOuterUV(base.overrideSprite);
				vector2 = DataUtility.GetInnerUV(base.overrideSprite);
				vector3 = base.overrideSprite.border;
				vector4 = base.overrideSprite.rect.size;
			}
			else
			{
				vector = Vector4.zero;
				vector2 = Vector4.zero;
				vector3 = Vector4.zero;
				vector4 = Vector2.one * 100f;
			}
			Rect pixelAdjustedRect = GetPixelAdjustedRect();
			float num = (vector4.x - vector3.x - vector3.z) / base.pixelsPerUnit;
			float num2 = (vector4.y - vector3.y - vector3.w) / base.pixelsPerUnit;
			vector3 = GetAdjustedBorders(vector3 / base.pixelsPerUnit, pixelAdjustedRect);
			vector += new Vector4(UVRect.xMin, UVRect.yMin, UVRect.xMax, UVRect.yMax);
			vector2 += new Vector4(UVRect.xMin, UVRect.yMin, UVRect.xMax, UVRect.yMax);
			Vector2 uvMin = new Vector2(vector2.x, vector2.y);
			Vector2 vector5 = new Vector2(vector2.z, vector2.w);
			UIVertex.simpleVert.color = color;
			float x = vector3.x;
			float num3 = pixelAdjustedRect.width - vector3.z;
			float y = vector3.y;
			float num4 = pixelAdjustedRect.height - vector3.w;
			toFill.Clear();
			Vector2 uvMax = vector5;
			if ((double)num == 0.0)
			{
				num = num3 - x;
			}
			if ((double)num2 == 0.0)
			{
				num2 = num4 - y;
			}
			if (base.fillCenter)
			{
				for (float num5 = y; (double)num5 < (double)num4; num5 += num2)
				{
					float num6 = num5 + num2;
					if ((double)num6 > (double)num4)
					{
						uvMax.y = uvMin.y + (float)(((double)vector5.y - (double)uvMin.y) * ((double)num4 - (double)num5) / ((double)num6 - (double)num5));
						num6 = num4;
					}
					uvMax.x = vector5.x;
					for (float num7 = x; (double)num7 < (double)num3; num7 += num)
					{
						float num8 = num7 + num;
						if ((double)num8 > (double)num3)
						{
							uvMax.x = uvMin.x + (float)(((double)vector5.x - (double)uvMin.x) * ((double)num3 - (double)num7) / ((double)num8 - (double)num7));
							num8 = num3;
						}
						AddQuad(toFill, new Vector2(num7, num5) + pixelAdjustedRect.position, new Vector2(num8, num6) + pixelAdjustedRect.position, color, uvMin, uvMax);
					}
				}
			}
			if (!base.hasBorder)
			{
				return;
			}
			Vector2 vector6 = vector5;
			for (float num9 = y; (double)num9 < (double)num4; num9 += num2)
			{
				float num10 = num9 + num2;
				if ((double)num10 > (double)num4)
				{
					vector6.y = uvMin.y + (float)(((double)vector5.y - (double)uvMin.y) * ((double)num4 - (double)num9) / ((double)num10 - (double)num9));
					num10 = num4;
				}
				AddQuad(toFill, new Vector2(0f, num9) + pixelAdjustedRect.position, new Vector2(x, num10) + pixelAdjustedRect.position, color, new Vector2(vector.x, uvMin.y), new Vector2(uvMin.x, vector6.y));
				AddQuad(toFill, new Vector2(num3, num9) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, num10) + pixelAdjustedRect.position, color, new Vector2(vector5.x, uvMin.y), new Vector2(vector.z, vector6.y));
			}
			vector6 = vector5;
			for (float num11 = x; (double)num11 < (double)num3; num11 += num)
			{
				float num12 = num11 + num;
				if ((double)num12 > (double)num3)
				{
					vector6.x = uvMin.x + (float)(((double)vector5.x - (double)uvMin.x) * ((double)num3 - (double)num11) / ((double)num12 - (double)num11));
					num12 = num3;
				}
				AddQuad(toFill, new Vector2(num11, 0f) + pixelAdjustedRect.position, new Vector2(num12, y) + pixelAdjustedRect.position, color, new Vector2(uvMin.x, vector.y), new Vector2(vector6.x, uvMin.y));
				AddQuad(toFill, new Vector2(num11, num4) + pixelAdjustedRect.position, new Vector2(num12, pixelAdjustedRect.height) + pixelAdjustedRect.position, color, new Vector2(uvMin.x, vector5.y), new Vector2(vector6.x, vector.w));
			}
			AddQuad(toFill, new Vector2(0f, 0f) + pixelAdjustedRect.position, new Vector2(x, y) + pixelAdjustedRect.position, color, new Vector2(vector.x, vector.y), new Vector2(uvMin.x, uvMin.y));
			AddQuad(toFill, new Vector2(num3, 0f) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, y) + pixelAdjustedRect.position, color, new Vector2(vector5.x, vector.y), new Vector2(vector.z, uvMin.y));
			AddQuad(toFill, new Vector2(0f, num4) + pixelAdjustedRect.position, new Vector2(x, pixelAdjustedRect.height) + pixelAdjustedRect.position, color, new Vector2(vector.x, vector5.y), new Vector2(uvMin.x, vector.w));
			AddQuad(toFill, new Vector2(num3, num4) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, pixelAdjustedRect.height) + pixelAdjustedRect.position, color, new Vector2(vector5.x, vector5.y), new Vector2(vector.z, vector.w));
		}

		private void GenerateFilledSprite(VertexHelper toFill, bool isPreserveAspect)
		{
			toFill.Clear();
			if ((double)base.fillAmount < 0.001)
			{
				return;
			}
			Vector4 drawingDimensions = GetDrawingDimensions(isPreserveAspect);
			Vector4 obj = ((!(base.overrideSprite != null)) ? Vector4.zero : DataUtility.GetOuterUV(base.overrideSprite));
			UIVertex.simpleVert.color = color;
			float num = obj.x + UVRect.x;
			float num2 = obj.y + UVRect.y;
			float num3 = obj.z + UVRect.width;
			float num4 = obj.w + UVRect.height;
			if (base.fillMethod == FillMethod.Horizontal || base.fillMethod == FillMethod.Vertical)
			{
				if (base.fillMethod == FillMethod.Horizontal)
				{
					float num5 = (num3 - num) * base.fillAmount;
					if (base.fillOrigin == 1)
					{
						drawingDimensions.x = drawingDimensions.z - (drawingDimensions.z - drawingDimensions.x) * base.fillAmount;
						num = num3 - num5;
					}
					else
					{
						drawingDimensions.z = drawingDimensions.x + (drawingDimensions.z - drawingDimensions.x) * base.fillAmount;
						num3 = num + num5;
					}
				}
				else if (base.fillMethod == FillMethod.Vertical)
				{
					float num6 = (num4 - num2) * base.fillAmount;
					if (base.fillOrigin == 1)
					{
						drawingDimensions.y = drawingDimensions.w - (drawingDimensions.w - drawingDimensions.y) * base.fillAmount;
						num2 = num4 - num6;
					}
					else
					{
						drawingDimensions.w = drawingDimensions.y + (drawingDimensions.w - drawingDimensions.y) * base.fillAmount;
						num4 = num2 + num6;
					}
				}
			}
			s_Xy[0] = new Vector2(drawingDimensions.x, drawingDimensions.y);
			s_Xy[1] = new Vector2(drawingDimensions.x, drawingDimensions.w);
			s_Xy[2] = new Vector2(drawingDimensions.z, drawingDimensions.w);
			s_Xy[3] = new Vector2(drawingDimensions.z, drawingDimensions.y);
			s_Uv[0] = new Vector2(num, num2);
			s_Uv[1] = new Vector2(num, num4);
			s_Uv[2] = new Vector2(num3, num4);
			s_Uv[3] = new Vector2(num3, num2);
			if ((double)base.fillAmount < 1.0 && base.fillMethod != 0 && base.fillMethod != FillMethod.Vertical)
			{
				if (base.fillMethod == FillMethod.Radial90)
				{
					if (RadialCut(s_Xy, s_Uv, base.fillAmount, base.fillClockwise, base.fillOrigin))
					{
						AddQuad(toFill, s_Xy, color, s_Uv);
					}
				}
				else if (base.fillMethod == FillMethod.Radial180)
				{
					for (int i = 0; i < 2; i++)
					{
						int num7 = ((base.fillOrigin > 1) ? 1 : 0);
						float t;
						float t2;
						float t3;
						float t4;
						if (base.fillOrigin == 0 || base.fillOrigin == 2)
						{
							t = 0f;
							t2 = 1f;
							if (i == num7)
							{
								t3 = 0f;
								t4 = 0.5f;
							}
							else
							{
								t3 = 0.5f;
								t4 = 1f;
							}
						}
						else
						{
							t3 = 0f;
							t4 = 1f;
							if (i == num7)
							{
								t = 0.5f;
								t2 = 1f;
							}
							else
							{
								t = 0f;
								t2 = 0.5f;
							}
						}
						s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t3);
						s_Xy[1].x = s_Xy[0].x;
						s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t4);
						s_Xy[3].x = s_Xy[2].x;
						s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t);
						s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t2);
						s_Xy[2].y = s_Xy[1].y;
						s_Xy[3].y = s_Xy[0].y;
						s_Uv[0].x = Mathf.Lerp(num, num3, t3);
						s_Uv[1].x = s_Uv[0].x;
						s_Uv[2].x = Mathf.Lerp(num, num3, t4);
						s_Uv[3].x = s_Uv[2].x;
						s_Uv[0].y = Mathf.Lerp(num2, num4, t);
						s_Uv[1].y = Mathf.Lerp(num2, num4, t2);
						s_Uv[2].y = s_Uv[1].y;
						s_Uv[3].y = s_Uv[0].y;
						float value = ((!base.fillClockwise) ? (base.fillAmount * 2f - (float)(1 - i)) : (base.fillAmount * 2f - (float)i));
						if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(value), base.fillClockwise, (i + base.fillOrigin + 3) % 4))
						{
							AddQuad(toFill, s_Xy, color, s_Uv);
						}
					}
				}
				else
				{
					if (base.fillMethod != FillMethod.Radial360)
					{
						return;
					}
					for (int j = 0; j < 4; j++)
					{
						float t5;
						float t6;
						if (j < 2)
						{
							t5 = 0f;
							t6 = 0.5f;
						}
						else
						{
							t5 = 0.5f;
							t6 = 1f;
						}
						float t7;
						float t8;
						if (j == 0 || j == 3)
						{
							t7 = 0f;
							t8 = 0.5f;
						}
						else
						{
							t7 = 0.5f;
							t8 = 1f;
						}
						s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t5);
						s_Xy[1].x = s_Xy[0].x;
						s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t6);
						s_Xy[3].x = s_Xy[2].x;
						s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t7);
						s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t8);
						s_Xy[2].y = s_Xy[1].y;
						s_Xy[3].y = s_Xy[0].y;
						s_Uv[0].x = Mathf.Lerp(num, num3, t5);
						s_Uv[1].x = s_Uv[0].x;
						s_Uv[2].x = Mathf.Lerp(num, num3, t6);
						s_Uv[3].x = s_Uv[2].x;
						s_Uv[0].y = Mathf.Lerp(num2, num4, t7);
						s_Uv[1].y = Mathf.Lerp(num2, num4, t8);
						s_Uv[2].y = s_Uv[1].y;
						s_Uv[3].y = s_Uv[0].y;
						float value2 = ((!base.fillClockwise) ? (base.fillAmount * 4f - (float)(3 - (j + base.fillOrigin) % 4)) : (base.fillAmount * 4f - (float)((j + base.fillOrigin) % 4)));
						if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(value2), base.fillClockwise, (j + 2) % 4))
						{
							AddQuad(toFill, s_Xy, color, s_Uv);
						}
					}
				}
			}
			else
			{
				AddQuad(toFill, s_Xy, color, s_Uv);
			}
		}

		private static void AddQuad(VertexHelper vertexHelper, Vector3[] quadPositions, Color32 color, Vector3[] quadUVs)
		{
			int currentVertCount = vertexHelper.currentVertCount;
			for (int i = 0; i < 4; i++)
			{
				vertexHelper.AddVert(quadPositions[i], color, quadUVs[i]);
			}
			vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
			vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
		}

		private static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax)
		{
			int currentVertCount = vertexHelper.currentVertCount;
			vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0f), color, new Vector2(uvMin.x, uvMin.y));
			vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0f), color, new Vector2(uvMin.x, uvMax.y));
			vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0f), color, new Vector2(uvMax.x, uvMax.y));
			vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0f), color, new Vector2(uvMax.x, uvMin.y));
			vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
			vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
		}

		private static Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
		{
			for (int i = 0; i <= 1; i++)
			{
				float num = border[i] + border[i + 2];
				if ((double)rect.size[i] < (double)num && (double)num != 0.0)
				{
					float num2 = rect.size[i] / num;
					border[i] *= num2;
					border[i + 2] *= num2;
				}
			}
			return border;
		}

		private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
		{
			Vector4 vector = ((!(base.overrideSprite == null)) ? DataUtility.GetPadding(base.overrideSprite) : Vector4.zero);
			Vector2 vector2 = ((!(base.overrideSprite == null)) ? new Vector2(base.overrideSprite.rect.width, base.overrideSprite.rect.height) : Vector2.zero);
			Rect pixelAdjustedRect = GetPixelAdjustedRect();
			int num = Mathf.RoundToInt(vector2.x);
			int num2 = Mathf.RoundToInt(vector2.y);
			Vector4 vector3 = new Vector4(vector.x / (float)num, vector.y / (float)num2, ((float)num - vector.z) / (float)num, ((float)num2 - vector.w) / (float)num2);
			if (shouldPreserveAspect && (double)vector2.sqrMagnitude > 0.0)
			{
				float num3 = vector2.x / vector2.y;
				float num4 = pixelAdjustedRect.width / pixelAdjustedRect.height;
				if ((double)num3 > (double)num4)
				{
					float height = pixelAdjustedRect.height;
					pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / num3);
					pixelAdjustedRect.y += (height - pixelAdjustedRect.height) * base.rectTransform.pivot.y;
				}
				else
				{
					float width = pixelAdjustedRect.width;
					pixelAdjustedRect.width = pixelAdjustedRect.height * num3;
					pixelAdjustedRect.x += (width - pixelAdjustedRect.width) * base.rectTransform.pivot.x;
				}
			}
			return new Vector4(pixelAdjustedRect.x + pixelAdjustedRect.width * vector3.x, pixelAdjustedRect.y + pixelAdjustedRect.height * vector3.y, pixelAdjustedRect.x + pixelAdjustedRect.width * vector3.z, pixelAdjustedRect.y + pixelAdjustedRect.height * vector3.w);
		}

		private static bool RadialCut(Vector3[] xy, Vector3[] uv, float fill, bool invert, int corner)
		{
			if ((double)fill < 0.001)
			{
				return false;
			}
			if ((corner & 1) == 1)
			{
				invert = !invert;
			}
			if (!invert && (double)fill > 0.999000012874603)
			{
				return true;
			}
			float num = Mathf.Clamp01(fill);
			if (invert)
			{
				num = 1f - num;
			}
			float f = num * 1.570796f;
			float cos = Mathf.Cos(f);
			float sin = Mathf.Sin(f);
			RadialCut(xy, cos, sin, invert, corner);
			RadialCut(uv, cos, sin, invert, corner);
			return true;
		}

		private static void RadialCut(Vector3[] xy, float cos, float sin, bool invert, int corner)
		{
			int num = (corner + 1) % 4;
			int num2 = (corner + 2) % 4;
			int num3 = (corner + 3) % 4;
			if ((corner & 1) == 1)
			{
				if ((double)sin > (double)cos)
				{
					cos /= sin;
					sin = 1f;
					if (invert)
					{
						xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
						xy[num2].x = xy[num].x;
					}
				}
				else if ((double)cos > (double)sin)
				{
					sin /= cos;
					cos = 1f;
					if (!invert)
					{
						xy[num2].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
						xy[num3].y = xy[num2].y;
					}
				}
				else
				{
					cos = 1f;
					sin = 1f;
				}
				if (!invert)
				{
					xy[num3].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
				}
				else
				{
					xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
				}
				return;
			}
			if ((double)cos > (double)sin)
			{
				sin /= cos;
				cos = 1f;
				if (!invert)
				{
					xy[num].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
					xy[num2].y = xy[num].y;
				}
			}
			else if ((double)sin > (double)cos)
			{
				cos /= sin;
				sin = 1f;
				if (invert)
				{
					xy[num2].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
					xy[num3].x = xy[num2].x;
				}
			}
			else
			{
				cos = 1f;
				sin = 1f;
			}
			if (invert)
			{
				xy[num3].y = Mathf.Lerp(xy[corner].y, xy[num2].y, sin);
			}
			else
			{
				xy[num].x = Mathf.Lerp(xy[corner].x, xy[num2].x, cos);
			}
		}
	}
}
