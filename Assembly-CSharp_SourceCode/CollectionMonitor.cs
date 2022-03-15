using DB;
using IO;
using Mai2.Voice_Partner_000001;
using Manager;
using UI.Common;
using UI.DaisyChainList;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class CollectionMonitor : MonitorBase, ICollectionMonitor
{
	private Color[] _selectorBaseColors;

	[Space]
	[SerializeField]
	private CollectionButtonController _buttonController;

	[Space]
	[SerializeField]
	private CollectionInfoButtonController _buttonInfoController;

	[SerializeField]
	[Header("ChainLists")]
	private CollectionChainList _collectionChainList;

	[Space]
	[SerializeField]
	private GameObject _colletionAcquitionPlateObj;

	[SerializeField]
	private Transform _collectionAcquitionParent;

	[SerializeField]
	private CollectionTabController _collectionTabController;

	[SerializeField]
	private Image _titleImage;

	[SerializeField]
	private Animator _customInfoAnimController;

	[SerializeField]
	private Transform _selectorBackgroundObjectTargetParent;

	[SerializeField]
	private GameObject _originalSelectorBackgroundObject;

	[SerializeField]
	private CanvasGroup _blur;

	[SerializeField]
	private Image _exitInfo;

	private CollectionAcquitionPlate _collectionAcquitionPlate;

	private ICollectionProcess _process;

	private AssetManager _assetManager;

	private CollectionProcess.SubSequence _prevSubSequence;

	private SelectorBackgroundController _selectorBgController;

	private const string _invokeFirstInfoName = "DisplayInfoButton";

	private float _syncTimer;

	private bool _isAnimation;

	private bool _isStartSlideIn;

	private float _duration;

	private CollectionListAnimationController.AnimationType _animationType;

	public CollectionGenreID _initGenreID = CollectionGenreID.Invalid;

	private void Awake()
	{
		_collectionAcquitionPlate = Object.Instantiate(_colletionAcquitionPlateObj, _collectionAcquitionParent).GetComponent<CollectionAcquitionPlate>();
	}

	public void PrevInit(ICollectionProcess collectionProcess, AssetManager manager)
	{
		_process = collectionProcess;
		_assetManager = manager;
	}

	public void Initialize(int monIndex, bool active, CollectionProcess.SubSequence subSequence)
	{
		base.Initialize(monIndex, active);
		if (IsActive())
		{
			MechaManager.LedIf[monIndex].ButtonLedReset();
		}
		if (!active)
		{
			Main.gameObject.SetActive(value: false);
			Sub.gameObject.SetActive(value: false);
		}
		_buttonController.Initialize(monIndex);
		_buttonInfoController.Initialize(monIndex);
		SetVisibleBlur(subSequence != CollectionProcess.SubSequence.SelectCollectionType);
		if (active)
		{
			_collectionTabController.Initialize(monIndex);
			_collectionChainList.Initialize();
			_collectionChainList.AdvancedInitialize(_process, _assetManager, monIndex);
			_collectionChainList.Deploy();
			_selectorBgController = Object.Instantiate(_originalSelectorBackgroundObject, _selectorBackgroundObjectTargetParent).GetComponent<SelectorBackgroundController>();
			_selectorBaseColors = new Color[6];
			for (int i = 0; i < 6; i++)
			{
				_selectorBaseColors[i] = Utility.ConvertColor(((CollectionGenreID)i).GetMainColor());
			}
			_collectionAcquitionPlate.Prepare(CollectionProcess.SubSequence.SelectCollectionType, _process.GetTotalCollectionNum(monIndex));
			SetVisibleIndicator(isActive: false);
			SetScrollMessage(CommonMessageID.Scroll_Collection_Top.GetName());
			SetVisibleTitleImage(isActive: true);
			SetVisibleExitImage(isActive: true);
			_customInfoAnimController.gameObject.SetActive(value: true);
			_customInfoAnimController.Play("In");
		}
	}

	public void StartAnimation(CollectionGenreID current, CollectionProcess.SubSequence subSequence)
	{
		SetSelectorBaseColor((int)current);
		_buttonInfoController.SetVisibleImmediate(false, default(int));
		if (subSequence == CollectionProcess.SubSequence.Information)
		{
			_buttonController.SetVisibleImmediate(false, 0, 1, 2, 3);
			Invoke("DisplayInfoButton", 3f);
		}
		else
		{
			_buttonController.SetVisibleImmediate(false, 3);
			_buttonController.SetVisibleImmediate(true, 2);
			SetSelectionButton();
		}
	}

	public override void ViewUpdate()
	{
		_buttonController.ViewUpdate(GameManager.GetGameMSecAdd());
		UpdateListSlideAnimation();
		_collectionChainList.ViewUpdate();
		UpdateIndicator();
		UpdateScroll();
		_collectionTabController.UpdateButtonAnimation();
	}

	private void UpdateListSlideAnimation()
	{
		if (!_isAnimation)
		{
			return;
		}
		_syncTimer += GameManager.GetGameMSecAdd();
		if (_syncTimer / _duration >= 1f)
		{
			if (!_isStartSlideIn)
			{
				ReDeploy();
				PrepareSlideInAnim(_animationType);
				_isStartSlideIn = true;
				_syncTimer = 0f;
			}
			else
			{
				_isAnimation = false;
			}
		}
	}

	private void ReDeploy()
	{
		_collectionChainList.Deploy();
	}

	public void ChangeSubSequence(CollectionProcess.SubSequence subSequence)
	{
		PrepareSlideOutAnim(CollectionListAnimationController.AnimationType.ToUpper);
		SetCollectionAcquitionPlate(subSequence);
		switch (subSequence)
		{
		case CollectionProcess.SubSequence.SelectCollectionType:
			ReturnCollectionGenreID(_prevSubSequence);
			break;
		case CollectionProcess.SubSequence.Title:
			CommonCollectionGenreID2Collection();
			CreateAnnounceMessage(subSequence);
			SoundManager.PlayPartnerVoice(Cue.VO_000159, base.MonitorIndex);
			break;
		case CollectionProcess.SubSequence.Icon:
			CommonCollectionGenreID2Collection();
			CreateAnnounceMessage(subSequence);
			SoundManager.PlayPartnerVoice(Cue.VO_000157, base.MonitorIndex);
			break;
		case CollectionProcess.SubSequence.NamePlate:
			CommonCollectionGenreID2Collection();
			CreateAnnounceMessage(subSequence);
			SoundManager.PlayPartnerVoice(Cue.VO_000158, base.MonitorIndex);
			break;
		case CollectionProcess.SubSequence.Prtner:
			CommonCollectionGenreID2Collection();
			CreateAnnounceMessage(subSequence);
			SoundManager.PlayPartnerVoice(Cue.VO_000247, base.MonitorIndex);
			break;
		case CollectionProcess.SubSequence.Frame:
			CommonCollectionGenreID2Collection();
			CreateAnnounceMessage(subSequence);
			SoundManager.PlayPartnerVoice(Cue.VO_000253, base.MonitorIndex);
			break;
		}
		_prevSubSequence = subSequence;
	}

	private void SetCollectionAcquitionPlate(CollectionProcess.SubSequence subSequence)
	{
		int num = 0;
		_collectionAcquitionPlate.gameObject.SetActive(value: true);
		switch (subSequence)
		{
		case CollectionProcess.SubSequence.SelectCollectionType:
			num = _process.GetTotalCollectionNum(base.MonitorIndex);
			break;
		case CollectionProcess.SubSequence.Title:
			num = _process.GetAllTitleNum(base.MonitorIndex);
			break;
		case CollectionProcess.SubSequence.Icon:
			num = _process.GetAllIconNum(base.MonitorIndex);
			break;
		case CollectionProcess.SubSequence.NamePlate:
			num = _process.GetAllPlateNum(base.MonitorIndex);
			break;
		case CollectionProcess.SubSequence.Prtner:
			num = _process.GetAllPartnerNum(base.MonitorIndex);
			break;
		case CollectionProcess.SubSequence.Frame:
			num = _process.GetAllFrameNum(base.MonitorIndex);
			break;
		}
		_collectionAcquitionPlate.Prepare(subSequence, num);
	}

	public void ScrollInitialCollection()
	{
		_collectionChainList.Scroll(Direction.Left);
		SetScrollCard(isVisible: false);
	}

	public bool ScrollCollectionListRight(bool isLongTap)
	{
		bool result = false;
		if (_collectionChainList.IsCrossBoundary(Direction.Right))
		{
			if (isLongTap)
			{
				ReDeploy();
			}
			else
			{
				PrepareSlideOutAnim(CollectionListAnimationController.AnimationType.ToRight);
				result = true;
			}
			PrepareIndicator();
			_collectionTabController.Change(_process.CurrentCategoryIndex(base.MonitorIndex), Direction.Right);
		}
		else
		{
			_collectionChainList.Scroll(Direction.Right);
		}
		_buttonController.SetAnimationActive(1);
		SetScrollCard(isLongTap);
		return result;
	}

	public bool ScrollCollectionListLeft(bool isLongTap)
	{
		bool result = false;
		if (_collectionChainList.IsCrossBoundary(Direction.Left))
		{
			if (isLongTap)
			{
				ReDeploy();
			}
			else
			{
				PrepareSlideOutAnim(CollectionListAnimationController.AnimationType.ToLeft);
				result = true;
			}
			PrepareIndicator();
			_collectionTabController.Change(_process.CurrentCategoryIndex(base.MonitorIndex), Direction.Left);
		}
		else
		{
			_collectionChainList.Scroll(Direction.Left);
		}
		_buttonController.SetAnimationActive(0);
		SetScrollCard(isLongTap);
		return result;
	}

	public void ScrollCategoryRight()
	{
		_collectionTabController.Change(_process.CurrentCategoryIndex(base.MonitorIndex), Direction.Left);
		_collectionTabController.PressedTabButton(isRight: true);
		PrepareIndicator();
		PrepareSlideOutAnim(CollectionListAnimationController.AnimationType.ToRight);
	}

	public void ScrollCategoryLeft()
	{
		_collectionTabController.Change(_process.CurrentCategoryIndex(base.MonitorIndex), Direction.Right);
		_collectionTabController.PressedTabButton(isRight: false);
		PrepareIndicator();
		PrepareSlideOutAnim(CollectionListAnimationController.AnimationType.ToLeft);
	}

	public void SetScrollCard(bool isVisible)
	{
		_collectionChainList.SetScrollCard(isVisible);
	}

	public void ChangeCategoryExit()
	{
		_buttonController.ChangeFlatButtonSymbol(2, 2);
	}

	public void ChangeCategoryType()
	{
		_buttonController.ChangeFlatButtonSymbol(2, 24);
	}

	private void ReturnCollectionGenreID(CollectionProcess.SubSequence subSequence)
	{
		_buttonController.SetVisible(false, 3);
		_buttonController.SetAnimationActive(3);
		_buttonController.ChangeFlatButtonSymbol(2, 24);
		_collectionChainList.SetScrollCard(isVisible: false);
		_collectionTabController.PlayOutAnimation();
		SetVisibleIndicator(isActive: false);
		SetSelectorBaseColor((int)(subSequence - 1));
		SetCollectionGenreIDListButton(_process.ConvertToCollectionGenreID(subSequence));
		SetVisibleTitleImage(isActive: true);
		SetVisibleExitImage(isActive: true);
		SetScrollMessage(CommonMessageID.Scroll_Collection_Top.GetName());
		_customInfoAnimController.gameObject.SetActive(value: true);
		_customInfoAnimController.Play("In");
	}

	private void CommonCollectionGenreID2Collection()
	{
		_buttonController.SetAnimationActive(2);
		_buttonController.SetVisible(true, 0, 1);
		_buttonController.ChangeFlatButtonSymbol(3, 6);
		_buttonController.ChangeFlatButtonSymbol(2, 20);
		_collectionTabController.CollectionType2Collection(_process.GetTabDatas(base.MonitorIndex), _process.CurrentCategoryIndex(base.MonitorIndex));
		SetVisibleIndicator(isActive: true);
		PrepareIndicator();
		SetVisibleExitImage(isActive: false);
		_customInfoAnimController.gameObject.SetActive(value: true);
		_customInfoAnimController.Play("Out");
	}

	private void PrepareSlideOutAnim(CollectionListAnimationController.AnimationType animationType)
	{
		_isStartSlideIn = false;
		_animationType = animationType;
		_isAnimation = true;
		_syncTimer = 0f;
	}

	private void PrepareSlideInAnim(CollectionListAnimationController.AnimationType animationType)
	{
		_syncTimer = 0f;
	}

	public void PushDesitionButton(CollectionProcess.SubSequence subSequence)
	{
		_buttonController.SetAnimationActive(2);
		ChangeEquipmentIcon();
	}

	private void ChangeEquipmentIcon()
	{
		_collectionChainList.ChangeEquipmentIcon();
	}

	public void SetActiveButtonAnimation(int index)
	{
		_buttonController.SetAnimationActive(index);
	}

	public void SetCollectionGenreIDListButton(CollectionGenreID type)
	{
		switch (type)
		{
		case CollectionGenreID.Title:
			_buttonController.SetVisible(false, 1);
			break;
		case CollectionGenreID.Exit:
			_buttonController.SetVisible(false, default(int));
			break;
		default:
			_buttonController.SetVisible(true, 0, 1);
			break;
		}
		int num = 0;
		switch (type)
		{
		case CollectionGenreID.Title:
			num = 0;
			break;
		case CollectionGenreID.Icon:
			num = 1;
			break;
		case CollectionGenreID.Plate:
			num = 2;
			break;
		case CollectionGenreID.Frame:
			num = 3;
			break;
		case CollectionGenreID.Exit:
			num = 5;
			break;
		case CollectionGenreID.Partner:
			num = 4;
			break;
		}
		_selectorBgController.SetBackgroundColor(_selectorBaseColors[num]);
	}

	private void SetSelectorBaseColor(int colorIndex)
	{
		_selectorBgController.SetBackgroundColor(_selectorBaseColors[colorIndex]);
		_selectorBgController.Play(SelectorBackgroundController.AnimationType.In);
	}

	public void DisplayInfoButton()
	{
		_buttonInfoController.SetVisible(true, default(int));
	}

	public void SetVisibleButton(bool isActive, params int[] indexs)
	{
		_buttonController.SetVisible(isActive, indexs);
	}

	public void SetSelectionButton()
	{
		if (_initGenreID == CollectionGenreID.Title || _initGenreID == CollectionGenreID.Exit)
		{
			if (_initGenreID == CollectionGenreID.Title)
			{
				_buttonController.SetVisibleImmediate(true, default(int));
				_buttonController.SetVisibleImmediate(false, 1);
			}
			else
			{
				_buttonController.SetVisibleImmediate(false, default(int));
				_buttonController.SetVisibleImmediate(true, 1);
			}
		}
		else
		{
			_buttonController.SetVisibleImmediate(true, default(int));
			_buttonController.SetVisibleImmediate(true, 1);
		}
	}

	public void InformationToCollection()
	{
		if (IsInvoking("DisplayInfoButton"))
		{
			CancelInvoke("DisplayInfoButton");
		}
		else
		{
			_buttonInfoController.SetAnimationActive(0);
			_buttonInfoController.SetVisible(false, default(int));
		}
		_buttonController.SetVisible(true, 2);
		SetSelectionButton();
	}

	public void SetVisibleBlur(bool isActive)
	{
		_blur.alpha = (isActive ? 1 : 0);
	}

	private void UpdateIndicator()
	{
		int index = _process.CurrentCollectionIndex(base.MonitorIndex);
		_selectorBgController.UpdateIndicator(index);
	}

	private void PrepareIndicator()
	{
		int collectionIndex = _process.CurrentCollectionIndex(base.MonitorIndex);
		int num = _process.CurrentCategoryCollectionNum(base.MonitorIndex);
		if (num != 0)
		{
			_selectorBgController.PrepareIndicator(collectionIndex, num);
		}
	}

	private void SetVisibleIndicator(bool isActive)
	{
		_selectorBgController.SetVisibleIndicator(isActive);
	}

	private void CreateAnnounceMessage(CollectionProcess.SubSequence subSequence)
	{
		string scrollMessage = "";
		switch (subSequence)
		{
		case CollectionProcess.SubSequence.Title:
			scrollMessage = CommonMessageID.Scroll_Collection_Title.GetName();
			break;
		case CollectionProcess.SubSequence.Icon:
			scrollMessage = CommonMessageID.Scroll_Collection_Icon.GetName();
			break;
		case CollectionProcess.SubSequence.NamePlate:
			scrollMessage = CommonMessageID.Scroll_Collection_Nameplate.GetName();
			break;
		case CollectionProcess.SubSequence.Prtner:
			scrollMessage = CommonMessageID.Scroll_Collection_Partner.GetName();
			break;
		case CollectionProcess.SubSequence.Frame:
			scrollMessage = CommonMessageID.Scroll_Collection_Frame.GetName();
			break;
		}
		SetScrollMessage(scrollMessage);
	}

	private void SetScrollMessage(string message)
	{
		_selectorBgController.SetScrollMessage(message);
	}

	private void UpdateScroll()
	{
		_selectorBgController.UpdateScroll();
	}

	private void SetVisibleTitleImage(bool isActive)
	{
		_titleImage.color = (isActive ? Color.white : Color.clear);
	}

	private void SetVisibleExitImage(bool isActive)
	{
		_exitInfo.color = (isActive ? Color.white : Color.clear);
	}
}
