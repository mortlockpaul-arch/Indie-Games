using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using _0001;
using _0002;
using _0004;
using _0013;
using _0014;
using D;
using E;
using I;
using L;
using Microsoft.Xna.Framework;
using N;
using P;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Collision.Legacy;
using SynapseGaming.LightingSystem.Core;
using T;
using X;
using Y;
using d;
using l;
using m;
using n;
using r;
using s;
using v;
using y;

namespace _0006
{
	internal sealed class h
	{
		private h()
		{
		}

		private static int _0004(byte[] P_0, int P_1)
		{
			return (P_0[P_1 + 3] << 24) | (P_0[P_1 + 2] << 16) | (P_0[P_1 + 1] << 8) | P_0[P_1];
		}

		private static uint i(byte[] P_0, int P_1)
		{
			return (uint)((P_0[P_1 + 3] << 24) | (P_0[P_1 + 2] << 16) | (P_0[P_1 + 1] << 8) | P_0[P_1]);
		}

		private static byte[] _0010(int P_0)
		{
			return new byte[4]
			{
				(byte)(P_0 & 0xFF),
				(byte)((P_0 >> 8) & 0xFF),
				(byte)((P_0 >> 16) & 0xFF),
				(byte)((P_0 >> 24) & 0xFF)
			};
		}

		private static byte[] G(byte[] P_0)
		{
			for (int i = 0; i < P_0.Length; i++)
			{
				if (P_0[i] != 0)
				{
					byte[] array = new byte[P_0.Length - i];
					Buffer.BlockCopy(P_0, i, array, 0, array.Length);
					return array;
				}
			}
			return null;
		}

		public static v.b FromCapiPrivateKeyBlob(byte[] blob)
		{
			return FromCapiPrivateKeyBlob(blob, 0);
		}

		public static v.b FromCapiPrivateKeyBlob(byte[] blob, int offset)
		{
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			if (offset >= blob.Length)
			{
				throw new ArgumentException("blob is too small.");
			}
			try
			{
				if (blob[offset] != 7 || blob[offset + 1] != 2 || blob[offset + 2] != 0 || blob[offset + 3] != 0 || i(blob, offset + 8) != 843141970)
				{
					throw new v._0006("Invalid blob header");
				}
				int num = _0004(blob, offset + 12);
				v._0001 parameters = default(v._0001);
				byte[] array = new byte[4];
				Buffer.BlockCopy(blob, offset + 16, array, 0, 4);
				Array.Reverse(array);
				parameters.Exponent = G(array);
				int num2 = offset + 20;
				int num3 = num >> 3;
				parameters.Modulus = new byte[num3];
				Buffer.BlockCopy(blob, num2, parameters.Modulus, 0, num3);
				Array.Reverse(parameters.Modulus);
				num2 += num3;
				int num4 = num3 >> 1;
				parameters.P = new byte[num4];
				Buffer.BlockCopy(blob, num2, parameters.P, 0, num4);
				Array.Reverse(parameters.P);
				num2 += num4;
				parameters.Q = new byte[num4];
				Buffer.BlockCopy(blob, num2, parameters.Q, 0, num4);
				Array.Reverse(parameters.Q);
				num2 += num4;
				parameters.DP = new byte[num4];
				Buffer.BlockCopy(blob, num2, parameters.DP, 0, num4);
				Array.Reverse(parameters.DP);
				num2 += num4;
				parameters.DQ = new byte[num4];
				Buffer.BlockCopy(blob, num2, parameters.DQ, 0, num4);
				Array.Reverse(parameters.DQ);
				num2 += num4;
				parameters.InverseQ = new byte[num4];
				Buffer.BlockCopy(blob, num2, parameters.InverseQ, 0, num4);
				Array.Reverse(parameters.InverseQ);
				num2 += num4;
				parameters.D = new byte[num3];
				if (num2 + num3 + offset <= blob.Length)
				{
					Buffer.BlockCopy(blob, num2, parameters.D, 0, num3);
					Array.Reverse(parameters.D);
				}
				v.b b2 = null;
				try
				{
					b2 = v.b.Create();
					b2.ImportParameters(parameters);
				}
				catch (v._0006)
				{
				}
				return b2;
			}
			catch (Exception inner)
			{
				throw new v._0006("Invalid blob.", inner);
			}
		}

		public static byte[] ToCapiPrivateKeyBlob(v.b rsa)
		{
			v._0001 obj = rsa.ExportParameters(include: true);
			int num = obj.Modulus.Length;
			byte[] array = new byte[20 + (num << 2) + (num >> 1)];
			array[0] = 7;
			array[1] = 2;
			array[5] = 36;
			array[8] = 82;
			array[9] = 83;
			array[10] = 65;
			array[11] = 50;
			byte[] array2 = _0010(num << 3);
			array[12] = array2[0];
			array[13] = array2[1];
			array[14] = array2[2];
			array[15] = array2[3];
			int num2 = 16;
			int num3 = obj.Exponent.Length;
			while (num3 > 0)
			{
				array[num2++] = obj.Exponent[--num3];
			}
			num2 = 20;
			byte[] modulus = obj.Modulus;
			int num4 = modulus.Length;
			Array.Reverse(modulus, 0, num4);
			Buffer.BlockCopy(modulus, 0, array, num2, num4);
			num2 += num4;
			modulus = obj.P;
			num4 = modulus.Length;
			Array.Reverse(modulus, 0, num4);
			Buffer.BlockCopy(modulus, 0, array, num2, num4);
			num2 += num4;
			modulus = obj.Q;
			num4 = modulus.Length;
			Array.Reverse(modulus, 0, num4);
			Buffer.BlockCopy(modulus, 0, array, num2, num4);
			num2 += num4;
			modulus = obj.DP;
			num4 = modulus.Length;
			Array.Reverse(modulus, 0, num4);
			Buffer.BlockCopy(modulus, 0, array, num2, num4);
			num2 += num4;
			modulus = obj.DQ;
			num4 = modulus.Length;
			Array.Reverse(modulus, 0, num4);
			Buffer.BlockCopy(modulus, 0, array, num2, num4);
			num2 += num4;
			modulus = obj.InverseQ;
			num4 = modulus.Length;
			Array.Reverse(modulus, 0, num4);
			Buffer.BlockCopy(modulus, 0, array, num2, num4);
			num2 += num4;
			modulus = obj.D;
			num4 = modulus.Length;
			Array.Reverse(modulus, 0, num4);
			Buffer.BlockCopy(modulus, 0, array, num2, num4);
			return array;
		}

		public static v.b FromCapiPublicKeyBlob(byte[] blob)
		{
			return FromCapiPublicKeyBlob(blob, 0);
		}

		public static v.b FromCapiPublicKeyBlob(byte[] blob, int offset)
		{
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			if (offset >= blob.Length)
			{
				throw new ArgumentException("blob is too small.");
			}
			try
			{
				if (blob[offset] != 6 || blob[offset + 1] != 2 || blob[offset + 2] != 0 || blob[offset + 3] != 0 || i(blob, offset + 8) != 826364754)
				{
					throw new v._0006("Invalid blob header");
				}
				int num = _0004(blob, offset + 12);
				v._0001 parameters = new v._0001
				{
					Exponent = new byte[3]
				};
				parameters.Exponent[0] = blob[offset + 18];
				parameters.Exponent[1] = blob[offset + 17];
				parameters.Exponent[2] = blob[offset + 16];
				int srcOffset = offset + 20;
				int num2 = num >> 3;
				parameters.Modulus = new byte[num2];
				Buffer.BlockCopy(blob, srcOffset, parameters.Modulus, 0, num2);
				Array.Reverse(parameters.Modulus);
				v.b b2 = null;
				try
				{
					b2 = v.b.Create();
					b2.ImportParameters(parameters);
				}
				catch (v._0006)
				{
				}
				return b2;
			}
			catch (Exception inner)
			{
				throw new v._0006("Invalid blob.", inner);
			}
		}

		public static byte[] ToCapiPublicKeyBlob(v.b rsa)
		{
			v._0001 obj = rsa.ExportParameters(include: false);
			int num = obj.Modulus.Length;
			byte[] array = new byte[20 + num];
			array[0] = 6;
			array[1] = 2;
			array[5] = 36;
			array[8] = 82;
			array[9] = 83;
			array[10] = 65;
			array[11] = 49;
			byte[] array2 = _0010(num << 3);
			array[12] = array2[0];
			array[13] = array2[1];
			array[14] = array2[2];
			array[15] = array2[3];
			int num2 = 16;
			int num3 = obj.Exponent.Length;
			while (num3 > 0)
			{
				array[num2++] = obj.Exponent[--num3];
			}
			num2 = 20;
			byte[] modulus = obj.Modulus;
			int num4 = modulus.Length;
			Array.Reverse(modulus, 0, num4);
			Buffer.BlockCopy(modulus, 0, array, num2, num4);
			num2 += num4;
			return array;
		}

		public static v.b FromCapiKeyBlob(byte[] blob)
		{
			return FromCapiKeyBlob(blob, 0);
		}

		public static v.b FromCapiKeyBlob(byte[] blob, int offset)
		{
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			if (offset >= blob.Length)
			{
				throw new ArgumentException("blob is too small.");
			}
			switch (blob[offset])
			{
			case 0:
				if (blob[offset + 12] == 6)
				{
					return FromCapiPublicKeyBlob(blob, offset + 12);
				}
				break;
			case 6:
				return FromCapiPublicKeyBlob(blob, offset);
			case 7:
				return FromCapiPrivateKeyBlob(blob, offset);
			}
			throw new v._0006("Unknown blob format.");
		}

		public static byte[] ToCapiKeyBlob(v.h keypair, bool includePrivateKey)
		{
			if (keypair == null)
			{
				throw new ArgumentNullException("keypair");
			}
			if (keypair is v.b)
			{
				return ToCapiKeyBlob((v.b)keypair, includePrivateKey);
			}
			return null;
		}

		public static byte[] ToCapiKeyBlob(v.b rsa, bool includePrivateKey)
		{
			if (rsa == null)
			{
				throw new ArgumentNullException("rsa");
			}
			if (includePrivateKey)
			{
				return ToCapiPrivateKeyBlob(rsa);
			}
			return ToCapiPublicKeyBlob(rsa);
		}

		public static string ToHex(byte[] input)
		{
			if (input == null)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(input.Length * 2);
			foreach (byte b2 in input)
			{
				stringBuilder.Append(b2.ToString("X2", CultureInfo.InvariantCulture));
			}
			return stringBuilder.ToString();
		}

		private static byte H(char P_0)
		{
			if (P_0 >= 'a' && P_0 <= 'f')
			{
				return (byte)(P_0 - 97 + 10);
			}
			if (P_0 >= 'A' && P_0 <= 'F')
			{
				return (byte)(P_0 - 65 + 10);
			}
			if (P_0 >= '0' && P_0 <= '9')
			{
				return (byte)(P_0 - 48);
			}
			throw new ArgumentException("invalid hex char");
		}

		public static byte[] FromHex(string hex)
		{
			if (hex == null)
			{
				return null;
			}
			if ((hex.Length & 1) == 1)
			{
				throw new ArgumentException("Length must be a multiple of 2");
			}
			byte[] array = new byte[hex.Length >> 1];
			int num = 0;
			int num2 = 0;
			while (num < array.Length)
			{
				array[num] = (byte)(H(hex[num2++]) << 4);
				array[num++] += H(hex[num2++]);
			}
			return array;
		}
	}
}
namespace _0018
{
	[Serializable]
	internal class h : IList, X.h, ICollection, IEnumerable
	{
		private sealed class _00065h : IEnumerator, X.h
		{
			private int a5h;

			private int a5b;

			private int a56;

			private object a5a;

			private h a57;

			private int a5_0006;

