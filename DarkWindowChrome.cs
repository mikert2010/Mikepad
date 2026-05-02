#nullable enable

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mikepad;

internal static class DarkWindowChrome
{
    private const int DwmwaUseImmersiveDarkModeBefore20h1 = 19;
    private const int DwmwaUseImmersiveDarkMode = 20;
    private const int DwmwaBorderColor = 34;
    private const int DwmwaCaptionColor = 35;
    private const int DwmwaTextColor = 36;

    public static void Apply(Form form)
    {
        if (form.Handle == IntPtr.Zero)
        {
            return;
        }

        int enabled = 1;
        TrySet(form.Handle, DwmwaUseImmersiveDarkMode, enabled);
        TrySet(form.Handle, DwmwaUseImmersiveDarkModeBefore20h1, enabled);

        uint caption = ToColorRef(MikepadBrand.TitleBarBackColor);
        uint border = ToColorRef(MikepadBrand.WindowBorderColor);
        uint text = ToColorRef(MikepadBrand.TextColor);

        TrySet(form.Handle, DwmwaCaptionColor, caption);
        TrySet(form.Handle, DwmwaBorderColor, border);
        TrySet(form.Handle, DwmwaTextColor, text);
    }

    private static uint ToColorRef(Color color)
    {
        return (uint)(color.R | (color.G << 8) | (color.B << 16));
    }

    private static void TrySet(IntPtr handle, int attribute, int value)
    {
        try
        {
            DwmSetWindowAttribute(handle, attribute, ref value, Marshal.SizeOf(typeof(int)));
        }
        catch (DllNotFoundException)
        {
        }
        catch (EntryPointNotFoundException)
        {
        }
    }

    private static void TrySet(IntPtr handle, int attribute, uint value)
    {
        try
        {
            DwmSetWindowAttribute(handle, attribute, ref value, Marshal.SizeOf(typeof(uint)));
        }
        catch (DllNotFoundException)
        {
        }
        catch (EntryPointNotFoundException)
        {
        }
    }

    [DllImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int attributeValue, int attributeSize);

    [DllImport("dwmapi.dll", EntryPoint = "DwmSetWindowAttribute")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref uint attributeValue, int attributeSize);
}
