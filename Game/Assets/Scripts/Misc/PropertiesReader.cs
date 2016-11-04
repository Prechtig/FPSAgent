using System;
using System.IO;
using System.Collections.Generic;
using Kajabity.Tools.Java;

public class PropertiesReader
{
	private static readonly char dirSepChar = Path.DirectorySeparatorChar;
	private static readonly string propertyFileFolder = ".." + dirSepChar + "config" + dirSepChar;

	private static readonly string projectPropertiesLocation			= propertyFileFolder + "project.properties";
	private static readonly string userCredentialsPropertiesLocation	= propertyFileFolder + "user_credentials.properties";
    private static readonly string neatPropertiesLocation               = propertyFileFolder + "neat.properties";


    private static readonly Dictionary<PropertyFile, JavaProperties> loadedProperties = new Dictionary<PropertyFile, JavaProperties>();

	private static JavaProperties LoadPropertyFile(string location) {
		JavaProperties properties = new JavaProperties ();
		properties.Load (new FileStream (location, FileMode.Open));
		return properties;
	}

	private static JavaProperties LoadPropertyFile(PropertyFile pf) {
		switch (pf) {
		case PropertyFile.UserCredentials:
			return LoadPropertyFile (userCredentialsPropertiesLocation);
		case PropertyFile.Project:
			return LoadPropertyFile (projectPropertiesLocation);
        case PropertyFile.NEAT:
            return LoadPropertyFile (neatPropertiesLocation);
		default:
			throw new InvalidOperationException ();
		}
	}

	public static JavaProperties GetPropertyFile(PropertyFile pf) {
		JavaProperties result;
		bool hasValue = loadedProperties.TryGetValue (pf, out result);
		if (!hasValue) {
			result = LoadPropertyFile (pf);
			loadedProperties.Add (pf, result);
		}
		return result;
	}

    public static string GetPropertyFilePath(PropertyFile pf)
    {
        switch (pf)
        {
            case PropertyFile.UserCredentials:
                return userCredentialsPropertiesLocation;
            case PropertyFile.Project:
                return projectPropertiesLocation;
            case PropertyFile.NEAT:
                return neatPropertiesLocation;
            default:
                throw new InvalidOperationException();
        }
    }
}

public enum PropertyFile {
	UserCredentials,
	Project,
    NEAT,
}
