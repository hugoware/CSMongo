using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace CSMongo {

    //extra helpers for programming
    internal static class Helpers {

        public static void ExportBytes(byte[] bytes, string path) {
            StringBuilder output = new StringBuilder();
            foreach (byte item in bytes) {
                //output.AppendLine(Convert.ToInt32(item).ToString());
                output.AppendLine(char.ConvertFromUtf32(Convert.ToInt32(item)));
            }
            File.WriteAllText(path, output.ToString());
        }

        public static string MakeRegexSafe(object value) {
            string text = (value ?? string.Empty).ToString();
            return Regex.Escape(text);
        }

    }
}
