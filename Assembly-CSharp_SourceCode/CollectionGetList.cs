using System;
using System.Collections.Generic;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager;
using UnityEngine;
using UnityEngine.UI;

public class CollectionGetList : MonoBehaviour
{
	private const int MaxOneLineConstraintCount = 5;

	private const int MaxItemNum = 10;

	private const float DelayTime = 0.05f;

	[SerializeField]
	private GameObject _cardObj;

	[SerializeField]
	private Transform _cardRootTran;

	[SerializeField]
	private GridLayoutGroup _layoutGroup;

	[SerializeField]
	private AnimationParts _getMore;

	[SerializeField]
	private Sprite _titleReplaceSp;

	private List<CollectionGetCard> _cardList = new List<CollectionGetCard>();

	private Action _next;

	private bool _isAnimation;

	private int _currentIndex;

	private float _delayCounter;

	private int _monitorIndex;

	private AssetManager _assetManager;

	private bool _isGetMore;

	public bool Prepare(int monitorIndex, AssetManager assetManager)
	{
		_monitorIndex = monitorIndex;
		_assetManager = assetManager;
		UserData userData = Singleton<UserDataManager>.Instance.GetUserData(monitorIndex);
		int num = 0;
		List<int> newIconList = userData.NewIconList;
		for (int i = 0; i < newIconList.Count; i++)
		{
			if (10 <= num)
			{
				break;
			}
			int id = newIconList[i];
			Texture2D iconTexture2D = _assetManager.GetIconTexture2D(_monitorIndex, id);
			Sprite sprite = Sprite.Create(iconTexture2D, new Rect(0f, 0f, iconTexture2D.width, iconTexture2D.height), Vector2.zero);
			AddCard(CollectionGetCard.CardType.Icon, sprite);
			num++;
		}
		List<int> newPlateList = userData.NewPlateList;
		for (int j = 0; j < newPlateList.Count; j++)
		{
			if (10 <= num)
			{
				break;
			}
			int id2 = newPlateList[j];
			Texture2D plateTexture2D = _assetManager.GetPlateTexture2D(id2);
			Sprite sprite2 = Sprite.Create(plateTexture2D, new Rect(0f, 0f, plateTexture2D.width, plateTexture2D.height), Vector2.zero);
			AddCard(CollectionGetCard.CardType.NamePlate, sprite2);
			num++;
		}
		List<int> newTitleList = userData.NewTitleList;
		for (int k = 0; k < newTitleList.Count; k++)
		{
			if (10 <= num)
			{
				break;
			}
			AddCard(CollectionGetCard.CardType.Title, null);
			num++;
		}
		List<int> newPartnerList = userData.NewPartnerList;
		for (int l = 0; l < newPartnerList.Count; l++)
		{
			if (10 <= num)
			{
				break;
			}
			int id3 = newPartnerList[l];
			Texture2D partnerTexture2D = _assetManager.GetPartnerTexture2D(id3);
			Sprite sprite3 = Sprite.Create(partnerTexture2D, new Rect(0f, 0f, partnerTexture2D.width, partnerTexture2D.height), Vector2.zero);
			AddCard(CollectionGetCard.CardType.Partner, sprite3);
			num++;
		}
		List<int> newFrameList = userData.NewFrameList;
		for (int m = 0; m < newFrameList.Count; m++)
		{
			if (10 <= num)
			{
				break;
			}
			int id4 = newFrameList[m];
			Texture2D frameThumbTexture2D = _assetManager.GetFrameThumbTexture2D(id4);
			Sprite sprite4 = Sprite.Create(frameThumbTexture2D, new Rect(0f, 0f, frameThumbTexture2D.width, frameThumbTexture2D.height), Vector2.zero);
			AddCard(CollectionGetCard.CardType.Frame, sprite4);
			num++;
		}
		List<int> newTicketList = userData.NewTicketList;
		for (int n = 0; n < newTicketList.Count; n++)
		{
			if (10 <= num)
			{
				break;
			}
			int ticketId = newTicketList[n];
			Sprite sprite5 = Resources.Load<Sprite>(Singleton<TicketManager>.Instance.GetCollectionFileName(ticketId));
			if (sprite5 != null)
			{
				Texture2D texture = sprite5.texture;
				Sprite sprite6 = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
				AddCard(CollectionGetCard.CardType.Ticket, sprite6);
				num++;
			}
		}
		int num2 = newIconList.Count + newTitleList.Count + newPlateList.Count + newPartnerList.Count + newFrameList.Count + newTicketList.Count;
		_isGetMore = 10 < num2;
		int count = _cardList.Count;
		if (5 < count && count <= 10)
		{
			int num3 = 10 - count;
			for (int num4 = 0; num4 < num3; num4++)
			{
				AddCard(CollectionGetCard.CardType.Blank, null);
			}
		}
		SetGridLayoutGroup(_cardList.Count);
		_getMore.Play("Idle");
		return 0 < num2;
	}

	private void PlayGetMoreAnim()
	{
		if (_isGetMore)
		{
			_getMore.Play("In");
		}
	}

	private void AddCard(CollectionGetCard.CardType type, Sprite sprite)
	{
		CollectionGetCard component = UnityEngine.Object.Instantiate(_cardObj, _cardRootTran).GetComponent<CollectionGetCard>();
		switch (type)
		{
		case CollectionGetCard.CardType.Icon:
			component.SetCard(CollectionGetCard.CardType.Icon, sprite);
			break;
		case CollectionGetCard.CardType.NamePlate:
			component.SetCard(CollectionGetCard.CardType.NamePlate, sprite);
			break;
		case CollectionGetCard.CardType.Title:
			component.SetCard(CollectionGetCard.CardType.Title, _titleReplaceSp);
			break;
		case CollectionGetCard.CardType.Partner:
			component.SetCard(CollectionGetCard.CardType.Partner, sprite);
			break;
		case CollectionGetCard.CardType.Frame:
			component.SetCard(CollectionGetCard.CardType.Frame, sprite);
			break;
		case CollectionGetCard.CardType.Ticket:
			component.SetCard(CollectionGetCard.CardType.Ticket, sprite);
			break;
		case CollectionGetCard.CardType.Blank:
			component.BlankSet();
			break;
		}
		_cardList.Add(component);
	}

	public void PrepareAddAnimation(Action next)
	{
		_next = next;
		_isAnimation = true;
		_cardList[_currentIndex].Play();
		_delayCounter = 0f;
		if (!_cardList[_currentIndex].IsBlank)
		{
			SoundManager.PlaySE(Cue.SE_COLLECTION_ALLGET_APPEAR, _monitorIndex);
		}
	}

	public void PlayAddAnimation(float deltaTime)
	{
		if (!_isAnimation)
		{
			return;
		}
		if (0.05f < _delayCounter)
		{
			_delayCounter = 0f;
			_currentIndex++;
			if (10 < _cardList.Count || _cardList.Count <= _currentIndex)
			{
				_isAnimation = false;
				_next?.Invoke();
				PlayGetMoreAnim();
				_next = null;
				return;
			}
			_cardList[_currentIndex].Play();
			if (!_cardList[_currentIndex].IsBlank)
			{
				SoundManager.PlaySE(Cue.SE_COLLECTION_ALLGET_APPEAR, _monitorIndex);
			}
		}
		_delayCounter += deltaTime;
	}

	private void SetGridLayoutGroup(int collectionNum)
	{
		if (collectionNum != 0)
		{
			_layoutGroup.constraintCount = ((5 <= collectionNum) ? 5 : collectionNum);
		}
	}
}
