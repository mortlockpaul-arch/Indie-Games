using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class ShineEffect : EffectHelper
{
	private Vector3 start;

	private float fullTime;

	private Vector3 shinePos;

	private Vector3 shineVel;

	private float shineDist;

	private bool loop;

	private float timeElapsed;

	public ShineEffect(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		fullTime = LevelLoader.GetFloatFromAtt(attributes, "length", 0f);
		shinePos = LevelLoader.GetVectorFromAtt(attributes, "shinepos");
		start = shinePos;
		shineVel = LevelLoader.GetVectorFromAtt(attributes, "shinevel");
		shineDist = LevelLoader.GetFloatFromAtt(attributes, "shinedist", -1f);
		loop = LevelLoader.GetBoolFromAtt(attributes, "loop", defVal: false);
		timeElapsed = 0f;
	}

	public override void Update(GameTime gametime)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		timeElapsed += (float)(gametime.ElapsedGameTime.TotalMilliseconds / 1000.0);
		if (timeElapsed > fullTime)
		{
			if (loop)
			{
				timeElapsed -= fullTime;
			}
			else
			{
				shineDist = -1f;
			}
		}
		shinePos = start + shineVel * timeElapsed;
		base.Update(gametime);
	}

	public override void Draw(GameTime gametime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Shine");
		BaseGame.SetAllEPCs(toUpdate, "ShinePos", shinePos);
		BaseGame.SetAllEPCs(toUpdate, "ShineDist", shineDist);
		base.Draw(gametime);
	}
}
