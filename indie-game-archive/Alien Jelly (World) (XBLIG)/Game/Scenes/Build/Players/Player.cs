using System;
using GKEngine.Entities;
using GKEngine.Input;
using Game.Data;
using Game.Grids;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Scenes.Build.Players;

public class Player : Base3D
{
	public enum MoveMode
	{
		XZ,
		Y,
		CAMERA
	}

	public enum MoveSnapMode
	{
		FREE,
		SNAPPING
	}

	private const float MOVE_SPEED = 0.1f;

	private const float MOVE_MULTIPLYER = 0.05f;

	private const int MOVE_COUNT_MAX = 6;

	private const int MOVE_Y_TIME = 200;

	private const int ATOM_PUSH_Y_TIME = 100;

	private const int BRUSHES_SCROLL_WAIT = 400;

	private const int BRUSHES_SCROLL_TIME = 100;

	private const float BRUSHES_SCROLL_THRESHOLD = 0.3f;

	public BuildUniverse universe;

	public PlayerCamera camera;

	public bool paused;

	public bool selecting;

	public bool deselecting;

	public bool inputPaused;

	public Vector3 move = default(Vector3);

	private bool moving;

	private int moveCount;

	public MoveMode moveMode;

	public MoveSnapMode moveSnapMode;

	private Vector3 movePosition = default(Vector3);

	private Vector3 movePositionPrevious = default(Vector3);

	private bool rotated;

	public GridPoint point = new GridPoint();

	public PlayerAvatar avatar;

	public PlayerShapeCursor shapeCursor;

	public bool atomsMoving;

	public bool atomsRotate;

	public bool atomsDefferedDeselect;

	public float cameraChangeMapYaw;

	public float cameraChangeMapPitch;

	public bool brushesScrollWaiting;

	public bool brushesScrollScrolling;

	public float brushesScrollTime;

	public int brushesScrollDir;

	public Player(BuildUniverse oUniverse)
	{
		universe = oUniverse;
		position = new Vector3(0f, 0f, 0f);
		rotation = Quaternion.Identity;
		point = new GridPoint();
		move = default(Vector3);
		Init();
	}

	public void Init()
	{
		avatar = new PlayerAvatar(this);
		avatar.Load();
		shapeCursor = new PlayerShapeCursor(this, universe.painter);
		shapeCursor.Load();
		camera = new PlayerCamera(this);
		universe.scene.cameras.SetActive(PlayerCamera.CAMERA_NAME);
	}

	public void Update(GameTime oGameTime)
	{
		if (paused)
		{
			return;
		}
		if (universe.mode == BuildUniverse.Modes.Edit)
		{
			if (atomsDefferedDeselect)
			{
				universe.atoms.Select_Deselect();
				atomsDefferedDeselect = false;
			}
			if (atomsMoving && moving)
			{
				Position_Update_Atoms(oGameTime);
			}
			else if (selecting || deselecting)
			{
				Position_Update_Selecting(oGameTime);
			}
			else if (moving)
			{
				Position_Update(oGameTime);
			}
		}
		else if (universe.mode == BuildUniverse.Modes.Add)
		{
			Position_Update_AtomCursor(oGameTime);
			shapeCursor.Update(oGameTime);
		}
		BrushesScroll_Update(oGameTime);
		avatar.Update(oGameTime);
		camera.Update(oGameTime);
	}

	public void Dispose()
	{
		avatar.Dispose();
	}

	private Vector3 GetAxis(float xX, float xY)
	{
		Vector3 result = default(Vector3);
		if (camera.axis.X == 1 || camera.axis.Y == 1)
		{
			if (Math.Abs(xY) >= Math.Abs(xX))
			{
				if (camera.axis.X == 0 || camera.axis.Y == 0)
				{
					result.X = Math.Sign(xY);
				}
				else
				{
					result.Z = Math.Sign(xY);
				}
			}
			else
			{
				result.Y = Math.Sign(xX);
			}
		}
		else if (Math.Abs(xY) >= Math.Abs(xX))
		{
			if (camera.axis.Y == 2)
			{
				result.X = Math.Sign(xY);
			}
			else
			{
				result.Z = Math.Sign(xY);
			}
		}
		else if (camera.axis.X == 2)
		{
			result.X = Math.Sign(xX);
		}
		else
		{
			result.Z = Math.Sign(xX);
		}
		return result;
	}

