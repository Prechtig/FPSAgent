package org.mma.imagerecognition.controller;

import java.util.List;

import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.dbaccess.TrainingDbDao;

public class Controller {
	public static void main(String[] args) {
		TrainingDbDao.initializeConnection();
		List<TrainingData> images = TrainingDbDao.getImages(2);
	}
}
