using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Configuration;
using Akka.Configuration.Hocon;
using Xunit;
using Xunit.Abstractions;

namespace Configuration.Hocon.Tests
{
    public class UnitTest1
    {
        public ITestOutputHelper Output { get; }

        public UnitTest1(ITestOutputHelper output)
        {
            Output = output;
        }

        [Fact]
        public void Test1()
        {
            var hocon = @"a {
  b {
      foo = hello
      bar = 123
  }
  c {
     d = xyz
     e = ${a.b}
  }
}";

            HoconRoot Include(string s) => Parser.Parse(s, Include);

            var root = Include(hocon);
            VisitHoconValue("", root.Value);
        }

        private void VisitHoconValue(string path, HoconValue value)
        {
            if (value.IsEmpty)
                return;

            if (value.IsObject())
            {
                VisitHoconObject(path, value.GetObject());
            }
            else if (value.IsArray())
            {
                VisitHoconArray(path, value.GetArray());
            }
            else if (value.IsString())
            {
                Output.WriteLine($"{path} = {value.GetString()}");
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