using System.Collections.Generic;
using UnityEngine;

public class UserMapData
{
	public const uint MaxDistance = 999999999u;

	private uint _distance;

	public int ID { get; set; }

	public Color MapColor { get; set; }

	public int CharaColorKey { get; set; }

	public string Name { get; set; }

	public uint Distance
	{
		get
		{
			return _distance;
		}
		set
		{
			_distance = value;
		}
	}

	public bool IsEvent { get; set; }

	public bool IsLock { get; set; }

	public bool IsFinishedOpening { get; set; }

	public bool IsClear { get; set; }

	public bool IsComplete { get; set; }

	public bool IsDeluxe { get; set; }

	public List<int> ReleaseIds { get; set; } = new List<int>();


	public UserMapData()
	{
		Clear();
	}

	public UserMapData(int id)
	{
		ID = id;
		Clear();
	}

	public void Clear()
	{
		Distance = 0u;
		IsLock = false;
		IsFinishedOpening = false;
		IsClear = false;
		IsComplete = false;
		IsDeluxe = false;
	}
}
