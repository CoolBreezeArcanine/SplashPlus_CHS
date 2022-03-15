using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using MAI2.Util;
using UnityEngine;

namespace Manager
{
	public class SlideManager : Singleton<SlideManager>
	{
		public class ControlPoint
		{
			public int Index;

			public Vector3 Anchore;

			public Vector3 Handle1;

			public Vector3 Handle2;

			public void RotateZ(float angle)
			{
				Anchore = new Vector3(Anchore.x - 540f, Anchore.y - 540f, Anchore.z);
				Handle1 = new Vector3(Handle1.x - 540f, Handle1.y - 540f, Handle1.z);
				Handle2 = new Vector3(Handle2.x - 540f, Handle2.y - 540f, Handle2.z);
				Anchore = Quaternion.Euler(0f, 0f, angle) * Anchore;
				Handle1 = Quaternion.Euler(0f, 0f, angle) * Handle1;
				Handle2 = Quaternion.Euler(0f, 0f, angle) * Handle2;
				Anchore = new Vector3(Anchore.x + 540f, Anchore.y + 540f, Anchore.z);
				Handle1 = new Vector3(Handle1.x + 540f, Handle1.y + 540f, Handle1.z);
				Handle2 = new Vector3(Handle2.x + 540f, Handle2.y + 540f, Handle2.z);
			}

			public void FlipX()
			{
				Anchore = new Vector3(1080f - Anchore.x, Anchore.y, Anchore.z);
				Handle1 = new Vector3(1080f - Handle1.x, Handle1.y, Handle1.z);
				Handle2 = new Vector3(1080f - Handle2.x, Handle2.y, Handle2.z);
			}
		}

		private struct SlidePath
		{
			public List<ControlPoint> Points;
		}

		public class HitArea
		{
			public double PushDistance;

			public double ReleaseDistance;

			public List<InputManager.TouchPanelArea> HitPoints = new List<InputManager.TouchPanelArea>();
		}

		private struct DataParam
		{
			public readonly List<string> DataPath;

			public readonly bool XFlip;

			public readonly bool Lerp;

			public DataParam(List<string> path, bool flip, bool lerp)
			{
				DataPath = path;
				XFlip = flip;
				Lerp = lerp;
			}
		}

		private enum Touch2Area
		{
			A,
			B,
			C,
			End
		}

		private const float CanvasSize = 1080f;

		public const float DivSize = 47.112f;

		private Thread _thread;

		private const float tergetDiffAngle = 10f;

		private const int PathDivideNum = 32768;

		private static readonly char[] RemoveChars = new char[3] { '\r', '\n', '\t' };

		private static readonly Vector3[] TouchHitArea = new Vector3[35];

		private const float A1A2_1_ACenter_To_AOut = 113.344421f;

		private const float A1A2_2_A1Out_To_A2In = 16.9288311f;

		private const float A1A2_3_AIn_To_AOut = 226.688843f;

		private const float A1A3_1_ACenter_To_AOut = 130.19072f;

		private const float A1A3_2_A1Out_To_AB2In = 129.126587f;

		private const float A1A3_3_AB2In_To_ABOut = 159.6083f;

		private const float A1A4_1_ACenter_To_AOut = 159.0022f;

		private const float A1A4_2_A1Out_To_B2In = 130.999969f;

		private const float A1A4_3_B2In_To_B2Out = 139.280029f;

		private const float A1A4_4_B2Out_To_B3In = 28.28998f;

		private const float A1A5_1_ACenter_To_AOut = 156.421249f;

		private const float A1A5_2_A1Out_To_B1In = 43.27424f;

		private const float A1A5_3_B1In_To_B1Out = 128.991776f;

		private const float A1A5_4_B2Out_To_CIn = 42.19922f;

		private const float A1A5_5_CIn_To_COut = 218.6303f;

		private const float A1A5_6_B1In_To_B1Center = 64.49589f;

		private const float A1B2_1_ACenter_To_AOut = 159.367233f;

		private const float A1B2_2_A1Out_To_B2In = 131.087326f;

		private const float A1B2_3_B2In_To_B2Center = 69.3482f;

		private const float B1B2_1_BCenter_To_B1Out = 75.9213638f;

		private const float B1B2_2_B1Out_To_B2In = 16.4650574f;

		private const float B1B2_3_BIn_To_Out = 151.842728f;

