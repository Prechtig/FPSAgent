package org.mma.imagerecognition.dbaccess;

import org.junit.Test;
import org.mma.imagerecognition.dao.TrainingDbDao;

public class TrainingDbDaoTest {

	@Test
	public void test() {
		int imagesPerRun = 500;
		int runs = 10;
		for(int i = 0; i < runs; i++) {
			downloadTrainingDb(imagesPerRun);
		}
	}
	
	private void downloadTrainingDb(int samples) {
		long startTime = System.currentTimeMillis();
		TrainingDbDao.getRandomImages(samples);
		System.out.println(String.format("It took %d milliseconds retrieving %d images from the db", ((System.currentTimeMillis() - startTime)), samples));
	}

}
