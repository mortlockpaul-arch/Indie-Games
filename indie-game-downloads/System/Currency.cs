namespace System;

internal struct Currency(decimal value)
{
	internal long m_value = decimal.ToOACurrency(value);
}
