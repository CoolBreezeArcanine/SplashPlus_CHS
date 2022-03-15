using DB;
using MAI2.Util;
using Manager;
using Manager.MaiStudio;
using Process;
using UI;
using UI.Common;
using UI.DaisyChainList;
using UnityEngine;
using Util;

namespace Monitor
{
	public class CategoryTabController : TabControllerBase
	{
		[SerializeField]
		private Color[] _tabGenreColors;

		[SerializeField]
		private MultipleImage _clearRankImage;

		[SerializeField]
		private MultipleImage _comboImage;

		[SerializeField]
		private MultipleImage _dxMedalImage;

		[SerializeField]
		[Header("タブプレート")]
		private Animator _tabPlateAnimator;

		[SerializeField]
		private CustomImage _tabPlateBackImage;

		[SerializeField]
		private MultiImage _tabPlateStarImage;

		[SerializeField]
		private CustomImage _tabPlateLineImage;

		[SerializeField]
		[Header("タブプレート色")]
		private Color[] _tabPlateBackColor;

		[SerializeField]
		private Color[] _tabPlateStarColor;

		[SerializeField]
		private Color[] _tabPlateLineColor;

		private IMusicSelectProcess _selectProcess;

		private int _playerIndex;

		private bool _isActive;

		private bool _isVisible;

		private bool _isMedalActive = true;

		public void Initialize(IMusicSelectProcess musicSelectProcess, int playerIndex, bool isActive)
		{
			_isActive = isActive;
			_selectProcess = musicSelectProcess;
			_playerIndex = playerIndex;
			_clearRankImage.gameObject.SetActive(value: false);
			_comboImage.gameObject.SetActive(value: false);
			_dxMedalImage.gameObject.SetActive(value: false);
			ChangeAllTab();
		}

		public void ChangeAllTab()
		{
			if (_isActive)
			{
				_tabName.text = GetTagName(0);
				_tabCount.text = GetElementText();
				int categoryGenruColor = _selectProcess.GetCategoryGenruColor(_playerIndex, 1);
				_rightButtonObject.SetTabName(GetTagName(1), GetElementText());
				_leftButtonObject.SetTabName(GetTagName(-1), GetElementText());
				if (categoryGenruColor >= 0)
				{
					Safe.ReadonlySortedDictionary<int, MusicGenreData> musicGenres = Singleton<DataManager>.Instance.GetMusicGenres();
					_rightButtonObject.SetTabColor(new Color32(musicGenres[categoryGenruColor].Color.R, musicGenres[categoryGenruColor].Color.G, musicGenres[categoryGenruColor].Color.B, byte.MaxValue));
					categoryGenruColor = _selectProcess.GetCategoryGenruColor(_playerIndex, -1);
					_leftButtonObject.SetTabColor(new Color32(musicGenres[categoryGenruColor].Color.R, musicGenres[categoryGenruColor].Color.G, musicGenres[categoryGenruColor].Color.B, byte.MaxValue));
				}
				MedalCheck();
			}
		}

		public void MedalCheck()
		{
			if (_isMedalActive)
			{
				MusicSelectProcess.MedalData categoryMedalData = _selectProcess.GetCategoryMedalData(_playerIndex, 0);
				SetClearRankMedal(categoryMedalData.MinimumClearRank);
				SetComboMedal(categoryMedalData.MinimumCombo);
				SetDxMedal(categoryMedalData.MinimumClearRank, categoryMedalData.MinimumCombo);
			}
		}

		public void SetActiveMedals(bool isActive)
		{
			_isMedalActive = isActive;
			if (_isMedalActive)
			{
				MedalCheck();
				return;
			}
			_tabPlateAnimator.gameObject.SetActive(value: false);
			_clearRankImage.gameObject.SetActive(value: false);
			_comboImage.gameObject.SetActive(value: false);
			_dxMedalImage.gameObject.SetActive(value: false);
		}

