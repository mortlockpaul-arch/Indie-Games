using System;
using System.Globalization;

namespace Microsoft.VisualBasic;

internal sealed class FormatInfoHolder : IFormatProvider
{
	private NumberFormatInfo nfi;

	internal FormatInfoHolder(NumberFormatInfo nfi)
	{
		this.nfi = nfi;
	}

	private object GetFormat(Type service)
	{
		if ((object)service == typeof(NumberFormatInfo))
		{
			return nfi;
		}
		throw new ArgumentException(System.SR.InternalError_VisualBasicRuntime);
	}

	object IFormatProvider.GetFormat(Type service)
	{
		//ILSpy generated this explicit interface implementation from .override directive in GetFormat
		return this.GetFormat(service);
	}
}
