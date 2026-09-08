using System;

namespace Microsoft.CSharp.RuntimeBinder;

internal readonly struct ArgumentObject(object value, CSharpArgumentInfo info, Type type)
{
	internal readonly object Value = value;

	internal readonly CSharpArgumentInfo Info = info;

	internal readonly Type Type = type;
}
