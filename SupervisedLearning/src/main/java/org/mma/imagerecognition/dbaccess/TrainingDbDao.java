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
}
