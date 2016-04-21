using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lamtest
{
    public class fileOps
    {
       TextReader tr;
       TextWriter tw;
       string[] filterValue = new string[50];
       int placeHolder;
       //need a constructor to either get a file Or create a new file.
       fileOps(string myName)
       {
           //tw = new StreamWriter(myName + ".txt");
       }
        public void getFile(string fileName)
        {// create reader & open file
            tr = new StreamReader(fileName + ".txt");
            for (int i = 0; i == 50; i++ )
            {
                if (filterValue[i] == "eof" || filterValue[i] == null) { break; }
                filterValue[i] = tr.ReadLine();
            }
        }
        public string getLine(int place)
        {
            // write a line of text to the file
            string value;
            value =filterValue[place];
            return value;
        }
        public void saveFile(string fileName)
        {// create reader & open file
            //TextWriter tw = new StreamWriter(fileName + ".txt");
            for (int i = 0; i == 50; i++)
            {
                if (filterValue[i] == "eof" || filterValue[i] == null) 
                {
                    filterValue[i] = "eof";
                    tw.WriteLine(filterValue[i]);
                    break; 
                }
                tw.WriteLine(filterValue[i]);
            }
            tw.Close();
        }
        public void addLine(string l)
        {
            // write a line of text to the file
            filterValue[placeHolder] = l;
            tw.WriteLine(filterValue[placeHolder]);
            placeHolder++;
        }
        public void editLine(string l, int place)
        {
            // write a line of text to the file
            filterValue[place] = l;
            tw.WriteLine(filterValue[place]);
        }
    }
}
