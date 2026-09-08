using System;
using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Items;
using Loot.NPCs;
using Loot.TileSets;
using Loot.Widgets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;

namespace Loot.Maps;

public class Map
{
	public delegate bool Matcher(Location loc);

	private const int VERSION = 2;

	private const string saveFilePrefix = "level";

	private const string saveFilePostfix = ".map";

	public const string DeletePattern = "*.map";

	protected long gameId;

	protected TimeSpan age = TimeSpan.Zero;

	protected int depth;

	protected TileSet tileSet;

	protected int rows;

	protected int cols;

	protected MapCell[,] cells;

	protected Location exitUp;

	protected Location exitDown;

	protected int openFloorCount;

	private static readonly TileType?[,] northWallPattern = new TileType?[3, 3]
	{
		{
			TileType.Wall,
			TileType.Wall,
			TileType.Wall
		},
		{
			TileType.Wall,
			null,
			TileType.Wall
		},
		{
			null,
			TileType.Floor,
			null
		}
	};

	public long GameId => gameId;

	public TimeSpan Age => age;

	public int Rows => rows;

	public int Cols => cols;

	public Location ExitUp => exitUp;

	public Location ExitDown => exitDown;

	public int OpenFloorCount => openFloorCount;

	public string SaveFileName => "level" + depth + ".map";

	private Map()
	{
	}

	protected Map(int depth, TileSet tileSet, int rows, int cols)
	{
		this.depth = depth;
		this.tileSet = tileSet;
		this.rows = rows;
		this.cols = cols;
		Generate();
	}

	public Map(StorageContainer container, int depth)
	{
		string file = "level" + depth + ".map";
		try
		{
			using Stream input = container.OpenFile(file, FileMode.Open, FileAccess.Read);
			using BinaryReader binaryReader = new BinaryReader(input);
			Read(binaryReader);
			binaryReader.Close();
		}
		catch (Exception)
		{
			throw new ResetIOException();
		}
	}

	public Tile GetTile(TileId id)
	{
		return tileSet.GetTile(id);
	}

