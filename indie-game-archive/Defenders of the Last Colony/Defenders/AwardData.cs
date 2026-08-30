using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

public class AwardData
{
	public bool unlocked;

	public string name;

	public string desc;

	public string date;

	public Texture2D image;

	public Texture2D locked;

	public uint points;

	public Vector2 pos;

	public AwardData(string name, string desc, Texture2D image, Texture2D locked, uint points)
	{
		this.name = name;
		this.desc = desc;
		date = "";
		this.image = image;
		this.locked = locked;
		this.points = points;
		pos = Vector2.Zero;
		unlocked = false;
	}
}
