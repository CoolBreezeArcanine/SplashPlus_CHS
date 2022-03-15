namespace Manager
{
	public struct CameraParameter
	{
		public readonly int Width;

		public readonly int Height;

		public readonly int Fps;

		public CameraParameter(int width, int height, int fps)
		{
			Width = width;
			Height = height;
			Fps = fps;
		}
	}
}
