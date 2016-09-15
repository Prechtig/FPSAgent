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
	private static final String DbName = "TrainingDB";
	private static final String tableName = "trainingData";
	private static final String databaseURL = "jdbc:mysql://mydb.itu.dk/" + DbName;
	private static Connection conn = null;
	
	private static PreparedStatement getWidthHeightPS;
	private static PreparedStatement getRandomTrainingDataPS;
	private static PreparedStatement getColumnCountPS;
	private static PreparedStatement getRowCountPS;
	private static PreparedStatement getColumnNamesPS;
	private static PreparedStatement getWidthPS;
	private static PreparedStatement getHeightPS;
	
	private static int numberOfNonGroundTruthColumns = 4;
	
	private static String ERROR_MESSAGE = "Cannot execute queries before connection has been initialized.";
	
	public static void initializeConnection() {
		try {
			conn = DriverManager.getConnection(databaseURL, PropertiesReader.getUserId(), PropertiesReader.getPassword());
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
	
	public static List<TrainingData> getImages(int rows) {
		if(conn==null) {
			throw new IllegalStateException(ERROR_MESSAGE);
		}
		
		
		if(getRandomTrainingDataPS == null) {
			try {
				final String GET_RANDOM_IMAGES = "SELECT * FROM " + tableName + " ORDER BY RAND() LIMIT ?";
				getRandomTrainingDataPS = conn.prepareStatement(GET_RANDOM_IMAGES);
			} catch (SQLException e) {
				e.printStackTrace();
			}
		}
		
		List<TrainingData> trainingSet = new ArrayList<>();
		try {
			getRandomTrainingDataPS.clearParameters();
			getRandomTrainingDataPS.setInt(1, rows);
			
			getRandomTrainingDataPS.execute();
			ResultSet resultSet = getRandomTrainingDataPS.getResultSet();
			
			while(resultSet.next()) {
				trainingSet.add(new TrainingData(resultSet));
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
		
		return trainingSet;
	}
	
	public static int getWidth() {
		if(conn==null) {
			throw new IllegalStateException(ERROR_MESSAGE);
		}
		
		
		if(getWidthPS == null) {
			try {
				final String GET_WIDTH_HEIGHT = "SELECT width FROM " + tableName + " LIMIT 1";
				getWidthPS = conn.prepareStatement(GET_WIDTH_HEIGHT);
			} catch (SQLException e) {
				e.printStackTrace();
			}
		}
		
		try {
			ResultSet rs = getWidthPS.executeQuery();
			rs.next();
			return rs.getInt(1);
			
		} catch (SQLException e) {
			e.printStackTrace();
		}
		
		throw new IllegalStateException();
	}
	
	public static int getHeight() {
		if(conn==null) {
			throw new IllegalStateException(ERROR_MESSAGE);
		}
		
		
		if(getHeightPS == null) {
			try {
				final String GET_WIDTH_HEIGHT = "SELECT height FROM " + tableName + " LIMIT 1";
				getHeightPS = conn.prepareStatement(GET_WIDTH_HEIGHT);
			} catch (SQLException e) {
				e.printStackTrace();
			}
		}
		
		try {
			ResultSet rs = getHeightPS.executeQuery();
			rs.next();
			return rs.getInt(1);
			
		} catch (SQLException e) {
			e.printStackTrace();
		}
		
		throw new IllegalStateException();
	}
	
	public static int getWidthHeightProduct() {
		if(conn==null) {
			throw new IllegalStateException(ERROR_MESSAGE);
		}
		
		
		if(getWidthHeightPS == null) {
			try {
				final String GET_WIDTH_HEIGHT = "SELECT width, height FROM " + tableName + " LIMIT 1";
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
	
	public static int getNumberOfGroundTruths() {
		if(conn==null) {
			throw new IllegalStateException(ERROR_MESSAGE);
		}
		
		
		if(getColumnCountPS == null) {
			try {
				final String GET_COLUMN_COUNT = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE table_schema = '" + DbName + "' AND table_name = '" + tableName + "'";
				getColumnCountPS = conn.prepareStatement(GET_COLUMN_COUNT);
			} catch (SQLException e) {
				e.printStackTrace();
			}
		}
		
		int columnCount = 0;
		
		try {
			ResultSet rs = getColumnCountPS.executeQuery();
			rs.next();
			columnCount = rs.getInt(1) - numberOfNonGroundTruthColumns;
			
		} catch (SQLException e) {
			e.printStackTrace();
		}
		
		return columnCount;
	}
	
	public static int getTotalNumberOfImages() {
		if(conn==null) {
			throw new IllegalStateException(ERROR_MESSAGE);
		}
		
		
		if(getRowCountPS == null) {
			try {
				final String GET_ROW_COUNT = "SELECT COUNT(*) FROM " + tableName;
				getRowCountPS = conn.prepareStatement(GET_ROW_COUNT);
			} catch (SQLException e) {
				e.printStackTrace();
			}
		}
		
		try {
			ResultSet rs = getRowCountPS.executeQuery();
			rs.next();
			return rs.getInt(1);
			
		} catch (SQLException e) {
			e.printStackTrace();
		}
		
		throw new IllegalStateException();
	}
	
	public static List<String> getGroundTruthLabels() {
		if(conn==null) {
			throw new IllegalStateException(ERROR_MESSAGE);
		}
		
		
		if(getColumnCountPS == null) {
			try {
				final String GET_COLUMN_NAMES = "SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME =  '" + tableName + "'";
				getColumnCountPS = conn.prepareStatement(GET_COLUMN_NAMES);
			} catch (SQLException e) {
				e.printStackTrace();
			}
		}
		
		List<String> result = new ArrayList<String>();
		
		try {
			ResultSet rs = getColumnCountPS.executeQuery();
			for(int i = 0; i < numberOfNonGroundTruthColumns; i++) {
				rs.next();
			}
			
			while(rs.next()) {
				result.add(rs.getString(1));
			}
			
		} catch (SQLException e) {
			e.printStackTrace();
		}
		
		if(result.isEmpty()) {
			throw new IllegalStateException("Result was empty");
		}
		
		return result;
	}
}
