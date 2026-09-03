namespace OluXNA;

internal class Triangle
{
	public int[] vindices;

	public int[] nindices;

	private Triangle()
	{
	}

	private Triangle(int a, int b, int c)
	{
		vindices = new int[3];
		nindices = new int[3];
		vindices[0] = a;
		vindices[1] = b;
		vindices[2] = c;
	}

	private Triangle(int[] vertices)
	{
		for (int i = 0; i < 3; i++)
		{
			vindices[i] = vertices[i];
		}
	}
}
