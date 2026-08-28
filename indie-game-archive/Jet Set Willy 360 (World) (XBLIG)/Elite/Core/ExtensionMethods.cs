using System;
using Microsoft.Xna.Framework.Input;

namespace Elite.Core;

public static class ExtensionMethods
{
	public static Keys ToKeys(this string key)
	{
		return (Keys)Enum.Parse(typeof(Keys), key.ToString(), ignoreCase: true);
	}
}
