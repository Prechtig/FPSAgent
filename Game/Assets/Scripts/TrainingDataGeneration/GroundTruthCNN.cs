using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using Assets.Scripts.TrainingDataGeneration;
using System.Net;
using System.Net.Sockets;

public class GroundTruthCNN
{
	private static bool littleEndian;
	private static int port;
	private static IPEndPoint serverAddress;
	private static Socket clientSocket;

	static GroundTruthCNN() {
		littleEndian = DetermineByteOrder ();
		port = int.Parse (PropertiesReader.GetPropertyFile (PropertyFile.Project).GetProperty ("bridge.port"));
		serverAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
		clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		clientSocket.Connect(serverAddress);
	}

	private static bool DetermineByteOrder() {
		String byteOrder = PropertiesReader.GetPropertyFile (PropertyFile.Project).GetProperty ("bridge.byteorder");
		if("little_endian".Equals(byteOrder)) {
			return true;
		} else if("big_endian".Equals(byteOrder)) {
			return false;
		} else {
			throw new InvalidProgramException ("Please specify \"bridge.byteorder\" in the property file. Valid options are: \"little_endian\" and \"big_endian\"");
		}
	}

	void OnDestroy() {
		if (clientSocket != null && clientSocket.Connected) {
			clientSocket.Close();
		}
	}

	public static double[] CalculateFeaturesAngular(Camera playerCam) {
		Screenshot screenshot = ScreenSnapper.SnapScreenshot (playerCam);
		byte[] rgbArray = screenshot.GetRGB ();
		SendMessage(rgbArray);
        double[] features = ReceiveMessage();

        //Angular input fix
        if (features[3] < 0.5)
        {
            return new double[6];
        }
        else
        {
            double[] output = new double[6];
            double cnnHorizontal = features[0];
            if (cnnHorizontal > 0)
            {
                output[0] = Clamp01(Math.Abs(cnnHorizontal));
            }
            else
            {
                output[1] = Clamp01(Math.Abs(cnnHorizontal));
            }

            double cnnVertical = features[1];
            if (cnnVertical > 0)
            {
                output[2] = Clamp01(Math.Abs(cnnVertical));
            }
            else
            {
                output[3] = Clamp01(Math.Abs(cnnVertical));
            }

            if (features[2] <= 0)
            {
                output[4] = 0;
            }
            else
            {
                output[4] = features[2];
            }
            output[5] = 1;
            return output;
        }
	}

    private static double Clamp01(double d)
    {
        if (d > 1)
        {
            return 1d;
        }
        else if (d < 0)
        {
            return 0d;
        }
        return d;
    }

	private static void SendMessage(byte[] message) {
		int toSendLen = message.Length;

		clientSocket.Send (FromInt(toSendLen));
		clientSocket.Send (message);
	}

	private static double[] ReceiveMessage() {
		byte[] rcvLenBytes = new byte[4];
		clientSocket.Receive (rcvLenBytes);
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

	private static byte[] ensureByteOrder(byte[] arr) {
		if (BitConverter.IsLittleEndian && littleEndian) {
			return arr;
		}
		return ToBigEndian (arr);
	}
}
