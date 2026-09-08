using System;
using System.Diagnostics;
using System.IO;

internal class Program
{
	private static void Main(string[] args)
	{
		if (args.Length < 2)
		{
			Console.WriteLine("Usage: <input file> <output file>");
			return;
		}
		string path = args[0];
		byte[] array = File.ReadAllBytes(path);
		byte[] sequence = new byte[4] { 16, 42, 17, 0 };
		int num = FindSequence(array, sequence);
		if (num >= 0)
		{
			byte[] array2 = new byte[array.Length - num];
			Array.Copy(array, num, array2, 0, array2.Length);
			string text = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(path)), "temp_" + Path.GetFileName(path));
			File.WriteAllBytes(text, array2);
			Console.WriteLine("File cleaned successfully.");
			path = text;
			Console.WriteLine("Sending to XenosRecomp...");
			Process process = new Process();
			process.StartInfo.FileName = Path.Combine(AppContext.BaseDirectory, "XenosRecomp", "XenosRecomp.exe");
			process.StartInfo.WorkingDirectory = AppContext.BaseDirectory;
			process.StartInfo.ArgumentList.Add(path);
			process.StartInfo.ArgumentList.Add(Path.GetFullPath(args[1]));
			process.StartInfo.ArgumentList.Add(Path.Combine(AppContext.BaseDirectory, "XenosRecomp", "shader_common.h"));
			process.Start();
			process.WaitForExit();
			Console.WriteLine($"XenosRecomp exit code: {process.ExitCode}");
		}
		else
		{
			Console.WriteLine("Byte sequence not found.");
		}
	}

	private static int FindSequence(byte[] source, byte[] sequence)
	{
		for (int i = 0; i <= source.Length - sequence.Length; i++)
		{
			bool flag = true;
			for (int j = 0; j < sequence.Length; j++)
			{
				if (source[i + j] != sequence[j])
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				return i;
			}
		}
		return -1;
	}
}
