using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;

namespace JsonCompare
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("JsonCompare v0.1 by Deblokt");
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: To compare two JSON files \"A\" and \"B\"");
                Console.WriteLine("Command: \"JsonCompare.exe A.json B.json\"");
            }
            else
            {
                JObject obj1;
                JObject obj2;

                using (StreamReader r1 = new StreamReader(args[0]))
                {
                    var sb = new StringBuilder(r1.ReadToEnd());
                    if (sb[0] == '[')
                    {
                        sb.Insert(0, "{'arr':");
                        sb.Append("}");
                    }
                    obj1 = JsonConvert.DeserializeObject<JObject>(sb.ToString());
                };
                using (StreamReader r2 = new StreamReader(args[1]))
                {
                    var sb = new StringBuilder(r2.ReadToEnd());
                    if (sb[0] == '[')
                    {
                        sb.Insert(0, "{'arr':");
                        sb.Append("}");
                    }
                    obj2 = JsonConvert.DeserializeObject<JObject>(sb.ToString());
                };

                bool isSame = JObject.DeepEquals(obj1, obj2);
                Console.WriteLine(string.Format("IsSame: {0}", isSame));
                if (!isSame)
                {
                    var diffBuilder = new InlineDiffBuilder(new Differ());
                    var diff = diffBuilder.BuildDiffModel(obj1.ToString(), obj2.ToString());

                    foreach (var line in diff.Lines)
                    {
                        switch (line.Type)
                        {
                            case ChangeType.Inserted:
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write("+ ");
                                Console.WriteLine(line.Text);
                                break;
                            case ChangeType.Deleted:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("- ");
                                Console.WriteLine(line.Text);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press ENTER to exit..");
            Console.ReadLine();
        }
    }
}
