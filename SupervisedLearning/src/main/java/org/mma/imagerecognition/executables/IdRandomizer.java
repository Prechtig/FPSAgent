package org.mma.imagerecognition.executables;

import java.sql.Connection;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.Collections;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.IntStream;

import org.mma.imagerecognition.dao.DbConnector;

public class IdRandomizer extends DbConnector {
	
	private static Statement stmt;
	
	public static void main(String[] args) {
		try(Connection conn = getConnection();
			Statement stmt = conn.createStatement()) {
			IdRandomizer.stmt = stmt;
			randomizeIds();
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
	
	public static void randomizeIds() throws SQLException {
		copyIdColumn();
		deleteUniqueIdConstraint();
		int maxId = getMaxId();
		updateIds(maxId);
		addUniqueIdConstraint(maxId);
	}
	
	private static void updateIds(int maxId) {
		System.out.print("Updating ids... ");
		List<Integer> shuffledIndices = getShuffledIndices(maxId);
		IntStream.rangeClosed(1, maxId).boxed().sequential().forEach(id -> updateId(id, shuffledIndices.get(id-1)));
		System.out.println("DONE");
	}
	
	private static void updateId(int oldId, int newId) {
		try {
			stmt.executeUpdate(String.format("UPDATE %s SET id = %d WHERE tmpId = %d", tableName, newId, oldId));
		} catch (SQLException e) {
			e.printStackTrace();
			System.exit(1);
		}
	}
	
	private static void copyIdColumn() throws SQLException {
		System.out.print("Setting temporary ids... ");
		stmt.executeUpdate(String.format("UPDATE %s SET tmpId = id", tableName));
		System.out.println("DONE");
	}
	
	private static void deleteUniqueIdConstraint() throws SQLException {
		System.out.print("Deleting constraints... ");
		stmt.executeUpdate(String.format("ALTER TABLE %s MODIFY id int(11) NOT NULL", tableName));
		stmt.executeUpdate(String.format("ALTER TABLE %s DROP PRIMARY KEY", tableName));
		System.out.println("DONE");
	}
	
	private static void addUniqueIdConstraint(int autoIncrementValue) throws SQLException {
		System.out.print("Adding constraints... ");
		stmt.executeUpdate(String.format("ALTER TABLE %s MODIFY id int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY", tableName));
		stmt.executeUpdate(String.format("ALTER TABLE %s AUTO_INCREMENT=%d", tableName, autoIncrementValue));
		System.out.println("DONE");
		
	}
	
	private static int getMaxId() throws SQLException {
		ResultSet rs = stmt.executeQuery(String.format("SELECT MAX(id) FROM %s", tableName));
		rs.next();
		return rs.getInt(1);
	}
	
	private static List<Integer> getShuffledIndices(int maxId) {
		List<Integer> shuffledIndices = IntStream.rangeClosed(1, maxId).boxed().collect(Collectors.toList());
		Collections.shuffle(shuffledIndices);
		return shuffledIndices;
	}
}