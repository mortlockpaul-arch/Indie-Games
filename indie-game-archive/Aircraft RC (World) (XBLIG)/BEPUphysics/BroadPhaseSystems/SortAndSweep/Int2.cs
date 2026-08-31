namespace BEPUphysics.BroadPhaseSystems.SortAndSweep;

internal struct Int2
{
	internal int Y;

	internal int Z;

	public override int GetHashCode()
	{
		return Y + Z;
	}

	internal int GetSortingHash()
	{
		return (int)((long)Y * 15485863L + (long)Z * 32452843L);
	}

	public override string ToString()
	{
		return "{" + Y + ", " + Z + "}";
	}
}
