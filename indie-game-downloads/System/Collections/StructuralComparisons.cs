namespace System.Collections;

public static class StructuralComparisons
{
	public static IComparer StructuralComparer => System.Collections.StructuralComparer.s_instance;

	public static IEqualityComparer StructuralEqualityComparer => System.Collections.StructuralEqualityComparer.s_instance;
}
