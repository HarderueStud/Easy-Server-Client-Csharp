using System;
using System.Net;
using System.Net.Sockets;

namespace Client_NTP
{
    class Program
    {
        static void Main(string[] args)
        {
            UdpClient client;
            byte[] trame = new byte[48];
            byte[] recu;
            IPEndPoint iep = new IPEndPoint(0, 0);
            // Construction de la trame à envoyer au serveur NTP
            // Champ VN à 4, champ Mode à 3, donc le premier octet de cette trame vaut :
            // 0 1 2 3 4 5 6 7
            // +-+-+-+-+-+-+-+-+
            // |LI | VN |Mode |
            // +-+-+-+-+-+-+-+-+
            // +0 0|1 0 0|0 1 1+
            // soit 2 | 3 en hexadécimal
            trame[0] = 0x23;
            try
            {
                client = new UdpClient("ntp.u-picardie.fr", 123);
                // Envoi de la trame au serveur NTP
                client.Send(trame, 48);
                // Réception de la trame de réponse – iep contient l’identité de l’émetteur du message
                recu = client.Receive(ref iep);
                Console.WriteLine("Réponse reçue du serveur {0}", iep.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            // Destruction de la socket;
            client.Close();
            // Passage de big endian vers litte endian du champ TT de la trame reçue
            Array.Reverse(recu, 40, 4);
            // Extraction de ce champ TT dans un entier non signé
            uint TT = BitConverter.ToUInt32(recu, 40);
            // Ajout du nb de seconde contenu dans TT à 01/01/1900
            DateTime heure = new DateTime(1900, 1, 1);
            heure = heure.AddSeconds(TT).ToLocalTime();
            // Affichage du résultat obtenu
            Console.WriteLine("Heure exacte : {0}", heure.ToString());
            Console.ReadKey();
        }
    }
}