	protected virtual void Generate()
	{
		cells = new MapCell[rows, cols];
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				cells[i, j].Tile = GetTile(TileId.Wall);
			}
		}
	}

	public void Finish()
	{
		openFloorCount = 0;
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				if (IsOpenFloor(new Location(i, j)))
				{
					openFloorCount++;
				}
			}
		}
	}

	public void Update(GameTime gameTime)
	{
		age += gameTime.ElapsedGameTime;
		tileSet.Update(gameTime);
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				cells[i, j].Update(gameTime);
			}
		}
	}

	public void DrawCell(SpriteBatch spriteBatch, Location loc, Vector2 pos, Color color, float scale)
	{
		MapCell mapCell = cells[loc.Row, loc.Col];
		mapCell.Tile.Draw(spriteBatch, pos, color, scale);
		tileSet.DrawTileEffect(spriteBatch, mapCell.Tile, loc, pos, color, scale);
		if (mapCell.Ornament != null)
		{
			mapCell.Ornament.Draw(spriteBatch, pos, color, scale);
		}
		if (mapCell.Widget != null)
		{
			mapCell.Widget.Draw(spriteBatch, pos, color, scale);
		}
		if (mapCell.Item != null)
		{
			mapCell.Item.DrawWithShadow(spriteBatch, pos, color, scale);
		}
	}

	public bool IsValid(Location loc)
	{
		return loc.Row >= 0 && loc.Row < rows && loc.Col >= 0 && loc.Col < cols;
	}

	public bool IsDiscovered(Location loc)
	{
		return IsValid(loc) && cells[loc.Row, loc.Col].Discovered;
	}

	public bool IsWall(Location loc)
	{
		return cells[loc.Row, loc.Col].Tile.IsWall();
	}

	public bool IsDoor(Location loc)
	{
		return cells[loc.Row, loc.Col].Tile.IsDoor();
	}

	public bool IsFloor(Location loc)
	{
		return IsValid(loc) && cells[loc.Row, loc.Col].Tile.IsFloor();
	}

	public bool IsOpenDoor(Location loc)
	{
		return cells[loc.Row, loc.Col].Tile.IsOpenDoor();
	}

	public bool IsOpenFloor(Location loc)
	{
		if (loc == DM.Player.Location)
		{
			return false;
		}
		MapCell mapCell = cells[loc.Row, loc.Col];
		return mapCell.Tile.IsFloor() && mapCell.Item == null && mapCell.NPC == null && mapCell.Widget == null;
	}

	public bool IsItem(Location loc)
	{
		return GetItem(loc) != null;
	}

	public bool IsNPC(Location loc)
	{
		return GetNPC(loc) != null;
	}

	public bool IsEnemy(Location loc)
	{
		NPC nPC = GetNPC(loc);
		return nPC != null && !nPC.IsAlly;
	}

	public bool CanAddNPC(Location loc)
	{
		if (loc == DM.Player.Location)
		{
			return false;
		}
		MapCell mapCell = cells[loc.Row, loc.Col];
		return mapCell.Tile.IsFloor() && mapCell.NPC == null;
	}

	public void Discover(Location loc)
	{
		cells[loc.Row, loc.Col].Discovered = true;
	}

	public Tile GetTile(Location loc)
	{
		return cells[loc.Row, loc.Col].Tile;
	}

	public void SetTile(Location loc, Tile tile)
	{
		cells[loc.Row, loc.Col].Tile = tile;
	}

	public Ornament GetOrnament(Location loc)
	{
		return cells[loc.Row, loc.Col].Ornament;
	}

	public void SetOrnament(Location loc, Ornament ornament)
	{
		cells[loc.Row, loc.Col].Ornament = ornament;
	}

	public Item GetItem(Location loc)
	{
		return cells[loc.Row, loc.Col].Item;
	}

	public void SetItem(Location loc, Item item)
	{
		cells[loc.Row, loc.Col].Item = item;
	}

	public Widget GetWidget(Location loc)
	{
		return cells[loc.Row, loc.Col].Widget;
	}

	public void SetWidget(Widget widget)
	{
		Location location = widget.Location;
		cells[location.Row, location.Col].Widget = widget;
	}

	public void ClearWidget(Location loc)
	{
		cells[loc.Row, loc.Col].Widget = null;
	}

	public NPC GetNPC(Location loc)
	{
		return cells[loc.Row, loc.Col].NPC;
	}

	public void SetNPC(Location loc, NPC npc)
	{
		cells[loc.Row, loc.Col].NPC = npc;
	}

	public void SetOpenDoor(Location loc)
	{
		if (cells[loc.Row + 1, loc.Col].Tile.IsFloor())
		{
			SetTile(loc, GetTile(TileId.OpenDoorNS));
		}
		else
		{
			SetTile(loc, GetTile(TileId.OpenDoorEW));
		}
	}

	public void SetExitUp(Location loc)
	{
		exitUp = loc;
		SetWidget(new WidgetExitUp(loc));
	}

	public void SetExitDown(Location loc)
	{
		exitDown = loc;
		SetWidget(new WidgetExitDown(loc));
	}

	public void SetExitOut(Location loc)
	{
		exitDown = loc;
		SetWidget(new WidgetExitOut(loc));
	}

	protected void randomizeFloor()
	{
		for (int i = 1; i < rows - 1; i++)
		{
			for (int j = 1; j < cols - 1; j++)
			{
				Location loc = new Location(i, j);
				if (IsFloor(loc) && DM.Random.Next(15) == 0)
				{
					SetTile(loc, tileSet.RandomFloor());
				}
			}
		}
	}

	protected void fixBorders()
	{
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				Location loc = new Location(i, j);
				if (IsWall(loc))
				{
					SetTile(loc, getBorder(loc));
				}
			}
		}
	}

	protected Tile getBorder(Location loc)
	{
		return (!IsFloor(loc.N)) ? ((!IsFloor(loc.S)) ? ((!IsFloor(loc.E)) ? (IsFloor(loc.W) ? GetTile(TileId.BorderW) : GetTile(TileId.Wall)) : (IsFloor(loc.W) ? GetTile(TileId.BorderEW) : GetTile(TileId.BorderE))) : ((!IsFloor(loc.E)) ? (IsFloor(loc.W) ? GetTile(TileId.BorderSW) : GetTile(TileId.BorderS)) : (IsFloor(loc.W) ? GetTile(TileId.BorderSEW) : GetTile(TileId.BorderSE)))) : ((!IsFloor(loc.S)) ? ((!IsFloor(loc.E)) ? (IsFloor(loc.W) ? GetTile(TileId.BorderNW) : GetTile(TileId.BorderN)) : (IsFloor(loc.W) ? GetTile(TileId.BorderNEW) : GetTile(TileId.BorderNE))) : ((!IsFloor(loc.E)) ? (IsFloor(loc.W) ? GetTile(TileId.BorderNSW) : GetTile(TileId.BorderNS)) : (IsFloor(loc.W) ? GetTile(TileId.BorderNSEW) : GetTile(TileId.BorderNSE))));
	}

	protected void AddExits()
	{
		if (depth > 1)
		{
			SetExitUp(FindRandom(IsOpenFloor));
		}
		if (depth < 50)
		{
			SetExitDown(FindRandom(IsOpenFloor));
			return;
		}
		Location location = FindRandom(MatchNorthWall);
		SetTile(location, GetTile(TileId.Floor));
		SetExitOut(location);
	}

	public Location FindRandom(Matcher match)
	{
		return FindNearest(new Location(DM.Random.Next(rows), DM.Random.Next(cols)), match);
	}

	public Location FindNearest(Location src, Matcher match)
	{
		Location result = Location.Zero;
		float num = float.MaxValue;
		for (int i = 1; i < rows - 1; i++)
		{
			for (int j = 1; j < cols - 1; j++)
			{
				Location location = new Location(i, j);
				if (match(location))
				{
					float num2 = src.Distance(location);
					if ((double)num2 < (double)num)
					{
						num = num2;
						result = location;
					}
				}
			}
		}
		return result;
	}

	public Location FindFarthest(Location src, Matcher match)
	{
		Location result = Location.Zero;
		float num = 0f;
		for (int i = 1; i < rows - 1; i++)
		{
			for (int j = 1; j < cols - 1; j++)
			{
				Location location = new Location(i, j);
				if (match(location))
				{
					float num2 = src.Distance(location);
					if ((double)num2 > (double)num)
					{
						num = num2;
						result = location;
					}
				}
			}
		}
		return result;
	}

	public bool MatchNorthWall(Location loc)
	{
		return MatchPattern(loc, northWallPattern);
	}

	private bool MatchPattern(Location loc, TileType?[,] pattern)
	{
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				TileType? tileType = pattern[i, j];
				if (tileType.HasValue)
				{
					TileType type = cells[loc.Row + i - 1, loc.Col + j - 1].Tile.Type;
					if (tileType.Value != type)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	public static bool SaveFileExists(StorageContainer container, int depth)
	{
		string file = "level" + depth + ".map";
		return container.FileExists(file);
	}

	public static void Delete(StorageContainer container, int depth)
	{
		string file = "level" + depth + ".map";
		if (container.FileExists(file))
		{
			container.DeleteFile(file);
		}
	}

	private void Read(BinaryReader reader)
	{
		switch (reader.ReadInt32())
		{
		case 1:
			gameId = 0L;
			age = TimeSpan.Zero;
			break;
		case 2:
			gameId = reader.ReadInt64();
			age = TimeSpan.FromSeconds((double)reader.ReadInt32());
			break;
		default:
			throw new ResetIOException();
		}
		depth = reader.ReadInt32();
		openFloorCount = reader.ReadInt32();
		tileSet = TileSet.GetTileSet((TileSetType)reader.ReadInt32());
		rows = reader.ReadInt32();
		cols = reader.ReadInt32();
		cells = new MapCell[rows, cols];
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				cells[i, j].Read(reader, tileSet);
			}
		}
		exitUp.Read(reader);
		exitDown.Read(reader);
	}

	public void Write(BinaryWriter writer)
	{
		writer.Write(2);
		writer.Write(DM.GameOptions.GameId);
		writer.Write((int)age.TotalSeconds);
		writer.Write(depth);
		writer.Write(openFloorCount);
		writer.Write((int)tileSet.Type);
		writer.Write(rows);
		writer.Write(cols);
		for (int i = 0; i < rows; i++)
		{
			for (int j = 0; j < cols; j++)
			{
				cells[i, j].Write(writer);
			}
		}
		exitUp.Write(writer);
		exitDown.Write(writer);
	}
}
