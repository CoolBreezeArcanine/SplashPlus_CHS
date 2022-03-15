using System;
using System.Collections.Generic;
using System.Linq;
using Manager;
using Net;
using Process.Entry;
using Process.Entry.State;
using Process.MapCore;

namespace Process
{
	public class EntryAgingProcess : MapProcess
	{
		private class Command
		{
			public readonly int Index;

			public readonly ulong UserId;

			public readonly StateType Type;

			public readonly int Mode;

			public readonly float Delay;

			public readonly Action<int, ulong> OnOnce;

			public readonly Action<int, ulong> OnRepeat;

			public Command(int index, ulong userId, StateType type, int mode, float delay, Action<int, ulong> onOnce, Action<int, ulong> onRepeat = null)
			{
				Index = index;
				UserId = userId;
				Type = type;
				Mode = mode;
				Delay = delay;
				OnOnce = onOnce;
				OnRepeat = onRepeat;
			}
		}

		private readonly Queue<Command> _commands = new Queue<Command>();

		private Command _command;

		private readonly EntryProcess _parent;

		private float _elapsedTime;

		public EntryAgingProcess(ProcessDataContainer container, EntryProcess parent)
			: base(container)
		{
			_parent = parent;
		}

		public override void OnStart()
		{
			SetTestCommands(UserID.GuestID(), UserID.GuestID());
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			_parent.GetCurrentState(out var type, out var mode);
			if (_command == null)
			{
				if (_commands.Any())
				{
					Command command = _commands.Peek();
					if (command.Type == type && command.Mode == mode)
					{
						_command = _commands.Dequeue();
						_elapsedTime = 0f;
					}
				}
			}
			else
			{
				_elapsedTime += (float)GameManager.GetGameMSecAdd() / 1000f;
				if (_elapsedTime >= _command.Delay)
				{
					_command.OnOnce?.Invoke(_command.Index, _command.UserId);
					_command = null;
				}
				else
				{
					_command.OnRepeat?.Invoke(_command.Index, _command.UserId);
				}
			}
		}

		private void SetTestCommands(ulong userId0, ulong userId1)
		{
			ulong[] array = new ulong[2] { userId0, userId1 };
			for (int i = 0; i < array.Length; i++)
			{
				ulong num = array[i];
				if (num == 0L)
				{
					continue;
				}
				if (UserID.IsGuest(num))
				{
					_commands.Enqueue(new Command(i, num, StateType.ConfirmEntry, 3, 1.5f, delegate(int idx, ulong _)
					{
						InputManager.SetButtonDown(idx, InputManager.ButtonSetting.Button04);
					}));
					_commands.Enqueue(new Command(i, num, StateType.ConfirmGuest, 0, 1.5f, delegate(int idx, ulong _)
					{
						InputManager.SetButtonDown(idx, InputManager.ButtonSetting.Button04);
					}));
					continue;
				}
				_commands.Enqueue(new Command(i, num, StateType.ConfirmEntry, 3, 1.5f, delegate(int _, ulong id)
				{
					TryAime.TestAimeId = UserID.ToAimeID(id);
				}));
				_commands.Enqueue(new Command(i, num, StateType.ConfirmPlay, 3, 4f, delegate(int idx, ulong _)
				{
					TryAime.TestAimeId = 0u;
					InputManager.SetButtonDown(idx, InputManager.ButtonSetting.Button04);
				}));
			}
			if (_commands.Any())
			{
				_commands.Enqueue(new Command(_commands.Peek().Index, 0uL, StateType.ConfirmEntry, 3, 1.75f, null, delegate(int idx, ulong _)
				{
					InputManager.SetButtonDown(idx, InputManager.ButtonSetting.Button04);
				}));
			}
		}

		private void SetTestFreedomCommands(int index, ulong userId)
		{
			if (userId != 0L)
			{
				_commands.Enqueue(new Command(index, userId, StateType.ConfirmEntry, 3, 1.5f, delegate(int _, ulong id)
				{
					TryAime.TestAimeId = UserID.ToAimeID(id);
				}));
				_commands.Enqueue(new Command(index, userId, StateType.ConfirmPlay, 3, 4f, delegate(int idx, ulong _)
				{
					TryAime.TestAimeId = 0u;
					InputManager.SetButtonDown(idx, InputManager.ButtonSetting.Button02);
				}));
				_commands.Enqueue(new Command(index, userId, StateType.ConfirmFreedom, 0, 1.5f, delegate(int idx, ulong _)
				{
					InputManager.SetButtonDown(idx, InputManager.ButtonSetting.Button04);
				}));
				_commands.Enqueue(new Command(index, userId, StateType.ConfirmFreedom, 1, 1.85f, null, delegate(int idx, ulong _)
				{
					InputManager.SetButtonDown(idx, InputManager.ButtonSetting.Button04);
				}));
			}
		}
	}
}
