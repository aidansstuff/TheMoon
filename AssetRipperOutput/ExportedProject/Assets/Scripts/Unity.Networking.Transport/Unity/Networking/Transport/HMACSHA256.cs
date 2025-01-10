namespace Unity.Networking.Transport
{
	internal static class HMACSHA256
	{
		public unsafe static void ComputeHash(byte* keyValue, int keyArrayLength, byte* messageBytes, int messageLength, byte* result)
		{
			byte* ptr = stackalloc byte[32];
			SHA256.SHA256State sHA256State = SHA256.SHA256State.Create();
			if (keyArrayLength > 64)
			{
				sHA256State.Update(keyValue, keyArrayLength);
				sHA256State.Final(ptr);
				keyValue = ptr;
				keyArrayLength = 32;
			}
			byte* ptr2 = stackalloc byte[64];
			for (int i = 0; i < keyArrayLength; i++)
			{
				ptr2[i] = (byte)(0x36u ^ keyValue[i]);
			}
			for (int j = keyArrayLength; j < 64; j++)
			{
				ptr2[j] = 54;
			}
			sHA256State = SHA256.SHA256State.Create();
			sHA256State.Update(ptr2, 64);
			sHA256State.Update(messageBytes, messageLength);
			sHA256State.Final(result);
			for (int k = 0; k < keyArrayLength; k++)
			{
				ptr2[k] = (byte)(0x5Cu ^ keyValue[k]);
			}
			for (int l = keyArrayLength; l < 64; l++)
			{
				ptr2[l] = 92;
			}
			sHA256State = SHA256.SHA256State.Create();
			sHA256State.Update(ptr2, 64);
			sHA256State.Update(result, 32);
			sHA256State.Final(result);
		}
	}
}
