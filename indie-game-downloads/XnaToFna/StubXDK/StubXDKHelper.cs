using System;
using System.Reflection;

namespace XnaToFna.StubXDK;

public static class StubXDKHelper
{
	private static Assembly _GamerServicesAsm;

	public static Assembly GamerServicesAsm
	{
		get
		{
			if (_GamerServicesAsm != null)
			{
				return _GamerServicesAsm;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				if (assembly.GetType("Microsoft.Xna.Framework.GamerServices.GamerPresence") != null)
				{
					return _GamerServicesAsm = assembly;
				}
			}
			return null;
		}
	}
}
