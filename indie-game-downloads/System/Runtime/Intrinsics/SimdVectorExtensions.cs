namespace System.Runtime.Intrinsics;

internal static class SimdVectorExtensions
{
	public unsafe static void Store<TVector, T>(this TVector source, T* destination) where TVector : ISimdVector<TVector, T>
	{
		TVector.Store(source, destination);
	}

	public static void StoreUnsafe<TVector, T>(this TVector vector, ref T destination, nuint elementOffset) where TVector : ISimdVector<TVector, T>
	{
		TVector.StoreUnsafe(vector, ref destination, elementOffset);
	}

	public static TVector WithElement<TVector, T>(this TVector vector, int index, T value) where TVector : ISimdVector<TVector, T>
	{
		return TVector.WithElement(vector, index, value);
	}
}
