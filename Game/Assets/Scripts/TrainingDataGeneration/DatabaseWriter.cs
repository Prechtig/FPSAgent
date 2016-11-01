using UnityEngine;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using System;

public class DatabaseWriter {

	public static string server = "mydb.itu.dk";
	public static string userID = "tadmin";
	public static string password = "";
	public static string database = "TrainingDB";

	public static MySqlConnection connection;
	public static MySqlCommand insertCommand;

	public static void InsertTrainingData(TrainingData td) {
		InitializeConnection ();
		PrepareInsertStatement ();

		insertCommand.Parameters.Clear ();

		insertCommand.Parameters.AddWithValue ("?pixeldata", td.GetScreenshot ().GetRGB ());
		insertCommand.Parameters.AddWithValue ("?width", td.GetScreenshot().GetWidth());
		insertCommand.Parameters.AddWithValue ("?height", td.GetScreenshot().GetHeight());

		float[] groundTruths = td.GetGroundTruths();
		int index = 0;

		insertCommand.Parameters.AddWithValue ("?horizontalangle",	groundTruths[index++]);
		insertCommand.Parameters.AddWithValue ("?verticalangle",	groundTruths[index++]);
		insertCommand.Parameters.AddWithValue ("?distance",			groundTruths[index++]);
		insertCommand.Parameters.AddWithValue ("?withinsight",		groundTruths[index++]);



		insertCommand.ExecuteNonQuery();
		CloseConnection ();
	}

	public static void Initialize() {
		LoadUserCredentials ();
		InitializeConnection ();
		PrepareInsertStatement ();
	}

	public static void CloseConnection() {
		connection.Close ();
	}

	private static void LoadUserCredentials() {
		userID = PropertiesReader.GetPropertyFile(PropertyFile.UserCredentials).GetProperty("userid");
		password = PropertiesReader.GetPropertyFile(PropertyFile.UserCredentials).GetProperty("password");
	}

	private static void InitializeConnection() {
		MySqlConnectionStringBuilder mysqlsb = new MySqlConnectionStringBuilder ();
		mysqlsb.Server = server;
		mysqlsb.UserID = userID;
		mysqlsb.Password = password;
		mysqlsb.Database = database;
		mysqlsb.Pooling = false;
		connection = new MySqlConnection(mysqlsb.ToString());
		connection.Open();
	}

	private static void PrepareInsertStatement() {
		if (connection == null)
			throw new InvalidOperationException ("Connection has not yet initialized");
		
		insertCommand = connection.CreateCommand();
		StringBuilder sb = new StringBuilder ();
		sb.Append ("INSERT INTO trainingData");
		sb.Append ("( pixeldata,  width,  height,  horizontalangle,  verticalangle,  distance,  withinsight)");
		sb.Append ("VALUES");
		sb.Append ("(?pixeldata, ?width, ?height, ?horizontalangle, ?verticalangle, ?distance, ?withinsight)");
		insertCommand.CommandText = sb.ToString ();
		insertCommand.Prepare ();

	}
}
