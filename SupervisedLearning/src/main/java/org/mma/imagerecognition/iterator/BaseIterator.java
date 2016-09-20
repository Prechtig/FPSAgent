package org.mma.imagerecognition.iterator;

import java.util.List;

import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.tools.ImageTool;
import org.nd4j.linalg.api.ndarray.INDArray;
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
		DataSet dataSet = new DataSet();
		
		double[] labelValues = new double[ batch() * totalOutcomes() ];
		double[][] features = new double[trainingData.size()][];
		
		for(int i = 0; i < trainingData.size(); i++) {
			TrainingData td = trainingData.get(i);
			
			double[] groundTruths = td.getFeatureDoubles();
			System.arraycopy(groundTruths, 0, labelValues, i * groundTruths.length, groundTruths.length);
			features[i] = ImageTool.toScaledDoubleStream(td.getPixelData()).toArray();
		}
		dataSet.setFeatures(ImageTool.convertToINDArray(features));
		INDArray labels = Nd4j.create(labelValues, new int[] { batch(), totalOutcomes() }, 'c');
		dataSet.setLabels(labels);
		
		return dataSet;
	}
}
