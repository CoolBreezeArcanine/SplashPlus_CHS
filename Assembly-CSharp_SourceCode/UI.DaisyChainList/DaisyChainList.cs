using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace UI.DaisyChainList
{
	public class DaisyChainList : MonoBehaviour
	{
		public const float ScrollTime = 100f;

		public const int ListCount = 9;

		public const int AnimationSlotCount = 5;

		public const int Center = 4;

		public const int MenuCount = 4;

		[SerializeField]
		protected Transform _viewTransform;

		[SerializeField]
		private ChainObject _originalChainObject;

		[SerializeField]
		private SeparateBaseChainObject _originalSeparateChainObject;

		[SerializeField]
		private Transform _activeSelectTarget;

		protected bool IsScrolling;

		private float _syncTimer;

		private float _scrollTimer;

		private float _timer;

		private bool _isInAnimation;

		protected readonly List<ChainObject> ChainObjectList = new List<ChainObject>();

		protected readonly List<SeparateBaseChainObject> SeparateObjectList = new List<SeparateBaseChainObject>();

		protected ChainObject[] SpotArray;

		protected ChainObject ScrollChainCard;

		protected ChainObject[] AnimationSpotArray;

		protected Coroutine CurrentCoroutine;

		private Direction _scrollToDirection;

		private Animator _chainListAnimator;

		public bool IsListEnable { get; set; }

		private void Awake()
		{
			_chainListAnimator = GetComponent<Animator>();
		}

		public virtual void Initialize()
		{
			SpotArray = new ChainObject[9];
			AnimationSpotArray = new ChainObject[5];
			for (int i = 0; i < 9; i++)
			{
				ChainObject chainObject = Object.Instantiate(_originalChainObject, _viewTransform);
				chainObject.name = chainObject.name.Replace("(Clone)", i.ToString("00"));
				chainObject.SetChainActive(isActive: false);
				ChainObjectList.Add(chainObject);
				if (_originalSeparateChainObject != null)
				{
					SeparateBaseChainObject separateBaseChainObject = Object.Instantiate(_originalSeparateChainObject, _viewTransform);
					separateBaseChainObject.name = separateBaseChainObject.name.Replace("(Clone)", "_" + base.gameObject.name + "_" + i.ToString("00"));
					separateBaseChainObject.SetChainActive(isActive: false);
					SeparateObjectList.Add(separateBaseChainObject);
				}
			}
			ScrollChainCard = Object.Instantiate(_originalChainObject, _viewTransform);
			ScrollChainCard.name = _originalChainObject.name + "_ScrollCard";
			ScrollChainCard.ResetChain();
			ScrollChainCard.SetChainActive(isActive: false);
		}

		public virtual void Deploy()
		{
			Positioning(isImmediate: true);
			IsListEnable = true;
		}

		public bool Scroll(Direction direction)
		{
			if (!IsListEnable)
			{
				return false;
			}
			SeparateBaseChainObject separateBaseChainObject = SpotArray[(int)(4 + direction)] as SeparateBaseChainObject;
			bool flag = separateBaseChainObject != null;
			if (flag)
			{
				separateBaseChainObject.SetScrollDirection(direction);
			}
			int num = ((!flag) ? 1 : 2);
			for (int i = 0; i < num; i++)
			{
				ChainObject[] array = new ChainObject[SpotArray.Length];
				for (int j = 0; j < SpotArray.Length; j++)
				{
					if (j + 1 < SpotArray.Length)
					{
						if (j + direction == (Direction)4 && SpotArray[(int)(j + direction)] != null && SpotArray[j + (int)direction * 2] != null)
						{
							SpotArray[j + (int)direction * 2].OnCenterIn();
							SpotArray[j + (int)direction * 2].transform.SetAsLastSibling();
							SpotArray[(int)(j + direction)].OnCenterOut();
						}
						if (direction == Direction.Right)
						{
							array[j + 1] = SpotArray[j];
						}
						else
						{
							array[j] = SpotArray[j + 1];
						}
					}
					else
					{
						Remove((direction == Direction.Right) ? SpotArray[SpotArray.Length - 1] : SpotArray[0]);
					}
				}
				SpotArray = array;
				Next((4 - (num - 1) + i) * (int)direction, direction);
			}
			_scrollToDirection = direction;
			Positioning(isImmediate: false);
			return flag;
		}

		public virtual void SetScrollCard(bool isVisible)
		{
			ScrollChainCard.SetChainActive(isVisible);
			if (isVisible)
			{
				ScrollChainCard.transform.SetAsLastSibling();
			}
		}

		protected virtual void Next(int targetIndex, Direction direction)
		{
			ChainObject chain = GetChain<ChainObject>();
			SetSpot((direction != Direction.Right) ? (SpotArray.Length - 1) : 0, chain);
		}

		protected virtual void Remove(ChainObject chain)
		{
			if (chain != null)
			{
				chain.ResetChain();
				chain.SetChainActive(isActive: false);
				chain.transform.localPosition = Vector3.zero;
			}
		}

		public virtual void RemoveAll()
		{
			IsListEnable = false;
			if (SpotArray == null)
			{
				return;
			}
			for (int i = 0; i < 9; i++)
			{
				if (!(SpotArray[i] == null))
				{
					Remove(SpotArray[i]);
					SpotArray[i] = null;
				}
			}
		}

		public void RemoveOut(MonoBehaviour behaviour)
		{
			if (CurrentCoroutine == null)
			{
				CurrentCoroutine = behaviour.StartCoroutine(RemoveOutCoroutine());
			}
		}

		public void RemoveOut()
		{
			if (CurrentCoroutine == null)
			{
				CurrentCoroutine = StartCoroutine(RemoveOutCoroutine());
			}
		}

		protected IEnumerator RemoveOutCoroutine()
		{
			PlayOut();
			yield return new WaitForSeconds(0.28f);
			RemoveAll();
			CurrentCoroutine = null;
		}

		public virtual void ViewUpdate()
		{
			if (SpotArray == null)
			{
				return;
			}
			ChainObject[] spotArray = SpotArray;
			foreach (ChainObject chainObject in spotArray)
			{
				if (chainObject != null)
				{
					chainObject.ViewUpdate(_syncTimer / 3f);
				}
			}
			if (IsScrolling)
			{
				spotArray = SpotArray;
				foreach (ChainObject chainObject2 in spotArray)
				{
					if (chainObject2 != null)
					{
						chainObject2.ScrollUpdate(_scrollTimer / 100f);
					}
				}
				_scrollTimer += GameManager.GetGameMSecAdd();
				if (_scrollTimer > 100f)
				{
					_scrollTimer = 0f;
					IsScrolling = false;
					spotArray = SpotArray;
					foreach (ChainObject chainObject3 in spotArray)
					{
						if (chainObject3 != null)
						{
							chainObject3.ScrollUpdate(1f);
						}
					}
					SpotArray[4]?.OnCenter();
					SpotArray[(int)(4 - _scrollToDirection)]?.OnCenterOutEnd();
				}
			}
			if (_isInAnimation)
			{
				spotArray = AnimationSpotArray;
				foreach (ChainObject chainObject4 in spotArray)
				{
					if (chainObject4 != null)
					{
						chainObject4.ScrollUpdate(_timer / 267f);
					}
				}
				if (_timer >= 267f)
				{
					_timer = 0f;
					_isInAnimation = false;
					if (_activeSelectTarget != null)
					{
						SpotArray[4]?.transform.SetParent(_viewTransform);
						SpotArray[4]?.transform.SetAsLastSibling();
					}
				}
				_timer += GameManager.GetGameMSecAdd();
			}
			_syncTimer += Time.deltaTime;
			if (_syncTimer >= 3f)
			{
				_syncTimer = 0f;
			}
		}

		public virtual void Play()
		{
			_timer = 0f;
			_isInAnimation = true;
			if (SpotArray == null)
			{
				return;
			}
			int num = 0;
			int num2 = -1;
			int num3 = SpotArray.Length;
			for (int i = 0; i <= 4; i++)
			{
				if (SpotArray[i] != null && SpotArray[i] is SeparateBaseChainObject)
				{
					num2 = i;
				}
			}
			for (int num4 = SpotArray.Length - 1; num4 > 4; num4--)
			{
				if (SpotArray[num4] != null && SpotArray[num4] is SeparateBaseChainObject)
				{
					num3 = num4;
				}
			}
			for (int j = 0; j < SpotArray.Length; j++)
			{
				if (!(SpotArray[j] != null))
				{
					continue;
				}
				if (j >= 2 && j <= 6 && j >= num2 && j <= num3)
				{
					if (j == 4)
					{
						SpotArray[j].transform.SetAsLastSibling();
					}
					AnimationSpotArray[num] = SpotArray[j];
					AnimationSpotArray[num].BeginScroll(Vector3.zero, AnimationSpotArray[num].FromPosition, AnimationSpotArray[num].transform.localScale);
					num++;
				}
				else
				{
					SpotArray[j].IsAnimation = false;
					SpotArray[j].Immediate(SpotArray[j].FromPosition, SpotArray[j].transform.localScale);
				}
			}
		}

		public void PlayOut()
		{
			if (SpotArray == null)
			{
				return;
			}
			_timer = 0f;
			_isInAnimation = true;
			int num = 0;
			for (int i = 2; i <= 6; i++)
			{
				if (!(SpotArray[i] != null))
				{
					continue;
				}
				if (i == 4)
				{
					if (_activeSelectTarget != null)
					{
						_activeSelectTarget.SetAsLastSibling();
						SpotArray[i].transform.SetParent(_activeSelectTarget);
					}
				}
				else
				{
					float x = (SpotArray[i].GetSize(i == 4).x + 540f) * (float)((i >= 4) ? 1 : (-1));
					Vector3 toPosition = new Vector3(x, 0f, 0f);
					AnimationSpotArray[num] = SpotArray[i];
					AnimationSpotArray[num].BeginScroll(AnimationSpotArray[num].transform.localPosition, toPosition, AnimationSpotArray[num].transform.localScale);
					num++;
				}
			}
			if (_chainListAnimator != null)
			{
				_chainListAnimator.SetTrigger("ActiveSelect");
			}
		}

		public void Positioning(bool isImmediate, bool isAnimation = false)
		{
			if (SpotArray == null)
			{
				return;
			}
			float num = 0f;
			float num2 = ((SpotArray[4] != null) ? ((0f - SpotArray[4].GetSize(isCenter: true).x) / 2f) : 0f);
			for (int i = 0; i <= 4; i++)
			{
				if (SpotArray[i] != null)
				{
					num = ((i != 4) ? (num + SpotArray[i].GetSize().x * SpotArray[i].InactiveScale.x) : (num + SpotArray[i].GetSize(isCenter: true).x * SpotArray[i].ActiveScale.x / 2f));
				}
			}
			SpotArray[4]?.transform.SetAsLastSibling();
			int num3 = SpotArray[4]?.transform.GetSiblingIndex() ?? (-1);
			for (int j = 0; j <= 4; j++)
			{
				if (!(SpotArray[j] != null))
				{
					continue;
				}
				Vector3 vector = ((j == 4) ? SpotArray[j].ActiveScale : SpotArray[j].InactiveScale);
				Vector3 vector2 = new Vector3(0f - num + SpotArray[j].GetSize(j == 4).x * vector.x / 2f, 0f, 0f);
				if (isImmediate || SpotArray[j].IsSetSpot)
				{
					if (isAnimation)
					{
						if (j >= 2)
						{
							SpotArray[j].IsAnimation = true;
						}
						if (SpotArray[j] is SeparateBaseChainObject)
						{
							((SeparateBaseChainObject)SpotArray[j]).SetVisible(isVisible: false, Direction.Left);
							((SeparateBaseChainObject)SpotArray[j]).SetVisible(isVisible: false, Direction.Right);
						}
					}
					else
					{
						SpotArray[j].IsAnimation = false;
					}
					SpotArray[j].Immediate(vector2, vector);
				}
				else
				{
					SpotArray[j].BeginScroll(vector2, vector);
				}
				num -= SpotArray[j].GetSize(j == 4).x * vector.x;
				SpotArray[j].transform.SetSiblingIndex(num3 - (4 - j) - 1);
			}
			for (int k = 4; k < SpotArray.Length; k++)
			{
				if (!(SpotArray[k] != null))
				{
					continue;
				}
				Vector3 vector3 = ((k == 4) ? SpotArray[k].ActiveScale : SpotArray[k].InactiveScale);
				Vector3 vector4 = new Vector3(num2 + SpotArray[k].GetSize(k == 4).x * vector3.x / 2f, 0f, 0f);
				if (isImmediate || SpotArray[k].IsSetSpot)
				{
					if (isAnimation)
					{
						if (k <= 6)
						{
							SpotArray[k].IsAnimation = true;
						}
						if (SpotArray[k] is SeparateBaseChainObject)
						{
							((SeparateBaseChainObject)SpotArray[k]).SetVisible(isVisible: false, Direction.Left);
							((SeparateBaseChainObject)SpotArray[k]).SetVisible(isVisible: false, Direction.Right);
						}
					}
					else
					{
						SpotArray[k].IsAnimation = false;
					}
					SpotArray[k].Immediate(vector4, vector3);
				}
				else
				{
					SpotArray[k].BeginScroll(vector4, vector3);
				}
				num2 += SpotArray[k].GetSize(k == 4).x * vector3.x;
				SpotArray[k].transform.SetSiblingIndex(num3 - (k - 4) - 1);
			}
			IsScrolling = !isImmediate;
		}

		protected virtual void SetSpot(int index, ChainObject chain)
		{
			SpotArray[index] = chain;
			chain.IsSetSpot = true;
		}

		protected T GetChain<T>() where T : ChainObject
		{
			for (int i = 0; i < ChainObjectList.Count; i++)
			{
				if (!ChainObjectList[i].IsUsed)
				{
					ChainObjectList[i].SetChainActive(isActive: true);
					if (!ChainObjectList[i].gameObject.activeSelf)
					{
						ChainObjectList[i].gameObject.SetActive(value: true);
					}
					return (T)ChainObjectList[i];
				}
			}
			return null;
		}

		protected SeparateBaseChainObject GetSeparate(string left, string right)
		{
			for (int i = 0; i < SeparateObjectList.Count; i++)
			{
				if (!SeparateObjectList[i].IsUsed)
				{
					SeparateObjectList[i].SetData(left, right);
					SeparateObjectList[i].SetChainActive(isActive: true);
					if (!SeparateObjectList[i].gameObject.activeSelf)
					{
						SeparateObjectList[i].gameObject.SetActive(value: true);
					}
					return SeparateObjectList[i];
				}
			}
			return null;
		}

		protected T GetCard<T>(int index) where T : ChainObject
		{
			if (index < ChainObjectList.Count)
			{
				return (T)ChainObjectList[index];
			}
			return null;
		}

		protected int GetCardListCount()
		{
			return ChainObjectList.Count;
		}

		public void Release()
		{
		}
	}
}
