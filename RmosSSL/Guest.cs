using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RmosSSL
{
    internal class Guest
    {
        protected bool IS_TC = true;

        protected bool IS_FOREIGN = false;

        public string KEY = null;

        public string ID = null;

        public string NAME = null;

        public string SURNAME = null;

        public string FATHER = null;

        public string MOTHER = null;

        public string BIRTH = null;

        public string CITY = null;

        public string COUNTRY = null;

        public string CHECKIN = null;

        public string PLATE = null;

        public int ROOM = 0;

        public int NATION = 1;

        public int GENDER = 0;

        public bool DETAIL = false;

        public void setTC()
        {
            this.IS_TC = true;
            this.IS_FOREIGN = false;
        }

        public void setForeign()
        {
            this.IS_TC = false;
            this.IS_FOREIGN = true;
        }

        public bool isTC()
        {
            return this.IS_TC;
        }

        public bool isForeign()
        {
            return this.IS_FOREIGN;
        }
    }
}
