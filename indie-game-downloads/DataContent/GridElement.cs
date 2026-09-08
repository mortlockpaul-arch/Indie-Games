using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DataContent;

[ContentSerializerRuntimeType("DataContent.GridElement, DataContent")]
public struct GridElement(ref Vector3 min, ref Vector3 max)
{
	public int[] Indices = null;

	public Vector3 GridMin = min;

	public Vector3 GridMax = max;

	public void SetIndices(int[] indices, int count)
	{
		if (count > 0)
		{
			Indices = new int[count];
			for (int i = 0; i < count; i++)
			{
				Indices[i] = indices[i];
			}
		}
	}
}
