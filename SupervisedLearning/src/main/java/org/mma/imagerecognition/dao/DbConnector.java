package org.mma.imagerecognition.dao;

import java.sql.Connection;
import java.sql.SQLException;

import javax.sql.DataSource;

import org.mma.imagerecognition.tools.PropertiesReader;

import com.mchange.v2.c3p0.ComboPooledDataSource;

public abstract class DbConnector {
	
	protected static final String DbName = "TrainingDB";
	protected static final String tableName = "trainingData";
	protected static final String databaseURL = "jdbc:mysql://mydb.itu.dk/" + DbName;
	
	protected static DataSource pooledDataSource = setupC3P0();
	
	protected static Connection getConnection() {
		try {
			return pooledDataSource.getConnection();
		} catch (SQLException e) {
			e.printStackTrace();
			System.exit(0);
		}
		return null;
	}
	
	protected static DataSource setupC3P0() {
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
	
	public static String getTableName() {
		return tableName;
	}
}
