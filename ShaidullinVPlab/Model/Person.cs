using JetBrains.Annotations;
using ShaidullinVPlab.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShaidullinVPlab.Model
{
    public class Person : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Birthday { get; set; }
        public Person() { }
        public Person(int id, int roleId, string firstName, string lastName, string birthday)
        {
            this.Id = id; this.RoleId = roleId;
            this.FirstName = firstName; this.LastName = lastName; this.Birthday = birthday;
        }

        public Person CopyFromPersonDpo(PersonDPO personDPO)
        {
            RoleViewModel vmRole = new RoleViewModel();
            int role = 0;
            foreach (var r in vmRole.ListRole)
            {
                if (r.NameRole == personDPO.RoleName)
                {
                    role = r.Id; break;
                }
            }
            if (role != 0)
            {
                this.Id = personDPO.Id;
                this.RoleId = role;
                this.FirstName = personDPO.FirstName;
                this.LastName = personDPO.LastName;
                this.Birthday = personDPO.Birthday;
            }
            return this;
        }
        public event PropertyChangedEventHandler PropertyChanged; [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
