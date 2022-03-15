using System;
using MAI2.Util;
using MAI2System;

namespace Manager
{
	public class WasapiExclusive
	{
		public static void Intialize()
		{
			CriAtomUserExtension.WaveFormatExtensible format = default(CriAtomUserExtension.WaveFormatExtensible);
			format.Format = default(CriAtomUserExtension.WaveFormatEx);
			format.Format.wFormatTag = 65534;
			format.Format.nSamplesPerSec = 48000u;
			format.Format.wBitsPerSample = 32;
			format.Format.cbSize = 22;
			format.Samples.wValidBitsPerSample = 24;
			format.SubFormat = new Guid("00000001-0000-0010-8000-00aa00389b71");
			if (Singleton<SystemConfig>.Instance.config.Is8Ch)
			{
				format.Format.nChannels = 8;
				format.dwChannelMask = 1599u;
				CriAtomUserExtension.SetAudioClientShareMode(CriAtomUserExtension.AudioClientShareMode.Exclusive);
			}
			else
			{
				format.Format.nChannels = 2;
				format.dwChannelMask = 3u;
				CriAtomUserExtension.SetAudioClientShareMode(CriAtomUserExtension.AudioClientShareMode.Shared);
			}
			format.Format.nBlockAlign = (ushort)((int)format.Format.wBitsPerSample / 8 * format.Format.nChannels);
			format.Format.nAvgBytesPerSec = format.Format.nSamplesPerSec * format.Format.nBlockAlign;
			CriAtomUserExtension.SetAudioBufferTime(160000uL);
			CriAtomUserExtension.SetAudioClientFormat(ref format);
		}
	}
}
