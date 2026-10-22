using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ShaidullinVPlab.Helper;
using ShaidullinVPlab.View;

namespace ShaidullinVPlab.ViewModel
{
    public class MainWindowViewModel
    {
        /// <summary>
        /// команда открытия окна "Сотрудники"
        /// </summary>
        private RelayCommand openEmployee;
        public RelayCommand OpenEmployee
        {
            get
            {
                return openEmployee ??
                (openEmployee = new RelayCommand(obj =>
                {
                    WindowEmployee wnEmployee = new WindowEmployee
                    {
                        Title = "Сотрудники"
                    };
                    wnEmployee.ShowDialog();
                }));
            }
        }

        /// <summary>
        /// команда открытия окна "Должности"
        /// </summary>
        private RelayCommand openRole;
        public RelayCommand OpenRole
        {
            get
            {
                return openRole ??
                (openRole = new RelayCommand(obj =>
                {
                    WindowRole wnRole = new WindowRole
                    {
                        Title = "Должности"
                    };
                    wnRole.ShowDialog();
                }));
            }
        }

        /// <summary>
        /// команда выхода из приложения
        /// </summary>
        private RelayCommand exitApp;
        public RelayCommand ExitApp
        {
            get
            {
                return exitApp ??
                (exitApp = new RelayCommand(obj =>
                {
                    Application.Current.Shutdown();
                }));
            }
        }
    }

}
