using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PullRefreshUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<string> data;
        int offset = 50;

        public MainPage()
        {
            this.InitializeComponent();

            RefreshPanel.Height = offset;
            data = new ObservableCollection<string>();
            for (int i = 0; i < 20; i++)
                data.Add($"item:{i}");
            DataList.ItemsSource = data;
            TimeText.Text = DateTime.Now.ToString();
        }

        private void ScrollView_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollView.ChangeView(null, offset, null);
        }

        private async void ScrollView_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ScrollViewer scroll = sender as ScrollViewer;
            if (!e.IsIntermediate)
            {
                //停止捲動
                if (scroll.VerticalOffset == 0)
                {
                    //pull refresh
                    UpdateRefresh("載入中...", true, null);
                    TimeText.Text = DateTime.Now.ToString();
                    await Task.Delay(500);
                    for (int i = 0; i < 3; i++)
                        data.Insert(0, $"new item{i}");

                    //finish pull refresh
                    UpdateRefresh("下拉更新", false, new SymbolIcon(Symbol.Download));
                    scroll.ChangeView(null, offset, null);
                }
                else if (scroll.VerticalOffset < offset)
                    scroll.ChangeView(null, offset, null);
            }
            else if (scroll.VerticalOffset <= offset)
            {
                if (scroll.VerticalOffset == 0)
                    UpdateRefresh("放開以更新", false, new SymbolIcon(Symbol.Upload));
                else
                    UpdateRefresh("下拉更新", false, new SymbolIcon(Symbol.Download));
            }
        }

        /// <summary>
        /// update refresh UI
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="isActive">isActive</param>
        /// <param name="icon">icon</param>
        private void UpdateRefresh(string text, bool isActive, SymbolIcon icon)
        {
            StateText.Text = text;
            Progress.IsActive = isActive;
            ArrowButton.Icon = icon;
        }
    }
}
