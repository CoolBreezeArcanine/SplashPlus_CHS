using System.Collections;
using System.Collections.Generic;
using Manager;
using TMPro;
using UI.Common;
using UnityEngine;

namespace Monitor.CourseSelect
{
	public class CourseSelectMonitor : MonitorBase
	{
		private enum ButtonEnum
		{
			OK,
			Back,
			Right,
			Left
		}

		[SerializeField]
		[Header("各種コントローラー")]
		private CourseSelectButtonController _buttonController;

		[SerializeField]
		[Header("nullポジション")]
		private Transform _courseBGTransform;

		[SerializeField]
		private Transform _courseSelectBGTransform;

		[SerializeField]
		private Transform _courseSelectTabTransform;

		[SerializeField]
		private Transform _courseSelectControllerTransform;

		[SerializeField]
		private Transform _itemInfoTransform;

		[SerializeField]
		private Transform _transitionTransform;

		[SerializeField]
		[Header("オリジナルprefab")]
		private Transform _courseBGObj;

		[SerializeField]
		private Transform _courseSelectBGObj;

		[SerializeField]
		private Transform _courseSelectTabObj;

		[SerializeField]
		private Transform _courseSelectControllerObj;

		[SerializeField]
		private Transform _itemInfoObj;

		[SerializeField]
		private Transform _transitionObj;

		[SerializeField]
		[Header("デバッグ表示用")]
		private GameObject _musicInfoObj;

		[SerializeField]
		private TextMeshProUGUI _musicInfoText;

		private CourseBGController _courseBg;

		private SelectorBackgroundController _courseSelectBg;

		private CourseTabController _tabController;

		private CourseCardSelectController _selectController;

		private CourseItemInfo _itemInfo;

		private CourseTransition _courseTransition;

		private List<CourseModeCardData> _modeCardDataList;

		private int _modeIndex;

		private int _courseIndex;

		private int _modeCardListCount;

		private int _cardListCount;

		private bool _isMode = true;

		public override void Initialize(int monIndex, bool active)
		{
			base.Initialize(monIndex, active);
			_courseBg = Object.Instantiate(_courseBGObj, _courseBGTransform).GetComponent<CourseBGController>();
			_courseSelectBg = Object.Instantiate(_courseSelectBGObj, _courseSelectBGTransform).GetComponent<SelectorBackgroundController>();
			_tabController = Object.Instantiate(_courseSelectTabObj, _courseSelectTabTransform).GetComponent<CourseTabController>();
			_selectController = Object.Instantiate(_courseSelectControllerObj, _courseSelectControllerTransform).GetComponent<CourseCardSelectController>();
			_itemInfo = Object.Instantiate(_itemInfoObj, _itemInfoTransform).GetComponent<CourseItemInfo>();
			_courseTransition = Object.Instantiate(_transitionObj, _transitionTransform).GetComponent<CourseTransition>();
			_tabController.Initialize(monIndex);
			if (active)
			{
				_buttonController.Initialize(monIndex);
				_buttonController.SetVisibleImmediate(true, 2);
				_buttonController.SetVisibleImmediate(true, 3);
				_buttonController.SetVisible(false, 1);
			}
		}

		public override void ViewUpdate()
		{
			UpdateButtonAnimation();
			_courseSelectBg?.UpdateScroll();
			CheckButton();
		}

		public void SetTabData(List<CourseModeCardData> datas)
		{
			_tabController.SetData(datas);
		}

		public void SetCourseModeData(List<CourseModeCardData> datas)
		{
			_selectController.SetModeSelectData(datas);
			_modeCardDataList = datas;
			_modeCardListCount = datas.Count;
		}

		public void SetItemGot(bool isGot)
		{
			_itemInfo.gameObject.SetActive(isGot);
		}

		public void SetCourseData(List<CourseCardData> datas)
		{
			_selectController.SetSelectData(datas);
			_cardListCount = datas.Count;
		}

		public void UpdateModeData(int modeIndex)
		{
			_selectController.UpdateModeSelectCard(modeIndex);
			_modeIndex = modeIndex;
		}

		public void UpdateCourseData(int modeIndex, int courseIndex)
		{
			_tabController.UpdateTab(modeIndex);
			_selectController.UpdateSelectCard(modeIndex, courseIndex);
			_modeIndex = modeIndex;
			_courseIndex = courseIndex;
		}

