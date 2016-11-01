package org.mma.imagerecognition.iterator;

import java.util.List;

import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.nd4j.linalg.dataset.DataSet;

public class FileSystemIterator extends BaseIterator {

	private static final long serialVersionUID = 4292186710513240524L;

	public FileSystemIterator(int batchSize, int numExamples) {
		super(batchSize, numExamples);
	}

	@Override
	public DataSet next(int num) {
		cursor++;
		return toDataSet(FileSystemDAO.getRandomImages(num));
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
			TrainingData td = FileSystemDAO.load(1);
			inputColumns = td.getWidth() * td.getHeight() * 3;
		}
		return inputColumns;
	}

	/**
	 * Maximum number of truth values
	 */
	@Override
	public int totalOutcomes() {
		if(outputColumns == -1) {
			outputColumns = FileSystemDAO.load(1).getFeatures().length;
		}
		return outputColumns;
	}

	@Override
	public List<String> getLabels() {
		throw new UnsupportedOperationException();
	}
}
