using System;

namespace ExpansionImage
{
	[Serializable]
	public class Wave
	{
		public float Frequency;

		public float Amplitude;

		public Wave(float amplitude, float frequency)
		{
			Amplitude = amplitude;
			Frequency = frequency;
		}
	}
}
