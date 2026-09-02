namespace SGSCore;

public abstract class SGSContent
{
	public string m_path = "";

	public SGSContent(string path)
	{
		m_path = path;
	}

	public abstract void Clear();
}
