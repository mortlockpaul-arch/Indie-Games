using System.Reflection;

namespace System.Diagnostics.Tracing;

[DefaultMember("Item")]
internal struct SessionMask(uint mask = 0u)
{
	private uint m_mask = mask & 0xF;

	public static SessionMask All => new SessionMask(15u);

	public ulong ToEventKeywords()
	{
		return (ulong)m_mask << 44;
	}

	public static SessionMask FromEventKeywords(ulong m)
	{
		return new SessionMask((uint)(m >> 44));
	}

	public static explicit operator uint(SessionMask m)
	{
		return m.m_mask;
	}
}
