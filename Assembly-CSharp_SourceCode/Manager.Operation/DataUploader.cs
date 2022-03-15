using System.Collections.Generic;
using System.IO;
using System.Linq;
using MAI2.Util;
using MAI2System;
using Net.Packet;
using Net.Packet.Helper;
using Net.Packet.Mai2;

namespace Manager.Operation
{
	public class DataUploader
	{
		public enum State
		{
			Idle,
			UploadSetting,
			UploadBookkeep,
			UploadFiles,
			UploadTestmode
		}

		private readonly Mode<DataUploader, State> _mode;

		private Queue<DailyLog> _dailyLogQueue;

		private Queue<string> _errorLogQueue;

		public bool IsFinished { get; private set; }

		public DataUploader()
		{
			_mode = new Mode<DataUploader, State>(this);
		}

		public void Execute()
		{
			_mode.update();
		}

		public bool Start()
		{
			if (_mode.get() != 0)
			{
				return false;
			}
			_dailyLogQueue = new Queue<DailyLog>(SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.bookkeep.getDailyLogList(needToday: false, needReport: true));
			_errorLogQueue = new Queue<string>(MAI2System.Path.GetErrorFiles(MAI2System.Path.ErrorLogPath, ".log"));
			_mode.set(State.UploadSetting);
			IsFinished = false;
			return true;
		}

		private void UploadSetting_Init()
		{
			PacketHelper.StartPacket(new PacketUpsertClientSetting(delegate
			{
				_mode.set(State.UploadBookkeep);
			}, NetworkError));
		}

		private void UploadBookkeep_Init()
		{
			if (!_dailyLogQueue.Any())
			{
				_mode.set(State.UploadFiles);
				return;
			}
			PacketHelper.StartPacket(new PacketUpsertClientBookkeeping(_dailyLogQueue, delegate
			{
				_mode.set(State.UploadFiles);
			}, NetworkError));
		}

		private void UploadFiles_Init()
		{
			if (!_errorLogQueue.Any())
			{
				_mode.set(State.UploadTestmode);
				return;
			}
			string text = _errorLogQueue.Peek();
			try
			{
				bool flag = false;
				if (Directory.Exists(MAI2System.Path.ErrorLogPath) && File.Exists(text))
				{
					FileInfo fileInfo = new FileInfo(text);
					if (fileInfo != null)
					{
						if ((fileInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
						{
							fileInfo.Attributes = FileAttributes.Normal;
						}
						long length = fileInfo.Length;
						long num = 7168L;
						if (length < num)
						{
							flag = true;
						}
						else
						{
							fileInfo.Delete();
						}
					}
				}
				if (flag)
				{
					byte[] source = File.ReadAllBytes(text);
					PacketHelper.StartPacket(new PacketUpsertClientUpload(text, source, delegate
					{
						_mode.set(State.UploadFiles);
					}, NetworkError));
				}
			}
			catch
			{
				_mode.set(State.UploadFiles);
			}
		}

		private void UploadTestmode_Init()
		{
			PacketHelper.StartPacket(new PacketUpsertClientTestmode(delegate
			{
				IsFinished = true;
				_mode.set(State.Idle);
			}, NetworkError));
		}

		private void NetworkError(PacketStatus err)
		{
			IsFinished = true;
			_mode.set(State.Idle);
		}
	}
}