			public object Current
			{
				get
				{
					if (a5h == a5b - 1)
					{
						throw new InvalidOperationException("Enumerator unusable (Reset pending, or past end of array.");
					}
					return a5a;
				}
			}

			public _00065h(h list)
				: this(list, 0, list.Count)
			{
			}

			public object Clone()
			{
				return MemberwiseClone();
			}

			public _00065h(h list, int index, int count)
			{
				a57 = list;
				a5b = index;
				a56 = count;
				a5h = a5b - 1;
				a5a = null;
				a5_0006 = list.a5a;
			}

			public bool MoveNext()
			{
				if (a57.a5a != a5_0006)
				{
					throw new InvalidOperationException("List has changed.");
				}
				a5h++;
				if (a5h - a5b < a56)
				{
					a5a = a57[a5h];
					return true;
				}
				return false;
			}

			public void Reset()
			{
				a5a = null;
				a5h = a5b - 1;
			}
		}

		private sealed class _00065b : IEnumerator, X.h
		{
			private h a5h;

			private int a5b;

			private int a56;

			private object a5a;

			private static object a57 = new object();

			public object Current
			{
				get
				{
					if (a5a == a57)
					{
						if (a5b == -1)
						{
							throw new InvalidOperationException("Enumerator not started");
						}
						throw new InvalidOperationException("Enumerator ended");
					}
					return a5a;
				}
			}

			public _00065b(h list)
			{
				a5h = list;
				a5b = -1;
				a56 = list.a5a;
				a5a = a57;
			}

			public object Clone()
			{
				return MemberwiseClone();
			}

			public bool MoveNext()
			{
				if (a56 != a5h.a5a)
				{
					throw new InvalidOperationException("List has changed.");
				}
				if (++a5b < a5h.Count)
				{
					a5a = a5h[a5b];
					return true;
				}
				a5a = a57;
				return false;
			}

			public void Reset()
			{
				if (a56 != a5h.a5a)
				{
					throw new InvalidOperationException("List has changed.");
				}
				a5a = a57;
				a5b = -1;
			}
		}

		[Serializable]
		private sealed class _000656 : h
		{
			private new sealed class _00065h : IEnumerator, X.h
			{
				private int a5h;

				private int a5b;

				private int a56;

				private IEnumerator a5a;

				public object Current => a5a.Current;

				public _00065h(IEnumerator enumerator, int index, int count)
				{
					a5b = 0;
					a5h = index;
					a56 = count;
					a5a = enumerator;
					Reset();
				}

				public object Clone()
				{
					return MemberwiseClone();
				}

				public bool MoveNext()
				{
					if (a5b >= a56)
					{
						return false;
					}
					a5b++;
					return a5a.MoveNext();
				}

				public void Reset()
				{
					a5b = 0;
					a5a.Reset();
					for (int i = 0; i < a5h; i++)
					{
						a5a.MoveNext();
					}
				}
			}

			private new IList a5h;

			public override object this[int index]
			{
				get
				{
					return a5h[index];
				}
				set
				{
					a5h[index] = value;
				}
			}

			public override int Count => a5h.Count;

			public override int Capacity
			{
				get
				{
					return a5h.Count;
				}
				set
				{
					if (value < a5h.Count)
					{
						throw new ArgumentException("capacity");
					}
				}
			}

			public override bool IsFixedSize => a5h.IsFixedSize;

			public override bool IsReadOnly => a5h.IsReadOnly;

			public override object SyncRoot => a5h.SyncRoot;

			public override bool IsSynchronized => a5h.IsSynchronized;

			public _000656(IList adaptee)
				: base(0, true)
			{
				a5h = adaptee;
			}

			public override int Add(object value)
			{
				return a5h.Add(value);
			}

			public override void Clear()
			{
				a5h.Clear();
			}

			public override bool Contains(object value)
			{
				return a5h.Contains(value);
			}

			public override int IndexOf(object value)
			{
				return a5h.IndexOf(value);
			}

			public override int IndexOf(object value, int startIndex)
			{
				return IndexOf(value, startIndex, a5h.Count - startIndex);
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				if (startIndex < 0 || startIndex > a5h.Count)
				{
					w("startIndex", startIndex, "Does not specify valid index.");
				}
				if (count < 0)
				{
					w("count", count, "Can't be less than 0.");
				}
				if (startIndex > a5h.Count - count)
				{
					throw new ArgumentOutOfRangeException("count", "Start index and count do not specify a valid range.");
				}
				if (value == null)
				{
					for (int i = startIndex; i < startIndex + count; i++)
					{
						if (a5h[i] == null)
						{
							return i;
						}
					}
				}
				else
				{
					for (int j = startIndex; j < startIndex + count; j++)
					{
						if (value.Equals(a5h[j]))
						{
							return j;
						}
					}
				}
				return -1;
			}

			public override int LastIndexOf(object value)
			{
				return LastIndexOf(value, a5h.Count - 1);
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				return LastIndexOf(value, startIndex, startIndex + 1);
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				if (startIndex < 0)
				{
					w("startIndex", startIndex, "< 0");
				}
				if (count < 0)
				{
					w("count", count, "count is negative.");
				}
				if (startIndex - count + 1 < 0)
				{
					w("count", count, "count is too large.");
				}
				if (value == null)
				{
					for (int num = startIndex; num > startIndex - count; num--)
					{
						if (a5h[num] == null)
						{
							return num;
						}
					}
				}
				else
				{
					for (int num2 = startIndex; num2 > startIndex - count; num2--)
					{
						if (value.Equals(a5h[num2]))
						{
							return num2;
						}
					}
				}
				return -1;
			}

			public override void Insert(int index, object value)
			{
				a5h.Insert(index, value);
			}

			public override void InsertRange(int index, ICollection c)
			{
				if (c == null)
				{
					throw new ArgumentNullException("c");
				}
				if (index > a5h.Count)
				{
					w("index", index, "Index must be >= 0 and <= Count.");
				}
				foreach (object item in c)
				{
					a5h.Insert(index++, item);
				}
			}

			public override void Remove(object value)
			{
				a5h.Remove(value);
			}

			public override void RemoveAt(int index)
			{
				a5h.RemoveAt(index);
			}

			public override void RemoveRange(int index, int count)
			{
				_0011(index, count, a5h.Count);
				for (int i = 0; i < count; i++)
				{
					a5h.RemoveAt(index);
				}
			}

			public override void Reverse()
			{
				Reverse(0, a5h.Count);
			}

			public override void Reverse(int index, int count)
			{
				_0011(index, count, a5h.Count);
				for (int i = 0; i < count / 2; i++)
				{
					object value = a5h[i + index];
					a5h[i + index] = a5h[index + count - i + index - 1];
					a5h[index + count - i + index - 1] = value;
				}
			}

			public override void SetRange(int index, ICollection c)
			{
				if (c == null)
				{
					throw new ArgumentNullException("c");
				}
				if (index < 0 || index + c.Count > a5h.Count)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				int num = index;
				foreach (object item in c)
				{
					a5h[num++] = item;
				}
			}

			public override void CopyTo(Array array)
			{
				a5h.CopyTo(array, 0);
			}

			public override void CopyTo(Array array, int index)
			{
				a5h.CopyTo(array, index);
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				if (index < 0)
				{
					w("index", index, "Can't be less than zero.");
				}
				if (arrayIndex < 0)
				{
					w("arrayIndex", arrayIndex, "Can't be less than zero.");
				}
				if (count < 0)
				{
					w("index", index, "Can't be less than zero.");
				}
				if (index >= a5h.Count)
				{
					throw new ArgumentException("Can't be more or equal to list count.", "index");
				}
				if (array.Rank > 1)
				{
					throw new ArgumentException("Can't copy into multi-dimensional array.");
				}
				if (arrayIndex >= array.Length)
				{
					throw new ArgumentException("arrayIndex can't be greater than array.Length - 1.");
				}
				if (array.Length - arrayIndex + 1 < count)
				{
					throw new ArgumentException("Destination array is too small.");
				}
				if (index > a5h.Count - count)
				{
					throw new ArgumentException("Index and count do not denote a valid range of elements.", "index");
				}
				for (int i = 0; i < count; i++)
				{
					array.SetValue(a5h[index + i], arrayIndex + i);
				}
			}

			public override IEnumerator GetEnumerator()
			{
				return a5h.GetEnumerator();
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				_0011(index, count, a5h.Count);
				return new _00065h(a5h.GetEnumerator(), index, count);
			}

			public override void AddRange(ICollection c)
			{
				foreach (object item in c)
				{
					a5h.Add(item);
				}
			}

			public override int BinarySearch(object value)
			{
				return BinarySearch(value, null);
			}

			public override int BinarySearch(object value, IComparer comparer)
			{
				return BinarySearch(0, a5h.Count, value, comparer);
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				_0011(index, count, a5h.Count);
				if (comparer == null)
				{
					comparer = b.Default;
				}
				int num = index;
				int num2 = index + count - 1;
				while (num <= num2)
				{
					int num3 = num + (num2 - num) / 2;
					int num4 = comparer.Compare(value, a5h[num3]);
					if (num4 < 0)
					{
						num2 = num3 - 1;
						continue;
					}
					if (num4 > 0)
					{
						num = num3 + 1;
						continue;
					}
					return num3;
				}
				return ~num;
			}

			public override object Clone()
			{
				return new _000656(a5h);
			}

			public override h GetRange(int index, int count)
			{
				_0011(index, count, a5h.Count);
				return new _00065B(this, index, count);
			}

			public override void TrimToSize()
			{
			}

			public override void Sort()
			{
				Sort(b.Default);
			}

			public override void Sort(IComparer comparer)
			{
				Sort(0, a5h.Count, comparer);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				_0011(index, count, a5h.Count);
				if (comparer == null)
				{
					comparer = b.Default;
				}
				R(a5h, index, index + count - 1, comparer);
			}

			private static void S(IList P_0, int P_1, int P_2)
			{
				object value = P_0[P_1];
				P_0[P_1] = P_0[P_2];
				P_0[P_2] = value;
			}

			internal static void R(IList P_0, int P_1, int P_2, IComparer P_3)
			{
				if (P_1 >= P_2)
				{
					return;
				}
				int num = P_1 + (P_2 - P_1) / 2;
				if (P_3.Compare(P_0[num], P_0[P_1]) < 0)
				{
					S(P_0, num, P_1);
				}
				if (P_3.Compare(P_0[P_2], P_0[P_1]) < 0)
				{
					S(P_0, P_2, P_1);
				}
				if (P_3.Compare(P_0[P_2], P_0[num]) < 0)
				{
					S(P_0, P_2, num);
				}
				if (P_2 - P_1 + 1 <= 3)
				{
					return;
				}
				S(P_0, P_2 - 1, num);
				object obj = P_0[P_2 - 1];
				int num2 = P_1;
				int num3 = P_2 - 1;
				while (true)
				{
					if (P_3.Compare(P_0[++num2], obj) >= 0)
					{
						while (P_3.Compare(P_0[--num3], obj) > 0)
						{
						}
						if (num2 >= num3)
						{
							break;
						}
						S(P_0, num2, num3);
					}
				}
				S(P_0, P_2 - 1, num2);
				R(P_0, P_1, num2 - 1, P_3);
				R(P_0, num2 + 1, P_2, P_3);
			}

			public override object[] ToArray()
			{
				object[] array = new object[a5h.Count];
				a5h.CopyTo(array, 0);
				return array;
			}

			public override Array ToArray(Type elementType)
			{
				Array array = Array.CreateInstance(elementType, a5h.Count);
				a5h.CopyTo(array, 0);
				return array;
			}
		}

		[Serializable]
		private class _00065a : h
		{
			protected h m_InnerArrayList;

