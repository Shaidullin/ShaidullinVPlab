using ShaidullinVPlab.Helper;
using ShaidullinVPlab.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ShaidullinVPlab.ViewModel
{
    public class NewPesronViewModel : INotifyPropertyChanged
    {
        private readonly PersonDPO personDPO;
        private readonly Window window;

        public NewPesronViewModel(PersonDPO personDPO, Window window)
        {
            this.personDPO = personDPO;
            this.window = window;
            Id = personDPO.Id;
            RoleName = personDPO.RoleName;
            FirstName = personDPO.FirstName;
            LastName = personDPO.LastName;
            Birthday = personDPO.Birthday;
            RoleViewModel vmRole = new RoleViewModel();
            Roles = vmRole.ListRole;
            SelectedRole = vmRole.ListRole.FirstOrDefault(r => r.NameRole == personDPO.RoleName);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }
        public ObservableCollection<Role> Roles { get; }

        private Role selectedRole;
        public Role SelectedRole
        {
            get => selectedRole;
            set { selectedRole = value; OnPropertyChanged(); }
        }
        private int id;
        public int Id
        {
            get => id;
            set { id = value; OnPropertyChanged(); }
        }
        private string firstName;
        public string FirstName
        {
            get => firstName;
            set { firstName = value; OnPropertyChanged(); }
        }
        private string lastName;
        public string LastName
        {
            get => lastName;
            set { lastName = value; OnPropertyChanged(); }
        }
        private DateTime birthday;
        public DateTime Birthday
        {
            get => birthday;
            set { birthday = value; OnPropertyChanged(); }
        }
        private string roleName;
        public string RoleName
        {
            get => roleName;
            set { roleName = value; OnPropertyChanged(); }
        }
        private string roleId;
        public string RoleId
        {
            get => roleId;
            set { roleId = value; OnPropertyChanged(); }
        }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }


        private void Save(object parameter)
        {
            // Записываем данные обратно в модель
            if (SelectedRole != null)
                personDPO.RoleName = SelectedRole.NameRole;

            personDPO.FirstName = FirstName;
            personDPO.LastName = LastName;
            personDPO.Birthday = Birthday;

            // Устанавливаем DialogResult = true и закрываем окно
            window.DialogResult = true;
        }

        private void Cancel(object parameter)
        {
            // Закрываем окно без сохранения
            window.DialogResult = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
