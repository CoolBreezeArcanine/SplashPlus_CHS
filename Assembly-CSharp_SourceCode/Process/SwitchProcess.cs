using Manager;
using UnityEngine;

namespace Process
{
	public class SwitchProcess : ProcessBase
	{
		public enum FadeType
		{
			Alpha,
			Fade
		}

		private readonly CanvasGroup[,] _canvasGroup;

		private readonly ProcessBase _fromProcess;

		private readonly ProcessBase _toProcess;

		private readonly FadeType _fadeType;

		private readonly float _fadeTime;

		private float _timer;

		private float _progress;

		private float _alpha;

		private bool _isFadeing;

		private bool _isAddProcess;

		public SwitchProcess(ProcessDataContainer dataContainer, ProcessBase from, ProcessBase to, MonitorBase[] monitorBase, float time, FadeType type)
			: base(dataContainer, ProcessType.SwitchProcess)
		{
			_fromProcess = from;
			_toProcess = to;
			_fadeTime = time / 100f;
			_fadeType = type;
			_canvasGroup = new CanvasGroup[2, 2];
			for (int i = 0; i < 2; i++)
			{
				_canvasGroup[i, 0] = monitorBase[i].transform.Find("Canvas/Main").GetComponent<CanvasGroup>();
				_canvasGroup[i, 1] = monitorBase[i].transform.Find("Canvas/Sub").GetComponent<CanvasGroup>();
			}
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			_isFadeing = true;
			if (_toProcess != null)
			{
				container.processManager.AddProcess(_toProcess, 20);
			}
		}

		public override void OnUpdate()
		{
		}

		private void ProcessingProcess()
		{
			if (!_isAddProcess)
			{
				if (_fromProcess != null)
				{
					container.processManager.ReleaseProcess(_fromProcess);
				}
				_isAddProcess = true;
			}
		}

		public override void OnLateUpdate()
		{
			if (!_isFadeing)
			{
				return;
			}
			_progress = _timer / (_fadeTime * 1000f);
			if ((double)_progress >= 1.0)
			{
				_timer = 0f;
				ProcessingProcess();
				_isFadeing = false;
				container.processManager.ReleaseProcess(this);
			}
			else
			{
				_alpha = Mathf.Lerp(1f, 0f, _progress);
				switch (_fadeType)
				{
				case FadeType.Alpha:
				{
					CanvasGroup[,] canvasGroup = _canvasGroup;
					int upperBound = canvasGroup.GetUpperBound(0);
					int upperBound2 = canvasGroup.GetUpperBound(1);
					for (int i = canvasGroup.GetLowerBound(0); i <= upperBound; i++)
					{
						for (int j = canvasGroup.GetLowerBound(1); j <= upperBound2; j++)
						{
							canvasGroup[i, j].alpha = _alpha;
						}
					}
					break;
				}
				}
			}
			_timer += GameManager.GetGameMSecAdd();
		}

		public override void OnRelease()
		{
		}

		public void StartSwitch()
		{
			_timer = 0f;
			_isFadeing = true;
		}
	}
}
