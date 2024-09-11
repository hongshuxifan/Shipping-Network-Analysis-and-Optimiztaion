using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
namespace _2018_2021_LNG_Container_AIS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        double radian(double d)
        {
            return d * 3.1415926 / 180.0;   //角度∠ = π / 180  
        }
        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }
        //计算经纬度距离(单位为M)
        public double get_distance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = radian(lat1);
            double radLat2 = radian(lat2);

            double a = radLat1 - radLat2;
            double b = radian(lng1) - radian(lng2);

            double dst = 2 * Math.Asin((Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) + Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2))));
            dst = dst * 6378137;
            //dst= round(dst * 10000) / 10000;  
            return dst;
        }
        //定义函数统计一个List含有某一元素的个数
        public int get_str_count(List<string> Lis, string va)
        {
            int i = 0;
            foreach (string a in Lis)
            {
                if (a.Contains(va))
                {
                    i++;
                }
            }
            return i;
        }
        //计算通用列表的标准差
        public double CalculateStdDev(IEnumerable<double> values)
        {
            double ret = 0;
            if (values.Count() > 0)
            {
                //Compute the Average      
                double avg = values.Average();
                //Perform the Sum of (value-avg)_2_2      
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }
        //判断字符串能否转为双精度类型
        public bool isNumberic(string message)
        {
            //判断字符串能否转为双精度类型
            System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"^[+-]?\d*[.]?\d*$");
            //判断字符串能否转为整型
            //System.Text.RegularExpressions.Regex rex = new System.Text.RegularExpressions.Regex(@"@"^[+-]?\d*$");
            double result;
            if (rex.IsMatch(message))
            {
                result = double.Parse(message);
                return true;
            }
            else
                return false;
            //double result;
            //try
            //{
            //    result = Convert.ToDouble(message);
            //    return true;
            //}
            //catch 
            //{ 
            //    return false;
            //}

        }
        //分天存储，方便解码
        private void button1_Click(object sender, EventArgs e)
        {
            //string targetPath = @"H:\2018 - 2021LNG和Container数据_Decoder\分天";
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据\export");
            string targetPath = @"H:\2018-2021LNG和Container数据\2022.06.01补齐11-12_分天\export";
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据\2022.06.01补齐11-12\export");
            DirectoryInfo[] dirFolder = theFolder.GetDirectories();
            foreach (DirectoryInfo nextFolder in dirFolder)
            {
                string destPath1 = System.IO.Path.Combine(targetPath, nextFolder.Name);
                if (Directory.Exists(destPath1) == false)
                {
                    Directory.CreateDirectory(destPath1);
                }
                FileInfo[] dirFile = nextFolder.GetFiles();
                foreach (FileInfo nextFile in dirFile)
                {
                    string[] arrtemp0 = nextFile.Name.Split('.');
                    string destPath2 = System.IO.Path.Combine(destPath1, arrtemp0[0]);
                    if (Directory.Exists(destPath2) == false)
                    {
                        Directory.CreateDirectory(destPath2);
                    }
                    StreamReader streamreader = new StreamReader(nextFile.FullName);
                    string line = "";
                    while ((line = streamreader.ReadLine()) != null)
                    {
                        string[] arrtemp1 = line.Split('~');
                        string[] arrtemp2 = arrtemp1[0].Split(' ');
                        string destPath3 = System.IO.Path.Combine(destPath2, arrtemp2[0] + ".csv");
                        FileStream fs = new FileStream(destPath3, FileMode.Append);
                        StreamWriter streamwriter = new StreamWriter(fs);
                        streamwriter.WriteLine(line);
                        streamwriter.Close();
                    }
                }
            }
            MessageBox.Show("OK");
        }
        //根据解码后的得到船舶轨迹
        private void button2_Click(object sender, EventArgs e)
        {
            //输出的字段包括MMSI、时间、航行状态、回旋速率ROT[AIS]、位置精度、经度、纬度、实际航向、真艏向
            // DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2018");
            // string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\2018_轨迹";
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2018\2018-03");
            string targetPath = @"F:\2018-2021LNG和Container处理_22_7_11\2018_船舶轨迹\2018-03";
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo theFile in dirFile)
            {
                StreamReader streamreader = new StreamReader(theFile.FullName);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split('|');
                    string[] arrtemp1 = arrtemp[0].Split('~');
                    //有些格式不太对
                    //if (arrtemp.Count() <= 20)
                    //{
                    //    continue;
                    //}
                    //else
                    //{

                    //    if (arrtemp.Count() > 28 && arrtemp[arrtemp.Count() - 18].Length >= 9)
                    //    {
                    //        string filename = arrtemp[arrtemp.Count() - 18] + ".csv";
                    //        string filename = arrtemp1[1] + ".csv";
                    //        string destPath = System.IO.Path.Combine(targetPath, filename);
                    //        FileStream fs = new FileStream(destPath, FileMode.Append);
                    //        StreamWriter streamwriter = new StreamWriter(fs);
                    //        streamwriter.WriteLine(arrtemp1[1] + '|' + arrtemp[arrtemp.Count() - 18] + '|' + arrtemp1[0] + '|' + arrtemp[arrtemp.Count() - 16] + '|' + arrtemp[arrtemp.Count() - 15] + '|' + arrtemp[arrtemp.Count() - 14] + '|' + arrtemp[arrtemp.Count() - 13] + '|' + arrtemp[arrtemp.Count() - 12] + '|' + arrtemp[arrtemp.Count() - 11] + '|' + arrtemp[arrtemp.Count() - 10] + '|' + arrtemp[arrtemp.Count() - 9]);
                    //        streamwriter.Close();
                    //    }
                    //    if (arrtemp.Count() <= 28 && arrtemp[arrtemp.Count() - 11].Length >= 9)
                    //    {
                    //        string filename = arrtemp[arrtemp.Count() - 11] + ".csv";
                    //        string filename = arrtemp1[1] + ".csv";
                    //        string destPath = System.IO.Path.Combine(targetPath, filename);
                    //        FileStream fs = new FileStream(destPath, FileMode.Append);
                    //        StreamWriter streamwriter = new StreamWriter(fs);
                    //        streamwriter.WriteLine(arrtemp1[1] + '|' + arrtemp[arrtemp.Count() - 11] + '|' + arrtemp1[0] + '|' + arrtemp[arrtemp.Count() - 9] + '|' + arrtemp[arrtemp.Count() - 8] + '|' + arrtemp[arrtemp.Count() - 7] + '|' + arrtemp[arrtemp.Count() - 6] + '|' + arrtemp[arrtemp.Count() - 5] + '|' + arrtemp[arrtemp.Count() - 4] + '|' + arrtemp[arrtemp.Count() - 3] + '|' + arrtemp[arrtemp.Count() - 2]);
                    //        streamwriter.Close();
                    //    }
                    //}

                    //if (arrtemp.Count() <= 20)
                    //{
                    //    continue;
                    //}
                    //else
                    //{
                    //    //有些格式不太对，2022 - 7 - 11月重新修改
                    //if (arrtemp.Count() > 28 && arrtemp[arrtemp.Count() - 18].Length >= 9 && arrtemp.Count() < 39)
                    //    {
                    //        //string filename = arrtemp[arrtemp.Count() - 18] + ".csv";
                    //        string filename = arrtemp1[1] + ".csv";
                    //        string destPath = System.IO.Path.Combine(targetPath, filename);
                    //        FileStream fs = new FileStream(destPath, FileMode.Append);
                    //        StreamWriter streamwriter = new StreamWriter(fs);
                    //        streamwriter.WriteLine(arrtemp1[1] + '|' + arrtemp[arrtemp.Count() - 18] + '|' + arrtemp1[0] + '|' + arrtemp[arrtemp.Count() - 16] + '|' + arrtemp[arrtemp.Count() - 15] + '|' + arrtemp[arrtemp.Count() - 14] + '|' + arrtemp[arrtemp.Count() - 13] + '|' + arrtemp[arrtemp.Count() - 12] + '|' + arrtemp[arrtemp.Count() - 11] + '|' + arrtemp[arrtemp.Count() - 10] + '|' + arrtemp[arrtemp.Count() - 9]);
                    //        streamwriter.Close();
                    //    }
                    //    if (arrtemp.Count() <= 28 && arrtemp[arrtemp.Count() - 11].Length >= 9)
                    //    {
                    //        // string filename = arrtemp[arrtemp.Count() - 11] + ".csv";
                    //        string filename = arrtemp1[1] + ".csv";
                    //        string destPath = System.IO.Path.Combine(targetPath, filename);
                    //        FileStream fs = new FileStream(destPath, FileMode.Append);
                    //        StreamWriter streamwriter = new StreamWriter(fs);
                    //        streamwriter.WriteLine(arrtemp1[1] + '|' + arrtemp[arrtemp.Count() - 11] + '|' + arrtemp1[0] + '|' + arrtemp[arrtemp.Count() - 9] + '|' + arrtemp[arrtemp.Count() - 8] + '|' + arrtemp[arrtemp.Count() - 7] + '|' + arrtemp[arrtemp.Count() - 6] + '|' + arrtemp[arrtemp.Count() - 5] + '|' + arrtemp[arrtemp.Count() - 4] + '|' + arrtemp[arrtemp.Count() - 3] + '|' + arrtemp[arrtemp.Count() - 2]);
                    //        streamwriter.Close();
                    //    }
                    //    if (arrtemp.Count() == 40)
                    //    {
                    //        string filename = arrtemp1[1] + ".csv";
                    //        string destPath = System.IO.Path.Combine(targetPath, filename);
                    //        FileStream fs = new FileStream(destPath, FileMode.Append);
                    //        StreamWriter streamwriter = new StreamWriter(fs);
                    //        streamwriter.WriteLine(arrtemp1[1] + '|' + arrtemp[arrtemp.Count() - 23] + '|' + arrtemp1[0] + '|' + arrtemp[arrtemp.Count() - 21] + '|' + arrtemp[arrtemp.Count() - 20] + '|' + arrtemp[arrtemp.Count() - 19] + '|' + arrtemp[arrtemp.Count() - 18] + '|' + arrtemp[arrtemp.Count() - 17] + '|' + arrtemp[arrtemp.Count() - 16] + '|' + arrtemp[arrtemp.Count() - 15] + '|' + arrtemp[arrtemp.Count() - 14]);
                    //        streamwriter.Close();
                    //    }
                    //    if (arrtemp.Count() > 39 && arrtemp.Count() != 40)
                    //    {
                    //        string filename = arrtemp1[1] + ".csv";
                    //        string destPath = System.IO.Path.Combine(targetPath, filename);
                    //        FileStream fs = new FileStream(destPath, FileMode.Append);
                    //        StreamWriter streamwriter = new StreamWriter(fs);
                    //        streamwriter.WriteLine(arrtemp1[1] + '|' + arrtemp[arrtemp.Count() - 23] + '|' + arrtemp1[0] + '|' + arrtemp[arrtemp.Count() - 21] + '|' + arrtemp[arrtemp.Count() - 20] + '|' + arrtemp[arrtemp.Count() - 19] + '|' + arrtemp[arrtemp.Count() - 18] + '|' + arrtemp[arrtemp.Count() - 17] + '|' + arrtemp[arrtemp.Count() - 16] + '|' + arrtemp[arrtemp.Count() - 15] + '|' + arrtemp[arrtemp.Count() - 14]);
                    //        streamwriter.Close();
                    //    }
                    //}
                    //有些格式不太对，2022-7-12月重新修改
                    #region
                    if (arrtemp.Count() <= 26)
                    {
                        continue;
                    }
                    else
                    {
                        if (arrtemp[16].Substring(0, 3) == arrtemp[17] || arrtemp[16].Substring(2, 3) == arrtemp[17])
                        {
                            if (Convert.ToDouble(arrtemp[18]) < 16 && isNumberic(arrtemp[20]) && Convert.ToDouble(arrtemp[20]) < 110)
                            {
                                string filename = arrtemp1[1] + ".csv";
                                string destPath = System.IO.Path.Combine(targetPath, filename);
                                FileStream fs = new FileStream(destPath, FileMode.Append);
                                StreamWriter streamwriter = new StreamWriter(fs);
                                streamwriter.WriteLine(arrtemp1[1] + '|' + arrtemp[16] + '|' + arrtemp1[0] + '|' + arrtemp[18] + '|' + arrtemp[19] + '|' + arrtemp[20] + '|' + arrtemp[21] + '|' + arrtemp[22] + '|' + arrtemp[23] + '|' + arrtemp[24] + '|' + arrtemp[25]);
                                streamwriter.Close();
                            }
                            else
                            {
                                if (arrtemp.Count() > 28 && (arrtemp[25].Contains('.') || arrtemp[26].Contains('.')))
                                {
                                    string filename = arrtemp1[1] + ".csv";
                                    string destPath = System.IO.Path.Combine(targetPath, filename);
                                    FileStream fs = new FileStream(destPath, FileMode.Append);
                                    StreamWriter streamwriter = new StreamWriter(fs);
                                    streamwriter.WriteLine(arrtemp1[1] + '|' + arrtemp[16] + '|' + arrtemp1[0] + '|' + arrtemp[21] + '|' + arrtemp[22] + '|' + arrtemp[23] + '|' + arrtemp[24] + '|' + arrtemp[25] + '|' + arrtemp[26] + '|' + arrtemp[27] + '|' + arrtemp[28]);
                                    streamwriter.Close();
                                }

                            }
                        }
                        else
                        {
                            string filename = arrtemp1[1] + ".csv";
                            string destPath = System.IO.Path.Combine(targetPath, filename);
                            FileStream fs = new FileStream(destPath, FileMode.Append);
                            StreamWriter streamwriter = new StreamWriter(fs);
                            streamwriter.WriteLine(arrtemp1[1] + '|' + arrtemp[16] + '|' + arrtemp1[0] + '|' + arrtemp[17] + '|' + arrtemp[18] + '|' + arrtemp[19] + '|' + arrtemp[20] + '|' + arrtemp[21] + '|' + arrtemp[22] + '|' + arrtemp[23] + '|' + arrtemp[24]);
                            streamwriter.Close();
                        }
                    }
                    #endregion

                }
            }
            MessageBox.Show("OK");
        }
        //按时间排序
        private void button3_Click(object sender, EventArgs e)
        {
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序";
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string destPath = System.IO.Path.Combine(targetPath, nextFile.Name);
                StreamWriter streamwriter = new StreamWriter(destPath);
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line = "";
                //定义列表存储数据
                DataTable DT = new DataTable();
                //定义列表的每一栏分别表示什么
                //第一栏船舶IMO
                DT.Columns.Add("IMO");
                //第二栏船舶MMSI
                DT.Columns.Add("MMSI");
                //第三栏定位时间
                DT.Columns.Add("PosTime");
                //第四栏航行状态
                DT.Columns.Add("NavigationStatus");
                //第五栏是船舶转向率
                DT.Columns.Add("ROT");
                //第六栏是航速
                DT.Columns.Add("Speed");
                //第七栏是定位精度
                DT.Columns.Add("Accuracy");
                //第八栏经度
                DT.Columns.Add("Lon");
                //第九栏纬度
                DT.Columns.Add("Lat");
                //第十栏是航向
                DT.Columns.Add("Cour");
                //第十一栏是船艏向
                DT.Columns.Add("TrueHeading");
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split('|');
                    DT.Rows.Add(new object[] { arrtemp[0], arrtemp[1], arrtemp[2], arrtemp[3], arrtemp[4], arrtemp[5], arrtemp[6], arrtemp[7], arrtemp[8], arrtemp[9], arrtemp[10] });
                }
                //按定位时间升序排序
                DT.DefaultView.Sort = "PosTime Asc";
                //返回一个新的DataTable
                DT = DT.DefaultView.ToTable();
                //写出表格中的内容
                //写出表头也就是写出列名称
                string data = "";
                for (int i = 0; i < DT.Columns.Count; i++)
                {
                    data += DT.Columns[i].ColumnName;
                    if (i < DT.Columns.Count - 1)
                    {
                        data += "|";
                    }
                }
                streamwriter.WriteLine(data);
                //写出各行数据
                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    data = "";
                    for (int j = 0; j < DT.Columns.Count; j++)
                    {
                        string str = DT.Rows[i][j].ToString();
                        data += str;
                        if (j < DT.Columns.Count - 1)
                        {
                            data += "|";
                        }
                    }
                    streamwriter.WriteLine(data);
                }
                DT.Dispose();
                streamwriter.Close();
            }
            MessageBox.Show("OK");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\portCenter.csv";

            //StreamReader streamreader = new StreamReader(inPath);
            ////将港口位置信息存储在字典中
            //Dictionary<string, List<double>> Dic_Po = new Dictionary<string, List<double>>();
            //string line = "";
            //while ((line = streamreader.ReadLine()) != null)
            //{
            //    string[] arrtemp = line.Split(',');
            //    List<double> Lis_temp = new List<double>();
            //    Lis_temp.Add(Convert.ToDouble(arrtemp[1]));
            //    Lis_temp.Add(Convert.ToDouble(arrtemp[2]));
            //    Dic_Po.Add(arrtemp[0], Lis_temp);
            //}

            string inPath = @"F:\2018-2021LNG和Container处理_22_7_11\World LNG Map2022.csv";
            StreamReader streamreader = new StreamReader(inPath);
            //将港口位置信息存储在字典中
            Dictionary<string, List<double>> Dic_Po = new Dictionary<string, List<double>>();
            string line = "";
            while ((line = streamreader.ReadLine()) != null)
            {
                string[] arrtemp = line.Split(',');
                if (arrtemp[2] == "Lat")
                {
                    continue;
                }
                else
                {
                    List<double> Lis_temp = new List<double>();
                    Lis_temp.Add(Convert.ToDouble(arrtemp[3]));
                    Lis_temp.Add(Convert.ToDouble(arrtemp[2]));
                    Dic_Po.Add(arrtemp[1], Lis_temp);
                }
            }
            //依次判断船舶经停地港口
            // string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口";
            // DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序");

            //string targetPath = @"F:\2018-2021LNG处理_LNG基础数据更新\2018_LNG_经停港口\2018-01";
            //DirectoryInfo theFolder = new DirectoryInfo(@"F:\2018-2021LNG和Container处理_22_7_11\2018_船舶轨迹_LNG_时间排序\2018-01");
            //FileInfo[] dirFile = theFolder.GetFiles();

            string targetPath = @"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口";
            DirectoryInfo theFolder = new DirectoryInfo(@"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理");
            DirectoryInfo[] dirFolder = theFolder.GetDirectories();
            foreach (DirectoryInfo nextFolder in dirFolder)
            {
                string destPath0 = System.IO.Path.Combine(targetPath, nextFolder.Name);
                if (Directory.Exists(destPath0) == false)
                {
                    Directory.CreateDirectory(destPath0);
                }
                DirectoryInfo[] dirFolder1 = nextFolder.GetDirectories();
                foreach (DirectoryInfo nextFolder1 in dirFolder1)
                {
                    string destPath1 = System.IO.Path.Combine(destPath0, nextFolder1.Name);
                    if (Directory.Exists(destPath1) == false)
                    {
                        Directory.CreateDirectory(destPath1);
                    }

                    FileInfo[] dirFile = nextFolder1.GetFiles();


                    //string ID = items[0];
                    //double X = Convert.ToDouble(items[4]);
                    //double Y = Convert.ToDouble(items[5]);
                    ////如果船舶在港口5公里范围内则认为船舶在该港口停靠
                    //double range = 5000;
                    //double yDirection = range / 111000;
                    //double xDirection = range / (111000 * Math.Cos(centerY / 180.0 * Math.PI));
                    //BoundaryBox mbr = new BoundaryBox(centerX - xDirection, centerY - yDirection, centerX + xDirection, centerY + yDirection);

                    //if (X < this.mbr.XMin || X > this.mbr.XMax || Y < this.mbr.YMin || Y > this.mbr.YMax) return null;

                    //double timeStamp = Convert.ToDouble(items[3]);

                    //Dictionary<string, object> innerData = new Dictionary<string, object>();
                    //innerData.Add("aistype", items[2]);
                    //innerData.Add("Origin", items[6]);
                    //innerData.Add("Destination", items[7]);

                    foreach (FileInfo nextFile in dirFile)
                    {
                        string destPath = System.IO.Path.Combine(destPath1, nextFile.Name);
                        StreamWriter streamwriter = new StreamWriter(destPath);
                        StreamReader streamreader1 = new StreamReader(nextFile.FullName);
                        string line1 = "";
                        while ((line1 = streamreader1.ReadLine()) != null)
                        {
                            string[] arrtemp1 = line1.Split(',');
                            if (arrtemp1[0] == "IMO")
                            {
                                //streamwriter.WriteLine(line1 + '|' + "InandOut" + '|' + "Port_Name" + '|' + "Lon" + '|' + "Lat");
                                streamwriter.WriteLine(line1 + ',' + "InandOut" + ',' + "Port_Name" + ',' + "Lon" + ',' + "Lat");
                            }
                            else
                            {
                                //double i = 0;
                                string line2 = line1;
                                foreach (KeyValuePair<string, List<double>> KVP1 in Dic_Po)
                                {
                                    //if (get_distance(Convert.ToDouble(arrtemp1[8]), Convert.ToDouble(arrtemp1[7]), KVP1.Value[1], KVP1.Value[0]) <= 5000)
                                    //{
                                    //    line2 = line2 + '|' + "In" + '|' + KVP1.Key + '|' + Convert.ToString(KVP1.Value[0]) + '|' + Convert.ToString(KVP1.Value[1]);
                                    //}

                                    //2022-7-14修改
                                    //if (arrtemp1[6].Contains('.') && isNumberic(arrtemp1[6]) && isNumberic(arrtemp1[7]))
                                    //{
                                    //    if (get_distance(Convert.ToDouble(arrtemp1[7]), Convert.ToDouble(arrtemp1[6]), KVP1.Value[1], KVP1.Value[0]) <= 5000)
                                    //    {
                                    //        line2 = line2 + '|' + "In" + '|' + KVP1.Key + '|' + Convert.ToString(KVP1.Value[0]) + '|' + Convert.ToString(KVP1.Value[1]);
                                    //    }
                                    //}
                                    //2022-7-14修改
                                    //else if (isNumberic(arrtemp1[8]) && isNumberic(arrtemp1[7]))
                                    //{
                                    //    if (get_distance(Convert.ToDouble(arrtemp1[8]), Convert.ToDouble(arrtemp1[7]), KVP1.Value[1], KVP1.Value[0]) <= 5000)
                                    //    {
                                    //        line2 = line2 + '|' + "In" + '|' + KVP1.Key + '|' + Convert.ToString(KVP1.Value[0]) + '|' + Convert.ToString(KVP1.Value[1]);
                                    //    }
                                    //}
                                    if (get_distance(Convert.ToDouble(arrtemp1[8]), Convert.ToDouble(arrtemp1[7]), KVP1.Value[1], KVP1.Value[0]) <= 5000)
                                    {
                                        line2 = line2 + ',' + "In" + ',' + KVP1.Key + ',' + Convert.ToString(KVP1.Value[0]) + ',' + Convert.ToString(KVP1.Value[1]);
                                    }

                                }
                                streamwriter.WriteLine(line2);
                            }

                        }
                        streamwriter.Close();
                    }
                }
            }
            MessageBox.Show("OK");
        }
        //删除异常值，经度范围是-180到180之间，纬度是-90到90之间，而且航行状态必须含有0和1
        //船舶AIS的航行状态必须包括0和1，0表示航行中，1表示抛锚，2表示没有在命令下，3表示操纵受限制
        //AIS的状态4表示系泊，5表示吃水限制，6搁浅，7进行捕捞，8操帆在航
        private void button5_Click(object sender, EventArgs e)
        {
            //DirectoryInfo theFolder = new DirectoryInfo(@"F:\2018-2021LNG处理_LNG基础数据更新\2021_LNG_经停港口\2021-11");
            //string targetPath = @"F:\2018-2021LNG处理_LNG基础数据更新\2021_LNG_经停港口_删异\2021-11";

            DirectoryInfo theFolder = new DirectoryInfo(@"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口");
            string targetPath = @"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口_删异";
            //DirectoryInfo theFolder = new DirectoryInfo(@"F:\2018-2021LNG和Container处理_22_7_11\2018_船舶轨迹_LNG_时间排序_经停港口\2018-01");
            //string targetPath = @"F:\2018-2021LNG和Container处理_22_7_11\2018_船舶轨迹_LNG_时间排序_经停港口_删异\2018-01";

            //FileInfo[] dirFile = theFolder.GetFiles();

            DirectoryInfo[] dirFolder = theFolder.GetDirectories();
            foreach (DirectoryInfo nextFolder in dirFolder)
            {
                string destPath0 = System.IO.Path.Combine(targetPath, nextFolder.Name);
                if (Directory.Exists(destPath0) == false)
                {
                    Directory.CreateDirectory(destPath0);
                }
                DirectoryInfo[] dirFolder1 = nextFolder.GetDirectories();
                foreach (DirectoryInfo nextFolder1 in dirFolder1)
                {
                    string destPath1 = System.IO.Path.Combine(destPath0, nextFolder1.Name);
                    if (Directory.Exists(destPath1) == false)
                    {
                        Directory.CreateDirectory(destPath1);
                    }

                    FileInfo[] dirFile = nextFolder1.GetFiles();

                    foreach (FileInfo nextFile in dirFile)
                    {
                        StreamReader streamreader = new StreamReader(nextFile.FullName);
                        string destPath = System.IO.Path.Combine(destPath1, nextFile.Name);
                        StreamWriter streamwriter = new StreamWriter(destPath);
                        string line = "";
                        while ((line = streamreader.ReadLine()) != null)
                        {
                            string[] arrtemp = line.Split(',');
                            if (arrtemp[1] == "MMSI")
                            {
                                streamwriter.WriteLine(line);
                            }
                            else
                            {
                                if (isNumberic(arrtemp[7]))
                                {
                                    if ((Convert.ToDouble(arrtemp[7]) >= -180) && (Convert.ToDouble(arrtemp[7]) <= 180) && (Convert.ToDouble(arrtemp[8]) >= -180) && (Convert.ToDouble(arrtemp[8]) <= 180))
                                    {
                                        streamwriter.WriteLine(line);
                                    }
                                }
                            }
                        }
                        streamwriter.Close();
                    }
                }
            }
            MessageBox.Show("OK");
        }
        //在判断是否在港口内，存在同时在多个港内的情况，只保留最近的那个
        //因为船舶的速度很低时也可能标和是在航状态0，所以不能基于保留0和1的状态的基础上做
        private void button6_Click(object sender, EventArgs e)
        {
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港";
            DirectoryInfo theFolder = new DirectoryInfo(@"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口_删异");
            string targetPath = @"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口_删异_IN唯一港";
            //FileInfo[] dirFile = theFolder.GetFiles();

            DirectoryInfo[] dirFolder = theFolder.GetDirectories();
            foreach (DirectoryInfo nextFolder in dirFolder)
            {
                string destPath0 = System.IO.Path.Combine(targetPath, nextFolder.Name);
                if (Directory.Exists(destPath0) == false)
                {
                    Directory.CreateDirectory(destPath0);
                }
                DirectoryInfo[] dirFolder1 = nextFolder.GetDirectories();
                foreach (DirectoryInfo nextFolder1 in dirFolder1)
                {
                    string destPath1 = System.IO.Path.Combine(destPath0, nextFolder1.Name);
                    if (Directory.Exists(destPath1) == false)
                    {
                        Directory.CreateDirectory(destPath1);
                    }

                    FileInfo[] dirFile = nextFolder1.GetFiles();
                    foreach (FileInfo nextFile in dirFile)
                    {
                        string destPath = System.IO.Path.Combine(destPath1, nextFile.Name);
                        StreamWriter streamwriter = new StreamWriter(destPath);
                        StreamReader streamreader = new StreamReader(nextFile.FullName);
                        string line = "";
                        while ((line = streamreader.ReadLine()) != null)
                        {
                            string[] arrtemp = line.Split(',');
                            if (arrtemp.Count() == 11 || arrtemp.Count() == 15)
                            {
                                streamwriter.WriteLine(line);
                            }
                            //判断船舶离哪个港口最近则认为它在哪个港口里面
                            else
                            {
                                //定义字典存储距离每个港口的距离
                                Dictionary<int, double> Dic = new Dictionary<int, double>();
                                for (int i = 11; i <= arrtemp.Count() - 4; i = i + 4)
                                {
                                    double dou = get_distance(Convert.ToDouble(arrtemp[8]), Convert.ToDouble(arrtemp[7]), Convert.ToDouble(arrtemp[i + 3]), Convert.ToDouble(arrtemp[i + 2]));
                                    Dic.Add(i, dou);
                                }
                                //判断距离最近的港口
                                int j = 0;
                                double dis_temp = 10000000000000000;
                                foreach (KeyValuePair<int, double> KVP1 in Dic)
                                {
                                    if (KVP1.Value <= dis_temp)
                                    {
                                        j = KVP1.Key;
                                        dis_temp = KVP1.Value;
                                    }
                                }
                                // streamwriter.WriteLine(arrtemp[0] + '|' + arrtemp[1] + '|' + arrtemp[2] + '|' + arrtemp[3] + '|' + arrtemp[4] + '|' + arrtemp[5] + '|' + arrtemp[6] + '|' + arrtemp[7] + '|' + arrtemp[8] + '|' + arrtemp[9] + '|' + arrtemp[10] + '|' + arrtemp[j] + '|' + arrtemp[j + 1] + '|' + arrtemp[j + 2] + '|' + arrtemp[j + 3] + '|' + Convert.ToString(dis_temp));
                                streamwriter.WriteLine(arrtemp[0] + ',' + arrtemp[1] + ',' + arrtemp[2] + ',' + arrtemp[3] + ',' + arrtemp[4] + ',' + arrtemp[5] + ',' + arrtemp[6] + ',' + arrtemp[7] + ',' + arrtemp[8] + ',' + arrtemp[9] + ',' + arrtemp[10] + ',' + arrtemp[j] + ',' + arrtemp[j + 1] + ',' + arrtemp[j + 2] + ',' + arrtemp[j + 3] + ',' + Convert.ToString(dis_temp));
                            }
                        }
                        streamwriter.Close();
                    }
                }
            }
            MessageBox.Show("OK");
        }
        //判断船舶状态
        private void button7_Click(object sender, EventArgs e)
        {
            //船舶状态的判定
            //如果速度小于1m/s每秒则判定为STOP
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态";
            //FileInfo[] dirFile = theFolder.GetFiles();

            DirectoryInfo theFolder = new DirectoryInfo(@"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口_删异_IN唯一港");
            string targetPath = @"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口_删异_IN唯一港_船舶状态";
            DirectoryInfo[] dirFolder = theFolder.GetDirectories();
            foreach (DirectoryInfo nextFolder in dirFolder)
            {
                string destPath0 = System.IO.Path.Combine(targetPath, nextFolder.Name);
                if (Directory.Exists(destPath0) == false)
                {
                    Directory.CreateDirectory(destPath0);
                }
                DirectoryInfo[] dirFolder1 = nextFolder.GetDirectories();
                foreach (DirectoryInfo nextFolder1 in dirFolder1)
                {
                    string destPath1 = System.IO.Path.Combine(destPath0, nextFolder1.Name);
                    if (Directory.Exists(destPath1) == false)
                    {
                        Directory.CreateDirectory(destPath1);
                    }

                    FileInfo[] dirFile = nextFolder1.GetFiles();
                    foreach (FileInfo nextFile in dirFile)
                    {
                        StreamReader streamreader = new StreamReader(nextFile.FullName);
                        string destPath = System.IO.Path.Combine(destPath1, nextFile.Name);
                        StreamWriter streamwriter = new StreamWriter(destPath);
                        string line = "";
                        while ((line = streamreader.ReadLine()) != null)
                        {
                            string[] arrtemp = line.Split(',');
                            if (line.Contains("MMSI"))
                            {
                                streamwriter.WriteLine(line);
                            }
                            else
                            {
                                if (arrtemp[10] == "null")
                                {
                                    // streamwriter.WriteLine(line);
                                    //如果速度为空就不输出了
                                    continue;
                                }
                                else
                                {
                                    if (Convert.ToDouble(arrtemp[5]) < 0.5 && arrtemp.Count() == 11)
                                    {
                                        streamwriter.WriteLine(line + ',' + "Stop_out_Port");
                                    }
                                    if (Convert.ToDouble(arrtemp[5]) < 0.5 && (arrtemp.Count() == 15 || arrtemp.Count() == 16))
                                    {
                                        streamwriter.WriteLine(line + ',' + "Stop" + '_' + arrtemp[11] + '_' + arrtemp[12]);
                                    }
                                    if (Convert.ToDouble(arrtemp[5]) >= 0.5 && arrtemp.Count() == 11)
                                    {
                                        streamwriter.WriteLine(line + ',' + "Sail_out_Port");
                                    }
                                    if (Convert.ToDouble(arrtemp[5]) >= 0.5 && (arrtemp.Count() == 15 || arrtemp.Count() == 16))
                                    {
                                        streamwriter.WriteLine(line + ',' + "Sail" + '_' + arrtemp[11] + '_' + arrtemp[12]);
                                    }
                                    //else
                                    //{
                                    //    streamwriter.WriteLine(line);
                                    //}
                                }
                            }
                        }


                        streamwriter.Close();
                    }
                }
            }
            MessageBox.Show("OK");
        }
        //定义表格判断船舶的Motifs
        private void button8_Click(object sender, EventArgs e)
        {
            //定义列表存储数据
            DataTable DT = new DataTable();
            //定义列表的每一栏分别表示什么
            DT.Columns.Add("IMO");
            //第一栏船舶MMSI
            DT.Columns.Add("MMSI");
            //第二栏港口名称
            DT.Columns.Add("RortName");
            //第三栏定义港口的经度
            DT.Columns.Add("Lon");
            //第四栏定义港口的纬度
            DT.Columns.Add("Lat");
            //第五栏状态类型
            DT.Columns.Add("Satue");
            //第六栏状态开始时间
            DT.Columns.Add("StartTime");
            //第七栏状态结束时间
            DT.Columns.Add("EndTime");
            //也可以定义字典存储，状态为键值，开始时间和结束时间放于列表中

            //PortName 如果在港外就写成 outPort
            //Statue 包括：Sail_for_In_Start、Sail_for_In_End、Stop_In_Start、Stop_In_End、Sail_for_Out_Start、Sail_for_Out_End  
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs";
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态");
            //string targetPath = @"F:\2018-2021LNG处理_LNG基础数据更新\2020_LNG_经停港口_IN唯一港_船舶状态_按季度合并_Motifs\1-4月";
            // DirectoryInfo theFolder = new DirectoryInfo(@"F:\2018-2021LNG处理_LNG基础数据更新\2020_LNG_经停港口_IN唯一港_船舶状态_按季度合并\1-4月");
            //FileInfo[] dirFile = theFolder.GetFiles();

            DirectoryInfo theFolder = new DirectoryInfo(@"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口_删异_IN唯一港_船舶状态");
            string targetPath = @"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口_删异_IN唯一港_船舶状态_Motifs";
            DirectoryInfo[] dirFolder = theFolder.GetDirectories();
            foreach (DirectoryInfo nextFolder in dirFolder)
            {
                string destPath0 = System.IO.Path.Combine(targetPath, nextFolder.Name);
                if (Directory.Exists(destPath0) == false)
                {
                    Directory.CreateDirectory(destPath0);
                }
                DirectoryInfo[] dirFolder1 = nextFolder.GetDirectories();
                foreach (DirectoryInfo nextFolder1 in dirFolder1)
                {
                    string destPath1 = System.IO.Path.Combine(destPath0, nextFolder1.Name);
                    if (Directory.Exists(destPath1) == false)
                    {
                        Directory.CreateDirectory(destPath1);
                    }

                    FileInfo[] dirFile = nextFolder1.GetFiles();

                    foreach (FileInfo nextFile in dirFile)
                    {
                        Dictionary<string, List<DateTime>> Dic = new Dictionary<string, List<DateTime>>();
                        StreamReader streamreader = new StreamReader(nextFile.FullName);
                        string line = "";
                        //定义列表，存储船舶状态
                        List<string> Lis_Sta = new List<string>();
                        string ex_Status = "";
                        int q = 0;
                        while ((line = streamreader.ReadLine()) != null)
                        {
                            string[] arrtemp = line.Split(',');
                            if (arrtemp[1] == "MMSI")
                            {
                                continue;
                            }
                            //先判断船舶的状态，后续在根据船舶的状态的进一步处理
                            else
                            {
                                //int j = 0;
                                ////说明是在港外航行状态
                                //DateTime dt_end = DateTime.Now;
                                //DateTime dt_start = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                                //TimeSpan tsend = dt_end - Convert.ToDateTime(arrtemp[3]);
                                //TimeSpan tsstar = Convert.ToDateTime(arrtemp[3]) - dt_start;
                                //if (tsend.TotalSeconds >= 0)
                                //{
                                //    dt_end = Convert.ToDateTime(arrtemp[3]);
                                //}


                                //int k = j-1;
                                //第一次出现某一种状态
                                //用前一个状态和后一个状态的叠加作为键值

                                if (Dic.ContainsKey(arrtemp[arrtemp.Count() - 1]) == false)
                                {
                                    List<DateTime> DT_Temp = new List<DateTime>();
                                    DT_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    DT_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dic.Add(arrtemp[arrtemp.Count() - 1], DT_Temp);
                                    ex_Status = arrtemp[arrtemp.Count() - 1];
                                    Lis_Sta.Add(arrtemp[arrtemp.Count() - 1]);

                                }
                                else
                                {
                                    if (Dic.ContainsKey(arrtemp[arrtemp.Count() - 1]) == true && ex_Status == arrtemp[arrtemp.Count() - 1] && Dic.ContainsKey(arrtemp[arrtemp.Count() - 1] + '_' + q) == false)
                                    {
                                        List<DateTime> DT_Temp1 = Dic[arrtemp[arrtemp.Count() - 1]];
                                        List<DateTime> DT_Temp2 = new List<DateTime>();
                                        TimeSpan ts = Convert.ToDateTime(arrtemp[2]) - DT_Temp1[1];
                                        if (ts.TotalSeconds >= 0)
                                        {
                                            DT_Temp2.Add(DT_Temp1[0]);
                                            DT_Temp2.Add(Convert.ToDateTime(arrtemp[2]));
                                        }
                                        Dic[arrtemp[arrtemp.Count() - 1]] = DT_Temp2;
                                        ex_Status = arrtemp[arrtemp.Count() - 1];
                                    }
                                    //这种状态重复出现了
                                    if (Dic.ContainsKey(arrtemp[arrtemp.Count() - 1]) == true && ex_Status != arrtemp[arrtemp.Count() - 1])
                                    {
                                        Lis_Sta.Add(arrtemp[arrtemp.Count() - 1]);
                                        q = Lis_Sta.Count(x => x.Contains(arrtemp[arrtemp.Count() - 1]));
                                        if (Dic.ContainsKey(arrtemp[arrtemp.Count() - 1] + '_' + q) == false)
                                        {

                                            List<DateTime> DT_Temp = new List<DateTime>();
                                            DT_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                            DT_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                            Dic.Add(arrtemp[arrtemp.Count() - 1] + '_' + q, DT_Temp);
                                            ex_Status = arrtemp[arrtemp.Count() - 1];


                                        }

                                    }

                                    if (Dic.ContainsKey(arrtemp[arrtemp.Count() - 1] + '_' + q) == true && ex_Status == arrtemp[arrtemp.Count() - 1])
                                    {
                                        List<DateTime> DT_Temp1 = Dic[arrtemp[arrtemp.Count() - 1] + '_' + q];
                                        List<DateTime> DT_Temp2 = new List<DateTime>();
                                        TimeSpan ts = Convert.ToDateTime(arrtemp[2]) - DT_Temp1[1];
                                        if (ts.TotalSeconds >= 0)
                                        {
                                            DT_Temp2.Add(DT_Temp1[0]);
                                            DT_Temp2.Add(Convert.ToDateTime(arrtemp[2]));
                                        }
                                        Dic[arrtemp[arrtemp.Count() - 1] + '_' + q] = DT_Temp2;
                                        ex_Status = arrtemp[arrtemp.Count() - 1];
                                    }
                                }
                            }


                        }



                        //输出船舶Moifs
                        string destPath = System.IO.Path.Combine(destPath1, nextFile.Name);
                        StreamWriter streamwriter = new StreamWriter(destPath);
                        foreach (KeyValuePair<string, List<DateTime>> KVP1 in Dic)
                        {
                            //streamwriter.WriteLine(KVP1.Key + '|' + Convert.ToString(KVP1.Value[0]) + '|' + Convert.ToString(KVP1.Value[1]));
                            streamwriter.WriteLine(KVP1.Key + ',' + Convert.ToString(KVP1.Value[0]) + ',' + Convert.ToString(KVP1.Value[1]));
                        }
                        streamwriter.Close();
                    }
                }
            }
            MessageBox.Show("OK");
        }
        //如果motifs中某一种状态的起始和截止状态相同则删除该状态
        private void button9_Click(object sender, EventArgs e)
        {
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs");
            // string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs_删异";
            // FileInfo[] dirFile = theFolder.GetFiles();

            DirectoryInfo theFolder = new DirectoryInfo(@"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口_删异_IN唯一港_船舶状态_Motifs");
            string targetPath = @"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口_删异_IN唯一港_船舶状态_Motifs_删异";
            DirectoryInfo[] dirFolder = theFolder.GetDirectories();
            foreach (DirectoryInfo nextFolder in dirFolder)
            {
                string destPath0 = System.IO.Path.Combine(targetPath, nextFolder.Name);
                if (Directory.Exists(destPath0) == false)
                {
                    Directory.CreateDirectory(destPath0);
                }
                DirectoryInfo[] dirFolder1 = nextFolder.GetDirectories();
                foreach (DirectoryInfo nextFolder1 in dirFolder1)
                {
                    string destPath1 = System.IO.Path.Combine(destPath0, nextFolder1.Name);
                    if (Directory.Exists(destPath1) == false)
                    {
                        Directory.CreateDirectory(destPath1);
                    }

                    FileInfo[] dirFile = nextFolder1.GetFiles();
                    foreach (FileInfo nextFile in dirFile)
                    {
                        StreamReader streamreader = new StreamReader(nextFile.FullName);
                        string destPath = System.IO.Path.Combine(destPath1, nextFile.Name);
                        StreamWriter streamwriter = new StreamWriter(destPath);
                        string line = "";
                        while ((line = streamreader.ReadLine()) != null)
                        {
                            string[] arrtemp = line.Split(',');
                            if (arrtemp[1] != arrtemp[2])
                            {
                                streamwriter.WriteLine(line);
                            }
                        }
                        streamwriter.Close();
                    }
                }
            }
            MessageBox.Show("OK");
        }
        //如果某条船的motifs在某港口内的状态只有saill或stop则删除不能算有效停留
        private void button10_Click(object sender, EventArgs e)
        {
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs_删异");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1";
            //FileInfo[] dirFile = theFolder.GetFiles();

            DirectoryInfo theFolder = new DirectoryInfo(@"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口_删异_IN唯一港_船舶状态_Motifs_删异");
            string targetPath = @"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1";
            DirectoryInfo[] dirFolder = theFolder.GetDirectories();
            foreach (DirectoryInfo nextFolder in dirFolder)
            {
                string destPath0 = System.IO.Path.Combine(targetPath, nextFolder.Name);
                if (Directory.Exists(destPath0) == false)
                {
                    Directory.CreateDirectory(destPath0);
                }
                DirectoryInfo[] dirFolder1 = nextFolder.GetDirectories();
                foreach (DirectoryInfo nextFolder1 in dirFolder1)
                {
                    string destPath1 = System.IO.Path.Combine(destPath0, nextFolder1.Name);
                    if (Directory.Exists(destPath1) == false)
                    {
                        Directory.CreateDirectory(destPath1);
                    }

                    FileInfo[] dirFile = nextFolder1.GetFiles();
                    foreach (FileInfo nextFile in dirFile)
                    {
                        string destPath = System.IO.Path.Combine(destPath1, nextFile.Name);
                        StreamWriter streamwriter = new StreamWriter(destPath);
                        StreamReader streamreader = new StreamReader(nextFile.FullName);
                        string line = "";
                        //定义列表暂时存储行
                        List<string> Lis_Temp = new List<string>();
                        //定义列表存储船舶经停或在航的港口名称
                        List<string> Lis_Temp1 = new List<string>();
                        while ((line = streamreader.ReadLine()) != null)
                        {
                            Lis_Temp.Add(line);
                            string[] arrtemp = line.Split(',');
                            if (arrtemp[0].Contains("Port") == false)
                            {
                                Lis_Temp1.Add(arrtemp[0]);
                            }

                        }
                        foreach (string a in Lis_Temp)
                        {
                            string[] arrtemp1 = a.Split(',');
                            string[] arrtemp2 = arrtemp1[0].Split('_');
                            if (Lis_Temp1.Contains(arrtemp1[0]) == false)
                            {
                                streamwriter.WriteLine(a);
                            }
                            else
                            {
                                if (get_str_count(Lis_Temp1, arrtemp2[2]) > 1)
                                {
                                    streamwriter.WriteLine(a);
                                }
                            }
                        }
                        streamwriter.Close();
                    }
                }
            }
            MessageBox.Show("OK");
        }
        //如果motifs中的前一状态与当前状态相同则进行合并
        private void button11_Click(object sender, EventArgs e)
        {
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1_同状态合并";
            //FileInfo[] dirFile = theFolder.GetFiles();

            DirectoryInfo theFolder = new DirectoryInfo(@"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1");
            string targetPath = @"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1_同状态合并";
            DirectoryInfo[] dirFolder = theFolder.GetDirectories();
            foreach (DirectoryInfo nextFolder in dirFolder)
            {
                string destPath0 = System.IO.Path.Combine(targetPath, nextFolder.Name);
                if (Directory.Exists(destPath0) == false)
                {
                    Directory.CreateDirectory(destPath0);
                }
                DirectoryInfo[] dirFolder1 = nextFolder.GetDirectories();
                foreach (DirectoryInfo nextFolder1 in dirFolder1)
                {
                    string destPath1 = System.IO.Path.Combine(destPath0, nextFolder1.Name);
                    if (Directory.Exists(destPath1) == false)
                    {
                        Directory.CreateDirectory(destPath1);
                    }

                    FileInfo[] dirFile = nextFolder1.GetFiles();
                    foreach (FileInfo nextFile in dirFile)
                    {
                        StreamReader streamreader = new StreamReader(nextFile.FullName);
                        string destPath = System.IO.Path.Combine(destPath1, nextFile.Name);
                        StreamWriter streamwriter = new StreamWriter(destPath);
                        Dictionary<string, List<DateTime>> Dic = new Dictionary<string, List<DateTime>>();
                        string line = "";
                        string exStatue = "";
                        while ((line = streamreader.ReadLine()) != null)
                        {
                            string[] arrtemp = line.Split(',');
                            if (exStatue == "")
                            {
                                exStatue = arrtemp[0];
                                List<DateTime> Lis_temp = new List<DateTime>();
                                Lis_temp.Add(Convert.ToDateTime(arrtemp[1]));
                                Lis_temp.Add(Convert.ToDateTime(arrtemp[2]));
                                Dic.Add(arrtemp[0], Lis_temp);
                            }
                            else
                            {
                                string[] arrtemp1 = exStatue.Split('_');
                                string[] arrtemp2 = arrtemp[0].Split('_');
                                if (arrtemp1[0] + '_' + arrtemp1[1] + '_' + arrtemp1[2] == arrtemp2[0] + '_' + arrtemp2[1] + '_' + arrtemp2[2])
                                {
                                    Dic[exStatue][1] = Convert.ToDateTime(arrtemp[2]);
                                }
                                else
                                {
                                    exStatue = arrtemp[0];
                                    List<DateTime> Lis_temp = new List<DateTime>();
                                    Lis_temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dic.Add(arrtemp[0], Lis_temp);
                                }
                            }
                        }
                        foreach (KeyValuePair<string, List<DateTime>> KVP1 in Dic)
                        {
                            // streamwriter.WriteLine(KVP1.Key + '|' + KVP1.Value[0] + '|' + KVP1.Value[1]);
                            streamwriter.WriteLine(KVP1.Key + ',' + KVP1.Value[0] + ',' + KVP1.Value[1]);
                        }
                        streamwriter.Close();
                    }
                }
            }
            MessageBox.Show("OK");
        }
        //由于船舶走走停停，进一步处理，获得时间序列
        private void button12_Click(object sender, EventArgs e)
        {
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1_同状态合并");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1_同状态合并_时间序列";
            //DirectoryInfo theFolder = new DirectoryInfo(@"F:\2018-2021LNG处理_LNG基础数据更新\2021_LNG_经停港口_IN唯一港_船舶状态_按季度合并_Motifs_删异_同状态合并\1-4月");
            //string targetPath = @"F:\2018-2021LNG处理_LNG基础数据更新\2021_LNG_经停港口_IN唯一港_船舶状态_按季度合并_Motifs_删异_同状态合并_时间序列\1-4月";

            DirectoryInfo theFolder = new DirectoryInfo(@"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1_同状态合并");
            string targetPath = @"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1_同状态合并_时间序列";

            //FileInfo[] dirFile = theFolder.GetFiles();

            DirectoryInfo[] dirFolder = theFolder.GetDirectories();
            foreach (DirectoryInfo nextFolder in dirFolder)
            {
                string destPath0 = System.IO.Path.Combine(targetPath, nextFolder.Name);
                if (Directory.Exists(destPath0) == false)
                {
                    Directory.CreateDirectory(destPath0);
                }
                DirectoryInfo[] dirFolder1 = nextFolder.GetDirectories();
                foreach (DirectoryInfo nextFolder1 in dirFolder1)
                {
                    string destPath1 = System.IO.Path.Combine(destPath0, nextFolder1.Name);
                    if (Directory.Exists(destPath1) == false)
                    {
                        Directory.CreateDirectory(destPath1);
                    }
                    FileInfo[] dirFile = nextFolder1.GetFiles();
                    foreach (FileInfo nextFile in dirFile)
                    {
                        string destPath = System.IO.Path.Combine(destPath1, nextFile.Name);
                        StreamWriter streamwriter = new StreamWriter(destPath);
                        StreamReader streamreader = new StreamReader(nextFile.FullName);
                        string line = "";
                        //定义字典获得时间序列
                        Dictionary<string, List<DateTime>> dic = new Dictionary<string, List<DateTime>>();
                        string exStatue = "";
                        string exStatue1 = "";
                        string exStatue2 = "";
                        int q = 0;
                        while ((line = streamreader.ReadLine()) != null)
                        {
                            string[] arrtemp = line.Split(',');
                            string[] arrtemp1 = arrtemp[0].Split('_');
                            if (exStatue == "")
                            {
                                List<DateTime> Lis_Temp = new List<DateTime>();
                                Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                dic.Add(arrtemp[0], Lis_Temp);
                                exStatue = arrtemp[0];
                                exStatue1 = arrtemp1[1] + '_' + arrtemp1[2];
                                exStatue2 = arrtemp1[0] + '_' + arrtemp1[1] + '_' + arrtemp1[2];
                            }
                            else
                            {
                                //说明是连续outPort/in which port的状态
                                if (exStatue1 == arrtemp1[1] + '_' + arrtemp1[2])
                                {
                                    List<string> Lis_Keys = new List<string>(dic.Keys);
                                    //定义列表存储存储包含当前arrtemp1[1] + '_' + arrtemp1[2]的所有键值
                                    List<string> Lis_Key_Spe = new List<string>();
                                    foreach (string a in Lis_Keys)
                                    {
                                        if (a.Contains(arrtemp1[0] + '_' + arrtemp1[1] + '_' + arrtemp1[2]))
                                        {

                                            Lis_Key_Spe.Add(a);
                                        }
                                    }
                                    //表示当前不存在sail/stop in  which port或sail/stop out port的状态
                                    if (Lis_Key_Spe.Count() == 0)
                                    {
                                        List<DateTime> Lis_Temp = new List<DateTime>();
                                        Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                        Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                        dic.Add(arrtemp[0], Lis_Temp);
                                        exStatue = arrtemp[0];
                                        exStatue1 = arrtemp1[1] + '_' + arrtemp1[2];
                                        exStatue2 = arrtemp1[0] + '_' + arrtemp1[1] + '_' + arrtemp1[2];
                                        q++;
                                    }
                                    //表示存在sail/stop in  which port或sail/stop out port的状态
                                    else
                                    {

                                        //表示状态切换之后是否是首次出现sail/stop in  which port或sail/stop out port的状态
                                        if (q == 0)
                                        {
                                            List<DateTime> Lis_Temp = new List<DateTime>();
                                            Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                            Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                            dic.Add(arrtemp[0], Lis_Temp);
                                            exStatue = arrtemp[0];
                                            exStatue1 = arrtemp1[1] + '_' + arrtemp1[2];
                                            exStatue2 = arrtemp1[0] + '_' + arrtemp1[1] + '_' + arrtemp1[2];
                                            q++;
                                        }
                                        else if (q != 0)
                                        {
                                            dic[Lis_Key_Spe[Lis_Key_Spe.Count() - 1]][1] = Convert.ToDateTime(arrtemp[2]);
                                            exStatue = arrtemp[0];
                                            exStatue1 = arrtemp1[1] + '_' + arrtemp1[2];
                                            exStatue2 = arrtemp1[0] + '_' + arrtemp1[1] + '_' + arrtemp1[2];

                                        }

                                    }


                                }
                                //说明是outPort与in which port间的切换状态
                                else
                                {
                                    if (arrtemp1.Count() > 3 && IsNumeric(arrtemp1[3]))
                                        q = Convert.ToInt32(arrtemp1[3]);
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    dic.Add(arrtemp[0], Lis_Temp);
                                    exStatue = arrtemp[0];
                                    exStatue1 = arrtemp1[1] + '_' + arrtemp1[2];
                                    exStatue2 = arrtemp1[0] + '_' + arrtemp1[1] + '_' + arrtemp1[2];
                                    q = 0;
                                }
                            }
                        }
                        foreach (KeyValuePair<string, List<DateTime>> KVP1 in dic)
                        {
                            //streamwriter.WriteLine(KVP1.Key + '|' + KVP1.Value[0] + '|' + KVP1.Value[1]);
                            streamwriter.WriteLine(KVP1.Key + ',' + KVP1.Value[0] + ',' + KVP1.Value[1]);
                        }
                        streamwriter.Close();
                    }
                }
            }
            MessageBox.Show("OK");
        }
        //统计船舶在港口内部和港口之间的航行时间和等待时间
        private void button13_Click(object sender, EventArgs e)
        {
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1_同状态合并_时间序列_时间指标统计";
            // DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1_同状态合并_时间序列");
            //string targetPath = @"F:\2018-2021LNG处理_LNG基础数据更新\2021_LNG_经停港口_IN唯一港_船舶状态_按季度合并_Motifs_删异_同状态合并_时间序列_时间指标统计\1-4月";
            //DirectoryInfo theFolder = new DirectoryInfo(@"F:\2018-2021LNG处理_LNG基础数据更新\2021_LNG_经停港口_IN唯一港_船舶状态_按季度合并_Motifs_删异_同状态合并_时间序列\1-4月");
            //FileInfo[] dirFile = theFolder.GetFiles();
            string targetPath = @"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1_同状态合并_时间序列_时间指标统计";
            DirectoryInfo theFolder = new DirectoryInfo(@"K:\陈丰\AIS轨迹处理_LNG\AIS_Trajectory_months_3_移位已处理_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1_同状态合并_时间序列");
            DirectoryInfo[] dirFolder = theFolder.GetDirectories();
            foreach (DirectoryInfo nextFolder in dirFolder)
            {
                string destPath0 = System.IO.Path.Combine(targetPath, nextFolder.Name);
                if (Directory.Exists(destPath0) == false)
                {
                    Directory.CreateDirectory(destPath0);
                }
                DirectoryInfo[] dirFolder1 = nextFolder.GetDirectories();
                foreach (DirectoryInfo nextFolder1 in dirFolder1)
                {
                    string destPath1 = System.IO.Path.Combine(destPath0, nextFolder1.Name);
                    if (Directory.Exists(destPath1) == false)
                    {
                        Directory.CreateDirectory(destPath1);
                    }
                    FileInfo[] dirFile = nextFolder1.GetFiles();
                    foreach (FileInfo nextFile in dirFile)
                    {
                        string destPath = System.IO.Path.Combine(destPath1, nextFile.Name);
                        StreamWriter streamwriter = new StreamWriter(destPath);
                        StreamReader streamreader = new StreamReader(nextFile.FullName);
                        string line = "";
                        //定义字典先把港口序列，既船舶在各个港口的航行和停留时间求出来
                        Dictionary<string, List<DateTime>> Dic = new Dictionary<string, List<DateTime>>();
                        //定义列表存储所有的行,便于后期统计船舶在港口之间的停留时间
                        List<string> Lines = new List<string>();
                        //定义变量存储船舶之前的状态
                        string exStatue = "";
                        while ((line = streamreader.ReadLine()) != null)
                        {
                            Lines.Add(line);
                            string[] arrtemp = line.Split(',');
                            string[] arrtemp1 = arrtemp[0].Split('_');
                            if (exStatue == "" && arrtemp[0].Contains("out_Port"))
                            {
                                continue;
                            }
                            else
                            {
                                //第一次出现在某个港口航行或停留的状态
                                if (exStatue == "")
                                {
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    //exStatue = arrtemp1[1] + '_' + arrtemp1[2];
                                    // Dic.Add(exStatue + "_to_" + arrtemp1[0] + '_' + arrtemp1[1] + '_' + arrtemp1[2], Lis_Temp);
                                    exStatue = arrtemp[0];
                                    Dic.Add(exStatue + "_to_" + arrtemp[0], Lis_Temp);
                                }
                                else
                                {
                                    if (arrtemp[0].Contains("out_Port") == false && exStatue == arrtemp1[1] + '_' + arrtemp1[2])
                                    {

                                        //表示存在航行或者停留某港口的状态
                                        //if (Dic.ContainsKey(exStatue + "_to_" + arrtemp1[0] + '_' + arrtemp1[1] + '_' + arrtemp1[2]))
                                        if (Dic.ContainsKey(exStatue + "_to_" + arrtemp[0]))
                                        {
                                            //Dic[exStatue + "_to_" + arrtemp1[0] + '_' + arrtemp1[1] + '_' + arrtemp1[2]][1] = Convert.ToDateTime(arrtemp[2]);
                                            Dic[exStatue + "_to_" + arrtemp[0]][1] = Convert.ToDateTime(arrtemp[2]);
                                            //exStatue = arrtemp1[1] + '_' + arrtemp1[2];
                                            exStatue = arrtemp[0];
                                        }
                                        else
                                        {
                                            List<DateTime> Lis_Temp = new List<DateTime>();
                                            Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                            Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                            // Dic.Add(exStatue + "_to_" + arrtemp1[0] + '_' + arrtemp1[1] + '_' + arrtemp1[2], Lis_Temp);
                                            Dic.Add(exStatue + "_to_" + arrtemp[0], Lis_Temp);
                                            //exStatue = arrtemp1[1] + '_' + arrtemp1[2];
                                            exStatue = arrtemp[0];
                                        }
                                    }
                                    //表示切换港口了
                                    else if (arrtemp[0].Contains("out_Port") == false && exStatue != arrtemp1[1] + '_' + arrtemp1[2])
                                    {
                                        List<DateTime> Lis_Temp = new List<DateTime>();
                                        Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                        Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                        //Dic.Add(exStatue + "_to_" + arrtemp1[0] + '_' + arrtemp1[1] + '_' + arrtemp1[2], Lis_Temp);
                                        Dic.Add(exStatue + "_to_" + arrtemp[0], Lis_Temp);
                                        //exStatue = arrtemp1[1] + '_' + arrtemp1[2];
                                        exStatue = arrtemp[0];
                                    }
                                }
                            }
                        }
                        //定义字典存储船舶在港口间等待的时间
                        Dictionary<string, List<DateTime>> Dic1 = new Dictionary<string, List<DateTime>>();
                        //统计某两个港口之间的停留时间
                        List<string> Lis_Keys = new List<string>(Dic.Keys);
                        for (int i = 1; i < Lis_Keys.Count() - 1; i++)
                        {
                            string[] arrtemp2 = Lis_Keys[i].Split('_');
                            //if (arrtemp2[1] == arrtemp2[5])
                            //{
                            //    continue;
                            //}
                            //else
                            //{
                            //判断前一个港口Sail/Stop哪个时间靠后
                            // TimeSpan ts = Dic[Lis_Keys[i - 1]][1] - Dic[Lis_Keys[i - 2]][1];
                            //判断当前港口Sail/Stop哪个时间靠前
                            // TimeSpan ts1 = Dic[Lis_Keys[i + 1]][0] - Dic[Lis_Keys[i]][0];
                            //定义列表存储船舶在两个港口间的停留时间
                            String Str_Key = "Stop_between_" + Lis_Keys[i];
                            //List<DateTime> Lis_DTtemp = new List<DateTime>();
                            //if (ts.TotalSeconds >= 0 && ts1.TotalSeconds >= 0)
                            //{
                            foreach (string a in Lines)
                            {
                                string[] arrtemp3_0 = a.Split(',');
                                string[] arrtemp3 = arrtemp3_0[0].Split('_');
                                if (arrtemp3_0[0].Contains("Stop_out_Port"))
                                {
                                    if ((Convert.ToDateTime(arrtemp3_0[1]) - Dic[Lis_Keys[i - 1]][1]).TotalSeconds >= 0 && (Convert.ToDateTime(arrtemp3_0[2]) - Dic[Lis_Keys[i]][0]).TotalSeconds <= 0)
                                    {
                                        //要判断字典中是否存在在这两个港口间停留的键值
                                        if (Dic1.ContainsKey(Str_Key))
                                        {
                                            if ((Dic1[Str_Key][0] - Convert.ToDateTime(arrtemp3_0[1])).TotalSeconds >= 0)
                                            {
                                                Dic1[Str_Key][0] = Convert.ToDateTime(arrtemp3_0[1]);
                                            }
                                            if ((Dic1[Str_Key][1] - Convert.ToDateTime(arrtemp3_0[2])).TotalSeconds <= 0)
                                            {
                                                Dic1[Str_Key][1] = Convert.ToDateTime(arrtemp3_0[2]);
                                            }
                                        }
                                        else
                                        {
                                            List<DateTime> List_temp3 = new List<DateTime>();
                                            List_temp3.Add(Convert.ToDateTime(arrtemp3_0[1]));
                                            List_temp3.Add(Convert.ToDateTime(arrtemp3_0[2]));
                                            Dic1.Add(Str_Key, List_temp3);
                                        }
                                    }
                                }
                            }
                            //}
                            //if (ts.TotalSeconds >= 0 && ts1.TotalSeconds < 0)
                            //{
                            //    foreach (string a in Lines)
                            //    {
                            //        string[] arrtemp3_0 = a.Split('|');
                            //        string[] arrtemp3 = arrtemp3_0[0].Split('_');
                            //        if (arrtemp3[0].Contains("Stop_out_Port"))
                            //        {
                            //            if ((Convert.ToDateTime(arrtemp3_0[1]) - Dic[Lis_Keys[i - 1]][1]).TotalSeconds >= 0 && (Convert.ToDateTime(arrtemp3_0[2]) - Dic[Lis_Keys[i+1]][0]).TotalSeconds <= 0)
                            //            {
                            //                //要判断字典中是否存在在这两个港口间停留的键值
                            //                if (Dic1.ContainsKey(Str_Key))
                            //                {
                            //                    if ((Dic1[Str_Key][0] - Convert.ToDateTime(arrtemp3_0[1])).TotalSeconds >= 0)
                            //                    {
                            //                        Dic1[Str_Key][0] = Convert.ToDateTime(arrtemp3_0[1]);
                            //                    }
                            //                    if ((Dic1[Str_Key][1] - Convert.ToDateTime(arrtemp3_0[2])).TotalSeconds <= 0)
                            //                    {
                            //                        Dic1[Str_Key][1] = Convert.ToDateTime(arrtemp3_0[2]);
                            //                    }
                            //                }
                            //                else
                            //                {
                            //                    List<DateTime> List_temp3 = new List<DateTime>();
                            //                    List_temp3.Add(Convert.ToDateTime(arrtemp3_0[1]));
                            //                    List_temp3.Add(Convert.ToDateTime(arrtemp3_0[2]));
                            //                    Dic1.Add(Str_Key, List_temp3);
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                            //if (ts.TotalSeconds < 0 && ts1.TotalSeconds < 0)
                            //{
                            //    foreach (string a in Lines)
                            //    {
                            //        string[] arrtemp3_0 = a.Split('|');
                            //        string[] arrtemp3 = arrtemp3_0[0].Split('_');
                            //        if (arrtemp3[0].Contains("Stop_out_Port"))
                            //        {
                            //            if ((Convert.ToDateTime(arrtemp3_0[1]) - Dic[Lis_Keys[i - 2]][1]).TotalSeconds >= 0 && (Convert.ToDateTime(arrtemp3_0[2]) - Dic[Lis_Keys[i + 1]][0]).TotalSeconds <= 0)
                            //            {
                            //                //要判断字典中是否存在在这两个港口间停留的键值
                            //                if (Dic1.ContainsKey(Str_Key))
                            //                {
                            //                    if ((Dic1[Str_Key][0] - Convert.ToDateTime(arrtemp3_0[1])).TotalSeconds >= 0)
                            //                    {
                            //                        Dic1[Str_Key][0] = Convert.ToDateTime(arrtemp3_0[1]);
                            //                    }
                            //                    if ((Dic1[Str_Key][1] - Convert.ToDateTime(arrtemp3_0[2])).TotalSeconds <= 0)
                            //                    {
                            //                        Dic1[Str_Key][1] = Convert.ToDateTime(arrtemp3_0[2]);
                            //                    }
                            //                }
                            //                else
                            //                {
                            //                    List<DateTime> List_temp3 = new List<DateTime>();
                            //                    List_temp3.Add(Convert.ToDateTime(arrtemp3_0[1]));
                            //                    List_temp3.Add(Convert.ToDateTime(arrtemp3_0[2]));
                            //                    Dic1.Add(Str_Key, List_temp3);
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                            //if (ts.TotalSeconds < 0 && ts1.TotalSeconds >= 0)
                            //{
                            //    foreach (string a in Lines)
                            //    {
                            //        string[] arrtemp3_0 = a.Split('|');
                            //        string[] arrtemp3 = arrtemp3_0[0].Split('_');
                            //        if (arrtemp3[0].Contains("Stop_out_Port"))
                            //        {
                            //            if ((Convert.ToDateTime(arrtemp3_0[1]) - Dic[Lis_Keys[i - 2]][1]).TotalSeconds >= 0 && (Convert.ToDateTime(arrtemp3_0[2]) - Dic[Lis_Keys[i ]][0]).TotalSeconds <= 0)
                            //            {
                            //                //要判断字典中是否存在在这两个港口间停留的键值
                            //                if (Dic1.ContainsKey(Str_Key))
                            //                {
                            //                    if ((Dic1[Str_Key][0] - Convert.ToDateTime(arrtemp3_0[1])).TotalSeconds >= 0)
                            //                    {
                            //                        Dic1[Str_Key][0] = Convert.ToDateTime(arrtemp3_0[1]);
                            //                    }
                            //                    if ((Dic1[Str_Key][1] - Convert.ToDateTime(arrtemp3_0[2])).TotalSeconds <= 0)
                            //                    {
                            //                        Dic1[Str_Key][1] = Convert.ToDateTime(arrtemp3_0[2]);
                            //                    }
                            //                }
                            //                else
                            //                {
                            //                    List<DateTime> List_temp3 = new List<DateTime>();
                            //                    List_temp3.Add(Convert.ToDateTime(arrtemp3_0[1]));
                            //                    List_temp3.Add(Convert.ToDateTime(arrtemp3_0[2]));
                            //                    Dic1.Add(Str_Key, List_temp3);
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                        }
                        //}
                        foreach (KeyValuePair<string, List<DateTime>> KVP1 in Dic)
                        {
                            //streamwriter.WriteLine(KVP1.Key + '|' + KVP1.Value[0] + '|' + KVP1.Value[1]);
                            streamwriter.WriteLine(KVP1.Key + ',' + KVP1.Value[0] + ',' + KVP1.Value[1]);
                        }
                        foreach (KeyValuePair<string, List<DateTime>> KVP1 in Dic1)
                        {
                            //streamwriter.WriteLine(KVP1.Key + '|' + KVP1.Value[0] + '|' + KVP1.Value[1]);
                            streamwriter.WriteLine(KVP1.Key + ',' + KVP1.Value[0] + ',' + KVP1.Value[1]);
                        }
                        streamwriter.Close();
                    }
                }
            }
            MessageBox.Show("OK");
        }
        //统计所有船舶在港口之间的等待时长
        private void button14_Click(object sender, EventArgs e)
        {
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1_同状态合并_时间序列_时间指标统计_LNG");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG\2021_LNG_Waiting_between_Ports.csv";
            DirectoryInfo theFolder = new DirectoryInfo(@"F:\2018-2021LNG处理_LNG基础数据更新\2021_LNG_经停港口_IN唯一港_船舶状态_按季度合并_Motifs_删异_同状态合并_时间序列_时间指标统计\1-4月");
            string targetPath = @"F:\2018-2021LNG处理_LNG基础数据更新\Waiting_between_Ports(2018-2021)_LNG\2021_LNG_Waiting_between_Ports_1-4.csv";
            FileInfo[] dirFile = theFolder.GetFiles();
            StreamWriter streamwriter = new StreamWriter(targetPath);
            foreach (FileInfo nextFile in dirFile)
            {
                string[] arrtemp0 = nextFile.Name.Split('.');
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split('|');
                    if (arrtemp[0].Contains("Stop_between"))
                    {
                        string[] arrtemp1 = arrtemp[0].Split('_');
                        if (arrtemp1.Count() == 11)
                        {
                            if (arrtemp1[4] != arrtemp1[9])
                            {
                                TimeSpan ts = Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1]);
                                streamwriter.WriteLine(arrtemp0[0] + '|' + arrtemp1[4] + '|' + arrtemp1[9] + '|' + arrtemp[1] + '|' + arrtemp[2] + '|' + Convert.ToString(ts.TotalMinutes));
                            }
                        }
                        if (arrtemp1.Count() == 9)
                        {
                            if (arrtemp1[4] != arrtemp1[8])
                            {
                                TimeSpan ts = Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1]);
                                streamwriter.WriteLine(arrtemp0[0] + '|' + arrtemp1[4] + '|' + arrtemp1[8] + '|' + arrtemp[1] + '|' + arrtemp[2] + '|' + Convert.ToString(ts.TotalMinutes));
                            }
                        }
                        if (arrtemp1.Count() == 10 && IsNumeric(arrtemp1[5]) == true)
                        {
                            if (arrtemp1[4] != arrtemp1[9])
                            {
                                TimeSpan ts = Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1]);
                                streamwriter.WriteLine(arrtemp0[0] + '|' + arrtemp1[4] + '|' + arrtemp1[9] + '|' + arrtemp[1] + '|' + arrtemp[2] + '|' + Convert.ToString(ts.TotalMinutes));
                            }
                        }
                        if (arrtemp1.Count() == 10 && IsNumeric(arrtemp1[5]) == false)
                        {
                            if (arrtemp1[4] != arrtemp1[8])
                            {
                                TimeSpan ts = Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1]);
                                streamwriter.WriteLine(arrtemp0[0] + '|' + arrtemp1[4] + '|' + arrtemp1[8] + '|' + arrtemp[1] + '|' + arrtemp[2] + '|' + Convert.ToString(ts.TotalMinutes));
                            }
                        }
                    }
                }
            }
            streamwriter.Close();
            MessageBox.Show("OK");
        }
        //统计船舶在各个港口的停留总时段和停留总时长
        private void button15_Click(object sender, EventArgs e)
        {
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1_同状态合并_时间序列_时间指标统计_LNG");
            DirectoryInfo theFolder = new DirectoryInfo(@"F:\2018-2021LNG处理_LNG基础数据更新\2021_LNG_经停港口_IN唯一港_船舶状态_按季度合并_Motifs_删异_同状态合并_时间序列_时间指标统计\1-4月");
            //定义文件存储船舶在各个港口的停留时段
            //string targetPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_In_Ports(Period)(2018-2021)_LNG\2021_LNG_Waiting_In_Ports(Period).csv";
            string targetPath1 = @"F:\2018-2021LNG处理_LNG基础数据更新\Waiting_In_Ports(Period)(2018-2021)_LNG\2021_LNG_Waiting_In_Ports(Period)_1-4.csv";
            StreamWriter streamwriter1 = new StreamWriter(targetPath1);
            // string targetPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_In_Ports(Duration)(2018-2021)_LNG\2021_LNG_Waiting_In_Ports(Duration).csv";
            string targetPath2 = @"F:\2018-2021LNG处理_LNG基础数据更新\Waiting_In_Ports(Duration)(2018-2021)_LNG\2021_LNG_Waiting_In_Ports(Duration)_1-4.csv";
            StreamWriter streamwriter2 = new StreamWriter(targetPath2);
            //定义字典存储船舶的停留时段
            Dictionary<string, Dictionary<string, List<DateTime>>> Dic_Period = new Dictionary<string, Dictionary<string, List<DateTime>>>();
            //定义字典存储船舶的停留时长
            Dictionary<string, Dictionary<string, double>> Dic_Duration = new Dictionary<string, Dictionary<string, double>>();
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string[] arrtemp0 = nextFile.Name.Split('.');
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split('|');
                    string[] arrtemp1 = arrtemp[0].Split('_');
                    if (arrtemp[0].Contains("Stop") == false || arrtemp[0].Contains("Stop_between"))
                    {
                        continue;
                    }
                    else
                    {
                        if (arrtemp1.Count() == 9)
                        {
                            if (arrtemp1[5] == "Stop")
                            {
                                //统计船舶停留的时段
                                if (Dic_Period.ContainsKey(arrtemp0[0]))
                                {
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dic_Period[arrtemp0[0]].Add(arrtemp1[7] + '_' + arrtemp1[8], Lis_Temp);
                                }
                                else
                                {
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dictionary<string, List<DateTime>> Dic_Temp = new Dictionary<string, List<DateTime>>();
                                    Dic_Temp.Add(arrtemp1[7] + '_' + arrtemp1[8], Lis_Temp);
                                    Dic_Period.Add(arrtemp0[0], Dic_Temp);
                                }
                                //统计船舶停留的时长
                                if (Dic_Duration.ContainsKey(arrtemp0[0]))
                                {
                                    if (Dic_Duration[arrtemp0[0]].ContainsKey(arrtemp1[7]))
                                    {
                                        Dic_Duration[arrtemp0[0]][arrtemp1[7]] += (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes;
                                    }
                                    else
                                    {
                                        //Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                                        Dic_Duration[arrtemp0[0]].Add(arrtemp1[7], (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes);
                                    }
                                }
                                else
                                {
                                    Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                                    Dic_temp1.Add(arrtemp1[7], (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes);
                                    Dic_Duration.Add(arrtemp0[0], Dic_temp1);
                                }
                            }
                        }

                        if (arrtemp1.Count() == 7)
                        {
                            if (arrtemp1[4] == "Stop")
                            {
                                //统计船舶停留的时段
                                if (Dic_Period.ContainsKey(arrtemp0[0]))
                                {
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dic_Period[arrtemp0[0]].Add(arrtemp1[6], Lis_Temp);
                                }
                                else
                                {
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dictionary<string, List<DateTime>> Dic_Temp = new Dictionary<string, List<DateTime>>();
                                    Dic_Temp.Add(arrtemp1[6], Lis_Temp);
                                    Dic_Period.Add(arrtemp0[0], Dic_Temp);
                                }
                                //统计船舶停留的时长
                                if (Dic_Duration.ContainsKey(arrtemp0[0]))
                                {
                                    if (Dic_Duration[arrtemp0[0]].ContainsKey(arrtemp1[6]))
                                    {
                                        Dic_Duration[arrtemp0[0]][arrtemp1[6]] += (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes;
                                    }
                                    else
                                    {
                                        //Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                                        Dic_Duration[arrtemp0[0]].Add(arrtemp1[6], (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes);
                                    }
                                }
                                else
                                {
                                    Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                                    Dic_temp1.Add(arrtemp1[6], (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes);
                                    Dic_Duration.Add(arrtemp0[0], Dic_temp1);
                                }
                            }
                        }

                        if (arrtemp1.Count() == 8)
                        {
                            if (arrtemp1[4] == "Stop")
                            {
                                //统计船舶停留的时段
                                if (Dic_Period.ContainsKey(arrtemp0[0]))
                                {
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dic_Period[arrtemp0[0]].Add(arrtemp1[6] + '_' + arrtemp1[7], Lis_Temp);
                                }
                                else
                                {
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dictionary<string, List<DateTime>> Dic_Temp = new Dictionary<string, List<DateTime>>();
                                    Dic_Temp.Add(arrtemp1[6] + '_' + arrtemp1[7], Lis_Temp);
                                    Dic_Period.Add(arrtemp0[0], Dic_Temp);
                                }
                                //统计船舶停留的时长
                                if (Dic_Duration.ContainsKey(arrtemp0[0]))
                                {
                                    if (Dic_Duration[arrtemp0[0]].ContainsKey(arrtemp1[6]))
                                    {
                                        Dic_Duration[arrtemp0[0]][arrtemp1[6]] += (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes;
                                    }
                                    else
                                    {
                                        //Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                                        Dic_Duration[arrtemp0[0]].Add(arrtemp1[6], (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes);
                                    }
                                }
                                else
                                {
                                    Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                                    Dic_temp1.Add(arrtemp1[6], (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes);
                                    Dic_Duration.Add(arrtemp0[0], Dic_temp1);
                                }
                            }
                        }

                        if (arrtemp1.Count() == 8)
                        {
                            if (arrtemp1[5] == "Stop")
                            {
                                //统计船舶停留的时段
                                if (Dic_Period.ContainsKey(arrtemp0[0]))
                                {
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dic_Period[arrtemp0[0]].Add(arrtemp1[7], Lis_Temp);
                                }
                                else
                                {
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dictionary<string, List<DateTime>> Dic_Temp = new Dictionary<string, List<DateTime>>();
                                    Dic_Temp.Add(arrtemp1[7], Lis_Temp);
                                    Dic_Period.Add(arrtemp0[0], Dic_Temp);
                                }
                                //统计船舶停留的时长
                                if (Dic_Duration.ContainsKey(arrtemp0[0]))
                                {
                                    if (Dic_Duration[arrtemp0[0]].ContainsKey(arrtemp1[7]))
                                    {
                                        Dic_Duration[arrtemp0[0]][arrtemp1[7]] += (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes;
                                    }
                                    else
                                    {
                                        //Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                                        Dic_Duration[arrtemp0[0]].Add(arrtemp1[7], (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes);
                                    }
                                }
                                else
                                {
                                    Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                                    Dic_temp1.Add(arrtemp1[7], (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes);
                                    Dic_Duration.Add(arrtemp0[0], Dic_temp1);
                                }
                            }
                        }

                    }
                }

            }
            // 把字典的内容输出
            foreach (KeyValuePair<string, Dictionary<string, List<DateTime>>> KVP1 in Dic_Period)
            {
                foreach (KeyValuePair<string, List<DateTime>> KVP2 in KVP1.Value)
                {

                    streamwriter1.WriteLine(KVP1.Key + '|' + KVP2.Key + '|' + KVP2.Value[0] + '|' + KVP2.Value[1]);
                }
            }
            streamwriter1.Close();
            foreach (KeyValuePair<string, Dictionary<string, double>> KVP3 in Dic_Duration)
            {
                foreach (KeyValuePair<string, double> KVP4 in KVP3.Value)
                {

                    streamwriter2.WriteLine(KVP3.Key + '|' + KVP4.Key + '|' + KVP4.Value);
                }
            }
            streamwriter2.Close();
            MessageBox.Show("OK");
        }
        //统计船舶在各个港口之间的航行时间
        private void button16_Click(object sender, EventArgs e)
        {
            //2022年9月24日：2020年5-8月、2020年9-12月、2021年5-8月的数据要重新处理

            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG\2021_LNG_Sailing_between_Ports.csv";
            string targetPath = @"F:\2018-2021LNG处理_LNG基础数据更新\Sailing_between_Ports(2018-2021)_LNG\2021_LNG_Sailing_between_Ports_9-12.csv";
            StreamWriter streamwriter = new StreamWriter(targetPath);
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1_同状态合并_时间序列_时间指标统计_LNG");
            DirectoryInfo theFolder = new DirectoryInfo(@"F:\2018-2021LNG处理_LNG基础数据更新\2021_LNG_经停港口_IN唯一港_船舶状态_按季度合并_Motifs_删异_同状态合并_时间序列_时间指标统计\9-12月");
            FileInfo[] dirFile = theFolder.GetFiles();
            //定义字符串，存储前一行

            foreach (FileInfo nextFile in dirFile)
            {
                string[] arrtemp0 = nextFile.Name.Split('.');
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string exLine = "";
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split('|');
                    string[] arrtemp1 = arrtemp[0].Split('_');
                    if (arrtemp[0].Contains("Stop_between"))
                    {
                        continue;
                    }
                    else
                    {
                        if (arrtemp1.Count() == 7)
                        {
                            if (arrtemp1[2] == arrtemp1[6])
                            {
                                exLine = line;
                            }
                            if (arrtemp1[2] != arrtemp1[6])
                            {
                                string[] arrtemp2 = exLine.Split('|');
                                streamwriter.WriteLine(arrtemp0[0] + '|' + arrtemp1[2] + '|' + arrtemp1[6] + '|' + arrtemp2[2] + '|' + arrtemp[1]);
                                exLine = line;
                            }
                        }
                        if (arrtemp1.Count() == 9)
                        {
                            if (arrtemp1[2] == arrtemp1[7])
                            {
                                exLine = line;
                            }
                            if (arrtemp1[2] != arrtemp1[7])
                            {
                                string[] arrtemp2 = exLine.Split('|');
                                streamwriter.WriteLine(arrtemp0[0] + '|' + arrtemp1[2] + '|' + arrtemp1[7] + '|' + arrtemp2[2] + '|' + arrtemp[1]);
                                exLine = line;
                            }
                        }
                        if (arrtemp1.Count() == 8 && IsNumeric(arrtemp1[3]))
                        {
                            if (arrtemp1[2] == arrtemp1[7])
                            {
                                exLine = line;
                            }
                            if (arrtemp1[2] != arrtemp1[7])
                            {
                                string[] arrtemp2 = exLine.Split('|');
                                streamwriter.WriteLine(arrtemp0[0] + '|' + arrtemp1[2] + '|' + arrtemp1[7] + '|' + arrtemp2[2] + '|' + arrtemp[1]);
                                exLine = line;
                            }
                        }
                        if (arrtemp1.Count() == 8 && IsNumeric(arrtemp1[3]) == false)
                        {
                            if (arrtemp1[2] == arrtemp1[6])
                            {
                                exLine = line;
                            }
                            if (arrtemp1[2] != arrtemp1[6])
                            {
                                string[] arrtemp2 = exLine.Split('|');
                                streamwriter.WriteLine(arrtemp0[0] + '|' + arrtemp1[2] + '|' + arrtemp1[6] + '|' + arrtemp2[2] + '|' + arrtemp[1]);
                                exLine = line;
                            }
                        }
                    }
                }
            }
            streamwriter.Close();
            MessageBox.Show("OK");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        //区分LNG和Container
        private void button17_Click(object sender, EventArgs e)
        {
            string inPath1 = @"H:\2018-2021LNG和Container数据\LNG.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            //定义列表存储LNG的IMO
            List<string> LNG_IMO = new List<string>();
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp = line1.Split(',');
                if (arrtemp[0] == "LRIMOShipNo")
                {
                    continue;
                }
                else
                {
                    LNG_IMO.Add(arrtemp[0]);
                }
            }
            //DirectoryInfo theFolder1 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1_同状态合并_时间序列_时间指标统计");
            DirectoryInfo theFolder1 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹");
            FileInfo[] dirFile1 = theFolder1.GetFiles();
            foreach (FileInfo theFile1 in dirFile1)
            {
                string[] arrtemp = theFile1.Name.Split('.');
                if (LNG_IMO.Contains(arrtemp[0]) == true)
                {
                    theFile1.CopyTo("H:\\2018-2021LNG和Container数据_Decoder\\解码\\2021_轨迹_LNG\\" + theFile1.Name);
                }
                else
                {
                    theFile1.CopyTo("H:\\2018-2021LNG和Container数据_Decoder\\解码\\2021_轨迹_Container\\" + theFile1.Name);
                }
            }

            MessageBox.Show("OK");
        }
        //统计船舶在各个港口内的航行时间
        private void button18_Click(object sender, EventArgs e)
        {
            //定义文件存储船舶在各个港口的航行时段
            string targetPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_In_Ports(Period)(2018-2021)_LNG\2021_LNG_Sailing_In_Ports(Period).csv";
            StreamWriter streamwriter1 = new StreamWriter(targetPath1);
            string targetPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_In_Ports(Duration)(2018-2021)_LNG\2021_LNG_Sailing_In_Ports(Duration).csv";
            StreamWriter streamwriter2 = new StreamWriter(targetPath2);
            //定义字典存储船舶的航行时段
            Dictionary<string, Dictionary<string, List<DateTime>>> Dic_Period = new Dictionary<string, Dictionary<string, List<DateTime>>>();
            //定义字典存储船舶的航行时长
            Dictionary<string, Dictionary<string, double>> Dic_Duration = new Dictionary<string, Dictionary<string, double>>();
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1_同状态合并_时间序列_时间指标统计_LNG");
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string[] arrtemp0 = nextFile.Name.Split('.');
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split('|');
                    string[] arrtemp1 = arrtemp[0].Split('_');
                    if (arrtemp[0].Contains("Sail") == false || arrtemp[0].Contains("Stop_between"))
                    {
                        continue;
                    }
                    else
                    {
                        if (arrtemp1.Count() == 9)
                        {
                            if (arrtemp1[5] == "Sail")
                            {
                                //统计船舶航行的时段
                                if (Dic_Period.ContainsKey(arrtemp0[0]))
                                {
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dic_Period[arrtemp0[0]].Add(arrtemp1[7] + '_' + arrtemp1[8], Lis_Temp);
                                }
                                else
                                {
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dictionary<string, List<DateTime>> Dic_Temp = new Dictionary<string, List<DateTime>>();
                                    Dic_Temp.Add(arrtemp1[7] + '_' + arrtemp1[8], Lis_Temp);
                                    Dic_Period.Add(arrtemp0[0], Dic_Temp);
                                }
                                //统计船舶航行的时长
                                if (Dic_Duration.ContainsKey(arrtemp0[0]))
                                {
                                    if (Dic_Duration[arrtemp0[0]].ContainsKey(arrtemp1[7]))
                                    {
                                        Dic_Duration[arrtemp0[0]][arrtemp1[7]] += (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes;
                                    }
                                    else
                                    {
                                        //Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                                        Dic_Duration[arrtemp0[0]].Add(arrtemp1[7], (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes);
                                    }
                                }
                                else
                                {
                                    Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                                    Dic_temp1.Add(arrtemp1[7], (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes);
                                    Dic_Duration.Add(arrtemp0[0], Dic_temp1);
                                }
                            }
                        }

                        if (arrtemp1.Count() == 7)
                        {
                            if (arrtemp1[4] == "Sail")
                            {
                                //统计船舶航行的时段
                                if (Dic_Period.ContainsKey(arrtemp0[0]))
                                {
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dic_Period[arrtemp0[0]].Add(arrtemp1[6], Lis_Temp);
                                }
                                else
                                {
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dictionary<string, List<DateTime>> Dic_Temp = new Dictionary<string, List<DateTime>>();
                                    Dic_Temp.Add(arrtemp1[6], Lis_Temp);
                                    Dic_Period.Add(arrtemp0[0], Dic_Temp);
                                }
                                //统计船舶航行的时长
                                if (Dic_Duration.ContainsKey(arrtemp0[0]))
                                {
                                    if (Dic_Duration[arrtemp0[0]].ContainsKey(arrtemp1[6]))
                                    {
                                        Dic_Duration[arrtemp0[0]][arrtemp1[6]] += (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes;
                                    }
                                    else
                                    {
                                        //Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                                        Dic_Duration[arrtemp0[0]].Add(arrtemp1[6], (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes);
                                    }
                                }
                                else
                                {
                                    Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                                    Dic_temp1.Add(arrtemp1[6], (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes);
                                    Dic_Duration.Add(arrtemp0[0], Dic_temp1);
                                }
                            }
                        }

                        if (arrtemp1.Count() == 8)
                        {
                            if (arrtemp1[4] == "Sail")
                            {
                                //统计船舶航行的时段
                                if (Dic_Period.ContainsKey(arrtemp0[0]))
                                {
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dic_Period[arrtemp0[0]].Add(arrtemp1[6] + '_' + arrtemp1[7], Lis_Temp);
                                }
                                else
                                {
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dictionary<string, List<DateTime>> Dic_Temp = new Dictionary<string, List<DateTime>>();
                                    Dic_Temp.Add(arrtemp1[6] + '_' + arrtemp1[7], Lis_Temp);
                                    Dic_Period.Add(arrtemp0[0], Dic_Temp);
                                }
                                //统计船舶航行的时长
                                if (Dic_Duration.ContainsKey(arrtemp0[0]))
                                {
                                    if (Dic_Duration[arrtemp0[0]].ContainsKey(arrtemp1[6]))
                                    {
                                        Dic_Duration[arrtemp0[0]][arrtemp1[6]] += (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes;
                                    }
                                    else
                                    {
                                        //Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                                        Dic_Duration[arrtemp0[0]].Add(arrtemp1[6], (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes);
                                    }
                                }
                                else
                                {
                                    Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                                    Dic_temp1.Add(arrtemp1[6], (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes);
                                    Dic_Duration.Add(arrtemp0[0], Dic_temp1);
                                }
                            }
                        }

                        if (arrtemp1.Count() == 8)
                        {
                            if (arrtemp1[5] == "Sail")
                            {
                                //统计船舶航行的时段
                                if (Dic_Period.ContainsKey(arrtemp0[0]))
                                {
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dic_Period[arrtemp0[0]].Add(arrtemp1[7], Lis_Temp);
                                }
                                else
                                {
                                    List<DateTime> Lis_Temp = new List<DateTime>();
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[1]));
                                    Lis_Temp.Add(Convert.ToDateTime(arrtemp[2]));
                                    Dictionary<string, List<DateTime>> Dic_Temp = new Dictionary<string, List<DateTime>>();
                                    Dic_Temp.Add(arrtemp1[7], Lis_Temp);
                                    Dic_Period.Add(arrtemp0[0], Dic_Temp);
                                }
                                //统计船舶航行的时长
                                if (Dic_Duration.ContainsKey(arrtemp0[0]))
                                {
                                    if (Dic_Duration[arrtemp0[0]].ContainsKey(arrtemp1[7]))
                                    {
                                        Dic_Duration[arrtemp0[0]][arrtemp1[7]] += (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes;
                                    }
                                    else
                                    {
                                        //Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                                        Dic_Duration[arrtemp0[0]].Add(arrtemp1[7], (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes);
                                    }
                                }
                                else
                                {
                                    Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                                    Dic_temp1.Add(arrtemp1[7], (Convert.ToDateTime(arrtemp[2]) - Convert.ToDateTime(arrtemp[1])).TotalMinutes);
                                    Dic_Duration.Add(arrtemp0[0], Dic_temp1);
                                }
                            }
                        }

                    }
                }


            }
            // 把字典的内容输出
            foreach (KeyValuePair<string, Dictionary<string, List<DateTime>>> KVP1 in Dic_Period)
            {
                foreach (KeyValuePair<string, List<DateTime>> KVP2 in KVP1.Value)
                {

                    streamwriter1.WriteLine(KVP1.Key + '|' + KVP2.Key + '|' + KVP2.Value[0] + '|' + KVP2.Value[1]);
                }
            }
            streamwriter1.Close();
            foreach (KeyValuePair<string, Dictionary<string, double>> KVP3 in Dic_Duration)
            {
                foreach (KeyValuePair<string, double> KVP4 in KVP3.Value)
                {

                    streamwriter2.WriteLine(KVP3.Key + '|' + KVP4.Key + '|' + KVP4.Value);
                }
            }
            streamwriter2.Close();
            MessageBox.Show("OK");
        }
        //删除Sailing between ports 是同一个国家的记录
        // //在执行程序之前需要把2016_LNG_Sailing_between_Ports.csv中的Tab空格替换成单空格
        private void button19_Click(object sender, EventArgs e)
        {
            #region //根据全球港口数据给PortCenter文件中的港口添加CountryCode
            string rePath = @"H:\2018-2021LNG和Container数据_Decoder\解码\全球港口属性列表.csv";
            StreamReader streamreader0 = new StreamReader(rePath);
            //定义字典存储每个港口的CountryCode
            Dictionary<string, string> Dic_Temp = new Dictionary<string, string>();
            string line0 = "";
            //存在不同国家有同名港口的情况
            int i = 0;
            while ((line0 = streamreader0.ReadLine()) != null)
            {
                string[] arrtemp0 = line0.Split(',');
                if (arrtemp0[0] == "World_port_index_number")
                {
                    i++;
                    continue;
                }
                else
                {
                    i++;
                    if (Dic_Temp.ContainsKey(i + " " + arrtemp0[2]) == false)
                    {
                        Dic_Temp.Add(i + " " + arrtemp0[2], arrtemp0[3]);
                    }
                }
            }
            #endregion
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1";
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string destPath = System.IO.Path.Combine(targetPath, nextFile.Name);
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                StreamWriter streamwriter = new StreamWriter(destPath);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split('|');
                    string Cd1 = "";
                    string Cd2 = "";
                    foreach (KeyValuePair<string, string> KVP1 in Dic_Temp)
                    {

                        if (arrtemp[1].Contains('.'))
                        {
                            string a = arrtemp[1].Substring(arrtemp[1].IndexOf(".") + 1);
                            if (arrtemp[1].Contains(KVP1.Key) || KVP1.Key.Contains(a))
                            {
                                Cd1 = KVP1.Value;
                            }
                        }
                        else
                        {
                            // string[] b = KVP1.Key.Split(' ');
                            //if (KVP1.Key.Contains(arrtemp[1]) || arrtemp[1].Contains(b[1]) || arrtemp[1].Contains(KVP1.Key) )
                            string b = arrtemp[1].Substring(arrtemp[1].IndexOf(" ") + 1);
                            if (KVP1.Key.Contains(arrtemp[1]) || arrtemp[1].Contains(KVP1.Key))
                            {
                                Cd1 = KVP1.Value;
                            }
                            else if (arrtemp[1].Contains(KVP1.Key) || KVP1.Key.Contains(b))
                            {
                                Cd1 = KVP1.Value;
                            }
                        }
                        if (arrtemp[2].Contains('.'))
                        {
                            string a = arrtemp[2].Substring(arrtemp[2].IndexOf(".") + 1);
                            if (arrtemp[2].Contains(KVP1.Key) || KVP1.Key.Contains(a))
                            {
                                Cd2 = KVP1.Value;
                            }
                        }
                        else
                        {
                            string b = arrtemp[2].Substring(arrtemp[2].IndexOf(" ") + 1);
                            if (KVP1.Key.Contains(arrtemp[2]) || arrtemp[2].Contains(KVP1.Key))
                            {
                                Cd2 = KVP1.Value;
                            }
                            else if (arrtemp[2].Contains(KVP1.Key) || KVP1.Key.Contains(b))
                            {
                                Cd2 = KVP1.Value;
                            }
                        }
                    }

                    //if (Cd1 == Cd2 && Cd2!="")
                    //{
                    //    streamwriter.WriteLine(line);
                    //}
                    if (Cd1 != Cd2)
                    {
                        streamwriter.WriteLine(line);
                    }
                }
                streamwriter.Close();
            }

            MessageBox.Show("OK");
        }
        //删除Waiting bewteen ports是同一个国家的记录
        //在执行程序之前需要把2016_LNG_Waiting_between_Ports.csv中的Tab空格替换成单空格
        private void button20_Click(object sender, EventArgs e)
        {
            #region //根据全球港口数据给PortCenter文件中的港口添加CountryCode
            string rePath = @"H:\2018-2021LNG和Container数据_Decoder\解码\全球港口属性列表.csv";
            StreamReader streamreader0 = new StreamReader(rePath);
            //定义字典存储每个港口的CountryCode
            Dictionary<string, string> Dic_Temp = new Dictionary<string, string>();
            string line0 = "";
            //存在不同国家有同名港口的情况
            int i = 0;
            while ((line0 = streamreader0.ReadLine()) != null)
            {
                string[] arrtemp0 = line0.Split(',');
                if (arrtemp0[0] == "World_port_index_number")
                {
                    i++;
                    continue;
                }
                else
                {
                    i++;
                    if (Dic_Temp.ContainsKey(i + " " + arrtemp0[2]) == false)
                    {
                        Dic_Temp.Add(i + " " + arrtemp0[2], arrtemp0[3]);
                    }
                }
            }
            #endregion
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1";
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string destPath = System.IO.Path.Combine(targetPath, nextFile.Name);
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                StreamWriter streamwriter = new StreamWriter(destPath);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split('|');
                    string Cd1 = "";
                    string Cd2 = "";
                    foreach (KeyValuePair<string, string> KVP1 in Dic_Temp)
                    {

                        if (arrtemp[1].Contains('.'))
                        {
                            string a = arrtemp[1].Substring(arrtemp[1].IndexOf(".") + 1);
                            if (arrtemp[1].Contains(KVP1.Key) || KVP1.Key.Contains(a))
                            {
                                Cd1 = KVP1.Value;
                            }
                        }
                        else
                        {
                            // string[] b = KVP1.Key.Split(' ');
                            //if (KVP1.Key.Contains(arrtemp[1]) || arrtemp[1].Contains(b[1]) || arrtemp[1].Contains(KVP1.Key) )
                            string b = arrtemp[1].Substring(arrtemp[1].IndexOf(" ") + 1);
                            if (KVP1.Key.Contains(arrtemp[1]) || arrtemp[1].Contains(KVP1.Key))
                            {
                                Cd1 = KVP1.Value;
                            }
                            else if (arrtemp[1].Contains(KVP1.Key) || KVP1.Key.Contains(b))
                            {
                                Cd1 = KVP1.Value;
                            }
                        }
                        if (arrtemp[2].Contains('.'))
                        {
                            string a = arrtemp[2].Substring(arrtemp[2].IndexOf(".") + 1);
                            if (arrtemp[2].Contains(KVP1.Key) || KVP1.Key.Contains(a))
                            {
                                Cd2 = KVP1.Value;
                            }
                        }
                        else
                        {
                            string b = arrtemp[2].Substring(arrtemp[2].IndexOf(" ") + 1);
                            if (KVP1.Key.Contains(arrtemp[2]) || arrtemp[2].Contains(KVP1.Key))
                            {
                                Cd2 = KVP1.Value;
                            }
                            else if (arrtemp[2].Contains(KVP1.Key) || KVP1.Key.Contains(b))
                            {
                                Cd2 = KVP1.Value;
                            }
                        }
                    }
                    //if (Cd1 == Cd2 && Cd2!="")
                    //{
                    //    streamwriter.WriteLine(line);
                    //}
                    if (Cd1 != Cd2)
                    {
                        streamwriter.WriteLine(line);
                    }
                }
                streamwriter.Close();
            }
            MessageBox.Show("OK");
        }
        //四年统计后的数据只保留目的港是属于进口国的
        private void button21_Click(object sender, EventArgs e)
        {
            #region //根据全球港口数据给PortCenter文件中的港口添加CountryCode
            string rePath = @"H:\2018-2021LNG和Container数据_Decoder\解码\全球港口属性列表.csv";
            StreamReader streamreader0 = new StreamReader(rePath);
            //定义字典存储每个港口的CountryCode
            Dictionary<string, string> Dic_Temp = new Dictionary<string, string>();
            string line0 = "";
            //存在不同国家有同名港口的情况
            int i = 0;
            while ((line0 = streamreader0.ReadLine()) != null)
            {
                string[] arrtemp0 = line0.Split(',');
                if (arrtemp0[0] == "World_port_index_number")
                {
                    i++;
                    continue;
                }
                else
                {
                    i++;
                    if (Dic_Temp.ContainsKey(i + " " + arrtemp0[2]) == false)
                    {
                        Dic_Temp.Add(i + " " + arrtemp0[2], arrtemp0[3]);
                    }
                }
            }
            #endregion
            #region----定义列表存储LNG进口国
            List<string> LNG_Imported_Cou = new List<string>();
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\LNG imported countries.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp1 = line1.Split(',');
                if (arrtemp1[0] == "LNG imported countries")
                {
                    continue;
                }
                else
                {
                    LNG_Imported_Cou.Add(arrtemp1[1]);
                }
            }
            #endregion
            //只写出目的港口是LNG进口国的
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast";
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast";
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string destPath = System.IO.Path.Combine(targetPath, nextFile.Name);
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                StreamWriter streamwriter = new StreamWriter(destPath);
                string line2 = "";
                while ((line2 = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp2 = line2.Split('|');
                    string Cd1 = "";
                    foreach (KeyValuePair<string, string> KVP1 in Dic_Temp)
                    {
                        if (arrtemp2[2].Contains('.'))
                        {
                            string a = arrtemp2[2].Substring(arrtemp2[2].IndexOf(".") + 1);
                            if (arrtemp2[2].Contains(KVP1.Key) || KVP1.Key.Contains(a))
                            {
                                Cd1 = KVP1.Value;
                            }
                        }
                        else
                        {
                            // string[] b = KVP1.Key.Split(' ');
                            //if (KVP1.Key.Contains(arrtemp[1]) || arrtemp[1].Contains(b[1]) || arrtemp[1].Contains(KVP1.Key) )
                            string b = arrtemp2[2].Substring(arrtemp2[2].IndexOf(" ") + 1);
                            if (KVP1.Key.Contains(arrtemp2[2]) || arrtemp2[2].Contains(KVP1.Key))
                            {
                                Cd1 = KVP1.Value;
                            }
                            else if (arrtemp2[2].Contains(KVP1.Key) || KVP1.Key.Contains(b))
                            {
                                Cd1 = KVP1.Value;
                            }
                        }
                    }
                    //判断是否包括Cd1
                    if (LNG_Imported_Cou.Contains(Cd1))
                    {
                        streamwriter.WriteLine(line2);
                    }
                }
                streamwriter.Close();
            }
            MessageBox.Show("OK");
        }
        //四年统计后船舶在港口等待的时间只保留目的港是属于进口国的
        private void button22_Click(object sender, EventArgs e)
        {
            #region //根据全球港口数据给PortCenter文件中的港口添加CountryCode
            string rePath = @"H:\2018-2021LNG和Container数据_Decoder\解码\全球港口属性列表.csv";
            StreamReader streamreader0 = new StreamReader(rePath);
            //定义字典存储每个港口的CountryCode
            Dictionary<string, string> Dic_Temp = new Dictionary<string, string>();
            string line0 = "";
            //存在不同国家有同名港口的情况
            int i = 0;
            while ((line0 = streamreader0.ReadLine()) != null)
            {
                string[] arrtemp0 = line0.Split(',');
                if (arrtemp0[0] == "World_port_index_number")
                {
                    i++;
                    continue;
                }
                else
                {
                    i++;
                    if (Dic_Temp.ContainsKey(i + " " + arrtemp0[2]) == false)
                    {
                        Dic_Temp.Add(i + " " + arrtemp0[2], arrtemp0[3]);
                    }
                }
            }
            #endregion
            #region----定义列表存储LNG进口国
            List<string> LNG_Imported_Cou = new List<string>();
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\LNG imported countries.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp1 = line1.Split(',');
                if (arrtemp1[0] == "LNG imported countries")
                {
                    continue;
                }
                else
                {
                    LNG_Imported_Cou.Add(arrtemp1[1]);
                }
            }
            #endregion
            //只写出港口是LNG进口国的           
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_In_Ports(Duration)(2018-2021)_LNG");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_In_Ports(Duration)(2018-2021)_LNG_Noballast";
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_In_Ports(Period)(2018-2021)_LNG");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_In_Ports(Period)(2018-2021)_LNG_Noballast";
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_In_Ports(Period)(2018-2021)_LNG");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_In_Ports(Period)(2018-2021)_LNG_Noballast";
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_In_Ports(Duration)(2018-2021)_LNG");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_In_Ports(Duration)(2018-2021)_LNG_Noballast";
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string destPath = System.IO.Path.Combine(targetPath, nextFile.Name);
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                StreamWriter streamwriter = new StreamWriter(destPath);
                string line2 = "";
                while ((line2 = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp2 = line2.Split('|');
                    string Cd1 = "";
                    foreach (KeyValuePair<string, string> KVP1 in Dic_Temp)
                    {
                        if (arrtemp2[1].Contains('.'))
                        {
                            string a = arrtemp2[1].Substring(arrtemp2[1].IndexOf(".") + 1);
                            if (arrtemp2[1].Contains(KVP1.Key) || KVP1.Key.Contains(a))
                            {
                                Cd1 = KVP1.Value;
                            }
                        }
                        else
                        {
                            // string[] b = KVP1.Key.Split(' ');
                            //if (KVP1.Key.Contains(arrtemp[1]) || arrtemp[1].Contains(b[1]) || arrtemp[1].Contains(KVP1.Key) )
                            string b = arrtemp2[1].Substring(arrtemp2[1].IndexOf(" ") + 1);
                            if (KVP1.Key.Contains(arrtemp2[1]) || arrtemp2[1].Contains(KVP1.Key))
                            {
                                Cd1 = KVP1.Value;
                            }
                            else if (arrtemp2[1].Contains(KVP1.Key) || KVP1.Key.Contains(b))
                            {
                                Cd1 = KVP1.Value;
                            }
                        }
                    }
                    //判断是否包括Cd1
                    if (LNG_Imported_Cou.Contains(Cd1))
                    {
                        streamwriter.WriteLine(line2);
                    }
                }
                streamwriter.Close();
            }
            MessageBox.Show("OK");
        }
        //统计出口国是reexported country的
        private void button23_Click(object sender, EventArgs e)
        {
            #region //根据全球港口数据给PortCenter文件中的港口添加CountryCode
            string rePath = @"H:\2018-2021LNG和Container数据_Decoder\解码\全球港口属性列表.csv";
            StreamReader streamreader0 = new StreamReader(rePath);
            //定义字典存储每个港口的CountryCode
            Dictionary<string, string> Dic_Temp = new Dictionary<string, string>();
            string line0 = "";
            //存在不同国家有同名港口的情况
            int i = 0;
            while ((line0 = streamreader0.ReadLine()) != null)
            {
                string[] arrtemp0 = line0.Split(',');
                if (arrtemp0[0] == "World_port_index_number")
                {
                    i++;
                    continue;
                }
                else
                {
                    i++;
                    if (Dic_Temp.ContainsKey(i + " " + arrtemp0[2]) == false)
                    {
                        Dic_Temp.Add(i + " " + arrtemp0[2], arrtemp0[3]);
                    }
                }
            }
            #endregion

            #region----定义字典存储LNG Reexported国和相应的进口国
            Dictionary<string, List<string>> LNG_ReImported_Cou = new Dictionary<string, List<string>>();
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\LNG Re-exports.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp1 = line1.Split(',');
                if (arrtemp1[0] == "Re-exporting Country")
                {
                    continue;
                }
                else
                {
                    if (LNG_ReImported_Cou.ContainsKey(arrtemp1[2]))
                    {
                        LNG_ReImported_Cou[arrtemp1[2]].Add(arrtemp1[4]);
                    }
                    else
                    {
                        List<string> Lis_temp = new List<string>();
                        Lis_temp.Add(arrtemp1[4]);
                        LNG_ReImported_Cou.Add(arrtemp1[2], Lis_temp);
                    }
                }
            }
            #endregion

            //只写出目的港口是LNG进口国的
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast");
            // DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports_v1_Noballast_添加时长1\Sailing_between_Ports_v1_Noballast_添加时长");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_Reexported";
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast");
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports_v1_Noballast1\Waiting_between_Ports_v1_Noballast");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast_Reexported";
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string[] arrtemp0 = nextFile.Name.Split('.');
                string[] arrtemp00 = arrtemp0[0].Split('_');
                string destPath = System.IO.Path.Combine(targetPath, arrtemp0[0] + "_Reexported.csv");
                StreamWriter streamwriter = new StreamWriter(destPath);
                StreamReader streamreader2 = new StreamReader(nextFile.FullName);
                string line2 = "";
                while ((line2 = streamreader2.ReadLine()) != null)
                {
                    string[] arrtemp2 = line2.Split('|');
                    //分别定义两个字符串，一个存储出口国，另一个存储进口国
                    string Cd1 = "";
                    string Cd2 = "";
                    foreach (KeyValuePair<string, string> KVP1 in Dic_Temp)
                    {
                        if (arrtemp2[1].Contains('.'))
                        {
                            string a = arrtemp2[1].Substring(arrtemp2[1].IndexOf(".") + 1);
                            if (arrtemp2[1].Contains(KVP1.Key) || KVP1.Key.Contains(a))
                            {
                                Cd1 = KVP1.Value;
                            }
                        }
                        else
                        {
                            // string[] b = KVP1.Key.Split(' ');
                            //if (KVP1.Key.Contains(arrtemp[1]) || arrtemp[1].Contains(b[1]) || arrtemp[1].Contains(KVP1.Key) )
                            string b = arrtemp2[1].Substring(arrtemp2[1].IndexOf(" ") + 1);
                            if (KVP1.Key.Contains(arrtemp2[1]) || arrtemp2[1].Contains(KVP1.Key))
                            {
                                Cd1 = KVP1.Value;
                            }
                            else if (arrtemp2[1].Contains(KVP1.Key) || KVP1.Key.Contains(b))
                            {
                                Cd1 = KVP1.Value;
                            }
                        }
                    }
                    foreach (KeyValuePair<string, string> KVP1 in Dic_Temp)
                    {
                        if (arrtemp2[2].Contains('.'))
                        {
                            string a = arrtemp2[2].Substring(arrtemp2[2].IndexOf(".") + 1);
                            if (arrtemp2[2].Contains(KVP1.Key) || KVP1.Key.Contains(a))
                            {
                                Cd2 = KVP1.Value;
                            }
                        }
                        else
                        {
                            // string[] b = KVP1.Key.Split(' ');
                            //if (KVP1.Key.Contains(arrtemp[1]) || arrtemp[1].Contains(b[1]) || arrtemp[1].Contains(KVP1.Key) )
                            string b = arrtemp2[2].Substring(arrtemp2[2].IndexOf(" ") + 1);
                            if (KVP1.Key.Contains(arrtemp2[2]) || arrtemp2[2].Contains(KVP1.Key))
                            {
                                Cd2 = KVP1.Value;
                            }
                            else if (arrtemp2[2].Contains(KVP1.Key) || KVP1.Key.Contains(b))
                            {
                                Cd2 = KVP1.Value;
                            }
                        }
                    }
                    //判断是否包括Cd1
                    if (LNG_ReImported_Cou.Keys.Contains(Cd1))
                    {
                        if (LNG_ReImported_Cou[Cd1].Contains(Cd2))
                        {
                            streamwriter.WriteLine(arrtemp00[0] + '|' + line2);
                        }
                    }
                }
                streamwriter.Close();
            }
            MessageBox.Show("OK");
        }
        //根据四年的Nonballast和Reimported文件，统计没有reexported的航线
        private void button24_Click(object sender, EventArgs e)
        {
            #region----定义字典存储每年的reexported
            Dictionary<string, List<string>> Dic_Reexp = new Dictionary<string, List<string>>();
            DirectoryInfo theFolder1 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_Reexported1");
            // DirectoryInfo theFolder1 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast_Reexported1");
            FileInfo[] dirFile1 = theFolder1.GetFiles();
            foreach (FileInfo nextFile1 in dirFile1)
            {
                string[] arrtemp1 = nextFile1.Name.Split('_');
                StreamReader streamreader = new StreamReader(nextFile1.FullName);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    if (Dic_Reexp.ContainsKey(arrtemp1[0]))
                    {
                        Dic_Reexp[arrtemp1[0]].Add(line);
                    }
                    else
                    {
                        List<string> Lis_temp = new List<string>();
                        Lis_temp.Add(line);
                        Dic_Reexp.Add(arrtemp1[0], Lis_temp);
                    }
                }
            }
            #endregion
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported";
            //DirectoryInfo theFolder2 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast");
            DirectoryInfo theFolder2 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports_v1_Noballast_添加时长1\Sailing_between_Ports_v1_Noballast_添加时长");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported";
            //DirectoryInfo theFolder2 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast");
            // DirectoryInfo theFolder2 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports_v1_Noballast1\Waiting_between_Ports_v1_Noballast");
            FileInfo[] dirFile2 = theFolder2.GetFiles();
            foreach (FileInfo nextFile in dirFile2)
            {
                string destPath = System.IO.Path.Combine(targetPath, nextFile.Name);
                StreamWriter streamwriter = new StreamWriter(destPath);
                string[] arrtemp2 = nextFile.Name.Split('_');
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line2 = "";
                while ((line2 = streamreader.ReadLine()) != null)
                {
                    int i = 0;
                    foreach (KeyValuePair<string, List<string>> KVP1 in Dic_Reexp)
                    {
                        if (KVP1.Key == arrtemp2[0] && KVP1.Value.Contains(line2))
                        {
                            i++;
                        }
                    }
                    if (i == 0)
                    {
                        streamwriter.WriteLine(line2);
                    }
                }
                streamwriter.Close();
            }
            MessageBox.Show("OK");
        }
        //将NoBallast中的NonReexported分为三类
        private void button25_Click(object sender, EventArgs e)
        {
            //根据rexexport的结果，将Nonexported的港口间的运输分为三类。
            //一类是出口港和进口港都不在reexported里面的，说明这类是没有再出口的
            //第二类是目的港是reexported的出口港，说明这类港口有再出口
            //第三类是目的港是reexported的进口港，说明这类港口的进口包括再出口的
            #region-----根据reexported对Nonreexported进行分类
            //定义字典存储每年的再出口的出发港口
            Dictionary<string, List<string>> Dic_PrePort = new Dictionary<string, List<string>>();
            //定义字典存储每年的再出口的目的港口
            Dictionary<string, List<string>> Dic_NxtPort = new Dictionary<string, List<string>>();
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_Reexported1\2017_LNG_Sailing_between_Ports_v1_Reexported.csv";
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast_Reexported1\2014_LNG_Waiting_between_Ports_v1_Reexported.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp1 = line1.Split('|');
                //if (Dic_PrePort.ContainsKey("2018"))
                //{
                //    Dic_PrePort["2018"].Add(arrtemp1[1]);
                //}
                //else
                //{
                //    List<string> Lis_Temp = new List<string>();
                //    Lis_Temp.Add(arrtemp1[1]);
                //    Dic_PrePort.Add("2018", Lis_Temp);
                //}
                //if (Dic_NxtPort.ContainsKey("2018"))
                //{
                //    Dic_NxtPort["2018"].Add(arrtemp1[2]);
                //}
                //else
                //{
                //    List<string> Lis_Temp = new List<string>();
                //    Lis_Temp.Add(arrtemp1[2]);
                //    Dic_NxtPort.Add("2018", Lis_Temp);
                //}

                if (Dic_PrePort.ContainsKey("2014"))
                {
                    Dic_PrePort["2014"].Add(arrtemp1[2]);
                }
                else
                {
                    List<string> Lis_Temp = new List<string>();
                    Lis_Temp.Add(arrtemp1[2]);
                    Dic_PrePort.Add("2014", Lis_Temp);
                }
                if (Dic_NxtPort.ContainsKey("2014"))
                {
                    Dic_NxtPort["2014"].Add(arrtemp1[3]);
                }
                else
                {
                    List<string> Lis_Temp = new List<string>();
                    Lis_Temp.Add(arrtemp1[3]);
                    Dic_NxtPort.Add("2014", Lis_Temp);
                }
            }
            #endregion
            //定义三个字典分别存储这三类
            Dictionary<string, List<string>> Dic_WithoutReexported = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> Dic_ProvideReexported = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> Dic_ReceiveReexported = new Dictionary<string, List<string>>();
            //string inPath= @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported\2017_LNG_Sailing_between_Ports_v1.csv";
            string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported\2014_LNG_Waiting_between_Ports_v1.csv";
            StreamReader streamreader2 = new StreamReader(inPath);
            string[] arrtemp2_1 = "2014_LNG_Waiting_between_Ports.csv".Split('_');
            string line2 = "";
            while ((line2 = streamreader2.ReadLine()) != null)
            {
                string[] arrtemp2_2 = line2.Split('|');
                if (Dic_PrePort[arrtemp2_1[0]].Contains(arrtemp2_2[1]) == false && Dic_NxtPort[arrtemp2_1[0]].Contains(arrtemp2_2[2]) == false)
                {
                    if (Dic_WithoutReexported.ContainsKey(arrtemp2_1[0]))
                    {
                        Dic_WithoutReexported[arrtemp2_1[0]].Add(line2);
                    }
                    else
                    {
                        List<string> Lis_temp2 = new List<string>();
                        Lis_temp2.Add(line2);
                        Dic_WithoutReexported.Add(arrtemp2_1[0], Lis_temp2);
                    }
                }

                //再出口出发港口是目前的目的港
                if (Dic_PrePort[arrtemp2_1[0]].Contains(arrtemp2_2[2]) == true && Dic_NxtPort[arrtemp2_1[0]].Contains(arrtemp2_2[1]) == false)
                {
                    if (Dic_ProvideReexported.ContainsKey(arrtemp2_1[0]))
                    {
                        Dic_ProvideReexported[arrtemp2_1[0]].Add(line2);
                    }
                    else
                    {
                        List<string> Lis_temp2 = new List<string>();
                        Lis_temp2.Add(line2);
                        Dic_ProvideReexported.Add(arrtemp2_1[0], Lis_temp2);
                    }
                }
                //再出口目的港口是目前的目的港
                if (Dic_PrePort[arrtemp2_1[0]].Contains(arrtemp2_2[1]) == false && Dic_NxtPort[arrtemp2_1[0]].Contains(arrtemp2_2[2]) == true)
                {
                    if (Dic_ReceiveReexported.ContainsKey(arrtemp2_1[0]))
                    {
                        Dic_ReceiveReexported[arrtemp2_1[0]].Add(line2);
                    }
                    else
                    {
                        List<string> Lis_temp2 = new List<string>();
                        Lis_temp2.Add(line2);
                        Dic_ReceiveReexported.Add(arrtemp2_1[0], Lis_temp2);
                    }
                }
            }

            //写出三个字典的内容

            //string outPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2017\WithoutReexported.csv";
            string outPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2014\WithoutReexported.csv";
            StreamWriter streamwriter1 = new StreamWriter(outPath1);

            //string outPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2017\ProvideReexported.csv";
            string outPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2014\ProvideReexported.csv";
            StreamWriter streamwriter2 = new StreamWriter(outPath2);

            //string outPath3 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2017\ReceiveReexported.csv";
            string outPath3 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2014\ReceiveReexported.csv";
            StreamWriter streamwriter3 = new StreamWriter(outPath3);
            foreach (KeyValuePair<string, List<string>> KVP1 in Dic_WithoutReexported)
            {
                foreach (string a in KVP1.Value)
                {
                    streamwriter1.WriteLine(KVP1.Key + '|' + a);
                }
            }
            foreach (KeyValuePair<string, List<string>> KVP1 in Dic_ProvideReexported)
            {
                foreach (string a in KVP1.Value)
                {
                    streamwriter2.WriteLine(KVP1.Key + '|' + a);
                }
            }
            foreach (KeyValuePair<string, List<string>> KVP1 in Dic_ReceiveReexported)
            {
                foreach (string a in KVP1.Value)
                {
                    streamwriter3.WriteLine(KVP1.Key + '|' + a);
                }
            }
            streamwriter1.Close();
            streamwriter2.Close();
            streamwriter3.Close();
            MessageBox.Show("OK");
        }
        //不同吨位等级的船舶在每条航线上的平均航行或等待时间
        private void button26_Click(object sender, EventArgs e)
        {
            #region----根据船名库，统计船舶的吨位
            string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\原始LNG船舶静态数据.csv";
            StreamReader streamreader = new StreamReader(inPath);
            string line = "";
            //定义字典存储LNG船舶的MMSI，及其吨位
            Dictionary<string, double> Dic_LNG = new Dictionary<string, double>();
            while ((line = streamreader.ReadLine()) != null)
            {
                string[] arrtemp = line.Split(',');
                //if (arrtemp[5] == "" || arrtemp[5].Contains("IMO"))
                //{
                //    continue;
                //}
                //else
                //{
                //    if (arrtemp[3] != "" && arrtemp[5] != "0")
                //    {
                //        Dic_LNG.Add(arrtemp[5], Convert.ToDouble(arrtemp[3]));
                //    }
                //    //用总吨位的百分之八十估算船舶的载重吨
                //    else if (arrtemp[10] != "" && arrtemp[5] != "0")
                //    {
                //        double dou_temp = Convert.ToDouble(arrtemp[10]) * 4 / 5;
                //        Dic_LNG.Add(arrtemp[5], dou_temp);
                //    }

                //}

                if (arrtemp[8] == "" || arrtemp[8].Contains("MMSI"))
                {
                    continue;
                }
                else
                {
                    if (arrtemp[3] != "" && arrtemp[8] != "0")
                    {
                        Dic_LNG.Add(arrtemp[8], Convert.ToDouble(arrtemp[3]));
                    }
                    //用总吨位的百分之八十估算船舶的载重吨
                    else if (arrtemp[10] != "" && arrtemp[8] != "0")
                    {
                        double dou_temp = Convert.ToDouble(arrtemp[10]) * 4 / 5;
                        Dic_LNG.Add(arrtemp[8], dou_temp);
                    }

                }
            }
            #endregion
            //对于没有再出口情况的LNG运输
            //定义字典存储航线不同吨位等级的船舶平均航行时间
            Dictionary<string, Dictionary<string, List<double>>> Dic_temp = new Dictionary<string, Dictionary<string, List<double>>>();
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_Reexported\2014_LNG_Sailing_between_Ports_Reexported.csv";
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast_Reexported\2014_LNG_Waiting_between_Ports_Reexported.csv";
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2014\WithoutReexported.csv";
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2014\WithoutReexported.csv";
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2014\ProvideReexported.csv";
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2014\ProvideReexported.csv";
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2014\ReceiveReexported.csv";
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2014\ReceiveReexported.csv";
            //输出每条航线上不同吨位等级的船舶平均航行时间
            //string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图\WithReexport\2014\AveSaiTime_DeadWeight_Reexport.csv";
            //string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图\WithReexport\2014\AveWatTime_DeadWeight_Reexport.csv";
            //string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图\WithoutReexport\2014\AveSaiTime_DeadWeight.csv";
            //string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图\WithoutReexport\2014\AveWatTime_DeadWeight.csv";
            //string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图\WithReexport\2014\AveSaiTime_DeadWeight_ProvideReexported.csv";
            // string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图\WithReexport\2014\AveWatTime_DeadWeight_ProvideReexported.csv";
            //string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图\WithReexport\2014\AveSaiTime_DeadWeight_ReceiveReexported.csv";
            string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图\WithReexport\2014\AveWatTime_DeadWeight_ReceiveReexported.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            StreamWriter streamwriter = new StreamWriter(outPath);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp1 = line1.Split('|');
                //定义变量存储船舶的吨位
                double dw_temp = 0.0;
                foreach (KeyValuePair<string, double> KVP1 in Dic_LNG)
                {
                    if (KVP1.Key == arrtemp1[1])
                    {
                        dw_temp = KVP1.Value;
                    }
                }
                if (Dic_temp.ContainsKey(arrtemp1[2] + '|' + arrtemp1[3]))
                {
                    #region
                    if (dw_temp <= 10000)
                    {
                        if (Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].ContainsKey("10000"))
                        {
                            TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                            Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]]["10000"].Add(ts.TotalMinutes);
                        }
                        else
                        {
                            TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                            List<double> Lis_temp = new List<double>();
                            Lis_temp.Add(ts.TotalMinutes);
                            Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].Add("10000", Lis_temp);
                        }
                    }
                    if (dw_temp > 10000 && dw_temp <= 55000)
                    {
                        if (Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].ContainsKey("55000"))
                        {
                            TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                            Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]]["55000"].Add(ts.TotalMinutes);
                        }
                        else
                        {
                            TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                            List<double> Lis_temp = new List<double>();
                            Lis_temp.Add(ts.TotalMinutes);
                            Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].Add("55000", Lis_temp);
                        }
                    }
                    if (dw_temp > 55000 && dw_temp <= 80000)
                    {
                        if (Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].ContainsKey("80000"))
                        {
                            TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                            Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]]["80000"].Add(ts.TotalMinutes);
                        }
                        else
                        {
                            TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                            List<double> Lis_temp = new List<double>();
                            Lis_temp.Add(ts.TotalMinutes);
                            Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].Add("80000", Lis_temp);
                        }
                    }
                    if (dw_temp > 80000 && dw_temp <= 120000)
                    {
                        if (Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].ContainsKey("120000"))
                        {
                            TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                            Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]]["120000"].Add(ts.TotalMinutes);
                        }
                        else
                        {
                            TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                            List<double> Lis_temp = new List<double>();
                            Lis_temp.Add(ts.TotalMinutes);
                            Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].Add("120000", Lis_temp);
                        }
                    }
                    if (dw_temp > 120000 && dw_temp <= 200000)
                    {
                        if (Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].ContainsKey("200000"))
                        {
                            TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                            Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]]["200000"].Add(ts.TotalMinutes);
                        }
                        else
                        {
                            TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                            List<double> Lis_temp = new List<double>();
                            Lis_temp.Add(ts.TotalMinutes);
                            Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].Add("200000", Lis_temp);
                        }
                    }
                    #endregion
                }
                else
                {
                    #region
                    if (dw_temp <= 10000)
                    {
                        TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                        List<double> Lis_temp = new List<double>();
                        Lis_temp.Add(ts.TotalMinutes);
                        Dictionary<string, List<double>> Dic_temmp = new Dictionary<string, List<double>>();
                        Dic_temmp.Add("10000", Lis_temp);
                        Dic_temp.Add(arrtemp1[2] + '|' + arrtemp1[3], Dic_temmp);
                    }
                    if (dw_temp > 10000 && dw_temp <= 55000)
                    {
                        TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                        List<double> Lis_temp = new List<double>();
                        Lis_temp.Add(ts.TotalMinutes);
                        Dictionary<string, List<double>> Dic_temmp = new Dictionary<string, List<double>>();
                        Dic_temmp.Add("55000", Lis_temp);
                        Dic_temp.Add(arrtemp1[2] + '|' + arrtemp1[3], Dic_temmp);
                    }
                    if (dw_temp > 55000 && dw_temp <= 80000)
                    {
                        TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                        List<double> Lis_temp = new List<double>();
                        Lis_temp.Add(ts.TotalMinutes);
                        Dictionary<string, List<double>> Dic_temmp = new Dictionary<string, List<double>>();
                        Dic_temmp.Add("80000", Lis_temp);
                        Dic_temp.Add(arrtemp1[2] + '|' + arrtemp1[3], Dic_temmp);
                    }
                    if (dw_temp > 80000 && dw_temp <= 120000)
                    {
                        TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                        List<double> Lis_temp = new List<double>();
                        Lis_temp.Add(ts.TotalMinutes);
                        Dictionary<string, List<double>> Dic_temmp = new Dictionary<string, List<double>>();
                        Dic_temmp.Add("120000", Lis_temp);
                        Dic_temp.Add(arrtemp1[2] + '|' + arrtemp1[3], Dic_temmp);
                    }
                    if (dw_temp > 120000 && dw_temp <= 200000)
                    {
                        TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                        List<double> Lis_temp = new List<double>();
                        Lis_temp.Add(ts.TotalMinutes);
                        Dictionary<string, List<double>> Dic_temmp = new Dictionary<string, List<double>>();
                        Dic_temmp.Add("200000", Lis_temp);
                        Dic_temp.Add(arrtemp1[2] + '|' + arrtemp1[3], Dic_temmp);
                    }
                    #endregion
                }
            }
            foreach (KeyValuePair<string, Dictionary<string, List<double>>> KVP1 in Dic_temp)
            {
                foreach (KeyValuePair<string, List<double>> KVP2 in KVP1.Value)
                {
                    double AVe = KVP2.Value.Average();
                    streamwriter.WriteLine(KVP1.Key + '|' + KVP2.Key + '|' + Convert.ToString(AVe));
                }
            }
            streamwriter.Close();
            MessageBox.Show("OK");
        }
        //统计每条航线上每年的吞吐量需求
        private void button27_Click(object sender, EventArgs e)
        {
            #region----根据船名库，统计船舶的吨位
            string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\原始LNG船舶静态数据.csv";
            StreamReader streamreader = new StreamReader(inPath);
            string line = "";
            //定义字典存储LNG船舶的MMSI，及其吨位
            Dictionary<string, double> Dic_LNG = new Dictionary<string, double>();
            while ((line = streamreader.ReadLine()) != null)
            {
                string[] arrtemp = line.Split(',');
                if (arrtemp[5] == "" || arrtemp[5].Contains("IMO"))
                {
                    continue;
                }
                else
                {
                    if (arrtemp[3] != "" && arrtemp[5] != "0")
                    {
                        Dic_LNG.Add(arrtemp[5], Convert.ToDouble(arrtemp[3]));
                    }
                    //用总吨位的百分之八十估算船舶的载重吨
                    else if (arrtemp[10] != "" && arrtemp[5] != "0")
                    {
                        double dou_temp = Convert.ToDouble(arrtemp[10]) * 4 / 5;
                        Dic_LNG.Add(arrtemp[5], dou_temp);
                    }

                }
            }
            #endregion
            //定义字典存储每条航线上每年的吞吐量
            Dictionary<string, Dictionary<string, double>> Dic_Temp = new Dictionary<string, Dictionary<string, double>>();
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2018\WithoutReexported.csv";
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2018\ProvideReexported.csv";
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2021\ReceiveReexported.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            //写出求解的内容
            string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图\WithoutReexport\2018\ShipLane_CapacityDemand.csv";
            // string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图\WithReexport\2018\ShipLane_CapacityDemand_ProvideReexported.csv";
            //string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图\WithReexport\2021\ShipLane_CapacityDemand_ReceiveReexported.csv";
            StreamWriter streamwriter = new StreamWriter(outPath);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp1 = line1.Split('|');
                //定义字符存储船舶的吨位
                double Wei = 0.0;
                foreach (KeyValuePair<string, double> KVP1 in Dic_LNG)
                {
                    if (KVP1.Key == arrtemp1[1])
                    {
                        Wei = KVP1.Value;
                    }
                }
                if (Dic_Temp.ContainsKey(arrtemp1[2] + '|' + arrtemp1[3]))
                {
                    if (Dic_Temp[arrtemp1[2] + '|' + arrtemp1[3]].ContainsKey(arrtemp1[0]))
                    {
                        Dic_Temp[arrtemp1[2] + '|' + arrtemp1[3]][arrtemp1[0]] += Wei;
                    }
                    else
                    {
                        Dic_Temp[arrtemp1[2] + '|' + arrtemp1[3]].Add(arrtemp1[0], Wei);
                    }
                }
                else
                {
                    Dictionary<string, double> Dic_Temmp = new Dictionary<string, double>();
                    Dic_Temmp.Add(arrtemp1[0], Wei);
                    Dic_Temp.Add(arrtemp1[2] + '|' + arrtemp1[3], Dic_Temmp);
                }
            }
            foreach (KeyValuePair<string, Dictionary<string, double>> KVP1 in Dic_Temp)
            {
                foreach (KeyValuePair<string, double> KVP2 in KVP1.Value)
                {
                    streamwriter.WriteLine(KVP1.Key + '|' + KVP2.Key + '|' + KVP2.Value);
                }
            }
            streamwriter.Close();
            MessageBox.Show("OK");
        }
        //由于航线上可能存在同一条船跑了好几趟的情况，从而造成把所有lng船舶分配还不能满足要求的情况
        private void button28_Click(object sender, EventArgs e)
        {
            #region----根据船名库，统计船舶的吨位
            string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\原始LNG船舶静态数据.csv";
            StreamReader streamreader = new StreamReader(inPath);
            string line = "";
            //定义字典存储LNG船舶的MMSI，及其吨位
            Dictionary<string, double> Dic_LNG = new Dictionary<string, double>();
            while ((line = streamreader.ReadLine()) != null)
            {
                string[] arrtemp = line.Split(',');
                if (arrtemp[5] == "" || arrtemp[5].Contains("IMO"))
                {
                    continue;
                }
                else
                {
                    if (arrtemp[3] != "" && arrtemp[5] != "0")
                    {
                        Dic_LNG.Add(arrtemp[5], Convert.ToDouble(arrtemp[3]));
                    }
                    //用总吨位的百分之八十估算船舶的载重吨
                    else if (arrtemp[10] != "" && arrtemp[5] != "0")
                    {
                        double dou_temp = Convert.ToDouble(arrtemp[10]) * 4 / 5;
                        Dic_LNG.Add(arrtemp[5], dou_temp);
                    }

                }
            }
            #endregion
            //定义字典存储每条航线上每年的吞吐量
            Dictionary<string, Dictionary<string, double>> Dic_Temp = new Dictionary<string, Dictionary<string, double>>();
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2021\WithoutReexported.csv";
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2021\ProvideReexported.csv";
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification\2021\ReceiveReexported.csv";

            StreamReader streamreader1 = new StreamReader(inPath1);
            //写出求解的内容
            //string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图\WithoutReexport\2021\ShipLane_CapacityDemand_NonRepetition.csv";
            // string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图\WithReexport\2021\ShipLane_CapacityDemand_ProvideReexported_NonRepetition.csv";
            string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图\WithReexport\2021\ShipLane_CapacityDemand_ReceiveReexported_NonRepetition.csv";
            StreamWriter streamwriter = new StreamWriter(outPath);
            string line1 = "";
            //定义列表存储每条航线上每年航行的船舶的MMSI，如果有重复的MMSI则跳过
            Dictionary<string, Dictionary<string, List<string>>> Dic_Lane_MMSI = new Dictionary<string, Dictionary<string, List<string>>>();
            while ((line1 = streamreader1.ReadLine()) != null)
            {

                string[] arrtemp_1 = line1.Split('|');
                if (Dic_Lane_MMSI.ContainsKey(arrtemp_1[2] + '|' + arrtemp_1[3]))
                {
                    if (Dic_Lane_MMSI[arrtemp_1[2] + '|' + arrtemp_1[3]].ContainsKey(arrtemp_1[0]))
                    {
                        //保证每条船的MMSI只被加入一次
                        if (Dic_Lane_MMSI[arrtemp_1[2] + '|' + arrtemp_1[3]][arrtemp_1[0]].Contains(arrtemp_1[1]) == false)
                        {
                            Dic_Lane_MMSI[arrtemp_1[2] + '|' + arrtemp_1[3]][arrtemp_1[0]].Add(arrtemp_1[1]);
                        }
                    }
                    else
                    {
                        List<string> Lis_temp = new List<string>();
                        Lis_temp.Add(arrtemp_1[1]);
                        Dic_Lane_MMSI[arrtemp_1[2] + '|' + arrtemp_1[3]].Add(arrtemp_1[0], Lis_temp);

                    }
                }
                else
                {
                    List<string> Lis_temp = new List<string>();
                    Lis_temp.Add(arrtemp_1[1]);
                    Dictionary<string, List<string>> Dic_temp1 = new Dictionary<string, List<string>>();
                    Dic_temp1.Add(arrtemp_1[0], Lis_temp);
                    Dic_Lane_MMSI.Add(arrtemp_1[2] + '|' + arrtemp_1[3], Dic_temp1);
                }

            }
            #region
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> KVP11 in Dic_Lane_MMSI)
            {
                foreach (KeyValuePair<string, List<string>> KVP12 in KVP11.Value)
                {
                    foreach (string a in KVP12.Value)
                    {
                        //定义字符存储船舶的吨位
                        double Wei = 0.0;
                        foreach (KeyValuePair<string, double> KVP1 in Dic_LNG)
                        {
                            if (KVP1.Key == a)
                            {
                                Wei = KVP1.Value;
                            }
                        }
                        if (Dic_Temp.ContainsKey(KVP11.Key))
                        {
                            if (Dic_Temp[KVP11.Key].ContainsKey(KVP12.Key))
                            {
                                Dic_Temp[KVP11.Key][KVP12.Key] += Wei;
                            }
                            else
                            {
                                Dic_Temp[KVP11.Key].Add(KVP12.Key, Wei);
                            }
                        }
                        else
                        {
                            Dictionary<string, double> Dic_Temmp = new Dictionary<string, double>();
                            Dic_Temmp.Add(KVP12.Key, Wei);
                            Dic_Temp.Add(KVP11.Key, Dic_Temmp);
                        }
                    }
                }
                #endregion
            }
            foreach (KeyValuePair<string, Dictionary<string, double>> KVP1 in Dic_Temp)
            {
                foreach (KeyValuePair<string, double> KVP2 in KVP1.Value)
                {
                    streamwriter.WriteLine(KVP1.Key + '|' + KVP2.Key + '|' + KVP2.Value);
                }
            }
            streamwriter.Close();
            MessageBox.Show("OK");
        }
        //不同吨位等级的船舶在每条航线上的平均航行或等待时间(用四年数据计算平均值,除reexported)
        private void button29_Click(object sender, EventArgs e)
        {
            #region----根据船名库，统计船舶的吨位
            string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\原始LNG船舶静态数据.csv";
            StreamReader streamreader = new StreamReader(inPath);
            string line = "";
            //定义字典存储LNG船舶的MMSI，及其吨位
            Dictionary<string, double> Dic_LNG = new Dictionary<string, double>();
            while ((line = streamreader.ReadLine()) != null)
            {
                string[] arrtemp = line.Split(',');
                if (arrtemp[5] == "" || arrtemp[5].Contains("IMO"))
                {
                    continue;
                }
                else
                {
                    if (arrtemp[3] != "" && arrtemp[5] != "0")
                    {
                        Dic_LNG.Add(arrtemp[5], Convert.ToDouble(arrtemp[3]));
                    }
                    //用总吨位的百分之八十估算船舶的载重吨
                    else if (arrtemp[10] != "" && arrtemp[5] != "0")
                    {
                        double dou_temp = Convert.ToDouble(arrtemp[10]) * 4 / 5;
                        Dic_LNG.Add(arrtemp[5], dou_temp);
                    }

                }
            }
            #endregion
            //对于没有再出口情况的LNG运输
            //定义字典存储航线不同吨位等级的船舶平均航行时间
            Dictionary<string, Dictionary<string, List<double>>> Dic_temp = new Dictionary<string, Dictionary<string, List<double>>>();
            //用四年的数据计算平均值
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification");
            DirectoryInfo[] dirFolder = theFolder.GetDirectories();
            string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveSaiTime_DeadWeight.csv";
            StreamWriter streamwriter = new StreamWriter(outPath);
            foreach (DirectoryInfo nextFolder in dirFolder)
            {
                FileInfo[] dirFile = nextFolder.GetFiles();
                foreach (FileInfo nextFile in dirFile)
                {
                    if (nextFile.Name.Contains("WithoutReexported"))
                    {
                        StreamReader streamreader1 = new StreamReader(nextFile.FullName);

                        string line1 = "";
                        while ((line1 = streamreader1.ReadLine()) != null)
                        {
                            string[] arrtemp1 = line1.Split('|');
                            //定义变量存储船舶的吨位
                            double dw_temp = 0.0;
                            foreach (KeyValuePair<string, double> KVP1 in Dic_LNG)
                            {
                                if (KVP1.Key == arrtemp1[1])
                                {
                                    dw_temp = KVP1.Value;
                                }
                            }
                            if (Dic_temp.ContainsKey(arrtemp1[2] + '|' + arrtemp1[3]))
                            {
                                #region
                                if (dw_temp <= 10000)
                                {
                                    if (Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].ContainsKey("10000"))
                                    {
                                        TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                        Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]]["10000"].Add(ts.TotalMinutes);
                                    }
                                    else
                                    {
                                        TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                        List<double> Lis_temp = new List<double>();
                                        Lis_temp.Add(ts.TotalMinutes);
                                        Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].Add("10000", Lis_temp);
                                    }
                                }
                                if (dw_temp > 10000 && dw_temp <= 55000)
                                {
                                    if (Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].ContainsKey("55000"))
                                    {
                                        TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                        Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]]["55000"].Add(ts.TotalMinutes);
                                    }
                                    else
                                    {
                                        TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                        List<double> Lis_temp = new List<double>();
                                        Lis_temp.Add(ts.TotalMinutes);
                                        Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].Add("55000", Lis_temp);
                                    }
                                }
                                if (dw_temp > 55000 && dw_temp <= 80000)
                                {
                                    if (Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].ContainsKey("80000"))
                                    {
                                        TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                        Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]]["80000"].Add(ts.TotalMinutes);
                                    }
                                    else
                                    {
                                        TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                        List<double> Lis_temp = new List<double>();
                                        Lis_temp.Add(ts.TotalMinutes);
                                        Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].Add("80000", Lis_temp);
                                    }
                                }
                                if (dw_temp > 80000 && dw_temp <= 120000)
                                {
                                    if (Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].ContainsKey("120000"))
                                    {
                                        TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                        Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]]["120000"].Add(ts.TotalMinutes);
                                    }
                                    else
                                    {
                                        TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                        List<double> Lis_temp = new List<double>();
                                        Lis_temp.Add(ts.TotalMinutes);
                                        Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].Add("120000", Lis_temp);
                                    }
                                }
                                if (dw_temp > 120000 && dw_temp <= 200000)
                                {
                                    if (Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].ContainsKey("200000"))
                                    {
                                        TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                        Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]]["200000"].Add(ts.TotalMinutes);
                                    }
                                    else
                                    {
                                        TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                        List<double> Lis_temp = new List<double>();
                                        Lis_temp.Add(ts.TotalMinutes);
                                        Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].Add("200000", Lis_temp);
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                #region
                                if (dw_temp <= 10000)
                                {
                                    TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                    List<double> Lis_temp = new List<double>();
                                    Lis_temp.Add(ts.TotalMinutes);
                                    Dictionary<string, List<double>> Dic_temmp = new Dictionary<string, List<double>>();
                                    Dic_temmp.Add("10000", Lis_temp);
                                    Dic_temp.Add(arrtemp1[2] + '|' + arrtemp1[3], Dic_temmp);
                                }
                                if (dw_temp > 10000 && dw_temp <= 55000)
                                {
                                    TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                    List<double> Lis_temp = new List<double>();
                                    Lis_temp.Add(ts.TotalMinutes);
                                    Dictionary<string, List<double>> Dic_temmp = new Dictionary<string, List<double>>();
                                    Dic_temmp.Add("55000", Lis_temp);
                                    Dic_temp.Add(arrtemp1[2] + '|' + arrtemp1[3], Dic_temmp);
                                }
                                if (dw_temp > 55000 && dw_temp <= 80000)
                                {
                                    TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                    List<double> Lis_temp = new List<double>();
                                    Lis_temp.Add(ts.TotalMinutes);
                                    Dictionary<string, List<double>> Dic_temmp = new Dictionary<string, List<double>>();
                                    Dic_temmp.Add("80000", Lis_temp);
                                    Dic_temp.Add(arrtemp1[2] + '|' + arrtemp1[3], Dic_temmp);
                                }
                                if (dw_temp > 80000 && dw_temp <= 120000)
                                {
                                    TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                    List<double> Lis_temp = new List<double>();
                                    Lis_temp.Add(ts.TotalMinutes);
                                    Dictionary<string, List<double>> Dic_temmp = new Dictionary<string, List<double>>();
                                    Dic_temmp.Add("120000", Lis_temp);
                                    Dic_temp.Add(arrtemp1[2] + '|' + arrtemp1[3], Dic_temmp);
                                }
                                if (dw_temp > 120000 && dw_temp <= 200000)
                                {
                                    TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                    List<double> Lis_temp = new List<double>();
                                    Lis_temp.Add(ts.TotalMinutes);
                                    Dictionary<string, List<double>> Dic_temmp = new Dictionary<string, List<double>>();
                                    Dic_temmp.Add("200000", Lis_temp);
                                    Dic_temp.Add(arrtemp1[2] + '|' + arrtemp1[3], Dic_temmp);
                                }
                                #endregion
                            }
                        }
                    }
                }
            }
            foreach (KeyValuePair<string, Dictionary<string, List<double>>> KVP1 in Dic_temp)
            {
                foreach (KeyValuePair<string, List<double>> KVP2 in KVP1.Value)
                {
                    double AVe = KVP2.Value.Average();
                    streamwriter.WriteLine(KVP1.Key + '|' + KVP2.Key + '|' + Convert.ToString(AVe));
                }
            }
            streamwriter.Close();
            MessageBox.Show("OK");
        }
        //不同吨位等级的船舶在每条航线上的平均航行或等待时间(用四年数据计算, 只针对reexported)
        private void button30_Click(object sender, EventArgs e)
        {
            #region----根据船名库，统计船舶的吨位
            string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\原始LNG船舶静态数据.csv";
            StreamReader streamreader = new StreamReader(inPath);
            string line = "";
            //定义字典存储LNG船舶的MMSI，及其吨位
            Dictionary<string, double> Dic_LNG = new Dictionary<string, double>();
            while ((line = streamreader.ReadLine()) != null)
            {
                string[] arrtemp = line.Split(',');
                if (arrtemp[5] == "" || arrtemp[5].Contains("IMO"))
                {
                    continue;
                }
                else
                {
                    if (arrtemp[3] != "" && arrtemp[5] != "0")
                    {
                        Dic_LNG.Add(arrtemp[5], Convert.ToDouble(arrtemp[3]));
                    }
                    //用总吨位的百分之八十估算船舶的载重吨
                    else if (arrtemp[10] != "" && arrtemp[5] != "0")
                    {
                        double dou_temp = Convert.ToDouble(arrtemp[10]) * 4 / 5;
                        Dic_LNG.Add(arrtemp[5], dou_temp);
                    }

                }
            }
            #endregion
            //对于没有再出口情况的LNG运输
            //定义字典存储航线不同吨位等级的船舶平均航行时间
            Dictionary<string, Dictionary<string, List<double>>> Dic_temp = new Dictionary<string, Dictionary<string, List<double>>>();
            //用四年的数据计算平均值
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast_Reexported");
            string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveWaiTime_DeadWeight_Reexport.csv";
            StreamWriter streamwriter = new StreamWriter(outPath);
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                StreamReader streamreader1 = new StreamReader(nextFile.FullName);
                string line1 = "";
                while ((line1 = streamreader1.ReadLine()) != null)
                {
                    string[] arrtemp1 = line1.Split('|');
                    //定义变量存储船舶的吨位
                    double dw_temp = 0.0;
                    foreach (KeyValuePair<string, double> KVP1 in Dic_LNG)
                    {
                        if (KVP1.Key == arrtemp1[1])
                        {
                            dw_temp = KVP1.Value;
                        }
                    }
                    if (Dic_temp.ContainsKey(arrtemp1[2] + '|' + arrtemp1[3]))
                    {
                        #region
                        if (dw_temp <= 10000)
                        {
                            if (Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].ContainsKey("10000"))
                            {
                                TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]]["10000"].Add(ts.TotalMinutes);
                            }
                            else
                            {
                                TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                List<double> Lis_temp = new List<double>();
                                Lis_temp.Add(ts.TotalMinutes);
                                Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].Add("10000", Lis_temp);
                            }
                        }
                        if (dw_temp > 10000 && dw_temp <= 55000)
                        {
                            if (Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].ContainsKey("55000"))
                            {
                                TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]]["55000"].Add(ts.TotalMinutes);
                            }
                            else
                            {
                                TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                List<double> Lis_temp = new List<double>();
                                Lis_temp.Add(ts.TotalMinutes);
                                Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].Add("55000", Lis_temp);
                            }
                        }
                        if (dw_temp > 55000 && dw_temp <= 80000)
                        {
                            if (Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].ContainsKey("80000"))
                            {
                                TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]]["80000"].Add(ts.TotalMinutes);
                            }
                            else
                            {
                                TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                List<double> Lis_temp = new List<double>();
                                Lis_temp.Add(ts.TotalMinutes);
                                Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].Add("80000", Lis_temp);
                            }
                        }
                        if (dw_temp > 80000 && dw_temp <= 120000)
                        {
                            if (Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].ContainsKey("120000"))
                            {
                                TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]]["120000"].Add(ts.TotalMinutes);
                            }
                            else
                            {
                                TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                List<double> Lis_temp = new List<double>();
                                Lis_temp.Add(ts.TotalMinutes);
                                Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].Add("120000", Lis_temp);
                            }
                        }
                        if (dw_temp > 120000 && dw_temp <= 200000)
                        {
                            if (Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].ContainsKey("200000"))
                            {
                                TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]]["200000"].Add(ts.TotalMinutes);
                            }
                            else
                            {
                                TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                                List<double> Lis_temp = new List<double>();
                                Lis_temp.Add(ts.TotalMinutes);
                                Dic_temp[arrtemp1[2] + '|' + arrtemp1[3]].Add("200000", Lis_temp);
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region
                        if (dw_temp <= 10000)
                        {
                            TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                            List<double> Lis_temp = new List<double>();
                            Lis_temp.Add(ts.TotalMinutes);
                            Dictionary<string, List<double>> Dic_temmp = new Dictionary<string, List<double>>();
                            Dic_temmp.Add("10000", Lis_temp);
                            Dic_temp.Add(arrtemp1[2] + '|' + arrtemp1[3], Dic_temmp);
                        }
                        if (dw_temp > 10000 && dw_temp <= 55000)
                        {
                            TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                            List<double> Lis_temp = new List<double>();
                            Lis_temp.Add(ts.TotalMinutes);
                            Dictionary<string, List<double>> Dic_temmp = new Dictionary<string, List<double>>();
                            Dic_temmp.Add("55000", Lis_temp);
                            Dic_temp.Add(arrtemp1[2] + '|' + arrtemp1[3], Dic_temmp);
                        }
                        if (dw_temp > 55000 && dw_temp <= 80000)
                        {
                            TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                            List<double> Lis_temp = new List<double>();
                            Lis_temp.Add(ts.TotalMinutes);
                            Dictionary<string, List<double>> Dic_temmp = new Dictionary<string, List<double>>();
                            Dic_temmp.Add("80000", Lis_temp);
                            Dic_temp.Add(arrtemp1[2] + '|' + arrtemp1[3], Dic_temmp);
                        }
                        if (dw_temp > 80000 && dw_temp <= 120000)
                        {
                            TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                            List<double> Lis_temp = new List<double>();
                            Lis_temp.Add(ts.TotalMinutes);
                            Dictionary<string, List<double>> Dic_temmp = new Dictionary<string, List<double>>();
                            Dic_temmp.Add("120000", Lis_temp);
                            Dic_temp.Add(arrtemp1[2] + '|' + arrtemp1[3], Dic_temmp);
                        }
                        if (dw_temp > 120000 && dw_temp <= 200000)
                        {
                            TimeSpan ts = Convert.ToDateTime(arrtemp1[5]) - Convert.ToDateTime(arrtemp1[4]);
                            List<double> Lis_temp = new List<double>();
                            Lis_temp.Add(ts.TotalMinutes);
                            Dictionary<string, List<double>> Dic_temmp = new Dictionary<string, List<double>>();
                            Dic_temmp.Add("200000", Lis_temp);
                            Dic_temp.Add(arrtemp1[2] + '|' + arrtemp1[3], Dic_temmp);
                        }
                        #endregion
                    }
                }
            }


            foreach (KeyValuePair<string, Dictionary<string, List<double>>> KVP1 in Dic_temp)
            {
                foreach (KeyValuePair<string, List<double>> KVP2 in KVP1.Value)
                {
                    double AVe = KVP2.Value.Average();
                    streamwriter.WriteLine(KVP1.Key + '|' + KVP2.Key + '|' + Convert.ToString(AVe));
                }
            }
            streamwriter.Close();
            MessageBox.Show("OK");
        }
        //将每条航线上的吞吐量需求和航行及等待时间整合成一个文档，便于后续模型的求解
        private void button31_Click(object sender, EventArgs e)
        {
            //定义列表存储2017年所存在的所有Ship_Lane
            List<string> Ship_Lane_2017 = new List<string>();
            #region

            string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\2021\ShipLane_CapacityDemand_NonRepetition.csv";
            // string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2021\ShipLane_CapacityDemand_ReceiveReexported_NonRepetition.csv";
            // string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2018\ShipLane_CapacityDemand_ProvideReexported_NonRepetition.csv";
            //string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2019\Reexport.csv";
            StreamReader streamreader = new StreamReader(inPath);
            string line = "";
            while ((line = streamreader.ReadLine()) != null)
            {
                string[] arrtemp = line.Split('|');
                //用于除了reexport文件之外的所有文件
                //只保留了需求量大于0的部分
                if (arrtemp[2] == "2021" && Convert.ToDouble(arrtemp[3]) > 0)
                {
                    Ship_Lane_2017.Add(arrtemp[0] + '|' + arrtemp[1]);

                }
                //用于reexport文件
                //if (arrtemp[0] == "2019" && Convert.ToDouble(arrtemp[3]) > 0)
                //{
                //    Ship_Lane_2017.Add(arrtemp[1] + '|' + arrtemp[2]);

                //}
            }

            #endregion
            //定义字典存储2017年航线不同吨位等级的船舶的等待和航行时间之和
            #region----船舶在各个港口之间的航行时间
            Dictionary<string, Dictionary<string, double>> Dic_temp = new Dictionary<string, Dictionary<string, double>>();
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\AveSaiTime_DeadWeight.csv";
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveSaiTime_DeadWeight_ReceiveReexported.csv";
            // string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveSaiTime_DeadWeight_ProvideReexported.csv";
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveSaiTime_DeadWeight_Reexport.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp1 = line1.Split('|');
                if (Ship_Lane_2017.Contains(arrtemp1[0] + '|' + arrtemp1[1]))
                {
                    if (Dic_temp.ContainsKey(arrtemp1[0] + '|' + arrtemp1[1]))
                    {
                        if (Dic_temp[arrtemp1[0] + '|' + arrtemp1[1]].ContainsKey(arrtemp1[2]))
                        {
                            Dic_temp[arrtemp1[0] + '|' + arrtemp1[1]][arrtemp1[2]] += Convert.ToDouble(arrtemp1[3]);
                        }
                        else
                        {
                            Dic_temp[arrtemp1[0] + '|' + arrtemp1[1]].Add(arrtemp1[2], Convert.ToDouble(arrtemp1[3]));
                        }
                    }
                    else
                    {
                        Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                        Dic_temp1.Add(arrtemp1[2], Convert.ToDouble(arrtemp1[3]));
                        Dic_temp.Add(arrtemp1[0] + '|' + arrtemp1[1], Dic_temp1);
                    }
                }
            }
            #endregion
            #region----统计船舶在各个港口之间所花费的总时间之和
            string inPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\AveWaiTime_DeadWeight.csv";
            // string inPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveWaiTime_DeadWeight_ReceiveReexported.csv";
            //string inPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveWaiTime_DeadWeight_ProvideReexported.csv";
            //string inPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveWaiTime_DeadWeight_Reexport.csv";
            StreamReader streamreader2 = new StreamReader(inPath2);
            string line2 = "";
            while ((line2 = streamreader2.ReadLine()) != null)
            {
                string[] arrtemp2 = line2.Split('|');
                if (Ship_Lane_2017.Contains(arrtemp2[0] + '|' + arrtemp2[1]))
                {
                    if (Dic_temp.ContainsKey(arrtemp2[0] + '|' + arrtemp2[1]))
                    {
                        if (Dic_temp[arrtemp2[0] + '|' + arrtemp2[1]].ContainsKey(arrtemp2[2]))
                        {
                            Dic_temp[arrtemp2[0] + '|' + arrtemp2[1]][arrtemp2[2]] += Convert.ToDouble(arrtemp2[3]);
                        }
                        else
                        {
                            Dic_temp[arrtemp2[0] + '|' + arrtemp2[1]].Add(arrtemp2[2], Convert.ToDouble(arrtemp2[3]));
                        }
                    }
                    else
                    {
                        Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                        Dic_temp1.Add(arrtemp2[2], Convert.ToDouble(arrtemp2[3]));
                        Dic_temp.Add(arrtemp2[0] + '|' + arrtemp2[1], Dic_temp1);
                    }
                }
            }
            #endregion
            string outPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\2021\AveTime_DeadWeight.csv";
            // string outPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2021\AveTime_DeadWeight_ReceiveReexported.csv";
            //string outPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2018\AveTime_DeadWeight_ProvideReexported.csv";
            //string outPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2019\AveTime_DeadWeight_Reexport.csv";
            StreamWriter streamwriter2 = new StreamWriter(outPath2);
            //定义列表存储LNG船舶的吨位等级
            List<string> Lng_DeadWeight = new List<string>();
            Lng_DeadWeight.Add("10000");
            Lng_DeadWeight.Add("55000");
            Lng_DeadWeight.Add("80000");
            Lng_DeadWeight.Add("120000");
            Lng_DeadWeight.Add("200000");
            foreach (KeyValuePair<string, Dictionary<string, double>> KVP2 in Dic_temp)
            {
                #region----这段代码是为了写出有些港口间没有这种吨位等级的船舶运输，那么成本应该是十分高昂的
                List<string> Lis_temp = new List<string>(KVP2.Value.Keys);
                foreach (string a in Lng_DeadWeight)
                {
                    if (Lis_temp.Contains(a) == false)
                    {
                        streamwriter2.WriteLine(KVP2.Key + '|' + a + '|' + Convert.ToString(100000000000));
                    }
                }
                #endregion
                foreach (KeyValuePair<string, double> KVP3 in KVP2.Value)
                {
                    streamwriter2.WriteLine(KVP2.Key + '|' + KVP3.Key + '|' + Convert.ToString(KVP3.Value));
                }
            }
            streamwriter2.Close();
            MessageBox.Show("OK");
        }
        //根据reexport和船舶吨位求每个港口每年的reexport吨位
        private void button33_Click(object sender, EventArgs e)
        {

            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_Reexported");
            FileInfo[] dirFile = theFolder.GetFiles();
            #region----根据船名库，统计船舶的吨位
            string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\原始LNG船舶静态数据.csv";
            StreamReader streamreader = new StreamReader(inPath);
            string line = "";
            //定义字典存储LNG船舶的MMSI，及其吨位
            Dictionary<string, double> Dic_LNG = new Dictionary<string, double>();
            while ((line = streamreader.ReadLine()) != null)
            {
                string[] arrtemp = line.Split(',');
                if (arrtemp[5] == "" || arrtemp[5].Contains("IMO"))
                {
                    continue;
                }
                else
                {
                    if (arrtemp[3] != "" && arrtemp[5] != "0")
                    {
                        Dic_LNG.Add(arrtemp[5], Convert.ToDouble(arrtemp[3]));
                    }
                    //用总吨位的百分之八十估算船舶的载重吨
                    else if (arrtemp[10] != "" && arrtemp[5] != "0")
                    {
                        double dou_temp = Convert.ToDouble(arrtemp[10]) * 4 / 5;
                        Dic_LNG.Add(arrtemp[5], dou_temp);
                    }

                }
            }
            #endregion
            foreach (FileInfo nextFile in dirFile)
            {
                //定义字典存储每年前端港口输出的reexported吨位
                Dictionary<string, Dictionary<string, double>> Dic_Reexp = new Dictionary<string, Dictionary<string, double>>();
                StreamReader streamreader1 = new StreamReader(nextFile.FullName);
                string[] arrtemp0 = nextFile.Name.Split('_');
                string line1 = "";
                while ((line1 = streamreader1.ReadLine()) != null)
                {
                    string[] arrtemp1 = line1.Split('|');
                    //定义字符存储船舶的吨位
                    double Wei = 0.0;
                    foreach (KeyValuePair<string, double> KVP1 in Dic_LNG)
                    {
                        if (KVP1.Key == arrtemp1[1])
                        {
                            Wei = KVP1.Value;
                        }
                    }
                    //统计港口间的reexport
                    if (Dic_Reexp.ContainsKey(arrtemp0[0]))
                    {
                        if (Dic_Reexp[arrtemp0[0]].ContainsKey(arrtemp1[2] + '|' + arrtemp1[3]))
                        {
                            Dic_Reexp[arrtemp0[0]][arrtemp1[2] + '|' + arrtemp1[3]] += Wei;
                        }
                        else
                        {
                            Dic_Reexp[arrtemp0[0]].Add(arrtemp1[2] + '|' + arrtemp1[3], Wei);
                        }
                    }
                    else
                    {
                        Dictionary<string, double> Dic_Reexp_temp = new Dictionary<string, double>();
                        Dic_Reexp_temp.Add(arrtemp1[2] + '|' + arrtemp1[3], Wei);
                        Dic_Reexp.Add(arrtemp0[0], Dic_Reexp_temp);
                    }

                }
                //写出字典中的内容
                string outPath1 = "H:\\2018-2021LNG和Container数据_Decoder\\解码\\作图1\\WithReexport\\" + arrtemp0[0] + "\\" + "Reexport.csv";
                StreamWriter streamwriter1 = new StreamWriter(outPath1);
                foreach (KeyValuePair<string, Dictionary<string, double>> KVP1 in Dic_Reexp)
                {
                    foreach (KeyValuePair<string, double> KVP2 in KVP1.Value)
                    {
                        streamwriter1.WriteLine(KVP1.Key + '|' + KVP2.Key + '|' + KVP2.Value);
                    }
                }
                streamwriter1.Close();
            }
            MessageBox.Show("OK");
        }
        //平均时间成本_只输出航线上船舶类型大于等于两种的
        private void button32_Click(object sender, EventArgs e)
        {
            //定义列表存储2018年所存在的所有Ship_Lane
            List<string> Ship_Lane_2017 = new List<string>();
            #region
            //string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\2021\ShipLane_CapacityDemand.csv";            
            //string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2018\ShipLane_CapacityDemand_ProvideReexported_NonRepetition.csv";
            // string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2021\ShipLane_CapacityDemand_ReceiveReexported_NonRepetition.csv";
            string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2021\Reexport.csv"; ;
            StreamReader streamreader = new StreamReader(inPath);

            string line = "";
            while ((line = streamreader.ReadLine()) != null)
            {
                string[] arrtemp = line.Split('|');
                //用于除reexported之外的文件
                //只保留了需求量大于0的部分
                //if (arrtemp[2] == "2021" && Convert.ToDouble(arrtemp[3]) > 0)
                //{
                //    Ship_Lane_2017.Add(arrtemp[0] + '|' + arrtemp[1]);

                //}
                //用于reexported的文件
                if (arrtemp[0] == "2021" && Convert.ToDouble(arrtemp[3]) > 0)
                {
                    Ship_Lane_2017.Add(arrtemp[1] + '|' + arrtemp[2]);

                }

            }
            // streanwriter.Close();
            #endregion
            //定义字典存储2017年航线不同吨位等级的船舶的等待和航行时间之和
            #region----船舶在各个港口之间的航行时间
            Dictionary<string, Dictionary<string, double>> Dic_temp = new Dictionary<string, Dictionary<string, double>>();
            // string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\AveSaiTime_DeadWeight.csv";
            // string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveSaiTime_DeadWeight_ProvideReexported.csv";
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveSaiTime_DeadWeight_ReceiveReexported.csv";
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveSaiTime_DeadWeight_Reexport.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp1 = line1.Split('|');
                if (Ship_Lane_2017.Contains(arrtemp1[0] + '|' + arrtemp1[1]))
                {
                    if (Dic_temp.ContainsKey(arrtemp1[0] + '|' + arrtemp1[1]))
                    {
                        if (Dic_temp[arrtemp1[0] + '|' + arrtemp1[1]].ContainsKey(arrtemp1[2]))
                        {
                            Dic_temp[arrtemp1[0] + '|' + arrtemp1[1]][arrtemp1[2]] += Convert.ToDouble(arrtemp1[3]);
                        }
                        else
                        {
                            Dic_temp[arrtemp1[0] + '|' + arrtemp1[1]].Add(arrtemp1[2], Convert.ToDouble(arrtemp1[3]));
                        }
                    }
                    else
                    {
                        Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                        Dic_temp1.Add(arrtemp1[2], Convert.ToDouble(arrtemp1[3]));
                        Dic_temp.Add(arrtemp1[0] + '|' + arrtemp1[1], Dic_temp1);
                    }
                }
            }
            #endregion
            #region----统计船舶在各个港口之间所花费的总时间之和
            //string inPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\AveWaiTime_DeadWeight.csv";
            //string inPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveWaiTime_DeadWeight_ProvideReexported.csv";
            // string inPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveWaiTime_DeadWeight_ReceiveReexported.csv";
            string inPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveWaiTime_DeadWeight_Reexport.csv";
            StreamReader streamreader2 = new StreamReader(inPath2);
            string line2 = "";
            while ((line2 = streamreader2.ReadLine()) != null)
            {
                string[] arrtemp2 = line2.Split('|');
                if (Ship_Lane_2017.Contains(arrtemp2[0] + '|' + arrtemp2[1]))
                {
                    if (Dic_temp.ContainsKey(arrtemp2[0] + '|' + arrtemp2[1]))
                    {
                        if (Dic_temp[arrtemp2[0] + '|' + arrtemp2[1]].ContainsKey(arrtemp2[2]))
                        {
                            Dic_temp[arrtemp2[0] + '|' + arrtemp2[1]][arrtemp2[2]] += Convert.ToDouble(arrtemp2[3]);
                        }
                        else
                        {
                            Dic_temp[arrtemp2[0] + '|' + arrtemp2[1]].Add(arrtemp2[2], Convert.ToDouble(arrtemp2[3]));
                        }
                    }
                    else
                    {
                        Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                        Dic_temp1.Add(arrtemp2[2], Convert.ToDouble(arrtemp2[3]));
                        Dic_temp.Add(arrtemp2[0] + '|' + arrtemp2[1], Dic_temp1);
                    }
                }
            }
            #endregion

            //string outPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\2021\AveTime_DeadWeight_ShipTypeMore3.csv";
            // string outPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2018\AveTime_DeadWeight_ProvideReexported_ShipTypeMore3.csv";
            //string outPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2021\AveTime_DeadWeight_ReceiveReexported_ShipTypeMore3.csv";
            string outPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2021\AveTime_DeadWeight_Reexported_ShipTypeMore2.csv";
            StreamWriter streamwriter2 = new StreamWriter(outPath2);
            //定义列表存储LNG船舶的吨位等级
            List<string> Lng_DeadWeight = new List<string>();
            Lng_DeadWeight.Add("10000");
            Lng_DeadWeight.Add("55000");
            Lng_DeadWeight.Add("80000");
            Lng_DeadWeight.Add("120000");
            Lng_DeadWeight.Add("200000");
            foreach (KeyValuePair<string, Dictionary<string, double>> KVP2 in Dic_temp)
            {
                #region----这段代码是为了写出有些港口间没有这种吨位等级的船舶运输，那么成本应该是十分高昂的
                List<string> Lis_temp = new List<string>(KVP2.Value.Keys);
                if (Lis_temp.Count() >= 2)
                // if (Lis_temp.Count() >= 3)
                {
                    foreach (string a in Lng_DeadWeight)
                    {
                        if (Lis_temp.Contains(a) == false)
                        {
                            streamwriter2.WriteLine(KVP2.Key + '|' + a + '|' + Convert.ToString(1000000000));
                        }
                    }

                    #endregion
                    foreach (KeyValuePair<string, double> KVP3 in KVP2.Value)
                    {
                        streamwriter2.WriteLine(KVP2.Key + '|' + KVP3.Key + '|' + Convert.ToString(KVP3.Value));
                    }
                }
            }
            streamwriter2.Close();
            MessageBox.Show("OK");
        }
        //对于reexport航线前期要有provide reexport航向与之对应
        private void button34_Click(object sender, EventArgs e)
        {
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2021\AveTime_DeadWeight_ProvideReexported_ShipTypeMore3.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            string line1 = "";
            //定义列表，存储再出口的港口
            List<string> Lis_temp = new List<string>();
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp1 = line1.Split('|');
                if (Lis_temp.Contains(arrtemp1[1]) == false)
                {
                    Lis_temp.Add(arrtemp1[1]);
                }
            }
            string inPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2021\AveTime_DeadWeight_Reexported_ShipTypeMore2.csv";
            string outPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2021\AveTime_DeadWeight_Reexported_ShipTypeMore2_1.csv";
            StreamReader streamreader = new StreamReader(inPath2);
            StreamWriter streamwriter = new StreamWriter(outPath2);
            string line2 = "";
            while ((line2 = streamreader.ReadLine()) != null)
            {
                string[] arrtemp2 = line2.Split('|');
                if (Lis_temp.Contains(arrtemp2[0]))
                {
                    streamwriter.WriteLine(line2);
                }
            }
            streamwriter.Close();
            MessageBox.Show("OK");
        }
        //航线吞吐量_只输出航线上船舶类型大于两种的
        private void button35_Click(object sender, EventArgs e)
        {
            string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\2018\AveTime_DeadWeight_ShipTypeMore3_with80000and120000.csv";
            //string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\2021\AveTime_DeadWeight_ShipTypeMore3.csv";
            //string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2021\AveTime_DeadWeight_ProvideReexported_ShipTypeMore3.csv";
            //string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2018\AveTime_DeadWeight_ReceiveReexported_ShipTypeMore3.csv";
            //string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2021\AveTime_DeadWeight_Reexported_ShipTypeMore2_1.csv";
            StreamReader streamreader = new StreamReader(inPath);
            string line = "";
            //定义列表存储有两种类型以上船舶航行的航线
            List<string> Lis_temp = new List<string>();
            while ((line = streamreader.ReadLine()) != null)
            {
                //string[] arrtemp = line.Split('|');
                string[] arrtemp = line.Split(',');
                if (Lis_temp.Contains(arrtemp[0] + '|' + arrtemp[1]) == false)
                {
                    Lis_temp.Add(arrtemp[0] + '|' + arrtemp[1]);
                }
            }
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\2018\ShipLane_CapacityDemand_NonRepetition.csv";
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2021\ShipLane_CapacityDemand_ProvideReexported_NonRepetition.csv";
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2018\ShipLane_CapacityDemand_ReceiveReexported_NonRepetition.csv";
            // string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2021\Reexport.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\2018\ShipLane_CapacityDemand_NonRepetition_ShipTypeMore3_with80000and120000.csv";
            //string outPath = @"D:\YuHongchu\作图\WithoutReexport\2017\ShipLane_CapacityDemand_NonRepetition_ShipTypeMore3.csv";
            //string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2021\ShipLane_CapacityDemand_ProvideReexported_NonRepetition_ShipTypeMore3.csv";
            //string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2018\ShipLane_CapacityDemand_ReceiveReexported_NonRepetition_ShipTypeMore3.csv";
            // string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2021\Reexport_ShipTypeMore2_1.csv";
            StreamWriter streamwriter = new StreamWriter(outPath);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp1 = line1.Split('|');
                //除了reexport文件之外，都用这个
                if (Lis_temp.Contains(arrtemp1[0] + '|' + arrtemp1[1]))
                {
                    streamwriter.WriteLine(line1);
                }
                //reexport文件，用这个
                //if (Lis_temp.Contains(arrtemp1[1] + '|' + arrtemp1[2]))
                //{
                //    streamwriter.WriteLine(line1);
                //}
            }
            streamwriter.Close();
            MessageBox.Show("OK");
        }
        //在运行前看下每条航线上是否按吨位进行排序
        //把AveTim数据组织成Lingo能读的矩阵格式
        private void button36_Click(object sender, EventArgs e)
        {
            string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\2018\AveTime_DeadWeight_ShipTypeMore3_with80000and120000.csv";
            StreamReader streamreader = new StreamReader(inPath);
            string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\2018\AveTime_DeadWeight_ShipTypeMore3_with80000and120000_Lingo.csv";
            StreamWriter streamwriter = new StreamWriter(outPath);
            string line = "";
            //创建表格
            //https://blog.csdn.net/zhruifei/article/details/78329066
            System.Data.DataTable Supply = new DataTable();
            double[,] arr = new double[54, 5];
            //https://zhidao.baidu.com/question/186554574.html?qbl=relate_question_2&word=C%23%BD%A8%D2%BB%B8%F65%B3%CB%D2%D4435%B5%C4%CA%FD%BE%DD%B1%ED%B8%F1
            //直接用先定义一个列表存储数据，然后将列表写成5成以435的文件
            List<double> Lis_temp = new List<double>();
            while ((line = streamreader.ReadLine()) != null)
            {
                string[] arrtemp = line.Split(',');
                Lis_temp.Add(Convert.ToDouble(arrtemp[3]));
            }
            // int coun = Lis_temp.Count();
            //int j = 0;
            for (int i = 0; i < 54; i++)
                for (int j = 0; j < 5; j++)
                {

                    arr[i, j] = Lis_temp[i * 5 + j];

                }
            //写出数组中的内容
            #region
            //for (int i = 0; i< 435;i++)
            //{
            //    for (int j = 0; j < 4; j++)
            //    {
            //        streamwriter.Write(Convert.ToString(arr[i,j])+'|');
            //    }
            //    streamwriter.Write(arr[i, 4]);
            //    streamwriter.Write("\r\n");
            //}
            #endregion
            for (int j = 0; j < 5; j++)
            {
                for (int i = 0; i < 53; i++)
                {
                    streamwriter.Write(Convert.ToString(arr[i, j]) + '|');
                }
                streamwriter.Write(arr[53, j]);
                streamwriter.Write("\r\n");
            }
            streamwriter.Close();
            MessageBox.Show("OK");
        }
        //把ShipCapacityDemand数据组织成Lingo能读的矩阵格式
        private void button37_Click(object sender, EventArgs e)
        {
            string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\2018\ShipLane_CapacityDemand_NonRepetition_ShipTypeMore3_with80000and120000.csv";
            //string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\2018\ShipLane_CapacityDemand_NonRepetition_ShipTypeMore3.csv";
            //string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2018\Reexport_ShipTypeMore2_1.csv";
            //string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2018\ShipLane_CapacityDemand_ProvideReexported_NonRepetition_ShipTypeMore3.csv";
            // string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2018\ShipLane_CapacityDemand_ReceiveReexported_NonRepetition_ShipTypeMore3.csv";
            StreamReader streamreader = new StreamReader(inPath);
            string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\2018\ShipLane_CapacityDemand_NonRepetition_ShipTypeMore3_with80000and120000_Lingo.csv";
            //string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\2018\ShipLane_CapacityDemand_NonRepetition_ShipTypeMore3_Lingo.csv";
            // string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2018\Reexport_ShipTypeMore2_1_Lingo.csv";
            //string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2018\ShipLane_CapacityDemand_ProvideReexported_NonRepetition_ShipTypeMore3_Lingo.csv";
            // string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\2018\ShipLane_CapacityDemand_ReceiveReexported_NonRepetition_ShipTypeMore3_Lingo.csv";
            StreamWriter streamwriter = new StreamWriter(outPath);
            string line = "";
            //定义列表存储需要写出的字符
            List<string> Lis_temp = new List<string>();
            while ((line = streamreader.ReadLine()) != null)
            {
                string[] arrtemp = line.Split('|');
                Lis_temp.Add(arrtemp[arrtemp.Count() - 1]);
            }
            for (int i = 0; i < Lis_temp.Count() - 1; i++)
            {
                streamwriter.Write(Lis_temp[i] + '|');
            }
            streamwriter.Write(Lis_temp[Lis_temp.Count() - 1]);
            streamwriter.Write("\r\n");
            streamwriter.Close();
            MessageBox.Show("OK");
        }
        /// <summary>
        /// 2018年withoutreport无解，试试每条航线中有三种船舶类型的再筛选出必须有80000和120000吨的
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button38_Click(object sender, EventArgs e)
        {
            //定义列表存储2018年所存在的所有Ship_Lane
            List<string> Ship_Lane_2017 = new List<string>();
            #region
            string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\2018\ShipLane_CapacityDemand_NonRepetition.csv"; ;
            StreamReader streamreader = new StreamReader(inPath);
            string line = "";
            while ((line = streamreader.ReadLine()) != null)
            {
                string[] arrtemp = line.Split('|');
                //用于除reexported之外的文件
                //只保留了需求量大于0的部分
                if (arrtemp[2] == "2018" && Convert.ToDouble(arrtemp[3]) > 0)
                {
                    Ship_Lane_2017.Add(arrtemp[0] + '|' + arrtemp[1]);

                }
                ////用于reexported的文件
                //if (arrtemp[0] == "2021" && Convert.ToDouble(arrtemp[3]) > 0)
                //{
                //    Ship_Lane_2017.Add(arrtemp[1] + '|' + arrtemp[2]);

                //}

            }
            // streanwriter.Close();
            #endregion
            //定义字典存储2017年航线不同吨位等级的船舶的等待和航行时间之和
            #region----船舶在各个港口之间的航行时间
            Dictionary<string, Dictionary<string, double>> Dic_temp = new Dictionary<string, Dictionary<string, double>>();
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\AveSaiTime_DeadWeight.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp1 = line1.Split('|');
                if (Ship_Lane_2017.Contains(arrtemp1[0] + '|' + arrtemp1[1]))
                {
                    if (Dic_temp.ContainsKey(arrtemp1[0] + '|' + arrtemp1[1]))
                    {
                        if (Dic_temp[arrtemp1[0] + '|' + arrtemp1[1]].ContainsKey(arrtemp1[2]))
                        {
                            Dic_temp[arrtemp1[0] + '|' + arrtemp1[1]][arrtemp1[2]] += Convert.ToDouble(arrtemp1[3]);
                        }
                        else
                        {
                            Dic_temp[arrtemp1[0] + '|' + arrtemp1[1]].Add(arrtemp1[2], Convert.ToDouble(arrtemp1[3]));
                        }
                    }
                    else
                    {
                        Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                        Dic_temp1.Add(arrtemp1[2], Convert.ToDouble(arrtemp1[3]));
                        Dic_temp.Add(arrtemp1[0] + '|' + arrtemp1[1], Dic_temp1);
                    }
                }
            }
            #endregion
            #region----统计船舶在各个港口之间所花费的总时间之和
            string inPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\AveWaiTime_DeadWeight.csv";
            StreamReader streamreader2 = new StreamReader(inPath2);
            string line2 = "";
            while ((line2 = streamreader2.ReadLine()) != null)
            {
                string[] arrtemp2 = line2.Split('|');
                if (Ship_Lane_2017.Contains(arrtemp2[0] + '|' + arrtemp2[1]))
                {
                    if (Dic_temp.ContainsKey(arrtemp2[0] + '|' + arrtemp2[1]))
                    {
                        if (Dic_temp[arrtemp2[0] + '|' + arrtemp2[1]].ContainsKey(arrtemp2[2]))
                        {
                            Dic_temp[arrtemp2[0] + '|' + arrtemp2[1]][arrtemp2[2]] += Convert.ToDouble(arrtemp2[3]);
                        }
                        else
                        {
                            Dic_temp[arrtemp2[0] + '|' + arrtemp2[1]].Add(arrtemp2[2], Convert.ToDouble(arrtemp2[3]));
                        }
                    }
                    else
                    {
                        Dictionary<string, double> Dic_temp1 = new Dictionary<string, double>();
                        Dic_temp1.Add(arrtemp2[2], Convert.ToDouble(arrtemp2[3]));
                        Dic_temp.Add(arrtemp2[0] + '|' + arrtemp2[1], Dic_temp1);
                    }
                }
            }
            #endregion

            string outPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\2018\AveTime_DeadWeight_ShipTypeMore3_with80000and120000.csv";
            StreamWriter streamwriter2 = new StreamWriter(outPath2);
            //定义列表存储LNG船舶的吨位等级
            List<string> Lng_DeadWeight = new List<string>();
            Lng_DeadWeight.Add("10000");
            Lng_DeadWeight.Add("55000");
            Lng_DeadWeight.Add("80000");
            Lng_DeadWeight.Add("120000");
            Lng_DeadWeight.Add("200000");
            foreach (KeyValuePair<string, Dictionary<string, double>> KVP2 in Dic_temp)
            {
                #region----这段代码是为了写出有些港口间没有这种吨位等级的船舶运输，那么成本应该是十分高昂的
                List<string> Lis_temp = new List<string>(KVP2.Value.Keys);
                // if (Lis_temp.Count() >= 2)
                // if (Lis_temp.Count() >= 3)
                if (Lis_temp.Count() >= 3 && Lis_temp.Contains("80000") && Lis_temp.Contains("120000"))
                {
                    foreach (string a in Lng_DeadWeight)
                    {
                        if (Lis_temp.Contains(a) == false)
                        {
                            streamwriter2.WriteLine(KVP2.Key + '|' + a + '|' + Convert.ToString(1000000000));
                        }
                    }

                    #endregion
                    foreach (KeyValuePair<string, double> KVP3 in KVP2.Value)
                    {
                        streamwriter2.WriteLine(KVP2.Key + '|' + KVP3.Key + '|' + Convert.ToString(KVP3.Value));
                    }
                }
            }
            streamwriter2.Close();
            MessageBox.Show("OK");
        }
        //将2018-2021年的港内停留时间、港间航行时间、港间停留合并,并添加船舶信息
        private void button39_Click(object sender, EventArgs e)
        {
            string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_In_Ports(Duration)(2018-2021)_LNG_Noballast.csv";
            //string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast.csv";
            //string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast.csv";
            StreamWriter streamwriter = new StreamWriter(outPath);
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_In_Ports(Duration)(2018-2021)_LNG_Noballast");
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast");
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_添加时长");
            FileInfo[] dirFile = theFolder.GetFiles();
            #region-----根据船舶的IMO,统计船舶的吨位
            //定义字典存储不同船舶的吨位
            Dictionary<string, double> Dic_Ton = new Dictionary<string, double>();
            string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\原始LNG船舶静态数据.csv";
            StreamReader streamreader1 = new StreamReader(inPath);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp = line1.Split(',');
                if (arrtemp[0] == "" || arrtemp[5].Contains("IMO"))
                {
                    continue;
                }
                else
                {
                    if (arrtemp[3] != "")
                    {
                        Dic_Ton.Add(arrtemp[5], Convert.ToDouble(arrtemp[3]));
                    }
                    //用总吨位的百分之八十估算船舶的载重吨
                    else if (arrtemp[10] != "")
                    {
                        double dou_temp = Convert.ToDouble(arrtemp[10]) * 4 / 5;
                        Dic_Ton.Add(arrtemp[5], dou_temp);
                    }
                }
            }

            #endregion
            foreach (FileInfo nextFile in dirFile)
            {
                string[] arrtemp0 = nextFile.Name.Split('_');
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split('|');
                    double Wei = 0.0;
                    foreach (KeyValuePair<string, double> kvp1 in Dic_Ton)
                    {
                        if (kvp1.Key == arrtemp[0])
                        {
                            Wei = kvp1.Value;
                        }
                    }
                    if (Wei != 0)
                    {
                        streamwriter.WriteLine(arrtemp0[0] + '|' + line + '|' + Convert.ToString(Wei));
                    }
                }
            }
            streamwriter.Close();
            MessageBox.Show("OK");
        }
        //对Sailing_between_ports计算时长
        private void button40_Click(object sender, EventArgs e)
        {
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_添加时长";
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string destPath = System.IO.Path.Combine(targetPath, nextFile.Name);
                StreamWriter streamwriter = new StreamWriter(destPath);
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split('|');
                    TimeSpan ts = Convert.ToDateTime(arrtemp[4]) - Convert.ToDateTime(arrtemp[3]);
                    streamwriter.WriteLine(line + '|' + ts.TotalMinutes);
                }
                streamwriter.Close();
            }
            MessageBox.Show("OK");
        }
        /// <summary>
        /// 提取中国的数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button41_Click(object sender, EventArgs e)
        {
            #region //根据全球港口数据给PortCenter文件中的港口添加CountryCode
            string rePath = @"H:\2018-2021LNG和Container数据_Decoder\解码\全球港口属性列表.csv";
            StreamReader streamreader0 = new StreamReader(rePath);
            //定义字典存储每个港口的CountryCode
            Dictionary<string, string> Dic_Temp = new Dictionary<string, string>();
            string line0 = "";
            //存在不同国家有同名港口的情况
            int i = 0;
            while ((line0 = streamreader0.ReadLine()) != null)
            {
                string[] arrtemp0 = line0.Split(',');
                if (arrtemp0[0] == "World_port_index_number")
                {
                    i++;
                    continue;
                }
                else
                {
                    i++;
                    if (Dic_Temp.ContainsKey(i + " " + arrtemp0[2]) == false)
                    {
                        Dic_Temp.Add(i + " " + arrtemp0[2], arrtemp0[3]);
                    }
                }
            }
            #endregion
            //定义列表存储有经停中国的LNG船舶的IMO
            List<string> Lis_Temp = new List<string>();
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2018_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1_同状态合并_时间序列");
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string[] arrtemp0 = nextFile.Name.Split('.');
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp1 = line.Split('|');
                    string[] arrtemp2 = arrtemp1[0].Split('_');
                    string Cd1 = "";
                    foreach (KeyValuePair<string, string> KVP1 in Dic_Temp)
                    {
                        if (arrtemp2[arrtemp2.Count() - 1].Contains('.'))
                        {
                            string a = arrtemp2[arrtemp2.Count() - 1].Substring(arrtemp2[arrtemp2.Count() - 1].IndexOf(".") + 1);
                            if (arrtemp2[arrtemp2.Count() - 1].Contains(KVP1.Key) || KVP1.Key.Contains(a))
                            {
                                Cd1 = KVP1.Value;
                            }
                        }
                        else
                        {
                            // string[] b = KVP1.Key.Split(' ');
                            //if (KVP1.Key.Contains(arrtemp[1]) || arrtemp[1].Contains(b[1]) || arrtemp[1].Contains(KVP1.Key) )
                            string b = arrtemp2[arrtemp2.Count() - 1].Substring(arrtemp2[arrtemp2.Count() - 1].IndexOf(" ") + 1);
                            if (KVP1.Key.Contains(arrtemp2[arrtemp2.Count() - 1]) || arrtemp2[arrtemp2.Count() - 1].Contains(KVP1.Key))
                            {
                                Cd1 = KVP1.Value;
                            }
                            else if (arrtemp2[arrtemp2.Count() - 1].Contains(KVP1.Key) || KVP1.Key.Contains(b))
                            {
                                Cd1 = KVP1.Value;
                            }
                        }
                    }
                    if (Cd1 == "CN")
                    {
                        Lis_Temp.Add(arrtemp0[0]);
                        break;
                    }
                }
            }
            //写出2020经停中国的所有LNG
            string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\2018_LNGandContainer_inCN.csv";
            StreamWriter streamwriter = new StreamWriter(outPath);
            foreach (string a in Lis_Temp)
            {
                streamwriter.WriteLine(a);
            }
            streamwriter.Close();
            MessageBox.Show("ok");
        }
        //提取经停中国的LNG轨迹
        private void button42_Click(object sender, EventArgs e)
        {
            //定义列表存储经停中国的轨迹的IMO
            #region
            List<string> Lis_temp = new List<string>();
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\2021_LNGandContainer_inCN_1.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                Lis_temp.Add(line1);
            }
            #endregion
            //定义列表存储LNG船舶的IMO
            #region
            string inPath2 = @"H:\2018-2021LNG和Container数据\LNG.csv";
            StreamReader streamreader2 = new StreamReader(inPath2);
            //定义列表存储LNG的IMO
            List<string> LNG_IMO = new List<string>();
            string line2 = "";
            while ((line2 = streamreader2.ReadLine()) != null)
            {
                string[] arrtemp = line2.Split(',');
                if (arrtemp[0] == "LRIMOShipNo")
                {
                    continue;
                }
                else
                {
                    LNG_IMO.Add(arrtemp[0]);
                }
            }
            #endregion
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序 - 副本");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停中国(LNG)";
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string[] arrtemp0 = nextFile.Name.Split('.');
                if (LNG_IMO.Contains(arrtemp0[0]) && Lis_temp.Contains(arrtemp0[0]))
                {
                    string destPath = System.IO.Path.Combine(targetPath, nextFile.Name);
                    nextFile.CopyTo(destPath);
                }
            }
            MessageBox.Show("ok");
        }
        //提取中国数据正确版2022_5_13
        private void button43_Click(object sender, EventArgs e)
        {
            #region //根据全球港口数据给PortCenter文件中的港口添加CountryCode
            string rePath = @"H:\2018-2021LNG和Container数据_Decoder\解码\全球港口属性列表.csv";
            StreamReader streamreader0 = new StreamReader(rePath);
            //定义字典存储每个港口的CountryCode
            Dictionary<string, string> Dic_Temp = new Dictionary<string, string>();
            string line0 = "";
            //存在不同国家有同名港口的情况
            int i = 0;
            while ((line0 = streamreader0.ReadLine()) != null)
            {
                string[] arrtemp0 = line0.Split(',');
                if (arrtemp0[0] == "World_port_index_number")
                {
                    i++;
                    continue;
                }
                else
                {
                    //i++;
                    //if (Dic_Temp.ContainsKey(i + " " + arrtemp0[2]) == false)
                    //{
                    //    Dic_Temp.Add(i + " " + arrtemp0[2], arrtemp0[3]);
                    //}
                    //只统计中国的
                    //i++;
                    if (Dic_Temp.ContainsKey(arrtemp0[2]) == false && arrtemp0[3] == "CN")
                    {
                        Dic_Temp.Add(arrtemp0[2], arrtemp0[3]);
                    }
                }
            }
            #endregion
            //定义列表存储有经停中国的LNG船舶的IMO
            List<string> Lis_Temp = new List<string>();
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2020_轨迹_时间排序_经停港口");
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string[] arrtemp0 = nextFile.Name.Split('.');
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp1 = line.Split('|');
                    if (arrtemp1.Count() == 11 || arrtemp1[11] == "InandOut")
                    {
                        continue;
                    }
                    else
                    {
                        string[] arrtemp2 = arrtemp1;
                        string Cd1 = "";
                        for (int j = 12; j <= arrtemp2.Count() - 3; j = j + 4)
                        {
                            foreach (KeyValuePair<string, string> KVP1 in Dic_Temp)
                            {

                                if (arrtemp2[j].Contains('.'))
                                {
                                    string a = arrtemp2[j].Substring(arrtemp2[j].IndexOf(".") + 1);
                                    if (arrtemp2[j].Contains(KVP1.Key) || KVP1.Key.Contains(a))
                                    {
                                        Cd1 = KVP1.Value;
                                    }
                                }
                                else
                                {
                                    // string[] b = KVP1.Key.Split(' ');
                                    //if (KVP1.Key.Contains(arrtemp[1]) || arrtemp[1].Contains(b[1]) || arrtemp[1].Contains(KVP1.Key) )

                                    if (arrtemp2[j].Contains("\\t"))
                                    {
                                        string b = arrtemp2[j].Substring(arrtemp2[j].IndexOf("\\t") + 1);
                                        //因为之前考虑到重复，添加了数字，所以如果用全部的应该修改如下
                                        //string c = KVP1.Key.Substring(KVP1.Key.IndexOf(" ") + 1);
                                        //然后用c去判断，而不用KVP1.Key
                                        if (KVP1.Key.Contains(arrtemp2[j]) || arrtemp2[j].Contains(KVP1.Key))
                                        {
                                            Cd1 = KVP1.Value;
                                        }
                                        else if (arrtemp2[j].Contains(KVP1.Key) || KVP1.Key.Contains(b))
                                        {
                                            Cd1 = KVP1.Value;
                                        }

                                    }
                                    else
                                    {
                                        string b = arrtemp2[j].Substring(arrtemp2[j].IndexOf(" ") + 1);
                                        if (KVP1.Key.Contains(arrtemp2[j]) || arrtemp2[j].Contains(KVP1.Key))
                                        {
                                            Cd1 = KVP1.Value;
                                        }
                                        else if (arrtemp2[j].Contains(KVP1.Key) || KVP1.Key.Contains(b))
                                        {
                                            Cd1 = KVP1.Value;
                                        }

                                    }

                                }
                            }

                            if (Cd1 == "CN")
                            {
                                Lis_Temp.Add(arrtemp0[0]);
                                break;
                            }

                        }
                        if (Cd1 == "CN")
                        {
                            break;
                        }
                    }
                }
            }
            //写出2020经停中国的所有LNG
            string outPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\2020_LNGandContainer_inCN_1.csv";
            StreamWriter streamwriter = new StreamWriter(outPath);
            foreach (string a in Lis_Temp)
            {
                streamwriter.WriteLine(a);
            }
            streamwriter.Close();
            MessageBox.Show("ok");
        }
        //2022_2_22对优化前后的网络进行对比，分析不同的航线分别节约了多少时间
        //可以根据Lingo计算出的结果，将其读入的文件还原，然后通过前后对比，从而计算其节约的时间
        //还要考虑同一种类型的船不止部署了一次的情况，所以需要根据LINGO计算出的X和A值，得到优化后的
        //LINGO部署的时候不存在同一条船重复部署的情况，所以在计算优化之前的结果不能将所有的叠加，而是要统计最大的值，因此改程序需要修改（2022.02.04）
        private void button44_Click(object sender, EventArgs e)
        {
            //先根据原数据计算每条航线上所有船舶的航行时间和等待时间的总和
            //定义字典存储原始的每条航线上每个MMSI对应船舶的最大航行时间和等待时间
            Dictionary<string, Dictionary<string, double>> Ave_SaiTime_Ori = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<string, double>> Ave_WaiTime_Ori = new Dictionary<string, Dictionary<string, double>>();
            #region
            string inPath1 = @"D:\YuHongchu\Sailing_between_Ports_v1_Noballast_添加时长\2014_LNG_Sailing_between_Ports_v1.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp = line1.Split('|');
                if (Ave_SaiTime_Ori.ContainsKey(arrtemp[1] + '|' + arrtemp[2]))
                {
                    //这是计算总和的方式
                    // Ave_Time_Ori[arrtemp[1] + '|' + arrtemp[2]] += Convert.ToDouble(arrtemp[5]);
                    if (Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]].ContainsKey(arrtemp[0]))
                    {
                        //这是计算最大值的方式
                        if (Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] <= Convert.ToDouble(arrtemp[5]))
                        {
                            Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] = Convert.ToDouble(arrtemp[5]);
                        }
                    }
                    else
                    {
                        Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]].Add(arrtemp[0], Convert.ToDouble(arrtemp[5]));
                    }

                }
                else
                {
                    Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                    dic_temp.Add(arrtemp[0], Convert.ToDouble(arrtemp[5]));
                    Ave_SaiTime_Ori.Add(arrtemp[1] + '|' + arrtemp[2], dic_temp);
                }
            }
            string inPath2 = @"D:\YuHongchu\Waiting_between_Ports_v1_Noballast\2014_LNG_Waiting_between_Ports_v1.csv";
            StreamReader streamreader2 = new StreamReader(inPath2);
            string line2 = "";
            while ((line2 = streamreader2.ReadLine()) != null)
            {
                string[] arrtemp = line2.Split('|');
                if (Ave_WaiTime_Ori.ContainsKey(arrtemp[1] + '|' + arrtemp[2]))
                {
                    //这是计算总和的方式
                    // Ave_Time_Ori[arrtemp[1] + '|' + arrtemp[2]] += Convert.ToDouble(arrtemp[5]);
                    if (Ave_WaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]].ContainsKey(arrtemp[0]))
                    {
                        //这是计算最大值的方式
                        if (Ave_WaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] <= Convert.ToDouble(arrtemp[5]))
                        {
                            Ave_WaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] = Convert.ToDouble(arrtemp[5]);
                        }
                    }
                    else
                    {
                        Ave_WaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]].Add(arrtemp[0], Convert.ToDouble(arrtemp[5]));
                    }

                }
                else
                {
                    Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                    dic_temp.Add(arrtemp[0], Convert.ToDouble(arrtemp[5]));
                    Ave_WaiTime_Ori.Add(arrtemp[1] + '|' + arrtemp[2], dic_temp);
                }
            }
            //将原始的每条航线上对应的每个MMSI船舶的航行时间与等待时间合并
            Dictionary<string, double> Ave_Time_Ori = new Dictionary<string, double>();
            Dictionary<string, Dictionary<string, double>> Ave_Time_Or_1 = new Dictionary<string, Dictionary<string, double>>();
            //该程序是为了将每条航线上对应的每个MMSI的航行时间与等待时间合并
            foreach (KeyValuePair<string, Dictionary<string, double>> kvp1 in Ave_SaiTime_Ori)
            {
                foreach (KeyValuePair<string, double> kvp11 in kvp1.Value)
                {
                    int i = 0;
                    foreach (KeyValuePair<string, Dictionary<string, double>> kvp2 in Ave_WaiTime_Ori)
                    {
                        foreach (KeyValuePair<string, double> kvp22 in kvp2.Value)
                        {
                            if (kvp1.Key == kvp2.Key && kvp11.Key == kvp22.Key)
                            {
                                i++;
                                if (Ave_Time_Or_1.ContainsKey(kvp1.Key))
                                {
                                    if (Ave_Time_Or_1[kvp1.Key].ContainsKey(kvp11.Key))
                                    {
                                        Ave_Time_Or_1[kvp1.Key][kvp11.Key] = kvp11.Value + kvp22.Value;
                                    }
                                    else
                                    {
                                        Ave_Time_Or_1[kvp1.Key].Add(kvp11.Key, kvp11.Value + kvp22.Value);
                                    }
                                }
                                else
                                {
                                    Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                                    dic_temp.Add(kvp11.Key, kvp11.Value + kvp22.Value);
                                    Ave_Time_Or_1.Add(kvp1.Key, dic_temp);
                                }
                            }

                        }
                        //在某一航线上，有某条MMSI船的航行时间，但是没有等待时间
                        if (i == 0 && kvp1.Key == kvp2.Key)
                        {
                            if (Ave_Time_Or_1.ContainsKey(kvp1.Key))
                            {

                                Ave_Time_Or_1[kvp1.Key].Add(kvp11.Key, kvp11.Value);

                            }
                            else
                            {
                                Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                                dic_temp.Add(kvp11.Key, kvp11.Value);
                                Ave_Time_Or_1.Add(kvp1.Key, dic_temp);
                            }
                        }
                    }
                }
            }
            foreach (KeyValuePair<string, Dictionary<string, double>> kvp1 in Ave_Time_Or_1)
            {
                double sum = 0.0;
                foreach (KeyValuePair<string, double> kvp2 in kvp1.Value)
                {
                    sum += kvp2.Value;
                }
                Ave_Time_Ori.Add(kvp1.Key, sum);
            }
            #endregion
            //处理没有再出口的
            //根据优化运行后的结果，将原始结果附在其前面，方便比较
            string inPath3 = @"D:\YuHongchu\作图\After Optimization\Withoutreexported\2014_AveTime_DeadWeight_ShipTypeMore3_Afteroptimization.csv";
            string destPath1 = @"D:\YuHongchu\作图\After Optimization\Comparision\2014_AveTime_DeadWeight_NoneReexported_ShipTypeMore3_Comp.csv";
            StreamReader streamreader3 = new StreamReader(inPath3);
            StreamWriter streamwriter1 = new StreamWriter(destPath1);
            string line3 = "";
            //定义字典存储需要输出的内容
            Dictionary<string, double> Dic_NonReexp_AftOP = new Dictionary<string, double>();
            while ((line3 = streamreader3.ReadLine()) != null)
            {
                string[] arrtemp = line3.Split(',');
                if (Dic_NonReexp_AftOP.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_NonReexp_AftOP[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[4]);
                }
                else
                {
                    Dic_NonReexp_AftOP.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[4]));
                }
            }
            //输出的时候先写出原始的，再写出优化后的
            foreach (KeyValuePair<string, double> kvp1 in Dic_NonReexp_AftOP)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_Time_Ori)
                {

                    if (kvp1.Key == kvp2.Key)
                    {
                        streamwriter1.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value);
                    }

                }
            }
            streamwriter1.Close();
            //处理包括再出口的
            //根据优化运行后的结果，将原始结果附在其前面，方便比较
            string inPath4 = @"D:\YuHongchu\作图\After Optimization\Withreexported\2014_AveTime_DeadWeight_ProvideReexported_ShipTypeMore3_Afteroptimization.csv";
            string destPath2 = @"D:\YuHongchu\作图\After Optimization\Comparision\2014_AveTime_DeadWeight_ProvideReexported_ShipTypeMore3_Comp.csv";
            StreamReader streamreader4 = new StreamReader(inPath4);
            StreamWriter streamwriter2 = new StreamWriter(destPath2);
            string line4 = "";
            //定义字典存储需要输出的内容
            Dictionary<string, double> Dic_ProReexp_AftOP = new Dictionary<string, double>();
            while ((line4 = streamreader4.ReadLine()) != null)
            {
                string[] arrtemp = line4.Split(',');
                if (Dic_ProReexp_AftOP.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_ProReexp_AftOP[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[4]);
                }
                else
                {
                    Dic_ProReexp_AftOP.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[4]));
                }
            }
            //输出的时候先写出原始的，再写出优化后的
            foreach (KeyValuePair<string, double> kvp1 in Dic_ProReexp_AftOP)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_Time_Ori)
                {
                    if (kvp1.Key == kvp2.Key)
                    {
                        streamwriter2.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value);
                    }
                }
            }
            streamwriter2.Close();

            string inPath5 = @"D:\YuHongchu\作图\After Optimization\Withreexported\2014_AveTime_DeadWeight_ReceiveReexported_ShipTypeMore3_Afteroptimization.csv";
            string destPath3 = @"D:\YuHongchu\作图\After Optimization\Comparision\2014_AveTime_DeadWeight_ReceiveReexported_ShipTypeMore3_Comp.csv";
            StreamReader streamreader5 = new StreamReader(inPath5);
            StreamWriter streamwriter3 = new StreamWriter(destPath3);
            string line5 = "";
            //定义字典存储需要输出的内容
            Dictionary<string, double> Dic_RecReexp_AftOP = new Dictionary<string, double>();
            while ((line5 = streamreader5.ReadLine()) != null)
            {
                string[] arrtemp = line5.Split(',');
                if (Dic_RecReexp_AftOP.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_RecReexp_AftOP[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[4]);
                }
                else
                {
                    Dic_RecReexp_AftOP.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[4]));
                }
            }
            //输出的时候先写出原始的，再写出优化后的
            foreach (KeyValuePair<string, double> kvp1 in Dic_RecReexp_AftOP)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_Time_Ori)
                {
                    if (kvp1.Key == kvp2.Key)
                    {
                        streamwriter3.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value);
                    }
                }
            }
            streamwriter3.Close();

            string inPath6 = @"D:\YuHongchu\作图\After Optimization\Withreexported\2014_AveTime_DeadWeight_Reexported_ShipTypeMore2_1_Afteroptimization.csv";
            string destPath4 = @"D:\YuHongchu\作图\After Optimization\Comparision\2014_AveTime_DeadWeight_Reexported_ShipTypeMore2_1_Comp.csv";
            StreamReader streamreader6 = new StreamReader(inPath6);
            StreamWriter streamwriter4 = new StreamWriter(destPath4);
            string line6 = "";
            //定义字典存储需要输出的内容
            Dictionary<string, double> Dic_Reexp_AftOP = new Dictionary<string, double>();
            while ((line6 = streamreader6.ReadLine()) != null)
            {
                string[] arrtemp = line6.Split(',');
                if (Dic_Reexp_AftOP.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_Reexp_AftOP[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[4]);
                }
                else
                {
                    Dic_Reexp_AftOP.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[4]));
                }
            }
            //输出的时候先写出原始的，再写出优化后的
            foreach (KeyValuePair<string, double> kvp1 in Dic_Reexp_AftOP)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_Time_Ori)
                {
                    if (kvp1.Key == kvp2.Key)
                    {
                        streamwriter4.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value);
                    }
                }
            }
            streamwriter4.Close();

            MessageBox.Show("OK");
        }
        //港口花费时间分月
        private void button45_Click(object sender, EventArgs e)
        {
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_In_Ports(Period)(2018-2021)_LNG");
            FileInfo[] dirFile = theFolder.GetFiles();
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_In_Ports(Period)(2018-2021)_LNG_Monthly";
            foreach (FileInfo nextFile in dirFile)
            {
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string[] arrtemp0 = nextFile.Name.Split('_');
                string destPath1 = System.IO.Path.Combine(targetPath, arrtemp0[0]);
                if (Directory.Exists(destPath1) == false)
                {
                    Directory.CreateDirectory(destPath1);
                }
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp1 = line.Split('|');
                    if ((Convert.ToDateTime(arrtemp1[3]) - Convert.ToDateTime(arrtemp1[2])).TotalMinutes > 5760 || (Convert.ToDateTime(arrtemp1[3]) - Convert.ToDateTime(arrtemp1[2])).TotalMinutes < 60)
                    {
                        continue;
                    }
                    else
                    {
                        string[] arrtemp2 = arrtemp1[3].Split('/');
                        string destPath2 = System.IO.Path.Combine(destPath1, arrtemp2[1] + ".csv");
                        FileStream fs = new FileStream(destPath2, FileMode.Append);
                        StreamWriter streamwriter = new StreamWriter(fs);
                        double t = (Convert.ToDateTime(arrtemp1[3]) - Convert.ToDateTime(arrtemp1[2])).TotalMinutes;
                        streamwriter.WriteLine(line + '|' + Convert.ToString(t));
                        streamwriter.Close();

                    }
                }
            }
            MessageBox.Show("OK");
        }
        //天津港分天
        private void button46_Click(object sender, EventArgs e)
        {
            //string targetPath = @"H:\2018 - 2021LNG和Container数据_Decoder\分天";
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据\export");
            string targetPath = @"H:\学生指导\白新宇\原始数据_分天";
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\学生指导\白新宇\原始数据");
            DirectoryInfo[] dirFolder = theFolder.GetDirectories();

            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string[] arrtemp0 = nextFile.Name.Split('.');
                string destPath2 = System.IO.Path.Combine(targetPath, arrtemp0[0]);
                if (Directory.Exists(destPath2) == false)
                {
                    Directory.CreateDirectory(destPath2);
                }
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp1 = line.Split(';');
                    if (arrtemp1.Count() > 1)
                    {
                        string[] arrtemp2 = arrtemp1[0].Split(':');
                        if (arrtemp2[0].Contains('-'))
                        {
                            string destPath3 = System.IO.Path.Combine(destPath2, arrtemp2[0] + ".csv");
                            FileStream fs = new FileStream(destPath3, FileMode.Append);
                            StreamWriter streamwriter = new StreamWriter(fs);
                            streamwriter.WriteLine(line);
                            streamwriter.Close();
                        }
                    }
                }
            }

            MessageBox.Show("OK");
        }
        //2022_7_11因为前面的11和12月的数据有问题所以重新处理
        private void button47_Click(object sender, EventArgs e)
        {
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹");
            FileInfo[] dirFile = theFolder.GetFiles();
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_正确";
            foreach (FileInfo nextFile in dirFile)
            {
                List<string> Lis_Temp = new List<string>();
                StreamReader str = new StreamReader(nextFile.FullName);
                string line = "";
                while ((line = str.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split('|');
                    //DateTime dt=Convert.ToDateTime("2021-11-01 00:00:00");
                    TimeSpan ts = Convert.ToDateTime("2021-11-01 00:00:00") - Convert.ToDateTime(arrtemp[2]);
                    if (ts.TotalSeconds > 0)
                    {
                        Lis_Temp.Add(line);
                    }
                }
                DirectoryInfo theFolder1 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据\2022.06.01补齐11-12_分天_解码_轨迹\export");
                FileInfo[] dirFile1 = theFolder1.GetFiles();
                foreach (FileInfo nextFile1 in dirFile1)
                {
                    if (nextFile1.Name == nextFile.Name)
                    {
                        StreamReader str1 = new StreamReader(nextFile1.FullName);
                        string line1 = "";
                        while ((line1 = str1.ReadLine()) != null)
                        {
                            Lis_Temp.Add(line1);
                        }
                    }
                }
                string destPath = System.IO.Path.Combine(targetPath, nextFile.Name);
                StreamWriter stwiter = new StreamWriter(destPath);
                foreach (string a in Lis_Temp)
                {
                    stwiter.WriteLine(a);
                }
                stwiter.Close();
            }
            MessageBox.Show("OK");
        }

        private void button48_Click(object sender, EventArgs e)
        {
            DirectoryInfo theFolder = new DirectoryInfo(@"F:\2018-2021LNG和Container处理_22_7_11\2018_船舶轨迹_Container_时间排序_经停港口\2018-01");
            string targetPath = @"F:\2018-2021LNG和Container处理_22_7_11\2018_船舶轨迹_Container_时间排序_经停港口\2018-01分天";
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {

                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp1 = line.Split('|');
                    if (arrtemp1[0] == "IMO")
                    {
                        continue;
                    }
                    else if (arrtemp1.Count() > 1)
                    {
                        string[] arrtemp2 = arrtemp1[2].Split(' ');
                        string destPath2 = System.IO.Path.Combine(targetPath, arrtemp2[0]);
                        if (Directory.Exists(destPath2) == false)
                        {
                            Directory.CreateDirectory(destPath2);
                        }

                        string destPath3 = System.IO.Path.Combine(destPath2, nextFile.Name);
                        FileStream fs = new FileStream(destPath3, FileMode.Append);
                        StreamWriter streamwriter = new StreamWriter(fs);
                        streamwriter.WriteLine(line);
                        streamwriter.Close();

                    }
                }
            }

            MessageBox.Show("OK");
        }
        //船舶状态按季度合并
        private void button49_Click(object sender, EventArgs e)
        {
            string targetPath = @"F:\2018-2021LNG处理_LNG基础数据更新\2018_LNG_经停港口_IN唯一港_船舶状态_按季度合并\5-8月";
            DirectoryInfo theFolder = new DirectoryInfo(@"F:\2018-2021LNG处理_LNG基础数据更新\2018_LNG_经停港口_IN唯一港_船舶状态\5-8月");
            DirectoryInfo[] dirFolder = theFolder.GetDirectories();
            foreach (DirectoryInfo nextFolder in dirFolder)
            {
                DirectoryInfo[] dirFolder1 = nextFolder.GetDirectories();
                foreach (DirectoryInfo nextFolder1 in dirFolder1)
                {
                    FileInfo[] dirFile = nextFolder1.GetFiles();
                    foreach (FileInfo nextFile in dirFile)
                    {
                        StreamReader SR = new StreamReader(nextFile.FullName);
                        string line = "";
                        while ((line = SR.ReadLine()) != null)
                        {
                            string destPath = System.IO.Path.Combine(targetPath, nextFile.Name);
                            FileStream fs = new FileStream(destPath, FileMode.Append);
                            StreamWriter SW = new StreamWriter(fs);
                            SW.WriteLine(line);
                            SW.Close();
                        }

                    }
                }
            }
            MessageBox.Show("ok");
        }
        //因未解出字段为空导致字段不一致，因此在两个竖杠之间插入空格
        private void button50_Click(object sender, EventArgs e)
        {
            string targetPath = @"E:\2018-2021LNG和Container数据_Decoder_处理字段长度\2021";
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021");
            DirectoryInfo[] dirFolder = theFolder.GetDirectories();
            foreach (DirectoryInfo nextFolder in dirFolder)
            {
                string destPath = System.IO.Path.Combine(targetPath, nextFolder.Name);
                if (Directory.Exists(destPath) == false)
                {
                    Directory.CreateDirectory(destPath);
                }
                FileInfo[] dirFile = nextFolder.GetFiles();
                foreach (FileInfo nextFile in dirFile)
                {
                    string destPath1 = System.IO.Path.Combine(destPath, nextFile.Name);
                    StreamWriter streamwriter = new StreamWriter(destPath1);
                    string line1 = "";
                    StreamReader streamreader = new StreamReader(nextFile.FullName);
                    while ((line1 = streamreader.ReadLine()) != null)
                    {
                        string line2 = line1.Replace("||", "| |");
                        streamwriter.WriteLine(line2);
                    }
                    streamwriter.Close();
                }
            }
            MessageBox.Show("ok");
        }
        //统计船舶在各个港口之间的航行时间_2022_9_24处理
        //2022年9月24日：2020年5-8月、2020年9-12月、2021年5-8月的数据要重新处理
        //可能是基础数据更新之后，数据的组织形式发生了变化
        private void button51_Click(object sender, EventArgs e)
        {

            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG\2021_LNG_Sailing_between_Ports.csv";
            string targetPath = @"F:\2018-2021LNG处理_LNG基础数据更新\Sailing_between_Ports(2018-2021)_LNG\2020_LNG_Sailing_between_Ports_9-12.csv";
            StreamWriter streamwriter = new StreamWriter(targetPath);
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\2021_轨迹_时间排序_经停港口_删异_IN唯一港_船舶状态_Motifs_删异1_同状态合并_时间序列_时间指标统计_LNG");
            DirectoryInfo theFolder = new DirectoryInfo(@"F:\2018-2021LNG处理_LNG基础数据更新\2020_LNG_经停港口_IN唯一港_船舶状态_按季度合并_Motifs_删异_同状态合并_时间序列_时间指标统计\9-12月");
            FileInfo[] dirFile = theFolder.GetFiles();
            //定义字符串，存储前一行

            foreach (FileInfo nextFile in dirFile)
            {
                string[] arrtemp0 = nextFile.Name.Split('.');
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string exLine = "";
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split('|');
                    string[] arrtemp1 = arrtemp[0].Split('_');
                    if (arrtemp[0].Contains("Stop_between"))
                    {
                        continue;
                    }
                    else
                    {
                        if (arrtemp1.Count() == 7)
                        {
                            if (arrtemp1[2] == arrtemp1[6])
                            {
                                exLine = line;
                            }
                            if (arrtemp1[2] != arrtemp1[6])
                            {
                                string[] arrtemp2 = exLine.Split('|');
                                streamwriter.WriteLine(arrtemp0[0] + '|' + arrtemp1[2] + '|' + arrtemp1[6] + '|' + arrtemp2[2] + '|' + arrtemp[1]);
                                exLine = line;
                            }
                        }
                        if (arrtemp1.Count() == 9)
                        {
                            //if (arrtemp1[2] == arrtemp1[7])
                            //{
                            //    exLine = line;
                            //}
                            //if (arrtemp1[2] != arrtemp1[7])
                            //{
                            //    string[] arrtemp2 = exLine.Split('|');
                            //    streamwriter.WriteLine(arrtemp0[0] + '|' + arrtemp1[2] + '|' + arrtemp1[7] + '|' + arrtemp2[2] + '|' + arrtemp[1]);
                            //    exLine = line;
                            //}
                            //2022年9月24日修改
                            if (arrtemp1[2] == arrtemp1[7] && arrtemp1[3] == arrtemp1[8])
                            {
                                exLine = line;
                            }
                            if (arrtemp1[2] == arrtemp1[7] && isNumberic(arrtemp1[3]))
                            {
                                exLine = line;
                            }
                            if (arrtemp1[2] != arrtemp1[7] && isNumberic(arrtemp1[3]))
                            {
                                string[] arrtemp2 = exLine.Split('|');
                                streamwriter.WriteLine(arrtemp0[0] + '|' + arrtemp1[2] + '|' + arrtemp1[7] + '|' + arrtemp2[2] + '|' + arrtemp[1]);
                                exLine = line;
                            }
                            if (arrtemp1[2] == arrtemp1[8] && isNumberic(arrtemp1[3]) == false)
                            {
                                exLine = line;
                            }
                            if (arrtemp1[2] != arrtemp1[8] && isNumberic(arrtemp1[3]) == false && arrtemp1[2] != arrtemp1[7])
                            {
                                string[] arrtemp2 = exLine.Split('|');
                                streamwriter.WriteLine(arrtemp0[0] + '|' + arrtemp1[2] + '|' + arrtemp1[8] + '|' + arrtemp2[2] + '|' + arrtemp[1]);
                                exLine = line;
                            }
                        }
                        //2022年9月24日增加，字符数为11的情况
                        if (arrtemp1.Count() == 11)
                        {
                            if (arrtemp1[2] == arrtemp1[8])
                            {
                                exLine = line;
                            }
                            if (arrtemp1[2] != arrtemp1[8])
                            {
                                string[] arrtemp2 = exLine.Split('|');
                                streamwriter.WriteLine(arrtemp0[0] + '|' + arrtemp1[2] + '|' + arrtemp1[8] + '|' + arrtemp2[2] + '|' + arrtemp[1]);
                                exLine = line;
                            }
                        }


                        if (arrtemp1.Count() == 8 && IsNumeric(arrtemp1[3]))
                        {
                            if (arrtemp1[2] == arrtemp1[7])
                            {
                                exLine = line;
                            }
                            if (arrtemp1[2] != arrtemp1[7])
                            {
                                string[] arrtemp2 = exLine.Split('|');
                                streamwriter.WriteLine(arrtemp0[0] + '|' + arrtemp1[2] + '|' + arrtemp1[7] + '|' + arrtemp2[2] + '|' + arrtemp[1]);
                                exLine = line;
                            }
                        }
                        if (arrtemp1.Count() == 8 && IsNumeric(arrtemp1[3]) == false)
                        {
                            if (arrtemp1[2] == arrtemp1[6])
                            {
                                exLine = line;
                            }
                            if (arrtemp1[2] != arrtemp1[6])
                            {
                                string[] arrtemp2 = exLine.Split('|');
                                streamwriter.WriteLine(arrtemp0[0] + '|' + arrtemp1[2] + '|' + arrtemp1[6] + '|' + arrtemp2[2] + '|' + arrtemp[1]);
                                exLine = line;
                            }
                        }
                    }
                }
            }
            streamwriter.Close();
            MessageBox.Show("OK");
        }
        //数据连接船名库
        private void button52_Click(object sender, EventArgs e)
        {
            #region-----定义字典存储船名库信息
            string inPath1 = @"F:\2018-2021LNG和Container处理_22_7_11\船名录2015_1.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            string line1 = "";
            //定义字典存储LNG船舶的IMO，及其船舶信息
            Dictionary<string, string> Dic_ShipIfo = new Dictionary<string, string>();
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp1 = line1.Split('|');
                if (arrtemp1[1] == "imo")
                {
                    continue;
                }
                else
                {
                    Dic_ShipIfo.Add(arrtemp1[1], line1);
                }
            }
            #endregion
            DirectoryInfo theFolder = new DirectoryInfo(@"F:\2018-2021LNG和Container处理_22_7_11\2018_船舶轨迹_Container副本1");
            string targetPath = @"F:\2018-2021LNG和Container处理_22_7_11\2018_船舶轨迹_Container_匹配船舶信息";
            DirectoryInfo[] dirFolder = theFolder.GetDirectories();
            foreach (DirectoryInfo nextFolder in dirFolder)
            {
                string destPath1 = System.IO.Path.Combine(targetPath, nextFolder.Name);
                if (Directory.Exists(destPath1) == false)
                {
                    Directory.CreateDirectory(destPath1);
                }
                DirectoryInfo[] dirFolder1 = nextFolder.GetDirectories();
                foreach (DirectoryInfo nextFolder1 in dirFolder1)
                {

                    string destPath2 = System.IO.Path.Combine(destPath1, nextFolder1.Name);
                    if (Directory.Exists(destPath2) == false)
                    {
                        Directory.CreateDirectory(destPath2);
                    }
                    FileInfo[] dirFile = nextFolder1.GetFiles();
                    foreach (FileInfo nextFile in dirFile)
                    {
                        StreamReader streamreader = new StreamReader(nextFile.FullName);
                        string destPath = System.IO.Path.Combine(destPath2, nextFile.Name);
                        StreamWriter streamwriter = new StreamWriter(destPath);
                        string line = "";
                        while ((line = streamreader.ReadLine()) != null)
                        {
                            string[] arrtemp = line.Split('|');
                            foreach (KeyValuePair<string, string> kvp1 in Dic_ShipIfo)
                            {
                                if (arrtemp[0] == kvp1.Key)
                                {
                                    streamwriter.WriteLine(line + '|' + kvp1.Value);
                                }
                            }
                        }
                        streamwriter.Close();
                    }

                }
            }

            MessageBox.Show("OK");
        }
        //2022_12_12对优化前后的网络进行对比，分析不同的航线分别节约了多少时间
        //2022_2_22对优化前后的网络进行对比，分析不同的航线分别节约了多少时间
        //2022_2_22没有考虑不同尺寸船舶的发动机的功率差异
        // //可以根据Lingo计算出的结果，将其读入的文件还原，然后通过前后对比，从而计算其节约的时间
        //还要考虑同一种类型的船不止部署了一次的情况，所以需要根据LINGO计算出的X和A值，得到优化后的
        //LINGO部署的时候不存在同一条船重复部署的情况，所以在计算优化之前的结果不能将所有的叠加，而是要统计最大的值，因此改程序需要修改（2022.02.04）
        private void button53_Click(object sender, EventArgs e)
        {
            #region----根据船名库，统计船舶的吨位
            string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\原始LNG船舶静态数据.csv";
            StreamReader streamreader = new StreamReader(inPath);
            string line = "";
            //定义字典存储LNG船舶的MMSI，及其吨位
            Dictionary<string, double> Dic_LNG = new Dictionary<string, double>();
            while ((line = streamreader.ReadLine()) != null)
            {
                string[] arrtemp = line.Split(',');
                if (arrtemp[8] == "" || arrtemp[8].Contains("MMSI"))
                {
                    continue;
                }
                else
                {
                    if (arrtemp[3] != "" && arrtemp[8] != "0")
                    {
                        Dic_LNG.Add(arrtemp[8], Convert.ToDouble(arrtemp[3]));
                    }
                    //用总吨位的百分之八十估算船舶的载重吨
                    else if (arrtemp[10] != "" && arrtemp[8] != "0")
                    {
                        double dou_temp = Convert.ToDouble(arrtemp[10]) * 4 / 5;
                        Dic_LNG.Add(arrtemp[8], dou_temp);
                    }

                }
            }
            #endregion

            //先根据原数据计算每条航线上所有船舶的航行时间和等待时间的总和
            //定义字典存储原始的每条航线上每个MMSI对应船舶的最大航行时间和等待时间
            Dictionary<string, Dictionary<string, double>> Ave_SaiTime_Ori = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<string, double>> Ave_WaiTime_Ori = new Dictionary<string, Dictionary<string, double>>();
            #region
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_添加时长\2021_LNG_Sailing_between_Ports.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp = line1.Split('|');
                if (Ave_SaiTime_Ori.ContainsKey(arrtemp[1] + '|' + arrtemp[2]))
                {
                    //这是计算总和的方式
                    // Ave_Time_Ori[arrtemp[1] + '|' + arrtemp[2]] += Convert.ToDouble(arrtemp[5]);
                    if (Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]].ContainsKey(arrtemp[0]))
                    {
                        //这是计算最大值的方式
                        //if (Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] <= Convert.ToDouble(arrtemp[5]))
                        //{
                        //    Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] = Convert.ToDouble(arrtemp[5]);
                        //}
                        //这是计算总和的方式
                        Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] += Convert.ToDouble(arrtemp[5]) * Convert.ToDouble(arrtemp[5]);

                    }
                    else
                    {
                        Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]].Add(arrtemp[0], Convert.ToDouble(arrtemp[5]) * Convert.ToDouble(arrtemp[5]));
                    }

                }
                else
                {
                    Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                    dic_temp.Add(arrtemp[0], Convert.ToDouble(arrtemp[5]) * Convert.ToDouble(arrtemp[5]));
                    Ave_SaiTime_Ori.Add(arrtemp[1] + '|' + arrtemp[2], dic_temp);
                }
            }
            string inPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast\2021_LNG_Waiting_between_Ports.csv";
            StreamReader streamreader2 = new StreamReader(inPath2);
            string line2 = "";
            while ((line2 = streamreader2.ReadLine()) != null)
            {
                string[] arrtemp = line2.Split('|');
                if (Ave_WaiTime_Ori.ContainsKey(arrtemp[1] + '|' + arrtemp[2]))
                {
                    //这是计算总和的方式
                    // Ave_Time_Ori[arrtemp[1] + '|' + arrtemp[2]] += Convert.ToDouble(arrtemp[5]);
                    if (Ave_WaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]].ContainsKey(arrtemp[0]))
                    {
                        //这是计算最大值的方式
                        //if (Ave_WaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] <= Convert.ToDouble(arrtemp[5]))
                        //{
                        //    Ave_WaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] = Convert.ToDouble(arrtemp[5]);
                        //}
                        Ave_WaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] += Convert.ToDouble(arrtemp[5]);
                    }
                    else
                    {
                        Ave_WaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]].Add(arrtemp[0], Convert.ToDouble(arrtemp[5]));
                    }

                }
                else
                {
                    Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                    dic_temp.Add(arrtemp[0], Convert.ToDouble(arrtemp[5]));
                    Ave_WaiTime_Ori.Add(arrtemp[1] + '|' + arrtemp[2], dic_temp);
                }
            }
            //将原始的每条航线上对应的每个MMSI船舶的航行时间与等待时间合并
            //2022_12_13日修改不需要合并
            //2022_12_13还是定义两个字典来保存加了发动机功率之后的           
            Dictionary<string, Dictionary<string, double>> Ave_SailTime_Or_1 = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<string, double>> Ave_WaitTime_Or_1 = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, double> Ave_SailTime_Tatal_Ori = new Dictionary<string, double>();
            Dictionary<string, double> Ave_WaitTime_Tatal_Ori = new Dictionary<string, double>();
            foreach (KeyValuePair<string, Dictionary<string, double>> kvp1 in Ave_SaiTime_Ori)
            {
                foreach (KeyValuePair<string, double> kvp11 in kvp1.Value)
                {
                    double kvp11_Value = 0.0;
                    #region----根据吨位赋发动机功率
                    int j = 0;
                    //访问存储船舶吨位的字典
                    foreach (KeyValuePair<string, double> kvpLng in Dic_LNG)
                    {
                        if (kvp11.Key == kvpLng.Key)
                        {
                            //根据船舶的吨位赋予不同的发动机系数
                            if (kvpLng.Value < 55000)
                            {

                                kvp11_Value = 2576 * kvp11.Value;
                                j++;

                            }
                            if (kvpLng.Value >= 55000 && kvpLng.Value < 80000)
                            {

                                kvp11_Value = 14632 * kvp11.Value;
                                j++;

                            }
                            if (kvpLng.Value >= 80000)
                            {

                                kvp11_Value = 33796 * kvp11.Value;
                                j++;
                            }
                        }
                    }
                    if (j == 0)
                    {
                        kvp11_Value = 33796 * kvp11.Value;
                    }
                    #endregion
                    //2022-12-13处理航行时间
                    #region
                    if (Ave_SailTime_Or_1.ContainsKey(kvp1.Key))
                    {
                        if (Ave_SailTime_Or_1[kvp1.Key].ContainsKey(kvp11.Key))
                        {


                            Ave_SailTime_Or_1[kvp1.Key][kvp11.Key] += kvp11_Value;
                        }
                        else
                        {
                            Ave_SailTime_Or_1[kvp1.Key].Add(kvp11.Key, kvp11_Value);
                        }
                    }
                    else
                    {
                        Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                        dic_temp.Add(kvp11.Key, kvp11_Value);
                        Ave_SailTime_Or_1.Add(kvp1.Key, dic_temp);
                    }
                    #endregion

                }

            }
            //处理等待时间

            foreach (KeyValuePair<string, Dictionary<string, double>> kvp2 in Ave_WaiTime_Ori)
            {
                foreach (KeyValuePair<string, double> kvp22 in kvp2.Value)
                {
                    double kvp22_Value = 0.0;
                    int j = 0;
                    #region----根据吨位赋发动机功率
                    //访问存储船舶吨位的字典
                    foreach (KeyValuePair<string, double> kvpLng in Dic_LNG)
                    {
                        if (kvp22.Key == kvpLng.Key)
                        {
                            //根据船舶的吨位赋予不同的发动机系数
                            if (kvpLng.Value < 55000)
                            {


                                kvp22_Value = 2676 * kvp22.Value;
                                j++;

                            }
                            if (kvpLng.Value >= 55000 && kvpLng.Value < 80000)
                            {


                                kvp22_Value = 14732 * kvp22.Value;
                                j++;

                            }
                            if (kvpLng.Value >= 80000)
                            {


                                kvp22_Value = 33266 * kvp22.Value;
                                j++;
                            }
                        }
                    }
                    #endregion
                    if (j == 0)
                    {
                        kvp22_Value = 33266 * kvp22.Value;
                    }
                    //2022-12-13处理等待时间
                    #region
                    if (Ave_WaitTime_Or_1.ContainsKey(kvp2.Key))
                    {
                        if (Ave_WaitTime_Or_1[kvp2.Key].ContainsKey(kvp22.Key))
                        {

                            Ave_WaitTime_Or_1[kvp2.Key][kvp22.Key] += kvp22_Value;
                        }
                        else
                        {
                            Ave_WaitTime_Or_1[kvp2.Key].Add(kvp22.Key, kvp22_Value);
                        }
                    }
                    else
                    {
                        Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                        dic_temp.Add(kvp22.Key, kvp22_Value);
                        Ave_WaitTime_Or_1.Add(kvp2.Key, dic_temp);
                    }
                    #endregion
                }

            }
            //对于有些在船舶航行时间和等待时间中没有MMSI的将其补齐？？？



            foreach (KeyValuePair<string, Dictionary<string, double>> kvp1 in Ave_SailTime_Or_1)
            {
                double sum = 0.0;
                foreach (KeyValuePair<string, double> kvp2 in kvp1.Value)
                {

                    sum += kvp2.Value;
                }
                Ave_SailTime_Tatal_Ori.Add(kvp1.Key, sum);
            }
            foreach (KeyValuePair<string, Dictionary<string, double>> kvp1 in Ave_WaitTime_Or_1)
            {
                double sum = 0.0;
                foreach (KeyValuePair<string, double> kvp2 in kvp1.Value)
                {

                    sum += kvp2.Value;
                }
                Ave_WaitTime_Tatal_Ori.Add(kvp1.Key, sum);
            }
            #endregion
            //处理没有再出口的     
            //根据优化运行后的结果，将原始结果附在其前面，方便比较
            string inPath3 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withoutreexported\2021_AveTime_DeadWeight_ShipTypeMore3_AfterOptimization.csv";
            string destPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_18\Comparision\2021_AveSailTime_DeadWeight_NoneReexported_ShipTypeMore3_Comp.csv";
            string destPath1_1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_18\Comparision\2021_AveWaitTime_DeadWeight_NoneReexported_ShipTypeMore3_Comp.csv";
            StreamReader streamreader3 = new StreamReader(inPath3);
            StreamWriter streamwriter1 = new StreamWriter(destPath1);
            StreamWriter streamwriter1_1 = new StreamWriter(destPath1_1);
            string line3 = "";
            //定义字典存储需要输出的内容
            Dictionary<string, double> Dic_NonReexp_AftOP_Sail = new Dictionary<string, double>();
            Dictionary<string, double> Dic_NonReexp_AftOP_Wait = new Dictionary<string, double>();
            while ((line3 = streamreader3.ReadLine()) != null)
            {
                string[] arrtemp = line3.Split(',');
                if (Dic_NonReexp_AftOP_Sail.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_NonReexp_AftOP_Sail[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[5]);
                }
                else
                {
                    Dic_NonReexp_AftOP_Sail.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[5]));
                }
                if (Dic_NonReexp_AftOP_Wait.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_NonReexp_AftOP_Wait[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[6]);
                }
                else
                {
                    Dic_NonReexp_AftOP_Wait.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[6]));
                }
            }

            //输出的时候先写出原始的，再写出优化后的
            foreach (KeyValuePair<string, double> kvp1 in Dic_NonReexp_AftOP_Sail)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_SailTime_Tatal_Ori)
                {

                    if (kvp1.Key == kvp2.Key)
                    {
                        //double temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        double temp = 0;
                        if (kvp2.Value > kvp1.Value)
                            temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        else temp = (kvp1.Value - kvp2.Value) / kvp1.Value;
                        streamwriter1.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value + '|' + Convert.ToString(temp));
                    }

                }
            }
            foreach (KeyValuePair<string, double> kvp1 in Dic_NonReexp_AftOP_Wait)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_WaitTime_Tatal_Ori)
                {

                    if (kvp1.Key == kvp2.Key)
                    {
                        //double temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        double temp = 0;
                        if (kvp2.Value > kvp1.Value)
                            temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        else temp = (kvp1.Value - kvp2.Value) / kvp1.Value;
                        streamwriter1_1.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value + '|' + Convert.ToString(temp));
                    }

                }
            }
            streamwriter1.Close();
            streamwriter1_1.Close();

            //处理包括再出口的
            //根据优化运行后的结果，将原始结果附在其前面，方便比较
            //string inPath4 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withreexported\2017_AveTime_DeadWeight_ProvideReexported_ShipTypeMore3_Afteroptimization.csv";
            string inPath4 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withreexported\2021_AveTime_DeadWeight_ProvideReexported_ShipTypeMore3_Afteroptimization.csv";
            string destPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_18\Comparision\2021_AveSailTime_DeadWeight_ProvideReexported_ShipTypeMore3_Comp.csv";
            string destPath2_2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_18\Comparision\2021_AveWaitTime_DeadWeight_ProvideReexported_ShipTypeMore3_Comp.csv";
            StreamReader streamreader4 = new StreamReader(inPath4);
            StreamWriter streamwriter2 = new StreamWriter(destPath2);
            StreamWriter streamwriter2_2 = new StreamWriter(destPath2_2);
            string line4 = "";
            //定义字典存储需要输出的内容
            Dictionary<string, double> Dic_ProReexp_AftOP_Sai = new Dictionary<string, double>();
            Dictionary<string, double> Dic_ProReexp_AftOP_Wai = new Dictionary<string, double>();
            while ((line4 = streamreader4.ReadLine()) != null)
            {
                string[] arrtemp = line4.Split(',');
                if (Dic_ProReexp_AftOP_Sai.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_ProReexp_AftOP_Sai[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[5]);
                }
                else
                {
                    Dic_ProReexp_AftOP_Sai.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[5]));
                }
                if (Dic_ProReexp_AftOP_Wai.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_ProReexp_AftOP_Wai[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[6]);
                }
                else
                {
                    Dic_ProReexp_AftOP_Wai.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[6]));
                }
            }

            //输出的时候先写出原始的，再写出优化后的
            foreach (KeyValuePair<string, double> kvp1 in Dic_ProReexp_AftOP_Sai)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_SailTime_Tatal_Ori)
                {
                    if (kvp1.Key == kvp2.Key)
                    {
                        //double temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        double temp = 0;
                        if (kvp2.Value > kvp1.Value)
                            temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        else temp = (kvp1.Value - kvp2.Value) / kvp1.Value;
                        streamwriter2.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value + '|' + Convert.ToString(temp));
                    }
                }
            }
            foreach (KeyValuePair<string, double> kvp1 in Dic_ProReexp_AftOP_Wai)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_WaitTime_Tatal_Ori)
                {
                    if (kvp1.Key == kvp2.Key)
                    {
                        // double temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        double temp = 0;
                        if (kvp2.Value > kvp1.Value)
                            temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        else temp = (kvp1.Value - kvp2.Value) / kvp1.Value;
                        streamwriter2_2.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value + '|' + Convert.ToString(temp));
                    }
                }
            }
            streamwriter2.Close();
            streamwriter2_2.Close();

            string inPath5 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withreexported\2021_AveTime_DeadWeight_ReceiveReexported_ShipTypeMore3_Afteroptimization.csv";
            string destPath3 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_18\Comparision\2021_AveSailTime_DeadWeight_ReceiveReexported_ShipTypeMore3_Comp.csv";
            string destPath3_3 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_18\Comparision\2021_AveWaitTime_DeadWeight_ReceiveReexported_ShipTypeMore3_Comp.csv";
            StreamReader streamreader5 = new StreamReader(inPath5);
            StreamWriter streamwriter3 = new StreamWriter(destPath3);
            StreamWriter streamwriter3_3 = new StreamWriter(destPath3_3);
            string line5 = "";
            //定义字典存储需要输出的内容
            Dictionary<string, double> Dic_RecReexp_AftOP_Sai = new Dictionary<string, double>();
            Dictionary<string, double> Dic_RecReexp_AftOP_Wai = new Dictionary<string, double>();
            while ((line5 = streamreader5.ReadLine()) != null)
            {
                string[] arrtemp = line5.Split(',');
                if (Dic_RecReexp_AftOP_Sai.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_RecReexp_AftOP_Sai[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[5]);
                }
                else
                {
                    Dic_RecReexp_AftOP_Sai.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[5]));
                }
                if (Dic_RecReexp_AftOP_Wai.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_RecReexp_AftOP_Wai[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[6]);
                }
                else
                {
                    Dic_RecReexp_AftOP_Wai.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[6]));
                }
            }

            //输出的时候先写出原始的，再写出优化后的
            foreach (KeyValuePair<string, double> kvp1 in Dic_RecReexp_AftOP_Sai)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_SailTime_Tatal_Ori)
                {
                    if (kvp1.Key == kvp2.Key)
                    {
                        //double temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        double temp = 0;
                        if (kvp2.Value > kvp1.Value)
                            temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        else temp = (kvp1.Value - kvp2.Value) / kvp1.Value;
                        streamwriter3.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value + '|' + Convert.ToString(temp));
                    }
                }
            }
            foreach (KeyValuePair<string, double> kvp1 in Dic_RecReexp_AftOP_Wai)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_WaitTime_Tatal_Ori)
                {
                    if (kvp1.Key == kvp2.Key)
                    {
                        //double temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        double temp = 0;
                        if (kvp2.Value > kvp1.Value)
                            temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        else temp = (kvp1.Value - kvp2.Value) / kvp1.Value;
                        streamwriter3_3.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value + '|' + Convert.ToString(temp));
                    }
                }
            }
            streamwriter3.Close();
            streamwriter3_3.Close();
            string inPath6 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withreexported\2021_AveTime_DeadWeight_Reexported_ShipTypeMore2_1_Afteroptimization.csv";
            string destPath4 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_18\Comparision\2021_AveSailTime_DeadWeight_Reexported_ShipTypeMore2_1_Comp.csv";
            string destPath4_4 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_18\Comparision\2021_AveWaitTime_DeadWeight_Reexported_ShipTypeMore2_1_Comp.csv";
            StreamReader streamreader6 = new StreamReader(inPath6);
            StreamWriter streamwriter4 = new StreamWriter(destPath4);
            StreamWriter streamwriter4_4 = new StreamWriter(destPath4_4);
            string line6 = "";
            //定义字典存储需要输出的内容
            Dictionary<string, double> Dic_Reexp_AftOP_Sail = new Dictionary<string, double>();
            Dictionary<string, double> Dic_Reexp_AftOP_Wait = new Dictionary<string, double>();
            while ((line6 = streamreader6.ReadLine()) != null)
            {
                string[] arrtemp = line6.Split(',');
                if (Dic_Reexp_AftOP_Sail.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_Reexp_AftOP_Sail[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[5]);
                }
                else
                {
                    Dic_Reexp_AftOP_Sail.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[5]));
                }
                if (Dic_Reexp_AftOP_Wait.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_Reexp_AftOP_Wait[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[6]);
                }
                else
                {
                    Dic_Reexp_AftOP_Wait.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[6]));
                }
            }

            //输出的时候先写出原始的，再写出优化后的
            foreach (KeyValuePair<string, double> kvp1 in Dic_Reexp_AftOP_Sail)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_SailTime_Tatal_Ori)
                {
                    if (kvp1.Key == kvp2.Key)
                    {
                        //double temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        double temp = 0;
                        if (kvp2.Value > kvp1.Value)
                            temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        else temp = (kvp1.Value - kvp2.Value) / kvp1.Value;
                        streamwriter4.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value + '|' + Convert.ToString(temp));
                    }
                }
            }
            foreach (KeyValuePair<string, double> kvp1 in Dic_Reexp_AftOP_Sail)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_WaitTime_Tatal_Ori)
                {
                    if (kvp1.Key == kvp2.Key)
                    {
                        //double temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        double temp = 0;
                        if (kvp2.Value > kvp1.Value)
                            temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        else temp = (kvp1.Value - kvp2.Value) / kvp1.Value;
                        streamwriter4_4.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value + '|' + Convert.ToString(temp));
                    }
                }
            }
            streamwriter4.Close();
            streamwriter4_4.Close();
            MessageBox.Show("OK");
        }
        //2022-12-13对网络优化后的结果处理
        private void button54_Click(object sender, EventArgs e)
        {
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withoutreexported\2017_AveTime_DeadWeight_ShipTypeMore3_Afteroptimization.csv";
            string inPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\作图2014-2017\作图\WithoutReexport\AveSaiTime_DeadWeight.csv";
            string inPath3 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\作图2014-2017\作图\WithoutReexport\AveWaiTime_DeadWeight.csv";
            string outPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withoutreexported\2017_AveSailTime_DeadWeight_ShipTypeMore3.csv";
            string outPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withoutreexported\2017_AveWaitTime_DeadWeight_ShipTypeMore3.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            StreamReader streamreader2 = new StreamReader(inPath2);
            StreamReader streamreader3 = new StreamReader(inPath3);
            StreamWriter streamwriter1 = new StreamWriter(outPath1);
            StreamWriter streamwriter2 = new StreamWriter(outPath2);
            string line1 = "";
            string line2 = "";
            string line3 = "";
            //定义字典存储航线，以及船舶的吨位等级
            Dictionary<string, List<string>> Dic1 = new Dictionary<string, List<string>>();
            //定义字典存储不同航线，不同吨位等级的船舶的航行时间
            Dictionary<string, Dictionary<string, string>> Dic1_SaiTime = new Dictionary<string, Dictionary<string, string>>();
            //定义字典存储不同航线，不同吨位等级的船舶的等待时间
            Dictionary<string, Dictionary<string, string>> Dic1_WaiTime = new Dictionary<string, Dictionary<string, string>>();
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp = line1.Split(',');
                if (Dic1.ContainsKey(arrtemp[0] + ',' + arrtemp[1]))
                {
                    Dic1[arrtemp[0] + ',' + arrtemp[1]].Add(arrtemp[2]);
                }
                else
                {
                    List<string> Lis_temp = new List<string>();
                    Lis_temp.Add(arrtemp[2]);
                    Dic1.Add(arrtemp[0] + ',' + arrtemp[1], Lis_temp);
                }
            }
            //1000000000
            #region
            //定义字典存储不同航线，不同吨位等级的船舶的航行时间
            Dictionary<string, Dictionary<string, string>> Dic1_SaiTime1 = new Dictionary<string, Dictionary<string, string>>();
            //定义字典存储不同航线，不同吨位等级的船舶的等待时间
            Dictionary<string, Dictionary<string, string>> Dic1_WaiTime1 = new Dictionary<string, Dictionary<string, string>>();
            while ((line2 = streamreader2.ReadLine()) != null)
            {
                string[] arrtemp1 = line2.Split('|');

                if (Dic1_SaiTime1.ContainsKey(arrtemp1[0] + ',' + arrtemp1[1]))
                {
                    Dic1_SaiTime1[arrtemp1[0] + ',' + arrtemp1[1]].Add(arrtemp1[2], arrtemp1[3]);
                }
                else
                {
                    Dictionary<string, string> Dic_temp = new Dictionary<string, string>();
                    Dic_temp.Add(arrtemp1[2], arrtemp1[3]);
                    Dic1_SaiTime1.Add(arrtemp1[0] + ',' + arrtemp1[1], Dic_temp);
                }

            }

            while ((line3 = streamreader3.ReadLine()) != null)
            {
                string[] arrtemp1 = line3.Split('|');

                if (Dic1_WaiTime1.ContainsKey(arrtemp1[0] + ',' + arrtemp1[1]))
                {
                    Dic1_WaiTime1[arrtemp1[0] + ',' + arrtemp1[1]].Add(arrtemp1[2], arrtemp1[3]);
                }
                else
                {
                    Dictionary<string, string> Dic_temp = new Dictionary<string, string>();
                    Dic_temp.Add(arrtemp1[2], arrtemp1[3]);
                    Dic1_WaiTime1.Add(arrtemp1[0] + ',' + arrtemp1[1], Dic_temp);
                }

            }
            #endregion
            #region---处理航行时间
            foreach (KeyValuePair<string, List<string>> kvp1 in Dic1)
            {
                foreach (string a in kvp1.Value)
                {
                    int j = 0;
                    foreach (KeyValuePair<string, Dictionary<string, string>> kvp11 in Dic1_SaiTime1)
                    {
                        foreach (KeyValuePair<string, string> kvp12 in kvp11.Value)
                        {
                            if (kvp1.Key == kvp11.Key && kvp12.Key == a)
                            {
                                j++;
                                if (Dic1_SaiTime.ContainsKey(kvp1.Key))
                                {
                                    Dic1_SaiTime[kvp1.Key].Add(kvp12.Key, kvp12.Value);
                                }
                                else
                                {
                                    Dictionary<string, string> Dic_temp = new Dictionary<string, string>();
                                    Dic_temp.Add(kvp12.Key, kvp12.Value);
                                    Dic1_SaiTime.Add(kvp1.Key, Dic_temp);
                                }
                            }

                        }
                    }
                    if (j == 0)
                    {
                        if (Dic1_SaiTime.ContainsKey(kvp1.Key))
                        {
                            Dic1_SaiTime[kvp1.Key].Add(a, "1000000000");
                        }
                        else
                        {
                            Dictionary<string, string> Dic_temp = new Dictionary<string, string>();
                            Dic_temp.Add(a, "1000000000");
                            Dic1_SaiTime.Add(kvp1.Key, Dic_temp);
                        }
                    }
                }
            }
            #endregion
            #region----处理等待时间
            foreach (KeyValuePair<string, List<string>> kvp1 in Dic1)
            {
                foreach (string a in kvp1.Value)
                {
                    int j = 0;
                    foreach (KeyValuePair<string, Dictionary<string, string>> kvp11 in Dic1_WaiTime1)
                    {
                        foreach (KeyValuePair<string, string> kvp12 in kvp11.Value)
                        {
                            if (kvp1.Key == kvp11.Key && kvp12.Key == a)
                            {
                                j++;
                                if (Dic1_WaiTime.ContainsKey(kvp1.Key))
                                {
                                    Dic1_WaiTime[kvp1.Key].Add(kvp12.Key, kvp12.Value);
                                }
                                else
                                {
                                    Dictionary<string, string> Dic_temp = new Dictionary<string, string>();
                                    Dic_temp.Add(kvp12.Key, kvp12.Value);
                                    Dic1_WaiTime.Add(kvp1.Key, Dic_temp);
                                }
                            }

                        }
                    }
                    if (j == 0)
                    {
                        if (Dic1_WaiTime.ContainsKey(kvp1.Key))
                        {
                            Dic1_WaiTime[kvp1.Key].Add(a, "1000000000");
                        }
                        else
                        {
                            Dictionary<string, string> Dic_temp = new Dictionary<string, string>();
                            Dic_temp.Add(a, "1000000000");
                            Dic1_WaiTime.Add(kvp1.Key, Dic_temp);
                        }
                    }
                }
            }
            #endregion
            //写出航行时间
            foreach (KeyValuePair<string, Dictionary<string, string>> kvp1 in Dic1_SaiTime)
            {
                foreach (KeyValuePair<string, string> kvp2 in kvp1.Value)
                {
                    streamwriter1.WriteLine(kvp1.Key + ',' + kvp2.Key + ',' + kvp2.Value);
                }
            }
            //写出等待时间
            foreach (KeyValuePair<string, Dictionary<string, string>> kvp1 in Dic1_WaiTime)
            {
                foreach (KeyValuePair<string, string> kvp2 in kvp1.Value)
                {
                    streamwriter2.WriteLine(kvp1.Key + ',' + kvp2.Key + ',' + kvp2.Value);
                }
            }
            streamwriter1.Close();
            streamwriter2.Close();
            MessageBox.Show("ok");
        }

        private void button55_Click(object sender, EventArgs e)
        {
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withreexported\2018_AveTime_DeadWeight_ReceiveReexported_ShipTypeMore3_AfterOptimization.csv";
            string inPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveSaiTime_DeadWeight_ReceiveReexported.csv";
            string inPath3 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveWaiTime_DeadWeight_ReceiveReexported.csv";
            string outPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withreexported\2018_AveSaiTime_DeadWeight_ReceiveReexported_ShipTypeMore3_AfterOptimization.csv";
            string outPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withreexported\2018_AveWatTime_DeadWeight_ReceiveReexported_ShipTypeMore3_AfterOptimization.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            StreamReader streamreader2 = new StreamReader(inPath2);
            StreamReader streamreader3 = new StreamReader(inPath3);
            StreamWriter streamwriter1 = new StreamWriter(outPath1);
            StreamWriter streamwriter2 = new StreamWriter(outPath2);
            string line1 = "";
            string line2 = "";
            string line3 = "";
            //定义字典存储航线，以及船舶的吨位等级
            Dictionary<string, List<string>> Dic1 = new Dictionary<string, List<string>>();
            //定义字典存储不同航线，不同吨位等级的船舶的航行时间
            Dictionary<string, Dictionary<string, string>> Dic1_SaiTime = new Dictionary<string, Dictionary<string, string>>();
            //定义字典存储不同航线，不同吨位等级的船舶的等待时间
            Dictionary<string, Dictionary<string, string>> Dic1_WaiTime = new Dictionary<string, Dictionary<string, string>>();
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp = line1.Split(',');
                if (Dic1.ContainsKey(arrtemp[0] + ',' + arrtemp[1]))
                {
                    Dic1[arrtemp[0] + ',' + arrtemp[1]].Add(arrtemp[2]);
                }
                else
                {
                    List<string> Lis_temp = new List<string>();
                    Lis_temp.Add(arrtemp[2]);
                    Dic1.Add(arrtemp[0] + ',' + arrtemp[1], Lis_temp);
                }
            }
            //1000000000
            #region
            //定义字典存储不同航线，不同吨位等级的船舶的航行时间
            Dictionary<string, Dictionary<string, string>> Dic1_SaiTime1 = new Dictionary<string, Dictionary<string, string>>();
            //定义字典存储不同航线，不同吨位等级的船舶的等待时间
            Dictionary<string, Dictionary<string, string>> Dic1_WaiTime1 = new Dictionary<string, Dictionary<string, string>>();
            while ((line2 = streamreader2.ReadLine()) != null)
            {
                string[] arrtemp1 = line2.Split('|');

                if (Dic1_SaiTime1.ContainsKey(arrtemp1[0] + ',' + arrtemp1[1]))
                {
                    Dic1_SaiTime1[arrtemp1[0] + ',' + arrtemp1[1]].Add(arrtemp1[2], arrtemp1[3]);
                }
                else
                {
                    Dictionary<string, string> Dic_temp = new Dictionary<string, string>();
                    Dic_temp.Add(arrtemp1[2], arrtemp1[3]);
                    Dic1_SaiTime1.Add(arrtemp1[0] + ',' + arrtemp1[1], Dic_temp);
                }

            }

            while ((line3 = streamreader3.ReadLine()) != null)
            {
                string[] arrtemp1 = line3.Split('|');

                if (Dic1_WaiTime1.ContainsKey(arrtemp1[0] + ',' + arrtemp1[1]))
                {
                    Dic1_WaiTime1[arrtemp1[0] + ',' + arrtemp1[1]].Add(arrtemp1[2], arrtemp1[3]);
                }
                else
                {
                    Dictionary<string, string> Dic_temp = new Dictionary<string, string>();
                    Dic_temp.Add(arrtemp1[2], arrtemp1[3]);
                    Dic1_WaiTime1.Add(arrtemp1[0] + ',' + arrtemp1[1], Dic_temp);
                }

            }
            #endregion
            #region---处理航行时间
            foreach (KeyValuePair<string, List<string>> kvp1 in Dic1)
            {
                foreach (string a in kvp1.Value)
                {
                    int j = 0;
                    foreach (KeyValuePair<string, Dictionary<string, string>> kvp11 in Dic1_SaiTime1)
                    {
                        foreach (KeyValuePair<string, string> kvp12 in kvp11.Value)
                        {
                            if (kvp1.Key == kvp11.Key && kvp12.Key == a)
                            {
                                j++;
                                if (Dic1_SaiTime.ContainsKey(kvp1.Key))
                                {
                                    Dic1_SaiTime[kvp1.Key].Add(kvp12.Key, kvp12.Value);
                                }
                                else
                                {
                                    Dictionary<string, string> Dic_temp = new Dictionary<string, string>();
                                    Dic_temp.Add(kvp12.Key, kvp12.Value);
                                    Dic1_SaiTime.Add(kvp1.Key, Dic_temp);
                                }
                            }

                        }
                    }
                    if (j == 0)
                    {
                        if (Dic1_SaiTime.ContainsKey(kvp1.Key))
                        {
                            Dic1_SaiTime[kvp1.Key].Add(a, "1000000000");
                        }
                        else
                        {
                            Dictionary<string, string> Dic_temp = new Dictionary<string, string>();
                            Dic_temp.Add(a, "1000000000");
                            Dic1_SaiTime.Add(kvp1.Key, Dic_temp);
                        }
                    }
                }
            }
            #endregion
            #region----处理等待时间
            foreach (KeyValuePair<string, List<string>> kvp1 in Dic1)
            {
                foreach (string a in kvp1.Value)
                {
                    int j = 0;
                    foreach (KeyValuePair<string, Dictionary<string, string>> kvp11 in Dic1_WaiTime1)
                    {
                        foreach (KeyValuePair<string, string> kvp12 in kvp11.Value)
                        {
                            if (kvp1.Key == kvp11.Key && kvp12.Key == a)
                            {
                                j++;
                                if (Dic1_WaiTime.ContainsKey(kvp1.Key))
                                {
                                    Dic1_WaiTime[kvp1.Key].Add(kvp12.Key, kvp12.Value);
                                }
                                else
                                {
                                    Dictionary<string, string> Dic_temp = new Dictionary<string, string>();
                                    Dic_temp.Add(kvp12.Key, kvp12.Value);
                                    Dic1_WaiTime.Add(kvp1.Key, Dic_temp);
                                }
                            }

                        }
                    }
                    if (j == 0)
                    {
                        if (Dic1_WaiTime.ContainsKey(kvp1.Key))
                        {
                            Dic1_WaiTime[kvp1.Key].Add(a, "1000000000");
                        }
                        else
                        {
                            Dictionary<string, string> Dic_temp = new Dictionary<string, string>();
                            Dic_temp.Add(a, "1000000000");
                            Dic1_WaiTime.Add(kvp1.Key, Dic_temp);
                        }
                    }
                }
            }
            #endregion
            //写出航行时间
            foreach (KeyValuePair<string, Dictionary<string, string>> kvp1 in Dic1_SaiTime)
            {
                foreach (KeyValuePair<string, string> kvp2 in kvp1.Value)
                {
                    streamwriter1.WriteLine(kvp1.Key + ',' + kvp2.Key + ',' + kvp2.Value);
                }
            }
            //写出等待时间
            foreach (KeyValuePair<string, Dictionary<string, string>> kvp1 in Dic1_WaiTime)
            {
                foreach (KeyValuePair<string, string> kvp2 in kvp1.Value)
                {
                    streamwriter2.WriteLine(kvp1.Key + ',' + kvp2.Key + ',' + kvp2.Value);
                }
            }
            streamwriter1.Close();
            streamwriter2.Close();
            MessageBox.Show("ok");
        }
        //2022-12-28优化前后的网络对比，分析不同航线分别节约了多少时间
        //2022-12-12之所以算出来的优化后的并没有比优化前的少，是因为有些船舶来回很多趟，而我们用的是均值输入优化软件进行优化的，
        //这就造成了跟原始的有很多短停留相比，反而没有减少。
        private void button56_Click(object sender, EventArgs e)
        {
            #region----根据船名库，统计船舶的吨位
            string inPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\原始LNG船舶静态数据.csv";
            StreamReader streamreader = new StreamReader(inPath);
            string line = "";
            //定义字典存储LNG船舶的MMSI，及其吨位
            Dictionary<string, double> Dic_LNG = new Dictionary<string, double>();
            while ((line = streamreader.ReadLine()) != null)
            {
                string[] arrtemp = line.Split(',');
                //2014-2017年用的MMSI
                //if (arrtemp[8] == "" || arrtemp[8].Contains("MMSI"))
                //{
                //    continue;
                //}
                //else
                //{
                //    if (arrtemp[3] != "" && arrtemp[8] != "0")
                //    {
                //        Dic_LNG.Add(arrtemp[8], Convert.ToDouble(arrtemp[3]));
                //    }
                //    //用总吨位的百分之八十估算船舶的载重吨
                //    else if (arrtemp[10] != "" && arrtemp[8] != "0")
                //    {
                //        double dou_temp = Convert.ToDouble(arrtemp[10]) * 4 / 5;
                //        Dic_LNG.Add(arrtemp[8], dou_temp);
                //    }

                //}
                //2018-2021年用的IMO
                if (arrtemp[5] == "" || arrtemp[5].Contains("IMO"))
                {
                    continue;
                }
                else
                {
                    if (arrtemp[3] != "" && arrtemp[5] != "0")
                    {
                        Dic_LNG.Add(arrtemp[5], Convert.ToDouble(arrtemp[3]));
                    }
                    //用总吨位的百分之八十估算船舶的载重吨
                    else if (arrtemp[10] != "" && arrtemp[5] != "0")
                    {
                        double dou_temp = Convert.ToDouble(arrtemp[10]) * 4 / 5;
                        Dic_LNG.Add(arrtemp[5], dou_temp);
                    }

                }
            }
            #endregion

            //先根据原数据计算每条航线上所有船舶的航行时间和等待时间的总和
            //定义字典存储原始的每条航线上每个MMSI对应船舶的最大航行时间和等待时间
            Dictionary<string, Dictionary<string, double>> Ave_SaiTime_Ori = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<string, double>> Ave_WaiTime_Ori = new Dictionary<string, Dictionary<string, double>>();
            //计算每条航线上分别有哪些船舶航行
            Dictionary<string, List<string>> Ave_SaiTime_Count = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> Ave_WaiTime_Count = new Dictionary<string, List<string>>();
            //
            #region
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_添加时长\2021_LNG_Sailing_between_Ports.csv";
            //string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_添加时长\2017_LNG_Sailing_between_Ports_v1.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp = line1.Split('|');
                if (Ave_SaiTime_Count.ContainsKey(arrtemp[1] + '|' + arrtemp[2]))
                {
                    Ave_SaiTime_Count[arrtemp[1] + '|' + arrtemp[2]].Add(arrtemp[0]);
                }
                else
                {
                    List<string> Lis_Temp = new List<string>();
                    Lis_Temp.Add(arrtemp[0]);
                    Ave_SaiTime_Count.Add(arrtemp[1] + '|' + arrtemp[2], Lis_Temp);
                }
                #region
                //if (Ave_SaiTime_Ori.ContainsKey(arrtemp[1] + '|' + arrtemp[2]))
                //{
                //    //这是计算总和的方式
                //    // Ave_Time_Ori[arrtemp[1] + '|' + arrtemp[2]] += Convert.ToDouble(arrtemp[5]);
                //    if (Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]].ContainsKey(arrtemp[0]))
                //    {
                //        //这是计算最大值的方式
                //        //if (Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] <= Convert.ToDouble(arrtemp[5]))
                //        //{
                //        //    Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] = Convert.ToDouble(arrtemp[5]);
                //        //}
                //        //这是计算总和的方式
                //        Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] += Convert.ToDouble(arrtemp[5]) * Convert.ToDouble(arrtemp[5]);

                //    }
                //    else
                //    {
                //        Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]].Add(arrtemp[0], Convert.ToDouble(arrtemp[5]) * Convert.ToDouble(arrtemp[5]));
                //    }

                //}
                //else
                //{
                //    Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                //    dic_temp.Add(arrtemp[0], Convert.ToDouble(arrtemp[5]) * Convert.ToDouble(arrtemp[5]));
                //    Ave_SaiTime_Ori.Add(arrtemp[1] + '|' + arrtemp[2], dic_temp);
                //}
                #endregion
            }
            string inPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast\2021_LNG_Waiting_between_Ports.csv";
            //string inPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast\2017_LNG_Waiting_between_Ports_v1.csv";
            StreamReader streamreader2 = new StreamReader(inPath2);
            string line2 = "";
            while ((line2 = streamreader2.ReadLine()) != null)
            {
                string[] arrtemp = line2.Split('|');
                if (Ave_WaiTime_Count.ContainsKey(arrtemp[1] + '|' + arrtemp[2]))
                {
                    Ave_WaiTime_Count[arrtemp[1] + '|' + arrtemp[2]].Add(arrtemp[0]);
                }
                else
                {
                    List<string> Lis_Temp = new List<string>();
                    Lis_Temp.Add(arrtemp[0]);
                    Ave_WaiTime_Count.Add(arrtemp[1] + '|' + arrtemp[2], Lis_Temp);
                }
                #region
                //if (Ave_WaiTime_Ori.ContainsKey(arrtemp[1] + '|' + arrtemp[2]))
                //{
                //    //这是计算总和的方式
                //    // Ave_Time_Ori[arrtemp[1] + '|' + arrtemp[2]] += Convert.ToDouble(arrtemp[5]);
                //    if (Ave_WaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]].ContainsKey(arrtemp[0]))
                //    {
                //        //这是计算最大值的方式
                //        //if (Ave_WaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] <= Convert.ToDouble(arrtemp[5]))
                //        //{
                //        //    Ave_WaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] = Convert.ToDouble(arrtemp[5]);
                //        //}
                //        Ave_WaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] += Convert.ToDouble(arrtemp[5]);
                //    }
                //    else
                //    {
                //        Ave_WaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]].Add(arrtemp[0], Convert.ToDouble(arrtemp[5]));
                //    }

                //}
                //else
                //{
                //    Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                //    dic_temp.Add(arrtemp[0], Convert.ToDouble(arrtemp[5]));
                //    Ave_WaiTime_Ori.Add(arrtemp[1] + '|' + arrtemp[2], dic_temp);
                //}
                #endregion
            }
            #region-----定义字典，存储没条航线上不同吨位等级的船舶的平均航行时间
            Dictionary<string, Dictionary<string, double>> Ave_SailTime = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<string, double>> Ave_WaitTime = new Dictionary<string, Dictionary<string, double>>();
            #region---处理航行时间
            string inpath1_1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveSaiTime_DeadWeight_ProvideReexported.csv";
            string inpath1_2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveSaiTime_DeadWeight_ReceiveReexported.csv";
            string inpath1_3 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveSaiTime_DeadWeight_Reexport.csv";
            string inpath1_4 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\AveSaiTime_DeadWeight.csv";
            //string inpath1_1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\作图2014-2017\作图\WithReexport\AveSaiTime_DeadWeight_ProvideReexported.csv";
            //string inpath1_2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\作图2014-2017\作图\WithReexport\AveSaiTime_DeadWeight_ReceiveReexported.csv";
            //string inpath1_3 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\作图2014-2017\作图\WithReexport\AveSaiTime_DeadWeight_Reexport.csv";
            //string inpath1_4 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\作图2014-2017\作图\WithoutReexport\AveSaiTime_DeadWeight.csv";
            StreamReader streamreader1_1 = new StreamReader(inpath1_1);
            StreamReader streamreader1_2 = new StreamReader(inpath1_2);
            StreamReader streamreader1_3 = new StreamReader(inpath1_3);
            StreamReader streamreader1_4 = new StreamReader(inpath1_4);
            string line1_1 = "";
            string line1_2 = "";
            string line1_3 = "";
            string line1_4 = "";
            while ((line1_1 = streamreader1_1.ReadLine()) != null)
            {
                string[] arrtemp = line1_1.Split('|');
                if (Ave_SailTime.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Ave_SailTime[arrtemp[0] + '|' + arrtemp[1]].Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                }
                else
                {
                    Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                    dic_temp.Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                    Ave_SailTime.Add(arrtemp[0] + '|' + arrtemp[1], dic_temp);
                }
            }
            while ((line1_2 = streamreader1_2.ReadLine()) != null)
            {
                string[] arrtemp = line1_2.Split('|');
                if (Ave_SailTime.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    //if(Ave_SailTime[arrtemp[0] + '|' + arrtemp[1]].ContainsKey(arrtemp[2]) ==false)
                    //Ave_SailTime[arrtemp[0] + '|' + arrtemp[1]].Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                    if (Ave_SailTime[arrtemp[0] + '|' + arrtemp[1]].ContainsKey(arrtemp[2]))
                    {
                        if (Ave_SailTime[arrtemp[0] + '|' + arrtemp[1]][arrtemp[2]] <= Convert.ToDouble(arrtemp[3]))
                            Ave_SailTime[arrtemp[0] + '|' + arrtemp[1]][arrtemp[2]] = Convert.ToDouble(arrtemp[3]);
                    }
                    else
                    {
                        Ave_SailTime[arrtemp[0] + '|' + arrtemp[1]].Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                    }
                }
                else
                {
                    Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                    dic_temp.Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                    Ave_SailTime.Add(arrtemp[0] + '|' + arrtemp[1], dic_temp);
                }
            }
            while ((line1_3 = streamreader1_3.ReadLine()) != null)
            {
                string[] arrtemp = line1_3.Split('|');
                if (Ave_SailTime.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Ave_SailTime[arrtemp[0] + '|' + arrtemp[1]].Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                }
                else
                {
                    Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                    dic_temp.Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                    Ave_SailTime.Add(arrtemp[0] + '|' + arrtemp[1], dic_temp);
                }
            }
            while ((line1_4 = streamreader1_4.ReadLine()) != null)
            {
                string[] arrtemp = line1_4.Split('|');
                if (Ave_SailTime.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    //if (Ave_SailTime[arrtemp[0] + '|' + arrtemp[1]].ContainsKey(arrtemp[2]) == false)
                    //    Ave_SailTime[arrtemp[0] + '|' + arrtemp[1]].Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                    if (Ave_SailTime[arrtemp[0] + '|' + arrtemp[1]].ContainsKey(arrtemp[2]))
                    {
                        if (Ave_SailTime[arrtemp[0] + '|' + arrtemp[1]][arrtemp[2]] <= Convert.ToDouble(arrtemp[3]))
                            Ave_SailTime[arrtemp[0] + '|' + arrtemp[1]][arrtemp[2]] = Convert.ToDouble(arrtemp[3]);
                    }
                    else
                    {
                        Ave_SailTime[arrtemp[0] + '|' + arrtemp[1]].Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                    }
                }
                else
                {
                    Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                    dic_temp.Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                    Ave_SailTime.Add(arrtemp[0] + '|' + arrtemp[1], dic_temp);
                }
            }
            #endregion
            #region---处理等待时间
            string inpath2_1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveWaiTime_DeadWeight_ProvideReexported.csv";
            string inpath2_2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveWaiTime_DeadWeight_ReceiveReexported.csv";
            string inpath2_3 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithReexport\AveWaiTime_DeadWeight_Reexport.csv";
            string inpath2_4 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\WithoutReexport\AveWaiTime_DeadWeight.csv";
            //string inpath2_1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\作图2014-2017\作图\WithReexport\AveWatTime_DeadWeight_ProvideReexported.csv";
            //string inpath2_2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\作图2014-2017\作图\WithReexport\AveWatTime_DeadWeight_ReceiveReexported.csv";
            //string inpath2_3 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\作图2014-2017\作图\WithReexport\AveWatTime_DeadWeight_Reexport.csv";
            //string inpath2_4 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\作图2014-2017\作图\WithoutReexport\AveWaiTime_DeadWeight.csv";
            StreamReader streamreader2_1 = new StreamReader(inpath2_1);
            StreamReader streamreader2_2 = new StreamReader(inpath2_2);
            StreamReader streamreader2_3 = new StreamReader(inpath2_3);
            StreamReader streamreader2_4 = new StreamReader(inpath2_4);
            string line2_1 = "";
            string line2_2 = "";
            string line2_3 = "";
            string line2_4 = "";
            while ((line2_1 = streamreader2_1.ReadLine()) != null)
            {
                string[] arrtemp = line2_1.Split('|');
                if (Ave_WaitTime.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Ave_WaitTime[arrtemp[0] + '|' + arrtemp[1]].Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                }
                else
                {
                    Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                    dic_temp.Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                    Ave_WaitTime.Add(arrtemp[0] + '|' + arrtemp[1], dic_temp);
                }
            }
            while ((line2_2 = streamreader2_2.ReadLine()) != null)
            {
                string[] arrtemp = line2_2.Split('|');
                if (Ave_WaitTime.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    //if (Ave_WaitTime[arrtemp[0] + '|' + arrtemp[1]].ContainsKey(arrtemp[2]) == false)
                    //    Ave_WaitTime[arrtemp[0] + '|' + arrtemp[1]].Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                    if (Ave_WaitTime[arrtemp[0] + '|' + arrtemp[1]].ContainsKey(arrtemp[2]))
                    {
                        if (Ave_WaitTime[arrtemp[0] + '|' + arrtemp[1]][arrtemp[2]] <= Convert.ToDouble(arrtemp[3]))
                            Ave_WaitTime[arrtemp[0] + '|' + arrtemp[1]][arrtemp[2]] = Convert.ToDouble(arrtemp[3]);
                    }
                    else
                    {
                        Ave_WaitTime[arrtemp[0] + '|' + arrtemp[1]].Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                    }
                }
                else
                {
                    Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                    dic_temp.Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                    Ave_WaitTime.Add(arrtemp[0] + '|' + arrtemp[1], dic_temp);
                }
            }
            while ((line2_3 = streamreader2_3.ReadLine()) != null)
            {
                string[] arrtemp = line2_3.Split('|');
                if (Ave_WaitTime.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Ave_WaitTime[arrtemp[0] + '|' + arrtemp[1]].Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                }
                else
                {
                    Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                    dic_temp.Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                    Ave_WaitTime.Add(arrtemp[0] + '|' + arrtemp[1], dic_temp);
                }
            }
            while ((line2_4 = streamreader2_4.ReadLine()) != null)
            {
                string[] arrtemp = line2_4.Split('|');
                if (Ave_WaitTime.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    //if (Ave_WaitTime[arrtemp[0] + '|' + arrtemp[1]].ContainsKey(arrtemp[2]) == false)
                    //    Ave_WaitTime[arrtemp[0] + '|' + arrtemp[1]].Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                    if (Ave_WaitTime[arrtemp[0] + '|' + arrtemp[1]].ContainsKey(arrtemp[2]))
                    {
                        if (Ave_WaitTime[arrtemp[0] + '|' + arrtemp[1]][arrtemp[2]] <= Convert.ToDouble(arrtemp[3]))
                            Ave_WaitTime[arrtemp[0] + '|' + arrtemp[1]][arrtemp[2]] = Convert.ToDouble(arrtemp[3]);
                    }
                    else
                    {
                        Ave_WaitTime[arrtemp[0] + '|' + arrtemp[1]].Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                    }
                }
                else
                {
                    Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                    dic_temp.Add(arrtemp[2], Convert.ToDouble(arrtemp[3]));
                    Ave_WaitTime.Add(arrtemp[0] + '|' + arrtemp[1], dic_temp);
                }
            }
            #endregion
            #region---得到优化前每条航线上的航行时间
            foreach (KeyValuePair<string, List<string>> kvp1 in Ave_SaiTime_Count)
            {
                foreach (string a in kvp1.Value)
                {
                    foreach (KeyValuePair<string, double> kvpLng in Dic_LNG)
                    {
                        if (a == kvpLng.Key)
                        {
                            #region
                            if (kvpLng.Value <= 10000)
                            {
                                foreach (KeyValuePair<string, Dictionary<string, double>> kvp1_1 in Ave_SailTime)
                                {
                                    foreach (KeyValuePair<string, double> kvp1_2 in kvp1_1.Value)
                                        if (kvp1.Key == kvp1_1.Key && kvp1_2.Key == "10000")
                                        {
                                            if (Ave_SaiTime_Ori.ContainsKey(kvp1.Key))
                                            {
                                                //这是计算总和的方式
                                                // Ave_Time_Ori[arrtemp[1] + '|' + arrtemp[2]] += Convert.ToDouble(arrtemp[5]);
                                                if (Ave_SaiTime_Ori[kvp1.Key].ContainsKey(a))
                                                {
                                                    //这是计算最大值的方式
                                                    //if (Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] <= Convert.ToDouble(arrtemp[5]))
                                                    //{
                                                    //    Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] = Convert.ToDouble(arrtemp[5]);
                                                    //}
                                                    //这是计算总和的方式
                                                    Ave_SaiTime_Ori[kvp1.Key][a] += Convert.ToDouble(kvp1_2.Value) * Convert.ToDouble(kvp1_2.Value);

                                                }
                                                else
                                                {
                                                    Ave_SaiTime_Ori[kvp1.Key].Add(a, Convert.ToDouble(kvp1_2.Value) * Convert.ToDouble(kvp1_2.Value));
                                                }

                                            }
                                            else
                                            {
                                                Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                                                dic_temp.Add(a, Convert.ToDouble(kvp1_2.Value) * Convert.ToDouble(kvp1_2.Value));
                                                Ave_SaiTime_Ori.Add(kvp1.Key, dic_temp);
                                            }
                                        }
                                }
                            }
                            if (kvpLng.Value <= 55000 && kvpLng.Value > 10000)
                            {
                                foreach (KeyValuePair<string, Dictionary<string, double>> kvp1_1 in Ave_SailTime)
                                {
                                    foreach (KeyValuePair<string, double> kvp1_2 in kvp1_1.Value)
                                        if (kvp1.Key == kvp1_1.Key && kvp1_2.Key == "55000")
                                        {
                                            if (Ave_SaiTime_Ori.ContainsKey(kvp1.Key))
                                            {
                                                //这是计算总和的方式
                                                // Ave_Time_Ori[arrtemp[1] + '|' + arrtemp[2]] += Convert.ToDouble(arrtemp[5]);
                                                if (Ave_SaiTime_Ori[kvp1.Key].ContainsKey(a))
                                                {
                                                    //这是计算最大值的方式
                                                    //if (Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] <= Convert.ToDouble(arrtemp[5]))
                                                    //{
                                                    //    Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] = Convert.ToDouble(arrtemp[5]);
                                                    //}
                                                    //这是计算总和的方式
                                                    Ave_SaiTime_Ori[kvp1.Key][a] += Convert.ToDouble(kvp1_2.Value) * Convert.ToDouble(kvp1_2.Value);

                                                }
                                                else
                                                {
                                                    Ave_SaiTime_Ori[kvp1.Key].Add(a, Convert.ToDouble(kvp1_2.Value) * Convert.ToDouble(kvp1_2.Value));
                                                }

                                            }
                                            else
                                            {
                                                Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                                                dic_temp.Add(a, Convert.ToDouble(kvp1_2.Value) * Convert.ToDouble(kvp1_2.Value));
                                                Ave_SaiTime_Ori.Add(kvp1.Key, dic_temp);
                                            }
                                        }
                                }
                            }
                            if (kvpLng.Value <= 80000 && kvpLng.Value > 55000)
                            {
                                foreach (KeyValuePair<string, Dictionary<string, double>> kvp1_1 in Ave_SailTime)
                                {
                                    foreach (KeyValuePair<string, double> kvp1_2 in kvp1_1.Value)
                                        if (kvp1.Key == kvp1_1.Key && kvp1_2.Key == "80000")
                                        {
                                            if (Ave_SaiTime_Ori.ContainsKey(kvp1.Key))
                                            {
                                                //这是计算总和的方式
                                                // Ave_Time_Ori[arrtemp[1] + '|' + arrtemp[2]] += Convert.ToDouble(arrtemp[5]);
                                                if (Ave_SaiTime_Ori[kvp1.Key].ContainsKey(a))
                                                {
                                                    //这是计算最大值的方式
                                                    //if (Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] <= Convert.ToDouble(arrtemp[5]))
                                                    //{
                                                    //    Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] = Convert.ToDouble(arrtemp[5]);
                                                    //}
                                                    //这是计算总和的方式
                                                    Ave_SaiTime_Ori[kvp1.Key][a] += Convert.ToDouble(kvp1_2.Value) * Convert.ToDouble(kvp1_2.Value);

                                                }
                                                else
                                                {
                                                    Ave_SaiTime_Ori[kvp1.Key].Add(a, Convert.ToDouble(kvp1_2.Value) * Convert.ToDouble(kvp1_2.Value));
                                                }

                                            }
                                            else
                                            {
                                                Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                                                dic_temp.Add(a, Convert.ToDouble(kvp1_2.Value) * Convert.ToDouble(kvp1_2.Value));
                                                Ave_SaiTime_Ori.Add(kvp1.Key, dic_temp);
                                            }
                                        }
                                }
                            }
                            if (kvpLng.Value <= 120000 && kvpLng.Value > 80000)
                            {
                                foreach (KeyValuePair<string, Dictionary<string, double>> kvp1_1 in Ave_SailTime)
                                {
                                    foreach (KeyValuePair<string, double> kvp1_2 in kvp1_1.Value)
                                        if (kvp1.Key == kvp1_1.Key && kvp1_2.Key == "120000")
                                        {
                                            if (Ave_SaiTime_Ori.ContainsKey(kvp1.Key))
                                            {
                                                //这是计算总和的方式
                                                // Ave_Time_Ori[arrtemp[1] + '|' + arrtemp[2]] += Convert.ToDouble(arrtemp[5]);
                                                if (Ave_SaiTime_Ori[kvp1.Key].ContainsKey(a))
                                                {
                                                    //这是计算最大值的方式
                                                    //if (Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] <= Convert.ToDouble(arrtemp[5]))
                                                    //{
                                                    //    Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] = Convert.ToDouble(arrtemp[5]);
                                                    //}
                                                    //这是计算总和的方式
                                                    Ave_SaiTime_Ori[kvp1.Key][a] += Convert.ToDouble(kvp1_2.Value) * Convert.ToDouble(kvp1_2.Value);

                                                }
                                                else
                                                {
                                                    Ave_SaiTime_Ori[kvp1.Key].Add(a, Convert.ToDouble(kvp1_2.Value) * Convert.ToDouble(kvp1_2.Value));
                                                }

                                            }
                                            else
                                            {
                                                Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                                                dic_temp.Add(a, Convert.ToDouble(kvp1_2.Value) * Convert.ToDouble(kvp1_2.Value));
                                                Ave_SaiTime_Ori.Add(kvp1.Key, dic_temp);
                                            }
                                        }
                                }
                            }
                            if (kvpLng.Value > 120000 && kvpLng.Value <= 200000)
                            {
                                foreach (KeyValuePair<string, Dictionary<string, double>> kvp1_1 in Ave_SailTime)
                                {
                                    foreach (KeyValuePair<string, double> kvp1_2 in kvp1_1.Value)
                                        if (kvp1.Key == kvp1_1.Key && kvp1_2.Key == "200000")
                                        {
                                            if (Ave_SaiTime_Ori.ContainsKey(kvp1.Key))
                                            {
                                                //这是计算总和的方式
                                                // Ave_Time_Ori[arrtemp[1] + '|' + arrtemp[2]] += Convert.ToDouble(arrtemp[5]);
                                                if (Ave_SaiTime_Ori[kvp1.Key].ContainsKey(a))
                                                {
                                                    //这是计算最大值的方式
                                                    //if (Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] <= Convert.ToDouble(arrtemp[5]))
                                                    //{
                                                    //    Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] = Convert.ToDouble(arrtemp[5]);
                                                    //}
                                                    //这是计算总和的方式
                                                    Ave_SaiTime_Ori[kvp1.Key][a] += Convert.ToDouble(kvp1_2.Value) * Convert.ToDouble(kvp1_2.Value);

                                                }
                                                else
                                                {
                                                    Ave_SaiTime_Ori[kvp1.Key].Add(a, Convert.ToDouble(kvp1_2.Value) * Convert.ToDouble(kvp1_2.Value));
                                                }

                                            }
                                            else
                                            {
                                                Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                                                dic_temp.Add(a, Convert.ToDouble(kvp1_2.Value) * Convert.ToDouble(kvp1_2.Value));
                                                Ave_SaiTime_Ori.Add(kvp1.Key, dic_temp);
                                            }
                                        }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            #endregion
            #region---得到优化前每条航线上的等待时间
            foreach (KeyValuePair<string, List<string>> kvp1 in Ave_WaiTime_Count)
            {
                foreach (string a in kvp1.Value)
                {
                    foreach (KeyValuePair<string, double> kvpLng in Dic_LNG)
                    {
                        if (a == kvpLng.Key)
                        {
                            #region
                            if (kvpLng.Value <= 10000)
                            {
                                foreach (KeyValuePair<string, Dictionary<string, double>> kvp1_1 in Ave_WaitTime)
                                {
                                    foreach (KeyValuePair<string, double> kvp1_2 in kvp1_1.Value)
                                        if (kvp1.Key == kvp1_1.Key && kvp1_2.Key == "10000")
                                        {
                                            if (Ave_WaiTime_Ori.ContainsKey(kvp1.Key))
                                            {
                                                //这是计算总和的方式
                                                // Ave_Time_Ori[arrtemp[1] + '|' + arrtemp[2]] += Convert.ToDouble(arrtemp[5]);
                                                if (Ave_WaiTime_Ori[kvp1.Key].ContainsKey(a))
                                                {
                                                    //这是计算最大值的方式
                                                    //if (Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] <= Convert.ToDouble(arrtemp[5]))
                                                    //{
                                                    //    Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] = Convert.ToDouble(arrtemp[5]);
                                                    //}
                                                    //这是计算总和的方式
                                                    Ave_WaiTime_Ori[kvp1.Key][a] += Convert.ToDouble(kvp1_2.Value);

                                                }
                                                else
                                                {
                                                    Ave_WaiTime_Ori[kvp1.Key].Add(a, Convert.ToDouble(kvp1_2.Value));
                                                }

                                            }
                                            else
                                            {
                                                Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                                                dic_temp.Add(a, Convert.ToDouble(kvp1_2.Value));
                                                Ave_WaiTime_Ori.Add(kvp1.Key, dic_temp);
                                            }
                                        }
                                }
                            }
                            if (kvpLng.Value <= 55000 && kvpLng.Value > 10000)
                            {
                                foreach (KeyValuePair<string, Dictionary<string, double>> kvp1_1 in Ave_WaitTime)
                                {
                                    foreach (KeyValuePair<string, double> kvp1_2 in kvp1_1.Value)
                                        if (kvp1.Key == kvp1_1.Key && kvp1_2.Key == "55000")
                                        {
                                            if (Ave_WaiTime_Ori.ContainsKey(kvp1.Key))
                                            {
                                                //这是计算总和的方式
                                                // Ave_Time_Ori[arrtemp[1] + '|' + arrtemp[2]] += Convert.ToDouble(arrtemp[5]);
                                                if (Ave_WaiTime_Ori[kvp1.Key].ContainsKey(a))
                                                {
                                                    //这是计算最大值的方式
                                                    //if (Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] <= Convert.ToDouble(arrtemp[5]))
                                                    //{
                                                    //    Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] = Convert.ToDouble(arrtemp[5]);
                                                    //}
                                                    //这是计算总和的方式
                                                    Ave_WaiTime_Ori[kvp1.Key][a] += Convert.ToDouble(kvp1_2.Value);

                                                }
                                                else
                                                {
                                                    Ave_WaiTime_Ori[kvp1.Key].Add(a, Convert.ToDouble(kvp1_2.Value));
                                                }

                                            }
                                            else
                                            {
                                                Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                                                dic_temp.Add(a, Convert.ToDouble(kvp1_2.Value));
                                                Ave_WaiTime_Ori.Add(kvp1.Key, dic_temp);
                                            }
                                        }
                                }
                            }
                            if (kvpLng.Value <= 80000 && kvpLng.Value > 55000)
                            {
                                foreach (KeyValuePair<string, Dictionary<string, double>> kvp1_1 in Ave_WaitTime)
                                {
                                    foreach (KeyValuePair<string, double> kvp1_2 in kvp1_1.Value)
                                        if (kvp1.Key == kvp1_1.Key && kvp1_2.Key == "80000")
                                        {
                                            if (Ave_WaiTime_Ori.ContainsKey(kvp1.Key))
                                            {
                                                //这是计算总和的方式
                                                // Ave_Time_Ori[arrtemp[1] + '|' + arrtemp[2]] += Convert.ToDouble(arrtemp[5]);
                                                if (Ave_WaiTime_Ori[kvp1.Key].ContainsKey(a))
                                                {
                                                    //这是计算最大值的方式
                                                    //if (Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] <= Convert.ToDouble(arrtemp[5]))
                                                    //{
                                                    //    Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] = Convert.ToDouble(arrtemp[5]);
                                                    //}
                                                    //这是计算总和的方式
                                                    Ave_WaiTime_Ori[kvp1.Key][a] += Convert.ToDouble(kvp1_2.Value);

                                                }
                                                else
                                                {
                                                    Ave_WaiTime_Ori[kvp1.Key].Add(a, Convert.ToDouble(kvp1_2.Value));
                                                }

                                            }
                                            else
                                            {
                                                Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                                                dic_temp.Add(a, Convert.ToDouble(kvp1_2.Value));
                                                Ave_WaiTime_Ori.Add(kvp1.Key, dic_temp);
                                            }
                                        }
                                }
                            }
                            if (kvpLng.Value <= 120000 && kvpLng.Value > 80000)
                            {
                                foreach (KeyValuePair<string, Dictionary<string, double>> kvp1_1 in Ave_WaitTime)
                                {
                                    foreach (KeyValuePair<string, double> kvp1_2 in kvp1_1.Value)
                                        if (kvp1.Key == kvp1_1.Key && kvp1_2.Key == "120000")
                                        {
                                            if (Ave_WaiTime_Ori.ContainsKey(kvp1.Key))
                                            {
                                                //这是计算总和的方式
                                                // Ave_Time_Ori[arrtemp[1] + '|' + arrtemp[2]] += Convert.ToDouble(arrtemp[5]);
                                                if (Ave_WaiTime_Ori[kvp1.Key].ContainsKey(a))
                                                {
                                                    //这是计算最大值的方式
                                                    //if (Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] <= Convert.ToDouble(arrtemp[5]))
                                                    //{
                                                    //    Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] = Convert.ToDouble(arrtemp[5]);
                                                    //}
                                                    //这是计算总和的方式
                                                    Ave_WaiTime_Ori[kvp1.Key][a] += Convert.ToDouble(kvp1_2.Value);

                                                }
                                                else
                                                {
                                                    Ave_WaiTime_Ori[kvp1.Key].Add(a, Convert.ToDouble(kvp1_2.Value));
                                                }

                                            }
                                            else
                                            {
                                                Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                                                dic_temp.Add(a, Convert.ToDouble(kvp1_2.Value));
                                                Ave_WaiTime_Ori.Add(kvp1.Key, dic_temp);
                                            }
                                        }
                                }
                            }
                            if (kvpLng.Value > 120000 && kvpLng.Value <= 200000)
                            {
                                foreach (KeyValuePair<string, Dictionary<string, double>> kvp1_1 in Ave_WaitTime)
                                {
                                    foreach (KeyValuePair<string, double> kvp1_2 in kvp1_1.Value)
                                        if (kvp1.Key == kvp1_1.Key && kvp1_2.Key == "200000")
                                        {
                                            if (Ave_WaiTime_Ori.ContainsKey(kvp1.Key))
                                            {
                                                //这是计算总和的方式
                                                // Ave_Time_Ori[arrtemp[1] + '|' + arrtemp[2]] += Convert.ToDouble(arrtemp[5]);
                                                if (Ave_WaiTime_Ori[kvp1.Key].ContainsKey(a))
                                                {
                                                    //这是计算最大值的方式
                                                    //if (Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] <= Convert.ToDouble(arrtemp[5]))
                                                    //{
                                                    //    Ave_SaiTime_Ori[arrtemp[1] + '|' + arrtemp[2]][arrtemp[0]] = Convert.ToDouble(arrtemp[5]);
                                                    //}
                                                    //这是计算总和的方式
                                                    Ave_WaiTime_Ori[kvp1.Key][a] += Convert.ToDouble(kvp1_2.Value);

                                                }
                                                else
                                                {
                                                    Ave_WaiTime_Ori[kvp1.Key].Add(a, Convert.ToDouble(kvp1_2.Value));
                                                }

                                            }
                                            else
                                            {
                                                Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                                                dic_temp.Add(a, Convert.ToDouble(kvp1_2.Value));
                                                Ave_WaiTime_Ori.Add(kvp1.Key, dic_temp);
                                            }
                                        }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            #endregion
            #endregion

            //将原始的每条航线上对应的每个MMSI船舶的航行时间与等待时间合并
            //2022_12_13日修改不需要合并
            //2022_12_13还是定义两个字典来保存加了发动机功率之后的           
            Dictionary<string, Dictionary<string, double>> Ave_SailTime_Or_1 = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, Dictionary<string, double>> Ave_WaitTime_Or_1 = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, double> Ave_SailTime_Tatal_Ori = new Dictionary<string, double>();
            Dictionary<string, double> Ave_WaitTime_Tatal_Ori = new Dictionary<string, double>();
            foreach (KeyValuePair<string, Dictionary<string, double>> kvp1 in Ave_SaiTime_Ori)
            {
                foreach (KeyValuePair<string, double> kvp11 in kvp1.Value)
                {
                    double kvp11_Value = 0.0;
                    #region----根据吨位赋发动机功率
                    int j = 0;
                    //访问存储船舶吨位的字典
                    foreach (KeyValuePair<string, double> kvpLng in Dic_LNG)
                    {
                        if (kvp11.Key == kvpLng.Key)
                        {
                            //根据船舶的吨位赋予不同的发动机系数
                            if (kvpLng.Value < 55000)
                            {

                                kvp11_Value = 2576 * kvp11.Value;
                                j++;

                            }
                            if (kvpLng.Value >= 55000 && kvpLng.Value < 80000)
                            {

                                kvp11_Value = 14632 * kvp11.Value;
                                j++;

                            }
                            if (kvpLng.Value >= 80000)
                            {

                                kvp11_Value = 33796 * kvp11.Value;
                                j++;
                            }
                        }
                    }
                    if (j == 0)
                    {
                        kvp11_Value = 33796 * kvp11.Value;
                    }
                    #endregion
                    //2022-12-13处理航行时间
                    #region
                    if (Ave_SailTime_Or_1.ContainsKey(kvp1.Key))
                    {
                        if (Ave_SailTime_Or_1[kvp1.Key].ContainsKey(kvp11.Key))
                        {


                            Ave_SailTime_Or_1[kvp1.Key][kvp11.Key] += kvp11_Value;
                        }
                        else
                        {
                            Ave_SailTime_Or_1[kvp1.Key].Add(kvp11.Key, kvp11_Value);
                        }
                    }
                    else
                    {
                        Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                        dic_temp.Add(kvp11.Key, kvp11_Value);
                        Ave_SailTime_Or_1.Add(kvp1.Key, dic_temp);
                    }
                    #endregion

                }

            }
            //处理等待时间

            foreach (KeyValuePair<string, Dictionary<string, double>> kvp2 in Ave_WaiTime_Ori)
            {
                foreach (KeyValuePair<string, double> kvp22 in kvp2.Value)
                {
                    double kvp22_Value = 0.0;
                    int j = 0;
                    #region----根据吨位赋发动机功率
                    //访问存储船舶吨位的字典
                    foreach (KeyValuePair<string, double> kvpLng in Dic_LNG)
                    {
                        if (kvp22.Key == kvpLng.Key)
                        {
                            //根据船舶的吨位赋予不同的发动机系数
                            if (kvpLng.Value < 55000)
                            {


                                kvp22_Value = 2676 * kvp22.Value;
                                j++;

                            }
                            if (kvpLng.Value >= 55000 && kvpLng.Value < 80000)
                            {


                                kvp22_Value = 14732 * kvp22.Value;
                                j++;

                            }
                            if (kvpLng.Value >= 80000)
                            {


                                kvp22_Value = 33266 * kvp22.Value;
                                j++;
                            }
                        }
                    }
                    #endregion
                    if (j == 0)
                    {
                        kvp22_Value = 33266 * kvp22.Value;
                    }
                    //2022-12-13处理等待时间
                    #region
                    if (Ave_WaitTime_Or_1.ContainsKey(kvp2.Key))
                    {
                        if (Ave_WaitTime_Or_1[kvp2.Key].ContainsKey(kvp22.Key))
                        {

                            Ave_WaitTime_Or_1[kvp2.Key][kvp22.Key] += kvp22_Value;
                        }
                        else
                        {
                            Ave_WaitTime_Or_1[kvp2.Key].Add(kvp22.Key, kvp22_Value);
                        }
                    }
                    else
                    {
                        Dictionary<string, double> dic_temp = new Dictionary<string, double>();
                        dic_temp.Add(kvp22.Key, kvp22_Value);
                        Ave_WaitTime_Or_1.Add(kvp2.Key, dic_temp);
                    }
                    #endregion
                }

            }
            //对于有些在船舶航行时间和等待时间中没有MMSI的将其补齐？？？



            foreach (KeyValuePair<string, Dictionary<string, double>> kvp1 in Ave_SailTime_Or_1)
            {
                double sum = 0.0;
                foreach (KeyValuePair<string, double> kvp2 in kvp1.Value)
                {

                    sum += kvp2.Value;
                }
                Ave_SailTime_Tatal_Ori.Add(kvp1.Key, sum);
            }
            foreach (KeyValuePair<string, Dictionary<string, double>> kvp1 in Ave_WaitTime_Or_1)
            {
                double sum = 0.0;
                foreach (KeyValuePair<string, double> kvp2 in kvp1.Value)
                {

                    sum += kvp2.Value;
                }
                Ave_WaitTime_Tatal_Ori.Add(kvp1.Key, sum);
            }
            #endregion

            //处理没有再出口的     
            //根据优化运行后的结果，将原始结果附在其前面，方便比较
            //string inPath3 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withoutreexported\2018_AveTime_DeadWeight_ShipTypeMore3_with80000and120000_AfterOptimization.csv";
            string inPath3 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withoutreexported\2021_AveTime_DeadWeight_ShipTypeMore3_AfterOptimization.csv";
            //string inPath3 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withoutreexported\2017_AveTime_DeadWeight_ShipTypeMore3_AfterOptimization.csv";
            string destPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\Comparision\2021_AveSailTime_DeadWeight_NoneReexported_ShipTypeMore3_Comp.csv";
            string destPath1_1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\Comparision\2021_AveWaitTime_DeadWeight_NoneReexported_ShipTypeMore3_Comp.csv";
            StreamReader streamreader3 = new StreamReader(inPath3);
            StreamWriter streamwriter1 = new StreamWriter(destPath1);
            StreamWriter streamwriter1_1 = new StreamWriter(destPath1_1);
            string line3 = "";
            //定义字典存储需要输出的内容
            Dictionary<string, double> Dic_NonReexp_AftOP_Sail = new Dictionary<string, double>();
            Dictionary<string, double> Dic_NonReexp_AftOP_Wait = new Dictionary<string, double>();
            while ((line3 = streamreader3.ReadLine()) != null)
            {
                string[] arrtemp = line3.Split(',');
                if (Dic_NonReexp_AftOP_Sail.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_NonReexp_AftOP_Sail[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[5]);
                }
                else
                {
                    Dic_NonReexp_AftOP_Sail.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[5]));
                }
                if (Dic_NonReexp_AftOP_Wait.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_NonReexp_AftOP_Wait[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[6]);
                }
                else
                {
                    Dic_NonReexp_AftOP_Wait.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[6]));
                }
            }

            //输出的时候先写出原始的，再写出优化后的
            foreach (KeyValuePair<string, double> kvp1 in Dic_NonReexp_AftOP_Sail)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_SailTime_Tatal_Ori)
                {

                    if (kvp1.Key == kvp2.Key)
                    {
                        //double temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        //double temp = 0;
                        //if (kvp2.Value > kvp1.Value)
                        //    temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        //else temp = (kvp1.Value - kvp2.Value) / kvp1.Value;
                        double temp = 0;
                        if (kvp2.Value > kvp1.Value)
                            temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        else temp = 0;
                        streamwriter1.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value + '|' + Convert.ToString(temp));
                    }

                }
            }
            foreach (KeyValuePair<string, double> kvp1 in Dic_NonReexp_AftOP_Wait)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_WaitTime_Tatal_Ori)
                {

                    if (kvp1.Key == kvp2.Key)
                    {
                        //double temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        //double temp = 0;
                        //if (kvp2.Value > kvp1.Value)
                        //    temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        //else temp = (kvp1.Value - kvp2.Value) / kvp1.Value;
                        double temp = 0;
                        if (kvp2.Value > kvp1.Value)
                            temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        else temp = 0;
                        streamwriter1_1.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value + '|' + Convert.ToString(temp));
                    }

                }
            }
            streamwriter1.Close();
            streamwriter1_1.Close();

            //处理包括再出口的
            //根据优化运行后的结果，将原始结果附在其前面，方便比较
            //string inPath4 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withreexported\2017_AveTime_DeadWeight_ProvideReexported_ShipTypeMore3_Afteroptimization.csv";
            string inPath4 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withreexported\2021_AveTime_DeadWeight_ProvideReexported_ShipTypeMore3_Afteroptimization.csv";
            string destPath2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\Comparision\2021_AveSailTime_DeadWeight_ProvideReexported_ShipTypeMore3_Comp.csv";
            string destPath2_2 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\Comparision\2021_AveWaitTime_DeadWeight_ProvideReexported_ShipTypeMore3_Comp.csv";
            StreamReader streamreader4 = new StreamReader(inPath4);
            StreamWriter streamwriter2 = new StreamWriter(destPath2);
            StreamWriter streamwriter2_2 = new StreamWriter(destPath2_2);
            string line4 = "";
            //定义字典存储需要输出的内容
            Dictionary<string, double> Dic_ProReexp_AftOP_Sai = new Dictionary<string, double>();
            Dictionary<string, double> Dic_ProReexp_AftOP_Wai = new Dictionary<string, double>();
            while ((line4 = streamreader4.ReadLine()) != null)
            {
                string[] arrtemp = line4.Split(',');
                if (Dic_ProReexp_AftOP_Sai.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_ProReexp_AftOP_Sai[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[5]);
                }
                else
                {
                    Dic_ProReexp_AftOP_Sai.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[5]));
                }
                if (Dic_ProReexp_AftOP_Wai.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_ProReexp_AftOP_Wai[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[6]);
                }
                else
                {
                    Dic_ProReexp_AftOP_Wai.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[6]));
                }
            }

            //输出的时候先写出原始的，再写出优化后的
            foreach (KeyValuePair<string, double> kvp1 in Dic_ProReexp_AftOP_Sai)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_SailTime_Tatal_Ori)
                {
                    if (kvp1.Key == kvp2.Key)
                    {
                        //double temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        //double temp = 0;
                        //if (kvp2.Value > kvp1.Value)
                        //    temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        //else temp = (kvp1.Value - kvp2.Value) / kvp1.Value;
                        double temp = 0;
                        if (kvp2.Value > kvp1.Value)
                            temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        else temp = 0;
                        streamwriter2.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value + '|' + Convert.ToString(temp));
                    }
                }
            }
            foreach (KeyValuePair<string, double> kvp1 in Dic_ProReexp_AftOP_Wai)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_WaitTime_Tatal_Ori)
                {
                    if (kvp1.Key == kvp2.Key)
                    {
                        //double temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        //double temp = 0;
                        //if (kvp2.Value > kvp1.Value)
                        //    temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        //else temp = (kvp1.Value - kvp2.Value) / kvp1.Value;
                        double temp = 0;
                        if (kvp2.Value > kvp1.Value)
                            temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        else temp = 0;
                        streamwriter2_2.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value + '|' + Convert.ToString(temp));
                    }
                }
            }
            streamwriter2.Close();
            streamwriter2_2.Close();

            string inPath5 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withreexported\2021_AveTime_DeadWeight_ReceiveReexported_ShipTypeMore3_Afteroptimization.csv";
            //string inPath5 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withreexported\2017_AveTime_DeadWeight_ReceiveReexported_ShipTypeMore3_Afteroptimization.csv";
            string destPath3 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\Comparision\2021_AveSailTime_DeadWeight_ReceiveReexported_ShipTypeMore3_Comp.csv";
            string destPath3_3 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\Comparision\2021_AveWaitTime_DeadWeight_ReceiveReexported_ShipTypeMore3_Comp.csv";
            StreamReader streamreader5 = new StreamReader(inPath5);
            StreamWriter streamwriter3 = new StreamWriter(destPath3);
            StreamWriter streamwriter3_3 = new StreamWriter(destPath3_3);
            string line5 = "";
            //定义字典存储需要输出的内容
            Dictionary<string, double> Dic_RecReexp_AftOP_Sai = new Dictionary<string, double>();
            Dictionary<string, double> Dic_RecReexp_AftOP_Wai = new Dictionary<string, double>();
            while ((line5 = streamreader5.ReadLine()) != null)
            {
                string[] arrtemp = line5.Split(',');
                if (Dic_RecReexp_AftOP_Sai.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_RecReexp_AftOP_Sai[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[5]);
                }
                else
                {
                    Dic_RecReexp_AftOP_Sai.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[5]));
                }
                if (Dic_RecReexp_AftOP_Wai.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_RecReexp_AftOP_Wai[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[6]);
                }
                else
                {
                    Dic_RecReexp_AftOP_Wai.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[6]));
                }
            }

            //输出的时候先写出原始的，再写出优化后的
            foreach (KeyValuePair<string, double> kvp1 in Dic_RecReexp_AftOP_Sai)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_SailTime_Tatal_Ori)
                {
                    if (kvp1.Key == kvp2.Key)
                    {
                        //double temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        //double temp = 0;
                        //if (kvp2.Value > kvp1.Value)
                        //    temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        //else temp = (kvp1.Value - kvp2.Value) / kvp1.Value;
                        double temp = 0;
                        if (kvp2.Value > kvp1.Value)
                            temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        else temp = 0;
                        streamwriter3.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value + '|' + Convert.ToString(temp));
                    }
                }
            }
            foreach (KeyValuePair<string, double> kvp1 in Dic_RecReexp_AftOP_Wai)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_WaitTime_Tatal_Ori)
                {
                    if (kvp1.Key == kvp2.Key)
                    {
                        //double temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        //double temp = 0;
                        //if (kvp2.Value > kvp1.Value)
                        //    temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        //else temp = (kvp1.Value - kvp2.Value) / kvp1.Value;
                        double temp = 0;
                        if (kvp2.Value > kvp1.Value)
                            temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        else temp = 0;
                        streamwriter3_3.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value + '|' + Convert.ToString(temp));
                    }
                }
            }
            streamwriter3.Close();
            streamwriter3_3.Close();
            string inPath6 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withreexported\2021_AveTime_DeadWeight_Reexported_ShipTypeMore2_1_AfterOptimization.csv";
            //string inPath6 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withreexported\2017_AveTime_DeadWeight_Reexported_ShipTypeMore2_1_Afteroptimization.csv";
            string destPath4 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\Comparision\2021_AveSailTime_DeadWeight_Reexported_ShipTypeMore2_1_Comp.csv";
            string destPath4_4 = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\Comparision\2021_AveWaitTime_DeadWeight_Reexported_ShipTypeMore2_1_Comp.csv";
            StreamReader streamreader6 = new StreamReader(inPath6);
            StreamWriter streamwriter4 = new StreamWriter(destPath4);
            StreamWriter streamwriter4_4 = new StreamWriter(destPath4_4);
            string line6 = "";
            //定义字典存储需要输出的内容
            Dictionary<string, double> Dic_Reexp_AftOP_Sail = new Dictionary<string, double>();
            Dictionary<string, double> Dic_Reexp_AftOP_Wait = new Dictionary<string, double>();
            while ((line6 = streamreader6.ReadLine()) != null)
            {
                string[] arrtemp = line6.Split(',');
                if (Dic_Reexp_AftOP_Sail.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_Reexp_AftOP_Sail[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[5]);
                }
                else
                {
                    Dic_Reexp_AftOP_Sail.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[5]));
                }
                if (Dic_Reexp_AftOP_Wait.ContainsKey(arrtemp[0] + '|' + arrtemp[1]))
                {
                    Dic_Reexp_AftOP_Wait[arrtemp[0] + '|' + arrtemp[1]] += Convert.ToDouble(arrtemp[6]);
                }
                else
                {
                    Dic_Reexp_AftOP_Wait.Add(arrtemp[0] + '|' + arrtemp[1], Convert.ToDouble(arrtemp[6]));
                }
            }

            //输出的时候先写出原始的，再写出优化后的
            foreach (KeyValuePair<string, double> kvp1 in Dic_Reexp_AftOP_Sail)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_SailTime_Tatal_Ori)
                {
                    if (kvp1.Key == kvp2.Key)
                    {
                        //double temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        //double temp = 0;
                        //if (kvp2.Value > kvp1.Value)
                        //    temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        //else temp = (kvp1.Value - kvp2.Value) / kvp1.Value;
                        double temp = 0;
                        if (kvp2.Value > kvp1.Value)
                            temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        else temp = 0;
                        streamwriter4.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value + '|' + Convert.ToString(temp));
                    }
                }
            }
            foreach (KeyValuePair<string, double> kvp1 in Dic_Reexp_AftOP_Sail)
            {
                foreach (KeyValuePair<string, double> kvp2 in Ave_WaitTime_Tatal_Ori)
                {
                    if (kvp1.Key == kvp2.Key)
                    {
                        // double temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        //double temp = 0;
                        //if (kvp2.Value > kvp1.Value)
                        //    temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        //else temp = (kvp1.Value - kvp2.Value) / kvp1.Value;
                        double temp = 0;
                        if (kvp2.Value > kvp1.Value)
                            temp = (kvp2.Value - kvp1.Value) / kvp2.Value;
                        else temp = 0;
                        streamwriter4_4.WriteLine(kvp1.Key + '|' + kvp2.Value + '|' + kvp1.Value + '|' + Convert.ToString(temp));
                    }
                }
            }
            streamwriter4.Close();
            streamwriter4_4.Close();
            MessageBox.Show("OK");
        }
        //2022-12-28-将每年的结果合并，就是航行时间减少的比例存储一个文件，等待时间减少的比例存储一个文件
        private void button57_Click(object sender, EventArgs e)
        {
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\Comparision");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\Comparision_Union";
            //定义字典存储每年航行时间不同航线节约的比例
            Dictionary<string, Dictionary<string, Dictionary<string, double>>> AveSailTime = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
            //定义字典存储每年等待时间不同航线节约的比例
            Dictionary<string, Dictionary<string, Dictionary<string, double>>> AveWaitTime = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string[] arrtemp = nextFile.Name.Split('_');
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp1 = line.Split('|');
                    if (nextFile.Name.Contains("AveSailTime"))
                    {
                        if (AveSailTime.ContainsKey(arrtemp[0] + '_' + arrtemp[1]))
                        {
                            if (AveSailTime[arrtemp[0] + '_' + arrtemp[1]].ContainsKey(arrtemp[3]))
                            {

                                if (AveSailTime[arrtemp[0] + '_' + arrtemp[1]][arrtemp[3]].ContainsKey(arrtemp1[0] + '|' + arrtemp1[1]))
                                {
                                    if (AveSailTime[arrtemp[0] + '_' + arrtemp[1]][arrtemp[3]][arrtemp1[0] + '|' + arrtemp1[1]] <= Convert.ToDouble(arrtemp1[4]))
                                        AveSailTime[arrtemp[0] + '_' + arrtemp[1]][arrtemp[3]][arrtemp1[0] + '|' + arrtemp1[1]] = Convert.ToDouble(arrtemp1[4]);
                                }

                                else
                                {
                                    AveSailTime[arrtemp[0] + '_' + arrtemp[1]][arrtemp[3]].Add(arrtemp1[0] + '|' + arrtemp1[1], Convert.ToDouble(arrtemp1[4]));
                                }
                            }

                            else
                            {
                                Dictionary<string, double> Dic_temp = new Dictionary<string, double>();
                                Dic_temp.Add(arrtemp1[0] + '|' + arrtemp1[1], Convert.ToDouble(arrtemp1[4]));
                                AveSailTime[arrtemp[0] + '_' + arrtemp[1]].Add(arrtemp[3], Dic_temp);
                            }
                        }
                        else
                        {
                            Dictionary<string, double> Dic_temp = new Dictionary<string, double>();
                            Dic_temp.Add(arrtemp1[0] + '|' + arrtemp1[1], Convert.ToDouble(arrtemp1[4]));
                            Dictionary<string, Dictionary<string, double>> Dic_temp1 = new Dictionary<string, Dictionary<string, double>>();
                            Dic_temp1.Add(arrtemp[3], Dic_temp);
                            AveSailTime.Add(arrtemp[0] + '_' + arrtemp[1], Dic_temp1);
                        }
                    }
                    if (nextFile.Name.Contains("AveWaitTime"))
                    {
                        if (AveWaitTime.ContainsKey(arrtemp[0] + '_' + arrtemp[1]))
                        {
                            if (AveWaitTime[arrtemp[0] + '_' + arrtemp[1]].ContainsKey(arrtemp[3]))
                            {

                                if (AveWaitTime[arrtemp[0] + '_' + arrtemp[1]][arrtemp[3]].ContainsKey(arrtemp1[0] + '|' + arrtemp1[1]))
                                {
                                    if (AveWaitTime[arrtemp[0] + '_' + arrtemp[1]][arrtemp[3]][arrtemp1[0] + '|' + arrtemp1[1]] <= Convert.ToDouble(arrtemp1[4]))
                                        AveWaitTime[arrtemp[0] + '_' + arrtemp[1]][arrtemp[3]][arrtemp1[0] + '|' + arrtemp1[1]] = Convert.ToDouble(arrtemp1[4]);
                                }

                                else
                                {
                                    AveWaitTime[arrtemp[0] + '_' + arrtemp[1]][arrtemp[3]].Add(arrtemp1[0] + '|' + arrtemp1[1], Convert.ToDouble(arrtemp1[4]));
                                }
                            }

                            else
                            {
                                Dictionary<string, double> Dic_temp = new Dictionary<string, double>();
                                Dic_temp.Add(arrtemp1[0] + '|' + arrtemp1[1], Convert.ToDouble(arrtemp1[4]));
                                AveWaitTime[arrtemp[0] + '_' + arrtemp[1]].Add(arrtemp[3], Dic_temp);
                            }
                        }
                        else
                        {
                            Dictionary<string, double> Dic_temp = new Dictionary<string, double>();
                            Dic_temp.Add(arrtemp1[0] + '|' + arrtemp1[1], Convert.ToDouble(arrtemp1[4]));
                            Dictionary<string, Dictionary<string, double>> Dic_temp1 = new Dictionary<string, Dictionary<string, double>>();
                            Dic_temp1.Add(arrtemp[3], Dic_temp);
                            AveWaitTime.Add(arrtemp[0] + '_' + arrtemp[1], Dic_temp1);
                        }
                    }
                }
            }
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, double>>> kvp1 in AveSailTime)
            {
                foreach (KeyValuePair<string, Dictionary<string, double>> kvp11 in kvp1.Value)
                {
                    foreach (KeyValuePair<string, double> kvp2 in kvp11.Value)
                    {
                        string destPath = System.IO.Path.Combine(targetPath, kvp1.Key + ".csv");
                        FileStream fs = new FileStream(destPath, FileMode.Append);
                        StreamWriter streamwriter = new StreamWriter(fs);
                        streamwriter.WriteLine(kvp2.Key + '|' + kvp11.Key + '|' + kvp2.Value);
                        streamwriter.Close();
                    }
                }
            }
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, double>>> kvp1 in AveWaitTime)
            {
                foreach (KeyValuePair<string, Dictionary<string, double>> kvp11 in kvp1.Value)
                {
                    foreach (KeyValuePair<string, double> kvp2 in kvp11.Value)
                    {
                        string destPath = System.IO.Path.Combine(targetPath, kvp1.Key + ".csv");
                        FileStream fs = new FileStream(destPath, FileMode.Append);
                        StreamWriter streamwriter = new StreamWriter(fs);
                        streamwriter.WriteLine(kvp2.Key + '|' + kvp11.Key + '|' + kvp2.Value);
                        streamwriter.Close();
                    }
                }
            }
            MessageBox.Show("OK");
        }
        //2022-12-28由于前面优化过程只考虑了船型大于3种的航线，因此根据均值将其中未考虑的其他航线补齐
        private void button58_Click(object sender, EventArgs e)
        {
            #region-----定义字典统计每年所有的航线(区分有无再进出口)
            DirectoryInfo theFolder1 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification");
            DirectoryInfo theFolder2 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast_NonReexported_Classification");
            DirectoryInfo theFolder3 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Sailing_between_Ports(2018-2021)_LNG_v1_Noballast_Reexported");
            DirectoryInfo theFolder4 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\Waiting_between_Ports(2018-2021)_LNG_v1_Noballast_Reexported");
            Dictionary<string, Dictionary<string, List<string>>> Lanes = new Dictionary<string, Dictionary<string, List<string>>>();
            DirectoryInfo[] dirFolder1 = theFolder1.GetDirectories();
            DirectoryInfo[] dirFolder2 = theFolder2.GetDirectories();
            foreach (DirectoryInfo nextFolder1 in dirFolder1)
            {
                FileInfo[] dirFile1 = nextFolder1.GetFiles();
                foreach (FileInfo nextFile1 in dirFile1)
                {
                    string[] arrtemp1 = nextFile1.Name.Split('.');
                    StreamReader streamreader1 = new StreamReader(nextFile1.FullName);
                    string line1 = "";
                    while ((line1 = streamreader1.ReadLine()) != null)
                    {
                        string[] arrtemp1_1 = line1.Split('|');
                        if (Lanes.ContainsKey(nextFolder1.Name + '_' + "Sailing"))
                        {
                            if (Lanes[nextFolder1.Name + '_' + "Sailing"].ContainsKey(arrtemp1[0]))
                            {
                                if (Lanes[nextFolder1.Name + '_' + "Sailing"][arrtemp1[0]].Contains(arrtemp1_1[2] + '|' + arrtemp1_1[3]) == false)

                                {
                                    Lanes[nextFolder1.Name + '_' + "Sailing"][arrtemp1[0]].Add(arrtemp1_1[2] + '|' + arrtemp1_1[3]);
                                }
                            }
                            else
                            {
                                List<string> Lis_Temp = new List<string>();
                                Lis_Temp.Add(arrtemp1_1[2] + '|' + arrtemp1_1[3]);
                                Lanes[nextFolder1.Name + '_' + "Sailing"].Add(arrtemp1[0], Lis_Temp);
                            }
                        }
                        else
                        {
                            List<string> Lis_Temp = new List<string>();
                            Lis_Temp.Add(arrtemp1_1[2] + '|' + arrtemp1_1[3]);
                            Dictionary<string, List<string>> Dic_TEMP1 = new Dictionary<string, List<string>>();
                            Dic_TEMP1.Add(arrtemp1[0], Lis_Temp);
                            Lanes.Add(nextFolder1.Name + '_' + "Sailing", Dic_TEMP1);
                        }

                    }
                }
            }
            foreach (DirectoryInfo nextFolder2 in dirFolder2)
            {
                FileInfo[] dirFile2 = nextFolder2.GetFiles();
                foreach (FileInfo nextFile1 in dirFile2)
                {
                    string[] arrtemp1 = nextFile1.Name.Split('.');
                    StreamReader streamreader1 = new StreamReader(nextFile1.FullName);
                    string line1 = "";
                    while ((line1 = streamreader1.ReadLine()) != null)
                    {
                        string[] arrtemp1_1 = line1.Split('|');
                        if (Lanes.ContainsKey(nextFolder2.Name + '_' + "Waiting"))
                        {
                            if (Lanes[nextFolder2.Name + '_' + "Waiting"].ContainsKey(arrtemp1[0]))
                            {
                                if (Lanes[nextFolder2.Name + '_' + "Waiting"][arrtemp1[0]].Contains(arrtemp1_1[2] + '|' + arrtemp1_1[3]) == false)

                                {
                                    Lanes[nextFolder2.Name + '_' + "Waiting"][arrtemp1[0]].Add(arrtemp1_1[2] + '|' + arrtemp1_1[3]);
                                }
                            }
                            else
                            {
                                List<string> Lis_Temp = new List<string>();
                                Lis_Temp.Add(arrtemp1_1[2] + '|' + arrtemp1_1[3]);
                                Lanes[nextFolder2.Name + '_' + "Waiting"].Add(arrtemp1[0], Lis_Temp);
                            }
                        }
                        else
                        {
                            List<string> Lis_Temp = new List<string>();
                            Lis_Temp.Add(arrtemp1_1[2] + '|' + arrtemp1_1[3]);
                            Dictionary<string, List<string>> Dic_TEMP1 = new Dictionary<string, List<string>>();
                            Dic_TEMP1.Add(arrtemp1[0], Lis_Temp);
                            Lanes.Add(nextFolder2.Name + '_' + "Waiting", Dic_TEMP1);
                        }

                    }
                }

            }
            FileInfo[] dirFile3 = theFolder3.GetFiles();
            FileInfo[] dirFile4 = theFolder4.GetFiles();
            foreach (FileInfo nextFile1 in dirFile3)
            {
                string[] arrtemp1 = nextFile1.Name.Split('_');
                StreamReader streamreader1 = new StreamReader(nextFile1.FullName);
                string line1 = "";
                while ((line1 = streamreader1.ReadLine()) != null)
                {
                    string[] arrtemp1_1 = line1.Split('|');
                    if (Lanes.ContainsKey(arrtemp1[0] + '_' + arrtemp1[2]))
                    {
                        if (Lanes[arrtemp1[0] + '_' + arrtemp1[2]].ContainsKey(arrtemp1[5]))
                        {
                            if (Lanes[arrtemp1[0] + '_' + arrtemp1[2]][arrtemp1[5]].Contains(arrtemp1_1[2] + '|' + arrtemp1_1[3]) == false)

                            {
                                Lanes[arrtemp1[0] + '_' + arrtemp1[2]][arrtemp1[5]].Add(arrtemp1_1[2] + '|' + arrtemp1_1[3]);
                            }
                        }
                        else
                        {
                            List<string> Lis_Temp = new List<string>();
                            Lis_Temp.Add(arrtemp1_1[2] + '|' + arrtemp1_1[3]);
                            Lanes[arrtemp1[0] + '_' + arrtemp1[2]].Add(arrtemp1[5], Lis_Temp);
                        }
                    }
                    else
                    {
                        List<string> Lis_Temp = new List<string>();
                        Lis_Temp.Add(arrtemp1_1[2] + '|' + arrtemp1_1[3]);
                        Dictionary<string, List<string>> Dic_TEMP1 = new Dictionary<string, List<string>>();
                        Dic_TEMP1.Add(arrtemp1[5], Lis_Temp);
                        Lanes.Add(arrtemp1[0] + '_' + arrtemp1[2], Dic_TEMP1);
                    }

                }
            }
            foreach (FileInfo nextFile2 in dirFile4)
            {
                string[] arrtemp1 = nextFile2.Name.Split('_');
                StreamReader streamreader1 = new StreamReader(nextFile2.FullName);
                string line1 = "";
                while ((line1 = streamreader1.ReadLine()) != null)
                {
                    string[] arrtemp1_1 = line1.Split('|');
                    if (Lanes.ContainsKey(arrtemp1[0] + '_' + arrtemp1[2]))
                    {
                        if (Lanes[arrtemp1[0] + '_' + arrtemp1[2]].ContainsKey(arrtemp1[5]))
                        {
                            if (Lanes[arrtemp1[0] + '_' + arrtemp1[2]][arrtemp1[5]].Contains(arrtemp1_1[2] + '|' + arrtemp1_1[3]) == false)

                            {
                                Lanes[arrtemp1[0] + '_' + arrtemp1[2]][arrtemp1[5]].Add(arrtemp1_1[2] + '|' + arrtemp1_1[3]);
                            }
                        }
                        else
                        {
                            List<string> Lis_Temp = new List<string>();
                            Lis_Temp.Add(arrtemp1_1[2] + '|' + arrtemp1_1[3]);
                            Lanes[arrtemp1[0] + '_' + arrtemp1[2]].Add(arrtemp1[5], Lis_Temp);
                        }
                    }
                    else
                    {
                        List<string> Lis_Temp = new List<string>();
                        Lis_Temp.Add(arrtemp1_1[2] + '|' + arrtemp1_1[3]);
                        Dictionary<string, List<string>> Dic_TEMP1 = new Dictionary<string, List<string>>();
                        Dic_TEMP1.Add(arrtemp1[5], Lis_Temp);
                        Lanes.Add(arrtemp1[0] + '_' + arrtemp1[2], Dic_TEMP1);
                    }
                }
            }
            #endregion
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\Comparision_Union");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\Comparision_Union_Making Up";
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string destPath = System.IO.Path.Combine(targetPath, nextFile.Name);
                StreamWriter streamwriter = new StreamWriter(destPath);
                string line2_0 = "";
                //定义字典存储每条航线减少的比例
                Dictionary<string, Dictionary<string, double>> result_0 = new Dictionary<string, Dictionary<string, double>>();
                //定义列表存储减少的比例集合
                List<double> Lis_0 = new List<double>();
                while ((line2_0 = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp2_1 = line2_0.Split('|');
                    if (result_0.ContainsKey(arrtemp2_1[0] + '|' + arrtemp2_1[1]))
                    {
                        result_0[arrtemp2_1[0] + '|' + arrtemp2_1[1]].Add(arrtemp2_1[2], Convert.ToDouble(arrtemp2_1[3]));
                    }
                    else
                    {
                        Dictionary<string, double> Dic_Temp = new Dictionary<string, double>();
                        Dic_Temp.Add(arrtemp2_1[2], Convert.ToDouble(arrtemp2_1[3]));
                        result_0.Add(arrtemp2_1[0] + '|' + arrtemp2_1[1], Dic_Temp);
                    }
                    Lis_0.Add(Convert.ToDouble(arrtemp2_1[3]));
                }
                string[] arrtemp2_0 = nextFile.Name.Split('_');
                if (arrtemp2_0[1].Contains("AveSailTime"))
                {
                    foreach (KeyValuePair<string, Dictionary<string, List<string>>> kvp1 in Lanes)
                    {
                        string a = arrtemp2_0[0] + '_' + "Sailing";
                        foreach (KeyValuePair<string, List<string>> kvp2 in kvp1.Value)
                        {
                            foreach (string b in kvp2.Value)
                            {

                                if (kvp1.Key == a && result_0.Keys.Contains(b) == false)
                                {
                                    Dictionary<string, double> Dic_temp = new Dictionary<string, double>();
                                    Dic_temp.Add(kvp2.Key, Lis_0.Average());
                                    result_0.Add(b, Dic_temp);
                                }

                                else if (kvp1.Key == a && result_0.ContainsKey(b) && result_0[b].ContainsKey(kvp2.Key) == false)
                                {

                                    result_0[b].Add(kvp2.Key, Lis_0.Average());



                                }

                            }
                        }
                    }
                }
                if (arrtemp2_0[1].Contains("AveWaitTime"))
                {
                    foreach (KeyValuePair<string, Dictionary<string, List<string>>> kvp1 in Lanes)
                    {
                        string b = arrtemp2_0[0] + '_' + "Waiting";
                        foreach (KeyValuePair<string, List<string>> kvp2 in kvp1.Value)
                        {
                            foreach (string a in kvp2.Value)
                            {

                                if (kvp1.Key == b && result_0.ContainsKey(a) == false)
                                {
                                    Dictionary<string, double> Dic_temp = new Dictionary<string, double>();
                                    Dic_temp.Add(kvp2.Key, Lis_0.Average());
                                    result_0.Add(a, Dic_temp);
                                }

                                else if (kvp1.Key == b && result_0.ContainsKey(a) && result_0[a].ContainsKey(kvp2.Key) == false)
                                {

                                    result_0[a].Add(kvp2.Key, Lis_0.Average());

                                }

                            }
                        }
                    }
                }
                foreach (KeyValuePair<string, Dictionary<string, double>> kvp2 in result_0)
                {
                    foreach (KeyValuePair<string, double> kvp22 in kvp2.Value)
                    {
                        streamwriter.WriteLine(kvp2.Key + '|' + Convert.ToString(kvp22.Key) + '|' + Convert.ToString(kvp22.Value));
                    }
                }
                streamwriter.Close();
            }
            MessageBox.Show("OK");
        }
        //2022-12-29根据结果中的航线添加港口坐标
        private void button59_Click(object sender, EventArgs e)
        {
            //定义字典，存储每个港口的经纬度坐标
            Dictionary<string, List<string>> Por_Cor = new Dictionary<string, List<string>>();
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\portCenter.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp1 = line1.Split(',');
                if (Por_Cor.ContainsKey(arrtemp1[0]))
                {
                    continue;
                }
                else
                {
                    List<string> Lis_temp = new List<string>();
                    Lis_temp.Add(arrtemp1[1]);
                    Lis_temp.Add(arrtemp1[2]);
                    Por_Cor.Add(arrtemp1[0], Lis_temp);
                }
            }
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\Comparision_Union_Making Up");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\Comparision_Union_Making Up_Adding Coordination";
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string outPath = System.IO.Path.Combine(targetPath, nextFile.Name);
                StreamWriter streamwriter = new StreamWriter(outPath);
                #region
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split('|');
                    string Cor_Ex = "";
                    string Cor_Im = "";
                    string d = "";
                    foreach (KeyValuePair<string, List<string>> KVP1_1 in Por_Cor)
                    {
                        //港口坐标存放的格式是先经度后维度
                        // if (arrtemp[0] == KVP1_1.Key)
                        string a = arrtemp[0].Substring(arrtemp[0].IndexOf(".") + 1);
                        string b = arrtemp[1].Substring(arrtemp[1].IndexOf(".") + 1);
                        if (arrtemp[0] == KVP1_1.Key || arrtemp[0].Contains(KVP1_1.Key) || KVP1_1.Key.Contains(a) || KVP1_1.Key.Contains(arrtemp[0]) || arrtemp[0].Replace(" ", "") == KVP1_1.Key.Replace("\t ", "") || Regex.Replace(arrtemp[0], @"\s", "\t") == KVP1_1.Key || Regex.Replace(arrtemp[0], @"  ", "\t") == Regex.Replace(KVP1_1.Key, @"\s", "\t") || Regex.Replace(arrtemp[0], @"\s", "\t") == Regex.Replace(KVP1_1.Key, @"\s", "\t") || Regex.Replace(arrtemp[0], @"  ", "\t") == KVP1_1.Key)
                        {
                            Cor_Ex = KVP1_1.Value[0] + '|' + KVP1_1.Value[1];

                        }
                        // if (arrtemp[1] == KVP1_1.Key)
                        if (arrtemp[1] == KVP1_1.Key || arrtemp[1].Contains(KVP1_1.Key) || KVP1_1.Key.Contains(b) || KVP1_1.Key.Contains(arrtemp[1]) || arrtemp[1].Replace(" ", "") == KVP1_1.Key.Replace("\t", "") || Regex.Replace(arrtemp[1], @"\s", "\t") == KVP1_1.Key || Regex.Replace(arrtemp[1], @"  ", "\t") == Regex.Replace(KVP1_1.Key, @"\s", "\t") || Regex.Replace(arrtemp[1], @"\s", "\t") == Regex.Replace(KVP1_1.Key, @"\s", "\t") || Regex.Replace(arrtemp[0], @"  ", "\t") == KVP1_1.Key)
                        {
                            Cor_Im = KVP1_1.Value[0] + '|' + KVP1_1.Value[1];
                        }
                        d = KVP1_1.Key;
                    }
                    if (Cor_Ex != "" && Cor_Im != "")
                    {
                        streamwriter.WriteLine(arrtemp[0] + '|' + Cor_Ex + '|' + arrtemp[1] + '|' + Cor_Im + '|' + arrtemp[2] + '|' + arrtemp[3]);
                    }

                    if (Cor_Ex == "" || Cor_Im == "")
                    {
                        int j = 0;
                    }
                    string c = Regex.Replace(arrtemp[0], @"  ", "\t");
                    d = Regex.Replace(d, @"\s", "\t");
                }
                #endregion
                streamwriter.Close();
            }
            MessageBox.Show("OK");
        }
        //2022_12_29_计算因Virtual Arrival带来的碳排放减少比例
        private void button60_Click(object sender, EventArgs e)
        {
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\Comparision");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\Comparision_VirtualArrival";
            //定义字典存储每年航行时间不同航线节约的比例
            Dictionary<string, Dictionary<string, double>> AveSailTime = new Dictionary<string, Dictionary<string, double>>();
            //定义字典存储每年等待时间不同航线节约的比例
            Dictionary<string, Dictionary<string, double>> AveWaitTime = new Dictionary<string, Dictionary<string, double>>();
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string[] arrtemp = nextFile.Name.Split('_');
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp1 = line.Split('|');
                    if (nextFile.Name.Contains("AveSailTime"))
                    {
                        if (AveSailTime.ContainsKey(arrtemp[0] + '_' + arrtemp[1]))
                        {
                            if (AveSailTime[arrtemp[0] + '_' + arrtemp[1]].ContainsKey(arrtemp1[0] + '|' + arrtemp1[1]))
                            {
                                if (AveSailTime[arrtemp[0] + '_' + arrtemp[1]][arrtemp1[0] + '|' + arrtemp1[1]] >= Convert.ToDouble(arrtemp1[3]))
                                {
                                    AveSailTime[arrtemp[0] + '_' + arrtemp[1]][arrtemp1[0] + '|' + arrtemp1[1]] = Convert.ToDouble(arrtemp1[3]);
                                }
                            }
                            else
                            {
                                AveSailTime[arrtemp[0] + '_' + arrtemp[1]].Add(arrtemp1[0] + '|' + arrtemp1[1], Convert.ToDouble(arrtemp1[3]));
                            }
                        }
                        else
                        {
                            Dictionary<string, double> Dic_temp = new Dictionary<string, double>();
                            Dic_temp.Add(arrtemp1[0] + '|' + arrtemp1[1], Convert.ToDouble(arrtemp1[3]));
                            AveSailTime.Add(arrtemp[0] + '_' + arrtemp[1], Dic_temp);
                        }
                    }
                    if (nextFile.Name.Contains("AveWaitTime"))
                    {
                        if (AveWaitTime.ContainsKey(arrtemp[0] + '_' + arrtemp[1]))
                        {
                            if (AveWaitTime[arrtemp[0] + '_' + arrtemp[1]].ContainsKey(arrtemp1[0] + '|' + arrtemp1[1]))
                            {
                                if (AveWaitTime[arrtemp[0] + '_' + arrtemp[1]][arrtemp1[0] + '|' + arrtemp1[1]] >= Convert.ToDouble(arrtemp1[3]))
                                {
                                    AveWaitTime[arrtemp[0] + '_' + arrtemp[1]][arrtemp1[0] + '|' + arrtemp1[1]] = Convert.ToDouble(arrtemp1[3]);
                                }
                            }
                            else
                            {
                                AveWaitTime[arrtemp[0] + '_' + arrtemp[1]].Add(arrtemp1[0] + '|' + arrtemp1[1], Convert.ToDouble(arrtemp1[3]));
                            }
                        }
                        else
                        {
                            Dictionary<string, double> Dic_temp = new Dictionary<string, double>();
                            Dic_temp.Add(arrtemp1[0] + '|' + arrtemp1[1], Convert.ToDouble(arrtemp1[3]));
                            AveWaitTime.Add(arrtemp[0] + '_' + arrtemp[1], Dic_temp);
                        }
                    }
                }
            }
            //定义字典,存储结果
            Dictionary<string, Dictionary<string, double>> DIC_Re = new Dictionary<string, Dictionary<string, double>>();
            foreach (KeyValuePair<string, Dictionary<string, double>> kvp1 in AveSailTime)
            {
                foreach (KeyValuePair<string, double> kvp2 in kvp1.Value)
                {
                    string destPath = System.IO.Path.Combine(targetPath, kvp1.Key + ".csv");
                    FileStream fs = new FileStream(destPath, FileMode.Append);
                    StreamWriter streamwriter = new StreamWriter(fs);
                    streamwriter.WriteLine(kvp2.Key + '|' + kvp2.Value);
                    streamwriter.Close();
                }
            }
            foreach (KeyValuePair<string, Dictionary<string, double>> kvp1 in AveWaitTime)
            {
                foreach (KeyValuePair<string, double> kvp2 in kvp1.Value)
                {
                    string destPath = System.IO.Path.Combine(targetPath, kvp1.Key + ".csv");
                    FileStream fs = new FileStream(destPath, FileMode.Append);
                    StreamWriter streamwriter = new StreamWriter(fs);
                    streamwriter.WriteLine(kvp2.Key + '|' + kvp2.Value);
                    streamwriter.Close();
                }
            }
            MessageBox.Show("OK");
        }
        //2023_01_02 Virtual Arrival碳排放减少计算数据组织
        //处理没有再出口的
        private void button61_Click(object sender, EventArgs e)
        {
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withoutreexported\ForVirtualArrival");
            //string destPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withoutreexported\ForVirtualArrival_Reorgnize";
            //DirectoryInfo theFolder1 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withoutreexported\新建文件夹");
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withoutreexported\ForVirtualArrival");
            string destPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withoutreexported\ForVirtualArrival_Reorgnize";
            DirectoryInfo theFolder1 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withoutreexported\新建文件夹");
            FileInfo[] dirFile = theFolder.GetFiles();
            #region----//定义字典存储每年每条航线上不同吨位的船舶的时间成本
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> Dic_Re = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            foreach (FileInfo nextFile in dirFile)
            {
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line = "";
                string[] arrtemp0 = nextFile.Name.Split('.');
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split(',');
                    if (Dic_Re.ContainsKey(arrtemp0[0]))
                    {
                        if (Dic_Re[arrtemp0[0]].ContainsKey(arrtemp[0] + ',' + arrtemp[1]))

                        {

                            Dic_Re[arrtemp0[0]][arrtemp[0] + ',' + arrtemp[1]].Add(arrtemp[2], arrtemp[3]);
                        }
                        else
                        {
                            Dictionary<string, string> Dic_temp = new Dictionary<string, string>();
                            Dic_temp.Add(arrtemp[2], arrtemp[3]);
                            Dic_Re[arrtemp0[0]].Add(arrtemp[0] + ',' + arrtemp[1], Dic_temp);
                        }
                    }
                    else
                    {
                        Dictionary<string, string> Dic_temp = new Dictionary<string, string>();
                        Dic_temp.Add(arrtemp[2], arrtemp[3]);
                        Dictionary<string, Dictionary<string, string>> Dic_temp1 = new Dictionary<string, Dictionary<string, string>>();
                        Dic_temp1.Add(arrtemp[0] + ',' + arrtemp[1], Dic_temp);
                        Dic_Re.Add(arrtemp0[0], Dic_temp1);
                    }
                }
            }
            #endregion
            #region----//定义字典分别存储每年优化后的航行时间和等待时间
            Dictionary<string, List<string>> SailTime = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> WaitTime = new Dictionary<string, List<string>>();
            FileInfo[] dirFile1 = theFolder1.GetFiles();
            foreach (FileInfo nextFile1 in dirFile1)
            {
                StreamReader streamreader1 = new StreamReader(nextFile1.FullName);
                string[] arrtemp0_0 = nextFile1.Name.Split('_');
                List<string> Lis_Temp = new List<string>();
                List<string> Lis_Temp1 = new List<string>();
                string line0_0 = "";
                int j = 0;
                while ((line0_0 = streamreader1.ReadLine()) != null)
                {
                    string[] arrtemp0_1 = line0_0.Split(',');
                    j = arrtemp0_1.Count();
                    for (int i = 0; i < arrtemp0_1.Count(); i++)
                    {
                        Lis_Temp.Add(arrtemp0_1[i]);
                    }
                }
                for (int i = 0; i < j; i++)
                {
                    Lis_Temp1.Add(Lis_Temp[i]);
                    Lis_Temp1.Add(Lis_Temp[i + j]);
                    Lis_Temp1.Add(Lis_Temp[i + 2 * j]);
                    Lis_Temp1.Add(Lis_Temp[i + 3 * j]);
                    Lis_Temp1.Add(Lis_Temp[i + 4 * j]);
                }
                if (arrtemp0_0[3] == "Sail")
                {
                    SailTime.Add(arrtemp0_0[0] + '_' + arrtemp0_0[3], Lis_Temp1);
                }
                if (arrtemp0_0[3] == "Wait")
                {
                    WaitTime.Add(arrtemp0_0[0] + '_' + arrtemp0_0[3], Lis_Temp1);
                }
            }
            #endregion
            //写出结果
            //定义字典存储需要输出的结果
            Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> Dic_Re_outPUT = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> Dic_Re_outPUT1 = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();
            #region
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, string>>> kvp1 in Dic_Re)
            {
                string[] arrtemp1_0 = kvp1.Key.Split('_');
                foreach (KeyValuePair<string, List<string>> kvp2 in SailTime)
                {
                    string[] arrtemp1_1 = kvp2.Key.Split('_');
                    if (arrtemp1_0[0] == arrtemp1_1[0])
                    {
                        int i = 0;
                        int j = 0;
                        foreach (KeyValuePair<string, Dictionary<string, string>> kvp3 in kvp1.Value)
                        {
                            foreach (KeyValuePair<string, string> kvp4 in kvp3.Value)
                            {
                                if (Dic_Re_outPUT.ContainsKey(kvp1.Key))
                                {
                                    if (Dic_Re_outPUT[kvp1.Key].ContainsKey(kvp3.Key))
                                    {
                                        if (Dic_Re_outPUT[kvp1.Key][kvp3.Key].ContainsKey(kvp4.Key))
                                        {
                                            List<string> Lis_TEMP = new List<string>();
                                            Lis_TEMP.Add(kvp4.Value);
                                            Lis_TEMP.Add(kvp2.Value[i + j]);
                                            Dic_Re_outPUT[kvp1.Key][kvp3.Key][kvp4.Key] = Lis_TEMP;
                                        }
                                        else
                                        {
                                            List<string> Lis_TEMP = new List<string>();
                                            Lis_TEMP.Add(kvp4.Value);
                                            Lis_TEMP.Add(kvp2.Value[i + j]);
                                            Dic_Re_outPUT[kvp1.Key][kvp3.Key].Add(kvp4.Key, Lis_TEMP);
                                        }
                                    }
                                    else
                                    {
                                        List<string> Lis_TEMP = new List<string>();
                                        Lis_TEMP.Add(kvp4.Value);
                                        Lis_TEMP.Add(kvp2.Value[i + j]);
                                        Dictionary<string, List<string>> Dic_temp = new Dictionary<string, List<string>>();
                                        Dic_temp.Add(kvp4.Key, Lis_TEMP);
                                        Dic_Re_outPUT[kvp1.Key].Add(kvp3.Key, Dic_temp);
                                    }
                                }
                                else
                                {
                                    List<string> Lis_TEMP = new List<string>();
                                    Lis_TEMP.Add(kvp4.Value);
                                    Lis_TEMP.Add(kvp2.Value[i + j]);
                                    Dictionary<string, List<string>> Dic_temp = new Dictionary<string, List<string>>();
                                    Dic_temp.Add(kvp4.Key, Lis_TEMP);
                                    Dictionary<string, Dictionary<string, List<string>>> Dic_temp1 = new Dictionary<string, Dictionary<string, List<string>>>();
                                    Dic_temp1.Add(kvp3.Key, Dic_temp);
                                    Dic_Re_outPUT.Add(kvp1.Key, Dic_temp1);
                                }
                                j++;
                            }
                            j--;
                            i++;
                        }

                    }

                }
            }
            #endregion
            #region
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, List<string>>>> kvp1 in Dic_Re_outPUT)
            {
                string[] arrtemp1_0 = kvp1.Key.Split('_');
                foreach (KeyValuePair<string, List<string>> kvp2 in WaitTime)
                {
                    string[] arrtemp1_1 = kvp2.Key.Split('_');
                    if (arrtemp1_0[0] == arrtemp1_1[0])
                    {
                        int i = 0;
                        int j = 0;
                        foreach (KeyValuePair<string, Dictionary<string, List<string>>> kvp3 in kvp1.Value)
                        {
                            foreach (KeyValuePair<string, List<string>> kvp4 in kvp3.Value)
                            {
                                if (Dic_Re_outPUT1.ContainsKey(kvp1.Key))
                                {
                                    if (Dic_Re_outPUT1[kvp1.Key].ContainsKey(kvp3.Key))
                                    {
                                        if (Dic_Re_outPUT1[kvp1.Key][kvp3.Key].ContainsKey(kvp4.Key))
                                        {
                                            List<string> Lis_TEMP = new List<string>();
                                            Lis_TEMP.Add(kvp4.Value[0]);
                                            Lis_TEMP.Add(kvp4.Value[1]);
                                            Lis_TEMP.Add(kvp2.Value[i + j]);
                                            Dic_Re_outPUT1[kvp1.Key][kvp3.Key][kvp4.Key] = Lis_TEMP;
                                        }
                                        else
                                        {
                                            List<string> Lis_TEMP = new List<string>();
                                            Lis_TEMP.Add(kvp4.Value[0]);
                                            Lis_TEMP.Add(kvp4.Value[1]);
                                            Lis_TEMP.Add(kvp2.Value[i + j]);
                                            Dic_Re_outPUT1[kvp1.Key][kvp3.Key].Add(kvp4.Key, Lis_TEMP);
                                        }
                                    }
                                    else
                                    {
                                        List<string> Lis_TEMP = new List<string>();
                                        Lis_TEMP.Add(kvp4.Value[0]);
                                        Lis_TEMP.Add(kvp4.Value[1]);
                                        Lis_TEMP.Add(kvp2.Value[i + j]);
                                        Dictionary<string, List<string>> Dic_temp = new Dictionary<string, List<string>>();
                                        Dic_temp.Add(kvp4.Key, Lis_TEMP);
                                        Dic_Re_outPUT1[kvp1.Key].Add(kvp3.Key, Dic_temp);
                                    }
                                }
                                else
                                {
                                    List<string> Lis_TEMP = new List<string>();
                                    Lis_TEMP.Add(kvp4.Value[0]);
                                    Lis_TEMP.Add(kvp4.Value[1]);
                                    Lis_TEMP.Add(kvp2.Value[i + j]);
                                    Dictionary<string, List<string>> Dic_temp = new Dictionary<string, List<string>>();
                                    Dic_temp.Add(kvp4.Key, Lis_TEMP);
                                    Dictionary<string, Dictionary<string, List<string>>> Dic_temp1 = new Dictionary<string, Dictionary<string, List<string>>>();
                                    Dic_temp1.Add(kvp3.Key, Dic_temp);
                                    Dic_Re_outPUT1.Add(kvp1.Key, Dic_temp1);
                                }
                                j++;
                            }
                            j--;
                            i++;
                        }

                    }


                }
            }
            #endregion
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, List<string>>>> kvp1 in Dic_Re_outPUT1)
            {
                string destPath1 = System.IO.Path.Combine(destPath, kvp1.Key + ".csv");
                StreamWriter streamwriter = new StreamWriter(destPath1);
                foreach (KeyValuePair<string, Dictionary<string, List<string>>> kvp2 in kvp1.Value)
                {
                    foreach (KeyValuePair<string, List<string>> kvp3 in kvp2.Value)
                    {
                        streamwriter.Write(kvp2.Key + ',' + kvp3.Key);
                        foreach (string a in kvp3.Value)
                        {
                            streamwriter.Write(',' + a);
                        }
                        streamwriter.Write("\r\n");
                    }
                }
                streamwriter.Close();
            }
            MessageBox.Show("OK");
        }
        //2023_1_3_数据成单列组织
        private void button62_Click(object sender, EventArgs e)
        {
            #region----//定义字典分别存储每年优化后的航行时间和等待时间
            string destPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withoutreexported\新建文件夹_成单列";
            DirectoryInfo theFolder1 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withoutreexported\新建文件夹");
            FileInfo[] dirFile1 = theFolder1.GetFiles();
            foreach (FileInfo nextFile1 in dirFile1)
            {
                StreamReader streamreader1 = new StreamReader(nextFile1.FullName);
                string[] arrtemp0_0 = nextFile1.Name.Split('_');
                List<string> Lis_Temp = new List<string>();
                List<string> Lis_Temp1 = new List<string>();
                string line0_0 = "";
                int j = 0;
                while ((line0_0 = streamreader1.ReadLine()) != null)
                {
                    string[] arrtemp0_1 = line0_0.Split(',');
                    j = arrtemp0_1.Count();
                    for (int i = 0; i < arrtemp0_1.Count(); i++)
                    {
                        Lis_Temp.Add(arrtemp0_1[i]);
                    }
                }
                for (int i = 0; i < j; i++)
                {
                    Lis_Temp1.Add(Lis_Temp[i]);
                    Lis_Temp1.Add(Lis_Temp[i + j]);
                    Lis_Temp1.Add(Lis_Temp[i + 2 * j]);
                    Lis_Temp1.Add(Lis_Temp[i + 3 * j]);
                    Lis_Temp1.Add(Lis_Temp[i + 4 * j]);
                }
                string destPath1 = System.IO.Path.Combine(destPath, nextFile1.Name);
                StreamWriter streamwriter = new StreamWriter(destPath1);
                foreach (string a in Lis_Temp1)
                {
                    streamwriter.WriteLine(a);
                }
                streamwriter.Close();
            }
            #endregion
            MessageBox.Show("OK");
        }
        //2023_01_02 Virtual Arrival碳排放减少计算数据组织
        //处理包括再出口的
        private void button63_Click(object sender, EventArgs e)
        {
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withreexported\ForVirtualArrival");
            //string destPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withreexported\ForVirtualArrival_Reorgnize";
            // DirectoryInfo theFolder1 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withreexported\新建文件夹");
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withreexported\ForVirtualArrival");
            string destPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withreexported\ForVirtualArrival_Reorgnize";
            DirectoryInfo theFolder1 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withreexported\新建文件夹");
            FileInfo[] dirFile = theFolder.GetFiles();
            #region----//定义字典存储每年每条航线上不同吨位的船舶的时间成本
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> Dic_Re = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            foreach (FileInfo nextFile in dirFile)
            {
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line = "";
                string[] arrtemp0 = nextFile.Name.Split('.');
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split(',');
                    if (Dic_Re.ContainsKey(arrtemp0[0]))
                    {
                        if (Dic_Re[arrtemp0[0]].ContainsKey(arrtemp[0] + ',' + arrtemp[1]))

                        {

                            Dic_Re[arrtemp0[0]][arrtemp[0] + ',' + arrtemp[1]].Add(arrtemp[2], arrtemp[3]);
                        }
                        else
                        {
                            Dictionary<string, string> Dic_temp = new Dictionary<string, string>();
                            Dic_temp.Add(arrtemp[2], arrtemp[3]);
                            Dic_Re[arrtemp0[0]].Add(arrtemp[0] + ',' + arrtemp[1], Dic_temp);
                        }
                    }
                    else
                    {
                        Dictionary<string, string> Dic_temp = new Dictionary<string, string>();
                        Dic_temp.Add(arrtemp[2], arrtemp[3]);
                        Dictionary<string, Dictionary<string, string>> Dic_temp1 = new Dictionary<string, Dictionary<string, string>>();
                        Dic_temp1.Add(arrtemp[0] + ',' + arrtemp[1], Dic_temp);
                        Dic_Re.Add(arrtemp0[0], Dic_temp1);
                    }
                }
            }
            #endregion
            #region----//定义字典分别存储每年优化后的航行时间和等待时间
            Dictionary<string, List<string>> SailTime = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> WaitTime = new Dictionary<string, List<string>>();
            FileInfo[] dirFile1 = theFolder1.GetFiles();
            foreach (FileInfo nextFile1 in dirFile1)
            {
                StreamReader streamreader1 = new StreamReader(nextFile1.FullName);
                string[] arrtemp0_0 = nextFile1.Name.Split('_');
                List<string> Lis_Temp = new List<string>();
                List<string> Lis_Temp1 = new List<string>();
                string line0_0 = "";
                int j = 0;
                while ((line0_0 = streamreader1.ReadLine()) != null)
                {
                    string[] arrtemp0_1 = line0_0.Split(',');
                    j = arrtemp0_1.Count();
                    for (int i = 0; i < arrtemp0_1.Count(); i++)
                    {
                        Lis_Temp.Add(arrtemp0_1[i]);
                    }
                }
                for (int i = 0; i < j; i++)
                {
                    Lis_Temp1.Add(Lis_Temp[i]);
                    Lis_Temp1.Add(Lis_Temp[i + j]);
                    Lis_Temp1.Add(Lis_Temp[i + 2 * j]);
                    Lis_Temp1.Add(Lis_Temp[i + 3 * j]);
                    Lis_Temp1.Add(Lis_Temp[i + 4 * j]);
                }
                if (arrtemp0_0[3] == "Sail")
                {
                    SailTime.Add(arrtemp0_0[0] + '_' + arrtemp0_0[2] + '_' + arrtemp0_0[3], Lis_Temp1);
                }
                if (arrtemp0_0[3] == "Wait")
                {
                    WaitTime.Add(arrtemp0_0[0] + '_' + arrtemp0_0[2] + '_' + arrtemp0_0[3], Lis_Temp1);
                }
            }
            #endregion
            //写出结果
            //定义字典存储需要输出的结果
            Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> Dic_Re_outPUT = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> Dic_Re_outPUT1 = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();
            #region
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, string>>> kvp1 in Dic_Re)
            {
                string[] arrtemp1_0 = kvp1.Key.Split('_');
                foreach (KeyValuePair<string, List<string>> kvp2 in SailTime)
                {
                    string[] arrtemp1_1 = kvp2.Key.Split('_');
                    if (arrtemp1_0[0] == arrtemp1_1[0] && arrtemp1_0[3] == arrtemp1_1[1])
                    {
                        int i = 0;
                        int j = 0;
                        foreach (KeyValuePair<string, Dictionary<string, string>> kvp3 in kvp1.Value)
                        {
                            foreach (KeyValuePair<string, string> kvp4 in kvp3.Value)
                            {
                                if (Dic_Re_outPUT.ContainsKey(kvp1.Key))
                                {
                                    if (Dic_Re_outPUT[kvp1.Key].ContainsKey(kvp3.Key))
                                    {
                                        if (Dic_Re_outPUT[kvp1.Key][kvp3.Key].ContainsKey(kvp4.Key))
                                        {
                                            List<string> Lis_TEMP = new List<string>();
                                            Lis_TEMP.Add(kvp4.Value);
                                            Lis_TEMP.Add(kvp2.Value[i + j]);
                                            Dic_Re_outPUT[kvp1.Key][kvp3.Key][kvp4.Key] = Lis_TEMP;
                                        }
                                        else
                                        {
                                            List<string> Lis_TEMP = new List<string>();
                                            Lis_TEMP.Add(kvp4.Value);
                                            Lis_TEMP.Add(kvp2.Value[i + j]);
                                            Dic_Re_outPUT[kvp1.Key][kvp3.Key].Add(kvp4.Key, Lis_TEMP);
                                        }
                                    }
                                    else
                                    {
                                        List<string> Lis_TEMP = new List<string>();
                                        Lis_TEMP.Add(kvp4.Value);
                                        Lis_TEMP.Add(kvp2.Value[i + j]);
                                        Dictionary<string, List<string>> Dic_temp = new Dictionary<string, List<string>>();
                                        Dic_temp.Add(kvp4.Key, Lis_TEMP);
                                        Dic_Re_outPUT[kvp1.Key].Add(kvp3.Key, Dic_temp);
                                    }
                                }
                                else
                                {
                                    List<string> Lis_TEMP = new List<string>();
                                    Lis_TEMP.Add(kvp4.Value);
                                    Lis_TEMP.Add(kvp2.Value[i + j]);
                                    Dictionary<string, List<string>> Dic_temp = new Dictionary<string, List<string>>();
                                    Dic_temp.Add(kvp4.Key, Lis_TEMP);
                                    Dictionary<string, Dictionary<string, List<string>>> Dic_temp1 = new Dictionary<string, Dictionary<string, List<string>>>();
                                    Dic_temp1.Add(kvp3.Key, Dic_temp);
                                    Dic_Re_outPUT.Add(kvp1.Key, Dic_temp1);
                                }
                                j++;
                            }
                            j--;
                            i++;
                        }

                    }

                }
            }
            #endregion
            #region
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, List<string>>>> kvp1 in Dic_Re_outPUT)
            {
                string[] arrtemp1_0 = kvp1.Key.Split('_');
                foreach (KeyValuePair<string, List<string>> kvp2 in WaitTime)
                {
                    string[] arrtemp1_1 = kvp2.Key.Split('_');
                    if (arrtemp1_0[0] == arrtemp1_1[0] && arrtemp1_0[3] == arrtemp1_1[1])
                    {
                        int i = 0;
                        int j = 0;
                        foreach (KeyValuePair<string, Dictionary<string, List<string>>> kvp3 in kvp1.Value)
                        {
                            foreach (KeyValuePair<string, List<string>> kvp4 in kvp3.Value)
                            {
                                if (Dic_Re_outPUT1.ContainsKey(kvp1.Key))
                                {
                                    if (Dic_Re_outPUT1[kvp1.Key].ContainsKey(kvp3.Key))
                                    {
                                        if (Dic_Re_outPUT1[kvp1.Key][kvp3.Key].ContainsKey(kvp4.Key))
                                        {
                                            List<string> Lis_TEMP = new List<string>();
                                            Lis_TEMP.Add(kvp4.Value[0]);
                                            Lis_TEMP.Add(kvp4.Value[1]);
                                            Lis_TEMP.Add(kvp2.Value[i + j]);
                                            Dic_Re_outPUT1[kvp1.Key][kvp3.Key][kvp4.Key] = Lis_TEMP;
                                        }
                                        else
                                        {
                                            List<string> Lis_TEMP = new List<string>();
                                            Lis_TEMP.Add(kvp4.Value[0]);
                                            Lis_TEMP.Add(kvp4.Value[1]);
                                            Lis_TEMP.Add(kvp2.Value[i + j]);
                                            Dic_Re_outPUT1[kvp1.Key][kvp3.Key].Add(kvp4.Key, Lis_TEMP);
                                        }
                                    }
                                    else
                                    {
                                        List<string> Lis_TEMP = new List<string>();
                                        Lis_TEMP.Add(kvp4.Value[0]);
                                        Lis_TEMP.Add(kvp4.Value[1]);
                                        Lis_TEMP.Add(kvp2.Value[i + j]);
                                        Dictionary<string, List<string>> Dic_temp = new Dictionary<string, List<string>>();
                                        Dic_temp.Add(kvp4.Key, Lis_TEMP);
                                        Dic_Re_outPUT1[kvp1.Key].Add(kvp3.Key, Dic_temp);
                                    }
                                }
                                else
                                {
                                    List<string> Lis_TEMP = new List<string>();
                                    Lis_TEMP.Add(kvp4.Value[0]);
                                    Lis_TEMP.Add(kvp4.Value[1]);
                                    Lis_TEMP.Add(kvp2.Value[i + j]);
                                    Dictionary<string, List<string>> Dic_temp = new Dictionary<string, List<string>>();
                                    Dic_temp.Add(kvp4.Key, Lis_TEMP);
                                    Dictionary<string, Dictionary<string, List<string>>> Dic_temp1 = new Dictionary<string, Dictionary<string, List<string>>>();
                                    Dic_temp1.Add(kvp3.Key, Dic_temp);
                                    Dic_Re_outPUT1.Add(kvp1.Key, Dic_temp1);
                                }
                                j++;
                            }
                            j--;
                            i++;
                        }

                    }


                }
            }
            #endregion
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, List<string>>>> kvp1 in Dic_Re_outPUT1)
            {
                string destPath1 = System.IO.Path.Combine(destPath, kvp1.Key + ".csv");
                StreamWriter streamwriter = new StreamWriter(destPath1);
                foreach (KeyValuePair<string, Dictionary<string, List<string>>> kvp2 in kvp1.Value)
                {
                    foreach (KeyValuePair<string, List<string>> kvp3 in kvp2.Value)
                    {
                        streamwriter.Write(kvp2.Key + ',' + kvp3.Key);
                        foreach (string a in kvp3.Value)
                        {
                            streamwriter.Write(',' + a);
                        }
                        streamwriter.Write("\r\n");
                    }
                }
                streamwriter.Close();
            }
            MessageBox.Show("OK");
        }
        //2023_1_3_对需要计算每条航线上因VirtualArrival带来的碳排放减少比例的文件添加港口坐标
        private void button64_Click(object sender, EventArgs e)
        {
            //定义字典，存储每个港口的经纬度坐标
            Dictionary<string, List<string>> Por_Cor = new Dictionary<string, List<string>>();
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\portCenter.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp1 = line1.Split(',');
                if (Por_Cor.ContainsKey(arrtemp1[0]))
                {
                    continue;
                }
                else
                {
                    List<string> Lis_temp = new List<string>();
                    Lis_temp.Add(arrtemp1[1]);
                    Lis_temp.Add(arrtemp1[2]);
                    Por_Cor.Add(arrtemp1[0], Lis_temp);
                }
            }
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withreexported\ForVirtualArrival_Reorgnize");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withreexported\ForVirtualArrival_Reorgnize_添加港口坐标";
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withoutreexported\ForVirtualArrival_Reorgnize");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withoutreexported\ForVirtualArrival_Reorgnize_添加港口坐标";
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withreexported\ForVirtualArrival_Reorgnize");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withreexported\ForVirtualArrival_Reorgnize_添加港口坐标";
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withoutreexported\ForVirtualArrival_Reorgnize");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withoutreexported\ForVirtualArrival_Reorgnize_添加港口坐标";
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string outPath = System.IO.Path.Combine(targetPath, nextFile.Name);
                StreamWriter streamwriter = new StreamWriter(outPath);
                #region
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split(',');
                    string Cor_Ex = "";
                    string Cor_Im = "";
                    string d = "";
                    foreach (KeyValuePair<string, List<string>> KVP1_1 in Por_Cor)
                    {
                        //港口坐标存放的格式是先经度后维度
                        // if (arrtemp[0] == KVP1_1.Key)
                        string a = arrtemp[0].Substring(arrtemp[0].IndexOf(".") + 1);
                        string b = arrtemp[1].Substring(arrtemp[1].IndexOf(".") + 1);
                        if (arrtemp[0] == KVP1_1.Key || arrtemp[0].Contains(KVP1_1.Key) || KVP1_1.Key.Contains(a) || KVP1_1.Key.Contains(arrtemp[0]) || arrtemp[0].Replace(" ", "") == KVP1_1.Key.Replace("\t ", "") || Regex.Replace(arrtemp[0], @"\s", "\t") == KVP1_1.Key || Regex.Replace(arrtemp[0], @"  ", "\t") == Regex.Replace(KVP1_1.Key, @"\s", "\t") || Regex.Replace(arrtemp[0], @"\s", "\t") == Regex.Replace(KVP1_1.Key, @"\s", "\t") || Regex.Replace(arrtemp[0], @"  ", "\t") == KVP1_1.Key)
                        {
                            Cor_Ex = KVP1_1.Value[0] + '|' + KVP1_1.Value[1];

                        }
                        // if (arrtemp[1] == KVP1_1.Key)
                        if (arrtemp[1] == KVP1_1.Key || arrtemp[1].Contains(KVP1_1.Key) || KVP1_1.Key.Contains(b) || KVP1_1.Key.Contains(arrtemp[1]) || arrtemp[1].Replace(" ", "") == KVP1_1.Key.Replace("\t", "") || Regex.Replace(arrtemp[1], @"\s", "\t") == KVP1_1.Key || Regex.Replace(arrtemp[1], @"  ", "\t") == Regex.Replace(KVP1_1.Key, @"\s", "\t") || Regex.Replace(arrtemp[1], @"\s", "\t") == Regex.Replace(KVP1_1.Key, @"\s", "\t") || Regex.Replace(arrtemp[0], @"  ", "\t") == KVP1_1.Key)
                        {
                            Cor_Im = KVP1_1.Value[0] + '|' + KVP1_1.Value[1];
                        }
                        d = KVP1_1.Key;
                    }
                    if (Cor_Ex != "" && Cor_Im != "")
                    {
                        streamwriter.WriteLine(arrtemp[0] + '|' + Cor_Ex + '|' + arrtemp[1] + '|' + Cor_Im + '|' + arrtemp[2] + '|' + arrtemp[3] + '|' + arrtemp[4] + '|' + arrtemp[5]);
                    }

                    if (Cor_Ex == "" || Cor_Im == "")
                    {
                        int j = 0;
                    }
                    string c = Regex.Replace(arrtemp[0], @"  ", "\t");
                    d = Regex.Replace(d, @"\s", "\t");
                }
                #endregion
                streamwriter.Close();
            }
            MessageBox.Show("OK");
        }
        //2023_1_3计算每条航线上因VirtualArrival带来的碳排放减少比例
        private void button65_Click(object sender, EventArgs e)
        {
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withreexported\ForVirtualArrival_Reorgnize_添加港口坐标");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withreexported\ForVirtualArrival_Reorgnize_添加港口坐标_Calculation";
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withoutreexported\ForVirtualArrival_Reorgnize_添加港口坐标");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withoutreexported\ForVirtualArrival_Reorgnize_添加港口坐标_Calculation";
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withoutreexported\ForVirtualArrival_Reorgnize_添加港口坐标");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withoutreexported\ForVirtualArrival_Reorgnize_添加港口坐标_Calculation";
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withreexported\ForVirtualArrival_Reorgnize_添加港口坐标");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withreexported\ForVirtualArrival_Reorgnize_添加港口坐标_Calculation";
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string destPath = System.IO.Path.Combine(targetPath, nextFile.Name);
                StreamWriter streamwriter = new StreamWriter(destPath);
                //定义字典存储每条航线上节约的碳排放比例
                // Dictionary<string, double> Dic_Re = new Dictionary<string, double>();
                Dictionary<string, double> Dic_Re_FENZI = new Dictionary<string, double>();
                Dictionary<string, double> Dic_Re_FENMU = new Dictionary<string, double>();
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split('|');
                    //计算港口的距离
                    double dist = get_distance(Convert.ToDouble(arrtemp[2]), Convert.ToDouble(arrtemp[1]), Convert.ToDouble(arrtemp[5]), Convert.ToDouble(arrtemp[4]));
                    double EP = 0.0;
                    if (arrtemp[6] == "10000" || arrtemp[6] == "55000")
                        EP = 2576;
                    if (arrtemp[6] == "80000")
                        EP = 14632;
                    if (arrtemp[6] == "120000" || arrtemp[6] == "200000")
                        EP = 33796;
                    double Fenzi = EP * dist * Convert.ToDouble(arrtemp[8]) * Convert.ToDouble(arrtemp[8]) * Convert.ToDouble(arrtemp[9]);
                    double FenMU = EP * (Convert.ToDouble(arrtemp[8]) + Convert.ToDouble(arrtemp[9])) * (Convert.ToDouble(arrtemp[8]) + Convert.ToDouble(arrtemp[9])) * (Convert.ToDouble(arrtemp[8]) * Convert.ToDouble(arrtemp[8]) + dist * Convert.ToDouble(arrtemp[9]));
                    if (Dic_Re_FENZI.ContainsKey(arrtemp[0] + '|' + arrtemp[3]))
                    {
                        Dic_Re_FENZI[arrtemp[0] + '|' + arrtemp[3]] += Fenzi;
                    }
                    else
                    {
                        Dic_Re_FENZI.Add(arrtemp[0] + '|' + arrtemp[3], Fenzi);
                    }
                    if (Dic_Re_FENMU.ContainsKey(arrtemp[0] + '|' + arrtemp[3]))
                    {
                        Dic_Re_FENMU[arrtemp[0] + '|' + arrtemp[3]] += FenMU;
                    }
                    else
                    {
                        Dic_Re_FENMU.Add(arrtemp[0] + '|' + arrtemp[3], FenMU);
                    }
                }
                foreach (KeyValuePair<string, double> kvp1 in Dic_Re_FENZI)
                {
                    foreach (KeyValuePair<string, double> kvp2 in Dic_Re_FENMU)
                    {
                        if (kvp1.Key == kvp2.Key)
                        {
                            streamwriter.WriteLine(kvp1.Key + '|' + Convert.ToString(kvp1.Value / kvp2.Value));
                        }
                    }
                }
                streamwriter.Close();
            }
            MessageBox.Show("OK");
        }
        //2023_1_3将Virtual Arrival每年的结果合并
        private void button66_Click(object sender, EventArgs e)
        {
            //DirectoryInfo theFolder1 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withreexported\ForVirtualArrival_Reorgnize_添加港口坐标_Calculation");
            //DirectoryInfo theFolder= new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\Withoutreexported\ForVirtualArrival_Reorgnize_添加港口坐标_Calculation");
            //string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\ForVirtualArrival_Reorgnize_添加港口坐标_Calculation_Union";
            DirectoryInfo theFolder1 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withreexported\ForVirtualArrival_Reorgnize_添加港口坐标_Calculation");
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\Withoutreexported\ForVirtualArrival_Reorgnize_添加港口坐标_Calculation");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\ForVirtualArrival_Reorgnize_添加港口坐标_Calculation_Union";
            //定义字典存储每年不同航线、什么类型的航线、节约的比例
            Dictionary<string, Dictionary<string, Dictionary<string, double>>> AveTime = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>();
            FileInfo[] dirFile = theFolder.GetFiles();
            FileInfo[] dirFile1 = theFolder1.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string[] arrtemp = nextFile.Name.Split('_');
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp1 = line.Split('|');
                    if (nextFile.Name.Contains("AveTime"))
                    {
                        if (AveTime.ContainsKey(arrtemp[0]))
                        {
                            if (AveTime[arrtemp[0]].ContainsKey("NoneReexport"))
                            {
                                if (AveTime[arrtemp[0]]["NoneReexport"].ContainsKey(arrtemp1[0] + '|' + arrtemp1[1]))
                                {
                                    if (AveTime[arrtemp[0]]["NoneReexport"][arrtemp1[0] + '|' + arrtemp1[1]] <= Convert.ToDouble(arrtemp1[2]))
                                    {
                                        AveTime[arrtemp[0]]["NoneReexport"][arrtemp1[0] + '|' + arrtemp1[1]] = Convert.ToDouble(arrtemp1[2]);
                                    }
                                }
                                else
                                {
                                    AveTime[arrtemp[0]]["NoneReexport"].Add(arrtemp1[0] + '|' + arrtemp1[1], Convert.ToDouble(arrtemp1[2]));
                                }
                            }
                            else
                            {
                                Dictionary<string, double> Dic_temp = new Dictionary<string, double>();
                                Dic_temp.Add(arrtemp1[0] + '|' + arrtemp1[1], Convert.ToDouble(arrtemp1[2]));
                                AveTime[arrtemp[0]].Add("NoneReexport", Dic_temp);
                            }
                        }
                        else
                        {
                            Dictionary<string, double> Dic_temp = new Dictionary<string, double>();
                            Dic_temp.Add(arrtemp1[0] + '|' + arrtemp1[1], Convert.ToDouble(arrtemp1[2]));
                            Dictionary<string, Dictionary<string, double>> Dic_temp1 = new Dictionary<string, Dictionary<string, double>>();
                            Dic_temp1.Add("NoneReexport", Dic_temp);
                            AveTime.Add(arrtemp[0], Dic_temp1);
                        }
                    }

                }
            }
            foreach (FileInfo nextFile1 in dirFile1)
            {
                string[] arrtemp = nextFile1.Name.Split('_');
                StreamReader streamreader = new StreamReader(nextFile1.FullName);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp1 = line.Split('|');
                    if (nextFile1.Name.Contains("AveTime"))
                    {
                        if (AveTime.ContainsKey(arrtemp[0]))
                        {
                            if (AveTime[arrtemp[0]].ContainsKey(arrtemp[3]))
                            {
                                if (AveTime[arrtemp[0]][arrtemp[3]].ContainsKey(arrtemp1[0] + '|' + arrtemp1[1]))
                                {
                                    if (AveTime[arrtemp[0]][arrtemp[3]][arrtemp1[0] + '|' + arrtemp1[1]] <= Convert.ToDouble(arrtemp1[2]))
                                    {
                                        AveTime[arrtemp[0]][arrtemp[3]][arrtemp1[0] + '|' + arrtemp1[1]] = Convert.ToDouble(arrtemp1[2]);
                                    }
                                }
                                else
                                {
                                    AveTime[arrtemp[0]][arrtemp[3]].Add(arrtemp1[0] + '|' + arrtemp1[1], Convert.ToDouble(arrtemp1[2]));
                                }
                            }
                            else
                            {
                                Dictionary<string, double> Dic_temp = new Dictionary<string, double>();
                                Dic_temp.Add(arrtemp1[0] + '|' + arrtemp1[1], Convert.ToDouble(arrtemp1[2]));
                                AveTime[arrtemp[0]].Add(arrtemp[3], Dic_temp);
                            }
                        }
                        else
                        {
                            Dictionary<string, double> Dic_temp = new Dictionary<string, double>();
                            Dic_temp.Add(arrtemp1[0] + '|' + arrtemp1[1], Convert.ToDouble(arrtemp1[2]));
                            Dictionary<string, Dictionary<string, double>> Dic_temp1 = new Dictionary<string, Dictionary<string, double>>();
                            Dic_temp1.Add(arrtemp[3], Dic_temp);
                            AveTime.Add(arrtemp[0], Dic_temp1);
                        }
                    }

                }
            }
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, double>>> kvp1 in AveTime)
            {
                foreach (KeyValuePair<string, Dictionary<string, double>> kvp2 in kvp1.Value)
                {
                    foreach (KeyValuePair<string, double> kvp3 in kvp2.Value)
                    {
                        string destPath = System.IO.Path.Combine(targetPath, kvp1.Key + ".csv");
                        FileStream fs = new FileStream(destPath, FileMode.Append);
                        StreamWriter streamwriter = new StreamWriter(fs);
                        streamwriter.WriteLine(kvp3.Key + '|' + kvp2.Key + '|' + kvp3.Value);
                        streamwriter.Close();
                    }
                }
            }

            MessageBox.Show("OK");
        }
        //2023-01-03由于前面优化过程只考虑了船型大于3种的航线，因此需要对其他航线计算Virtual Arrival 带来的减少比例
        private void button67_Click(object sender, EventArgs e)
        {
            #region-----定义字典统计每年所有的航线的航行时间和等待时间(区分有无再进出口)
            DirectoryInfo theFolder1 = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图");
            //每年 是否再出口 什么航线 什么吨位船舶的航行时间和等待时间
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<double>>>>> Lanes = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<double>>>>>();
            DirectoryInfo[] dirFolder1 = theFolder1.GetDirectories();
            foreach (DirectoryInfo nextFolder1 in dirFolder1)
            {
                //#region------区分Sailing和Waiting
                //DirectoryInfo[] dirFolder2 = nextFolder1.GetDirectories();
                //string b_0 = "";
                //if (nextFolder1.Name == "WithoutReexport")
                //{
                //    b_0 = "NoneReexport";
                //}
                //foreach (DirectoryInfo nextFolder2 in dirFolder2)
                //{
                //    FileInfo[] dirFile1 = nextFolder2.GetFiles();

                //    foreach (FileInfo nextFile1 in dirFile1)
                //    {
                //        string[] arrtemp0 = nextFile1.Name.Split('.');
                //        string[] arrtemp1 = arrtemp0[0].Split('_');
                //        string a_0 = "";
                //        if (arrtemp1[0] == "AveSaiTime")
                //        {
                //            a_0 = "Sailing";
                //        }
                //        else
                //        {
                //            a_0 = "Waiting";
                //        }
                //        if (b_0 == "")
                //        {
                //            b_0 = arrtemp1[2];
                //        }
                //        StreamReader streamreader1 = new StreamReader(nextFile1.FullName);
                //        string line1_1 = "";
                //        while ((line1_1 = streamreader1.ReadLine()) != null)
                //        {
                //            string[] arrtemp1_1 = line1_1.Split('|');
                //            if (Lanes.ContainsKey(nextFolder2.Name + '_' + a_0))
                //            {
                //                if (Lanes[nextFolder2.Name + '_' + a_0].ContainsKey(b_0))
                //                {
                //                    if (Lanes[nextFolder2.Name + '_' + a_0][b_0].ContainsKey(arrtemp1_1[0] + '|' + arrtemp1_1[1]))
                //                    {
                //                        if (Lanes[nextFolder2.Name + '_' + a_0][b_0][arrtemp1_1[0] + '|' + arrtemp1_1[1]].ContainsKey(arrtemp1_1[2]))
                //                        {
                //                            Lanes[nextFolder2.Name + '_' + a_0][b_0][arrtemp1_1[0] + '|' + arrtemp1_1[1]][arrtemp1_1[2]].Add(Convert.ToDouble(arrtemp1_1[3]));
                //                        }
                //                        else
                //                        {
                //                            List<double> Lis_Temp = new List<double>();
                //                            Lis_Temp.Add(Convert.ToDouble(arrtemp1_1[3]));
                //                            Lanes[nextFolder2.Name + '_' + a_0][b_0][arrtemp1_1[0] + '|' + arrtemp1_1[1]].Add(arrtemp1_1[2], Lis_Temp);
                //                        }
                //                    }
                //                    else
                //                    {
                //                        List<double> Lis_Temp = new List<double>();
                //                        Lis_Temp.Add(Convert.ToDouble(arrtemp1_1[3]));
                //                        Dictionary<string, List<double>> Dic_Temp = new Dictionary<string, List<double>>();
                //                        Dic_Temp.Add(arrtemp1_1[2], Lis_Temp);
                //                        Lanes[nextFolder2.Name + '_' + a_0][b_0].Add(arrtemp1_1[0] + '|' + arrtemp1_1[1], Dic_Temp);
                //                    }
                //                }
                //                else
                //                {
                //                    List<double> Lis_Temp = new List<double>();
                //                    Lis_Temp.Add(Convert.ToDouble(arrtemp1_1[3]));
                //                    Dictionary<string, List<double>> Dic_Temp = new Dictionary<string, List<double>>();
                //                    Dic_Temp.Add(arrtemp1_1[2], Lis_Temp);
                //                    Dictionary<string, Dictionary<string, List<double>>> Dic_Temp1 = new Dictionary<string, Dictionary<string, List<double>>>();
                //                    Dic_Temp1.Add(arrtemp1_1[0] + '|' + arrtemp1_1[1], Dic_Temp);
                //                    Lanes[nextFolder2.Name + '_' + a_0].Add(b_0, Dic_Temp1);
                //                }
                //            }
                //            else
                //            {
                //                List<double> Lis_Temp = new List<double>();
                //                Lis_Temp.Add(Convert.ToDouble(arrtemp1_1[3]));
                //                Dictionary<string, List<double>> Dic_Temp = new Dictionary<string, List<double>>();
                //                Dic_Temp.Add(arrtemp1_1[2], Lis_Temp);
                //                Dictionary<string, Dictionary<string, List<double>>> Dic_Temp1 = new Dictionary<string, Dictionary<string, List<double>>>();
                //                Dic_Temp1.Add(arrtemp1_1[0] + '|' + arrtemp1_1[1], Dic_Temp);
                //                Dictionary<string, Dictionary<string, Dictionary<string, List<double>>>> Dic_Temp2 = new Dictionary<string, Dictionary<string, Dictionary<string, List<double>>>>();
                //                Dic_Temp2.Add(b_0, Dic_Temp1);
                //                Lanes.Add(nextFolder2.Name + '_' + a_0, Dic_Temp2);

                //            }

                //        }
                //        if (b_0 != "NoneReexport")
                //            b_0 = "";
                //    }

                //}
                //#endregion
                #region-------字典键值不区分Sailing和Waiting
                DirectoryInfo[] dirFolder2 = nextFolder1.GetDirectories();
                string b_0 = "";
                if (nextFolder1.Name == "WithoutReexport")
                {
                    b_0 = "NoneReexport";
                }
                foreach (DirectoryInfo nextFolder2 in dirFolder2)
                {
                    FileInfo[] dirFile1 = nextFolder2.GetFiles();

                    foreach (FileInfo nextFile1 in dirFile1)
                    {
                        string[] arrtemp0 = nextFile1.Name.Split('.');
                        string[] arrtemp1 = arrtemp0[0].Split('_');
                        //string a_0 = "";
                        //if (arrtemp1[0] == "AveSaiTime")
                        //{
                        //    a_0 = "Sailing";
                        //}
                        //else
                        //{
                        //    a_0 = "Waiting";
                        //}
                        if (b_0 == "")
                        {
                            b_0 = arrtemp1[2];
                        }
                        StreamReader streamreader1 = new StreamReader(nextFile1.FullName);
                        string line1_1 = "";
                        while ((line1_1 = streamreader1.ReadLine()) != null)
                        {
                            string[] arrtemp1_1 = line1_1.Split('|');
                            if (Lanes.ContainsKey(nextFolder2.Name))
                            {
                                if (Lanes[nextFolder2.Name].ContainsKey(b_0))
                                {
                                    if (Lanes[nextFolder2.Name][b_0].ContainsKey(arrtemp1_1[0] + '|' + arrtemp1_1[1]))
                                    {
                                        if (Lanes[nextFolder2.Name][b_0][arrtemp1_1[0] + '|' + arrtemp1_1[1]].ContainsKey(arrtemp1_1[2]))
                                        {
                                            Lanes[nextFolder2.Name][b_0][arrtemp1_1[0] + '|' + arrtemp1_1[1]][arrtemp1_1[2]].Add(Convert.ToDouble(arrtemp1_1[3]));
                                        }
                                        else
                                        {
                                            List<double> Lis_Temp = new List<double>();
                                            Lis_Temp.Add(Convert.ToDouble(arrtemp1_1[3]));
                                            Lanes[nextFolder2.Name][b_0][arrtemp1_1[0] + '|' + arrtemp1_1[1]].Add(arrtemp1_1[2], Lis_Temp);
                                        }
                                    }
                                    else
                                    {
                                        List<double> Lis_Temp = new List<double>();
                                        Lis_Temp.Add(Convert.ToDouble(arrtemp1_1[3]));
                                        Dictionary<string, List<double>> Dic_Temp = new Dictionary<string, List<double>>();
                                        Dic_Temp.Add(arrtemp1_1[2], Lis_Temp);
                                        Lanes[nextFolder2.Name][b_0].Add(arrtemp1_1[0] + '|' + arrtemp1_1[1], Dic_Temp);
                                    }
                                }
                                else
                                {
                                    List<double> Lis_Temp = new List<double>();
                                    Lis_Temp.Add(Convert.ToDouble(arrtemp1_1[3]));
                                    Dictionary<string, List<double>> Dic_Temp = new Dictionary<string, List<double>>();
                                    Dic_Temp.Add(arrtemp1_1[2], Lis_Temp);
                                    Dictionary<string, Dictionary<string, List<double>>> Dic_Temp1 = new Dictionary<string, Dictionary<string, List<double>>>();
                                    Dic_Temp1.Add(arrtemp1_1[0] + '|' + arrtemp1_1[1], Dic_Temp);
                                    Lanes[nextFolder2.Name].Add(b_0, Dic_Temp1);
                                }
                            }
                            else
                            {
                                List<double> Lis_Temp = new List<double>();
                                Lis_Temp.Add(Convert.ToDouble(arrtemp1_1[3]));
                                Dictionary<string, List<double>> Dic_Temp = new Dictionary<string, List<double>>();
                                Dic_Temp.Add(arrtemp1_1[2], Lis_Temp);
                                Dictionary<string, Dictionary<string, List<double>>> Dic_Temp1 = new Dictionary<string, Dictionary<string, List<double>>>();
                                Dic_Temp1.Add(arrtemp1_1[0] + '|' + arrtemp1_1[1], Dic_Temp);
                                Dictionary<string, Dictionary<string, Dictionary<string, List<double>>>> Dic_Temp2 = new Dictionary<string, Dictionary<string, Dictionary<string, List<double>>>>();
                                Dic_Temp2.Add(b_0, Dic_Temp1);
                                Lanes.Add(nextFolder2.Name, Dic_Temp2);

                            }

                        }
                        if (b_0 != "NoneReexport")
                            b_0 = "";
                    }

                }
                #endregion

            }
            #region------定义字典存储港口的经纬度坐标
            //定义字典，存储每个港口的经纬度坐标
            Dictionary<string, List<string>> Por_Cor = new Dictionary<string, List<string>>();
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\portCenter.csv";
            StreamReader streamreader0 = new StreamReader(inPath1);
            string line1 = "";
            while ((line1 = streamreader0.ReadLine()) != null)
            {
                string[] arrtemp1 = line1.Split(',');
                if (Por_Cor.ContainsKey(arrtemp1[0]))
                {
                    continue;
                }
                else
                {
                    List<string> Lis_temp = new List<string>();
                    Lis_temp.Add(arrtemp1[1]);
                    Lis_temp.Add(arrtemp1[2]);
                    Por_Cor.Add(arrtemp1[0], Lis_temp);
                }
            }
            #endregion
            #endregion
            //DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\AfterOptimization\ForVirtualArrival_Reorgnize_添加港口坐标_Calculation_Union");
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2017\ForVirtualArrival_Reorgnize_添加港口坐标_Calculation_Union");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\VirtualArrival_Union_Making Up";
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                StreamReader streamreader2 = new StreamReader(nextFile.FullName);
                string destPath = System.IO.Path.Combine(targetPath, nextFile.Name);
                StreamWriter streamwriter = new StreamWriter(destPath);
                string line2_0 = "";
                //定义字典存储每条航线减少的比例
                Dictionary<string, Dictionary<string, double>> result_0 = new Dictionary<string, Dictionary<string, double>>();
                //定义列表存储减少的比例集合
                List<double> Lis_0 = new List<double>();
                while ((line2_0 = streamreader2.ReadLine()) != null)
                {
                    string[] arrtemp2_1 = line2_0.Split('|');
                    if (result_0.ContainsKey(arrtemp2_1[0] + '|' + arrtemp2_1[1]))
                    {
                        result_0[arrtemp2_1[0] + '|' + arrtemp2_1[1]].Add(arrtemp2_1[2], Convert.ToDouble(arrtemp2_1[3]));
                    }
                    else
                    {
                        Dictionary<string, double> Dic_Temp = new Dictionary<string, double>();
                        Dic_Temp.Add(arrtemp2_1[2], Convert.ToDouble(arrtemp2_1[3]));
                        result_0.Add(arrtemp2_1[0] + '|' + arrtemp2_1[1], Dic_Temp);
                    }
                    Lis_0.Add(Convert.ToDouble(arrtemp2_1[3]));
                }
                string[] arrtemp2_0 = nextFile.Name.Split('.');
                Dictionary<string, Dictionary<string, double>> result_01 = new Dictionary<string, Dictionary<string, double>>();
                foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, Dictionary<string, List<double>>>>> kvp1 in Lanes)
                {
                    if (kvp1.Key == arrtemp2_0[0])
                    {
                        foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, List<double>>>> kvp2 in kvp1.Value)
                        {
                            foreach (KeyValuePair<string, Dictionary<string, List<double>>> kvp3 in kvp2.Value)
                            {

                                foreach (KeyValuePair<string, Dictionary<string, double>> kvp1_1 in result_0)
                                {
                                    string[] arrtemp = kvp1_1.Key.Split('|');
                                    foreach (KeyValuePair<string, double> KVP2_1 in kvp1_1.Value)
                                    {
                                        if (KVP2_1.Key == kvp2.Key && result_0.Keys.Contains(kvp3.Key) == false)
                                        {
                                            double Fenzi = 0.0;
                                            double FenMU = 0.0;
                                            foreach (KeyValuePair<string, List<double>> kvp4 in kvp3.Value)
                                            {
                                                if (kvp4.Value.Count() == 2)
                                                {
                                                    #region------得到经纬度坐标
                                                    string Cor_Ex = "";
                                                    string Cor_Im = "";
                                                    string d = "";
                                                    foreach (KeyValuePair<string, List<string>> KVP1_1 in Por_Cor)
                                                    {
                                                        //港口坐标存放的格式是先经度后维度
                                                        // if (arrtemp[0] == KVP1_1.Key)
                                                        string a = arrtemp[0].Substring(arrtemp[0].IndexOf(".") + 1);
                                                        string b = arrtemp[1].Substring(arrtemp[1].IndexOf(".") + 1);
                                                        if (arrtemp[0] == KVP1_1.Key || arrtemp[0].Contains(KVP1_1.Key) || KVP1_1.Key.Contains(a) || KVP1_1.Key.Contains(arrtemp[0]) || arrtemp[0].Replace(" ", "") == KVP1_1.Key.Replace("\t ", "") || Regex.Replace(arrtemp[0], @"\s", "\t") == KVP1_1.Key || Regex.Replace(arrtemp[0], @"  ", "\t") == Regex.Replace(KVP1_1.Key, @"\s", "\t") || Regex.Replace(arrtemp[0], @"\s", "\t") == Regex.Replace(KVP1_1.Key, @"\s", "\t") || Regex.Replace(arrtemp[0], @"  ", "\t") == KVP1_1.Key)
                                                        {
                                                            Cor_Ex = KVP1_1.Value[0] + '|' + KVP1_1.Value[1];

                                                        }
                                                        // if (arrtemp[1] == KVP1_1.Key)
                                                        if (arrtemp[1] == KVP1_1.Key || arrtemp[1].Contains(KVP1_1.Key) || KVP1_1.Key.Contains(b) || KVP1_1.Key.Contains(arrtemp[1]) || arrtemp[1].Replace(" ", "") == KVP1_1.Key.Replace("\t", "") || Regex.Replace(arrtemp[1], @"\s", "\t") == KVP1_1.Key || Regex.Replace(arrtemp[1], @"  ", "\t") == Regex.Replace(KVP1_1.Key, @"\s", "\t") || Regex.Replace(arrtemp[1], @"\s", "\t") == Regex.Replace(KVP1_1.Key, @"\s", "\t") || Regex.Replace(arrtemp[0], @"  ", "\t") == KVP1_1.Key)
                                                        {
                                                            Cor_Im = KVP1_1.Value[0] + '|' + KVP1_1.Value[1];
                                                        }
                                                        d = KVP1_1.Key;
                                                    }
                                                    #endregion
                                                    string[] arrtemp1_EX = Cor_Ex.Split('|');
                                                    string[] arrtemp1_IM = Cor_Im.Split('|');
                                                    double dist = get_distance(Convert.ToDouble(arrtemp1_EX[1]), Convert.ToDouble(arrtemp1_EX[0]), Convert.ToDouble(arrtemp1_IM[1]), Convert.ToDouble(arrtemp1_IM[0]));
                                                    double EP = 0.0;
                                                    if (kvp4.Key == "10000" || kvp4.Key == "55000")
                                                        EP = 2576;
                                                    if (kvp4.Key == "80000")
                                                        EP = 14632;
                                                    if (kvp4.Key == "120000" || kvp4.Key == "200000")
                                                        EP = 33796;
                                                    Fenzi += EP * dist * Convert.ToDouble(kvp4.Value[0]) * Convert.ToDouble(kvp4.Value[0]) * Convert.ToDouble(kvp4.Value[1]);
                                                    FenMU += EP * (Convert.ToDouble(kvp4.Value[0]) + Convert.ToDouble(kvp4.Value[1])) * (Convert.ToDouble(kvp4.Value[0]) + Convert.ToDouble(kvp4.Value[1])) * (Convert.ToDouble(kvp4.Value[0]) * Convert.ToDouble(kvp4.Value[0]) + dist * Convert.ToDouble(kvp4.Value[1]));
                                                }
                                                if (result_01.ContainsKey(kvp3.Key))
                                                {
                                                    if (result_01[kvp3.Key].ContainsKey(KVP2_1.Key))
                                                    {
                                                        if (result_01[kvp3.Key][KVP2_1.Key] <= Fenzi / FenMU)
                                                        {
                                                            result_01[kvp3.Key][KVP2_1.Key] = Fenzi / FenMU;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        result_01[kvp3.Key].Add(KVP2_1.Key, Fenzi / FenMU);
                                                    }
                                                }
                                                else
                                                {
                                                    Dictionary<string, double> DIC_TEMP00 = new Dictionary<string, double>();
                                                    DIC_TEMP00.Add(KVP2_1.Key, Fenzi / FenMU);
                                                    result_01.Add(kvp3.Key, DIC_TEMP00);
                                                }
                                            }

                                        }
                                    }

                                }
                            }

                        }
                    }
                }


                foreach (KeyValuePair<string, Dictionary<string, double>> kvp2 in result_0)
                {
                    foreach (KeyValuePair<string, double> kvp22 in kvp2.Value)
                    {
                        streamwriter.WriteLine(kvp2.Key + '|' + Convert.ToString(kvp22.Key) + '|' + Convert.ToString(kvp22.Value));
                    }
                }
                foreach (KeyValuePair<string, Dictionary<string, double>> kvp2 in result_01)
                {
                    foreach (KeyValuePair<string, double> kvp22 in kvp2.Value)
                    {
                        if (kvp22.Value != System.Double.NaN)
                            streamwriter.WriteLine(kvp2.Key + '|' + Convert.ToString(kvp22.Key) + '|' + Convert.ToString(kvp22.Value));
                    }
                }

                streamwriter.Close();
            }
            MessageBox.Show("OK");
        }
        //23_1_07为VirtualArrival减少的比例添加坐标
        private void button68_Click(object sender, EventArgs e)
        {
            //定义字典，存储每个港口的经纬度坐标
            Dictionary<string, List<string>> Por_Cor = new Dictionary<string, List<string>>();
            string inPath1 = @"H:\2018-2021LNG和Container数据_Decoder\解码\portCenter.csv";
            StreamReader streamreader1 = new StreamReader(inPath1);
            string line1 = "";
            while ((line1 = streamreader1.ReadLine()) != null)
            {
                string[] arrtemp1 = line1.Split(',');
                if (Por_Cor.ContainsKey(arrtemp1[0]))
                {
                    continue;
                }
                else
                {
                    List<string> Lis_temp = new List<string>();
                    Lis_temp.Add(arrtemp1[1]);
                    Lis_temp.Add(arrtemp1[2]);
                    Por_Cor.Add(arrtemp1[0], Lis_temp);
                }
            }
            DirectoryInfo theFolder = new DirectoryInfo(@"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\VirtualArrival_Union_Making Up");
            string targetPath = @"H:\2018-2021LNG和Container数据_Decoder\解码\作图1\After Optimization 2014-2021_22_12_28\VirtualArrival_Union_Making Up_Adding Coordination";
            FileInfo[] dirFile = theFolder.GetFiles();
            foreach (FileInfo nextFile in dirFile)
            {
                string outPath = System.IO.Path.Combine(targetPath, nextFile.Name);
                StreamWriter streamwriter = new StreamWriter(outPath);
                #region
                StreamReader streamreader = new StreamReader(nextFile.FullName);
                string line = "";
                while ((line = streamreader.ReadLine()) != null)
                {
                    string[] arrtemp = line.Split('|');
                    string Cor_Ex = "";
                    string Cor_Im = "";
                    string d = "";
                    foreach (KeyValuePair<string, List<string>> KVP1_1 in Por_Cor)
                    {
                        //港口坐标存放的格式是先经度后维度
                        // if (arrtemp[0] == KVP1_1.Key)
                        string a = arrtemp[0].Substring(arrtemp[0].IndexOf(".") + 1);
                        string b = arrtemp[1].Substring(arrtemp[1].IndexOf(".") + 1);
                        if (arrtemp[0] == KVP1_1.Key || arrtemp[0].Contains(KVP1_1.Key) || KVP1_1.Key.Contains(a) || KVP1_1.Key.Contains(arrtemp[0]) || arrtemp[0].Replace(" ", "") == KVP1_1.Key.Replace("\t ", "") || Regex.Replace(arrtemp[0], @"\s", "\t") == KVP1_1.Key || Regex.Replace(arrtemp[0], @"  ", "\t") == Regex.Replace(KVP1_1.Key, @"\s", "\t") || Regex.Replace(arrtemp[0], @"\s", "\t") == Regex.Replace(KVP1_1.Key, @"\s", "\t") || Regex.Replace(arrtemp[0], @"  ", "\t") == KVP1_1.Key)
                        {
                            Cor_Ex = KVP1_1.Value[0] + '|' + KVP1_1.Value[1];

                        }
                        // if (arrtemp[1] == KVP1_1.Key)
                        if (arrtemp[1] == KVP1_1.Key || arrtemp[1].Contains(KVP1_1.Key) || KVP1_1.Key.Contains(b) || KVP1_1.Key.Contains(arrtemp[1]) || arrtemp[1].Replace(" ", "") == KVP1_1.Key.Replace("\t", "") || Regex.Replace(arrtemp[1], @"\s", "\t") == KVP1_1.Key || Regex.Replace(arrtemp[1], @"  ", "\t") == Regex.Replace(KVP1_1.Key, @"\s", "\t") || Regex.Replace(arrtemp[1], @"\s", "\t") == Regex.Replace(KVP1_1.Key, @"\s", "\t") || Regex.Replace(arrtemp[0], @"  ", "\t") == KVP1_1.Key)
                        {
                            Cor_Im = KVP1_1.Value[0] + '|' + KVP1_1.Value[1];
                        }
                        d = KVP1_1.Key;
                    }
                    if (Cor_Ex != "" && Cor_Im != "")
                    {
                        streamwriter.WriteLine(arrtemp[0] + '|' + Cor_Ex + '|' + arrtemp[1] + '|' + Cor_Im + '|' + arrtemp[2] + '|' + arrtemp[3]);
                    }

                    if (Cor_Ex == "" || Cor_Im == "")
                    {
                        int j = 0;
                    }
                    string c = Regex.Replace(arrtemp[0], @"  ", "\t");
                    d = Regex.Replace(d, @"\s", "\t");
                }
                #endregion
                streamwriter.Close();
            }
            MessageBox.Show("OK");
        }
        //20240723统计减少碳排放的LINK中不同国家的占比
        private void button69_Click(object sender, EventArgs e)
        {
            #region //根据全球港口数据给PortCenter文件中的港口添加CountryCode
            string rePath = @"E:\武理工工作2021-2022\在研论文\全球港口属性列表.csv";
            StreamReader streamreader0 = new StreamReader(rePath);
            //定义字典存储每个港口的CountryCode
            Dictionary<string, string> Dic_Temp = new Dictionary<string, string>();
            string line0 = "";
            //存在不同国家有同名港口的情况
            int i = 0;
            while ((line0 = streamreader0.ReadLine()) != null)
            {
                string[] arrtemp0 = line0.Split(',');
                if (arrtemp0[0] == "World_port_index_number")
                {
                    i++;
                    continue;
                }
                else
                {
                    i++;
                    if (Dic_Temp.ContainsKey(i + " " + arrtemp0[2]) == false)
                    {
                        Dic_Temp.Add(i + " " + arrtemp0[2], arrtemp0[3]);
                    }
                }
            }
            #endregion
            string targetPath = @"E:\武理工工作2021-2022\在研论文\论文撰写_22_6_20\After Optimization 2014-2021_22_12_28\After Optimization 2014-2021_22_12_28\Comparision_Union_Making Up\Comparision_Union_Making Up_AddCounCo.csv";
            string theFile = @"E:\武理工工作2021-2022\在研论文\论文撰写_22_6_20\After Optimization 2014-2021_22_12_28\After Optimization 2014-2021_22_12_28\Comparision_Union_Making Up\Comparision_Union_Making Up.csv";
            //string targetPath= @"E:\武理工工作2021-2022\在研论文\论文撰写_22_6_20\After Optimization 2014-2021_22_12_28\After Optimization 2014-2021_22_12_28\VirtualArrival_Union_Making Up\VirtualArrival_Union_Making Up_删异常_AddCounCo.csv";
            //string theFile = @"E:\武理工工作2021-2022\在研论文\论文撰写_22_6_20\After Optimization 2014-2021_22_12_28\After Optimization 2014-2021_22_12_28\VirtualArrival_Union_Making Up\VirtualArrival_Union_Making Up_删异常.csv";
            StreamWriter sw = new StreamWriter(targetPath);
            StreamReader sr = new StreamReader(theFile);
            string line1 = "";
            while ((line1 = sr.ReadLine()) != null)
            {
                string[] arrtemp0 = line1.Split(',');
                if (arrtemp0[0] == "Year")
                {
                    sw.WriteLine(line1 + ',' + "Code1" + ',' + "Code2");
                }
                else
                {
                    string code1 = "";
                    string code2 = "";
                    foreach (KeyValuePair<string, string> kvp1 in Dic_Temp)
                    {
                        //用于Sailing Time Reduction和Waiting Time Reduction
                        //if (kvp1.Key.Contains(arrtemp0[2]))
                        //{ 
                        //    code1 = kvp1.Value;
                        //}
                        //if (kvp1.Key.Contains(arrtemp0[3]))
                        //{
                        //    code2 = kvp1.Value;
                        //}
                        #region
                        if (arrtemp0[2].Contains('.'))
                        {
                            string a = arrtemp0[2].Substring(arrtemp0[2].IndexOf(".") + 1);
                            if (arrtemp0[2].Contains(kvp1.Key) || kvp1.Key.Contains(a))
                            {
                                code1 = kvp1.Value;
                            }
                        }
                        else
                        {
                            // string[] b = KVP1.Key.Split(' ');
                            //if (KVP1.Key.Contains(arrtemp[1]) || arrtemp[1].Contains(b[1]) || arrtemp[1].Contains(KVP1.Key) )
                            string b = arrtemp0[2].Substring(arrtemp0[2].IndexOf(" ") + 1);
                            if (kvp1.Key.Contains(arrtemp0[2]) || arrtemp0[2].Contains(kvp1.Key))
                            {
                                code1 = kvp1.Value;
                            }
                            else if (arrtemp0[2].Contains(kvp1.Key) || kvp1.Key.Contains(b))
                            {
                                code1 = kvp1.Value;
                            }
                        }
                        if (arrtemp0[3].Contains('.'))
                        {
                            string a = arrtemp0[3].Substring(arrtemp0[3].IndexOf(".") + 1);
                            if (arrtemp0[3].Contains(kvp1.Key) || kvp1.Key.Contains(a))
                            {
                                code1 = kvp1.Value;
                            }
                        }
                        else
                        {
                            // string[] b = KVP1.Key.Split(' ');
                            //if (KVP1.Key.Contains(arrtemp[1]) || arrtemp[1].Contains(b[1]) || arrtemp[1].Contains(KVP1.Key) )
                            string b = arrtemp0[3].Substring(arrtemp0[3].IndexOf(" ") + 1);
                            if (kvp1.Key.Contains(arrtemp0[3]) || arrtemp0[3].Contains(kvp1.Key))
                            {
                                code2 = kvp1.Value;
                            }
                            else if (arrtemp0[3].Contains(kvp1.Key) || kvp1.Key.Contains(b))
                            {
                                code2 = kvp1.Value;
                            }
                        }
                        #endregion
                        //用于Virtual Arrival Policy Implementation
                        //if (kvp1.Key.Contains(arrtemp0[1]))
                        //{
                        //    code1 = kvp1.Value;
                        //}
                        //if (kvp1.Key.Contains(arrtemp0[2]))
                        //{
                        //    code2 = kvp1.Value;
                        //}
                        #region
                        //if (arrtemp0[1].Contains('.'))
                        //{
                        //    string a = arrtemp0[1].Substring(arrtemp0[1].IndexOf(".") + 1);
                        //    if (arrtemp0[1].Contains(kvp1.Key) || kvp1.Key.Contains(a))
                        //    {
                        //        code1 = kvp1.Value;
                        //    }
                        //}
                        //else
                        //{
                        //    // string[] b = KVP1.Key.Split(' ');
                        //    //if (KVP1.Key.Contains(arrtemp[1]) || arrtemp[1].Contains(b[1]) || arrtemp[1].Contains(KVP1.Key) )
                        //    string b = arrtemp0[1].Substring(arrtemp0[1].IndexOf(" ") + 1);
                        //    if (kvp1.Key.Contains(arrtemp0[1]) || arrtemp0[1].Contains(kvp1.Key))
                        //    {
                        //        code1 = kvp1.Value;
                        //    }
                        //    else if (arrtemp0[1].Contains(kvp1.Key) || kvp1.Key.Contains(b))
                        //    {
                        //        code1 = kvp1.Value;
                        //    }
                        //}
                        //if (arrtemp0[2].Contains('.'))
                        //{
                        //    string a = arrtemp0[2].Substring(arrtemp0[2].IndexOf(".") + 1);
                        //    if (arrtemp0[2].Contains(kvp1.Key) || kvp1.Key.Contains(a))
                        //    {
                        //        code1 = kvp1.Value;
                        //    }
                        //}
                        //else
                        //{
                        //    // string[] b = KVP1.Key.Split(' ');
                        //    //if (KVP1.Key.Contains(arrtemp[1]) || arrtemp[1].Contains(b[1]) || arrtemp[1].Contains(KVP1.Key) )
                        //    string b = arrtemp0[2].Substring(arrtemp0[2].IndexOf(" ") + 1);
                        //    if (kvp1.Key.Contains(arrtemp0[2]) || arrtemp0[2].Contains(kvp1.Key))
                        //    {
                        //        code2 = kvp1.Value;
                        //    }
                        //    else if (arrtemp0[2].Contains(kvp1.Key) || kvp1.Key.Contains(b))
                        //    {
                        //        code2 = kvp1.Value;
                        //    }
                        //}
                        #endregion
                    }
                    sw.WriteLine(line1 + ',' + code1 + ',' + code2);
                }
            }
            sw.Close();
            MessageBox.Show("OK");
        }
        //2024_07_23统计碳排放减少的不同国家的Link数量（Sailing Time and Waiting Time）
        private void button70_Click(object sender, EventArgs e)
        {
            string inPath = @"E:\武理工工作2021-2022\在研论文\论文撰写_22_6_20\After Optimization 2014-2021_22_12_28\After Optimization 2014-2021_22_12_28\Comparision_Union_Making Up\Comparision_Union_Making Up_AddCounCo.csv";
            string outPath1 = @"E:\武理工工作2021-2022\在研论文\论文撰写_22_6_20\After Optimization 2014-2021_22_12_28\After Optimization 2014-2021_22_12_28\Comparision_Union_Making Up\Comparision_Union_Making Up_AddCounCo_SailTimeCount.csv";
            string outPath2 = @"E:\武理工工作2021-2022\在研论文\论文撰写_22_6_20\After Optimization 2014-2021_22_12_28\After Optimization 2014-2021_22_12_28\Comparision_Union_Making Up\Comparision_Union_Making Up_AddCounCo_WaitTimeCount.csv";
            StreamReader sr = new StreamReader(inPath);
            StreamWriter sw1 = new StreamWriter(outPath1);
            StreamWriter sw2 = new StreamWriter(outPath2);
            string line1 = "";
            //还需统计年份
            Dictionary<string, Dictionary<string, Dictionary<string, int>>> Sail_Count = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
            Dictionary<string, Dictionary<string, Dictionary<string, int>>> Wait_Count = new Dictionary<string, Dictionary<string, Dictionary<string, int>>>();
            while ((line1 = sr.ReadLine()) != null)
            {
                string[] arrtemp0 = line1.Split(',');
                //排除Value为0的
                if (arrtemp0[0] == "Year" || Convert.ToDouble(arrtemp0[5]) == 0)
                {
                    continue;
                }
                else
                {
                    //int i = 0;
                    //if (arrtemp0[0] == "2021")
                    //    i++;
                    //if (i == 1)
                    //{
                    //    i++;
                    //}
                    if (arrtemp0[1] == "Sailing Time Reduction")
                    {

                        if (Sail_Count.ContainsKey("Sialing Time Reduction"))
                        {
                            if (Sail_Count["Sialing Time Reduction"].ContainsKey(arrtemp0[0]))
                            {
                                if (Sail_Count["Sialing Time Reduction"][arrtemp0[0]].ContainsKey(arrtemp0[6]))
                                {
                                    Sail_Count["Sialing Time Reduction"][arrtemp0[0]][arrtemp0[6]]++;
                                }
                                else
                                {
                                    Sail_Count["Sialing Time Reduction"][arrtemp0[0]].Add(arrtemp0[6], 1);
                                }
                                if (Sail_Count["Sialing Time Reduction"][arrtemp0[0]].ContainsKey(arrtemp0[7]))
                                {
                                    Sail_Count["Sialing Time Reduction"][arrtemp0[0]][arrtemp0[7]]++;
                                }
                                else
                                {
                                    Sail_Count["Sialing Time Reduction"][arrtemp0[0]].Add(arrtemp0[7], 1);
                                }
                            }
                            else
                            {
                                Dictionary<string, int> Dic_1 = new Dictionary<string, int>();
                                Dic_1.Add(arrtemp0[6], 1);
                                Dic_1.Add(arrtemp0[7], 1);
                                Sail_Count["Sialing Time Reduction"].Add(arrtemp0[0], Dic_1);
                            }
                        }

                        else
                        {
                            Dictionary<string, int> Dic_1 = new Dictionary<string, int>();
                            Dic_1.Add(arrtemp0[6], 1);
                            Dic_1.Add(arrtemp0[7], 1);
                            Dictionary<string, Dictionary<string, int>> Dic_2 = new Dictionary<string, Dictionary<string, int>>();
                            Dic_2.Add(arrtemp0[0], Dic_1);
                            Sail_Count.Add("Sialing Time Reduction", Dic_2);
                        }
                    }
                    else
                    {
                        if (Wait_Count.ContainsKey("Waiting Time Reduction"))
                        {
                            if (Wait_Count["Waiting Time Reduction"].ContainsKey(arrtemp0[0]))
                            {
                                if (Wait_Count["Waiting Time Reduction"][arrtemp0[0]].ContainsKey(arrtemp0[6]))
                                {
                                    Wait_Count["Waiting Time Reduction"][arrtemp0[0]][arrtemp0[6]]++;
                                }
                                else
                                {
                                    Wait_Count["Waiting Time Reduction"][arrtemp0[0]].Add(arrtemp0[6], 1);
                                }
                            }
                            else
                            {
                                Dictionary<string, int> Dic_1 = new Dictionary<string, int>();
                                Dic_1.Add(arrtemp0[6], 1);
                                Wait_Count["Waiting Time Reduction"].Add(arrtemp0[0], Dic_1);
                            }
                            if (Wait_Count["Waiting Time Reduction"].ContainsKey(arrtemp0[0]))
                            {
                                if (Wait_Count["Waiting Time Reduction"][arrtemp0[0]].ContainsKey(arrtemp0[7]))
                                {
                                    Wait_Count["Waiting Time Reduction"][arrtemp0[0]][arrtemp0[7]]++;
                                }
                                else
                                {
                                    Wait_Count["Waiting Time Reduction"][arrtemp0[0]].Add(arrtemp0[7], 1);
                                }
                            }
                            else
                            {
                                Dictionary<string, int> Dic_1 = new Dictionary<string, int>();
                                Dic_1.Add(arrtemp0[7], 1);
                                Wait_Count["Waiting Time Reduction"].Add(arrtemp0[0], Dic_1);
                            }

                        }
                        else
                        {
                            Dictionary<string, int> Dic_1 = new Dictionary<string, int>();
                            Dic_1.Add(arrtemp0[6], 1);
                            Dic_1.Add(arrtemp0[7], 1);
                            Dictionary<string, Dictionary<string, int>> Dic_2 = new Dictionary<string, Dictionary<string, int>>();
                            Dic_2.Add(arrtemp0[0], Dic_1);
                            Wait_Count.Add("Waiting Time Reduction", Dic_2);
                        }
                    }
                }
            }
            //写出两个字典中的统计内容
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, int>>> kvp1 in Sail_Count)
            {
                foreach (KeyValuePair<string, Dictionary<string, int>> kvp2 in kvp1.Value)
                {
                    foreach (KeyValuePair<string, int> kvp3 in kvp2.Value)
                    {
                        sw1.WriteLine(kvp1.Key + ',' + kvp2.Key + ',' + kvp3.Key + ',' + kvp3.Value);
                    }
                }
            }
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, int>>> kvp1 in Wait_Count)
            {
                foreach (KeyValuePair<string, Dictionary<string, int>> kvp2 in kvp1.Value)
                {
                    foreach (KeyValuePair<string, int> kvp3 in kvp2.Value)
                    {
                        sw2.WriteLine(kvp1.Key + ',' + kvp2.Key + ',' + kvp3.Key + ',' + kvp3.Value);
                    }
                }
            }
            sw1.Close();
            sw2.Close();
            MessageBox.Show("OK");
        }
        //2024_07_23统计碳排放减少的不同国家的Link数量（Virtual Arrival Policy Implementation）
        private void button71_Click(object sender, EventArgs e)
        {
            string inPath = @"E:\武理工工作2021-2022\在研论文\论文撰写_22_6_20\After Optimization 2014-2021_22_12_28\After Optimization 2014-2021_22_12_28\VirtualArrival_Union_Making Up\VirtualArrival_Union_Making Up_删异常_AddCounCo.csv";
            string outPath1 = @"E:\武理工工作2021-2022\在研论文\论文撰写_22_6_20\After Optimization 2014-2021_22_12_28\After Optimization 2014-2021_22_12_28\VirtualArrival_Union_Making Up\VirtualArrival_Union_Making Up_删异常_AddCounCo_Count.csv";
            StreamReader sr = new StreamReader(inPath);
            StreamWriter sw1 = new StreamWriter(outPath1);
            string line1 = "";
            Dictionary<string, Dictionary<string, int>> Count = new Dictionary<string, Dictionary<string, int>>();
            while ((line1 = sr.ReadLine()) != null)
            {
                string[] arrtemp0 = line1.Split(',');
                //排除Value为0的
                if (arrtemp0[0] == "Year" || Convert.ToDouble(arrtemp0[4]) == 0)
                {
                    continue;
                }
                else
                {
                    if (Count.ContainsKey(arrtemp0[0]))
                    {
                        if (Count[arrtemp0[0]].ContainsKey(arrtemp0[5]))
                        {
                            Count[arrtemp0[0]][arrtemp0[5]]++;
                        }
                        else
                        {
                            Count[arrtemp0[0]].Add(arrtemp0[5], 1);
                        }
                        if (Count[arrtemp0[0]].ContainsKey(arrtemp0[6]))
                        {
                            Count[arrtemp0[0]][arrtemp0[6]]++;
                        }
                        else
                        {
                            Count[arrtemp0[0]].Add(arrtemp0[6], 1);
                        }
                    }
                    else
                    {
                        Dictionary<string, int> Dic_1 = new Dictionary<string, int>();
                        Dic_1.Add(arrtemp0[5], 1);
                        Dic_1.Add(arrtemp0[6], 1);
                        Count.Add(arrtemp0[0], Dic_1);
                    }
                }
            }
            foreach (KeyValuePair<string, Dictionary<string, int>> kvp2 in Count)
            {
                foreach (KeyValuePair<string, int> kvp3 in kvp2.Value)
                {
                    sw1.WriteLine(kvp2.Key + ',' + kvp3.Key + ',' + kvp3.Value);
                }
            }
            sw1.Close();
            MessageBox.Show("OK");
        }
    }

}



