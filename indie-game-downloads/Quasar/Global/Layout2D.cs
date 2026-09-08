using System;
using Microsoft.Xna.Framework;

namespace Quasar.Global;

public static class Layout2D
{
	public struct LayoutData
	{
		public Vector2 position;

		public Vector2 size;

		public float Height => size.Y;

		public float Width => size.X;

		public Vector2 Top => new Vector2(position.X, position.Y + size.Y * 0.5f);

		public Vector2 Bottom => new Vector2(position.X, position.Y - size.Y * 0.5f);

		public Vector2 Left => new Vector2(position.X - size.X * 0.5f, position.Y);

		public Vector2 Right => new Vector2(position.X + size.X * 0.5f, position.Y);

		public Vector2 TopLeft => new Vector2(position.X - size.X * 0.5f, position.Y + size.Y * 0.5f);

		public Vector2 TopRight => new Vector2(position.X + size.X * 0.5f, position.Y + size.Y * 0.5f);

		public Vector2 BottomLeft => new Vector2(position.X - size.X * 0.5f, position.Y - size.Y * 0.5f);

		public Vector2 BottomRight => new Vector2(position.X + size.X * 0.5f, position.Y - size.Y * 0.5f);

		public LayoutData(Vector2 center, Vector2 size)
		{
			position = center;
			this.size = size;
		}

		public LayoutData(Vector2 size)
		{
			position = Vector2.Zero;
			this.size = size;
		}
	}

	public static void InsideBorderLayout(LayoutData area, float border, out LayoutData result)
	{
		result = new LayoutData(area.position, new Vector2(area.size.X - 2f * border, area.size.Y - 2f * border));
	}

	public static void InsideVerticalBorderLayout(LayoutData area, float border, out LayoutData result)
	{
		result = new LayoutData(area.position, new Vector2(area.size.X, area.size.Y - 2f * border));
	}

	public static void InsideHorizontalBorderLayout(LayoutData area, float border, out LayoutData result)
	{
		result = new LayoutData(area.position, new Vector2(area.size.X - 2f * border, area.size.Y));
	}

	public static void OutsideBorderLayout(LayoutData area, float border, out LayoutData result)
	{
		result = new LayoutData(area.position, new Vector2(area.size.X + 2f * border, area.size.Y + 2f * border));
	}

	public static void HorizontalFixedFloatLayout(LayoutData container, float fixedWidth, float margin, out LayoutData fixedLayout, out LayoutData floatLayout)
	{
		fixedLayout = new LayoutData(new Vector2(container.position.X - container.size.X * 0.5f + fixedWidth * 0.5f, container.position.Y), new Vector2(fixedWidth, container.size.Y));
		float num = container.size.X - fixedWidth - margin;
		floatLayout = new LayoutData(new Vector2(container.position.X - container.size.X * 0.5f + fixedWidth + margin + num * 0.5f, container.position.Y), new Vector2(num, container.size.Y));
	}

	public static void HorizontalRelativeSizeLayout(LayoutData container, float leftRelativeSize, float margin, out LayoutData leftLayout, out LayoutData rightLayout)
	{
		float num = leftRelativeSize * (container.size.X - margin);
		leftLayout = new LayoutData(new Vector2(container.position.X - container.size.X * 0.5f + num * 0.5f, container.position.Y), new Vector2(num, container.size.Y));
		float num2 = (1f - leftRelativeSize) * (container.size.X - margin);
		rightLayout = new LayoutData(new Vector2(container.position.X + container.size.X * 0.5f - num2 * 0.5f, container.position.Y), new Vector2(num2, container.size.Y));
	}

	public static void HorizontalFloatFixedLayout(LayoutData container, float fixedWidth, float margin, out LayoutData fixedLayout, out LayoutData floatLayout)
	{
		fixedLayout = new LayoutData(new Vector2(container.position.X + container.size.X * 0.5f - fixedWidth * 0.5f, container.position.Y), new Vector2(fixedWidth, container.size.Y));
		float num = container.size.X - fixedWidth - margin;
		floatLayout = new LayoutData(new Vector2(container.position.X + container.size.X * 0.5f - fixedWidth - margin - num * 0.5f, container.position.Y), new Vector2(num, container.size.Y));
	}