		public void UpdateButtonAnimation()
		{
			if (isPlayerActive)
			{
				_tabController.UpdateButtonView();
				_buttonController.ViewUpdate(GameManager.GetGameMSecAdd());
			}
		}

		public void SetFirstFadeIn()
		{
			if (isPlayerActive)
			{
				StartCoroutine(FirstFadeInCoroutine());
			}
		}

		private IEnumerator FirstFadeInCoroutine()
		{
			_selectController.SetHideSelectCard();
			_tabController.SetVisibleTab(isActive: false);
			_tabController.SetVisibleParts(isActive: false);
			_courseSelectBg.SetActiveIndicator(isActive: true);
			_courseSelectBg.PrepareIndicator(_modeIndex, _modeCardListCount);
			_courseSelectBg.UpdateIndicator(_modeIndex);
			_itemInfo.SetAnim(CourseItemInfo.BGAnim.In);
			_courseTransition.SetAnim(CourseTransition.BGAnim.Idle, base.MonitorIndex);
			_courseBg.SetAnim(CourseBGController.BGAnim.Dani_Loop);
			_courseSelectBg.Play(SelectorBackgroundController.AnimationType.In);
			_selectController.PlayInAnimation();
			yield return new WaitForSeconds(1f);
			_courseTransition.SetAnim(CourseTransition.BGAnim.In, base.MonitorIndex);
		}

		public void SetModeDecide(int courseModeIndex, int courseIndex)
		{
			if (isPlayerActive)
			{
				StartCoroutine(ModeDecideCoroutine(courseModeIndex, courseIndex));
			}
		}

		private IEnumerator ModeDecideCoroutine(int courseModeIndex, int courseIndex)
		{
			if (isPlayerActive)
			{
				_selectController.PlayOutAnimation();
				_buttonController.SetAnimationActive(0);
				_courseSelectBg.Play(SelectorBackgroundController.AnimationType.OutBack);
				yield return new WaitForSeconds(0.2f);
				_buttonController.SetVisible(true, 1);
				_selectController.SetHideModeSelectCard();
				_selectController.PlayInAnimation();
				_tabController.SetVisibleTab(isActive: true);
				_tabController.SetVisibleParts(isActive: true);
				_tabController.PlayInAnimation();
				UpdateCourseData(courseModeIndex, courseIndex);
				if (_modeCardDataList[courseModeIndex]._courseMode == 2)
				{
					_courseBg.SetAnim(CourseBGController.BGAnim.SinDani_In);
				}
				else
				{
					_courseBg.SetAnim(CourseBGController.BGAnim.Dani_Loop);
				}
				_courseSelectBg.PrepareIndicator(_courseIndex, _cardListCount);
				_courseSelectBg.UpdateIndicator(_courseIndex);
				_isMode = false;
			}
		}

		public void SetModeRightMove()
		{
			if (isPlayerActive)
			{
				_selectController.PlayMoveRightAnimation();
				_buttonController.SetAnimationActive(2);
				_courseSelectBg.UpdateIndicator(_modeIndex);
			}
		}

		public void SetModeLeftMove()
		{
			if (isPlayerActive)
			{
				_selectController.PlayMoveLeftAnimation();
				_buttonController.SetAnimationActive(3);
				_courseSelectBg.UpdateIndicator(_modeIndex);
			}
		}

		public void SetCourseBack(int categoryIndex)
		{
			if (isPlayerActive)
			{
				StartCoroutine(CourseBackCoroutine(categoryIndex));
			}
		}

		private IEnumerator CourseBackCoroutine(int categoryIndex)
		{
			_tabController.PlayOutAnimation();
			_selectController.PlayOutAnimation();
			_buttonController.SetAnimationActive(1);
			_courseSelectBg.Play(SelectorBackgroundController.AnimationType.OutBack);
			yield return new WaitForSeconds(0.2f);
			_buttonController.SetVisible(false, 1);
			_courseBg.SetAnim(CourseBGController.BGAnim.Dani_Loop);
			_selectController.SetHideSelectCard();
			_tabController.SetVisibleTab(isActive: false);
			_tabController.SetVisibleParts(isActive: false);
			_selectController.PlayInAnimation();
			UpdateModeData(categoryIndex);
			_courseSelectBg.PrepareIndicator(_modeIndex, _modeCardListCount);
			_courseSelectBg.UpdateIndicator(_modeIndex);
			_isMode = true;
		}

