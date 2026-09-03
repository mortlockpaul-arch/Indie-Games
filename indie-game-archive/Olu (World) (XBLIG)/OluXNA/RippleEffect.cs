using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class RippleEffect : EffectHelper
{
	private Vector3 start;

	private float fullTime;

	private float ripple1;

	private float ripple2;

	private float rippleVel1;

	private float rippleVel2;

	private float timeStart;

	private float timeEnd;

	private bool loop;

	public bool done;

	private float timeElapsed;

	public Vector3 pos
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return start;
		}
	}

	public RippleEffect(Vector3 _start, float _vel1, float _vel2, float _timeStart, float _timeEnd, float _length, bool _loop, float _radStart)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		start = _start;
		rippleVel1 = _vel1;
		rippleVel2 = _vel2;
		timeStart = _timeStart;
		timeEnd = _timeEnd;
		fullTime = _length;
		loop = _loop;
		ripple1 = _radStart;
		ripple2 = 0f;
		done = false;
	}

	public RippleEffect(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		fullTime = LevelLoader.GetFloatFromAtt(attributes, "length", 0f);
		start = LevelLoader.GetVectorFromAtt(attributes, "ripplepos");
		rippleVel1 = LevelLoader.GetFloatFromAtt(attributes, "ripplevel1", 1f);
		rippleVel2 = LevelLoader.GetFloatFromAtt(attributes, "ripplevel2", 1f);
		timeStart = LevelLoader.GetFloatFromAtt(attributes, "start", 1f);
		timeEnd = LevelLoader.GetFloatFromAtt(attributes, "end", 1f);
		loop = LevelLoader.GetBoolFromAtt(attributes, "loop", defVal: false);
		timeElapsed = 0f;
		ripple1 = (ripple2 = 0f);
		done = false;
	}

	public override void Update(GameTime gametime)
	{
		float num = (float)(gametime.ElapsedGameTime.TotalMilliseconds / 1000.0);
		timeElapsed += num;
		if (timeElapsed > fullTime)
		{
			if (loop)
			{
				timeElapsed -= fullTime;
				ripple1 = 0f;
				ripple2 = 0f;
			}
			else
			{
				ripple1 = 0f;
				done = true;
			}
		}
		if (timeElapsed > timeStart)
		{
			ripple1 += rippleVel1 * num;
		}
		if (timeElapsed > timeEnd)
		{
			ripple2 += rippleVel2 * num;
		}
		base.Update(gametime);
	}

	public override void Draw(GameTime gametime)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Ripple");
		fxUpdate["RipplePos"].SetValue(start);
		fxUpdate["RippleIn"].SetValue(ripple2);
		fxUpdate["RippleOut"].SetValue(ripple1);
		base.Draw(gametime);
	}
}
