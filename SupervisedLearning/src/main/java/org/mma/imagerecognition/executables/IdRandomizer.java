package org.mma.imagerecognition.executables;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.Collections;
import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.IntStream;

import org.mma.imagerecognition.dao.DbConnector;

public class IdRandomizer extends DbConnector {
	
	private static final int BATCH_SIZE = 50;
	
	private static Connection conn;
	private static Statement stmt;
	
	public static void main(String[] args) {
		try(Connection conn = getConnection();
			Statement stmt = conn.createStatement()) {
			conn.setAutoCommit(false);
			IdRandomizer.conn = conn;
			IdRandomizer.stmt = stmt;
			randomizeIds();
			conn.commit();
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
	
	public static void randomizeIds() throws SQLException {
		copyIdColumn();
		deleteUniqueIdConstraint();
		int maxId = getMaxId();
		long currentTimeMillis = System.currentTimeMillis();
		updateIds(maxId);
		long currentTimeMillis2 = System.currentTimeMillis();
		System.out.println(String.format("Took %d", currentTimeMillis2 - currentTimeMillis));
		addUniqueIdConstraint(maxId);
	}
	
	private static void updateIds(int maxId) throws SQLException {
		System.out.println("Updating ids... ");
		PreparedStatement preparedStatement = conn.prepareStatement(String.format("UPDATE %s SET id = ? WHERE tmpId = ?", tableName));
		
		List<Integer> shuffledIndices = getShuffledIndices(maxId);
		IntStream.rangeClosed(1, maxId).boxed().sequential().forEach(id -> updateId(preparedStatement, id, shuffledIndices.get(id-1)));
		System.out.println("DONE");
	}
	
	private static void updateId(PreparedStatement preparedStatement, int oldId, int newId) {
		try {
			preparedStatement.setInt(1, newId);
			preparedStatement.setInt(2, oldId);
			preparedStatement.addBatch();
			
			if(oldId % BATCH_SIZE == 0) {
				preparedStatement.executeBatch();
				System.out.println(String.format("Changed % 8d ids", oldId));
			}
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
		ResultSet rs = stmt.executeQuery(String.format("SELECT MAX(tmpId) FROM %s", tableName));
		rs.next();
		return rs.getInt(1);
	}
	
	private static List<Integer> getShuffledIndices(int maxId) {
		List<Integer> shuffledIndices = IntStream.rangeClosed(1, maxId).boxed().collect(Collectors.toList());
		Collections.shuffle(shuffledIndices);
		return shuffledIndices;
	}
}