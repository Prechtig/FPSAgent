package org.mma.imagerecognition.bridge;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.Properties;
import java.util.stream.DoubleStream;
import java.util.stream.IntStream;

import org.apache.commons.io.IOUtils;
import org.apache.commons.lang3.ArrayUtils;
import org.mma.imagerecognition.tools.PropertiesReader;

public class SingleThreadedBridge {
	
	private int port;
	private ServerSocket serverSocket;
	private Socket clientSocket;
	private ByteOrder bridgeByteOrder;
	private InputStream is;
	private OutputStream os;
	
	public SingleThreadedBridge() {
		readProperties();
		openServerSocket();
	}
	
	public void listen() {
		try {
			clientSocket = serverSocket.accept();
			is = clientSocket.getInputStream();
			os = clientSocket.getOutputStream();
		} catch (IOException e) {
			throw new RuntimeException("Error accepting client connection", e);
		}
	}
	
	public byte[] readData() {
		try {
			int length = readLength();
			return readData(length);
		} catch (IOException e) {
			throw new RuntimeException("Could not read data from client", e);
		}
	}
	
	public void sendData(double[] ds) {
		try {
			byte[] data = DoubleStream.of(ds)
							.boxed()
							.map(d -> fromDouble(d))
							.reduce(new byte[0], (cumArr, curArr) -> ArrayUtils.addAll(cumArr, curArr));
			sendData(data);
		} catch(IOException e) {
			throw new RuntimeException("Could not write data to client", e);
		}
	}
	
	private void sendData(byte[] data) throws IOException {
		byte[] toSendLength = fromInt(data.length);
		IOUtils.write(toSendLength, os);
		IOUtils.write(data, os);
	}
	
	private byte[] readData(int length) throws IOException {
		byte[] receivedBytes = new byte[length];
		IOUtils.read(is, receivedBytes);
		return receivedBytes;
	}
	
	private int readLength() throws IOException {
		byte[] lenBytes = new byte[4];
		IOUtils.read(is, lenBytes);
		return toInt(lenBytes);
	}
	
	private int toInt(byte[] arr) {
		if(arr.length != 4) {
			throw new IllegalStateException("Trying to convert a byte[] to int, but the length of the byte[] is not 4.");
		}
		return ByteBuffer.wrap(arr).order(bridgeByteOrder).getInt();
	}
	
	private byte[] fromInt(int i) {
		byte[] bytes = new byte[4];
	    ByteBuffer.wrap(bytes).order(bridgeByteOrder).putInt(i);
	    return bytes;
	}
	
	public byte[] fromDouble(double value) {
	    byte[] bytes = new byte[8];
	    ByteBuffer.wrap(bytes).order(bridgeByteOrder).putDouble(value);
	    return bytes;
	}
	
	private void readProperties() {
		Properties projectProperties = PropertiesReader.getProjectProperties();
		String byteOrder = projectProperties.getProperty("bridge.byteorder");
		if("big_endian".equals(byteOrder)) {
			bridgeByteOrder = ByteOrder.BIG_ENDIAN;
		} else if("little_endian".equals(byteOrder)) {
			bridgeByteOrder = ByteOrder.LITTLE_ENDIAN;
		} else {
			throw new IllegalStateException("Please specify bridge.byteorder in the project properties file. Should be either big_endian or little_endian");
		}
		
		port = Integer.parseInt(projectProperties.getProperty("bridge.port"));
	}

	private void openServerSocket() {
		try {
			this.serverSocket = new ServerSocket(port);
		} catch (IOException e) {
			throw new RuntimeException("Cannot open port " + port, e);
		}
	}
	
	public void closeServer() {
		try {
			if(serverSocket != null) {
				serverSocket.close();
			}
			if(clientSocket != null) {
				clientSocket.close();
			}
			if(os != null) {
				os.close();
			}
			if(is != null) {
				is.close();
			}
		} catch(IOException e) {
			e.printStackTrace();
		}
	}
	
	public static void main(String[] args) {
		SingleThreadedBridge server = new SingleThreadedBridge();
		server.listen();

		server.readData();
		server.sendData(IntStream.range(0, 20).asDoubleStream().toArray());
		
		server.closeServer();
		
	}
}