			public override object this[int index]
			{
				get
				{
					return m_InnerArrayList[index];
				}
				set
				{
					m_InnerArrayList[index] = value;
				}
			}

			public override int Count => m_InnerArrayList.Count;

			public override int Capacity
			{
				get
				{
					return m_InnerArrayList.Capacity;
				}
				set
				{
					m_InnerArrayList.Capacity = value;
				}
			}

			public override bool IsFixedSize => m_InnerArrayList.IsFixedSize;

			public override bool IsReadOnly => m_InnerArrayList.IsReadOnly;

			public override bool IsSynchronized => m_InnerArrayList.IsSynchronized;

			public override object SyncRoot => m_InnerArrayList.SyncRoot;

			public _00065a(h innerArrayList)
			{
				m_InnerArrayList = innerArrayList;
			}

			public override int Add(object value)
			{
				return m_InnerArrayList.Add(value);
			}

			public override void Clear()
			{
				m_InnerArrayList.Clear();
			}

			public override bool Contains(object value)
			{
				return m_InnerArrayList.Contains(value);
			}

			public override int IndexOf(object value)
			{
				return m_InnerArrayList.IndexOf(value);
			}

			public override int IndexOf(object value, int startIndex)
			{
				return m_InnerArrayList.IndexOf(value, startIndex);
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				return m_InnerArrayList.IndexOf(value, startIndex, count);
			}

			public override int LastIndexOf(object value)
			{
				return m_InnerArrayList.LastIndexOf(value);
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				return m_InnerArrayList.LastIndexOf(value, startIndex);
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				return m_InnerArrayList.LastIndexOf(value, startIndex, count);
			}

			public override void Insert(int index, object value)
			{
				m_InnerArrayList.Insert(index, value);
			}

			public override void InsertRange(int index, ICollection c)
			{
				m_InnerArrayList.InsertRange(index, c);
			}

			public override void Remove(object value)
			{
				m_InnerArrayList.Remove(value);
			}

			public override void RemoveAt(int index)
			{
				m_InnerArrayList.RemoveAt(index);
			}

			public override void RemoveRange(int index, int count)
			{
				m_InnerArrayList.RemoveRange(index, count);
			}

			public override void Reverse()
			{
				m_InnerArrayList.Reverse();
			}

			public override void Reverse(int index, int count)
			{
				m_InnerArrayList.Reverse(index, count);
			}

			public override void SetRange(int index, ICollection c)
			{
				m_InnerArrayList.SetRange(index, c);
			}

			public override void CopyTo(Array array)
			{
				m_InnerArrayList.CopyTo(array);
			}

			public override void CopyTo(Array array, int index)
			{
				m_InnerArrayList.CopyTo(array, index);
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				m_InnerArrayList.CopyTo(index, array, arrayIndex, count);
			}

			public override IEnumerator GetEnumerator()
			{
				return m_InnerArrayList.GetEnumerator();
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				return m_InnerArrayList.GetEnumerator(index, count);
			}

			public override void AddRange(ICollection c)
			{
				m_InnerArrayList.AddRange(c);
			}

			public override int BinarySearch(object value)
			{
				return m_InnerArrayList.BinarySearch(value);
			}

			public override int BinarySearch(object value, IComparer comparer)
			{
				return m_InnerArrayList.BinarySearch(value, comparer);
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				return m_InnerArrayList.BinarySearch(index, count, value, comparer);
			}

			public override object Clone()
			{
				return m_InnerArrayList.Clone();
			}

			public override h GetRange(int index, int count)
			{
				return m_InnerArrayList.GetRange(index, count);
			}

			public override void TrimToSize()
			{
				m_InnerArrayList.TrimToSize();
			}

			public override void Sort()
			{
				m_InnerArrayList.Sort();
			}

			public override void Sort(IComparer comparer)
			{
				m_InnerArrayList.Sort(comparer);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				m_InnerArrayList.Sort(index, count, comparer);
			}

			public override object[] ToArray()
			{
				return m_InnerArrayList.ToArray();
			}

			public override Array ToArray(Type elementType)
			{
				return m_InnerArrayList.ToArray(elementType);
			}
		}

		[Serializable]
		private sealed class _000657 : _00065a
		{
			private new object a5h;

			public override object this[int index]
			{
				get
				{
					lock (a5h)
					{
						return m_InnerArrayList[index];
					}
				}
				set
				{
					lock (a5h)
					{
						m_InnerArrayList[index] = value;
					}
				}
			}

			public override int Count
			{
				get
				{
					lock (a5h)
					{
						return m_InnerArrayList.Count;
					}
				}
			}

			public override int Capacity
			{
				get
				{
					lock (a5h)
					{
						return m_InnerArrayList.Capacity;
					}
				}
				set
				{
					lock (a5h)
					{
						m_InnerArrayList.Capacity = value;
					}
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					lock (a5h)
					{
						return m_InnerArrayList.IsFixedSize;
					}
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					lock (a5h)
					{
						return m_InnerArrayList.IsReadOnly;
					}
				}
			}

			public override bool IsSynchronized => true;

			public override object SyncRoot => a5h;

			internal _000657(h P_0)
				: base(P_0)
			{
				a5h = P_0.SyncRoot;
			}

			public override int Add(object value)
			{
				lock (a5h)
				{
					return m_InnerArrayList.Add(value);
				}
			}

			public override void Clear()
			{
				lock (a5h)
				{
					m_InnerArrayList.Clear();
				}
			}

			public override bool Contains(object value)
			{
				lock (a5h)
				{
					return m_InnerArrayList.Contains(value);
				}
			}

			public override int IndexOf(object value)
			{
				lock (a5h)
				{
					return m_InnerArrayList.IndexOf(value);
				}
			}

			public override int IndexOf(object value, int startIndex)
			{
				lock (a5h)
				{
					return m_InnerArrayList.IndexOf(value, startIndex);
				}
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				lock (a5h)
				{
					return m_InnerArrayList.IndexOf(value, startIndex, count);
				}
			}

			public override int LastIndexOf(object value)
			{
				lock (a5h)
				{
					return m_InnerArrayList.LastIndexOf(value);
				}
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				lock (a5h)
				{
					return m_InnerArrayList.LastIndexOf(value, startIndex);
				}
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				lock (a5h)
				{
					return m_InnerArrayList.LastIndexOf(value, startIndex, count);
				}
			}

			public override void Insert(int index, object value)
			{
				lock (a5h)
				{
					m_InnerArrayList.Insert(index, value);
				}
			}

			public override void InsertRange(int index, ICollection c)
			{
				lock (a5h)
				{
					m_InnerArrayList.InsertRange(index, c);
				}
			}

			public override void Remove(object value)
			{
				lock (a5h)
				{
					m_InnerArrayList.Remove(value);
				}
			}

			public override void RemoveAt(int index)
			{
				lock (a5h)
				{
					m_InnerArrayList.RemoveAt(index);
				}
			}

			public override void RemoveRange(int index, int count)
			{
				lock (a5h)
				{
					m_InnerArrayList.RemoveRange(index, count);
				}
			}

			public override void Reverse()
			{
				lock (a5h)
				{
					m_InnerArrayList.Reverse();
				}
			}

			public override void Reverse(int index, int count)
			{
				lock (a5h)
				{
					m_InnerArrayList.Reverse(index, count);
				}
			}

			public override void CopyTo(Array array)
			{
				lock (a5h)
				{
					m_InnerArrayList.CopyTo(array);
				}
			}

			public override void CopyTo(Array array, int index)
			{
				lock (a5h)
				{
					m_InnerArrayList.CopyTo(array, index);
				}
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				lock (a5h)
				{
					m_InnerArrayList.CopyTo(index, array, arrayIndex, count);
				}
			}

			public override IEnumerator GetEnumerator()
			{
				lock (a5h)
				{
					return m_InnerArrayList.GetEnumerator();
				}
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				lock (a5h)
				{
					return m_InnerArrayList.GetEnumerator(index, count);
				}
			}

			public override void AddRange(ICollection c)
			{
				lock (a5h)
				{
					m_InnerArrayList.AddRange(c);
				}
			}

			public override int BinarySearch(object value)
			{
				lock (a5h)
				{
					return m_InnerArrayList.BinarySearch(value);
				}
			}

			public override int BinarySearch(object value, IComparer comparer)
			{
				lock (a5h)
				{
					return m_InnerArrayList.BinarySearch(value, comparer);
				}
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				lock (a5h)
				{
					return m_InnerArrayList.BinarySearch(index, count, value, comparer);
				}
			}

			public override object Clone()
			{
				lock (a5h)
				{
					return m_InnerArrayList.Clone();
				}
			}

			public override h GetRange(int index, int count)
			{
				lock (a5h)
				{
					return m_InnerArrayList.GetRange(index, count);
				}
			}

			public override void TrimToSize()
			{
				lock (a5h)
				{
					m_InnerArrayList.TrimToSize();
				}
			}

			public override void Sort()
			{
				lock (a5h)
				{
					m_InnerArrayList.Sort();
				}
			}

			public override void Sort(IComparer comparer)
			{
				lock (a5h)
				{
					m_InnerArrayList.Sort(comparer);
				}
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				lock (a5h)
				{
					m_InnerArrayList.Sort(index, count, comparer);
				}
			}

			public override object[] ToArray()
			{
				lock (a5h)
				{
					return m_InnerArrayList.ToArray();
				}
			}

			public override Array ToArray(Type elementType)
			{
				lock (a5h)
				{
					return m_InnerArrayList.ToArray(elementType);
				}
			}
		}

		[Serializable]
		private class _00065_0006 : _00065a
		{
			protected virtual string ErrorMessage => "Can't add or remove from a fixed-size list.";

			public override int Capacity
			{
				get
				{
					return base.Capacity;
				}
				set
				{
					throw new NotSupportedException(ErrorMessage);
				}
			}

			public override bool IsFixedSize => true;

			public _00065_0006(h innerList)
				: base(innerList)
			{
			}

