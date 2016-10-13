package org.mma.imagerecognition.executables;

import java.io.File;
import java.io.IOException;
import java.util.List;

import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.tools.ImageTool;

public class Sampler {

	public static void main(String[] args) throws IOException {
		int imagesToPersist = 250;
		FileSystemDAO.createFolders();
		persistRepresentativeSampleOfDatabase(imagesToPersist);
		System.out.format("Persisted %d sample images in samples folder", imagesToPersist);
	}

	public static void persistRepresentativeSampleOfDatabase(int sampleSize) throws IOException {
		List<TrainingData> randomImages = TrainingDbDao.getRandomImages(sampleSize);
		
		for(TrainingData td : randomImages) {
			ImageTool.printColoredPngImage(td.getPixelData(), td.getWidth(), new File(FileSystemDAO.getSamplesFolder().resolve("image" + td.getId() + ".png").toString()));
		}
	}
}
