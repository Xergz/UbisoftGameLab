using System;
using System.IO;

namespace Backend {
	namespace Core {

		/**
		 * @brief Implémentation de l'algorithme de hachage Murmur3.
		 * @reference https://en.wikipedia.org/wiki/MurmurHash
		 */
		public class Murmur3
		{
			// Définition de constantes nécéssaire à l'algorithme
			private const UInt32 C1 = 0xCC9E2D51;
			private const UInt32 C2 = 0x1B873593;
			private const int R1 = 15;
			private const int R2 = 13;
			private const UInt32 M =  5;
			private const UInt32 N =  0XE6546B64;

			private static UInt32 Rotate4Bytes(UInt32 x, byte y) {
				return ((x << y) | (x >> (32 - y)));
			}

			/**
			 * @brief Hache un tableau de données
			 * @param data Les données à hacher
			 * @param seed La valeur initiale du hash
			 * @return La valeur du hache
			 */
			public static UInt32 Hash(byte[] data, uint seed) {
				// Le seed initialise la valeur du hash
				UInt32 hash = seed;

				// Sépare le buffer de données en blocs de 32 bits (4 octets)
				uint nbBlocks = (uint)data.Length / 4;
				for (int i = 0; i < nbBlocks; ++i) {
					// On récupère un bloc de 32 bits
					UInt32 currentBlock = BitConverter.ToUInt32(data, i * 4);
					currentBlock *= C1;
					currentBlock = Rotate4Bytes(currentBlock, R1);
					currentBlock *= C2;

					hash ^= currentBlock;
					hash = Rotate4Bytes(hash, R2) * M + N;
				}

				// On gère le dernier bloc qui pourrait être de taille inférieur à 32 bits
				UInt32 tailLength = (uint)data.Length - nbBlocks * 4;
				if (tailLength > 0) {
					// On récupère les derniers octets
					Byte[] tail = new Byte[tailLength];
					Array.Copy (data, nbBlocks * 4, tail, 0, tailLength);
					
					UInt32 k1 = 0;
					
					switch ((uint)data.Length & 3) {
					case 3:
						k1 ^= (uint)tail[2] << 16;
						break;
					case 2:
						k1 ^= (uint)tail[1] << 8;
						break;
					case 1:
						k1 ^= tail[0];
						break;
					}
					k1 = Rotate4Bytes(k1, R1);
					k1 *= C2;
					hash ^= k1;
				}

				hash ^= (uint)data.Length;
				hash ^= (hash >> 16);
				hash *= 0X85EBCA6B;
				hash ^= (hash >> 13);
				hash *= 0xC2B2AE35;
				hash ^= (hash >> 16);

				return hash;
			}
		}
	}
}