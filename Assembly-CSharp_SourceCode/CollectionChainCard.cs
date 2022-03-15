using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using UI;
using UI.DaisyChainList;
using UnityEngine;

public class CollectionChainCard : ChainObject
{
	private static readonly Vector2 DefaultSize = new Vector2(360f, 426f);

	private static readonly Vector2 ShortSize = new Vector2(360f, 366f);

	[SerializeField]
	[Header("Common")]
	private GameObject _originNewIconObj;

	[SerializeField]
	private Transform _newIconTran;

	[SerializeField]
	[Header("カードフレーム")]
	private MultipleImage _cardFrameImage;

	[SerializeField]
	private CollectionTypeParts _typeParts;

	[SerializeField]
	private CollectionParts _collectionParts;

	[SerializeField]
	private CollectionOtherParts _otherParts;

	[SerializeField]
	private CollectionName _collectionName;

	[SerializeField]
	[Header("MainCardのみ")]
	protected CanvasGroup _miniCardGroup;

	[SerializeField]
	private RectTransform _mainCardRect;

	[SerializeField]
	protected CanvasGroup _mainCardGroup;

	[SerializeField]
	protected CollectionChainCard _miniCard;

	private float _syncTimer;

	private GameObject _newIconObj;

	private Animator _newIconAnimator;

	protected override void Awake()
	{
		base.Awake();
		_newIconObj = Object.Instantiate(_originNewIconObj, _newIconTran);
		_newIconAnimator = _newIconObj.GetComponent<Animator>();
	}

	public void SetCurrentParts(CollectionGenreID type, string itemName, string acquisitionText, bool isHaveNewCard, bool isSpecial)
	{
		bool isTypeCard = true;
		SetCommonParts(type, itemName, acquisitionText, isHaveNewCard, isSpecial, isTypeCard);
		if (_mainCardRect != null)
		{
			_mainCardRect.sizeDelta = ShortSize;
		}
		_typeParts?.SetParts((int)type);
		if (_miniCard != null)
		{
			_miniCard.GetComponent<CollectionChainCard>().SetCurrentParts(type, itemName, acquisitionText, isHaveNewCard, isSpecial);
		}
	}

	public void SetPartsData(CollectionGenreID type, CollectionData data, bool isEquipment, int monitorIndex)
	{
		string nameStr = data.NameStr;
		string normText = data.NormText;
		bool isHave = data.IsHave;
		bool isNew = data.IsNew;
		bool isDisp = data.IsDisp;
		bool flag = !(!isHave && isDisp);
		bool isRandom = data.IsRandom;
		bool isSpecial = false;
		bool isTypeCard = false;
		data.GetID();
		if (type == CollectionGenreID.Frame)
		{
			FrameData frame = Singleton<DataManager>.Instance.GetFrame(data.GetID());
			if (frame != null && frame.isEffect)
			{
				isSpecial = true;
			}
		}
		SetCommonParts(type, nameStr, normText, isNew, isSpecial, isTypeCard);
		if (_mainCardRect != null)
		{
			_mainCardRect.sizeDelta = ((!isRandom && flag) ? DefaultSize : ShortSize);
		}
		_otherParts?.SetOtherParts(isHave, flag, isEquipment, isRandom, isSpecial, monitorIndex);
		if (_miniCard != null)
		{
			_miniCard.GetComponent<CollectionChainCard>().SetPartsData(type, data, isEquipment, monitorIndex);
		}
	}

