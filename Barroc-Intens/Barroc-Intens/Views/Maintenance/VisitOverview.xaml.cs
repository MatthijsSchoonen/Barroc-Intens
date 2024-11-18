using Barroc_Intens.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens.Views.Maintenance
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VisitOverview : Page
    {
        User LoggedInUser = LocalStore.GetLoggedInUser();
        List<DateTime> DaysOfTheWeek = new();

        public VisitOverview()
        {
            this.InitializeComponent();
            RnLoggedInUser.Text = LoggedInUser.Name.ToString();
            DaysOfTheWeek = CalculateDaysOfTWeek();
            GvWeekOverview.ItemsSource = DaysOfTheWeek;


        }

        private List<DateTime> CalculateDaysOfTWeek()
        {
            DateTime currentDate = DateTime.Now;

            // Configurable start
            DayOfWeek weekStart = DayOfWeek.Monday;

            // Calculate the start of the week
            DateTime startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)weekStart);

            // Checks if the current day of the week is before the start day of the week.
            // Would mean the startOfWeek is set to snext week, so: substract 7 days from it.
            if (currentDate.DayOfWeek < weekStart)
            {
                startOfWeek = startOfWeek.AddDays(-7);
            }

            // Add all days into array and returns it.
            List<DateTime> daysOfTheWeek = new();
            for (int i = 0; i < 7; i++)
            {
                daysOfTheWeek.Add(startOfWeek.AddDays(i));
            }
            return daysOfTheWeek;
            //foreach (DateTime day in daysOfTheWeek)
            //{
            //    Debug.WriteLine($"{day.ToString("dddd")}: {day.ToString("dd MMMM yyyy")}");
            //}
        }
    }
}
