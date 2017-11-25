using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace TonusClub.Infrastructure
{
    public static class Imaging
    {
        public static byte[] StoreImageWithResize(Stream stream, int maxW, int maxH)
        {
            var ms = ResizeImageFromStream(stream, maxW, maxH);

            byte[] ImageData = new byte[ms.Length];

            ms.Read(ImageData, 0, (int)ms.Length);

            return ImageData;
        }

        public static Image GetImage(byte[] bytes)
        {
            return Image.FromStream(new MemoryStream(bytes));
        }

        private static Stream ResizeImageFromStream(Stream stream, int maxW, int maxH)
        {
            Image img = Image.FromStream(stream);

            var newH = img.Size.Height;
            var newW = img.Size.Width;

            if (newH > maxH)
            {
                newW = (int)Math.Floor(newW * maxH / (double)newH);
                newH = maxH;
            }
            if (newW > maxW)
            {
                newH = (int)Math.Floor(newH * maxW / (double)newW);
                newW = maxW;
            }


            Bitmap ni = new Bitmap(newW, newH);
            var g = Graphics.FromImage(ni);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.DrawImage(img, new Rectangle(0, 0, newW, newH));

            MemoryStream ms = new MemoryStream();
            ni.Save(ms, ImageFormat.Jpeg);
            ms.Position = 0;
            return ms;
        }

    }
}
