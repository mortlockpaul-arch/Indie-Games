# Legacy Method Scan Results - All Projects

## Summary
Found **10 FrameRateCounter.cs files** across projects. **Only 1 has the broken legacy method.**

| Project | Status | Issue |
|---------|--------|-------|
| Murder Miners ✓ | OK | Uses `Update()` and `Draw()` - CORRECT |
| 20 Games to Play with Your Mates ✓ | OK | Uses `LoadContent()` - CORRECT |
| Allure ✓ | OK | Uses `LoadContent()` - CORRECT |
| Comedy Night ✓ | OK | Uses `LoadContent()` - CORRECT |
| Demon House FPS ✓ | OK | Uses `LoadContent()` - CORRECT |
| Zombie Estate ✓ | OK | Uses `LoadContent()` - CORRECT |
| Storage Inc ❌ | BROKEN | Uses `LoadGraphicsContent(bool)` - LEGACY METHOD |

---

## The One Problem: Storage Inc

**File**: `Storage Inc (World) (XBLIG)\PhysCoop\FrameRateCounter.cs`

**Issue**: Uses legacy methods `LoadGraphicsContent(bool)` and `UnloadGraphicsContent(bool)` that don't exist in modern FNA

```csharp
// ❌ WRONG - These methods don't exist in FNA
protected override void LoadGraphicsContent(bool loadAllContent)
{
	if (loadAllContent)
	{
		spriteBatch = new SpriteBatch(GraphicsDevice);
		spriteFont = content.Load<SpriteFont>("Content\\ScreenManagerResources\\FrameRateFont");
	}
}

protected override void UnloadGraphicsContent(bool unloadAllContent)
{
	if (unloadAllContent)
	{
		content.Unload();
	}
}
```

---

## The Fix

Replace the legacy methods with modern equivalents:

```csharp
// ✓ CORRECT - Use modern FNA methods
protected override void LoadContent()
{
	spriteBatch = new SpriteBatch(GraphicsDevice);
	spriteFont = content.Load<SpriteFont>("Content\\ScreenManagerResources\\FrameRateFont");
}

protected override void UnloadContent()
{
	content.Unload();
}
```

**Changes**:
- `LoadGraphicsContent(bool loadAllContent)` → `LoadContent()`
- `UnloadGraphicsContent(bool unloadAllContent)` → `UnloadContent()`
- Remove the `if` condition checks (these are always called in modern FNA)

---

## All Projects Status

### ✓ Murder Miners (XBLIG) - CLEAN
```csharp
public class FrameRateCounter : DrawableGameComponent
{
	public override void Update(GameTime gameTime) { ... }
	public override void Draw(GameTime gameTime) { ... }
}
```
**Status**: Uses modern methods - NO ISSUES

---

### ✓ 20 Games to Play with Your Mates - CLEAN
```csharp
public class FrameRateCounter : DrawableGameComponent
{
	protected override void LoadContent() { ... }
}
```
**Status**: Uses modern methods - NO ISSUES

---

### ✓ Allure (World) (XBLIG) - CLEAN
```csharp
public class FrameRateCounter : DrawableGameComponent
{
	protected override void LoadContent() { ... }
	protected override void UnloadContent() { ... }
}
```
**Status**: Uses modern methods - NO ISSUES

---

### ✓ Comedy Night (World) (XBLIG) - CLEAN
```csharp
public class FrameRateCounter : DrawableGameComponent
{
	protected override void LoadContent() { ... }
}
```
**Status**: Uses modern methods - NO ISSUES

---

### ✓ Demon House FPS (XBLIG) - CLEAN
```csharp
public class FrameRateCounter : DrawableGameComponent
{
	protected override void LoadContent() { ... }
	protected override void UnloadContent() { ... }
}
```
**Status**: Uses modern methods - NO ISSUES

---

### ✓ Zombie Estate (World) (XBLIG) - CLEAN
```csharp
public class FrameRateCounter : DrawableGameComponent
{
	protected override void LoadContent() { ... }
	protected override void UnloadContent() { ... }
}
```
**Status**: Uses modern methods - NO ISSUES

---

### ❌ Storage Inc (World) (XBLIG) - BROKEN
```csharp
public class FrameRateCounter : DrawableGameComponent
{
	protected override void LoadGraphicsContent(bool loadAllContent) { ... }  // ❌ LEGACY
	protected override void UnloadGraphicsContent(bool unloadAllContent) { ... }  // ❌ LEGACY
}
```
**Status**: Uses LEGACY methods that don't exist in FNA - NEEDS FIX

---

## Summary

**Good News:**
- 9 out of 10 projects are clean and use modern FNA methods
- Only 1 project (Storage Inc) has the legacy method issue
- It's a simple fix (just remove the method parameters)

**Action Required:**
- Fix Storage Inc's FrameRateCounter.cs

---

## Other Legacy Methods to Check

Besides `LoadGraphicsContent` and `UnloadGraphicsContent`, there are other legacy methods that might cause issues:

| Legacy Method | Modern Replacement |
|---|---|
| `LoadGraphicsContent(bool)` | `LoadContent()` |
| `UnloadGraphicsContent(bool)` | `UnloadContent()` |
| `OnDeviceReset()` | `LoadContent()` |
| `OnDeviceLost()` | `UnloadContent()` |

**All 9 other projects are using the modern methods, so they should be fine.**

