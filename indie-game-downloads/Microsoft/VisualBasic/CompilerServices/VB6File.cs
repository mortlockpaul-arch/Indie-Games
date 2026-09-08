using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Security;
using System.Text;
using System.Threading;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
internal abstract class VB6File
{
	internal int m_lCurrentColumn;

	internal int m_lWidth;

	internal int m_lRecordLen;

	internal long m_lRecordStart;

	internal string m_sFullPath;

	internal OpenShare m_share;

	internal OpenAccess m_access;

	internal bool m_eof;

	internal long m_position;

	internal FileStream m_file;

	internal bool m_fAppend;

	internal bool m_bPrint;

	protected StreamWriter m_sw;

	protected StreamReader m_sr;

	protected BinaryWriter m_bw;

	protected BinaryReader m_br;

	protected Encoding m_Encoding;

	protected VB6File(string sPath, OpenAccess access, OpenShare share, int lRecordLen)
	{
		if (access != OpenAccess.Read && access != OpenAccess.ReadWrite && access != OpenAccess.Write)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Access"));
		}
		m_access = access;
		if (share != OpenShare.Shared && share != OpenShare.LockRead && share != OpenShare.LockReadWrite && share != OpenShare.LockWrite)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Share"));
		}
		m_share = share;
		m_lRecordLen = lRecordLen;
		m_sFullPath = new FileInfo(sPath).FullName;
	}

	internal string GetAbsolutePath()
	{
		return m_sFullPath;
	}

	internal virtual void OpenFile()
	{
		try
		{
			if (File.Exists(m_sFullPath))
			{
				m_file = new FileStream(m_sFullPath, FileMode.Open, (FileAccess)m_access, (FileShare)m_share);
			}
			else
			{
				m_file = new FileStream(m_sFullPath, FileMode.Create, (FileAccess)m_access, (FileShare)m_share);
			}
		}
		catch (SecurityException)
		{
			throw ExceptionUtils.VbMakeException(53);
		}
	}

	internal virtual void CloseFile()
	{
		CloseTheFile();
	}

	protected void CloseTheFile()
	{
		if (m_sw != null)
		{
			m_sw.Close();
			m_sw = null;
		}
		if (m_sr != null)
		{
			m_sr.Close();
			m_sr = null;
		}
		if (m_file != null)
		{
			m_file.Close();
			m_file = null;
		}
	}

	internal int GetColumn()
	{
		return m_lCurrentColumn;
	}

	internal void SetColumn(int lColumn)
	{
		checked
		{
			if (m_lWidth != 0 && m_lCurrentColumn != 0 && lColumn + 14 > m_lWidth)
			{
				WriteLine(null);
			}
			else
			{
				SPC(lColumn - m_lCurrentColumn);
			}
		}
	}

	internal int GetWidth()
	{
		return m_lWidth;
	}

	internal void SetWidth(int RecordWidth)
	{
		if (RecordWidth < 0 || RecordWidth > 255)
		{
			throw ExceptionUtils.VbMakeException(5);
		}
		m_lWidth = RecordWidth;
	}

	internal virtual void WriteLine(string s)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void WriteString(string s)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual bool EOF()
	{
		return m_eof;
	}

	internal long LOF()
	{
		return m_file.Length;
	}

	internal virtual long LOC()
	{
		checked
		{
			if (m_lRecordLen == -1 || GetMode() != OpenMode.Random)
			{
				return m_position + 1;
			}
			if (m_lRecordLen == 0)
			{
				throw ExceptionUtils.VbMakeException(51);
			}
			if (m_position == 0L)
			{
				return 0L;
			}
			return unchecked(m_position / m_lRecordLen) + 1;
		}
	}

	internal void SetRecord(long RecordNumber)
	{
		if (m_lRecordLen == 0 || RecordNumber == 0L)
		{
			return;
		}
		checked
		{
			long num = default(long);
			if (m_lRecordLen == -1)
			{
				if (RecordNumber == -1)
				{
					return;
				}
				num = RecordNumber - 1;
			}
			else
			{
				switch (RecordNumber)
				{
				case -1L:
					num = GetPos();
					if (num == 0L)
					{
						m_lRecordStart = 0L;
						return;
					}
					if (unchecked(num % m_lRecordLen) == 0L)
					{
						m_lRecordStart = num;
						return;
					}
					num = m_lRecordLen * (unchecked(num / m_lRecordLen) + 1);
					break;
				default:
					num = ((m_lRecordLen != -1) ? ((RecordNumber - 1) * m_lRecordLen) : RecordNumber);
					break;
				case 0L:
					break;
				}
			}
			SeekOffset(num);
			m_lRecordStart = num;
		}
	}

	internal virtual void Seek(long BaseOnePosition)
	{
		if (BaseOnePosition <= 0)
		{
			throw ExceptionUtils.VbMakeException(63);
		}
		long num = checked(BaseOnePosition - 1);
		if (num > m_file.Length)
		{
			m_file.SetLength(num);
		}
		m_file.Position = num;
		m_position = num;
		m_eof = m_position >= m_file.Length;
		if (m_sr != null)
		{
			m_sr.DiscardBufferedData();
		}
	}

	internal virtual long Seek()
	{
		return checked(m_position + 1);
	}

	internal void SeekOffset(long offset)
	{
		m_position = offset;
		m_file.Position = offset;
		if (m_sr != null)
		{
			m_sr.DiscardBufferedData();
		}
	}

	internal long GetPos()
	{
		return m_position;
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("macos")]
	[UnsupportedOSPlatform("tvos")]
	internal virtual void Lock()
	{
		m_file.Lock(0L, 2147483647L);
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("macos")]
	[UnsupportedOSPlatform("tvos")]
	internal virtual void Unlock()
	{
		m_file.Unlock(0L, 2147483647L);
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("macos")]
	[UnsupportedOSPlatform("tvos")]
	internal virtual void Lock(long Record)
	{
		checked
		{
			if (m_lRecordLen == -1)
			{
				m_file.Lock(Record - 1, 1L);
			}
			else
			{
				m_file.Lock((Record - 1) * m_lRecordLen, m_lRecordLen);
			}
		}
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("macos")]
	[UnsupportedOSPlatform("tvos")]
	internal virtual void Unlock(long Record)
	{
		checked
		{
			if (m_lRecordLen == -1)
			{
				m_file.Unlock(Record - 1, 1L);
			}
			else
			{
				m_file.Unlock((Record - 1) * m_lRecordLen, m_lRecordLen);
			}
		}
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("macos")]
	[UnsupportedOSPlatform("tvos")]
	internal virtual void Lock(long RecordStart, long RecordEnd)
	{
		checked
		{
			if (m_lRecordLen == -1)
			{
				m_file.Lock(RecordStart - 1, RecordEnd - RecordStart + 1);
			}
			else
			{
				m_file.Lock((RecordStart - 1) * m_lRecordLen, (RecordEnd - RecordStart + 1) * m_lRecordLen);
			}
		}
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("macos")]
	[UnsupportedOSPlatform("tvos")]
	internal virtual void Unlock(long RecordStart, long RecordEnd)
	{
		checked
		{
			if (m_lRecordLen == -1)
			{
				m_file.Unlock(RecordStart - 1, RecordEnd - RecordStart + 1);
			}
			else
			{
				m_file.Unlock((RecordStart - 1) * m_lRecordLen, (RecordEnd - RecordStart + 1) * m_lRecordLen);
			}
		}
	}

	internal string LineInput()
	{
		ValidateReadable();
		string text = m_sr.ReadLine();
		if (text == null)
		{
			text = "";
		}
		checked
		{
			m_position += m_Encoding.GetByteCount(text) + 2;
			m_eof = CheckEOF(m_sr.Peek());
			return text;
		}
	}

	internal virtual bool CanInput()
	{
		return false;
	}

	protected virtual string InputStr()
	{
		ValidateReadable();
		checked
		{
			string result;
			if (SkipWhiteSpaceEOF() == 34)
			{
				m_sr.Read();
				m_position++;
				result = ReadInField(1);
			}
			else
			{
				result = ReadInField(2);
			}
			SkipTrailingWhiteSpace();
			return result;
		}
	}

	protected virtual object InputNum(VariantType vt)
	{
		ValidateReadable();
		SkipWhiteSpaceEOF();
		object result = ReadInField(3);
		SkipTrailingWhiteSpace();
		return result;
	}

	public abstract OpenMode GetMode();

	internal string InputString(int lLen)
	{
		ValidateReadable();
		StringBuilder stringBuilder = new StringBuilder(lLen);
		OpenMode mode = GetMode();
		checked
		{
			for (int i = 1; i <= lLen; i++)
			{
				int num;
				if (mode == OpenMode.Binary)
				{
					num = m_br.Read();
					m_position++;
					if (num == -1)
					{
						break;
					}
				}
				else
				{
					if (mode != OpenMode.Input)
					{
						throw ExceptionUtils.VbMakeException(54);
					}
					num = m_sr.Read();
					m_position++;
					if ((num == -1) | (num == 26))
					{
						m_eof = true;
						throw ExceptionUtils.VbMakeException(62);
					}
				}
				if (num != 0)
				{
					stringBuilder.Append(Strings.ChrW(num));
				}
			}
			if (mode == OpenMode.Binary)
			{
				m_eof = m_br.PeekChar() == -1;
			}
			else
			{
				m_eof = CheckEOF(m_sr.Peek());
			}
			return stringBuilder.ToString();
		}
	}

	internal void SPC(int iCount)
	{
		if (iCount <= 0)
		{
			return;
		}
		int num = GetColumn();
		int width = GetWidth();
		checked
		{
			if (width != 0)
			{
				if (iCount >= width)
				{
					iCount = unchecked(iCount % width);
				}
				if (iCount + num > width)
				{
					iCount -= width - num;
					goto IL_0036;
				}
			}
			iCount += num;
			if (iCount < num)
			{
				goto IL_0036;
			}
			goto IL_003f;
		}
		IL_0036:
		WriteLine(null);
		num = 0;
		goto IL_003f;
		IL_003f:
		if (iCount > num)
		{
			string s = new string(' ', checked(iCount - num));
			WriteString(s);
		}
	}

	internal void Tab(int Column)
	{
		if (Column < 1)
		{
			Column = 1;
		}
		Column = checked(Column - 1);
		int num = GetColumn();
		int width = GetWidth();
		if (width != 0 && Column >= width)
		{
			Column %= width;
		}
		if (Column < num)
		{
			WriteLine(null);
			num = 0;
		}
		if (Column > num)
		{
			string s = new string(' ', checked(Column - num));
			WriteString(s);
		}
	}

	internal void SetPrintMode()
	{
		OpenMode mode = GetMode();
		if (mode == OpenMode.Input || mode == OpenMode.Binary || mode == OpenMode.Random)
		{
			throw ExceptionUtils.VbMakeException(54);
		}
		m_bPrint = true;
	}

	internal static VT VTType(object VarName)
	{
		if (VarName == null)
		{
			return VT.Variant;
		}
		return VTFromComType(VarName.GetType());
	}

	internal static VT VTFromComType(Type typ)
	{
		if ((object)typ == null)
		{
			return VT.Variant;
		}
		if (typ.IsArray)
		{
			typ = typ.GetElementType();
			if (typ.IsArray)
			{
				return (VT)8204;
			}
			VT vT = VTFromComType(typ);
			if ((vT & VT.Array) != VT.Empty)
			{
				return (VT)8204;
			}
			return vT | VT.Array;
		}
		if (typ.IsEnum)
		{
			typ = Enum.GetUnderlyingType(typ);
		}
		if ((object)typ == null)
		{
			return VT.Empty;
		}
		switch (Type.GetTypeCode(typ))
		{
		case TypeCode.String:
			return VT.String;
		case TypeCode.Int32:
			return VT.Integer;
		case TypeCode.Int16:
			return VT.Short;
		case TypeCode.Int64:
			return VT.Long;
		case TypeCode.Single:
			return VT.Single;
		case TypeCode.Double:
			return VT.Double;
		case TypeCode.DateTime:
			return VT.Date;
		case TypeCode.Boolean:
			return VT.Boolean;
		case TypeCode.Decimal:
			return VT.Decimal;
		case TypeCode.Byte:
			return VT.Byte;
		case TypeCode.Char:
			return VT.Char;
		case TypeCode.DBNull:
			return VT.DBNull;
		default:
			if ((object)typ == typeof(Missing))
			{
				return VT.Error;
			}
			if ((object)typ == typeof(Exception) || typ.IsSubclassOf(typeof(Exception)))
			{
				return VT.Error;
			}
			if (typ.IsValueType)
			{
				return VT.Structure;
			}
			return VT.Variant;
		}
	}

	[RequiresUnreferencedCode("Calls PutArrayData")]
	internal void PutFixedArray(long RecordNumber, Array arr, Type ElementType, int FixedStringLength = -1, int FirstBound = -1, int SecondBound = -1)
	{
		SetRecord(RecordNumber);
		if ((object)ElementType == null)
		{
			ElementType = arr.GetType().GetElementType();
		}
		PutArrayData(arr, ElementType, FixedStringLength, FirstBound, SecondBound);
	}

	[RequiresUnreferencedCode("Calls PutArrayData")]
	internal void PutDynamicArray(long RecordNumber, Array arr, bool ContainedInVariant = true, int FixedStringLength = -1)
	{
		int num;
		int upperBound = default(int);
		if (arr == null)
		{
			num = 0;
		}
		else
		{
			num = arr.Rank;
			upperBound = arr.GetUpperBound(0);
		}
		int secondBound = default(int);
		switch (num)
		{
		case 1:
			secondBound = -1;
			break;
		case 2:
			secondBound = arr.GetUpperBound(1);
			break;
		default:
			throw new ArgumentException(System.SR.Argument_UnsupportedArrayDimensions);
		case 0:
			break;
		}
		SetRecord(RecordNumber);
		if (ContainedInVariant)
		{
			VT vT = VTType(arr);
			m_bw.Write((short)vT);
			checked
			{
				m_position += 2L;
				if ((vT & VT.Array) == 0)
				{
					throw ExceptionUtils.VbMakeException(458);
				}
			}
		}
		PutArrayDesc(arr);
		if (num != 0)
		{
			PutArrayData(arr, arr.GetType().GetElementType(), FixedStringLength, upperBound, secondBound);
		}
	}

	internal void LengthCheck(int Length)
	{
		if (m_lRecordLen != -1)
		{
			if (Length > m_lRecordLen)
			{
				throw ExceptionUtils.VbMakeException(59);
			}
			if (checked(GetPos() + Length > m_lRecordStart + m_lRecordLen))
			{
				throw ExceptionUtils.VbMakeException(59);
			}
		}
	}

	internal void PutFixedLengthString(long RecordNumber, string s, int lengthToWrite)
	{
		char character = ' ';
		if (s == null)
		{
			s = "";
		}
		if (Operators.CompareString(s, "", TextCompare: false) == 0)
		{
			character = '\0';
		}
		int byteCount = m_Encoding.GetByteCount(s);
		checked
		{
			if (byteCount > lengthToWrite)
			{
				if (byteCount == s.Length)
				{
					s = Strings.Left(s, lengthToWrite);
				}
				else
				{
					byte[] bytes = m_Encoding.GetBytes(s);
					s = m_Encoding.GetString(bytes, 0, lengthToWrite);
					byteCount = m_Encoding.GetByteCount(s);
					if (byteCount > lengthToWrite)
					{
						for (int i = lengthToWrite - 1; i >= 0; i += -1)
						{
							bytes[i] = 0;
							s = m_Encoding.GetString(bytes, 0, lengthToWrite);
							byteCount = m_Encoding.GetByteCount(s);
							if (byteCount <= lengthToWrite)
							{
								break;
							}
						}
					}
				}
			}
			if (byteCount < lengthToWrite)
			{
				s += Strings.StrDup(lengthToWrite - byteCount, character);
			}
			SetRecord(RecordNumber);
			LengthCheck(lengthToWrite);
			m_sw.Write(s);
			m_position += lengthToWrite;
		}
	}

	internal void PutVariantString(long RecordNumber, string s)
	{
		if (s == null)
		{
			s = "";
		}
		int byteCount = m_Encoding.GetByteCount(s);
		SetRecord(RecordNumber);
		checked
		{
			LengthCheck(byteCount + 2 + 2);
			m_bw.Write((short)8);
			m_bw.Write((short)byteCount);
			if (byteCount != 0)
			{
				m_sw.Write(s);
			}
			m_position += byteCount + 2 + 2;
		}
	}

	internal void PutString(long RecordNumber, string s)
	{
		if (s == null)
		{
			s = "";
		}
		int byteCount = m_Encoding.GetByteCount(s);
		SetRecord(RecordNumber);
		LengthCheck(byteCount);
		if (byteCount != 0)
		{
			m_sw.Write(s);
		}
		checked
		{
			m_position += byteCount;
		}
	}

	internal void PutStringWithLength(long RecordNumber, string s)
	{
		if (s == null)
		{
			s = "";
		}
		int byteCount = m_Encoding.GetByteCount(s);
		SetRecord(RecordNumber);
		checked
		{
			LengthCheck(byteCount + 2);
			m_bw.Write((short)byteCount);
			if (byteCount != 0)
			{
				m_sw.Write(s);
			}
			m_position += byteCount + 2;
		}
	}

	internal void PutDate(long RecordNumber, DateTime dt, bool ContainedInVariant = false)
	{
		int num = 8;
		checked
		{
			if (ContainedInVariant)
			{
				num += 2;
			}
			SetRecord(RecordNumber);
			LengthCheck(num);
			if (ContainedInVariant)
			{
				m_bw.Write((short)7);
			}
			double value = dt.ToOADate();
			m_bw.Write(value);
			m_position += num;
		}
	}

	internal void PutShort(long RecordNumber, short i, bool ContainedInVariant = false)
	{
		int num = 2;
		checked
		{
			if (ContainedInVariant)
			{
				num += 2;
			}
			SetRecord(RecordNumber);
			LengthCheck(num);
			if (ContainedInVariant)
			{
				m_bw.Write((short)2);
			}
			m_bw.Write(i);
			m_position += num;
		}
	}

	internal void PutInteger(long RecordNumber, int l, bool ContainedInVariant = false)
	{
		int num = 4;
		checked
		{
			if (ContainedInVariant)
			{
				num += 2;
			}
			SetRecord(RecordNumber);
			LengthCheck(num);
			if (ContainedInVariant)
			{
				m_bw.Write((short)3);
			}
			m_bw.Write(l);
			m_position += num;
		}
	}

	internal void PutLong(long RecordNumber, long l, bool ContainedInVariant = false)
	{
		int num = 8;
		checked
		{
			if (ContainedInVariant)
			{
				num += 2;
			}
			SetRecord(RecordNumber);
			LengthCheck(num);
			if (ContainedInVariant)
			{
				m_bw.Write((short)20);
			}
			m_bw.Write(l);
			m_position += num;
		}
	}

	internal void PutByte(long RecordNumber, byte byt, bool ContainedInVariant = false)
	{
		int num = 1;
		checked
		{
			if (ContainedInVariant)
			{
				num += 2;
			}
			SetRecord(RecordNumber);
			LengthCheck(num);
			if (ContainedInVariant)
			{
				m_bw.Write((short)17);
			}
			m_bw.Write(byt);
			m_position += num;
		}
	}

	internal void PutChar(long RecordNumber, char ch, bool ContainedInVariant = false)
	{
		int num = 2;
		checked
		{
			if (ContainedInVariant)
			{
				num += 2;
			}
			SetRecord(RecordNumber);
			LengthCheck(num);
			if (ContainedInVariant)
			{
				m_bw.Write((short)18);
			}
			m_bw.Write(ch);
			m_position += num;
		}
	}

	internal void PutSingle(long RecordNumber, float sng, bool ContainedInVariant = false)
	{
		int num = 4;
		checked
		{
			if (ContainedInVariant)
			{
				num += 2;
			}
			SetRecord(RecordNumber);
			LengthCheck(num);
			if (ContainedInVariant)
			{
				m_bw.Write((short)4);
			}
			m_bw.Write(sng);
			m_position += num;
		}
	}

	internal void PutDouble(long RecordNumber, double dbl, bool ContainedInVariant = false)
	{
		int num = 8;
		checked
		{
			if (ContainedInVariant)
			{
				num += 2;
			}
			SetRecord(RecordNumber);
			LengthCheck(num);
			if (ContainedInVariant)
			{
				m_bw.Write((short)5);
			}
			m_bw.Write(dbl);
			m_position += num;
		}
	}

	internal void PutEmpty(long RecordNumber)
	{
		SetRecord(RecordNumber);
		LengthCheck(2);
		m_bw.Write((short)0);
		checked
		{
			m_position += 2L;
		}
	}

	internal void PutBoolean(long RecordNumber, bool b, bool ContainedInVariant = false)
	{
		int num = 2;
		checked
		{
			if (ContainedInVariant)
			{
				num += 2;
			}
			SetRecord(RecordNumber);
			LengthCheck(num);
			if (ContainedInVariant)
			{
				m_bw.Write((short)11);
			}
			if (b)
			{
				m_bw.Write((short)(-1));
			}
			else
			{
				m_bw.Write((short)0);
			}
			m_position += num;
		}
	}

	internal void PutDecimal(long RecordNumber, decimal dec, bool ContainedInVariant = false)
	{
		int num = 16;
		checked
		{
			if (ContainedInVariant)
			{
				num += 2;
			}
			SetRecord(RecordNumber);
			LengthCheck(num);
			if (ContainedInVariant)
			{
				m_bw.Write((short)14);
			}
			int[] bits = decimal.GetBits(dec);
			byte value = (byte)unchecked((bits[3] & 0x7FFFFFFF) / 65536);
			int value2 = bits[0];
			int value3 = bits[1];
			int value4 = bits[2];
			byte value5 = default(byte);
			if ((bits[3] & int.MinValue) != 0)
			{
				value5 = 128;
			}
			m_bw.Write((short)14);
			m_bw.Write(value);
			m_bw.Write(value5);
			m_bw.Write(value4);
			m_bw.Write(value2);
			m_bw.Write(value3);
			m_position += num;
		}
	}

	internal void PutCurrency(long RecordNumber, decimal dec, bool ContainedInVariant = false)
	{
		int num = 16;
		checked
		{
			if (ContainedInVariant)
			{
				num += 2;
			}
			SetRecord(RecordNumber);
			LengthCheck(num);
			if (ContainedInVariant)
			{
				m_bw.Write((short)6);
			}
			m_bw.Write(decimal.ToOACurrency(dec));
			m_position += num;
		}
	}

	[RequiresUnreferencedCode("Calls EnumerateUDT")]
	internal void PutRecord(long RecordNumber, ValueType o)
	{
		if (o == null)
		{
			throw new NullReferenceException();
		}
		SetRecord(RecordNumber);
		IRecordEnum recordEnum = new PutHandler(this);
		if (recordEnum == null)
		{
			throw ExceptionUtils.VbMakeException(5);
		}
		StructUtils.EnumerateUDT(o, recordEnum, fGet: false);
	}

	internal Type ComTypeFromVT(VT vtype)
	{
		return vtype switch
		{
			VT.Variant => typeof(object), 
			VT.Empty => null, 
			VT.DBNull => typeof(DBNull), 
			VT.Short => typeof(short), 
			VT.Integer => typeof(int), 
			VT.Long => typeof(long), 
			VT.Single => typeof(float), 
			VT.Double => typeof(double), 
			VT.Date => typeof(DateTime), 
			VT.String => typeof(string), 
			VT.Error => typeof(Exception), 
			VT.Boolean => typeof(bool), 
			VT.Decimal => typeof(decimal), 
			VT.Byte => typeof(byte), 
			VT.Char => typeof(char), 
			_ => throw ExceptionUtils.VbMakeException(458), 
		};
	}

	[RequiresUnreferencedCode("Calls GetArrayData")]
	internal void GetFixedArray(long RecordNumber, ref Array arr, Type FieldType, int FirstBound = -1, int SecondBound = -1, int FixedStringLength = -1)
	{
		checked
		{
			if (SecondBound == -1)
			{
				arr = Array.CreateInstance(FieldType, FirstBound + 1);
			}
			else
			{
				arr = Array.CreateInstance(FieldType, FirstBound + 1, SecondBound + 1);
			}
			SetRecord(RecordNumber);
			GetArrayData(arr, FieldType, FirstBound, SecondBound, FixedStringLength);
		}
	}

	[RequiresUnreferencedCode("Calls GetArrayData")]
	internal void GetDynamicArray(ref Array arr, Type t, int FixedStringLength = -1)
	{
		arr = GetArrayDesc(t);
		int rank = arr.Rank;
		GetArrayData(FirstBound: arr.GetUpperBound(0), SecondBound: (rank != 1) ? arr.GetUpperBound(1) : (-1), arr: arr, typ: t, FixedStringLength: FixedStringLength);
	}

	private void PutArrayDesc(Array arr)
	{
		short num = (short)((arr != null) ? checked((short)arr.Rank) : 0);
		m_bw.Write(num);
		checked
		{
			m_position += 2L;
			if (num != 0)
			{
				int num2 = num - 1;
				for (int i = 0; i <= num2; i++)
				{
					m_bw.Write(arr.GetLength(i));
					m_bw.Write(arr.GetLowerBound(i));
					m_position += 8L;
				}
			}
		}
	}

	internal Array GetArrayDesc(Type typ)
	{
		int num = m_br.ReadInt16();
		checked
		{
			m_position += 2L;
			if (num == 0)
			{
				return Array.CreateInstance(typ, 0);
			}
			int[] array = new int[num - 1 + 1];
			int[] array2 = new int[num - 1 + 1];
			int num2 = num - 1;
			for (int i = 0; i <= num2; i++)
			{
				array[i] = m_br.ReadInt32();
				array2[i] = m_br.ReadInt32();
				m_position += 8L;
			}
			return Array.CreateInstance(typ, array, array2);
		}
	}

	internal virtual string GetLengthPrefixedString(long RecordNumber)
	{
		SetRecord(RecordNumber);
		if (EOF())
		{
			return "";
		}
		return ReadString();
	}

	internal virtual string GetFixedLengthString(long RecordNumber, int ByteLength)
	{
		SetRecord(RecordNumber);
		return ReadString(ByteLength);
	}

	protected string ReadString(int ByteLength)
	{
		if (ByteLength == 0)
		{
			return null;
		}
		byte[] bytes = m_br.ReadBytes(ByteLength);
		checked
		{
			m_position += ByteLength;
			return m_Encoding.GetString(bytes);
		}
	}

	protected string ReadString()
	{
		int num = m_br.ReadInt16();
		checked
		{
			m_position += 2L;
			if (num == 0)
			{
				return null;
			}
			LengthCheck(num);
			return ReadString(num);
		}
	}

	internal DateTime GetDate(long RecordNumber)
	{
		SetRecord(RecordNumber);
		double d = m_br.ReadDouble();
		checked
		{
			m_position += 8L;
			return DateTime.FromOADate(d);
		}
	}

	internal short GetShort(long RecordNumber)
	{
		SetRecord(RecordNumber);
		short result = m_br.ReadInt16();
		checked
		{
			m_position += 2L;
			return result;
		}
	}

	internal int GetInteger(long RecordNumber)
	{
		SetRecord(RecordNumber);
		int result = m_br.ReadInt32();
		checked
		{
			m_position += 4L;
			return result;
		}
	}

	internal long GetLong(long RecordNumber)
	{
		SetRecord(RecordNumber);
		long result = m_br.ReadInt64();
		checked
		{
			m_position += 8L;
			return result;
		}
	}

	internal byte GetByte(long RecordNumber)
	{
		SetRecord(RecordNumber);
		byte result = m_br.ReadByte();
		checked
		{
			m_position++;
			return result;
		}
	}

	internal char GetChar(long RecordNumber)
	{
		SetRecord(RecordNumber);
		char result = m_br.ReadChar();
		checked
		{
			m_position++;
			return result;
		}
	}

	internal float GetSingle(long RecordNumber)
	{
		SetRecord(RecordNumber);
		float result = m_br.ReadSingle();
		checked
		{
			m_position += 4L;
			return result;
		}
	}

	internal double GetDouble(long RecordNumber)
	{
		SetRecord(RecordNumber);
		double result = m_br.ReadDouble();
		checked
		{
			m_position += 8L;
			return result;
		}
	}

	internal decimal GetDecimal(long RecordNumber)
	{
		SetRecord(RecordNumber);
		m_br.ReadInt16();
		byte scale = m_br.ReadByte();
		byte num = m_br.ReadByte();
		int hi = m_br.ReadInt32();
		int lo = m_br.ReadInt32();
		int mid = m_br.ReadInt32();
		checked
		{
			m_position += 16L;
			bool isNegative = default(bool);
			if (num != 0)
			{
				isNegative = true;
			}
			return new decimal(lo, mid, hi, isNegative, scale);
		}
	}

	internal decimal GetCurrency(long RecordNumber)
	{
		SetRecord(RecordNumber);
		long cy = m_br.ReadInt64();
		checked
		{
			m_position += 8L;
			return decimal.FromOACurrency(cy);
		}
	}

	internal bool GetBoolean(long RecordNumber)
	{
		SetRecord(RecordNumber);
		short num = m_br.ReadInt16();
		checked
		{
			m_position += 2L;
			if (num == 0)
			{
				return false;
			}
			return true;
		}
	}

	[RequiresUnreferencedCode("Calls EnumerateUDT")]
	internal void GetRecord(long RecordNumber, ref ValueType o, bool ContainedInVariant = false)
	{
		if (o == null)
		{
			throw new NullReferenceException();
		}
		SetRecord(RecordNumber);
		IRecordEnum recordEnum = new GetHandler(this);
		if (recordEnum == null)
		{
			throw ExceptionUtils.VbMakeException(5);
		}
		StructUtils.EnumerateUDT(o, recordEnum, fGet: true);
	}

	[RequiresUnreferencedCode("Calls PutObject")]
	internal void PutArrayData(Array arr, Type typ, int FixedStringLength, int FirstBound, int SecondBound)
	{
		string text = null;
		char[] buffer = null;
		int num = default(int);
		int num2 = default(int);
		if (arr == null)
		{
			num = -1;
			num2 = -1;
		}
		else if (arr.GetUpperBound(0) > FirstBound)
		{
			throw new ArgumentException(System.SR.Argument_ArrayDimensionsDontMatch);
		}
		if ((object)typ == null)
		{
			typ = arr.GetType().GetElementType();
		}
		VT vT = VTFromComType(typ);
		int num3;
		int num4;
		if (SecondBound == -1)
		{
			num3 = 0;
			num4 = FirstBound;
			if (arr != null)
			{
				num = arr.GetUpperBound(0);
			}
		}
		else
		{
			num3 = SecondBound;
			num4 = FirstBound;
			if (arr != null)
			{
				if (arr.Rank != 2 || arr.GetUpperBound(1) != SecondBound)
				{
					throw new ArgumentException(System.SR.Argument_ArrayDimensionsDontMatch);
				}
				num = arr.GetUpperBound(0);
				num2 = arr.GetUpperBound(1);
			}
		}
		if (vT == VT.String)
		{
			if (FixedStringLength == 0)
			{
				object obj = ((SecondBound != -1) ? arr.GetValue(0, 0) : arr.GetValue(0));
				if (obj != null)
				{
					FixedStringLength = obj.ToString().Length;
				}
			}
			if (FixedStringLength == 0)
			{
				throw new ArgumentException(System.SR.Argument_InvalidFixedLengthString);
			}
			if (FixedStringLength > 0)
			{
				text = Strings.StrDup(FixedStringLength, ' ');
				buffer = text.ToCharArray();
			}
		}
		int byteLength = GetByteLength(vT);
		checked
		{
			if (SecondBound == -1 && byteLength > 0 && num4 == num)
			{
				int num5 = byteLength * (num4 + 1);
				if (GetPos() + num5 <= m_lRecordStart + m_lRecordLen)
				{
					byte[] array = new byte[num5 - 1 + 1];
					Buffer.BlockCopy(arr, 0, array, 0, num5);
					m_bw.Write(array);
					m_position += num5;
					return;
				}
			}
			int num6 = num3;
			for (int i = 0; i <= num6; i++)
			{
				int num7 = num4;
				for (int j = 0; j <= num7; j++)
				{
					object obj;
					try
					{
						obj = ((SecondBound == -1) ? ((j <= num) ? arr.GetValue(j) : null) : ((j <= num && i <= num2) ? arr.GetValue(j, i) : null));
					}
					catch (IndexOutOfRangeException)
					{
						obj = 0;
					}
					switch (vT)
					{
					case VT.Byte:
						LengthCheck(1);
						m_bw.Write(ByteType.FromObject(obj));
						m_position++;
						break;
					case VT.Short:
						LengthCheck(2);
						m_bw.Write(ShortType.FromObject(obj));
						m_position += 2L;
						break;
					case VT.Boolean:
						LengthCheck(2);
						if (BooleanType.FromObject(obj))
						{
							m_bw.Write((short)(-1));
						}
						else
						{
							m_bw.Write((short)0);
						}
						m_position += 2L;
						break;
					case VT.Integer:
						LengthCheck(4);
						m_bw.Write(IntegerType.FromObject(obj));
						m_position += 4L;
						break;
					case VT.Long:
						LengthCheck(8);
						m_bw.Write(LongType.FromObject(obj));
						m_position += 8L;
						break;
					case VT.Single:
						LengthCheck(4);
						m_bw.Write(SingleType.FromObject(obj));
						m_position += 4L;
						break;
					case VT.Error:
						throw ExceptionUtils.VbMakeException(13);
					case VT.Double:
						LengthCheck(8);
						m_bw.Write(DoubleType.FromObject(obj));
						m_position += 8L;
						break;
					case VT.Date:
						LengthCheck(8);
						m_bw.Write(DateType.FromObject(obj).ToOADate());
						m_position += 8L;
						break;
					case VT.Decimal:
						LengthCheck(8);
						m_bw.Write(decimal.ToOACurrency(DecimalType.FromObject(obj)));
						m_position += 8L;
						break;
					case VT.String:
					{
						string text2;
						int num8;
						if (obj == null)
						{
							if (FixedStringLength > 0)
							{
								text2 = text;
								num8 = FixedStringLength;
							}
							else
							{
								text2 = "";
								num8 = 0;
							}
						}
						else
						{
							text2 = obj.ToString();
							num8 = m_Encoding.GetByteCount(text2);
							if (FixedStringLength > 0 && num8 > FixedStringLength)
							{
								if (num8 == text2.Length)
								{
									text2 = Strings.Left(text2, FixedStringLength);
									num8 = FixedStringLength;
								}
								else
								{
									byte[] bytes = m_Encoding.GetBytes(text2);
									text2 = m_Encoding.GetString(bytes, 0, FixedStringLength);
									num8 = m_Encoding.GetByteCount(text2);
								}
							}
						}
						if (num8 > 32767)
						{
							throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.FileIO_StringLengthExceeded), 5);
						}
						if (FixedStringLength > 0)
						{
							LengthCheck(FixedStringLength);
							m_sw.Write(text2);
							if (num8 < FixedStringLength)
							{
								m_sw.Write(buffer, 0, FixedStringLength - num8);
							}
							m_position += FixedStringLength;
						}
						else
						{
							LengthCheck(num8 + 2);
							m_bw.Write((short)num8);
							m_sw.Write(text2);
							m_position += 2 + num8;
						}
						break;
					}
					case VT.Char:
						LengthCheck(2);
						m_bw.Write(CharType.FromObject(obj));
						m_position += 2L;
						break;
					case VT.Variant:
						PutObject(obj, 0L);
						break;
					case VT.Structure:
						PutObject(obj, 0L, ContainedInVariant: false);
						break;
					default:
						if ((vT & VT.Array) != VT.Empty)
						{
							throw ExceptionUtils.VbMakeException(13);
						}
						throw ExceptionUtils.VbMakeException(458);
					case VT.Empty:
					case VT.DBNull:
						break;
					}
				}
			}
		}
	}

	[RequiresUnreferencedCode("Calls GetObject")]
	internal void GetArrayData(Array arr, Type typ, int FirstBound = -1, int SecondBound = -1, int FixedStringLength = -1)
	{
		object Value = null;
		if (arr == null)
		{
			throw new ArgumentException(System.SR.Argument_ArrayNotInitialized);
		}
		if ((object)typ == null)
		{
			typ = arr.GetType().GetElementType();
		}
		VT vT = VTFromComType(typ);
		int num;
		int num2;
		if (SecondBound == -1)
		{
			num = 0;
			num2 = FirstBound;
		}
		else
		{
			num = SecondBound;
			num2 = FirstBound;
		}
		int byteLength = GetByteLength(vT);
		checked
		{
			if (SecondBound == -1 && byteLength > 0 && num2 == arr.GetUpperBound(0))
			{
				int num3 = byteLength * (num2 + 1);
				if (num3 <= arr.Length * byteLength)
				{
					Buffer.BlockCopy(m_br.ReadBytes(num3), 0, arr, 0, num3);
					m_position += num3;
					return;
				}
			}
			int num4 = num;
			for (int i = 0; i <= num4; i++)
			{
				int num5 = num2;
				for (int j = 0; j <= num5; j++)
				{
					switch (vT)
					{
					case VT.Byte:
						Value = m_br.ReadByte();
						m_position++;
						break;
					case VT.Short:
						Value = m_br.ReadInt16();
						m_position += 2L;
						break;
					case VT.Boolean:
						Value = m_br.ReadInt16() != 0;
						m_position += 2L;
						break;
					case VT.Integer:
						Value = m_br.ReadInt32();
						m_position += 4L;
						break;
					case VT.Long:
						Value = m_br.ReadInt64();
						m_position += 8L;
						break;
					case VT.Single:
						Value = m_br.ReadSingle();
						m_position += 4L;
						break;
					case VT.Double:
						Value = m_br.ReadDouble();
						m_position += 8L;
						break;
					case VT.Date:
						Value = DateTime.FromOADate(m_br.ReadDouble());
						m_position += 8L;
						break;
					case VT.Decimal:
					{
						long cy = m_br.ReadInt64();
						m_position += 8L;
						Value = decimal.FromOACurrency(cy);
						break;
					}
					case VT.String:
						Value = ((FixedStringLength < 0) ? ReadString() : ReadString(FixedStringLength));
						break;
					case VT.Char:
						Value = m_br.ReadChar();
						m_position++;
						break;
					case VT.Variant:
						Value = ((SecondBound != -1) ? arr.GetValue(j, i) : arr.GetValue(j));
						GetObject(ref Value, 0L);
						break;
					case VT.Structure:
						Value = ((SecondBound != -1) ? arr.GetValue(j, i) : arr.GetValue(j));
						GetObject(ref Value, 0L, ContainedInVariant: false);
						break;
					default:
						if ((vT & VT.Array) == 0)
						{
							throw ExceptionUtils.VbMakeException(458);
						}
						vT ^= VT.Array;
						if (vT == VT.Variant)
						{
							throw ExceptionUtils.VbMakeException(13);
						}
						if (vT > VT.Variant && vT != VT.Byte && vT != VT.Decimal && vT != VT.Char && vT != VT.Long)
						{
							throw ExceptionUtils.VbMakeException(458);
						}
						break;
					case VT.Empty:
					case VT.DBNull:
					case VT.Error:
						break;
					}
					try
					{
						if (SecondBound == -1)
						{
							arr.SetValue(Value, j);
						}
						else
						{
							arr.SetValue(Value, j, i);
						}
					}
					catch (IndexOutOfRangeException)
					{
						throw new ArgumentException(System.SR.Argument_ArrayDimensionsDontMatch);
					}
				}
			}
		}
	}

	private int GetByteLength(VT vtype)
	{
		return vtype switch
		{
			VT.Byte => 1, 
			VT.Short => 2, 
			VT.Integer => 4, 
			VT.Long => 8, 
			VT.Single => 4, 
			VT.Double => 8, 
			_ => -1, 
		};
	}

	private void PrintTab(TabInfo ti)
	{
		checked
		{
			if (ti.Column == -1)
			{
				int column = GetColumn();
				column += 14 - unchecked(column % 14);
				SetColumn(column);
			}
			else
			{
				Tab(ti.Column);
			}
		}
	}

	private string AddSpaces(string s)
	{
		string negativeSign = Thread.CurrentThread.CurrentCulture.NumberFormat.NegativeSign;
		if (negativeSign.Length == 1)
		{
			if (s[0] == negativeSign[0])
			{
				return s + " ";
			}
		}
		else if (Operators.CompareString(Strings.Left(s, negativeSign.Length), negativeSign, TextCompare: false) == 0)
		{
			return s + " ";
		}
		return " " + s + " ";
	}

	internal void PrintLine(params object[] Output)
	{
		Print(Output);
		WriteLine(null);
	}

	internal void Print(params object[] Output)
	{
		SetPrintMode();
		if (Output == null || Output.Length == 0)
		{
			return;
		}
		int upperBound = Output.GetUpperBound(0);
		int num = -1;
		int num2 = upperBound;
		checked
		{
			for (int i = 0; i <= num2; i++)
			{
				string text = null;
				object obj = Output[i];
				Type type;
				if (obj == null)
				{
					type = null;
				}
				else
				{
					type = obj.GetType();
					if (type.IsEnum)
					{
						type = Enum.GetUnderlyingType(type);
					}
				}
				if (obj == null)
				{
					text = "";
				}
				if ((object)type == null)
				{
					text = "";
				}
				else
				{
					switch (Type.GetTypeCode(type))
					{
					case TypeCode.String:
						text = obj.ToString();
						break;
					case TypeCode.Int16:
						text = AddSpaces(StringType.FromShort(ShortType.FromObject(obj)));
						break;
					case TypeCode.Int32:
						text = AddSpaces(StringType.FromInteger(IntegerType.FromObject(obj)));
						break;
					case TypeCode.Int64:
						text = AddSpaces(StringType.FromLong(LongType.FromObject(obj)));
						break;
					case TypeCode.Byte:
						text = AddSpaces(StringType.FromByte(ByteType.FromObject(obj)));
						break;
					case TypeCode.DateTime:
						text = StringType.FromDate(DateType.FromObject(obj)) + " ";
						break;
					case TypeCode.Double:
						text = AddSpaces(StringType.FromDouble(DoubleType.FromObject(obj)));
						break;
					case TypeCode.Single:
						text = AddSpaces(StringType.FromSingle(SingleType.FromObject(obj)));
						break;
					case TypeCode.Decimal:
						text = AddSpaces(StringType.FromDecimal(DecimalType.FromObject(obj)));
						break;
					case TypeCode.DBNull:
						text = "Null";
						break;
					case TypeCode.Boolean:
						text = StringType.FromBoolean(BooleanType.FromObject(obj));
						break;
					case TypeCode.Char:
						text = StringType.FromChar(CharType.FromObject(obj));
						break;
					default:
						if ((object)type == typeof(TabInfo))
						{
							PrintTab((obj != null) ? ((TabInfo)obj) : default(TabInfo));
							num = i;
							continue;
						}
						if ((object)type == typeof(SpcInfo))
						{
							SpcInfo obj2 = ((obj != null) ? ((SpcInfo)obj) : default(SpcInfo));
							SPC(obj2.Count);
							num = i;
							continue;
						}
						if ((object)type == typeof(Missing))
						{
							text = "Error 448";
							break;
						}
						throw new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedIOType1, Utils.VBFriendlyName(type)));
					}
				}
				if (num != i - 1)
				{
					int column = GetColumn();
					SetColumn(column + (14 - unchecked(column % 14)));
				}
				WriteString(text);
			}
		}
	}

	internal void WriteLineHelper(params object[] Output)
	{
		InternalWriteHelper(Output);
		WriteLine(null);
	}

	internal void WriteHelper(params object[] Output)
	{
		InternalWriteHelper(Output);
		WriteString(",");
	}

	private void InternalWriteHelper(params object[] Output)
	{
		Type typeFromHandle = typeof(SpcInfo);
		Type type = typeFromHandle;
		NumberFormatInfo numberFormat = Utils.GetInvariantCultureInfo().NumberFormat;
		int upperBound = Output.GetUpperBound(0);
		for (int i = 0; i <= upperBound; i = checked(i + 1))
		{
			object obj = Output[i];
			if (obj == null)
			{
				WriteString("#ERROR 448#");
				continue;
			}
			if ((object)type != typeFromHandle)
			{
				WriteString(",");
			}
			type = obj.GetType();
			if ((object)type == typeFromHandle)
			{
				SpcInfo obj2 = ((obj != null) ? ((SpcInfo)obj) : default(SpcInfo));
				SPC(obj2.Count);
				continue;
			}
			if ((object)type == typeof(TabInfo))
			{
				TabInfo ti = ((obj != null) ? ((TabInfo)obj) : default(TabInfo));
				if (ti.Column >= 0)
				{
					PrintTab(ti);
				}
				continue;
			}
			if ((object)type == typeof(Missing))
			{
				WriteString("#ERROR 448#");
				continue;
			}
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.String:
				WriteString(GetQuotedString(obj.ToString()));
				continue;
			case TypeCode.Int16:
				WriteString(StringType.FromShort(ShortType.FromObject(obj)));
				continue;
			case TypeCode.Int32:
				WriteString(StringType.FromInteger(IntegerType.FromObject(obj)));
				continue;
			case TypeCode.Int64:
				WriteString(StringType.FromLong(LongType.FromObject(obj)));
				continue;
			case TypeCode.Byte:
				WriteString(StringType.FromByte(ByteType.FromObject(obj)));
				continue;
			case TypeCode.DateTime:
				WriteString(FormatUniversalDate(DateType.FromObject(obj)));
				continue;
			case TypeCode.Double:
				WriteString(IOStrFromDouble(DoubleType.FromObject(obj), numberFormat));
				continue;
			case TypeCode.Single:
				WriteString(IOStrFromSingle(SingleType.FromObject(obj), numberFormat));
				continue;
			case TypeCode.Decimal:
				WriteString(IOStrFromDecimal(DecimalType.FromObject(obj), numberFormat));
				continue;
			case TypeCode.DBNull:
				WriteString("#NULL#");
				continue;
			case TypeCode.Boolean:
				if (BooleanType.FromObject(obj))
				{
					WriteString("#TRUE#");
				}
				else
				{
					WriteString("#FALSE#");
				}
				continue;
			case TypeCode.Char:
				WriteString(StringType.FromChar(CharType.FromObject(obj)));
				continue;
			}
			if (obj is char[] && ((Array)obj).Rank == 1)
			{
				WriteString(new string(CharArrayType.FromObject(obj)));
				continue;
			}
			throw ExceptionUtils.VbMakeException(5);
		}
	}

	private string IOStrFromSingle(float Value, NumberFormatInfo NumberFormat)
	{
		return Value.ToString(null, NumberFormat);
	}

	private string IOStrFromDouble(double Value, NumberFormatInfo NumberFormat)
	{
		return Value.ToString(null, NumberFormat);
	}

	private string IOStrFromDecimal(decimal Value, NumberFormatInfo NumberFormat)
	{
		return Value.ToString("G29", NumberFormat);
	}

	internal string FormatUniversalDate(DateTime dt)
	{
		string text = "T";
		bool flag = default(bool);
		if (dt.Year != 0 || dt.Month != 1 || dt.Day != 1)
		{
			flag = true;
			text = "d";
		}
		if (checked(dt.Hour + dt.Minute + dt.Second) != 0 && flag)
		{
			text = "F";
		}
		return dt.ToString(text, FileSystem.m_WriteDateFormatInfo);
	}

	protected string GetQuotedString(string Value)
	{
		return "\"" + Value.Replace("\"", "\"\"") + "\"";
	}

	[RequiresUnreferencedCode("VB6RandomFile implementation is unsafe.")]
	internal virtual void GetObject(ref object Value, long RecordNumber = 0L, bool ContainedInVariant = true)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	[RequiresUnreferencedCode("VB6RandomFile implementation is unsafe.")]
	internal virtual void Get(ref ValueType Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	[RequiresUnreferencedCode("VB6RandomFile implementation is unsafe.")]
	internal virtual void Get(ref Array Value, long RecordNumber = 0L, bool ArrayIsDynamic = false, bool StringIsFixedLength = false)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Get(ref bool Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Get(ref byte Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Get(ref short Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Get(ref int Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Get(ref long Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Get(ref char Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Get(ref float Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Get(ref double Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Get(ref decimal Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Get(ref string Value, long RecordNumber = 0L, bool StringIsFixedLength = false)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Get(ref DateTime Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	[RequiresUnreferencedCode("VB6RandomFile implementation is unsafe.")]
	internal virtual void PutObject(object Value, long RecordNumber = 0L, bool ContainedInVariant = true)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	[RequiresUnreferencedCode("VB6RandomFile implementation is unsafe.")]
	internal virtual void Put(ValueType Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	[RequiresUnreferencedCode("VB6RandomFile implementation is unsafe.")]
	internal virtual void Put(Array Value, long RecordNumber = 0L, bool ArrayIsDynamic = false, bool StringIsFixedLength = false)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Put(bool Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Put(byte Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Put(short Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Put(int Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Put(long Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Put(char Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Put(float Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Put(double Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Put(decimal Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Put(string Value, long RecordNumber = 0L, bool StringIsFixedLength = false)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Put(DateTime Value, long RecordNumber = 0L)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	[RequiresUnreferencedCode("Implementation of Vb6InputFile is unsafe.")]
	internal virtual void Input(ref object obj)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Input(ref bool Value)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Input(ref byte Value)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Input(ref short Value)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Input(ref int Value)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Input(ref long Value)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Input(ref char Value)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Input(ref float Value)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Input(ref double Value)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Input(ref decimal Value)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Input(ref string Value)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	internal virtual void Input(ref DateTime Value)
	{
		throw ExceptionUtils.VbMakeException(54);
	}

	protected int SkipWhiteSpace()
	{
		int num = m_sr.Peek();
		checked
		{
			if (CheckEOF(num))
			{
				m_eof = true;
			}
			else
			{
				while (IntlIsSpace(num) || num == 9)
				{
					m_sr.Read();
					m_position++;
					num = m_sr.Peek();
					if (CheckEOF(num))
					{
						m_eof = true;
						break;
					}
				}
			}
			return num;
		}
	}

	private string GetFileInTerm(short iTermType)
	{
		return iTermType switch
		{
			6 => " ,\t\r", 
			0 => "\r", 
			1 => "\"", 
			2 => ",\r", 
			3 => " ,\t\r", 
			_ => throw ExceptionUtils.VbMakeException(5), 
		};
	}

	protected bool IntlIsSpace(int lch)
	{
		return (lch == 32) | (lch == 12288);
	}

	protected bool IntlIsDoubleQuote(int lch)
	{
		return lch == 34;
	}

	protected bool IntlIsComma(int lch)
	{
		return lch == 44;
	}

	protected int SkipWhiteSpaceEOF()
	{
		int num = SkipWhiteSpace();
		if (CheckEOF(num))
		{
			throw ExceptionUtils.VbMakeException(62);
		}
		return num;
	}

	protected void SkipTrailingWhiteSpace()
	{
		int num = m_sr.Peek();
		if (CheckEOF(num))
		{
			m_eof = true;
			return;
		}
		checked
		{
			if (IntlIsSpace(num) || IntlIsDoubleQuote(num) || num == 9)
			{
				num = m_sr.Read();
				m_position++;
				num = m_sr.Peek();
				if (CheckEOF(num))
				{
					m_eof = true;
					return;
				}
				while (IntlIsSpace(num) || num == 9)
				{
					m_sr.Read();
					m_position++;
					num = m_sr.Peek();
					if (CheckEOF(num))
					{
						m_eof = true;
						return;
					}
				}
			}
			if (num == 13)
			{
				num = m_sr.Read();
				m_position++;
				if (CheckEOF(num))
				{
					m_eof = true;
					return;
				}
				if (m_sr.Peek() == 10)
				{
					num = m_sr.Read();
					m_position++;
				}
			}
			else if (IntlIsComma(num))
			{
				num = m_sr.Read();
				m_position++;
			}
			num = m_sr.Peek();
			if (CheckEOF(num))
			{
				m_eof = true;
			}
		}
	}

	protected string ReadInField(short iTermType)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string fileInTerm = GetFileInTerm(iTermType);
		int num = m_sr.Peek();
		checked
		{
			if (CheckEOF(num))
			{
				m_eof = true;
			}
			else
			{
				while (!fileInTerm.Contains(Strings.ChrW(num)))
				{
					num = m_sr.Read();
					m_position++;
					if (num != 0)
					{
						stringBuilder.Append(Strings.ChrW(num));
					}
					num = m_sr.Peek();
					if (CheckEOF(num))
					{
						m_eof = true;
						break;
					}
				}
			}
			if (iTermType == 2 || iTermType == 3)
			{
				return Strings.RTrim(stringBuilder.ToString());
			}
			return stringBuilder.ToString();
		}
	}

	protected bool CheckEOF(int lChar)
	{
		if (lChar != -1)
		{
			return lChar == 26;
		}
		return true;
	}

	private void ValidateReadable()
	{
		if (m_access != OpenAccess.ReadWrite && m_access != OpenAccess.Read)
		{
			throw new NullReferenceException(new NullReferenceException().Message, new IOException(System.SR.FileOpenedNoRead));
		}
	}
}
