namespace System.Collections.Generic;

public interface IComparer<in T> where T : allows ref struct
{
	int Compare(T? x, T? y);
}
