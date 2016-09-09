using UnityEngine;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using System;

public class DatabaseWriter {

	public static string server = "mydb.itu.dk";
	public static string userID = "tadmin";
	public static string password = "proword";
	public static string database = "TrainingDB";

	public static MySqlConnection connection;
	public static MySqlCommand insertCommand;

	public static string verticalAngleKey = "verticalangle";
	public static string horizontalAngleKey = "horizontalangle";
	public static string distanceKey = "distance";
	public static string withinSightKey = "withinsight";
	public static int numberOfBots = 5;

	public static void InsertTrainingData(TrainingData td) {
		if (connection == null)
			throw new InvalidOperationException ("Connection has not been initialized");
		if (insertCommand == null)
			throw new InvalidOperationException ("Insert statement has not been prepared");

		insertCommand.Parameters.Clear ();

		insertCommand.Parameters.AddWithValue ("?pixeldata", td.GetScreenshot ().GetRGB ());
		insertCommand.Parameters.AddWithValue ("?width", td.GetScreenshot().GetWidth());
		insertCommand.Parameters.AddWithValue ("?height", td.GetScreenshot().GetHeight());

		string[] keys = GetKeys ();
		int keyNumber;
		for (int i = 1; i <= numberOfBots; i++) {
			keyNumber = 1;
			foreach(string key in keys) {
				insertCommand.Parameters.AddWithValue (GetNumeratedKey(i, true, key), td.GetGroundTruths()[keyNumber+((i-1)*keys.Length)-1]);
				keyNumber++;
			}
		}

		insertCommand.ExecuteNonQuery();
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
		//Propertie
		userID = PropertiesReader.GetUserId ();
		password = PropertiesReader.GetPassword ();
	}

	private static void InitializeConnection() {
		MySqlConnectionStringBuilder mysqlsb = new MySqlConnectionStringBuilder ();
		mysqlsb.Server = server;
		mysqlsb.UserID = userID;
		mysqlsb.Password = password;
		mysqlsb.Database = database;
		connection = new MySqlConnection(mysqlsb.ToString());
		connection.Open();
	}

	private static void PrepareInsertStatement() {
		if (connection == null)
			throw new InvalidOperationException ("Connection has not yet initialized");
		
		insertCommand = connection.CreateCommand();
		StringBuilder sb = new StringBuilder ();
		sb.Append ("INSERT INTO trainingData(");
		sb.Append("pixeldata, width, height, ");
		sb.Append (GetCSNumeratedKeys(numberOfBots, false, GetKeys()));
		sb.Append (") VALUES(?pixeldata, ?width, ?height, ");
		sb.Append (GetCSNumeratedKeys(numberOfBots, true, GetKeys()));
		sb.Append (")");
		insertCommand.CommandText = sb.ToString ();
		insertCommand.Prepare ();

	}

	private static string GetCSNumeratedKeys(int numberOfBots, bool prefixAt, params string[] keys) {
		StringBuilder sb = new StringBuilder ();
		for(int i = 1; i <= numberOfBots; i++) {
			foreach(string key in keys) {
				sb.Append (GetNumeratedKey(i, prefixAt, key));
				if(!(i == numberOfBots && key == keys[keys.Length-1]))
					sb.Append(", ");
			}
		}

		return sb.ToString();
	}

	private static string GetNumeratedKey(int numeration, bool prefixAt, string key) {
		StringBuilder sb = new StringBuilder ();
		if (prefixAt)
			sb.Append ("?");
		sb.Append (key);
		sb.Append (numeration);
		return sb.ToString ();
	}

	private static string[] GetKeys() {
		return GetAsArray (horizontalAngleKey, verticalAngleKey, distanceKey, withinSightKey);
	}

	private static string[] GetAsArray(params string[] strings) {
		return strings;
	}
}
