namespace System;

public delegate bool Predicate<in T>(T obj) where T : allows ref struct;
