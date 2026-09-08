using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Security;

namespace Microsoft.VisualBasic.CompilerServices;

[DebuggerNonUserCode]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ProjectData
{
	internal ErrObject m_Err;

	internal int m_rndSeed;

	internal byte[] m_numprsPtr;

	internal byte[] m_DigitArray;

	[ThreadStatic]
	private static ProjectData m_oProject;

	internal Hashtable m_AssemblyData;

	private Assembly m_CachedMSCoreLibAssembly;

	private ProjectData()
	{
		m_rndSeed = 327680;
		m_CachedMSCoreLibAssembly = typeof(int).Assembly;
		m_AssemblyData = new Hashtable();
		m_numprsPtr = new byte[24];
		m_DigitArray = new byte[30];
	}

	internal AssemblyData GetAssemblyData(Assembly assem)
	{
		if ((object)assem == Utils.VBRuntimeAssembly || (object)assem == m_CachedMSCoreLibAssembly)
		{
			throw new SecurityException(System.SR.Security_LateBoundCallsNotPermitted);
		}
		AssemblyData assemblyData = (AssemblyData)m_AssemblyData[assem];
		if (assemblyData == null)
		{
			assemblyData = new AssemblyData();
			m_AssemblyData[assem] = assemblyData;
		}
		return assemblyData;
	}

	internal static ProjectData GetProjectData()
	{
		ProjectData projectData = m_oProject;
		if (projectData == null)
		{
			projectData = (m_oProject = new ProjectData());
		}
		return projectData;
	}

	public static Exception CreateProjectError(int hr)
	{
		ErrObject errObject = Information.Err();
		errObject.Clear();
		int resourceId = errObject.MapErrorNumber(hr);
		return errObject.CreateException(hr, Utils.GetResourceString((vbErrors)resourceId));
	}

	public static void SetProjectError(Exception ex)
	{
		Information.Err().CaptureException(ex);
	}

	public static void SetProjectError(Exception ex, int lErl)
	{
		Information.Err().CaptureException(ex, lErl);
	}

	public static void ClearProjectError()
	{
		Information.Err().Clear();
	}

	public static void EndApp()
	{
		FileSystem.CloseAllFiles(Assembly.GetCallingAssembly());
		Environment.Exit(0);
	}
}
