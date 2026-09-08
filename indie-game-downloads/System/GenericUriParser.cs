namespace System;

public class GenericUriParser : UriParser
{
	public GenericUriParser(GenericUriParserOptions options)
		: base(MapGenericParserOptions(options))
	{
	}

	private static System.UriSyntaxFlags MapGenericParserOptions(GenericUriParserOptions options)
	{
		System.UriSyntaxFlags uriSyntaxFlags = System.UriSyntaxFlags.AllowAnInternetHost | System.UriSyntaxFlags.MustHaveAuthority | System.UriSyntaxFlags.MayHaveUserInfo | System.UriSyntaxFlags.MayHavePort | System.UriSyntaxFlags.MayHavePath | System.UriSyntaxFlags.MayHaveQuery | System.UriSyntaxFlags.MayHaveFragment | System.UriSyntaxFlags.AllowUncHost | System.UriSyntaxFlags.PathIsRooted | System.UriSyntaxFlags.ConvertPathSlashes | System.UriSyntaxFlags.CompressPath | System.UriSyntaxFlags.CanonicalizeAsFilePath | System.UriSyntaxFlags.UnEscapeDotsAndSlashes;
		if ((options & GenericUriParserOptions.GenericAuthority) != GenericUriParserOptions.Default)
		{
			uriSyntaxFlags &= ~(System.UriSyntaxFlags.AllowAnInternetHost | System.UriSyntaxFlags.MayHaveUserInfo | System.UriSyntaxFlags.MayHavePort | System.UriSyntaxFlags.AllowUncHost);
			uriSyntaxFlags |= System.UriSyntaxFlags.AllowAnyOtherHost;
		}
		if ((options & GenericUriParserOptions.AllowEmptyAuthority) != GenericUriParserOptions.Default)
		{
			uriSyntaxFlags |= System.UriSyntaxFlags.AllowEmptyHost;
		}
		if ((options & GenericUriParserOptions.NoUserInfo) != GenericUriParserOptions.Default)
		{
			uriSyntaxFlags &= ~System.UriSyntaxFlags.MayHaveUserInfo;
		}
		if ((options & GenericUriParserOptions.NoPort) != GenericUriParserOptions.Default)
		{
			uriSyntaxFlags &= ~System.UriSyntaxFlags.MayHavePort;
		}
		if ((options & GenericUriParserOptions.NoQuery) != GenericUriParserOptions.Default)
		{
			uriSyntaxFlags &= ~System.UriSyntaxFlags.MayHaveQuery;
		}
		if ((options & GenericUriParserOptions.NoFragment) != GenericUriParserOptions.Default)
		{
			uriSyntaxFlags &= ~System.UriSyntaxFlags.MayHaveFragment;
		}
		if ((options & GenericUriParserOptions.DontConvertPathBackslashes) != GenericUriParserOptions.Default)
		{
			uriSyntaxFlags &= ~System.UriSyntaxFlags.ConvertPathSlashes;
		}
		if ((options & GenericUriParserOptions.DontCompressPath) != GenericUriParserOptions.Default)
		{
			uriSyntaxFlags &= ~(System.UriSyntaxFlags.CompressPath | System.UriSyntaxFlags.CanonicalizeAsFilePath);
		}
		if ((options & GenericUriParserOptions.DontUnescapePathDotsAndSlashes) != GenericUriParserOptions.Default)
		{
			uriSyntaxFlags &= ~System.UriSyntaxFlags.UnEscapeDotsAndSlashes;
		}
		if ((options & GenericUriParserOptions.Idn) != GenericUriParserOptions.Default)
		{
			uriSyntaxFlags |= System.UriSyntaxFlags.AllowIdn;
		}
		if ((options & GenericUriParserOptions.IriParsing) != GenericUriParserOptions.Default)
		{
			uriSyntaxFlags |= System.UriSyntaxFlags.AllowIriParsing;
		}
		return uriSyntaxFlags;
	}
}
