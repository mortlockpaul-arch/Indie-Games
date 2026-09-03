using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class ChannelEffectSet : EffectHelper
{
	private int channelNum;

	private float coolDown;

	private float curVal;

	private TransformSet transforms;

	private List<ChannelEffect> chanFX;

	public ChannelEffectSet(Dictionary<string, string> attributes, XmlNode node)
	{
		channelNum = LevelLoader.GetIntFromAtt(attributes, "channel", 0);
		coolDown = LevelLoader.GetFloatFromAtt(attributes, "cooldown", 0.35f) * 1000f;
		chanFX = new List<ChannelEffect>();
		LevelLoader.BuildTransform(node, out transforms, BaseGame.Get().level.activeZone);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.Name != "Transform")
			{
				chanFX.Add((ChannelEffect)LevelLoader.MakeObj(childNode));
			}
		}
	}

	public override void Update(GameTime gametime)
	{
		if (BaseGame.Get().channels[channelNum] > 0.9f)
		{
			curVal = BaseGame.Get().channels[channelNum];
		}
		else if (curVal > 0f)
		{
			curVal -= (float)gametime.ElapsedGameTime.TotalMilliseconds / coolDown;
		}
		if (curVal > 1f)
		{
			curVal = 1f;
		}
		if (curVal < 0f)
		{
			curVal = 0f;
		}
		transforms.Update(gametime);
		for (int num = chanFX.Count - 1; num >= 0; num--)
		{
			chanFX[num].Update(gametime);
		}
		base.Update(gametime);
	}

	public override void Draw(GameTime gametime)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().matStack.ApplyMatrix(transforms.GetAllMatrix(curVal));
		for (int num = chanFX.Count - 1; num >= 0; num--)
		{
			chanFX[num].Draw(gametime, curVal, ref toUpdate);
		}
		base.Draw(gametime);
	}
}
