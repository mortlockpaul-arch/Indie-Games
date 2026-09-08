namespace System;

internal enum ParsingError
{
	None,
	BadFormat,
	BadScheme,
	BadAuthority,
	EmptyUriString,
	LastErrorOkayForRelativeUris,
	SchemeLimit,
	MustRootedPath,
	BadHostName,
	NonEmptyHost,
	BadPort,
	BadAuthorityTerminator,
	CannotCreateRelative
}
