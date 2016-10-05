using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


namespace SeijaTelegram.Utils
{
    public class Record
    {
        public Record()
        {

        }
        public Record(string K, string V)
        {
            k = K;
            v = V;
        }
        public string k;
        public string v;
    }
    class XmlDictionary
    {
        public XmlDictionary(string Path)
        {
            path = Path;
            Read();
        }
        public void AddPair(string Key, string Value)
        {
            pairs.Add(Key,Value);
            Write();
        }
        public void Write()
        {
            List<Record> temp = new List<Record>();
            List<string> keys = new List<string>(pairs.Keys.ToArray());
            foreach (string key in keys)
            {
                temp.Add(new Record( key, pairs[key]));
            }

            var serializer = new XmlSerializer(typeof(List<Record>));
            using (var sw = new StreamWriter(new FileStream(path, FileMode.OpenOrCreate)))
            {
                serializer.Serialize(sw, temp);
            }            
        }
        public void Read()
        {
            List<Record> temp = new List<Record>();
            using (var stream = new StreamReader(new FileStream(path, FileMode.OpenOrCreate)))
            {
                if (stream.BaseStream.Length != 0)
                {
                    var ser = new XmlSerializer(typeof(List<Record>));
                    temp = (List<Record>)ser.Deserialize(stream);
                    pairs = new Dictionary<string, string>();
                    foreach (Record rec in temp)
                    {
                        pairs.Add(rec.k, rec.v);
                    }
                }
                else
                {
                    pairs = new Dictionary<string, string>();
                }
            }
        }
        string path;
        Dictionary<string, string> pairs;
        public Dictionary<string, string> Pairs
        {
            get { return pairs; }
            set
            {
                pairs = value;
                Write();
            }
        }
    }
}
