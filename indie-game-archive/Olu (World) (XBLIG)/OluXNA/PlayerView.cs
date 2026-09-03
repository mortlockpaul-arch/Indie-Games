using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class PlayerView : Enemy
{
	private Vector3 curDir;

	private Vector3 startDir;

	private Vector3 endDir;

	private float progress;

	private float speed;

	public PlayerView()
	{
		progress = 0f;
	}

	public PlayerView(Vector3 _start, Vector3 _end, float _speed)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		startDir = Vector3.Normalize(_start);
		curDir = startDir;
		endDir = Vector3.Normalize(_end);
		speed = _speed;
	}

	public PlayerView(Dictionary<string, string> attributes, XmlNode node)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		startDir = LevelLoader.GetVectorFromAtt(attributes, "start");
		endDir = LevelLoader.GetVectorFromAtt(attributes, "end");
		curDir = startDir;
		speed = LevelLoader.GetFloatFromAtt(attributes, "speed", 1f);
	}

	public override void draw(GameTime gametime)
	{
	}

	public override void act(GameTime gametime)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (exists)
		{
			progress += (float)gametime.ElapsedGameTime.TotalSeconds * speed;
			if (progress >= 1f)
			{
				leave();
				return;
			}
			curDir = Vector3.Normalize(startDir + progress * (endDir - startDir));
			BaseGame.Get().MovePlayerDir(curDir);
		}
	}

	public override void start()
	{
		base.start();
		BaseGame.Get().actualEnem--;
	}

	public override string name()
	{
		return "[view 0x0ABC]";
	}

	public override void die()
	{
		BaseGame.Get().actualEnem++;
		base.die();
	}

	public override void leave()
	{
		BaseGame.Get().actualEnem++;
		base.leave();
	}
}
