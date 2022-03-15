using System;
using System.Runtime.InteropServices;
using MAI2.Util;

namespace Net
{
	internal class CipherAES : Singleton<CipherAES>
	{
		[DllImport("Cake")]
		private static extern int GetEncryptedSize(int inSize);

		[DllImport("Cake")]
		private static extern bool Encrypt(byte[] inData, byte[] outData, int inSize, ref int outSize);

		[DllImport("Cake")]
		private static extern bool Decrypt(byte[] inData, byte[] outData, int inSize, ref int outSize);

		public static bool Encrypt(byte[] data, out byte[] encryptData)
		{
			encryptData = new byte[GetEncryptedSize(data.Length)];
			int outSize = 0;
			return Encrypt(data, encryptData, data.Length, ref outSize);
		}

		public static bool Decrypt(byte[] encryptData, out byte[] plainData)
		{
			plainData = new byte[encryptData.Length];
			int outSize = 0;
			bool result = Decrypt(encryptData, plainData, encryptData.Length, ref outSize);
			Array.Resize(ref plainData, outSize);
			return result;
		}
	}
}
