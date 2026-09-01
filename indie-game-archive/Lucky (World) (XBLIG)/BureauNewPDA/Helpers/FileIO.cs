using System.IO;
using System.Xml.Serialization;
using BureauNewPDA.Data;

namespace BureauNewPDA.Helpers;

public class FileIO
{
	public SampleData loadData(string fileName)
	{
		FileStream fileStream = new FileStream("Content\\XML\\" + fileName + ".xml", FileMode.Open, FileAccess.Read, FileShare.Read);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(SampleData));
		SampleData result = (SampleData)xmlSerializer.Deserialize(fileStream);
		fileStream.Close();
		return result;
	}

	public RefDumpClass loadRefData()
	{
		FileStream fileStream = new FileStream("Content\\XML\\RefDataDump.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(RefDumpClass));
		RefDumpClass result = (RefDumpClass)xmlSerializer.Deserialize(fileStream);
		fileStream.Close();
		return result;
	}
}