	public static void VerticalRelativeSizeLayout(LayoutData container, float topRelativeSize, float margin, out LayoutData topLayout, out LayoutData bottomLayout)
	{
		float num = topRelativeSize * (container.size.Y - margin);
		topLayout = new LayoutData(new Vector2(container.position.X, container.position.Y + container.size.Y * 0.5f - num * 0.5f), new Vector2(container.size.X, num));
		float num2 = (1f - topRelativeSize) * (container.size.Y - margin);
		bottomLayout = new LayoutData(new Vector2(container.position.X, container.position.Y - container.size.Y * 0.5f + num2 * 0.5f), new Vector2(container.size.X, num2));
	}

	public static void VerticalFixedFloatLayout(LayoutData container, float fixedHeight, float margin, out LayoutData fixedLayout, out LayoutData floatLayout)
	{
		fixedLayout = new LayoutData(new Vector2(container.position.X, container.position.Y + container.size.Y * 0.5f - fixedHeight * 0.5f), new Vector2(container.size.X, fixedHeight));
		float num = container.size.Y - fixedHeight - margin;
		floatLayout = new LayoutData(new Vector2(container.position.X, container.position.Y + container.size.Y * 0.5f - fixedHeight - margin - num * 0.5f), new Vector2(container.size.X, num));
	}

	public static void VerticalFloatFixedLayout(LayoutData container, float fixedHeight, float margin, out LayoutData fixedLayout, out LayoutData floatLayout)
	{
		fixedLayout = new LayoutData(new Vector2(container.position.X, container.position.Y - container.size.Y * 0.5f + fixedHeight * 0.5f), new Vector2(container.size.X, fixedHeight));
		float num = container.size.Y - fixedHeight - margin;
		floatLayout = new LayoutData(new Vector2(container.position.X, container.position.Y - container.size.Y * 0.5f + fixedHeight + margin + num * 0.5f), new Vector2(container.size.X, num));
	}

	public static void GridLayout(LayoutData container, int cols, int rows, float margin, int index, out LayoutData cellLayout)
	{
		cellLayout = default(LayoutData);
		float num = (container.size.X - margin * (float)(cols - 1)) / (float)cols;
		float num2 = (container.size.Y - margin * (float)(rows - 1)) / (float)rows;
		cellLayout.size.X = num;
		cellLayout.size.Y = num2;
		int num3 = index % cols;
		int num4 = index / cols;
		cellLayout.position.X = container.position.X - container.size.X * 0.5f + num * ((float)num3 + 0.5f) + (float)num3 * margin;
		cellLayout.position.Y = container.position.Y + container.size.Y * 0.5f - num2 * ((float)num4 + 0.5f) - (float)num4 * margin;
	}

	public static void KeepAspectRatio(LayoutData layout, float desiredAspectRatio, out LayoutData result)
	{
		float num = layout.size.X / layout.size.Y;
		if (num > desiredAspectRatio)
		{
			result = new LayoutData(layout.position, new Vector2(layout.size.Y * desiredAspectRatio, layout.size.Y));
		}
		else
		{
			result = new LayoutData(layout.position, new Vector2(layout.size.X, layout.size.X / desiredAspectRatio));
		}
	}

	public static void KeepPixelAlign(ref LayoutData layout)
	{
		layout.position.X = (float)Math.Floor(layout.position.X) + 0.5f;
		layout.position.Y = (float)Math.Floor(layout.position.Y) + 0.5f;
		layout.size.X = (float)Math.Floor(layout.size.X * 0.5f) * 2f;
		layout.size.Y = (float)Math.Floor(layout.size.Y * 0.5f) * 2f;
	}

	public static void GrowLeft(LayoutData layout, float sizeIncreaseLeft, out LayoutData result)
	{
		result = new LayoutData(layout.position - new Vector2(sizeIncreaseLeft * 0.5f, 0f), layout.size + new Vector2(sizeIncreaseLeft, 0f));
	}

	public static void GrowRight(LayoutData layout, float sizeIncreaseRight, out LayoutData result)
	{
		result = new LayoutData(layout.position + new Vector2(sizeIncreaseRight * 0.5f, 0f), layout.size + new Vector2(sizeIncreaseRight, 0f));
	}

	public static void GrowUp(LayoutData layout, float sizeIncreaseUp, out LayoutData result)
	{
		result = new LayoutData(layout.position + new Vector2(0f, sizeIncreaseUp * 0.5f), layout.size + new Vector2(0f, sizeIncreaseUp));
	}

	public static void GrowDown(LayoutData layout, float sizeIncreaseDown, out LayoutData result)
	{
		result = new LayoutData(layout.position - new Vector2(0f, sizeIncreaseDown * 0.5f), layout.size + new Vector2(0f, sizeIncreaseDown));
	}
}
