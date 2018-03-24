using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Akka.Configuration;
using Akka.Configuration.Hocon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Configuration.Hocon
{
    public class HoconConfigurationProvider : FileConfigurationProvider
    {
        public HoconConfigurationProvider(HoconConfigurationSource source) : base(source)
        {
            this.Data = new Dictionary<string, string>();
        }

        public override void Load(Stream stream)
        {
            var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();

            HoconRoot Include(string s) => Parser.Parse(s, Include);

            var root = Include(text);
            VisitHoconValue("", root.Value);
        }

        private void VisitHoconValue(string path, HoconValue value)
        {
            if (value.IsEmpty)
                return;

            if (value.IsString())
            {
                Data.Add(path, value.GetString());
            }
            else if (value.IsObject())
            {
                VisitHoconObject(path, value.GetObject());
            }
            else if (value.IsArray())
            {
                VisitHoconArray(path, value.GetArray());
            }
        }

        private void VisitHoconArray(string path, IList<HoconValue> array)
        {
            if (array == null)
                return;

            for (int i = 0; i < array.Count; i++)
            {
                VisitHoconValue(PathCombine(path, i.ToString()), array[i]);
            }
        }

        private void VisitHoconObject(string path, HoconObject o)
        {
            if (o == null)
                return;

            foreach (var value in o.Items)
                VisitHoconValue(PathCombine(path, value.Key), value.Value);
        }

        private string PathCombine(string left, string right)
        {
            if (string.IsNullOrEmpty(left))
                return right;

            if (string.IsNullOrEmpty(right))
                return left;

            return $"{left}:{right}";
        }
    }
}