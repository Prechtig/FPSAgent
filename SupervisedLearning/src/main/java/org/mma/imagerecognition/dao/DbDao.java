package org.mma.imagerecognition.dao;

import java.io.ByteArrayInputStream;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;
import java.util.List;

import org.mma.imagerecognition.dataobjects.TrainingData;

public class DbDao extends TrainingDbDao {
	
	private final String tableName;
	private static final String insertSQL = "INSERT INTO %s (pixeldata, width, height, horizontalangle, verticalangle, distance, withinsight) VALUES (?, ?, ?, ?, ?, ?, ?)";
	
	public DbDao(String tableName) {
		this.tableName = tableName;
	}
	
	public void persist(List<TrainingData> tds) {
		for(TrainingData td : tds) {
			persist(td);
		}
	}
	
	public void persist(TrainingData td) {
		try (Connection conn = getConnection()) {
			PreparedStatement insertStatement = conn.prepareStatement(String.format(insertSQL, tableName));
			
			int idx = 1;
			
			insertStatement.setBinaryStream(idx++, new ByteArrayInputStream(td.getPixelData()));
			insertStatement.setInt(idx++, td.getWidth());
			insertStatement.setInt(idx++, td.getHeight());
			insertStatement.setDouble(idx++, td.getHorizontalAngle());
			insertStatement.setDouble(idx++, td.getVerticalAngle());
			insertStatement.setDouble(idx++, td.getDistance());
			insertStatement.setDouble(idx++, td.isWithinSight() ? 1d : 0d);
			
			insertStatement.execute();
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
}
