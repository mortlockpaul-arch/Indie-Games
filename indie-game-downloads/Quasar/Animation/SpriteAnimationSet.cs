using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Quasar.ContentPipeline;
using Quasar.Global;

namespace Quasar.Animation;

public class SpriteAnimationSet : IDisposable
{
	public const string ANIMATION_DIR = "SpriteAnims/";

	private Dictionary<string, SpriteAnimation> animations;

	private int gridSizeW = 1;

	private int gridSizeH = 1;

	private SpriteAnimation defaultAnimation;

	public int GridSizeW
	{
		get
		{
			return gridSizeW;
		}
		set
		{
			gridSizeW = value;
		}
	}

	public int GridSizeH
	{
		get
		{
			return gridSizeH;
		}
		set
		{
			gridSizeH = value;
		}
	}

	public int AnimationCount => animations.Keys.Count;

	public SpriteAnimation DefaultAnimation => defaultAnimation;

	public SpriteAnimationSet()
	{
		animations = new Dictionary<string, SpriteAnimation>();
	}

	public SpriteAnimation GetAnimation(string name)
	{
		animations.TryGetValue(name, out var value);
		return value;
	}

	public SpriteAnimation GetFirstAnimation()
	{
		return animations.First().Value;
	}

	public static SpriteAnimationSet Load(string name)
	{
		XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>("SpriteAnims/" + name);
		XDocument xDocument = XDocument.Parse(xmlSource.XmlCode);
		SpriteAnimationSet spriteAnimationSet = new SpriteAnimationSet();
		spriteAnimationSet.gridSizeW = XDocHelper.ParseIntAttribute(xDocument.Root, "cols");
		spriteAnimationSet.gridSizeH = XDocHelper.ParseIntAttribute(xDocument.Root, "rows");
		foreach (XElement item in xDocument.Root.Elements("Animation"))
		{
			SpriteAnimation spriteAnimation = SpriteAnimation.Parse(item);
			if (spriteAnimation != null)
			{
				spriteAnimationSet.addAnimation(spriteAnimation);
			}
		}
		spriteAnimationSet.setDefaultAnimation(XDocHelper.GetAttribute(xDocument.Root, "default"));
		spriteAnimationSet.updateNextAnimations();
		return spriteAnimationSet;
	}

	public void setDefaultAnimation(string defaultAnimation)
	{
		this.defaultAnimation = GetAnimation(defaultAnimation);
	}

	public void addAnimation(SpriteAnimation animation)
	{
		animations.Add(animation.Id, animation);
	}

	private void updateNextAnimations()
	{
		foreach (SpriteAnimation value in animations.Values)
		{
			value.setNextAnimation(GetAnimation(value.NextId));
		}
	}

	public void removeAnimation(string name)
	{
		animations.Remove(name);
	}

	public void Dispose()
	{
	}
}
