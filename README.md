# Matrix Hacker Saver

Windows için Matrix tarzı ekran koruyucu: yukarıdan akan yeşil kodlar ve hacker filmlerindeki gibi akan sahte konsol çıktısı. Yalnızca görsel amaçlıdır; gerçek bir tarama veya saldırı yapmaz.

Ayrıntılı açıklama için [index.html](index.html) dosyasına bakın.

## Kurulum
1. [Releases](../../releases/latest) sayfasından `MatrixSaverSetup.exe` dosyasını indirip çalıştırın (yönetici izni gerekmez).
2. Kaldırmak için: `%LOCALAPPDATA%\MatrixHackerSaver\Uninstall.exe /uninstall`
3. Elle kurulum: `MatrixSaver.scr` → sağ tık → **Install**.

## Derleme
```
csc /target:winexe /out:MatrixSaver.scr /r:System.Windows.Forms.dll /r:System.Drawing.dll MatrixSaver.cs
```

## Lisans
MIT, bkz. [LICENSE](LICENSE).