	public void SetActiveChildren(GameObject obj, bool active)
	{
		int childCount = obj.transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.SetActive(active);
		}
	}

	public void SetGenreName(bool isActive)
	{
		_typeParts?.SetVisible(isActive);
	}

	public void SetGenreNameMiniCard(bool isActive)
	{
		if (_miniCard != null)
		{
			_miniCard.GetComponent<CollectionChainCard>().SetGenreName(isActive);
		}
	}

	private void SetCommonParts(CollectionGenreID type, string itemName, string conditionText, bool isNew, bool isSpecial, bool isTypeCard)
	{
		bool flag = false;
		SetCardFrameSprite(type);
		_collectionParts?.SetVisibleCollectionParts(type, isSpecial);
		SetCollectionNameAndAcquitionCondition(type, itemName, conditionText);
		SetVisibleNewIcon(isNew);
		SetGenreName(isTypeCard);
		flag = !isTypeCard;
		if (isSpecial)
		{
			flag = true;
		}
		_otherParts?.SetVisible(flag);
		if (_otherParts != null)
		{
			SetActiveChildren(_otherParts.gameObject, active: false);
		}
	}

	public void SetCardFrameSprite(CollectionGenreID type)
	{
		if (_cardFrameImage != null)
		{
			_cardFrameImage.ChangeSprite((int)type);
		}
	}

	public void SetDetailParts(Texture2D tex, CollectionGenreID type)
	{
		switch (type)
		{
		case CollectionGenreID.Icon:
		case CollectionGenreID.Exit:
			_collectionParts.SetIconTexture(tex);
			break;
		case CollectionGenreID.Plate:
		{
			Sprite plateSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), Vector2.zero);
			_collectionParts.SetPlateSprite(plateSprite);
			break;
		}
		case CollectionGenreID.Partner:
			_collectionParts.SetPartnerTexture(tex);
			break;
		case CollectionGenreID.Frame:
			_collectionParts.SetFrameTexture(tex);
			break;
		}
		_miniCard?.SetDetailParts(tex, type);
	}

	public void SetDetailParts(string title, Sprite titleBaseSprite)
	{
		_collectionParts.SetTitle(title, titleBaseSprite);
		_miniCard?.SetDetailParts(title, titleBaseSprite);
	}

	private void SetCollectionNameAndAcquitionCondition(CollectionGenreID type, string collectionName, string condition)
	{
		if (_collectionName != null)
		{
			_collectionName.Prepare(type, collectionName, condition);
		}
	}

	private void SetVisibleNewIcon(bool isActive)
	{
		if (_newIconObj != null)
		{
			_newIconObj.SetActive(isActive);
			if (isActive)
			{
				_newIconAnimator.Play(Animator.StringToHash("Loop"), 0, 0f);
			}
		}
	}

	public override void ViewUpdate(float gameMsecAdd)
	{
		base.ViewUpdate(gameMsecAdd);
		_syncTimer += (float)GameManager.GetGameMSecAdd() / 1000f;
		if (_otherParts.IsActive())
		{
			_otherParts.UpdateView(_syncTimer);
		}
		if (_collectionParts.IsActive())
		{
			_collectionParts.UpdateView(_syncTimer);
		}
		if (_collectionName != null)
		{
			_collectionName.ViewUpdate();
		}
		if (1f < _syncTimer)
		{
			_syncTimer = 0f;
		}
	}

	public void ChangeSize(bool isMainActive)
	{
		if (_miniCard != null)
		{
			_miniCardGroup.alpha = ((!isMainActive) ? 1 : 0);
			_mainCardGroup.alpha = (isMainActive ? 1 : 0);
		}
	}

	public override void OnCenterIn()
	{
		ChangeSize(isMainActive: true);
		ResetPosition();
	}

	public override void OnCenterOut()
	{
		ChangeSize(isMainActive: false);
		ResetPosition();
	}

	private void ResetPosition()
	{
		if (_collectionName != null)
		{
			_collectionName.ResetPosition();
		}
		if (_collectionParts.IsActive())
		{
			_collectionParts.ResetScrollText();
		}
	}

	public void SetVisibleSetIcon(bool isEquipment)
	{
		_otherParts?.SetVisibleSetIcon(isEquipment);
		if (_miniCard != null)
		{
			_miniCard.SetVisibleSetIcon(isEquipment);
		}
	}
}
