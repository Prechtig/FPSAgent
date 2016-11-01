package org.mma.imagerecognition.configuration;

import java.io.FileNotFoundException;
import java.io.IOException;

import org.deeplearning4j.parallelism.ParallelWrapper;
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;

public class ContinuousParallelTraining extends ContinuousTraining {
	
	public ContinuousParallelTraining() throws FileNotFoundException, IOException {
		super();
	}

	@Override
	public void train(DataSetIterator trainIterator, DataSetIterator testIterator) {
		ParallelWrapper wrapper = new ParallelWrapper.Builder(model)
	        .prefetchBuffer(4)
	        .workers(4)
	        .averagingFrequency(1)
	        .reportScoreAfterAveraging(true)
	        .useLegacyAveraging(true)
	        .build();
			
		for(int i = latestEpoch; i <= nEpochs; i++) {
            wrapper.fit(trainIterator);
            System.out.println(String.format("*** Completed epoch %d ***", i));
            testIterator.reset();
            
            saveModel(model, i);
            outputDeadNeurons(model);
        }
	}
}
