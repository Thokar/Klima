using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KlimaRechner
{
  using System.Globalization;
  using System.IO;
  using System.Windows.Forms.VisualStyles;

  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      var path = "produkt_klima_Tageswerte_19360101_20151231_02014.txt";
      var seperator = ';';
      var path1 = "produkt_klima_Tageswerte_20151126_20170528_02014.txt";

      var dt = GetDataTable(path, seperator);

      var dt1 = GetDataTable(path1, seperator);


      foreach (DataRow r in dt1.Rows)
      {
        dt.ImportRow(r);
      }

      var colNameYear = "CYEAR";
      var colNameTemp = "CTEMP";
      var provider = new CultureInfo("en-GB");

      dt.Columns.Add(colNameYear, typeof(int));
      dt.Columns.Add(colNameTemp, typeof(decimal));

      Dictionary<string, string> result = new Dictionary<string, string>();

      foreach (DataRow row in dt.Rows)
      {
        if (row["MESS_DATUM"] != null && row["MESS_DATUM"] != DBNull.Value)
        {
          string date = (string)row["MESS_DATUM"];
          string temp = (string)row["LUFTTEMPERATUR_MAXIMUM"];

          row[colNameYear] = int.Parse(date.Substring(0, 4));
          row[colNameTemp] = decimal.Parse(temp.Trim(), provider) +  int.Parse(textBoxDeltaTemp.Text);
        }
      }

      for (int i = int.Parse(textBoxYear.Text); i <= 2017; i++)
      {

        for (int y = int.Parse(textBoxMinTemp.Text); y <= 36; y++)
        //  for (int y = 10; y <= 36; y++)
        {
          var i1 = i;
          var result1 = from resultRow in dt.AsEnumerable()
                        where resultRow[colNameTemp] != DBNull.Value
                        && resultRow.Field<decimal>(colNameTemp) > decimal.Parse(y.ToString(), provider)
                        && resultRow.Field<int>(colNameYear) == i1
                        select resultRow;

          this.listBox1.Items.Add("JAHR " + i + ": " + result1.Count() + " Tage über " + y + " Grad");
        }
      }


    }
    private DataTable GetDataTable(string path, char seperator)
    {
      DataTable dt = new DataTable();
      FileStream aFile = new FileStream(path, FileMode.Open);
      using (StreamReader sr = new StreamReader(aFile, System.Text.Encoding.Default))
      {
        string strLine = sr.ReadLine();
        string[] strArray = strLine.Split(seperator);

        foreach (string value in strArray)
          dt.Columns.Add(value.Trim());

        DataRow dr = dt.NewRow();

        while (sr.Peek() > -1)
        {
          strLine = sr.ReadLine();
          strArray = strLine.Split(seperator);
          dt.Rows.Add(strArray);
        }
      }
      return dt;
    }

    private void label6_Click(object sender, EventArgs e)
    {

    }


  }
}