		private readonly InputManager.TouchPanelArea[] TouchAreaFlip = new InputManager.TouchPanelArea[35]
		{
			InputManager.TouchPanelArea.A1,
			InputManager.TouchPanelArea.A8,
			InputManager.TouchPanelArea.A7,
			InputManager.TouchPanelArea.A6,
			InputManager.TouchPanelArea.A5,
			InputManager.TouchPanelArea.A4,
			InputManager.TouchPanelArea.A3,
			InputManager.TouchPanelArea.A2,
			InputManager.TouchPanelArea.B1,
			InputManager.TouchPanelArea.B8,
			InputManager.TouchPanelArea.B7,
			InputManager.TouchPanelArea.B6,
			InputManager.TouchPanelArea.B5,
			InputManager.TouchPanelArea.B4,
			InputManager.TouchPanelArea.B3,
			InputManager.TouchPanelArea.B2,
			InputManager.TouchPanelArea.C2,
			InputManager.TouchPanelArea.C1,
			InputManager.TouchPanelArea.D2,
			InputManager.TouchPanelArea.D1,
			InputManager.TouchPanelArea.D8,
			InputManager.TouchPanelArea.D7,
			InputManager.TouchPanelArea.D6,
			InputManager.TouchPanelArea.D5,
			InputManager.TouchPanelArea.D4,
			InputManager.TouchPanelArea.D3,
			InputManager.TouchPanelArea.E2,
			InputManager.TouchPanelArea.E1,
			InputManager.TouchPanelArea.E8,
			InputManager.TouchPanelArea.E7,
			InputManager.TouchPanelArea.E6,
			InputManager.TouchPanelArea.E5,
			InputManager.TouchPanelArea.E4,
			InputManager.TouchPanelArea.E3,
			InputManager.TouchPanelArea.Blank
		};

		private readonly List<string> _blankDataList = new List<string> { "", "", "", "", "", "", "", "" };

		private readonly List<string> _lineDataList = new List<string> { "", "", "Straight_3.svg", "Straight_4.svg", "Straight_5.svg", "Straight_6.svg", "Straight_7.svg", "" };

