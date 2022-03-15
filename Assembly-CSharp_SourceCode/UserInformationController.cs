using DB;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Manager.UserDatas;
using Process;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInformationController : MonoBehaviour
{
	[SerializeField]
	protected RawImage userIconImage;

	[SerializeField]
	protected RawImage namePlate;

	[SerializeField]
	protected Mask namePlateMask;

	[SerializeField]
	protected RawImage frameImage;

	[SerializeField]
	protected bool _isKiraFrame;

	[SerializeField]
	[Header("キラフレーム")]
	protected KiraFrameImage _kiraFrameImage;

	[SerializeField]
	protected TextMeshProUGUI userNameText;

	[SerializeField]
	[Header("レーティング表示土台")]
	protected Image rateingBase;

	[SerializeField]
	protected SpriteCounter rateingText;

	[SerializeField]
	protected Image titleBaseImage;

	[SerializeField]
	protected CustomTextScroll titleText;

	[SerializeField]
	[Header("パス")]
	protected Image _passBase;

	[SerializeField]
	protected Image passImage;

	[SerializeField]
	[Header("段位バッチ")]
	protected GameObject udemaeBadgeObj;

	[SerializeField]
	protected Image _udemaeImage;

	[SerializeField]
	[Header("ランクバッチ")]
	protected GameObject rankBadgeObj;

	[SerializeField]
	protected Image rankImage;

	[SerializeField]
	protected Image readerIcon;

	[SerializeField]
	protected Image contributionImage;

	private RectTransform _rectTransform;

	private Material _material;

	public RectTransform RectTransform => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());

	public void SetUserData(MessageUserInformationData data)
	{
		_isKiraFrame = false;
		SetUserIcon(data.Icon);
		userNameText.text = data.Name;
		SetUserRating(data.Rateing);
		SetTitle(data.Title, data.TitleBg);
		SetNamePlate(data.Plate);
		SetFrame(data.Frame, data.IsKiraFrame, data.KiraFrame, data.KiraFrameMask);
		SetUdemae(data.ClassID);
		SetRank(data.DaniID);
		if (data.IsSubMonitor)
		{
			SetPass(data.Pass, data.BaseColor, data.PassMaterial);
			if (data.Pass == null)
			{
				passImage.gameObject.SetActive(value: false);
			}
		}
		SetDispRate(data.DispRate);
		if (null != namePlateMask)
		{
			namePlateMask.enabled = !data.IsSubMonitor;
		}
	}

	public void SetUserDataParts(int playerIndex, AssetManager manager, UserDetail data, UserOption option)
	{
		Texture2D iconTexture2D = manager.GetIconTexture2D(playerIndex, data.EquipIconID);
		Texture2D plateTexture2D = manager.GetPlateTexture2D(data.EquipPlateID);
		Texture2D frameTexture2D = manager.GetFrameTexture2D(data.EquipFrameID);
		bool flag = false;
		Texture2D texture2D = null;
		Texture2D texture2D2 = null;
		FrameData frame = Singleton<DataManager>.Instance.GetFrame(data.EquipFrameID);
		if (frame != null && frame.isEffect)
		{
			flag = true;
			texture2D = manager.GetFrameTexture2D(data.EquipFrameID);
			texture2D2 = manager.GetFrameMaskTexture2D(data.EquipFrameID);
		}
		else
		{
			flag = false;
			texture2D = null;
			texture2D2 = null;
		}
		TitleData title = Singleton<DataManager>.Instance.GetTitle(data.EquipTitleID);
		string str = title.name.str;
		Sprite titleBg = Resources.Load<Sprite>("Process/Common/Sprites/UpperMonitor/UI_CMN_Shougou_" + title.rareType);
		Sprite pass = Resources.Load<Sprite>("Process/Common/Sprites/UpperMonitor/UI_CMN_DXPass_Platinum");
		UdemaeID classRank = (UdemaeID)data.ClassRank;
		uint courseRank = data.CourseRank;
		SetUserData(iconTexture2D, plateTexture2D, data.UserName, data.Rating, str, classRank, courseRank, titleBg, pass, option.DispRate, frameTexture2D, flag, texture2D, texture2D2);
	}

	private void SetUserData(Texture2D userIcon, Texture2D plate, string userName, uint rateing, string title, UdemaeID classID, uint daniID, Sprite titleBg, Sprite pass, OptionDisprateID dispRate, Texture2D frame, bool isKira, Texture2D kira_source_tex, Texture2D kira_mask_tex)
	{
		SetUserIcon(userIcon);
		userNameText.text = userName;
		SetUserRating(rateing);
		SetTitle(title, titleBg);
		SetNamePlate(plate);
		SetFrame(frame, isKira, kira_source_tex, kira_mask_tex);
		SetDispRate(dispRate);
		SetPass(pass);
		SetUdemae(classID);
		SetRank(daniID);
	}

	public void ResetInformation()
	{
		SetUserIcon(null);
		userNameText.text = "";
		SetUserRating(0u);
		titleText.SetData("");
	}

	public void SetDispRate(OptionDisprateID dispRate)
	{
		switch (dispRate)
		{
		case OptionDisprateID.AllDisp:
			rateingBase?.gameObject.SetActive(value: true);
			if (null != udemaeBadgeObj)
			{
				udemaeBadgeObj.SetActive(value: true);
			}
			if (null != rankBadgeObj)
			{
				rankBadgeObj.SetActive(value: true);
			}
			break;
		case OptionDisprateID.DispRateDan:
			rateingBase?.gameObject.SetActive(value: true);
			if (null != udemaeBadgeObj)
			{
				udemaeBadgeObj.SetActive(value: true);
			}
			if (null != rankBadgeObj)
			{
				rankBadgeObj.SetActive(value: false);
			}
			break;
		case OptionDisprateID.DispRateClass:
			rateingBase?.gameObject.SetActive(value: true);
			if (null != udemaeBadgeObj)
			{
				udemaeBadgeObj.SetActive(value: false);
			}
			if (null != rankBadgeObj)
			{
				rankBadgeObj.SetActive(value: true);
			}
			break;
		case OptionDisprateID.DispDanClass:
			rateingBase?.gameObject.SetActive(value: false);
			if (null != udemaeBadgeObj)
			{
				udemaeBadgeObj.SetActive(value: true);
			}
			if (null != rankBadgeObj)
			{
				rankBadgeObj.SetActive(value: true);
			}
			break;
		case OptionDisprateID.DispRate:
			rateingBase?.gameObject.SetActive(value: true);
			if (null != udemaeBadgeObj)
			{
				udemaeBadgeObj.SetActive(value: false);
			}
			if (null != rankBadgeObj)
			{
				rankBadgeObj.SetActive(value: false);
			}
			break;
		case OptionDisprateID.DispDan:
			rateingBase?.gameObject.SetActive(value: false);
			if (null != udemaeBadgeObj)
			{
				udemaeBadgeObj.SetActive(value: true);
			}
			if (null != rankBadgeObj)
			{
				rankBadgeObj.SetActive(value: false);
			}
			break;
		case OptionDisprateID.DispClass:
			rateingBase?.gameObject.SetActive(value: false);
			if (null != udemaeBadgeObj)
			{
				udemaeBadgeObj.SetActive(value: false);
			}
			if (null != rankBadgeObj)
			{
				rankBadgeObj.SetActive(value: true);
			}
			break;
		case OptionDisprateID.Hide:
			rateingBase?.gameObject.SetActive(value: false);
			if (null != udemaeBadgeObj)
			{
				udemaeBadgeObj.SetActive(value: false);
			}
			if (null != rankBadgeObj)
			{
				rankBadgeObj.SetActive(value: false);
			}
			break;
		}
	}

	public void SetClassicMode(bool isActive)
	{
		rateingText.gameObject.SetActive(!isActive);
	}

	public void SetUserIcon(Texture2D texture)
	{
		if (userIconImage != null)
		{
			userIconImage.texture = texture;
		}
	}

	public void SetNamePlate(Texture2D texture)
	{
		if (namePlate != null)
		{
			namePlate.texture = texture;
		}
	}

	public void SetFrame(Texture2D texture, bool isKira, Texture2D kira_source_tex, Texture2D kira_mask_tex)
	{
		bool flag = false;
		_isKiraFrame = isKira;
		if (_isKiraFrame)
		{
			if (_kiraFrameImage != null)
			{
				if (kira_source_tex == null || kira_mask_tex == null)
				{
					_kiraFrameImage.gameObject.SetActive(value: false);
					flag = true;
				}
				else
				{
					Material material = Resources.Load<Material>("Common/Materials/UI_Frame/FX_Frame");
					if (material != null)
					{
						if (_material != null)
						{
							Object.DestroyImmediate(_material);
							_material = null;
						}
						_material = new Material(material);
						if (_material != null)
						{
							_kiraFrameImage.gameObject.GetComponent<Image>().material = _material;
						}
						_kiraFrameImage.gameObject.SetActive(value: true);
						_kiraFrameImage.SetSourceImage(kira_source_tex);
						_kiraFrameImage.SetSubTexImage(kira_mask_tex);
					}
				}
			}
			if (!(frameImage != null))
			{
				return;
			}
			frameImage.gameObject.SetActive(value: false);
			if (flag)
			{
				if (texture == null)
				{
					frameImage.gameObject.SetActive(value: false);
					return;
				}
				frameImage.gameObject.SetActive(value: true);
				frameImage.texture = texture;
			}
			return;
		}
		if (_kiraFrameImage != null)
		{
			_kiraFrameImage.gameObject.SetActive(value: false);
		}
		if (frameImage != null)
		{
			if (texture == null)
			{
				frameImage.gameObject.SetActive(value: false);
				return;
			}
			frameImage.gameObject.SetActive(value: true);
			frameImage.texture = texture;
		}
	}

	public void SetKiraFrame(Texture2D mainTex, Texture2D subTex)
	{
	}

	public void SetVisibleKiraFrame(bool isVisisble)
	{
		_kiraFrameImage?.gameObject.SetActive(isVisisble);
	}

	public void SetTitle(string title, Sprite titleBg)
	{
		if (titleText != null)
		{
			titleText.SetData(title);
			titleText.ResetPosition();
		}
		if (titleBaseImage != null)
		{
			titleBaseImage.sprite = titleBg;
		}
	}

	public void SetUserRating(uint rate)
	{
		if (rateingText != null)
		{
			rateingText.ChangeText(rate.ToString().PadLeft(5));
			rateingBase.sprite = Resources.Load<Sprite>("Common/Sprites/DXRating/UI_CMN_DXRating_S_" + ResultProcess.GetRatePlate(rate));
		}
	}

	public void SetUdemae(UdemaeID classID)
	{
		if (rankImage != null)
		{
			rankImage.sprite = Resources.Load<Sprite>($"Common/Sprites/Class/UI_CMN_Class_S_{classID.GetEnumValue():00}");
		}
	}

	public void SetRank(uint danibaseId)
	{
		if (_udemaeImage != null)
		{
			_udemaeImage.sprite = Resources.Load<Sprite>("Process/Course/Sprites/DaniPlate/UI_DNM_DaniPlate_" + danibaseId.ToString("00"));
		}
	}

	public void SetPass(Sprite sp)
	{
		if (passImage != null)
		{
			if (!passImage.gameObject.activeSelf)
			{
				passImage.gameObject.SetActive(value: true);
			}
			if (!_passBase.gameObject.activeSelf)
			{
				_passBase.gameObject.SetActive(value: true);
			}
			passImage.sprite = sp;
		}
	}

	public void SetPass(Sprite pass, Color baseColor, Material material)
	{
		bool flag = pass == null;
		passImage.gameObject.SetActive(!flag);
		_passBase.gameObject.SetActive(!flag);
		if (!flag)
		{
			_passBase.color = baseColor;
			passImage.sprite = pass;
			passImage.material = material;
		}
	}

	public void UpdateTextScroll()
	{
		if (titleText != null)
		{
			titleText.ViewUpdate();
		}
	}
}
