using System.Text;
using AvatarFarmOnline.Logic.Stage;
using AvatarFarmOnline.Template;
using Microsoft.Xna.Framework;
using Quasar;
using Quasar.Global;
using Quasar.Meshes.Text;

namespace AvatarFarmOnline.Items.Game.HUD;

internal class ActionLog : RenderItem
{
	private class LogItem : RenderItem
	{
		private TextBoxMesh mesh;

		private long showTime;

		public StringBuilder StringBuilder => mesh.StringBuilder;

		public long ShowTime => showTime;

		public LogItem(Layout2D.LayoutData layout)
		{
			mesh = new TextBoxMesh(AvatarFarmOnline.Template.ExtendedGameTemplate.HUDFont, new TextBoxDrawProperties(layout, 1f, HorizontalAlignment.Left, VerticalAlignment.Center), 80, useStringBuilder: true);
			addMesh(mesh);
		}

		public void SetData(LogItem logItem)
		{
			Visible = logItem.Visible;
			mesh.Diffuse = logItem.mesh.Diffuse;
			mesh.StringBuilder.Length = 0;
			StringBuilder stringBuilder = logItem.StringBuilder;
			int length = stringBuilder.Length;
			for (int i = 0; i < length; i++)
			{
				mesh.StringBuilder.Append(stringBuilder[i]);
			}
			showTime = logItem.showTime;
		}

		public void Show(Vector3 diffuse)
		{
			mesh.Diffuse = diffuse;
			showTime = Timer.DefaultTimer.TotalTime;
		}
	}

	private const int LINES = 10;

	private const int TEXT_DURATION = 8000;

	private static readonly Vector3 WorkColor = GameMath.RGBToVector(234, 236, 73);

	private AvatarFarmOnline.Logic.Stage.Stage stage;

	private LogItem[] lines;

	private LogItem firstLine;

	public ActionLog(AvatarFarmOnline.Logic.Stage.Stage stage)
	{
		this.stage = stage;
		Layout2D.LayoutData container = new Layout2D.LayoutData(new Vector2((0f - Engine.GUIWidth) * 0.2f, Engine.GUIHeight * 0.3f - 140f - 15f), new Vector2(Engine.GUIWidth * 0.5f, Engine.GUIHeight * 0.3f));
		stage.OnPlayerAdded += stage_OnPlayerAdded;
		stage.OnPlayerRemoved += stage_OnPlayerRemoved;
		lines = new LogItem[10];
		for (int i = 0; i < 10; i++)
		{
			Layout2D.GridLayout(container, 1, 10, 4f, i, out var cellLayout);
			lines[i] = new LogItem(cellLayout);
			addChild(lines[i]);
		}
		firstLine = lines[0];
	}

	private void stage_OnPlayerAdded(AvatarFarmOnline.Logic.Stage.Player obj)
	{
		if (obj is AvatarFarmOnline.Logic.Stage.NetworkPlayer)
		{
			PushTexts();
			AppendPlayerName(obj);
			firstLine.StringBuilder.Append(" joined the farm");
			firstLine.Show(Vector3.One);
		}
	}

	private void stage_OnPlayerRemoved(AvatarFarmOnline.Logic.Stage.Player obj)
	{
		if (obj is AvatarFarmOnline.Logic.Stage.NetworkPlayer)
		{
			PushTexts();
			AppendPlayerName(obj);
			firstLine.StringBuilder.Append(" left the farm");
			firstLine.Show(Vector3.One);
		}
	}

	private void AppendPlayerName(AvatarFarmOnline.Logic.Stage.Player player)
	{
		if (player is AvatarFarmOnline.Logic.Stage.LocalPlayer)
		{
			firstLine.StringBuilder.Append("You");
		}
		else
		{
			firstLine.StringBuilder.Append(player.Selection.PlayerName);
		}
	}

	private void PushTexts()
	{
		for (int num = 8; num >= 0; num--)
		{
			lines[num + 1].SetData(lines[num]);
		}
		firstLine.StringBuilder.Length = 0;
	}

	protected override void DoUpdate()
	{
		for (int i = 0; i < 10; i++)
		{
			lines[i].Visible = Timer.DefaultTimer.TimeSince(lines[i].ShowTime) <= 8000;
		}
		base.DoUpdate();
	}
}
