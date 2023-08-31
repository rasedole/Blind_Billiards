using System.Net.Sockets;
using System.IO;
using System;
using UnityEngine;

public class TCP_Client
{
	string clientName;

	bool socketReady;
	TcpClient socket;
	NetworkStream stream;
	StreamWriter writer;
	StreamReader reader;

	// Connect or disconnect the client.
	// =====================================================================================
	public void ConnectToServer()
	{
		if (socketReady)
		{
			TCP_Core.Message("Client already running!");
			return;
		}

		// make socket
		try
		{
			TCP_Core.SetIpAndPort();
			socket = new TcpClient(TCP_Core.ipString, TCP_Core.port);
			stream = socket.GetStream();
			writer = new StreamWriter(stream);
			reader = new StreamReader(stream);
			socketReady = true;
		}
		catch (ArgumentNullException e)
		{
			socketReady = false;
			TCP_Core.Message(e + "");
		}
		catch (SocketException e)
		{
			socketReady = false;
			TCP_Core.Message(e + "");
		}
		catch (Exception e)
		{
			socketReady = false;
			TCP_Core.Message($"소켓에러 : {e.Message}");
		}
	}

	public void CloseSocket()
	{
		if (!socketReady)
		{
			TCP_Core.Message("Client already stopped!");
			return;
		}

		writer.Close();
		reader.Close();
		socket.Close();
		socketReady = false;
	}

	// Must be update everytime.
	// =====================================================================================
	public void Listen()
	{
		if (socketReady && stream.DataAvailable)
		{
			string data = reader.ReadLine();
			if (data != null)
				OnIncomingData(data);
		}
	}

	// Receve or send data.
	// =====================================================================================

	// When data receved from server.
	void OnIncomingData(string data)
	{
		if (data == TCP_Core.command.GetCommand(-1))
		{
			clientName = TCP_Core.id == "" ? "Guest" + UnityEngine.Random.Range(1000, 10000) : TCP_Core.id;
			Send(-1, clientName);
			return;
		}

		TCP_Core.Message(data);
	}

	// Send message to server
	public void Send(int index, string data)
	{
		try
		{
			if (!socketReady)
			{
				TCP_Core.Message("Client not running!");
				return;
			}

			writer.WriteLine(TCP_Core.command.GetCommand(index) + data);
			writer.Flush();
		}
		catch (Exception e)
		{
			TCP_Core.Message(e + "");
		}
	}
}
