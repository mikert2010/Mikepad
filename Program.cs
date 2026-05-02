#nullable enable

using System;
using System.Windows.Forms;

namespace Mikepad;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        Application.SetCompatibleTextRenderingDefault(false);

        using var context = new MikepadApplicationContext(args);
        Application.Run(context);
    }
}
