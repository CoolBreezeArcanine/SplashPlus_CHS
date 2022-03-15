using System;
using System.Collections.Generic;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using UI.DaisyChainList;
using UnityEngine;

public class CollectionChainList : DaisyChainList
{
	private const string TITLE_SPRITE_PATH = "Process/Common/Sprites/UpperMonitor/UI_CMN_Shougou_";

	private const int ErrorValue = -999;

	protected ICollectionProcess _process;

	protected AssetManager _assetManager;

	protected int _monitorId;

	private CollectionProcess.SubSequence _currentSequence;

	private CollectionGenreID _currentCollectionType;

	private Dictionary<TrophyRareType, Sprite> _trophyTypeSprites = new Dictionary<TrophyRareType, Sprite>();

	public virtual void AdvancedInitialize(ICollectionProcess process, AssetManager manager, int monitorId)
	{
		_process = process;
		_monitorId = monitorId;
		_assetManager = manager;
		LoadSprite();
	}

	public bool IsCrossBoundary(Direction direction)
	{
		return SpotArray[(int)(4 + direction)] is SeparateChainObject;
	}

	public override void Deploy()
	{
		RemoveAll();
		_currentSequence = _process.CurrentSubSequence(_monitorId);
		_currentCollectionType = _process.CurrentColletionType(_monitorId);
		if (_currentSequence == CollectionProcess.SubSequence.SelectCollectionType || _currentSequence == CollectionProcess.SubSequence.Information)
		{
			DeployOnCollectionType();
		}
		else
		{
			DeployOnCollection();
		}
		base.Deploy();
	}

	protected override void Next(int targetIndex, Direction direction)
	{
		if (_currentSequence == CollectionProcess.SubSequence.Icon || _currentSequence == CollectionProcess.SubSequence.Title || _currentSequence == CollectionProcess.SubSequence.NamePlate || _currentSequence == CollectionProcess.SubSequence.Prtner || _currentSequence == CollectionProcess.SubSequence.Frame)
		{
			NextOnCollection(targetIndex, direction);
		}
		else if (_currentSequence == CollectionProcess.SubSequence.SelectCollectionType || _currentSequence == CollectionProcess.SubSequence.Information)
		{
			NextOnCollectionType(targetIndex, direction);
		}
	}

	public void ChangeEquipmentIcon()
	{
		for (int i = 0; i < 9; i++)
		{
			if (SpotArray[i] != null && SpotArray[i] is CollectionChainCard)
			{
				((CollectionChainCard)SpotArray[i]).SetVisibleSetIcon(i == 4);
			}
		}
	}

	public override void SetScrollCard(bool isVisible)
	{
		if (isVisible)
		{
			CollectionChainCard collectionChainCard = (CollectionChainCard)ScrollChainCard;
			collectionChainCard.ChangeSize(isMainActive: true);
			SetCollectionData(collectionChainCard, 0);
		}
		base.SetScrollCard(isVisible);
	}

