package org.mma.imagerecognition.iterator;

import java.util.List;

import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.nd4j.linalg.dataset.DataSet;

public class DatabaseIterator extends BaseIterator {

	private static final long serialVersionUID = -1553877073572732933L;
	private int totalExamples = -1;
	private List<String> labels;
	
	public DatabaseIterator(int batchSize, int numExamples) {
		super(batchSize, numExamples);
	}

	@Override
	public DataSet next(int num) {
		cursor++;
		return toDataSet(TrainingDbDao.getRandomImages(num));
	}
	
	@Override
	public int totalExamples() {
		if(totalExamples == -1) {
			totalExamples = FileSystemDAO.findLatestTrainingDataId();
		}
		return totalExamples;
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
	public List<String> getLabels() {
		if(labels == null) {
			labels = TrainingDbDao.getGroundTruthLabels();
		}
		return labels;
	}
}
