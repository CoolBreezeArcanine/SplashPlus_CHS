using System;
using System.Collections.Generic;
using DB;
using MAI2.Util;
using Manager;
using UnityEngine;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageSystemInfo2 : TestModePage
	{
		private static int _currentPage;

		public static void ClearCurrentPage()
		{
			_currentPage = 0;
		}

		public static void AddCurrentPage()
		{
			_currentPage++;
		}

		protected override void OnCreate()
		{
			Delivery delivery = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Delivery;
			int num = delivery.testModeOptionPageCount + 1;
			int num2 = _currentPage + 2;
			SetTitleText(0, TestmodeSystemInfo2ID.Title0.GetName() + $"({num2}/{num})");
			SetTitleText(1, TestmodeSystemInfo2ID.Title1.GetName());
			ItemList[0].SetLabel(TestmodeSystemInfo2ID.Label00.GetName());
			ItemList[0].SetValueString(TestmodeSystemInfo2ID.Label00_1.GetName());
			List<OptionImageInfo> testModeOptionImageInfo = delivery.getTestModeOptionImageInfo(_currentPage);
			if (testModeOptionImageInfo != null)
			{
				for (int i = 0; i < testModeOptionImageInfo.Count; i++)
				{
					int num3 = i + 1;
					if (num3 < ItemList.Count)
					{
						OptionImageInfo optionImageInfo = testModeOptionImageInfo[i];
						ItemList[num3].SetLabel(optionImageInfo.Name);
						ItemList[num3].SetValueString(optionImageInfo.CreationTime.ToString("yyyy/MM/dd"));
					}
				}
			}
			OpType opType = ((num2 == num) ? OpType.TestExit : OpType.TestContinue);
			ChangeOpType(opType);
			AddCurrentPage();
		}

		protected override void OnPostCreate(GameObject prefab, object arg)
		{
			NextPagePrefab = prefab;
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeSystemInfo2ID)Enum.Parse(typeof(TestmodeSystemInfo2ID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeSystemInfo2ID)Enum.Parse(typeof(TestmodeSystemInfo2ID), GetTitleName(index))).GetName();
		}
	}
}
