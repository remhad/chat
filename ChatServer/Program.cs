using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
	class Program
	{
		static List<TcpClient> Clients = new List<TcpClient>();
		static bool ParallelExecution = false;
		static Timer SendClientUpdateTimer;
		

		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;
			Console.WriteLine("Soll der Server parallel verarbeitet? (y - für yes / anykey - für no)");
			var input = Console.ReadKey();

			ParallelExecution = (input.Key == ConsoleKey.Y);
			if (input.Key == ConsoleKey.Y)
			{
				Console.WriteLine("Parallele verarbeitung aktiviert.");
				ParallelExecution = true;
			}
			else
			{
				Console.WriteLine("Parallele verarbeitung deaktiviert.");
			}

			var tcpListener = new TcpListener(IPAddress.Any, 3000);//initialisieren Server-höre auf Port 5000
			tcpListener.Start();

			SendClientUpdateTimer = new Timer(SendClientUpdate, null, 0, 1000);

			while (true) // Endlosschleife, damit Server sich nicht beendet nach 1.Verbindung
			{
				var client = tcpListener.AcceptTcpClient();// wird ausgeführt, wenn Client .connect() ausführt(Programm wartet an dieser Stelle
				Clients.Add(client); // wir speichern den Client in Liste

				var thread = new Thread(() => HandleClient(client));// Thread startet, um auf Nachrichten von Client zu hören
				thread.Start();

				Console.WriteLine("Verbunden: {0}", client.Client.RemoteEndPoint);
				Console.WriteLine("Es sind {0} Teilnehmer online.", Clients.Count);				
			}
		}

		private static void SendClientUpdate(object state)
		{
			string message = string.Format("onlinezaehler|{0}", Clients.Count);
			var byteMessage = Encoding.UTF8.GetBytes(message);
			Broadcast(byteMessage);
		}

		static void HandleClient(TcpClient client)
		{
			while (true) // Endlosschleife , um permanent eingehende Nachrichten zu überprüfen
				//Ohne Schleife: erse Nachricht abfragen dann Ende
				//Kein Thread: Nachrichtenabfrage blockirt Programm
			{
				//NetworkStream stream = client.GetStream(); -man kann es auch so schreiben-
				var stream = client.GetStream(); // Stream in Variable - Methode wird in einem Thread ausgeführt
				var buffer = new byte[1024]; //byte[] buffer = new byte[1024]; byte array - 
				//in den buffer wird die empfangene Nachricht zw. gespeichert "hat eine fixe Größe"
				var byteCount = stream.Read(buffer, 0, buffer.Length);

				if (byteCount > 0)// wenn etwas empfangen wurde/wenn Datenpaket (buffer) nicht leer.
				{
					Broadcast(buffer);

					string data = Encoding.UTF8.GetString(buffer, 0, byteCount); // Rückkonvertierung von ASCII in String um Daten ausgeben zu können
					Console.WriteLine(data);
				}
			}
		}

		private static void Broadcast(byte[] buffer)
		{
			if (ParallelExecution)
			{
				Parallel.ForEach(Clients, (otherClient) =>
				{
					var otherStream = otherClient.GetStream();
					otherStream.Write(buffer, 0, buffer.Length);
				});
			}
			else
			{
				// nicht Parallel
				foreach (var otherClient in Clients)
				{
					var otherStream = otherClient.GetStream();
					otherStream.Write(buffer, 0, buffer.Length);
				}
			}
		}
	}
}
