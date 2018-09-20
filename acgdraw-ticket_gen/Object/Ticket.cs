namespace Objects
{
    class Ticket
    {
        public string QRCodeString = "example";     // 二维码字符串
        public string QRCodeSize = "550x550";       // 二维码大小 宽x高
        public int QRCodeMargin = 1;                // 二维码边距
        public string QRCodePosition = "1735,200";  // 二维码位置 x偏移,y偏移

        public string Number = "1000000000";        // 门票号
        public string NumberPrefix = "门票号 ";     // 门票号前缀
        public string NumberFont = "Microsoft Yahei";//编号字体
        public string NumberColor = "#FFFFFF";      // 编号字体颜色
        public int NumberSize = 48;                 // 编号字体大小
        public string NumberPosition = "30,850";    // 编号位置 x偏移,y偏移

        public string Background = System.AppDomain.CurrentDomain.BaseDirectory + "/ticket_background.bmp";
    }
}
