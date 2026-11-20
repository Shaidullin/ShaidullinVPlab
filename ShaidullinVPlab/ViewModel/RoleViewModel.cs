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
        private void RefreshRoles()
        {
            ListRole.Clear();

            foreach (Role role in GetRoles())
            {
                ListRole.Add(role);
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
                    Role newRole = new Role();
                    WindowNewRole wnRole = new WindowNewRole()
                    {
                        Title = "Новая должность",
                    };
                    wnRole.DataContext = new NewRoleViewModel(newRole, wnRole);
                    if (wnRole.ShowDialog() == true)
                    {
                        using (var context = new CompanyEntities())
                        {
                            context.Roles.Add(newRole);
                            context.SaveChanges();

                            RefreshRoles();

                            SelectedRole = ListRole.FirstOrDefault(r => r.Id == newRole.Id);

                        }
                    }
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
                    Role selected = SelectedRole;
                    if (selected == null)
                        return;
                    Role editRole = new Role()
                    {
                        Id = selected.Id,
                        NameRole = selected.NameRole
                    };
                    WindowNewRole wnRole = new WindowNewRole
                    {
                        Title = "Редактирование должности",
                    };
                    wnRole.DataContext = new NewRoleViewModel(editRole, wnRole);
                    if (wnRole.ShowDialog() == true)
                    {
                        using (var context = new CompanyEntities())
                        {
                            Role role = context.Roles.Find(editRole.Id);
                            if (role != null)
                            {
                                if (role.NameRole != editRole.NameRole)
                                    role.NameRole = editRole.NameRole.Trim();
                                context.SaveChanges();
                                RefreshRoles();
                                SelectedRole = ListRole.FirstOrDefault(
                                    r => r.Id == editRole.Id);
                            }
                        }
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
                    Role selected = SelectedRole;
                    if (selected == null)
                        return;
                    MessageBoxResult result = MessageBox.Show(
                        "Удалить данные по должности: " + selected.NameRole,
                        "Предупреждение",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        using (var context = new CompanyEntities())
                        {
                            Role delRole = context.Roles.Find(selected.Id);

                            if (delRole != null)
                            {
                                context.Roles.Remove(delRole);
                                context.SaveChanges();

                                ListRole.Remove(selected);
                                SelectedRole = null;
                            }
                        }
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
            ListRole = new ObservableCollection<Role>();
            // Загрузка данных по должностям сотрудников
            ListRole = GetRoles();

        }
        public ObservableCollection<Role> GetRoles()
        {
            // Вместо new CompanyEntities() передаём строку подключения напрямую
            using (var context = new CompanyEntities())
            {
                var roles = context.Roles.ToList();
                return new ObservableCollection<Role>(roles);
            }
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