	private void Move(Vector2 vDir, int elapsed)
	{
		moving = true;
		DataManager.local.settings.InversionMove(ref vDir);
		if (camera.axis.X == 1 || camera.axis.Y == 1)
		{
			move.Y += vDir.Y * (0.1f + (float)moveCount * 0.05f) * (float)elapsed;
			if (camera.axis.X == 0 || camera.axis.Y == 0)
			{
				move.X += vDir.X * (0.1f + (float)moveCount * 0.05f) * (float)elapsed;
			}
			else
			{
				move.Z += vDir.X * (0.1f + (float)moveCount * 0.05f) * (float)elapsed;
			}
		}
		else
		{
			move.X += vDir.X * (0.1f + (float)moveCount * 0.05f) * (float)elapsed;
			move.Z += vDir.Y * (0.1f + (float)moveCount * 0.05f) * (float)elapsed * -1f;
		}
	}

	private void MoveTransform(ref Vector3 vIn, ref Vector3 vOut)
	{
		Quaternion quaternion = default(Quaternion);
		if (camera.axis.X == 1 || camera.axis.Y == 1)
		{
			if (camera.axis.X == 0 || camera.axis.Y == 0)
			{
				vOut.X = vIn.X * (float)Math.Sign(camera.dot.X);
				vOut.Y = vIn.Y;
				vOut.Z = 0f;
			}
			else
			{
				vOut.X = 0f;
				vOut.Y = vIn.Y;
				vOut.Z = vIn.Z * (float)Math.Sign(camera.dot.Z);
			}
		}
		else
		{
			quaternion = Quaternion.CreateFromAxisAngle(Vector3.Up, camera.yaw);
			vOut = Vector3.Transform(vIn, quaternion);
		}
	}

	private void MoveRevert()
	{
		position = movePositionPrevious;
		moveCount = 0;
	}

	private void MoveStop()
	{
		moving = false;
		move.X = 0f;
		move.Y = 0f;
		move.Z = 0f;
		moveCount = 0;
		point.FromPosition(position);
		point.ToPosition(ref _position);
	}

	public GridPoint Position_Update(GameTime oGameTime)
	{
		if (moveSnapMode == MoveSnapMode.SNAPPING)
		{
			return Position_Update_Snapping(oGameTime);
		}
		return Position_Update_Free(oGameTime);
	}

	public GridPoint Position_Update_Free(GameTime oGameTime)
	{
		Vector3 vOut = default(Vector3);
		GridPoint gridPoint = new GridPoint();
		GridPoint gridPoint2 = new GridPoint();
		movePositionPrevious = position;
		gridPoint.FromPosition(position);
		MoveTransform(ref move, ref vOut);
		X += vOut.X;
		Y += vOut.Y;
		Z += vOut.Z;
		X = MathHelper.Clamp(X, (float)universe.grid.fromX * Grid.SPACING.X, (float)universe.grid.toX * Grid.SPACING.X);
		Y = MathHelper.Clamp(Y, (float)universe.grid.fromY * Grid.SPACING.Y, (float)universe.grid.toY * Grid.SPACING.Y);
		Z = MathHelper.Clamp(Z, (float)universe.grid.fromZ * Grid.SPACING.Z, (float)universe.grid.toZ * Grid.SPACING.Z);
		move.X = 0f;
		move.Y = 0f;
		move.Z = 0f;
		gridPoint2.FromPosition(position);
		gridPoint.X = gridPoint2.X - gridPoint.X;
		gridPoint.Y = gridPoint2.Y - gridPoint.Y;
		gridPoint.Z = gridPoint2.Z - gridPoint.Z;
		if (gridPoint.X != 0 || gridPoint.Y != 0 || gridPoint.Z != 0)
		{
			moveCount++;
			moveCount = Math.Min(6, moveCount);
			universe.scene.audio.EventCues_Trigger("Build Snap");
		}
		return gridPoint;
	}

