package org.mma.imagerecognition.configuration;

import java.util.List;

import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.dbaccess.TrainingDbDao;
import org.mma.imagerecognition.tools.ImageTool;
import org.nd4j.linalg.api.ndarray.INDArray;
import org.nd4j.linalg.dataset.DataSet;
import org.nd4j.linalg.dataset.api.DataSetPreProcessor;
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;
import org.nd4j.linalg.factory.Nd4j;

public class DbIterator implements DataSetIterator {

	private static final long serialVersionUID = 4292186710513240524L;
	
	private final int batchSize, numExamples, batches;
	private int cursor = 0;
	private int inputColumns = -1;
	private int outputColumns = -1;
	
	private List<String> labels;
	

	public DbIterator(int batchSize, int numExamples) {
		this.batchSize = batchSize;
		this.numExamples = numExamples;
//		TrainingDbDao.initializeConnection();
		
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
	public DataSet next(int num) {
		cursor++;
		return toDataSet(TrainingDbDao.getImages(num));
	}
	
	private DataSet toDataSet(List<TrainingData> trainingData) {
		DataSet dataSet = new DataSet();
		
		double[] labelValues = new double[ batch() * totalOutcomes() ];
		double[][] features = new double[trainingData.size()][];
		
		for(int i = 0; i < trainingData.size(); i++) {
			TrainingData td = trainingData.get(i);
			
			double[] groundTruths = td.getFeatureDoubles();
			System.arraycopy(groundTruths, 0, labelValues, i * groundTruths.length, groundTruths.length);
			
//			ImageTool.convertToINDArray(ImageTool.toScaledDoubleStream(td.getPixelData()), td.getWidth());
//			// TODO: Not correct
//			if(i == 0) {
//				dataSet.setFeatures(features);
//			} else {
//				dataSet.addFeatureVector(features);
//			}
			features[i] = ImageTool.toScaledDoubleStream(td.getPixelData()).toArray();
		}
		dataSet.setFeatures(ImageTool.convertToINDArray(features));
		INDArray labels = Nd4j.create(labelValues, new int[] { batch(), totalOutcomes() }, 'c');
		dataSet.setLabels(labels);
		
		return dataSet;
	}

	@Override
	public int totalExamples() {
		return TrainingDbDao.getTotalNumberOfImages();
	}

	/**
	 * Length of the flattened input array
	 */
	@Override
	public int inputColumns() {
		if(inputColumns == -1) {
			inputColumns = TrainingDbDao.getWidthHeightProduct();
		}
		return inputColumns;
	}

	/**
	 * Maximum number of truth values
	 */
	@Override
	public int totalOutcomes() {
		if(outputColumns == -1) {
			outputColumns = TrainingDbDao.getNumberOfGroundTruths();
		}
		return outputColumns;
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

	@Override
	public List<String> getLabels() {
		if(labels == null) {
			labels = TrainingDbDao.getGroundTruthLabels();
		}
		return labels;
		
	}
}
