using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SkiaSharp;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SafeFutureInventorySystem.Helpers;

public static class QrCodeHelper
{
    public static string BuildInventoryDetailsPath(IUrlHelper urlHelper, int itemId)
    {
        return urlHelper.Action("Details", "Inventory", new { id = itemId }) ?? $"/Inventory/Details/{itemId}";
    }

    public static string ResolveToAbsolute(IConfiguration? config, HttpRequest? request, string path)
    {
        if (string.IsNullOrEmpty(path)) return path ?? string.Empty;

        if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return path;
        }

        var configuredBase = config?["QrCodeBaseUrl"];
        if (!string.IsNullOrEmpty(configuredBase))
        {
            return configuredBase.TrimEnd('/') + (path.StartsWith("/") ? path : "/" + path);
        }

        var scheme = request?.Scheme ?? "http";
        var host = request?.Host.Value ?? "localhost";
        return scheme + "://" + host + (path.StartsWith("/") ? path : "/" + path);
    }

    public static byte[] GeneratePng(string value, int size = 250, int margin = 1)
    {
        var writer = new ZXing.BarcodeWriterPixelData
        {
            Format = ZXing.BarcodeFormat.QR_CODE,
            Options = new ZXing.Common.EncodingOptions
            {
                Height = size,
                Width = size,
                Margin = margin
            }
        };

        var pixelData = writer.Write(value);

        using (var surface = SKSurface.Create(new SKImageInfo(pixelData.Width, pixelData.Height)))
        {
            using (var canvas = surface.Canvas)
            {
                using (var bitmap = new SKBitmap(new SKImageInfo(pixelData.Width, pixelData.Height)))
                {
                    var ptr = bitmap.GetPixels();
                    Marshal.Copy(pixelData.Pixels, 0, ptr, pixelData.Pixels.Length);

                    canvas.Clear(SKColors.White);
                    canvas.DrawBitmap(bitmap, 0, 0);
                }
            }

            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var ms = new MemoryStream())
            {
                data.SaveTo(ms);
                return ms.ToArray();
            }
        }
    }
}
