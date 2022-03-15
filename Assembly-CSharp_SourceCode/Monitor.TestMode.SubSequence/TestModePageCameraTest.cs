using System;
using System.Diagnostics;
using DB;
using IO;
using MAI2System;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Monitor.TestMode.SubSequence
{
	public class TestModePageCameraTest : TestModePage
	{
		private enum State
		{
			Start,
			Initialize,
			Proc,
			Finish
		}

		private enum Status
		{
			NA,
			None,
			BAD,
			Warn,
			WarnBad,
			OK
		}

		private enum CardType
		{
			NA,
			Unknown,
			Normal,
			End
		}

		private struct Unit
		{
			public int _index;

			public Status _cameraStatus;

			public Status _readStatus;

			public CardType _readResult;
		}

		public const float InitTime = 20f;

		private State _state;

		protected Stopwatch Timer = new Stopwatch();

		private bool _isTimeOut;

		private readonly RawImage[] _camIDImages = new RawImage[3];

		private TextMeshProUGUI _textProgress;

		private readonly TextMeshProUGUI[] _textCameraStatus = new TextMeshProUGUI[3];

		private readonly TextMeshProUGUI[] _textCameraIDs = new TextMeshProUGUI[3];

		private readonly TextMeshProUGUI[] _textCards = new TextMeshProUGUI[3];

		private readonly TextMeshProUGUI[] _textStatus = new TextMeshProUGUI[3];

		private Unit[] _units = new Unit[3];

		private const string NoneString = "";

		private const string InvalidString = "-----";

		private const string InvalidUnknown = "UNKNOWN";

		protected override void OnCreate()
		{
			base.OnCreate();
			Transform transform = base.transform;
			_camIDImages[0] = transform.Find("ImageLeft").GetComponent<RawImage>();
			_camIDImages[1] = transform.Find("ImageRight").GetComponent<RawImage>();
			_camIDImages[2] = transform.Find("ImagePhoto").GetComponent<RawImage>();
			_textProgress = transform.Find("LabelCameraView").Find("Progress").GetComponent<TextMeshProUGUI>();
			Transform transform2 = transform.Find("LabelCameraStatus");
			_textCameraStatus[0] = transform2.Find("Status0").GetComponent<TextMeshProUGUI>();
			_textCameraStatus[1] = transform2.Find("Status1").GetComponent<TextMeshProUGUI>();
			_textCameraStatus[2] = transform2.Find("Status2").GetComponent<TextMeshProUGUI>();
			Transform transform3 = transform.Find("LabelCodeReader");
			_textCameraIDs[0] = transform3.Find("ID0").GetComponent<TextMeshProUGUI>();
			_textCameraIDs[1] = transform3.Find("ID1").GetComponent<TextMeshProUGUI>();
			_textCameraIDs[2] = transform3.Find("ID2").GetComponent<TextMeshProUGUI>();
			Transform transform4 = transform.Find("LabelDetectedCard");
			_textCards[0] = transform4.Find("Card0").GetComponent<TextMeshProUGUI>();
			_textCards[1] = transform4.Find("Card1").GetComponent<TextMeshProUGUI>();
			_textCards[2] = transform4.Find("Card2").GetComponent<TextMeshProUGUI>();
			Transform transform5 = transform.Find("LabelStatus");
			_textStatus[0] = transform5.Find("Status0").GetComponent<TextMeshProUGUI>();
			_textStatus[1] = transform5.Find("Status1").GetComponent<TextMeshProUGUI>();
			_textStatus[2] = transform5.Find("Status2").GetComponent<TextMeshProUGUI>();
			for (int i = 0; i < 3; i++)
			{
				_textCameraStatus[i].text = string.Empty;
				_textCameraIDs[i].text = "-----";
				_textCards[i].text = "-----";
				_textStatus[i].text = "-----";
				_units[i]._index = i;
			}
			_state = State.Start;
			_textProgress.text = string.Empty;
			Timer.Restart();
		}

		protected override void OnUpdate()
		{
			_ = MechaManager.QrReader;
			switch (_state)
			{
			case State.Start:
				ProcInit();
				break;
			case State.Initialize:
				_textProgress.text = (Utility.isBlinkDisp(Timer) ? TestmodeCameraID.CameraInit.GetName() : string.Empty);
				if (CameraManager.IsReady)
				{
					ProcInitializeEnd();
				}
				break;
			case State.Proc:
				UpdateCameraStatus();
				UpdateText();
				break;
			case State.Finish:
				break;
			}
		}

		protected override void Destroy()
		{
			for (int i = 0; i < 3; i++)
			{
				_camIDImages[i].texture = null;
			}
			if (MechaManager.QrReader != null)
			{
				MechaManager.QrReader.Stop();
			}
			WebCamManager.Stop();
			LedBlockID.QrLed_1P.SetColor(Color.black);
			LedBlockID.QrLed_2P.SetColor(Color.black);
		}

		private void ProcInit()
		{
			_textProgress.text = string.Empty;
			_state = State.Initialize;
			CameraManager.Initialize(this);
			WebCamManager.Reset();
			WebCamManager.Initialize(this);
			MechaManager.QrReader.Initialize(this);
		}

		private void ProcInitializeEnd()
		{
			SetCameraTextures();
			_textProgress.text = string.Empty;
			_state = State.Proc;
			WebCamManager.Play();
			MechaManager.QrReader.Play();
		}

		private void SetCameraTextures()
		{
			for (int i = 0; i < 3; i++)
			{
				_camIDImages[i].texture = CameraManager.GetTexture((CameraManager.CameraTypeEnum)i);
			}
		}

		private void UpdateCameraStatus()
		{
			QrCamera qrReader = MechaManager.QrReader;
			for (int i = 0; i < 2; i++)
			{
				if (CameraManager.IsAvailableCameras[i])
				{
					if (qrReader.Warnning(i))
					{
						_units[i]._cameraStatus = Status.Warn;
					}
					else
					{
						_units[i]._cameraStatus = Status.OK;
					}
				}
				else if (qrReader.MayBeCodeCamera(i))
				{
					_units[i]._cameraStatus = Status.WarnBad;
				}
				else
				{
					_units[i]._cameraStatus = Status.BAD;
				}
			}
			qrReader.Update();
			for (int j = 0; j < 2; j++)
			{
				if (!qrReader.Exists(j))
				{
					_units[j]._readStatus = Status.None;
					_units[j]._readResult = CardType.End;
					continue;
				}
				switch (qrReader.TryTestModeParse(5915972u, j))
				{
				case 1:
					_units[j]._readStatus = Status.OK;
					_units[j]._readResult = CardType.Normal;
					break;
				default:
					_units[j]._readStatus = Status.BAD;
					_units[j]._readResult = CardType.Unknown;
					break;
				case 2:
				case 3:
					_units[j]._readStatus = Status.None;
					_units[j]._readResult = CardType.Unknown;
					break;
				case 0:
					_units[j]._readStatus = Status.None;
					_units[j]._readResult = CardType.End;
					break;
				}
			}
			if (!WebCamManager.IsAvailableCamera())
			{
				_units[2]._cameraStatus = Status.BAD;
				_units[2]._readStatus = Status.NA;
				_units[2]._readResult = CardType.NA;
			}
			else
			{
				_units[2]._cameraStatus = Status.OK;
				_units[2]._readStatus = Status.NA;
				_units[2]._readResult = CardType.NA;
			}
		}

		private void UpdateText()
		{
			MechaManager.QrReader.Update();
			for (int i = 0; i < 3; i++)
			{
				if (Status.OK == _units[i]._cameraStatus || Status.Warn == _units[i]._cameraStatus)
				{
					_textCameraStatus[i].text = ((_units[i]._cameraStatus == Status.OK) ? ConstParameter.TestString_Good : ConstParameter.TestString_Warn);
					switch (_units[i]._index)
					{
					case 0:
						_textCameraIDs[i].text = TestmodeCameraID.LeftCamera.GetName();
						break;
					case 1:
						_textCameraIDs[i].text = TestmodeCameraID.RightCamera.GetName();
						break;
					case 2:
						_textCameraIDs[i].text = TestmodeCameraID.PhotoCamera.GetName();
						break;
					default:
						_textCameraIDs[i].text = "UNKNOWN";
						break;
					}
				}
				else if (Status.WarnBad == _units[i]._cameraStatus)
				{
					_textCameraStatus[i].text = ConstParameter.TestString_Warn;
					_textCameraIDs[i].text = "UNKNOWN";
				}
				else
				{
					_textCameraStatus[i].text = ConstParameter.TestString_Bad;
					_textCameraIDs[i].text = "UNKNOWN";
				}
				switch (_units[i]._readStatus)
				{
				case Status.NA:
					_textStatus[i].text = "";
					break;
				case Status.None:
					_textStatus[i].text = "-----";
					break;
				case Status.BAD:
					_textStatus[i].text = ConstParameter.TestString_Bad;
					break;
				default:
					_textStatus[i].text = ConstParameter.TestString_Good;
					break;
				}
				switch (_units[i]._readResult)
				{
				case CardType.NA:
					_textCards[i].text = "";
					break;
				case CardType.Unknown:
					_textCards[i].text = "UNKNOWN";
					break;
				case CardType.Normal:
					_textCards[i].text = "maimaiDX";
					break;
				default:
					_textCards[i].text = "-----";
					break;
				}
			}
		}

		protected override string GetLabelString(int index)
		{
			return ((TestmodeCameraID)Enum.Parse(typeof(TestmodeCameraID), GetLabelName(index))).GetName();
		}

		protected override string GetTitleString(int index)
		{
			return ((TestmodeCameraID)Enum.Parse(typeof(TestmodeCameraID), GetTitleName(index))).GetName();
		}
	}
}
