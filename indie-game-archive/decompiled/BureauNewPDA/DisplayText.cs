using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BureauNewPDA;

public class DisplayText
{
	public enum GroupTextType
	{
		Header,
		Footer,
		Regular,
		TextBoxSpecial,
		UnAssigned
	}

	public string spriteTextRaw;

	private string spriteTextAdjusted;

	public Color color;

	public Vector2 position;

	public bool isFinishedDrawing;

	public bool isSpace;

	private bool isType;

	public bool isQuestion;

	private int lines = 1;

	public GroupTextType groupType = GroupTextType.UnAssigned;

	public string groupName = "";

	public Vector2 origin = Vector2.Zero;

	private int textId;

	public SpriteFont myFont;

	public bool finishAnimationNow;

	private int subCount;

	private int progress;

	public int returnLines;

	private StringBuilder displayBuilderText = new StringBuilder();

	private char[] displayBuilderTextHold;

	public void addTextRaw(GroupTextType _groupType, string text, Color _color, Vector2 _position, bool isTypedAnimated, SpriteFont myFont, int width)
	{
		spriteTextRaw = text;
		spriteTextAdjusted = parseText(text, myFont, width);
		color = _color;
		isType = isTypedAnimated;
		this.myFont = myFont;
		if (!isType | (text.Length < 2))
		{
			isFinishedDrawing = true;
		}
		lines = lineCount(text, myFont, width);
		position = _position;
		groupType = _groupType;
	}

	public void addTextRawForBuilder(string text, SpriteFont myFont, int columnSize)
	{
		displayBuilderText.Length = 0;
		displayBuilderTextHold = parseText(text, myFont, columnSize).ToCharArray();
	}

	public void advanceDisplayBuilderText()
	{
		if (displayBuilderText.Length < displayBuilderTextHold.Length)
		{
			displayBuilderText.Append(displayBuilderTextHold[displayBuilderText.Length]);
			isFinishedDrawing = false;
		}
		else
		{
			isFinishedDrawing = true;
		}
	}

	public StringBuilder getDisplayBuilderText()
	{
		return displayBuilderText;
	}

	public void addTextRawAdjustByLine(string text, Color _color, Vector2 _position, bool isTypedAnimated, SpriteFont myFont, int width, float yAdjust)
	{
		spriteTextRaw = text;
		spriteTextAdjusted = parseText(text, myFont, width);
		color = _color;
		isType = isTypedAnimated;
		lines = lineCount(text, myFont, width);
		float num = yAdjust * (float)lines;
		position = new Vector2(_position.X, _position.Y - num);
	}

	public string getText()
	{
		if (isType)
		{
			char[] array = spriteTextAdjusted.ToCharArray();
			string text = "";
			if ((progress == 0) | (subCount > 1))
			{
				progress++;
				subCount = 0;
			}
			else
			{
				subCount++;
			}
			if ((progress == array.Length) | finishAnimationNow)
			{
				isType = false;
				isFinishedDrawing = true;
				finishAnimationNow = false;
				progress = array.Length;
			}
			for (int i = 0; i < progress; i++)
			{
				text += array[i];
			}
			if (array[progress - 1].ToString() == " ")
			{
				isSpace = true;
			}
			else
			{
				isSpace = false;
			}
			return text;
		}
		return spriteTextAdjusted;
	}

	private string parseText(string text, SpriteFont myFont, int width)
	{
		returnLines = 1;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string[] array = text.Split(' ');
		string[] array2 = array;
		foreach (string text4 in array2)
		{
			if (myFont.MeasureString(text2 + text4).Length() > (float)width)
			{
				text3 = text3 + text2 + '\n';
				returnLines++;
				text2 = string.Empty;
			}
			text2 = text2 + text4 + ' ';
		}
		return text3 + text2;
	}

	public int lineCount(string text, SpriteFont myFont, int width)
	{
		string text2 = string.Empty;
		string text3 = string.Empty;
		string[] array = text.Split(' ');
		int num = 1;
		string[] array2 = array;
		foreach (string text4 in array2)
		{
			if (myFont.MeasureString(text2 + text4).Length() > (float)width)
			{
				text3 = text3 + text2 + '\n';
				num++;
				text2 = string.Empty;
			}
			text2 = text2 + text4 + ' ';
		}
		return num;
	}
}
