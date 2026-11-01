using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;

static class Installer
{
    const string Title = "Matrix Hacker Saver";
    const string ScrName = "MatrixSaver.scr";

    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MatrixHackerSaver");
        string target = Path.Combine(dir, ScrName);

        try
        {
            if (args.Length > 0 && args[0].ToLower() == "/uninstall")
            {
                Uninstall(dir, target);
                MessageBox.Show("Kaldirildi.", Title);
                return;
            }

            if (MessageBox.Show("Matrix Hacker Saver kurulsun mu?\n\nKonum: " + dir + "\nYonetici izni gerekmez.",
                Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            Directory.CreateDirectory(dir);
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream(ScrName))
            using (FileStream f = File.Create(target))
                s.CopyTo(f);

            using (RegistryKey k = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
            {
                k.SetValue("SCRNSAVE.EXE", target);
                k.SetValue("ScreenSaveActive", "1");
                k.SetValue("ScreenSaveTimeOut", "300");
            }

            // Kaldirma icin installer'in kendisini de kopyala
            File.Copy(Application.ExecutablePath, Path.Combine(dir, "Uninstall.exe"), true);

            MessageBox.Show("Kurulum tamam. Ekran koruyucu 5 dakika bosta kalinca baslar.\n" +
                "Ayarlar > Kisisellestirme > Kilit ekrani > Ekran koruyucu bolumunden degistirebilirsin.\n\n" +
                "Kaldirmak icin: " + Path.Combine(dir, "Uninstall.exe") + " /uninstall", Title);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Hata: " + ex.Message, Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    static void Uninstall(string dir, string target)
    {
        using (RegistryKey k = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
        {
            object cur = k.GetValue("SCRNSAVE.EXE");
            if (cur != null && string.Equals(cur.ToString(), target, StringComparison.OrdinalIgnoreCase))
            {
                k.DeleteValue("SCRNSAVE.EXE", false);
                k.SetValue("ScreenSaveActive", "0");
            }
        }
        if (File.Exists(target)) File.Delete(target);
    }
}
