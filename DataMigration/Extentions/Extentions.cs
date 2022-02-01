using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DataMigration
{
    public static class Extentions
    {
        public static List<List<T>> SplitIntoChunks<T>(this List<T> list, int chunkSize)
        {
            List<List<T>> retVal = new List<List<T>>();
            if (chunkSize > 0)
            {
                int index = 0;
                while (index < list.Count)
                {
                    int count = list.Count - index > chunkSize ? chunkSize : list.Count - index;
                    retVal.Add(list.GetRange(index, count));

                    index += chunkSize;
                }
            }
            else
            {
                retVal.Add(list);
            }
            return retVal;
        }

        public static string GetXml(this IPolicyQuoteTransaction transaction)
        {
            using (var stringwriter = new StringWriter())
            {
                var serializer = new XmlSerializer(transaction.GetType());
                //serializer.Serialize(stringwriter, transaction);
                //return stringwriter.ToString();
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                serializer.Serialize(stringwriter, transaction, ns);
                var xElement = XElement.Parse(stringwriter.ToString());
                xElement.Descendants().Where(x => String.IsNullOrEmpty(x.Value) && x.Attributes().Where(y => y.Name.LocalName == "nil" && y.Value == "true").Count() > 0).Remove();
                xElement.Descendants().Where(x => x.IsEmpty && !x.HasAttributes).Remove();
                return xElement.ToString(SaveOptions.DisableFormatting);
            }
        }
    }
}
