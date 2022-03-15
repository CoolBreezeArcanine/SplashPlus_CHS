using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DB;
using MAI2.Util;
using Manager;
using UnityEngine;

namespace Monitor.Common
{
	public class OperationInformationController : MonoBehaviour
	{
		public enum InformationType
		{
			Hide,
			OnePlayer,
			TwoPlayer,
			Freedom,
			TimerSkip,
			AimeX_CoinX_Zero,
			AimeX_CoinX_NZero,
			AimeX_CoinO,
			AimeO_CoinX_Zero,
			AimeO_CoinX_NZero,
			AimeO_CoinO,
			ReaderX_CoinX_Zero,
			ReaderX_CoinX_NZero,
			ReaderX_CoinO,
			Maintenance,
			OServerX_CoinX_Zero,
			OServerX_CoinX_NZero,
			OServerX_CoinO,
			Nop
		}

		private class LayerParam
		{
			public readonly string Message;

			public readonly bool IsAttention;

			public readonly bool ShowButton;

			public LayerParam(CommonMessageID mId, bool attention = false, bool button = false)
			{
				Message = mId.GetName();
				IsAttention = attention;
				ShowButton = button;
			}
		}

		[SerializeField]
		private Sprite[] _sprites;

		[SerializeField]
		private List<OperationInformationLayerObject> _layers;

		private Animator _animator;

		private Coroutine _blink;

		private InformationType _cacheType;

		private InformationType _requestType = InformationType.Nop;

		private static readonly Dictionary<InformationType, LayerParam[]> TypeParams = new Dictionary<InformationType, LayerParam[]>
		{
			[InformationType.Hide] = Array.Empty<LayerParam>(),
			[InformationType.OnePlayer] = new LayerParam[1]
			{
				new LayerParam(CommonMessageID.Entry_OnePlayer)
			},
			[InformationType.TwoPlayer] = new LayerParam[1]
			{
				new LayerParam(CommonMessageID.Entry_TwoPlayer)
			},
			[InformationType.Freedom] = new LayerParam[1]
			{
				new LayerParam(CommonMessageID.Entry_Freedom)
			},
			[InformationType.TimerSkip] = new LayerParam[1]
			{
				new LayerParam(CommonMessageID.Entry_TimeSkip)
			},
			[InformationType.AimeX_CoinX_Zero] = new LayerParam[2]
			{
				new LayerParam(CommonMessageID.Entry_InfoAime),
				new LayerParam(CommonMessageID.Entry_InfoCoin)
			},
			[InformationType.AimeX_CoinX_NZero] = new LayerParam[2]
			{
				new LayerParam(CommonMessageID.Entry_InfoAime),
				new LayerParam(CommonMessageID.Entry_InfoMoreCoin, attention: true)
			},
			[InformationType.AimeX_CoinO] = new LayerParam[2]
			{
				new LayerParam(CommonMessageID.Entry_InfoAime),
				new LayerParam(CommonMessageID.Entry_InfoButton, attention: false, button: true)
			},
			[InformationType.AimeO_CoinX_Zero] = new LayerParam[1]
			{
				new LayerParam(CommonMessageID.Entry_InfoCoin)
			},
			[InformationType.AimeO_CoinX_NZero] = new LayerParam[1]
			{
				new LayerParam(CommonMessageID.Entry_InfoMoreCoin, attention: true)
			},
			[InformationType.AimeO_CoinO] = new LayerParam[1]
			{
				new LayerParam(CommonMessageID.Entry_InfoButton, attention: false, button: true)
			},
			[InformationType.ReaderX_CoinX_Zero] = new LayerParam[2]
			{
				new LayerParam(CommonMessageID.AimeOffline, attention: true),
				new LayerParam(CommonMessageID.Entry_InfoCoin)
			},
			[InformationType.ReaderX_CoinX_NZero] = new LayerParam[2]
			{
				new LayerParam(CommonMessageID.AimeOffline, attention: true),
				new LayerParam(CommonMessageID.Entry_InfoMoreCoin, attention: true)
			},
			[InformationType.ReaderX_CoinO] = new LayerParam[2]
			{
				new LayerParam(CommonMessageID.AimeOffline, attention: true),
				new LayerParam(CommonMessageID.Entry_InfoButton, attention: false, button: true)
			},
			[InformationType.Maintenance] = new LayerParam[1]
			{
				new LayerParam(CommonMessageID.UnderServerMaintenance, attention: true)
			},
			[InformationType.OServerX_CoinX_Zero] = new LayerParam[2]
			{
				new LayerParam(CommonMessageID.Entry_OldSrvDisconnect, attention: true),
				new LayerParam(CommonMessageID.Entry_InfoCoin)
			},
			[InformationType.OServerX_CoinX_NZero] = new LayerParam[2]
			{
				new LayerParam(CommonMessageID.Entry_OldSrvDisconnect, attention: true),
				new LayerParam(CommonMessageID.Entry_InfoMoreCoin, attention: true)
			},
			[InformationType.OServerX_CoinO] = new LayerParam[2]
			{
				new LayerParam(CommonMessageID.Entry_OldSrvDisconnect, attention: true),
				new LayerParam(CommonMessageID.Entry_InfoButton, attention: false, button: true)
			}
		};

