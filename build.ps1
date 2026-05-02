$ErrorActionPreference = 'Stop'

$compiler = 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\Roslyn\csc.exe'
if (-not (Test-Path -LiteralPath $compiler)) {
    $compiler = Get-ChildItem 'C:\Program Files\Microsoft Visual Studio' -Recurse -Filter csc.exe -ErrorAction SilentlyContinue |
        Where-Object { $_.FullName -like '*\Roslyn\csc.exe' } |
        Select-Object -First 1 -ExpandProperty FullName
}

if ([string]::IsNullOrWhiteSpace($compiler) -or -not (Test-Path -LiteralPath $compiler)) {
    throw 'Could not find a modern Roslyn C# compiler on this machine.'
}

New-Item -ItemType Directory -Force dist | Out-Null

powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Generate-MikepadIcon.ps1

& $compiler `
    /nologo `
    /target:winexe `
    /langversion:latest `
    /win32manifest:app.manifest `
    /win32icon:assets\Mikepad.ico `
    /out:dist\Mikepad.exe `
    /reference:System.dll `
    /reference:System.Core.dll `
    /reference:System.Drawing.dll `
    /reference:System.Windows.Forms.dll `
    Program.cs `
    MikepadApplicationContext.cs `
    DarkWindowChrome.cs `
    MikepadBrand.cs `
    MikepadDialog.cs `
    MikepadTextEditor.cs `
    MikepadForm.cs

if ($LASTEXITCODE -ne 0) {
    throw "C# compiler failed with exit code $LASTEXITCODE."
}
