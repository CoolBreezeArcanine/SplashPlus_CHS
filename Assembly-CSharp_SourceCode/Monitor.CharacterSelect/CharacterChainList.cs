using Manager.UserDatas;
using Process;
using UI.DaisyChainList;
using UnityEngine;

namespace Monitor.CharacterSelect
{
	public class CharacterChainList : DaisyChainList
	{
		private int _playerIndex;

		private ICharacterSelectProcess _selectProcess;

		public bool IsLeaderChange;

		public override void Initialize()
		{
			base.Initialize();
			foreach (CharacterChainObject chainObject in ChainObjectList)
			{
				chainObject.Initialize();
			}
			((CharacterChainObject)ScrollChainCard).Initialize();
		}

		public void SetData(int playerIndex, ICharacterSelectProcess selectProcess)
		{
			_playerIndex = playerIndex;
			_selectProcess = selectProcess;
		}

		public override void SetScrollCard(bool isVisible)
		{
			if (isVisible)
			{
				CharacterChainObject characterChainObject = (CharacterChainObject)ScrollChainCard;
				CharacterData userCharaData = _selectProcess.GetUserCharaData(_playerIndex, 0);
				if (userCharaData.Data != null && userCharaData.UserChara != null)
				{
					Texture2D texture = userCharaData.Texture;
					Sprite face = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
					CharacterMapColorData mapColorData = _selectProcess.GetMapColorData(userCharaData.Data.genre.id);
					characterChainObject.SetData((int)userCharaData.UserChara.Level, userCharaData.UserChara.NextAwakePercent, (int)userCharaData.UserChara.Awakening, face, mapColorData, userCharaData.ShadowColor);
					bool flag = _selectProcess.IsMatchColor(_playerIndex, userCharaData.UserChara.ID);
					WeakPoint weakPoint = ((!flag) ? WeakPoint.Weak : WeakPoint.Forte);
					bool isLeaderChange = IsLeaderChange;
					if (isLeaderChange)
					{
						weakPoint = WeakPoint.Leader;
					}
					int movementParam = (int)userCharaData.UserChara.GetMovementParam(flag, isLeaderChange);
					CharacterSelectProces.ArrowDirection arrowDirection = _selectProcess.GetArrowDirection(_playerIndex, movementParam);
					characterChainObject.SetInformation(_selectProcess.IsSelectedCharacter(_playerIndex, userCharaData.ID), userCharaData.Data.name.str, movementParam, weakPoint, arrowDirection);
				}
				else
				{
					characterChainObject.SetBlank();
				}
				characterChainObject.OnCenterIn();
			}
			base.SetScrollCard(isVisible);
		}

		public override void Deploy()
		{
			RemoveAll();
			int num = 0;
			int overCount;
			for (int i = 0; i <= 4; i++)
			{
				if (_selectProcess.IsChangeCharacterCategory(_playerIndex, i - 4, out overCount))
				{
					num++;
				}
			}
			int num2 = -4 + num;
			for (int j = 0; j < 9; j++)
			{
				int diffIndex = j - 4;
				if (_selectProcess.IsChangeCharacterCategory(_playerIndex, diffIndex, out overCount))
				{
					string characterCategoryName = _selectProcess.GetCharacterCategoryName(_playerIndex, overCount - 1);
					string characterCategoryName2 = _selectProcess.GetCharacterCategoryName(_playerIndex, overCount);
					SetSpot(j, GetSeparate(characterCategoryName2, characterCategoryName));
					continue;
				}
				CharacterChainObject chain = GetChain<CharacterChainObject>();
				CharacterData userCharaData = _selectProcess.GetUserCharaData(_playerIndex, num2);
				if (userCharaData.Data != null && userCharaData.UserChara != null)
				{
					CharacterMapColorData mapColorData = _selectProcess.GetMapColorData(userCharaData.Data.genre.id);
					Texture2D texture = userCharaData.Texture;
					Sprite face = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
					chain.SetData((int)userCharaData.UserChara.Level, userCharaData.UserChara.NextAwakePercent, (int)userCharaData.UserChara.Awakening, face, mapColorData, userCharaData.ShadowColor);
					bool flag = _selectProcess.IsMatchColor(_playerIndex, userCharaData.UserChara.ID);
					WeakPoint weakPoint = ((!flag) ? WeakPoint.Weak : WeakPoint.Forte);
					bool isLeaderChange = IsLeaderChange;
					if (isLeaderChange)
					{
						weakPoint = WeakPoint.Leader;
					}
					int movementParam = (int)userCharaData.UserChara.GetMovementParam(flag, isLeaderChange);
					CharacterSelectProces.ArrowDirection arrowDirection = _selectProcess.GetArrowDirection(_playerIndex, movementParam);
					chain.SetInformation(_selectProcess.IsSelectedCharacter(_playerIndex, userCharaData.ID), userCharaData.Data.name.str, movementParam, weakPoint, arrowDirection);
				}
				else
				{
					chain.SetBlank();
				}
				SetSpot(j, chain);
				if (j == 4)
				{
					chain.OnCenterIn();
				}
				else
				{
					chain.OnCenterOut();
				}
				num2++;
			}
			base.Deploy();
		}

