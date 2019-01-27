using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;

namespace WebBlazor.Models
{
    public class ResourceService
    {
        private readonly string resourceReader;

        public static Tuple<string, string>[] Languages = new[]
        {
            new Tuple<string, string>("english", "../Resources/Resources.en-US.resx"),
            new Tuple<string, string>("русский", "../Resources/Resources.ru-RU.resx")
        };

        public ResourceService(string language)
        {
            resourceReader = Languages.SingleOrDefault(a => a.Item1 == language).Item2;
        }

        public string Read(string keyWord)
        {
            using (ResourceReader reader = new ResourceReader(resourceReader))
            {
                return reader.Cast<DictionaryEntry>().SingleOrDefault(a => (a.Key as string) == keyWord).Value as string;
            }
        }
        public static List<string> GetLanguages() => Languages.Select(a => a.Item1).ToList();
    }
}
