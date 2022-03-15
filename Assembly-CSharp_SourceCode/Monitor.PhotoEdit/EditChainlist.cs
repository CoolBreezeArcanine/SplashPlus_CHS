using Process;
using UI.DaisyChainList;

namespace Monitor.PhotoEdit
{
	public class EditChainlist : DaisyChainList
	{
		private int _playerIndex;

		private IPhotoEditProcess _editProcess;

		public void AdvancedInitialize(IPhotoEditProcess editProcess, int playerIndex)
		{
			_playerIndex = playerIndex;
			_editProcess = editProcess;
		}

		public void SetValue(Direction direction, string switchText)
		{
			((EditCardObject)SpotArray[4]).SetValue(direction, switchText);
		}

		public override void Deploy()
		{
			RemoveAll();
			int num = 5;
			int num2 = (int)(4 - _editProcess.GetCurrentSettingIndex(_playerIndex));
			for (int i = 0; i < num; i++)
			{
				int num3 = i - (4 - num2);
				EditCardObject chain = GetChain<EditCardObject>();
				_editProcess.GetSettingSwitchValue(_playerIndex, num3, out var switchName);
				_editProcess.IsCheckCategory(_playerIndex, num3, out var isLeftActive, out var isRightActive);
				chain.SetData(_editProcess.GetSettingNameSprite(_playerIndex, num3), switchName, isLeftActive, isRightActive);
				if (num3 == 0)
				{
					chain.OnCenterIn();
				}
				else
				{
					chain.ResetChain();
				}
				SetSpot(num2 + i, chain);
			}
			base.Deploy();
		}

		protected override void Next(int targetIndex, Direction direction)
		{
		}

		public void Preseedbutton(Direction direction, bool isLongTouch, bool toOut)
		{
			EditCardObject editCardObject = (EditCardObject)SpotArray[4];
			if (editCardObject != null)
			{
				editCardObject.PressedButton(direction, isLongTouch, toOut);
			}
		}

		public void SetVisibleButton(bool isVisible, Direction direction)
		{
			EditCardObject editCardObject = (EditCardObject)SpotArray[4];
			if (editCardObject != null)
			{
				editCardObject.SetVisibleButton(isVisible, direction);
			}
		}
	}
}
