#nullable enable

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Mikepad;

internal sealed class MikepadForm : Form
{
    private static readonly Encoding SaveEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    private readonly MikepadApplicationContext _appContext;
    private readonly MikepadTextEditor _editor;
    private readonly ToolStripStatusLabel _statusLabel;
    private readonly ToolStripMenuItem _wordWrapMenuItem;

    private string? _filePath;
    private bool _isDirty;
    private bool _wordWrap = true;
    private bool _loading;

    public MikepadForm(MikepadApplicationContext appContext)
    {
        _appContext = appContext;

        Text = "Untitled - Mikepad";
        Icon = MikepadBrand.AppIcon;
        ClientSize = new Size(760, 520);
        MinimumSize = new Size(360, 240);
        BackColor = MikepadBrand.FrameBackColor;
        ForeColor = MikepadBrand.TextColor;
        Font = SystemFonts.MessageBoxFont;
        StartPosition = FormStartPosition.CenterScreen;
        AutoScaleMode = AutoScaleMode.Dpi;

        _wordWrapMenuItem = new ToolStripMenuItem("&Word Wrap", null, (_, _) => ToggleWordWrap())
        {
            Checked = _wordWrap
        };

        _statusLabel = new ToolStripStatusLabel
        {
            ForeColor = MikepadBrand.MutedTextColor,
            Spring = true,
            TextAlign = ContentAlignment.MiddleLeft
        };

        MenuStrip menu = BuildMenu();
        StatusStrip statusStrip = BuildStatusStrip();
        Panel editorFrame = BuildEditorFrame();

        var root = new TableLayoutPanel
        {
            BackColor = MikepadBrand.FrameBackColor,
            ColumnCount = 1,
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            RowCount = 3
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.Controls.Add(menu, 0, 0);
        root.Controls.Add(editorFrame, 0, 1);
        root.Controls.Add(statusStrip, 0, 2);

        Controls.Add(root);
        MainMenuStrip = menu;

        _editor = BuildEditor();
        editorFrame.Controls.Add(_editor);

        UpdateStatusBar();
    }

    public void OpenFileOnStartup(string filePath)
    {
        LoadFile(filePath);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        DarkWindowChrome.Apply(this);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!ConfirmSaveChanges())
        {
            e.Cancel = true;
            return;
        }

        base.OnClosing(e);
    }

    private MenuStrip BuildMenu()
    {
        var fileMenu = new ToolStripMenuItem("&File");
        fileMenu.DropDownItems.Add(new ToolStripMenuItem("&New Window", null, (_, _) => _appContext.OpenWindow(), Keys.Control | Keys.N));
        fileMenu.DropDownItems.Add(new ToolStripMenuItem("&Open...", null, (_, _) => OpenFile(), Keys.Control | Keys.O));
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add(new ToolStripMenuItem("&Save", null, (_, _) => SaveFile(), Keys.Control | Keys.S));
        fileMenu.DropDownItems.Add(new ToolStripMenuItem("Save &As...", null, (_, _) => SaveFileAs()));
        fileMenu.DropDownItems.Add(new ToolStripSeparator());
        fileMenu.DropDownItems.Add(new ToolStripMenuItem("E&xit", null, (_, _) => Close(), Keys.Alt | Keys.F4));

        var formatMenu = new ToolStripMenuItem("F&ormat");
        formatMenu.DropDownItems.Add(_wordWrapMenuItem);

        var helpMenu = new ToolStripMenuItem("&Help");
        helpMenu.DropDownItems.Add(new ToolStripMenuItem("&About Mikepad", null, (_, _) => ShowAbout()));

        var menu = new MenuStrip
        {
            BackColor = MikepadBrand.MenuBackColor,
            Dock = DockStyle.Fill,
            ForeColor = MikepadBrand.TextColor,
            Padding = new Padding(3, 2, 0, 2)
        };
        MikepadBrand.StyleToolStrip(menu);
        menu.Items.Add(fileMenu);
        menu.Items.Add(formatMenu);
        menu.Items.Add(helpMenu);
        MikepadBrand.StyleMenuItems(menu.Items);

        return menu;
    }

