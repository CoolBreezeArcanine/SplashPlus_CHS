using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(TMP_Text))]
	public class TMPCircleEffect : UIBehaviour
	{
		[SerializeField]
		private float _radius = 100f;

		[SerializeField]
		private float _offsetAngle;

		[SerializeField]
		private float _diff = 10f;

		[SerializeField]
		private bool _turnLeft;

		private TMP_Text _tmpText;

		public TMP_Text TmpText => _tmpText ?? GetComponent<TMP_Text>();

		private void Update()
		{
			if (TmpText.havePropertiesChanged)
			{
				MeshUpdate();
			}
		}

		private void MeshUpdate()
		{
			if (TmpText == null)
			{
				return;
			}
			TmpText.ForceMeshUpdate();
			TMP_TextInfo textInfo = TmpText.textInfo;
			int characterCount = textInfo.characterCount;
			Vector3[] array = null;
			if (characterCount == 0)
			{
				return;
			}
			float num = _offsetAngle;
			for (int i = 0; i < characterCount; i++)
			{
				if (textInfo.characterInfo.Length < characterCount)
				{
					break;
				}
				if (textInfo.characterInfo[i].isVisible)
				{
					int vertexIndex = textInfo.characterInfo[i].vertexIndex;
					int materialReferenceIndex = textInfo.characterInfo[i].materialReferenceIndex;
					array = textInfo.meshInfo[materialReferenceIndex].vertices;
					Vector3 vector = new Vector3(Mathf.Cos(num * ((float)Math.PI / 180f)), Mathf.Sin(num * ((float)Math.PI / 180f)), 0f) * _radius;
					float f = (array[vertexIndex].x + array[vertexIndex + 2].x) / 2f;
					float f2 = (array[vertexIndex].y + array[vertexIndex + 2].y) / 2f;
					float num2 = Mathf.Abs(Mathf.Abs(array[vertexIndex].x) - Mathf.Abs(f));
					float num3 = Mathf.Abs(Mathf.Abs(array[vertexIndex].y) - Mathf.Abs(f2));
					array[vertexIndex] = new Vector3(vector.x - num2, vector.y - num3, 0f);
					array[vertexIndex + 1] = new Vector3(vector.x - num2, vector.y + num3, 0f);
					array[vertexIndex + 2] = new Vector3(vector.x + num2, vector.y + num3, 0f);
					array[vertexIndex + 3] = new Vector3(vector.x + num2, vector.y - num3, 0f);
					num += _diff * (_turnLeft ? 1f : (-1f));
				}
			}
			if (array != null && array.Length >= 4)
			{
				TmpText.UpdateVertexData();
			}
		}
	}
}
