using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class VB6InputFile : VB6File
{
	public VB6InputFile(string FileName, OpenShare share)
		: base(FileName, OpenAccess.Read, share, -1)
	{
	}

	internal override void OpenFile()
	{
		try
		{
			m_file = new FileStream(m_sFullPath, FileMode.Open, (FileAccess)m_access, (FileShare)m_share);
		}
		catch (FileNotFoundException ex)
		{
			throw ExceptionUtils.VbMakeException(ex, 53);
		}
		catch (SecurityException)
		{
			throw ExceptionUtils.VbMakeException(53);
		}
		catch (DirectoryNotFoundException ex3)
		{
			throw ExceptionUtils.VbMakeException(ex3, 76);
		}
		catch (IOException ex4)
		{
			throw ExceptionUtils.VbMakeException(ex4, 75);
		}
		catch (StackOverflowException ex5)
		{
			throw ex5;
		}
		catch (OutOfMemoryException ex6)
		{
			throw ex6;
		}
		catch (Exception ex7)
		{
			throw ExceptionUtils.VbMakeException(ex7, 76);
		}
		m_Encoding = Utils.GetFileIOEncoding();
		m_sr = new StreamReader(m_file, m_Encoding, detectEncodingFromByteOrderMarks: false, 128);
		m_eof = m_file.Length == 0;
	}

	internal override bool CanInput()
	{
		return true;
	}

	internal override bool EOF()
	{
		return m_eof;
	}

	public override OpenMode GetMode()
	{
		return OpenMode.Input;
	}

	internal object ParseInputString(ref string sInput)
	{
		object result = sInput;
		if (sInput[0] == '#' && sInput.Length != 1)
		{
			sInput = sInput.Substring(1, checked(sInput.Length - 2));
			if (Operators.CompareString(sInput, "NULL", TextCompare: false) == 0)
			{
				result = DBNull.Value;
			}
			else if (Operators.CompareString(sInput, "TRUE", TextCompare: false) == 0)
			{
				result = true;
			}
			else if (Operators.CompareString(sInput, "FALSE", TextCompare: false) == 0)
			{
				result = false;
			}
			else if (Operators.CompareString(Strings.Left(sInput, 6), "ERROR ", TextCompare: false) == 0)
			{
				int num = default(int);
				if (sInput.Length > 6)
				{
					num = IntegerType.FromString(Strings.Mid(sInput, 7));
				}
				result = num;
			}
			else
			{
				try
				{
					result = DateTime.Parse(Utils.ToHalfwidthNumbers(sInput, Utils.GetCultureInfo()));
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
				}
			}
		}
		return result;
	}

	[RequiresUnreferencedCode("Calls Conversion.ParseInputField")]
	internal override void Input(ref object obj)
	{
		checked
		{
			switch (SkipWhiteSpaceEOF())
			{
			case 34:
			{
				int num = m_sr.Read();
				m_position++;
				obj = ReadInField(1);
				SkipTrailingWhiteSpace();
				break;
			}
			case 35:
			{
				string sInput = InputStr();
				obj = ParseInputString(ref sInput);
				break;
			}
			default:
			{
				string value = ReadInField(3);
				obj = Conversion.ParseInputField(value, VariantType.Empty);
				SkipTrailingWhiteSpace();
				break;
			}
			}
		}
	}

	internal override void Input(ref bool Value)
	{
		string sInput = InputStr();
		Value = BooleanType.FromObject(ParseInputString(ref sInput));
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

	internal override void Input(ref float Value)
	{
		Value = SingleType.FromObject(InputNum(VariantType.Single), Utils.GetInvariantCultureInfo().NumberFormat);
	}

	internal override void Input(ref double Value)
	{
		Value = DoubleType.FromObject(InputNum(VariantType.Double), Utils.GetInvariantCultureInfo().NumberFormat);
	}

	internal override void Input(ref decimal Value)
	{
		Value = DecimalType.FromObject(InputNum(VariantType.Decimal), Utils.GetInvariantCultureInfo().NumberFormat);
	}

	internal override void Input(ref string Value)
	{
		Value = InputStr();
	}

	internal override void Input(ref DateTime Value)
	{
		string sInput = InputStr();
		Value = DateType.FromObject(ParseInputString(ref sInput));
	}

	internal override long LOC()
	{
		return checked(m_position + 127) / 128;
	}
}
