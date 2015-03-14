using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace LoggerLib
{
    public class Logger
    {
        String directory;
        String fileName;

        /// <summary>
        /// Creates a logger which writes to the location specified 
        /// by the file name and directory defined in the App.config
        /// keys "LoggerFileDirectory" and "LoggerFileName"
        /// </summary>
        public Logger()
        {
           directory = System.Configuration.ConfigurationManager.AppSettings["LoggerFileDirectory"];
           fileName = FormatDateString(DateTime.Now.ToString()) + System.Configuration.ConfigurationManager.AppSettings["LoggerFileName"];
        }

        /// <summary>
        /// Attempts to write a single line to a text file.
        /// </summary>
        /// <param name="line"></param>
        public void WriteToFile(String line)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@directory + fileName, true))
                {
                    file.WriteLine(line);
                }
            }
            catch (Exception ex)
            {
                ///TODO Ask MC about what to do with internal exceptions like this.
            }
       
        }

        /// <summary>
        /// Removes invalid characters from the string 
        /// representation of datetime to be used as a 
        /// filename.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string FormatDateString(String fileName)
        {
            fileName = fileName.Replace("/", "_");
            fileName = fileName.Replace(" ", "_");
            fileName = fileName.Replace(":", "_");
            return fileName;
        }
    
    }
}
