using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualBasic.FileIO;

internal sealed class QuoteDelimitedFieldBuilder
{
	private StringBuilder m_Field;

	private bool m_FieldFinished;

	private int m_Index;

	private int m_DelimiterLength;

	private Regex m_DelimiterRegex;

	private string m_SpaceChars;

	private bool m_MalformedLine;

	public bool FieldFinished => m_FieldFinished;

	public string Field => m_Field.ToString();

	public int Index => m_Index;

	public int DelimiterLength => m_DelimiterLength;

	public bool MalformedLine => m_MalformedLine;

	public QuoteDelimitedFieldBuilder(Regex DelimiterRegex, string SpaceChars)
	{
		m_Field = new StringBuilder();
		m_DelimiterRegex = DelimiterRegex;
		m_SpaceChars = SpaceChars;
	}

	public void BuildField(string Line, int StartAt)
	{
		m_Index = StartAt;
		int length = Line.Length;
		checked
		{
			while (m_Index < length)
			{
				if (Line[m_Index] == '"')
				{
					if (m_Index + 1 == length)
					{
						m_FieldFinished = true;
						m_DelimiterLength = 1;
						m_Index++;
						break;
					}
					if (!((m_Index + 1 < Line.Length) & (Line[m_Index + 1] == '"')))
					{
						Match match = m_DelimiterRegex.Match(Line, m_Index + 1);
						int num = (match.Success ? (match.Index - 1) : (length - 1));
						int num2 = m_Index + 1;
						int num3 = num;
						for (int i = num2; i <= num3; i++)
						{
							if (m_SpaceChars.IndexOf(Line[i]) < 0)
							{
								m_MalformedLine = true;
								return;
							}
						}
						m_DelimiterLength = 1 + num - m_Index;
						if (match.Success)
						{
							m_DelimiterLength += match.Length;
						}
						m_FieldFinished = true;
						break;
					}
					m_Field.Append('"');
					m_Index += 2;
				}
				else
				{
					m_Field.Append(Line[m_Index]);
					m_Index++;
				}
			}
		}
	}
}
