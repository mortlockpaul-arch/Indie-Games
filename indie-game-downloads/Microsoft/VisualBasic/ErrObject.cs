using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.VisualBasic;

public sealed class ErrObject
{
	private Exception m_curException;

	private int m_curErl;

	private int m_curNumber;

	private string m_curDescription;

	private bool m_NumberIsSet;

	private bool m_ClearOnCapture;

	private bool m_DescriptionIsSet;

	private string m_curSource;

	private bool m_SourceIsSet;

	private string m_curHelpFile;

	private int m_curHelpContext;

	private bool m_HelpFileIsSet;

	private bool m_HelpContextIsSet;

	public int Erl => m_curErl;

	public int Number
	{
		get
		{
			if (m_NumberIsSet)
			{
				return m_curNumber;
			}
			if (m_curException != null)
			{
				Number = MapExceptionToNumber(m_curException);
				return m_curNumber;
			}
			return 0;
		}
		set
		{
			m_curNumber = MapErrorNumber(value);
			m_NumberIsSet = true;
		}
	}

	public string Source
	{
		get
		{
			if (m_SourceIsSet)
			{
				return m_curSource;
			}
			if (m_curException != null)
			{
				Source = m_curException.Source;
				return m_curSource;
			}
			return "";
		}
		set
		{
			m_curSource = value;
			m_SourceIsSet = true;
		}
	}

	public string Description
	{
		get
		{
			if (m_DescriptionIsSet)
			{
				return m_curDescription;
			}
			if (m_curException != null)
			{
				Description = FilterDefaultMessage(m_curException.Message);
				return m_curDescription;
			}
			return "";
		}
		set
		{
			m_curDescription = value;
			m_DescriptionIsSet = true;
		}
	}

	public string HelpFile
	{
		get
		{
			if (m_HelpFileIsSet)
			{
				return m_curHelpFile;
			}
			if (m_curException != null)
			{
				ParseHelpLink(m_curException.HelpLink);
				return m_curHelpFile;
			}
			return "";
		}
		set
		{
			m_curHelpFile = value;
			m_HelpFileIsSet = true;
		}
	}

	public int HelpContext
	{
		get
		{
			if (m_HelpContextIsSet)
			{
				return m_curHelpContext;
			}
			if (m_curException != null)
			{
				ParseHelpLink(m_curException.HelpLink);
				return m_curHelpContext;
			}
			return 0;
		}
		set
		{
			m_curHelpContext = value;
			m_HelpContextIsSet = true;
		}
	}

	public int LastDllError => Marshal.GetLastWin32Error();

	internal ErrObject()
	{
		Clear();
	}

	private string FilterDefaultMessage(string Msg)
	{
		if (m_curException == null)
		{
			return Msg;
		}
		int number = Number;
		if (Msg == null || Msg.Length == 0)
		{
			Msg = System.SR.GetResourceString("ID" + Conversions.ToString(number));
		}
		else if (string.CompareOrdinal("Exception from HRESULT: 0x", 0, Msg, 0, Math.Min(Msg.Length, 26)) == 0)
		{
			string resourceString = System.SR.GetResourceString("ID" + Conversions.ToString(m_curNumber));
			if (resourceString != null)
			{
				Msg = resourceString;
			}
		}
		return Msg;
	}

	private string MakeHelpLink(string HelpFile, int HelpContext)
	{
		return HelpFile + "#" + Conversions.ToString(HelpContext);
	}

	private void ParseHelpLink(string HelpLink)
	{
		if (HelpLink == null || HelpLink.Length == 0)
		{
			if (!m_HelpContextIsSet)
			{
				HelpContext = 0;
			}
			if (!m_HelpFileIsSet)
			{
				HelpFile = "";
			}
			return;
		}
		int num = Strings.m_InvariantCompareInfo.IndexOf(HelpLink, "#", CompareOptions.Ordinal);
		if (num != -1)
		{
			if (!m_HelpContextIsSet)
			{
				if (num < HelpLink.Length)
				{
					HelpContext = Conversions.ToInteger(HelpLink.Substring(checked(num + 1)));
				}
				else
				{
					HelpContext = 0;
				}
			}
			if (!m_HelpFileIsSet)
			{
				HelpFile = HelpLink.Substring(0, num);
			}
		}
		else
		{
			if (!m_HelpContextIsSet)
			{
				HelpContext = 0;
			}
			if (!m_HelpFileIsSet)
			{
				HelpFile = HelpLink;
			}
		}
	}

	public Exception GetException()
	{
		return m_curException;
	}

