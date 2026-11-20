using JetBrains.Annotations;
using ShaidullinVPlab.Helper;
using ShaidullinVPlab.Model;
using ShaidullinVPlab.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using System.IO;

namespace ShaidullinVPlab.ViewModel
{
    public class PersonViewModel : INotifyPropertyChanged
    {
        readonly string path = @"C:\Users\Рафкат\source\repos\ShaidullinVPlab\ShaidullinVPlab\DataModels\PersonData.json";
        string _jsonPersons = String.Empty;
        public string Error { get; set; }
        public ObservableCollection<Person> LoadPerson()
        {
            _jsonPersons = File.ReadAllText(path);
            if (_jsonPersons != null)
            {
                ListPerson = JsonConvert.DeserializeObject<ObservableCollection<Person>>(_jsonPersons);
                return ListPerson;
            }
            else
            {
                return null;
            }
        }
        private void SaveChanges(ObservableCollection<Person> listPersons)
        {
            var jsonPerson = JsonConvert.SerializeObject(listPersons);
            try
            {
                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.Write(jsonPerson);
                }
            }
            catch (IOException e)
            {
                Error = "Ошибка записи json файла /n" + e.Message;
            }
        }

        private Person _selectedPerson;

        /// <summary>
        /// Выделенные в списке данные по сотруднику
        /// </summary>
        public Person SelectedPerson
        {
            get { return _selectedPerson; }
            set
            {
                _selectedPerson = value;
                OnPropertyChanged("SelectedPerson");
            }
        }
        public ObservableCollection<Person> ListPerson { get; set; } = new ObservableCollection<Person>();
        public PersonViewModel()
        {
            ListPerson = new ObservableCollection<Person>();
            ListPerson = GetPersons();
        }
        private ObservableCollection<Person> GetPersons()
        {
            using (var context = new CompanyEntities())
            {
                var query = from per in context.Persons
                .Include("Role")
                            orderby per.LastName
                            select per;
                if (query.Count() != 0)
                {
                    foreach (var p in query)
                    {
                        ListPerson.Add(p);
                    }
                }
            }
            return ListPerson;
        }


        public int MaxId()
        {
            int max = 0;
            foreach (var r in this.ListPerson)
            {
                if (max < r.Id)
                {
                    max = r.Id;
                }
                ;
            }
            return max;
        }

        private RelayCommand addPerson;
        /// <summary>
        /// добавление сотрудника
        /// </summary>
        public RelayCommand AddPerson
        {
            get
            {
                return addPerson ??
                (addPerson = new RelayCommand(obj =>
                {
                    Person newPerson = new Person
                    {
                        Birthday = DateTime.Now
                    };
                    WindowNewEmployee wnPerson = new WindowNewEmployee
                    {
                        Title = "Новый сотрудник"
                    };
                    RoleViewModel vmRole = new RoleViewModel();
                    wnPerson.CbRole.ItemsSource = vmRole.ListRole;
                    wnPerson.CbRole.DisplayMemberPath = "NameRole";
                    wnPerson.DataContext = new NewPesronViewModel(newPerson, wnPerson);
                    if (wnPerson.ShowDialog() == true)
                    {
                        using (var context = new CompanyEntities())
                        {
                            context.Persons.Add(newPerson);
                            context.SaveChanges();

                            ListPerson.Clear();
                            ListPerson = GetPersons();
                        }
                    }
                },
                (obj) => true));
            }
        }

        private RelayCommand editPerson;
        public RelayCommand EditPerson
        {
            get
            {
                return editPerson ??
                    (editPerson = new RelayCommand(obj =>
                    {
                        Person editPerson = SelectedPerson;
                        WindowNewEmployee wnPerson = new WindowNewEmployee
                        {
                            Title = "Редактирование данных сотрудника",
                        };
                        wnPerson.DataContext = new NewPesronViewModel(editPerson, wnPerson);
                        if (wnPerson.ShowDialog() == true)
                        {
                            using (var context = new CompanyEntities())
                            {
                                Person person = context.Persons.Find(editPerson.Id);

                                if (person != null)
                                {
                                    if (person.RoleId != editPerson.RoleId)
                                        person.RoleId = editPerson.RoleId;

                                    if (person.FirstName != editPerson.FirstName)
                                        person.FirstName = editPerson.FirstName;

                                    if (person.LastName != editPerson.LastName)
                                        person.LastName = editPerson.LastName;

                                    if (person.Birthday != editPerson.Birthday)
                                        person.Birthday = editPerson.Birthday;
                                    context.SaveChanges();
                                    ListPerson.Clear();
                                    ListPerson = GetPersons();
                                }
                            }
                        }
                        else
                        {
                            ListPerson.Clear();
                            ListPerson = GetPersons();
                        }
                    }, (obj) => SelectedPerson != null && ListPerson.Count > 0));
            }
        }

        private RelayCommand deletePerson;
        public RelayCommand DeletePerson
        {
            get
            {
                return deletePerson ??
                    (deletePerson = new RelayCommand(obj =>
                    {
                    Person delPerson = SelectedPerson;
                        using (var context = new CompanyEntities())
                        {
                            Person person = context.Persons.Find(delPerson.Id);

                            if (person != null)
                            {
                                MessageBoxResult result = MessageBox.Show(
                                    "Удалить данные по сотруднику: \nФамилия: "
                                    + person.LastName
                                    + "\nИмя: "
                                    + person.FirstName,
                                    "Предупреждение",
                                    MessageBoxButton.OKCancel);

                                if (result == MessageBoxResult.OK)
                                {
                                    try
                                    {
                                        context.Persons.Remove(person);
                                        context.SaveChanges();
                                        ListPerson.Remove(delPerson);
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(
                                            "\nОшибка удаления данных!\n" + ex.Message,
                                            "Предупреждение");
                                    }
                                }
                            }
                        }
                    }, (obj) => SelectedPerson != null && ListPerson.Count > 0));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged; [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
