# Publishing Mikepad

These steps assume the GitHub repository will be `mikert2010/Mikepad`.

## First Push

```powershell
git init -b main
git add .
git commit -m "Initial Mikepad release"
gh repo create mikert2010/Mikepad --public --source . --remote origin --push
```

If Git asks for your name or email first:

```powershell
git config user.name "Your Name"
git config user.email "your-email@example.com"
```

Use the email address you want associated with the commit. GitHub also supports private noreply addresses.

## Create A Downloadable Release

```powershell
.\tools\New-ReleaseZip.ps1
gh release create v0.1.0 .\release\Mikepad.zip --title "Mikepad v0.1.0" --notes "First public Mikepad build."
```

After this, the website download button will point to:

```text
https://github.com/mikert2010/Mikepad/releases/latest/download/Mikepad.zip
```

## Enable The Website

1. Open the repository on GitHub.
2. Go to Settings.
3. Go to Pages.
4. Set Source to `Deploy from a branch`.
5. Set Branch to `main` and folder to `/docs`.
6. Save.

The site will publish at:

```text
https://mikert2010.github.io/Mikepad/
```

## Later Installer Option

For now, GitHub Releases should host `Mikepad.zip`. A real Windows installer can be added later with Inno Setup or WiX once the app name, icon, and first release are stable.
