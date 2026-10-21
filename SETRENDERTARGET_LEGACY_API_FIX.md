# SetRenderTarget Legacy API Migration - GameplayScreen Fix

## Issue
```
CS1503: Argument 2: cannot convert from 'RenderTarget2D' to 'CubeMapFace'
File: Storage Inc (World) (XBLIG)/PhysCoop/GameplayScreen.cs:399
```

## Root Cause
**Legacy XNA API**: The old `SetRenderTarget(int index, RenderTarget2D target)` method signature doesn't exist in modern FNA. This was an old multi-render-target API from XNA that supported up to 4 render targets via an index parameter.

### What was wrong:
```csharp
// LEGACY XNA API (doesn't work in modern FNA)
PhysCoop.graphics.GraphicsDevice.SetRenderTarget(0, shadingTarget);   // Line 399
PhysCoop.graphics.GraphicsDevice.SetRenderTarget(0, null);             // Line 419
PhysCoop.graphics.GraphicsDevice.SetRenderTarget(0, shadowTarget);     // Line 436
PhysCoop.graphics.GraphicsDevice.SetRenderTarget(0, null);             // Line 439
```

## Solution Applied
✅ **Updated to modern FNA API**

### Changes Made:

**Modern FNA API** - Single render target (no index parameter):
```csharp
// MODERN FNA API (correct for FNA)
PhysCoop.graphics.GraphicsDevice.SetRenderTarget(shadingTarget);
PhysCoop.graphics.GraphicsDevice.SetRenderTarget(null);
PhysCoop.graphics.GraphicsDevice.SetRenderTarget(shadowTarget);
PhysCoop.graphics.GraphicsDevice.SetRenderTarget(null);
```

## Fixed Locations

| Line | Method | Before | After |
|------|--------|--------|-------|
| 399 | AddLight | `SetRenderTarget(0, shadingTarget)` | `SetRenderTarget(shadingTarget)` |
| 419 | AddLight | `SetRenderTarget(0, null)` | `SetRenderTarget(null)` |
| 436 | RenderShadowMap | `SetRenderTarget(0, shadowTarget)` | `SetRenderTarget(shadowTarget)` |
| 439 | RenderShadowMap | `SetRenderTarget(0, null)` | `SetRenderTarget(null)` |

## Why This Works

**Old XNA (Multiple Render Targets)**:
- Supported up to 4 render targets simultaneously
- Required index parameter to specify which target
- Syntax: `SetRenderTarget(int index, RenderTarget2D target)`

**Modern FNA (Single Render Target)**:
- Simplified to support only 1 render target at a time
- Index parameter removed
- Syntax: `SetRenderTarget(RenderTarget2D target)`

**Migration Strategy**:
- If you had multi-target rendering (MRT), you need to:
  - Render to target 0, then composite results
  - OR use pixel shader techniques without MRT
- In this case, Storage Inc uses deferred rendering with single render targets

## File Modified
- `Storage Inc (World) (XBLIG)/PhysCoop/GameplayScreen.cs`

## Status
✅ FIXED - All 4 SetRenderTarget calls migrated to modern API

---

## Related Legacy Removals in Storage Inc

Summary of all legacy API fixes applied:
1. ✅ `LoadGraphicsContent(bool)` → `LoadContent()`
2. ✅ `UnloadGraphicsContent(bool)` → `UnloadContent()`
3. ✅ `ResolveTexture2D` → Removed (unused)
4. ✅ `SpriteBlendMode.None` → Removed from `Begin()`
5. ✅ `SetRenderTarget(int, RenderTarget2D)` → `SetRenderTarget(RenderTarget2D)`

All critical API modernization fixes for Storage Inc are now complete.
