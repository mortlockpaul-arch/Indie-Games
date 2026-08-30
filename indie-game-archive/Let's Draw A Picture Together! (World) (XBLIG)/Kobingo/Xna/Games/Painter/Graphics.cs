using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Kobingo.Xna.Games.Painter;

internal class Graphics
{
	public static Texture2D Brush { get; private set; }

	public static Texture2D Brush1 { get; private set; }

	public static Texture2D Pencil { get; private set; }

	public static Texture2D Pencil1 { get; private set; }

	public static Texture2D Bucket { get; private set; }

	public static Texture2D Locked { get; private set; }

	public static Texture2D Border { get; private set; }

	public static Texture2D Palette { get; private set; }

	public static Texture2D Controls1 { get; private set; }

	public static Texture2D Controls2 { get; private set; }

	public static Texture2D Controls3 { get; private set; }

	public static Texture2D Controls4 { get; private set; }

	public static Texture2D Controls5 { get; private set; }

	public static Texture2D Progress { get; private set; }

	public static Texture2D MenuBack { get; private set; }

	public static Texture2D UnlockBack { get; private set; }

	public static Texture2D Title { get; private set; }

	public static Texture2D Background { get; private set; }

	public static Texture2D GalleryBack { get; private set; }

	public static Texture2D Blank { get; private set; }

	public static Texture2D ButtonA { get; private set; }

	public static Texture2D ButtonB { get; private set; }

	public static Texture2D ButtonX { get; private set; }

	public static Texture2D ButtonY { get; private set; }

	public static void Load(GraphicsDevice graphicsDevice, ContentManager content)
	{
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Expected O, but got Unknown
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		Brush = content.Load<Texture2D>("Graphics/Brush");
		Brush1 = content.Load<Texture2D>("Graphics/Brush1");
		Pencil = content.Load<Texture2D>("Graphics/Pencil");
		Pencil1 = content.Load<Texture2D>("Graphics/Pencil1");
		Bucket = content.Load<Texture2D>("Graphics/Bucket");
		Locked = content.Load<Texture2D>("Graphics/Locked");
		Brush = content.Load<Texture2D>("Graphics/Brush");
		Border = content.Load<Texture2D>("Graphics/Border");
		Palette = content.Load<Texture2D>("Graphics/Palette");
		Progress = content.Load<Texture2D>("Graphics/Progress");
		MenuBack = content.Load<Texture2D>("Graphics/MenuBack");
		UnlockBack = content.Load<Texture2D>("Graphics/UnlockBack");
		Controls1 = content.Load<Texture2D>("Graphics/Controls1");
		Controls2 = content.Load<Texture2D>("Graphics/Controls2");
		Controls3 = content.Load<Texture2D>("Graphics/Controls3");
		Controls4 = content.Load<Texture2D>("Graphics/Controls4");
		Controls5 = content.Load<Texture2D>("Graphics/Controls5");
		Background = content.Load<Texture2D>("Graphics/Background");
		GalleryBack = content.Load<Texture2D>("Graphics/GalleryBack");
		Title = content.Load<Texture2D>("Graphics/Title");
		ButtonA = content.Load<Texture2D>("Graphics/ButtonA");
		ButtonB = content.Load<Texture2D>("Graphics/ButtonB");
		ButtonX = content.Load<Texture2D>("Graphics/ButtonX");
		ButtonY = content.Load<Texture2D>("Graphics/ButtonY");
		Blank = new Texture2D(graphicsDevice, 46, 46);
		Color[] array = (Color[])(object)new Color[Blank.Width * Blank.Height];
		for (int i = 0; i < array.Length; i++)
		{
			ref Color reference = ref array[i];
			reference = Color.White;
		}
		Blank.SetData<Color>(array);
	}
}
