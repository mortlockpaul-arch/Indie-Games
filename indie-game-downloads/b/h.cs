using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using _0018;
using v;

namespace b
{
	internal class h
	{
		private v._000E a5h = new v._000E();

		private v._000E a5b = new v._000E();

		private v.Y a56 = new v.Y();

		private List<byte[]> a5a = new List<byte[]>();

		internal h(byte[] P_0, byte[] P_1)
		{
			a5b.PersistKeyInCsp = false;
			a5b.ImportCspBlob(P_0);
			a5h.PersistKeyInCsp = false;
			a5h.ImportCspBlob(P_1);
		}

		internal byte[] Z(byte[] P_0)
		{
			int num = b.L(P_0, 0);
			byte[] array = new byte[num];
			byte[] array2 = new byte[P_0.Length - (num + 4)];
			Array.Copy(P_0, 4, array, 0, array.Length);
			Array.Copy(P_0, num + 4, array2, 0, array2.Length);
			byte[] rgbHash = a56.ComputeHash(array2);
			if (!a5h.VerifyHash(rgbHash, v._7.MapNameToOID("SHA1"), array))
			{
				return null;
			}
			return u(array2);
		}

		private byte[] u(byte[] P_0)
		{
			int num = a5b.KeySize / 8;
			byte[] array = new byte[num];
			int num2 = 0;
			int num3 = 0;
			a5a.Clear();
			while (num2 < P_0.Length)
			{
				int num4 = Math.Min(num, P_0.Length - num2);
				byte[] array2 = array;
				if (num4 < array2.Length)
				{
					array2 = new byte[num4];
				}
				Array.Copy(P_0, num2, array2, 0, num4);
				byte[] array3 = a5b.Decrypt(array2, fOAEP: true);
				num2 += num4;
				num3 += array3.Length;
				a5a.Add(array3);
			}
			byte[] array4 = new byte[num3];
			num2 = 0;
			foreach (byte[] item in a5a)
			{
				item.CopyTo(array4, num2);
				num2 += item.Length;
			}
			return array4;
		}
	}
}
namespace B
{
	internal class h
	{
		private byte a5h;

		private byte[] a5b;

		private _0018.h a56;

		public int Count
		{
			get
			{
				if (a56 == null)
				{
					return 0;
				}
				return a56.Count;
			}
		}

		public byte Tag => a5h;

		public int Length
		{
			get
			{
				if (a5b != null)
				{
					return a5b.Length;
				}
				return 0;
			}
		}

		public byte[] Value
		{
			get
			{
				if (a5b == null)
				{
					GetBytes();
				}
				return (byte[])a5b.Clone();
			}
			set
			{
				if (value != null)
				{
					a5b = (byte[])value.Clone();
				}
			}
		}

		public h this[int index]
		{
			get
			{
				try
				{
					if (a56 == null || index >= a56.Count)
					{
						return null;
					}
					return (h)a56[index];
				}
				catch (ArgumentOutOfRangeException)
				{
					return null;
				}
			}
		}

		public h()
			: this(0, null)
		{
		}

		public h(byte tag)
			: this(tag, null)
		{
		}

		public h(byte tag, byte[] data)
		{
			a5h = tag;
			a5b = data;
		}

		public h(byte[] data)
		{
			a5h = data[0];
			int num = 0;
			int num2 = data[1];
			if (num2 > 128)
			{
				num = num2 - 128;
				num2 = 0;
				for (int i = 0; i < num; i++)
				{
					num2 *= 256;
					num2 += data[i + 2];
				}
			}
			else if (num2 == 128)
			{
				throw new NotSupportedException("Undefined length encoding.");
			}
			a5b = new byte[num2];
			Buffer.BlockCopy(data, 2 + num, a5b, 0, num2);
			if ((a5h & 0x20) == 32)
			{
				int anPos = 2 + num;
				Decode(data, ref anPos, data.Length);
			}
		}

		private bool c(byte[] P_0, byte[] P_1)
		{
			bool flag = P_0.Length == P_1.Length;
			if (flag)
			{
				for (int i = 0; i < P_0.Length; i++)
				{
					if (P_0[i] != P_1[i])
					{
						return false;
					}
				}
			}
			return flag;
		}

		public bool Equals(byte[] asn1)
		{
			return c(GetBytes(), asn1);
		}

		public bool CompareValue(byte[] value)
		{
			return c(a5b, value);
		}

		public h Add(h asn1)
		{
			if (asn1 != null)
			{
				if (a56 == null)
				{
					a56 = new _0018.h();
				}
				a56.Add(asn1);
			}
			return asn1;
		}

