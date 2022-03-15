using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class TextMoveEffect : UIBehaviour
{
	public enum ControlleMode
	{
		None,
		Simple,
		Active
	}

	[SerializeField]
	private ControlleMode _mode;

	[SerializeField]
	private float _radius = 1.5f;

	[SerializeField]
	private float _frequency = 0.05f;

	[SerializeField]
	private bool _distortion;

	private readonly TMP_Text _tmpText;

	public ControlleMode Mode => _mode;

	public float Frequency => _frequency;

	public float Timer { get; set; }

	public bool IsTypewriter { get; set; }

	public TMP_Text TmpText => _tmpText ?? GetComponent<TMP_Text>();

	private void Update()
	{
		Timer += Time.deltaTime;
		if (Timer > _frequency)
		{
			Timer = 0f;
			MeshUpdate();
		}
	}

	public void MeshUpdate()
	{
		if (TmpText == null)
		{
			return;
		}
		if (!_distortion)
		{
			TmpText.ForceMeshUpdate();
		}
		TMP_TextInfo textInfo = TmpText.textInfo;
		int characterCount = textInfo.characterCount;
		Vector3[] vertices = null;
		if (characterCount == 0)
		{
			return;
		}
		for (int i = 0; i < characterCount && (!IsTypewriter || i != characterCount - 1); i++)
		{
			if (textInfo.characterInfo.Length < characterCount)
			{
				break;
			}
			if (textInfo.characterInfo[i].isVisible)
			{
				int vertexIndex = textInfo.characterInfo[i].vertexIndex;
				int materialReferenceIndex = textInfo.characterInfo[i].materialReferenceIndex;
				vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
				TextMove(vertexIndex, ref vertices);
			}
		}
		if (vertices != null && vertices.Length >= 4)
		{
			TmpText.UpdateVertexData();
		}
	}

	public void TextMove(int vertexIndex, ref Vector3[] vertices)
	{
		switch (_mode)
		{
		case ControlleMode.Simple:
			TextMoveSimple(vertexIndex, ref vertices);
			break;
		case ControlleMode.Active:
			TextMoveActive(vertexIndex, ref vertices);
			break;
		}
	}

	private void TextMoveSimple(int vertexIndex, ref Vector3[] vertices)
	{
		float f = (float)UnityEngine.Random.Range(0, 360) * ((float)Math.PI / 180f);
		Vector3 vector = new Vector3(_radius * Mathf.Cos(f), _radius * Mathf.Sin(f), 0f);
		for (int i = 0; i < 4; i++)
		{
			Vector3 vector2 = vertices[vertexIndex + i];
			vector2 += vector;
			vertices[vertexIndex + i] = vector2;
		}
	}

	private void TextMoveActive(int vertexIndex, ref Vector3[] vertices)
	{
		Vector3[] array = new Vector3[4];
		for (int i = 0; i < 4; i++)
		{
			float f = (float)UnityEngine.Random.Range(0, 360) * ((float)Math.PI / 180f);
			float num = _radius * UnityEngine.Random.value;
			Vector3 vector = (array[i] = new Vector3(num * Mathf.Cos(f), num * Mathf.Sin(f), 0f));
		}
		for (int j = 0; j < 4; j++)
		{
			Vector3 vector2 = vertices[vertexIndex + j];
			vector2 += array[j];
			vertices[vertexIndex + j] = vector2;
		}
	}
}
