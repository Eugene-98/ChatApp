using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;

namespace ChatClient.MVVM.ViewModel
{
	public class MainViewModel
	{
		public ObservableCollection<UserModel> Users { get; set; }
		public ObservableCollection<string> Messages { get; set; }

		public RelayCommand ConnectToServerCommand { get; set; }
		public RelayCommand SendMessageCommand { get; set; }

		public string Username { get; set; }
		public string Message { get; set; }

		private Server _server;

		public MainViewModel()
		{
			Users = new ObservableCollection<UserModel>();
			Messages = new ObservableCollection<string>();

			_server = new Server();
			_server.connectedEvent += UserConnected;
			_server.messageEvent += MessageReceived;
			_server.disconnectedEvent += RemovesUser;
			ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));

			SendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(Message), o => !string.IsNullOrEmpty(Message));
		}

		public void RemovesUser()
		{
			var uid = _server.PacketReader.ReadMessage();
			var user = Users.Where(x => x.UserId == uid).FirstOrDefault();
			Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
		}

		public void MessageReceived()
		{
			var msg = _server.PacketReader.ReadMessage();
			Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
		}

		public void UserConnected()
		{
			var user = new UserModel
			{
				Username = _server.PacketReader.ReadMessage(),
				UserId = _server.PacketReader.ReadMessage(),
			};

			if (!Users.Any(x => x.UserId == user.UserId))
			{
				Application.Current.Dispatcher.Invoke(() => Users.Add(user));
			}
		}
	}
}
