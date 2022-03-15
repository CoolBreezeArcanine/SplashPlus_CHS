using System.IO;
using CriMana;
using MAI2.Util;
using UnityEngine;

namespace Manager
{
	public class MovieController : MonoBehaviour
	{
		private MovieMaterialMai2 _moviePlayers;

		public Material MovieMaterial;

		private bool _runFlag;

		private const double MovieFps = 33.333333333333336;

		private void Awake()
		{
			_moviePlayers = base.gameObject.AddComponent<MovieMaterialMai2>();
			_moviePlayers.material = MovieMaterial;
			_moviePlayers.loop = false;
		}

		public void OnDestroy()
		{
			_moviePlayers = null;
			MovieMaterial = null;
		}

		public void SetMusicMovie(int movieId)
		{
			_moviePlayers.player.SetFile(null, CreateMusicMoviePath(movieId));
			_moviePlayers.player.SetAudioTrack(Player.AudioTrack.Off);
			_moviePlayers.player.SetVolume(0f);
			_moviePlayers.player.PrepareForRendering();
		}

		public void SetMovieFile(string movieName)
		{
			_moviePlayers.player.SetFile(null, CreateMoviePath(movieName));
			_moviePlayers.player.SetAudioTrack(Player.AudioTrack.Off);
			_moviePlayers.player.SetVolume(0f);
			_moviePlayers.player.PrepareForRendering();
		}

		public bool IsMoviePrepare()
		{
			return _moviePlayers.player.status == Player.Status.ReadyForRendering;
		}

		public uint GetMovieHeight()
		{
			return _moviePlayers.player.movieInfo.dispHeight;
		}

		public uint GetMovieWidth()
		{
			return _moviePlayers.player.movieInfo.dispWidth;
		}

		public void Play(int frame = 2)
		{
			_runFlag = false;
			_moviePlayers.player.SetSeekPosition(frame);
			_moviePlayers.player.Start();
			_moviePlayers.ForceUpdate();
		}

		public void Pause(bool pauseFlag)
		{
			_runFlag = pauseFlag;
			_moviePlayers.player.Pause(_runFlag);
		}

		public ulong GetTime()
		{
			FrameInfo frameInfo = _moviePlayers.player.frameInfo;
			if (frameInfo == null)
			{
				return 0uL;
			}
			return frameInfo.time * 1000 / frameInfo.tunit;
		}

		public void SetSeekFrame(double msec)
		{
			int seekPosition = (int)(msec / 33.333333333333336);
			_moviePlayers.player.Stop();
			while (_moviePlayers.player.status != 0)
			{
				_moviePlayers.player.Update();
			}
			_moviePlayers.player.SetSeekPosition(seekPosition);
			_moviePlayers.player.Pause(sw: true);
			_moviePlayers.player.Start();
		}

		public void Stop()
		{
			_runFlag = false;
			_moviePlayers.player.Stop();
		}

		public bool IsEnd()
		{
			return _moviePlayers.player.status == Player.Status.PlayEnd;
		}

		private string CreateMusicMoviePath(int musicId)
		{
			return CreateMoviePath($"{musicId:000000}");
		}

		private string CreateMoviePath(string movieName)
		{
			string text = string.Format(Singleton<OptionDataManager>.Instance.GetMovieDataPath(movieName) + ".dat");
			if (!File.Exists(text))
			{
				text = Application.streamingAssetsPath + "/A000/MovieData/dummy.dat";
			}
			return text;
		}
	}
}
