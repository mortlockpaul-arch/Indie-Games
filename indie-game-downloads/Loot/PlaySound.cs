using Eyehook.Framework;

namespace Loot;

public static class PlaySound
{
	private const string awardment = "Awardment";

	private const string coinCue = "coin";

	private const string critCue = "critical";

	private const string deathCue = "Death";

	private const string doorCue = "door";

	private const string encounterCue = "Encounter";

	private const string failCue = "fail";

	private const string goDownCue = "goDown";

	private const string gongCue = "gong";

	private const string heartbeatCue = "heartbeat";

	private const string hitCue = "hit";

	private const string jumpCue = "jump";

	private const string maxChain = "maxChain";

	private const string missCue = "miss";

	private const string pickupCue = "pickup";

	private const string poofCue = "poof";

	private const string potion = "potion";

	private const string resurrectCue = "Resurrect";

	private const string secretDoorCue = "secretDoor";

	private const string squishCue = "squish";

	private const string swingCue = "swing";

	private const string windCue = "wind";

	private const string woodBlockCue = "woodBlock";

	private const string woodBlockHighCue = "woodBlockHigh";

	private const string zotCue = "zot";

	public static void Awardment()
	{
		MC.AudioManager.playCue("Awardment");
	}

	public static void Coin()
	{
		MC.AudioManager.playCue("coin");
	}

	public static void Critical()
	{
		MC.AudioManager.playCue("critical");
	}

	public static void Death()
	{
		MC.AudioManager.playCue("Death");
	}

	public static void Door()
	{
		MC.AudioManager.playCue("door");
	}

	public static void Chest()
	{
		MC.AudioManager.playCue("door");
	}

	public static void DustCloud()
	{
		MC.AudioManager.playCue("miss");
	}

	public static void Encounter()
	{
		MC.AudioManager.playCue("Encounter");
	}

	public static void Fail()
	{
		MC.AudioManager.playCue("fail");
	}

	public static void GoDown()
	{
		MC.AudioManager.playCue("goDown");
	}

	public static void Gong()
	{
		MC.AudioManager.playCue("gong");
	}

	public static void Heartbeat()
	{
		MC.AudioManager.playCue("heartbeat");
	}

	public static void Hit()
	{
		MC.AudioManager.playCue("hit");
	}

	public static void Jump()
	{
		MC.AudioManager.playCue("jump");
	}

	public static void LevelUp()
	{
		MC.AudioManager.playCue("gong");
	}

	public static void MaxChain()
	{
		MC.AudioManager.playCue("maxChain");
	}

	public static void MenuMove()
	{
		MC.AudioManager.playCue("woodBlock");
	}

	public static void MenuCancel()
	{
		MC.AudioManager.playCue("woodBlockHigh");
	}

	public static void MenuClick()
	{
		MC.AudioManager.playCue("woodBlockHigh");
	}

	public static void Miss()
	{
		MC.AudioManager.playCue("miss");
	}

	public static void PickUp()
	{
		MC.AudioManager.playCue("pickup");
	}

	public static void Poof()
	{
		MC.AudioManager.playCue("poof");
	}

	public static void Potion()
	{
		MC.AudioManager.playCue("potion");
	}

	public static void Resurrect()
	{
		MC.AudioManager.playCue("Resurrect");
	}

	public static void SecretDoor()
	{
		MC.AudioManager.playCue("secretDoor");
	}

	public static void Squish()
	{
		MC.AudioManager.playCue("squish");
	}

	public static void Swing()
	{
		MC.AudioManager.playCue("swing");
	}

	public static void Wind()
	{
		MC.AudioManager.playCue("wind");
	}

	public static void WoodBlock()
	{
		MC.AudioManager.playCue("woodBlock");
	}

	public static void WoodBlockHigh()
	{
		MC.AudioManager.playCue("woodBlockHigh");
	}

	public static void Zot()
	{
		MC.AudioManager.playCue("zot");
	}
}
