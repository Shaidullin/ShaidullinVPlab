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

namespace ShaidullinVPlab.ViewModel
{
    public class PersonViewModel : INotifyPropertyChanged
    {
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
            this.ListPerson.Add(new Person
            {
                Id = 1,
                RoleId = 1,
                FirstName = "Иван",
                LastName = "Иванов",
                Birthday = new DateTime(1980, 02, 28)
            });
            this.ListPerson.Add(new Person
            {
                Id = 2,
                RoleId = 2,
                FirstName = "Петр",
                LastName = "Петров",
                Birthday = new DateTime(1981, 03, 20)
            });
            this.ListPerson.Add(new Person
            {
                Id = 3,
                RoleId = 3,
                FirstName = "Виктор",
                LastName = "Викторов",
                Birthday = new DateTime(1982, 04, 15)
            });
            this.ListPerson.Add(new Person
            {
                Id = 4,
                RoleId = 3,
                FirstName = "Сидор",
                LastName = "Сидоров",
                Birthday = new DateTime(1983, 05, 10)
            });
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
                        Birthday = DateTime.Now
                    };
                    RoleViewModel vmRole = new RoleViewModel();
                    wnPerson.CbRole.ItemsSource = vmRole.ListRole;
                    wnPerson.CbRole.DisplayMemberPath = "NameRole";
                    wnPerson.DataContext = new NewPesronViewModel(per, wnPerson);
                    if (wnPerson.ShowDialog() == true)
                    {
                        Role r = (Role)wnPerson.CbRole.SelectedValue; per.RoleName = r.NameRole; ListPersonDPO.Add(per);
                        // добавление нового сотрудника в коллекцию ListPerson<Person>
                        Person p = new Person();
                        p = p.CopyFromPersonDpo(per); ListPerson.Add(p);
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
                            personDPO.Birthday = tempPerson.Birthday;
                            FindPerson finder = new FindPerson(personDPO.Id);
                            List<Person> listPerson = ListPerson.ToList(); Person p = listPerson.Find(new Predicate<Person>(finder.PersonPredicate));
                            p = p.CopyFromPersonDpo(personDPO);
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
                            // удаление данных в списке отображения данных
                            ListPersonDPO.Remove(person);
                            // удаление данных в списке классов ListPerson<Person>
                            Person per = new Person();
                            per = per.CopyFromPersonDpo(person); ListPerson.Remove(per);
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