		public void SetCourseDecide()
		{
			_ = isPlayerActive;
		}

		public void SetCourseRightMove()
		{
			if (isPlayerActive)
			{
				_selectController.PlayMoveRightAnimation();
				_buttonController.SetAnimationActive(2);
				_courseSelectBg.UpdateIndicator(_courseIndex);
			}
		}

		public void SetCourseLeftMove()
		{
			if (isPlayerActive)
			{
				_selectController.PlayMoveLeftAnimation();
				_buttonController.SetAnimationActive(3);
				_courseSelectBg.UpdateIndicator(_courseIndex);
			}
		}

		public void SetCourseRightCategoryMoveByMoveButton()
		{
			if (isPlayerActive)
			{
				_tabController.PlayMoveRightAnimation();
				_selectController.PlayMoveRightAnimation();
				_buttonController.SetAnimationActive(2);
				SetBGState();
				_courseSelectBg.PrepareIndicator(_courseIndex, _cardListCount);
				_courseSelectBg.UpdateIndicator(_courseIndex);
			}
		}

		public void SetCourseLeftCategoryMoveByMoveButton()
		{
			if (isPlayerActive)
			{
				_tabController.PlayMoveLeftAnimation();
				_selectController.PlayMoveLeftAnimation();
				_buttonController.SetAnimationActive(3);
				SetBGState();
				_courseSelectBg.PrepareIndicator(_courseIndex, _cardListCount);
				_courseSelectBg.UpdateIndicator(_courseIndex);
			}
		}

		public void SetCourseRightCategoryMoveByCategoryButton()
		{
			if (isPlayerActive)
			{
				_tabController.PlayMoveRightAnimation();
				_tabController.PressedTabButton(isRight: true);
				_selectController.PlayInAnimation();
				SetBGState();
				_courseSelectBg.PrepareIndicator(_courseIndex, _cardListCount);
				_courseSelectBg.UpdateIndicator(_courseIndex);
			}
		}

		public void SetCourseLeftCategoryMoveByCategoryButton()
		{
			if (isPlayerActive)
			{
				_tabController.PlayMoveLeftAnimation();
				_tabController.PressedTabButton(isRight: false);
				_selectController.PlayInAnimation();
				SetBGState();
				_courseSelectBg.PrepareIndicator(_courseIndex, _cardListCount);
				_courseSelectBg.UpdateIndicator(_courseIndex);
			}
		}

		public void SetCourseConfirmBack()
		{
			_ = isPlayerActive;
		}

		public void SetCourseConfirmWait()
		{
			_ = isPlayerActive;
		}

		public void SetCourseConfirmWaitBack()
		{
			_ = isPlayerActive;
		}

		public void SetCourseStart()
		{
			if (isPlayerActive)
			{
				_tabController.PlayOutAnimation();
				_selectController.PlayOutAnimation();
				_courseTransition.SetAnim(CourseTransition.BGAnim.Out, base.MonitorIndex);
				_buttonController.SetAnimationActive(0);
				_courseSelectBg.Play(SelectorBackgroundController.AnimationType.Out);
			}
		}

		public void SetDebugView(bool isView)
		{
			if (isPlayerActive)
			{
				_musicInfoObj.SetActive(isView);
			}
		}

		public void SetDebugMusicInfoText(string text)
		{
			if (isPlayerActive)
			{
				_musicInfoText.SetText(text);
			}
		}

		private void SetBGState()
		{
			if (_modeCardDataList[_modeIndex]._courseMode == 2)
			{
				_courseBg.SetAnim(CourseBGController.BGAnim.SinDani_Loop);
			}
			else
			{
				_courseBg.SetAnim(CourseBGController.BGAnim.Dani_Loop);
			}
		}

		public void SetSideMessage(string message)
		{
			_courseSelectBg?.SetScrollMessage(message);
		}

		private void CheckButton()
		{
			bool flag = true;
			bool flag2 = true;
			if (_isMode)
			{
				flag = _modeIndex != _modeCardListCount - 1;
				flag2 = _modeIndex != 0;
			}
			_buttonController.SetVisible(flag, 2);
			_buttonController.SetVisible(flag2, 3);
			_buttonController.SetVisibleImmediate(flag, 2);
			_buttonController.SetVisibleImmediate(flag2, 3);
		}
	}
}
