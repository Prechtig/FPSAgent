package org.mma.imagerecognition.executables;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;

import org.mma.imagerecognition.dao.DbConnector;


public class EqualDbDistribution extends DbConnector {
	
	private static final String countImagesWithOrWithoutBots = "SELECT COUNT(*) FROM " + tableName + " WHERE withinsight1 = ";
	
	public static void main(String[] args) {
		try(Connection conn = getConnection();
			Statement stmt = conn.createStatement()) {
			ResultSet imagesWithBotsRS = stmt.executeQuery(countImagesWithOrWithoutBots + "1");
			imagesWithBotsRS.next();
			int imagesWithBots = imagesWithBotsRS.getInt(1);
			System.out.println("There are " + imagesWithBots + " images with bots in the database");
			
			ResultSet imagesWithoutBotsRs = stmt.executeQuery(countImagesWithOrWithoutBots + "0");
			imagesWithoutBotsRs.next();
			int imagesWithoutBots = imagesWithoutBotsRs.getInt(1);
			System.out.println("There are " + imagesWithoutBots + " images without bots in the database");
			
			int rowsDeleted = 0;
			if (imagesWithBots < imagesWithoutBots) {
				// Delete images without bots
				rowsDeleted = deleteImages(stmt, imagesWithoutBots - imagesWithBots, false);
			} else if (imagesWithoutBots < imagesWithBots) {
				// Delete images with bots
				rowsDeleted = deleteImages(stmt, imagesWithBots - imagesWithoutBots, true);
			}
			if(0 < rowsDeleted) {
				reassignIds(stmt);
			}
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
	
	private static int deleteImages(Statement stmt, int count, boolean withBots) throws SQLException {
		String withWithout = withBots ? "with" : "without";
		System.out.println("Deleting " + count + " images " + withWithout + " bots");
		
		int withinSight = withBots ? 1 : 0;
		int rowsDeleted = stmt.executeUpdate("DELETE FROM " + tableName + " WHERE withinsight1 = " + withinSight + " LIMIT " + count);
		if(rowsDeleted != count) {
			throw new RuntimeException("Could not delete " + count + " rows as requested. Only deleted " + rowsDeleted + " rows.");
		}
		return rowsDeleted;
	}
	
	private static void reassignIds(Statement stmt) throws SQLException {
		System.out.println("Deleting column id");
		stmt.executeUpdate("ALTER TABLE " + tableName + " DROP COLUMN id");
		System.out.println("Adding column id");
		stmt.executeUpdate("ALTER TABLE " + tableName + " ADD id int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY AUTO_INCREMENT");
	}
}
