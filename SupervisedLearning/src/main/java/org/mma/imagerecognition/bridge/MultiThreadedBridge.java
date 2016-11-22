package org.mma.imagerecognition.bridge;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;
import java.nio.ByteOrder;

import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.deeplearning4j.util.ModelSerializer;

import org.mma.imagerecognition.tools.PropertiesReader;

public class MultiThreadedBridge implements Runnable {

	protected final int serverPort;
	protected ServerSocket serverSocket = null;
	protected boolean isStopped = false;
	protected Thread runningThread = null;
	protected final ByteOrder bridgeByteOrder;
	protected final MultiLayerNetwork network;

	public MultiThreadedBridge(File networkFile) {
		this.network = loadNetwork(networkFile);
		this.serverPort = determinePort();
		bridgeByteOrder = determineByteOrder();
	}
	
	private MultiLayerNetwork loadNetwork(File networkFile) {
		try {
			System.out.println(String.format("Using model %s", networkFile.getAbsolutePath()));
			return ModelSerializer.restoreMultiLayerNetwork(new FileInputStream(networkFile));
		} catch (IOException e) {
			throw new RuntimeException("Could not load network");
		}
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
			new Thread(new WorkerRunnable(clientSocket, bridgeByteOrder, network)).start();
		}
		System.out.println("Server Stopped.");
	}

	private synchronized boolean isStopped() {
		return isStopped;
	}

	public synchronized void stop() {
		this.isStopped = true;
		try {
			serverSocket.close();
		} catch (IOException e) {
			throw new RuntimeException("Error closing server", e);
		}
	}

	private void openServerSocket() {
		try {
			serverSocket = new ServerSocket(serverPort);
			System.out.println(String.format("Server started on port %d", serverPort));
		} catch (IOException e) {
			throw new RuntimeException(String.format("Cannot open port %d", serverPort), e);
		}
	}
	
	private static int determinePort() {
		String port = PropertiesReader.getProjectProperties().getProperty("bridge.port");
		if(port == null) {
			throw new IllegalStateException("Please specify bridge.port in the project properties file.");
		}
		return Integer.parseInt(port);
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
		MultiThreadedBridge server = new MultiThreadedBridge(new File("models/Shallow97%Acc.bin"));
		new Thread(server).start();

		try {
		    Thread.sleep(30 * 60 * 1000);
		} catch (InterruptedException e) {
		    e.printStackTrace();
		}
		System.out.println("Stopping Server");
		server.stop();
		
	}

}