using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class PAbstract
{
	public int beat;

	public Vector3 pos;

	public Vector3 dir;

	public Vector3 up;

	public PAbstract(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		beat = LevelLoader.GetIntFromAtt(attributes, "beat", -1);
		pos = LevelLoader.GetVectorFromAtt(attributes, "pos");
		dir = LevelLoader.GetVectorFromAtt(attributes, "dir");
		up = LevelLoader.GetVectorFromAtt(attributes, "up");
	}
}
