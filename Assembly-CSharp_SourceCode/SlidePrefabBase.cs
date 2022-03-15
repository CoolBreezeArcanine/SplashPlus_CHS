using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class SlidePrefabBase : MonoBehaviour
{
	[Serializable]
	public class SlideArrowBuf
	{
		public float X;

		public float Y;

		public float Rotate;
	}

	[Serializable]
	public class SlideArrowList
	{
		public List<SlideArrowBuf> SlidePathList = new List<SlideArrowBuf>();
	}

	[Serializable]
	public class TouchArea
	{
		public int HideArrowNum;

		public List<InputManager.TouchPanelArea> Area = new List<InputManager.TouchPanelArea>();
	}

	[Serializable]
	public class TouchAreaList
	{
		public List<TouchArea> Areas = new List<TouchArea>();
	}

	public TouchAreaList SlideTouch = new TouchAreaList();

	public SlideArrowList SlideArrows = new SlideArrowList();

	public Vector3 StartPos;

	public Vector3 EndPos;
}
