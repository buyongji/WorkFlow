using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS
{
    [Serializable]
   public class OrganizationInfo
   {
       #region Properties
        public string AcctOrgType
        {
            get;
            set;
        }
        public List<long> FunctionIds
        {
            get;
            set;
        }
        public long ID
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
       #endregion

       #region Methods
       
        /// <summary>
        /// 克隆一份  
        /// </summary>
        /// <returns></returns>
        public OrganizationInfo Clone()
        {
            return new OrganizationInfo
            {
                ID=this.ID,
                Name= this.Name,
                AcctOrgType = this.AcctOrgType
            };
        }

       #endregion
   }
}
