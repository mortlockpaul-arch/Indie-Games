using System;
using GKEngine;
using GKEngine.Input;
using Game.QBits;
using Game.Scenes.Play.Players.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Scenes.Play.Players;

public class PlayerManager
{
	public int PLAYERS_MAX = 4;

	private Rectangle _view_out;

	private Rectangle _view_in;

	private Matrix _view_matrix = Matrix.Identity;

	private Vector4 _view_result = default(Vector4);

	private Point _view_point = default(Point);

	public PlayScene scene;

	public PlayUniverse universe;

	public Player[] players;

	public PlayerCamera camera;

	public bool paused;

	public int count;

	public int singlePlayerIndex;

	public PlayerUI ui;

	public bool hinting;

	private Vector3 _position = default(Vector3);

	public Vector3 position
	{
		get
		{
			_position.X = 0f;
			_position.Y = 0f;
			_position.Z = 0f;
			if (count > 1)
			{
				for (int i = 0; i < players.Length; i++)
				{
					if (players[i].active)
					{
						_position.X += players[i].position.X;
						_position.Y += players[i].position.Y;
						_position.Z += players[i].position.Z;
					}
				}
				if (count > 0)
				{
					_position.X /= (float)count;
					_position.Y /= (float)count;
					_position.Z /= (float)count;
				}
			}
			else if (count == 1)
			{
				_position.X = players[singlePlayerIndex].position.X;
				_position.Y = players[singlePlayerIndex].position.Y;
				_position.Z = players[singlePlayerIndex].position.Z;
			}
			return _position;
		}
	}

	public Player primaryPlayer => players[UniversalInput.gamePadPrimaryIndex];

	public PlayerManager(PlayUniverse oUniverse)
	{
		universe = oUniverse;
		scene = universe.scene;
		singlePlayerIndex = UniversalInput.gamePadPrimaryIndex;
		Init();
	}

	public void Init()
	{
		ui = new PlayerUI(this);
		camera = new PlayerCamera(this);
		scene.cameras.SetActive(PlayerCamera.CAMERA_NAME);
		players = new Player[4]
		{
			new Player(this, 0),
			new Player(this, 1),
			new Player(this, 2),
			new Player(this, 3)
		};
		Viewport viewport = GameEngine.Graphics.GraphicsDevice.Viewport;
		_view_out = new Rectangle(viewport.TitleSafeArea.X + 75, viewport.TitleSafeArea.Y + 50, viewport.TitleSafeArea.Width - 150, viewport.TitleSafeArea.Height - 100);
		_view_in = new Rectangle(viewport.TitleSafeArea.X + 200, viewport.TitleSafeArea.Y + 150, viewport.TitleSafeArea.Width - 400, viewport.TitleSafeArea.Height - 300);
		Activate(singlePlayerIndex);
	}

	public void Activate(int xIndex)
	{
		QBit freeQBit = GetFreeQBit(0, null);
		if (freeQBit != null)
		{
			players[xIndex].Activate(freeQBit);
			camera.Input_Activate(xIndex);
			count++;
		}
		SinglePlayerIndexRefresh();
		ui.Resolve();
	}

	public void Deactivate(int xIndex)
	{
		if (count > 1)
		{
			camera.Input_Deactivate(xIndex);
			players[xIndex].Deactivate();
			camera.Refresh();
			count--;
		}
		SinglePlayerIndexRefresh();
		ui.Resolve();
	}

