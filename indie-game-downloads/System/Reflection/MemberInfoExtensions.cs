using System.ComponentModel;

namespace System.Reflection;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class MemberInfoExtensions
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static bool HasMetadataToken(this MemberInfo member)
	{
		ArgumentNullException.ThrowIfNull(member, "member");
		try
		{
			return member.GetMetadataTokenOrZeroOrThrow() != 0;
		}
		catch (InvalidOperationException)
		{
			return false;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public static int GetMetadataToken(this MemberInfo member)
	{
		ArgumentNullException.ThrowIfNull(member, "member");
		int metadataTokenOrZeroOrThrow = member.GetMetadataTokenOrZeroOrThrow();
		if (metadataTokenOrZeroOrThrow == 0)
		{
			throw new InvalidOperationException(System.SR.NoMetadataTokenAvailable);
		}
		return metadataTokenOrZeroOrThrow;
	}

	private static int GetMetadataTokenOrZeroOrThrow(this MemberInfo member)
	{
		int metadataToken = member.MetadataToken;
		if ((metadataToken & 0xFFFFFF) == 0)
		{
			return 0;
		}
		return metadataToken;
	}
}
