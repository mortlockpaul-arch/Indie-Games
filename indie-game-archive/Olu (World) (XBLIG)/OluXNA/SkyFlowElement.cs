using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class SkyFlowElement : BackgroundElement
{
	private bool endTransform;

	public SkyFlowElement(Dictionary<string, string> attributes, XmlNode node)
	{
		endTransform = LevelLoader.GetBoolFromAtt(attributes, "endtransform", defVal: false);
		if (node.SelectSingleNode("transforms") != null)
		{
			LevelLoader.BuildTransform(node.SelectSingleNode("transforms"), out transforms, BaseGame.Get().level.activeZone);
		}
	}

	public override void Start()
	{
	}

	public override void Update(GameTime gametime)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		transforms.Update(gametime);
		BaseGame.Get().skyFlow = Vector3.Transform(Vector3.Forward, transforms.GetScaleMatrix() * transforms.GetMatrix());
	}

	public override void Draw(GameTime gametime)
	{
	}

	public override void LoadGraphics()
	{
	}
}
