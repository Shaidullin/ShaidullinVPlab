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
    public class Person
    {
        public int Id { get; set; }      // код
        public int RoleId { get; set; }    // код должности
        public string FirstName { get; set; } // имя
        public string LastName { get; set; } // фамилия
        public DateTime Birthday { get; set; } // дата рождения
        public virtual Role Role { get; set; }
    }
}
