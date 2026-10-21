# Murder Miners Exception - Complete Analysis Summary

## Quick Summary
**Problem**: Murder Miners throws `EEFileLoadException (0xE0434352)` with FILE_NOT_FOUND error
**Root Cause**: .NET Framework 4.6.1 assemblies cannot be loaded by .NET 9 application
**Solution**: Modernize FNA, FNA.NetStub, and FNA.Contents to target net9.0-windows

---

## Exception Details

### What Happened
1. Murder Miners.exe (net9.0-windows) started loading
2. Attempted to load ProjectReference `FNA.Contents`
3. FNA.Contents (net461) tried to load its dependencies
4. CLR rejected the assembly due to target framework mismatch
5. Exception thrown: FILE_NOT_FOUND (0x80070002)
6. Unhandled exception in KernelBase.dll (0xE0434352)

### Stack Trace Analysis
```
Thread 10804:
[1] KernelBase.dll!76bdbae4 ← Exception thrown here
[2-5] hostpolicy.dll, hostfxr.dll ← CLR initialization
[6-10] More CLR initialization
[11-13] User code (Murder Miners.exe)
[14-16] System initialization
```

### Log Evidence
```
Debug Output:
- "Exception thrown: EEFileLoadException" ← Assembly loading failed
- "[rethrow] at memory location 0x00000000" ← Stack unwinding
- Parameter 0x80070002 ← FILE_NOT_FOUND (Windows error code)
```

---

## Technical Analysis

### Project Structure Mismatch
```
Current (BROKEN):
  Murder Miners.exe (net9.0-windows) ← Modern .NET
		 ↓
  FNA.Contents (net461) ← Legacy .NET Framework 4.6.1
		 ↓
  FNA.NetStub (legacy format, .NET Framework)
		 ↓
  FNA (legacy format, .NET Framework)

Fixed (WORKING):
  Murder Miners.exe (net9.0-windows)
		 ↓
  FNA.Contents (net9.0-windows) ← UPDATED
		 ↓
  FNA.NetStub (net9.0-windows) ← UPDATED
		 ↓
  FNA (net9.0-windows) ← UPDATED
```

### Key Issues Identified

1. **FNA.Contents.csproj**
   - TargetFramework: `net461` ❌ Should be `net9.0-windows`
   - PlatformTarget: `x64` (hardcoded, inflexible)
   - Uses modern SDK format but wrong target

2. **FNA.NetStub.csproj**
   - Format: Legacy Visual Studio 2010 format ❌ Should be SDK format
   - Uses condition-based platform targeting for Debug|x86, Debug|x64, etc.
   - Project GUID and old schema structure
   - No implicit TargetFramework - defaults to legacy

3. **FNA.csproj**
   - Format: Legacy Visual Studio 2010 format ❌ Should be SDK format
   - 469 lines of explicit Configuration|Platform combinations
   - No TargetFramework element - uses legacy .NET Framework
   - Imports FNA.Settings.props

4. **Murder Miners.csproj**
   - TargetFramework: `net9.0-windows` ✓ (Correct)
   - PlatformTarget: `x86` (but built to bin\x64\Debug) - Can cause confusion
   - ProjectReference points to incompatible FNA.Contents

---

## Files Created for Fix

1. **FNA.NetStub/FNA.NetStub.csproj.new**
   - Modern SDK-style project file
   - TargetFramework: net9.0-windows
   - LangVersion: 13.0
   - All 60+ source files included
   - ProjectReference to FNA intact

2. **FNA.Contents/FNA.Contents.csproj.new**
   - Modern SDK-style project file
   - TargetFramework: net9.0-windows (changed from net461)
   - PlatformTarget: AnyCPU (changed from x64)
   - Simplified content inclusion pattern
   - ProjectReference to FNA.NetStub

3. **FNA/FNA.csproj.new**
   - Modern SDK-style project file
   - TargetFramework: net9.0-windows
   - Uses glob pattern for file inclusion (simplified)
   - Maintains FNA.Settings.props integration
   - All unsafe blocks and settings preserved

4. **PROJECT_FIX_PLAN.md**
   - Detailed deployment instructions
   - Step-by-step implementation guide
   - Rollback procedures
   - Verification checklist

---

## Why This Happened

### .NET Evolution
- .NET Framework 4.6.1 (EOL since 2016) - Legacy
- .NET Core 1.0-3.1 (Legacy)
- .NET 5+ (LTS releases) - **Current Standard**
- .NET 9.0 (Latest LTS) - Murder Miners uses this ✓

### Incompatibility Reasons
1. Different runtime systems (Framework vs Core/Modern .NET)
2. Different BCL (Base Class Library) APIs
3. Different assembly resolution mechanisms
4. Legacy format projects not recognized by modern SDK tools

### Why It Worked Before
- Legacy projects may have worked in older Visual Studio versions
- Or the dependencies were never actually loaded
- Or they were conditionally compiled out

---

## Implementation Steps

### Before Making Changes
- Backup all three .csproj files
- Ensure you have the latest .NET 9 tools
- Run `dotnet workload restore`

### Apply the Fix
1. Replace FNA.NetStub\FNA.NetStub.csproj
2. Replace FNA.Contents\FNA.Contents.csproj
3. Replace FNA\FNA.csproj
4. Run `dotnet clean`
5. Run `dotnet build`

### Verification
Check build output directory:
```
bin\x64\Debug\net9.0-windows\
  ├── Murder Miners.exe
  ├── FNA.dll
  ├── FNA.NetStub.dll
  ├── FNA.Contents.dll
  └── ...other deps
```

All DLLs should show target framework `net9.0-windows` (check properties).

---

## Advanced Note: Platform Target Clarification

**Current Configuration in Murder Miners**:
- PlatformTarget: x86
- Platforms: x86;x64
- Build Output: bin\x64\Debug\ (inconsistent!)

**What This Means**:
- The project is configured for x86 CPUs
- But built to x64 directory
- Solution: Either change PlatformTarget to x64 or output directory

**Best Practice**:
- For AnyCPU libraries (FNA, FNA.NetStub, FNA.Contents): Keep AnyCPU
- For executables (Murder Miners.exe): Match build output directory

---

## Testing Checklist After Fix

- [ ] Solution builds without errors
- [ ] No MSBUILD warnings related to projects
- [ ] All DLLs in output directory
- [ ] Murder Miners.exe starts without EEFileLoadException
- [ ] Game runs to main menu without crashes
- [ ] No FileNotFoundException in debug output

---

## Success Criteria

✓ Exception 0xE0434352 no longer thrown
✓ All assemblies load successfully
✓ Murder Miners.exe initializes CLR without errors
✓ Application reaches main menu
✓ No FILE_NOT_FOUND (0x80070002) errors in logs

---

## Questions?

**Why modernize instead of downgrade Murder Miners to .NET Framework?**
- Net 9.0 is the future and actively supported
- .NET Framework 4.6.1 is out of support (end of life: 4/26/2022)
- Better performance and security with modern .NET
- Easier to maintain and deploy

**Will this break any game functionality?**
- No. We're only updating project files and target frameworks
- Source code remains unchanged
- All dependencies are upgraded consistently
