#nullable enable

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Mikepad;

internal static class MikepadBrand
{
    public static readonly Color EditorBackColor = Color.FromArgb(30, 24, 18);
    public static readonly Color TextColor = Color.FromArgb(246, 239, 222);
    public static readonly Color MutedTextColor = Color.FromArgb(210, 198, 170);
    public static readonly Color FrameBackColor = Color.FromArgb(43, 34, 27);
    public static readonly Color TitleBarBackColor = Color.FromArgb(37, 30, 24);
    public static readonly Color MenuBackColor = Color.FromArgb(50, 40, 31);
    public static readonly Color MenuDropDownColor = Color.FromArgb(38, 31, 25);
    public static readonly Color WindowBorderColor = Color.FromArgb(94, 74, 55);
    public static readonly Color AccentColor = Color.FromArgb(37, 138, 133);
    public static readonly Color AccentHoverColor = Color.FromArgb(45, 164, 157);
    public static readonly Color ButtonBackColor = Color.FromArgb(57, 45, 34);
    public static readonly Color ButtonHoverColor = Color.FromArgb(75, 58, 42);

    private static readonly Lazy<Icon> AppIconValue = new(LoadIcon);

    public static Icon AppIcon => AppIconValue.Value;

    public static void StyleButton(Button button)
    {
        button.BackColor = ButtonBackColor;
        button.ForeColor = TextColor;
        button.FlatStyle = FlatStyle.Flat;
        button.UseVisualStyleBackColor = false;
        button.FlatAppearance.BorderColor = WindowBorderColor;
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.MouseOverBackColor = ButtonHoverColor;
        button.FlatAppearance.MouseDownBackColor = AccentColor;
        button.Font = SystemFonts.MessageBoxFont;
        button.MinimumSize = new Size(96, 34);
        button.Size = new Size(96, 34);
        button.Padding = new Padding(10, 2, 10, 3);
        button.TextAlign = ContentAlignment.MiddleCenter;
        button.UseCompatibleTextRendering = true;
    }

    public static void StyleToolStrip(ToolStrip strip)
    {
        strip.BackColor = MenuBackColor;
        strip.ForeColor = TextColor;
        strip.Renderer = new MikepadToolStripRenderer();
        strip.GripStyle = ToolStripGripStyle.Hidden;
    }

    public static void StyleMenuItems(ToolStripItemCollection items)
    {
        foreach (ToolStripItem item in items)
        {
            item.BackColor = MenuBackColor;
            item.ForeColor = TextColor;

            if (item is ToolStripMenuItem menuItem)
            {
                menuItem.DropDown.BackColor = MenuDropDownColor;
                menuItem.DropDown.ForeColor = TextColor;
                menuItem.DropDown.Renderer = new MikepadToolStripRenderer();
                StyleMenuItems(menuItem.DropDownItems);
            }
        }
    }

    private static Icon LoadIcon()
    {
        string assetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "Mikepad.ico");
        if (File.Exists(assetPath))
        {
            return new Icon(assetPath);
        }

        try
        {
            Icon? icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            if (icon is not null)
            {
                return icon;
            }
        }
        catch (ArgumentException)
        {
        }

        return SystemIcons.Application;
    }

    private sealed class MikepadToolStripRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using var brush = new SolidBrush(e.ToolStrip is ToolStripDropDown ? MenuDropDownColor : MenuBackColor);
            e.Graphics.FillRectangle(brush, e.AffectedBounds);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            using var pen = new Pen(WindowBorderColor);
            Rectangle bounds = new(Point.Empty, e.ToolStrip.Size);
            bounds.Width -= 1;
            bounds.Height -= 1;
            e.Graphics.DrawRectangle(pen, bounds);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            Rectangle bounds = new(Point.Empty, e.Item.Size);
            Color fill = e.Item.Selected ? AccentColor : e.Item.BackColor;

            using var brush = new SolidBrush(fill);
            e.Graphics.FillRectangle(brush, bounds);

            if (e.Item.Selected)
            {
                using var pen = new Pen(AccentHoverColor);
                bounds.Width -= 1;
                bounds.Height -= 1;
                e.Graphics.DrawRectangle(pen, bounds);
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            int y = e.Item.Height / 2;
            using var pen = new Pen(WindowBorderColor);
            e.Graphics.DrawLine(pen, 4, y, e.Item.Width - 4, y);
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            Rectangle bounds = new(3, 2, e.Item.Height - 5, e.Item.Height - 5);
            using var brush = new SolidBrush(AccentColor);
            using var pen = new Pen(TextColor, 2);
            e.Graphics.FillRectangle(brush, bounds);
            e.Graphics.DrawLines(pen, new[]
            {
                new Point(bounds.Left + 4, bounds.Top + bounds.Height / 2),
                new Point(bounds.Left + bounds.Width / 2 - 1, bounds.Bottom - 5),
                new Point(bounds.Right - 4, bounds.Top + 5)
            });
        }
    }
}
