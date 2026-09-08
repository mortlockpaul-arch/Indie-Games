using System.Text.Encodings.Web;

namespace System.Text.Json;

public struct JsonWriterOptions
{
	private static readonly string s_alternateNewLine = ((Environment.NewLine.Length == 2) ? "\n" : "\r\n");

	private int _maxDepth;

	private int _optionsMask;

	public JavaScriptEncoder? Encoder { get; set; }

	public bool Indented
	{
		get
		{
			return (_optionsMask & 1) != 0;
		}
		set
		{
			if (value)
			{
				_optionsMask |= 1;
			}
			else
			{
				_optionsMask &= -2;
			}
		}
	}

	public char IndentCharacter
	{
		readonly get
		{
			if ((_optionsMask & 8) == 0)
			{
				return ' ';
			}
			return '\t';
		}
		set
		{
			JsonWriterHelper.ValidateIndentCharacter(value);
			if (value != ' ')
			{
				_optionsMask |= 8;
			}
			else
			{
				_optionsMask &= -9;
			}
		}
	}

	public int IndentSize
	{
		readonly get
		{
			return EncodeIndentSize((_optionsMask & 0x7F0) >> 4);
		}
		set
		{
			JsonWriterHelper.ValidateIndentSize(value);
			_optionsMask = (_optionsMask & -2033) | (EncodeIndentSize(value) << 4);
		}
	}

	public int MaxDepth
	{
		readonly get
		{
			return _maxDepth;
		}
		set
		{
			if (value < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_MaxDepthMustBePositive("value");
			}
			_maxDepth = value;
		}
	}

	public bool SkipValidation
	{
		get
		{
			return (_optionsMask & 2) != 0;
		}
		set
		{
			if (value)
			{
				_optionsMask |= 2;
			}
			else
			{
				_optionsMask &= -3;
			}
		}
	}

	public string NewLine
	{
		get
		{
			if ((_optionsMask & 4) == 0)
			{
				return Environment.NewLine;
			}
			return s_alternateNewLine;
		}
		set
		{
			JsonWriterHelper.ValidateNewLine(value);
			if (value != Environment.NewLine)
			{
				_optionsMask |= 4;
			}
			else
			{
				_optionsMask &= -5;
			}
		}
	}

	internal bool IndentedOrNotSkipValidation => (_optionsMask & 3) != 2;

	private static int EncodeIndentSize(int value)
	{
		return value switch
		{
			0 => 2, 
			2 => 0, 
			_ => value, 
		};
	}
}
