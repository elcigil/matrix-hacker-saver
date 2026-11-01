using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

class Drop { public float Y; public float Speed; public int Len; public char[] Chars; }

class SaverForm : Form
{
    static readonly Random R = new Random();
    const string Glyphs = "0123456789ABCDEFabcdef<>/|=+*#$%&@ｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜﾝ";

    static readonly string[] Lines = {
        "[*] Initializing framework modules...",
        "[*] Loading payload database... done",
        "[*] Scanning 10.10.14.0/24 for open ports",
        "[+] 10.10.14.37:22 open (ssh)",
        "[+] 10.10.14.37:443 open (https)",
        "[*] Brute-forcing handshake key ... 4096 bit",
        "[*] Bypassing firewall layer 3 of 7",
        "[+] Access granted: root@mainframe",
        "[*] Decrypting packet stream 0xA4F2C91E",
        "[+] Session 1 opened (10.10.14.2:4444 -> 10.10.14.37:51822)",
        "[*] Downloading /etc/shadow ... 100%",
        "[-] Connection reset by peer, retrying",
        "[*] Injecting shellcode into process 4812",
        "[+] Trace blocked. Proxy chain: 7 hops",
        "[*] Compiling exploit... OK",
        "msf6 > use auxiliary/scanner/discovery",
        "msf6 > set RHOSTS 192.168.0.0/16",
        "msf6 > run",
        "meterpreter > sysinfo",
        "[+] System compromised. Uploading backdoor...",
    };

    Timer timer = new Timer();
    Font rainFont, termFont;
    int cw, ch;
    Drop[] cols;
    List<string> term = new List<string>();
    int tick;
    Point startMouse = Point.Empty;
    bool preview;

    public SaverForm(Rectangle bounds, bool preview)
    {
        this.preview = preview;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        Bounds = bounds;
        BackColor = Color.Black;
        DoubleBuffered = true;
        ShowInTaskbar = false;
        cw = preview ? 7 : 16;
        ch = preview ? 9 : 20;
        rainFont = new Font("MS Gothic", preview ? 6 : 13, FontStyle.Bold);
        termFont = new Font("Consolas", 12, FontStyle.Regular);
        if (!preview) { TopMost = true; Cursor.Hide(); }

        cols = new Drop[Width / cw + 1];
        for (int i = 0; i < cols.Length; i++) cols[i] = NewDrop(true);

        timer.Interval = 40;
        timer.Tick += delegate { Step(); Invalidate(); };
        timer.Start();

        FormClosed += delegate { Application.Exit(); };
        KeyDown += delegate { if (!this.preview) Close(); };
        MouseDown += delegate { if (!this.preview) Close(); };
        MouseMove += delegate(object s, MouseEventArgs e)
        {
            if (this.preview) return;
            if (startMouse == Point.Empty) { startMouse = e.Location; return; }
            if (Math.Abs(e.X - startMouse.X) > 8 || Math.Abs(e.Y - startMouse.Y) > 8) Close();
        };
    }

    Drop NewDrop(bool initial)
    {
        int rows = Height / ch + 1;
        Drop d = new Drop();
        d.Len = R.Next(8, 30);
        d.Speed = 0.3f + (float)R.NextDouble() * 1.0f;
        d.Y = initial ? R.Next(-rows, rows) : -R.Next(2, 30);
        d.Chars = new char[d.Len];
        for (int i = 0; i < d.Len; i++) d.Chars[i] = Glyphs[R.Next(Glyphs.Length)];
        return d;
    }

    void Step()
    {
        int rows = Height / ch + 1;
        for (int i = 0; i < cols.Length; i++)
        {
            Drop d = cols[i];
            d.Y += d.Speed;
            if (R.Next(6) == 0) d.Chars[R.Next(d.Len)] = Glyphs[R.Next(Glyphs.Length)];
            if (d.Y - d.Len > rows) cols[i] = NewDrop(false);
        }
        tick++;
        if (!preview && tick % 12 == 0)
        {
            term.Add(Lines[R.Next(Lines.Length)]);
            int max = Math.Max(4, Height / 4 / 18);
            while (term.Count > max) term.RemoveAt(0);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.Clear(Color.Black);
        for (int i = 0; i < cols.Length; i++)
        {
            Drop d = cols[i];
            for (int k = 0; k < d.Len; k++)
            {
                int row = (int)d.Y - k;
                if (row < 0) continue;
                int y = row * ch;
                if (y > Height) continue;
                int shade = 255 - (255 * k / d.Len);
                Color c = k == 0 ? Color.FromArgb(220, 255, 220) : Color.FromArgb(0, Math.Max(40, shade), 0);
                using (SolidBrush b = new SolidBrush(c))
                    g.DrawString(d.Chars[k].ToString(), rainFont, b, i * cw, y);
            }
        }
        if (!preview && term.Count > 0)
        {
            int h = term.Count * 18 + 14;
            Rectangle box = new Rectangle(30, Height - h - 30, Math.Min(Width - 60, 760), h);
            using (SolidBrush bg = new SolidBrush(Color.FromArgb(200, 0, 12, 0))) g.FillRectangle(bg, box);
            using (Pen p = new Pen(Color.FromArgb(0, 200, 0))) g.DrawRectangle(p, box);
            using (SolidBrush fg = new SolidBrush(Color.FromArgb(0, 255, 70)))
                for (int i = 0; i < term.Count; i++)
                    g.DrawString(term[i], termFont, fg, box.X + 10, box.Y + 7 + i * 18);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) timer.Dispose();
        base.Dispose(disposing);
    }
}

static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        string a = args.Length > 0 ? args[0].ToLower().Substring(0, 2) : "/s";
        if (a == "/c")
        {
            MessageBox.Show("Matrix Hacker Saver\nAyar yok.", "Screensaver");
            return;
        }
        if (a == "/p")
        {
            // Onizleme penceresi: Windows kucuk pencere handle'i verir; basitce cik.
            return;
        }
        foreach (Screen s in Screen.AllScreens)
            new SaverForm(s.Bounds, false).Show();
        Application.Run();
    }
}