    private StatusStrip BuildStatusStrip()
    {
        var statusStrip = new StatusStrip
        {
            BackColor = MikepadBrand.TitleBarBackColor,
            Dock = DockStyle.Fill,
            ForeColor = MikepadBrand.MutedTextColor,
            Padding = new Padding(4, 0, 8, 0),
            SizingGrip = false
        };
        MikepadBrand.StyleToolStrip(statusStrip);
        statusStrip.Items.Add(_statusLabel);

        return statusStrip;
    }

    private static Panel BuildEditorFrame()
    {
        return new Panel
        {
            BackColor = MikepadBrand.WindowBorderColor,
            Dock = DockStyle.Fill,
            Margin = new Padding(4, 0, 4, 0),
            Padding = new Padding(1)
        };
    }

    private MikepadTextEditor BuildEditor()
    {
        var editor = new MikepadTextEditor
        {
            BackColor = MikepadBrand.EditorBackColor,
            Dock = DockStyle.Fill,
            ForeColor = MikepadBrand.TextColor,
            Margin = Padding.Empty,
            WordWrap = _wordWrap
        };
        editor.ContentChanged += (_, _) =>
        {
            if (_loading)
            {
                return;
            }

            _isDirty = true;
            UpdateTitle();
            UpdateStatusBar();
        };

        return editor;
    }

    private void OpenFile()
    {
        if (!ConfirmSaveChanges())
        {
            return;
        }

        using var dialog = new OpenFileDialog
        {
            Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*",
            Title = "Open",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            LoadFile(dialog.FileName);
        }
    }

    private void LoadFile(string filePath)
    {
        try
        {
            _loading = true;
            _editor.EditorText = File.ReadAllText(filePath, Encoding.UTF8);
            _editor.SelectionStart = 0;
            _editor.SelectionLength = 0;
            _filePath = filePath;
            _isDirty = false;
            UpdateTitle();
            UpdateStatusBar();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or DecoderFallbackException)
        {
            MikepadDialog.ShowError(this, "Open", $"Mikepad could not open this file:\r\n\r\n{ex.Message}");
            UpdateTitle();
        }
        finally
        {
            _loading = false;
        }
    }

    private bool SaveFile()
    {
        if (string.IsNullOrWhiteSpace(_filePath))
        {
            return SaveFileAs();
        }

        return SaveToPath(_filePath!);
    }

    private bool SaveFileAs()
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*",
            Title = "Save As",
            FileName = string.IsNullOrWhiteSpace(_filePath) ? "Untitled.txt" : Path.GetFileName(_filePath)
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return false;
        }

        return SaveToPath(dialog.FileName);
    }

    private bool SaveToPath(string filePath)
    {
        try
        {
            File.WriteAllText(filePath, _editor.EditorText, SaveEncoding);
            _filePath = filePath;
            _isDirty = false;
            UpdateTitle();
            UpdateStatusBar();
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            MikepadDialog.ShowError(this, "Save", $"Mikepad could not save this file:\r\n\r\n{ex.Message}");
            return false;
        }
    }

    private bool ConfirmSaveChanges()
    {
        if (!_isDirty)
        {
            return true;
        }

        string name = string.IsNullOrWhiteSpace(_filePath) ? "Untitled" : Path.GetFileName(_filePath);
        DialogResult result = MikepadDialog.AskSaveChanges(this, name);

        return result switch
        {
            DialogResult.Yes => SaveFile(),
            DialogResult.No => true,
            _ => false
        };
    }

    private void ToggleWordWrap()
    {
        _wordWrap = !_wordWrap;
        _editor.WordWrap = _wordWrap;
        _wordWrapMenuItem.Checked = _wordWrap;
        UpdateStatusBar();
    }

    private void UpdateTitle()
    {
        string name = string.IsNullOrWhiteSpace(_filePath) ? "Untitled" : Path.GetFileName(_filePath);
        Text = $"{(_isDirty ? "*" : string.Empty)}{name} - Mikepad";
    }

    private void UpdateStatusBar()
    {
        string wrap = _wordWrap ? "Word Wrap On" : "Word Wrap Off";
        string changed = _isDirty ? "Modified" : "Saved";
        _statusLabel.Text = $"{wrap} | {changed}";
    }

    private void ShowAbout()
    {
        MikepadDialog.ShowAbout(this);
    }
}
