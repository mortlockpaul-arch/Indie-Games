using Microsoft.Xna.Framework;
using Quasar.GameUtils.Meshes;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Template.Controls;
using Quasar.Global;
using Quasar.Language;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Shaders;

namespace Quasar.GameUtils.Template;

public class PlayerSelectItem : RenderItem
{
	private const float Width = 400f;

	private const float Height = 225f;

	private const float bgOffset = -3f;

	private const float bgBorder = 32f;

	private const float PICTURE_X = -140f;

	private const float PICTURE_Y = 55f;

	private const float PICTURE_SIZE = 64f;

	private const float BORDER = 2f;

	private const float GAMERTAG_X = -85f;

	private const float GAMERTAG_Y = 65f;

	private const float READY_TEXT_X = -85f;

	private const float READY_TEXT_Y = -30f;

	private const string READY_TEXTURE = "GUI/Ready";

	private PlayerSelect playerSelect;

	protected PlayerIndex playerIndex;

	private Sized2DRectangleMesh picture;

	private Sized2DRectangleMesh border;

	private TextMesh gamertag;

	private TextMesh readyText;

	private HUDRectangle background;

	protected float opacity;

	public PlayerSelectItem(PlayerSelect playerSelect, PlayerIndex playerIndex)
	{
		this.playerSelect = playerSelect;
		this.playerIndex = playerIndex;
		background = new HUDRectangle(new Vector2(400f, 225f));
		addMesh(background);
		HUDDetailRectangle m = new HUDDetailRectangle(new Vector2(375f, 200f));
		addMesh(m);
		border = new Sized2DRectangleMesh(new Vector2(64f), new Vector4(Vector3.Zero, 1f));
		border.Offset = new Vector2(-138f, 53f);
		border.FirstMaterial.RenderPriority = (Material.Priority)101;
		addMesh(border);
		picture = new Sized2DRectangleMesh(new Vector2(64f), Quasar.GameUtils.Player.Player.GetPlayerPicture(playerIndex));
		picture.Offset = new Vector2(-140f, 55f);
		picture.Shader = ShaderManager.Shaders["PointGUI"];
		addMesh(picture);
		gamertag = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(-85f, 65f), HorizontalAlignment.Left), 40, useStringBuilder: false);
		addMesh(gamertag);
		readyText = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(-85f, -30f), HorizontalAlignment.Left), 40, useStringBuilder: false);
		readyText.Text = LanguageManager.Texts["READY"];
		readyText.Diffuse = GameTemplate.DialogTitleColor;
		readyText.Alpha = (playerSelect.Ready(playerIndex) ? 1 : 0);
		addMesh(readyText);
		opacity = (playerSelect.Ready(playerIndex) ? 1f : 0.3f);
	}

	protected override void DoUpdate()
	{
		bool flag = playerSelect.Connected(playerIndex);
		bool flag2 = playerSelect.Ready(playerIndex);
		float to = (flag2 ? 1f : 0f);
		opacity = GameMath.Damping(opacity, to, 0.05f, Timer.DefaultTimer.LastIntervalSeconds);
		float alpha = opacity * 0.7f + 0.3f;
		border.Diffuse = Vector3.Zero;
		border.Alpha = alpha;
		picture.Diffuse = Vector3.One;
		picture.Alpha = alpha;
		picture.FirstMaterial.Texture = Quasar.GameUtils.Player.Player.GetPlayerPicture(playerIndex);
		gamertag.Text = (flag ? Quasar.GameUtils.Player.Player.GetPlayerName(playerIndex) : LanguageManager.Texts["UNCONNECTED"]);
		gamertag.Diffuse = Vector3.One;
		gamertag.Alpha = alpha;
		readyText.Diffuse = Vector3.Lerp(GameTemplate.DialogTitleColorDisabled, GameTemplate.DialogTitleColor, opacity);
		readyText.Alpha = alpha;
		readyText.Text = (flag2 ? LanguageManager.Texts["READY"] : LanguageManager.Texts["NOT_READY"]);
		base.DoUpdate();
	}
}
