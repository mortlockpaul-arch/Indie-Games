using System;
using System.Runtime.InteropServices;

namespace ProjectMercury;

[ComVisible(true)]
public class NotFiniteNumberException : ArithmeticException
{
	private const string EXCEPTION_MESSAGE = "Number encountered was not a finite quantity.";

	private const int HRESULT = -2146233048;

	public double OffendingNumber { get; private set; }

	public NotFiniteNumberException()
		: base("Number encountered was not a finite quantity.")
	{
		base.HResult = -2146233048;
	}

	public NotFiniteNumberException(double offendingNumber)
		: this()
	{
		OffendingNumber = offendingNumber;
	}
}
