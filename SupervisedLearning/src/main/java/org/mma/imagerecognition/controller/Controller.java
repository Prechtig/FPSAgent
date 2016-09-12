package org.mma.imagerecognition.controller;

import java.io.IOException;

import org.mma.imagerecognition.dbaccess.TrainingDbDao;

public class Controller {
	public static void main(String[] args) throws IOException {
		TrainingDbDao.initializeConnection();
		System.out.println(TrainingDbDao.getWidthHeightProduct());
		
//        DataSetIterator mnistTest512 = new MnistDataSetIterator(20,512,false,false,true,12345);
//        
//        
//        while(mnistTest512.hasNext()) {
//        	DataSet next = mnistTest512.next();
//        	for(int i = 0; i < 20; i++) {
//        		org.nd4j.linalg.dataset.DataSet dataSet = next.get(i);
//        		
//        		System.out.println(dataSet.getFeatures());
//        		System.out.println(dataSet.getLabels());
//        		System.out.println(dataSet);
//        	}
//        	System.out.println("");
//        }
        
	}
}
