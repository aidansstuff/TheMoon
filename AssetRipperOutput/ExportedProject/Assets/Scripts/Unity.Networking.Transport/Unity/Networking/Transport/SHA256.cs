namespace Unity.Networking.Transport
{
	internal static class SHA256
	{
		internal struct SHA256State
		{
			public unsafe fixed uint state[8];

			public unsafe fixed byte buffer[64];

			private ulong count;

			private static readonly uint[] K = new uint[64]
			{
				1116352408u, 1899447441u, 3049323471u, 3921009573u, 961987163u, 1508970993u, 2453635748u, 2870763221u, 3624381080u, 310598401u,
				607225278u, 1426881987u, 1925078388u, 2162078206u, 2614888103u, 3248222580u, 3835390401u, 4022224774u, 264347078u, 604807628u,
				770255983u, 1249150122u, 1555081692u, 1996064986u, 2554220882u, 2821834349u, 2952996808u, 3210313671u, 3336571891u, 3584528711u,
				113926993u, 338241895u, 666307205u, 773529912u, 1294757372u, 1396182291u, 1695183700u, 1986661051u, 2177026350u, 2456956037u,
				2730485921u, 2820302411u, 3259730800u, 3345764771u, 3516065817u, 3600352804u, 4094571909u, 275423344u, 430227734u, 506948616u,
				659060556u, 883997877u, 958139571u, 1322822218u, 1537002063u, 1747873779u, 1955562222u, 2024104815u, 2227730452u, 2361852424u,
				2428436474u, 2756734187u, 3204031479u, 3329325298u
			};

			public unsafe static SHA256State Create()
			{
				SHA256State result = default(SHA256State);
				result.state[0] = 1779033703u;
				result.state[1] = 3144134277u;
				result.state[2] = 1013904242u;
				result.state[3] = 2773480762u;
				result.state[4] = 1359893119u;
				result.state[5] = 2600822924u;
				result.state[6] = 528734635u;
				result.state[7] = 1541459225u;
				return result;
			}

			public unsafe void Update(byte* data, int length)
			{
				ulong num = count & 0x3F;
				while (length > 0)
				{
					buffer[num++] = *(data++);
					count++;
					length--;
					if (num == 64)
					{
						num = 0uL;
						WriteByteBlock();
					}
				}
			}

			public unsafe void Final(byte* dest)
			{
				ulong num = count << 3;
				uint num2 = (uint)(count & 0x3F);
				buffer[num2++] = 128;
				while (num2 != 56)
				{
					num2 &= 0x3Fu;
					if (num2 == 0)
					{
						WriteByteBlock();
					}
					buffer[num2++] = 0;
				}
				for (int i = 0; i < 8; i++)
				{
					buffer[num2++] = (byte)(num >> 56);
					num <<= 8;
				}
				WriteByteBlock();
				for (int j = 0; j < 8; j++)
				{
					*(dest++) = (byte)(state[j] >> 24);
					*(dest++) = (byte)(state[j] >> 16);
					*(dest++) = (byte)(state[j] >> 8);
					*(dest++) = (byte)state[j];
				}
			}

			private unsafe void WriteByteBlock()
			{
				uint* ptr = stackalloc uint[16];
				for (int i = 0; i < 16; i++)
				{
					ptr[i] = (uint)((buffer[i * 4] << 24) + (buffer[i * 4 + 1] << 16) + (buffer[i * 4 + 2] << 8) + buffer[i * 4 + 3]);
				}
				Transform(ptr);
			}

			private unsafe void Transform(uint* data)
			{
				uint* ptr = stackalloc uint[16];
				uint* ptr2 = stackalloc uint[8];
				for (int i = 0; i < 8; i++)
				{
					ptr2[i] = state[i];
				}
				for (int j = 0; j < 64; j += 16)
				{
					for (int k = 0; k < 16; k++)
					{
						ptr2[(7 - k) & 7] += S1(ptr2[(4 - k) & 7]) + Ch(ptr2[(4 - k) & 7], ptr2[(5 - k) & 7], ptr2[(6 - k) & 7]) + K[k + j] + ((j != 0) ? (ptr[k & 0xF] += s1(ptr[(k - 2) & 0xF]) + ptr[(k - 7) & 0xF] + s0(ptr[(k - 15) & 0xF])) : (ptr[k] = data[k]));
						ptr2[(3 - k) & 7] += ptr2[(7 - k) & 7];
						ptr2[(7 - k) & 7] += S0(ptr2[-k & 7]) + Maj(ptr2[-k & 7], ptr2[(1 - k) & 7], ptr2[(2 - k) & 7]);
					}
				}
				for (int l = 0; l < 8; l++)
				{
					ref uint reference = ref state[l];
					reference += ptr2[l];
				}
				static uint Ch(uint x, uint y, uint z)
				{
					return z ^ (x & (y ^ z));
				}
				static uint Maj(uint x, uint y, uint z)
				{
					return (x & y) | (z & (x | y));
				}
				static uint ROTR32(uint x, byte n)
				{
					return (x << 32 - n) | (x >> (int)n);
				}
				static uint S0(uint x)
				{
					return ROTR32(x, 2) ^ ROTR32(x, 13) ^ ROTR32(x, 22);
				}
				static uint S1(uint x)
				{
					return ROTR32(x, 6) ^ ROTR32(x, 11) ^ ROTR32(x, 25);
				}
				static uint s0(uint x)
				{
					return ROTR32(x, 7) ^ ROTR32(x, 18) ^ (x >> 3);
				}
				static uint s1(uint x)
				{
					return ROTR32(x, 17) ^ ROTR32(x, 19) ^ (x >> 10);
				}
			}
		}
	}
}