		public virtual byte[] GetBytes()
		{
			byte[] array = null;
			if (Count > 0)
			{
				int num = 0;
				_0018.h h2 = new _0018.h();
				foreach (h item in a56)
				{
					byte[] bytes = item.GetBytes();
					h2.Add(bytes);
					num += bytes.Length;
				}
				array = new byte[num];
				int num2 = 0;
				for (int i = 0; i < a56.Count; i++)
				{
					byte[] array2 = (byte[])h2[i];
					Buffer.BlockCopy(array2, 0, array, num2, array2.Length);
					num2 += array2.Length;
				}
			}
			else if (a5b != null)
			{
				array = a5b;
			}
			int num3 = 0;
			byte[] array3;
			if (array != null)
			{
				int num4 = array.Length;
				if (num4 > 127)
				{
					if (num4 <= 255)
					{
						array3 = new byte[3 + num4];
						Buffer.BlockCopy(array, 0, array3, 3, num4);
						num3 = 129;
						array3[2] = (byte)num4;
					}
					else if (num4 <= 65535)
					{
						array3 = new byte[4 + num4];
						Buffer.BlockCopy(array, 0, array3, 4, num4);
						num3 = 130;
						array3[2] = (byte)(num4 >> 8);
						array3[3] = (byte)num4;
					}
					else if (num4 <= 16777215)
					{
						array3 = new byte[5 + num4];
						Buffer.BlockCopy(array, 0, array3, 5, num4);
						num3 = 131;
						array3[2] = (byte)(num4 >> 16);
						array3[3] = (byte)(num4 >> 8);
						array3[4] = (byte)num4;
					}
					else
					{
						array3 = new byte[6 + num4];
						Buffer.BlockCopy(array, 0, array3, 6, num4);
						num3 = 132;
						array3[2] = (byte)(num4 >> 24);
						array3[3] = (byte)(num4 >> 16);
						array3[4] = (byte)(num4 >> 8);
						array3[5] = (byte)num4;
					}
				}
				else
				{
					array3 = new byte[2 + num4];
					Buffer.BlockCopy(array, 0, array3, 2, num4);
					num3 = num4;
				}
				if (a5b == null)
				{
					a5b = array;
				}
			}
			else
			{
				array3 = new byte[2];
			}
			array3[0] = a5h;
			array3[1] = (byte)num3;
			return array3;
		}

		protected void Decode(byte[] asn1, ref int anPos, int anLength)
		{
			while (anPos < anLength - 1)
			{
				DecodeTLV(asn1, ref anPos, out var tag, out var length, out var content);
				if (tag != 0)
				{
					h h2 = Add(new h(tag, content));
					if ((tag & 0x20) == 32)
					{
						int anPos2 = anPos;
						h2.Decode(asn1, ref anPos2, anPos2 + length);
					}
					anPos += length;
				}
			}
		}

		protected void DecodeTLV(byte[] asn1, ref int pos, out byte tag, out int length, out byte[] content)
		{
			tag = asn1[pos++];
			length = asn1[pos++];
			if ((length & 0x80) == 128)
			{
				int num = length & 0x7F;
				length = 0;
				for (int i = 0; i < num; i++)
				{
					length = length * 256 + asn1[pos++];
				}
			}
			content = new byte[length];
			Buffer.BlockCopy(asn1, pos, content, 0, length);
		}

		public h Element(int index, byte anTag)
		{
			try
			{
				if (a56 == null || index >= a56.Count)
				{
					return null;
				}
				h h2 = (h)a56[index];
				if (h2.Tag == anTag)
				{
					return h2;
				}
				return null;
			}
			catch (ArgumentOutOfRangeException)
			{
				return null;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Tag: {0} {1}", new object[2]
			{
				a5h.ToString("X2"),
				Environment.NewLine
			});
			stringBuilder.AppendFormat("Length: {0} {1}", new object[2]
			{
				Value.Length,
				Environment.NewLine
			});
			stringBuilder.Append("Value: ");
			stringBuilder.Append(Environment.NewLine);
			for (int i = 0; i < Value.Length; i++)
			{
				stringBuilder.AppendFormat("{0} ", new object[1] { Value[i].ToString("X2") });
				if ((i + 1) % 16 == 0)
				{
					stringBuilder.AppendFormat(Environment.NewLine);
				}
			}
			return stringBuilder.ToString();
		}

		public void SaveToFile(string filename)
		{
			if (filename == null)
			{
				throw new ArgumentNullException("filename");
			}
			using FileStream fileStream = File.OpenWrite(filename);
			byte[] bytes = GetBytes();
			fileStream.Write(bytes, 0, bytes.Length);
			fileStream.Flush();
			fileStream.Close();
		}
	}
}
