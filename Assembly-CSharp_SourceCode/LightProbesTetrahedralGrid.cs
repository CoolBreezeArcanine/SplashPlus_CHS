using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LightProbeGroup))]
public class LightProbesTetrahedralGrid : MonoBehaviour
{
	private struct TriangleProps
	{
		public float side;

		public float halfSide;

		public float height;

		public float inscribedCircleRadius;

		public float circumscribedCircleRadius;

		public TriangleProps(float triangleSide)
		{
			side = triangleSide;
			halfSide = side / 2f;
			height = Mathf.Sqrt(3f) * side / 2f;
			inscribedCircleRadius = Mathf.Sqrt(3f) * side / 6f;
			circumscribedCircleRadius = 2f * height / 3f;
		}
	}

	public float m_Side = 1f;

	public float m_Radius = 5f;

	public float m_InnerRadius = 0.1f;

	public float m_Height = 2f;

	public uint m_Levels = 3u;

	private const float kMinSide = 0.05f;

	private const float kMinHeight = 0.05f;

	private const float kMinInnerRadius = 0.1f;

	private const uint kMinIterations = 4u;

	private TriangleProps m_TriangleProps;

	public void OnValidate()
	{
		m_Side = Mathf.Max(0.05f, m_Side);
		m_Height = Mathf.Max(0.05f, m_Height);
		if (m_InnerRadius < 0.1f)
		{
			m_Radius = Mathf.Max(new TriangleProps(m_Side).circumscribedCircleRadius + 0.01f, m_Radius);
			return;
		}
		m_Radius = Mathf.Max(0.1f, m_Radius);
		m_InnerRadius = Mathf.Min(m_Radius, m_InnerRadius);
	}

	public void Generate()
	{
		GetComponent<LightProbeGroup>();
		List<Vector3> outPositions = new List<Vector3>();
		m_TriangleProps = new TriangleProps(m_Side);
		if (m_InnerRadius < 0.1f)
		{
			GenerateCylinder(m_TriangleProps, m_Radius, m_Height, m_Levels, outPositions);
		}
		else
		{
			GenerateRing(m_TriangleProps, m_Radius, m_InnerRadius, m_Height, m_Levels, outPositions);
		}
	}

	private static void AttemptAdding(Vector3 position, Vector3 center, float distanceCutoffSquared, List<Vector3> outPositions)
	{
		if ((position - center).sqrMagnitude < distanceCutoffSquared)
		{
			outPositions.Add(position);
		}
	}

	private uint CalculateCylinderIterations(TriangleProps props, float radius)
	{
		int num = Mathf.CeilToInt((radius + props.height - props.inscribedCircleRadius) / props.height);
		if (num > 0)
		{
			return (uint)num;
		}
		return 0u;
	}

