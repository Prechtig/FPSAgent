package org.mma.imagerecognition.dao;

import java.io.IOException;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.nio.file.StandardOpenOption;
import java.util.Collections;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.IntStream;
import java.util.stream.Stream;

import org.mma.imagerecognition.dataobjects.TrainingData;

public class FileSystemDAO {
	
	private static final String ROOT_FOLDER = "trainingdata";
	private static final String FEATURE_FOLDER = "features";
	private static final String PIXEL_DATA_FOLDER = "pixeldata";
	
	public static List<TrainingData> getRandomImages(int amount) {
		List<Integer> range = IntStream.rangeClosed(1, findLatestTrainingDataId()).boxed().collect(Collectors.toList());
		Collections.shuffle(range);
		return load(range.parallelStream().limit(amount));
	}
	
	public static void persist(List<TrainingData> tds) {
		tds.parallelStream().forEach(td -> persist(td));
	}
	
	public static void persist(TrainingData td) {
		int id = td.getId();
		byte[] pixelData = td.getPixelData();
		int width = td.getWidth();
		int height = td.getHeight();
		double[] features = td.getFeatureDoubles();
		
		StringBuilder sb = new StringBuilder();
		sb.append(id).append(System.lineSeparator());
		sb.append(width).append(System.lineSeparator());
		sb.append(height).append(System.lineSeparator());
		for(double feature : features) {
			sb.append(feature).append(System.lineSeparator());
		}
		
		try {
			Files.write(getPixelDataPath(id), pixelData, StandardOpenOption.CREATE);
			Files.write(getFeaturePath(id), sb.toString().getBytes(StandardCharsets.UTF_8), StandardOpenOption.CREATE);
		} catch(IOException e) {
			e.printStackTrace();
			System.exit(31);
		}
	}
	
	public static List<TrainingData> load(List<Integer> ids) {
		return load(ids.parallelStream());
	}
	
	private static List<TrainingData> load(Stream<Integer> ids ) {
		return ids.map(id -> load(id)).collect(Collectors.toList());
	}
	
	public static TrainingData load(int id) {
		try {
			byte[] pixelData = Files.readAllBytes(getPixelDataPath(id));
			List<String> lines = Files.readAllLines(getFeaturePath(id));
			int width = Integer.valueOf(lines.get(1));
			int height = Integer.valueOf(lines.get(2));
			
			int offset = 3;
			double[] features = new double[lines.size() - offset];
			for(int i = offset; i < lines.size(); i++) {
				features[i-offset] = Double.valueOf(lines.get(i)); 
			}
			return new TrainingData(id, width, height, pixelData, features);
		} catch(IOException e) {
			e.printStackTrace();
			System.exit(31);
		}
		return null;
	}
	
	public static int findLatestTrainingDataId() {
		if(!Files.exists(getFeatureFolder())) {
			return 0;
		}
		try {
			return Files.walk(getFeatureFolder())
					.filter(Files::isRegularFile)
					.map(file -> Integer.valueOf(file.getFileName().toString())).sorted()
					.reduce(0, (curMax, cur) -> Math.max(curMax, cur));
		} catch(IOException e) {
			e.printStackTrace();
			System.exit(1);
			return 0;
		}
	}
	
	public static void createFolders() throws IOException {
		Files.createDirectories(getPersistenceFolder());
		Files.createDirectories(getFeatureFolder());
		Files.createDirectories(getPixelDataFolder());
	}
	
	private static Path getFeaturePath(int id) {
		return getFeatureFolder().resolve(Integer.toString(id));
	}
	
	private static Path getPixelDataPath(int id) {
		return getPixelDataFolder().resolve(Integer.toString(id));
	}
	
	private static Path getFeatureFolder() {
		return getPersistenceFolder().resolve(FEATURE_FOLDER);
	}
	
	private static Path getPixelDataFolder() {
		return getPersistenceFolder().resolve(PIXEL_DATA_FOLDER);
	}
	
	private static Path getPersistenceFolder() {
		return Paths.get(ROOT_FOLDER);
	}
}
