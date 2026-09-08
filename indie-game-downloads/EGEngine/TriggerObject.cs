using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class TriggerObject : WorldObject
{
	public TriggerTypes triggerType;

	public override WorldObject Create(WorldObject e, string name, Matrix transform)
	{
		((TriggerObject)e).triggerType = TriggerTypes.Undeclared;
		e.objType = ObjectTypes.Trigger;
		return e;
	}

	public override void PrevObject()
	{
	}

	public override void NextObject()
	{
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}

	public override void Draw(int qIndex, RenderPass pass)
	{
		base.Draw(qIndex, pass);
	}

	public override void DrawEditor(ref Vector2 textPos, float scale, float fontHeight)
	{
		base.DrawEditor(ref textPos, scale, fontHeight);
		textPos.Y += fontHeight;
		Menu.spriteBatch.DrawString(Menu.defaultFont, "TriggerType: " + triggerType, textPos, new Color(255, 255, 255, 255), 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
	}

	public override MaterialType RayCast(int qIndex, ref Vector3 origin, ref Vector3 direction, ref Vector3 hitPos, ref Vector3 hitNorm)
	{
		return MaterialType.Undefined;
	}
}
