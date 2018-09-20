using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Objects;

namespace Utilis
{
    class TicketImage
    {
        public static void Create(Canvas canvas, Ticket ticket)
        {
            canvas.Children.Clear();

            // Set Preview canvas to 20*8cm(300dpi)
            canvas.Width = 2362;
            canvas.Height = 945;

            // Add background to canvas
            canvas.Children.Add(new Image()
            {
                Width = canvas.Width,
                Height = canvas.Height,
                Source = new BitmapImage(new Uri(ticket.Background))
            });

            // Add qrcode to canvas
            QRcode qrcode = new QRcode(Convert.ToInt32(ticket.QRCodeSize.Split('x')[0]),
                Convert.ToInt32(ticket.QRCodeSize.Split('x')[0]), ticket.QRCodeMargin);
            Image ticketQr = qrcode.Create(ticket.QRCodeString);

            Canvas.SetLeft(ticketQr, Convert.ToDouble(ticket.QRCodePosition.Split(',')[0]));
            Canvas.SetTop(ticketQr, Convert.ToDouble(ticket.QRCodePosition.Split(',')[1]));
            canvas.Children.Add(ticketQr);

            // Add ticket number to canvas
            TextBlock textBlock = new TextBlock();
            textBlock.Text = ticket.NumberPrefix + ticket.Number;
            textBlock.FontFamily = new FontFamily(ticket.NumberFont);
            textBlock.FontSize = ticket.NumberSize;
            textBlock.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ticket.NumberColor));
            Canvas.SetLeft(textBlock, Convert.ToDouble(ticket.NumberPosition.Split(',')[0]));
            Canvas.SetTop(textBlock, Convert.ToDouble(ticket.NumberPosition.Split(',')[1]));
            canvas.Children.Add(textBlock);
        }

        // From: https://teusje.wordpress.com/2012/05/01/c-save-a-canvas-as-an-image/
        public static void Save (Canvas canvas, string filename)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(
             (int)canvas.Width, (int)canvas.Height,
             96d, 96d, PixelFormats.Pbgra32);
            // needed otherwise the image output is black
            canvas.Measure(new Size((int)canvas.Width, (int)canvas.Height));
            canvas.Arrange(new Rect(new Size((int)canvas.Width, (int)canvas.Height)));

            renderBitmap.Render(canvas);

            //JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            using (FileStream file = File.Create(filename))
            {
                encoder.Save(file);
            }
        }

        public static void Preview (Image image, string filename)
        {
            image.Source = new BitmapImage(new Uri(filename));
        }
    }
}