			public override int Add(object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void AddRange(ICollection c)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Clear()
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Insert(int index, object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void InsertRange(int index, ICollection c)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Remove(object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void RemoveAt(int index)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void RemoveRange(int index, int count)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void TrimToSize()
			{
				throw new NotSupportedException(ErrorMessage);
			}
		}

		[Serializable]
		private sealed class _00065v : _00065_0006
		{
			protected override string ErrorMessage => "Can't modify a readonly list.";

			public override bool IsReadOnly => true;

			public override object this[int index]
			{
				get
				{
					return m_InnerArrayList[index];
				}
				set
				{
					throw new NotSupportedException(ErrorMessage);
				}
			}

			public _00065v(h innerArrayList)
				: base(innerArrayList)
			{
			}

			public override void Reverse()
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Reverse(int index, int count)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void SetRange(int index, ICollection c)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Sort()
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Sort(IComparer comparer)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				throw new NotSupportedException(ErrorMessage);
			}
		}

		[Serializable]
		private sealed class _00065B : _00065a
		{
			private new int a5h;

			private new int a5b;

			private new int a56;

			public override bool IsSynchronized => false;

			public override object this[int index]
			{
				get
				{
					if (index < 0 || index > a5b)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					return m_InnerArrayList[a5h + index];
				}
				set
				{
					if (index < 0 || index > a5b)
					{
						throw new ArgumentOutOfRangeException("index");
					}
					m_InnerArrayList[a5h + index] = value;
				}
			}

			public override int Count
			{
				get
				{
					_0003();
					return a5b;
				}
			}

			public override int Capacity
			{
				get
				{
					return m_InnerArrayList.Capacity;
				}
				set
				{
					if (value < a5b)
					{
						throw new ArgumentOutOfRangeException();
					}
				}
			}

			public _00065B(h innerList, int index, int count)
				: base(innerList)
			{
				a5h = index;
				a5b = count;
				a56 = innerList.a5a;
			}

			private void _0003()
			{
				if (a56 != m_InnerArrayList.a5a)
				{
					throw new InvalidOperationException("ArrayList view is invalid because the underlying ArrayList was modified.");
				}
			}

			public override int Add(object value)
			{
				_0003();
				m_InnerArrayList.Insert(a5h + a5b, value);
				a56 = m_InnerArrayList.a5a;
				return ++a5b;
			}

			public override void Clear()
			{
				_0003();
				m_InnerArrayList.RemoveRange(a5h, a5b);
				a5b = 0;
				a56 = m_InnerArrayList.a5a;
			}

			public override bool Contains(object value)
			{
				return m_InnerArrayList.f(value, a5h, a5b);
			}

			public override int IndexOf(object value)
			{
				return IndexOf(value, 0);
			}

			public override int IndexOf(object value, int startIndex)
			{
				return IndexOf(value, startIndex, a5b - startIndex);
			}

			public override int IndexOf(object value, int startIndex, int count)
			{
				if (startIndex < 0 || startIndex > a5b)
				{
					w("startIndex", startIndex, "Does not specify valid index.");
				}
				if (count < 0)
				{
					w("count", count, "Can't be less than 0.");
				}
				if (startIndex > a5b - count)
				{
					throw new ArgumentOutOfRangeException("count", "Start index and count do not specify a valid range.");
				}
				int num = m_InnerArrayList.IndexOf(value, a5h + startIndex, count);
				if (num == -1)
				{
					return -1;
				}
				return num - a5h;
			}

			public override int LastIndexOf(object value)
			{
				return LastIndexOf(value, a5b - 1);
			}

			public override int LastIndexOf(object value, int startIndex)
			{
				return LastIndexOf(value, startIndex, startIndex + 1);
			}

			public override int LastIndexOf(object value, int startIndex, int count)
			{
				if (startIndex < 0)
				{
					w("startIndex", startIndex, "< 0");
				}
				if (count < 0)
				{
					w("count", count, "count is negative.");
				}
				int num = m_InnerArrayList.LastIndexOf(value, a5h + startIndex, count);
				if (num == -1)
				{
					return -1;
				}
				return num - a5h;
			}

			public override void Insert(int index, object value)
			{
				_0003();
				if (index < 0 || index > a5b)
				{
					w("index", index, "Index must be >= 0 and <= Count.");
				}
				m_InnerArrayList.Insert(a5h + index, value);
				a5b++;
				a56 = m_InnerArrayList.a5a;
			}

			public override void InsertRange(int index, ICollection c)
			{
				_0003();
				if (index < 0 || index > a5b)
				{
					w("index", index, "Index must be >= 0 and <= Count.");
				}
				m_InnerArrayList.InsertRange(a5h + index, c);
				a5b += c.Count;
				a56 = m_InnerArrayList.a5a;
			}

			public override void Remove(object value)
			{
				_0003();
				int num = IndexOf(value);
				if (num > -1)
				{
					RemoveAt(num);
				}
				a56 = m_InnerArrayList.a5a;
			}

			public override void RemoveAt(int index)
			{
				_0003();
				if (index < 0 || index > a5b)
				{
					w("index", index, "Index must be >= 0 and <= Count.");
				}
				m_InnerArrayList.RemoveAt(a5h + index);
				a5b--;
				a56 = m_InnerArrayList.a5a;
			}

			public override void RemoveRange(int index, int count)
			{
				_0003();
				_0011(index, count, a5b);
				m_InnerArrayList.RemoveRange(a5h + index, count);
				a5b -= count;
				a56 = m_InnerArrayList.a5a;
			}

			public override void Reverse()
			{
				Reverse(0, a5b);
			}

			public override void Reverse(int index, int count)
			{
				_0003();
				_0011(index, count, a5b);
				m_InnerArrayList.Reverse(a5h + index, count);
				a56 = m_InnerArrayList.a5a;
			}

			public override void SetRange(int index, ICollection c)
			{
				_0003();
				if (index < 0 || index > a5b)
				{
					w("index", index, "Index must be >= 0 and <= Count.");
				}
				m_InnerArrayList.SetRange(a5h + index, c);
				a56 = m_InnerArrayList.a5a;
			}

			public override void CopyTo(Array array)
			{
				CopyTo(array, 0);
			}

			public override void CopyTo(Array array, int index)
			{
				CopyTo(0, array, index, a5b);
			}

			public override void CopyTo(int index, Array array, int arrayIndex, int count)
			{
				_0011(index, count, a5b);
				m_InnerArrayList.CopyTo(a5h + index, array, arrayIndex, count);
			}

			public override IEnumerator GetEnumerator()
			{
				return GetEnumerator(0, a5b);
			}

			public override IEnumerator GetEnumerator(int index, int count)
			{
				_0011(index, count, a5b);
				return m_InnerArrayList.GetEnumerator(a5h + index, count);
			}

			public override void AddRange(ICollection c)
			{
				_0003();
				m_InnerArrayList.InsertRange(a5b, c);
				a5b += c.Count;
				a56 = m_InnerArrayList.a5a;
			}

			public override int BinarySearch(object value)
			{
				return BinarySearch(0, a5b, value, b.Default);
			}

			public override int BinarySearch(object value, IComparer comparer)
			{
				return BinarySearch(0, a5b, value, comparer);
			}

			public override int BinarySearch(int index, int count, object value, IComparer comparer)
			{
				_0011(index, count, a5b);
				return m_InnerArrayList.BinarySearch(a5h + index, count, value, comparer);
			}

			public override object Clone()
			{
				return new _00065B((h)m_InnerArrayList.Clone(), a5h, a5b);
			}

			public override h GetRange(int index, int count)
			{
				_0011(index, count, a5b);
				return new _00065B(this, index, count);
			}

			public override void TrimToSize()
			{
				throw new NotSupportedException();
			}

			public override void Sort()
			{
				Sort(b.Default);
			}

			public override void Sort(IComparer comparer)
			{
				Sort(0, a5b, comparer);
			}

			public override void Sort(int index, int count, IComparer comparer)
			{
				_0003();
				_0011(index, count, a5b);
				m_InnerArrayList.Sort(a5h + index, count, comparer);
				a56 = m_InnerArrayList.a5a;
			}

			public override object[] ToArray()
			{
				object[] array = new object[a5b];
				m_InnerArrayList.CopyTo(a5h, array, 0, a5b);
				return array;
			}

			public override Array ToArray(Type elementType)
			{
				Array array = Array.CreateInstance(elementType, a5b);
				m_InnerArrayList.CopyTo(a5h, array, 0, a5b);
				return array;
			}
		}

		[Serializable]
		private class _00065X : IList, ICollection, IEnumerable
		{
			protected IList m_InnerList;

			public virtual object this[int index]
			{
				get
				{
					return m_InnerList[index];
				}
				set
				{
					m_InnerList[index] = value;
				}
			}

			public virtual int Count => m_InnerList.Count;

			public virtual bool IsSynchronized => m_InnerList.IsSynchronized;

			public virtual object SyncRoot => m_InnerList.SyncRoot;

			public virtual bool IsFixedSize => m_InnerList.IsFixedSize;

			public virtual bool IsReadOnly => m_InnerList.IsReadOnly;

			public _00065X(IList innerList)
			{
				m_InnerList = innerList;
			}

			public virtual int Add(object value)
			{
				return m_InnerList.Add(value);
			}

			public virtual void Clear()
			{
				m_InnerList.Clear();
			}

			public virtual bool Contains(object value)
			{
				return m_InnerList.Contains(value);
			}

			public virtual int IndexOf(object value)
			{
				return m_InnerList.IndexOf(value);
			}

			public virtual void Insert(int index, object value)
			{
				m_InnerList.Insert(index, value);
			}

			public virtual void Remove(object value)
			{
				m_InnerList.Remove(value);
			}

			public virtual void RemoveAt(int index)
			{
				m_InnerList.RemoveAt(index);
			}

			public virtual void CopyTo(Array array, int index)
			{
				m_InnerList.CopyTo(array, index);
			}

			public virtual IEnumerator GetEnumerator()
			{
				return m_InnerList.GetEnumerator();
			}
		}

		[Serializable]
		private sealed class _00065_0018 : _00065X
		{
			private object a5h;

			public override int Count
			{
				get
				{
					lock (a5h)
					{
						return m_InnerList.Count;
					}
				}
			}

			public override bool IsSynchronized => true;

			public override object SyncRoot
			{
				get
				{
					lock (a5h)
					{
						return m_InnerList.SyncRoot;
					}
				}
			}

			public override bool IsFixedSize
			{
				get
				{
					lock (a5h)
					{
						return m_InnerList.IsFixedSize;
					}
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					lock (a5h)
					{
						return m_InnerList.IsReadOnly;
					}
				}
			}

			public override object this[int index]
			{
				get
				{
					lock (a5h)
					{
						return m_InnerList[index];
					}
				}
				set
				{
					lock (a5h)
					{
						m_InnerList[index] = value;
					}
				}
			}

			public _00065_0018(IList innerList)
				: base(innerList)
			{
				a5h = innerList.SyncRoot;
			}

			public override int Add(object value)
			{
				lock (a5h)
				{
					return m_InnerList.Add(value);
				}
			}

			public override void Clear()
			{
				lock (a5h)
				{
					m_InnerList.Clear();
				}
			}

			public override bool Contains(object value)
			{
				lock (a5h)
				{
					return m_InnerList.Contains(value);
				}
			}

			public override int IndexOf(object value)
			{
				lock (a5h)
				{
					return m_InnerList.IndexOf(value);
				}
			}

			public override void Insert(int index, object value)
			{
				lock (a5h)
				{
					m_InnerList.Insert(index, value);
				}
			}

			public override void Remove(object value)
			{
				lock (a5h)
				{
					m_InnerList.Remove(value);
				}
			}

			public override void RemoveAt(int index)
			{
				lock (a5h)
				{
					m_InnerList.RemoveAt(index);
				}
			}

			public override void CopyTo(Array array, int index)
			{
				lock (a5h)
				{
					m_InnerList.CopyTo(array, index);
				}
			}

			public override IEnumerator GetEnumerator()
			{
				lock (a5h)
				{
					return m_InnerList.GetEnumerator();
				}
			}
		}

		[Serializable]
		private class _00065W : _00065X
		{
			protected virtual string ErrorMessage => "List is fixed-size.";

			public override bool IsFixedSize => true;

			public _00065W(IList innerList)
				: base(innerList)
			{
			}

			public override int Add(object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Clear()
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Insert(int index, object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void Remove(object value)
			{
				throw new NotSupportedException(ErrorMessage);
			}

			public override void RemoveAt(int index)
			{
				throw new NotSupportedException(ErrorMessage);
			}
		}

		[Serializable]
		private sealed class _00065_0002 : _00065W
		{
			protected override string ErrorMessage => "List is read-only.";

			public override bool IsReadOnly => true;

			public override object this[int index]
			{
				get
				{
					return m_InnerList[index];
				}
				set
				{
					throw new NotSupportedException(ErrorMessage);
				}
			}

			public _00065_0002(IList innerList)
				: base(innerList)
			{
			}
		}

		private const int a5h = 16;

		private int a5b;

		private object[] a56;

		private int a5a;

		public virtual object this[int index]
		{
			get
			{
				if (index < 0 || index >= a5b)
				{
					w("index", index, "Index is less than 0 or more than or equal to the list count.");
				}
				return a56[index];
			}
			set
			{
				if (index < 0 || index >= a5b)
				{
					w("index", index, "Index is less than 0 or more than or equal to the list count.");
				}
				a56[index] = value;
				a5a++;
			}
		}

		public virtual int Count => a5b;

		public virtual int Capacity
		{
			get
			{
				return a56.Length;
			}
			set
			{
				if (value < a5b)
				{
					w("Capacity", value, "Must be more than count.");
				}
				object[] destinationArray = new object[value];
				Array.Copy(a56, 0, destinationArray, 0, a5b);
				a56 = destinationArray;
			}
		}

		public virtual bool IsFixedSize => false;

		public virtual bool IsReadOnly => false;

		public virtual bool IsSynchronized => false;

		public virtual object SyncRoot => this;

		public h()
		{
			a56 = new object[16];
		}

		public h(ICollection c)
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			if (c is Array { Rank: not 1 })
			{
				throw new RankException();
			}
			a56 = new object[c.Count];
			AddRange(c);
		}

		public h(int capacity)
		{
			if (capacity < 0)
			{
				w("capacity", capacity, "The initial capacity can't be smaller than zero.");
			}
			if (capacity == 0)
			{
				capacity = 16;
			}
			a56 = new object[capacity];
		}

		private h(int P_0, bool P_1)
		{
			if (P_1)
			{
				a56 = null;
				return;
			}
			throw new InvalidOperationException("Use ArrayList(int)");
		}

		private h(object[] P_0, int P_1, int P_2)
		{
			if (P_2 == 0)
			{
				a56 = new object[16];
			}
			else
			{
				a56 = new object[P_2];
			}
			Array.Copy(P_0, P_1, a56, 0, P_2);
			a5b = P_2;
		}

		private void _9(int P_0)
		{
			if (P_0 > a56.Length)
			{
				int num = a56.Length << 1;
				if (num == 0)
				{
					num = 16;
				}
				while (num < P_0)
				{
					num <<= 1;
				}
				object[] destinationArray = new object[num];
				Array.Copy(a56, 0, destinationArray, 0, a56.Length);
				a56 = destinationArray;
			}
		}

		private void _1(int P_0, int P_1)
		{
			if (P_1 > 0)
			{
				if (a5b + P_1 > a56.Length)
				{
					int num;
					for (num = ((a56.Length <= 0) ? 1 : (a56.Length << 1)); num < a5b + P_1; num <<= 1)
					{
					}
					object[] destinationArray = new object[num];
					Array.Copy(a56, 0, destinationArray, 0, P_0);
					Array.Copy(a56, P_0, destinationArray, P_0 + P_1, a5b - P_0);
					a56 = destinationArray;
				}
				else
				{
					Array.Copy(a56, P_0, a56, P_0 + P_1, a5b - P_0);
				}
			}
			else if (P_1 < 0)
			{
				int num2 = P_0 - P_1;
				Array.Copy(a56, num2, a56, P_0, a5b - num2);
				Array.Clear(a56, a5b + P_1, -P_1);
			}
		}

		public virtual int Add(object value)
		{
			if (a56.Length <= a5b)
			{
				_9(a5b + 1);
			}
			a56[a5b] = value;
			a5a++;
			return a5b++;
		}

		public virtual void Clear()
		{
			Array.Clear(a56, 0, a5b);
			a5b = 0;
			a5a++;
		}

		public virtual bool Contains(object item)
		{
			return IndexOf(item, 0, a5b) > -1;
		}

		internal virtual bool f(object P_0, int P_1, int P_2)
		{
			return IndexOf(P_0, P_1, P_2) > -1;
		}

		public virtual int IndexOf(object value)
		{
			return IndexOf(value, 0);
		}

		public virtual int IndexOf(object value, int startIndex)
		{
			return IndexOf(value, startIndex, a5b - startIndex);
		}

		public virtual int IndexOf(object value, int startIndex, int count)
		{
			if (startIndex < 0 || startIndex > a5b)
			{
				w("startIndex", startIndex, "Does not specify valid index.");
			}
			if (count < 0)
			{
				w("count", count, "Can't be less than 0.");
			}
			if (startIndex > a5b - count)
			{
				throw new ArgumentOutOfRangeException("count", "Start index and count do not specify a valid range.");
			}
			return Array.IndexOf(a56, value, startIndex, count);
		}

		public virtual int LastIndexOf(object value)
		{
			return LastIndexOf(value, a5b - 1);
		}

		public virtual int LastIndexOf(object value, int startIndex)
		{
			return LastIndexOf(value, startIndex, startIndex + 1);
		}

		public virtual int LastIndexOf(object value, int startIndex, int count)
		{
			return Array.LastIndexOf(a56, value, startIndex, count);
		}

		public virtual void Insert(int index, object value)
		{
			if (index < 0 || index > a5b)
			{
				w("index", index, "Index must be >= 0 and <= Count.");
			}
			_1(index, 1);
			a56[index] = value;
			a5b++;
			a5a++;
		}

		public virtual void InsertRange(int index, ICollection c)
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			if (index < 0 || index > a5b)
			{
				w("index", index, "Index must be >= 0 and <= Count.");
			}
			int count = c.Count;
			if (a56.Length < a5b + count)
			{
				_9(a5b + count);
			}
			if (index < a5b)
			{
				Array.Copy(a56, index, a56, index + count, a5b - index);
			}
			if (this == c.SyncRoot)
			{
				Array.Copy(a56, 0, a56, index, index);
				Array.Copy(a56, index + count, a56, index << 1, a5b - index);
			}
			else
			{
				c.CopyTo(a56, index);
			}
			a5b += c.Count;
			a5a++;
		}

		public virtual void Remove(object obj)
		{
			int num = IndexOf(obj);
			if (num > -1)
			{
				RemoveAt(num);
			}
			a5a++;
		}

		public virtual void RemoveAt(int index)
		{
			if (index < 0 || index >= a5b)
			{
				w("index", index, "Less than 0 or more than list count.");
			}
			_1(index, -1);
			a5b--;
			a5a++;
		}

		public virtual void RemoveRange(int index, int count)
		{
			_0011(index, count, a5b);
			_1(index, -count);
			a5b -= count;
			a5a++;
		}

		public virtual void Reverse()
		{
			Array.Reverse(a56, 0, a5b);
			a5a++;
		}

		public virtual void Reverse(int index, int count)
		{
			_0011(index, count, a5b);
			Array.Reverse(a56, index, count);
			a5a++;
		}

		public virtual void CopyTo(Array array)
		{
			Array.Copy(a56, array, a5b);
		}

		public virtual void CopyTo(Array array, int arrayIndex)
		{
			CopyTo(0, array, arrayIndex, a5b);
		}

		public virtual void CopyTo(int index, Array array, int arrayIndex, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("Must have only 1 dimensions.", "array");
			}
			Array.Copy(a56, index, array, arrayIndex, count);
		}

