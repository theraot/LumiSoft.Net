﻿using System;
using System.Text;

namespace LumiSoft.Net.ABNF
{
    /// <summary>
    /// This class represent ABNF "repetition". Defined in RFC 5234 4.
    /// </summary>
    public class ABNF_Repetition
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="min">Minimum repetitions.</param>
        /// <param name="max">Maximum repetitions.</param>
        /// <param name="element">Repeated element.</param>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>element</b> is null reference.</exception>
        public ABNF_Repetition(int min, int max, ABNF_Element element)
        {
            if (min < 0)
            {
                throw new ArgumentException("Argument 'min' value must be >= 0.");
            }
            if (max < 0)
            {
                throw new ArgumentException("Argument 'max' value must be >= 0.");
            }
            if (min > max)
            {
                throw new ArgumentException("Argument 'min' value must be <= argument 'max' value.");
            }

            Min = min;
            Max = max;
            Element = element ?? throw new ArgumentNullException("element");
        }

        /// <summary>
        /// Gets repeated element.
        /// </summary>
        public ABNF_Element Element { get; }

        /// <summary>
        /// Gets maximum repetitions.
        /// </summary>
        public int Max { get; } = int.MaxValue;

        /// <summary>
        /// Gets minimum repetitions.
        /// </summary>
        public int Min { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ABNF_Repetition Parse(System.IO.StringReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            /*
                repetition     =  [repeat] element
                repeat         =  1*DIGIT / (*DIGIT "*" *DIGIT)
                element        =  rulename / group / option / char-val / num-val / prose-val
            */

            int min = 0;
            int max = int.MaxValue;

            // --- range ------------------------------------
            if (char.IsDigit((char)reader.Peek()))
            {
                var minString = new StringBuilder();
                while (char.IsDigit((char)reader.Peek()))
                {
                    minString.Append((char)reader.Read());
                }
                min = Convert.ToInt32(minString.ToString());
            }
            if (reader.Peek() == '*')
            {
                reader.Read();
            }
            if (char.IsDigit((char)reader.Peek()))
            {
                var maxString = new StringBuilder();
                while (char.IsDigit((char)reader.Peek()))
                {
                    maxString.Append((char)reader.Read());
                }
                max = Convert.ToInt32(maxString.ToString());
            }
            //-----------------------------------------------

            // End of stream reached.
            if (reader.Peek() == -1)
            {
                return null;
            }
            // We have rulename.

            if (char.IsLetter((char)reader.Peek()))
            {
                return new ABNF_Repetition(min, max, ABNF_RuleName.Parse(reader));
            }
            // We have group.
            if (reader.Peek() == '(')
            {
                return new ABNF_Repetition(min, max, ABFN_Group.Parse(reader));
            }
            // We have option.
            if (reader.Peek() == '[')
            {
                return new ABNF_Repetition(min, max, ABNF_Option.Parse(reader));
            }
            // We have char-val.
            if (reader.Peek() == '\"')
            {
                return new ABNF_Repetition(min, max, ABNF_CharVal.Parse(reader));
            }
            // We have num-val.
            if (reader.Peek() == '%')
            {
                // Eat '%'.
                reader.Read();

                if (reader.Peek() == 'd')
                {
                    return new ABNF_Repetition(min, max, ABNF_DecVal.Parse(reader));
                }

                throw new ParseException("Invalid 'num-val' value '" + reader.ReadToEnd() + "'.");
            }
            // We have prose-val.
            if (reader.Peek() == '<')
            {
                return new ABNF_Repetition(min, max, ABNF_ProseVal.Parse(reader));
            }

            return null;
        }
    }
}