		public override void ViewUpdate()
		{
			if (!base.gameObject.activeSelf || !IsAnimation)
			{
				return;
			}
			if (Timer > _splitFlap.GetCurrentAnimatorClipInfo(0)[0].clip.length)
			{
				Timer = 0f;
				IsAnimation = false;
				_tabName.text = GetTagName(0);
				_tabCount.text = GetElementText();
				_rightButtonObject.SetTabName(GetTagName(1), GetElementText());
				_leftButtonObject.SetTabName(GetTagName(-1), GetElementText());
				Safe.ReadonlySortedDictionary<int, MusicGenreData> musicGenres = Singleton<DataManager>.Instance.GetMusicGenres();
				int categoryGenruColor = _selectProcess.GetCategoryGenruColor(_playerIndex, 1);
				if (categoryGenruColor >= 0)
				{
					_rightButtonObject.SetTabColor(new Color32(musicGenres[categoryGenruColor].Color.R, musicGenres[categoryGenruColor].Color.G, musicGenres[categoryGenruColor].Color.B, byte.MaxValue));
				}
				categoryGenruColor = _selectProcess.GetCategoryGenruColor(_playerIndex, -1);
				if (categoryGenruColor >= 0)
				{
					_leftButtonObject.SetTabColor(new Color32(musicGenres[categoryGenruColor].Color.R, musicGenres[categoryGenruColor].Color.G, musicGenres[categoryGenruColor].Color.B, byte.MaxValue));
				}
			}
			Timer += GameManager.GetGameMSecAdd();
		}

		public void UpdateElement()
		{
			_tabCount.text = GetElementText();
		}

		private string GetTagName(int diff)
		{
			return _selectProcess.GetCategoryName(_playerIndex, diff).Replace("\\n", " ");
		}

		public void SetClearRankMedal(MusicClearrankID rank)
		{
			if (rank < MusicClearrankID.Rank_S || MusicClearrankID.End <= rank)
			{
				_tabPlateAnimator.gameObject.SetActive(value: false);
				_clearRankImage.gameObject.SetActive(value: false);
				return;
			}
			if (!_tabPlateAnimator.gameObject.activeSelf)
			{
				_tabPlateAnimator.gameObject.SetActive(value: true);
				_tabPlateAnimator.Rebind();
			}
			if (!_clearRankImage.gameObject.activeSelf)
			{
				_clearRankImage.gameObject.SetActive(value: true);
			}
			if (rank >= MusicClearrankID.Rank_SSS)
			{
				_tabPlateBackImage.color = _tabPlateBackColor[1];
				_tabPlateStarImage.color = _tabPlateStarColor[1];
				_tabPlateLineImage.color = _tabPlateLineColor[1];
			}
			else
			{
				_tabPlateBackImage.color = _tabPlateBackColor[0];
				_tabPlateStarImage.color = _tabPlateStarColor[0];
				_tabPlateLineImage.color = _tabPlateLineColor[0];
			}
			_clearRankImage.ChangeSprite((int)(rank - 8));
		}

		public void SetComboMedal(PlayComboflagID combo)
		{
			if (combo <= PlayComboflagID.None || PlayComboflagID.End <= combo)
			{
				_comboImage.gameObject.SetActive(value: false);
				return;
			}
			if (!_comboImage.gameObject.activeSelf)
			{
				_comboImage.gameObject.SetActive(value: true);
			}
			_comboImage.ChangeSprite((int)(combo - 1));
		}

		public void SetDxMedal(MusicClearrankID rank, PlayComboflagID combo)
		{
			bool flag = rank >= MusicClearrankID.Rank_S && combo >= PlayComboflagID.Silver;
			_dxMedalImage.gameObject.SetActive(flag);
			if (flag)
			{
				if (rank >= MusicClearrankID.Rank_SSS && combo >= PlayComboflagID.AllPerfect)
				{
					_dxMedalImage.ChangeSprite(1);
				}
				else
				{
					_dxMedalImage.ChangeSprite(0);
				}
			}
		}

		public void CategoryScroll(bool toRight)
		{
			if (_isActive)
			{
				if (base.gameObject.activeSelf)
				{
					_splitFlap.SetTrigger(toRight ? "ChangeRight" : "ChangeLeft");
				}
				IsAnimation = true;
				MedalCheck();
			}
		}

		public void CategoryScroll(Direction direction)
		{
			if (_isActive)
			{
				Scroll(direction);
				IsAnimation = true;
				MedalCheck();
			}
		}

		public void SetVisibleAnimation(bool isVisible)
		{
			if (_isVisible != isVisible)
			{
				_isVisible = isVisible;
				if (base.gameObject.activeSelf)
				{
					_splitFlap.SetTrigger(isVisible ? "In" : "Out");
				}
			}
		}

		private string GetElementText()
		{
			return _selectProcess.GetCurrentListIndex(_playerIndex) + 1 + " / " + _selectProcess.GetCurrentCategoryMax(_playerIndex);
		}
	}
}
