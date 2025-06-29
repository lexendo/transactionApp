using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace NotSoEpicApp
{
    public partial class SupervisedUsers : Page, INotifyPropertyChanged
    {
        ObservableCollection<SupervisedUser> _allUsers = new ObservableCollection<SupervisedUser>();
        ObservableCollection<SupervisedUser> _filteredUsers = new ObservableCollection<SupervisedUser>();


        public ObservableCollection<SupervisedUser> FilteredUsers
        {
            get => _filteredUsers;
            set { _filteredUsers = value; OnPropertyChanged(nameof(FilteredUsers)); }
        }

        public SupervisedUsers()
        {
            InitializeComponent();
            this.DataContext = this;
            LoadUsers();
        }

        private async void LoadUsers()
        {
            var users = await Database.GetSupervisedUsersWithPermissions();
            _allUsers = new ObservableCollection<SupervisedUser>(users);
            FilteredUsers = new ObservableCollection<SupervisedUser>(_allUsers);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();
            FilteredUsers = new ObservableCollection<SupervisedUser>(
                _allUsers.Where(u => u.Username.ToLower().Contains(query)));
        }

        private void ManageUser_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string username)
            {
                MessageBox.Show($"Spravovanie používateľa: {username}");
                // Môžeš tu otvoriť detail používateľa alebo navigovať na inú stránku
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void MyAccount_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new ManageAccount());
        }

        private void SupervisedUsersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
