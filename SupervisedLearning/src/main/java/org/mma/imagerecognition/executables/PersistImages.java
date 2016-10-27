package org.mma.imagerecognition.executables;

import java.io.File;
import java.io.IOException;
import java.util.List;

import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dao.TrainingDbDao;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.tools.ImageTool;

public class PersistImages {
	
	private static final String imagesFolder = "images";
	private static final String imagesWithBotsFolder = "images-with-bots";
	private static final String imagesWithoutBotsFolder = "images-without-bots";
	
	public static void main(String[] args) throws IOException {
		int upToId = 250;
		
		int maxPersistedId = FileSystemDAO.findLatestTrainingDataId();
		
		if(maxPersistedId < upToId) {
			List<TrainingData> trainingData = TrainingDbDao.getTrainingData(1, upToId);
			FileSystemDAO.createFolders();
			FileSystemDAO.persist(trainingData);
		}
		
		createFolders();
		
		saveImages(upToId);
	}
	
	public static void saveImages(int upToId) throws IOException {
		System.out.println(String.format("Persisting % 5d images", upToId));
		
		for(int i = 1; i <= upToId; i++) {
			TrainingData td = FileSystemDAO.load(i);
			if(td.getFeatureDoubles()[3] == 1d) {
				ImageTool.printColoredPngImage(td.getPixelData(), td.getWidth(), new File(imagesWithBotsFolder + File.separator + td.getId() + ".png"));
			} else {
				ImageTool.printColoredPngImage(td.getPixelData(), td.getWidth(), new File(imagesWithoutBotsFolder + File.separator + td.getId() + ".png"));
			}
			ImageTool.printColoredPngImage(td.getPixelData(), td.getWidth(), new File(imagesFolder + File.separator + td.getId() + ".png"));
			if(i % 100 == 0 || i == upToId) {
				System.out.println(String.format("Persisted % 6d images", i));
			}
		}
	}
	
	public static void createFolders() {
		new File(imagesFolder).mkdirs();
		new File(imagesWithBotsFolder).mkdirs();
		new File(imagesWithoutBotsFolder).mkdirs();
	}
}
