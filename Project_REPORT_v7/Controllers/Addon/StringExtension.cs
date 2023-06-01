using System.Linq;

namespace Project_REPORT_v7.Controllers.Addon
{
    public static class StringExtension
    {
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
            string result = "";
            if ( temp.Length > 1 )
            {
                foreach ( var item in temp )
                {
                    if ( item.Length > 1 && !item.StartsWith( " " ) )
                    {
                        result += item.Substring( 0, 1 ).ToUpper() + item.Substring( 1 ).ToLower() + ". ";
                    }
                    else if ( item.Length > 1 )
                    {
                        result += item.Substring( 0, 2 ).ToUpper() + item.Substring( 2 ).ToLower() + ". ";
                    }
                }
            }
            else
            {
                result = str.Substring( 0, 1 ).ToUpper() + str.Substring( 1 ).ToLower() + ".";
            }
            return result;
        }

        /// <summary>
        /// Convert every first letter of every word to uppercase.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToCapitalize( this string str, bool ending = true )
        {
            var last = str.Split(' ').Last();
            var temp = str.Split(' ');
            string result = "";

            if ( temp.Length > 1 )
            {
                foreach ( var item in temp )
                {
                    if ( !ending )
                    {
                        if ( item.StartsWith( " " ) && !item.Equals( last ) )
                        {
                            result += item.Substring( 0, 2 ).ToUpper() + item.Substring( 2 ).ToLower() + ". ";
                        }
                        else if ( item.StartsWith( " " ) && item.Equals( last ) )
                        {
                            result += item.Substring( 0, 2 ).ToUpper() + item.Substring( 2 ).ToLower();
                        }
                        else if ( !item.StartsWith( " " ) && item.Equals( last ) )
                        {
                            result += item.Substring( 0, 1 ).ToUpper() + item.Substring( 1 ).ToLower();
                        }
                        else
                        {
                            result += item.Substring( 0, 1 ).ToUpper() + item.Substring( 1 ).ToLower() + ".";
                        }
                    }
                    else
                    {
                        if ( item.StartsWith( " " ) )
                        {
                            result += item.Substring( 0, 2 ).ToUpper() + item.Substring( 2 ).ToLower() + ". ";
                        }
                        else
                        {
                            result += item.Substring( 0, 1 ).ToUpper() + item.Substring( 1 ).ToLower() + ".";
                        }
                    }
                }
            }
            else
            {
                result = temp.Length > 0 ? temp [0].Substring( 0, 1 ).ToUpper() + temp [0].Substring( 1 ).ToLower() : "";
            }
            return result;
        }
    }
}