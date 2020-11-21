using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace BusTimer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private int requireSecond;
        private double canvasWidth = 800 * (5 / 7.0);

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            requireSecond = GetRequireSecond();

            DispatcherTimer busTimer = new DispatcherTimer();
            busTimer.Tick += BusTimer_Tick;
            busTimer.Interval = new TimeSpan(0, 0, 1);
            busTimer.Start();
        }

        private void BusTimer_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            // 버스 출발 시간
            DateTime startDateTime = GetBusStartTime(now);
            DateTime currentBusStartDateTime = startDateTime;

            // 출발해야하는 시간
            DateTime leaveTime = startDateTime.AddSeconds(-requireSecond);

            // 이미 버스를 놓친 경우
            if (DateTime.Compare(now, leaveTime) > 0)
            {
                startDateTime = startDateTime.AddMinutes(20);
                leaveTime = startDateTime.AddSeconds(-requireSecond);
            }

            tbLeaveTime.Text = leaveTime.ToString("HH:mm:ss");

            TimeSpan leftTime = currentBusStartDateTime.Subtract(now);

            tbLeftTime.Text = string.Format("버스 도착까지 {0}분 {1}초 남았습니다", leftTime.Minutes, leftTime.Seconds);

            // 버스 애니메이션
            double leftSeconds = leftTime.Minutes * 60 + leftTime.Seconds;
            double rate = 1 - (leftSeconds / (Setting.BUS_INTERVAL_MINUTE * 60));

            double width = canvasWidth;

            double busXPos = (width * rate) - Setting.BUS_ICON_SIZE;
            Canvas.SetLeft(cvBusWrapper, busXPos);
        }


        private int GetRequireSecond()
        {
            double distanceKM = Setting.DISTANCE_M / 1000.0;
            int requireSecond = Convert.ToInt32((distanceKM / Setting.SPEED_KM_PER_H) * 3600);

            return requireSecond;
        }

        private DateTime GetBusStartTime(DateTime date)
        {
            int hour = date.Hour;
            int minute = 0;

            if (date.Minute < 20)
            {
                minute = 20;
            } else if (date.Minute < 40)
            {
                minute = 40;
            } else if (date.Minute < 60)
            {
                hour += 1;
                minute = 0;
            }

            date = date.AddHours(hour - date.Hour);
            date = date.AddMinutes(minute - date.Minute);
            date = date.AddSeconds(0 - date.Second);
            date = date.AddMilliseconds(0 - date.Millisecond);

            return date;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canvasWidth = e.NewSize.Width * (5 / 7.0);
        }
    }
}
