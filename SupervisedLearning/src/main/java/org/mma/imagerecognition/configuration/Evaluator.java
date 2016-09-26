package org.mma.imagerecognition.configuration;

import org.deeplearning4j.eval.Evaluation;
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.nd4j.linalg.dataset.api.iterator.DataSetIterator;

public class Evaluator {
	public static void Evaluate(DataSetIterator testIterator) {
		int featureCount = TrainingDbDao.getNumberOfGroundTruths();
        System.out.println("Evaluate model....");
        
        Evaluation eval = new Evaluation(featureCount);
//        while(testIterator.hasNext()){
//            DataSet ds = testIterator.next();
//            INDArray output = model.output(ds.getFeatureMatrix(), false);
//            eval.eval(ds.getLabels(), output);
//        }
	}
}
