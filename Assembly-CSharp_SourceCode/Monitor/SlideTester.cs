using System.Collections.Generic;
using System.IO;
using MAI2.Util;
using Manager;
using Process;
using UnityEngine;

namespace Monitor
{
	public class SlideTester : MonoBehaviour
	{
		[SerializeField]
		private GameObject _slideprefub;

		private void Start()
		{
			GameNotePrefabContainer.Initialize();
			GameNoteImageContainer.Initialize();
			List<string> dirs = new List<string>();
			GetDirs(ref dirs);
			GameNoteImageContainer.Initialize();
			GameNotePrefabContainer.Initialize();
			Singleton<UserDataManager>.Instance.SetDefault(0L);
			Singleton<UserDataManager>.Instance.GetUserData(0L).IsEntry = true;
			Singleton<GamePlayManager>.Instance.AddPlayLog();
			Singleton<SlideManager>.Instance.Initialize(Application.streamingAssetsPath + "/Table/slide/");
			while (Singleton<SlideManager>.Instance.IsActiveThread())
			{
			}
			GameObject gameObject = new GameObject();
			gameObject.transform.SetPositionAndRotation(new Vector3(-540f, -420f, 0f), default(Quaternion));
			for (int i = 0; i < 13; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					NoteData noteData = new NoteData
					{
						slideData = 
						{
							type = (SlideType)i
						},
						startButtonPos = 0
					};
					noteData.slideData.targetNote = j;
					if (!Singleton<SlideManager>.Instance.CheckSlideEnable(noteData.slideData.type, noteData.startButtonPos, j))
					{
						continue;
					}
					GameObject gameObject2 = new GameObject();
					gameObject2.transform.parent = gameObject.transform;
					gameObject2.transform.localPosition = new Vector3(0f, 0f, 0f);
					gameObject2.AddComponent<SlidePrefabBase>();
					SlidePrefabBase component = gameObject2.GetComponent<SlidePrefabBase>();
					List<Vector4> slidePath = Singleton<SlideManager>.Instance.GetSlidePath(noteData.slideData.type, noteData.startButtonPos, noteData.slideData.targetNote);
					component.SlideArrows.SlidePathList.Clear();
					for (int k = 0; k < slidePath.Count; k++)
					{
						component.SlideArrows.SlidePathList.Add(new SlidePrefabBase.SlideArrowBuf
						{
							X = slidePath[k].x,
							Y = slidePath[k].y,
							Rotate = slidePath[k].w
						});
					}
					component.StartPos = slidePath[0];
					component.EndPos = slidePath[slidePath.Count - 1];
					List<SlideManager.HitArea> slideHitArea = Singleton<SlideManager>.Instance.GetSlideHitArea(noteData.slideData.type, noteData.startButtonPos, noteData.slideData.targetNote);
					for (int l = 0; l < slideHitArea.Count; l++)
					{
						SlidePrefabBase.TouchArea touchArea = new SlidePrefabBase.TouchArea();
						for (int m = 0; m < slideHitArea[l].HitPoints.Count; m++)
						{
							touchArea.Area.Add(slideHitArea[l].HitPoints[m]);
						}
						component.SlideTouch.Areas.Add(touchArea);
					}
					SlideType slideType = (SlideType)i;
					gameObject2.name = slideType.ToString() + "_1_" + (j + 1);
				}
			}
		}

		public void GetDirs(ref List<string> dirs)
		{
			dirs.Clear();
			string streamingAssetsPath = Application.streamingAssetsPath;
			string streamingAssetsPath2 = Application.streamingAssetsPath;
			string searchPattern = "A???";
			string[] directories = Directory.GetDirectories(streamingAssetsPath, searchPattern, SearchOption.TopDirectoryOnly);
			string[] directories2 = Directory.GetDirectories(streamingAssetsPath2, searchPattern, SearchOption.TopDirectoryOnly);
			dirs.AddRange(directories);
			dirs.Sort();
			List<string> list = new List<string>();
			list.AddRange(directories2);
			list.Sort();
			dirs.AddRange(list);
		}
	}
}
