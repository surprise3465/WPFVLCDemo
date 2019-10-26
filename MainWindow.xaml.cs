using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Vlc.DotNet.Wpf;

namespace VLCPlayer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string path = AppDomain.CurrentDomain.BaseDirectory;
        private DispatcherTimer videotimer = new DispatcherTimer();
        private readonly DirectoryInfo vlcLibDirectory;
        private VlcControl vlcControl;
        public MainWindow()
        {
            InitializeComponent();

            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            // Default installation path of VideoLAN.LibVLC.Windows
            vlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, "..","..","build", IntPtr.Size == 4 ? "x86" : "x64"));
        }
        private void palyfileBtn_Click(object sender, RoutedEventArgs e)
        {
            vlcControl?.Dispose();
            vlcControl = new VlcControl();
            ControlContainer.Content = vlcControl;
            vlcControl.SourceProvider.CreatePlayer(vlcLibDirectory);
            vlcControl.SourceProvider.MediaPlayer.Play(new Uri("rtmp://58.200.131.2:1935/livetv/hunantv"));
            //FileInfo fileInfo = new FileInfo(@"E:\2.avi");
            //vlcControl.SourceProvider.MediaPlayer.Play(fileInfo);
            Videostatusclock();
        }

        private void stopPlayBtn_Click(object sender, RoutedEventArgs e)
        {
            vlcControl?.Dispose();
            vlcControl = null;
            videotimer.Stop();
        }

        private void pauseBtn_Click(object sender, RoutedEventArgs e)
        {
            vlcControl.SourceProvider.MediaPlayer.Pause();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            vlcControl.SourceProvider.MediaPlayer.Rate *= (float)0.5;
        }

        private void forwardBtn_Click(object sender, RoutedEventArgs e)
        {
            if (vlcControl == null)
            {
                return;
            }

            vlcControl.SourceProvider.MediaPlayer.Rate *= 2;
        }

        private void addURLBtn_Click(object sender, RoutedEventArgs e)
        {
            string mrl = fileNametextBox.Text;
        }

        private void openfileBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Videostatusclock()
        {
            videotimer.Interval = TimeSpan.FromSeconds(1);
            videotimer.Tick += VideoTimerTick;
            videotimer.Start();
        }

        public static string formatLongToTimeStr(long l)
        {
            int hour = 0;
            int minute = 0;
            int second = 0;

            second = (int)l / 1000;

            if (second > 60)
            {
                minute = second / 60;
                second = second % 60;
            }
            if (minute > 60)
            {
                hour = minute / 60;
                minute = minute % 60;
            }
            return $"{hour:00}:{minute:00}:{second:00}";
        }

        private void VideoTimerTick(object sender, EventArgs e)
        {
            if (vlcControl == null)
            {
                return;
            }

            if (vlcControl.SourceProvider.MediaPlayer.Length != 0)
            {
                currentTime.Content = formatLongToTimeStr(vlcControl.SourceProvider.MediaPlayer.Time);
                totalLength.Content = formatLongToTimeStr(vlcControl.SourceProvider.MediaPlayer.Length);
                sliderVideo.Maximum = vlcControl.SourceProvider.MediaPlayer.Length;
                sliderVideo.Value = vlcControl.SourceProvider.MediaPlayer.Time;
                sliderVolume.Value = vlcControl.SourceProvider.MediaPlayer.Audio.Volume;
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            vlcControl?.Dispose();
            vlcControl = null;
            videotimer.Stop();
        }

        private void sliderVideo_Drag(object sender, DragCompletedEventArgs dragCompletedEventArgs)
        {
            if (vlcControl == null)
            {
                return;
            }

            vlcControl.SourceProvider.MediaPlayer.Time = (long)sliderVideo.Value;
        }

        private void sliderVolume_Drag(object sender, DragCompletedEventArgs dragCompletedEventArgs)
        {
            if (vlcControl == null)
            {
                return;
            }

            vlcControl.SourceProvider.MediaPlayer.Audio.Volume = (int)sliderVolume.Value;
        }

        private void sliderVideo_MouseLefButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (vlcControl == null)
            {
                return;
            }

            vlcControl.SourceProvider.MediaPlayer.Time = (long)sliderVideo.Value;
        }

        private void sliderVolume_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (vlcControl == null)
            {
                return;
            }

            vlcControl.SourceProvider.MediaPlayer.Audio.Volume = (int)sliderVolume.Value;
        }
    }
}
