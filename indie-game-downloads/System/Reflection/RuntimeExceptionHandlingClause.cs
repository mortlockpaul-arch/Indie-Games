using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.Reflection;

internal sealed class RuntimeExceptionHandlingClause : ExceptionHandlingClause
{
	private RuntimeMethodBody _methodBody;

	private ExceptionHandlingClauseOptions _flags;

	private int _tryOffset;

	private int _tryLength;

	private int _handlerOffset;

	private int _handlerLength;

	private int _catchMetadataToken;

	private int _filterOffset;

	public override ExceptionHandlingClauseOptions Flags => _flags;

	public override int TryOffset => _tryOffset;

	public override int TryLength => _tryLength;

	public override int HandlerOffset => _handlerOffset;

	public override int HandlerLength => _handlerLength;

	public override int FilterOffset
	{
		get
		{
			if (_flags != ExceptionHandlingClauseOptions.Filter)
			{
				throw new InvalidOperationException(SR.Arg_EHClauseNotFilter);
			}
			return _filterOffset;
		}
	}

	public override Type CatchType
	{
		[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "Module.ResolveType is marked as RequiresUnreferencedCode because it relies on tokenswhich are not guaranteed to be stable across trimming. So if somebody hardcodes a token it could break.The usage here is not like that as all these tokens come from existing metadata loaded from some ILand so trimming has no effect (the tokens are read AFTER trimming occurred).")]
		get
		{
			if (_flags != ExceptionHandlingClauseOptions.Clause)
			{
				throw new InvalidOperationException(SR.Arg_EHClauseNotClause);
			}
			Type result = null;
			if (!MetadataToken.IsNullToken(_catchMetadataToken))
			{
				Type declaringType = _methodBody._methodBase.DeclaringType;
				result = ((declaringType == null) ? _methodBody._methodBase.Module : declaringType.Module).ResolveType(_catchMetadataToken, declaringType?.GetGenericArguments(), (_methodBody._methodBase is MethodInfo) ? _methodBody._methodBase.GetGenericArguments() : null);
			}
			return result;
		}
	}

	private RuntimeExceptionHandlingClause()
	{
	}

	public override string ToString()
	{
		IFormatProvider currentUICulture;
		if (Flags == ExceptionHandlingClauseOptions.Clause)
		{
			currentUICulture = CultureInfo.CurrentUICulture;
			IFormatProvider provider = currentUICulture;
			DefaultInterpolatedStringHandler handler = new DefaultInterpolatedStringHandler(74, 6, currentUICulture);
			handler.AppendLiteral("Flags=");
			handler.AppendFormatted(Flags);
			handler.AppendLiteral(", TryOffset=");
			handler.AppendFormatted(TryOffset);
			handler.AppendLiteral(", TryLength=");
			handler.AppendFormatted(TryLength);
			handler.AppendLiteral(", HandlerOffset=");
			handler.AppendFormatted(HandlerOffset);
			handler.AppendLiteral(", HandlerLength=");
			handler.AppendFormatted(HandlerLength);
			handler.AppendLiteral(", CatchType=");
			handler.AppendFormatted(CatchType);
			return string.Create(provider, ref handler);
		}
		if (Flags == ExceptionHandlingClauseOptions.Filter)
		{
			currentUICulture = CultureInfo.CurrentUICulture;
			IFormatProvider provider2 = currentUICulture;
			DefaultInterpolatedStringHandler handler2 = new DefaultInterpolatedStringHandler(77, 6, currentUICulture);
			handler2.AppendLiteral("Flags=");
			handler2.AppendFormatted(Flags);
			handler2.AppendLiteral(", TryOffset=");
			handler2.AppendFormatted(TryOffset);
			handler2.AppendLiteral(", TryLength=");
			handler2.AppendFormatted(TryLength);
			handler2.AppendLiteral(", HandlerOffset=");
			handler2.AppendFormatted(HandlerOffset);
			handler2.AppendLiteral(", HandlerLength=");
			handler2.AppendFormatted(HandlerLength);
			handler2.AppendLiteral(", FilterOffset=");
			handler2.AppendFormatted(FilterOffset);
			return string.Create(provider2, ref handler2);
		}
		currentUICulture = CultureInfo.CurrentUICulture;
		IFormatProvider provider3 = currentUICulture;
		DefaultInterpolatedStringHandler handler3 = new DefaultInterpolatedStringHandler(62, 5, currentUICulture);
		handler3.AppendLiteral("Flags=");
		handler3.AppendFormatted(Flags);
		handler3.AppendLiteral(", TryOffset=");
		handler3.AppendFormatted(TryOffset);
		handler3.AppendLiteral(", TryLength=");
		handler3.AppendFormatted(TryLength);
		handler3.AppendLiteral(", HandlerOffset=");
		handler3.AppendFormatted(HandlerOffset);
		handler3.AppendLiteral(", HandlerLength=");
		handler3.AppendFormatted(HandlerLength);
		return string.Create(provider3, ref handler3);
	}
}
