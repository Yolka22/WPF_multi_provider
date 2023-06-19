using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_multi_provider
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DbConnection Connection = null;
        DbProviderFactory Factory = null;
        string ProviderName = "";

        public MainWindow()
        {
            InitializeComponent();
            Found_Providers();
        }

        static string MakeConnectionString(string transform)
        {
            string[] splited = transform.Split(';');

            transform = splited[0] + ";" + splited[1] + ";" + splited[2];

            return transform;
        }

        static string GetConnectionStringProvider(string provider)
        {
            string found = null;

            ConnectionStringSettingsCollection sett = ConfigurationManager.ConnectionStrings;

            if (sett != null)
            {
                foreach (ConnectionStringSettings item in sett)
                {
                    if (item.ProviderName != provider)//2 БД SQL поэтому тут не равно но так быть не должно
                    {
                        found = item.ConnectionString;
                        
                    }
                }
            }

            return found;
        }

        private void Found_Providers()
        {
            DataTable dataTable = DbProviderFactories.GetFactoryClasses();


            foreach (DataRow dataRow in dataTable.Rows)
            {
                MenuItem menuItem = new MenuItem();
                menuItem.Header = dataRow["InvariantName"];
                menuItem.Click += new RoutedEventHandler(Menu_Providers_Click);

                providers.Items.Add(menuItem);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Menu_Providers_Click(object sender, RoutedEventArgs e)
        {

            Factory = DbProviderFactories.GetFactory((sender as MenuItem).Header.ToString());
            Connection = Factory.CreateConnection();
            
            ProviderName = GetConnectionStringProvider((sender as MenuItem).Header.ToString());

            current_provider.Content = MakeConnectionString(ProviderName);

        }

        private void runQuery_Click(object sender, RoutedEventArgs e)
        {
            Connection.ConnectionString = MakeConnectionString(ProviderName);

            DbDataAdapter DA = Factory.CreateDataAdapter();
            DA.SelectCommand=Connection.CreateCommand();
            DA.SelectCommand.CommandText=Query_TextBox.Text;
            DataTable DT = new DataTable();
            DA.Fill(DT);

            main_grid.ItemsSource = DT.DefaultView;
        }
    }
}
