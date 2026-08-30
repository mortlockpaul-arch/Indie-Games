using System.Collections.Generic;

namespace Defenders;

public class Room
{
	public int id { get; set; }

	public int width { get; set; }

	public int height { get; set; }

	public int locationx { get; set; }

	public int locationy { get; set; }

	public List<Item> items { get; set; }
}
