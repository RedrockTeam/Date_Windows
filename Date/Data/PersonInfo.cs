using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Date.Data
{
    public class PersonInfo
    {
        private int id;
        private string nickname;
        private string head;
        private string gender;
        private string grade;
        private string academy;
        private string telephone;
        private string qq;
        private string weixin;

        public PersonInfo(int id, string nickname, string head, string gender, string grade, string academy,string telephone,string qq,string weixin)
        {
            this.id = id;
            this.nickname = nickname;
            this.head = head;
            this.gender = gender;
            this.grade = grade;
            this.academy = academy;
            this.telephone = telephone;
            this.qq = qq;
            this.weixin = weixin;
        }
    }
}
