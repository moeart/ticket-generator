using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using Utilis;
using Objects;
using Microsoft.Win32;
using System.IO;
using System.Threading;

namespace acgdraw_ticket_gen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static JObject TicketData;
        private Ticket Ticket = new Ticket();
        private string WorkDir = @AppDomain.CurrentDomain.BaseDirectory + "/";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadConfig(object sender, RoutedEventArgs e)
        {
            try
            {
                // open and load ticket data
                OpenFileDialog configOpenBox = new OpenFileDialog();
                configOpenBox.Filter = "门票数据|*.tdat";
                if (configOpenBox.ShowDialog() == true)
                {
                    TicketData = JObject.Parse(File.ReadAllText(configOpenBox.FileName));
                    WorkDir = @System.IO.Path.GetDirectoryName(configOpenBox.FileName) + "/";
                }

                // enable generate button when data no problem
                if (TicketData["options"] != null
                    && TicketData["tickets"] != null)
                {
                    // loading ticket options
                    if (TicketData["options"]["NumberColor"] != null
                        && TicketData["options"]["NumberColor"].ToString().Length > 0) Ticket.NumberColor = TicketData["options"]["NumberColor"].ToString();
                    if (TicketData["options"]["NumberSize"] != null
                        && TicketData["options"]["NumberSize"].ToString().Length > 0) Ticket.NumberSize = Convert.ToInt32(TicketData["options"]["NumberSize"]);
                    if (TicketData["options"]["NumberPosition"] != null
                        && TicketData["options"]["NumberPosition"].ToString().Length > 0) Ticket.NumberPosition = TicketData["options"]["NumberPosition"].ToString();
                    if (TicketData["options"]["NumberFont"] != null
                        && TicketData["options"]["NumberFont"].ToString().Length > 0) Ticket.NumberFont = TicketData["options"]["NumberFont"].ToString();
                    if (TicketData["options"]["NumberPrefix"] != null
                        && TicketData["options"]["NumberPrefix"].ToString().Length > 0) Ticket.NumberPrefix = TicketData["options"]["NumberPrefix"].ToString();
                    if (TicketData["options"]["QRCodeSize"] != null
                        && TicketData["options"]["QRCodeSize"].ToString().Length > 0) Ticket.QRCodeSize = TicketData["options"]["QRCodeSize"].ToString();
                    if (TicketData["options"]["QRCodePosition"] != null
                        && TicketData["options"]["QRCodePosition"].ToString().Length > 0) Ticket.QRCodePosition = TicketData["options"]["QRCodePosition"].ToString();
                    if (TicketData["options"]["QRCodeMargin"] != null
                        && TicketData["options"]["QRCodeMargin"].ToString().Length > 0) Ticket.QRCodeMargin = Convert.ToInt32(TicketData["options"]["QRCodeMargin"]);

                    btnGenerate.IsEnabled = true;
                }
                else
                {
                    btnGenerate.IsEnabled = false;
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show("载入门票数据发生了错误，错误详细信息：\n"+e1.Message, "错误",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void GenTicket(object sender, RoutedEventArgs e)
        {
            Double totalCount = TicketData["tickets"].Count();
            Double counter = 1;
            btnGenerate.IsEnabled = false;

            Thread Worker = new Thread(() =>
            {
                try
                {
                    Canvas ticketImg = new Canvas();

                    foreach (JArray ticket in TicketData["tickets"])
                    {
                        Ticket.Number = ticket[0].ToString();
                        Ticket.QRCodeString = ticket[1].ToString();

                        // generate images
                        TicketImage.Create(ticketImg, Ticket);
                        TicketImage.Save(ticketImg, WorkDir + Ticket.Number.ToString() + ".png");

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            // enable preview if checked
                            if (previewEnable.IsChecked == true)
                                TicketImage.Preview(Preview, WorkDir + Ticket.Number.ToString() + ".png");

                            // set progress bar
                            Progress.Value = (counter++ / totalCount) * 100;
                        });
                    }

                    // finished ask open or not
                    Application.Current.Dispatcher.Invoke(() => { btnGenerate.IsEnabled = true; });
                    if (MessageBox.Show("所有门票生成完成，是否立即查看？", "成功",
                            MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                        System.Diagnostics.Process.Start(WorkDir);
                }
                catch (Exception e1)
                {
                    Application.Current.Dispatcher.Invoke(() => { btnGenerate.IsEnabled = true; });
                    MessageBox.Show("生成门票时发生了错误，错误详细信息：\n" + e1.Message, "错误",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            });
            Worker.SetApartmentState(ApartmentState.STA); // as STA thread
            Worker.Start();
        }

        private void LoadBackground(object sender, RoutedEventArgs e)
        {
            OpenFileDialog backgroundOpenBox = new OpenFileDialog();
            backgroundOpenBox.Filter = "支持的图片|*.bmp;*.png;*.jpg";
            if (backgroundOpenBox.ShowDialog() == true)
            {
                Ticket.Background = backgroundOpenBox.FileName;
            }
        }
    }
}
