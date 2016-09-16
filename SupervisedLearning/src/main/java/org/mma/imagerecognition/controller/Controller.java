package org.mma.imagerecognition.controller;

import java.io.IOException;

import org.mma.imagerecognition.dbaccess.TrainingDbDao;

public class Controller {
	public static void main(String[] args) throws IOException {
//		TrainingDbDao.initializeConnection();
		System.out.println(TrainingDbDao.getTotalNumberOfImages());
		System.out.println(TrainingDbDao.getGroundTruthLabels());
	}
}
