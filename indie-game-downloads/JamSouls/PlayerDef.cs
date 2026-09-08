using Microsoft.Xna.Framework;

namespace JamSouls;

public struct PlayerDef
{
	public PlayerController Controller;

	public int CharacterIdx;

	public PlayerIndex pIndex;

	public int SlotIdx;

	public string Name;

	public PlayerConfig.SBIRE_DEF SbireDef;
}
