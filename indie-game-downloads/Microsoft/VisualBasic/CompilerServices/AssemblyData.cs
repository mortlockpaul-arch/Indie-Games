using System.Collections;
using System.ComponentModel;
using System.IO;

namespace Microsoft.VisualBasic.CompilerServices;

[EditorBrowsable(EditorBrowsableState.Never)]
internal sealed class AssemblyData
{
	public ArrayList m_Files;

	internal FileSystemInfo[] m_DirFiles;

	internal int m_DirNextFileIndex;

	internal FileAttributes m_DirAttributes;

	internal AssemblyData()
	{
		ArrayList arrayList = new ArrayList(256);
		object value = null;
		int num = 0;
		do
		{
			arrayList.Add(value);
			num = checked(num + 1);
		}
		while (num <= 255);
		m_Files = arrayList;
	}

	internal VB6File GetChannelObj(int lChannel)
	{
		object obj = ((lChannel >= m_Files.Count) ? null : m_Files[lChannel]);
		return (VB6File)obj;
	}

	internal void SetChannelObj(int lChannel, VB6File oFile)
	{
		if (m_Files == null)
		{
			m_Files = new ArrayList(256);
		}
		if (oFile == null)
		{
			((VB6File)m_Files[lChannel])?.CloseFile();
			m_Files[lChannel] = null;
		}
		else
		{
			m_Files[lChannel] = oFile;
		}
	}
}
