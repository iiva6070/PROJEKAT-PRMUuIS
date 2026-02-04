using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Contract
{
    public class NacinKomunikacije
    {
        public Socket UticnicaKlijenta { get; set; }
        public string Algoritam { get; set; }
        public string Kljuc { get; set; }
        public NacinKomunikacije()
        {
        }
        public NacinKomunikacije(Socket uticnica, string algoritam, string kljuc)
        {
            UticnicaKlijenta = uticnica;
            Algoritam = algoritam;
            Kljuc = kljuc;
        }
    }
}
