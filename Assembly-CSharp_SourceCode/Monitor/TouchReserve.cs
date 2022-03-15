using System.Collections.Generic;
using UnityEngine;

namespace Monitor
{
	public class TouchReserve : MonoBehaviour
	{
		public struct ReserveData
		{
			public int Index;

			public bool IsEach;

			public ReserveData(int index, bool each)
			{
				Index = index;
				IsEach = each;
			}
		}

		private const int ReserveCountMax = 2;

		[SerializeField]
		private GameObject[] _reserveSimbole = new GameObject[2];

		private SpriteRenderer[] _reserveSimboleSprite = new SpriteRenderer[2];

		[SerializeField]
		private Sprite[] _reserveSingleSprite = new Sprite[2];

		[SerializeField]
		private Sprite[] _reserveEachSprite = new Sprite[2];

		private List<ReserveData> targetIndex = new List<ReserveData>();

		private List<ReserveData> deathIndex = new List<ReserveData>();

		private List<ReserveData> liveIndex = new List<ReserveData>();

		public void Initialize(int monitorIndex)
		{
			targetIndex.Clear();
			deathIndex.Clear();
			liveIndex.Clear();
			DispUpdate();
			float num = TouchNoteB.NotesScale[0];
			for (int i = 0; i < _reserveSimbole.Length; i++)
			{
				_reserveSimbole[i].transform.localScale = new Vector3(num, num, 1f);
				_reserveSimboleSprite[i] = _reserveSimbole[i].GetComponent<SpriteRenderer>();
			}
		}

		public void SetReserveCount(List<ReserveData> noteIndex)
		{
			foreach (ReserveData item in noteIndex)
			{
				if (!targetIndex.Contains(item))
				{
					targetIndex.Add(item);
					if (!liveIndex.Contains(item))
					{
						liveIndex.Add(item);
					}
				}
			}
			DispUpdate();
		}

		public void DeathNote()
		{
			if (liveIndex.Count != 0)
			{
				liveIndex.RemoveAt(0);
			}
			DispUpdate();
		}

		private void DispUpdate()
		{
			for (int i = 0; i < _reserveSimbole.Length; i++)
			{
				if (i < liveIndex.Count)
				{
					_reserveSimbole[i].SetActive(value: true);
					_reserveSimboleSprite[i].sprite = (liveIndex[i].IsEach ? _reserveEachSprite[i] : _reserveSingleSprite[i]);
				}
				else
				{
					_reserveSimbole[i].SetActive(value: false);
				}
			}
		}
	}
}
