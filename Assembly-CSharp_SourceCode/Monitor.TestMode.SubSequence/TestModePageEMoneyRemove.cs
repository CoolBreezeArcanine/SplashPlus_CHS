using System;
using System.Diagnostics;
using AMDaemon;
using DB;
using Util;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageEMoneyRemove : TestModePage
	{
		private enum ItemEnum
		{
			Remove,
			Exit
		}

		private const string _InstructionText = "「撤去する」を選ぶと、端末撤去を開始します";

		private const string _CancelText = "テストボタンでキャンセルします";

		private const int State_Init = 0;

		private const int State_Remove = 1;

		private const int State_Cancel = 2;

		private bool _isEMoneyAvailable;

		private bool _isEMoneyServiceAlive;

		private bool _isEMoneyIsAuthCompleted;

		private int _state;

		private string _idleValueText = string.Empty;

		private readonly Stopwatch _blinkTimer = new Stopwatch();

		private bool _isErrorOccurred;

		private int _count;

		private bool _previousIsCancellable;

		protected override void OnCreate()
		{
			base.OnCreate();
			_isEMoneyAvailable = EMoney.IsAvailable;
			_isEMoneyServiceAlive = EMoney.IsServiceAlive;
			_isEMoneyIsAuthCompleted = EMoney.IsAuthCompleted;
			_isErrorOccurred = false;
			_count = 0;
			_previousIsCancellable = EMoney.Operation.IsCancellable;
		}

		protected override void OnUpdate()
		{
			base.OnUpdate();
			_isEMoneyAvailable = EMoney.IsAvailable;
			_isEMoneyServiceAlive = EMoney.IsServiceAlive;
			_isEMoneyIsAuthCompleted = EMoney.IsAuthCompleted;
			switch (_state)
			{
			case 0:
				InstructionText.text = "「撤去する」を選ぶと、端末撤去を開始します";
				_count = 0;
				break;
			case 1:
				if (!EMoney.Operation.IsBusy)
				{
					_state = 0;
					_isErrorOccurred = EMoney.Operation.IsErrorOccurred;
					if (!_isErrorOccurred)
					{
						_idleValueText = "";
						InstructionText.text = string.Empty;
						Finish(Result.GoBack);
					}
					else
					{
						_idleValueText = "エラー";
						InstructionText.text = (EMoney.Operation.IsCancellable ? "テストボタンでキャンセルします" : string.Empty);
					}
					_count = 0;
				}
				else
				{
					if (EMoney.Operation.IsCancellable == _previousIsCancellable)
					{
						_count++;
					}
					else
					{
						_count = 0;
					}
					if (_count > 60)
					{
						InstructionText.text = (EMoney.Operation.IsCancellable ? "テストボタンでキャンセルします" : string.Empty);
					}
					else
					{
						InstructionText.text = string.Empty;
					}
				}
				_previousIsCancellable = EMoney.Operation.IsCancellable;
				break;
			case 2:
				if (!EMoney.Operation.IsBusy)
				{
					_idleValueText = "";
					_state = 0;
					_idleValueText = string.Empty;
				}
				InstructionText.text = string.Empty;
				_count = 0;
				break;
			}
		}

		protected override void OnInitializeItem(Item item, int index)
		{
			base.OnInitializeItem(item, index);
			if (index == 0)
			{
				if (_isEMoneyIsAuthCompleted)
				{
					item.SetState(Item.State.Selectable);
				}
				else
				{
					item.SetState(Item.State.UnselectableTemp);
				}
			}
		}

		protected override void OnUpdateItem(Item item, int index)
		{
			base.OnUpdateItem(item, index);
			switch (index)
			{
			case 0:
				if (_isEMoneyIsAuthCompleted)
				{
					switch (_state)
					{
					case 0:
						item.ValueText.text = _idleValueText;
						item.SetState((!EMoney.IsAuthCompleted) ? Item.State.UnselectableTemp : Item.State.Selectable);
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
					break;
				}
				item.SetState(Item.State.UnselectableTemp);
				switch (_state)
				{
				case 0:
					if (_isErrorOccurred)
					{
						item.ValueText.text = "エラー";
						item.SetState(Item.State.UnselectableTemp);
					}
					break;
				case 1:
					item.ValueText.text = (Utility.isBlinkDisp(_blinkTimer) ? _idleValueText : "");
					break;
				case 2:
					item.ValueText.text = _idleValueText;
					break;
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
					if (EMoney.Operation.RemoveTerminal())
					{
						_idleValueText = "撤去中";
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
			return ((TestmodeEmoneyRemoveID)Enum.Parse(typeof(TestmodeEmoneyRemoveID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeEmoneyRemoveID)Enum.Parse(typeof(TestmodeEmoneyRemoveID), GetTitleName(index))).GetName();
		}
	}
}
