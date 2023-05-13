using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml;

public static class Serializer
{
    public static void WriteObject<T>(string fileName, T objectToWrite)
    {
        // Create a new instance of the Person class and
        // serialize it to an XML file.
        // Create a new instance of a StreamWriter
        // to read and write the data.
        FileStream fs = new FileStream("../../Assets/Levels/" + fileName,
        FileMode.Create);
        XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(fs);
        DataContractSerializer ser =
            new DataContractSerializer(typeof(T));
        ser.WriteObject(writer, objectToWrite);
        Console.WriteLine("Finished writing object.");
        writer.Close();
        fs.Close();
    }
    public static T ReadObject<T>(string fileName)
    {
        // Deserialize an instance of the Person class
        // from an XML file. First create an instance of the
        // XmlDictionaryReader.
        FileStream fs = new FileStream("../../Assets/Levels/" + fileName, FileMode.OpenOrCreate);
        XmlDictionaryReader reader =
            XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());

        // Create the DataContractSerializer instance.
        DataContractSerializer ser =
            new DataContractSerializer(typeof(T));

        // Deserialize the data and read it from the instance.
        T obj = (T)ser.ReadObject(reader);
        fs.Close();
        return obj;
    }
}


