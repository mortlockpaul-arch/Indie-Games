# Murder Miners - Exception Analysis & Fix Plan

## Exception Summary
- **Exception Type**: 0xE0434352 (CLR Exception)
- **Error Code**: 0x80070002 (FILE_NOT_FOUND)
- **Root Cause**: `EEFileLoadException` - Failed to load a managed assembly during runtime initialization
- **Details**: Target framework mismatch between .NET 9 application and .NET Framework 4.6.1 dependency

## Root Cause Analysis

### The Problem
Murder Miners.exe is built with **net9.0-windows** but depends on:
1. **FNA.Contents** → targets **net461** (.NET Framework 4.6.1) ❌
2. **FNA.NetStub** → uses legacy VS2010 format targeting .NET Framework ❌
3. **FNA** → uses legacy VS2010 format targeting .NET Framework ❌

When the CLR tries to load FNA.Contents assembly (which depends on FNA.NetStub and FNA, both .NET Framework assemblies), it fails because .NET 9 cannot directly load .NET Framework assemblies that don't have a compatible target framework.

### Dependency Chain
```
Murder Miners.exe (net9.0-windows)
   └─ FNA.Contents (net461) ❌ INCOMPATIBLE
	  └─ FNA.NetStub (legacy .NET Framework) ❌
		 └─ FNA (legacy .NET Framework) ❌
```

## Solution: Target Framework Modernization

All projects must be updated to target **net9.0-windows** to ensure compatibility.

### Changes Required

#### 1. FNA.NetStub/FNA.NetStub.csproj.new
- **Before**: Legacy VS2010 format with configuration-based platform targets
- **After**: Modern SDK-style format targeting net9.0-windows
- **Key Changes**:
  - Convert from legacy project format to SDK format
  - Change LangVersion from 4 to 13.0
  - Update TargetFramework to net9.0-windows
  - Keep all source files (60+ files)
  - Maintain ProjectReference to FNA

#### 2. FNA.Contents/FNA.Contents.csproj.new
- **Before**: net461 target
- **After**: net9.0-windows target
- **Key Changes**:
  - Change TargetFramework from net461 to net9.0-windows
  - Update PlatformTarget from x64 to AnyCPU
  - Simplify platform-specific content handling
  - Update ProjectReference to FNA.NetStub

#### 3. FNA/FNA.csproj.new
- **Before**: Legacy VS2010 format with 469 lines of configuration
- **After**: Modern SDK-style format
- **Key Changes**:
  - Convert from legacy format to SDK format
  - Change TargetFramework to net9.0-windows
  - Use glob pattern `src/**/*.cs` instead of explicit file includes
  - Maintain FNA.Settings.props integration
  - Keep AllowUnsafeBlocks = True

## Deployment Instructions

### Step 1: Backup Original Files
```bash
# Create backups of original project files
copy FNA.NetStub\FNA.NetStub.csproj FNA.NetStub\FNA.NetStub.csproj.backup
copy FNA.Contents\FNA.Contents.csproj FNA.Contents\FNA.Contents.csproj.backup
copy FNA\FNA.csproj FNA\FNA.csproj.backup
```

### Step 2: Replace Project Files
```bash
# Replace with new SDK-style project files
del FNA.NetStub\FNA.NetStub.csproj
ren FNA.NetStub\FNA.NetStub.csproj.new FNA.NetStub.csproj

del FNA.Contents\FNA.Contents.csproj
ren FNA.Contents\FNA.Contents.csproj.new FNA.Contents.csproj

del FNA\FNA.csproj
ren FNA\FNA.csproj.new FNA.csproj
```

### Step 3: Clean and Rebuild
```bash
# Clean all build outputs
dotnet clean

# Rebuild the entire solution
dotnet build --configuration Debug --platform x64

# For the specific project
cd "indie-game-archive\Murder Miners (XBLIG)"
dotnet build --configuration Debug
```

### Step 4: Verify Build Output
Check that all DLLs are present in:
```
bin\x64\Debug\net9.0-windows\
├── FNA.dll
├── FNA.NetStub.dll
├── FNA.Contents.dll
├── Murder Miners.dll
└── ...other dependencies
```

### Step 5: Test Run
Execute the application to verify it no longer throws 0xE0434352 exception.

## Key Technical Details

### .NET Framework vs .NET 9 Compatibility
- .NET Framework 4.6.1 assemblies cannot be directly loaded by .NET 9 applications
- All dependencies must target the same TFM (Target Framework Moniker) or BCL-compatible versions
- To support interoperability, all projects must target net9.0-windows

### Platform Target Settings
- **Murder Miners.exe**: PlatformTarget x86 (with Platforms x86;x64)
- **FNA.Contents**: Changed from x64-specific to AnyCPU (more flexible)
- **FNA.NetStub**: Changed to AnyCPU (platform-agnostic library)
- **FNA**: Changed to AnyCPU (platform-agnostic library)

## Expected Outcome
After applying these changes:
1. CLR can load all assemblies (same target framework)
2. Assembly resolver finds FNA.Contents, FNA.NetStub, and FNA
3. Murder Miners.exe initializes successfully
4. EEFileLoadException (0xE0434352) is resolved

## Rollback Plan
If issues occur:
1. Restore backup files
2. Run `dotnet clean`
3. Rebuild with original project files
4. File issue with detailed error messages

## Notes
- These modernized projects preserve all original functionality
- SDK-style projects are the modern .NET standard
- Explicit file includes (60+ files in FNA.NetStub) can be replaced with glob patterns but are maintained for compatibility
- FNA.Settings.props integration is preserved for custom property overrides
