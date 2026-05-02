# Mikepad

Mikepad is a small retro-inspired Windows text editor with a Windows 98-style menu layout, one document per window, word wrap, custom dark scrollbars, and a dark sepia editor surface.

![Mikepad icon](assets/Mikepad.ico)

## Features

- Plain text editing
- One file per window
- Open, save, and save as
- Word wrap toggle
- Dark sepia editor theme
- Custom teal notepad icon

## Build

Build the executable:

```powershell
.\build.ps1
```

The executable is written to:

```text
dist\Mikepad.exe
```

Create a zip file for GitHub Releases:

```powershell
.\tools\New-ReleaseZip.ps1
```

The release zip is written to:

```text
release\Mikepad.zip
```

You can also publish through the .NET SDK on a machine where restore can write normally:

```powershell
dotnet publish -c Release
```

The executable is written to:

```text
bin\Release\net8.0-windows\publish\Mikepad.exe
```

You can open a file from the command line:

```powershell
.\dist\Mikepad.exe C:\path\to\file.txt
```

## Website

The `docs` folder is ready for GitHub Pages. Once the repository is published as `mikert2010/Mikepad`, enable Pages from the `docs` folder on the `main` branch.

The page links to:

```text
https://github.com/mikert2010/Mikepad/releases/latest/download/Mikepad.zip
```

See [PUBLISHING.md](PUBLISHING.md) for the exact GitHub commands.
