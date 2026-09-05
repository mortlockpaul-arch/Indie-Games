using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LightsOnCrazies;

public class Game1 : Game
{
	private GraphicsDeviceManager graphics;

	private SpriteBatch spriteBatch;

	public SpriteFont font;

	public SoundEffect[] sounds = new SoundEffect[9];

	public SoundEffect[] intros = new SoundEffect[13];

	private bool thePlayer = false;

	private bool loaded = false;

	private int doomed = 0;

	public bool demo = false;

	private PlayerIndex myIndex;

	private GamePadState previous;

	private int death_timer_max = 4;

	private bool dead = false;

	private string death_type = "scare";

	private int death = 0;

	private int death_index_start = 0;

	private int death_index_end = 0;

	private int light_between_time = 5;

	private int level = 0;

	private int game_timer = 0;

	private int game_begin_time = 0;

	private int between_levels = 0;

	private int between_levels_max = 300;

	private bool motion = false;

	private int motion_time = 0;

	private int max_motion_time = 20;

	private Color cam_text = Color.White;

	private bool to_hallway = false;

	private int hallway_view = 0;

	private int hallway_index = 0;

	private int load_time = 0;

	private int look_room = 10;

	private bool light_on = false;

	private int light_index;

	private int light_delay;

	private int light_delay_max = 25;

	private bool fixing_light = false;

	private int fixing_light_time_max = 400;

	private int fixing_light_time = 0;

	private float batteries = 100f;

	private int blink_time = 0;

	private int blink_time_max = 10;

	private int fixing_index = 0;

	private string look_location = "Front Door";

	private Texture2D current_room;

	private Texture2D map;

	private Texture2D buttons;

	private Texture2D title;

	private bool on_computer = true;

	private Texture2D eye;

	private Vector2 eye_coord = new Vector2(0f, 0f);

	private int camera_flash = 0;

	private int camera_flash_max = 8;

	private Texture2D statics;

	private int tutorial = 0;

	private bool new_level = true;

	private Texture2D[] laptop = new Texture2D[8];

	private Texture2D[] light = new Texture2D[5];

	private Texture2D[] B1 = new Texture2D[5];

	private Texture2D[] B2 = new Texture2D[5];

	private Texture2D[] Basement = new Texture2D[5];

	private Texture2D[] Bed = new Texture2D[5];

	private Texture2D[] Dining = new Texture2D[5];

	private Texture2D[] Family = new Texture2D[5];

	private Texture2D[] Front = new Texture2D[6];

	private Texture2D[] Kill = new Texture2D[52];

	private Texture2D[] Kitchen = new Texture2D[5];

	private Texture2D[] Laundry = new Texture2D[5];

	private Texture2D[] Mud = new Texture2D[5];

	private Texture2D[] Office = new Texture2D[5];

	private Texture2D[] Rec = new Texture2D[5];

	private Texture2D[] Scare = new Texture2D[57];

	private Texture2D[] Upstairs = new Texture2D[5];

	private Texture2D[] Workout = new Texture2D[5];

	private Texture2D[] Fixing = new Texture2D[22];

	private Texture2D[] Fixing_End = new Texture2D[12];

	private Room[] rooms = new Room[15];

	private List<Crazy> crazies = new List<Crazy>();

	public Game1()
	{
		graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
	}

