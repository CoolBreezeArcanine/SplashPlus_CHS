using DB;
using MAI2.Util;
using Mecha;
using UnityEngine;

namespace IO
{
	public class MechaManager : Singleton<MechaManager>
	{
		public class InitParam
		{
			public Bd15070_4IF.InitParam[] LedParam = new Bd15070_4IF.InitParam[2];

			public InitParam()
			{
				for (int i = 0; i < 2; i++)
				{
					LedParam[i] = new Bd15070_4IF.InitParam();
				}
			}
		}

		public static Jvs Jvs => Singleton<MechaManager>.Instance._jvs;

		public static QrCamera QrReader => Singleton<MechaManager>.Instance._qrReader;

		public static Bd15070_4IF[] LedIf => Singleton<MechaManager>.Instance._ledIf;

		public static CoinBlocker CoinBlocker => Singleton<MechaManager>.Instance._coinBlocker;

		public static bool IsInitialized { get; private set; }

		private Jvs _jvs { get; set; }

		private QrCamera _qrReader { get; set; }

		private Bd15070_4IF[] _ledIf { get; set; }

		private CoinBlocker _coinBlocker { get; set; }

		public static void Initialize(InitParam initParam)
		{
			Singleton<MechaManager>.Instance._initialize(initParam);
		}

		public static void Terminate()
		{
			Singleton<MechaManager>.Instance._terminate();
		}

		public static void Execute()
		{
			if (IsInitialized)
			{
				Singleton<MechaManager>.Instance._execute();
			}
		}

		public static void SetAllCuOff()
		{
			if (IsInitialized)
			{
				Singleton<MechaManager>.Instance._allCutOff();
			}
		}

		private void _initialize(InitParam initParam)
		{
			_jvs = new Jvs();
			_jvs.Initialize();
			_qrReader = new QrCamera();
			_ledIf = new Bd15070_4IF[2];
			for (int i = 0; i < _ledIf.Length; i++)
			{
				_ledIf[i] = new Bd15070_4IF(initParam.LedParam[i]);
			}
			_coinBlocker = new CoinBlocker();
			_coinBlocker.Initialize();
			IsInitialized = true;
		}

		private void _terminate()
		{
			if (_ledIf != null)
			{
				for (int i = 0; i < _ledIf.Length; i++)
				{
					_ledIf[i].Terminate();
					_ledIf[i] = null;
				}
			}
			_ledIf = null;
			_jvs?.Terminate();
			_jvs = null;
			_qrReader?.Terminate();
			_qrReader = null;
			_coinBlocker?.Terminate();
			_coinBlocker = null;
		}

		private void _execute()
		{
			_coinBlocker.Execute();
			_jvs.Execute();
			Bd15070_4IF[] ledIf = _ledIf;
			for (int i = 0; i < ledIf.Length; i++)
			{
				ledIf[i].PreExecute();
			}
		}

		private void _allCutOff()
		{
			if (Jvs != null)
			{
				for (int i = 0; i < 5; i++)
				{
					Jvs.SetOutput((JvsOutputID)i, on: false);
				}
				for (byte b = 0; b < 2; b = (byte)(b + 1))
				{
					Jvs.SetPwmOutput(b, Color.black);
				}
			}
			if (_ledIf != null)
			{
				for (int j = 0; j < _ledIf.Length; j++)
				{
					_ledIf[j].SetLedAllOff();
				}
			}
		}
	}
}
