using System.Linq;
using Monitor.MapCore;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.MapResult.Parts
{
	public class OdoSpriteText : MapBehaviour
	{
		[SerializeField]
		private bool _useMeter = true;

		[SerializeField]
		private Sprite[] _srcSignSprites;

		[SerializeField]
		private Sprite[] _srcNumSprites;

		[SerializeField]
		private Image[] _dstNumImages;

		[SerializeField]
		private GameObject _dstSignGo;

		[SerializeField]
		private GameObject _dstCommaGo;

		[SerializeField]
		private GameObject _dstPeriodGo;

		[SerializeField]
		private GameObject _dstKiloGo;

		[SerializeField]
		private GameObject _dstMeterGo;

		[SerializeField]
		private bool _plusSign;

		private Image _signImage;

		private int _cacheOdo = -1;

		private Image DstSignImage
		{
			get
			{
				if (_signImage == null && _dstSignGo != null)
				{
					_signImage = _dstSignGo.GetComponentInChildren<Image>();
				}
				return _signImage;
			}
		}

		public void SetOdoText(int odo)
		{
			if (_cacheOdo != odo)
			{
				SetSign(odo);
				SetNumber(odo);
				_cacheOdo = odo;
			}
		}

		private void SetSign(int odo)
		{
			if (_srcSignSprites == null)
			{
				return;
			}
			if (Mathf.Sign(odo) >= 0f)
			{
				if (DstSignImage != null)
				{
					DstSignImage.sprite = _srcSignSprites[0];
				}
				if (_dstSignGo != null)
				{
					_dstSignGo.SetActive(_plusSign);
				}
			}
			else
			{
				if (DstSignImage != null)
				{
					DstSignImage.sprite = _srcSignSprites[1];
				}
				if (_dstSignGo != null)
				{
					_dstSignGo.SetActive(value: true);
				}
			}
		}

		private void SetNumber(int odo)
		{
			odo = Mathf.Abs(odo);
			string text = odo.ToString();
			if (text.Length > _dstNumImages.Length)
			{
				return;
			}
			int num = ((8 < _dstNumImages.Length) ? 3 : ((4 < _dstNumImages.Length) ? 2 : 0));
			Image[] array;
			if (_useMeter && odo <= 9999)
			{
				if (_dstKiloGo != null)
				{
					_dstKiloGo.SetActive(value: false);
				}
				if (_dstMeterGo != null)
				{
					_dstMeterGo.SetActive(value: true);
				}
				if (_dstCommaGo != null)
				{
					_dstCommaGo.SetActive(text.Length > 3);
				}
				if (_dstPeriodGo != null)
				{
					_dstPeriodGo.SetActive(value: false);
				}
				array = _dstNumImages.Skip(num).ToArray();
				for (int i = 0; i < num; i++)
				{
					_dstNumImages[i].gameObject.SetActive(value: false);
				}
			}
			else
			{
				if (_dstKiloGo != null)
				{
					_dstKiloGo.SetActive(value: true);
				}
				if (_dstMeterGo != null)
				{
					_dstMeterGo.SetActive(value: false);
				}
				if (_dstCommaGo != null)
				{
					_dstCommaGo.SetActive(text.Length > 7);
				}
				if (_dstPeriodGo != null && _useMeter)
				{
					_dstPeriodGo.SetActive(value: true);
				}
				else if (_dstPeriodGo != null)
				{
					_dstPeriodGo.SetActive(value: false);
				}
				array = _dstNumImages;
				int num2 = ((odo == 0) ? 1 : ((int)Mathf.Log10(odo) + 1));
				if (8 < num2)
				{
					odo = ((8 < _dstNumImages.Length) ? odo : Mathf.FloorToInt((float)odo / 10f));
				}
				text = odo.ToString();
			}
			char[] array2 = text.Reverse().ToArray();
			if (_srcNumSprites != null && array != null && array2 != null)
			{
				for (int j = 0; j < array2.Length; j++)
				{
					array[j].sprite = _srcNumSprites[array2[j] - 48];
					array[j].gameObject.SetActive(value: true);
				}
			}
			if (array == null || array2 == null)
			{
				return;
			}
			for (int k = array2.Length; k < array.Length; k++)
			{
				if (array[k] != null)
				{
					array[k].gameObject.SetActive(value: false);
				}
			}
		}
	}
}
