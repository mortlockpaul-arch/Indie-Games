using System.ComponentModel;
using System.IO;
using System.Security;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class VB6OutputFile : VB6File
{
	internal VB6OutputFile(string FileName, OpenShare share, bool fAppend)
		: base(FileName, OpenAccess.Write, share, -1)
	{
		m_fAppend = fAppend;
	}

	internal override void OpenFile()
	{
		try
		{
			if (m_fAppend)
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
			else
			{
				m_file = new FileStream(m_sFullPath, FileMode.Create, (FileAccess)m_access, (FileShare)m_share);
			}
		}
		catch (FileNotFoundException ex)
		{
			throw ExceptionUtils.VbMakeException(ex, 53);
		}
		catch (SecurityException ex2)
		{
			throw ExceptionUtils.VbMakeException(ex2, 53);
		}
		catch (DirectoryNotFoundException ex3)
		{
			throw ExceptionUtils.VbMakeException(ex3, 76);
		}
		catch (IOException ex4)
		{
			throw ExceptionUtils.VbMakeException(ex4, 75);
		}
		m_Encoding = Utils.GetFileIOEncoding();
		m_sw = new StreamWriter(m_file, m_Encoding);
		m_sw.AutoFlush = true;
		if (m_fAppend)
		{
			long length = m_file.Length;
			m_file.Position = length;
			m_position = length;
		}
	}

	internal override void WriteLine(string s)
	{
		checked
		{
			if (s == null)
			{
				m_sw.WriteLine();
				m_position += 2L;
			}
			else
			{
				if (m_bPrint && m_lWidth != 0 && m_lCurrentColumn >= m_lWidth)
				{
					m_sw.WriteLine();
					m_position += 2L;
				}
				m_sw.WriteLine(s);
				m_position += m_Encoding.GetByteCount(s) + 2;
			}
			m_lCurrentColumn = 0;
		}
	}

	internal override void WriteString(string s)
	{
		checked
		{
			if (s != null && s.Length != 0)
			{
				if (m_bPrint && m_lWidth != 0 && (m_lCurrentColumn >= m_lWidth || (m_lCurrentColumn != 0 && m_lCurrentColumn + s.Length > m_lWidth)))
				{
					m_sw.WriteLine();
					m_position += 2L;
					m_lCurrentColumn = 0;
				}
				m_sw.Write(s);
				int byteCount = m_Encoding.GetByteCount(s);
				m_position += byteCount;
				m_lCurrentColumn += s.Length;
			}
		}
	}

	public override OpenMode GetMode()
	{
		if (m_fAppend)
		{
			return OpenMode.Append;
		}
		return OpenMode.Output;
	}

	internal override bool EOF()
	{
		return true;
	}

	internal override long LOC()
	{
		return checked(m_position + 127) / 128;
	}
}
