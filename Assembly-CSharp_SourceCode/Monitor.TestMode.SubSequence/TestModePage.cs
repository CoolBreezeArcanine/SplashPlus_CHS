using System.Collections.Generic;
using DB;
using Manager;
using Process;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Monitor.TestMode.SubSequence
{
	[RequireComponent(typeof(RectTransform))]
	public class TestModePage : TestModeObject
	{
		public enum Result
		{
			GoBack,
			GoToSystemTest
		}

		public delegate void OnFinish(Result result);

		private const float CursorOffset = 1.5f;

		[SerializeField]
		private ItemDefine[] _itemDefines;

		[SerializeField]
		private OpType _opType;

		[SerializeField]
		protected GameObject NextPagePrefab;

		public TestModePage Parent;

		public OnFinish OnFinishCallBack;

		private Vector3 _cursorOffset;

		protected List<Item> ItemList;

		private Transform _cursorObjectRt;

		private int _cursorIndex;

		private bool _pause;

		public TMP_FontAsset Font => BaseFont;

		protected virtual object parameter => null;

		public virtual void OnFinishChild(Result result)
		{
			if (result == Result.GoToSystemTest)
			{
				Finish(Result.GoToSystemTest);
			}
			else
			{
				base.gameObject.SetActive(value: true);
			}
		}

		private new void Awake()
		{
			base.Awake();
			OnCreate();
			RawImage rawImage = Utility.findChildRecursive<RawImage>(base.transform, "RawImage");
			if (null != rawImage)
			{
				rawImage.texture = TestModeProcess.TestModeRenderTexture;
			}
		}

		private void Update()
		{
			ExecuteInput();
			UpdateCursor();
			OnUpdate();
			UpdateItem();
		}

		private void OnDestroy()
		{
			Destroy();
		}

		protected virtual void OnCreate()
		{
			SetTitleText(0, GetTitleString(0));
		}

		protected virtual void OnPostCreate(GameObject prefab, object arg)
		{
		}

		protected virtual void OnUpdate()
		{
		}

		protected virtual void Destroy()
		{
		}

		protected virtual void OnInitializeItem(Item item, int index)
		{
		}

		protected virtual void OnUpdateItem(Item item, int index)
		{
		}

		protected virtual void OnSelectItem(Item item, int index)
		{
		}

		protected void SetCursor(int index)
		{
			index = Mathf.Clamp(index, 0, ItemList.Count);
			_cursorIndex = index;
			UpdateCursor();
		}

		protected override void InitializeParamater()
		{
			base.InitializeParamater();
			if (BaseFont == null)
			{
				BaseFont = TestModeObject.DefaultFont;
			}
			_cursorOffset = new Vector3(-30f, 0f, 0f);
		}

		protected override void CreateInstruction()
		{
			base.CreateInstruction();
			OnChangeOpType();
		}

		protected override void CreateItems()
		{
			_cursorIndex = 0;
			ItemList = new List<Item>();
			for (int i = 0; i < _itemDefines.Length; i++)
			{
				ItemDefine itemDefine = _itemDefines[i];
				Item item = new Item
				{
					Define = itemDefine
				};
				item.SetState((!itemDefine.IsSelectable) ? Item.State.Unselectable : Item.State.Selectable);
				float y = TopPositionY - (float)((itemDefine.LineNumber + ItemTopLine) * 24);
				TextMeshProUGUI textMeshProUGUI = MakeItemLabelText(itemDefine.HasValueField);
				textMeshProUGUI.text = GetLabelString(i);
				textMeshProUGUI.transform.SetParent(base.transform, worldPositionStays: false);
				textMeshProUGUI.transform.localPosition = new Vector3(LabelPositionX, y, 0f);
				item.LabelText = textMeshProUGUI;
				if (itemDefine.HasValueField)
				{
					int num = ((itemDefine.NumValueField <= 0) ? 1 : itemDefine.NumValueField);
					for (int j = 0; j < num; j++)
					{
						TextMeshProUGUI textMeshProUGUI2 = MakeItemValueText();
						textMeshProUGUI2.text = "";
						textMeshProUGUI2.transform.SetParent(textMeshProUGUI.transform, worldPositionStays: false);
						Vector2 sizeDelta = textMeshProUGUI2.rectTransform.sizeDelta;
						sizeDelta.x /= num;
						textMeshProUGUI2.rectTransform.sizeDelta = sizeDelta;
						textMeshProUGUI2.transform.localPosition = new Vector3(ValuePositionX + sizeDelta.x * (float)j, 0f, 0f);
						item.ValueTextList.Add(textMeshProUGUI2);
					}
				}
				ItemList.Add(item);
				if (itemDefine.IsDefaultSelection)
				{
					_cursorIndex = i;
				}
				OnInitializeItem(item, i);
			}
			if (_opType == OpType.Select)
			{
				GameObject gameObject = MakeCursor();
				if (gameObject != null)
				{
					_cursorObjectRt = gameObject.transform;
					gameObject.transform.SetParent(base.transform, worldPositionStays: false);
				}
			}
		}

		protected void SetPause(bool pause)
		{
			_pause = pause;
			_cursorObjectRt.gameObject.SetActive(!pause);
		}

		private GameObject MakeCursor()
		{
			TextMeshProUGUI textMeshProUGUI = MakeText();
			if (textMeshProUGUI != null)
			{
				RectTransform rectTransform = textMeshProUGUI.transform as RectTransform;
				if (rectTransform != null)
				{
					rectTransform.transform.position = Vector3.zero;
					rectTransform.sizeDelta = new Vector2(20f, 20f);
					textMeshProUGUI.text = "＞";
					textMeshProUGUI.alignment = TextAlignmentOptions.TopLeft;
					textMeshProUGUI.name = "Cursor";
					rectTransform.pivot = new Vector2(0f, 0f);
				}
			}
			if (!(textMeshProUGUI != null))
			{
				return null;
			}
			return textMeshProUGUI.gameObject;
		}

		private void ExecuteInput()
		{
			if (!_pause)
			{
				switch (_opType)
				{
				case OpType.Select:
					ExecuteInputSelect();
					break;
				case OpType.TestExit:
					ExecuteInputTestExit();
					break;
				case OpType.TestServiceExit:
					ExecuteInputTestServiceExit();
					break;
				case OpType.TestContinue:
					ExecuteInputTestContinue();
					break;
				case OpType.TestServiceAbort:
					ExecuteInputTestServiceAbort();
					break;
				}
			}
		}

		private void ExecuteInputSelect()
		{
			int num = 0;
			if (GetTriggerPrev())
			{
				do
				{
					if (--_cursorIndex < 0)
					{
						_cursorIndex = ItemList.Count - 1;
					}
				}
				while (++num < ItemList.Count && !ItemList[_cursorIndex].IsSelectable);
			}
			else if (GetTriggerNext())
			{
				do
				{
					if (++_cursorIndex >= ItemList.Count)
					{
						_cursorIndex = 0;
					}
				}
				while (++num < ItemList.Count && !ItemList[_cursorIndex].IsSelectable);
			}
			else if (GetTriggerDecision())
			{
				Item item = ItemList[_cursorIndex];
				if ((bool)item.Define.NextPagePrefab)
				{
					MakeChildAndSleep(item.Define.NextPagePrefab);
				}
				else if (item.Define.IsFinishOnSelect)
				{
					Finish(Result.GoBack);
				}
				else
				{
					OnSelectItem(item, _cursorIndex);
				}
			}
		}

		private void ExecuteInputTestExit()
		{
			if (InputManager.GetSystemInputDown(InputManager.SystemButtonSetting.ButtonTest) || InputManager.GetMonitorButtonDown(InputManager.ButtonSetting.Button05))
			{
				Finish(Result.GoBack);
			}
		}

		private void ExecuteInputTestServiceExit()
		{
			if (InputManager.GetSystemInputPush(InputManager.SystemButtonSetting.ButtonTest) && InputManager.GetSystemInputPush(InputManager.SystemButtonSetting.ButtonService))
			{
				Finish(Result.GoBack);
			}
		}

		private void ExecuteInputTestContinue()
		{
			if ((InputManager.GetSystemInputDown(InputManager.SystemButtonSetting.ButtonTest) || InputManager.GetMonitorButtonDown(InputManager.ButtonSetting.Button05)) && NextPagePrefab != null)
			{
				MakeSiblingAndDie(NextPagePrefab);
			}
		}

		private void ExecuteInputTestServiceAbort()
		{
			if (InputManager.GetSystemInputPush(InputManager.SystemButtonSetting.ButtonTest) && InputManager.GetSystemInputPush(InputManager.SystemButtonSetting.ButtonService))
			{
				Finish(Result.GoBack);
			}
		}

		protected void MakeChildAndSleep(GameObject prefab)
		{
			TestModePage component = Object.Instantiate(prefab, base.transform.parent).GetComponent<TestModePage>();
			if (component != null)
			{
				component.OnPostCreate(prefab, parameter);
				component.Parent = this;
				base.gameObject.SetActive(value: false);
			}
		}

		protected void MakeSiblingAndDie(GameObject prefab)
		{
			TestModePage component = Object.Instantiate(prefab, base.transform.parent).GetComponent<TestModePage>();
			if (component != null)
			{
				component.OnPostCreate(prefab, parameter);
				component.Parent = Parent;
				Object.Destroy(base.gameObject);
			}
		}

		protected void Finish(Result result)
		{
			if (Parent != null)
			{
				Parent.OnFinishChild(result);
				Object.Destroy(base.gameObject);
			}
			else
			{
				OnFinishCallBack?.Invoke(result);
			}
		}

		private bool GetTriggerPrev()
		{
			return false | InputManager.GetMonitorButtonDown(InputManager.ButtonSetting.Button01);
		}

		private bool GetTriggerNext()
		{
			return false | InputManager.GetSystemInputDown(InputManager.SystemButtonSetting.ButtonService) | InputManager.GetMonitorButtonDown(InputManager.ButtonSetting.Button04);
		}

		private bool GetTriggerDecision()
		{
			return false | InputManager.GetSystemInputDown(InputManager.SystemButtonSetting.ButtonTest) | InputManager.GetMonitorButtonDown(InputManager.ButtonSetting.Button05);
		}

		private void UpdateCursor()
		{
			if (_cursorObjectRt != null)
			{
				Item item = ItemList[_cursorIndex];
				if (item != null)
				{
					_cursorObjectRt.position = item.LabelText.transform.position + _cursorOffset;
				}
			}
		}

		private void UpdateItem()
		{
			int num = 0;
			foreach (Item item in ItemList)
			{
				OnUpdateItem(item, num);
				num++;
			}
		}

		protected void ChangeOpType(OpType opType)
		{
			_opType = opType;
			OnChangeOpType();
		}

		private void OnChangeOpType()
		{
			string text = "";
			switch (_opType)
			{
			case OpType.Select:
				text = TestmodeGenericID.OpTypeSelect.GetName();
				break;
			case OpType.TestExit:
				text = TestmodeGenericID.OpTypeTextExit.GetName();
				break;
			case OpType.TestServiceExit:
				text = TestmodeGenericID.OpTypeTestServiceExit.GetName();
				break;
			case OpType.TestContinue:
				text = TestmodeGenericID.OpTypeTextContinue.GetName();
				break;
			case OpType.TestServiceAbort:
				text = TestmodeGenericID.OpTypeServiceAbort.GetName();
				break;
			}
			InstructionText.text = text;
		}

		protected virtual string GetLabelString(int index)
		{
			return "";
		}

		protected string GetLabelName(int index)
		{
			return "Label" + index.ToString("00");
		}

		protected virtual string GetTitleString(int index)
		{
			return "";
		}

		protected string GetTitleName(int index)
		{
			return "Title" + index.ToString("0");
		}
	}
}