	private void SinglePlayerIndexRefresh()
	{
		if (primaryPlayer.active)
		{
			singlePlayerIndex = UniversalInput.gamePadPrimaryIndex;
			return;
		}
		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].active)
			{
				singlePlayerIndex = i;
				break;
			}
		}
	}

	public void Update(GameTime elapsed)
	{
		for (int i = 0; i < players.Length; i++)
		{
			players[i].Update(elapsed);
		}
		ui.Update(elapsed);
		AutoZoom(elapsed);
		camera.Update(elapsed);
	}

	public void Dispose()
	{
		ui.Dispose();
		for (int i = 0; i < players.Length; i++)
		{
			players[i].Dispose();
		}
		players = null;
		camera.Dispose();
	}

	public void Input_Update(GameTime oGameTime)
	{
		if (!paused)
		{
			for (int i = 0; i < players.Length; i++)
			{
				players[i].Input_Update(oGameTime);
			}
			camera.Input_Update(oGameTime);
		}
	}

	public void Input_Activate()
	{
		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].active)
			{
				players[i].Input_Activate();
			}
		}
	}

	public void Input_Deactivate()
	{
		for (int i = 0; i < players.Length; i++)
		{
			players[i].Input_Deactivate();
		}
	}

	public QBit GetFreeQBit(int xIndex, QBit.QBitType? oType)
	{
		QBit result = null;
		bool flag = false;
		for (int i = 0; i < universe.qbits.qbits.Count; i++)
		{
			flag = false;
			int index = (xIndex + i) % universe.qbits.qbits.Count;
			if (universe.qbits.qbits[index].home || universe.qbits.qbits[index].dead || universe.qbits.qbits[index].dying || universe.qbits.qbits[index].exiting || (oType.HasValue && (!oType.HasValue || oType.Value != universe.qbits.qbits[index].type)))
			{
				continue;
			}
			flag = true;
			for (int j = 0; j < players.Length; j++)
			{
				if (players[j].active && scene.universe.qbits.qbits[index] == players[j].qbit)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				result = universe.qbits.qbits[index];
				break;
			}
		}
		return result;
	}

	public void ReassignPlayer(QBit oQBit)
	{
		if (universe.history.reversing)
		{
			return;
		}
		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].qbit == oQBit)
			{
				QBit freeQBit = GetFreeQBit(0, null);
				if (freeQBit != null)
				{
					players[i].QBit_Set(freeQBit);
				}
				else
				{
					players[i].QBit_Set(null);
				}
				break;
			}
		}
		ui.Resolve();
	}

	public void AutoZoom(GameTime oGameTime)
	{
		if (count <= 1)
		{
			return;
		}
		Viewport viewport = GameEngine.Graphics.GraphicsDevice.Viewport;
		bool flag = false;
		int num = 0;
		_view_matrix = Matrix.Identity * camera.camera.view * camera.camera.projection;
		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].active)
			{
				Vector4.Transform(ref players[i]._position, ref _view_matrix, out _view_result);
				_view_point.X = (int)((_view_result.X / _view_result.W + 1f) * ((float)viewport.Width * 0.5f));
				_view_point.Y = (int)((1f - _view_result.Y / _view_result.W) * ((float)viewport.Height * 0.5f));
				if (!_view_out.Contains(_view_point))
				{
					flag = true;
				}
				if (_view_in.Contains(_view_point))
				{
					num++;
				}
			}
		}
		if (flag)
		{
			camera.radius += (float)(oGameTime.ElapsedGameTime.TotalMilliseconds * 0.10000000149011612);
		}
		else if (!camera.zoomed && num >= count)
		{
			camera.radius = Math.Max(camera.radius - (float)(oGameTime.ElapsedGameTime.TotalMilliseconds * 0.10000000149011612), PlayerCamera.RADIUS_MIN);
		}
	}

	public void Hint()
	{
		if (!hinting)
		{
			scene.audio.EventCues_Trigger("Sound_Button");
		}
		hinting = true;
	}

	public void Hint_Halt()
	{
		bool flag = true;
		for (int i = 0; i < players.Length; i++)
		{
			if (players[i].inputHint.isDown)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			hinting = false;
		}
	}

	public bool PressCheck_Reversing_Switch(GameTime oGameTime)
	{
		bool result = false;
		for (int i = 0; i < players.Length; i++)
		{
			players[i].inputSwitch.Update(oGameTime);
			if (players[i].inputSwitch.pressed && players[i].qbit != null && !players[i].qbit.dead && !players[i].qbit.dying)
			{
				result = true;
				break;
			}
		}
		return result;
	}
}
