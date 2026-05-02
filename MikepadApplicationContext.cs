#nullable enable

using System;
using System.Windows.Forms;

namespace Mikepad;

internal sealed class MikepadApplicationContext : ApplicationContext
{
    private int _openWindows;

    public MikepadApplicationContext(string[] filesToOpen)
    {
        if (filesToOpen.Length == 0)
        {
            OpenWindow();
            return;
        }

        foreach (string file in filesToOpen)
        {
            OpenWindow(file);
        }
    }

    public void OpenWindow(string? filePath = null)
    {
        var form = new MikepadForm(this);
        form.FormClosed += (_, _) =>
        {
            _openWindows--;
            if (_openWindows <= 0)
            {
                ExitThread();
            }
        };

        _openWindows++;

        if (!string.IsNullOrWhiteSpace(filePath))
        {
            form.OpenFileOnStartup(filePath!);
        }

        form.Show();
    }
}
