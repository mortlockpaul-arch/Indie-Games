using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class ModelManager
{
	private Dictionary<string, int> modelDict;

	private ContentManager content;

	private ModelWrapper[] storage;

	private int curIndex;

	public ModelManager(ContentManager _content)
	{
		modelDict = new Dictionary<string, int>();
		content = _content;
		storage = new ModelWrapper[300];
		curIndex = 0;
	}

	public ModelWrapper GetModel(string modelName)
	{
		return GetModel(modelName, copyData: false, copyEPC: false);
	}

	public ModelWrapper GetModel(string modelName, bool copyData, bool copyEPC)
	{
		int num;
		lock (BaseGame.Get().modelLock)
		{
			if (modelDict.ContainsKey(modelName))
			{
				num = modelDict[modelName];
			}
			else
			{
				num = curIndex;
				storage[curIndex] = new ModelWrapper(content.Load<Model>(modelName), copyData);
				storage[curIndex].GetEffectParameters(BaseGame.Get().graphics.GraphicsDevice, BaseGame.Get().fogEffect);
				modelDict.Add(modelName, curIndex);
				curIndex++;
			}
		}
		return new ModelWrapper(storage[num], copyEPC);
	}
}
