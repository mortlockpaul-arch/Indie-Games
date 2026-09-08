using System.IO;
using Eyehook.Framework;
using Loot.Encounters;
using Loot.Screens;
using Microsoft.Xna.Framework;

namespace Loot.Widgets;

public class WidgetEncounter : WidgetAction
{
	private Encounter encounter;

	protected override Sprite Sprite => WidgetSprite.Encounter;

	public WidgetEncounter(Encounter encounter)
		: base(encounter.Location)
	{
		this.encounter = encounter;
	}

	public WidgetEncounter(BinaryReader reader)
		: base(reader)
	{
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
		encounter.Update(gameTime);
	}

	public override void OnClick()
	{
		base.OnClick();
		PlaySound.Encounter();
		MC.ScreenManager.addScreen(new RippleScreen(clickEncounter));
	}

	private void clickEncounter()
	{
		encounter.OnClick();
	}

	public override void Read(BinaryReader reader)
	{
		base.Read(reader);
		encounter = EncounterRegistry.Load(reader);
	}

	public override void Write(BinaryWriter writer)
	{
		base.Write(writer);
		EncounterRegistry.Save(writer, encounter);
	}
}
