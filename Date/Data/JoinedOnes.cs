using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Date.Data
{
    public class JoinedOnes
    {
        private int user_id;

        public int User_id
        {
            get { return user_id; }
            set { user_id = value; }
        }
        private int date_id;

        public int Date_id
        {
            get { return date_id; }
            set { date_id = value; }
        }
        private string nickname;

        public string Nickname
        {
            get { return nickname; }
            set { nickname = value; }
        }
        private string signature;

        public string Signature
        {
            get { return signature; }
            set { signature = value; }
        }
        private string head;

        public string Head
        {
            get { return head; }
            set { head = value; }
        }

        private int gender;

        public int Gender
        {
            get { return gender; }
            set { gender = value; }
        }
    }
}
