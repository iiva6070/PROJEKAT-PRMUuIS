using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Contract;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*********");
            Console.WriteLine(" CLIENT");
            Console.WriteLine("*********");
            Console.WriteLine("\n");
            Console.WriteLine("Izaberite protokol za komunikaciju:");
            Console.WriteLine("1.TCP protokol");
            Console.WriteLine("2.UDP protokol");


            //izbor TCP ili UDP komunikacije
            string komunikacija = Console.ReadLine();

            if (komunikacija == "1")
            {
                TCPClient();
            }
            else if (komunikacija == "2")
            {
                UDPClient();
            }
            else
            {
                Console.WriteLine("Nevalidna opcija!");
            }
        }

        //TCP komunikacija i izbor algoritma
        static void TCPClient()
        {
            try
            {
                Console.WriteLine("\nIzaberite algoritam za šifrovanje poruka:");
                Console.WriteLine("1.Homofonski algoritam");
                Console.WriteLine("2.Plejferov algoritam");
                Console.WriteLine("3.Transpozicija matrica");

                string opcija = Console.ReadLine();
                string algoritam = "";
                string kljuc = "";

                switch (opcija)
                {
                    case "1":
                        algoritam = "Homofonsko";
                        HomofoniAlgoritam homo = new HomofoniAlgoritam("TEST");
                        kljuc = homo.Kljuc;
                        break;
                    case "2":
                        algoritam = "Plejfer";
                        PlejferovAlgoritam plejfer = new PlejferovAlgoritam("", "ABCDEFGHIKLMNOPQRSTUVWXYZ");
                        kljuc = plejfer.Kljuc;
                        break;
                    case "3":
                        algoritam = "Transpozicija";
                        TranspozicijaMatrica transp = new TranspozicijaMatrica("KOMUNIKACIJA", 3, 7);
                        kljuc = transp.Kljuc;
                        break;
                    default:
                        Console.WriteLine("Nevažeći izbor!");
                        return;
                }


                //pravimo socket za klijenta i povezujemo se na server
                Socket klijentSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                IPEndPoint endPoint = new IPEndPoint(ip, 5000);

                Console.WriteLine("\n[CLIENT] Klijent se povezuje na server...");
                klijentSocket.Connect(endPoint);
                Console.WriteLine("[CLIENT] Klijent je povezan na server!");


                string informacije = $"{algoritam}|{kljuc}";
                byte[] bufferInfo = Encoding.UTF8.GetBytes(informacije);
                klijentSocket.Send(bufferInfo);
                Console.WriteLine($"[CLIENT] Poslate informacije o algoritmu: {algoritam}\n");


                while (true)
                {
                    Console.Write("[CLIENT] Unesite poruku koju cete poslati serveru: ");
                    string poruka = Console.ReadLine();

                    if (poruka.ToLower() == "izlaz")
                    {
                        break;
                    }


                    string sifrovanaPoruka = SifrovanjePoruke(poruka, algoritam, kljuc);
                    Console.WriteLine($"[CLIENT] Šifrovana poruka: {sifrovanaPoruka}");


                    byte[] bufferPoruka = Encoding.UTF8.GetBytes(sifrovanaPoruka);
                    klijentSocket.Send(bufferPoruka);


                    byte[] bufferOdgovor = new byte[2048];
                    int primljeno = klijentSocket.Receive(bufferOdgovor);
                    string sifrovanOdgovor = Encoding.UTF8.GetString(bufferOdgovor, 0, primljeno);
                    Console.WriteLine($"[CLIENT] Primljen šifrovani odgovor: {sifrovanOdgovor}");


                    string desifrovaniOdgovor = DesifrovanjePoruke(sifrovanOdgovor, algoritam, kljuc);
                    //Console.WriteLine($"[CLIENT] Dešifrovani odgovor: {desifrovaniOdgovor}\n");
                    Console.WriteLine("[CLIENT] Dešifrovani odgovor: PORUKA PRIMLJENA: " + desifrovaniOdgovor);

                }

                klijentSocket.Close();
                Console.WriteLine("[CLIENT] Veza sa serverom zatvorena.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CLIENT] Greška: {ex.Message}");
            }
        }


        //UDP komunikacija
        static void UDPClient()
        {
            try
            {
                Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                IPEndPoint serverEndPoint = new IPEndPoint(ip, 5001);

                Console.WriteLine("[CLIENT] Klijent je povezan!");

                while (true)
                {
                    Console.Write("[CLIENT] Unesite poruku: ");
                    string poruka = Console.ReadLine();

                    if (poruka.ToLower() == "izlaz")
                    {
                        break;
                    }

                    byte[] buffer = Encoding.UTF8.GetBytes(poruka);
                    udpSocket.SendTo(buffer, serverEndPoint);
                    Console.WriteLine($"[CLIENT] Datagram poslаt: {poruka}");


                    byte[] bufferOdgovor = new byte[2048];
                    EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    int primljeno = udpSocket.ReceiveFrom(bufferOdgovor, ref remoteEndPoint);
                    string odgovor = Encoding.UTF8.GetString(bufferOdgovor, 0, primljeno);
                    Console.WriteLine($"[CLIENT] Odgovor servera: {odgovor}\n");
                }

                udpSocket.Close();
                Console.WriteLine("[CLIENT] UDP klijent zatvoren.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CLIENT] Greška: {ex.Message}");
            }
        }

        static string SifrovanjePoruke(string tekst, string algoritam, string kljuc)
        {
            switch (algoritam.ToLower())
            {
                case "homofonsko":
                    {
                        HomofoniAlgoritam homo = new HomofoniAlgoritam();
                        homo.Kljuc = kljuc;
                        homo.Tekst = tekst;
                        return homo.Enkripcija();
                    }
                case "plejfer":
                    {
                        PlejferovAlgoritam plejfer = new PlejferovAlgoritam();
                        plejfer.Kljuc = kljuc;
                        plejfer.Tekst = tekst;
                        return plejfer.Enkripcija();
                    }
                case "transpozicija":
                    {
                        TranspozicijaMatrica transp = new TranspozicijaMatrica();
                        transp.Kljuc = kljuc;
                        transp.Tekst = tekst;
                        return transp.Enkripcija();
                    }
                default:
                    return tekst;
            }
        }

        static string DesifrovanjePoruke(string sifrovana, string algoritam, string kljuc)
        {
            switch (algoritam.ToLower())
            {
                case "homofonsko":
                    {
                        HomofoniAlgoritam homo = new HomofoniAlgoritam();
                        homo.Kljuc = kljuc;
                        return homo.Dekripcija(sifrovana);
                    }
                case "plejfer":
                    {
                        PlejferovAlgoritam plejfer = new PlejferovAlgoritam();
                        plejfer.Kljuc = kljuc;
                        return plejfer.Dekripcija(sifrovana);
                    }
                case "transpozicija":
                    {
                        TranspozicijaMatrica transp = new TranspozicijaMatrica();
                        transp.Kljuc = kljuc;
                        return transp.Dekripcija(sifrovana);
                    }
                default:
                    return sifrovana;
            }
        }

    }
}