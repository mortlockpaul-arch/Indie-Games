using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Security;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
internal class VB6RandomFile : VB6File
{
	public VB6RandomFile(string FileName, OpenAccess access, OpenShare share, int lRecordLen)
		: base(FileName, access, share, lRecordLen)
	{
	}

	private void OpenFileHelper(FileMode fm, OpenAccess fa)
	{
		try
		{
			m_file = new FileStream(m_sFullPath, fm, (FileAccess)fa, (FileShare)m_share);
		}
		catch (FileNotFoundException ex)
		{
			throw ExceptionUtils.VbMakeException(ex, 53);
		}
		catch (DirectoryNotFoundException ex2)
		{
			throw ExceptionUtils.VbMakeException(ex2, 76);
		}
		catch (SecurityException ex3)
		{
			throw ExceptionUtils.VbMakeException(ex3, 53);
		}
		catch (IOException ex4)
		{
			throw ExceptionUtils.VbMakeException(ex4, 75);
		}
		catch (UnauthorizedAccessException ex5)
		{
			throw ExceptionUtils.VbMakeException(ex5, 75);
		}
		catch (ArgumentException ex6)
		{
			throw ExceptionUtils.VbMakeException(ex6, 75);
		}
		catch (StackOverflowException ex7)
		{
			throw ex7;
		}
		catch (OutOfMemoryException ex8)
		{
			throw ex8;
		}
		catch (Exception)
		{
			throw ExceptionUtils.VbMakeException(51);
		}
	}

	internal override void OpenFile()
	{
		FileMode fm = (File.Exists(m_sFullPath) ? FileMode.Open : ((m_access != OpenAccess.Read) ? FileMode.Create : FileMode.OpenOrCreate));
		if (m_access == OpenAccess.Default)
		{
			m_access = OpenAccess.ReadWrite;
			try
			{
				OpenFileHelper(fm, m_access);
			}
			catch (StackOverflowException ex)
			{
				throw ex;
			}
			catch (OutOfMemoryException ex2)
			{
				throw ex2;
			}
			catch (Exception)
			{
				m_access = OpenAccess.Write;
				try
				{
					OpenFileHelper(fm, m_access);
				}
				catch (StackOverflowException ex4)
				{
					throw ex4;
				}
				catch (OutOfMemoryException ex5)
				{
					throw ex5;
				}
				catch (Exception)
				{
					m_access = OpenAccess.Read;
					OpenFileHelper(fm, m_access);
				}
			}
		}
		else
		{
			OpenFileHelper(fm, m_access);
		}
		m_Encoding = Utils.GetFileIOEncoding();
		Stream file = m_file;
		if (m_access == OpenAccess.Write || m_access == OpenAccess.ReadWrite)
		{
			m_sw = new StreamWriter(file, m_Encoding);
			m_sw.AutoFlush = true;
			m_bw = new BinaryWriter(file, m_Encoding);
		}
		if (m_access == OpenAccess.Read || m_access == OpenAccess.ReadWrite)
		{
			m_br = new BinaryReader(file, m_Encoding);
			if (GetMode() == OpenMode.Binary)
			{
				m_sr = new StreamReader(file, m_Encoding, detectEncodingFromByteOrderMarks: false, 128);
			}
		}
	}

