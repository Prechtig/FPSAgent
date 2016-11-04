package org.mma.imagerecognition.bridge;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.Socket;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.stream.DoubleStream;
import java.util.stream.IntStream;

import org.apache.commons.io.IOUtils;
import org.apache.commons.lang3.ArrayUtils;
import org.deeplearning4j.nn.multilayer.MultiLayerNetwork;
import org.mma.imagerecognition.tools.INDArrayTool;
import org.mma.imagerecognition.tools.ImageTool;
import org.mma.imagerecognition.tools.PropertiesReader;
import org.nd4j.linalg.api.ndarray.INDArray;
import org.nd4j.linalg.factory.Nd4j;

public class WorkerRunnable implements Runnable {

	protected final Socket clientSocket;
	protected final ByteOrder bridgeByteOrder;
	protected InputStream is;
	protected OutputStream os;
	protected final MultiLayerNetwork network;
	protected final int width, height;

	public WorkerRunnable(Socket clientSocket, ByteOrder bridgeByteOrder, MultiLayerNetwork network) {
		this.clientSocket = clientSocket;
		this.bridgeByteOrder = bridgeByteOrder;
		this.network = network;
		width = Integer.parseInt(PropertiesReader.getProjectProperties().getProperty("training.image.width"));
		height = Integer.parseInt(PropertiesReader.getProjectProperties().getProperty("training.image.height"));
	}

	public void run() {
		try {
			is = clientSocket.getInputStream();
			os = clientSocket.getOutputStream();
			for(int i = 0;; i++) {
				// Receiving
				int length = readLength();
				if(length == 0) {
					System.out.println(String.format("Handled %d calls", i));
					break;
				}
				
				// Read pixel data
				byte[] data = readData(length);
				
				// Run the image through the network
				INDArray output = network.output(Nd4j.create(ImageTool.toScaledDoubles(data), new int[] { 1, 3, width, height}), false);
				
				// Send the output of the network
				sendData(INDArrayTool.toFlatDoubleArray(output));
			}
		} catch (IOException e) {
			// report exception somewhere.
			e.printStackTrace();
		}
	}
	
	@SuppressWarnings("unused")
	private void sendFakeOutput() throws IOException {
		sendData(IntStream.range(0, 20).asDoubleStream().toArray());
	}
	
	private void sendData(double[] ds) throws IOException {
		byte[] data = DoubleStream.of(ds)
			.boxed()
			.map(d -> fromDouble(d))
			.reduce(new byte[0], (cumArr, curArr) -> ArrayUtils.addAll(cumArr, curArr));
		sendData(data);
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
}