		public virtual IEnumerator GetEnumerator()
		{
			return new _00065b(this);
		}

		public virtual IEnumerator GetEnumerator(int index, int count)
		{
			_0011(index, count, a5b);
			return new _00065h(this, index, count);
		}

		public virtual void AddRange(ICollection c)
		{
			InsertRange(a5b, c);
		}

		public virtual int BinarySearch(object value)
		{
			try
			{
				return Array.BinarySearch(a56, 0, a5b, value);
			}
			catch (InvalidOperationException ex)
			{
				throw new ArgumentException(ex.Message);
			}
		}

		public virtual int BinarySearch(object value, IComparer comparer)
		{
			try
			{
				return Array.BinarySearch(a56, 0, a5b, value, comparer);
			}
			catch (InvalidOperationException ex)
			{
				throw new ArgumentException(ex.Message);
			}
		}

		public virtual int BinarySearch(int index, int count, object value, IComparer comparer)
		{
			try
			{
				return Array.BinarySearch(a56, index, count, value, comparer);
			}
			catch (InvalidOperationException ex)
			{
				throw new ArgumentException(ex.Message);
			}
		}

		public virtual h GetRange(int index, int count)
		{
			_0011(index, count, a5b);
			if (IsSynchronized)
			{
				return Synchronized(new _00065B(this, index, count));
			}
			return new _00065B(this, index, count);
		}

		public virtual void SetRange(int index, ICollection c)
		{
			if (c == null)
			{
				throw new ArgumentNullException("c");
			}
			if (index < 0 || index + c.Count > a5b)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			c.CopyTo(a56, index);
			a5a++;
		}

		public virtual void TrimToSize()
		{
			if (a56.Length > a5b)
			{
				object[] destinationArray = ((a5b != 0) ? new object[a5b] : new object[16]);
				Array.Copy(a56, 0, destinationArray, 0, a5b);
				a56 = destinationArray;
			}
		}

		public virtual void Sort()
		{
			Array.Sort(a56, 0, a5b);
			a5a++;
		}

		public virtual void Sort(IComparer comparer)
		{
			Array.Sort(a56, 0, a5b, comparer);
		}

		public virtual void Sort(int index, int count, IComparer comparer)
		{
			_0011(index, count, a5b);
			Array.Sort(a56, index, count, comparer);
		}

		public virtual object[] ToArray()
		{
			object[] array = new object[a5b];
			CopyTo(array);
			return array;
		}

		public virtual Array ToArray(Type type)
		{
			Array array = Array.CreateInstance(type, a5b);
			CopyTo(array);
			return array;
		}

		public virtual object Clone()
		{
			return new h(a56, 0, a5b);
		}

		internal static void _0011(int P_0, int P_1, int P_2)
		{
			if (P_0 < 0)
			{
				w("index", P_0, "Can't be less than 0.");
			}
			if (P_1 < 0)
			{
				w("count", P_1, "Can't be less than 0.");
			}
			if (P_0 > P_2 - P_1)
			{
				throw new ArgumentException("Index and count do not denote a valid range of elements.", "index");
			}
		}

		internal static void w(string P_0, object P_1, string P_2)
		{
			throw new ArgumentOutOfRangeException(P_0, P_2);
		}

		public static h Adapter(IList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list is h result)
			{
				return result;
			}
			h h2 = new _000656(list);
			if (list.IsSynchronized)
			{
				return Synchronized(h2);
			}
			return h2;
		}

		public static h Synchronized(h list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsSynchronized)
			{
				return list;
			}
			return new _000657(list);
		}

		public static IList Synchronized(IList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsSynchronized)
			{
				return list;
			}
			return new _00065_0018(list);
		}

		public static h ReadOnly(h list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsReadOnly)
			{
				return list;
			}
			return new _00065v(list);
		}

		public static IList ReadOnly(IList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsReadOnly)
			{
				return list;
			}
			return new _00065_0002(list);
		}

		public static h FixedSize(h list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsFixedSize)
			{
				return list;
			}
			return new _00065_0006(list);
		}

		public static IList FixedSize(IList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			if (list.IsFixedSize)
			{
				return list;
			}
			return new _00065W(list);
		}

		public static h Repeat(object value, int count)
		{
			h h2 = new h(count);
			for (int i = 0; i < count; i++)
			{
				h2.Add(value);
			}
			return h2;
		}
	}
}
namespace _0002
{
	internal interface h
	{
		void OnPairCreated(Y.a other, D.h collisionPair);

		void OnPairRemoved(Y.a other);

		void OnPairUpdated(Y.a other, D.h collisionPair);
	}
}
namespace _000E
{
	internal abstract class h
	{
		private Action<h> a5h;

		public event Action<h> ShapeChanged
		{
			add
			{
				Action<h> action = a5h;
				Action<h> action2;
				do
				{
					action2 = action;
					Action<h> value2 = (Action<h>)Delegate.Combine(action2, value);
					action = Interlocked.CompareExchange(ref a5h, value2, action2);
				}
				while ((object)action != action2);
			}
			remove
			{
				Action<h> action = a5h;
				Action<h> action2;
				do
				{
					action2 = action;
					Action<h> value2 = (Action<h>)Delegate.Remove(action2, value);
					action = Interlocked.CompareExchange(ref a5h, value2, action2);
				}
				while ((object)action != action2);
			}
		}