	public GridPoint Position_Update_Snapping(GameTime oGameTime)
	{
		Vector3 vOut = default(Vector3);
		GridPoint gridPoint = new GridPoint();
		GridPoint gridPoint2 = new GridPoint();
		movePositionPrevious = movePosition;
		gridPoint.FromPosition(movePosition);
		MoveTransform(ref move, ref vOut);
		movePosition.X += vOut.X;
		movePosition.Y += vOut.Y;
		movePosition.Z += vOut.Z;
		movePosition.X = MathHelper.Clamp(movePosition.X, (float)universe.grid.fromX * Grid.SPACING.X, (float)universe.grid.toX * Grid.SPACING.X);
		movePosition.Y = MathHelper.Clamp(movePosition.Y, (float)universe.grid.fromY * Grid.SPACING.Y, (float)universe.grid.toY * Grid.SPACING.Y);
		movePosition.Z = MathHelper.Clamp(movePosition.Z, (float)universe.grid.fromZ * Grid.SPACING.Z, (float)universe.grid.toZ * Grid.SPACING.Z);
		move.X = 0f;
		move.Y = 0f;
		move.Z = 0f;
		gridPoint2.FromPosition(position);
		gridPoint.X = gridPoint2.X - gridPoint.X;
		gridPoint.Y = gridPoint2.Y - gridPoint.Y;
		gridPoint.Z = gridPoint2.Z - gridPoint.Z;
		if (gridPoint.X != 0 || gridPoint.Y != 0 || gridPoint.Z != 0)
		{
			point.FromPosition(movePosition);
			universe.scene.audio.EventCues_Trigger("Build Snap");
			moveCount++;
			moveCount = Math.Min(6, moveCount);
		}
		return gridPoint;
	}

	public void Position_Update_Selecting(GameTime oGameTime)
	{
		_ = oGameTime.ElapsedGameTime.Milliseconds;
		GridPoint gridPoint = Position_Update(oGameTime);
		if (gridPoint.X != 0 || gridPoint.Y != 0 || gridPoint.Z != 0)
		{
			point.X += gridPoint.X;
			point.Y += gridPoint.Y;
			point.Z += gridPoint.Z;
			if (selecting && !deselecting)
			{
				universe.atoms.Select_Select(point);
			}
			else if (selecting && deselecting)
			{
				universe.atoms.Select_DeselectAt(point);
			}
		}
	}

	public void Position_Update_Atoms(GameTime oGameTime)
	{
		_ = oGameTime.ElapsedGameTime.Milliseconds;
		GridPoint gridPoint = Position_Update(oGameTime);
		if (gridPoint.X != 0 || gridPoint.Y != 0 || gridPoint.Z != 0)
		{
			if (universe.atoms.Select_Move(gridPoint))
			{
				point.X += gridPoint.X;
				point.Y += gridPoint.Y;
				point.Z += gridPoint.Z;
			}
			else
			{
				universe.scene.audio.EventCues_Trigger("Menu_Wrong");
				MoveRevert();
			}
		}
	}

	public void Position_Update_AtomCursor(GameTime oGameTime)
	{
		_ = oGameTime.ElapsedGameTime.Milliseconds;
		_ = position;
		GridPoint gridPoint = Position_Update(oGameTime);
		if (gridPoint.X != 0 || gridPoint.Y != 0 || gridPoint.Z != 0)
		{
			if (universe.painter.Brushes_Move(gridPoint))
			{
				point.X += gridPoint.X;
				point.Y += gridPoint.Y;
				point.Z += gridPoint.Z;
			}
			else
			{
				MoveRevert();
			}
		}
	}

	private void Atoms_Move_Start()
	{
		if (universe.atoms.selected.Count == 0)
		{
			universe.atoms.Select_Toggle(point);
		}
		GridPoint gridPoint = universe.atoms.Select_GetCenter();
		if (gridPoint != null)
		{
			point.X = gridPoint.X;
			point.Y = gridPoint.Y;
			point.Z = gridPoint.Z;
			atomsMoving = true;
			universe.atoms.Event_Select_ChangeStart();
		}
	}

	private void Atoms_Move_End()
	{
		atomsMoving = false;
		universe.atoms.Event_Select_ChangeEnd();
	}

	public void Atoms_Cursor_Change(GridPoint oPoint)
	{
		point.X = oPoint.X;
		point.Y = oPoint.Y;
		point.Z = oPoint.Z;
	}

	private void Atoms_Rotate(Vector2 vDir)
	{
		rotated = true;
		DataManager.local.settings.InversionMove(ref vDir);
		if (universe.mode == BuildUniverse.Modes.Edit)
		{
			if (universe.atoms.selected.Count == 0)
			{
				universe.atoms.Select_Toggle(point);
			}
			universe.scene.audio.EventCues_Trigger("Build Move");
			universe.atoms.Event_Select_ChangeStart();
			if (!universe.atoms.Select_Rotate(point, GetAxis(vDir.X, vDir.Y)))
			{
				universe.scene.audio.EventCues_Trigger("Menu_Wrong");
			}
			universe.atoms.Event_Select_ChangeEnd();
		}
		else
		{
			universe.painter.Brushes_Rotate(GetAxis(vDir.X, vDir.Y));
		}
	}