	private void GenerateCylinder(TriangleProps props, float radius, float height, uint levels, List<Vector3> outPositions)
	{
		uint num = CalculateCylinderIterations(props, radius);
		float distanceCutoffSquared = radius * radius;
		Vector3 vector = new Vector3(props.circumscribedCircleRadius, 0f, 0f);
		Vector3 vector2 = new Vector3(0f - props.inscribedCircleRadius, 0f, 0f - props.halfSide);
		Vector3 vector3 = new Vector3(0f - props.inscribedCircleRadius, 0f, props.halfSide);
		for (uint num2 = 0u; num2 < levels; num2++)
		{
			float num3 = ((levels == 1) ? 0f : ((float)num2 / (float)(levels - 1)));
			Vector3 vector4 = new Vector3(0f, num3 * height, 0f);
			if (num2 % 2u == 0)
			{
				for (uint num4 = 0u; num4 < num; num4++)
				{
					Vector3 vector5 = vector4 + vector + num4 * vector * 2f * 3f / 2f;
					Vector3 vector6 = vector4 + vector2 + num4 * vector2 * 2f * 3f / 2f;
					Vector3 vector7 = vector4 + vector3 + num4 * vector3 * 2f * 3f / 2f;
					AttemptAdding(vector5, vector4, distanceCutoffSquared, outPositions);
					AttemptAdding(vector6, vector4, distanceCutoffSquared, outPositions);
					AttemptAdding(vector7, vector4, distanceCutoffSquared, outPositions);
					Vector3 vector8 = vector5 - vector6;
					Vector3 vector9 = vector7 - vector5;
					Vector3 vector10 = vector6 - vector7;
					uint num5 = 3 * num4 + 1;
					for (uint num6 = 1u; num6 < num5; num6++)
					{
						AttemptAdding(vector6 + vector8 * num6 / num5, vector4, distanceCutoffSquared, outPositions);
						AttemptAdding(vector5 + vector9 * num6 / num5, vector4, distanceCutoffSquared, outPositions);
						AttemptAdding(vector7 + vector10 * num6 / num5, vector4, distanceCutoffSquared, outPositions);
					}
				}
				continue;
			}
			for (uint num7 = 0u; num7 < num; num7++)
			{
				Vector3 vector11 = vector4 + num7 * (2f * vector * 3f / 2f);
				Vector3 vector12 = vector4 + num7 * (2f * vector2 * 3f / 2f);
				Vector3 vector13 = vector4 + num7 * (2f * vector3 * 3f / 2f);
				AttemptAdding(vector11, vector4, distanceCutoffSquared, outPositions);
				AttemptAdding(vector12, vector4, distanceCutoffSquared, outPositions);
				AttemptAdding(vector13, vector4, distanceCutoffSquared, outPositions);
				Vector3 vector14 = vector11 - vector12;
				Vector3 vector15 = vector13 - vector11;
				Vector3 vector16 = vector12 - vector13;
				uint num8 = 3 * num7;
				for (uint num9 = 1u; num9 < num8; num9++)
				{
					AttemptAdding(vector12 + vector14 * num9 / num8, vector4, distanceCutoffSquared, outPositions);
					AttemptAdding(vector11 + vector15 * num9 / num8, vector4, distanceCutoffSquared, outPositions);
					AttemptAdding(vector13 + vector16 * num9 / num8, vector4, distanceCutoffSquared, outPositions);
				}
			}
		}
	}

	private void GenerateRing(TriangleProps props, float radius, float innerRadius, float height, uint levels, List<Vector3> outPositions)
	{
		float side = props.side;
		float num = Mathf.Clamp(2f * Mathf.Asin(side / (2f * radius)), 0.01f, (float)Math.PI * 2f);
		uint num2 = (uint)Mathf.FloorToInt((float)Math.PI * 2f / num);
		uint num3 = (uint)Mathf.Max(Mathf.Ceil((radius - innerRadius) / props.height), 0f);
		for (uint num4 = 0u; num4 < levels; num4++)
		{
			float num5 = ((levels == 1) ? 0f : ((float)num4 / (float)(levels - 1)));
			float y = height * num5;
			float num6 = ((num4 % 2u == 0) ? 0f : 0.5f);
			for (uint num7 = 0u; num7 < num3; num7++)
			{
				float num8 = ((num3 == 1) ? 1f : ((float)num7 / (float)(num3 - 1)));
				float t = (num8 * (radius - innerRadius) + innerRadius - 0.1f) / (radius - 0.1f);
				uint num9 = (uint)Mathf.CeilToInt(Mathf.Lerp(4f, num2, t));
				float x = innerRadius + (radius - innerRadius) * num8;
				Vector3 vector = new Vector3(x, y, 0f);
				float num10 = ((num7 % 2u == 0) ? 0f : 0.5f);
				for (uint num11 = 0u; num11 < num9; num11++)
				{
					Quaternion quaternion = Quaternion.Euler(0f, ((float)num11 + num6 + num10) * 360f / (float)num9, 0f);
					outPositions.Add(quaternion * vector);
				}
			}
		}
	}
}