		private readonly List<List<HitArea>> _lineHitAreaList = new List<List<HitArea>>
		{
			new List<HitArea>(),
			new List<HitArea>(),
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 130.19071960449219,
					ReleaseDistance = 129.1265869140625
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea>
					{
						InputManager.TouchPanelArea.A2,
						InputManager.TouchPanelArea.B2
					},
					PushDistance = 159.60830688476563,
					ReleaseDistance = 129.1265869140625
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A3 },
					PushDistance = 130.19071960449219,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 159.002197265625,
					ReleaseDistance = 130.99996948242188
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B2 },
					PushDistance = 139.280029296875,
					ReleaseDistance = 28.289979934692383
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B3 },
					PushDistance = 139.280029296875,
					ReleaseDistance = 130.99996948242188
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A4 },
					PushDistance = 159.002197265625,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B5 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A5 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 159.002197265625,
					ReleaseDistance = 130.99996948242188
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B8 },
					PushDistance = 139.280029296875,
					ReleaseDistance = 28.289979934692383
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B7 },
					PushDistance = 139.280029296875,
					ReleaseDistance = 130.99996948242188
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A6 },
					PushDistance = 159.002197265625,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 130.19071960449219,
					ReleaseDistance = 129.1265869140625
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea>
					{
						InputManager.TouchPanelArea.A8,
						InputManager.TouchPanelArea.B8
					},
					PushDistance = 159.60830688476563,
					ReleaseDistance = 129.1265869140625
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A7 },
					PushDistance = 130.19071960449219,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>()
		};

		private readonly List<string> _circleDataList = new List<string> { "Circle_1.svg", "Circle_2.svg", "Circle_3.svg", "Circle_4.svg", "Circle_5.svg", "Circle_6.svg", "Circle_7.svg", "Circle_8.svg" };

		private readonly List<List<HitArea>> _circleHitAreaListL = new List<List<HitArea>>
		{
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A8 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A7 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A6 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A5 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A4 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A3 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A2 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A8 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A7 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A6 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A5 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A4 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A3 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A2 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A8 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A7 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A6 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A5 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A4 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A3 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A8 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A7 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A6 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A5 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A4 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A8 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A7 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A6 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A5 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A8 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A7 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A6 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A8 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A7 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A8 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 0.0
				}
			}
		};

		private readonly List<string> _uDataList = new List<string> { "U_Curve_1.svg", "U_Curve_2.svg", "U_Curve_3.svg", "U_Curve_4.svg", "U_Curve_5.svg", "U_Curve_6.svg", "U_Curve_7.svg", "U_Curve_8.svg" };

		private readonly List<List<HitArea>> _uHitAreaListL = new List<List<HitArea>>
		{
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B8 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B7 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B6 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B5 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B4 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B3 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B2 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B8 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B7 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B6 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B5 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B4 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B3 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A2 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B8 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B7 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B6 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B5 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B4 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A3 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B8 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B7 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B6 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B5 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A4 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B8 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B7 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B6 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A5 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B8 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B7 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B6 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B5 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B4 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B3 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B2 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B8 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B7 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A6 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B8 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B7 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B6 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B5 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B4 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B3 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B2 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B8 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A7 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B8 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B7 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B6 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B5 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B4 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B3 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B2 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A8 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 0.0
				}
			}
		};

		private readonly List<string> _thunderDataList = new List<string> { "", "", "", "", "Thunder_5.svg", "", "", "" };

		private readonly List<List<HitArea>> _ThunderHitAreaListL = new List<List<HitArea>>
		{
			new List<HitArea>(),
			new List<HitArea>(),
			new List<HitArea>(),
			new List<HitArea>(),
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B8 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B7 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B3 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B4 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A5 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>(),
			new List<HitArea>(),
			new List<HitArea>()
		};

		private readonly List<string> _vDataList = new List<string> { "", "V_2.svg", "V_3.svg", "V_4.svg", "", "V_6.svg", "V_7.svg", "V_8.svg" };

		private readonly List<List<HitArea>> _vHitAreaList = new List<List<HitArea>>
		{
			new List<HitArea>(),
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B2 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A2 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B3 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A3 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B4 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A4 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>(),
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B6 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A6 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B7 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A7 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B8 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A8 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 0.0
				}
			}
		};

		private readonly List<string> _cupDataList = new List<string> { "CUP_Curve_1.svg", "CUP_Curve_2.svg", "CUP_Curve_3.svg", "CUP_Curve_4.svg", "CUP_Curve_5.svg", "CUP_Curve_6.svg", "CUP_Curve_7.svg", "CUP_Curve_8.svg" };

		private readonly List<List<HitArea>> _cupHitAreaListL = new List<List<HitArea>>
		{
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B4 },
					PushDistance = 133.84408569335938,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A3 },
					PushDistance = 272.711669921875,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A2 },
					PushDistance = 226.6888427734375,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B4 },
					PushDistance = 133.84408569335938,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A3 },
					PushDistance = 272.711669921875,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A2 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B4 },
					PushDistance = 133.84408569335938,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A3 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B4 },
					PushDistance = 133.84408569335938,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A3 },
					PushDistance = 272.711669921875,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A2 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 133.84408569335938,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B4 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A4 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B4 },
					PushDistance = 133.84408569335938,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A3 },
					PushDistance = 272.711669921875,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A2 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 133.84408569335938,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B5 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A5 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B4 },
					PushDistance = 133.84408569335938,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A3 },
					PushDistance = 272.711669921875,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A2 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea>
					{
						InputManager.TouchPanelArea.B8,
						InputManager.TouchPanelArea.C1
					},
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea>
					{
						InputManager.TouchPanelArea.B7,
						InputManager.TouchPanelArea.B6
					},
					PushDistance = 145.26956176757813,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A6 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B4 },
					PushDistance = 133.84408569335938,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A3 },
					PushDistance = 272.711669921875,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A2 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B8 },
					PushDistance = 151.84272766113281,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A7 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B4 },
					PushDistance = 133.84408569335938,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A3 },
					PushDistance = 272.711669921875,
					ReleaseDistance = 16.928831100463867
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A2 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea>
					{
						InputManager.TouchPanelArea.A1,
						InputManager.TouchPanelArea.B1
					},
					PushDistance = 159.36723327636719,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A8 },
					PushDistance = 113.34442138671875,
					ReleaseDistance = 0.0
				}
			}
		};

		private readonly List<string> _lDataList = new List<string> { "", "L_2.svg", "L_3.svg", "L_4.svg", "L_5.svg", "", "", "" };

		private readonly List<List<HitArea>> _lHitAreaListL = new List<List<HitArea>>
		{
			new List<HitArea>(),
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 130.19071960449219,
					ReleaseDistance = 129.1265869140625
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea>
					{
						InputManager.TouchPanelArea.A8,
						InputManager.TouchPanelArea.B8
					},
					PushDistance = 159.60830688476563,
					ReleaseDistance = 129.1265869140625
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A7 },
					PushDistance = 289.55795288085938,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B8 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A2 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 130.19071960449219,
					ReleaseDistance = 129.1265869140625
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea>
					{
						InputManager.TouchPanelArea.A8,
						InputManager.TouchPanelArea.B8
					},
					PushDistance = 159.60830688476563,
					ReleaseDistance = 129.1265869140625
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A7 },
					PushDistance = 286.61196899414063,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B7 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B3 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A3 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 130.19071960449219,
					ReleaseDistance = 129.1265869140625
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea>
					{
						InputManager.TouchPanelArea.A8,
						InputManager.TouchPanelArea.B8
					},
					PushDistance = 159.60830688476563,
					ReleaseDistance = 129.1265869140625
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A7 },
					PushDistance = 289.55795288085938,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B6 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 16.465057373046875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B5 },
					PushDistance = 145.26956176757813,
					ReleaseDistance = 131.08732604980469
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A4 },
					PushDistance = 159.36723327636719,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 130.19071960449219,
					ReleaseDistance = 129.1265869140625
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea>
					{
						InputManager.TouchPanelArea.A8,
						InputManager.TouchPanelArea.B8
					},
					PushDistance = 159.60830688476563,
					ReleaseDistance = 129.1265869140625
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A7 },
					PushDistance = 260.38143920898438,
					ReleaseDistance = 129.1265869140625
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea>
					{
						InputManager.TouchPanelArea.A6,
						InputManager.TouchPanelArea.B6
					},
					PushDistance = 159.60830688476563,
					ReleaseDistance = 129.1265869140625
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A5 },
					PushDistance = 130.19071960449219,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>(),
			new List<HitArea>(),
			new List<HitArea>()
		};

		private readonly List<string> _fanDataList = new List<string> { "", "", "", "Straight_4.svg", "Straight_5.svg", "Straight_6.svg", "", "" };

		private readonly List<List<HitArea>> _fanHitAreaList = new List<List<HitArea>>
		{
			new List<HitArea>(),
			new List<HitArea>(),
			new List<HitArea>(),
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 159.002197265625,
					ReleaseDistance = 130.99996948242188
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B2 },
					PushDistance = 139.280029296875,
					ReleaseDistance = 28.289979934692383
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B3 },
					PushDistance = 139.280029296875,
					ReleaseDistance = 130.99996948242188
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea>
					{
						InputManager.TouchPanelArea.A4,
						InputManager.TouchPanelArea.D5
					},
					PushDistance = 159.002197265625,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B1 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.C1 },
					PushDistance = 218.63029479980469,
					ReleaseDistance = 42.19921875
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B5 },
					PushDistance = 128.99177551269531,
					ReleaseDistance = 43.274238586425781
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A5 },
					PushDistance = 156.42124938964844,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>
			{
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.A1 },
					PushDistance = 159.002197265625,
					ReleaseDistance = 130.99996948242188
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B8 },
					PushDistance = 139.280029296875,
					ReleaseDistance = 28.289979934692383
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea> { InputManager.TouchPanelArea.B7 },
					PushDistance = 139.280029296875,
					ReleaseDistance = 130.99996948242188
				},
				new HitArea
				{
					HitPoints = new List<InputManager.TouchPanelArea>
					{
						InputManager.TouchPanelArea.A6,
						InputManager.TouchPanelArea.D6
					},
					PushDistance = 159.002197265625,
					ReleaseDistance = 0.0
				}
			},
			new List<HitArea>(),
			new List<HitArea>()
		};

		private readonly List<DataParam> _slideDataList = new List<DataParam>();

		private readonly List<List<List<Vector4>>> _slidePathList = new List<List<List<Vector4>>>();

		private readonly List<List<List<HitArea>>> _hitAreaList = new List<List<List<HitArea>>>();

		public static int MaxSlideLane { get; private set; }

		public static Vector3 GetTouchAreaPositon(InputManager.TouchPanelArea touch)
		{
			return TouchHitArea[(int)touch];
		}

		private void AddHitArea(List<List<HitArea>> hitArea, bool flip = false)
		{
			List<List<HitArea>> list = new List<List<HitArea>>();
			for (int i = 0; i < 8; i++)
			{
				list.Add(new List<HitArea>());
			}
			for (int j = 0; j < 8; j++)
			{
				int index = j;
				for (int k = 0; k < hitArea[j].Count; k++)
				{
					HitArea hitArea2 = new HitArea();
					foreach (InputManager.TouchPanelArea hitPoint in hitArea[j][k].HitPoints)
					{
						hitArea2.HitPoints.Add(flip ? TouchAreaFlip[(int)hitPoint] : hitPoint);
					}
					hitArea2.PushDistance = hitArea[j][k].PushDistance;
					hitArea2.ReleaseDistance = hitArea[j][k].ReleaseDistance;
					list[index].Add(hitArea2);
				}
			}
			_hitAreaList.Add(list);
		}

		public SlideManager()
		{
			_slideDataList.Clear();
			_slideDataList.Add(new DataParam(_blankDataList, flip: false, lerp: false));
			_slideDataList.Add(new DataParam(_lineDataList, flip: false, lerp: false));
			_slideDataList.Add(new DataParam(_circleDataList, flip: false, lerp: true));
			_slideDataList.Add(new DataParam(_circleDataList, flip: true, lerp: true));
			_slideDataList.Add(new DataParam(_uDataList, flip: false, lerp: true));
			_slideDataList.Add(new DataParam(_uDataList, flip: true, lerp: true));
			_slideDataList.Add(new DataParam(_thunderDataList, flip: false, lerp: false));
			_slideDataList.Add(new DataParam(_thunderDataList, flip: true, lerp: false));
			_slideDataList.Add(new DataParam(_vDataList, flip: false, lerp: false));
			_slideDataList.Add(new DataParam(_cupDataList, flip: false, lerp: true));
			_slideDataList.Add(new DataParam(_cupDataList, flip: true, lerp: true));
			_slideDataList.Add(new DataParam(_lDataList, flip: false, lerp: false));
			_slideDataList.Add(new DataParam(_lDataList, flip: true, lerp: false));
			_slideDataList.Add(new DataParam(_fanDataList, flip: false, lerp: false));
			_hitAreaList.Clear();
			_hitAreaList.Add(new List<List<HitArea>>());
			AddHitArea(_lineHitAreaList);
			AddHitArea(_circleHitAreaListL);
			AddHitArea(_circleHitAreaListL, flip: true);
			AddHitArea(_uHitAreaListL);
			AddHitArea(_uHitAreaListL, flip: true);
			AddHitArea(_ThunderHitAreaListL);
			AddHitArea(_ThunderHitAreaListL, flip: true);
			AddHitArea(_vHitAreaList);
			AddHitArea(_cupHitAreaListL);
			AddHitArea(_cupHitAreaListL, flip: true);
			AddHitArea(_lHitAreaListL);
			AddHitArea(_lHitAreaListL, flip: true);
			AddHitArea(_fanHitAreaList);
		}

		public bool CheckSlideEnable(SlideType slideType, int start, int end)
		{
			int slideTypeLength = GetSlideTypeLength(slideType, start, end);
			return _slidePathList[(int)slideType][slideTypeLength].Count != 0;
		}

		public int GetSlideTypeLength(SlideType slideType, int start, int end)
		{
			int num = ((end < start) ? (8 + end - start) : (end - start));
			if (slideType == SlideType.Slide_Bend_R || slideType == SlideType.Slide_Circle_R || slideType == SlideType.Slide_Curve_R || slideType == SlideType.Slide_Skip_R || slideType == SlideType.Slide_Thunder_R)
			{
				num = (8 - num) % 8;
			}
			return num;
		}

		public float GetSlideLength(SlideType slideType, int start, int end)
		{
			int slideTypeLength = GetSlideTypeLength(slideType, start, end);
			_ = 8;
			_ = 13;
			return _slidePathList[(int)slideType][slideTypeLength][_slidePathList[(int)slideType][slideTypeLength].Count - 1].z;
		}

		public List<Vector4> GetSlidePath(SlideType slideType, int start, int end)
		{
			int slideTypeLength = GetSlideTypeLength(slideType, start, end);
			_ = 8;
			_ = 13;
			return _slidePathList[(int)slideType][slideTypeLength];
		}

		public List<HitArea> GetSlideHitArea(SlideType slideType, int start, int end)
		{
			int slideTypeLength = GetSlideTypeLength(slideType, start, end);
			_ = 8;
			_ = 13;
			return _hitAreaList[(int)slideType][slideTypeLength];
		}

		public void Initialize(string dir)
		{
			for (int i = 0; i < TouchHitArea.Length; i++)
			{
				TouchHitArea[i] = InputManager.MouseTouchPanel[0].GetPosition((InputManager.TouchPanelArea)i);
			}
			_thread = new Thread(ThreadRun);
			_thread.Start(dir);
		}

		public bool IsActiveThread()
		{
			return _thread.IsAlive;
		}

		private void ThreadRun(object dir)
		{
			SlidePath slidePath = default(SlidePath);
			slidePath.Points = new List<ControlPoint>();
			SlidePath path = slidePath;
			_slidePathList.Clear();
			int num = 0;
			int num2 = 0;
			MaxSlideLane = 0;
			foreach (DataParam slideData in _slideDataList)
			{
				_slidePathList.Add(new List<List<Vector4>>());
				num2 = 0;
				foreach (string item in slideData.DataPath)
				{
					List<Vector4> pathRef = new List<Vector4>();
					path.Points.Clear();
					if ("" == item)
					{
						_slidePathList[num].Add(pathRef);
					}
					else if (slideData.XFlip)
					{
						pathRef = new List<Vector4>(_slidePathList[num - 1][num2]);
						FlipPoints(ref pathRef);
						_slidePathList[num].Add(pathRef);
						SetHitDistance(_hitAreaList[num][num2]);
					}
					else if (loadXML(path, string.Concat(dir, item)))
					{
						GetPoints(path, 32768, ref pathRef, slideData.Lerp);
						_slidePathList[num].Add(pathRef);
						SetHitDistance(_hitAreaList[num][num2]);
						if (MaxSlideLane < pathRef.Count)
						{
							MaxSlideLane = pathRef.Count;
						}
					}
					num2++;
				}
				num++;
			}
		}

		private bool loadXML(SlidePath path, string dataPath)
		{
			if (!File.Exists(dataPath))
			{
				return false;
			}
			XmlDocument xmlDocument = new XmlDocument
			{
				XmlResolver = null
			};
			try
			{
				xmlDocument.Load(dataPath);
				Regex regex = new Regex("([MCLSclsvVhH(points)])((-?\\d+(\\.\\d+)?[,-]?\\d+(\\.\\d+)?),?)+", RegexOptions.Multiline);
				Regex regex2 = new Regex("(-?\\d+(\\.\\d+)?),?(-?\\d+(\\.\\d+)?)", RegexOptions.Multiline);
				Regex regex3 = new Regex("(-?\\d+(\\.\\d+)?),?(-?\\d+(\\.\\d+)?)", RegexOptions.Multiline);
				ControlPoint controlPoint = new ControlPoint();
				foreach (XmlElement item4 in xmlDocument.GetElementsByTagName("line"))
				{
					string attribute = item4.GetAttribute("x1");
					Vector2 vector = new Vector2(y: float.Parse(item4.GetAttribute("y1")), x: float.Parse(attribute));
					ControlPoint item = new ControlPoint
					{
						Handle1 = new Vector3(vector.x, vector.y, 0f),
						Handle2 = new Vector3(vector.x, vector.y, 0f),
						Anchore = new Vector3(vector.x, vector.y, 0f)
					};
					path.Points.Add(item);
					attribute = item4.GetAttribute("x2");
					string attribute2 = item4.GetAttribute("y2");
					vector = new Vector2(float.Parse(attribute), float.Parse(attribute2));
					ControlPoint item2 = new ControlPoint
					{
						Handle1 = new Vector3(vector.x, vector.y, 0f),
						Handle2 = new Vector3(vector.x, vector.y, 0f),
						Anchore = new Vector3(vector.x, vector.y, 0f)
					};
					path.Points.Add(item2);
				}
				foreach (XmlElement item5 in xmlDocument.GetElementsByTagName("polyline"))
				{
					string attribute3 = item5.GetAttribute("points");
					foreach (Match item6 in regex3.Matches(attribute3))
					{
						Vector2 vector2 = new Vector2(float.Parse(item6.Groups[1].Value), float.Parse(item6.Groups[3].Value));
						ControlPoint item3 = new ControlPoint
						{
							Handle1 = new Vector3(vector2.x, vector2.y, 0f),
							Handle2 = new Vector3(vector2.x, vector2.y, 0f),
							Anchore = new Vector3(vector2.x, vector2.y, 0f)
						};
						path.Points.Add(item3);
					}
				}
				foreach (XmlElement item7 in xmlDocument.GetElementsByTagName("path"))
				{
					string text = item7.GetAttribute("d");
					List<Vector2> list = new List<Vector2>();
					char[] removeChars = RemoveChars;
					foreach (char c in removeChars)
					{
						text = text.Replace(c.ToString(), "");
					}
					foreach (Match item8 in regex.Matches(text))
					{
						ControlPoint controlPoint2 = new ControlPoint();
						list.Clear();
						foreach (Match item9 in regex2.Matches(item8.Value))
						{
							if (item8.Groups[1].Value == "h" || item8.Groups[1].Value == "H")
							{
								list.Add(new Vector2(float.Parse(item9.Value), 0f));
							}
							else if (item8.Groups[1].Value == "V" || item8.Groups[1].Value == "V")
							{
								list.Add(new Vector2(0f, float.Parse(item9.Value)));
							}
							else
							{
								list.Add(new Vector2(float.Parse(item9.Groups[1].Value), float.Parse(item9.Groups[3].Value)));
							}
						}
						if (item8.Groups[1].Value == "C")
						{
							controlPoint2.Handle1 = new Vector3(list[0].x, list[0].y, 0f);
							controlPoint2.Handle2 = new Vector3(list[1].x, list[1].y, 0f);
							controlPoint2.Anchore = new Vector3(list[2].x, list[2].y, 0f);
						}
						else if (item8.Groups[1].Value == "c")
						{
							controlPoint2.Handle1 = new Vector3(controlPoint.Anchore.x + list[0].x, controlPoint.Anchore.y + list[0].y, 0f);
							controlPoint2.Handle2 = new Vector3(controlPoint.Anchore.x + list[1].x, controlPoint.Anchore.y + list[1].y, 0f);
							controlPoint2.Anchore = new Vector3(controlPoint.Anchore.x + list[2].x, controlPoint.Anchore.y + list[2].y, 0f);
						}
						else if (item8.Groups[1].Value == "s")
						{
							if (path.Points.Count > 0)
							{
								Vector3 vector3 = controlPoint.Anchore - (controlPoint.Handle2 - controlPoint.Anchore);
								controlPoint2.Handle1 = new Vector3(vector3.x, vector3.y, 0f);
								controlPoint2.Handle2 = new Vector3(controlPoint.Anchore.x + list[0].x, controlPoint.Anchore.y + list[0].y, 0f);
								controlPoint2.Anchore = new Vector3(controlPoint.Anchore.x + list[1].x, controlPoint.Anchore.y + list[1].y, 0f);
							}
							else
							{
								controlPoint2.Handle1 = new Vector3(controlPoint.Anchore.x + list[1].x, controlPoint.Anchore.y + list[1].y, 0f);
								controlPoint2.Handle2 = new Vector3(controlPoint.Anchore.x + list[0].x, controlPoint.Anchore.y + list[0].y, 0f);
								controlPoint2.Anchore = new Vector3(controlPoint.Anchore.x + list[1].x, controlPoint.Anchore.y + list[1].y, 0f);
							}
						}
						else if (item8.Groups[1].Value == "S")
						{
							if (path.Points.Count > 0)
							{
								Vector3 vector4 = controlPoint.Anchore - (controlPoint.Handle2 - controlPoint.Anchore);
								controlPoint2.Handle1 = new Vector3(vector4.x, vector4.y, 0f);
								controlPoint2.Handle2 = new Vector3(list[0].x, list[0].y, 0f);
								controlPoint2.Anchore = new Vector3(list[1].x, list[1].y, 0f);
							}
							else
							{
								controlPoint2.Handle1 = new Vector3(list[1].x, list[1].y, 0f);
								controlPoint2.Handle2 = new Vector3(list[0].x, list[0].y, 0f);
								controlPoint2.Anchore = new Vector3(list[1].x, list[1].y, 0f);
							}
						}
						else if (item8.Groups[1].Value == "l")
						{
							controlPoint2.Handle1 = new Vector3(controlPoint.Anchore.x + list[0].x, controlPoint.Anchore.y + list[0].y, 0f);
							controlPoint2.Handle2 = new Vector3(controlPoint.Anchore.x + list[0].x, controlPoint.Anchore.y + list[0].y, 0f);
							controlPoint2.Anchore = new Vector3(controlPoint.Anchore.x + list[0].x, controlPoint.Anchore.y + list[0].y, 0f);
						}
						else if (item8.Groups[1].Value == "L")
						{
							controlPoint2.Handle1 = new Vector3(list[0].x, list[0].y, 0f);
							controlPoint2.Handle2 = new Vector3(list[0].x, list[0].y, 0f);
							controlPoint2.Anchore = new Vector3(list[0].x, list[0].y, 0f);
						}
						else if (item8.Groups[1].Value == "v")
						{
							controlPoint2.Handle1 = new Vector3(controlPoint.Anchore.x + list[0].x, controlPoint.Anchore.y, 0f);
							controlPoint2.Handle2 = new Vector3(controlPoint.Anchore.x + list[0].x, controlPoint.Anchore.y, 0f);
							controlPoint2.Anchore = new Vector3(controlPoint.Anchore.x + list[0].x, controlPoint.Anchore.y, 0f);
						}
						else if (item8.Groups[1].Value == "V")
						{
							controlPoint2.Handle1 = new Vector3(controlPoint.Anchore.x, list[0].y, 0f);
							controlPoint2.Handle2 = new Vector3(controlPoint.Anchore.x, list[0].y, 0f);
							controlPoint2.Anchore = new Vector3(controlPoint.Anchore.x, list[0].y, 0f);
						}
						else if (item8.Groups[1].Value == "h")
						{
							controlPoint2.Handle1 = new Vector3(controlPoint.Anchore.x, controlPoint.Anchore.y + list[0].y, 0f);
							controlPoint2.Handle2 = new Vector3(controlPoint.Anchore.x, controlPoint.Anchore.y + list[0].y, 0f);
							controlPoint2.Anchore = new Vector3(controlPoint.Anchore.x, controlPoint.Anchore.y + list[0].y, 0f);
						}
						else if (item8.Groups[1].Value == "H")
						{
							controlPoint2.Handle1 = new Vector3(list[0].x, controlPoint.Anchore.y, 0f);
							controlPoint2.Handle2 = new Vector3(list[0].x, controlPoint.Anchore.y, 0f);
							controlPoint2.Anchore = new Vector3(list[0].x, controlPoint.Anchore.y, 0f);
						}
						else
						{
							controlPoint2.Anchore = new Vector3(list[0].x, list[0].y, 0f);
							controlPoint2.Handle1 = new Vector3(list[0].x, list[0].y, 0f);
							controlPoint2.Handle2 = new Vector3(list[0].x, list[0].y, 0f);
						}
						path.Points.Add(controlPoint2);
						controlPoint = controlPoint2;
					}
				}
			}
			catch
			{
				return false;
			}
			return true;
		}

		private static void SetHitDistance(List<HitArea> hitList)
		{
			HitArea hitArea = hitList[hitList.Count - 1];
			hitArea.ReleaseDistance = 0.0;
			for (int i = 0; i < hitList.Count; i++)
			{
				hitArea.ReleaseDistance += hitList[i].PushDistance;
				if (i != hitList.Count - 1)
				{
					hitArea.ReleaseDistance += hitList[i].ReleaseDistance;
				}
			}
		}

		private static void FlipPoints(ref List<Vector4> pathRef)
		{
			Vector3 vector = new Vector3(1f, 0f, 0f);
			Vector3 vector2 = Quaternion.Euler(0f, 0f, -22.5f) * vector;
			for (int i = 0; i < pathRef.Count; i++)
			{
				Vector2 vector3 = Vector2.Reflect(new Vector2(pathRef[i].x, pathRef[i].y), new Vector2(vector2.x, vector2.y));
				pathRef[i] = new Vector4(vector3.x, vector3.y, pathRef[i].z, (pathRef[i].w + -67.5f) * -1f + 67.5f);
			}
		}

		private static void GetPoints(SlidePath path, int segments, ref List<Vector4> pathRef, bool lerp)
		{
			segments = Mathf.Max(segments, 1);
			List<Vector3> list = new List<Vector3>();
			float num = segments;
			Vector3 vector = default(Vector3);
			for (int i = 1; i < path.Points.Count; i++)
			{
				Vector3 anchore = path.Points[i - 1].Anchore;
				Vector3 handle = path.Points[i].Handle1;
				Vector3 handle2 = path.Points[i].Handle2;
				Vector3 anchore2 = path.Points[i].Anchore;
				anchore.y = 1080f - anchore.y;
				handle.y = 1080f - handle.y;
				handle2.y = 1080f - handle2.y;
				anchore2.y = 1080f - anchore2.y;
				for (int j = 0; j <= segments; j++)
				{
					list.Add(GetPoint(anchore, handle, handle2, anchore2, (float)j / num));
					float num2 = Vector2.Distance(list[list.Count - 1], vector);
					if (1 != i || j != 0)
					{
						list[list.Count - 1] = new Vector3(list[list.Count - 1].x, list[list.Count - 1].y, num2 + vector.z);
					}
					vector = list[list.Count - 1];
				}
			}
			float num3 = 0f;
			uint num4 = 1u;
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k].z >= num3 || list.Count - 1 == k)
				{
					pathRef.Add(list[k]);
					num3 = 47.112f * (float)num4;
					num4++;
					if (pathRef.Count > 1)
					{
						Vector4 vector2 = pathRef[pathRef.Count - 2];
						float num5 = vector2.y - list[k].y;
						float num6 = vector2.x - list[k].x;
						float num7 = (float)(Math.Atan2(num5, num6) * (180.0 / Math.PI));
						num7 = ((num7 < 0f) ? (360f + num7) : num7) % 360f;
						pathRef[pathRef.Count - 2] = new Vector4(vector2.x, vector2.y, vector2.z, num7);
					}
				}
			}
			pathRef[pathRef.Count - 1] = new Vector4(pathRef[pathRef.Count - 1].x, pathRef[pathRef.Count - 1].y, pathRef[pathRef.Count - 1].z, pathRef[pathRef.Count - 2].w);
			list.Clear();
			for (int l = 1; l < pathRef.Count - 1; l++)
			{
				float w = pathRef[l - 1].w;
				float w2 = pathRef[l].w;
				float w3 = pathRef[l + 1].w;
				if (lerp)
				{
					if (w > w2)
					{
						pathRef[l] = new Vector4(pathRef[l].x, pathRef[l].y, pathRef[l].z, Mathf.LerpAngle(Math.Abs(w), Math.Abs(w2), 0.5f));
					}
					else
					{
						pathRef[l] = new Vector4(pathRef[l].x, pathRef[l].y, pathRef[l].z, Mathf.LerpAngle(Math.Abs(w2), Math.Abs(w), 0.5f));
					}
				}
				else if (Math.Abs(Mathf.DeltaAngle(Math.Abs(w), Math.Abs(w3))) > 10f && Math.Abs(w2 - w3) > 0.01f)
				{
					pathRef[l] = new Vector4(pathRef[l].x, pathRef[l].y, pathRef[l].z, w);
				}
				else
				{
					pathRef[l] = new Vector4(pathRef[l].x, pathRef[l].y, pathRef[l].z, w2);
				}
			}
			for (int m = 0; m < pathRef.Count; m++)
			{
				pathRef[m] = new Vector4(pathRef[m].x - 540f, pathRef[m].y - 540f, pathRef[m].z, pathRef[m].w);
			}
		}

		private static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			Vector3 a = Vector3.Lerp(p0, p1, t);
			Vector3 vector = Vector3.Lerp(p1, p2, t);
			Vector3 b = Vector3.Lerp(p2, p3, t);
			Vector3 a2 = Vector3.Lerp(a, vector, t);
			Vector3 b2 = Vector3.Lerp(vector, b, t);
			return Vector3.Lerp(a2, b2, t);
		}

		private static Touch2Area ConvertTouchAreaToType(InputManager.TouchPanelArea area)
		{
			Touch2Area result = Touch2Area.End;
			if (area < InputManager.TouchPanelArea.B1)
			{
				result = Touch2Area.A;
			}
			else if (area < InputManager.TouchPanelArea.C1)
			{
				result = Touch2Area.B;
			}
			else if (area < InputManager.TouchPanelArea.D1)
			{
				result = Touch2Area.C;
			}
			return result;
		}
	}
}
