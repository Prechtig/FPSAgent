package org.mma.imagerecognition.dbaccess;

import static org.junit.Assert.assertEquals;

import java.util.List;

import org.junit.Test;
import org.mma.imagerecognition.dataobjects.TrainingData;

public class TrainingDbDaoTest {

	@Test
	public void test() {
		int images = 10;
		
		long startTime = System.currentTimeMillis();
		List<TrainingData> td = TrainingDbDao.getImages(images);
		System.out.println(String.format("It took %d milliseconds retrieving %d images from the db", ((System.currentTimeMillis() - startTime)), images));
		assertEquals(images, td.size());
		
		startTime = System.currentTimeMillis();
		TrainingDbDao.getImages(images);
		System.out.println(String.format("It took %d milliseconds retrieving %d images from the db", ((System.currentTimeMillis() - startTime)), images));
		
		startTime = System.currentTimeMillis();
		TrainingDbDao.getImages(images);
		System.out.println(String.format("It took %d milliseconds retrieving %d images from the db", ((System.currentTimeMillis() - startTime)), images));
		
		assertEquals(1d,  1d, 1d);
	}

}
