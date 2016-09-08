package org.mma.imagerecognition.controller;

import java.net.MalformedURLException;
import java.net.URL;
import java.util.Arrays;
import java.util.List;

import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.dbaccess.TrainingDbDao;
import org.mma.imagerecognition.tools.ImageTool;

public class Controller {
	public static void main(String[] args) throws MalformedURLException {
		if(args.length < 2 || args.length > 2) {
			throw new IllegalArgumentException("Must specify exactly two arguments  - database username and password");
		}
		
		TrainingDbDao.initializeConnection(args[0], args[1]);
		List<TrainingData> images = TrainingDbDao.getImages(2);
		int[][][] inputVolume = ImageTool.toInputVolume(new URL("http://www.intechopen.com/assets/default/css/i/icons/trokut.png"));
		System.out.println(Arrays.toString(ImageTool.toInputVolume(new URL("http://www.intechopen.com/assets/default/css/i/icons/trokut.png"))));
		
	}
}