		protected virtual void OnShapeChanged()
		{
			if (a5h != null)
			{
				a5h(this);
			}
		}
	}
}
namespace _0001
{
	internal interface h : global::r.h
	{
		bool IsUpdatedSequentially { get; set; }

		bool IsUpdating { get; set; }

		List<X> Managers { get; }
	}
}
namespace _000F
{
	internal class h : _0001.b, _0001._6, _0001.h, global::r.h, global::_0002._6
	{
		private float a5h = 4.5f;

		private float a5b = 3f;

		private float a56 = 1f;

		private static d.v a5a = new d.v();

		private y a57;

		private bool a5_0006;

		[CompilerGenerated]
		private _0013.h a5v;

		[CompilerGenerated]
		private B a5B;

		[CompilerGenerated]
		private _0006 a5X;

		[CompilerGenerated]
		private a a5_0018;

		[CompilerGenerated]
		private b a5W;

		[CompilerGenerated]
		private r a5_0002;

		[CompilerGenerated]
		private X a5_000E;

		public _0013.h Body
		{
			[CompilerGenerated]
			get
			{
				return a5v;
			}
			[CompilerGenerated]
			private set
			{
				a5v = h2;
			}
		}

		public B StepManager
		{
			[CompilerGenerated]
			get
			{
				return a5B;
			}
			[CompilerGenerated]
			private set
			{
				a5B = b2;
			}
		}

		public _0006 StanceManager
		{
			[CompilerGenerated]
			get
			{
				return a5X;
			}
			[CompilerGenerated]
			private set
			{
				a5X = obj;
			}
		}

		public a QueryManager
		{
			[CompilerGenerated]
			get
			{
				return a5_0018;
			}
			[CompilerGenerated]
			private set
			{
				a5_0018 = a2;
			}
		}

		public b HorizontalMotionConstraint
		{
			[CompilerGenerated]
			get
			{
				return a5W;
			}
			[CompilerGenerated]
			private set
			{
				a5W = b2;
			}
		}

		public r VerticalMotionConstraint
		{
			[CompilerGenerated]
			get
			{
				return a5_0002;
			}
			[CompilerGenerated]
			private set
			{
				a5_0002 = r2;
			}
		}

		public float JumpSpeed
		{
			get
			{
				return a5h;
			}
			set
			{
				if (value < 0f)
				{
					throw new Exception("Value must be nonnegative.");
				}
				a5h = value;
			}
		}

		public float SlidingJumpSpeed
		{
			get
			{
				return a5b;
			}
			set
			{
				if (value < 0f)
				{
					throw new Exception("Value must be nonnegative.");
				}
				a5b = value;
			}
		}

		public float JumpForceFactor
		{
			get
			{
				return a56;
			}
			set
			{
				if (value < 0f)
				{
					throw new Exception("Value must be nonnegative.");
				}
				a56 = value;
			}
		}

		public X SupportFinder
		{
			[CompilerGenerated]
			get
			{
				return a5_000E;
			}
			[CompilerGenerated]
			private set
			{
				a5_000E = x2;
			}
		}

		public h()
		{
			Body = new _0013.h(Vector3.Zero, 1.7f, 0.6f, 10f);
			Body.IgnoreShapeChanges = true;
			Body.CollisionInformation.Shape.CollisionMargin = 0.1f;
			Body.PositionUpdateMode = m._7.Continuous;
			Body.LocalInertiaTensorInverse = default(N._7);
			Body.CollisionInformation.Events.DirectDispatchDetectingInitialCollisionEventHandler = this;
			Body.LinearDamping = 0f;
			SupportFinder = new X(this);
			HorizontalMotionConstraint = new b(this);
			VerticalMotionConstraint = new r(this);
			StepManager = new B(this);
			StanceManager = new _0006(this);
			QueryManager = new a(this);
			base.IsUpdatedSequentially = false;
		}

		public void OnDetectingInitialCollision(P.h sender, P.h other, D.b pair)
		{
			pair?.UpdateMaterialProperties(default(T.b));
		}

		private void bh()
		{
			if (Body.ActivityInformation.IsActive)
			{
				float num = Body.CollisionInformation.Shape.CollisionMargin * 1.1f;
				Vector3 value = new Vector3
				{
					X = num,
					Y = StepManager.MaximumStepHeight,
					Z = num
				};
				BoundingBox boundingBox = Body.CollisionInformation.BoundingBox;
				Vector3.Add(ref boundingBox.Max, ref value, out boundingBox.Max);
				Vector3.Subtract(ref boundingBox.Min, ref value, out boundingBox.Min);
				Body.CollisionInformation.BoundingBox = boundingBox;
			}
		}

		private void bb()
		{
			SupportFinder.UpdateSupports();
			if (SupportFinder.HasSupport)
			{
				if (SupportFinder.HasTraction)
				{
					a57 = SupportFinder.TractionData.Value;
				}
				else
				{
					a57 = SupportFinder.SupportData.Value;
				}
			}
			else
			{
				a57 = default(y);
			}
		}

		void _0001._6.Update(float dt)
		{
			ba();
			bool hasTraction = SupportFinder.HasTraction;
			bb();
			b7(ref a57, out var vector);
			float num = Vector3.Dot(a57.Normal, vector);
			_ = vector - a57.Normal * num;
			if (SupportFinder.HasTraction && !hasTraction && num < 0f)
			{
				SupportFinder.bP();
				a57 = default(y);
			}
			if (SupportFinder.HasTraction && num < (0f - VerticalMotionConstraint.MaximumGlueForce) * dt / VerticalMotionConstraint.EffectiveMass)
			{
				SupportFinder.bP();
				a57 = default(y);
			}
			if (a5_0006 && StanceManager.CurrentStance != v.Crouching)
			{
				if (SupportFinder.HasTraction)
				{
					float num2 = Vector3.Dot(Body.OrientationMatrix.Up, vector);
					float num3 = Math.Max(a5h - num2, 0f);
					b_0006(ref a57, Body.OrientationMatrix.Up * num3, ref vector);
					foreach (D.b pair in Body.CollisionInformation.Pairs)
					{
						pair.ClearContacts();
					}
					SupportFinder.bP();
					a57 = default(y);
				}
				else if (SupportFinder.HasSupport)
				{
					float num4 = Vector3.Dot(a57.Normal, vector);
					float num5 = Math.Max(a5b - num4, 0f);
					b_0006(ref a57, a57.Normal * (0f - num5), ref vector);
					foreach (D.b pair2 in Body.CollisionInformation.Pairs)
					{
						pair2.ClearContacts();
					}
					SupportFinder.bP();
					a57 = default(y);
				}
			}
			a5_0006 = false;
			if (StepManager.TryToStepDown(out var newPosition) || StepManager.TryToStepUp(out newPosition))
			{
				b6(newPosition, dt);
			}
			if (StanceManager.UpdateStance(out newPosition))
			{
				b6(newPosition, dt);
			}
			Vector3 movementDirection = new Vector3(HorizontalMotionConstraint.MovementDirection.X, 0f, HorizontalMotionConstraint.MovementDirection.Y);
			SupportFinder.GetTractionInDirection(ref movementDirection, out var supportData);
			a5a.Enter();
			HorizontalMotionConstraint.SupportData = a57;
			VerticalMotionConstraint.SupportData = supportData;
			a5a.Exit();
		}

		private void b6(Vector3 P_0, float P_1)
		{
			Body.Position = P_0;
			Quaternion orientation = Body.Orientation;
			Body.CollisionInformation.UpdateWorldTransform(ref P_0, ref orientation);
			foreach (D.b pair in Body.CollisionInformation.Pairs)
			{
				pair.ClearContacts();
				pair.UpdateCollision(P_1);
			}
			bb();
		}

		private void ba()
		{
			Vector3 vector = Body.OrientationMatrix.Down;
			Vector3 value = Body.Position;
			float collisionMargin = Body.CollisionInformation.Shape.CollisionMargin;
			float num = Body.Height * 0.5f - collisionMargin;
			float num2 = Body.Radius - collisionMargin;
			float num3 = num2 * num2;
			foreach (D.b pair in Body.CollisionInformation.Pairs)
			{
				foreach (D._0001 contact2 in pair.Contacts)
				{
					_0004.h contact = contact2.Contact;
					Vector3 vector2 = contact.Position - Body.Position;
					Vector3.Dot(ref vector2, ref vector, out var result);
					if (!(result > num))
					{
						continue;
					}
					Vector3.Dot(ref vector2, ref vector, out result);
					Vector3.Multiply(ref vector, result, out var result2);
					Vector3.Subtract(ref vector2, ref result2, out result2);
					float num4 = result2.LengthSquared();
					if (num4 > num3)
					{
						Vector3.Multiply(ref result2, num2 / (float)Math.Sqrt(num4), out result2);
					}
					Vector3.Multiply(ref vector, num, out var result3);
					Vector3.Add(ref result3, ref result2, out result3);
					Vector3.Add(ref result3, ref value, out result3);
					Vector3.Subtract(ref contact.Position, ref result3, out var result4);
					num4 = result4.LengthSquared();
					if (!(num4 > 1E-07f))
					{
						continue;
					}
					Vector3.Divide(ref result4, (float)Math.Sqrt(num4), out result4);
					Vector3.Dot(ref result4, ref vector, out result);
					Vector3.Dot(ref contact.Normal, ref vector, out var result5);
					if (Math.Abs(result) > Math.Abs(result5))
					{
						Vector3.Dot(ref result4, ref contact.Normal, out result);
						if (result < 0f)
						{
							Vector3.Negate(ref result4, out result4);
							result = 0f - result;
						}
						contact.PenetrationDepth *= result;
						contact.Normal = result4;
					}
				}
			}
		}

		private void b7(ref y P_0, out Vector3 P_1)
		{
			P_1 = Body.LinearVelocity;
			if (SupportFinder.HasSupport && P_0.SupportObject is s.b b2)
			{
				Vector3 value = global::r.X.GetVelocityOfPoint(P_0.Position, b2.Entity);
				Vector3.Subtract(ref P_1, ref value, out P_1);
			}
		}

		private void b_0006(ref y P_0, Vector3 P_1, ref Vector3 P_2)
		{
			Body.LinearVelocity += P_1;
			if (P_0.SupportObject is s.b b2 && b2.Entity.IsDynamic)
			{
				Vector3 vector = P_1 * a56;
				b2.Entity.LinearMomentum += vector * (0f - Body.Mass);
				P_1 += vector;
			}
			Vector3.Add(ref P_2, ref P_1, out P_2);
		}

		private void bv(Vector3 P_0, ref Vector3 P_1)
		{
			Body.LinearVelocity += P_0;
			Vector3.Add(ref P_1, ref P_0, out P_1);
		}

		public void Jump()
		{
			a5_0006 = true;
		}

		public override void OnAdditionToSpace(global::r.a newSpace)
		{
			newSpace.Add(Body);
			newSpace.Add(HorizontalMotionConstraint);
			newSpace.Add(VerticalMotionConstraint);
			((global::r.v)newSpace).BoundingBoxUpdater.Finishing += bh;
			Body.AngularVelocity = default(Vector3);
			Body.LinearVelocity = default(Vector3);
		}

		public override void OnRemovalFromSpace(global::r.a oldSpace)
		{
			oldSpace.Remove(Body);
			oldSpace.Remove(HorizontalMotionConstraint);
			oldSpace.Remove(VerticalMotionConstraint);
			((global::r.v)oldSpace).BoundingBoxUpdater.Finishing -= bh;
			SupportFinder.bP();
			Body.AngularVelocity = default(Vector3);
			Body.LinearVelocity = default(Vector3);
		}
	}
}
namespace _0013
{
	internal class h : E.b<s.B<y.b>>
	{
		public float Height
		{
			get
			{
				return base.CollisionInformation.Shape.Height;
			}
			set
			{
				base.CollisionInformation.Shape.Height = value;
			}
		}

