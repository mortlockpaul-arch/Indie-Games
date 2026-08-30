using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content;

namespace Maximinus;

public class CSV
{
	private static readonly char defaultSep = ',';

	public static List<string[]> Parse(ContentManager Content, string path)
	{
		return Parse(Content, path, defaultSep);
	}

	public static List<string[]> Parse(ContentManager Content, string path, char separator)
	{
		StreamReader streamReader = new StreamReader(path);
		List<string[]> list = new List<string[]>();
		string text;
		while ((text = streamReader.ReadLine()) != null)
		{
			string[] array = text.Split(separator);
			string[] array2 = new string[array.Count()];
			for (int i = 0; i < array.Count(); i++)
			{
				array2[i] = array[i].Trim('"');
			}
			list.Add(array2);
		}
		streamReader.Close();
		return list;
	}
}
