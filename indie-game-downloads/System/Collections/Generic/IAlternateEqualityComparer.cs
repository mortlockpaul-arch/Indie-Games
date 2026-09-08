namespace System.Collections.Generic;

public interface IAlternateEqualityComparer<in TAlternate, T> where TAlternate : allows ref struct where T : allows ref struct
{
	bool Equals(TAlternate alternate, T other);

	int GetHashCode(TAlternate alternate);

	T Create(TAlternate alternate);
}
