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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens.Sales
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NotePage : Page
    {
        private readonly ObservableCollection<Note> NotesCollection;
        public NotePage()
        {
            this.InitializeComponent();
            NotesCollection = new ObservableCollection<Note>();
            NotesListView.ItemsSource = NotesCollection;
            LoadNotes();
            LoadCompaniesAsync();
        }


        private async Task LoadCompaniesAsync()
        {
            //connect to the database get the companies
            using (AppDbContext context = new AppDbContext())
            {
                var companies = await context.Companies.ToListAsync();
                CompanyComboBox.ItemsSource = companies; 
                CompanyComboBox.DisplayMemberPath = "Name"; 
                CompanyComboBox.SelectedValuePath = "Id"; 
            }
        }

        private void LoadNotes()
        {
            //load notes from the databse
            using (AppDbContext context = new AppDbContext())
            {
                if (User.LoggedInUser == null)
                { 
                    return;
                }
 
                var companies = User.LoggedInUser.Companies;

                int userId = User.LoggedInUser.Id;
    
                var notes = context.Notes
                    .Where(note =>  note.UserId == userId)
                    .ToList(); 

                NotesCollection.Clear();
                foreach (var note in notes)
                {
                    NotesCollection.Add(note);
                }
            }
        }


        private async void SaveNoteButton_Click(object sender, RoutedEventArgs e)
        {
            //create a new note
            string content = ContentInput.Text;
            DateTime date = DatePickerInput.Date.DateTime;

            if (User.LoggedInUser == null)
            {             
                return;
            }

            
            var selectedCompany = CompanyComboBox.SelectedItem as Company;
            int? companyId = selectedCompany?.Id;

            var newNote = new Note
            {
                Content = content,
                Date = date,
                CompanyId = companyId, 
                UserId = User.LoggedInUser.Id
            };

            //save to note to database
            using (var context = new AppDbContext())
            {
                context.Notes.Add(newNote);
                await context.SaveChangesAsync();
            }

            NotesCollection.Add(newNote);
            ContentInput.Text = string.Empty;
            DatePickerInput.Date = DateTime.Now;
            CompanyComboBox.SelectedItem = null; 
        }


    }
}

