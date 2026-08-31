using System;
using System.Collections.Generic;
using System.IO;
using SynapseGaming.LightingSystem.Core;

namespace Z
{
	internal class Z<Tkey, Tvalue>
	{
		internal static uint HCB = 2147483647u;

		internal Tkey HC_0002;

		internal global::Z.Z<Tkey, Tvalue> HC_0012;

		internal global::Z.Z<Tkey, Tvalue> HCH;

		internal List<Tvalue> HC7 = new List<Tvalue>();

		internal uint HC_0001;

		private global::Z.Z<Tkey, Tvalue> HCw;

		private global::Z.Z<Tkey, Tvalue> HCZ;

		private global::Z.Z<Tkey, Tvalue> HC_000F;

		private static PooledObjectFactory<global::Z.Z<Tkey, Tvalue>> HCy = new PooledObjectFactory<global::Z.Z<Tkey, Tvalue>>();

		public Z(uint hashcode)
		{
			HC_0001 = hashcode;
		}

		public Z()
		{
			HC_0001 = HCB;
		}

		internal global::Z.Z<Tkey, Tvalue> _0002D()
		{
			return _0002K(default(Tkey), 0u);
		}

		internal global::Z.Z<Tkey, Tvalue> _0002_0011(Tkey P_0)
		{
			return _0002K(P_0);
		}

		internal global::Z.Z<Tkey, Tvalue> _0002K(Tkey P_0)
		{
			return _0002K(P_0, (uint)P_0.GetHashCode());
		}

		internal global::Z.Z<Tkey, Tvalue> _0002K(Tkey P_0, uint P_1)
		{
			if (P_1 == HC_0001)
			{
				if (EqualityComparer<Tkey>.Default.Equals(HC_0002, default(Tkey)))
				{
					HC_0002 = P_0;
				}
				return this;
			}
			if (P_1 > HC_0001)
			{
				if (HCw == null)
				{
					HCw = HCy.New();
					HCw.HC_0001 = P_1;
					HCw.HC_000F = this;
					_0002_0003(HC_0012, this, HCw);
				}
				return HCw._0002K(P_0, P_1);
			}
			if (HCZ == null)
			{
				HCZ = HCy.New();
				HCZ.HC_0001 = P_1;
				HCZ.HC_000F = this;
				_0002_0003(this, HCH, HCZ);
			}
			return HCZ._0002K(P_0, P_1);
		}

		internal void G()
		{
			HC_0002 = default(Tkey);
			HC7.Clear();
			HC_0001 = HCB;
			HC_0012 = null;
			HCH = null;
			HC_000F = null;
			if (HCw != null)
			{
				HCw.G();
				HCy.Free(HCw);
				HCw = null;
			}
			if (HCZ != null)
			{
				HCZ.G();
				HCy.Free(HCZ);
				HCZ = null;
			}
		}

		private void _0002_0003(global::Z.Z<Tkey, Tvalue> P_0, global::Z.Z<Tkey, Tvalue> P_1, global::Z.Z<Tkey, Tvalue> P_2)
		{
			if (P_0 != null)
			{
				P_0.HCH = P_2;
			}
			if (P_1 != null)
			{
				P_1.HC_0012 = P_2;
			}
			P_2.HC_0012 = P_0;
			P_2.HCH = P_1;
		}

		internal static void _3(global::Z.Z<Tkey, Tvalue> P_0)
		{
			if (P_0.HC_000F == null)
			{
				return;
			}
			if (P_0.HC_0012 != null)
			{
				P_0.HC_0012.HCH = P_0.HCH;
			}
			if (P_0.HCH != null)
			{
				P_0.HCH.HC_0012 = P_0.HC_0012;
			}
			if (P_0.HCw == null)
			{
				if (P_0.Equals(P_0.HC_000F.HCw))
				{
					P_0.HC_000F.HCw = P_0.HCZ;
				}
				else
				{
					P_0.HC_000F.HCZ = P_0.HCZ;
				}
				if (P_0.HCZ != null)
				{
					P_0.HCZ.HC_000F = P_0.HC_000F;
				}
			}
			else if (P_0.HCZ == null)
			{
				if (P_0.Equals(P_0.HC_000F.HCw))
				{
					P_0.HC_000F.HCw = P_0.HCw;
				}
				else
				{
					P_0.HC_000F.HCZ = P_0.HCw;
				}
				if (P_0.HCw != null)
				{
					P_0.HCw.HC_000F = P_0.HC_000F;
				}
			}
			else
			{
				if (P_0.Equals(P_0.HC_000F.HCw))
				{
					P_0.HC_000F.HCw = P_0.HCZ;
				}
				else
				{
					P_0.HC_000F.HCZ = P_0.HCZ;
				}
				P_0.HCZ.HC_000F = P_0.HC_000F;
				global::Z.Z<Tkey, Tvalue> z = P_0.HCH;
				while (z.HCw != null)
				{
					z = z.HCw;
				}
				z.HCw = P_0.HCw;
				P_0.HCw.HC_000F = z;
			}
			P_0.HCw = null;
			P_0.HCZ = null;
			P_0.G();
			HCy.Free(P_0);
		}

