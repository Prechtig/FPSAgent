package org.mma.imagerecognition.configuration;

import static org.junit.Assert.*;

import org.junit.Assert;
import org.junit.Test;
import org.mma.imagerecognition.iterator.DatabaseIterator;
import org.nd4j.linalg.dataset.DataSet;

public class DbIteratorTest {

	@Test
	public void asdasd() {
		final int batchSize = 3;
		DatabaseIterator it = new DatabaseIterator(batchSize, 5);
		Assert.assertTrue(it.hasNext());
		
		DataSet first = it.next();
		
		assertArrayEquals(new int[] {batchSize, 640 * 360 * 3}, first.getFeatures().shape());
	}
}