package org.mma.imagerecognition.dao;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.IntStream;
import java.util.stream.Stream;

import javax.sql.DataSource;

import org.mma.imagerecognition.dataobjects.TrainingData;
import org.mma.imagerecognition.tools.PropertiesReader;

import com.mchange.v2.c3p0.ComboPooledDataSource;

public class TrainingDbDao {
	private static final String DbName = "TrainingDB";
	private static final String tableName = "trainingData";
	private static final String databaseURL = "jdbc:mysql://mydb.itu.dk/" + DbName;
	
	private static final String GET_IMAGES = "SELECT * FROM " + tableName + " WHERE id IN ";
	private static final String GET_WIDTH = "SELECT width FROM " + tableName + " LIMIT 1";
	private static final String GET_HEIGHT = "SELECT height FROM " + tableName + " LIMIT 1";
	private static final String GET_WIDTH_HEIGHT = "SELECT width, height FROM " + tableName + " LIMIT 1";
	private static final String GET_COLUMN_COUNT = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE table_schema = '" + DbName +
													"' AND table_name = '" + tableName + "'";
	private static final String GET_ROW_COUNT = "SELECT COUNT(*) FROM " + tableName;
	private static final String GET_COLUMN_NAMES = "SELECT column_name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME =  '" + tableName + "'";
	
	private static int numberOfNonGroundTruthColumns = 4;
	
	private static DataSource pooledDataSource = setupC3P0();
	
	public static List<TrainingData> getImages(String indices) {
		try (Connection conn = getConnection()) {
			PreparedStatement getTrainingDataPS = conn.prepareStatement(GET_IMAGES + indices);
		
			getTrainingDataPS.clearParameters();
			getTrainingDataPS.execute();
			ResultSet resultSet = getTrainingDataPS.getResultSet();
			
			List<TrainingData> result = new ArrayList<TrainingData>();
			while(resultSet.next()) {
				result.add(new TrainingData(resultSet));
			}
			return result;
		} catch (SQLException e) {
			e.printStackTrace();
		}
		throw new IllegalStateException();
	}
	
	public static List<TrainingData> getRandomImages(int numberOfImages) {
		return getImages(getRandomIndices(numberOfImages));
	}
	
	public static List<TrainingData> getTrainingData(int fromId, int toId) {
		return getImages(getIndices(fromId, toId));
	}
	
	public static int getWidth() {
		try (Connection conn = getConnection()) {
			PreparedStatement getWidthPS = conn.prepareStatement(GET_WIDTH);

			ResultSet rs = getWidthPS.executeQuery();
			rs.next();
			return rs.getInt(1);
		} catch (SQLException e) {
			e.printStackTrace();
		}
		throw new IllegalStateException();
	}
	
	public static int getHeight() {
		try (Connection conn = getConnection()) {
			PreparedStatement getHeightPS = conn.prepareStatement(GET_HEIGHT);
		
			ResultSet rs = getHeightPS.executeQuery();
			rs.next();
			return rs.getInt(1);
		} catch (SQLException e) {
			e.printStackTrace();
		}
		throw new IllegalStateException();
	}
	
	public static int getWidthHeightProduct() {
		try (Connection conn = getConnection()) {
			PreparedStatement getWidthHeightPS = conn.prepareStatement(GET_WIDTH_HEIGHT);

			ResultSet rs = getWidthHeightPS.executeQuery();
			rs.next();
			return rs.getInt(1) * rs.getInt(2);
		} catch (SQLException e) {
			e.printStackTrace();
		}
		throw new IllegalStateException();
	}
	
	public static int getNumberOfGroundTruths() {
		try (Connection conn = getConnection()) {
			PreparedStatement getColumnCountPS = conn.prepareStatement(GET_COLUMN_COUNT);

			ResultSet rs = getColumnCountPS.executeQuery();
			rs.next();
			return rs.getInt(1) - numberOfNonGroundTruthColumns;
		} catch (SQLException e) {
			e.printStackTrace();
		}
		throw new IllegalStateException();
	}
	
	public static int getTotalNumberOfImages() {
		try (Connection conn = getConnection()) {
			PreparedStatement getRowCountPS = conn.prepareStatement(GET_ROW_COUNT);
		
			ResultSet rs = getRowCountPS.executeQuery();
			rs.next();
			return rs.getInt(1);
		} catch (SQLException e) {
			e.printStackTrace();
		}
		throw new IllegalStateException();
	}
	
	public static List<String> getGroundTruthLabels() {
		List<String> result = new ArrayList<String>();
		try (Connection conn = getConnection()) {
			PreparedStatement getColumnNamesPS = conn.prepareStatement(GET_COLUMN_NAMES);
		
			ResultSet rs = getColumnNamesPS.executeQuery();
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
	
	private static Connection getConnection() {
		try {
			return pooledDataSource.getConnection();
		} catch (SQLException e) {
			e.printStackTrace();
			System.exit(0);
		}
		return null;
	}
	
	private static DataSource setupC3P0() {
		ComboPooledDataSource cpds = new ComboPooledDataSource();
		cpds.setJdbcUrl(databaseURL);
		cpds.setUser(PropertiesReader.getUserId());
		cpds.setPassword(PropertiesReader.getPassword());
		
		cpds.setIdleConnectionTestPeriod(120);

		cpds.setMinPoolSize(1);
		cpds.setAcquireIncrement(5);
		cpds.setMaxPoolSize(20);
		cpds.setMaxStatements( 50 ); //Prepared Statement pooling
		
		return cpds;
	}
	
	public static String getRandomIndices(int numberOfImages) {
		List<Integer> range = IntStream.rangeClosed(0, getTotalNumberOfImages()).boxed().collect(Collectors.toList());
		Collections.shuffle(range);
		return indicesToString(range.parallelStream(), numberOfImages);
	}
	
	public static String getIndices(int from, int to) {
		return indicesToString(IntStream.rangeClosed(from, to).boxed(), to-from + 1);
	}
	
	private static String indicesToString(Stream<Integer> indices, int limit) {
		return "(" + indices.limit(limit).map(Object::toString).collect(Collectors.joining(", ")) + ")";
	}
}
