namespace Game.Data;

public class DataShareSender
{
	public enum Step
	{
		Header,
		Data,
		Done
	}

	public const int CHUNK_SIZE = 256;

	public bool ready;

	public Step step;

	public int byteIndex;

	public DataShareSender()
	{
		byteIndex = 0;
		ready = true;
		step = Step.Header;
	}
}
