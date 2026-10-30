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

        private PersonDPO selectedPersonDPO;
        /// <summary>
        /// выделенные в списке данные по сотруднику
        /// </summary>
        public PersonDPO SelectedPersonDPO
        {
            get { return selectedPersonDPO; }
            set
            {
                selectedPersonDPO = value;
                OnPropertyChanged("SelectedPersonDPO");
            }
        }
        public ObservableCollection<Person> ListPerson { get; set; } = new ObservableCollection<Person>();
        public ObservableCollection<PersonDPO> ListPersonDPO { get; set; } = new ObservableCollection<PersonDPO>();
        public PersonViewModel()
        {
            ListPerson = new ObservableCollection<Person>();
            ListPersonDPO = new ObservableCollection<PersonDPO>();
            ListPerson = LoadPerson();
            ListPersonDPO = GetListPersonDPO();
        }
        public ObservableCollection<PersonDPO> GetListPersonDPO()
        {
            foreach (var person in ListPerson)
            {
                PersonDPO p = new PersonDPO(); p = p.CopyFromPerson(person); ListPersonDPO.Add(p);
            }
            return ListPersonDPO;
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
                    WindowNewEmployee wnPerson = new WindowNewEmployee
                    {
                        Title = "Новый сотрудник"
                    };
                    // формирование кода нового собрудника
                    int maxIdPerson = MaxId() + 1; PersonDPO per = new PersonDPO
                    {
                        Id = maxIdPerson,
                        Birthday = DateTime.Now.ToString(),
                    };
                    RoleViewModel vmRole = new RoleViewModel();
                    wnPerson.CbRole.ItemsSource = vmRole.ListRole;
                    wnPerson.CbRole.DisplayMemberPath = "NameRole";
                    wnPerson.DataContext = new NewPesronViewModel(per, wnPerson);
                    if (wnPerson.ShowDialog() == true)
                    {
                        Role r = (Role)wnPerson.CbRole.SelectedValue; per.RoleName = r.NameRole;
                        per.Birthday = PersonDPO.GetStringBirthday(per.Birthday);
                        ListPersonDPO.Add(per);
                        // добавление нового сотрудника в коллекцию ListPerson<Person>
                        Person p = new Person();
                        p = p.CopyFromPersonDpo(per); ListPerson.Add(p);
                        try
                        {
                            // сохранение изменений в файле json
                            SaveChanges(ListPerson);
                        }
                        catch (Exception e)
                        {
                            Error = "Ошибка добавления данных в json файл\n" +
                            e.Message;
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
                        WindowNewEmployee wnPerson = new WindowNewEmployee()
                        {
                            Title = "Редактирование данных сотрудника",
                        };
                        PersonDPO personDPO = SelectedPersonDPO;
                        PersonDPO tempPerson = new PersonDPO();
                        tempPerson = personDPO.ShallowCopy();
                        wnPerson.DataContext = new NewPesronViewModel(tempPerson, wnPerson);
                        

                        if (wnPerson.ShowDialog() == true)
                        {
                            Role r = (Role)wnPerson.CbRole.SelectedValue;
                            personDPO.RoleName = r.NameRole;
                            personDPO.FirstName = tempPerson.FirstName;
                            personDPO.LastName = tempPerson.LastName;
                            personDPO.Birthday = PersonDPO.GetStringBirthday(tempPerson.Birthday);

                            FindPerson finder = new FindPerson(personDPO.Id);
                            Person p = ListPerson.FirstOrDefault(x => x.Id == personDPO.Id);
                            p = p.CopyFromPersonDpo(personDPO);
                            try
                            {
                                SaveChanges(ListPerson);
                            }
                            catch (Exception e)
                            {
                                Error = "Ошибка добавления данных в json файл\n" +
                                e.Message;
                            }

                        }
                    }, (obj) => SelectedPersonDPO != null && ListPersonDPO.Count > 0));
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
                        PersonDPO person = SelectedPersonDPO;
                        MessageBoxResult result = MessageBox.Show("Удалить данные по сотруднику: \n" + person.LastName + " " + person.FirstName,
                        "Предупреждение", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                        if (result == MessageBoxResult.OK)
                        {
                            {
                                try
                                {
                                    // удаление данных в списке отображения данных
                                    ListPersonDPO.Remove(person);
                                    // поиск удаляемого класса в коллекции ListRoles
                                    var per = ListPerson.FirstOrDefault(p => p.Id == person.Id);

                                    if (per != null)
                                    {
                                        ListPerson.Remove(per);
                                        // сохраненее данных в файле json
                                        SaveChanges(ListPerson);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Error = "Ошибка удаления данных\n" + e.Message;
                                }
                            }

                        }
                    }, (obj) => SelectedPersonDPO != null && ListPersonDPO.Count > 0));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged; [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
