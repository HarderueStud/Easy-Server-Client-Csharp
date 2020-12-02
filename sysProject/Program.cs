using System;
using System.Net.Sockets;

namespace sysProject
{
    class Program
    {
        static void Main(string[] args)
        {
            int nbAnnees;
            double capital;

            try
            {
                Console.WriteLine("Saisir le nombre d'années :");
                nbAnnees = int.Parse(Console.ReadLine());
                Console.WriteLine("Saisir le capital initial :");
                capital = double.Parse(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);   // Affichage msg erreur
                return;                         // On quitte le programme
            }

            try
            {
                Socket S = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                S.Connect("localhost", 1212);

                byte[] b_an = BitConverter.GetBytes(nbAnnees);
                byte[] b_cap = BitConverter.GetBytes(capital);

                S.Send(b_an);
                S.Send(b_cap);

                byte[] b_capFinal = new byte[8];
                S.Receive(b_capFinal);

                S.Shutdown(SocketShutdown.Both);
                S.Close();

                double capitalFinal = BitConverter.ToDouble(b_capFinal, 0);
                Console.WriteLine($"Capital final : {capitalFinal:F} euros.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);   // Affichage msg erreur
                return;                         // On quitte le programme
            }
        }
    }
}
