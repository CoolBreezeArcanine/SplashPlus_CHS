using System;
using System.Runtime.InteropServices;

namespace Manager
{
	public static class CriAtomUserExtension
	{
		public enum AudioClientShareMode
		{
			Shared,
			Exclusive
		}

		public struct WaveFormatEx
		{
			public ushort wFormatTag;

			public ushort nChannels;

			public uint nSamplesPerSec;

			public uint nAvgBytesPerSec;

			public ushort nBlockAlign;

			public ushort wBitsPerSample;

			public ushort cbSize;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct Samples
		{
			[FieldOffset(0)]
			public ushort wValidBitsPerSample;

			[FieldOffset(0)]
			public ushort wSamplesPerBlock;

			[FieldOffset(0)]
			public ushort wReserved;
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct WaveFormatExtensible
		{
			[FieldOffset(0)]
			public WaveFormatEx Format;

			[FieldOffset(18)]
			public Samples Samples;

			[FieldOffset(20)]
			public uint dwChannelMask;

			[FieldOffset(24)]
			public Guid SubFormat;
		}

		public static void SetAudioClientShareMode(AudioClientShareMode mode)
		{
			criAtom_SetAudioClientShareMode_WASAPI(mode);
		}

		public static void SetAudioClientFormat(ref WaveFormatExtensible format)
		{
			criAtom_SetAudioClientFormat_WASAPI(ref format);
		}

		public static void SetAudioBufferTime(ulong ref_time)
		{
			criAtom_SetAudioClientBufferDuration_WASAPI(ref_time);
		}

		[DllImport("cri_ware_unity")]
		private static extern void criAtom_SetAudioClientShareMode_WASAPI(AudioClientShareMode mode);

		[DllImport("cri_ware_unity")]
		private static extern void criAtom_SetAudioClientFormat_WASAPI([In] ref WaveFormatExtensible format);

		[DllImport("cri_ware_unity")]
		private static extern void criAtom_SetAudioClientBufferDuration_WASAPI(ulong ref_time);
	}
}
