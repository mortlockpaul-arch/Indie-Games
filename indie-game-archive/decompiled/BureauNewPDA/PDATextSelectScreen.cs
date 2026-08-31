using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BureauNewPDA;

public class PDATextSelectScreen
{
	public class headerTextData
	{
		public string headerText = "";

		public int positionX;
	}

	public bool isOn;

	public List<headerTextData> headerTextList = new List<headerTextData>();

	public CoreDisplayElements myCoreDisplayElements = new CoreDisplayElements();

	private headerTextData h = new headerTextData();

	private List<DisplayText> displayText = new List<DisplayText>();

	private List<DisplayData> displayList = new List<DisplayData>();

	public List<PDATextListData> tableDataList = new List<PDATextListData>();

	public List<PDATextListData> backupTableDataList = new List<PDATextListData>();

	private DisplayText t = new DisplayText();

	private DisplayData d = new DisplayData();

	private int columnAx = 50;

	private int columnBx = 600;

	private int columnCx;

	public int selectedPosition = 1;

	private int startOrderId = 1;

	private int endOrderId = 4;

	private int maxPositionDisplayed = 1;

	private float yOffset;

	private float xOffset;

	private float lastXOffset;

	private float lastYOffset;

	private float deltaX;

	private float deltaY;

	private string currentTimeDisplay = "";

	private bool isArrowDownOn;

	private float arrowDownScale = 0.8f;

	private int arrowScaleCounter = 3;

	private bool isArrowUpOn;

	private float arrowUpScale = 0.8f;

	private int arrowScaleUpCounter = 3;

	private Color myColorGlobal = Color.White;

	private Color myColorGlobalBlack = Color.Black;

	private double joyProgressA;

	private TimeSpan joyTimeSpan = TimeSpan.FromMilliseconds(160.0);

	public bool isActive;

	public PDATextListData currentCaseDataItem = new PDATextListData();

	public ResearchControlData.ResearchData currentResearchData = new ResearchControlData.ResearchData();

	public DisplayText textBoxSummary = new DisplayText();

	public bool isReturnDisplay;

	private bool finished = true;

	private bool found;

	private int count;

	private bool foundFirst;

	private bool findSecond;

	private bool foundSecond;

	private Vector2 tempPosition = Vector2.Zero;

	private int yAdjuster;

	public void reset()
	{
		displayText.Clear();
		displayList.Clear();
	}

	public void updateBasePosition(float x, float y)
	{
		lastXOffset = xOffset;
		lastYOffset = yOffset;
		yOffset = y;
		xOffset = x;
		deltaX = xOffset - lastXOffset;
		deltaY = yOffset - lastYOffset;
	}

	public void updateLineItemColors(Color myColor, bool noRemoved)
	{
		foreach (PDATextListData tableData in tableDataList)
		{
			if (noRemoved)
			{
				if (tableData.displayState != ResearchControlData.ResearchData.DisplayState.Removed)
				{
					tableData.myColor = myColor;
				}
			}
			else
			{
				tableData.myColor = myColor;
			}
		}
	}

	public void updateColors(Color baseColor)
	{
		myColorGlobal = baseColor;
		myColorGlobalBlack = new Color(0, 0, 0, baseColor.A);
	}

	public void reset(int currentTime)
	{
		displayText.Clear();
		displayList.Clear();
		selectedPosition = 1;
		currentTimeDisplay = "Current Time " + formattTime(currentTime);
		startOrderId = 1;
		isOn = false;
	}

	private string formattTime(int time)
	{
		string text = time.ToString();
		if (text.Length == 2)
		{
			return "12:" + text.Substring(0, 2) + " AM";
		}
		if (text.Length == 3)
		{
			return text.Substring(0, 1) + ":" + text.Substring(1, 2) + " AM";
		}
		if (time > 1159)
		{
			if (time > 1259)
			{
				int num = time - 1200;
				if (num.ToString().Length == 3)
				{
					return num.ToString().Substring(0, 1) + ":" + text.Substring(2, 2) + " PM";
				}
				return num.ToString().Substring(0, 2) + ":" + text.Substring(2, 2) + " PM";
			}
			return text.Substring(0, 2) + ":" + text.Substring(2, 2) + " PM";
		}
		return text.Substring(0, 2) + ":" + text.Substring(2, 2) + " AM";
	}

