﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SeijaTelegram.Utils
{
    class XmlStringsArray
    {
        public XmlStringsArray(string Path)
        {
            path = Path;
            Read();
        }
        List<string> strings;
        public List<string> Strings
        {
            get { return strings; }
            set
            {
                strings = value;
                Write();
            }
        }
        public void AddString(string str)
        {
            strings.Add(str);
            Write();
        }
        string path;
        public void Write()
        {
            var serializer = new XmlSerializer(typeof(List<string>));
            using (var sw = new StreamWriter(new FileStream(path, FileMode.OpenOrCreate)))
            {
                serializer.Serialize(sw, strings);
            }
        }
        public void Read()
        {
            using (var stream = new StreamReader(new FileStream(path, FileMode.OpenOrCreate)))
            {

                if (stream.BaseStream.Length != 0)
                {
                    var ser = new XmlSerializer(typeof(List<string>));
                    strings = (List<string>)ser.Deserialize(stream);
                }
                else
                {
                    strings = new List<string>();
                }
            }

        }
        public string GetRandomString()
        {
            if (strings.Count < 1)
                return "Массив строк пуст (✿◕⁀◕)";
            var rng = new Random();
            return strings[rng.Next(0, strings.Count - 1)];
        }
    }
}
