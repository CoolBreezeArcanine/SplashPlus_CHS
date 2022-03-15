using Process;
using UnityEngine;

namespace Monitor
{
	public class NoteGuide : MonoBehaviour
	{
		public enum Color
		{
			Normal,
			Each,
			Slide,
			Break,
			Ex,
			Max
		}

		private SpriteRenderer _spriteRender;

		protected Material _eachGuideMaterial;

		private SpriteRenderer _spriteEachRender;

		protected const string ShaderAmountStr = "_Amount";

		private int _eachIndex = -1;

		public Transform ParentTransform { get; set; }

		public int EachIndex => _eachIndex;

		public void SetColor(Color color)
		{
			_spriteRender.sprite = GameNoteImageContainer.Guide[(int)color];
		}

		public void SetAlpha(float alpha)
		{
			_spriteRender.color = new UnityEngine.Color(1f, 1f, 1f, alpha);
		}

		public void Initialize(int angle, int eachIndex)
		{
			base.transform.localScale = new Vector3(1f, 1f, 1f);
			_eachIndex = eachIndex;
			if (angle != 0 && _eachGuideMaterial.HasProperty("_Amount"))
			{
				if (angle == 180)
				{
					_eachGuideMaterial.SetFloat("_Amount", 1f);
				}
				else
				{
					_eachGuideMaterial.SetFloat("_Amount", (float)Mathf.Abs(angle) / 360f);
				}
				_spriteEachRender.flipX = angle < 0;
				_spriteEachRender.gameObject.SetActive(value: true);
			}
			else
			{
				_spriteEachRender.gameObject.SetActive(value: false);
			}
		}

		private void Awake()
		{
			_spriteRender = base.gameObject.GetComponent<SpriteRenderer>();
			_spriteEachRender = base.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
			_eachGuideMaterial = _spriteEachRender.material;
		}

		public void ReturnToBase()
		{
			base.gameObject.SetActive(value: false);
			base.transform.SetParent(ParentTransform, worldPositionStays: false);
		}

		public void HideEachGuide()
		{
			if (_spriteEachRender.gameObject.activeSelf)
			{
				_spriteEachRender.gameObject.SetActive(value: false);
			}
		}
	}
}
