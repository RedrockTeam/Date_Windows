using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Date.Data
{
    public class MyDate
    {
        public MyDate()
        {
            this.nickname = "";
            this.head = "";
            this.gender = "";
            this.date_id = "";
            this.title = "";
            this.place = "";
            this.date_time = "";
            this.created_at = "";
            this.cost_model = "";
            this.Date_status = "";
        }
        public MyDate(string nickname, string head, string gender, string date_id, string title, string place,
            string date_time, string created_at, string cost_model, string date_status)
        {
            this.nickname = nickname;
            this.head = head;
            this.gender = gender;
            this.date_id = date_id;
            this.title = title;
            this.place = place;
            this.date_time = date_time;
            this.created_at = created_at;
            this.cost_model = cost_model;
            this.Date_status = date_status;
        }

        public void GetAttribute(JObject temp)
        {
            Nickname = temp["nickname"].ToString();
            Head = temp["head"].ToString();
            Gender = temp["gender"].ToString();
            Date_id = temp["date_id"].ToString();
            Title = temp["title"].ToString();
            Place= temp["place"].ToString();
            Date_time=Util.Utils.GetTime(temp["date_time"].ToString()).ToString();
            Created_at = temp["created_at"].ToString();
            Cost_model = temp["cost_model"].ToString();
            Date_status = temp["date_status"].ToString();
        }
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

        private string date_id;

        public string Date_id
        {
            get { return date_id; }
            set { date_id = value; }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private string place;

        public string Place
        {
            get { return place; }
            set { place = value; }
        }

        private string date_time;

        public string Date_time
        {
            get { return date_time; }
            set { date_time = value; }
        }

        private string created_at;

        public string Created_at
        {
            get { return created_at; }
            set { created_at = value; }
        }

        private string cost_model;

        public string Cost_model
        {
            get { return cost_model; }
            set { cost_model = value; }
        }

        private string date_status;

        public string Date_status
        {
            get { return date_status; }
            set { date_status = value; }
        }
    }
}
