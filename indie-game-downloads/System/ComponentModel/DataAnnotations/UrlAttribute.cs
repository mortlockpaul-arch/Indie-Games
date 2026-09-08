namespace System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class UrlAttribute : DataTypeAttribute
{
	public UrlAttribute()
		: base(DataType.Url)
	{
		base.DefaultErrorMessage = System.SR.UrlAttribute_Invalid;
	}

	public override bool IsValid(object? value)
	{
		if (!(value is Uri uri))
		{
			if (value is string text)
			{
				if (!text.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !text.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
				{
					return text.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase);
				}
				return true;
			}
			if (value == null)
			{
				return true;
			}
		}
		else if (uri.IsAbsoluteUri)
		{
			if (!(uri.Scheme == Uri.UriSchemeHttp) && !(uri.Scheme == Uri.UriSchemeHttps))
			{
				return uri.Scheme == Uri.UriSchemeFtp;
			}
			return true;
		}
		return false;
	}
}
