#nullable enable

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mikepad;

internal sealed class MikepadDialog : Form
{
    private const string SmileyGlyph = "\ud83d\ude42";

    private MikepadDialog(string title, string message, DialogButton[] buttons)
    {
        Text = title;
        Icon = MikepadBrand.AppIcon;
        ShowIcon = true;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        BackColor = MikepadBrand.FrameBackColor;
        ForeColor = MikepadBrand.TextColor;
        Font = SystemFonts.MessageBoxFont;
        ClientSize = new Size(484, 194);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = MikepadBrand.FrameBackColor,
            Padding = new Padding(18, 16, 18, 14),
            ColumnCount = 2,
            RowCount = 2
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 76));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));

        var glyph = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            Text = SmileyGlyph,
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = MikepadBrand.AccentHoverColor,
            Font = ResolveEmojiFont()
        };

        var messageLabel = new Label
        {
            AutoEllipsis = false,
            Dock = DockStyle.Fill,
            ForeColor = MikepadBrand.TextColor,
            Text = message,
            TextAlign = ContentAlignment.MiddleLeft
        };

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 10, 0, 0)
        };

        for (int index = buttons.Length - 1; index >= 0; index--)
        {
            Button button = CreateButton(buttons[index]);
            buttonPanel.Controls.Add(button);

            if (buttons[index].Result == DialogResult.Cancel)
            {
                CancelButton = button;
            }
        }

        if (buttons.Length > 0)
        {
            AcceptButton = buttonPanel.Controls[buttonPanel.Controls.Count - 1] as Button;
        }

        layout.Controls.Add(glyph, 0, 0);
        layout.Controls.Add(messageLabel, 1, 0);
        layout.Controls.Add(buttonPanel, 0, 1);
        layout.SetColumnSpan(buttonPanel, 2);

        Controls.Add(layout);
    }

    public static DialogResult AskSaveChanges(IWin32Window owner, string documentName)
    {
        using var dialog = new MikepadDialog(
            "Mikepad",
            $"Save changes to {documentName}?",
            new[]
            {
                new DialogButton("&Yes", DialogResult.Yes),
                new DialogButton("&No", DialogResult.No),
                new DialogButton("Cancel", DialogResult.Cancel)
            });

        return dialog.ShowDialog(owner);
    }

    public static void ShowError(IWin32Window owner, string title, string message)
    {
        using var dialog = new MikepadDialog(
            title,
            message,
            new[] { new DialogButton("OK", DialogResult.OK) });

        dialog.ShowDialog(owner);
    }

    public static void ShowAbout(IWin32Window owner)
    {
        using var dialog = new MikepadDialog(
            "About Mikepad",
            "Mikepad\r\n\r\nA small dark sepia notepad for one text file per window.",
            new[] { new DialogButton("OK", DialogResult.OK) });

        dialog.ShowDialog(owner);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        DarkWindowChrome.Apply(this);
    }

    private static Button CreateButton(DialogButton spec)
    {
        var button = new Button
        {
            Text = spec.Text,
            DialogResult = spec.Result,
            Margin = new Padding(8, 0, 0, 0)
        };

        MikepadBrand.StyleButton(button);
        return button;
    }

    private static Font ResolveEmojiFont()
    {
        try
        {
            return new Font("Segoe UI Emoji", 28f, FontStyle.Regular, GraphicsUnit.Point);
        }
        catch (ArgumentException)
        {
            return new Font(SystemFonts.MessageBoxFont.FontFamily, 28f, FontStyle.Regular, GraphicsUnit.Point);
        }
    }

    private sealed class DialogButton
    {
        public DialogButton(string text, DialogResult result)
        {
            Text = text;
            Result = result;
        }

        public string Text { get; }
        public DialogResult Result { get; }
    }
}
