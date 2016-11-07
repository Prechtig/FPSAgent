package org.mma.imagerecognition.executables;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.stream.Collectors;

import org.mma.imagerecognition.dao.DbConnector;

public class EnsurePercentageOfPicturesWithoutBots extends DbConnector {
	
	private static Connection conn;
	private static Statement stmt;
	
	public static void main(String[] args) throws SQLException {
		EnsurePercentageOfPicturesWithoutBots.EnsureImagePercentage(0.15);
	}
	
	public static void EnsureImagePercentage(double percentage) {
		try (Connection conn = getConnection(); Statement stmt = conn.createStatement()) {
			EnsurePercentageOfPicturesWithoutBots.conn = conn;
			EnsurePercentageOfPicturesWithoutBots.stmt = stmt;

			int totalNumberOfImages = getTotalNumberOfImages();

			List<Integer> idsOfRowsWithoutBot = getIdsOfRowsWithoutBot();
			Collections.shuffle(idsOfRowsWithoutBot);

			int imagesWithBots = totalNumberOfImages - idsOfRowsWithoutBot.size();

			int numberOfImagesWithBotsToKeep = (int) (-((imagesWithBots * percentage) / (percentage - 1)));

			int numberOfImagesWithBotsToDelete = idsOfRowsWithoutBot.size() - numberOfImagesWithBotsToKeep;

			List<Integer> idsToDelete = idsOfRowsWithoutBot.stream().limit(numberOfImagesWithBotsToDelete).collect(Collectors.toList());
			
			if(numberOfImagesWithBotsToDelete > 0) {
				deleteImages(idsToDelete);

				IdReassinger.reassignIds();
				IdRandomizer.randomizeIds();
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
	
	private static void deleteImages(List<Integer> idsToDelete) throws SQLException {
		int batchSize = 1000;
		
		for(int i = 0; i < idsToDelete.size(); i += batchSize) {
			String ids = "(" + idsToDelete.stream().skip(i).limit(batchSize).map(Object::toString).collect(Collectors.joining(", ")) + ")";
			stmt.executeUpdate(String.format("DELETE FROM %s WHERE id IN %S", tableName, ids));
			System.out.println(String.format("Deleted % 8d images", i+batchSize));
		}
	}
	
	private static int getTotalNumberOfImages() throws SQLException {
		ResultSet rs = stmt.executeQuery(String.format("SELECT COUNT(id) FROM %s", tableName));
		rs.next();
		return rs.getInt(1);
	}
	
	private static List<Integer> getIdsOfRowsWithoutBot() throws SQLException {
		Statement scrollableStmt = conn.createStatement(ResultSet.TYPE_FORWARD_ONLY, ResultSet.CONCUR_READ_ONLY);
		ResultSet rs = scrollableStmt.executeQuery(String.format("SELECT id FROM %s WHERE withinsight = 0", tableName));
		
		List<Integer> ids = new ArrayList<Integer>();
		while(rs.next()) {
			ids.add(rs.getInt(1));
		}
		return ids;
	}

}
