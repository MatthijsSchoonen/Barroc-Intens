using Barroc_Intens.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens.Views.Maintenance
{
    public sealed partial class VisitOverview : Page
    {
        private User LoggedInUser = LocalStore.GetLoggedInUser();
        private ObservableCollection<MaintenanceDaySchedule> MaintenanceDaySchedules = new ObservableCollection<MaintenanceDaySchedule>();
        private ObservableCollection<MaintenanceAppointment> Visits = new ObservableCollection<MaintenanceAppointment>();

        public VisitOverview()
        {
            this.InitializeComponent();
            PreparePage();
        }

        private void PreparePage()
        {
            RnLoggedInUser.Text = LoggedInUser.Name.ToString();
            SetVisits();
            MaintenanceDaySchedules = CreateDaySchedules(CalculateDaysOfTWeek());
            Debug.WriteLine($"MaintenanceDaySchedules count: {MaintenanceDaySchedules.Count}");
            foreach (MaintenanceDaySchedule schedule in MaintenanceDaySchedules)
            {
                Debug.WriteLine($"Day: {schedule.DayOfTheWeek}, Appointments: {schedule.Appointments.Count}");
                foreach(MaintenanceAppointment app in schedule.Appointments)
                {
                    Debug.WriteLine(app.Title);
                    Debug.WriteLine(app.Company.Address);
                }
            }
            GvWeekOverview.ItemsSource = MaintenanceDaySchedules;
        }

        private void SetVisits()
        {
            using (AppDbContext dbContext = new AppDbContext())
            {
                List<MaintenanceAppointment> visits = dbContext.MaintenanceAppointments.Where(m => m.User.Id == LoggedInUser.Id).Include(m => m.Company).ToList();
                foreach (MaintenanceAppointment visit in visits)
                {
                    Visits.Add(visit);
                }
            }
        }

        private List<DateTime> CalculateDaysOfTWeek()
        {
            DateTime currentDate = DateTime.Now;
            DayOfWeek weekStart = DayOfWeek.Monday;
            DateTime startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)weekStart);
            if (currentDate.DayOfWeek < weekStart)
            {
                startOfWeek = startOfWeek.AddDays(-7);
            }

            List<DateTime> daysOfTheWeek = new List<DateTime>();
            for (int i = 0; i < 7; i++)
            {
                daysOfTheWeek.Add(startOfWeek.AddDays(i));
            }
            return daysOfTheWeek;
        }

        private ObservableCollection<MaintenanceDaySchedule> CreateDaySchedules(List<DateTime> daysOfTheWeek)
        {
            ObservableCollection<MaintenanceDaySchedule> daySchedules = new ObservableCollection<MaintenanceDaySchedule>();
            foreach (DateTime day in daysOfTheWeek)
            {
                List<MaintenanceAppointment> visitsOfTheDay = Visits.Where(v => v.StartTime.Day == day.Day).ToList();
                MaintenanceDaySchedule daySchedule = new MaintenanceDaySchedule
                {
                    Year = day.Year,
                    Month = day.Month,
                    DayOfTheWeek = day.DayOfWeek,
                    DayOfTheMonth = day.Day,
                    MonthInText = day.ToString("MMMM"),
                    DateTimeObject = day,
                };

                foreach (MaintenanceAppointment visit in visitsOfTheDay)
                {
                    daySchedule.Appointments.Add(visit);
                }
                daySchedules.Add(daySchedule);
            }
            return daySchedules;
        }

        private void LvAppointments_ItemClick(object sender, ItemClickEventArgs e)
        {
            MaintenanceAppointment selectedItem = e.ClickedItem as MaintenanceAppointment;
            Debug.WriteLine($"Selecteditem: {selectedItem} | Frame: {Frame}");
            Frame.Navigate(typeof(VisitDetails),selectedItem);
        }
    }
}