		public float Radius
		{
			get
			{
				return base.CollisionInformation.Shape.Radius;
			}
			set
			{
				base.CollisionInformation.Shape.Radius = value;
			}
		}

		private h(float P_0, float P_1, float P_2)
			: base(new s.B<y.b>(new y.b(P_0, P_1)), P_2)
		{
		}

		private h(float P_0, float P_1)
			: base(new s.B<y.b>(new y.b(P_0, P_1)))
		{
		}

		public h(Vector3 position, float height, float radius, float mass)
			: this(height, radius, mass)
		{
			base.Position = position;
		}

		public h(Vector3 position, float height, float radius)
			: this(height, radius)
		{
			base.Position = position;
		}

		public h(n.B motionState, float height, float radius, float mass)
			: this(height, radius, mass)
		{
			base.MotionState = motionState;
		}

		public h(n.B motionState, float height, float radius)
			: this(height, radius)
		{
			base.MotionState = motionState;
		}
	}
}
namespace _0012
{
	internal class h
	{
		internal static SystemStatistic a5h = SystemConsole.GetStatistic("Collision_MeshTestCount", SystemStatisticCategory.Collision);

		internal static SystemStatistic a5b = SystemConsole.GetStatistic("Collision_PolyTestCount", SystemStatisticCategory.Collision);

		internal static SystemStatistic a56 = SystemConsole.GetStatistic("Collision_PolyProjTestCount", SystemStatisticCategory.Collision);

		internal static SystemStatistic a5a = SystemConsole.GetStatistic("Collision_PolyProjPartialTestCount", SystemStatisticCategory.Collision);

		internal static SystemStatistic a57 = SystemConsole.GetStatistic("Collision_PolyProjFullTestCount", SystemStatisticCategory.Collision);

		internal static SystemStatistic a5_0006 = SystemConsole.GetStatistic("Collision_PolyDeepTestCount", SystemStatisticCategory.Collision);

		internal static Vector3 a5v = Vector3.UnitX;

		internal static List<CollisionMesh.CollisionSurface> a5B = new List<CollisionMesh.CollisionSurface>();

		internal static bool bc(ICollisionObject P_0, ICollisionObject P_1, CollisionPoint P_2)
		{
			if (P_0.CollisionType != CollisionType.Trigger && P_1.CollisionType != CollisionType.Trigger)
			{
				return false;
			}
			P_2.Triggers.Add(P_1);
			return true;
		}
	}
}
namespace _0017
{
	internal static class h
	{
		public static int MaximumGJKIterations = 15;

		public static int HighGJKIterations = 8;

		public static bool AreShapesIntersecting(y.h shapeA, y.h shapeB, ref N._0006 transformA, ref N._0006 transformB)
		{
			Vector3 localSeparatingAxis = r.X.ZeroVector;
			return AreShapesIntersecting(shapeA, shapeB, ref transformA, ref transformB, ref localSeparatingAxis);
		}

		public static bool AreShapesIntersecting(y.h shapeA, y.h shapeB, ref N._0006 transformA, ref N._0006 transformB, ref Vector3 localSeparatingAxis)
		{
			I._0006.GetLocalTransform(ref transformA, ref transformB, out var localTransformB);
			v v2 = default(v);
			I._0006.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref localSeparatingAxis, ref localTransformB, out var extremePoint);
			v2.AddNewSimplexPoint(ref extremePoint);
			int num = 0;
			while (num++ < MaximumGJKIterations)
			{
				if (v2.GetPointClosestToOrigin(out var point) || point.LengthSquared() <= v2.GetErrorTolerance() * 1E-05f)
				{
					return true;
				}
				Vector3.Negate(ref point, out var result);
				I._0006.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref result, ref localTransformB, out extremePoint);
				Vector3.Dot(ref extremePoint, ref point, out var result2);
				if (result2 > 0f)
				{
					localSeparatingAxis = result;
					return false;
				}
				v2.AddNewSimplexPoint(ref extremePoint);
			}
			return false;
		}

		public static bool GetClosestPoints(y.h shapeA, y.h shapeB, ref N._0006 transformA, ref N._0006 transformB, out Vector3 closestPointA, out Vector3 closestPointB)
		{
			I._0006.GetLocalTransform(ref transformA, ref transformB, out var localTransformB);
			_6 obj = new _6
			{
				State = b.Point
			};
			bool result = _6B(shapeA, shapeB, ref localTransformB, ref obj, out closestPointA, out closestPointB);
			N._0006.Transform(ref closestPointA, ref transformA, out closestPointA);
			N._0006.Transform(ref closestPointB, ref transformA, out closestPointB);
			return result;
		}

		public static bool GetClosestPoints(y.h shapeA, y.h shapeB, ref N._0006 transformA, ref N._0006 transformB, ref _6 cachedSimplex, out Vector3 closestPointA, out Vector3 closestPointB)
		{
			I._0006.GetLocalTransform(ref transformA, ref transformB, out var localTransformB);
			bool result = _6B(shapeA, shapeB, ref localTransformB, ref cachedSimplex, out closestPointA, out closestPointB);
			N._0006.Transform(ref closestPointA, ref transformA, out closestPointA);
			N._0006.Transform(ref closestPointB, ref transformA, out closestPointB);
			return result;
		}

		private static bool _6B(y.h P_0, y.h P_1, ref N._0006 P_2, ref _6 P_3, out Vector3 P_4, out Vector3 P_5)
		{
			_7 obj = new _7(ref P_3, ref P_2);
			int num = 0;
			Vector3 point;
			do
			{
				if (obj.GetPointClosestToOrigin(out point) || point.LengthSquared() <= 1E-07f * obj.a5h)
				{
					P_4 = r.X.ZeroVector;
					P_5 = r.X.ZeroVector;
					obj.UpdateCachedSimplex(ref P_3);
					return true;
				}
			}
			while (++num <= MaximumGJKIterations && !obj.GetNewSimplexPoint(P_0, P_1, num, ref point));
			obj.GetClosestPoints(out P_4, out P_5);
			obj.UpdateCachedSimplex(ref P_3);
			return false;
		}

		public static bool RayCast(Ray ray, y.h shape, ref N._0006 shapeTransform, float maximumLength, out r._0006 hit)
		{
			Vector3.Subtract(ref ray.Position, ref shapeTransform.Position, out ray.Position);
			Quaternion.Conjugate(ref shapeTransform.Orientation, out var result);
			Vector3.Transform(ref ray.Position, ref result, out ray.Position);
			Vector3.Transform(ref ray.Direction, ref result, out ray.Direction);
			hit.T = 0f;
			hit.Location = ray.Position;
			hit.Normal = r.X.ZeroVector;
			Vector3 vector = hit.Location;
			_0006 simplex = default(_0006);
			int num = 0;
			while (vector.LengthSquared() >= 1E-07f * simplex.GetErrorTolerance(ref ray.Position))
			{
				if (++num > MaximumGJKIterations)
				{
					hit = default(r._0006);
					return false;
				}
				shape.GetLocalExtremePoint(vector, out var extremePoint);
				Vector3.Subtract(ref hit.Location, ref extremePoint, out var result2);
				Vector3.Dot(ref vector, ref result2, out var result3);
				if (result3 > 0f)
				{
					Vector3.Dot(ref vector, ref ray.Direction, out var result4);
					if (result4 >= 0f)
					{
						hit = default(r._0006);
						return false;
					}
					hit.T -= result3 / result4;
					if (hit.T > maximumLength)
					{
						hit = default(r._0006);
						return false;
					}
					Vector3.Multiply(ref ray.Direction, hit.T, out hit.Location);
					Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
					hit.Normal = vector;
				}
				simplex.AddNewSimplexPoint(ref extremePoint, ref hit.Location, out var shiftedSimplex);
				shiftedSimplex.GetPointClosestToOrigin(ref simplex, out vector);
			}
			Vector3.Transform(ref hit.Normal, ref shapeTransform.Orientation, out hit.Normal);
			Vector3.Transform(ref hit.Location, ref shapeTransform.Orientation, out hit.Location);
			Vector3.Add(ref hit.Location, ref shapeTransform.Position, out hit.Location);
			return true;
		}

		public static bool ConvexCast(y.h sweptShape, y.h target, ref Vector3 sweep, ref N._0006 startingSweptTransform, ref N._0006 targetTransform, out r._0006 hit)
		{
			return ConvexCast(sweptShape, target, ref sweep, ref r.X.ZeroVector, ref startingSweptTransform, ref targetTransform, out hit);
		}

		public static bool ConvexCast(y.h shapeA, y.h shapeB, ref Vector3 sweepA, ref Vector3 sweepB, ref N._0006 transformA, ref N._0006 transformB, out r._0006 hit)
		{
			Vector3.Subtract(ref sweepB, ref sweepA, out var result);
			Quaternion.Conjugate(ref transformA.Orientation, out var result2);
			Vector3.Transform(ref result, ref result2, out var result3);
			N._0006 localTransformB = default(N._0006);
			Quaternion.Concatenate(ref transformB.Orientation, ref result2, out localTransformB.Orientation);
			Vector3.Subtract(ref transformB.Position, ref transformA.Position, out localTransformB.Position);
			Vector3.Transform(ref localTransformB.Position, ref result2, out localTransformB.Position);
			hit.T = 0f;
			hit.Location = Vector3.Zero;
			hit.Normal = r.X.ZeroVector;
			Vector3 direction = hit.Location;
			_0006 simplex = default(_0006);
			int num = 0;
			do
			{
				if (++num > MaximumGJKIterations)
				{
					hit = default(r._0006);
					return false;
				}
				I._0006.GetLocalMinkowskiExtremePoint(shapeA, shapeB, ref direction, ref localTransformB, out var extremePoint);
				Vector3.Subtract(ref hit.Location, ref extremePoint, out var result4);
				Vector3.Dot(ref direction, ref result4, out var result5);
				if (result5 > 0f)
				{
					Vector3.Dot(ref direction, ref result3, out var result6);
					if (result6 >= 0f)
					{
						hit = default(r._0006);
						return false;
					}
					hit.T -= result5 / result6;
					if (hit.T > 1f)
					{
						hit = default(r._0006);
						return false;
					}
					Vector3.Multiply(ref result3, hit.T, out hit.Location);
					hit.Normal = direction;
				}
				simplex.AddNewSimplexPoint(ref extremePoint, ref hit.Location, out var shiftedSimplex);
				shiftedSimplex.GetPointClosestToOrigin(ref simplex, out direction);
			}
			while (direction.LengthSquared() >= 1E-07f * simplex.GetErrorTolerance(ref r.X.ZeroVector));
			Vector3.Transform(ref hit.Normal, ref transformA.Orientation, out hit.Normal);
			Vector3.Multiply(ref result, hit.T, out hit.Location);
			Vector3.Add(ref hit.Location, ref transformA.Position, out hit.Location);
			return true;
		}

		public static bool SphereCast(Ray ray, float radius, y.h shape, ref N._0006 shapeTransform, float maximumLength, out r._0006 hit)
		{
			Vector3.Subtract(ref ray.Position, ref shapeTransform.Position, out ray.Position);
			Quaternion.Conjugate(ref shapeTransform.Orientation, out var result);
			Vector3.Transform(ref ray.Position, ref result, out ray.Position);
			Vector3.Transform(ref ray.Direction, ref result, out ray.Direction);
			hit.T = 0f;
			hit.Location = ray.Position;
			hit.Normal = r.X.ZeroVector;
			Vector3 direction = hit.Location;
			_0006 simplex = default(_0006);
			int num = 0;
			while (direction.LengthSquared() >= 1E-07f * simplex.GetErrorTolerance(ref ray.Position))
			{
				if (++num > MaximumGJKIterations)
				{
					hit = default(r._0006);
					return false;
				}
				shape.GetLocalExtremePointWithoutMargin(ref direction, out var extremePoint);
				I._0006.ExpandMinkowskiSum(shape.collisionMargin, radius, ref direction, out var contribution);
				Vector3.Add(ref extremePoint, ref contribution, out extremePoint);
				Vector3.Subtract(ref hit.Location, ref extremePoint, out var result2);
				Vector3.Dot(ref direction, ref result2, out var result3);
				if (result3 > 0f)
				{
					Vector3.Dot(ref direction, ref ray.Direction, out var result4);
					hit.T -= result3 / result4;
					if (result4 >= 0f)
					{
						return false;
					}
					if (hit.T > maximumLength)
					{
						return false;
					}
					Vector3.Multiply(ref ray.Direction, hit.T, out hit.Location);
					Vector3.Add(ref hit.Location, ref ray.Position, out hit.Location);
					hit.Normal = direction;
				}
				simplex.AddNewSimplexPoint(ref extremePoint, ref hit.Location, out var shiftedSimplex);
				shiftedSimplex.GetPointClosestToOrigin(ref simplex, out direction);
			}
			Vector3.Transform(ref hit.Normal, ref shapeTransform.Orientation, out hit.Normal);
			Vector3.Transform(ref hit.Location, ref shapeTransform.Orientation, out hit.Location);
			Vector3.Add(ref hit.Location, ref shapeTransform.Position, out hit.Location);
			return true;
		}

		public static bool CCDSphereCast(Ray ray, float radius, y.h target, ref N._0006 shapeTransform, float maximumLength, out r._0006 hit)
		{
			int num = 0;
			do
			{
				if (SphereCast(ray, radius, target, ref shapeTransform, maximumLength, out hit) && hit.T > 0f)
				{
					return true;
				}
				if (hit.T > maximumLength || hit.T < 0f)
				{
					return false;
				}
				radius *= _0014._6.CoreShapeScaling;
				num++;
			}
			while (num <= 3);
			if (RayCast(ray, target, ref shapeTransform, maximumLength, out hit))
			{
				return hit.T > 0f;
			}
			return false;
		}
	}
}
namespace _0004
{
	internal class h : IEquatable<h>
	{
		public float PenetrationDepth;

