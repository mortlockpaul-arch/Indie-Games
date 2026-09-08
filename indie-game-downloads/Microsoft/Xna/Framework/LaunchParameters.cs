using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework;

public class LaunchParameters : Dictionary<string, string>
{
	private static readonly char[] flags = new char[2] { '/', '-' };

	public LaunchParameters()
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 1; i < commandLineArgs.Length; i++)
		{
			string text = commandLineArgs[i].TrimStart(flags);
			string value = string.Empty;
			int num = text.IndexOf(":");
			if (num != -1)
			{
				value = text.Substring(num + 1);
				text = text.Substring(0, num);
			}
			if (text.Length != 0 && !ContainsKey(text))
			{
				Add(text, value);
			}
		}
	}
}
