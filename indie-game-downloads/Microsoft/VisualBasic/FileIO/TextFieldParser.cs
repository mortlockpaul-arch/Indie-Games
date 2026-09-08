using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.VisualBasic.FileIO;

public class TextFieldParser : IDisposable
{
	private delegate int ChangeBufferFunction();

	private bool m_Disposed;

	private TextReader m_Reader;

	private string[] m_CommentTokens;

	private long m_LineNumber;

	private bool m_EndOfData;

	private string m_ErrorLine;

	private long m_ErrorLineNumber;

	private FieldType m_TextFieldType;

	private int[] m_FieldWidths;

	private string[] m_Delimiters;

	private int[] m_FieldWidthsCopy;

	private string[] m_DelimitersCopy;

	private Regex m_DelimiterRegex;

	private Regex m_DelimiterWithEndCharsRegex;

	private int[] m_WhitespaceCodes;

	private Regex m_BeginQuotesRegex;

	private Regex m_WhiteSpaceRegEx;

	private bool m_TrimWhiteSpace;

	private int m_Position;

	private int m_PeekPosition;

	private int m_CharsRead;

	private bool m_NeedPropertyCheck;

	private char[] m_Buffer;

	private int m_LineLength;

	private bool m_HasFieldsEnclosedInQuotes;

	private string m_SpaceChars;

	private int m_MaxLineSize;

	private int m_MaxBufferSize;

	private bool m_LeaveOpen;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public string[] CommentTokens
	{
		get
		{
			return m_CommentTokens;
		}
		set
		{
			CheckCommentTokensForWhitespace(value);
			m_CommentTokens = value;
			m_NeedPropertyCheck = true;
		}
	}

