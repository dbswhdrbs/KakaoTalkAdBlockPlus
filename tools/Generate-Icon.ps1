<#
.SYNOPSIS
    Renders Assets\AppIconDrawing.xaml (256x256 vector) into a multi-size AppIcon.ico.

.DESCRIPTION
    Sizes 16-64 are stored as 32-bit DIB frames (best compatibility with System.Drawing.Icon),
    128 and 256 as PNG frames. Run again whenever AppIconDrawing.xaml changes.

.EXAMPLE
    powershell -ExecutionPolicy Bypass -File tools\Generate-Icon.ps1
    powershell -ExecutionPolicy Bypass -File tools\Generate-Icon.ps1 -PreviewDirectory C:\temp\icon-preview
#>
param(
    [string] $Source = (Join-Path $PSScriptRoot '..\src\KakaoTalkAdBlockPlus\Assets\AppIconDrawing.xaml'),
    [string] $Output = (Join-Path $PSScriptRoot '..\src\KakaoTalkAdBlockPlus\Assets\AppIcon.ico'),
    [string] $PreviewDirectory
)

$ErrorActionPreference = 'Stop'
$assemblies = 'PresentationCore', 'PresentationFramework', 'WindowsBase', 'System.Xaml'
Add-Type -AssemblyName $assemblies

# Windows PowerShell 5.1 compiles this with the C# 5 compiler: keep the syntax simple.
Add-Type -ReferencedAssemblies $assemblies -TypeDefinition @'
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

public static class IconGenerator
{
    private static readonly int[] Sizes = { 16, 20, 24, 32, 40, 48, 64, 128, 256 };

    public static void Generate(string xamlPath, string icoPath, string previewDirectory)
    {
        var resources = (ResourceDictionary)XamlReader.Parse(File.ReadAllText(xamlPath));
        var image = (DrawingImage)resources["AppIconImage"];
        var frames = new List<KeyValuePair<int, byte[]>>();

        foreach (var size in Sizes)
        {
            var bitmap = Render(image, size);
            if (!string.IsNullOrEmpty(previewDirectory))
            {
                File.WriteAllBytes(Path.Combine(previewDirectory, "icon-" + size + ".png"), EncodePng(bitmap));
            }

            frames.Add(new KeyValuePair<int, byte[]>(size, size >= 128 ? EncodePng(bitmap) : EncodeDib(bitmap)));
        }

        WriteIco(icoPath, frames);
    }

    private static BitmapSource Render(DrawingImage image, int size)
    {
        var visual = new DrawingVisual();
        using (var context = visual.RenderOpen())
        {
            context.DrawImage(image, new Rect(0, 0, size, size));
        }

        var target = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
        target.Render(visual);
        return new FormatConvertedBitmap(target, PixelFormats.Bgra32, null, 0);
    }

    private static byte[] EncodePng(BitmapSource bitmap)
    {
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using (var stream = new MemoryStream())
        {
            encoder.Save(stream);
            return stream.ToArray();
        }
    }

    /// <summary>BITMAPINFOHEADER + bottom-up BGRA pixels + AND mask (height is doubled).</summary>
    private static byte[] EncodeDib(BitmapSource bitmap)
    {
        int width = bitmap.PixelWidth, height = bitmap.PixelHeight, stride = width * 4;
        var pixels = new byte[stride * height];
        bitmap.CopyPixels(pixels, stride, 0);
        int maskStride = (width + 31) / 32 * 4;

        using (var stream = new MemoryStream())
        using (var writer = new BinaryWriter(stream))
        {
            writer.Write(40);
            writer.Write(width);
            writer.Write(height * 2);
            writer.Write((short)1);
            writer.Write((short)32);
            writer.Write(0);
            writer.Write(stride * height + maskStride * height);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);

            for (int y = height - 1; y >= 0; y--)
            {
                writer.Write(pixels, y * stride, stride);
            }

            for (int y = height - 1; y >= 0; y--)
            {
                var row = new byte[maskStride];
                for (int x = 0; x < width; x++)
                {
                    if (pixels[y * stride + x * 4 + 3] == 0) row[x / 8] |= (byte)(0x80 >> (x % 8));
                }

                writer.Write(row);
            }

            writer.Flush();
            return stream.ToArray();
        }
    }

    private static void WriteIco(string path, List<KeyValuePair<int, byte[]>> frames)
    {
        using (var writer = new BinaryWriter(File.Create(path)))
        {
            writer.Write((short)0);
            writer.Write((short)1);
            writer.Write((short)frames.Count);

            int offset = 6 + 16 * frames.Count;
            foreach (var frame in frames)
            {
                byte dimension = (byte)(frame.Key >= 256 ? 0 : frame.Key);
                writer.Write(dimension);
                writer.Write(dimension);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((short)1);
                writer.Write((short)32);
                writer.Write(frame.Value.Length);
                writer.Write(offset);
                offset += frame.Value.Length;
            }

            foreach (var frame in frames)
            {
                writer.Write(frame.Value);
            }
        }
    }
}
'@

if ($PreviewDirectory) { New-Item -ItemType Directory -Force -Path $PreviewDirectory | Out-Null }
[IconGenerator]::Generate((Resolve-Path $Source).Path, [IO.Path]::GetFullPath($Output), $PreviewDirectory)
Write-Host "Icon written: $([IO.Path]::GetFullPath($Output))"
