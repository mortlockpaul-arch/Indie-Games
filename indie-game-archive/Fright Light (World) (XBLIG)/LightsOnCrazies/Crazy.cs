namespace LightsOnCrazies;

internal class Crazy
{
	public string name;

	public int max_movement_time;

	public int movement_time;

	public bool light;

	public int special_timer;

	public int special_timer_max;

	public int difficulty;

	public string room_name;

	public Crazy(string new_name, int new_difficulty)
	{
		special_timer_max = 12;
		light = false;
		name = new_name;
		difficulty = new_difficulty;
		max_movement_time = 600 - difficulty * 10;
	}
}