	private void Atoms_Flip(int xX, int xY)
	{
		universe.scene.audio.EventCues_Trigger("Build Other");
		universe.atoms.Flip_Start(GetAxis(xX, xY));
	}

	private void Atoms_Properties()
	{
		universe.scene.audio.EventCues_Trigger("Build Other");
		universe.scene.dialogs.Show("AtomPropertyMenu");
	}

	private void BrushesScroll_Set(int xDir)
	{
		universe.painter.Brushes_Next(xDir);
		brushesScrollWaiting = true;
		brushesScrollScrolling = false;
		brushesScrollTime = 0f;
		brushesScrollDir = xDir;
	}

	private void BrushesScroll_Stop()
	{
		brushesScrollWaiting = false;
		brushesScrollScrolling = false;
		brushesScrollTime = 0f;
		brushesScrollDir = 0;
	}

	private void BrushesScroll_Start()
	{
		universe.painter.Brushes_Next(brushesScrollDir);
		brushesScrollWaiting = false;
		brushesScrollScrolling = true;
		brushesScrollTime = 0f;
	}

	private void BrushesScroll_Update(GameTime oGameTime)
	{
		if (universe.mode == BuildUniverse.Modes.Add)
		{
			if (brushesScrollWaiting)
			{
				brushesScrollTime += (float)oGameTime.ElapsedGameTime.TotalMilliseconds;
				if (brushesScrollTime >= 400f)
				{
					BrushesScroll_Start();
				}
			}
			else if (brushesScrollScrolling)
			{
				brushesScrollTime += (float)oGameTime.ElapsedGameTime.TotalMilliseconds;
				if (brushesScrollTime >= 100f)
				{
					brushesScrollTime %= 100f;
					universe.painter.Brushes_Next(brushesScrollDir);
				}
			}
		}
		else if (brushesScrollWaiting || brushesScrollScrolling)
		{
			brushesScrollScrolling = false;
			brushesScrollWaiting = false;
		}
	}

