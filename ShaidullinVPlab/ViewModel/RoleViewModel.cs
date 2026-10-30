using ShaidullinVPlab.Helper;
using ShaidullinVPlab.Model;
using ShaidullinVPlab.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;


namespace ShaidullinVPlab.ViewModel
{
    public class RoleViewModel : INotifyPropertyChanged
    {
        readonly string path = @"C:\Users\Рафкат\source\repos\ShaidullinVPlab\ShaidullinVPlab\DataModels\RoleData.json";
        string _jsonRoles = String.Empty; public string Error { get; set; }
        public ObservableCollection<Role> LoadRole()
        {
            _jsonRoles = File.ReadAllText(path); if (_jsonRoles != null)
            {
                ListRole = JsonConvert.DeserializeObject<ObservableCollection<Role>>(_jsonRoles);
                return ListRole;
            }
            else
            {
                return null;
            }
        }
        private void SaveChanges(ObservableCollection<Role> listRole)
        {
            var jsonRole = JsonConvert.SerializeObject(listRole); try
            {
                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.Write(jsonRole);
                }
            }
            catch (IOException e)
            {
                Error = "Ошибка записи json файла /n" + e.Message;
            }
        }


        private RelayCommand addRole;
        public RelayCommand AddRole
        {
            get
            {
                return addRole ??
                (addRole = new RelayCommand(obj =>
                {
                    // формирование кода новой должности
                    int maxIdRole = MaxId() + 1;
                    Role role = new Role { Id = maxIdRole };
                    WindowNewRole wnRole = new WindowNewRole()
                    {
                        Title = "Новая должность",
                    };
                    wnRole.DataContext = new NewRoleViewModel(role, wnRole);
                    if (wnRole.ShowDialog() == true)
                    {
                        ListRole.Add(role); SaveChanges(ListRole);
                    }
                    SelectedRole = role;
                }));
            }
        }
        private RelayCommand editRole;
        public RelayCommand EditRole
        {
            get
            {
                return editRole ??
                (editRole = new RelayCommand(obj =>
                {
                    Role role = SelectedRole;
                    Role tempRole = new Role(); tempRole = role.ShallowCopy();
                    WindowNewRole wnRole = new WindowNewRole()
                    {
                        Title = "Редактирование должности",
                    };
                    wnRole.DataContext = new NewRoleViewModel(tempRole, wnRole); if (wnRole.ShowDialog() == true)
                    {
                        // сохранение данных в оперативной памяти
                        role.NameRole = tempRole.NameRole; SaveChanges(ListRole);
                    }
                }, (obj) => SelectedRole != null && ListRole.Count > 0));
            }
        }
        private RelayCommand deleteRole;
        public RelayCommand DeleteRole
        {
            get
            {
                return deleteRole ??
                (deleteRole = new RelayCommand(obj =>
                {
                    Role role = SelectedRole;
                    MessageBoxResult result = MessageBox.Show("Удалить данные по должности: " +
    role.NameRole, "Предупреждение", MessageBoxButton.OKCancel,
    MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        ListRole.Remove(role); SaveChanges(ListRole);
                    }
                }, (obj) => SelectedRole != null && ListRole.Count > 0));
            }
        }


        private Role selectedRole;
        public Role SelectedRole
        {
            get
            {
                return selectedRole;
            }
            set
            {
                selectedRole = value; OnPropertyChanged("SelectedRole");
                EditRole.CanExecute(true);
            }
        }

        public ObservableCollection<Role> ListRole { get; set; } = new ObservableCollection<Role>();
        public RoleViewModel()
        {
            ListRole = LoadRole();
        }

        public int MaxId()
        {
            int max = 0;
            foreach (var r in this.ListRole)
            {
                if (max < r.Id)
                {
                    max = r.Id;
                }
                ;
            }
            return max;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
