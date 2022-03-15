using Process.CodeRead;
using UI.DaisyChainList;
using UnityEngine;

namespace Monitor.CodeRead.ChainList
{
	public class ReadCardChainList : DaisyChainList
	{
		private ICodeReadProcess _process;

		private int _playerIndex;

		public override void Initialize()
		{
			base.Initialize();
			foreach (CardFlameChainObject chainObject in ChainObjectList)
			{
				chainObject.Initialize();
			}
		}

		public void SetData(ICodeReadProcess process, int playerIndex)
		{
			_process = process;
			_playerIndex = playerIndex;
		}

		public void SetDecision(bool isDecision)
		{
			foreach (CardFlameChainObject chainObject in ChainObjectList)
			{
				chainObject?.SetDecision(isDecision: false);
			}
			((CardFlameChainObject)SpotArray[4])?.SetDecision(isDecision);
		}

		public override void Deploy()
		{
			RemoveAll();
			int generateCardNum = _process.GetGenerateCardNum(_playerIndex);
			int num = 4 - _process.SelectCardIndexs[_playerIndex];
			for (int i = 0; i < generateCardNum; i++)
			{
				CardFlameChainObject chain = GetChain<CardFlameChainObject>();
				int cardListID = _process.GetCardListID(_playerIndex, i - (4 - num));
				CodeReadProcess.ReadCard readCard = null;
				readCard = ((i != 0 || !_process.IsCardReaded(_playerIndex)) ? _process.GetReadCard(_playerIndex, cardListID) : _process.GetCurrentReadedCard(_playerIndex));
				if (readCard != null)
				{
					Sprite background = readCard.Background;
					Sprite character = readCard.Character;
					Sprite frame = readCard.Frame;
					CodeReadProcess.GetUserCard(_playerIndex, CodeReadProcess.GetCardTypeData(cardListID).name.id, out var status);
					chain.SetData(_playerIndex, status, background, character, frame);
					if (_process.IsDecisionCard(_playerIndex, i))
					{
						chain.SetDecision(isDecision: true);
					}
					if (i == 0)
					{
						chain.OnCenterIn();
					}
					else
					{
						chain.OnCenterOut();
					}
					SetSpot(num + i, chain);
				}
			}
			Positioning(isImmediate: true, isAnimation: true);
			base.IsListEnable = true;
		}

		protected override void Next(int targetIndex, Direction direction)
		{
		}

		protected override void Remove(ChainObject chain)
		{
			((CardFlameChainObject)chain)?.SetDecision(isDecision: false);
			base.Remove(chain);
		}
	}
}
