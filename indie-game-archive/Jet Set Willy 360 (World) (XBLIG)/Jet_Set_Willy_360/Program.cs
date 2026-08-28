using Elite.Core;
using Elite.Core.Shared;

namespace Jet_Set_Willy_360;

internal static class Program
{
	private static void Main(string[] args)
	{
		GameFile gameFile = new GameFile();
		gameFile.ButtonY = null;
		gameFile.GameSaveContainerName = "Jet Set Willy Game Saves";
		gameFile.GameVersions = new GameVersion[2]
		{
			new GameVersion
			{
				GameFile = "jetset_k.z80",
				Name = "NORMAL"
			},
			new GameVersion
			{
				GameFile = "jetset_k_cheat.z80",
				Name = "CHEAT"
			}
		};
		gameFile.InfoPages = new InfoPage[2]
		{
			new InfoPage
			{
				Header = "help",
				Name = "help",
				NumberOfPages = 8,
				InformationText = "CONTROLS:\nLEFT - Walk Left \nRIGHT - Walk Right \nA - Jump / Start game\nBACK - Exit game or Menu \n\nMiner Willy, intrepid explorer and noveau-riche socialite, has been reaping the benefits of his fortunate discovery in surbiton. He has a yacht, a cliff-top mansion, an italian housekeeper and a French cook, and hundreds of new found friends who REALLY know how to enjoy themselves at a party.\r\n\r\nHis housekeeper Maria, however, takes a very dim view of all his revelry, and finally after a particularly boisterous thrash she puts her foot down. When the last of the louts disappears down the drive in his Aston Martin, all Willy can think about is crashing out in his four-poster. But Maria won't let him into his room until ALL the discarded glasses and bottles have been cleared away.\r\n\r\nCan you help Willy out of his dilemma? He hasn't explored his mansion properly yet (it IS a large place and he HAS been VERY busy) and there are some very strange things going on in the further recesses of the house (I wonder what the last owner WAS doing in his laboratory the night he disappeared).\r\n\r\nYou should manage O.K though you will probably find some loonies have been up on the roof and I would check down the road and on the beach if I was you. Good luck and don't worry, all you can lose in this game is sleep.\r\n"
			},
			new InfoPage
			{
				Header = "history",
				Name = "history",
				NumberOfPages = 36,
				InformationText = "JET SET WILLY HISTORY\r\n\r\nJet Set Willy is a computer game originally written for the ZX Spectrum home computer. It was published in 1984 by Software Projects and ported to most home computers of the time.\r\n\r\nThe game is a sequel to Manic Miner (1983), and is the second game in the immensely popular Miner Willy Series. It was a significant development in the platform game genre on the home micro.\r\n\r\nPLOT\r\nA tired Miner Willy has to tidy up all the items left around his house after a huge party. With this done his housekeeper Maria will allow him access to his bedroom. Willy's mansion was bought with the wealth obtained from his adventures in Manic Miner but much of it remains unexplored and it appears to be full of strange creatures, possibly a result of the previous (missing) owner's experiments. Willy must explore the enormous mansion and its grounds (including a beach and a yacht) to fully tidy up the house so he can get some much needed sleep.\r\n\r\nGAMEPLAY\r\nJet Set Willy is a platform game in which the player moves the protagonist, Willy, from room to room in his mansion collecting objects. The game is an early example of a nonlinear title since, unlike the screen-by-screen style of its prequel, the player can explore the mansion at will and tackle the screens in the order of their choosing. Willy is controlled using only left, right and jump. He can climb stairs by walking into them (jumping through them to avoid them) and climb swinging ropes by pushing left or right depending on what direction the rope is swinging in. The play area itself consists of 60 flick-screens making-up the mansion and its grounds and containing hazards (static killer objects), patrolling monsters (killer guardians which move along predetermined paths), various platforms and collectable objects. The collectable items glow to distinguish them from other items in the room.\r\nWilly loses a life if he touches an enemy or falls too far, and he is returned to the point at which he entered the room. This may lead to a game-ending situation in which Willy repeatedly falls from a height, losing all lives in succession.\r\n\r\nBUGS\r\nAs originally released, the game could not be completed due to several bugs. Although actually four completely unrelated issues, they became known collectively as 'The Attic Bug'. After the player entered the room The Attic, various rooms would undergo corruption on all subsequent game plays, including all monsters disappearing from The Chapel, and other screens triggering instant death. This was caused by an error in the path of an arrow in The Attic, resulting in the sprite travelling past the end of the Spectrum's video memory and overwriting crucial game data instead. This bears similarities to a buffer overflow, and as such is an early example of such an error - and the problems it can cause.\r\nInitially Software Projects attempted to pass off this bug off as an intentional feature to make the game more difficult, claiming that the rooms in question were filled with poison gas. However, they later rescinded this claim and issued a set of POKEs to correct the flaws.\r\nOther bugs included a case where an item under The Conservatory Roof was placed too close to both the screen entrance and a killer object making it impossible to collect. The Software Projects fix removed the killer object. There was also an invisible and impossible to reach item in First Landing. The Software Projects fix relocated the item to The Hall - although some fixes relocated the object to The Bathroom where it became visible as another tap item, by poking value 33, instead of 11.\r\nThe Banyan Tree was impassable in an upward direction - the Software Projects fix changed the status of an essential block from solid to passable.\r\n\r\nTHE QUIRKAFLEEG\r\nOne of the more bizarrely named rooms in the game is We Must Perform A Quirkafleeg. (The pre-release name for the screen was 'The Gaping Pit'.) This is a reference to the comic strip Fat Freddy's Cat, a spin-off from the Fabulous Furry Freak Brothers; in the original comic, the quirkafleeg was an obscure ritual in a foreign country, required to be performed upon the sight of dead furry animals.\r\n\r\nPIRACY PROTECTION\r\nLike most ZX Spectrum games, Jet Set Willy was stored on a cassette tape. Simply making an audio copy of the cassette allowed people to easily copy Spectrum games. Jet Set Willy was one of the first to come with a form of copy protection: a card with 180 coloured codes on it was bundled with the cassette. Upon loading, one of the codes from the card had to be entered before the game would start. Although the cassette could be duplicated, a copy of the card was also needed and at the time, home colour reproduction was hard to do. Thus copying Jet Set Willy was trickier than most Spectrum games. However, means of circumventing the card were quickly found. Reflecting a different attitude to software piracy at the time, one method was published in a UK computer magazine.\r\n"
			}
		};
		gameFile.StartInstructions = "Press Y to Start";
		gameFile.Name = "Jet Set Willy";
		gameFile.HeaderTextures = new string[4] { "willy1", "willy2", "willy3", "willy4" };
		gameFile.HeaderTextureSpeed = 25;
		gameFile.HeaderX = 370f;
		gameFile.HeaderY = 100f;
		gameFile.CopyrightText = "TM & © 1984-2012";
		using ZXGame zXGame = new ZXGame(gameFile);
		zXGame.Run();
	}
}
