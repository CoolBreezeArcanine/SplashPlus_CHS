using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace UI
{
	[RequireComponent(typeof(Image))]
	public class CustomUV : BaseMeshEffect
	{
		private static readonly Vector2[] SVertScratch = new Vector2[4];

		private static readonly Vector2[] SUVScratch = new Vector2[4];

		private static readonly Vector3[] SXy = new Vector3[4];

		private static readonly Vector3[] SUv = new Vector3[4];

		[SerializeField]
		private Rect _uvRect = new Rect(0f, 0f, 0f, 0f);

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
					((Image)base.graphic).SetVerticesDirty();
				}
			}
		}

		public override void ModifyMesh(VertexHelper vh)
		{
			if (IsActive())
			{
				Image image = base.graphic as Image;
				switch (image.type)
				{
				case Image.Type.Simple:
					GenerateSimpleSprite(image, vh, image.preserveAspect);
					break;
				case Image.Type.Sliced:
					GenerateSlicedSprite(image, vh);
					break;
				case Image.Type.Tiled:
					GenerateTiledSprite(image, vh);
					break;
				case Image.Type.Filled:
					GenerateFilledSprite(image, vh, image.preserveAspect);
					break;
				}
			}
		}

		private void GenerateSimpleSprite(Image image, VertexHelper vh, bool lPreserveAspect)
		{
			Vector4 drawingDimensions = GetDrawingDimensions(image, lPreserveAspect);
			Vector4 vector = ((!(image.overrideSprite != null)) ? Vector4.zero : DataUtility.GetOuterUV(image.overrideSprite));
			Color color = image.color;
			vh.Clear();
			vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.y), color, new Vector2(vector.x + _uvRect.xMin, vector.y + _uvRect.yMin));
			vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.w), color, new Vector2(vector.x + _uvRect.xMin, vector.w + _uvRect.yMax));
			vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.w), color, new Vector2(vector.z + _uvRect.xMax, vector.w + _uvRect.yMax));
			vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.y), color, new Vector2(vector.z + _uvRect.xMax, vector.y + _uvRect.yMin));
			vh.AddTriangle(0, 1, 2);
			vh.AddTriangle(2, 3, 0);
		}

		private void GenerateSlicedSprite(Image image, VertexHelper toFill)
		{
			if (!image.hasBorder)
			{
				GenerateSimpleSprite(image, toFill, lPreserveAspect: false);
				return;
			}
			Vector4 vector;
			Vector4 vector2;
			Vector4 vector3;
			Vector4 vector4;
			if (image.overrideSprite != null)
			{
				vector = DataUtility.GetOuterUV(image.overrideSprite);
				vector2 = DataUtility.GetInnerUV(image.overrideSprite);
				vector3 = DataUtility.GetPadding(image.overrideSprite);
				vector4 = image.overrideSprite.border;
			}
			else
			{
				vector = Vector4.zero;
				vector2 = Vector4.zero;
				vector3 = Vector4.zero;
				vector4 = Vector4.zero;
			}
			Rect pixelAdjustedRect = image.GetPixelAdjustedRect();
			Vector4 adjustedBorders = GetAdjustedBorders(vector4 / image.pixelsPerUnit, pixelAdjustedRect);
			Vector4 vector5 = vector3 / image.pixelsPerUnit;
			SVertScratch[0] = new Vector2(vector5.x, vector5.y);
			SVertScratch[3] = new Vector2(pixelAdjustedRect.width - vector5.z, pixelAdjustedRect.height - vector5.w);
			SVertScratch[1].x = adjustedBorders.x;
			SVertScratch[1].y = adjustedBorders.y;
			SVertScratch[2].x = pixelAdjustedRect.width - adjustedBorders.z;
			SVertScratch[2].y = pixelAdjustedRect.height - adjustedBorders.w;
			for (int i = 0; i < 4; i++)
			{
				SVertScratch[i].x += pixelAdjustedRect.x;
				SVertScratch[i].y += pixelAdjustedRect.y;
			}
			SUVScratch[0] = new Vector2(vector.x, vector.y);
			SUVScratch[1] = new Vector2(vector2.x, vector2.y);
			SUVScratch[2] = new Vector2(vector2.z, vector2.w);
			SUVScratch[3] = new Vector2(vector.z, vector.w);
			toFill.Clear();
			for (int j = 0; j < 3; j++)
			{
				int num = j + 1;
				for (int k = 0; k < 3; k++)
				{
					if (image.fillCenter || j != 1 || k != 1)
					{
						int num2 = k + 1;
						AddQuad(toFill, new Vector2(SVertScratch[j].x, SVertScratch[k].y), new Vector2(SVertScratch[num].x, SVertScratch[num2].y), image.color, new Vector2(SUVScratch[j].x, SUVScratch[k].y), new Vector2(SUVScratch[num].x, SUVScratch[num2].y));
					}
				}
			}
		}

		private void GenerateTiledSprite(Image image, VertexHelper toFill)
		{
			Vector4 vector;
			Vector4 vector2;
			Vector2 vector4;
			Vector4 vector3;
			if (image.overrideSprite != null)
			{
				vector = DataUtility.GetOuterUV(image.overrideSprite);
				vector2 = DataUtility.GetInnerUV(image.overrideSprite);
				vector3 = image.overrideSprite.border;
				vector4 = image.overrideSprite.rect.size;
			}
			else
			{
				vector = Vector4.zero;
				vector2 = Vector4.zero;
				vector3 = Vector4.zero;
				vector4 = Vector2.one * 100f;
			}
			Rect pixelAdjustedRect = image.GetPixelAdjustedRect();
			float num = (vector4.x - vector3.x - vector3.z) / image.pixelsPerUnit;
			float num2 = (vector4.y - vector3.y - vector3.w) / image.pixelsPerUnit;
			vector3 = GetAdjustedBorders(vector3 / image.pixelsPerUnit, pixelAdjustedRect);
			vector += new Vector4(UVRect.xMin, UVRect.yMin, UVRect.xMax, UVRect.yMax);
			vector2 += new Vector4(UVRect.xMin, UVRect.yMin, UVRect.xMax, UVRect.yMax);
			Vector2 uvMin = new Vector2(vector2.x, vector2.y);
			Vector2 vector5 = new Vector2(vector2.z, vector2.w);
			UIVertex.simpleVert.color = image.color;
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
			if (image.fillCenter)
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
						AddQuad(toFill, new Vector2(num7, num5) + pixelAdjustedRect.position, new Vector2(num8, num6) + pixelAdjustedRect.position, image.color, uvMin, uvMax);
					}
				}
			}
			if (!image.hasBorder)
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
				AddQuad(toFill, new Vector2(0f, num9) + pixelAdjustedRect.position, new Vector2(x, num10) + pixelAdjustedRect.position, image.color, new Vector2(vector.x, uvMin.y), new Vector2(uvMin.x, vector6.y));
				AddQuad(toFill, new Vector2(num3, num9) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, num10) + pixelAdjustedRect.position, image.color, new Vector2(vector5.x, uvMin.y), new Vector2(vector.z, vector6.y));
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
				AddQuad(toFill, new Vector2(num11, 0f) + pixelAdjustedRect.position, new Vector2(num12, y) + pixelAdjustedRect.position, image.color, new Vector2(uvMin.x, vector.y), new Vector2(vector6.x, uvMin.y));
				AddQuad(toFill, new Vector2(num11, num4) + pixelAdjustedRect.position, new Vector2(num12, pixelAdjustedRect.height) + pixelAdjustedRect.position, image.color, new Vector2(uvMin.x, vector5.y), new Vector2(vector6.x, vector.w));
			}
			AddQuad(toFill, new Vector2(0f, 0f) + pixelAdjustedRect.position, new Vector2(x, y) + pixelAdjustedRect.position, image.color, new Vector2(vector.x, vector.y), new Vector2(uvMin.x, uvMin.y));
			AddQuad(toFill, new Vector2(num3, 0f) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, y) + pixelAdjustedRect.position, image.color, new Vector2(vector5.x, vector.y), new Vector2(vector.z, uvMin.y));
			AddQuad(toFill, new Vector2(0f, num4) + pixelAdjustedRect.position, new Vector2(x, pixelAdjustedRect.height) + pixelAdjustedRect.position, image.color, new Vector2(vector.x, vector5.y), new Vector2(uvMin.x, vector.w));
			AddQuad(toFill, new Vector2(num3, num4) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, pixelAdjustedRect.height) + pixelAdjustedRect.position, image.color, new Vector2(vector5.x, vector5.y), new Vector2(vector.z, vector.w));
		}

		private void GenerateFilledSprite(Image image, VertexHelper toFill, bool isPreserveAspect)
		{
			toFill.Clear();
			if ((double)image.fillAmount < 0.001)
			{
				return;
			}
			Vector4 drawingDimensions = GetDrawingDimensions(image, isPreserveAspect);
			Vector4 obj = ((!(image.overrideSprite != null)) ? Vector4.zero : DataUtility.GetOuterUV(image.overrideSprite));
			UIVertex.simpleVert.color = image.color;
			float num = obj.x + UVRect.x;
			float num2 = obj.y + UVRect.y;
			float num3 = obj.z + UVRect.width;
			float num4 = obj.w + UVRect.height;
			if (image.fillMethod == Image.FillMethod.Horizontal || image.fillMethod == Image.FillMethod.Vertical)
			{
				if (image.fillMethod == Image.FillMethod.Horizontal)
				{
					float num5 = (num3 - num) * image.fillAmount;
					if (image.fillOrigin == 1)
					{
						drawingDimensions.x = drawingDimensions.z - (drawingDimensions.z - drawingDimensions.x) * image.fillAmount;
						num = num3 - num5;
					}
					else
					{
						drawingDimensions.z = drawingDimensions.x + (drawingDimensions.z - drawingDimensions.x) * image.fillAmount;
						num3 = num + num5;
					}
				}
				else if (image.fillMethod == Image.FillMethod.Vertical)
				{
					float num6 = (num4 - num2) * image.fillAmount;
					if (image.fillOrigin == 1)
					{
						drawingDimensions.y = drawingDimensions.w - (drawingDimensions.w - drawingDimensions.y) * image.fillAmount;
						num2 = num4 - num6;
					}
					else
					{
						drawingDimensions.w = drawingDimensions.y + (drawingDimensions.w - drawingDimensions.y) * image.fillAmount;
						num4 = num2 + num6;
					}
				}
			}
			SXy[0] = new Vector2(drawingDimensions.x, drawingDimensions.y);
			SXy[1] = new Vector2(drawingDimensions.x, drawingDimensions.w);
			SXy[2] = new Vector2(drawingDimensions.z, drawingDimensions.w);
			SXy[3] = new Vector2(drawingDimensions.z, drawingDimensions.y);
			SUv[0] = new Vector2(num, num2);
			SUv[1] = new Vector2(num, num4);
			SUv[2] = new Vector2(num3, num4);
			SUv[3] = new Vector2(num3, num2);
			if ((double)image.fillAmount < 1.0 && image.fillMethod != 0 && image.fillMethod != Image.FillMethod.Vertical)
			{
				if (image.fillMethod == Image.FillMethod.Radial90)
				{
					if (RadialCut(SXy, SUv, image.fillAmount, image.fillClockwise, image.fillOrigin))
					{
						AddQuad(toFill, SXy, image.color, SUv);
					}
				}
				else if (image.fillMethod == Image.FillMethod.Radial180)
				{
					for (int i = 0; i < 2; i++)
					{
						int num7 = ((image.fillOrigin > 1) ? 1 : 0);
						float t;
						float t2;
						float t3;
						float t4;
						if (image.fillOrigin == 0 || image.fillOrigin == 2)
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
						SXy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t3);
						SXy[1].x = SXy[0].x;
						SXy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t4);
						SXy[3].x = SXy[2].x;
						SXy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t);
						SXy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t2);
						SXy[2].y = SXy[1].y;
						SXy[3].y = SXy[0].y;
						SUv[0].x = Mathf.Lerp(num, num3, t3);
						SUv[1].x = SUv[0].x;
						SUv[2].x = Mathf.Lerp(num, num3, t4);
						SUv[3].x = SUv[2].x;
						SUv[0].y = Mathf.Lerp(num2, num4, t);
						SUv[1].y = Mathf.Lerp(num2, num4, t2);
						SUv[2].y = SUv[1].y;
						SUv[3].y = SUv[0].y;
						float value = ((!image.fillClockwise) ? (image.fillAmount * 2f - (float)(1 - i)) : (image.fillAmount * 2f - (float)i));
						if (RadialCut(SXy, SUv, Mathf.Clamp01(value), image.fillClockwise, (i + image.fillOrigin + 3) % 4))
						{
							AddQuad(toFill, SXy, image.color, SUv);
						}
					}
				}
				else
				{
					if (image.fillMethod != Image.FillMethod.Radial360)
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
						SXy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t5);
						SXy[1].x = SXy[0].x;
						SXy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, t6);
						SXy[3].x = SXy[2].x;
						SXy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t7);
						SXy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, t8);
						SXy[2].y = SXy[1].y;
						SXy[3].y = SXy[0].y;
						SUv[0].x = Mathf.Lerp(num, num3, t5);
						SUv[1].x = SUv[0].x;
						SUv[2].x = Mathf.Lerp(num, num3, t6);
						SUv[3].x = SUv[2].x;
						SUv[0].y = Mathf.Lerp(num2, num4, t7);
						SUv[1].y = Mathf.Lerp(num2, num4, t8);
						SUv[2].y = SUv[1].y;
						SUv[3].y = SUv[0].y;
						float value2 = ((!image.fillClockwise) ? (image.fillAmount * 4f - (float)(3 - (j + image.fillOrigin) % 4)) : (image.fillAmount * 4f - (float)((j + image.fillOrigin) % 4)));
						if (RadialCut(SXy, SUv, Mathf.Clamp01(value2), image.fillClockwise, (j + 2) % 4))
						{
							AddQuad(toFill, SXy, image.color, SUv);
						}
					}
				}
			}
			else
			{
				AddQuad(toFill, SXy, image.color, SUv);
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

		private Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
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

		private Vector4 GetDrawingDimensions(Image image, bool shouldPreserveAspect)
		{
			Vector4 vector = ((!(image.overrideSprite == null)) ? DataUtility.GetPadding(image.overrideSprite) : Vector4.zero);
			Vector2 vector2 = ((!(image.overrideSprite == null)) ? new Vector2(image.overrideSprite.rect.width, image.overrideSprite.rect.height) : Vector2.zero);
			Rect pixelAdjustedRect = image.GetPixelAdjustedRect();
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
					pixelAdjustedRect.y += (height - pixelAdjustedRect.height) * image.rectTransform.pivot.y;
				}
				else
				{
					float width = pixelAdjustedRect.width;
					pixelAdjustedRect.width = pixelAdjustedRect.height * num3;
					pixelAdjustedRect.x += (width - pixelAdjustedRect.width) * image.rectTransform.pivot.x;
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
