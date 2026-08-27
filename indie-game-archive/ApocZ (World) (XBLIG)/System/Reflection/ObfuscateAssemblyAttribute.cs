using System.Runtime.InteropServices;

namespace System.Reflection;

[ComVisible(true)]
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
internal sealed class ObfuscateAssemblyAttribute : Attribute
{
	private bool m_assemblyIsPrivate;

	private bool m_stripAfterObfuscation;

	public bool AssemblyIsPrivate => m_assemblyIsPrivate;

	public bool StripAfterObfuscation
	{
		get
		{
			return m_stripAfterObfuscation;
		}
		set
		{
			m_stripAfterObfuscation = value;
		}
	}

	public ObfuscateAssemblyAttribute(bool assemblyIsPrivate)
	{
		m_assemblyIsPrivate = assemblyIsPrivate;
		m_stripAfterObfuscation = true;
	}
}
