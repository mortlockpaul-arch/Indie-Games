using System.IO;

namespace ZXBox.Snapshot;

public class FileFormatFactory
{
	public static ISnapshot GetSnapShotHandler(string filename)
	{
		if (filename != null)
		{
			return Path.GetExtension(filename).ToLower() switch
			{
				".z80" => new Z80FileFormat(), 
				".sna" => new SNAFileFormat(), 
				_ => null, 
			};
		}
		return null;
	}
}
