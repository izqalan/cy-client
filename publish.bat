echo off

dotnet publish .\src\CyberdropDownloader.Avalonia\ -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishProfile=win-x64 -v quiet /p:DebugType=None
echo win-x64 published

dotnet publish .\src\CyberdropDownloader.Avalonia\ -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishProfile=win-x86 -v quiet /p:DebugType=None
echo win-x86 published
 
dotnet publish .\src\CyberdropDownloader.Avalonia\ -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=false -p:PublishProfile=linux-x64 -v quiet /p:DebugType=None
echo linux-x64 published
 
dotnet publish .\src\CyberdropDownloader.Avalonia\ -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:PublishProfile=osx-x64 -v quiet /p:DebugType=None
echo osx-x64 published