		internal static void _0002k(global::Z.Z<Tkey, Tvalue> P_0)
		{
			global::Z.Z<Tkey, Tvalue> z = P_0;
			while (z.HC_000F != null)
			{
				z = z.HC_000F;
			}
			global::Z.Z<Tkey, Tvalue> z2 = z._0002D();
			uint num = z2.HC_0001;
			int num2 = 0;
			while (z2 != null)
			{
				if (num > z2.HC_0001 || (num == z2.HC_0001 && num != 0))
				{
					throw new Exception("Tree failed link verification.");
				}
				num2++;
				num = z2.HC_0001;
				z2 = z2.HC_0012;
			}
			int num3 = 0;
			_0002s(z, ref num3);
			if (num2 != num3)
			{
				throw new Exception("Tree failed comparison verification.");
			}
		}

		internal static void _0002s(global::Z.Z<Tkey, Tvalue> P_0, ref int P_1)
		{
			bool flag = false;
			if (P_0.HCZ != null && P_0.HCZ.HC_0001 >= P_0.HC_0001)
			{
				flag = true;
			}
			if (P_0.HCw != null && P_0.HCw.HC_0001 <= P_0.HC_0001)
			{
				flag = true;
			}
			if (flag)
			{
				throw new Exception("Tree failed map verification.");
			}
			if (P_0.HCZ != null)
			{
				_0002s(P_0.HCZ, ref P_1);
			}
			if (P_0.HCw != null)
			{
				_0002s(P_0.HCw, ref P_1);
			}
			P_1++;
		}
	}
}
namespace z
{
	internal abstract class Z : w, IDisposable
	{
		protected byte[] HashValue;

		protected int HashSizeValue;

		protected int State;

		private bool HCB;

		public virtual bool CanTransformMultipleBlocks => true;

		public virtual bool CanReuseTransform => true;

		public virtual byte[] Hash
		{
			get
			{
				if (HashValue == null)
				{
					throw new _0001("No hash value computed.");
				}
				return HashValue;
			}
		}

		public virtual int HashSize => HashSizeValue;

		public virtual int InputBlockSize => 1;

		public virtual int OutputBlockSize => 1;

		protected Z()
		{
			HCB = false;
		}

		public void Clear()
		{
			Dispose(disposing: true);
		}

		public byte[] ComputeHash(byte[] input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			return ComputeHash(input, 0, input.Length);
		}

		public byte[] ComputeHash(byte[] buffer, int offset, int count)
		{
			if (HCB)
			{
				throw new ObjectDisposedException("HashAlgorithm");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentException("count", "< 0");
			}
			if (offset > buffer.Length - count)
			{
				throw new ArgumentException("offset + count", "Overflow");
			}
			HashCore(buffer, offset, count);
			HashValue = HashFinal();
			Initialize();
			return HashValue;
		}

		public byte[] ComputeHash(Stream inputStream)
		{
			if (HCB)
			{
				throw new ObjectDisposedException("HashAlgorithm");
			}
			byte[] array = new byte[4096];
			for (int num = inputStream.Read(array, 0, 4096); num > 0; num = inputStream.Read(array, 0, 4096))
			{
				HashCore(array, 0, num);
			}
			HashValue = HashFinal();
			Initialize();
			return HashValue;
		}

		public static Z Create()
		{
			throw new Exception("HashAlgorithm.Create not supported.");
		}

		protected abstract void HashCore(byte[] rgb, int start, int size);

		protected abstract byte[] HashFinal();

		public abstract void Initialize();

		protected virtual void Dispose(bool disposing)
		{
			HCB = true;
		}

		void IDisposable.Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			if (inputBuffer == null)
			{
				throw new ArgumentNullException("inputBuffer");
			}
			if (inputOffset < 0)
			{
				throw new ArgumentOutOfRangeException("inputOffset", "< 0");
			}
			if (inputCount < 0)
			{
				throw new ArgumentException("inputCount");
			}
			if (inputOffset < 0 || inputOffset > inputBuffer.Length - inputCount)
			{
				throw new ArgumentException("inputBuffer");
			}
			if (outputBuffer != null)
			{
				if (outputOffset < 0)
				{
					throw new IndexOutOfRangeException("outputBuffer");
				}
				if (outputOffset > outputBuffer.Length - inputCount)
				{
					throw new IndexOutOfRangeException("outputBuffer");
				}
			}
			HashCore(inputBuffer, inputOffset, inputCount);
			if (outputBuffer != null)
			{
				Buffer.BlockCopy(inputBuffer, inputOffset, outputBuffer, outputOffset, inputCount);
			}
			return inputCount;
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			if (inputBuffer == null)
			{
				throw new ArgumentNullException("inputBuffer");
			}
			if (inputCount < 0)
			{
				throw new ArgumentException("inputCount");
			}
			if (inputOffset > inputBuffer.Length - inputCount)
			{
				throw new ArgumentException("inputOffset + inputCount", "Overflow");
			}
			byte[] array = new byte[inputCount];
			Buffer.BlockCopy(inputBuffer, inputOffset, array, 0, inputCount);
			HashCore(inputBuffer, inputOffset, inputCount);
			HashValue = HashFinal();
			Initialize();
			return array;
		}
	}
}
