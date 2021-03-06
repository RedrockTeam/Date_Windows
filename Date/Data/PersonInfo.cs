﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Windows.Storage;
using Date.Util;

namespace Date.Data
{

    public class PersonInfo
    {
        private ApplicationDataContainer appSetting;
        List<GradeList> gradelist = new List<GradeList>();
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private string head;

        public string Head
        {
            get { return head; }
            set { head = value; }
        }
        private string signature;

        public string Signature
        {
            get { return signature; }
            set { signature = value; }
        }
        private string nickname;

        public string Nickname
        {
            get { return nickname; }
            set { nickname = value; }
        }
        private string gender;

        public string Gender
        {
            get { return gender; }
            set { gender = value; }
        }
        private string grade_id;

        public string Grade_id
        {
            get { return grade_id; }
            set { grade_id = value; }
        }
        private string grade;

        public string Grade
        {
            get { return grade; }
            set { grade = value; }
        }
        private string academy_id;

        public string Academy_id
        {
            get { return academy_id; }
            set { academy_id = value; }
        }
        private string academy;

        public string Academy
        {
            get { return academy; }
            set { academy = value; }
        }
        private string qq;

        public string Qq
        {
            get { return qq; }
            set { qq = value; }
        }
        private string weixin;

        public string Weixin
        {
            get { return weixin; }
            set { weixin = value; }
        }
        private string telephone;

        public string Telephone
        {
            get { return telephone; }
            set { telephone = value; }
        }

        public PersonInfo(int id, string nickname, string head, string signature, string gender, string grade, string grade_id, string academy, string academy_id, string telephone, string qq, string weixin)
        {
            this.id = id;
            this.nickname = nickname;
            this.head = head;
            this.signature = signature;
            this.grade_id = grade_id;
            this.gender = gender;
            this.grade = grade;
            this.academy_id = academy_id;
            this.academy = academy;
            this.telephone = telephone;
            this.qq = qq;
            this.weixin = weixin;
            appSetting = ApplicationData.Current.LocalSettings; //本地存储
            getGradeInfor();

        }

        public PersonInfo()
        {
            this.id = 0;
            this.nickname = "";
            this.head = "";
            this.signature = "";
            this.grade_id = "";
            this.gender = "";
            this.grade = "";
            this.academy_id = "";
            this.academy = "";
            this.telephone = "";
            this.qq = "";
            this.weixin = "";
            appSetting = ApplicationData.Current.LocalSettings; //本地存储
            getGradeInfor();

        }

        public void GetAttribute(JObject obj)
        {
            Id = Int32.Parse(obj["data"]["id"].ToString());
            Head = obj["data"]["head"].ToString();
            Signature = obj["data"]["signature"].ToString();
            Nickname = obj["data"]["nickname"] != null ? obj["data"]["nickname"].ToString() : "";
            Gender = obj["data"]["gender"].ToString();
            Grade_id = obj["data"]["grade_id"].ToString();
            Grade = obj["data"]["grade"].ToString();
            Academy_id = obj["data"]["academy_id"] != null ? obj["data"]["academy_id"].ToString() : "";
            Academy = obj["data"]["academy"] != null ? obj["data"]["academy"].ToString() : "";
            Qq = obj["data"]["qq"] != null ? obj["data"]["qq"].ToString() : "";
            Weixin = obj["data"]["weixin"] != null ? obj["data"]["weixin"].ToString() : "";
            Telephone = obj["data"]["telephone"] != null ? obj["data"]["telephone"].ToString() : "";
        }

        public void GetOtherAttribute(JObject obj)
        {
            Id = Int32.Parse(obj["data"]["id"].ToString());
            Head = obj["data"]["head"].ToString();
            Signature = obj["data"]["signature"].ToString();
            Nickname = obj["data"]["nickname"] != null ? obj["data"]["nickname"].ToString() : "";
            GradeList g = gradelist.Find(p => p.Id.Equals((Int32)obj["data"]["grade"]));
            Grade = g.Name;
            Academy_id = obj["data"]["academy_id"] != null ? obj["data"]["academy_id"].ToString() : "";
            Academy = obj["data"]["academy"] != null ? obj["data"]["academy"].ToString() : "";
            Qq = obj["data"]["qq"] != null ? obj["data"]["qq"].ToString() : "";
            Weixin = obj["data"]["weixin"] != null ? obj["data"]["weixin"].ToString() : "";
            Telephone = obj["data"]["telephone"] != null ? obj["data"]["telephone"].ToString() : "";
        }

        private async void getGradeInfor()
        {
            //年级
            string grade = "";
            if (appSetting.Values.ContainsKey("grade_json"))
                grade = appSetting.Values["grade_json"].ToString();
            if (grade != "")
            {
                JArray gradeArray = Utils.ReadJso(grade);
                for (int i = 0; i < gradeArray.Count; i++)
                {
                    JObject jobj = (JObject)gradeArray[i];
                    var b = new GradeList
                    {
                        Id = Convert.ToInt32(jobj["id"].ToString()),
                        Name = jobj["name"].ToString()
                    };
                    gradelist.Add(b);
                }
            }
            else
            {
                grade = Utils.ConvertUnicodeStringToChinese(await NetWork.getHttpWebRequest("/public/grade", new List<KeyValuePair<String, String>>()));
                getGradeInfor();
            }
        }
    }
}
