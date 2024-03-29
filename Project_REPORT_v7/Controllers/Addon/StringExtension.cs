﻿using System.Linq;

namespace Project_REPORT_v7.Controllers.Addon
{
    /// <summary>
    /// String extension custom static class.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Converts all letters to Uppercase.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToUpperCaps( this string str )
        {
            return str.ToUpper();
        }

        /// <summary>
        /// Convert every first letter of every sentence to uppercase.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToAutoCapitalize( this string str )
        {
            var temp = str.Split('.');
            var last = temp.Last();
            string result = "";
            if ( temp.Length > 1 )
            {
                foreach ( var item in temp )
                {
                    if ( item.Length > 1 && !item.StartsWith( " " ) )
                    {
                        result += item.Substring( 0, 1 ).ToUpper() + item.Substring( 1 )+".";
                    }
                    else if ( item.Length > 1 )
                    {
                        result += item.Substring( 0, 2 ).ToUpper() + item.Substring( 2 )+".";
                    }
                }
                result = result.TrimEnd( ' ' );
            }
            else
            {
                result = str.Substring( 0, 1 ).ToUpper() + str.Substring( 1 ) + ".";
            }
            return result;
        }

        /// <summary>
        /// Convert every first letter of every word to uppercase.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToCapitalize( this string str )
        {
            var temp = str.Split(' ');
            var last = temp.Last();
            string result = "";

            if ( temp.Length > 1 )
            {
                foreach ( var item in temp )
                {
                    if ( item.Length > 1 )
                    {
                        if ( item.StartsWith( " " ) )
                        {
                            result += item.Substring( 0, 2 ).ToUpper() + item.Substring( 2 ) + " ";
                        }
                        else
                        {
                            result += item.Substring( 0, 1 ).ToUpper() + item.Substring( 1 ) + " ";
                        }

                        if (item.Equals(last))
                        {
                            result = result.TrimEnd(' ');
                        }
                    }
                    else
                    {
                        result += item + " ";
                    }
                }
            }
            else
            {
                result = temp [0].Substring(0, 1).ToUpper() + temp [0].Substring(1);
            }
            return result;
        }
    }
}