using Microsoft.Xna.Framework;

namespace BureauNewPDA;

public class PDATextListData
{
	public enum type
	{
		BasicResearch,
		VideoSolve,
		NA
	}

	public int id = -1;

	public int orderId = -1;

	public string columnA = "";

	public string colummB = "";

	public string columnC = "";

	public int columAX;

	public int columBX;

	public int columCX;

	public string displayBoxText = "";

	public bool isAvailable;

	public bool isComplete;

	public type DataType;

	public ResearchControlData.ResearchData.DisplayState displayState;

	public Color myColor = Color.White;

	public void addData2Column(int id, int order, string a, string b, int aX, int bX, string displayData, type DataType, bool isAvailable, bool isComplete)
	{
		this.id = id;
		orderId = order;
		columnA = a;
		colummB = b;
		columAX = aX;
		columBX = bX;
		this.isAvailable = isAvailable;
		this.isComplete = isComplete;
		displayBoxText = displayData;
	}
}
