namespace Eyehook.Framework;

public class ResetIODisabledException : ResetException
{
	public ResetIODisabledException()
		: base("Read or write access to your storage device was denied.\n\nPlease make sure you are logged in to a profile that has access to your storage device.")
	{
	}
}