	private void NextOnCollection(int targetIndex, Direction direction)
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
				else if (i > 4 && direction == Direction.Left)
				{
					num--;
				}
			}
		}
		int index = ((direction != Direction.Right) ? 8 : 0);
		if (IsBoundary(4 * (int)direction, out var overCount))
		{
			string right = _process.CategoryName(_monitorId, overCount - 1);
			string left = _process.CategoryName(_monitorId, overCount);
			SetSpot(index, GetSeparate(left, right));
		}
		else
		{
			CollectionChainCard chain = GetChain<CollectionChainCard>();
			SetCollectionData(chain, targetIndex + num);
			chain.ChangeSize(isMainActive: false);
			SetSpot(index, chain);
		}
	}

	private void NextOnCollectionType(int targetIndex, Direction direction)
	{
		int num = (int)_process.CurrentColletionType(_monitorId);
		int index = ((direction != Direction.Right) ? 8 : 0);
		int num2 = targetIndex + num;
		if (num2 >= 0 && 6 > num2)
		{
			SetCollectionData(null, index, num2);
		}
	}

	private void DeployOnCollection()
	{
		int num = 0;
		int overCount;
		for (int i = 0; i < 4; i++)
		{
			int index = -4 + i;
			if (IsBoundary(index, out overCount))
			{
				num++;
			}
		}
		int num2 = -4 + num;
		int num3 = 0;
		int num4 = 0;
		for (int j = 0; j < 9; j++)
		{
			int num5 = -4 + j;
			if (IsBoundary(num5, out overCount))
			{
				string right;
				string left;
				if (num5 > 0)
				{
					right = _process.CategoryName(_monitorId, overCount + 1);
					left = _process.CategoryName(_monitorId, overCount);
				}
				else
				{
					right = _process.CategoryName(_monitorId, overCount - 1);
					left = _process.CategoryName(_monitorId, overCount);
				}
				num3++;
				SetSpot(j, GetSeparate(left, right));
			}
			else
			{
				CollectionChainCard chain = GetChain<CollectionChainCard>();
				SetCollectionData(chain, num2);
				chain.ChangeSize(j == 4);
				SetSpot(j, chain);
				num4++;
				num2++;
			}
		}
	}

	private void DeployOnCollectionType()
	{
		int num = (int)_process.CurrentColletionType(_monitorId);
		int num2 = num - 4;
		if (num2 < 0)
		{
			num2 = 0;
		}
		int num3 = 4 - num;
		if (num3 < 0)
		{
			num3 = 0;
		}
		for (int i = num3; i < 9; i++)
		{
			SetChainData(num2, i);
			num2++;
			if (num2 >= 6)
			{
				break;
			}
		}
	}

	private void SetChainData(int typeCount, int index)
	{
		if (6 <= typeCount)
		{
			return;
		}
		CollectionChainCard chain = GetChain<CollectionChainCard>();
		CollectionGenreID collectionGenreID = CollectionGenreID.End;
		string text = "";
		bool isHaveNewCard = false;
		string acquisitionText = "";
		CollectionData collectionData = null;
		bool flag = false;
		switch (typeCount)
		{
		case 0:
		{
			collectionGenreID = CollectionGenreID.Title;
			int equipTitleID = Singleton<UserDataManager>.Instance.GetUserData(_monitorId).Detail.EquipTitleID;
			collectionData = _process.GetTitleById(_monitorId, equipTitleID);
			if (collectionData != null)
			{
				text = collectionData.NameStr;
				acquisitionText = collectionData.NormText;
				isHaveNewCard = _process.IsHaveNewTitle(_monitorId);
				Sprite titleBaseSprite = _trophyTypeSprites[collectionData.TrophyRareType];
				chain.SetDetailParts(text, titleBaseSprite);
			}
			break;
		}
		case 1:
		{
			collectionGenreID = CollectionGenreID.Icon;
			int equipIconID = Singleton<UserDataManager>.Instance.GetUserData(_monitorId).Detail.EquipIconID;
			collectionData = _process.GetIconById(_monitorId, equipIconID);
			if (collectionData != null)
			{
				text = collectionData.NameStr;
				acquisitionText = collectionData.NormText;
				isHaveNewCard = _process.IsHaveNewIcon(_monitorId);
				Texture2D iconTexture2D = _assetManager.GetIconTexture2D(_monitorId, collectionData.GetID());
				chain.SetDetailParts(iconTexture2D, collectionGenreID);
			}
			break;
		}
		case 2:
		{
			collectionGenreID = CollectionGenreID.Plate;
			int equipPlateID = Singleton<UserDataManager>.Instance.GetUserData(_monitorId).Detail.EquipPlateID;
			collectionData = _process.GetPlateById(_monitorId, equipPlateID);
			if (collectionData != null)
			{
				text = collectionData.NameStr;
				acquisitionText = collectionData.NormText;
				isHaveNewCard = _process.IsHaveNewNamePlate(_monitorId);
				Texture2D plateTexture2D = _assetManager.GetPlateTexture2D(collectionData.GetID());
				chain.SetDetailParts(plateTexture2D, collectionGenreID);
			}
			break;
		}
		case 4:
		{
			collectionGenreID = CollectionGenreID.Partner;
			int equipPartnerID = Singleton<UserDataManager>.Instance.GetUserData(_monitorId).Detail.EquipPartnerID;
			collectionData = _process.GetPartnerById(_monitorId, equipPartnerID);
			if (collectionData != null)
			{
				text = collectionData.NameStr;
				acquisitionText = collectionData.NormText;
				isHaveNewCard = _process.IsHaveNewPartner(_monitorId);
				Texture2D partnerTexture2D = _assetManager.GetPartnerTexture2D(collectionData.GetID());
				chain.SetDetailParts(partnerTexture2D, collectionGenreID);
			}
			break;
		}
		case 3:
		{
			collectionGenreID = CollectionGenreID.Frame;
			int equipFrameID = Singleton<UserDataManager>.Instance.GetUserData(_monitorId).Detail.EquipFrameID;
			collectionData = _process.GetFrameById(_monitorId, equipFrameID);
			if (collectionData != null)
			{
				text = collectionData.NameStr;
				acquisitionText = collectionData.NormText;
				isHaveNewCard = _process.IsHaveNewFrame(_monitorId);
				Texture2D frameThumbTexture2D = _assetManager.GetFrameThumbTexture2D(collectionData.GetID());
				chain.SetDetailParts(frameThumbTexture2D, collectionGenreID);
				FrameData frame = Singleton<DataManager>.Instance.GetFrame(collectionData.GetID());
				if (frame != null && frame.isEffect)
				{
					flag = true;
				}
			}
			break;
		}
		case 5:
		{
			collectionGenreID = CollectionGenreID.Exit;
			text = CollectionGenreID.Exit.GetGenreName();
			acquisitionText = CollectionGenreID.Exit.GetGenreName();
			Texture2D tex = Resources.Load<Texture2D>("Process/Collection/Sprites/UI_CLC_Exit");
			chain.SetDetailParts(tex, collectionGenreID);
			break;
		}
		}
		chain.SetCurrentParts(collectionGenreID, text, acquisitionText, isHaveNewCard, flag);
		if (flag && collectionData != null)
		{
			bool isEquipment = false;
			bool flag2 = true;
			bool flag3 = false;
			if (collectionGenreID == CollectionGenreID.Frame)
			{
				chain.SetPartsData(CollectionGenreID.Frame, collectionData, isEquipment, _monitorId);
				flag3 = true;
			}
			if (flag3)
			{
				chain.SetGenreName(flag2);
				chain.SetGenreNameMiniCard(flag2);
			}
		}
		chain.ChangeSize(index == 4);
		SetSpot(index, chain);
	}

	private void SetCollectionData(CollectionChainCard card, int index, int loadIndex = 0)
	{
		CollectionData collectionData = null;
		switch (_currentSequence)
		{
		case CollectionProcess.SubSequence.Title:
			collectionData = _process.GetTitle(_monitorId, index);
			SetTitleData(card, collectionData);
			break;
		case CollectionProcess.SubSequence.Icon:
			collectionData = _process.GetIcon(_monitorId, index);
			SetIconData(card, collectionData);
			break;
		case CollectionProcess.SubSequence.NamePlate:
			collectionData = _process.GetPlate(_monitorId, index);
			SetPlateData(card, collectionData);
			break;
		case CollectionProcess.SubSequence.Prtner:
			collectionData = _process.GetPartner(_monitorId, index);
			SetPartnerData(card, collectionData);
			break;
		case CollectionProcess.SubSequence.Frame:
			collectionData = _process.GetFrame(_monitorId, index);
			SetFrameData(card, collectionData);
			break;
		case CollectionProcess.SubSequence.SelectCollectionType:
		case CollectionProcess.SubSequence.Information:
			SetChainData(loadIndex, index);
			break;
		case CollectionProcess.SubSequence.Exit:
			break;
		}
	}

	private void SetIconData(CollectionChainCard card, CollectionData data)
	{
		int equipIconID = Singleton<UserDataManager>.Instance.GetUserData(_monitorId).Detail.EquipIconID;
		if (data != null)
		{
			int iD = data.GetID();
			bool isEquipment = equipIconID == iD;
			Texture2D iconTexture2D = _assetManager.GetIconTexture2D(_monitorId, data.GetID());
			if (card != null)
			{
				card.SetPartsData(CollectionGenreID.Icon, data, isEquipment, _monitorId);
				card.SetDetailParts(iconTexture2D, CollectionGenreID.Icon);
			}
		}
	}

	private void SetTitleData(CollectionChainCard card, CollectionData data)
	{
		int equipTitleID = Singleton<UserDataManager>.Instance.GetUserData(_monitorId).Detail.EquipTitleID;
		if (data != null)
		{
			int iD = data.GetID();
			TrophyRareType trophyRareType = data.TrophyRareType;
			Sprite titleBaseSprite = _trophyTypeSprites[trophyRareType];
			bool isEquipment = equipTitleID == iD;
			string nameStr = data.NameStr;
			if (card != null)
			{
				card.SetPartsData(CollectionGenreID.Title, data, isEquipment, _monitorId);
				card.SetDetailParts(nameStr, titleBaseSprite);
			}
		}
	}

	private void SetPlateData(CollectionChainCard card, CollectionData data)
	{
		int equipPlateID = Singleton<UserDataManager>.Instance.GetUserData(_monitorId).Detail.EquipPlateID;
		if (data != null)
		{
			int iD = data.GetID();
			Texture2D plateTexture2D = _assetManager.GetPlateTexture2D(data.GetID());
			bool isEquipment = equipPlateID == iD;
			if (card != null)
			{
				card.SetPartsData(CollectionGenreID.Plate, data, isEquipment, _monitorId);
				card.SetDetailParts(plateTexture2D, CollectionGenreID.Plate);
			}
		}
	}

	private void SetPartnerData(CollectionChainCard card, CollectionData data)
	{
		int equipPartnerID = Singleton<UserDataManager>.Instance.GetUserData(_monitorId).Detail.EquipPartnerID;
		if (data != null)
		{
			int iD = data.GetID();
			Texture2D partnerTexture2D = _assetManager.GetPartnerTexture2D(data.GetID());
			bool isEquipment = equipPartnerID == iD;
			if (card != null)
			{
				card.SetPartsData(CollectionGenreID.Partner, data, isEquipment, _monitorId);
				card.SetDetailParts(partnerTexture2D, CollectionGenreID.Partner);
			}
		}
	}

	private void SetFrameData(CollectionChainCard card, CollectionData data)
	{
		int equipFrameID = Singleton<UserDataManager>.Instance.GetUserData(_monitorId).Detail.EquipFrameID;
		if (data != null)
		{
			int iD = data.GetID();
			Texture2D frameThumbTexture2D = _assetManager.GetFrameThumbTexture2D(data.GetID());
			bool isEquipment = equipFrameID == iD;
			if (card != null)
			{
				card.SetPartsData(CollectionGenreID.Frame, data, isEquipment, _monitorId);
				card.SetDetailParts(frameThumbTexture2D, CollectionGenreID.Frame);
			}
		}
	}

	private bool IsBoundary(int index, out int overCount)
	{
		switch (_currentCollectionType)
		{
		case CollectionGenreID.Icon:
			return _process.IsIconBoundary(_monitorId, index, out overCount);
		case CollectionGenreID.Title:
			return _process.IsTitleBoundary(_monitorId, index, out overCount);
		case CollectionGenreID.Plate:
			return _process.IsPlateBoundary(_monitorId, index, out overCount);
		case CollectionGenreID.Partner:
			return _process.IsPartnerBoundary(_monitorId, index, out overCount);
		case CollectionGenreID.Frame:
			return _process.IsFrameBoundary(_monitorId, index, out overCount);
		default:
			overCount = -999;
			return false;
		}
	}

	private void LoadSprite()
	{
		foreach (TrophyRareType value2 in Enum.GetValues(typeof(TrophyRareType)))
		{
			Sprite value = Resources.Load<Sprite>("Process/Common/Sprites/UpperMonitor/UI_CMN_Shougou_" + value2);
			_trophyTypeSprites[value2] = value;
		}
	}
}
