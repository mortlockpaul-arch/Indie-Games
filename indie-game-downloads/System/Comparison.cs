namespace System;

public delegate int Comparison<in T>(T x, T y) where T : allows ref struct;
