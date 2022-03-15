using Mai2.Mai2Cue;
using Manager;
using Process;
using Process.CodeRead;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserEntryPlate : MonoBehaviour
{
	private const string SPACE = "\u3000";

	private const string CLEAR_STRING = "<color=#00000000>Ａ</color>";

	[SerializeField]
	[Header("ユーザー情報")]
	private CanvasGroup _menuGroup;

	[SerializeField]
	[Tooltip("ユーザーアイコン")]
	private RawImage _userIconImage;

	[SerializeField]
	[Tooltip("ユーザー名")]
	private TextMeshProUGUI _userNameText;

	[SerializeField]
	[Tooltip("ユーザーレーティング土台")]
	private Image _userRatingBase;

	[SerializeField]
	[Tooltip("ユーザーレート")]
	private SpriteCounter _userRatingText;

	[SerializeField]
	[Tooltip("段位土台")]
	private GameObject _userDaniBase;

	[SerializeField]
	[Tooltip("累計転生回数")]
	private SpriteCounter _userTotalAwake;

	private SpriteCounter _userTotalAwakeBase;

	[SerializeField]
	private Animator _userInfomationAnimator;

	[SerializeField]
	[Header("でらっくパス")]
	private Animator _dxPathAnimator;

	[SerializeField]
	private Image _dxFrameImage;

	private int _monitorIndex;

	public void Initialize(int monitorIndex)
	{
		_monitorIndex = monitorIndex;
		_userTotalAwakeBase = _userTotalAwake?.transform.GetChild(0)?.gameObject.GetComponent<SpriteCounter>();
	}

	public void AnimIn()
	{
		_userInfomationAnimator.SetTrigger("In");
		SoundManager.PlaySE(Cue.SE_ENTRY_USERINFO, _monitorIndex);
	}

	public void AnimOut()
	{
		_userInfomationAnimator.SetTrigger("Out");
	}

	public void AnimLoop()
	{
		_userInfomationAnimator.Play("Loop");
	}

	public void AnimHide()
	{
		_userInfomationAnimator.Play(Animator.StringToHash("Hide"), 0, 0f);
	}

	public void SetUserData(Texture2D icon, string userName, int rating, int dispRate, int totalAwake)
	{
		_userIconImage.texture = icon;
		string text = userName.Replace("\u3000", "<color=#00000000>Ａ</color>");
		_userNameText.text = text;
		_userRatingText.ChangeText(rating.ToString().PadLeft(5));
		_userRatingBase.sprite = Resources.Load<Sprite>("Common/Sprites/DXRating/UI_CMN_DXRating_" + ResultProcess.GetRatePlate((uint)rating));
		bool active = false;
		bool active2 = false;
		switch (dispRate)
		{
		case 0:
			active = true;
			active2 = true;
			break;
		case 1:
			active = true;
			active2 = true;
			break;
		case 2:
			active = true;
			break;
		case 3:
			active2 = true;
			break;
		case 4:
			active = true;
			break;
		case 5:
			active2 = true;
			break;
		}
		_userRatingBase.gameObject.SetActive(active);
		_userDaniBase.SetActive(active2);
		_userTotalAwake.ChangeText(totalAwake.ToString().PadLeft(4));
		_userTotalAwakeBase?.ChangeText(totalAwake.ToString().PadLeft(4));
	}

	public void SetIconData(Texture2D icon)
	{
		_userIconImage.texture = icon;
	}

	public void SetDxPathData(Sprite frame, Material material)
	{
		_dxFrameImage.sprite = frame;
		_dxFrameImage.material = material;
	}

	public void SetDxPathStatus(CodeReadProcess.CardStatus status)
	{
		string text = "";
		switch (status)
		{
		case CodeReadProcess.CardStatus.Normal:
			text = "DXPass_Active";
			break;
		case CodeReadProcess.CardStatus.Expired:
			text = "DXPass_Expired";
			break;
		case CodeReadProcess.CardStatus.Unowned:
			text = "DXPass_None";
			break;
		}
		_dxPathAnimator.Play(Animator.StringToHash(text), 0, 0f);
	}
}
