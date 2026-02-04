using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Contract;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
                Console.WriteLine("************************");
                Console.WriteLine("         SERVER       ");
                Console.WriteLine("************************");
                Console.WriteLine("\n");
                Console.WriteLine("Izaberite protokol za komunikaciju:");
                Console.WriteLine("1. TCP");
                Console.WriteLine("2. UDP");


                //izbor TCP ili UDP komunikacije
                string izbor = Console.ReadLine();

                if (izbor == "1")
                {
                    TCPServer();
                }
                else if (izbor == "2")
                {
                    UDPServer();
                }
                else
                {
                    Console.WriteLine("Nevažeći izbor!");
                }
        }
          
            static void TCPServer()
            {
                try
                {
                    string ipAdresa = "127.0.0.1";
                    int port = 5000;
                    IPAddress ip = IPAddress.Parse(ipAdresa);
                    IPEndPoint endPoint = new IPEndPoint(ip, port);

                    Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    serverSocket.Bind(endPoint);
                    serverSocket.Listen(5);

                   //ispis IP adrese i broj porta na kojem očekujemo poruke/zahteve za uspostavu veze.
                    Console.WriteLine($"\n[SERVER] TCP server pokrenut na: {ipAdresa}:{port}!");
                    Console.WriteLine("[SERVER] Čekam na klijente...\n");

                    List<NacinKomunikacije> klijenti = new List<NacinKomunikacije>();


                   //rad sa klijentima

                    while (true)
                    {
                        try
                        {
                            
                            if (serverSocket.Poll(100000, SelectMode.SelectRead))
                            {
                                Socket klijentSocket = serverSocket.Accept();
                                Console.WriteLine($"[SERVER] Klijent je povezan: {klijentSocket.RemoteEndPoint}");

                                
                                byte[] buffer = new byte[1024];
                                int primljeno = klijentSocket.Receive(buffer);
                                string informacije = Encoding.UTF8.GetString(buffer, 0, primljeno);

                                Console.WriteLine($"[SERVER] Primljene informacije: {informacije}");

                               
                                string[] delovi = informacije.Split('|');
                                string algoritam = delovi[0];
                                string kljuc = delovi.Length > 1 ? delovi[1] : "";

                                
                                NacinKomunikacije nacinKom = new NacinKomunikacije(klijentSocket, algoritam, kljuc);
                                klijenti.Add(nacinKom);

                                Console.WriteLine($"[SERVER] Klijent koristi algoritam: {algoritam}");
                            }

                            
                            for (int i = klijenti.Count - 1; i >= 0; i--)
                            {
                                Socket klijentSocket = klijenti[i].UticnicaKlijenta;

                                if (klijentSocket.Poll(100000, SelectMode.SelectRead))
                                {
                                    try
                                    {
                                        byte[] buffer = new byte[2048];
                                        int primljeno = klijentSocket.Receive(buffer);

                                        if (primljeno > 0)
                                        {
                                            string sifrovanaPorukaUlaz = Encoding.UTF8.GetString(buffer, 0, primljeno);
                                            Console.WriteLine($"[SERVER] Primljena šifrovana poruka od  klijenta: {klijentSocket.RemoteEndPoint}: {sifrovanaPorukaUlaz}");

                                            
                                            string desifrovana = DesifrujPoruku(sifrovanaPorukaUlaz, klijenti[i]);
                                            Console.WriteLine($"[SERVER] Dešifrovana poruka: {desifrovana}");

                                            
                                            string odgovor = "Poruka primljena: " + desifrovana;

                                            
                                            string sifrovanOdgovor = SifrujPoruku(odgovor, klijenti[i]);

                                            
                                            byte[] bufferOdgovor = Encoding.UTF8.GetBytes(sifrovanOdgovor);
                                            klijentSocket.Send(bufferOdgovor);
                                            Console.WriteLine($"[SERVER] Šifrovani odgovor poslаt klijentu: {sifrovanOdgovor}\n");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"[SERVER] Klijent {klijentSocket.RemoteEndPoint} je prekinuo vezu");
                                            klijentSocket.Close();
                                            klijenti.RemoveAt(i);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"[SERVER] Greška pri komunikaciji {ex.Message}");
                                        klijentSocket.Close();
                                        klijenti.RemoveAt(i);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[SERVER] Greška: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SERVER] Greška pri pokretanju servera: {ex.Message}");
                }
            }

            static void UDPServer()
            {
                try
                {
                    string ipAdresa = "127.0.0.1";
                    int port = 5001;
                    IPAddress ip = IPAddress.Parse(ipAdresa);
                    IPEndPoint endPoint = new IPEndPoint(ip, port);

                    Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    udpSocket.Bind(endPoint);

                   Console.WriteLine($"\n[SERVER] UDP server pokrenut na: {ipAdresa}:{port}!");

                    byte[] buffer = new byte[2048];

                    while (true)
                    {
                        try
                        {
                            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                            int primljeno = udpSocket.ReceiveFrom(buffer, ref remoteEndPoint);

                            string poruka = Encoding.UTF8.GetString(buffer, 0, primljeno);
                            Console.WriteLine($"[SERVER] Datagram primljen od {remoteEndPoint}: {poruka}");

                           
                            string odgovor = "Server primio: " + poruka;
                            byte[] odgovorBuffer = Encoding.UTF8.GetBytes(odgovor);
                            udpSocket.SendTo(odgovorBuffer, remoteEndPoint);
                            Console.WriteLine($"[SERVER] Odgovor poslаt klijentu\n");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[SERVER] Greška: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SERVER] Greška pri pokretanju UDP servera: {ex.Message}");
                }
            }


        }
    }
