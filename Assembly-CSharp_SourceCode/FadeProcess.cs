using Manager;
using Process;
using UnityEngine;

public class FadeProcess : ProcessBase
{
	protected enum FadeState
	{
		DoFadeOut,
		FadeOut,
		DoFadeIn,
		FadeIn
	}

	protected enum CanvasTarget
	{
		Main,
		Sub,
		End
	}

	public enum FadeType
	{
		Type1 = 1,
		Type2,
		Type3
	}

	private int _counter;

	protected GameObject[] fadeObject = new GameObject[2];

	protected GameObject[,] canvasObject = new GameObject[2, 2];

	protected Animator[,] animator = new Animator[2, 2];

	protected ProcessBase fromProcess;

	protected ProcessBase toProcess;

	protected FadeState state;

	protected FadeType fadeType = FadeType.Type1;

	protected bool isFadeing;

	protected bool isAddProcess;

	private int _restartPleaseWaitID = -1;

	public FadeProcess(ProcessDataContainer dataContainer, ProcessBase from)
		: base(dataContainer, ProcessType.FadeProcess)
	{
		fromProcess = from;
		toProcess = null;
	}

	public FadeProcess(ProcessDataContainer dataContainer, ProcessBase from, ProcessBase to)
		: base(dataContainer, ProcessType.FadeProcess)
	{
		fromProcess = from;
		toProcess = to;
		fadeType = FadeType.Type1;
	}

	public FadeProcess(ProcessDataContainer dataContainer, ProcessBase from, ProcessBase to, int monitorIndex = -1)
		: base(dataContainer, ProcessType.FadeProcess)
	{
		fromProcess = from;
		toProcess = to;
		fadeType = FadeType.Type1;
		_restartPleaseWaitID = monitorIndex;
	}

	public FadeProcess(ProcessDataContainer dataContainer, ProcessBase from, ProcessBase to, FadeType type)
		: base(dataContainer, ProcessType.FadeProcess)
	{
		fromProcess = from;
		toProcess = to;
		fadeType = type;
	}

	public override void OnAddProcess()
	{
	}

	public override void OnStart()
	{
		GameObject prefs = Resources.Load<GameObject>("Process/Fade/FadeProcess");
		fadeObject[0] = CreateInstanceAndSetParent(prefs, container.LeftMonitor);
		fadeObject[1] = CreateInstanceAndSetParent(prefs, container.RightMonitor);
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		gameObject = Resources.Load<GameObject>("Process/ChangeScreen/Prefabs/ChangeScreen_0" + (int)(fadeType + 3));
		gameObject2 = Resources.Load<GameObject>("Process/ChangeScreen/Prefabs/Sub_ChangeScreen");
		for (int i = 0; i < 2; i++)
		{
			canvasObject[i, 0] = CreateInstanceAndSetParent(gameObject, fadeObject[i].transform.Find("Canvas/Main"));
			canvasObject[i, 1] = CreateInstanceAndSetParent(gameObject2, fadeObject[i].transform.Find("Canvas/Sub"));
			for (int j = 0; j < 2; j++)
			{
				animator[i, j] = canvasObject[i, j].GetComponent<Animator>();
			}
		}
		isFadeing = true;
	}

	public override void OnUpdate()
	{
	}

	public virtual void ProcessingProcess()
	{
		if (!isAddProcess)
		{
			if (fromProcess != null)
			{
				container.processManager.ReleaseProcess(fromProcess);
			}
			if (toProcess != null)
			{
				container.processManager.AddProcess(toProcess, 20);
			}
			isAddProcess = true;
		}
	}

	public override void OnLateUpdate()
	{
		if (!isFadeing)
		{
			return;
		}
		AnimatorStateInfo currentAnimatorStateInfo = animator[1, 0].GetCurrentAnimatorStateInfo(0);
		switch (state)
		{
		case FadeState.DoFadeOut:
			if (3 < _counter)
			{
				for (int k = 0; k < 2; k++)
				{
					for (int l = 0; l < 2; l++)
					{
						animator[k, l].SetTrigger("In");
					}
				}
				_counter = 0;
				state = FadeState.FadeOut;
			}
			else
			{
				_counter++;
			}
			break;
		case FadeState.FadeOut:
			if (currentAnimatorStateInfo.IsName("In"))
			{
				break;
			}
			if (_restartPleaseWaitID >= 0)
			{
				switch (_restartPleaseWaitID)
				{
				case 0:
					container.processManager.SendMessage(new Message(ProcessType.PleaseWaitProcess, 20005));
					break;
				case 1:
					container.processManager.SendMessage(new Message(ProcessType.PleaseWaitProcess, 20006));
					break;
				}
				_restartPleaseWaitID = -1;
			}
			ProcessingProcess();
			isFadeing = false;
			break;
		case FadeState.DoFadeIn:
			if (3 < _counter)
			{
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						if (animator[i, j] != null)
						{
							animator[i, j].SetTrigger("Out");
						}
					}
				}
				_counter = 0;
				state = FadeState.FadeIn;
			}
			else
			{
				_counter++;
			}
			break;
		case FadeState.FadeIn:
			if (currentAnimatorStateInfo.normalizedTime >= 1f)
			{
				ProcessingProcess();
				container.processManager.ReleaseProcess(this);
				isFadeing = false;
			}
			break;
		}
	}

	public virtual void StartFadeIn()
	{
		state = FadeState.DoFadeIn;
		isFadeing = true;
		_counter = 0;
	}

	public override void OnRelease()
	{
		for (int i = 0; i < 2; i++)
		{
			if (fadeObject[i] != null)
			{
				Object.Destroy(fadeObject[i]);
			}
		}
	}
}
