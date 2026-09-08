using Microsoft.Xna.Framework;
using Quasar.GameUtils.Awards;
using Quasar.GameUtils.Meshes;
using Quasar.Global;
using Quasar.Language;
using Quasar.Meshes;
using Quasar.Meshes.Text;

namespace Quasar.GameUtils.Template.Controls;

internal class AwardsNodeItem : RenderItem
{
	private const int PROGRESS_WIDTH = 300;

	private const int PROGRESS_HEIGHT = 30;

	private const int PROGRESS_PADDING_X = 16;

	private const int PROGRESS_PADDING_Y = 12;

	private const int PROGRESS_BAR_WIDTH = 268;

	private const int PROGRESS_BAR_HEIGHT = 6;

	private const int PROGRESS_Y = -35;

	private Sized2DRectangleMesh image;

	private TextMesh name;

	private TextMesh description;

	private TextMesh unlocked;

	private Sized2DRectangleMesh progressBar;

	private TextMesh progressText;

	private HUDDetailRectangle progressBG;

	public AwardsNodeItem()
	{
		addMesh(new HUDDetailRectangle(new Vector2(650f, 130f)));
		image = new Sized2DRectangleMesh(new Vector2(90f), null);
		image.Offset = new Vector2(261f, 0f);
		addMesh(image);
		name = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(HorizontalAlignment.Left), 40, useStringBuilder: false);
		name.Offset = new Vector2(-300f, 45f);
		name.Diffuse = GameTemplate.DialogTitleColor;
		addMesh(name);
		description = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(-300f, 5f), 0.8f, HorizontalAlignment.Left), 100, useStringBuilder: false);
		addMesh(description);
		unlocked = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(0.8f, HorizontalAlignment.Left), 30, useStringBuilder: false);
		unlocked.Offset = new Vector2(-300f, -28f);
		addMesh(unlocked);
		progressText = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(0.8f, HorizontalAlignment.Left), 30, useStringBuilder: false);
		progressText.Offset = new Vector2(5f, -28f);
		addMesh(progressText);
		progressBG = new HUDDetailRectangle(new Vector2(300f, 30f));
		progressBG.Offset = new Vector2(-150f, -35f);
		addMesh(progressBG);
		progressBar = new Sized2DRectangleMesh(new Vector2(268f, 6f), GameTemplate.DialogTitleColor);
		progressBar.Offset = new Vector2(-150f, -35f);
		addMesh(progressBar);
	}

	public void SetData(AwardProgress item)
	{
		name.Text = ((item.Award.IsSecret && !item.IsUnlocked) ? LanguageManager.Texts["SECRET_AWARD_NAME"] : item.Award.Name);
		description.Text = (item.IsUnlocked ? item.Award.Description : (item.Award.IsSecret ? LanguageManager.Texts["SECRET_AWARD_HINT"] : item.Award.Hint));
		image.FirstMaterial.Texture = ((item.Award.IsSecret && !item.IsUnlocked) ? Award.SecretTexture : item.Award.Texture);
		image.Diffuse = Vector3.One;
		image.Alpha = (item.IsUnlocked ? 1f : 0.4f);
		name.Diffuse = (item.IsUnlocked ? GameTemplate.DialogTitleColor : GameTemplate.DialogTitleColorDisabled);
		if (item.IsUnlocked)
		{
			if (item.UnlockDate.Ticks != 0)
			{
				unlocked.Text = string.Format(LanguageManager.Texts["UNLOCKED_ON_{0}"], GameMath.DateToString(item.UnlockDate));
			}
			else
			{
				unlocked.Text = string.Format(LanguageManager.Texts["AWARD_UNLOCKED"], GameMath.DateToString(item.UnlockDate));
			}
			progressBar.Alpha = 0f;
			progressBG.Alpha = 0f;
			progressText.Text = "";
		}
		else if (!item.Award.IsSecret && item.Award.IsProgress)
		{
			unlocked.Text = "";
			progressBar.Diffuse = GameTemplate.DialogTitleColor;
			progressBar.Alpha = 1f;
			float num = 268f * item.Percentage;
			progressBar.Size = new Vector2(num, 6f);
			progressBar.Offset = new Vector2(-284f + num * 0.5f, -35f);
			progressBG.Diffuse = Vector3.One;
			progressBG.Alpha = 1f;
			string text = string.Format("{0} / {1}", GameMath.UIntToString(item.Progress, "#,0"), GameMath.UIntToString(item.Award.ProgressNeeded, "#,0"));
			progressText.Text = text;
			progressText.Scale = ((text.Length <= 15) ? 0.8f : ((text.Length > 25) ? 0.5f : 0.6f));
		}
		else
		{
			unlocked.Text = LanguageManager.Texts["AWARD_NOT_UNLOCKED"];
			progressBar.Alpha = 0f;
			progressBG.Alpha = 0f;
			progressText.Text = "";
		}
		Visible = true;
	}

	public void Reset()
	{
		Visible = false;
	}
}