	public void Clear()
	{
		m_curException = null;
		m_curNumber = 0;
		m_curSource = "";
		m_curHelpFile = "";
		m_curHelpContext = 0;
		m_SourceIsSet = false;
		m_HelpFileIsSet = false;
		m_HelpContextIsSet = false;
		m_curDescription = "";
		m_curErl = 0;
		m_NumberIsSet = false;
		m_DescriptionIsSet = false;
		m_ClearOnCapture = true;
	}

	public void Raise(int Number, object Source = null, object Description = null, object HelpFile = null, object HelpContext = null)
	{
		if (Number == 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Number"));
		}
		this.Number = Number;
		if (Source != null)
		{
			this.Source = Conversions.ToString(Source);
		}
		else
		{
			string fullName = Assembly.GetCallingAssembly().FullName;
			int num = Strings.InStr(fullName, ",");
			if (num < 1)
			{
				this.Source = fullName;
			}
			else
			{
				this.Source = Strings.Left(fullName, checked(num - 1));
			}
		}
		if (HelpFile != null)
		{
			this.HelpFile = Conversions.ToString(HelpFile);
		}
		if (HelpContext != null)
		{
			this.HelpContext = Conversions.ToInteger(HelpContext);
		}
		if (Description != null)
		{
			this.Description = Conversions.ToString(Description);
		}
		else if (!m_DescriptionIsSet)
		{
			this.Description = Utils.GetResourceString((vbErrors)m_curNumber);
		}
		Exception ex = MapNumberToException(m_curNumber, m_curDescription);
		ex.Source = m_curSource;
		ex.HelpLink = MakeHelpLink(m_curHelpFile, m_curHelpContext);
		m_ClearOnCapture = false;
		throw ex;
	}

	internal void SetUnmappedError(int Number)
	{
		Clear();
		this.Number = Number;
		m_ClearOnCapture = false;
	}

	internal Exception CreateException(int Number, string Description)
	{
		Clear();
		this.Number = Number;
		if (Number == 0)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1, "Number"));
		}
		Exception result = MapNumberToException(m_curNumber, Description);
		m_ClearOnCapture = false;
		return result;
	}

	internal void CaptureException(Exception ex)
	{
		if (ex != m_curException)
		{
			if (m_ClearOnCapture)
			{
				Clear();
			}
			else
			{
				m_ClearOnCapture = true;
			}
			m_curException = ex;
		}
	}

	internal void CaptureException(Exception ex, int lErl)
	{
		CaptureException(ex);
		m_curErl = lErl;
	}

	private int MapExceptionToNumber(Exception e)
	{
		Type type = e.GetType();
		if ((object)type == typeof(IndexOutOfRangeException))
		{
			return 9;
		}
		if ((object)type == typeof(RankException))
		{
			return 9;
		}
		if ((object)type == typeof(DivideByZeroException))
		{
			return 11;
		}
		if ((object)type == typeof(OverflowException))
		{
			return 6;
		}
		if ((object)type == typeof(NotFiniteNumberException))
		{
			if (((NotFiniteNumberException)e).OffendingNumber == 0.0)
			{
				return 11;
			}
			return 6;
		}
		if ((object)type == typeof(NullReferenceException))
		{
			return 91;
		}
		if (e is AccessViolationException)
		{
			return -2147467261;
		}
		if ((object)type == typeof(InvalidCastException))
		{
			return 13;
		}
		if ((object)type == typeof(NotSupportedException))
		{
			return 13;
		}
		if ((object)type == typeof(SEHException))
		{
			return 99;
		}
		if ((object)type == typeof(DllNotFoundException))
		{
			return 53;
		}
		if ((object)type == typeof(EntryPointNotFoundException))
		{
			return 453;
		}
		if ((object)type == typeof(TypeLoadException))
		{
			return 429;
		}
		if ((object)type == typeof(OutOfMemoryException))
		{
			return 7;
		}
		if ((object)type == typeof(FormatException))
		{
			return 13;
		}
		if ((object)type == typeof(DirectoryNotFoundException))
		{
			return 76;
		}
		if ((object)type == typeof(IOException))
		{
			return 57;
		}
		if ((object)type == typeof(FileNotFoundException))
		{
			return 53;
		}
		if (e is MissingMemberException)
		{
			return 438;
		}
		if (e is InvalidOleVariantTypeException)
		{
			return 458;
		}
		return 5;
	}

	private Exception MapNumberToException(int Number, string Description)
	{
		bool VBDefinedError = false;
		return ExceptionUtils.BuildException(Number, Description, ref VBDefinedError);
	}

	internal int MapErrorNumber(int Number)
	{
		if (Number > 65535)
		{
			throw new ArgumentException(System.SR.Format(System.SR.Argument_InvalidValue1), "Number");
		}
		if (Number >= 0)
		{
			return Number;
		}
		if ((Number & 0x1FFF0000) == 655360)
		{
			return Number & 0xFFFF;
		}
		return Number;
	}
}
