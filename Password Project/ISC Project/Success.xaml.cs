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
using System.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace ISC_Project
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Success : Page
    {
        public Success()
        {
            this.InitializeComponent();
            Title.Text += " " + ApplicationData.Current.LocalSettings.Values["Name"].ToString();
            Calc2.Text = "Time to crack password 1234: " + ((0.034 * Math.Pow(10, 4)) / 2).ToString() + " seconds";
            Calc3.Text = "Time to crack password something$: " + ((0.034 * Math.Pow(36, 10)) / 2).ToString() + " seconds or " + ((0.034 * Math.Pow(36, 10)) / 435_456_000).ToString() + "years";
            Calc4.Text = "Time to crack password something9$: " + ((0.034 * Math.Pow(46, 11)) / 2).ToString() + " seconds or " + ((0.034 * Math.Pow(46, 11)) / 435_456_000).ToString() + " years";
            Calc.Text = "Time to crack picture password if backup password is enabled includes multiple things: \n" +
                "Time to find screen resolution, time to find the exact time the password was chosen,\n" +
                "time to find username, time to find number of password grid locations, time to crack the password itself/time to crack the backup";
        }
    }
}