		public void Initialize()
		{
			_animator = GetComponent<Animator>();
			_animator.PlayInFixedTime("Out", -1, float.MaxValue);
			foreach (OperationInformationLayerObject layer in _layers)
			{
				layer.Initialize();
			}
		}

		public void ViewUpdate()
		{
			if (_requestType != InformationType.Nop && !IsPlaying() && _layers.All((OperationInformationLayerObject l) => !l.IsPlaying()))
			{
				ChangeInformation(_requestType);
				_requestType = InformationType.Nop;
			}
			foreach (OperationInformationLayerObject layer in _layers)
			{
				layer.ViewUpdate();
			}
		}

		public void RequestInformation(InformationType type)
		{
			_requestType = ((_cacheType != type) ? type : InformationType.Nop);
		}

		public void RequestInformation(bool isCredit, bool isCoinZero, bool isAimeUser, bool isAimeUnavailable, bool isMaintenance)
		{
			InformationType informationType = InformationType.Nop;
			if (isMaintenance)
			{
				informationType = InformationType.Maintenance;
				RequestInformation(informationType);
			}
			else if (isAimeUnavailable)
			{
				informationType = (isCredit ? InformationType.ReaderX_CoinO : ((!isCoinZero) ? InformationType.ReaderX_CoinX_NZero : InformationType.ReaderX_CoinX_Zero));
				RequestInformation(informationType);
			}
			else if (isAimeUser)
			{
				informationType = (isCredit ? InformationType.AimeO_CoinO : ((!isCoinZero) ? InformationType.AimeO_CoinX_NZero : InformationType.AimeO_CoinX_Zero));
				RequestInformation(informationType);
			}
			else if (Singleton<OperationManager>.Instance.IsAimeLoginDisable())
			{
				informationType = (isCredit ? InformationType.AimeO_CoinO : ((!isCoinZero) ? InformationType.AimeO_CoinX_NZero : InformationType.AimeO_CoinX_Zero));
				RequestInformation(informationType);
			}
			else
			{
				informationType = (isCredit ? InformationType.AimeX_CoinO : ((!isCoinZero) ? InformationType.AimeX_CoinX_NZero : InformationType.AimeX_CoinX_Zero));
				RequestInformation(informationType);
			}
		}

		private void ChangeInformation(InformationType type)
		{
			if (_blink != null)
			{
				StopCoroutine(_blink);
				_blink = null;
			}
			_cacheType = type;
			int num = TypeParams[type].Length;
			if (num == 0)
			{
				_animator.Play("Out");
				return;
			}
			for (int i = 0; i < num; i++)
			{
				_layers[i].SetParameter(i != 0, TypeParams[type][i].IsAttention ? _sprites[1] : _sprites[0], TypeParams[type][i].Message, TypeParams[type][i].ShowButton);
			}
			if (num > 1)
			{
				_blink = StartCoroutine(BlinkCoroutine(_layers[0], _layers[1]));
			}
			_animator.Play("In");
			_layers[0].Play("In");
			_layers[1].Play("Out");
		}

		public bool IsPlaying()
		{
			return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f;
		}

		private static IEnumerator BlinkCoroutine(OperationInformationLayerObject layer0, OperationInformationLayerObject layer1)
		{
			while (true)
			{
				yield return new WaitForSeconds(5f);
				layer0.Play("Out");
				layer1.Play("In");
				yield return new WaitForSeconds(5f);
				layer0.Play("In");
				layer1.Play("Out");
			}
		}
	}
}
