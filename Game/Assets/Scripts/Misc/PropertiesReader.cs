using System;
using System.IO;
using Kajabity.Tools.Java;

public class PropertiesReader
{
	private static readonly char dirSepChar = Path.DirectorySeparatorChar;
	private static string propertiesLocation = ".." + dirSepChar + "config" + dirSepChar;
	private static string projectPropertiesLocation = propertiesLocation + "project.properties";
	private static string userCredentialsPropertiesLocation = propertiesLocation + "user_credentials.properties";
	private static JavaProperties properties;

	private static string userKey = "userid";
	private static string passwordKey = "password";

	private static JavaProperties GetProperties(string location) {
		if (properties == null) {
			properties = new JavaProperties ();
			properties.Load (new FileStream(location, FileMode.Open));
		}
		return properties;
	}

	private static JavaProperties GetUserCredentialsProperties() {
		return GetProperties (userCredentialsPropertiesLocation);
	}

	public static string GetProperty(JavaProperties properties, string key) {
		return properties.GetProperty (key);
	}

	public static string GetUserId() {
		return GetProperty (GetUserCredentialsProperties(), userKey);
	}

	public static string GetPassword() {
		return GetProperty (GetUserCredentialsProperties(), passwordKey);
	}
}