using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Web;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace Question_Getter_Front_End
{
    class WikiInterface
    {
        public const string wUrl = "http://en.wikipedia.org/w/api.php?";
        public const string randUrl = wUrl + "action=query&list=random&format=xml";
        public const string blUrl = wUrl + "action=query&list=backlinks&bllimit=500&format=xml&bltitle=";
        private const string userAgent = "QuestionGetter/1.0";
        WordChecker wc;
        public WikiInterface()
        {
            wc = new WordChecker();
        }

        public XElement getXml(string url, bool parse)
        {
            XElement root;
            int id;
             if (url.Equals(randUrl))
            {
                string title;
                do
                {
                    root = request(url);
                    if (root == null)
                        return null;
                    root = root.Element("query");
                    root = root.Element("random");
                    root = root.Element("page");
                    title = (string)root.Attribute("title");
                } while (title.Contains(":"));
                id = int.Parse((string)root.Attribute("id"));
            }
            else
            {
                root = request(url);
                id = int.Parse((string)root.Element("query").Element("pages").Element("page").Attribute("pageid"));
            }
            root = request(parseUrl(id));
            if (parse)
            {
                root = xmlTrim(root);
            }
            return root;
        }

        public string getSite(string url, bool parse)
        {
            XElement root;
            string ret;
            string artTitle;
            int id;
            if (url.Equals(randUrl))
            {
                string title;
                do
                {
                    root = request(url);
                    if (root == null)
                        return null;
                    root = root.Element("query");
                    root = root.Element("random");
                    root = root.Element("page");
                    title = (string)root.Attribute("title");
                } while (title.Contains(":")||title.Contains("List of")||title.Contains("disambiguation"));
                id = int.Parse((string)root.Attribute("id"));
            }
            else
            {
                root = request(url);
                id = int.Parse((string)root.Element("query").Element("pages").Element("page").Attribute("pageid"));
            }
            root = request(parseUrl(id));
            ret = (string)root.Element("parse").Element("text");
            artTitle = (string)root.Element("parse").Attribute("displaytitle");
            if (parse)
            {
                ret = parseText(ret);
                ret = wc.classifyWords(ret);
            }
            ret = ret.Trim();
            while (ret.Substring(0, 1).Equals("("))
            {
                ret=remElement(ret, 0, ")");
                ret = ret.Trim();
            }
            ret = "[" + artTitle + "] " + ret;
            return ret;
        }

        public XElement request(string url)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            XElement root;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = userAgent;
                response = (HttpWebResponse)request.GetResponse();
                root = XElement.Load(XmlReader.Create(response.GetResponseStream()));
                response.Close();
                return root;
            }
            catch (WebException except)
            {
                MessageBox.Show("Error:" + except.Message + "\nThe url used was " + url);
                return null;
            }
            catch (XmlException except)
            {
                MessageBox.Show("Error:" + except.Message + "\nThe url used was " + url);
                return null;
            }
        }

        public string queryUrl(string query)
        {
            return wUrl + "action=query&titles=" + query + "&format=xml";
        }

        private string parseUrl(int id)
        {
            return wUrl + "action=parse&pageid=" + id + "&format=xml";
        }

        private string parseText(string s)
        {
            int x = 0;
            while(x!=s.Length)
            {
                if (s.Substring(x, 1).Equals("<"))
                    s = remElement(s, x, ">");
                else if (s.Substring(x, 1).Equals("["))
                    s = remElement(s, x, "]");
                x++;
            }
            if (s.IndexOf("See also") >= 0)
                s = s.Substring(0, s.IndexOf("See also"));
            else if (s.IndexOf("Notes") >= 0)
                s = s.Substring(0, s.IndexOf("Notes"));
            else if (s.IndexOf("References") >= 0)
                s = s.Substring(0, s.IndexOf("References"));
            else if (s.IndexOf("Bibliography") >= 0)
                s = s.Substring(0, s.IndexOf("Bibliography"));
            else if (s.IndexOf("External Links") >= 0)
                s = s.Substring(0, s.IndexOf("External Links"));
            return s;
        }
        private string tableElim(string str, int tableBegin)
        {
            int x = tableBegin;
            int y = x + 1;
            int z;
            while (true)
            {
                while (!str.Substring(y, 1).Equals("<"))
                    y++;
                z = y;
                while (!str.Substring(z, 1).Equals(">"))
                    z++;
                if (str.Substring(y, (z - y) + 1).Equals("</table>"))
                {
                    return str.Substring(0, x) + " " + str.Substring(z + 1);
                }
                y++;
            }
        }

        private string remElement(string str, int x,string check)
        {
            int y;
            int z;
                y = x;
                while (!str.Substring(y, 1).Equals(check))
                {
                    y++;
                }
                if (str.Substring(x, y - x).Contains("href") && check.Equals(">") &&!str.Substring(x, y - x).Contains("class=\"reference\""))
                    return str.Substring(0, x) + " (href) " + str.Substring(y + 1);
                else if ((str.Substring(x, y - x).Contains("dablink") || str.Substring(x, y - x).Contains("mbox-text")) && check.Equals(">"))
                {
                    while (true)
                    {
                        while (!str.Substring(y, 1).Equals("<"))
                            y++;
                        z = y;
                        while (!str.Substring(z, 1).Equals(">"))
                            z++;
                        if (str.Substring(y, z - y).Contains("div") || str.Substring(y, z - y).Contains("td"))
                            return str.Substring(0, x) + " " + str.Substring(z + 1);
                        y++;
                    }
                }
                else if(str.Substring(x, y - x).Contains("<table "))
                    return tableElim(str,x);
                else
                    return str.Substring(0, x) + " " + str.Substring(y + 1);
            }

        private XElement xmlTrim(XElement root)
        {
            IEnumerable<XElement> f;
            removeXElement(root.Element("langlinks"));
            f = from a in root.Element("links").Elements("pl")
                where ((string)a).Contains(":")
                select a;
            foreach (XElement x in f)
            {
                x.Remove();
            }
            removeXElement(root.Element("templates"));
            removeXElement(root.Element("images"));
            removeXElement(root.Element("externallinks"));
            return root;
        }

        private void removeXElement(XElement root)
        {
            root.RemoveAll();
            root.Remove();
        }

        public string[][] getBackLinks(string title)
        {
            XElement root;
            bool cont;
            root = request(blUrl + title);
            string ttl;
            int n = getNumBackLinks(title);
            double countx = 0;
            int county=0;
            string[][] ret = new string[(n / 1000)+1][];
            int numLeftOut = 0;
            for (int x = 0; x < (n / 1000) + 1; x++)
                ret[x] = new string[1000];
            do
            {
                cont=false;
                IEnumerable<XElement> nodes = root.Element("query").Element("backlinks").Descendants("bl");
                foreach (XElement x in nodes)
                {
                    ttl = (string)x.Attribute("title");
                    if (!ttl.Contains(":") && !ttl.Contains("List of") && !ttl.Contains("disambiguation"))
                    {
                        ret[(int)countx][county] = ttl;
                        county++;
                    }
                    else
                    {
                        numLeftOut++;
                        continue;
                    }
                }
                if(root.Element("query-continue") != null)
                {
                    double test;
                    cont=true;
                    root = request(blUrl + title +"&blcontinue=" + (string)root.Element("query-continue").Element("backlinks").Attribute("blcontinue"));
                    countx += 0.5;
                    test = ((int)countx)/countx;
                    if(test == 1)
                        county = 0;
                }
            }while(cont);
            Console.WriteLine(n);
            Console.WriteLine(numLeftOut);
            return ret;
        }

        public int getNumBackLinks(string title)
        {
            XElement root = request(blUrl+title);
            IEnumerable<XElement> nodes;
            bool cont;
            int count = 0;
            do
            {
                cont = false;
                nodes = root.Element("query").Element("backlinks").Descendants("bl");
                foreach (XElement x in nodes)
                {
                    count++;
                }
                if (root.Element("query-continue") != null)
                {
                    cont = true;
                    root = request(blUrl + title + "&blcontinue=" + (string)root.Element("query-continue").Element("backlinks").Attribute("blcontinue"));
                }
            } while (cont);
            return count;
        }

        public string sortTopic(string title)
        {
            XElement categories = getCateogories(title);
            string check;
            foreach (XElement x in categories.Descendants())
            {
                check = (string)x.Attribute("title");
                if (check.Contains("births") || check.Contains("deaths"))
                {
                    return "person";
                }
            }
            foreach (XElement x in categories.Descendants())
            {
                check = (string) x.Attribute("title");
                if (check.Contains("states") || check.Contains("cities") || check.Contains("towns") || check.Contains("Cities") || check.Contains("Towns"))
                {
                    return "place";
                }
            }
            return "neither";
        }

        private XElement getCateogories(string title)
        {
            return request(wUrl + "action=query&prop=categories&cllimit=500&format=xml&titles=" + title).Element("query").Element("pages").Element("page").Element("categories");
        }
    }
}
