namespace System.Security.Cryptography;

public sealed class CngUIPolicy
{
	public CngUIProtectionLevels ProtectionLevel { get; }

	public string? FriendlyName { get; }

	public string? Description { get; }

	public string? UseContext { get; }

	public string? CreationTitle { get; }

	public CngUIPolicy(CngUIProtectionLevels protectionLevel)
		: this(protectionLevel, null)
	{
	}

	public CngUIPolicy(CngUIProtectionLevels protectionLevel, string? friendlyName)
		: this(protectionLevel, friendlyName, null)
	{
	}

	public CngUIPolicy(CngUIProtectionLevels protectionLevel, string? friendlyName, string? description)
		: this(protectionLevel, friendlyName, description, null)
	{
	}

	public CngUIPolicy(CngUIProtectionLevels protectionLevel, string? friendlyName, string? description, string? useContext)
		: this(protectionLevel, friendlyName, description, useContext, null)
	{
	}

	public CngUIPolicy(CngUIProtectionLevels protectionLevel, string? friendlyName, string? description, string? useContext, string? creationTitle)
	{
		ProtectionLevel = protectionLevel;
		FriendlyName = friendlyName;
		Description = description;
		UseContext = useContext;
		CreationTitle = creationTitle;
	}
}