	internal override void CloseFile()
	{
		if (m_sw != null)
		{
			m_sw.Flush();
		}
		CloseTheFile();
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("macos")]
	[UnsupportedOSPlatform("tvos")]
	internal override void Lock(long lStart, long lEnd)
	{
		if (lStart > lEnd)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Start"));
		}
		checked
		{
			long position = (lStart - 1) * m_lRecordLen;
			long length = (lEnd - lStart + 1) * m_lRecordLen;
			m_file.Lock(position, length);
		}
	}

	[UnsupportedOSPlatform("ios")]
	[UnsupportedOSPlatform("macos")]
	[UnsupportedOSPlatform("tvos")]
	internal override void Unlock(long lStart, long lEnd)
	{
		if (lStart > lEnd)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Start"));
		}
		checked
		{
			long position = (lStart - 1) * m_lRecordLen;
			long length = (lEnd - lStart + 1) * m_lRecordLen;
			m_file.Unlock(position, length);
		}
	}

	public override OpenMode GetMode()
	{
		return OpenMode.Random;
	}

	internal override bool EOF()
	{
		m_eof = m_position >= m_file.Length;
		return m_eof;
	}

	internal override long LOC()
	{
		if (m_lRecordLen == 0)
		{
			throw ExceptionUtils.VbMakeException(51);
		}
		return checked(m_position + m_lRecordLen - 1) / m_lRecordLen;
	}

	internal override void Seek(long Position)
	{
		SetRecord(Position);
	}

	internal override long Seek()
	{
		return checked(LOC() + 1);
	}

	[RequiresUnreferencedCode("Calls GetRecord")]
	internal override void GetObject(ref object Value, long RecordNumber = 0L, bool ContainedInVariant = true)
	{
		Type type = null;
		ValidateReadable();
		SetRecord(RecordNumber);
		VT vT;
		if (ContainedInVariant)
		{
			vT = (VT)m_br.ReadInt16();
			checked
			{
				m_position += 2L;
			}
		}
		else
		{
			type = Value.GetType();
			vT = Type.GetTypeCode(type) switch
			{
				TypeCode.String => VT.String, 
				TypeCode.Int16 => VT.Short, 
				TypeCode.Int32 => VT.Integer, 
				TypeCode.Int64 => VT.Long, 
				TypeCode.Byte => VT.Byte, 
				TypeCode.DateTime => VT.Date, 
				TypeCode.Double => VT.Double, 
				TypeCode.Single => VT.Single, 
				TypeCode.Decimal => VT.Decimal, 
				TypeCode.Boolean => VT.Boolean, 
				TypeCode.Char => VT.Char, 
				TypeCode.Object => (!type.IsValueType) ? VT.Variant : VT.Structure, 
				_ => VT.Variant, 
			};
		}
		if ((vT & VT.Array) != VT.Empty)
		{
			Array arr = null;
			VT vtype = vT ^ VT.Array;
			GetDynamicArray(ref arr, ComTypeFromVT(vtype));
			Value = arr;
			return;
		}
		switch (vT)
		{
		case VT.String:
			Value = GetLengthPrefixedString(0L);
			return;
		case VT.Short:
			Value = GetShort(0L);
			return;
		case VT.Integer:
			Value = GetInteger(0L);
			return;
		case VT.Long:
			Value = GetLong(0L);
			return;
		case VT.Byte:
			Value = GetByte(0L);
			return;
		case VT.Date:
			Value = GetDate(0L);
			return;
		case VT.Double:
			Value = GetDouble(0L);
			return;
		case VT.Single:
			Value = GetSingle(0L);
			return;
		case VT.Currency:
			Value = GetCurrency(0L);
			return;
		case VT.Decimal:
			Value = GetDecimal(0L);
			return;
		case VT.Boolean:
			Value = GetBoolean(0L);
			return;
		case VT.Char:
			Value = GetChar(0L);
			return;
		case VT.Structure:
		{
			ValueType o = (ValueType)Value;
			GetRecord(0L, ref o);
			Value = o;
			return;
		}
		case VT.DBNull:
			if (ContainedInVariant)
			{
				Value = DBNull.Value;
				return;
			}
			break;
		}
		switch (vT)
		{
		case VT.DBNull:
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedIOType1, "DBNull")), 5);
		case VT.Empty:
			Value = null;
			break;
		case VT.Currency:
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedIOType1, "Currency")), 5);
		default:
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedIOType1, type.FullName)), 5);
		}
	}

	[RequiresUnreferencedCode("Calls GetRecord")]
	internal override void Get(ref ValueType Value, long RecordNumber = 0L)
	{
		ValidateReadable();
		GetRecord(RecordNumber, ref Value);
	}

	[RequiresUnreferencedCode("Calls GetFixedArray and GetArrayData")]
	internal override void Get(ref Array Value, long RecordNumber = 0L, bool ArrayIsDynamic = false, bool StringIsFixedLength = false)
	{
		ValidateReadable();
		if (Value == null)
		{
			throw new ArgumentException(System.SR.Argument_ArrayNotInitialized);
		}
		Type elementType = Value.GetType().GetElementType();
		int num = -1;
		int rank = Value.Rank;
		int num2 = -1;
		int secondBound = -1;
		SetRecord(RecordNumber);
		if (m_file.Position >= m_file.Length)
		{
			return;
		}
		if (StringIsFixedLength && (object)elementType == typeof(string))
		{
			object obj = rank switch
			{
				1 => Value.GetValue(0), 
				2 => Value.GetValue(0, 0), 
				_ => throw new ArgumentException(System.SR.Argument_UnsupportedArrayDimensions), 
			};
			num = ((obj != null) ? ((string)obj).Length : 0);
			if (num == 0)
			{
				throw new ArgumentException(System.SR.Argument_InvalidFixedLengthString);
			}
		}
		if (ArrayIsDynamic)
		{
			Value = GetArrayDesc(elementType);
			rank = Value.Rank;
		}
		num2 = Value.GetUpperBound(0);
		switch (rank)
		{
		case 2:
			secondBound = Value.GetUpperBound(1);
			break;
		default:
			throw new ArgumentException(System.SR.Argument_UnsupportedArrayDimensions);
		case 1:
			break;
		}
		if (ArrayIsDynamic)
		{
			GetArrayData(Value, elementType, num2, secondBound, num);
		}
		else
		{
			GetFixedArray(RecordNumber, ref Value, elementType, num2, secondBound, num);
		}
	}

	internal override void Get(ref bool Value, long RecordNumber = 0L)
	{
		ValidateReadable();
		Value = GetBoolean(RecordNumber);
	}

	internal override void Get(ref byte Value, long RecordNumber = 0L)
	{
		ValidateReadable();
		Value = GetByte(RecordNumber);
	}

	internal override void Get(ref short Value, long RecordNumber = 0L)
	{
		ValidateReadable();
		Value = GetShort(RecordNumber);
	}

	internal override void Get(ref int Value, long RecordNumber = 0L)
	{
		ValidateReadable();
		Value = GetInteger(RecordNumber);
	}

	internal override void Get(ref long Value, long RecordNumber = 0L)
	{
		ValidateReadable();
		Value = GetLong(RecordNumber);
	}

	internal override void Get(ref char Value, long RecordNumber = 0L)
	{
		ValidateReadable();
		Value = GetChar(RecordNumber);
	}

	internal override void Get(ref float Value, long RecordNumber = 0L)
	{
		ValidateReadable();
		Value = GetSingle(RecordNumber);
	}

	internal override void Get(ref double Value, long RecordNumber = 0L)
	{
		ValidateReadable();
		Value = GetDouble(RecordNumber);
	}

	internal override void Get(ref decimal Value, long RecordNumber = 0L)
	{
		ValidateReadable();
		Value = GetCurrency(RecordNumber);
	}

	internal override void Get(ref string Value, long RecordNumber = 0L, bool StringIsFixedLength = false)
	{
		ValidateReadable();
		if (StringIsFixedLength)
		{
			int byteLength = ((Value != null) ? m_Encoding.GetByteCount(Value) : 0);
			Value = GetFixedLengthString(RecordNumber, byteLength);
		}
		else
		{
			Value = GetLengthPrefixedString(RecordNumber);
		}
	}

	internal override void Get(ref DateTime Value, long RecordNumber = 0L)
	{
		ValidateReadable();
		Value = GetDate(RecordNumber);
	}

	[RequiresUnreferencedCode("Calls PutRecord")]
	internal override void PutObject(object Value, long RecordNumber = 0L, bool ContainedInVariant = true)
	{
		ValidateWriteable();
		if (Value == null)
		{
			PutEmpty(RecordNumber);
			return;
		}
		Type type = Value.GetType();
		if ((object)type == null)
		{
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedIOType1, "Empty")), 5);
		}
		if (type.IsArray)
		{
			PutDynamicArray(RecordNumber, (Array)Value);
			return;
		}
		if (type.IsEnum)
		{
			type = Enum.GetUnderlyingType(type);
		}
		switch (Type.GetTypeCode(type))
		{
		case TypeCode.String:
			PutVariantString(RecordNumber, Value.ToString());
			return;
		case TypeCode.Int16:
			PutShort(RecordNumber, ShortType.FromObject(Value), ContainedInVariant);
			return;
		case TypeCode.Int32:
			PutInteger(RecordNumber, IntegerType.FromObject(Value), ContainedInVariant);
			return;
		case TypeCode.Int64:
			PutLong(RecordNumber, LongType.FromObject(Value), ContainedInVariant);
			return;
		case TypeCode.Byte:
			PutByte(RecordNumber, ByteType.FromObject(Value), ContainedInVariant);
			return;
		case TypeCode.DateTime:
			PutDate(RecordNumber, DateType.FromObject(Value), ContainedInVariant);
			return;
		case TypeCode.Double:
			PutDouble(RecordNumber, DoubleType.FromObject(Value), ContainedInVariant);
			return;
		case TypeCode.Single:
			PutSingle(RecordNumber, SingleType.FromObject(Value), ContainedInVariant);
			return;
		case TypeCode.Decimal:
			PutDecimal(RecordNumber, DecimalType.FromObject(Value), ContainedInVariant);
			return;
		case TypeCode.Boolean:
			PutBoolean(RecordNumber, BooleanType.FromObject(Value), ContainedInVariant);
			return;
		case TypeCode.Char:
			PutChar(RecordNumber, CharType.FromObject(Value), ContainedInVariant);
			return;
		case TypeCode.DBNull:
			PutShort(RecordNumber, 1);
			return;
		}
		if ((object)type == typeof(Missing))
		{
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedIOType1, "Missing")), 5);
		}
		if (type.IsValueType && !ContainedInVariant)
		{
			PutRecord(RecordNumber, (ValueType)Value);
			return;
		}
		if (ContainedInVariant && type.IsValueType)
		{
			throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_PutObjectOfValueType1, Utils.VBFriendlyName(type, Value))), 5);
		}
		throw ExceptionUtils.VbMakeException(new ArgumentException(System.SR.Format(System.SR.Argument_UnsupportedIOType1, Utils.VBFriendlyName(type, Value))), 5);
	}

	[RequiresUnreferencedCode("Calls PutRecord")]
	internal override void Put(ValueType Value, long RecordNumber = 0L)
	{
		ValidateWriteable();
		PutRecord(RecordNumber, Value);
	}

	[RequiresUnreferencedCode("Calls PutFixedArray and PutDynamicArray")]
	internal override void Put(Array Value, long RecordNumber = 0L, bool ArrayIsDynamic = false, bool StringIsFixedLength = false)
	{
		ValidateWriteable();
		if (Value == null)
		{
			PutEmpty(RecordNumber);
			return;
		}
		int upperBound = Value.GetUpperBound(0);
		int secondBound = -1;
		int fixedStringLength = -1;
		if (Value.Rank == 2)
		{
			secondBound = Value.GetUpperBound(1);
		}
		if (StringIsFixedLength)
		{
			fixedStringLength = 0;
		}
		Type elementType = Value.GetType().GetElementType();
		if (ArrayIsDynamic)
		{
			PutDynamicArray(RecordNumber, Value, ContainedInVariant: false, fixedStringLength);
		}
		else
		{
			PutFixedArray(RecordNumber, Value, elementType, fixedStringLength, upperBound, secondBound);
		}
	}

	internal override void Put(bool Value, long RecordNumber = 0L)
	{
		ValidateWriteable();
		PutBoolean(RecordNumber, Value);
	}

	internal override void Put(byte Value, long RecordNumber = 0L)
	{
		ValidateWriteable();
		PutByte(RecordNumber, Value);
	}

	internal override void Put(short Value, long RecordNumber = 0L)
	{
		ValidateWriteable();
		PutShort(RecordNumber, Value);
	}

	internal override void Put(int Value, long RecordNumber = 0L)
	{
		ValidateWriteable();
		PutInteger(RecordNumber, Value);
	}

	internal override void Put(long Value, long RecordNumber = 0L)
	{
		ValidateWriteable();
		PutLong(RecordNumber, Value);
	}

	internal override void Put(char Value, long RecordNumber = 0L)
	{
		ValidateWriteable();
		PutChar(RecordNumber, Value);
	}

	internal override void Put(float Value, long RecordNumber = 0L)
	{
		ValidateWriteable();
		PutSingle(RecordNumber, Value);
	}

	internal override void Put(double Value, long RecordNumber = 0L)
	{
		ValidateWriteable();
		PutDouble(RecordNumber, Value);
	}

	internal override void Put(decimal Value, long RecordNumber = 0L)
	{
		ValidateWriteable();
		PutCurrency(RecordNumber, Value);
	}

	internal override void Put(string Value, long RecordNumber = 0L, bool StringIsFixedLength = false)
	{
		ValidateWriteable();
		if (StringIsFixedLength)
		{
			PutString(RecordNumber, Value);
		}
		else
		{
			PutStringWithLength(RecordNumber, Value);
		}
	}

	internal override void Put(DateTime Value, long RecordNumber = 0L)
	{
		ValidateWriteable();
		PutDate(RecordNumber, Value);
	}

	protected void ValidateWriteable()
	{
		if (m_access != OpenAccess.ReadWrite && m_access != OpenAccess.Write)
		{
			throw ExceptionUtils.VbMakeExceptionEx(75, System.SR.FileOpenedNoWrite);
		}
	}

	protected void ValidateReadable()
	{
		if (m_access != OpenAccess.ReadWrite && m_access != OpenAccess.Read)
		{
			throw ExceptionUtils.VbMakeExceptionEx(75, System.SR.FileOpenedNoRead);
		}
	}
}
