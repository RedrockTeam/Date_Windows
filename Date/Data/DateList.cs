using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Date.Data
{
    public class DateList
    {
        public DateList() { }
        public DateList(int date_id,string head ,string nickname,string gender,string signature,string title,string place ,string date_time,string cost_model )
        {
            this.date_id = date_id;
            this.head = head;
            this.nickname = nickname;
            this.gender = gender;
            this.signature = signature;
            this.title = title;
            this.place = place;
            this.date_time = date_time;
            this.cost_model = cost_model;
        }

        public int date_id;
        public int user_id;
        public string head;
        public string created_at;
        public string date_time;
        public string place;
        public string title;
        public string date_type;
        public string cost_model;
        public string nickname;
        public string gender;
        public int category_id;
        public string signature;
        public string timescore;
        public int userscore;
        public string datepercent;
        public int total;

        public int Date_id { get; set; }
        public int User_id { get; set; }
        public string Head { get; set; }
        public string Created_at { get; set; }
        public string Date_time { get; set; }
        public string Place { get; set; }
        public string Title { get; set; }
        public string Date_type { get; set; }
        public string Cost_model { get; set; }
        public string Nickname { get; set; }
        public string Gender { get; set; }
        public int Category_id { get; set; }
        public string Signature { get; set; }
        public string Timescore { get; set; }
        public int Userscore { get; set; }
        public string Datepercent { get; set; }
        public int Total { get; set; }



    }
}
