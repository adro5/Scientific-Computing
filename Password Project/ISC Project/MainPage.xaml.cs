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
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Security.Credentials;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ISC_Project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ApplicationDataContainer settings = ApplicationData.Current.LocalSettings;
        PasswordVault vault = new PasswordVault();
        public MainPage()
        {
            this.InitializeComponent();
            backupPassword.IsEnabled = false;
            filename.Visibility = Visibility.Collapsed;
            fileselect.Visibility = Visibility.Collapsed;
            preview.Visibility = Visibility.Collapsed;

            // Attaching functions to buttons
            btnNext1.Click += BtnNext1_Click;
            lockCheck.Checked += LockCheck_Checked;
            lockCheck.Unchecked += LockCheck_Unchecked;
            fileselect.Click += Fileselect_Click;
            
        }

        private async void Fileselect_Click(object sender, RoutedEventArgs e)
        {
            // File IO
            var filepicker = new FileOpenPicker();
            filepicker.ViewMode = PickerViewMode.Thumbnail;
            filepicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            filepicker.FileTypeFilter.Add(".jpg");
            filepicker.FileTypeFilter.Add(".png");
            filepicker.FileTypeFilter.Add(".jpeg");

            StorageFile file = await filepicker.PickSingleFileAsync();

            if (file != null)
            {
                // Checking if picture exists in local app storage. Will crash if this isn't here or exception isn't handled
                if (ApplicationData.Current.LocalFolder.TryGetItemAsync(file.Name) == null)
                {
                    await file.CopyAsync(ApplicationData.Current.LocalFolder);
                }
                ApplicationData.Current.LocalSettings.Values["ImageName"] = file.Name;

                filename.Text = file.Name;
                BitmapImage bitmap = new BitmapImage();
                StorageFile file2 = await ApplicationData.Current.LocalFolder.GetFileAsync(file.Name.ToString());
                using (IRandomAccessStream fileStream = await file2.OpenAsync(FileAccessMode.Read))
                {
                    bitmap.SetSource(fileStream);
                    preview.Source = bitmap;
                }
            }

        }

        private void LockCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            backupPassword.IsEnabled = false;
        }

        private void LockCheck_Checked(object sender, RoutedEventArgs e)
        {
            backupPassword.IsEnabled = true;

            // Setting a tool tip (the hover thing) for the password box
            ToolTip toolTip = new ToolTip();
            toolTip.Content = "Passwords must have:\n\n" +
                "At least on of these characters: !, @, #, $, %, ^, &, *, (, )\n" +
                "At least 8 characters";
            ToolTipService.SetToolTip(backupPassword, toolTip);
        }

        private async void BtnNext1_Click(object sender, RoutedEventArgs e)
        {

            if (filename.Visibility == Visibility.Collapsed)
            {
                try
                {
                    if (!String.IsNullOrWhiteSpace(userCreate.Text))
                    {
                        // Add User's Name to Local Settings
                        settings.Values["Name"] = userCreate.Text;

                        //Add Back Up Credentials to local settings and check if valid password
                        if (backupPassword.IsEnabled && !String.IsNullOrWhiteSpace(backupPassword.Password) && checkValidPassword(backupPassword.Password))
                        {
                            settings.Values["Backup"] = backupPassword.Password;
                        }
                        else if (backupPassword.IsEnabled && (String.IsNullOrWhiteSpace(backupPassword.Password) || !checkValidPassword(backupPassword.Password)))
                        {
                            // Getting fancy with custom exceptions
                            throw new InvalidPasswordException("Invalid Password");
                        }
                    }
                    else
                    {
                        throw new InvalidNameException("Invalid Name");
                    }

                    if (lockCheck.IsChecked == true)
                    {
                        settings.Values["Lock"] = true;
                    }
                    else
                    {
                        settings.Values["Lock"] = false;
                    }

                    filename.Visibility = Visibility.Visible;
                    fileselect.Visibility = Visibility.Visible;
                    preview.Visibility = Visibility.Visible;
                    backupPassword.Visibility = Visibility.Collapsed;
                    userCreate.Visibility = Visibility.Collapsed;
                    lockCheck.Visibility = Visibility.Collapsed;

                    Title.Text = "Let's Pick a Picture";
                    }
                catch (Exception ex)
                {
                    if (ex is InvalidPasswordException)
                    {
                        retrieveMsgDialog("Please enter a valid backup password", ex.Message);
                    }
                    else if (ex is InvalidNameException)
                    {
                        retrieveMsgDialog("Please enter a valid name", ex.Message);
                    }
                }
            }
            else
            {
                // Navigating to the next page, Password Creation
                CoreApplicationView view = CoreApplication.CreateNewView();
                int newViewId = 0;
                await view.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    // C# Lambda Expression

                    Frame frame = new Frame();
                    frame.Navigate(typeof(Password));
                    Window.Current.Content = frame;
                    Window.Current.Activate();

                    newViewId = ApplicationView.GetForCurrentView().Id;
                });
                await ApplicationViewSwitcher.SwitchAsync(newViewId);
            }
        }

        private bool checkValidPassword(string password)
        {
            // Checks if password has any special characters and has the proper length

            /* Fun fact: Back in the early days of computing, some processor architectures took a slight 
             * performance penalty for using >= instead of > because of how some instruction sets were implemented.
             * If you know assembly, in the MIPS architecture, the instructions, ble and bge (Branch on Less than/Greater than)
             * allowed devs to use >= and <= without performance penalty. These didn't exist in certain older architectures
             * It doesn't matter if I use > 7 or >= 8 nowadays but I'm using it just to tell a story 
             */
            if (password.IndexOfAny("!@#$%^&*()".ToCharArray()) != -1 && password.Length > 7)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async void retrieveMsgDialog(string content, string title)
        {
            MessageDialog message = new MessageDialog(content, title);
            await message.ShowAsync();
        }
    }
}
