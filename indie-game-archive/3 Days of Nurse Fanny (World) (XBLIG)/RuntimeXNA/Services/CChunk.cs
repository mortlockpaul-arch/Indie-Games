namespace RuntimeXNA.Services;

internal class CChunk
{
	public const short CHUNK_LAST = 32639;

	public short chID;

	public short chFlags;

	public int chSize;

	public short readHeader(CFile file)
	{
		chID = file.readAShort();
		chFlags = file.readAShort();
		chSize = file.readAInt();
		return chID;
	}

	public void skipChunk(CFile file)
	{
		file.skipBytes(chSize);
	}
}
