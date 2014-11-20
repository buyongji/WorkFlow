using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS
{
    [Serializable]
   public class LanguageInfo
   {
       #region Fields

        private string _alias;
        [NonSerialized]
        public const int EnglishLocaleId = 0x409;
        public const int InvariantLocaleId = 0x7f;
        [NonSerialized]
        public const int SimplifiedLocaleId = 0x804;
        [NonSerialized]
        public const int TraditionalLocaleId = 0xc04;

       #endregion

       #region Properties



       #endregion
   }
}