	public void addResultsButton()
	{
		DisplayText displayText = new DisplayText();
		displayText.addTextRaw(DisplayText.GroupTextType.TextBoxSpecial, "Continue:", Color.White, new Vector2(1000f, 600f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontRegular, 700);
		this.displayText.Add(displayText);
		addPressAButton();
	}

	public void updateArrows()
	{
		if (isArrowDownOn)
		{
			arrowScaleCounter++;
			if ((arrowDownScale == 1f) & (arrowScaleCounter > 20))
			{
				arrowDownScale = 0.5f;
			}
			else if ((arrowScaleCounter > 2) & (arrowDownScale != 1f))
			{
				if (arrowDownScale < 1.05f)
				{
					arrowDownScale += 0.04f;
				}
				else
				{
					arrowDownScale = 1f;
				}
				arrowScaleCounter = 0;
			}
			addLowerArrow(380);
		}
		else
		{
			if (!isArrowUpOn)
			{
				return;
			}
			arrowScaleUpCounter++;
			if ((arrowUpScale == 1f) & (arrowScaleUpCounter > 20))
			{
				arrowUpScale = 0.5f;
			}
			else if ((arrowScaleUpCounter > 2) & (arrowUpScale != 1f))
			{
				if (arrowUpScale < 1.05f)
				{
					arrowUpScale += 0.04f;
				}
				else
				{
					arrowUpScale = 1f;
				}
				arrowScaleUpCounter = 0;
			}
			addUpperArrow(135);
		}
	}

	public void updateSelect(int y, GameTime gameTime)
	{
		joyProgressA += gameTime.ElapsedGameTime.TotalMilliseconds / joyTimeSpan.TotalMilliseconds;
		if (!(joyProgressA > 1.0))
		{
			return;
		}
		int num = selectedPosition;
		if ((y > 0) & (selectedPosition < tableDataList.Count()))
		{
			selectedPosition++;
		}
		else if ((y < 0) & (selectedPosition != 1))
		{
			selectedPosition--;
		}
		if (num != selectedPosition)
		{
			joyProgressA = 0.0;
			if (selectedPosition > endOrderId)
			{
				updateTextList();
				return;
			}
			if (selectedPosition < startOrderId)
			{
				updateTextList();
				return;
			}
			updateTableSelectionChanged();
			if (endOrderId < tableDataList.Count())
			{
				isArrowDownOn = true;
			}
			if (startOrderId != 1)
			{
				isArrowUpOn = true;
			}
		}
		else
		{
			joyProgressA = 1.0;
		}
	}

	public bool fadeInAllGraphicsText()
	{
		finished = true;
		foreach (DisplayData display in displayList)
		{
			if (display.isDisplayed & (display.objectType != DisplayData.ObjectTypeEnum.TextBox))
			{
				if (display.myColor.R <= 240)
				{
					display.myColor = new Color(display.myColor.A + 5, display.myColor.G + 5, display.myColor.B + 5, display.myColor.A + 5);
					finished = false;
				}
				else
				{
					display.myColor = new Color(255, 255, 255, 255);
				}
			}
		}
		foreach (DisplayText item in displayText)
		{
			if (item.color.R <= 240)
			{
				item.color = new Color(item.color.R + 5, item.color.G + 5, item.color.B + 5, item.color.A + 5);
				finished = false;
			}
			else
			{
				item.color = new Color(255, 255, 255, 255);
			}
		}
		if (myColorGlobal.R <= 240)
		{
			myColorGlobal = new Color(myColorGlobal.R + 5, myColorGlobal.G + 5, myColorGlobal.B + 5, myColorGlobal.A + 5);
			finished = false;
		}
		else
		{
			myColorGlobal = new Color(255, 255, 255, 255);
		}
		return finished;
	}

	public bool fadeOutAllGraphicsText()
	{
		finished = true;
		foreach (DisplayData display in displayList)
		{
			if (display.isDisplayed)
			{
				if (display.myColor.R >= 20)
				{
					display.myColor = new Color(display.myColor.A - 5, display.myColor.G - 5, display.myColor.B - 5, display.myColor.A - 5);
					finished = false;
				}
				else
				{
					display.myColor = new Color(0, 0, 0, 0);
				}
			}
		}
		foreach (DisplayText item in displayText)
		{
			if (item.color.R >= 20)
			{
				item.color = new Color(item.color.R - 5, item.color.G - 5, item.color.B - 5, item.color.A - 5);
				finished = false;
			}
			else
			{
				item.color = new Color(0, 0, 0, 0);
			}
		}
		foreach (PDATextListData tableData in tableDataList)
		{
			if (tableData.myColor.R >= 20)
			{
				tableData.myColor = new Color(tableData.myColor.R - 5, tableData.myColor.G - 5, tableData.myColor.B - 5, tableData.myColor.A - 5);
				finished = false;
			}
			else
			{
				tableData.myColor = new Color(0, 0, 0, 0);
			}
		}
		if (myColorGlobal.R >= 20)
		{
			myColorGlobal = new Color(myColorGlobal.R - 5, myColorGlobal.G - 5, myColorGlobal.B - 5, myColorGlobal.A - 5);
			finished = false;
		}
		else
		{
			myColorGlobal = new Color(0, 0, 0, 0);
		}
		return finished;
	}

	private void updateTableSelectionChanged()
	{
		foreach (DisplayData display in displayList)
		{
			if (display.baseImageName == "TextBoxHighlight")
			{
				display.baseImageName = "TextBox";
			}
			else if (display.baseImageName == "BoxCompletedHighLight")
			{
				display.baseImageName = "BoxCompleted";
			}
			else if (display.baseImageName == "BoxUnavailableHighlight")
			{
				display.baseImageName = "BoxUnavailable";
			}
			else if ((display.baseImageName == "TextBox") & (display.objectId == selectedPosition))
			{
				display.baseImageName = "TextBoxHighlight";
			}
			else if ((display.baseImageName == "BoxCompleted") & (display.objectId == selectedPosition))
			{
				display.baseImageName = "BoxCompletedHighLight";
			}
			else if ((display.baseImageName == "BoxUnavailable") & (display.objectId == selectedPosition))
			{
				display.baseImageName = "BoxUnavailableHighlight";
			}
		}
		foreach (PDATextListData tableData in tableDataList)
		{
			if (tableData.orderId == selectedPosition)
			{
				updateTextBox(tableData, 260, 300, isReturnResults: false);
			}
		}
	}

	public int selectedChoice()
	{
		getCurrentCase();
		return currentCaseDataItem.id;
	}

	private void getCurrentCase()
	{
		foreach (PDATextListData tableData in tableDataList)
		{
			if (tableData.orderId == selectedPosition)
			{
				currentCaseDataItem = tableData;
				break;
			}
		}
	}

	public void getCurrentItem(int id)
	{
		foreach (PDATextListData tableData in tableDataList)
		{
			if (tableData.id == id)
			{
				currentCaseDataItem = tableData;
				break;
			}
		}
	}

	public void addHeaderText(string text, int xPosition, bool startOpacity)
	{
		isOn = true;
		int num = 260;
		h = new headerTextData();
		h.headerText = text;
		h.positionX = xPosition;
		headerTextList.Add(h);
		t = new DisplayText();
		Color color = Color.White;
		if (startOpacity)
		{
			color = new Color(0, 0, 0, 0);
		}
		t.addTextRaw(DisplayText.GroupTextType.Header, text, color, new Vector2((float)(xPosition + num + 15) + xOffset, 90f + yOffset), isTypedAnimated: false, myCoreDisplayElements.myPDAFontHeader, 200);
		displayText.Add(t);
	}

	public void addTime()
	{
		t = new DisplayText();
		Color white = Color.White;
		t.addTextRaw(DisplayText.GroupTextType.Header, currentTimeDisplay, white, new Vector2(540f, 600f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontHeader, 400);
		displayText.Add(t);
	}

	public void updateTextBoxDirect(ResearchControlData.ResearchData r, int x, int y, bool isReturnResults)
	{
		found = true;
		while (found)
		{
			found = false;
			foreach (DisplayText item in displayText)
			{
				if (item.groupType == DisplayText.GroupTextType.TextBoxSpecial)
				{
					displayText.Remove(item);
					removeTextBox();
					found = true;
					break;
				}
			}
		}
		if (y != 200)
		{
			if (endOrderId == 4)
			{
				y = 300;
			}
			else if (endOrderId == 5)
			{
				y = 340;
			}
			else if (endOrderId == 6)
			{
				y = 380;
			}
			else if (endOrderId >= 7)
			{
				y = 420;
			}
		}
		textBoxSummary = new DisplayText();
		textBoxSummary.addTextRaw(DisplayText.GroupTextType.Regular, r.completedheaderTxt, Color.White, new Vector2(270f, 125f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontRegular, 800);
		displayText.Add(textBoxSummary);
		textBoxSummary = new DisplayText();
		textBoxSummary.addTextRaw(DisplayText.GroupTextType.TextBoxSpecial, r.completedBodyTxt, Color.White, new Vector2((float)(x + 20) + xOffset, (float)y + yOffset), isReturnResults, myCoreDisplayElements.myPDAFontRegular, 800);
		displayText.Add(textBoxSummary);
		addTextBox(y, x, textBoxSummary.lineCount(r.completedBodyTxt, myCoreDisplayElements.myPDAFontRegular, 700), isActive: false);
	}

	public void updateTextBox(PDATextListData l, int x, int y, bool isReturnResults)
	{
		found = true;
		while (found)
		{
			found = false;
			foreach (DisplayText item in displayText)
			{
				if (item.groupType == DisplayText.GroupTextType.TextBoxSpecial)
				{
					displayText.Remove(item);
					removeTextBox();
					found = true;
					break;
				}
			}
		}
		if (y != 200)
		{
			if (endOrderId == 4)
			{
				y = 300;
			}
			else if (endOrderId == 5)
			{
				y = 340;
			}
			else if (endOrderId == 6)
			{
				y = 380;
			}
			else if (endOrderId >= 7)
			{
				y = 420;
			}
		}
		textBoxSummary = new DisplayText();
		textBoxSummary.addTextRaw(DisplayText.GroupTextType.TextBoxSpecial, l.displayBoxText, Color.White, new Vector2((float)(l.columAX + x + 20) + xOffset, (float)y + yOffset), isReturnResults, myCoreDisplayElements.myPDAFontRegular, 800);
		displayText.Add(textBoxSummary);
		addTextBox(y, x, textBoxSummary.lineCount(l.displayBoxText, myCoreDisplayElements.myPDAFontRegular, 700), l.isAvailable);
	}

	private void removeTextBox()
	{
		foreach (DisplayData display in displayList)
		{
			if (display.baseImageName == "BigBoxTop")
			{
				displayList.Remove(display);
				break;
			}
		}
		foreach (DisplayData display2 in displayList)
		{
			if (display2.baseImageName == "BigBoxMiddle")
			{
				displayList.Remove(display2);
				break;
			}
		}
		foreach (DisplayData display3 in displayList)
		{
			if (display3.baseImageName == "BigBoxMiddle")
			{
				displayList.Remove(display3);
				break;
			}
		}
		foreach (DisplayData display4 in displayList)
		{
			if (display4.baseImageName == "BigBoxMiddle")
			{
				displayList.Remove(display4);
				break;
			}
		}
		foreach (DisplayData display5 in displayList)
		{
			if (display5.baseImageName == "BigBoxMiddle")
			{
				displayList.Remove(display5);
				break;
			}
		}
		foreach (DisplayData display6 in displayList)
		{
			if (display6.baseImageName == "BigBoxMiddle")
			{
				displayList.Remove(display6);
				break;
			}
		}
		foreach (DisplayData display7 in displayList)
		{
			if (display7.baseImageName == "BigBoxMiddle")
			{
				displayList.Remove(display7);
				break;
			}
		}
		foreach (DisplayData display8 in displayList)
		{
			if (display8.baseImageName == "BigBoxMiddle")
			{
				displayList.Remove(display8);
				break;
			}
		}
		foreach (DisplayData display9 in displayList)
		{
			if (display9.baseImageName == "BigBoxMiddle")
			{
				displayList.Remove(display9);
				break;
			}
		}
		foreach (DisplayData display10 in displayList)
		{
			if (display10.baseImageName == "BigBoxBottom")
			{
				displayList.Remove(display10);
				break;
			}
		}
		foreach (DisplayData display11 in displayList)
		{
			if (display11.baseImageName == "ButtonA")
			{
				displayList.Remove(display11);
				break;
			}
		}
	}

	public bool checkNextPhaseForUpdatingStatus(bool isAvailable, bool isComplete)
	{
		foreach (PDATextListData tableData in tableDataList)
		{
			if ((tableData.isAvailable == isAvailable) & (tableData.isComplete == isComplete))
			{
				return true;
			}
		}
		return false;
	}

	public bool checkNextPhaseForUpdating(ResearchControlData.ResearchData.DisplayState state)
	{
		foreach (PDATextListData tableData in tableDataList)
		{
			if (tableData.displayState == state)
			{
				return true;
			}
		}
		return false;
	}

	public void updatePhaseState(ResearchControlData.ResearchData.DisplayState state, ResearchControlData.ResearchData.DisplayState newState)
	{
		foreach (PDATextListData tableData in tableDataList)
		{
			if (tableData.displayState == state)
			{
				tableData.displayState = newState;
			}
		}
	}

	public bool updateTextBoxColor(ResearchControlData.ResearchData.DisplayState state)
	{
		count = 1;
		found = true;
		foundFirst = false;
		findSecond = false;
		foundSecond = false;
		while (found)
		{
			found = false;
			foreach (PDATextListData tableData in tableDataList)
			{
				if (tableData.orderId != count)
				{
					continue;
				}
				found = true;
				count++;
				if (((tableData.displayState == state) & (tableData.myColor.R != byte.MaxValue)) && state != ResearchControlData.ResearchData.DisplayState.Remove && state != ResearchControlData.ResearchData.DisplayState.Removed)
				{
					if (!foundFirst)
					{
						foundFirst = true;
						tableData.myColor.A += 15;
						tableData.myColor.R += 15;
						tableData.myColor.G += 15;
						tableData.myColor.B += 15;
						if (tableData.myColor.R > 120)
						{
							findSecond = true;
						}
						updateTextBoxColorAtLocation(tableData.myColor, tableData.orderId);
					}
					else if (foundFirst & findSecond & !foundSecond)
					{
						foundSecond = true;
						tableData.myColor.A += 15;
						tableData.myColor.R += 15;
						tableData.myColor.G += 15;
						tableData.myColor.B += 15;
						if (tableData.myColor.R > 120)
						{
							foundSecond = true;
						}
						updateTextBoxColorAtLocation(tableData.myColor, tableData.orderId);
					}
				}
				else
				{
					if (!(((tableData.displayState == state) & (tableData.myColor.R > 0)) && state == ResearchControlData.ResearchData.DisplayState.Remove))
					{
						continue;
					}
					if (!foundFirst)
					{
						foundFirst = true;
						tableData.myColor.A -= 15;
						tableData.myColor.R -= 15;
						tableData.myColor.G -= 15;
						tableData.myColor.B -= 15;
						if (tableData.myColor.R < 120)
						{
							findSecond = true;
						}
						updateTextBoxColorAtLocation(tableData.myColor, tableData.orderId);
					}
					else if (foundFirst & findSecond & !foundSecond)
					{
						foundSecond = true;
						tableData.myColor.A -= 15;
						tableData.myColor.R -= 15;
						tableData.myColor.G -= 15;
						tableData.myColor.B -= 15;
						if (tableData.myColor.R < 120)
						{
							foundSecond = true;
						}
						updateTextBoxColorAtLocation(tableData.myColor, tableData.orderId);
					}
				}
			}
		}
		if (!foundFirst)
		{
			return true;
		}
		return false;
	}

	private void updateTextBoxColorAtLocation(Color myColor, int orderId)
	{
		foreach (DisplayData display in displayList)
		{
			if ((display.objectType == DisplayData.ObjectTypeEnum.TextBox) & (display.objectId == orderId))
			{
				display.myColor = myColor;
				break;
			}
		}
	}

	public void updateTextListOnReturn()
	{
		int y = 120;
		int num = 260;
		addLineItemGraphic(y, 2, num, 50, 0, 1, 1, isAvailable: true, isComplete: true, Color.White);
		updateTextBoxDirect(currentResearchData, num, 200, isReturnResults: true);
	}

	public void updateTextList()
	{
		int num = 120;
		int num2 = 260;
		if (selectedPosition > endOrderId)
		{
			startOrderId = selectedPosition - 6;
			endOrderId = selectedPosition;
			resetDisplay();
		}
		else if (selectedPosition < startOrderId)
		{
			startOrderId = selectedPosition;
			endOrderId = 6 + selectedPosition;
			resetDisplay();
		}
		else if (tableDataList.Count() >= 7)
		{
			endOrderId = 7;
		}
		else
		{
			endOrderId = tableDataList.Count();
		}
		found = false;
		if (tableDataList.Count > endOrderId)
		{
			isArrowDownOn = true;
		}
		else
		{
			isArrowDownOn = false;
			turnOffDisplayById(117);
		}
		if (startOrderId != 1)
		{
			isArrowUpOn = true;
		}
		else
		{
			isArrowUpOn = false;
			turnOffDisplayById(118);
		}
		foreach (PDATextListData tableData in tableDataList)
		{
			if ((tableData.orderId >= startOrderId) & (tableData.orderId <= endOrderId))
			{
				found = true;
				addLineItemGraphic(num, 2, num2, 50, 0, tableData.orderId - startOrderId, tableData.orderId, tableData.isAvailable, tableData.isComplete, tableData.myColor);
				if (tableData.orderId == selectedPosition)
				{
					updateTextBox(tableData, num2, 300, isReturnResults: false);
				}
				maxPositionDisplayed = tableData.orderId;
				num += 40;
			}
		}
	}

	private void turnOffDisplayById(int id)
	{
		foreach (DisplayData display in displayList)
		{
			if (display.objectId == id)
			{
				display.isDisplayed = false;
				break;
			}
		}
	}

	private void addPressAButton()
	{
		d = new DisplayData();
		d.baseImageName = "ButtonA";
		d._textureName = "PDA";
		d.position = new Vector2(1100f, 600f);
		d.origin = new Vector2(0f, 0f);
		d.isDisplayed = true;
		d.depth = 0.71f;
		d.scale = 1f;
		d.objectId = 112;
		d.rotation = 0f;
		d.myColor = Color.White;
		addGraphic(d);
	}

	private void resetDisplay()
	{
		displayList.Clear();
	}

	private void addLowerArrow(int y)
	{
		d = new DisplayData();
		d.baseImageName = "ArrowSmall";
		d._textureName = "PDA";
		d.position = new Vector2(1090f, y);
		d.origin = new Vector2(11f, 10f);
		d.isDisplayed = true;
		d.depth = 0.71f;
		d.scale = arrowDownScale;
		d.objectId = 117;
		d.rotation = (float)Math.PI;
		d.myColor = Color.White;
		updateGraphic(d);
	}

	private void addUpperArrow(int y)
	{
		d = new DisplayData();
		d.baseImageName = "ArrowSmall";
		d._textureName = "PDA";
		d.position = new Vector2(1090f, y);
		d.origin = new Vector2(11f, 10f);
		d.isDisplayed = true;
		d.depth = 0.71f;
		d.scale = arrowUpScale;
		d.objectId = 118;
		d.rotation = 0f;
		d.myColor = Color.White;
		updateGraphic(d);
	}

	private void addTextBox(int y, int x, int lines, bool isActive)
	{
		int num = y;
		d = new DisplayData();
		d.baseImageName = "BigBoxTop";
		d._textureName = "PDA";
		num -= 5;
		d.position = new Vector2((float)x + xOffset, (float)num + yOffset);
		d.origin = new Vector2(0f, 0f);
		d.isDisplayed = true;
		d.depth = 0.51f;
		d.scale = 1f;
		d.objectId = 109;
		d.rotation = 0f;
		d.myColor = Color.White;
		addGraphic(d);
		d = new DisplayData();
		d.baseImageName = "BigBoxMiddle";
		d._textureName = "PDA";
		num += 17;
		d.position = new Vector2((float)x + xOffset, (float)num + yOffset);
		d.origin = new Vector2(0f, 0f);
		d.isDisplayed = true;
		d.depth = 0.51f;
		d.scale = 1f;
		d.objectId = 110;
		d.rotation = 0f;
		d.myColor = Color.White;
		addGraphic(d);
		d = new DisplayData();
		d.baseImageName = "BigBoxMiddle";
		d._textureName = "PDA";
		num += 17;
		d.position = new Vector2((float)x + xOffset, (float)num + yOffset);
		d.origin = new Vector2(0f, 0f);
		d.isDisplayed = true;
		d.depth = 0.51f;
		d.scale = 1f;
		d.objectId = 110;
		d.rotation = 0f;
		d.myColor = Color.White;
		addGraphic(d);
		if (lines > 2)
		{
			d = new DisplayData();
			d.baseImageName = "BigBoxMiddle";
			d._textureName = "PDA";
			num += 17;
			d.position = new Vector2((float)x + xOffset, (float)num + yOffset);
			d.origin = new Vector2(0f, 0f);
			d.isDisplayed = true;
			d.depth = 0.51f;
			d.scale = 1f;
			d.objectId = 110;
			d.rotation = 0f;
			d.myColor = Color.White;
			addGraphic(d);
		}
		if (lines > 2)
		{
			d = new DisplayData();
			d.baseImageName = "BigBoxMiddle";
			d._textureName = "PDA";
			num += 17;
			d.position = new Vector2((float)x + xOffset, (float)num + yOffset);
			d.origin = new Vector2(0f, 0f);
			d.isDisplayed = true;
			d.depth = 0.51f;
			d.scale = 1f;
			d.objectId = 110;
			d.rotation = 0f;
			d.myColor = Color.White;
			addGraphic(d);
		}
		if (lines > 3)
		{
			d = new DisplayData();
			d.baseImageName = "BigBoxMiddle";
			d._textureName = "PDA";
			num += 17;
			d.position = new Vector2((float)x + xOffset, (float)num + yOffset);
			d.origin = new Vector2(0f, 0f);
			d.isDisplayed = true;
			d.depth = 0.51f;
			d.scale = 1f;
			d.objectId = 110;
			d.rotation = 0f;
			d.myColor = Color.White;
			addGraphic(d);
		}
		if (lines > 4)
		{
			d = new DisplayData();
			d.baseImageName = "BigBoxMiddle";
			d._textureName = "PDA";
			num += 17;
			d.position = new Vector2((float)x + xOffset, (float)num + yOffset);
			d.origin = new Vector2(0f, 0f);
			d.isDisplayed = true;
			d.depth = 0.51f;
			d.scale = 1f;
			d.objectId = 110;
			d.rotation = 0f;
			d.myColor = Color.White;
			addGraphic(d);
		}
		if (lines > 5)
		{
			d = new DisplayData();
			d.baseImageName = "BigBoxMiddle";
			d._textureName = "PDA";
			num += 17;
			d.position = new Vector2((float)x + xOffset, (float)num + yOffset);
			d.origin = new Vector2(0f, 0f);
			d.isDisplayed = true;
			d.depth = 0.51f;
			d.scale = 1f;
			d.objectId = 110;
			d.rotation = 0f;
			d.myColor = Color.White;
			addGraphic(d);
		}
		if (lines > 6)
		{
			d = new DisplayData();
			d.baseImageName = "BigBoxMiddle";
			d._textureName = "PDA";
			num += 17;
			d.position = new Vector2((float)x + xOffset, (float)num + yOffset);
			d.origin = new Vector2(0f, 0f);
			d.isDisplayed = true;
			d.depth = 0.51f;
			d.scale = 1f;
			d.objectId = 110;
			d.rotation = 0f;
			d.myColor = Color.White;
			addGraphic(d);
		}
		d = new DisplayData();
		d.baseImageName = "BigBoxBottom";
		d._textureName = "PDA";
		num += 35;
		d.position = new Vector2((float)x + xOffset, (float)num + yOffset);
		d.origin = new Vector2(0f, 0f);
		d.isDisplayed = true;
		d.depth = 0.51f;
		d.scale = 1f;
		d.objectId = 111;
		d.rotation = 0f;
		d.myColor = Color.White;
		addGraphic(d);
		if (isActive)
		{
			t = new DisplayText();
			t.addTextRaw(DisplayText.GroupTextType.TextBoxSpecial, "Select:", Color.White, new Vector2(1010f, 600f), isTypedAnimated: false, myCoreDisplayElements.myPDAFontRegular, 700);
			displayText.Add(t);
			addPressAButton();
		}
	}

	private void addLineItemGraphic(int y, int columns, int x1, int x2, int x3, int id, int orderId, bool isAvailable, bool isComplete, Color myColor)
	{
		d = new DisplayData();
		if (orderId == selectedPosition)
		{
			if (isAvailable)
			{
				d.baseImageName = "TextBoxHighlight";
			}
			else if (!isAvailable && !isComplete)
			{
				d.baseImageName = "BoxUnavailableHighlight";
			}
			else
			{
				d.baseImageName = "BoxCompletedHighLight";
			}
		}
		else if (isAvailable)
		{
			d.baseImageName = "TextBox";
		}
		else if (!isAvailable && !isComplete)
		{
			d.baseImageName = "BoxUnavailable";
		}
		else
		{
			d.baseImageName = "BoxCompleted";
		}
		d._textureName = "PDA";
		d.position = new Vector2((float)x1 + xOffset, (float)y + yOffset);
		d.origin = new Vector2(0f, 0f);
		d.isDisplayed = true;
		d.depth = 0.51f;
		d.scale = 1f;
		d.objectId = orderId;
		d.rotation = 0f;
		d.objectType = DisplayData.ObjectTypeEnum.TextBox;
		d.myColor = myColor;
		updateGraphic(d);
	}

	private void updateGraphic(DisplayData newD)
	{
		bool flag = false;
		foreach (DisplayData display in displayList)
		{
			if (display.objectId == newD.objectId)
			{
				display.isDisplayed = true;
				display.myColor = newD.myColor;
				display.position = newD.position;
				display.rotation = newD.rotation;
				display.scale = newD.scale;
				flag = true;
			}
		}
		if (!flag)
		{
			newD.isDisplayed = true;
			displayList.Add(newD);
		}
	}

	private void addGraphic(DisplayData newD)
	{
		newD.isDisplayed = true;
		displayList.Add(newD);
	}

	public void updatePositions()
	{
		foreach (DisplayText item in displayText)
		{
			if (item.groupType == DisplayText.GroupTextType.Header)
			{
				item.position += new Vector2(deltaX, deltaY);
			}
			else if (item.groupType == DisplayText.GroupTextType.Regular)
			{
				item.position += new Vector2(deltaX, deltaY);
			}
			else if (item.groupType == DisplayText.GroupTextType.TextBoxSpecial)
			{
				item.position += new Vector2(deltaX, deltaY);
			}
		}
		foreach (DisplayData display in displayList)
		{
			if ((display.objectId >= 100) | (display.objectId <= 105))
			{
				display.position += new Vector2(deltaX, deltaY);
			}
		}
	}

	public void updateHeaderTextColor(Color myColor)
	{
		foreach (DisplayText item in displayText)
		{
			if (item.groupType == DisplayText.GroupTextType.Header)
			{
				item.color = myColor;
			}
		}
		foreach (DisplayData display in displayList)
		{
			if ((display.objectId >= 100) | (display.objectId <= 105))
			{
				display.isDisplayed = true;
			}
		}
	}

	private void drawText(SpriteBatch spriteBatch)
	{
		if (!isActive)
		{
			return;
		}
		if (isReturnDisplay)
		{
			foreach (PDATextListData tableData in tableDataList)
			{
				if (tableData.orderId == selectedPosition)
				{
					yAdjuster = 0;
					drawSingleText(tableData.columnA, new Vector2(270f + xOffset, 125f + yOffset + (float)yAdjuster), spriteBatch, myCoreDisplayElements.myPDAFontRegular, tableData.myColor);
					drawSingleText(tableData.colummB, new Vector2((float)(tableData.columBX + 200) + xOffset, 125f + yOffset + (float)yAdjuster), spriteBatch, myCoreDisplayElements.myPDAFontRegular, tableData.myColor);
				}
			}
			return;
		}
		foreach (PDATextListData tableData2 in tableDataList)
		{
			if ((tableData2.orderId >= startOrderId) & (tableData2.orderId <= endOrderId))
			{
				yAdjuster = 40 * (tableData2.orderId - startOrderId);
				drawSingleText(tableData2.columnA, new Vector2(270f + xOffset, 125f + yOffset + (float)yAdjuster), spriteBatch, myCoreDisplayElements.myPDAFontRegular, tableData2.myColor);
				drawSingleText(tableData2.colummB, new Vector2((float)(tableData2.columBX + 200) + xOffset, 125f + yOffset + (float)yAdjuster), spriteBatch, myCoreDisplayElements.myPDAFontRegular, tableData2.myColor);
			}
		}
	}

	private void drawSingleText(string text, Vector2 position, SpriteBatch spriteBatch, SpriteFont myFont, Color myColor)
	{
		tempPosition = position;
		spriteBatch.DrawString(myFont, text, position, myColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.994f);
		tempPosition.X += 2f;
		tempPosition.Y += 2f;
		spriteBatch.DrawString(myFont, text, tempPosition, new Color(0, 0, 0, myColor.A), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.992f);
		tempPosition.X -= 3f;
		tempPosition.Y -= 3f;
	}

	public void draw(SpriteBatch spriteBatch)
	{
		if (!isOn)
		{
			return;
		}
		foreach (DisplayData display in displayList)
		{
			if (display.isDisplayed)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture(display.textureName), display.position, myCoreDisplayElements.spriteRDM.getSpriteRectangle(display.textureName, display.baseImageName, display.currentFrame), display.myColor, display.rotation, display.origin, display.scale, SpriteEffects.None, display.depth);
			}
		}
		drawText(spriteBatch);
		try
		{
			foreach (DisplayText item in displayText)
			{
				tempPosition = item.position;
				spriteBatch.DrawString(item.myFont, item.getText(), item.position, item.color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.994f);
				tempPosition.X += 2f;
				tempPosition.Y += 2f;
				spriteBatch.DrawString(item.myFont, item.getText(), tempPosition, new Color(0, 0, 0, item.color.A), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.992f);
				tempPosition.X -= 3f;
				tempPosition.Y -= 3f;
				if (!item.isFinishedDrawing)
				{
					break;
				}
			}
		}
		catch
		{
		}
	}
}
