using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geo
{
    class GGA
    {
        private DateTime _date;
        private double _north;
        private double _west;
        private int _typeLocation;
        private int _nbSat;
        private double _altitude;
        private string _nextData;//on l'initialise pas, la flemme

        public GGA(string trameGGA)
        {
            //va découper une trame au format
            //$GPGGA,153709.00,4326.7373771,N,00133.0838793,W,1,21,1.3,43.2222,M,50

            string[] infoGGA = trameGGA.Split(',');
            setDateFromGGA(infoGGA[1]);

            try
            {
                this._north = Double.Parse(infoGGA[2].Replace('.',','))/100;
                this._west = Double.Parse(infoGGA[4].Replace('.', ','))/100;
                this._typeLocation = Int32.Parse(infoGGA[6]);
                this._nbSat = Int32.Parse(infoGGA[7]);
                this._altitude = Double.Parse(infoGGA[9].Replace('.', ','))/100;
            } catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public double North { get => _north; set => _north = value; }
        public double West { get => _west; set => _west = value; }
        public double Altitude { get => _altitude; set => _altitude = value; }

        public void setDateFromGGA(string laDate)
        {

            // Extraire l'heure, les minutes et les secondes de la chaîne
            int heure = int.Parse(laDate.Substring(0, 2));
            int minutes = int.Parse(laDate.Substring(2, 2));
            int secondes = int.Parse(laDate.Substring(4, 2));

            // Initialiser un objet DateTime avec la date actuelle et l'heure spécifiée
            this._date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, heure, minutes, secondes);
        }
    }
}
