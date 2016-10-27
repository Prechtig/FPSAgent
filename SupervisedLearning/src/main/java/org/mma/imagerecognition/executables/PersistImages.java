package org.mma.imagerecognition.executables;

import java.io.File;
import java.io.IOException;

import org.mma.imagerecognition.dao.FileSystemDAO;
import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.tools.ImageTool;

public class PersistImages {
	
	public static void main(String[] args) throws IOException {
		saveImages(250);
	}
	
	public static void saveImages(int upToId) throws IOException {
		int maxIdToPersist = Math.min(FileSystemDAO.findLatestTrainingDataId(), upToId);
		
		new File("images").mkdirs();
		
		System.out.println(String.format("Persisting  %d images", maxIdToPersist));
		
		for(int i = 1; i <= maxIdToPersist; i++) {
			TrainingData td = FileSystemDAO.load(i);
			ImageTool.printColoredPngImage(td.getPixelData(), td.getWidth(), new File("images" + File.separator + i + ".png"));
			if(i % 100 == 0 || i == maxIdToPersist) {
				System.out.println(String.format("Persisted % 5d images", i));
			}
		}
	}
}
