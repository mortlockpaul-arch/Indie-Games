namespace BEPUphysics.CollisionTests.CollisionAlgorithms;

public struct BoxContactDataCache
{
	public BoxContactData D1;

	public BoxContactData D2;

	public BoxContactData D3;

	public BoxContactData D4;

	public BoxContactData D5;

	public BoxContactData D6;

	public BoxContactData D7;

	public BoxContactData D8;

	/// <summary>
	/// Number of elements in the cache.
	/// </summary>
	public byte Count;

	/// <summary>
	/// Removes an item at the given index.
	/// </summary>
	/// <param name="index">Index to remove.</param>
	public unsafe void RemoveAt(int index)
	{
		BoxContactDataCache boxContactDataCache = this;
		BoxContactData* ptr = &boxContactDataCache.D1;
		ptr[index] = ptr[Count - 1];
		this = boxContactDataCache;
		Count--;
	}
}
