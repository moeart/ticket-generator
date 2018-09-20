using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.QrCode;

namespace Utilis
{
    class QRcode
    {
        private QrCodeEncodingOptions options = new QrCodeEncodingOptions();

        public QRcode(int width = 0, int height = 0, int margin = 0)
        {
            options.DisableECI = true;
            options.CharacterSet = "UTF-8";
            options.Width = width > 0 ? width : 500;
            options.Height = height > 0 ? height : 500;
            options.Margin = margin > 0 ? margin : 0;
            options.ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.H;
        }

        public System.Windows.Controls.Image Create(string text)
        {
            // preparing create qrcode
            BarcodeWriter writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;
            writer.Options = options;

            // create qrcode to bitmap stream
            using (var bitmap = writer.Write(text))

            // convernt bitmap stream to image
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);

                // convernt bitmap to bitmapimage
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                stream.Seek(0, SeekOrigin.Begin);
                bi.StreamSource = stream;
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.EndInit();

                // convernt bitmapimage to image
                System.Windows.Controls.Image img = new System.Windows.Controls.Image()
                {
                    Source = bi
                };

                return img;
            }
        }
    }
}
