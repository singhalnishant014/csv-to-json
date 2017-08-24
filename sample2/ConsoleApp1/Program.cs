using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace example1
{
    public class part1
    {
        public string year { get; set; }
        public string IndicatorName { get; set; }
        public string Countrycode { get; set; }
        public string countryname { get; set; }
        public double value { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            StreamReader streamReader = new StreamReader(new FileStream(@"C:\Users\Training\Desktop\CSV\Indicators.csv", FileMode.OpenOrCreate));
            StreamWriter sw = new StreamWriter(new FileStream(@"C:\Users\Training\Desktop\CSV\line.json", FileMode.OpenOrCreate));
            StreamWriter sw2 = new StreamWriter(new FileStream(@"C:\Users\Training\Desktop\CSV\area.json", FileMode.OpenOrCreate));
            StreamWriter sw3 = new StreamWriter(new FileStream(@"C:\Users\Training\Desktop\CSV\stack.json", FileMode.OpenOrCreate));
            string line;
            string[] countrycode = { "AFG", "ARM", "AZE", "BHR", "BGD", "BTN", "BRN", "KHM", "CHN", "CXR", "CCK", "IOT", "GEO", "HKG", "IND", "IDN", "IRN", "IRQ", "ISR", "JPN", "JOR", "KAZ", "KWT", "KGZ", "LAO", "LBN", "MAC", "MYS", "MDV", "MNG", "MMR", "NPL", "PRK", "OMN", "PAK", "PHL", "QAT", "SAU", "SGP", "KOR", "LKA", "SYR", "TWN", "TJK", "THA", "TUR", "TKM", "ARE", "UZB", "VNM", "YEM" };
            List<part1> jsonRow = new List<part1>();
            List<part1> piechart = new List<part1>();
            List<part1> barchart = new List<part1>();
            List<part1> sub = new List<part1>();
            string[] headers = streamReader.ReadLine().Split(',');
            while ((line = streamReader.ReadLine()) != null)
            {
                string[] values = line.Split('"');
                if (values.Length > 1)
                    values[1] = values[1].Replace(",", "*");
                line = "";
                foreach (var a in values)
                    line += a;
                string[] val = line.Split(',');
                for (int m = 0; m < val.Length; m++)
                    val[m] = val[m].Replace("*", ",");
                if ((val[2] == "Urban population (% of total)" || val[2] == "Rural population (% of total population)") && val[1] == "IND")
                {
                    double.TryParse(val[5], out double temp2);
                    jsonRow.Add(new part1() { year = val[4], IndicatorName = val[2], value = temp2 });
                }
                if ((val[2] == "Urban population growth (annual %)") && val[1] == "IND")
                {
                    double.TryParse(val[5], out double temp2);
                    piechart.Add(new part1() { year = val[4], IndicatorName = val[2], value = temp2 });
                }
                for (int i = 0; i < countrycode.Length; i++)
                {
                    if ((countrycode[i] == val[1]) && (val[2] == "Urban population" || val[2] == "Rural population"))
                    {
                        val[2] = val[2].Replace(" ", "_");
                        double.TryParse(val[5], out double temp2);
                        barchart.Add(new part1() { year = val[4], IndicatorName = val[2], value = temp2, Countrycode = val[1], countryname = val[0] });
                    }
                }
            }
            var query = from part in jsonRow group new { part.IndicatorName, part.value } by part.year into yeargroup select yeargroup;
            sw.WriteLine("{\"INDIA\" : [");
            foreach (var a in query)
            {
                sw.WriteLine("\n{\n" + "\"year\" : " + a.Key + ",");
                string temp = "";
                foreach (var item in a)
                    temp = temp + "\t\"" + item.IndicatorName + "\" : \"" + item.value + "\",";
                temp = temp.Remove(temp.Length - 1);
                sw.Write(temp + "\n" + (a.Key == "2014" ? "}" : "},"));
            }
            sw.Write("]}");
            sw.Flush();
            sw2.WriteLine("[");
            foreach (var a in piechart)
                sw2.WriteLine("{\n\"year\" : " + a.year + ",\n\"" + a.IndicatorName + "\" :  " + a.value + "" + "\n" + (a.year == "2014" ? "}" : "},"));
            sw2.Write("]");
            sw2.Flush();
            var query1 = from part1 in barchart group new { part1.IndicatorName, part1.value, part1.countryname } by part1.Countrycode into countrygroup select countrygroup;
            foreach (var a in query1)
            {
                var query2 =from part1 in a group new { part1.value, part1.countryname } by part1.IndicatorName into ingroup select ingroup;
                foreach (var c in query2)
                {
                    double sum = 0;
                    string name = "";
                    foreach (var d in c)
                    {
                        sum = sum + d.value;
                        name = d.countryname;
                    }
                    sub.Add(new part1() { Countrycode = a.Key, value = sum, countryname = name, IndicatorName = c.Key });
                }
            }
            sw3.WriteLine("[");
            var subquery =from part1 in sub group new { part1.IndicatorName, part1.value, part1.Countrycode } by part1.countryname into countrygroup select countrygroup;
            foreach (var a in subquery)
            {
                sw3.WriteLine("{\t\"Country\" : \"" + a.Key + "\",");
                string temp = "";
                foreach (var b in a)
                    temp = temp + "\t\"" + b.IndicatorName + "\" : \"" + b.value + "\",";
                temp = temp.Remove(temp.Length - 1);
                sw3.WriteLine(temp+"\n"+(a.Key == "Yemen, Rep."?"}":"},"));
            }
            sw3.Write("]");
            sw3.Flush();
        }
    }
}