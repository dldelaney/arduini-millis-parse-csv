using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AirSensorMillis2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //read from file
            //break into array on comma
            //find millis
            //if larger than the value before. skip
            //if not larger than (or not a number) than add current milis to old value, plus 10 sec (check arduino reset time)

            //-----------------------------------------------------

            //read from file
            openFileDialog1.ShowDialog();
            string filepath = openFileDialog1.FileName;
            string text = System.IO.File.ReadAllText(filepath);
            string[] textLines = text.Split('\n');
            List<string> lines = new List<string>();
            List<string> failedParseLines = new List<string>();

            for (int i = 0; i < textLines.Length; i++) {
                //for each line
                string temp1 = textLines[i]; //get the string
                string[] temp2 = temp1.Split(','); //split on comma
                //take the first position and try to cast to int
                int testOut = -1;

                //if test fails, throw out the line, otherwise, put it in the list
                if (Int32.TryParse(temp2[0], out testOut)) {
                    //if it doesnt fail, add the string to the list
                    lines.Add(temp1);
                }
                else
                {
                    Console.Out.Write("Line parse failed: " + temp1); //all lines have a \n at the back, so a new line is generated from that
                    failedParseLines.Add(temp1);
                }
            }
            //now lines is a list of all valid strings

            //break each line on commas
            //if the differnce between the new millis and old millis is greater than 0, add the difference to the running count
            //if the difference is less than 0, add 10 sec (10000 ms) as well as the new millis, and continue to next millis (still update old millis)

            for (int i = 0; i < 10; i++){
                Console.WriteLine(lines[i]);
            }
            


            int oldMillis = Int32.Parse(lines[0].Split(',')[0]);
            int newMillis = -1;
            int runningMillis = 0;
            List<int> runningInt = new List<int>();
            runningInt.Add(oldMillis);
            for(int i = 1; i < lines.Count; i++){
                newMillis = Int32.Parse(lines[i].Split(',')[0]);
                int difference = newMillis - oldMillis;

                if(difference > 0){
                    runningMillis += difference;
                }
                else{ // if time somehow goes backwards (reset occurs)
                    runningMillis += 10000; //add 10 sec (reset timeout timer) and continue
                }

                runningInt.Add(runningMillis);
                oldMillis = newMillis;

                Console.Out.WriteLine(runningMillis);
            }


            //write to new text file
            using (StreamWriter writer = new StreamWriter(Path.GetDirectoryName(filepath) + "//FINAL_OUTPUT.TXT")){
                //take the failed parse lines from before and stick them at the top first
                for(int i = 0; i < failedParseLines.Count; i++){
                    writer.Write(failedParseLines[i]);
                }

                for(int i = 0; i < lines.Count; i++){
                    writer.Write(runningInt[i]);
                    for(int j = 1; j < lines[i].Split(',').Length; j++){
                        writer.Write(",");
                        writer.Write(lines[i].Split(',')[j]);
                    }
                    
                }

            }



            this.Close();
        }


    }
}
