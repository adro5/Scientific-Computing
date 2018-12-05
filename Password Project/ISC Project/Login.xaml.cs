using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Security.Credentials;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ISC_Project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login : Page
    {
        PasswordVault vault = new PasswordVault();
        string password = "";
        int counter = 4;
        int triesRemaining = 5;
        ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        public Login()
        {
            this.InitializeComponent();
            btnLogin.Click += BtnLogin_ClickAsync;
        }

        private async void BtnLogin_ClickAsync(object sender, RoutedEventArgs e)
        {
            if (Name.Visibility == Visibility.Visible)
            {
                Name.Visibility = Visibility.Collapsed;
                btnLogin.Visibility = Visibility.Collapsed;
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(settings.Values["ImageName"].ToString());
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.SetSource(stream);
                    Tiles.Background = new ImageBrush() { ImageSource = bitmap };
                }

                CreateGrid();
            }
        }

        private void CreateGrid()
        {
            Button[] btn = new Button[7];
            int count = 1;
            int column = 0;
            int row = 0;
            double left = 0, right = 0;
            for (int i = 0; i < btn.Length; i++)
            {
                btn[i] = new Button();
                btn[i].Tag = count;
                // 1920 is the width in pixels of my screen. This is for testing purposes. I wouldn't hardcode this
                btn[i].Width = 1920 / 8;
                btn[i].Height = 1080;
                btn[i].VerticalAlignment = VerticalAlignment.Top;
                btn[i].HorizontalAlignment = HorizontalAlignment.Left;
                btn[i].Margin = new Thickness(left, right, 0, 0);
                btn[i].Click += Login_Click;

                column++;
                row++;

                Tiles.Children.Add(btn[i]);
                Grid.SetColumn(btn[i], column);
                Grid.SetRow(btn[i], row);
                count++;
                left += (1920 / 8);
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            password += btn.Tag;
            counter--;
            PasswordCredential passwordCredential;
            if (counter == 0)
            {
                var credentials = vault.FindAllByUserName(settings.Values["Name"].ToString());

                // For these purposes, we should only have one name that matches
                if (credentials.Count == 1)
                {
                    passwordCredential = credentials[0];
                    passwordCredential.RetrievePassword();
                    if (passwordCredential.Password == password)
                        await Navigate();
                }
            }
            if (settings.Values["Lock"].ToString() == "true" && counter == 0)
            {
                if (triesRemaining == 0)
                {
                    var message = new MessageDialog("Please contact administrator or enter back up password", "Account Locked");
                    await message.ShowAsync();
                    password = "";
                    // Backup Password Dialog
                    Backup backup = new Backup()
                    {
                        PrimaryButtonText = "Login",
                        SecondaryButtonText = "Cancel"
                    };
                    ContentDialogResult dialogResult = await backup.ShowAsync();
                    while (dialogResult != ContentDialogResult.Secondary)
                    {
                        // Checking if backup password is a match
                        if (dialogResult == ContentDialogResult.Primary && backup.result == true)
                        {
                            await Navigate();
                            break;
                        }
                        dialogResult = await backup.ShowAsync();
                    }
                }
                else if (triesRemaining > 0)
                {
                    password = "";
                    triesRemaining--;
                    counter = 4;

                }
            }
        }


        private async Task Navigate()
        {
            CoreApplicationView view = CoreApplication.CreateNewView();
            int newViewId = 0;
            await view.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // C# Lambda Expression

                Frame frame = new Frame();
                frame.Navigate(typeof(Success));
                Window.Current.Content = frame;
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            await ApplicationViewSwitcher.SwitchAsync(newViewId);
        }
    }
}
