package org.mma.imagerecognition.dbaccess;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.tools.PropertiesReader;

public class TrainingDbDao {
	private static String databaseURL = "jdbc:mysql://mydb.itu.dk/TrainingDB";
	private static Connection conn = null;
	
	private static PreparedStatement getWidthHeightPS;
	private static PreparedStatement getRandomTrainingDataPS;
	private static String GET_WIDTH_HEIGHT = "SELECT width, height FROM trainingData LIMIT 1";
	
	
	public static void initializeConnection() {
		try {
			conn = DriverManager.getConnection(databaseURL, PropertiesReader.getUserId(), PropertiesReader.getPassword());
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
	
	public static List<TrainingData> getImages(int rows) {
		if(conn==null) {
			throw new IllegalStateException("Cannot execute queries before  onnection has been initialized.");
		}
		
		List<TrainingData> trainingSet = new ArrayList<>();
		String GET_RANDOM_IMAGES = "SELECT * FROM trainingData ORDER BY RAND() LIMIT " + rows; //TODO: Optimize this to use prepared statements correctly
		PreparedStatement ps = null;
		
		try {
			ps = conn.prepareStatement(GET_RANDOM_IMAGES);
			ps.execute();
			ResultSet resultSet = ps.getResultSet();
			
			while(resultSet.next()) {
				trainingSet.add(new TrainingData(resultSet));
			}
			
		} catch(Exception e){
			e.printStackTrace();;
		}finally {
			try {
				ps.close();
			} catch (SQLException e) {
				e.printStackTrace();
			}
		}
		
		return trainingSet;
	}
	
	public static int getWidthHeightProduct() {
		if(conn==null) {
			throw new IllegalStateException("Cannot execute queries before connection has been initialized.");
		}
		
		if(getWidthHeightPS == null) {
			try {
				getWidthHeightPS = conn.prepareStatement(GET_WIDTH_HEIGHT);
			} catch (SQLException e) {
				e.printStackTrace();
			}
		}
		
		int result = 0;
		try {
			ResultSet rs = getWidthHeightPS.executeQuery();
			rs.next();
			result = rs.getInt(1) * rs.getInt(2);
			
		} catch (SQLException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		if(result == 0) {
			throw new IllegalStateException();
		}
		
		return result;
	}
}
