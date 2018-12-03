using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Security.Credentials;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ISC_Project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Password : Page
    {
        Random rnd = new Random();
        PasswordVault vault = new PasswordVault();
        string password = "";
        int counter = 3;

        public Password()
        {
            this.InitializeComponent();

            CreateGrid(7);
        }



        private async void CreateGrid(int generatedTiles)
        {
            // Setting our grid background
            StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(ApplicationData.Current.LocalSettings.Values["ImageName"].ToString());
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(stream);
                Tiles.Background = new ImageBrush() {ImageSource = bitmap};
            }

            Button[] btn = new Button[generatedTiles];
            int count = 1;
            int column = 0;
            int row = 0;
            double left = 0, right = 0;
            for (int i = 0; i < btn.Length; i++)
            {
                btn[i] = new Button();
                btn[i].Tag = count;
                btn[i].Content = count;
                // 1920 is the width in pixels of my screen. This is for testing purposes. I wouldn't hardcode this
                btn[i].Width = 1920 / (generatedTiles + 1);
                btn[i].Height = 1080;
                btn[i].VerticalAlignment = VerticalAlignment.Top;
                btn[i].HorizontalAlignment = HorizontalAlignment.Left;
                btn[i].Margin = new Thickness(left, right, 0, 0);
                btn[i].Click += Btn_Click;
                
                column++;
                row++;

                Tiles.Children.Add(btn[i]);
                Grid.SetColumn(btn[i], column);
                Grid.SetRow(btn[i], row);
                count++;
                left += (1920 / (generatedTiles + 1));
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            password += btn.Tag;
            counter--;
            if (counter == 0)
            {
                Login();
            }
        }

        private async void Login()
        {
            vault.Add(new PasswordCredential("ISC", ApplicationData.Current.LocalSettings.Values["Name"].ToString(), password));

            CoreApplicationView view = CoreApplication.CreateNewView();
            int newViewId = 0;
            await view.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // C# Lambda Expression

                Frame frame = new Frame();
                frame.Navigate(typeof(Login));
                Window.Current.Content = frame;
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            await ApplicationViewSwitcher.SwitchAsync(newViewId);
        }
    }
}
