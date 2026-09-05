using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LightsOnCrazies;

internal class Room
{
	public struct Connections
	{
		public string direction;

		public string destination;
	}

	public string name;

	public Vector2 map_pos;

	public Texture2D[] texture = new Texture2D[5];

	public List<Connections> connections = new List<Connections>();

	public List<Connections> cam_only = new List<Connections>();

	public Room(string my_name, Texture2D[] new_textures, Vector2 new_map_position)
	{
		name = my_name;
		texture = new_textures;
		map_pos = new_map_position;
	}

	public void create_connection(string new_direction, string new_destination)
	{
		Connections item = new Connections
		{
			direction = new_direction,
			destination = new_destination
		};
		connections.Add(item);
	}

	public void create_cam_connection(string new_direction, string new_destination)
	{
		Connections item = new Connections
		{
			direction = new_direction,
			destination = new_destination
		};
		cam_only.Add(item);
	}

	public string Move_Decide()
	{
		if (name == "Hallway")
		{
			return "You";
		}
		Random random = new Random();
		int index = random.Next(0, connections.Count);
		return connections[index].destination;
	}

	public string Camera_Move(string input_dir)
	{
		string destination = name;
		foreach (Connections item in cam_only)
		{
			if (item.direction == input_dir)
			{
				destination = item.destination;
				break;
			}
		}
		if (destination == name)
		{
			foreach (Connections connection in connections)
			{
				if (connection.direction == input_dir && connection.destination != "Hallway")
				{
					destination = connection.destination;
					break;
				}
			}
		}
		return destination;
	}
}