	public bool EndOfData
	{
		get
		{
			if (m_EndOfData)
			{
				return m_EndOfData;
			}
			if ((m_Reader == null) | (m_Buffer == null))
			{
				m_EndOfData = true;
				return true;
			}
			if (PeekNextDataLine() != null)
			{
				return false;
			}
			m_EndOfData = true;
			return true;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public long LineNumber
	{
		get
		{
			if (m_LineNumber != -1 && ((m_Reader.Peek() == -1) & (m_Position == m_CharsRead)))
			{
				CloseReader();
			}
			return m_LineNumber;
		}
	}

	public string ErrorLine => m_ErrorLine;

	public long ErrorLineNumber => m_ErrorLineNumber;

	public FieldType TextFieldType
	{
		get
		{
			return m_TextFieldType;
		}
		set
		{
			ValidateFieldTypeEnumValue(value, "value");
			m_TextFieldType = value;
			m_NeedPropertyCheck = true;
		}
	}

	public int[] FieldWidths
	{
		get
		{
			return m_FieldWidths;
		}
		set
		{
			if (value != null)
			{
				ValidateFieldWidthsOnInput(value);
				m_FieldWidthsCopy = (int[])value.Clone();
			}
			else
			{
				m_FieldWidthsCopy = null;
			}
			m_FieldWidths = value;
			m_NeedPropertyCheck = true;
		}
	}

	public string[] Delimiters
	{
		get
		{
			return m_Delimiters;
		}
		set
		{
			if (value != null)
			{
				ValidateDelimiters(value);
				m_DelimitersCopy = (string[])value.Clone();
			}
			else
			{
				m_DelimitersCopy = null;
			}
			m_Delimiters = value;
			m_NeedPropertyCheck = true;
			m_BeginQuotesRegex = null;
		}
	}

	public bool TrimWhiteSpace
	{
		get
		{
			return m_TrimWhiteSpace;
		}
		set
		{
			m_TrimWhiteSpace = value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public bool HasFieldsEnclosedInQuotes
	{
		get
		{
			return m_HasFieldsEnclosedInQuotes;
		}
		set
		{
			m_HasFieldsEnclosedInQuotes = value;
		}
	}

	private Regex BeginQuotesRegex
	{
		get
		{
			if (m_BeginQuotesRegex == null)
			{
				string pattern = string.Format(CultureInfo.InvariantCulture, "\\G[{0}]*\"", WhitespacePattern);
				m_BeginQuotesRegex = new Regex(pattern);
			}
			return m_BeginQuotesRegex;
		}
	}

	private string EndQuotePattern => string.Format(CultureInfo.InvariantCulture, "\"[{0}]*", WhitespacePattern);

	private string WhitespaceCharacters
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			int[] whitespaceCodes = m_WhitespaceCodes;
			for (int i = 0; i < whitespaceCodes.Length; i = checked(i + 1))
			{
				char c = Strings.ChrW(whitespaceCodes[i]);
				if (!CharacterIsInDelimiter(c))
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}
	}

	private string WhitespacePattern
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			int[] whitespaceCodes = m_WhitespaceCodes;
			for (int i = 0; i < whitespaceCodes.Length; i = checked(i + 1))
			{
				int charCode = whitespaceCodes[i];
				char testCharacter = Strings.ChrW(charCode);
				if (!CharacterIsInDelimiter(testCharacter))
				{
					stringBuilder.Append("\\u" + charCode.ToString("X4", CultureInfo.InvariantCulture));
				}
			}
			return stringBuilder.ToString();
		}
	}

	public TextFieldParser(string path)
	{
		m_CommentTokens = Array.Empty<string>();
		m_LineNumber = 1L;
		m_ErrorLine = "";
		m_ErrorLineNumber = -1L;
		m_TextFieldType = FieldType.Delimited;
		m_WhitespaceCodes = new int[23]
		{
			9, 11, 12, 32, 133, 160, 5760, 8192, 8193, 8194,
			8195, 8196, 8197, 8198, 8199, 8200, 8201, 8202, 8203, 8232,
			8233, 12288, 65279
		};
		m_WhiteSpaceRegEx = new Regex("\\s");
		m_TrimWhiteSpace = true;
		m_NeedPropertyCheck = true;
		m_Buffer = new char[4096];
		m_HasFieldsEnclosedInQuotes = true;
		m_MaxLineSize = 10000000;
		m_MaxBufferSize = 10000000;
		InitializeFromPath(path, Encoding.UTF8, detectEncoding: true);
	}

	public TextFieldParser(string path, Encoding defaultEncoding)
	{
		m_CommentTokens = Array.Empty<string>();
		m_LineNumber = 1L;
		m_ErrorLine = "";
		m_ErrorLineNumber = -1L;
		m_TextFieldType = FieldType.Delimited;
		m_WhitespaceCodes = new int[23]
		{
			9, 11, 12, 32, 133, 160, 5760, 8192, 8193, 8194,
			8195, 8196, 8197, 8198, 8199, 8200, 8201, 8202, 8203, 8232,
			8233, 12288, 65279
		};
		m_WhiteSpaceRegEx = new Regex("\\s");
		m_TrimWhiteSpace = true;
		m_NeedPropertyCheck = true;
		m_Buffer = new char[4096];
		m_HasFieldsEnclosedInQuotes = true;
		m_MaxLineSize = 10000000;
		m_MaxBufferSize = 10000000;
		InitializeFromPath(path, defaultEncoding, detectEncoding: true);
	}

	public TextFieldParser(string path, Encoding defaultEncoding, bool detectEncoding)
	{
		m_CommentTokens = Array.Empty<string>();
		m_LineNumber = 1L;
		m_ErrorLine = "";
		m_ErrorLineNumber = -1L;
		m_TextFieldType = FieldType.Delimited;
		m_WhitespaceCodes = new int[23]
		{
			9, 11, 12, 32, 133, 160, 5760, 8192, 8193, 8194,
			8195, 8196, 8197, 8198, 8199, 8200, 8201, 8202, 8203, 8232,
			8233, 12288, 65279
		};
		m_WhiteSpaceRegEx = new Regex("\\s");
		m_TrimWhiteSpace = true;
		m_NeedPropertyCheck = true;
		m_Buffer = new char[4096];
		m_HasFieldsEnclosedInQuotes = true;
		m_MaxLineSize = 10000000;
		m_MaxBufferSize = 10000000;
		InitializeFromPath(path, defaultEncoding, detectEncoding);
	}

	public TextFieldParser(Stream stream)
	{
		m_CommentTokens = Array.Empty<string>();
		m_LineNumber = 1L;
		m_ErrorLine = "";
		m_ErrorLineNumber = -1L;
		m_TextFieldType = FieldType.Delimited;
		m_WhitespaceCodes = new int[23]
		{
			9, 11, 12, 32, 133, 160, 5760, 8192, 8193, 8194,
			8195, 8196, 8197, 8198, 8199, 8200, 8201, 8202, 8203, 8232,
			8233, 12288, 65279
		};
		m_WhiteSpaceRegEx = new Regex("\\s");
		m_TrimWhiteSpace = true;
		m_NeedPropertyCheck = true;
		m_Buffer = new char[4096];
		m_HasFieldsEnclosedInQuotes = true;
		m_MaxLineSize = 10000000;
		m_MaxBufferSize = 10000000;
		InitializeFromStream(stream, Encoding.UTF8, detectEncoding: true);
	}

	public TextFieldParser(Stream stream, Encoding defaultEncoding)
	{
		m_CommentTokens = Array.Empty<string>();
		m_LineNumber = 1L;
		m_ErrorLine = "";
		m_ErrorLineNumber = -1L;
		m_TextFieldType = FieldType.Delimited;
		m_WhitespaceCodes = new int[23]
		{
			9, 11, 12, 32, 133, 160, 5760, 8192, 8193, 8194,
			8195, 8196, 8197, 8198, 8199, 8200, 8201, 8202, 8203, 8232,
			8233, 12288, 65279
		};
		m_WhiteSpaceRegEx = new Regex("\\s");
		m_TrimWhiteSpace = true;
		m_NeedPropertyCheck = true;
		m_Buffer = new char[4096];
		m_HasFieldsEnclosedInQuotes = true;
		m_MaxLineSize = 10000000;
		m_MaxBufferSize = 10000000;
		InitializeFromStream(stream, defaultEncoding, detectEncoding: true);
	}

	public TextFieldParser(Stream stream, Encoding defaultEncoding, bool detectEncoding)
	{
		m_CommentTokens = Array.Empty<string>();
		m_LineNumber = 1L;
		m_ErrorLine = "";
		m_ErrorLineNumber = -1L;
		m_TextFieldType = FieldType.Delimited;
		m_WhitespaceCodes = new int[23]
		{
			9, 11, 12, 32, 133, 160, 5760, 8192, 8193, 8194,
			8195, 8196, 8197, 8198, 8199, 8200, 8201, 8202, 8203, 8232,
			8233, 12288, 65279
		};
		m_WhiteSpaceRegEx = new Regex("\\s");
		m_TrimWhiteSpace = true;
		m_NeedPropertyCheck = true;
		m_Buffer = new char[4096];
		m_HasFieldsEnclosedInQuotes = true;
		m_MaxLineSize = 10000000;
		m_MaxBufferSize = 10000000;
		InitializeFromStream(stream, defaultEncoding, detectEncoding);
	}

	public TextFieldParser(Stream stream, Encoding defaultEncoding, bool detectEncoding, bool leaveOpen)
	{
		m_CommentTokens = Array.Empty<string>();
		m_LineNumber = 1L;
		m_ErrorLine = "";
		m_ErrorLineNumber = -1L;
		m_TextFieldType = FieldType.Delimited;
		m_WhitespaceCodes = new int[23]
		{
			9, 11, 12, 32, 133, 160, 5760, 8192, 8193, 8194,
			8195, 8196, 8197, 8198, 8199, 8200, 8201, 8202, 8203, 8232,
			8233, 12288, 65279
		};
		m_WhiteSpaceRegEx = new Regex("\\s");
		m_TrimWhiteSpace = true;
		m_NeedPropertyCheck = true;
		m_Buffer = new char[4096];
		m_HasFieldsEnclosedInQuotes = true;
		m_MaxLineSize = 10000000;
		m_MaxBufferSize = 10000000;
		m_LeaveOpen = leaveOpen;
		InitializeFromStream(stream, defaultEncoding, detectEncoding);
	}

	public TextFieldParser(TextReader reader)
	{
		m_CommentTokens = Array.Empty<string>();
		m_LineNumber = 1L;
		m_ErrorLine = "";
		m_ErrorLineNumber = -1L;
		m_TextFieldType = FieldType.Delimited;
		m_WhitespaceCodes = new int[23]
		{
			9, 11, 12, 32, 133, 160, 5760, 8192, 8193, 8194,
			8195, 8196, 8197, 8198, 8199, 8200, 8201, 8202, 8203, 8232,
			8233, 12288, 65279
		};
		m_WhiteSpaceRegEx = new Regex("\\s");
		m_TrimWhiteSpace = true;
		m_NeedPropertyCheck = true;
		m_Buffer = new char[4096];
		m_HasFieldsEnclosedInQuotes = true;
		m_MaxLineSize = 10000000;
		m_MaxBufferSize = 10000000;
		if (reader == null)
		{
			throw ExceptionUtils.GetArgumentNullException("reader");
		}
		m_Reader = reader;
		ReadToBuffer();
	}

	public void SetDelimiters(params string[] delimiters)
	{
		Delimiters = delimiters;
	}

	public void SetFieldWidths(params int[] fieldWidths)
	{
		FieldWidths = fieldWidths;
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public string ReadLine()
	{
		if ((m_Reader == null) | (m_Buffer == null))
		{
			return null;
		}
		ChangeBufferFunction changeBuffer = ReadToBuffer;
		string text = ReadNextLine(ref m_Position, changeBuffer);
		if (text == null)
		{
			FinishReading();
			return null;
		}
		checked
		{
			m_LineNumber++;
			return text.TrimEnd(new char[2] { '\r', '\n' });
		}
	}

	public string[] ReadFields()
	{
		if ((m_Reader == null) | (m_Buffer == null))
		{
			return null;
		}
		ValidateReadyToRead();
		return m_TextFieldType switch
		{
			FieldType.FixedWidth => ParseFixedWidthLine(), 
			FieldType.Delimited => ParseDelimitedLine(), 
			_ => null, 
		};
	}

	public string PeekChars(int numberOfChars)
	{
		if (numberOfChars <= 0)
		{
			throw ExceptionUtils.GetArgumentExceptionWithArgName("numberOfChars", System.SR.TextFieldParser_NumberOfCharsMustBePositive);
		}
		if ((m_Reader == null) | (m_Buffer == null))
		{
			return null;
		}
		if (m_EndOfData)
		{
			return null;
		}
		string text = PeekNextDataLine();
		if (text == null)
		{
			m_EndOfData = true;
			return null;
		}
		text = text.TrimEnd(new char[2] { '\r', '\n' });
		if (text.Length < numberOfChars)
		{
			return text;
		}
		return new StringInfo(text).SubstringByTextElements(0, numberOfChars);
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public string ReadToEnd()
	{
		if ((m_Reader == null) | (m_Buffer == null))
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder(m_Buffer.Length);
		stringBuilder.Append(m_Buffer, m_Position, checked(m_CharsRead - m_Position));
		stringBuilder.Append(m_Reader.ReadToEnd());
		FinishReading();
		return stringBuilder.ToString();
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public void Close()
	{
		CloseReader();
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	void IDisposable.Dispose()
	{
		//ILSpy generated this explicit interface implementation from .override directive in Dispose
		this.Dispose();
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (!m_Disposed)
			{
				Close();
			}
			m_Disposed = true;
		}
	}

	private void ValidateFieldTypeEnumValue(FieldType value, string paramName)
	{
		if (value < FieldType.Delimited || value > FieldType.FixedWidth)
		{
			throw new InvalidEnumArgumentException(paramName, (int)value, typeof(FieldType));
		}
	}

	~TextFieldParser()
	{
		Dispose(disposing: false);
		base.Finalize();
	}

	private void CloseReader()
	{
		FinishReading();
		if (m_Reader != null)
		{
			if (!m_LeaveOpen)
			{
				m_Reader.Close();
			}
			m_Reader = null;
		}
	}

	private void FinishReading()
	{
		m_LineNumber = -1L;
		m_EndOfData = true;
		m_Buffer = null;
		m_DelimiterRegex = null;
		m_BeginQuotesRegex = null;
	}

	private void InitializeFromPath(string path, Encoding defaultEncoding, bool detectEncoding)
	{
		if (Operators.CompareString(path, "", TextCompare: false) == 0)
		{
			throw ExceptionUtils.GetArgumentNullException("path");
		}
		if (defaultEncoding == null)
		{
			throw ExceptionUtils.GetArgumentNullException("defaultEncoding");
		}
		string path2 = ValidatePath(path);
		FileStream stream = new FileStream(path2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		m_Reader = new StreamReader(stream, defaultEncoding, detectEncoding);
		ReadToBuffer();
	}

	private void InitializeFromStream(Stream stream, Encoding defaultEncoding, bool detectEncoding)
	{
		if (stream == null)
		{
			throw ExceptionUtils.GetArgumentNullException("stream");
		}
		if (!stream.CanRead)
		{
			throw ExceptionUtils.GetArgumentExceptionWithArgName("stream", System.SR.TextFieldParser_StreamNotReadable, "stream");
		}
		if (defaultEncoding == null)
		{
			throw ExceptionUtils.GetArgumentNullException("defaultEncoding");
		}
		m_Reader = new StreamReader(stream, defaultEncoding, detectEncoding);
		ReadToBuffer();
	}

	private string ValidatePath(string path)
	{
		string text = FileSystem.NormalizeFilePath(path, "path");
		if (!File.Exists(text))
		{
			throw new FileNotFoundException(System.SR.Format(System.SR.IO_FileNotFound_Path, text));
		}
		return text;
	}

	private bool IgnoreLine(string line)
	{
		if (line == null)
		{
			return false;
		}
		string text = line.Trim();
		if (text.Length == 0)
		{
			return true;
		}
		if (m_CommentTokens != null)
		{
			string[] commentTokens = m_CommentTokens;
			foreach (string text2 in commentTokens)
			{
				if (Operators.CompareString(text2, "", TextCompare: false) != 0)
				{
					if (text.StartsWith(text2, StringComparison.Ordinal))
					{
						return true;
					}
					if (line.StartsWith(text2, StringComparison.Ordinal))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private int ReadToBuffer()
	{
		m_Position = 0;
		int num = m_Buffer.Length;
		if (num > 4096)
		{
			num = 4096;
			m_Buffer = new char[checked(num - 1 + 1)];
		}
		m_CharsRead = m_Reader.Read(m_Buffer, 0, num);
		return m_CharsRead;
	}

	private int SlideCursorToStartOfBuffer()
	{
		checked
		{
			if (m_Position > 0)
			{
				int num = m_CharsRead - m_Position;
				Array.Copy(m_Buffer, m_Position, m_Buffer, 0, num);
				int num2 = m_Reader.Read(m_Buffer, num, m_Buffer.Length - num);
				m_CharsRead = num + num2;
				m_Position = 0;
				return num2;
			}
			return 0;
		}
	}

	private int IncreaseBufferSize()
	{
		m_PeekPosition = m_CharsRead;
		checked
		{
			if (m_CharsRead == m_Buffer.Length)
			{
				int num = m_Buffer.Length + 4096;
				if (num > m_MaxBufferSize)
				{
					throw ExceptionUtils.GetInvalidOperationException(System.SR.TextFieldParser_BufferExceededMaxSize);
				}
				char[] array = new char[num - 1 + 1];
				Array.Copy(m_Buffer, array, m_Buffer.Length);
				m_Buffer = array;
			}
			int num2 = m_Reader.Read(m_Buffer, m_CharsRead, m_Buffer.Length - m_CharsRead);
			m_CharsRead += num2;
			return num2;
		}
	}

	private string ReadNextDataLine()
	{
		ChangeBufferFunction changeBuffer = ReadToBuffer;
		checked
		{
			string text;
			do
			{
				text = ReadNextLine(ref m_Position, changeBuffer);
				m_LineNumber++;
			}
			while (IgnoreLine(text));
			if (text == null)
			{
				CloseReader();
			}
			return text;
		}
	}

	private string PeekNextDataLine()
	{
		ChangeBufferFunction changeBuffer = IncreaseBufferSize;
		SlideCursorToStartOfBuffer();
		m_PeekPosition = 0;
		string text;
		do
		{
			text = ReadNextLine(ref m_PeekPosition, changeBuffer);
		}
		while (IgnoreLine(text));
		return text;
	}

	private string ReadNextLine(ref int Cursor, ChangeBufferFunction ChangeBuffer)
	{
		if (Cursor == m_CharsRead && ChangeBuffer() == 0)
		{
			return null;
		}
		StringBuilder stringBuilder = null;
		checked
		{
			do
			{
				int num = Cursor;
				int num2 = m_CharsRead - 1;
				for (int i = num; i <= num2; i++)
				{
					char c = m_Buffer[i];
					if (!((Operators.CompareString(Conversions.ToString(c), "\r", TextCompare: false) == 0) | (Operators.CompareString(Conversions.ToString(c), "\n", TextCompare: false) == 0)))
					{
						continue;
					}
					if (stringBuilder != null)
					{
						stringBuilder.Append(m_Buffer, Cursor, i - Cursor + 1);
					}
					else
					{
						stringBuilder = new StringBuilder(i + 1);
						stringBuilder.Append(m_Buffer, Cursor, i - Cursor + 1);
					}
					Cursor = i + 1;
					if (Operators.CompareString(Conversions.ToString(c), "\r", TextCompare: false) == 0)
					{
						if (Cursor < m_CharsRead)
						{
							if (Operators.CompareString(Conversions.ToString(m_Buffer[Cursor]), "\n", TextCompare: false) == 0)
							{
								Cursor++;
								stringBuilder.Append("\n");
							}
						}
						else if (ChangeBuffer() > 0 && Operators.CompareString(Conversions.ToString(m_Buffer[Cursor]), "\n", TextCompare: false) == 0)
						{
							Cursor++;
							stringBuilder.Append("\n");
						}
					}
					return stringBuilder.ToString();
				}
				int num3 = m_CharsRead - Cursor;
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(num3 + 10);
				}
				stringBuilder.Append(m_Buffer, Cursor, num3);
			}
			while (ChangeBuffer() > 0);
			return stringBuilder.ToString();
		}
	}

	private string[] ParseDelimitedLine()
	{
		string text = ReadNextDataLine();
		if (text == null)
		{
			return null;
		}
		checked
		{
			long num = m_LineNumber - 1;
			int num2 = 0;
			List<string> list = new List<string>();
			int endOfLineIndex = GetEndOfLineIndex(text);
			while (num2 <= endOfLineIndex)
			{
				Match match = null;
				bool flag = false;
				if (m_HasFieldsEnclosedInQuotes)
				{
					match = BeginQuotesRegex.Match(text, num2);
					flag = match.Success;
				}
				string text2;
				if (flag)
				{
					num2 = match.Index + match.Length;
					QuoteDelimitedFieldBuilder quoteDelimitedFieldBuilder = new QuoteDelimitedFieldBuilder(m_DelimiterWithEndCharsRegex, m_SpaceChars);
					quoteDelimitedFieldBuilder.BuildField(text, num2);
					if (quoteDelimitedFieldBuilder.MalformedLine)
					{
						m_ErrorLine = text.TrimEnd(new char[2] { '\r', '\n' });
						m_ErrorLineNumber = num;
						throw new MalformedLineException(System.SR.Format(System.SR.TextFieldParser_MalFormedDelimitedLine, num.ToString(CultureInfo.InvariantCulture)), num);
					}
					if (quoteDelimitedFieldBuilder.FieldFinished)
					{
						text2 = quoteDelimitedFieldBuilder.Field;
						num2 = quoteDelimitedFieldBuilder.Index + quoteDelimitedFieldBuilder.DelimiterLength;
					}
					else
					{
						do
						{
							int length = text.Length;
							string text3 = ReadNextDataLine();
							if (text3 == null)
							{
								m_ErrorLine = text.TrimEnd(new char[2] { '\r', '\n' });
								m_ErrorLineNumber = num;
								throw new MalformedLineException(System.SR.Format(System.SR.TextFieldParser_MalFormedDelimitedLine, num.ToString(CultureInfo.InvariantCulture)), num);
							}
							if (text.Length + text3.Length > m_MaxLineSize)
							{
								m_ErrorLine = text.TrimEnd(new char[2] { '\r', '\n' });
								m_ErrorLineNumber = num;
								throw new MalformedLineException(System.SR.Format(System.SR.TextFieldParser_MaxLineSizeExceeded, num.ToString(CultureInfo.InvariantCulture)), num);
							}
							text += text3;
							endOfLineIndex = GetEndOfLineIndex(text);
							quoteDelimitedFieldBuilder.BuildField(text, length);
							if (quoteDelimitedFieldBuilder.MalformedLine)
							{
								m_ErrorLine = text.TrimEnd(new char[2] { '\r', '\n' });
								m_ErrorLineNumber = num;
								throw new MalformedLineException(System.SR.Format(System.SR.TextFieldParser_MalFormedDelimitedLine, num.ToString(CultureInfo.InvariantCulture)), num);
							}
						}
						while (!quoteDelimitedFieldBuilder.FieldFinished);
						text2 = quoteDelimitedFieldBuilder.Field;
						num2 = quoteDelimitedFieldBuilder.Index + quoteDelimitedFieldBuilder.DelimiterLength;
					}
					if (m_TrimWhiteSpace)
					{
						text2 = text2.Trim();
					}
					list.Add(text2);
					continue;
				}
				Match match2 = m_DelimiterRegex.Match(text, num2);
				if (match2.Success)
				{
					text2 = text.Substring(num2, match2.Index - num2);
					if (m_TrimWhiteSpace)
					{
						text2 = text2.Trim();
					}
					list.Add(text2);
					num2 = match2.Index + match2.Length;
					continue;
				}
				text2 = text.Substring(num2).TrimEnd(new char[2] { '\r', '\n' });
				if (m_TrimWhiteSpace)
				{
					text2 = text2.Trim();
				}
				list.Add(text2);
				break;
			}
			return list.ToArray();
		}
	}

	private string[] ParseFixedWidthLine()
	{
		string text = ReadNextDataLine();
		if (text == null)
		{
			return null;
		}
		text = text.TrimEnd(new char[2] { '\r', '\n' });
		StringInfo line = new StringInfo(text);
		checked
		{
			ValidateFixedWidthLine(line, m_LineNumber - 1);
			int num = 0;
			int num2 = m_FieldWidths.Length - 1;
			string[] array = new string[num2 + 1];
			int num3 = num2;
			for (int i = 0; i <= num3; i++)
			{
				array[i] = GetFixedWidthField(line, num, m_FieldWidths[i]);
				num += m_FieldWidths[i];
			}
			return array;
		}
	}

	private string GetFixedWidthField(StringInfo Line, int Index, int FieldLength)
	{
		string text = ((FieldLength > 0) ? Line.SubstringByTextElements(Index, FieldLength) : ((Index < Line.LengthInTextElements) ? Line.SubstringByTextElements(Index).TrimEnd(new char[2] { '\r', '\n' }) : string.Empty));
		if (m_TrimWhiteSpace)
		{
			return text.Trim();
		}
		return text;
	}

	private int GetEndOfLineIndex(string Line)
	{
		int length = Line.Length;
		if (length == 1)
		{
			return length;
		}
		checked
		{
			if ((Operators.CompareString(Conversions.ToString(Line[length - 2]), "\r", TextCompare: false) == 0) | (Operators.CompareString(Conversions.ToString(Line[length - 2]), "\n", TextCompare: false) == 0))
			{
				return length - 2;
			}
			if ((Operators.CompareString(Conversions.ToString(Line[length - 1]), "\r", TextCompare: false) == 0) | (Operators.CompareString(Conversions.ToString(Line[length - 1]), "\n", TextCompare: false) == 0))
			{
				return length - 1;
			}
			return length;
		}
	}

	private void ValidateFixedWidthLine(StringInfo Line, long LineNumber)
	{
		if (Line.LengthInTextElements < m_LineLength)
		{
			m_ErrorLine = Line.String;
			m_ErrorLineNumber = checked(m_LineNumber - 1);
			throw new MalformedLineException(System.SR.Format(System.SR.TextFieldParser_MalFormedFixedWidthLine, LineNumber.ToString(CultureInfo.InvariantCulture)), LineNumber);
		}
	}

	private void ValidateFieldWidths()
	{
		if (m_FieldWidths == null)
		{
			throw ExceptionUtils.GetInvalidOperationException(System.SR.TextFieldParser_FieldWidthsNothing);
		}
		if (m_FieldWidths.Length == 0)
		{
			throw ExceptionUtils.GetInvalidOperationException(System.SR.TextFieldParser_FieldWidthsNothing);
		}
		checked
		{
			int num = m_FieldWidths.Length - 1;
			m_LineLength = 0;
			int num2 = num - 1;
			for (int i = 0; i <= num2; i++)
			{
				m_LineLength += m_FieldWidths[i];
			}
			if (m_FieldWidths[num] > 0)
			{
				m_LineLength += m_FieldWidths[num];
			}
		}
	}

	private void ValidateFieldWidthsOnInput(int[] Widths)
	{
		checked
		{
			int num = Widths.Length - 1 - 1;
			for (int i = 0; i <= num; i++)
			{
				if (Widths[i] < 1)
				{
					throw ExceptionUtils.GetArgumentExceptionWithArgName("FieldWidths", System.SR.TextFieldParser_FieldWidthsMustPositive);
				}
			}
		}
	}

	private void ValidateAndEscapeDelimiters()
	{
		if (m_Delimiters == null)
		{
			throw ExceptionUtils.GetArgumentExceptionWithArgName("Delimiters", System.SR.TextFieldParser_DelimitersNothing);
		}
		if (m_Delimiters.Length == 0)
		{
			throw ExceptionUtils.GetArgumentExceptionWithArgName("Delimiters", System.SR.TextFieldParser_DelimitersNothing);
		}
		int num = m_Delimiters.Length;
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.Append(EndQuotePattern + "(");
		checked
		{
			int num2 = num - 1;
			for (int i = 0; i <= num2; i++)
			{
				if (m_Delimiters[i] != null)
				{
					if (m_HasFieldsEnclosedInQuotes && m_Delimiters[i].IndexOf('"') > -1)
					{
						throw ExceptionUtils.GetInvalidOperationException(System.SR.TextFieldParser_IllegalDelimiter);
					}
					string text = Regex.Escape(m_Delimiters[i]);
					stringBuilder.Append(text + "|");
					stringBuilder2.Append(text + "|");
				}
			}
			m_SpaceChars = WhitespaceCharacters;
			m_DelimiterRegex = new Regex(stringBuilder.ToString(0, stringBuilder.Length - 1));
			stringBuilder.Append("\r|\n");
			m_DelimiterWithEndCharsRegex = new Regex(stringBuilder.ToString());
			stringBuilder2.Append("\r|\n)|\"$");
		}
	}

	private void ValidateReadyToRead()
	{
		if (!(m_NeedPropertyCheck | ArrayHasChanged()))
		{
			return;
		}
		switch (m_TextFieldType)
		{
		case FieldType.Delimited:
			ValidateAndEscapeDelimiters();
			break;
		case FieldType.FixedWidth:
			ValidateFieldWidths();
			break;
		}
		if (m_CommentTokens != null)
		{
			string[] commentTokens = m_CommentTokens;
			foreach (string text in commentTokens)
			{
				if (Operators.CompareString(text, "", TextCompare: false) != 0 && (m_HasFieldsEnclosedInQuotes & (m_TextFieldType == FieldType.Delimited)) && string.Equals(text.Trim(), "\"", StringComparison.Ordinal))
				{
					throw ExceptionUtils.GetInvalidOperationException(System.SR.TextFieldParser_InvalidComment);
				}
			}
		}
		m_NeedPropertyCheck = false;
	}

	private void ValidateDelimiters(string[] delimiterArray)
	{
		if (delimiterArray == null)
		{
			return;
		}
		foreach (string text in delimiterArray)
		{
			if (Operators.CompareString(text, "", TextCompare: false) == 0)
			{
				throw ExceptionUtils.GetArgumentExceptionWithArgName("Delimiters", System.SR.TextFieldParser_DelimiterNothing, "Delimiters");
			}
			if (text.IndexOfAny(new char[2] { '\r', '\n' }) > -1)
			{
				throw ExceptionUtils.GetArgumentExceptionWithArgName("Delimiters", System.SR.TextFieldParser_EndCharsInDelimiter);
			}
		}
	}

	private bool ArrayHasChanged()
	{
		int num = 0;
		checked
		{
			switch (m_TextFieldType)
			{
			case FieldType.Delimited:
			{
				if (m_Delimiters == null)
				{
					return false;
				}
				num = m_DelimitersCopy.GetLowerBound(0);
				int upperBound2 = m_DelimitersCopy.GetUpperBound(0);
				int num4 = num;
				int num5 = upperBound2;
				for (int j = num4; j <= num5; j++)
				{
					if (Operators.CompareString(m_Delimiters[j], m_DelimitersCopy[j], TextCompare: false) != 0)
					{
						return true;
					}
				}
				break;
			}
			case FieldType.FixedWidth:
			{
				if (m_FieldWidths == null)
				{
					return false;
				}
				num = m_FieldWidthsCopy.GetLowerBound(0);
				int upperBound = m_FieldWidthsCopy.GetUpperBound(0);
				int num2 = num;
				int num3 = upperBound;
				for (int i = num2; i <= num3; i++)
				{
					if (m_FieldWidths[i] != m_FieldWidthsCopy[i])
					{
						return true;
					}
				}
				break;
			}
			}
			return false;
		}
	}

	private void CheckCommentTokensForWhitespace(string[] tokens)
	{
		if (tokens == null)
		{
			return;
		}
		foreach (string input in tokens)
		{
			if (m_WhiteSpaceRegEx.IsMatch(input))
			{
				throw ExceptionUtils.GetArgumentExceptionWithArgName("CommentTokens", System.SR.TextFieldParser_WhitespaceInToken);
			}
		}
	}

	private bool CharacterIsInDelimiter(char testCharacter)
	{
		string[] delimiters = m_Delimiters;
		for (int i = 0; i < delimiters.Length; i = checked(i + 1))
		{
			if (delimiters[i].IndexOf(testCharacter) > -1)
			{
				return true;
			}
		}
		return false;
	}
}
