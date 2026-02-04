using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("***********************");
            Console.WriteLine("         CLIENT       ");
            Console.WriteLine("***********************");
            Console.WriteLine("\n");
            Console.WriteLine("Izaberite protokol za komunikaciju:");
            Console.WriteLine("1. TCP");
            Console.WriteLine("2. UDP");


            //izbor TCP ili UDP komunikacije
            string izbor = Console.ReadLine();

            if (izbor == "1")
            {
                TCPClient();
            }
            else if (izbor == "2")
            {
                UDPClient();
            }
            else
            {
                Console.WriteLine("Nevažeći izbor!");
            }
        }

        //TCP komunikacija i izbor algoritma
        static void TCPClient()
        {
            try
            {
                Console.WriteLine("\nIzaberite algoritam za šifrovanje poruka:");
                Console.WriteLine("1. Homofonsko");
                Console.WriteLine("2. Plejfer");
                Console.WriteLine("3. Transpozicija matrica");

                string izbor = Console.ReadLine();
                string algoritam = "";
                string kljuc = "";

                switch (izbor)
                {
                    case "1":
                        algoritam = "Homofonsko";
                        HomofonoSifrovanje homo = new HomofonoSifrovanje("KRIPTOGRAFIJA");
                        kljuc = homo.Kljuc;
                        break;
                    case "2":
                        algoritam = "Plejfer";
                        PlejferovAlgoritam plejfer = new PlejferovAlgoritam("", "ABCDEFGHIKLMNOPQRSTUVWXYZ");
                        kljuc = plejfer.Kljuc;
                        break;
                    case "3":
                        algoritam = "Transpozicija";
                        TranspozicijaMatrica transp = new TranspozicijaMatrica("KOMUNIKACIJA", 3, 5);
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

                Console.WriteLine("\n[CLIENT] Povezujem se na server...");
                klijentSocket.Connect(endPoint);
                Console.WriteLine("[CLIENT] Klijent je povezan na server!");


                string informacije = $"{algoritam}|{kljuc}";
                byte[] bufferInfo = Encoding.UTF8.GetBytes(informacije);
                klijentSocket.Send(bufferInfo);
                Console.WriteLine($"[CLIENT] Poslate informacije o algoritmu: {algoritam}\n");


                while (true)
                {
                    Console.Write("[CLIENT] Unesite poruku: ");
                    string poruka = Console.ReadLine();

                    if (poruka.ToLower() == "izlaz")
                    {
                        break;
                    }


                    string sifrovanaPoruka = SifrujPoruku(poruka, algoritam, kljuc);
                    Console.WriteLine($"[CLIENT] Šifrovana poruka: {sifrovanaPoruka}");


                    byte[] bufferPoruka = Encoding.UTF8.GetBytes(sifrovanaPoruka);
                    klijentSocket.Send(bufferPoruka);


                    byte[] bufferOdgovor = new byte[2048];
                    int primljeno = klijentSocket.Receive(bufferOdgovor);
                    string sifrovanOdgovor = Encoding.UTF8.GetString(bufferOdgovor, 0, primljeno);
                    Console.WriteLine($"[CLIENT] Primljen šifrovani odgovor: {sifrovanOdgovor}");


                    string desifrovaniOdgovor = DesifrujPoruku(sifrovanOdgovor, algoritam, kljuc);
                    Console.WriteLine($"[CLIENT] Dešifrovani odgovor: {desifrovaniOdgovor}\n");
                }

                klijentSocket.Close();
                Console.WriteLine("[CLIENT] Veza sa serverom zatvorena.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CLIENT] Greška: {ex.Message}");
            }
        }


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



    }
}
}
