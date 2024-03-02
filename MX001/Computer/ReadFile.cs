using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerryTestFramework.testitem.Computer
{
    /// <summary>
    /// 读取文件
    /// </summary>
    public class ReadFile
    {
        /// <summary>
        /// 异步读取Text文件内容并存入字符串字典
        /// </summary>
        /// <param name="path">文件路径</param>
        ///  <param name="Separator">文件中key和value的分隔符</param>
        /// <returns>读取到的字符串集合，读取失败则为null</returns>
        public Task<Dictionary<string, string>> GetDicDataAsync(string path, char Separator)
        {
            return Task.Run(() =>
            {
                var bdlist = new Dictionary<string, string>();
                if (File.Exists(path))// 是文件
                {
                    using (var sr = new StreamReader(path, Encoding.UTF8))
                    {
                        while (true)
                        {
                            var st = sr.ReadLine();
                            if (st == null || st == "") break;
                            var starr = st.Split(Separator);
                            var flag = true;
                            var keys = "";
                            var values = "";
                            foreach (var item in starr)
                            {
                                if (flag)
                                {
                                    keys = item;
                                    flag = !flag;
                                }
                                else
                                {
                                    values += item + Separator;
                                }
                            }
                            bdlist.Add(keys, values.Remove(values.Length - 1, 1));
                        }
                    }
                    return bdlist;
                }
                else
                {
                    return null;
                }
            });
        }
    }
}