	public static void Input_Set()
	{
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "Menu", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["Menu"].Add(new InputButton(GamePadButton.Start));
		UniversalInput.inputEntities["Menu"].Add(new InputButton(Keys.Enter));
		UniversalInput.inputEntities["Menu"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "Help", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["Help"].Add(new InputButton(GamePadButton.Back));
		UniversalInput.inputEntities["Help"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "ShoulderLeft", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["ShoulderLeft"].Add(new InputButton(GamePadButton.ShoulderLeft));
		UniversalInput.inputEntities["ShoulderLeft"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "ShoulderRight", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["ShoulderRight"].Add(new InputButton(GamePadButton.ShoulderRight));
		UniversalInput.inputEntities["ShoulderRight"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Analog2D, "PlayerMove", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["PlayerMove"].Add(new InputAnalog2D(GamePadAnalog2D.Left));
		UniversalInput.inputEntities["PlayerMove"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "MoveMode", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["MoveMode"].Add(new InputButton(GamePadButton.AnalogLeft));
		UniversalInput.inputEntities["MoveMode"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "DPadUp", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["DPadUp"].Add(new InputButton(GamePadButton.Up));
		UniversalInput.inputEntities["DPadUp"].Add(new InputButton(Keys.PageUp));
		UniversalInput.inputEntities["DPadUp"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "DPadDown", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["DPadDown"].Add(new InputButton(GamePadButton.Down));
		UniversalInput.inputEntities["DPadDown"].Add(new InputButton(Keys.PageDown));
		UniversalInput.inputEntities["DPadDown"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "DPadLeft", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["DPadLeft"].Add(new InputButton(GamePadButton.Left));
		UniversalInput.inputEntities["DPadLeft"].Add(new InputButton(Keys.Insert));
		UniversalInput.inputEntities["DPadLeft"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "DPadRight", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["DPadRight"].Add(new InputButton(GamePadButton.Right));
		UniversalInput.inputEntities["DPadRight"].Add(new InputButton(Keys.Delete));
		UniversalInput.inputEntities["DPadRight"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "ButtonA", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["ButtonA"].Add(new InputButton(GamePadButton.A));
		UniversalInput.inputEntities["ButtonA"].Add(new InputButton(Keys.Z));
		UniversalInput.inputEntities["ButtonA"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "ButtonB", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["ButtonB"].Add(new InputButton(GamePadButton.B));
		UniversalInput.inputEntities["ButtonB"].Add(new InputButton(Keys.X));
		UniversalInput.inputEntities["ButtonB"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "ButtonX", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["ButtonX"].Add(new InputButton(GamePadButton.X));
		UniversalInput.inputEntities["ButtonX"].Add(new InputButton(Keys.C));
		UniversalInput.inputEntities["ButtonX"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "ButtonY", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["ButtonY"].Add(new InputButton(GamePadButton.Y));
		UniversalInput.inputEntities["ButtonY"].Add(new InputButton(Keys.V));
		UniversalInput.inputEntities["ButtonY"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Analog2D, "PlayerCam", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["PlayerCam"].Add(new InputAnalog2D(GamePadAnalog2D.Right));
		UniversalInput.inputEntities["PlayerCam"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Button, "PlayerCamToggle", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["PlayerCamToggle"].Add(new InputButton(GamePadButton.AnalogRight));
		UniversalInput.inputEntities["PlayerCamToggle"].Add(new InputButton(Keys.RightControl));
		UniversalInput.inputEntities["PlayerCamToggle"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Analog1D, "PlayerTriggerLeft", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["PlayerTriggerLeft"].Add(new InputAnalog1D(GamePadAnalog1D.Left));
		UniversalInput.inputEntities["PlayerTriggerLeft"].active = true;
		UniversalInput.InputEntity_Add(new InputEntity(InputEntity.Type.Analog1D, "PlayerTriggerRight", InputEntity.Scope.Scene));
		UniversalInput.inputEntities["PlayerTriggerRight"].Add(new InputAnalog1D(GamePadAnalog1D.Right));
		UniversalInput.inputEntities["PlayerTriggerRight"].active = true;
	}

	public void Input_Update(GameTime oGameTime)
	{
		int milliseconds = oGameTime.ElapsedGameTime.Milliseconds;
		if (inputPaused)
		{
			return;
		}
		if (universe.mode == BuildUniverse.Modes.Add || universe.mode == BuildUniverse.Modes.Edit)
		{
			if (UniversalInput.inputEntities["Menu"].pressed)
			{
				universe.scene.dialogs.Show("Build");
			}
			if (UniversalInput.inputEntities["Help"].pressed)
			{
				universe.scene.dialogs.Show("BuildHelp");
			}
			if (UniversalInput.inputEntities["DPadUp"].downed)
			{
				Atoms_Flip(0, -1);
			}
			if (UniversalInput.inputEntities["DPadDown"].downed)
			{
				Atoms_Flip(0, 1);
			}
			if (UniversalInput.inputEntities["DPadLeft"].downed)
			{
				Atoms_Flip(-1, 0);
			}
			if (UniversalInput.inputEntities["DPadRight"].downed)
			{
				Atoms_Flip(1, 0);
			}
			atomsRotate = UniversalInput.inputEntities["ButtonX"].isDown;
			switch (universe.mode)
			{
			case BuildUniverse.Modes.Edit:
				Input_Update_Edit(milliseconds);
				break;
			case BuildUniverse.Modes.Add:
				Input_Update_Add(milliseconds);
				break;
			}
			Input_Update_Move(milliseconds);
		}
		camera.Input_Update(milliseconds);
	}

	private void Input_Update_Add(int elapsed)
	{
		if (UniversalInput.inputEntities["ButtonY"].downed)
		{
			UniversalInput.inputEntities["ButtonY"].downed = false;
			universe.Modes_SetEdit();
			universe.scene.audio.EventCues_Trigger("Build Move");
		}
		else if (UniversalInput.inputEntities["PlayerTriggerLeft"].value1D >= 0.3f && UniversalInput.inputEntities["PlayerTriggerLeft"].previous1D < 0.3f)
		{
			BrushesScroll_Set(-1);
		}
		else if (UniversalInput.inputEntities["PlayerTriggerRight"].value1D >= 0.3f && UniversalInput.inputEntities["PlayerTriggerRight"].previous1D < 0.3f)
		{
			BrushesScroll_Set(1);
		}
		else if ((UniversalInput.inputEntities["PlayerTriggerLeft"].value1D < 0.3f && UniversalInput.inputEntities["PlayerTriggerLeft"].previous1D >= 0.3f) || (UniversalInput.inputEntities["PlayerTriggerRight"].value1D < 0.3f && UniversalInput.inputEntities["PlayerTriggerRight"].previous1D >= 0.3f))
		{
			BrushesScroll_Stop();
		}
		if (UniversalInput.inputEntities["ButtonA"].downed)
		{
			universe.painter.Atoms_Add(point);
		}
		if (UniversalInput.inputEntities["ButtonB"].downed)
		{
			universe.painter.Undo();
		}
	}

	private void Input_Update_Edit(int elapsed)
	{
		if (UniversalInput.inputEntities["ButtonY"].downed)
		{
			UniversalInput.inputEntities["ButtonY"].downed = false;
			universe.Modes_SetAdd();
			universe.scene.audio.EventCues_Trigger("Build Move");
		}
		if (UniversalInput.inputEntities["PlayerTriggerRight"].value1D >= 0.8f && UniversalInput.inputEntities["PlayerTriggerRight"].previous1D < 0.8f)
		{
			if (!selecting && !deselecting)
			{
				if (universe.atoms.selected.Count > 0)
				{
					universe.atoms.Select_Deselect();
				}
				else
				{
					universe.atoms.Select_SelectAll();
				}
			}
			else if (selecting)
			{
				deselecting = true;
				universe.atoms.Select_Toggle(avatar.point);
			}
		}
		else if (UniversalInput.inputEntities["PlayerTriggerRight"].value1D < 0.8f && UniversalInput.inputEntities["PlayerTriggerRight"].previous1D >= 0.8f && deselecting)
		{
			deselecting = false;
		}
		if (UniversalInput.inputEntities["PlayerTriggerLeft"].value1D >= 0.8f && UniversalInput.inputEntities["PlayerTriggerLeft"].previous1D < 0.8f)
		{
			selecting = true;
			deselecting = false;
			universe.atoms.Select_Select(avatar.point);
		}
		else if (UniversalInput.inputEntities["PlayerTriggerLeft"].value1D < 0.8f && UniversalInput.inputEntities["PlayerTriggerLeft"].previous1D >= 0.8f)
		{
			selecting = false;
			deselecting = false;
		}
		if (UniversalInput.inputEntities["ShoulderRight"].downed)
		{
			Atoms_Properties();
		}
		if (UniversalInput.inputEntities["ButtonA"].downed && !atomsMoving)
		{
			Atoms_Move_Start();
		}
		else if (UniversalInput.inputEntities["ButtonA"].pressed && atomsMoving)
		{
			Atoms_Move_End();
		}
		if (UniversalInput.inputEntities["ButtonB"].downed && (universe.atoms.selected.Count > 0 || universe.atoms.over != null))
		{
			universe.scene.dialogs.Show("DeleteConfirm");
		}
	}

	private void Input_Update_Move(int elapsed)
	{
		if (UniversalInput.inputEntities["MoveMode"].downed && moveMode != MoveMode.CAMERA)
		{
			if (moveMode == MoveMode.XZ)
			{
				moveMode = MoveMode.Y;
				camera.yaw += 1E-06f;
			}
			else
			{
				moveMode = MoveMode.XZ;
				camera.yaw += 1E-06f;
			}
		}
		if (UniversalInput.inputEntities["ShoulderLeft"].downed)
		{
			universe.scene.audio.EventCues_Trigger("Build Axis Change");
			moveMode = MoveMode.Y;
			camera.yaw += 1E-06f;
		}
		if (UniversalInput.inputEntities["ShoulderLeft"].pressed)
		{
			universe.scene.audio.EventCues_Trigger("Build Axis Change");
			moveMode = MoveMode.XZ;
			camera.yaw += 1E-06f;
		}
		if (atomsRotate)
		{
			if (!selecting && !rotated && (Math.Abs(UniversalInput.inputEntities["PlayerMove"].value2D.X) > 0.5f || Math.Abs(UniversalInput.inputEntities["PlayerMove"].value2D.Y) > 0.5f))
			{
				Atoms_Rotate(UniversalInput.inputEntities["PlayerMove"].value2D);
			}
		}
		else if (Math.Abs(UniversalInput.inputEntities["PlayerMove"].value2D.X) > 0.1f || Math.Abs(UniversalInput.inputEntities["PlayerMove"].value2D.Y) > 0.1f)
		{
			Move(UniversalInput.inputEntities["PlayerMove"].value2D, elapsed);
		}
		else if (moving)
		{
			MoveStop();
		}
		if (rotated && Math.Abs(UniversalInput.inputEntities["PlayerMove"].value2D.X) < 0.1f && Math.Abs(UniversalInput.inputEntities["PlayerMove"].value2D.Y) < 0.1f)
		{
			rotated = false;
		}
	}

	public void Event_LevelLoaded()
	{
	}

	public void Event_LevelLoad()
	{
	}
}
