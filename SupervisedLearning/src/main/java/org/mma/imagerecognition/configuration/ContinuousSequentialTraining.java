package org.mma.imagerecognition.configuration;

import java.io.FileNotFoundException;
import java.io.IOException;

import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;

public class ContinuousSequentialTraining extends ContinuousTraining {
	
	public ContinuousSequentialTraining() throws FileNotFoundException, IOException {
		super();
	}

	@Override
	public void train(DataSetIterator trainIterator, DataSetIterator testIterator) {
        for(int i = latestEpoch; i <= nEpochs; i++) {
            model.fit(trainIterator);
            System.out.println(String.format("*** Completed epoch %d ***", i));
            testIterator.reset();
            
            saveModel(model, i);
            outputDeadNeurons(model);
        }
	}
}
