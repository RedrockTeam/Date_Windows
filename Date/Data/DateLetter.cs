using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Date.Data
{
    public class DateLetter
    {



        private int letter_id;


        public int Letter_id
        {
            get { return letter_id; }
            set { letter_id = value; }
        }
        private int user_id;

        public int User_id
        {
            get { return user_id; }
            set { user_id = value; }
        }
        private string user_name;

        public string User_name
        {
            get { return user_name; }
            set { user_name = value; }
        }
        private string user_signature;

        public string User_signature
        {
            get { return user_signature; }
            set { user_signature = value; }
        }
        private string user_avatar;

        public string User_avatar
        {
            get { return user_avatar; }
            set { user_avatar = value; }
        }
        private int user_gender;

        public int User_gender
        {
            get { return user_gender; }
            set { user_gender = value; }
        }
        private string content;

        public string Content
        {
            get { return content; }
            set { content = value; }
        }
        private int date_id;

        public int Date_id
        {
            get { return date_id; }
            set { date_id = value; }
        }
        private int letter_status;

        public int Letter_status
        {
            get { return letter_status; }
            set { letter_status = value; }
        }
        //用户和约会的状态，1接受，0拒绝 ， 2未处理
        private int user_date_status;

        public int User_date_status
        {
            get { return user_date_status; }
            set { user_date_status = value; }
        }
        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        private int user_score;

        public int User_score
        {
            get { return user_score; }
            set { user_score = value; }
        }

        public void GetAttribute(JObject jObject)
        {
            Letter_id = Int32.Parse(jObject["letter_id"].ToString());
            User_id = Int32.Parse(jObject["user_id"].ToString());
            User_name = jObject["user_name"].ToString();
            User_signature = jObject["user_signature"].ToString();
            User_avatar = jObject["user_avatar"].ToString();
            User_gender = Int32.Parse(jObject["user_gender"].ToString());
            Content = jObject["content"].ToString();
            Date_id = Int32.Parse(jObject["date_id"].ToString());
            Letter_status = Int32.Parse(jObject["letter_status"].ToString());
            Title = jObject["title"].ToString();
            //User_score = Int32.Parse(jObject["user_score"].ToString());

        }
    }
}
