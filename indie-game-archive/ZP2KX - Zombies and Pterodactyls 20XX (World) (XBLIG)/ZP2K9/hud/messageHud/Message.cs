using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ZP2K9.hud.messageHud;

public class Message
{
	public StringBuilder txt1;

	public StringBuilder txt2;

	public static StringBuilder msgJoined = new StringBuilder(" joined the game!");

	public static StringBuilder msgQuit = new StringBuilder(" left the game!");

	public static StringBuilder msgGotFlag = new StringBuilder(" got the flag!");

	public static StringBuilder msgDroppedFlag = new StringBuilder(" dropped the flag!");

	public static StringBuilder msgCappedFlag = new StringBuilder(" captured the flag!");

	public int team1;

	public int team2;

	public int kill;

	private Color GetTeamColor(int team, float a)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(team switch
		{
			1 => new Color(new Vector4(0.7f, 0.7f, 1f, a)), 
			2 => new Color(new Vector4(1f, 0.7f, 0.7f, a)), 
			_ => new Color(new Vector4(1f, 1f, 1f, a)), 
		});
	}

	public void Draw(SpriteBatch sprite, float y, float a)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(90f, 90f);
		sprite.DrawString(Game1.impact, txt1, val + new Vector2(0f, y), GetTeamColor(team1, a), 0f, default(Vector2), 1f, (SpriteEffects)0, 1f);
		float x = Game1.impact.MeasureString(txt1).X;
		if (kill > -1)
		{
			Rectangle value = default(Rectangle);
			((Rectangle)(ref value))._002Ector(256 + kill * 64, 800, 64, 64);
			if (kill >= 12)
			{
				value.X = 384 + (kill - 12) * 64;
				value.Y = 736;
			}
			sprite.Draw(Game1.spritesTex, val + new Vector2(x + 12f, y - 3f), (Rectangle?)value, new Color(new Vector4(1f, 1f, 1f, a)), 0f, default(Vector2), 0.43f, (SpriteEffects)0, 1f);
		}
		sprite.DrawString(Game1.impact, txt2, val + new Vector2(x + ((kill > -1) ? 48f : 0f), y), GetTeamColor(team2, a), 0f, default(Vector2), 1f, (SpriteEffects)0, 1f);
	}
}
