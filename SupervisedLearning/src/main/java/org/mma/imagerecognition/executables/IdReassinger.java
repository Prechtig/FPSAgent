package org.mma.imagerecognition.executables;

import java.sql.Connection;
import java.sql.SQLException;
import java.sql.Statement;

import org.mma.imagerecognition.dao.DbConnector;

public class IdReassinger extends DbConnector {
	public static void main(String[] args) {
		reassignIds();
	}

	public static void reassignIds() {
		try (Connection conn = getConnection(); Statement stmt = conn.createStatement()) {
			System.out.println("Deleting column id");
			stmt.executeUpdate("ALTER TABLE " + tableName + " DROP COLUMN id");
			System.out.println("Adding column id");
			stmt.executeUpdate("ALTER TABLE " + tableName + " ADD id int(11) NOT NULL AUTO_INCREMENT PRIMARY KEY AUTO_INCREMENT AFTER height");
		} catch (SQLException e) {
			e.printStackTrace();
		}
	}
}
