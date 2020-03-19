using System;
using System.Collections.Generic;
using System.Text;

namespace PlacePopularity.Extensions
{
    public static class StringExtensions
    {
        public static string ToUtf8(this string self)
        {
            var sb = new StringBuilder();
            int position = 0;
            var bytes = new List<byte>();
            while (position < self.Length)
            {
                char c = self[position++];
                if (c == '\\')
                {
                    if (position < self.Length)
                    {
                        c = self[position++];
                        if (c == 'x' && position <= self.Length - 2)
                        {
                            var b = Convert.ToByte(self.Substring(position, 2), 16);
                            position += 2;
                            bytes.Add(b);
                        }
                        else
                        {
                            AppendBytes(sb, bytes);
                            sb.Append('\\');
                            sb.Append(c);
                        }
                        continue;
                    }
                }
                AppendBytes(sb, bytes);
                sb.Append(c);
            }
            AppendBytes(sb, bytes);
            return sb.ToString();
        }

        private static void AppendBytes(StringBuilder sb, List<byte> bytes)
        {
            if (bytes.Count != 0)
            {
                var str = System.Text.Encoding.UTF8.GetString(bytes.ToArray());
                sb.Append(str);
                bytes.Clear();
            }
        }
    }
}