		public int Id = -1;

		public Vector3 Normal;

		public Vector3 Position;

		public void Setup(ref b candidate)
		{
			Position = candidate.Position;
			Normal = candidate.Normal;
			PenetrationDepth = candidate.PenetrationDepth;
			Id = candidate.Id;
		}

		public bool Equals(h other)
		{
			if (Id == other.Id)
			{
				if (Id == -1)
				{
					Vector3.DistanceSquared(ref other.Position, ref Position, out var result);
					return result < 0.001f;
				}
				return true;
			}
			return false;
		}

		public override string ToString()
		{
			return string.Concat("Position: ", Position, " Normal: ", Normal, " Depth: ", PenetrationDepth);
		}
	}
}
namespace _0010
{
	internal class h : L.h
	{
		private new b a5h;

		private a a5b;

		internal float a56;

		private float a5a;

		private float a57;

		private float a5_0006;

		private float a5v;

		private float a5B;

		private float a5X;

		private float a5_0018;

		internal float a5W;

		internal float a5_0002;

		internal float a5_000E;

		private E.h a5y;

		private E.h a5r;

		private bool a5_0001;

		private bool a5_000F;

		private float a5Z;

		public b ContactManifoldConstraint => a5h;

		public a PenetrationConstraint => a5b;

		public Vector3 FrictionDirection => new Vector3(a5W, a5_0002, a5_000E);

		public float TotalForce => a56;

		public float RelativeVelocity
		{
			get
			{
				float num = 0f;
				if (a5y != null)
				{
					num += a5y.a5a.X * a5W + a5y.a5a.Y * a5_0002 + a5y.a5a.Z * a5_000E + a5y.a5_0006.X * a5a + a5y.a5_0006.Y * a57 + a5y.a5_0006.Z * a5_0006;
				}
				if (a5r != null)
				{
					num += (0f - a5r.a5a.X) * a5W - a5r.a5a.Y * a5_0002 - a5r.a5a.Z * a5_000E + a5r.a5_0006.X * a5v + a5r.a5_0006.Y * a5B + a5r.a5_0006.Z * a5X;
				}
				return num;
			}
		}

		public h()
		{
			isActive = false;
		}

		public void Setup(b contactManifoldConstraint, a penetrationConstraint)
		{
			a5h = contactManifoldConstraint;
			a5b = penetrationConstraint;
			base.IsActive = true;
			a5W = 0f;
			a5_0002 = 0f;
			a5_000E = 0f;
			a5y = contactManifoldConstraint.EntityA;
			a5r = contactManifoldConstraint.EntityB;
			a5_0001 = a5y != null && a5y.a5B;
			a5_000F = a5r != null && a5r.a5B;
		}

		public void CleanUp()
		{
			a56 = 0f;
			a5h = null;
			a5b = null;
			a5y = null;
			a5r = null;
			base.IsActive = false;
		}

		public override float SolveIteration()
		{
			float num = RelativeVelocity * a5Z;
			float num2 = a56;
			float num3 = a5_0018 * a5b.a5b;
			a56 = MathHelper.Clamp(a56 + num, 0f - num3, num3);
			num = a56 - num2;
			Vector3 impulse = default(Vector3);
			Vector3 impulse2 = default(Vector3);
			impulse.X = num * a5W;
			impulse.Y = num * a5_0002;
			impulse.Z = num * a5_000E;
			if (a5_0001)
			{
				impulse2.X = num * a5a;
				impulse2.Y = num * a57;
				impulse2.Z = num * a5_0006;
				a5y.ApplyLinearImpulse(ref impulse);
				a5y.ApplyAngularImpulse(ref impulse2);
			}
			if (a5_000F)
			{
				impulse.X = 0f - impulse.X;
				impulse.Y = 0f - impulse.Y;
				impulse.Z = 0f - impulse.Z;
				impulse2.X = num * a5v;
				impulse2.Y = num * a5B;
				impulse2.Z = num * a5X;
				a5r.ApplyLinearImpulse(ref impulse);
				a5r.ApplyAngularImpulse(ref impulse2);
			}
			return Math.Abs(num);
		}

		public override void Update(float dt)
		{
			Vector3 result = default(Vector3);
			Vector3 result2 = default(Vector3);
			Vector3 vector = a5b.a5u;
			Vector3 vector2 = a5b.a5L;
			if (a5y != null)
			{
				Vector3.Cross(ref a5y.a5_0006, ref vector, out result);
				Vector3.Add(ref result, ref a5y.a5a, out result);
			}
			if (a5r != null)
			{
				Vector3.Cross(ref a5r.a5_0006, ref vector2, out result2);
				Vector3.Add(ref result2, ref a5r.a5a, out result2);
			}
			Vector3.Subtract(ref result, ref result2, out var result3);
			Vector3 normal = a5b.a5h.Normal;
			float num = normal.X * result3.X + normal.Y * result3.Y + normal.Z * result3.Z;
			result3.X -= num * normal.X;
			result3.Y -= num * normal.Y;
			result3.Z -= num * normal.Z;
			float num2 = result3.LengthSquared();
			if (num2 > 1E-07f)
			{
				num2 = (float)Math.Sqrt(num2);
				a5W = result3.X / num2;
				a5_0002 = result3.Y / num2;
				a5_000E = result3.Z / num2;
				a5_0018 = ((num2 > _0014.b.StaticFrictionVelocityThreshold) ? a5h.a5h.KineticFriction : a5h.a5h.StaticFriction);
			}
			else
			{
				if (a5W == 0f && a5_0002 == 0f && a5_000E == 0f)
				{
					isActiveInSolver = false;
					return;
				}
				a5_0018 = a5h.a5h.StaticFriction;
			}
			a5a = vector.Y * a5_000E - vector.Z * a5_0002;
			a57 = vector.Z * a5W - vector.X * a5_000E;
			a5_0006 = vector.X * a5_0002 - vector.Y * a5W;
			a5v = a5_0002 * vector2.Z - a5_000E * vector2.Y;
			a5B = a5_000E * vector2.X - a5W * vector2.Z;
			a5X = a5W * vector2.Y - a5_0002 * vector2.X;
			float num6;
			if (a5_0001)
			{
				float num3 = a5a * a5y.a5_0018.M11 + a57 * a5y.a5_0018.M21 + a5_0006 * a5y.a5_0018.M31;
				float num4 = a5a * a5y.a5_0018.M12 + a57 * a5y.a5_0018.M22 + a5_0006 * a5y.a5_0018.M32;
				float num5 = a5a * a5y.a5_0018.M13 + a57 * a5y.a5_0018.M23 + a5_0006 * a5y.a5_0018.M33;
				num6 = num3 * a5a + num4 * a57 + num5 * a5_0006 + a5y.a5r;
			}
			else
			{
				num6 = 0f;
			}
			float num7;
			if (a5_000F)
			{
				float num3 = a5v * a5r.a5_0018.M11 + a5B * a5r.a5_0018.M21 + a5X * a5r.a5_0018.M31;
				float num4 = a5v * a5r.a5_0018.M12 + a5B * a5r.a5_0018.M22 + a5X * a5r.a5_0018.M32;
				float num5 = a5v * a5r.a5_0018.M13 + a5B * a5r.a5_0018.M23 + a5X * a5r.a5_0018.M33;
				num7 = num3 * a5v + num4 * a5B + num5 * a5X + a5r.a5r;
			}
			else
			{
				num7 = 0f;
			}
			a5Z = -1f / (num6 + num7);
		}

		public override void ExclusiveUpdate()
		{
			Vector3 impulse = default(Vector3);
			Vector3 impulse2 = default(Vector3);
			impulse.X = a56 * a5W;
			impulse.Y = a56 * a5_0002;
			impulse.Z = a56 * a5_000E;
			if (a5_0001)
			{
				impulse2.X = a56 * a5a;
				impulse2.Y = a56 * a57;
				impulse2.Z = a56 * a5_0006;
				a5y.ApplyLinearImpulse(ref impulse);
				a5y.ApplyAngularImpulse(ref impulse2);
			}
			if (a5_000F)
			{
				impulse.X = 0f - impulse.X;
				impulse.Y = 0f - impulse.Y;
				impulse.Z = 0f - impulse.Z;
				impulse2.X = a56 * a5v;
				impulse2.Y = a56 * a5B;
				impulse2.Z = a56 * a5X;
				a5r.ApplyLinearImpulse(ref impulse);
				a5r.ApplyAngularImpulse(ref impulse2);
			}
		}

		protected internal override void CollectInvolvedEntities(l._7<E.h> outputInvolvedEntities)
		{
			if (a5y != null)
			{
				outputInvolvedEntities.Add(a5y);
			}
			if (a5r != null)
			{
				outputInvolvedEntities.Add(a5r);
			}
		}
	}
}
namespace _0014
{
	internal static class h
	{
		public static float ContactInvalidationLengthSquared = 0.01f;

		public static float ContactMinimumSeparationDistanceSquared = 0.01f;

		internal static float a5h = 0.99f;

		public static float AllowedPenetration = 0.01f;

		public static float DefaultMargin = 0.04f;

		internal static float a5b = 0.1f;

		public static float NonconvexNormalAngleDifferenceMinimum
		{
			get
			{
				return (float)Math.Acos(a5h);
			}
			set
			{
				a5h = (float)Math.Cos(value);
			}
		}

		public static float MaximumContactDistance
		{
			get
			{
				return a5b;
			}
			set
			{
				if (value >= 0f)
				{
					a5b = value;
					return;
				}
				throw new Exception("Distance must be nonnegative.");
			}
		}
	}
}