		public void NewcomerDeploy()
		{
			RemoveAll();
			CharacterChainObject chain = GetChain<CharacterChainObject>();
			CharacterData newcomerData = _selectProcess.GetNewcomerData(_playerIndex);
			if (newcomerData.Data != null && newcomerData.UserChara != null)
			{
				CharacterMapColorData mapColorData = _selectProcess.GetMapColorData(newcomerData.Data.genre.id);
				Texture2D texture = newcomerData.Texture;
				Sprite face = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
				chain.SetData((int)newcomerData.UserChara.Level, newcomerData.UserChara.NextAwakePercent, (int)newcomerData.UserChara.Awakening, face, mapColorData, newcomerData.ShadowColor);
				bool flag = _selectProcess.IsMatchColor(_playerIndex, newcomerData.UserChara.ID);
				WeakPoint weakPoint = ((!flag) ? WeakPoint.Weak : WeakPoint.Forte);
				bool isLeaderChange = IsLeaderChange;
				if (isLeaderChange)
				{
					weakPoint = WeakPoint.Leader;
				}
				int movementParam = (int)newcomerData.UserChara.GetMovementParam(flag, isLeaderChange);
				CharacterSelectProces.ArrowDirection arrowDirection = _selectProcess.GetArrowDirection(_playerIndex, movementParam);
				chain.SetInformation(_selectProcess.IsSelectedCharacter(_playerIndex, newcomerData.ID), newcomerData.Data.name.str, movementParam, weakPoint, arrowDirection);
				chain.SetVisibleCenterDecorate(isVisible: false);
				chain.OnCenterIn();
				SetSpot(4, chain);
			}
			base.Deploy();
		}

		public void FavoriteDeploy()
		{
			RemoveAll();
			int num = -4;
			for (int i = 0; i < 9; i++)
			{
				CharacterChainObject chain = GetChain<CharacterChainObject>();
				CharacterData userCharaData = _selectProcess.GetUserCharaData(_playerIndex, num);
				if (userCharaData.Data != null && userCharaData.UserChara != null)
				{
					CharacterMapColorData mapColorData = _selectProcess.GetMapColorData(userCharaData.Data.genre.id);
					Texture2D texture = userCharaData.Texture;
					Sprite face = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
					chain.SetData((int)userCharaData.UserChara.Level, userCharaData.UserChara.NextAwakePercent, (int)userCharaData.UserChara.Awakening, face, mapColorData, userCharaData.ShadowColor);
					bool flag = _selectProcess.IsMatchColor(_playerIndex, userCharaData.UserChara.ID);
					WeakPoint weakPoint = ((!flag) ? WeakPoint.Weak : WeakPoint.Forte);
					bool isLeaderChange = IsLeaderChange;
					if (isLeaderChange)
					{
						weakPoint = WeakPoint.Leader;
					}
					int movementParam = (int)userCharaData.UserChara.GetMovementParam(flag, isLeaderChange);
					CharacterSelectProces.ArrowDirection arrowDirection = _selectProcess.GetArrowDirection(_playerIndex, movementParam);
					chain.SetInformation(_selectProcess.IsSelectedCharacter(_playerIndex, userCharaData.ID), userCharaData.Data.name.str, movementParam, weakPoint, arrowDirection);
				}
				else
				{
					chain.SetBlank();
				}
				if (i == 4)
				{
					chain.OnCenterIn();
				}
				else
				{
					chain.OnCenterOut();
				}
				SetSpot(i, chain);
				num++;
			}
			base.Deploy();
		}

		public void SetFavorite(bool isFavorite)
		{
		}

		protected override void Next(int targetIndex, Direction direction)
		{
			int num = 0;
			for (int i = 0; i < 9; i++)
			{
				if (SpotArray[i] is SeparateChainObject)
				{
					if (i < 4 && direction == Direction.Right)
					{
						num++;
					}
					else if (4 < i && direction == Direction.Left)
					{
						num--;
					}
				}
			}
			int index = ((direction != Direction.Right) ? 8 : 0);
			if (_selectProcess.IsChangeCharacterCategory(_playerIndex, targetIndex, out var overCount))
			{
				string characterCategoryName = _selectProcess.GetCharacterCategoryName(_playerIndex, overCount - 1);
				string characterCategoryName2 = _selectProcess.GetCharacterCategoryName(_playerIndex, overCount);
				SetSpot(index, GetSeparate(characterCategoryName2, characterCategoryName));
				return;
			}
			CharacterChainObject chain = GetChain<CharacterChainObject>();
			CharacterData userCharaData = _selectProcess.GetUserCharaData(_playerIndex, targetIndex + num);
			if (userCharaData.Data != null && userCharaData.UserChara != null)
			{
				CharacterMapColorData mapColorData = _selectProcess.GetMapColorData(userCharaData.Data.genre.id);
				Texture2D texture = userCharaData.Texture;
				Sprite face = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
				chain.SetData((int)userCharaData.UserChara.Level, userCharaData.UserChara.NextAwakePercent, (int)userCharaData.UserChara.Awakening, face, mapColorData, userCharaData.ShadowColor);
				bool flag = _selectProcess.IsMatchColor(_playerIndex, userCharaData.UserChara.ID);
				WeakPoint weakPoint = ((!flag) ? WeakPoint.Weak : WeakPoint.Forte);
				bool isLeaderChange = IsLeaderChange;
				if (isLeaderChange)
				{
					weakPoint = WeakPoint.Leader;
				}
				int movementParam = (int)userCharaData.UserChara.GetMovementParam(flag, isLeaderChange);
				CharacterSelectProces.ArrowDirection arrowDirection = _selectProcess.GetArrowDirection(_playerIndex, movementParam);
				chain.SetInformation(_selectProcess.IsSelectedCharacter(_playerIndex, userCharaData.ID), userCharaData.Data.name.str, movementParam, weakPoint, arrowDirection);
			}
			else
			{
				chain.SetBlank();
			}
			chain.OnCenterOut();
			SetSpot(index, chain);
		}

		private bool IsFavorite(UserChara chara)
		{
			return CharacterSelectMonitor.IsFavorite(_playerIndex, chara);
		}
	}
}
