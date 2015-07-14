using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Date.Data
{
    public class DateDetail
    {
        private string nickname;

        public string Nickname
        {
            get { return nickname; }
            set { nickname = value; }
        }
        private string head;

        public string Head
        {
            get { return head; }
            set { head = value; }
        }
        private string gender;

        public string Gender
        {
            get { return gender; }
            set { gender = value; }
        }
        private int date_id;

        public int Date_id
        {
            get { return date_id; }
            set { date_id = value; }
        }
        private int user_id;

        public int User_id
        {
            get { return user_id; }
            set { user_id = value; }
        }
        private string created_at;

        public string Created_at
        {
            get { return created_at; }
            set { created_at = value; }
        }
        private string date_time;

        public string Date_time
        {
            get { return date_time; }
            set { date_time = value; }
        }
        private string place;

        public string Place
        {
            get { return place; }
            set { place = value; }
        }
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; }
        }
        private int date_type;

        public int Date_type
        {
            get { return date_type; }
            set { date_type = value; }
        }
        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        private int category_id;

        public int Category_id
        {
            get { return category_id; }
            set { category_id = value; }
        }
        private int people_limit;

        public int People_limit
        {
            get { return people_limit; }
            set { people_limit = value; }
        }
        private int gender_limit;

        public int Gender_limit
        {
            get { return gender_limit; }
            set { gender_limit = value; }
        }
        private int cost_model;

        public int Cost_model
        {
            get { return cost_model; }
            set { cost_model = value; }
        }
        private string signature;

        public string Signature
        {
            get { return signature; }
            set { signature = value; }
        }
        private int date_status;

        public int Date_status
        {
            get { return date_status; }
            set { date_status = value; }
        }
        private int[] grade_limit;

        public int[] Grade_limit
        {
            get { return grade_limit; }
            set { grade_limit = value; }
        }
        private string[] grade;

        public string[] Grade
        {
            get { return grade; }
            set { grade = value; }
        }
        private int collection_status;

        public int Collection_status
        {
            get { return collection_status; }
            set { collection_status = value; }
        }
        private int apply_status;

        public int Apply_status
        {
            get { return apply_status; }
            set { apply_status = value; }
        }
        private int user_score;

        public int User_score
        {
            get { return user_score; }
            set { user_score = value; }
        }
        private JoinedOnes[] joined;

        public JoinedOnes[] Joined
        {
            get { return joined; }
            set { joined = value; }
        }

        public void GetAttribute(JObject datedetailJObject)
        {
            var gradelimit = JArray.Parse(datedetailJObject["data"]["grade_limit"].ToString());
            int[] temp = new int[gradelimit.Count];
            for (int i = 0; i < gradelimit.Count; ++i)
            {
                temp[i] = Int32.Parse(gradelimit[i].ToString());
            }
            var grade = JArray.Parse(datedetailJObject["data"]["grade"].ToString());
            string[] temp2 = new string[grade.Count];
            for (int i = 0; i < grade.Count; ++i)
            {
                temp2[i] = grade[i].ToString();
            }
            Grade = temp2;
            Nickname = datedetailJObject["data"]["nickname"].ToString();
            Head = datedetailJObject["data"]["head"].ToString();
            Gender = datedetailJObject["data"]["gender"].ToString();
            Date_id = Int32.Parse(datedetailJObject["data"]["date_id"].ToString());
            User_id = Int32.Parse(datedetailJObject["data"]["user_id"].ToString());
            Created_at = datedetailJObject["data"]["created_at"].ToString();
            Date_time = datedetailJObject["data"]["date_time"].ToString();
            Place = datedetailJObject["data"]["place"].ToString();
            Title = datedetailJObject["data"]["title"].ToString();
            Content = datedetailJObject["data"]["content"].ToString();
            Date_type = Int32.Parse(datedetailJObject["data"]["date_type"].ToString());
            Type = datedetailJObject["data"]["type"].ToString();
            Category_id = Int32.Parse(datedetailJObject["data"]["category_id"].ToString());
            People_limit = Int32.Parse(datedetailJObject["data"]["people_limit"].ToString());
            Gender_limit = Int32.Parse(datedetailJObject["data"]["gender_limit"].ToString());
            Cost_model = Int32.Parse(datedetailJObject["data"]["cost_model"].ToString());
            Signature = datedetailJObject["data"]["signature"].ToString();
            Collection_status = Int32.Parse(datedetailJObject["data"]["collection_status"].ToString());
            Apply_status = Int32.Parse(datedetailJObject["data"]["apply_status"].ToString());
            User_score = Int32.Parse(datedetailJObject["data"]["user_score"].ToString());
            var joined = JArray.Parse(datedetailJObject["data"]["joined"].ToString());
            JoinedOnes[] joineds = new JoinedOnes[joined.Count];
            try
            {

                for (int i = 0; i < joined.Count; i++)
                {
                    JObject oneperson = JObject.Parse(joined[i].ToString());
                    joineds[i]=new JoinedOnes();
                    joineds[i].Nickname = oneperson["nickname"].ToString();
                    joineds[i].User_id = Int32.Parse(oneperson["user_id"].ToString());
                    joineds[i].Date_id = Int32.Parse(oneperson["date_id"].ToString());
                    joineds[i].Gender = Int32.Parse(oneperson["gender"].ToString());
                    joineds[i].Signature = oneperson["user_id"].ToString();
                    joineds[i].Head = oneperson["user_id"].ToString();
                }
                Joined = joineds;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message + "读取参加成员异常");
            }
        }
    }
}
