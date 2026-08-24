using System.Collections.Generic;
using GKEngine;
using GKEngine.Input;
using Game.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.QBits;

public class QBitConversations
{
	private const string PATH_TEXTURES = "Content/Materials/QBits/Speech/Sheet_";

	private const string PATH_TEXTURE_BACKGROUND = "Content/Materials/QBits/Speech/SpeechBackground";

	private const int WAIT_TIME = 1000;

	private QBitManager manager;

	private int speechIndex;

	private int qbitForceIndex = -1;

	private DataConversation conversation;

	private QBitSpeech speech;

	private QBitSpeech.SpeechDelegate __callback;

	public bool talking;

	public List<int> textureIndexs;

	public Texture2D[] textures;

	public Texture2D textureBackground;

	public InputEntity inputContinue;

	public QBitConversations(QBitManager pManager)
	{
		manager = pManager;
		Load();
	}

	public void Load()
	{
		textureIndexs = new List<int>();
		if (DataManager.level.conversations != null)
		{
			for (int i = 0; i < DataManager.level.conversations.Count; i++)
			{
				for (int j = 0; j < DataManager.level.conversations[i].speech.Length; j++)
				{
					if (!textureIndexs.Contains(DataManager.level.conversations[i].speech[j].sheet))
					{
						textureIndexs.Add(DataManager.level.conversations[i].speech[j].sheet);
					}
				}
			}
		}
		textures = new Texture2D[textureIndexs.Count];
		for (int i = 0; i < textureIndexs.Count; i++)
		{
			textures[i] = GameEngine.SceneContent.Load<Texture2D>("Content/Materials/QBits/Speech/Sheet_" + textureIndexs[i]);
		}
		textureBackground = GameEngine.SceneContent.Load<Texture2D>("Content/Materials/QBits/Speech/SpeechBackground");
	}

	public void Dispose()
	{
		for (int i = 0; i < textures.Length; i++)
		{
			textures[i] = null;
		}
		Input_Clear();
		textureBackground = null;
		textures = null;
		textureIndexs = null;
	}

	public void Show(int pIndex, QBitSpeech.SpeechDelegate pCompleted, int pForceQBit)
	{
		Halt();
		if (pIndex < DataManager.level.conversations.Count && DataManager.level.conversations[pIndex].speech.Length > 0)
		{
			__callback = pCompleted;
			conversation = DataManager.level.conversations[pIndex];
			qbitForceIndex = pForceQBit;
			speechIndex = 0;
			talking = true;
			SpeechStart();
		}
	}

	private void SpeechStart()
	{
		if (conversation.speech[speechIndex].qbit < manager.qbits.Count)
		{
			speech = manager.qbits[(qbitForceIndex >= 0) ? qbitForceIndex : conversation.speech[speechIndex].qbit].speech;
			speech.Show(textures[textureIndexs.IndexOf(conversation.speech[speechIndex].sheet)], textureBackground, conversation.speech[speechIndex].x, conversation.speech[speechIndex].y, 1000, Event_Speech_Ready, Event_Speech_Done);
		}
	}

	public void Halt()
	{
		if (talking)
		{
			if (speech != null)
			{
				speech.Halt();
				speech = null;
			}
			talking = false;
		}
	}

	public void Update(GameTime oGameTime)
	{
		Input_Update(oGameTime);
	}

	public void Event_Speech_Ready()
	{
		manager.universe.players.ui.YToContinue_Show();
		Input_Set();
	}

	public void Event_Speech_Done()
	{
		speechIndex++;
		if (speechIndex >= conversation.speech.Length)
		{
			talking = false;
			speech = null;
			if (__callback != null)
			{
				__callback();
			}
		}
		else
		{
			SpeechStart();
		}
	}

	private void Input_Set()
	{
		inputContinue = new InputEntity(InputEntity.Type.Button, "SpeechContinue", InputEntity.Scope.Scene);
		UniversalInput.InputEntity_Add(inputContinue);
		for (int i = 0; i < manager.universe.players.players.Length; i++)
		{
			if (manager.universe.players.players[i] != null && manager.universe.players.players[i].active)
			{
				inputContinue.Add(new InputButton(GamePadButton.Y, manager.universe.players.players[i].index));
			}
		}
		inputContinue.active = true;
	}

	private void Input_Clear()
	{
		UniversalInput.InputEntity_Remove(inputContinue);
		inputContinue = null;
	}

	private void Input_Update(GameTime oGameTime)
	{
		if (inputContinue != null && inputContinue.pressed)
		{
			manager.universe.players.ui.YToContinue_Hide();
			Input_Clear();
			speech.Hide();
		}
	}
}
