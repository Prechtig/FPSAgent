package org.mma.imagerecognition.iterator;

import java.util.List;

import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.tools.ImageTool;
import org.nd4j.linalg.dataset.DataSet;
import org.nd4j.linalg.dataset.api.DataSetPreProcessor;
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;
import org.nd4j.linalg.factory.Nd4j;

public abstract class BaseIterator implements DataSetIterator {

	private static final long serialVersionUID = 5040904762180029909L;
	protected final int batchSize, numExamples, batches;
	protected int cursor = 0;
	protected int inputColumns = -1;
	protected int outputColumns = -1;
	
	public BaseIterator(int batchSize, int numExamples) {
		this.batchSize = batchSize;
		this.numExamples = numExamples;
		batches = numExamples/batchSize + (numExamples % batchSize == 0 ? 0 : 1);
	}

	@Override
	public boolean hasNext() {
		return cursor+1 < batches;
	}

	@Override
	public DataSet next() {
		return next(batchSize);
	}

	@Override
	public boolean resetSupported() {
		return true;
	}

	@Override
	public void reset() {
		cursor = 0;
	}

	@Override
	public int batch() {
		return batchSize;
	}

	@Override
	public int cursor() {
		return cursor;
	}

	@Override
	public int numExamples() {
		return numExamples;
	}

	@Override
	public void setPreProcessor(DataSetPreProcessor preProcessor) {
		throw new UnsupportedOperationException();
	}

	@Override
	public DataSetPreProcessor getPreProcessor() {
		return null;
	}
	
	protected DataSet toDataSet(List<TrainingData> trainingData) {
		double[][] pixelData			= new double[batch()][];
		double[][] groundTruthValues	= new double[batch()][];
		
		for(int i = 0; i < trainingData.size(); i++) {
			TrainingData td = trainingData.get(i);
			pixelData[i] = ImageTool.toScaledDoubleStream(td.getPixelData()).toArray();
			groundTruthValues[i] = td.getFeatureDoubles();
		}
		return new DataSet(Nd4j.create(pixelData), Nd4j.create(groundTruthValues));
	}
	
	@Override
	public boolean asyncSupported() {
		return false;
	}
}
