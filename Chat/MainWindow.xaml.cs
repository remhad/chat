using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace Chat
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{


		public MainWindow()
		{
			InitializeComponent();
		}

		TcpClient client;
		string Username = string.Empty;

		void ReceiveData(TcpClient client)


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

					string data = Encoding.UTF8.GetString(buffer, 0, byteCount); // Rückkonvertierung von ASCII in String um Daten ausgeben zu können
					data = data.Replace("\0", string.Empty);

					var messageParts = data.Split('|');
					switch (messageParts[0])
					{
						case "message":
							Dispatcher.Invoke(() => // braucht man invoke dass man auf Oberfläche zugreifen kann
							{
								ChatText.Text = messageParts[1] + ": " + messageParts[2] + Environment.NewLine + ChatText.Text; //oder// ChatText.Text += Environment.NewLine + data;
							});
							break;

						case "onlinezaehler":
							Dispatcher.Invoke(() => // braucht man invoke dass man auf Oberfläche zugreifen kann
							{
								OnlineOutput.Content = messageParts[1];
							});
							break;
						case "user_list":
							var userList = messageParts[1].Split(',');
							string userListText = string.Empty;
							foreach (var userName in userList)
							{
								userListText += userName + Environment.NewLine;
							}
							Dispatcher.Invoke(() => 
							{
								userList.Text = userListText;
							});
							break;
						default:
							break;
					}
				}
			}
		}
		private void ConnectButton_Click(object sender, RoutedEventArgs e)
		{

			try
			{
				Username = NameInput.Text;

				var ipAddress = IPAddress.Parse(IpAdressInput.Text);
				client = new TcpClient();
				client.Connect(ipAddress, 5000);
				SendButton.IsEnabled = true;

				SendData(string.Format("connest|{0}", Username));

				ChatText.Text = string.Empty;

				var thread = new Thread(() => ReceiveData(client));
				thread.Start();
			}
			catch (FormatException)
			{
				MessageBox.Show("Ungültige IP-Adresse");
			}
			catch (SocketException)
			{
				MessageBox.Show("Server nicht erreichbar");
			}
		}

		private void SendData(string messageText)
		{
			var stream = client.GetStream();
			var messageTextBytes = Encoding.UTF8.GetBytes(messageText);
			stream.Write(messageTextBytes, 0, messageTextBytes.Length);
		}

		private void SendButton_Click(object sender, RoutedEventArgs e)
		{
			var messageText = string.Format("message|{0}|{1}", Username, MessageInput.Text);
			MessageInput.Text = string.Empty; // wird gelöscht oder geleert

			var stream = client.GetStream();

			var messageTextBytes = Encoding.UTF8.GetBytes(messageText);
			stream.Write(messageTextBytes, 0, messageTextBytes.Length);
		}

		private void OnlineOutput_Click(object sender, System.Windows.Controls.ContextMenuEventArgs e)
		{
			
		}
	}
}
