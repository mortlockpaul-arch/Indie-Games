namespace Eyehook.Framework;

public class ResetIOException : ResetException
{
	public ResetIOException()
		: base("An error occurred while accessing your storage device.\n\nPlease make sure that it has enough free space and is properly connected.")
	{
	}
}
