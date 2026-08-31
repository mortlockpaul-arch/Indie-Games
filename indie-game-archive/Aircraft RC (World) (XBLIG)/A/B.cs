using System;
using System.IO;
using System.Text;
using Y;

namespace A;

internal class B
{
	private byte HCB;

	private byte[] HC_0002;

	private Y.B HC_0012;

	public int Count
	{
		get
		{
			if (HC_0012 == null)
			{
				return 0;
			}
			return HC_0012.Count;
		}
	}

	public byte Tag => HCB;

	public int Length
	{
		get
		{
			if (HC_0002 != null)
			{
				return HC_0002.Length;
			}
			return 0;
		}
	}

	public byte[] Value
	{
		get
		{
			if (HC_0002 == null)
			{
				GetBytes();
			}
			return (byte[])HC_0002.Clone();
		}
		set
		{
			if (value != null)
			{
				HC_0002 = (byte[])value.Clone();
			}
		}
	}

	public B this[int index]
	{
		get
		{
			try
			{
				if (HC_0012 == null || index >= HC_0012.Count)
				{
					return null;
				}
				return (B)HC_0012[index];
			}
			catch (ArgumentOutOfRangeException)
			{
				return null;
			}
		}
	}

	public B()
		: this(0, null)
	{
	}

	public B(byte tag)
		: this(tag, null)
	{
	}

	public B(byte tag, byte[] data)
	{
		HCB = tag;
		HC_0002 = data;
	}

	public B(byte[] data)
	{
		HCB = data[0];
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
		HC_0002 = new byte[num2];
		Buffer.BlockCopy(data, 2 + num, HC_0002, 0, num2);
		if ((HCB & 0x20) == 32)
		{
			int anPos = 2 + num;
			Decode(data, ref anPos, data.Length);
		}
	}

	private bool H_0019(byte[] P_0, byte[] P_1)
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
		return H_0019(GetBytes(), asn1);
	}

	public bool CompareValue(byte[] value)
	{
		return H_0019(HC_0002, value);
	}

	public B Add(B asn1)
	{
		if (asn1 != null)
		{
			if (HC_0012 == null)
			{
				HC_0012 = new Y.B();
			}
			HC_0012.Add(asn1);
		}
		return asn1;
	}

	public virtual byte[] GetBytes()
	{
		byte[] array = null;
		if (Count > 0)
		{
			int num = 0;
			Y.B b = new Y.B();
			foreach (B item in HC_0012)
			{
				byte[] bytes = item.GetBytes();
				b.Add(bytes);
				num += bytes.Length;
			}
			array = new byte[num];
			int num2 = 0;
			for (int i = 0; i < HC_0012.Count; i++)
			{
				byte[] array2 = (byte[])b[i];
				Buffer.BlockCopy(array2, 0, array, num2, array2.Length);
				num2 += array2.Length;
			}
		}
		else if (HC_0002 != null)
		{
			array = HC_0002;
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
			if (HC_0002 == null)
			{
				HC_0002 = array;
			}
		}
		else
		{
			array3 = new byte[2];
		}
		array3[0] = HCB;
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
				B b = Add(new B(tag, content));
				if ((tag & 0x20) == 32)
				{
					int anPos2 = anPos;
					b.Decode(asn1, ref anPos2, anPos2 + length);
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

	public B Element(int index, byte anTag)
	{
		try
		{
			if (HC_0012 == null || index >= HC_0012.Count)
			{
				return null;
			}
			B b = (B)HC_0012[index];
			if (b.Tag == anTag)
			{
				return b;
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
			HCB.ToString("X2"),
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
