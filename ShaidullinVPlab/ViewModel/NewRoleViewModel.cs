using ShaidullinVPlab.Helper;
using ShaidullinVPlab.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ShaidullinVPlab.ViewModel
{
    public class NewRoleViewModel : INotifyPropertyChanged
    {
        private readonly Role role;
        private readonly Window window;

        public NewRoleViewModel(Role role, Window window)
        {
            this.role = role;
            this.window = window;
            Id = role.Id;
            NameRole = role.NameRole;
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }
        private int id;
        public int Id
        {
            get => id;
            set { id = value; OnPropertyChanged(); }
        }

        private string nameRole;
        public string NameRole
        {
            get => nameRole;
            set { nameRole = value; OnPropertyChanged(); }
        }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }


        private void Save(object parameter)
        {
            // Записываем данные обратно в модель
            role.NameRole = NameRole;

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
