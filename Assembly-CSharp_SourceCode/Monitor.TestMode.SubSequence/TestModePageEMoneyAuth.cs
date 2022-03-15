using System;
using System.Diagnostics;
using AMDaemon;
using DB;
using Util;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageEMoneyAuth : TestModePage
	{
		private enum ItemEnum
		{
			Auth,
			Exit
		}

		private const string _InstructionText = "「認証する」を選ぶと、認証を開始します";

		private const string _CancelText = "テストボタンでキャンセルします";

		private const int State_Init = 0;

		private const int State_Auth = 1;

		private const int State_Cancel = 2;

		private bool _isEMoneyAvailable;

		private int _state;

		private string _idleValueText = string.Empty;

		private readonly Stopwatch _blinkTimer = new Stopwatch();

		protected override void OnCreate()
		{
			base.OnCreate();
			_isEMoneyAvailable = EMoney.IsAvailable;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			_isEMoneyAvailable = EMoney.IsAvailable;
			switch (_state)
			{
			case 0:
				InstructionText.text = "「認証する」を選ぶと、認証を開始します";
				break;
			case 1:
				if (!EMoney.Operation.IsBusy)
				{
					_state = 0;
					if (!EMoney.Operation.IsErrorOccurred)
					{
						Finish(Result.GoBack);
					}
					else
					{
						_idleValueText = "エラー";
					}
				}
				InstructionText.text = (EMoney.Operation.IsCancellable ? "テストボタンでキャンセルします" : string.Empty);
				break;
			case 2:
				if (!EMoney.Operation.IsBusy)
				{
					_idleValueText = "";
					_state = 0;
				}
				InstructionText.text = string.Empty;
				break;
			}
		}

		protected override void OnInitializeItem(Item item, int index)
		{
			base.OnInitializeItem(item, index);
			if (index == 0)
			{
				item.SetState((!_isEMoneyAvailable) ? Item.State.UnselectableTemp : Item.State.Selectable);
			}
		}

		protected override void OnUpdateItem(Item item, int index)
		{
			base.OnUpdateItem(item, index);
			switch (index)
			{
			case 0:
				if (_isEMoneyAvailable)
				{
					switch (_state)
					{
					case 0:
						item.ValueText.text = _idleValueText;
						item.SetState(Item.State.Selectable);
						break;
					case 1:
						item.ValueText.text = (Utility.isBlinkDisp(_blinkTimer) ? _idleValueText : "");
						item.SetState(Item.State.Selectable);
						break;
					case 2:
						item.ValueText.text = _idleValueText;
						item.SetState(Item.State.Selectable);
						break;
					}
				}
				else
				{
					item.SetState(Item.State.UnselectableTemp);
					switch (_state)
					{
					case 1:
						item.ValueText.text = (Utility.isBlinkDisp(_blinkTimer) ? _idleValueText : "");
						break;
					case 2:
						item.ValueText.text = _idleValueText;
						break;
					}
				}
				break;
			case 1:
				item.SetState((_state != 0) ? Item.State.UnselectableTemp : Item.State.Selectable);
				break;
			}
		}

		protected override void OnSelectItem(Item item, int index)
		{
			switch (index)
			{
			case 0:
				switch (_state)
				{
				case 0:
					if (EMoney.Operation.AuthTerminal())
					{
						_idleValueText = "認証中";
						_state = 1;
						InstructionText.text = "テストボタンでキャンセルします";
						_blinkTimer.Restart();
					}
					break;
				case 1:
					if (EMoney.Operation.IsCancellable && EMoney.Operation.Cancel())
					{
						_idleValueText = "キャンセル中";
						_state = 2;
					}
					break;
				}
				break;
			case 1:
				if (_state == 0)
				{
					Finish(Result.GoBack);
				}
				break;
			}
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeEmoneyAuthID)Enum.Parse(typeof(TestmodeEmoneyAuthID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeEmoneyAuthID)Enum.Parse(typeof(TestmodeEmoneyAuthID), GetTitleName(index))).GetName();
		}
	}
}
