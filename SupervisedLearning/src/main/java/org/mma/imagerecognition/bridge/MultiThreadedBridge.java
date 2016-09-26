package org.mma.imagerecognition.bridge;

import java.net.ServerSocket;
import java.net.Socket;
import java.nio.ByteOrder;

import org.mma.imagerecognition.tools.PropertiesReader;

import java.io.IOException;

public class MultiThreadedBridge implements Runnable {

	protected int serverPort = 8080;
	protected ServerSocket serverSocket = null;
	protected boolean isStopped = false;
	protected Thread runningThread = null;
	protected final ByteOrder bridgeByteOrder;

	public MultiThreadedBridge(int port) {
		this.serverPort = port;
		bridgeByteOrder = determineByteOrder();
	}

	public void run() {
		synchronized (this) {
			this.runningThread = Thread.currentThread();
		}
		openServerSocket();
		while (!isStopped()) {
			Socket clientSocket = null;
			try {
				clientSocket = this.serverSocket.accept();
			} catch (IOException e) {
				if (isStopped()) {
					System.out.println("Server Stopped.");
					return;
				}
				throw new RuntimeException("Error accepting client connection", e);
			}
			new Thread(new WorkerRunnable(clientSocket, bridgeByteOrder)).start();
		}
		System.out.println("Server Stopped.");
	}

	private synchronized boolean isStopped() {
		return this.isStopped;
	}

	public synchronized void stop() {
		this.isStopped = true;
		try {
			this.serverSocket.close();
		} catch (IOException e) {
			throw new RuntimeException("Error closing server", e);
		}
	}

	private void openServerSocket() {
		try {
			this.serverSocket = new ServerSocket(this.serverPort);
		} catch (IOException e) {
			throw new RuntimeException("Cannot open port 8080", e);
		}
	}
	
	private static ByteOrder determineByteOrder() {
		String byteOrder = PropertiesReader.getProjectProperties().getProperty("bridge.byteorder");
		if("big_endian".equals(byteOrder)) {
			return ByteOrder.BIG_ENDIAN;
		} else if("little_endian".equals(byteOrder)) {
			return ByteOrder.LITTLE_ENDIAN;
		}
		throw new IllegalStateException("Please specify bridge.byteorder in the project properties file. Should be either big_endian or little_endian");
	}
	
	public static void main(String[] args) {
		MultiThreadedBridge server = new MultiThreadedBridge(4343);
		new Thread(server).start();

		try {
		    Thread.sleep(10 * 60 * 1000);
		} catch (InterruptedException e) {
		    e.printStackTrace();
		}
		System.out.println("Stopping Server");
		server.stop();
		
	}

}