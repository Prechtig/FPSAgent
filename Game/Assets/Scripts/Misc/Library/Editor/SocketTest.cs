using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

class MainClass
{
	[Test]
	public static void Main ()
	{
		byte[] flattened = Enumerable.Repeat((byte) 0x55, (640*360*3)).ToArray();

//		IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse("192.168.0.6"), 4343);
		IPEndPoint serverAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4343);

		Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		clientSocket.Connect(serverAddress);

//		byte[] rcvLenBytes, rcvBytes;
//		int rcvLen;
//		String rcv;
//
//		// Sending
//		int toSendLen = flattened.Length;
//		byte[] toSendLenBytes = System.BitConverter.GetBytes(toSendLen);

		for (int i = 0; i < 10; i++) {
			double[] response = SendMessage (clientSocket, flattened);
			Console.WriteLine (string.Join(", ", response.Select(d => d.ToString()).ToArray()));
			Assert.AreEqual (20, response.Length);
//			clientSocket.Send (toSendLenBytes);
//			clientSocket.Send (flattened);
//
//			// Receiving
//			rcvLenBytes = new byte[4];
//			clientSocket.Receive (rcvLenBytes);
//			rcvLen = System.BitConverter.ToInt32 (rcvLenBytes, 0);
//			rcvBytes = new byte[rcvLen];
//			clientSocket.Receive (rcvBytes);
//			rcv = System.Text.Encoding.ASCII.GetString (rcvBytes);
//
//			Console.WriteLine ("Client received: " + rcv);
//			Assert.AreEqual ("OK", rcv);
		}

//		string closeMessage = "close";
//		toSendLen = System.Text.Encoding.ASCII.GetByteCount (closeMessage);
//		toSendLenBytes = System.BitConverter.GetBytes(toSendLen);
//		clientSocket.Send (toSendLenBytes);
//		clientSocket.Send (System.Text.Encoding.ASCII.GetBytes (closeMessage));
//
//		// Receiving
//		rcvLenBytes = new byte[4];
//		clientSocket.Receive (rcvLenBytes);
//		rcvLen = System.BitConverter.ToInt32 (rcvLenBytes, 0);
//		rcvBytes = new byte[rcvLen];
//		clientSocket.Receive (rcvBytes);
//		rcv = System.Text.Encoding.ASCII.GetString (rcvBytes);
//
//		Console.WriteLine ("Client received: " + rcv);
//		Assert.AreEqual ("OK", rcv);
//		SendMessage(clientSocket, "close");

		clientSocket.Close();
	}

	private static double[] SendMessage(Socket clientSocket, byte[] message) {
		int toSendLen = message.Length;

		clientSocket.Send (FromInt(toSendLen));
		clientSocket.Send (message);

		// Receiving
		byte[] rcvLenBytes = new byte[4];
		clientSocket.Receive (rcvLenBytes);
//		int rcvLen = System.BitConverter.ToInt32 (rcvLenBytes, 0);
		int rcvLen = ToInt(rcvLenBytes);
		byte[] rcvBytes = new byte[rcvLen];
		clientSocket.Receive (rcvBytes);

		return Enumerable
			.Range(0, rcvLen/8)
			.Select(i => ToDouble(rcvBytes, i))
			.ToArray();
	}

	private static byte[] FromInt(int i ) {
		return ToBigEndian (BitConverter.GetBytes (i));
	}

	private static int ToInt(byte[] arr) {
		return BitConverter.ToInt32 (ToBigEndian (arr), 0);
	}

	private static double ToDouble(byte[] arr) {
		return BitConverter.ToDouble (ToBigEndian (arr), 0);
	}

	private static double ToDouble(byte[] arr, int nth) {
		byte[] slice = new byte[8];
		Array.Copy (arr, nth*8, slice, 0, 8);
		return ToDouble (slice);
	}

	private static byte[] ToBigEndian(byte[] arr) {
		if (BitConverter.IsLittleEndian) {
			Array.Reverse (arr);
		}
		return arr;
	}
}
