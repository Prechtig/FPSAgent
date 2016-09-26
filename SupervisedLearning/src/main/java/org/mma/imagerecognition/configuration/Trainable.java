package org.mma.imagerecognition.configuration;

import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;

public interface Trainable {
	public void train(DataSetIterator trainIterator, DataSetIterator testIterator);
}
