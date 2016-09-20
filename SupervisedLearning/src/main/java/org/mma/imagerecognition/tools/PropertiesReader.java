package org.mma.imagerecognition.tools;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

public class PropertiesReader {
	
	private static char fsc = File.separatorChar;
	private static String propertiesPath = ".." + fsc + "config" + fsc;
	private static String userCredentialsPath = propertiesPath + "user_credentials.properties";
	private static String projectPath = propertiesPath + "project.properties";
	
	private static Properties userCredentialsProperties;
	private static Properties projectProperties;
	
	private static String userKey = "userid";
	private static String passwordKey = "password";
	
	public static String getUserId() {
		return getUserCredentialsProperties().getProperty(userKey);
	}
	
	public static String getPassword() {
		return getUserCredentialsProperties().getProperty(passwordKey);
	}
	
	private static Properties getUserCredentialsProperties() {
		if(userCredentialsProperties == null) {
			try {
				InputStream is = new FileInputStream(new File(userCredentialsPath));
				userCredentialsProperties = new Properties();
				userCredentialsProperties.load(is);
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
		return userCredentialsProperties;
	}
	
	public static Properties getProjectProperties() {
		if(projectProperties == null) {
			try {
				InputStream is = new FileInputStream(new File(projectPath));
				projectProperties = new Properties();
				projectProperties.load(is);
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
		return projectProperties;
	}
}
