using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Versioning;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class VB6BinaryFile : VB6RandomFile
{
	public VB6BinaryFile(string FileName, OpenAccess access, OpenShare share)
		: base(FileName, access, share, -1)
	{
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
		long num = ((m_lRecordLen != -1) ? m_lRecordLen : 1);
		checked
		{
			long position = (lStart - 1) * num;
			long length = (lEnd - lStart + 1) * num;
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
		long num = ((m_lRecordLen != -1) ? m_lRecordLen : 1);
		checked
		{
			long position = (lStart - 1) * num;
			long length = (lEnd - lStart + 1) * num;
			m_file.Unlock(position, length);
		}
	}

	public override OpenMode GetMode()
	{
		return OpenMode.Binary;
	}

	internal override long Seek()
	{
		return checked(m_position + 1);
	}

	internal override void Seek(long BaseOnePosition)
	{
		if (BaseOnePosition <= 0)
		{
			throw ExceptionUtils.VbMakeException(63);
		}
		long position = checked(BaseOnePosition - 1);
		m_file.Position = position;
		m_position = position;
		if (m_sr != null)
		{
			m_sr.DiscardBufferedData();
		}
	}

	internal override long LOC()
	{
		return m_position;
	}

	internal override bool CanInput()
	{
		return true;
	}

	[RequiresUnreferencedCode("Implementation of Vb6InputFile is unsafe.")]
	internal override void Input(ref object Value)
	{
		Value = InputStr();
	}

	internal override void Input(ref string Value)
	{
		Value = InputStr();
	}

	internal override void Input(ref char Value)
	{
		string text = InputStr();
		if (text.Length > 0)
		{
			Value = text[0];
		}
		else
		{
			Value = '\0';
		}
	}

	internal override void Input(ref bool Value)
	{
		Value = BooleanType.FromString(InputStr());
	}

	internal override void Input(ref byte Value)
	{
		Value = ByteType.FromObject(InputNum(VariantType.Byte));
	}

	internal override void Input(ref short Value)
	{
		Value = ShortType.FromObject(InputNum(VariantType.Short));
	}

	internal override void Input(ref int Value)
	{
		Value = IntegerType.FromObject(InputNum(VariantType.Integer));
	}

	internal override void Input(ref long Value)
	{
		Value = LongType.FromObject(InputNum(VariantType.Long));
	}

	internal override void Input(ref float Value)
	{
		Value = SingleType.FromObject(InputNum(VariantType.Single));
	}

	internal override void Input(ref double Value)
	{
		Value = DoubleType.FromObject(InputNum(VariantType.Double));
	}

	internal override void Input(ref decimal Value)
	{
		Value = DecimalType.FromObject(InputNum(VariantType.Decimal));
	}

	internal override void Input(ref DateTime Value)
	{
		Value = DateType.FromString(InputStr(), Utils.GetCultureInfo());
	}

	internal override void Put(string Value, long RecordNumber = 0L, bool StringIsFixedLength = false)
	{
		ValidateWriteable();
		PutString(RecordNumber, Value);
	}

	internal override void Get(ref string Value, long RecordNumber = 0L, bool StringIsFixedLength = false)
	{
		ValidateReadable();
		int byteLength = ((Value != null) ? m_Encoding.GetByteCount(Value) : 0);
		Value = GetFixedLengthString(RecordNumber, byteLength);
	}

	protected override string InputStr()
	{
		if (m_access != OpenAccess.ReadWrite && m_access != OpenAccess.Read)
		{
			throw new NullReferenceException(new NullReferenceException().Message, new IOException(System.SR.FileOpenedNoRead));
		}
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
}
