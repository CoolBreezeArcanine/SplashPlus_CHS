using System.Collections.Generic;
using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Mai2.Voice_000001;
using Mai2.Voice_Partner_000001;
using Manager;
using Manager.MaiStudio;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace Monitor.LoginBonus
{
	public class LoginBonusMonitor : MonitorBase
	{
		private enum MonitorState
		{
			None,
			Init,
			SelectCardFadeIn,
			SelectCardFadeInJudge,
			SelectCardFadeInJudgeWait,
			SelectCardFadeInWait,
			SelectCardWaitInit,
			SelectCardWait,
			SelectCardDecideInit,
			SelectCardDecide,
			SelectCardMoveRight,
			SelectCardMoveLeft,
			GetStampFadeIn,
			GetStampFadeInWait,
			GetStampOpen,
			GetStampOpenWait,
			GetStampAdd,
			GetStampAddWait,
			GetStampAddJudge,
			GetStampAddJudgeWait,
			GetCharacterFadeIn,
			GetCharacterFadeInWait,
			GetCharacterWait,
			GetCharacterFadeOut,
			GetCharacterFadeOutWait1,
			GetCharacterFadeOutWait2,
			GetCharacterFadeOutWait3,
			GetStampFadeOut,
			GetStampFadeOutWait,
			Finish,
			End
		}

		public enum InfoWindowState
		{
			None,
			Judge,
			Open,
			OpenWait,
			Wait,
			Close,
			CloseWait,
			End
		}

		[SerializeField]
		public LoginBonusButtonController _buttonController;

		[SerializeField]
		public GameObject _selectorBG;

		[SerializeField]
		public GameObject _masterNullCard;

		[SerializeField]
		public GameObject _masterNullChara;

		[SerializeField]
		public TextMeshProUGUI _infoText;

		[SerializeField]
		public TextMeshProUGUI _stampText;

		[SerializeField]
		public TextMeshProUGUI _characterSelectText;

		private MonitorState _state = MonitorState.End;

		private int _timer;

		private int _sub_count;

		private int _monitorID;

		public bool _isEntry;

		public bool _isGuest;

		private bool _isEnableOK;

		public bool _isCheckOK;

		public bool _isOK = true;

		private bool _isCheckLR;

		private bool _isRight = true;

		private bool _isLeft = true;

		public AssetManager _assetManager;

		private CommonBase m = new CommonBase();

		private GameObject _prefabCard;

		private GameObject _prefabCharacter;

		private GameObject _Character;

		private List<GameObject> _nullListCard = new List<GameObject>();

		private List<GameObject> _objListCard = new List<GameObject>();

		private List<Animator> _animListCard = new List<Animator>();

		private List<int> _objColorIDListCard = new List<int>();

		private List<CommonValue> _valueListCardPosX = new List<CommonValue>();

		private List<CommonValue> _valueListCardScaleXY = new List<CommonValue>();

		private List<int> _selectableColorIDListCard = new List<int>();

		private List<int> _loginBonusPartnerIDList = new List<int>();

		private string[] str_layer_color = new string[7] { "Color_01", "Color_02", "Color_03", "Color_04", "Color_05", "Color_06", "Color_08" };

		private int _dispCardMax = 5;

		private float[] _cardPosX = new float[5] { -880f, -440f, 0f, 440f, 880f };

		private float[] _cardScaleXY = new float[5] { 0.9f, 0.9f, 1f, 0.9f, 0.9f };

		private float _cardMoveTime = 30f;

		private int _cardPosEnd;

		private int _currentLayerColorID;

		private int _currentStampNum;

		public bool _isSetCurrentCard;

		public bool _isEnableTimer;

		public bool _isTimerVisible;

		public bool _isDecidedOK;

		public bool _isBothDecideOK;

		public bool _isTimeUp;

		public bool _isDispInfoWindow;

		public bool _isInvisibleButtonDispInfoWindow;

		public bool _isCallVoice;

		public bool _isInfoWindowVoice;

		public bool _isChangeSibling;

		public bool _isEnableLoginBonus;

		public int _EquippedPartnerVoiceID;

		public bool _isLastOne;

		public bool _isMajorVersionUp;

		public InfoWindowState _info_state;

		public uint _info_timer;

		public uint _info_count;

		private void Awake()
		{
			_infoText.text = CommonMessageID.LoginBonusInfo.GetName();
			_stampText.text = CommonMessageID.LoginBonusStmp.GetName();
			_characterSelectText.text = CommonMessageID.LoginBonusCharacterSelect.GetName();
		}

		public void OKButtonAnim(InputManager.ButtonSetting button)
		{
			int animationActive = 0;
			if (_isCheckOK && !_isOK)
			{
				_buttonController.SetAnimationActive(animationActive);
				_isOK = true;
			}
		}

		public void RightButtonAnim(InputManager.ButtonSetting button)
		{
			int animationActive = 1;
			if (_isCheckLR && !_isRight && !_isLeft)
			{
				_buttonController.SetAnimationActive(animationActive);
				_isRight = true;
			}
		}

		public void LeftButtonAnim(InputManager.ButtonSetting button)
		{
			int animationActive = 2;
			if (_isCheckLR && !_isRight && !_isLeft)
			{
				_buttonController.SetAnimationActive(animationActive);
				_isLeft = true;
			}
		}

		public void isOKTimerZero()
		{
			if (_isOK)
			{
				_timer = 0;
			}
		}

		public void OKStart()
		{
			if (!_isEnableOK)
			{
				_isEnableOK = true;
				_buttonController.SetVisible(true, default(int));
				_isCheckOK = true;
			}
		}

		public void ResetOKStart()
		{
			if (_isEnableOK)
			{
				_buttonController.SetVisible(false, default(int));
			}
			_isEnableOK = false;
			_isCheckOK = false;
			_isOK = false;
		}

		public void ResetLayerColor(Animator anim)
		{
			for (int i = 0; i < str_layer_color.Length; i++)
			{
				int layerIndex = anim.GetLayerIndex(str_layer_color[i]);
				anim.SetLayerWeight(layerIndex, 0f);
			}
		}

		public void SetAssetManager(AssetManager manager)
		{
			_assetManager = manager;
		}

		public GameObject GetBlurObject()
		{
			int index = 4;
			if (_isChangeSibling)
			{
				index = 3;
			}
			return _masterNullCard.transform.parent.gameObject.gameObject.transform.parent.gameObject.transform.GetChild(index).gameObject;
		}

		public void SetBlur()
		{
			GameObject blurObject = GetBlurObject();
			blurObject.SetActive(value: true);
			CanvasGroup component = blurObject.GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = 1f;
			}
		}

		public void ResetBlur()
		{
			GameObject blurObject = GetBlurObject();
			blurObject.SetActive(value: false);
			CanvasGroup component = blurObject.GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = 0f;
			}
		}

		public void SetLastBlur()
		{
			GameObject blurObject = GetBlurObject();
			int siblingIndex = blurObject.transform.GetSiblingIndex();
			blurObject.transform.SetSiblingIndex(siblingIndex + 2);
			blurObject.SetActive(value: true);
			CanvasGroup component = blurObject.GetComponent<CanvasGroup>();
			if (component != null)
			{
				component.alpha = 1f;
			}
		}

		public void SetLayerColorAnim(Animator anim, int color_id, string str, float f)
		{
			int layerIndex = anim.GetLayerIndex("Base Layer");
			anim.SetLayerWeight(layerIndex, 1f);
			anim.Play(str, layerIndex, f);
			ResetLayerColor(anim);
			layerIndex = anim.GetLayerIndex(str_layer_color[color_id]);
			anim.SetLayerWeight(layerIndex, 1f);
		}

		public int GetLayerColorID(int ref_id)
		{
			int index = ref_id;
			int result = -1;
			if (_selectableColorIDListCard.Count > 0)
			{
				if (ref_id < 0)
				{
					int num = -1 * ref_id;
					if (num >= _selectableColorIDListCard.Count)
					{
						num %= _selectableColorIDListCard.Count;
					}
					num = -1 * num;
					index = _selectableColorIDListCard.Count + num;
				}
				else if (ref_id >= _selectableColorIDListCard.Count)
				{
					index = ref_id % _selectableColorIDListCard.Count;
				}
				result = _selectableColorIDListCard[index];
			}
			return result;
		}

		public int GetLayerColorRefID(int color_id)
		{
			return _selectableColorIDListCard.FindIndex((int n) => n == color_id);
		}

		public Animator GetStampAnim(GameObject card, int id)
		{
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			GameObject gameObject3 = card.transform.GetChild(0).gameObject;
			switch (id)
			{
			case 3:
			case 4:
			case 8:
			case 9:
				gameObject = gameObject3.transform.GetChild(2).gameObject;
				gameObject2 = gameObject.transform.GetChild(3).gameObject;
				gameObject = gameObject2;
				break;
			case 0:
			case 1:
			case 2:
			case 5:
			case 6:
			case 7:
				gameObject = gameObject3.transform.GetChild(1).gameObject;
				gameObject2 = gameObject.transform.GetChild(0).gameObject;
				gameObject = gameObject2;
				break;
			}
			switch (id)
			{
			case 3:
				gameObject2 = gameObject.transform.GetChild(0).gameObject;
				break;
			case 4:
				gameObject2 = gameObject.transform.GetChild(1).gameObject;
				break;
			case 8:
				gameObject2 = gameObject.transform.GetChild(2).gameObject;
				break;
			case 9:
				gameObject2 = gameObject.transform.GetChild(3).gameObject;
				break;
			case 0:
				gameObject2 = gameObject.transform.GetChild(0).gameObject;
				break;
			case 1:
				gameObject2 = gameObject.transform.GetChild(1).gameObject;
				break;
			case 2:
				gameObject2 = gameObject.transform.GetChild(2).gameObject;
				break;
			case 5:
				gameObject2 = gameObject.transform.GetChild(3).gameObject;
				break;
			case 6:
				gameObject2 = gameObject.transform.GetChild(4).gameObject;
				break;
			case 7:
				gameObject2 = gameObject.transform.GetChild(5).gameObject;
				break;
			}
			return gameObject2.transform.GetChild(0).gameObject.GetComponent<Animator>();
		}

		public LoginBonusData GetPartnerData(int color_id)
		{
			int id = 1;
			if (color_id < _loginBonusPartnerIDList.Count)
			{
				id = _loginBonusPartnerIDList[color_id];
			}
			return Singleton<DataManager>.Instance.GetLoginBonus(id);
		}

		public void InitializeGetWindow()
		{
			GameObject gameObject = null;
			TextMeshProUGUI textMeshProUGUI = null;
			gameObject = _masterNullCard.gameObject.transform.parent.gameObject.gameObject.transform.parent.gameObject.transform.GetChild(2).gameObject;
			gameObject.SetActive(value: true);
			gameObject.transform.GetChild(4).gameObject.SetActive(value: true);
			LoginBonusData partnerData = GetPartnerData(_currentLayerColorID);
			int id = 1;
			if (partnerData != null)
			{
				id = partnerData.PartnerId.id;
			}
			PartnerData partner = Singleton<DataManager>.Instance.GetPartner(id);
			int id2 = 0;
			if (partner != null)
			{
				id2 = partner.naviChara.id;
			}
			textMeshProUGUI = gameObject.transform.GetChild(4).gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
			if (partner != null)
			{
				textMeshProUGUI.text = partner.name.str;
			}
			_prefabCharacter = AssetManager.Instance().GetNaviCharaPrefab(id2);
			_Character = Object.Instantiate(_prefabCharacter);
			m.SetEmptyPrefab(_masterNullChara, _prefabCharacter, _Character);
			PlayableDirector component = gameObject.GetComponent<PlayableDirector>();
			string text = monitorIndex.ToString("0");
			component.playableAsset = null;
			component.playableAsset = Resources.Load<PlayableAsset>("Process/LoginBonus/Animations/UI_GetWindow_LoginBonus_" + text);
			PlayableAsset playableAsset = component.playableAsset;
			int num = 0;
			num = 0;
			PlayableBinding playableBinding = default(PlayableBinding);
			foreach (PlayableBinding output in playableAsset.outputs)
			{
				if (num == 0)
				{
					playableBinding = output;
					break;
				}
				num++;
			}
			component.SetGenericBinding(playableBinding.sourceObject, gameObject.GetComponent<Animator>());
			AnimationClip[] animationClips = gameObject.GetComponent<Animator>().runtimeAnimatorController.animationClips;
			uint num2 = 0u;
			uint num3 = 0u;
			for (int i = 0; i < animationClips.Length; i++)
			{
				if (animationClips[i].name == "In")
				{
					num2 = (uint)i;
				}
				if (animationClips[i].name == "Out")
				{
					num3 = (uint)i;
				}
			}
			num = 0;
			foreach (TrackAsset outputTrack in ((TimelineAsset)component.playableAsset).GetOutputTracks())
			{
				if (num == 0)
				{
					foreach (TimelineClip clip in outputTrack.GetClips())
					{
						if (clip.displayName == "In")
						{
							((AnimationPlayableAsset)clip.asset).clip = animationClips[num2];
						}
						if (clip.displayName == "Out")
						{
							((AnimationPlayableAsset)clip.asset).clip = animationClips[num3];
						}
					}
					break;
				}
				num++;
			}
			num = 0;
			foreach (PlayableBinding output2 in playableAsset.outputs)
			{
				if (num == 1)
				{
					playableBinding = output2;
					break;
				}
				num++;
			}
			component.SetGenericBinding(playableBinding.sourceObject, _Character.GetComponent<Animator>());
			AnimationClip[] animationClips2 = _Character.GetComponent<Animator>().runtimeAnimatorController.animationClips;
			uint num4 = 0u;
			uint num5 = 0u;
			for (int j = 0; j < animationClips2.Length; j++)
			{
				if (animationClips2[j].name == "Navi_Default")
				{
					num4 = (uint)j;
				}
				if (animationClips2[j].name == "Navi_Welcom")
				{
					num5 = (uint)j;
				}
			}
			num = 0;
			foreach (TrackAsset outputTrack2 in ((TimelineAsset)component.playableAsset).GetOutputTracks())
			{
				if (num == 1)
				{
					foreach (TimelineClip clip2 in outputTrack2.GetClips())
					{
						if (clip2.displayName == "Navi_Default")
						{
							((AnimationPlayableAsset)clip2.asset).clip = animationClips2[num4];
						}
						if (clip2.displayName == "Navi_Welcom")
						{
							((AnimationPlayableAsset)clip2.asset).clip = animationClips2[num5];
						}
					}
					break;
				}
				num++;
			}
		}

		public override void Initialize(int monIndex, bool active)
		{
			_monitorID = monIndex;
			base.Initialize(_monitorID, active);
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(_monitorID);
			_isEntry = userData.IsEntry;
			_isGuest = userData.IsGuest();
			_currentLayerColorID = 0;
			_currentStampNum = 0;
			GameObject gameObject = null;
			GameObject gameObject2 = null;
			GameObject gameObject3 = null;
			GameObject obj = _masterNullCard.gameObject.transform.parent.gameObject;
			gameObject = obj.gameObject.transform.parent.gameObject;
			if (gameObject.transform.parent.transform.childCount >= 3)
			{
				gameObject.transform.parent.transform.GetChild(2).gameObject.SetActive(value: false);
			}
			gameObject3 = obj.transform.GetChild(0).gameObject;
			obj.transform.GetChild(1).gameObject.SetActive(value: false);
			obj.transform.GetChild(2).gameObject.SetActive(value: false);
			gameObject2 = gameObject.transform.GetChild(2).gameObject;
			gameObject2.SetActive(value: false);
			gameObject2.GetComponent<PlayableDirector>().Stop();
			if (!_isEntry || _isGuest)
			{
				for (int i = 0; i < gameObject.transform.childCount; i++)
				{
					gameObject.transform.GetChild(i).gameObject.SetActive(value: false);
				}
				_state = MonitorState.End;
				return;
			}
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, LoginBonusData> loginBonuse in Singleton<DataManager>.Instance.GetLoginBonuses())
			{
				int key = loginBonuse.Key;
				if (loginBonuse.Value.BonusType == LoginBonusType.Partner)
				{
					list.Add(key);
				}
			}
			int[] array = new int[7] { 11, 12, 13, 14, 15, 16, 18 };
			int num = -1;
			for (int j = 0; j < array.Length; j++)
			{
				num = array[j];
				for (int k = 0; k < list.Count; k++)
				{
					int num2 = list[k];
					LoginBonusData loginBonus = Singleton<DataManager>.Instance.GetLoginBonus(num2);
					if (loginBonus != null && loginBonus.PartnerId.id == num)
					{
						_loginBonusPartnerIDList.Add(num2);
						list.RemoveAt(k);
						break;
					}
				}
			}
			int num3 = 0;
			num3 = _loginBonusPartnerIDList.Count;
			for (int l = 0; l < list.Count; l++)
			{
				int item = list[l];
				_loginBonusPartnerIDList.Add(item);
			}
			num = -1;
			for (int m = 0; m < userData.LoginBonusList.Count; m++)
			{
				bool flag = false;
				UserLoginBonus userLoginBonus = userData.LoginBonusList[m];
				if (userLoginBonus == null || userLoginBonus.IsComplete || !userLoginBonus.IsCurrent || userLoginBonus.Point >= 10)
				{
					continue;
				}
				num = m;
				if (num == -1)
				{
					continue;
				}
				int num4 = 0;
				num4 = userLoginBonus.ID;
				for (int n = 0; n < num3; n++)
				{
					int color_id = n;
					LoginBonusData partnerData = GetPartnerData(color_id);
					if (partnerData != null && partnerData.GetID() == num4)
					{
						num = m;
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
				num = -1;
				userLoginBonus.IsCurrent = false;
			}
			if (num != -1)
			{
				UserLoginBonus userLoginBonus2 = userData.LoginBonusList[num];
				_isSetCurrentCard = true;
				_currentStampNum = (int)userLoginBonus2.Point;
				int num5 = 0;
				num5 = userLoginBonus2.ID;
				for (int num6 = 0; num6 < num3; num6++)
				{
					int num7 = num6;
					LoginBonusData partnerData2 = GetPartnerData(num7);
					if (partnerData2 != null && partnerData2.GetID() == num5)
					{
						_currentLayerColorID = num7;
						_isEnableLoginBonus = true;
						break;
					}
				}
			}
			else
			{
				_isSetCurrentCard = false;
				for (int num8 = 0; num8 < num3; num8++)
				{
					num = -1;
					int color_id2 = num8;
					LoginBonusData partnerData3 = GetPartnerData(color_id2);
					if (partnerData3 == null)
					{
						continue;
					}
					int num9 = 0;
					num9 = partnerData3.GetID();
					for (int num10 = 0; num10 < userData.LoginBonusList.Count; num10++)
					{
						UserLoginBonus userLoginBonus3 = userData.LoginBonusList[num10];
						if (userLoginBonus3 != null && userLoginBonus3.IsComplete && userLoginBonus3.ID == num9)
						{
							num = num8;
							break;
						}
					}
					if (num == -1)
					{
						_selectableColorIDListCard.Add(num8);
					}
				}
				_currentStampNum = 0;
				if (_selectableColorIDListCard.Count == 0)
				{
					_currentLayerColorID = 0;
					_isEnableLoginBonus = false;
				}
				else
				{
					_currentLayerColorID = _selectableColorIDListCard[0];
					_isEnableLoginBonus = true;
					if (_selectableColorIDListCard.Count <= 2)
					{
						int num11 = 0;
						switch (_selectableColorIDListCard.Count)
						{
						case 2:
							num11 = _selectableColorIDListCard[0];
							_selectableColorIDListCard.Add(num11);
							num11 = _selectableColorIDListCard[1];
							_selectableColorIDListCard.Add(num11);
							break;
						case 1:
							num11 = _selectableColorIDListCard[0];
							_selectableColorIDListCard.Add(num11);
							_selectableColorIDListCard.Add(num11);
							_isLastOne = true;
							break;
						}
					}
				}
			}
			bool flag2 = false;
			if (_isEnableLoginBonus && (_isMajorVersionUp || userData.UserType == UserData.UserIDType.Inherit || userData.UserType == UserData.UserIDType.New))
			{
				flag2 = true;
			}
			if (!Singleton<UserDataManager>.Instance.GetUserData(base.MonitorIndex).Detail.FirstPlayOnDay)
			{
				_isEnableLoginBonus = false;
			}
			if (flag2)
			{
				_isEnableLoginBonus = true;
			}
			if (!_isEnableLoginBonus)
			{
				gameObject = _masterNullCard.gameObject.transform.parent.gameObject.gameObject.transform.parent.gameObject;
				for (int num12 = 0; num12 < gameObject.transform.childCount; num12++)
				{
					gameObject.transform.GetChild(num12).gameObject.SetActive(value: false);
				}
				_state = MonitorState.End;
				return;
			}
			_EquippedPartnerVoiceID = userData.Detail.EquipPartnerID;
			if (!Singleton<UserDataManager>.Instance.GetUserData(_monitorID).Detail.ContentBit.IsFlagOn(ContentBitID.FirstLoginBonus))
			{
				_isInvisibleButtonDispInfoWindow = true;
			}
			_buttonController.Initialize(_monitorID);
			_isEnableOK = false;
			_isCheckOK = false;
			_isOK = false;
			if (!_isSetCurrentCard)
			{
				_buttonController.SetVisibleImmediate(false, default(int));
				_buttonController.SetVisibleImmediate(false, 1);
				_buttonController.SetVisibleImmediate(false, 2);
				_isCheckLR = false;
				_isRight = false;
				_isLeft = false;
			}
			else
			{
				_buttonController.SetVisibleImmediate(false, default(int));
				_buttonController.SetVisibleImmediate(false, 1);
				_buttonController.SetVisibleImmediate(false, 2);
				_isCheckLR = false;
				_isRight = false;
				_isLeft = false;
			}
			for (int num13 = 0; num13 < gameObject3.transform.childCount; num13++)
			{
				gameObject3.transform.GetChild(num13).gameObject.SetActive(value: false);
			}
			_prefabCard = Resources.Load<GameObject>("Process/LoginBonus/Prefabs/UI_Stampcard");
			if (!_isSetCurrentCard)
			{
				_masterNullCard.transform.GetChild(0).gameObject.SetActive(value: false);
				for (int num14 = 0; num14 < _dispCardMax; num14++)
				{
					this.m.SetEmptyWithPrefab(_masterNullCard.transform, _nullListCard, _objListCard, _prefabCard);
					_objListCard[num14].transform.GetChild(0).gameObject.SetActive(value: true);
					_animListCard.Add(_objListCard[num14].GetComponent<Animator>());
					_nullListCard[num14].transform.localPosition = new Vector3(_cardPosX[num14], 0f, 0f);
					_objListCard[num14].transform.localScale = new Vector3(_cardScaleXY[num14], _cardScaleXY[num14], 1f);
					GameObject obj2 = _objListCard[num14].transform.GetChild(0).gameObject;
					obj2.transform.GetChild(2).gameObject.transform.GetChild(3).gameObject.SetActive(value: false);
					obj2.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.SetActive(value: false);
				}
				int num15 = (_dispCardMax - 1) / 2;
				int num16 = -1 * num15;
				for (int num17 = 0; num17 < _dispCardMax; num17++)
				{
					Animator anim = _animListCard[num17];
					int num18 = 0;
					int ref_id = num16 + num17;
					num18 = GetLayerColorID(ref_id);
					_objColorIDListCard.Add(num18);
					SetLayerColorAnim(anim, num18, "Loop_Clause", 1f);
				}
				for (int num19 = 0; num19 < _dispCardMax; num19++)
				{
					CommonValue item2 = new CommonValue();
					_valueListCardPosX.Add(item2);
				}
				for (int num20 = 0; num20 < _dispCardMax; num20++)
				{
					CommonValue item3 = new CommonValue();
					_valueListCardScaleXY.Add(item3);
				}
				GameObject obj3 = _masterNullCard.gameObject.transform.parent.gameObject;
				gameObject = obj3.gameObject.transform.parent.gameObject;
				gameObject3 = obj3.transform.GetChild(0).gameObject;
				gameObject3.SetActive(value: true);
				gameObject3.transform.GetChild(0).gameObject.SetActive(value: false);
				gameObject3.transform.GetChild(1).gameObject.SetActive(value: false);
				TextMeshProUGUI textMeshProUGUI = null;
				int index = num15;
				int num21 = _objColorIDListCard[index];
				if (num21 <= 0)
				{
					num21 = 0;
				}
				LoginBonusData partnerData4 = GetPartnerData(num21);
				int id = 1;
				if (partnerData4 != null)
				{
					id = partnerData4.PartnerId.id;
				}
				PartnerData partner = Singleton<DataManager>.Instance.GetPartner(id);
				textMeshProUGUI = gameObject3.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
				if (partner != null)
				{
					textMeshProUGUI.text = partner.name.str;
				}
				if (_isLastOne)
				{
					int num22 = num15;
					for (int num23 = 0; num23 < _dispCardMax; num23++)
					{
						if (num23 != num22)
						{
							_objListCard[num23].SetActive(value: false);
						}
					}
				}
				_selectorBG.SetActive(value: true);
				_selectorBG.transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.GetComponent<ImageColorGroup>().SetColor(6);
				_selectorBG.transform.GetChild(0).gameObject.transform.GetChild(3).gameObject.transform.GetChild(0).gameObject.SetActive(value: false);
				_selectorBG.transform.GetChild(0).gameObject.transform.GetChild(4).gameObject.SetActive(value: false);
			}
			else
			{
				gameObject.transform.GetChild(0).gameObject.SetActive(value: false);
				gameObject3.transform.GetChild(0).gameObject.SetActive(value: true);
				gameObject3.transform.GetChild(1).gameObject.SetActive(value: false);
				_selectorBG.SetActive(value: false);
				for (int num24 = 0; num24 < _masterNullCard.transform.childCount; num24++)
				{
					_masterNullCard.transform.GetChild(num24).gameObject.SetActive(value: false);
				}
			}
			_isDispInfoWindow = false;
			_isChangeSibling = false;
			_info_state = InfoWindowState.None;
			_info_timer = 0u;
			_info_count = 0u;
			_state = MonitorState.None;
			if (_isEnableLoginBonus && _isSetCurrentCard && _currentStampNum + 1 == 10)
			{
				InitializeGetWindow();
				gameObject2.SetActive(value: true);
				gameObject2.GetComponent<PlayableDirector>().Stop();
				gameObject2.SetActive(value: false);
			}
		}

		public override void ViewUpdate()
		{
			if (_timer > 0)
			{
				_timer--;
			}
			switch (_state)
			{
			case MonitorState.None:
				_state = MonitorState.Init;
				break;
			case MonitorState.Init:
				_isCheckOK = false;
				_isCheckLR = false;
				if (!_isSetCurrentCard)
				{
					_state = MonitorState.SelectCardFadeIn;
				}
				else
				{
					_state = MonitorState.SelectCardDecide;
				}
				break;
			case MonitorState.SelectCardFadeIn:
			{
				TextMeshProUGUI textMeshProUGUI2 = null;
				GameObject obj13 = _masterNullCard.gameObject.transform.parent.gameObject;
				_ = obj13.gameObject.transform.parent.gameObject;
				GameObject obj14 = obj13.transform.GetChild(0).gameObject;
				obj14.SetActive(value: true);
				obj14.transform.GetChild(0).gameObject.SetActive(value: false);
				obj14.transform.GetChild(1).gameObject.SetActive(value: true);
				obj14.transform.GetChild(1).gameObject.GetComponent<Animator>().Play("In", 0, 0f);
				int index5 = (_dispCardMax - 1) / 2;
				int num14 = _objColorIDListCard[index5];
				if (num14 <= 0)
				{
					num14 = 0;
				}
				LoginBonusData partnerData6 = GetPartnerData(num14);
				int id4 = 1;
				if (partnerData6 != null)
				{
					id4 = partnerData6.PartnerId.id;
				}
				PartnerData partner2 = Singleton<DataManager>.Instance.GetPartner(id4);
				textMeshProUGUI2 = obj14.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
				if (partner2 != null)
				{
					textMeshProUGUI2.text = partner2.name.str;
				}
				GameObject obj15 = obj13.transform.GetChild(1).gameObject;
				obj15.SetActive(value: true);
				obj15.GetComponent<Animator>().Play("In", 0, 0f);
				obj13.transform.GetChild(2).gameObject.SetActive(value: false);
				_selectorBG.GetComponent<Animator>().Play("In", 0, 0f);
				_timer = 0;
				_state = MonitorState.SelectCardFadeInJudge;
				break;
			}
			case MonitorState.SelectCardFadeInJudge:
				if (!Singleton<UserDataManager>.Instance.GetUserData(_monitorID).Detail.ContentBit.IsFlagOn(ContentBitID.FirstLoginBonus))
				{
					_isDispInfoWindow = true;
					_info_state = InfoWindowState.None;
					_info_timer = 0u;
					_state = MonitorState.SelectCardFadeInJudgeWait;
				}
				else
				{
					_state = MonitorState.SelectCardFadeInWait;
				}
				break;
			case MonitorState.SelectCardFadeInJudgeWait:
				if (!_isDispInfoWindow)
				{
					_state = MonitorState.SelectCardFadeInWait;
				}
				else if (_isInfoWindowVoice && !_isCallVoice)
				{
					_isCallVoice = true;
					SoundManager.PlayVoice(Mai2.Voice_000001.Cue.VO_000228, _monitorID);
				}
				break;
			case MonitorState.SelectCardFadeInWait:
				if (_timer == 0)
				{
					_state = MonitorState.SelectCardWaitInit;
					_timer = 90;
				}
				break;
			case MonitorState.SelectCardWaitInit:
				if (_timer == 0)
				{
					if (!_isSetCurrentCard && !_isLastOne)
					{
						_buttonController.SetVisibleImmediate(true, 1);
						_buttonController.SetVisibleImmediate(true, 2);
					}
					if (!_isLastOne)
					{
						_isCheckLR = true;
					}
					OKStart();
					_state = MonitorState.SelectCardWait;
				}
				break;
			case MonitorState.SelectCardWait:
				if (_isOK || _isTimeUp)
				{
					_isDecidedOK = true;
					int index4 = (_dispCardMax - 1) / 2;
					_currentLayerColorID = _objColorIDListCard[index4];
					if (_currentLayerColorID <= 0)
					{
						_currentLayerColorID = 0;
					}
					_currentStampNum = 0;
					_buttonController.SetVisibleImmediate(false, 1);
					_buttonController.SetVisibleImmediate(false, 2);
					_isCheckLR = false;
					_isRight = false;
					_isLeft = false;
					_isCheckOK = false;
					_timer = 60;
					ResetOKStart();
					_state = MonitorState.SelectCardDecideInit;
					LoginBonusData partnerData5 = GetPartnerData(_currentLayerColorID);
					int voiceID2 = 1;
					if (partnerData5 != null)
					{
						voiceID2 = partnerData5.PartnerId.id;
					}
					SoundManager.SetPartnerVoiceCue(_monitorID, voiceID2);
					break;
				}
				if (_isRight)
				{
					for (int num12 = 0; num12 < _dispCardMax; num12++)
					{
						CommonValue commonValue3 = _valueListCardPosX[num12];
						CommonValue commonValue4 = _valueListCardScaleXY[num12];
						commonValue3.start = _cardPosX[num12];
						commonValue3.current = _cardPosX[num12];
						commonValue4.start = _cardScaleXY[num12];
						commonValue4.current = _cardScaleXY[num12];
						if (num12 == 0)
						{
							commonValue3.end = _cardPosX[num12];
							commonValue4.end = _cardScaleXY[num12];
						}
						else
						{
							commonValue3.end = _cardPosX[num12 - 1];
							commonValue4.end = _cardScaleXY[num12 - 1];
						}
						commonValue3.diff = (commonValue3.end - commonValue3.start) / _cardMoveTime;
						commonValue4.diff = (commonValue4.end - commonValue4.start) / _cardMoveTime;
						_cardPosEnd = 0;
					}
					_state = MonitorState.SelectCardMoveRight;
				}
				else if (_isLeft)
				{
					for (int num13 = 0; num13 < _dispCardMax; num13++)
					{
						CommonValue commonValue5 = _valueListCardPosX[num13];
						CommonValue commonValue6 = _valueListCardScaleXY[num13];
						commonValue5.start = _cardPosX[num13];
						commonValue5.current = _cardPosX[num13];
						commonValue6.start = _cardScaleXY[num13];
						commonValue6.current = _cardScaleXY[num13];
						if (num13 == _dispCardMax - 1)
						{
							commonValue5.end = _cardPosX[num13];
							commonValue6.end = _cardScaleXY[num13];
						}
						else
						{
							commonValue5.end = _cardPosX[num13 + 1];
							commonValue6.end = _cardScaleXY[num13 + 1];
						}
						commonValue5.diff = (commonValue5.end - commonValue5.start) / _cardMoveTime;
						commonValue6.diff = (commonValue6.end - commonValue6.start) / _cardMoveTime;
						_cardPosEnd = 0;
					}
					_state = MonitorState.SelectCardMoveLeft;
				}
				if (_isRight || _isLeft)
				{
					GameObject obj11 = _masterNullCard.gameObject.transform.parent.gameObject;
					_ = obj11.gameObject.transform.parent.gameObject;
					GameObject obj12 = obj11.transform.GetChild(0).gameObject;
					obj12.SetActive(value: true);
					obj12.transform.GetChild(0).gameObject.SetActive(value: false);
					obj12.transform.GetChild(1).gameObject.SetActive(value: true);
					obj12.transform.GetChild(1).gameObject.GetComponent<Animator>().Play("Out", 0, 0f);
				}
				break;
			case MonitorState.SelectCardMoveRight:
			case MonitorState.SelectCardMoveLeft:
			{
				_cardPosEnd = 0;
				for (int m = 0; m < _dispCardMax; m++)
				{
					CommonValue commonValue = _valueListCardPosX[m];
					CommonValue commonValue2 = _valueListCardScaleXY[m];
					if (commonValue.UpdateValue())
					{
						_cardPosEnd++;
					}
					commonValue2.UpdateValue();
					_nullListCard[m].transform.localPosition = new Vector3(commonValue.current, 0f, 0f);
					_objListCard[m].transform.localScale = new Vector3(commonValue2.current, commonValue2.current, 1f);
				}
				if (_cardPosEnd != _dispCardMax)
				{
					break;
				}
				int num4 = 0;
				if (_isRight)
				{
					int index = (_dispCardMax - 1) / 2 + 1;
					int num5 = _objColorIDListCard[index];
					if (num5 <= 0)
					{
						num5 = 0;
					}
					num4 = GetLayerColorRefID(num5);
				}
				else if (_isLeft)
				{
					int index2 = (_dispCardMax - 1) / 2 - 1;
					int num6 = _objColorIDListCard[index2];
					if (num6 <= 0)
					{
						num6 = 0;
					}
					num4 = GetLayerColorRefID(num6);
				}
				int num7 = (_dispCardMax - 1) / 2;
				int num8 = num4 - num7;
				for (int n = 0; n < _dispCardMax; n++)
				{
					Animator anim = _animListCard[n];
					int num9 = 0;
					int ref_id = num8 + n;
					num9 = GetLayerColorID(ref_id);
					_objColorIDListCard[n] = num9;
					SetLayerColorAnim(anim, num9, "Loop_Clause", 1f);
				}
				for (int num10 = 0; num10 < _dispCardMax; num10++)
				{
					_nullListCard[num10].transform.localPosition = new Vector3(_cardPosX[num10], 0f, 0f);
					_objListCard[num10].transform.localScale = new Vector3(_cardScaleXY[num10], _cardScaleXY[num10], 1f);
				}
				TextMeshProUGUI textMeshProUGUI = null;
				GameObject obj9 = _masterNullCard.gameObject.transform.parent.gameObject;
				_ = obj9.gameObject.transform.parent.gameObject;
				GameObject obj10 = obj9.transform.GetChild(0).gameObject;
				obj10.SetActive(value: true);
				obj10.transform.GetChild(0).gameObject.SetActive(value: false);
				obj10.transform.GetChild(1).gameObject.SetActive(value: true);
				obj10.transform.GetChild(1).gameObject.GetComponent<Animator>().Play("In", 0, 0f);
				int index3 = num7;
				int num11 = _objColorIDListCard[index3];
				if (num11 <= 0)
				{
					num11 = 0;
				}
				LoginBonusData partnerData4 = GetPartnerData(num11);
				int id = 1;
				if (partnerData4 != null)
				{
					id = partnerData4.PartnerId.id;
				}
				PartnerData partner = Singleton<DataManager>.Instance.GetPartner(id);
				textMeshProUGUI = obj10.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();
				if (partner != null)
				{
					textMeshProUGUI.text = partner.name.str;
				}
				_state = MonitorState.SelectCardWait;
				_isRight = false;
				_isLeft = false;
				break;
			}
			case MonitorState.SelectCardDecideInit:
				if (_timer == 0)
				{
					_buttonController.SetVisible(false, default(int));
					_isOK = false;
					if (!_isSetCurrentCard)
					{
						_selectorBG.GetComponent<Animator>().Play("Out", 0, 0f);
						SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000250, _monitorID);
					}
					GameObject obj5 = _masterNullCard.gameObject.transform.parent.gameObject;
					_ = obj5.gameObject.transform.parent.gameObject;
					GameObject obj6 = obj5.transform.GetChild(0).gameObject;
					obj6.SetActive(value: true);
					obj6.transform.GetChild(0).gameObject.SetActive(value: false);
					obj6.transform.GetChild(1).gameObject.SetActive(value: true);
					obj6.transform.GetChild(1).gameObject.GetComponent<Animator>().Play("Out", 0, 0f);
					_state = MonitorState.SelectCardDecide;
				}
				break;
			case MonitorState.SelectCardDecide:
			{
				_isOK = false;
				GameObject gameObject2 = null;
				gameObject2 = _masterNullCard.gameObject.transform.parent.gameObject;
				_ = gameObject2.gameObject.transform.parent.gameObject;
				GameObject obj7 = gameObject2.transform.GetChild(0).gameObject;
				obj7.SetActive(value: true);
				obj7.transform.GetChild(0).gameObject.SetActive(value: true);
				obj7.transform.GetChild(0).gameObject.GetComponent<Animator>().Play("in", 0, 0f);
				obj7.transform.GetChild(1).gameObject.SetActive(value: false);
				if (_isSetCurrentCard)
				{
					GameObject obj8 = gameObject2.transform.GetChild(2).gameObject;
					obj8.SetActive(value: true);
					obj8.GetComponent<Animator>().Play("In", 0, 0f);
				}
				for (int j = 0; j < _masterNullCard.transform.childCount; j++)
				{
					_masterNullCard.transform.GetChild(j).gameObject.SetActive(value: false);
				}
				GameObject gameObject3 = _masterNullCard.transform.GetChild(0).gameObject;
				gameObject3.SetActive(value: true);
				Animator component4 = gameObject3.GetComponent<Animator>();
				SetLayerColorAnim(component4, _currentLayerColorID, "Loop_Clause", 1f);
				for (int k = 0; k < _currentStampNum; k++)
				{
					component4 = GetStampAnim(gameObject3, k);
					SetLayerColorAnim(component4, _currentLayerColorID, "Loop_Active", 1f);
				}
				for (int l = _currentStampNum; l < 10; l++)
				{
					component4 = GetStampAnim(gameObject3, l);
					SetLayerColorAnim(component4, _currentLayerColorID, "Loop_Base", 1f);
				}
				Image component5 = gameObject3.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject.transform.GetChild(1).gameObject.GetComponent<Image>();
				LoginBonusData partnerData3 = GetPartnerData(_currentLayerColorID);
				int num3 = -1;
				if (partnerData3 != null)
				{
					num3 = partnerData3.PartnerId.id;
				}
				Texture2D texture2D = null;
				Sprite sprite = null;
				if (partnerData3 != null && num3 != -1)
				{
					texture2D = _assetManager.GetPartnerTexture2D(num3);
					sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
				}
				if (sprite != null)
				{
					component5.sprite = Object.Instantiate(sprite);
				}
				if (!_isSetCurrentCard)
				{
					_timer = 0;
				}
				else
				{
					_timer = 60;
				}
				_state = MonitorState.GetStampFadeIn;
				break;
			}
			case MonitorState.GetStampFadeIn:
				if (_timer == 0)
				{
					GameObject obj4 = _masterNullCard.gameObject.transform.parent.gameObject;
					_ = obj4.gameObject.transform.parent.gameObject;
					obj4.transform.GetChild(1).gameObject.SetActive(value: false);
					Animator component3 = _masterNullCard.transform.GetChild(0).gameObject.GetComponent<Animator>();
					SetLayerColorAnim(component3, _currentLayerColorID, "In", 0f);
					_state = MonitorState.GetStampFadeInWait;
				}
				break;
			case MonitorState.GetStampFadeInWait:
			{
				Animator component8 = _masterNullCard.transform.GetChild(0).gameObject.GetComponent<Animator>();
				if (this.m.IsEndAnim(component8))
				{
					_state = MonitorState.GetStampOpen;
				}
				break;
			}
			case MonitorState.GetStampOpen:
				if (_timer == 0)
				{
					Animator component7 = _masterNullCard.transform.GetChild(0).gameObject.GetComponent<Animator>();
					SetLayerColorAnim(component7, _currentLayerColorID, "Open", 0f);
					SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_STAMP_OPEN, _monitorID);
					_timer = 60;
					_state = MonitorState.GetStampOpenWait;
				}
				break;
			case MonitorState.GetStampOpenWait:
				if (_timer == 0)
				{
					_state = MonitorState.GetStampAdd;
				}
				break;
			case MonitorState.GetStampAdd:
			{
				GameObject card2 = _masterNullCard.transform.GetChild(0).gameObject;
				int id3 = _currentStampNum + 1 - 1;
				Animator stampAnim2 = GetStampAnim(card2, id3);
				SetLayerColorAnim(stampAnim2, _currentLayerColorID, "Get_Stamp", 0f);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_STAMP_GET, _monitorID);
				_state = MonitorState.GetStampAddWait;
				break;
			}
			case MonitorState.GetStampAddWait:
			{
				GameObject card = _masterNullCard.transform.GetChild(0).gameObject;
				int id2 = _currentStampNum + 1 - 1;
				Animator stampAnim = GetStampAnim(card, id2);
				if (this.m.IsEndAnim(stampAnim))
				{
					_timer = 60;
					_state = MonitorState.GetStampAddJudge;
					if (_currentStampNum + 1 == 10)
					{
						SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_STAMP_COMPLETE, _monitorID);
					}
				}
				break;
			}
			case MonitorState.GetStampAddJudge:
				if (_timer == 0)
				{
					if (_currentStampNum + 1 == 10)
					{
						_timer = 0;
						_state = MonitorState.GetStampAddJudgeWait;
					}
					else
					{
						OKStart();
						_timer = 600;
						_state = MonitorState.GetStampAddJudgeWait;
					}
				}
				break;
			case MonitorState.GetStampAddJudgeWait:
				isOKTimerZero();
				if (_timer == 0)
				{
					if (_currentStampNum + 1 == 10)
					{
						_state = MonitorState.GetCharacterFadeIn;
						break;
					}
					ResetOKStart();
					_state = MonitorState.GetStampFadeOut;
				}
				break;
			case MonitorState.GetCharacterFadeIn:
			{
				GameObject obj3 = _masterNullCard.gameObject.transform.parent.gameObject.gameObject.transform.parent.gameObject.transform.GetChild(2).gameObject;
				obj3.SetActive(value: true);
				obj3.GetComponent<PlayableDirector>().Play();
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.JINGLE_CHARA_GET, _monitorID);
				LoginBonusData partnerData2 = GetPartnerData(_currentLayerColorID);
				int voiceID = 1;
				if (partnerData2 != null)
				{
					voiceID = partnerData2.PartnerId.id;
				}
				SoundManager.SetPartnerVoiceCue(_monitorID, voiceID);
				_sub_count = 0;
				_timer = 300;
				_state = MonitorState.GetCharacterFadeInWait;
				break;
			}
			case MonitorState.GetCharacterFadeInWait:
				_sub_count++;
				_state = MonitorState.GetCharacterWait;
				break;
			case MonitorState.GetCharacterWait:
				_sub_count++;
				_state = MonitorState.GetCharacterFadeOut;
				break;
			case MonitorState.GetCharacterFadeOut:
				_sub_count++;
				_state = MonitorState.GetCharacterFadeOutWait1;
				break;
			case MonitorState.GetCharacterFadeOutWait1:
				_sub_count++;
				if (_sub_count == 62)
				{
					SoundManager.PlayPartnerVoice(Mai2.Voice_Partner_000001.Cue.VO_000249, _monitorID);
				}
				_masterNullCard.gameObject.transform.parent.gameObject.gameObject.transform.parent.gameObject.transform.GetChild(2).gameObject.GetComponent<PlayableDirector>();
				if (_sub_count == 299)
				{
					_buttonController.ChangeButtonType();
					OKStart();
					_timer = 600;
					_state = MonitorState.GetCharacterFadeOutWait2;
				}
				break;
			case MonitorState.GetCharacterFadeOutWait2:
				_sub_count++;
				isOKTimerZero();
				if (_timer == 0)
				{
					GameObject obj16 = _masterNullCard.gameObject.transform.parent.gameObject.gameObject.transform.parent.gameObject.transform.GetChild(2).gameObject;
					obj16.GetComponent<PlayableDirector>().Stop();
					obj16.GetComponent<Animator>().Play("Out", 0, 0f);
					ResetOKStart();
					_sub_count = 0;
					_state = MonitorState.GetCharacterFadeOutWait3;
				}
				break;
			case MonitorState.GetCharacterFadeOutWait3:
			{
				GameObject gameObject4 = null;
				gameObject4 = _masterNullCard.gameObject.transform.parent.gameObject.gameObject.transform.parent.gameObject.transform.GetChild(2).gameObject;
				Animator component6 = gameObject4.GetComponent<Animator>();
				if (this.m.IsEndAnim(component6))
				{
					gameObject4.SetActive(value: false);
					_state = MonitorState.GetStampFadeOut;
				}
				break;
			}
			case MonitorState.GetStampFadeOut:
			{
				if (_timer != 0)
				{
					break;
				}
				Animator component2 = _masterNullCard.transform.GetChild(0).gameObject.GetComponent<Animator>();
				SetLayerColorAnim(component2, _currentLayerColorID, "Out", 0f);
				SoundManager.PlaySE(Mai2.Mai2Cue.Cue.SE_STAMP_CLOSE, _monitorID);
				GameObject gameObject = null;
				gameObject = _masterNullCard.gameObject.transform.parent.gameObject;
				_ = gameObject.gameObject.transform.parent.gameObject;
				GameObject obj = gameObject.transform.GetChild(0).gameObject;
				obj.SetActive(value: true);
				obj.transform.GetChild(0).gameObject.SetActive(value: true);
				obj.transform.GetChild(0).gameObject.GetComponent<Animator>().Play("Out", 0, 0f);
				obj.transform.GetChild(1).gameObject.SetActive(value: false);
				gameObject.transform.GetChild(1).gameObject.SetActive(value: false);
				if (_isSetCurrentCard)
				{
					GameObject obj2 = gameObject.transform.GetChild(2).gameObject;
					obj2.SetActive(value: true);
					obj2.GetComponent<Animator>().Play("Out", 0, 0f);
				}
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(_monitorID);
				int num = -1;
				LoginBonusData partnerData = GetPartnerData(_currentLayerColorID);
				_ = partnerData?.PartnerId.id;
				int num2 = 0;
				if (partnerData != null)
				{
					num2 = partnerData.GetID();
				}
				for (int i = 0; i < userData.LoginBonusList.Count; i++)
				{
					UserLoginBonus userLoginBonus = userData.LoginBonusList[i];
					if (num2 == userLoginBonus.ID)
					{
						num = i;
						break;
					}
				}
				if (num != -1)
				{
					UserLoginBonus userLoginBonus2 = userData.LoginBonusList[num];
					if (_currentStampNum + 1 >= 10)
					{
						userLoginBonus2.IsComplete = true;
						userLoginBonus2.Point = 10u;
						userLoginBonus2.IsCurrent = false;
						userData.AddCollections(UserData.Collection.Partner, partnerData.PartnerId.id, addNewGet: true);
					}
					else
					{
						userLoginBonus2.IsComplete = false;
						userLoginBonus2.Point = (uint)(_currentStampNum + 1);
						userLoginBonus2.IsCurrent = true;
					}
				}
				else
				{
					UserLoginBonus userLoginBonus3 = new UserLoginBonus();
					userLoginBonus3.ID = num2;
					userLoginBonus3.IsCurrent = true;
					if (_currentStampNum + 1 >= 10)
					{
						userLoginBonus3.IsComplete = true;
						userLoginBonus3.Point = 10u;
					}
					else
					{
						userLoginBonus3.IsComplete = false;
						userLoginBonus3.Point = (uint)(_currentStampNum + 1);
					}
					userData.LoginBonusList.Add(userLoginBonus3);
				}
				SoundManager.SetPartnerVoiceCue(_monitorID, _EquippedPartnerVoiceID);
				_state = MonitorState.GetStampFadeOutWait;
				break;
			}
			case MonitorState.GetStampFadeOutWait:
			{
				Animator component = _masterNullCard.transform.GetChild(0).gameObject.GetComponent<Animator>();
				if (this.m.IsEndAnim(component))
				{
					_state = MonitorState.Finish;
				}
				break;
			}
			case MonitorState.Finish:
				_state = MonitorState.End;
				break;
			}
		}

		public bool IsEnd()
		{
			return _state == MonitorState.End;
		}
	}
}
