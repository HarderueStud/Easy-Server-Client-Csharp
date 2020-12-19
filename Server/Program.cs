using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;


namespace Server
{
    class Program
    {
        // Programme principal
        static void Main(string[] args)
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 1212);
            try
            {
                // Etape 1
                Socket sock_ecoute = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // Etape 2
                sock_ecoute.Bind(iep);
                //Etape 3
                sock_ecoute.Listen(5);
                while (true)
                {
                    // Etape 4.1
                    Socket sock_serv = sock_ecoute.Accept();
                    // Etape 4.2
                    ThreadPool.QueueUserWorkItem(Communique, sock_serv);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }

        // Sous-programme de communication avec le client
        // Paramètre d'entrée. Arg : socket utilisée pour la communication
        // Valeur de retour. Aucune
        static void Communique(object arg)
        {
            Socket sock = (Socket)arg;
            byte[] b_an = new byte[4];
            byte[] b_si = new byte[8];

            Console.WriteLine("{0} - Client : {1}", DateTime.Now, sock.RemoteEndPoint.ToString());

            // Récupération du nb d'année et du capital initial envoyés par le client
            sock.Receive(b_an);
            sock.Receive(b_si);

            // Conversion tableau d'octets -> nombres
            int nbAnnees = BitConverter.ToInt32(b_an, 0);
            double si = BitConverter.ToDouble(b_si, 0);

            // Calcul du capital final
            for (int i = 0; i < nbAnnees; i++)
                si = si + (si * 0.75 / 100);

            // Conversion nombre -> tableau d'octets
            byte[] b_sf = BitConverter.GetBytes(si);

            // Envoi au client
            sock.Send(b_sf);

            // Désactivation et destruction de la socket
            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
        }
    }
}
