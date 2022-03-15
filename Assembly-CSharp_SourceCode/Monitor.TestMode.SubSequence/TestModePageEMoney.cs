using System;
using System.Text;
using AMDaemon;
using DB;
using MAI2.Util;
using Manager;
using TMPro;
using UnityEngine;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageEMoney : TestModePage
	{
		private enum ItemEnum
		{
			Auth,
			Remove,
			Exit,
			TerminalID,
			Brand,
			LabelDeal,
			TitleDeal,
			Deal0,
			Deal1,
			Deal2,
			Deal3,
			Deal4
		}

		private const char Space = ' ';

		private bool isEMoneyAvailable_;

		private bool isEMoneyServiceAlive_;

		private bool isEMoneyIsAuthCompleted_;

		private TestModePageEMoneyResult[] results_ = new TestModePageEMoneyResult[5];

		private StringBuilder stringBuilder_ = new StringBuilder();

		private const int LabelFontSize = 20;

		protected override void OnCreate()
		{
			base.OnCreate();
			isEMoneyAvailable_ = AMDaemon.EMoney.IsAvailable;
			isEMoneyServiceAlive_ = AMDaemon.EMoney.IsServiceAlive;
			isEMoneyIsAuthCompleted_ = AMDaemon.EMoney.IsAuthCompleted;
			results_[0] = base.transform.Find("Result0").GetComponent<TestModePageEMoneyResult>();
			results_[1] = base.transform.Find("Result1").GetComponent<TestModePageEMoneyResult>();
			results_[2] = base.transform.Find("Result2").GetComponent<TestModePageEMoneyResult>();
			results_[3] = base.transform.Find("Result3").GetComponent<TestModePageEMoneyResult>();
			results_[4] = base.transform.Find("Result4").GetComponent<TestModePageEMoneyResult>();
			TestModePageEMoneyResult[] array = results_;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: false);
			}
		}

		protected override void Destroy()
		{
			base.OnUpdate();
			isEMoneyAvailable_ = AMDaemon.EMoney.IsAvailable;
			isEMoneyServiceAlive_ = AMDaemon.EMoney.IsServiceAlive;
			isEMoneyIsAuthCompleted_ = AMDaemon.EMoney.IsAuthCompleted;
		}

		private void SetAuth(Item item, int index)
		{
			item.SetState((!isEMoneyAvailable_) ? Item.State.UnselectableTemp : Item.State.Selectable);
		}

		private void SetRemove(Item item, int index)
		{
			if (isEMoneyIsAuthCompleted_)
			{
				item.SetState(Item.State.Selectable);
			}
			else
			{
				item.SetState(Item.State.UnselectableTemp);
			}
		}

		private void SetTerminalId(Item item, int index)
		{
			string terminalId = AMDaemon.EMoney.TerminalId;
			item.ValueText.text = ((!string.IsNullOrEmpty(terminalId)) ? terminalId : "----");
			Vector2 anchoredPosition = item.ValueText.rectTransform.anchoredPosition;
			anchoredPosition.x = 0f;
			item.ValueText.rectTransform.anchoredPosition = anchoredPosition;
			Vector2 sizeDelta = item.ValueText.rectTransform.sizeDelta;
			sizeDelta.x = 640f;
			item.ValueText.rectTransform.sizeDelta = sizeDelta;
		}

		private void SetBrand(Item item, int index)
		{
			bool flag = false;
			stringBuilder_.Length = 0;
			int availableBrandCount = AMDaemon.EMoney.AvailableBrandCount;
			LazyCollection<EMoneyBrand> availableBrands = AMDaemon.EMoney.AvailableBrands;
			for (int i = 0; i < availableBrandCount; i++)
			{
				flag = false;
				if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsCheckDisableEmoneyBrandID)
				{
					flag = AMDaemon.EMoney.AvailableBrands[i].Id == EMoneyBrandId.Sapica;
				}
				if (!flag)
				{
					stringBuilder_.AppendFormat("{0} ", availableBrands[i].Name);
				}
			}
			item.ValueText.text = stringBuilder_.ToString();
			Vector2 anchoredPosition = item.ValueText.rectTransform.anchoredPosition;
			anchoredPosition.x = 0f;
			item.ValueText.rectTransform.anchoredPosition = anchoredPosition;
			Vector2 sizeDelta = item.ValueText.rectTransform.sizeDelta;
			sizeDelta.x = 640f;
			item.ValueText.rectTransform.sizeDelta = sizeDelta;
		}

		private void SetResultString(int index)
		{
			if (SingletonStateMachine<AmManager, AmManager.EState>.Instance.IsEnableEmoneyExecute)
			{
				if (AMDaemon.EMoney.DealResultCount <= index)
				{
					results_[index].gameObject.SetActive(value: false);
					return;
				}
				results_[index].gameObject.SetActive(value: true);
				LazyCollection<EMoneyResult> dealResults = AMDaemon.EMoney.DealResults;
				results_[index].Set(dealResults[index]);
			}
			else
			{
				results_[index].gameObject.SetActive(value: true);
				results_[index].SetDummy(index);
			}
		}

		protected override void OnInitializeItem(Item item, int index)
		{
			base.OnInitializeItem(item, index);
			switch (index)
			{
			case 0:
				SetAuth(item, index);
				break;
			case 1:
				SetRemove(item, index);
				break;
			case 3:
				SetTerminalId(item, index);
				break;
			case 4:
				SetBrand(item, index);
				break;
			case 5:
			{
				Vector2 sizeDelta2 = item.LabelText.rectTransform.sizeDelta;
				Vector2 anchoredPosition2 = item.LabelText.rectTransform.anchoredPosition;
				anchoredPosition2.x = (0f - sizeDelta2.x) * 0.5f;
				item.LabelText.rectTransform.anchoredPosition = anchoredPosition2;
				item.LabelText.alignment = TextAlignmentOptions.Top;
				break;
			}
			case 6:
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
			{
				Vector2 anchoredPosition = item.LabelText.rectTransform.anchoredPosition;
				anchoredPosition.x = -485f;
				item.LabelText.rectTransform.anchoredPosition = anchoredPosition;
				Vector2 sizeDelta = item.LabelText.rectTransform.sizeDelta;
				sizeDelta.x = 1000f;
				item.LabelText.rectTransform.sizeDelta = sizeDelta;
				item.LabelText.fontSize = 20f;
				break;
			}
			case 2:
				break;
			}
		}

		protected override void OnUpdateItem(Item item, int index)
		{
			base.OnUpdateItem(item, index);
			switch (index)
			{
			case 0:
				SetAuth(item, index);
				break;
			case 1:
				SetRemove(item, index);
				break;
			case 3:
				SetTerminalId(item, index);
				break;
			case 4:
				SetBrand(item, index);
				break;
			case 7:
				SetResultString(0);
				break;
			case 8:
				SetResultString(1);
				break;
			case 9:
				SetResultString(2);
				break;
			case 10:
				SetResultString(3);
				break;
			case 11:
				SetResultString(4);
				break;
			case 2:
			case 5:
			case 6:
				break;
			}
		}

		protected override void OnSelectItem(Item item, int index)
		{
			if (index == 2)
			{
				Finish(Result.GoBack);
			}
		}

		public override void OnFinishChild(Result result)
		{
			base.OnFinishChild(result);
			SetCursor(2);
			isEMoneyAvailable_ = AMDaemon.EMoney.IsAvailable;
			isEMoneyServiceAlive_ = AMDaemon.EMoney.IsServiceAlive;
			isEMoneyIsAuthCompleted_ = AMDaemon.EMoney.IsAuthCompleted;
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeEmoneyID)Enum.Parse(typeof(TestmodeEmoneyID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeEmoneyID)Enum.Parse(typeof(TestmodeEmoneyID), GetTitleName(index))).GetName();
		}
	}
}
