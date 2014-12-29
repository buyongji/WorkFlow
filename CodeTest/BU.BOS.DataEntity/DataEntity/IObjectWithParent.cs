using System;
namespace BU.BOS.Orm.DataEntity
{
   public  interface IObjectWithParent
    {
       object Parent { get; set; }
    }
}
