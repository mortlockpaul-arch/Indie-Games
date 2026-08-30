namespace Maximinus.DebugTools;

public interface IDebugEchoListner
{
	void Echo(DebugCommandMessage messageType, string text);
}
