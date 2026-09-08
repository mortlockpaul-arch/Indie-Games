namespace System.Reflection.Emit;

internal sealed class RuntimeLocalBuilder : LocalBuilder
{
	private readonly int m_localIndex;

	private readonly Type m_localType;

	private readonly MethodInfo m_methodBuilder;

	private readonly bool m_isPinned;

	public override bool IsPinned => m_isPinned;

	public override Type LocalType => m_localType;

	public override int LocalIndex => m_localIndex;

	internal RuntimeLocalBuilder(int localIndex, Type localType, MethodInfo methodBuilder, bool isPinned)
	{
		m_isPinned = isPinned;
		m_localIndex = localIndex;
		m_localType = localType;
		m_methodBuilder = methodBuilder;
	}

	internal MethodInfo GetMethodBuilder()
	{
		return m_methodBuilder;
	}
}
