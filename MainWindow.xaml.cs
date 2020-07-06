using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DatenbankDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Artist> artistList;

        public MainWindow()
        {
            InitializeComponent();
            artistList = new ObservableCollection<Artist>();
        }

        private void btnLoadData_Click(object sender, RoutedEventArgs e)
        {
            /*
             *+1. dll laden für die entsprechende Datenbank (falls es nicht Microsoft ist)
             *+2. Connectionstring aufbauen
             *+3. Verbindung zum Datenbankserver anhand vom connectionstring herstellen
             *+4. SQL-Kommando vorbereiten
             * 5. SQL-Kommando durch die connection senden
             * 6. Schleife Zeilenweise die Daten entgegenimmt und in einer Vorbereiteten struktur speichert
             * 7. Alles dicht machen
             */

            // https://www.connectionstrings.com/mysql/
            MySqlConnectionStringBuilder connectionStringBuilder = new MySqlConnectionStringBuilder();
            connectionStringBuilder.Server = "192.168.2.2"; // IP Adresse des servers, DNS name, Localhost und . funktioniert auch
            connectionStringBuilder.UserID = "MusicDBUser"; // Benutzername innerhalb des DBMS, dieser nutzer sollte so wenig rechte wie möglich bekommen
            connectionStringBuilder.Password = "MusicDBPass"; // Passwort zu dem Benutzernamen
            connectionStringBuilder.Database = "musicdb"; // Datenbankname mit der sich verbunden werden soll, alle SQL statements sind dann relativ zu dieser Datenbank (siehe USE )
            connectionStringBuilder.SslMode = MySqlSslMode.None; // None ist ok für testumgebungen, im Internet immer verschlüsseln. Benötigt extra CPU-Leistung

            string connectionstring = connectionStringBuilder.ToString();

            using (MySqlConnection sqlConnection = new MySqlConnection(connectionstring)) // Verbindungsobjekt erstellen und anhand des connectionstring konfigurieren
            {
                sqlConnection.Open(); // verbindung zur datenbank herstellen
                MySqlCommand sqlCommand = sqlConnection.CreateCommand(); // Commandoobjekt passend zu dieser Verbindung erstellen
                sqlCommand.CommandType = System.Data.CommandType.Text; // Commandoobjekt auf ein Text-SQL kommando vorbereiten, normalerweise stored procedures aus sicherheitsgründen
                sqlCommand.CommandText = "select * from Artists;"; // SQL-Befehle dem Commandoobjekt hinzufügen. Standardmässig nur ein Befehl (Servereinstellung)

                using (MySqlDataReader sqlResponse = sqlCommand.ExecuteReader())
                {
                    while (sqlResponse.Read())
                    {
                        Artist aktuellerDatensatz = new Artist();
                        aktuellerDatensatz.ID = sqlResponse.GetInt32(0); // liesst spalte 0 als integer aus der aktuellen Zeile
                        aktuellerDatensatz.Name = sqlResponse.GetString(1); // liesst spalte 1 als string ...
                        aktuellerDatensatz.OriginID = sqlResponse.GetUInt16(2); // liesst spalte 2

                        artistList.Add(aktuellerDatensatz);
                    }
                } 

            }

        }
    }
}