	protected override void Initialize()
	{
		between_levels = 0;
		map = base.Content.Load<Texture2D>("map");
		eye = base.Content.Load<Texture2D>("eye");
		buttons = base.Content.Load<Texture2D>("buttons");
		statics = base.Content.Load<Texture2D>("static1");
		title = base.Content.Load<Texture2D>("FrightLight");
		intros[0] = base.Content.Load<SoundEffect>("intro_1");
		intros[1] = base.Content.Load<SoundEffect>("intro_2");
		intros[2] = base.Content.Load<SoundEffect>("intro_3");
		intros[3] = base.Content.Load<SoundEffect>("intro_4");
		intros[4] = base.Content.Load<SoundEffect>("intro_5");
		intros[5] = base.Content.Load<SoundEffect>("intro_6");
		intros[6] = base.Content.Load<SoundEffect>("intro_7");
		intros[7] = base.Content.Load<SoundEffect>("intro_8");
		intros[8] = base.Content.Load<SoundEffect>("intro_9");
		intros[9] = base.Content.Load<SoundEffect>("intro_10");
		intros[10] = base.Content.Load<SoundEffect>("intro_11");
		intros[11] = base.Content.Load<SoundEffect>("intro_12");
		sounds[0] = base.Content.Load<SoundEffect>("chris_roar");
		sounds[1] = base.Content.Load<SoundEffect>("chris_roar");
		sounds[2] = base.Content.Load<SoundEffect>("surprise");
		sounds[3] = base.Content.Load<SoundEffect>("chris_roar");
		sounds[4] = base.Content.Load<SoundEffect>("chris_roar");
		sounds[5] = base.Content.Load<SoundEffect>("flashlight");
		sounds[6] = base.Content.Load<SoundEffect>("battery");
		sounds[7] = base.Content.Load<SoundEffect>("chh");
		sounds[8] = base.Content.Load<SoundEffect>("Siren");
		rooms[0] = new Room("Workout Room", Workout, new Vector2(80f, 15f));
		rooms[0].create_connection("Down", "Office");
		rooms[1] = new Room("Office", Office, new Vector2(82f, 38f));
		rooms[1].create_connection("Up", "Workout Room");
		rooms[1].create_connection("Right", "Hallway");
		rooms[1].create_connection("Down", "Bedroom");
		rooms[1].create_cam_connection("Right", "Front Door");
		rooms[2] = new Room("Bathroom 1", B1, new Vector2(23f, 60f));
		rooms[2].create_connection("Right", "Bedroom");
		rooms[3] = new Room("Bedroom", Bed, new Vector2(82f, 60f));
		rooms[3].create_connection("Up", "Office");
		rooms[3].create_connection("Down", "Rec Room");
		rooms[3].create_connection("Left", "Bathroom 1");
		rooms[3].create_cam_connection("Right", "Bathroom 2");
		rooms[5] = new Room("Rec Room", Rec, new Vector2(76f, 83f));
		rooms[5].create_connection("Up", "Bedroom");
		rooms[5].create_connection("Right", "Laundry");
		rooms[14] = new Room("Hallway", Office, new Vector2(0f, 0f));
		rooms[6] = new Room("Bathroom 2", B2, new Vector2(128f, 60f));
		rooms[6].create_connection("Up", "Hallway");
		rooms[6].create_connection("Down", "Laundry");
		rooms[6].create_cam_connection("Left", "Bedroom");
		rooms[6].create_cam_connection("Right", "Stairway");
		rooms[7] = new Room("Laundry", Laundry, new Vector2(120f, 84f));
		rooms[7].create_connection("Up", "Bathroom 2");
		rooms[7].create_connection("Left", "Rec Room");
		rooms[7].create_cam_connection("Right", "Mud Room");
		rooms[8] = new Room("Stairway", Upstairs, new Vector2(158f, 61f));
		rooms[8].create_connection("Up", "Hallway");
		rooms[8].create_connection("Down", "Mud Room");
		rooms[8].create_cam_connection("Left", "Bathroom 2");
		rooms[8].create_cam_connection("Right", "Kitchen");
		rooms[9] = new Room("Mud Room", Mud, new Vector2(162f, 83f));
		rooms[9].create_connection("Up", "Stairway");
		rooms[9].create_connection("Right", "Basement");
		rooms[9].create_cam_connection("Left", "Laundry");
		rooms[10] = new Room("Front Door", Front, new Vector2(212f, 32f));
		rooms[10].create_connection("Right", "Family Room");
		rooms[10].create_connection("Down", "Kitchen");
		rooms[10].create_connection("Left", "Hallway");
		rooms[10].create_cam_connection("Left", "Office");
		rooms[11] = new Room("Kitchen", Kitchen, new Vector2(214f, 61f));
		rooms[11].create_connection("Up", "Front Door");
		rooms[11].create_connection("Right", "Dining Room");
		rooms[11].create_connection("Down", "Basement");
		rooms[11].create_cam_connection("Left", "Stairway");
		rooms[12] = new Room("Basement", Basement, new Vector2(214f, 86f));
		rooms[12].create_connection("Up", "Kitchen");
		rooms[12].create_connection("Left", "Mud Room");
		rooms[13] = new Room("Family Room", Family, new Vector2(284f, 31f));
		rooms[13].create_connection("Down", "Dining Room");
		rooms[13].create_connection("Left", "Front Door");
		rooms[4] = new Room("Dining Room", Dining, new Vector2(284f, 60f));
		rooms[4].create_connection("Up", "Family Room");
		rooms[4].create_connection("Left", "Kitchen");
		base.Initialize();
	}

	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatch(base.GraphicsDevice);
		font = base.Content.Load<SpriteFont>("Font1");
	}

	protected override void UnloadContent()
	{
	}

	public string Reset_Crazy(string name)
	{
		bool flag = false;
		int num = 0;
		while (!flag)
		{
			Random random = new Random();
			num = random.Next(0, rooms.Count() - 1);
			bool flag2 = false;
			foreach (Crazy crazy in crazies)
			{
				if (crazy.name != name && crazy.room_name == rooms[num].name)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				flag = true;
			}
		}
		return rooms[num].name;
	}

	public void New_Level()
	{
		if (demo && level > 4)
		{
			level = 1;
			dead = false;
		}
		if (!dead)
		{
		}
		crazies.Clear();
		doomed = 0;
		to_hallway = false;
		on_computer = true;
		motion_time = 0;
		cam_text = Color.White;
		hallway_view = 0;
		hallway_index = 0;
		look_room = 10;
		light_on = false;
		light_index = 0;
		light_delay = 0;
		fixing_light = false;
		fixing_light_time = 0;
		batteries = 100f;
		blink_time = 0;
		fixing_index = 0;
		camera_flash = 0;
		look_location = "Front Door";
		current_room = Front[0];
		eye_coord = rooms[10].map_pos;
		switch (level)
		{
		case 1:
		{
			game_timer = 8000;
			game_begin_time = 4000;
			Crazy crazy = new Crazy("hockey", 0);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			break;
		}
		case 2:
		{
			game_timer = 5300;
			game_begin_time = 4000;
			Crazy crazy = new Crazy("hockey", 10);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("spider", 10);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			break;
		}
		case 3:
		{
			game_timer = 4900;
			game_begin_time = 4000;
			Crazy crazy = new Crazy("axe", 15);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("hockey", 15);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			break;
		}
		case 4:
		{
			game_timer = 4900;
			game_begin_time = 4000;
			Crazy crazy = new Crazy("wolf", 20);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("axe", 20);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("spider", 25);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			break;
		}
		case 5:
		{
			game_timer = 4700;
			game_begin_time = 4000;
			Crazy crazy = new Crazy("wolf", 20);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("axe", 20);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("spider", 20);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("hockey", 20);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			break;
		}
		case 6:
		{
			game_timer = 4500;
			game_begin_time = 4000;
			Crazy crazy = new Crazy("hockey", 25);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("spider", 25);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			break;
		}
		case 7:
		{
			game_timer = 4500;
			game_begin_time = 4000;
			Crazy crazy = new Crazy("wolf", 30);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("spider", 30);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			break;
		}
		case 8:
		{
			game_timer = 4500;
			game_begin_time = 4000;
			Crazy crazy = new Crazy("hockey", 40);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			break;
		}
		case 9:
		{
			game_timer = 4800;
			game_begin_time = 4000;
			Crazy crazy = new Crazy("wolf", 30);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("axe", 30);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("spider", 30);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("hockey", 30);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			break;
		}
		case 10:
		{
			game_timer = 5000;
			game_begin_time = 4000;
			Crazy crazy = new Crazy("axe", 30);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("spider", 30);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("hockey", 30);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("wolf", 30);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			break;
		}
		case 11:
		{
			game_timer = 4800;
			game_begin_time = 4000;
			Crazy crazy = new Crazy("hockey", 50);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			break;
		}
		case 12:
			game_timer = 1100;
			game_begin_time = 1;
			break;
		}
		if (level > 12)
		{
			game_timer = 4300;
			game_begin_time = 4000;
			Crazy crazy = new Crazy("axe", 30 + level - 10);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("spider", 30 + level - 10);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("hockey", 30 + level - 10);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
			crazy = new Crazy("wolf", 30 + level - 10);
			crazy.movement_time = crazy.max_movement_time;
			crazy.room_name = "You";
			crazies.Add(crazy);
		}
		if (new_level && level < 13)
		{
			intros[level - 1].Play();
		}
		else
		{
			game_timer = game_begin_time + 300;
		}
		new_level = false;
	}

	protected override void Update(GameTime gameTime)
	{
		if (!thePlayer)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed && !thePlayer)
			{
				myIndex = PlayerIndex.One;
				thePlayer = true;
			}
			if (GamePad.GetState(PlayerIndex.Two).Buttons.A == ButtonState.Pressed && !thePlayer)
			{
				myIndex = PlayerIndex.Two;
				thePlayer = true;
			}
			if (GamePad.GetState(PlayerIndex.Three).Buttons.A == ButtonState.Pressed && !thePlayer)
			{
				myIndex = PlayerIndex.Three;
				thePlayer = true;
			}
			if (GamePad.GetState(PlayerIndex.Four).Buttons.A == ButtonState.Pressed && !thePlayer)
			{
				myIndex = PlayerIndex.Four;
				thePlayer = true;
			}
		}
		else if (!loaded)
		{
			switch (load_time)
			{
			case 0:
				light[0] = base.Content.Load<Texture2D>("light_on_empty");
				light[1] = base.Content.Load<Texture2D>("light_on_spider");
				light[2] = base.Content.Load<Texture2D>("light_on_hockey");
				light[3] = base.Content.Load<Texture2D>("light_on_axe");
				light[4] = base.Content.Load<Texture2D>("light_on_wolf");
				break;
			case 5:
				laptop[0] = base.Content.Load<Texture2D>("laptop");
				laptop[1] = base.Content.Load<Texture2D>("laptop_to_hall_1");
				laptop[2] = base.Content.Load<Texture2D>("laptop_to_hall_2");
				laptop[3] = base.Content.Load<Texture2D>("laptop_to_hall_3");
				laptop[4] = base.Content.Load<Texture2D>("laptop_to_hall_4");
				laptop[5] = base.Content.Load<Texture2D>("laptop_to_hall_5");
				laptop[6] = base.Content.Load<Texture2D>("laptop_to_hall_6");
				laptop[7] = base.Content.Load<Texture2D>("laptop_to_hall_7");
				break;
			case 12:
				B1[0] = base.Content.Load<Texture2D>("B1-empty");
				B1[1] = base.Content.Load<Texture2D>("B1-spider");
				B1[2] = base.Content.Load<Texture2D>("B1-hockey");
				B1[3] = base.Content.Load<Texture2D>("B1-axe");
				B1[4] = base.Content.Load<Texture2D>("B1-wolf");
				break;
			case 17:
				B2[0] = base.Content.Load<Texture2D>("B2-empty");
				B2[1] = base.Content.Load<Texture2D>("B2-spider");
				B2[2] = base.Content.Load<Texture2D>("B2-hockey");
				B2[3] = base.Content.Load<Texture2D>("B2-axe");
				B2[4] = base.Content.Load<Texture2D>("B2-wolf");
				break;
			case 22:
				Basement[0] = base.Content.Load<Texture2D>("Basement-empty");
				Basement[1] = base.Content.Load<Texture2D>("Basement-spider");
				Basement[2] = base.Content.Load<Texture2D>("Basement-hockey");
				Basement[3] = base.Content.Load<Texture2D>("Basement-axe");
				Basement[4] = base.Content.Load<Texture2D>("Basement-wolf");
				break;
			case 27:
				Bed[0] = base.Content.Load<Texture2D>("Bed-empty");
				Bed[1] = base.Content.Load<Texture2D>("Bed-spider");
				Bed[2] = base.Content.Load<Texture2D>("Bed-hockey");
				Bed[3] = base.Content.Load<Texture2D>("Bed-axe");
				Bed[4] = base.Content.Load<Texture2D>("Bed-wolf");
				break;
			case 33:
				Dining[0] = base.Content.Load<Texture2D>("dining-empty");
				Dining[1] = base.Content.Load<Texture2D>("dining-spider");
				Dining[2] = base.Content.Load<Texture2D>("dining-hockey");
				Dining[3] = base.Content.Load<Texture2D>("dining-axe");
				Dining[4] = base.Content.Load<Texture2D>("dining-wolf");
				break;
			case 38:
				Family[0] = base.Content.Load<Texture2D>("family-empty");
				Family[1] = base.Content.Load<Texture2D>("family-spider");
				Family[2] = base.Content.Load<Texture2D>("family-hockey");
				Family[3] = base.Content.Load<Texture2D>("family-axe");
				Family[4] = base.Content.Load<Texture2D>("family-wolf");
				break;
			case 43:
				Front[0] = base.Content.Load<Texture2D>("front-open");
				Front[1] = base.Content.Load<Texture2D>("front-spider");
				Front[2] = base.Content.Load<Texture2D>("front-hockey");
				Front[3] = base.Content.Load<Texture2D>("front-axe");
				Front[4] = base.Content.Load<Texture2D>("front-wolf");
				Front[5] = base.Content.Load<Texture2D>("front-shut");
				break;
			case 63:
				Kill[0] = base.Content.Load<Texture2D>("kill_axe_1");
				Kill[1] = base.Content.Load<Texture2D>("kill_axe_2");
				Kill[2] = base.Content.Load<Texture2D>("kill_axe_3");
				Kill[3] = base.Content.Load<Texture2D>("kill_axe_4");
				Kill[4] = base.Content.Load<Texture2D>("kill_axe_5");
				Kill[5] = base.Content.Load<Texture2D>("kill_axe_6");
				Kill[6] = base.Content.Load<Texture2D>("kill_axe_7");
				Kill[7] = base.Content.Load<Texture2D>("kill_axe_8");
				Kill[8] = base.Content.Load<Texture2D>("kill_axe_9");
				Kill[9] = base.Content.Load<Texture2D>("kill_axe_10");
				Kill[17] = base.Content.Load<Texture2D>("kill_hockey_1");
				Kill[18] = base.Content.Load<Texture2D>("kill_hockey_2");
				Kill[19] = base.Content.Load<Texture2D>("kill_hockey_3");
				Kill[20] = base.Content.Load<Texture2D>("kill_hockey_4");
				Kill[21] = base.Content.Load<Texture2D>("kill_hockey_5");
				Kill[22] = base.Content.Load<Texture2D>("kill_hockey_6");
				Kill[23] = base.Content.Load<Texture2D>("kill_hockey_7");
				Kill[24] = base.Content.Load<Texture2D>("kill_hockey_8");
				Kill[25] = base.Content.Load<Texture2D>("kill_hockey_9");
				Kill[26] = base.Content.Load<Texture2D>("kill_hockey_10");
				Kill[27] = base.Content.Load<Texture2D>("kill_spider_1");
				Kill[28] = base.Content.Load<Texture2D>("kill_spider_2");
				Kill[29] = base.Content.Load<Texture2D>("kill_spider_3");
				Kill[30] = base.Content.Load<Texture2D>("kill_spider_4");
				Kill[31] = base.Content.Load<Texture2D>("kill_spider_5");
				Kill[32] = base.Content.Load<Texture2D>("kill_spider_6");
				Kill[33] = base.Content.Load<Texture2D>("kill_spider_7");
				Kill[34] = base.Content.Load<Texture2D>("kill_spider_8");
				Kill[35] = base.Content.Load<Texture2D>("kill_spider_9");
				Kill[36] = base.Content.Load<Texture2D>("kill_spider_10");
				Kill[37] = base.Content.Load<Texture2D>("kill_wolf_1");
				Kill[38] = base.Content.Load<Texture2D>("kill_wolf_2");
				Kill[39] = base.Content.Load<Texture2D>("kill_wolf_3");
				Kill[40] = base.Content.Load<Texture2D>("kill_wolf_4");
				Kill[41] = base.Content.Load<Texture2D>("kill_wolf_5");
				Kill[42] = base.Content.Load<Texture2D>("kill_wolf_6");
				Kill[43] = base.Content.Load<Texture2D>("kill_wolf_7");
				Kill[44] = base.Content.Load<Texture2D>("kill_wolf_8");
				Kill[45] = base.Content.Load<Texture2D>("kill_wolf_9");
				Kill[46] = base.Content.Load<Texture2D>("kill_wolf_10");
				Kill[47] = base.Content.Load<Texture2D>("kill_wolf_11");
				Kill[48] = base.Content.Load<Texture2D>("kill_wolf_12");
				Kill[49] = base.Content.Load<Texture2D>("kill_wolf_13");
				Kill[50] = base.Content.Load<Texture2D>("kill_wolf_14");
				Kill[51] = base.Content.Load<Texture2D>("kill_wolf_15");
				break;
			case 68:
				Kitchen[0] = base.Content.Load<Texture2D>("kitchen-empty");
				Kitchen[1] = base.Content.Load<Texture2D>("kitchen-spider");
				Kitchen[2] = base.Content.Load<Texture2D>("kitchen-hockey");
				Kitchen[3] = base.Content.Load<Texture2D>("kitchen-axe");
				Kitchen[4] = base.Content.Load<Texture2D>("kitchen-wolf");
				break;
			case 73:
				Laundry[0] = base.Content.Load<Texture2D>("laundry-empty");
				Laundry[1] = base.Content.Load<Texture2D>("laundry-spider");
				Laundry[2] = base.Content.Load<Texture2D>("laundry-hockey");
				Laundry[3] = base.Content.Load<Texture2D>("laundry-axe");
				Laundry[4] = base.Content.Load<Texture2D>("laundry-wolf");
				break;
			case 78:
				Mud[0] = base.Content.Load<Texture2D>("mud-empty");
				Mud[1] = base.Content.Load<Texture2D>("mud-spider");
				Mud[2] = base.Content.Load<Texture2D>("mud-hockey");
				Mud[3] = base.Content.Load<Texture2D>("mud-axe");
				Mud[4] = base.Content.Load<Texture2D>("mud-wolf");
				Fixing[0] = base.Content.Load<Texture2D>("fixing_1");
				Fixing[1] = base.Content.Load<Texture2D>("fixing_2");
				Fixing[2] = base.Content.Load<Texture2D>("fixing_3");
				Fixing[3] = base.Content.Load<Texture2D>("fixing_4");
				Fixing[4] = base.Content.Load<Texture2D>("fixing_5");
				Fixing[5] = base.Content.Load<Texture2D>("fixing_6");
				Fixing[6] = base.Content.Load<Texture2D>("fixing_7");
				Fixing[7] = base.Content.Load<Texture2D>("fixing_8");
				Fixing[8] = base.Content.Load<Texture2D>("fixing_9");
				Fixing[9] = base.Content.Load<Texture2D>("fixing_10");
				Fixing[10] = base.Content.Load<Texture2D>("fixing_11");
				Fixing[11] = base.Content.Load<Texture2D>("fixing_11");
				Fixing[12] = base.Content.Load<Texture2D>("fixing_13");
				Fixing[13] = base.Content.Load<Texture2D>("fixing_14");
				Fixing[14] = base.Content.Load<Texture2D>("fixing_15");
				Fixing[15] = base.Content.Load<Texture2D>("fixing_17");
				Fixing[16] = base.Content.Load<Texture2D>("fixing_17");
				Fixing[17] = base.Content.Load<Texture2D>("fixing_18");
				Fixing[18] = base.Content.Load<Texture2D>("fixing_19");
				Fixing[19] = base.Content.Load<Texture2D>("fixing_19");
				Fixing[20] = base.Content.Load<Texture2D>("fixing_21");
				Fixing[21] = base.Content.Load<Texture2D>("fixing_22");
				break;
			case 83:
				Office[0] = base.Content.Load<Texture2D>("office-empty");
				Office[1] = base.Content.Load<Texture2D>("office-spider");
				Office[2] = base.Content.Load<Texture2D>("office-hockey");
				Office[3] = base.Content.Load<Texture2D>("office-axe");
				Office[4] = base.Content.Load<Texture2D>("office-wolf");
				break;
			case 88:
				Rec[0] = base.Content.Load<Texture2D>("rec-empty");
				Rec[1] = base.Content.Load<Texture2D>("rec-spider");
				Rec[2] = base.Content.Load<Texture2D>("rec-hockey");
				Rec[3] = base.Content.Load<Texture2D>("rec-axe");
				Rec[4] = base.Content.Load<Texture2D>("rec-wolf");
				Fixing_End[0] = base.Content.Load<Texture2D>("fixing_end_1");
				Fixing_End[1] = base.Content.Load<Texture2D>("fixing_end_2");
				Fixing_End[2] = base.Content.Load<Texture2D>("fixing_end_3");
				Fixing_End[3] = base.Content.Load<Texture2D>("fixing_end_4");
				Fixing_End[4] = base.Content.Load<Texture2D>("fixing_end_5");
				Fixing_End[5] = base.Content.Load<Texture2D>("fixing_end_6");
				Fixing_End[6] = base.Content.Load<Texture2D>("fixing_end_7");
				Fixing_End[7] = base.Content.Load<Texture2D>("fixing_end_8");
				Fixing_End[8] = base.Content.Load<Texture2D>("fixing_end_9");
				Fixing_End[9] = base.Content.Load<Texture2D>("fixing_end_10");
				Fixing_End[10] = base.Content.Load<Texture2D>("fixing_end_11");
				Fixing_End[11] = base.Content.Load<Texture2D>("fixing_end12");
				break;
			case 90:
				Scare[0] = base.Content.Load<Texture2D>("scare_axe_1");
				Scare[1] = base.Content.Load<Texture2D>("scare_axe_2");
				Scare[2] = base.Content.Load<Texture2D>("scare_axe_3");
				Scare[3] = base.Content.Load<Texture2D>("scare_axe_4");
				Scare[4] = base.Content.Load<Texture2D>("scare_axe_5");
				Scare[5] = base.Content.Load<Texture2D>("scare_axe_10");
				Scare[6] = base.Content.Load<Texture2D>("scare_axe_11");
				Scare[7] = base.Content.Load<Texture2D>("scare_axe_12");
				Scare[8] = base.Content.Load<Texture2D>("scare_axe_13");
				Scare[9] = base.Content.Load<Texture2D>("scare_axe_14");
				Scare[10] = base.Content.Load<Texture2D>("scare_axe_15");
				Scare[11] = base.Content.Load<Texture2D>("scare_axe_16");
				Scare[12] = base.Content.Load<Texture2D>("scare_axe_17");
				Scare[13] = base.Content.Load<Texture2D>("scare_axe_18");
				Scare[18] = base.Content.Load<Texture2D>("scare_hockey_1");
				Scare[19] = base.Content.Load<Texture2D>("scare_hockey_2");
				Scare[20] = base.Content.Load<Texture2D>("scare_hockey_3");
				Scare[21] = base.Content.Load<Texture2D>("scare_hockey_4");
				Scare[22] = base.Content.Load<Texture2D>("scare_hockey_5");
				Scare[23] = base.Content.Load<Texture2D>("scare_hockey_6");
				Scare[24] = base.Content.Load<Texture2D>("scare_hockey_7");
				Scare[25] = base.Content.Load<Texture2D>("scare_hockey_8");
				Scare[26] = base.Content.Load<Texture2D>("scare_hockey_9");
				Scare[27] = base.Content.Load<Texture2D>("scare_hockey_10");
				Scare[28] = base.Content.Load<Texture2D>("scare_hockey_11");
				Scare[29] = base.Content.Load<Texture2D>("scare_hockey_12");
				Scare[30] = base.Content.Load<Texture2D>("scare_spider_1");
				Scare[31] = base.Content.Load<Texture2D>("scare_spider_2");
				Scare[32] = base.Content.Load<Texture2D>("scare_spider_3");
				Scare[33] = base.Content.Load<Texture2D>("scare_spider_4");
				Scare[34] = base.Content.Load<Texture2D>("scare_spider_5");
				Scare[35] = base.Content.Load<Texture2D>("scare_spider_6");
				Scare[36] = base.Content.Load<Texture2D>("scare_spider_7");
				Scare[37] = base.Content.Load<Texture2D>("scare_spider_8");
				Scare[38] = base.Content.Load<Texture2D>("scare_spider_9");
				Scare[39] = base.Content.Load<Texture2D>("scare_spider_10");
				Scare[40] = base.Content.Load<Texture2D>("scare_spider_11");
				Scare[41] = base.Content.Load<Texture2D>("scare_spider_12");
				Scare[42] = base.Content.Load<Texture2D>("scare_spider_13");
				Scare[43] = base.Content.Load<Texture2D>("scare_wolf_1");
				Scare[44] = base.Content.Load<Texture2D>("scare_wolf_2");
				Scare[45] = base.Content.Load<Texture2D>("scare_wolf_3");
				Scare[46] = base.Content.Load<Texture2D>("scare_wolf_4");
				Scare[47] = base.Content.Load<Texture2D>("scare_wolf_5");
				Scare[48] = base.Content.Load<Texture2D>("scare_wolf_6");
				Scare[49] = base.Content.Load<Texture2D>("scare_wolf_7");
				Scare[50] = base.Content.Load<Texture2D>("scare_wolf_8");
				Scare[51] = base.Content.Load<Texture2D>("scare_wolf_9");
				Scare[52] = base.Content.Load<Texture2D>("scare_wolf_10");
				Scare[53] = base.Content.Load<Texture2D>("scare_wolf_11");
				Scare[54] = base.Content.Load<Texture2D>("scare_wolf_12");
				Scare[55] = base.Content.Load<Texture2D>("scare_wolf_13");
				Scare[56] = base.Content.Load<Texture2D>("scare_wolf_14");
				break;
			case 95:
				Upstairs[0] = base.Content.Load<Texture2D>("upstairs-empty");
				Upstairs[1] = base.Content.Load<Texture2D>("upstairs-spider");
				Upstairs[2] = base.Content.Load<Texture2D>("upstairs-hockey");
				Upstairs[3] = base.Content.Load<Texture2D>("upstairs-axe");
				Upstairs[4] = base.Content.Load<Texture2D>("upstairs-wolf");
				break;
			case 99:
				Workout[0] = base.Content.Load<Texture2D>("workout-empty");
				Workout[1] = base.Content.Load<Texture2D>("workout-spider");
				Workout[2] = base.Content.Load<Texture2D>("workout-hockey");
				Workout[3] = base.Content.Load<Texture2D>("workout-axe");
				Workout[4] = base.Content.Load<Texture2D>("workout-wolf");
				break;
			case 100:
				loaded = true;
				look_location = "Front Door";
				current_room = Front[0];
				eye_coord = rooms[10].map_pos;
				level = 1;
				New_Level();
				break;
			}
			load_time++;
		}
		else if (dead && doomed == 0)
		{
			if (death > 0)
			{
				death--;
			}
			if (death == 0)
			{
				death = death_timer_max;
				death_index_start++;
				if (death_index_start > death_index_end)
				{
					New_Level();
					death = -1;
				}
			}
			if (death < 0)
			{
				death--;
			}
			if (death == -1 * between_levels_max)
			{
				death = 0;
				dead = false;
			}
		}
		else
		{
			if (doomed > 0)
			{
				doomed--;
			}
			if (game_timer <= 0)
			{
				if (between_levels == 0)
				{
					if (level < 12)
					{
						sounds[8].Play();
					}
					between_levels = between_levels_max;
				}
				between_levels--;
				if (between_levels == 1)
				{
					new_level = true;
					level++;
					between_levels = 0;
					New_Level();
				}
			}
			else
			{
				GamePadState state = GamePad.GetState(myIndex);
				if (game_timer == game_begin_time)
				{
					foreach (Crazy crazy in crazies)
					{
						crazy.room_name = Reset_Crazy(crazy.name);
						crazy.movement_time = crazy.max_movement_time;
						if (crazy.room_name == look_location)
						{
							camera_flash = camera_flash_max;
						}
					}
				}
				if (game_timer < game_begin_time)
				{
					foreach (Crazy crazy2 in crazies)
					{
						if (doomed > 0 && hallway_index >= 6)
						{
							doomed = 0;
						}
						if (crazy2.room_name == "You" && dead && doomed == 0)
						{
							if (on_computer)
							{
								death_type = "scare";
								switch (crazy2.name)
								{
								case "axe":
									death_index_start = 0;
									death_index_end = 13;
									sounds[0].Play();
									death_timer_max = 3;
									break;
								case "hockey":
									death_index_start = 18;
									death_index_end = 29;
									sounds[0].Play();
									death_timer_max = 4;
									break;
								case "spider":
									death_index_start = 30;
									death_index_end = 42;
									sounds[0].Play();
									death_timer_max = 4;
									break;
								case "wolf":
									death_index_start = 43;
									death_index_end = 56;
									sounds[0].Play();
									death_timer_max = 3;
									break;
								}
							}
							else
							{
								death_type = "kill";
								switch (crazy2.name)
								{
								case "axe":
									death_index_start = 0;
									death_index_end = 9;
									sounds[0].Play();
									death_timer_max = 3;
									break;
								case "hockey":
									death_index_start = 17;
									death_index_end = 26;
									sounds[0].Play();
									death_timer_max = 5;
									break;
								case "spider":
									death_index_start = 27;
									death_index_end = 36;
									sounds[0].Play();
									death_timer_max = 5;
									break;
								case "wolf":
									death_index_start = 37;
									death_index_end = 51;
									sounds[0].Play();
									death_timer_max = 3;
									break;
								}
							}
							death = death_timer_max * 2;
						}
						if (crazy2.room_name == "You" && !dead)
						{
							dead = true;
							if (on_computer)
							{
								doomed = 200;
							}
							else
							{
								death_type = "kill";
								switch (crazy2.name)
								{
								case "axe":
									death_index_start = 0;
									death_index_end = 9;
									sounds[0].Play();
									death_timer_max = 3;
									break;
								case "hockey":
									death_index_start = 17;
									death_index_end = 26;
									sounds[0].Play();
									death_timer_max = 5;
									break;
								case "spider":
									death_index_start = 27;
									death_index_end = 36;
									sounds[0].Play();
									death_timer_max = 5;
									break;
								case "wolf":
									death_index_start = 37;
									death_index_end = 51;
									sounds[0].Play();
									death_timer_max = 3;
									break;
								}
								death = death_timer_max * 2;
							}
						}
						if (!crazy2.light)
						{
							crazy2.movement_time--;
							if (crazy2.movement_time > 0)
							{
								continue;
							}
							crazy2.movement_time = crazy2.max_movement_time;
							string room_name = crazy2.room_name;
							string text = crazy2.room_name;
							for (int i = 0; i < rooms.Count(); i++)
							{
								if (rooms[i].name == crazy2.room_name)
								{
									text = rooms[i].Move_Decide();
									break;
								}
							}
							for (int i = 0; i < crazies.Count(); i++)
							{
								if (crazy2.name != crazies[i].name && text == crazies[i].room_name)
								{
									text = room_name;
									break;
								}
							}
							if (room_name == look_location || text == look_location)
							{
								camera_flash = camera_flash_max;
							}
							crazy2.room_name = text;
						}
						else
						{
							if (crazy2.special_timer > 0)
							{
								crazy2.special_timer--;
							}
							if (crazy2.special_timer == 0)
							{
								crazy2.room_name = Reset_Crazy(crazy2.name);
								crazy2.movement_time = crazy2.max_movement_time;
								crazy2.light = false;
							}
						}
					}
				}
				if (camera_flash > 0)
				{
					if (camera_flash == camera_flash_max && on_computer)
					{
						sounds[7].Play();
					}
					camera_flash--;
				}
				if (on_computer)
				{
					if (tutorial == 8)
					{
						tutorial = 9;
					}
					if (fixing_light)
					{
						if (tutorial == 9)
						{
							tutorial = 10;
						}
						fixing_light_time--;
						if (fixing_light_time <= 0)
						{
							fixing_light = false;
							batteries = 100f;
							light_delay = 0;
							light_on = false;
							blink_time = 0;
						}
						if (fixing_index == 1 && fixing_light_time % 8 == 0)
						{
							sounds[6].Play();
						}
						if (fixing_light_time % light_between_time == 0 && (fixing_light_time >= fixing_light_time_max - 18 * light_between_time || fixing_light_time <= 11 * light_between_time))
						{
							fixing_index++;
							if (fixing_index == 10)
							{
								fixing_index = 11;
							}
							if (fixing_index == 15)
							{
								fixing_index = 16;
							}
							if (fixing_index == 18)
							{
								fixing_index = 19;
							}
						}
					}
					else
					{
						bool flag = false;
						foreach (Crazy crazy3 in crazies)
						{
							if (crazy3.room_name == rooms[look_room].name)
							{
								switch (crazy3.name)
								{
								case "wolf":
									current_room = rooms[look_room].texture[4];
									break;
								case "spider":
									current_room = rooms[look_room].texture[1];
									break;
								case "hockey":
									current_room = rooms[look_room].texture[2];
									break;
								case "axe":
									current_room = rooms[look_room].texture[3];
									break;
								}
								motion = true;
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							current_room = rooms[look_room].texture[0];
							if (look_location == "Front Door" && game_timer > game_begin_time)
							{
								current_room = rooms[look_room].texture[5];
							}
							motion = false;
						}
						string text2 = look_location;
						if (state.DPad.Down == ButtonState.Pressed && previous.DPad.Down == ButtonState.Released)
						{
							look_location = rooms[look_room].Camera_Move("Down");
						}
						else if (state.DPad.Up == ButtonState.Pressed && previous.DPad.Up == ButtonState.Released)
						{
							look_location = rooms[look_room].Camera_Move("Up");
						}
						else if (state.DPad.Left == ButtonState.Pressed && previous.DPad.Left == ButtonState.Released)
						{
							look_location = rooms[look_room].Camera_Move("Left");
						}
						else if (state.DPad.Right == ButtonState.Pressed && previous.DPad.Right == ButtonState.Released)
						{
							look_location = rooms[look_room].Camera_Move("Right");
						}
						if (look_location != text2)
						{
							if (tutorial < 4)
							{
								tutorial++;
							}
							camera_flash = camera_flash_max;
							look_room = Find_Room_Index();
							eye_coord = rooms[look_room].map_pos;
						}
						if (state.Buttons.RightShoulder == ButtonState.Pressed && previous.Buttons.RightShoulder == ButtonState.Released)
						{
							on_computer = false;
							to_hallway = true;
							hallway_index = 1;
							hallway_view = 3;
						}
						if (state.Buttons.X == ButtonState.Pressed && previous.Buttons.X == ButtonState.Released)
						{
							fixing_index = 0;
							fixing_light = true;
							fixing_light_time = fixing_light_time_max;
						}
						if (motion)
						{
							motion_time--;
							if (motion_time <= 0)
							{
								motion_time = max_motion_time;
								if (cam_text == Color.White)
								{
									cam_text = Color.Red;
								}
								else if (cam_text == Color.Red)
								{
									cam_text = Color.White;
								}
							}
						}
						else
						{
							motion_time = max_motion_time;
							cam_text = Color.White;
						}
					}
				}
				else
				{
					if (light_delay > 0)
					{
						light_delay--;
						if (batteries > 0f && light_delay == 0)
						{
							if (!light_on)
							{
								light_on = true;
							}
							else if (light_on)
							{
								light_on = false;
							}
						}
					}
					if (to_hallway)
					{
						if (hallway_view < 30)
						{
							hallway_view++;
						}
					}
					else if (hallway_view > 0)
					{
						hallway_view--;
					}
					switch (hallway_view)
					{
					case 0:
						hallway_index = 0;
						on_computer = true;
						break;
					case 5:
						hallway_index = 1;
						break;
					case 10:
						hallway_index = 2;
						break;
					case 15:
						hallway_index = 3;
						break;
					case 20:
						hallway_index = 4;
						break;
					case 25:
						hallway_index = 5;
						break;
					case 30:
						hallway_index = 6;
						break;
					}
					if (hallway_index == 6)
					{
						if (tutorial == 4)
						{
							tutorial = 5;
						}
						if (state.Buttons.LeftShoulder == ButtonState.Pressed && previous.Buttons.LeftShoulder == ButtonState.Released && light_delay == 0 && !light_on)
						{
							to_hallway = false;
							hallway_index = 5;
							hallway_view = 25;
							light_on = false;
						}
						if (state.Buttons.A == ButtonState.Pressed && previous.Buttons.A == ButtonState.Released && light_delay == 0)
						{
							if (light_on)
							{
								light_delay = light_delay_max;
								sounds[5].Play();
							}
							else if (!light_on)
							{
								light_delay = light_delay_max;
								sounds[5].Play();
							}
							if (batteries <= 0f)
							{
								light_on = false;
							}
						}
						if (light_on)
						{
							if (tutorial == 5)
							{
								tutorial = 6;
							}
							if (tutorial == 6 && batteries < 90f)
							{
								tutorial = 7;
							}
							batteries -= 0.2f;
							if (batteries <= 0f)
							{
								light_on = false;
							}
							else
							{
								if (batteries >= 25f && batteries < 50f)
								{
									blink_time_max = 50;
									blink_time--;
									if (blink_time <= 0)
									{
										blink_time = blink_time_max;
									}
								}
								if (batteries > 0f && batteries < 25f)
								{
									blink_time_max = 25;
									blink_time--;
									if (blink_time <= 0)
									{
										blink_time = blink_time_max;
									}
								}
							}
							string text3 = "";
							foreach (Crazy crazy4 in crazies)
							{
								if (crazy4.room_name == "Hallway")
								{
									text3 = crazy4.name;
									if (!crazy4.light)
									{
										sounds[2].Play();
										crazy4.special_timer = crazy4.special_timer_max;
										crazy4.light = true;
									}
								}
							}
							switch (text3)
							{
							case "":
								light_index = 0;
								break;
							case "spider":
								light_index = 1;
								break;
							case "hockey":
								light_index = 2;
								break;
							case "axe":
								light_index = 3;
								break;
							case "wolf":
								light_index = 4;
								break;
							}
						}
						else if (tutorial == 7 || tutorial == 6)
						{
							tutorial = 8;
						}
					}
				}
				previous = state;
			}
			if (!dead)
			{
				game_timer--;
				if (tutorial <= 9 && game_timer < 4300)
				{
					game_timer = 4300;
				}
			}
		}
		base.Update(gameTime);
	}

	public int Find_Room_Index()
	{
		for (int i = 0; i < rooms.Count(); i++)
		{
			if (rooms[i].name == look_location)
			{
				return i;
			}
		}
		return 0;
	}

	protected override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.Clear(Color.Black);
		spriteBatch.Begin();
		if (!thePlayer)
		{
			spriteBatch.Draw(title, new Rectangle(0, 0, 800, 450), Color.White);
			spriteBatch.DrawString(font, "Fright Light", new Vector2(100f, 100f), Color.Red);
			spriteBatch.DrawString(font, "Press A", new Vector2(100f, 150f), Color.Green);
		}
		else if (!loaded)
		{
			spriteBatch.DrawString(font, "Loading Scary Images...", new Vector2(150f, 200f), Color.Red);
			spriteBatch.DrawString(font, load_time + " % Complete", new Vector2(150f, 250f), Color.Red);
		}
		else if (dead && doomed == 0)
		{
			if (death_index_start > death_index_end)
			{
				spriteBatch.DrawString(font, "You Failed Hour: " + level, new Vector2(150f, 200f), Color.Red);
			}
			else if (death_type == "scare")
			{
				spriteBatch.Draw(Scare[death_index_start], new Rectangle(0, 0, 800, 450), Color.White);
			}
			else
			{
				spriteBatch.Draw(Kill[death_index_start], new Rectangle(0, 0, 800, 450), Color.White);
			}
		}
		else
		{
			if (game_timer <= 0)
			{
				if (level < 12)
				{
					spriteBatch.DrawString(font, "You Survived Hour: " + level, new Vector2(150f, 200f), Color.Red);
				}
				else if (level == 12)
				{
					spriteBatch.DrawString(font, "YOU WON!!!", new Vector2(150f, 200f), Color.Red);
				}
				else
				{
					spriteBatch.DrawString(font, "You Survived Hour: " + (level - 1), new Vector2(150f, 200f), Color.Red);
				}
			}
			else if (on_computer)
			{
				if (fixing_light)
				{
					if (fixing_light_time <= fixing_light_time_max - 18 * light_between_time && fixing_light_time >= 10 * light_between_time)
					{
						spriteBatch.Draw(Fixing[0], new Rectangle(0, 0, 800, 450), Color.White);
					}
					else if (fixing_index <= 21)
					{
						spriteBatch.Draw(Fixing[fixing_index], new Rectangle(0, 0, 800, 450), Color.White);
					}
					else
					{
						spriteBatch.Draw(Fixing_End[fixing_index - 22], new Rectangle(0, 0, 800, 450), Color.White);
					}
				}
				else
				{
					spriteBatch.Draw(laptop[0], new Rectangle(0, 0, 800, 450), Color.White);
					if (camera_flash == 0)
					{
						spriteBatch.Draw(current_room, new Rectangle(242, 85, 326, 183), Color.White);
					}
					else
					{
						spriteBatch.Draw(statics, new Rectangle(242, 85, 326, 183), Color.White);
					}
					spriteBatch.DrawString(font, look_location, new Vector2(325f, 250f), cam_text);
					spriteBatch.Draw(map, new Rectangle(256, 314, 320, 96), Color.White);
					spriteBatch.Draw(eye, new Rectangle(256 + (int)eye_coord.X - 5, 314 + (int)eye_coord.Y - 5, 10, 10), Color.White);
				}
			}
			else if (!light_on)
			{
				spriteBatch.Draw(laptop[hallway_index], new Rectangle(0, 0, 800, 450), Color.White);
			}
			else if (blink_time < blink_time_max - 4)
			{
				spriteBatch.Draw(light[light_index], new Rectangle(0, 0, 800, 450), Color.White);
			}
			else
			{
				spriteBatch.Draw(light[light_index], new Rectangle(0, 0, 800, 450), Color.LightGray);
			}
			switch (tutorial)
			{
			case 0:
				spriteBatch.Draw(buttons, new Rectangle(264, 114, 80, 80), new Rectangle(0, 0, 52, 54), Color.White);
				break;
			case 1:
				spriteBatch.Draw(buttons, new Rectangle(264, 114, 80, 80), new Rectangle(0, 0, 52, 54), Color.White);
				break;
			case 2:
				spriteBatch.Draw(buttons, new Rectangle(264, 114, 80, 80), new Rectangle(0, 0, 52, 54), Color.White);
				break;
			case 3:
				spriteBatch.Draw(buttons, new Rectangle(264, 114, 80, 80), new Rectangle(0, 0, 52, 54), Color.White);
				break;
			case 4:
				spriteBatch.Draw(buttons, new Rectangle(264, 114, 80, 80), new Rectangle(59, 0, 55, 21), Color.White);
				break;
			case 5:
				spriteBatch.Draw(buttons, new Rectangle(264, 114, 80, 80), new Rectangle(186, 0, 44, 40), Color.White);
				break;
			case 7:
				spriteBatch.Draw(buttons, new Rectangle(264, 114, 80, 80), new Rectangle(186, 0, 44, 40), Color.White);
				break;
			case 8:
				spriteBatch.Draw(buttons, new Rectangle(264, 114, 80, 80), new Rectangle(59, 25, 55, 20), Color.White);
				break;
			case 9:
				spriteBatch.Draw(buttons, new Rectangle(264, 114, 80, 80), new Rectangle(136, 0, 41, 39), Color.White);
				break;
			}
		}
		spriteBatch.End();
		base.Draw(gameTime);
	}
}
