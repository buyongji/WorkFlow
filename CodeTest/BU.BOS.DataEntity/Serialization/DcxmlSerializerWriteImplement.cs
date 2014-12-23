using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BU.BOS.Serialization
{
    class DcxmlSerializerWriteImplement
    {
        private DcxmlBinder dcxmlBinder;
        private System.Xml.XmlWriter writer;
        private bool p1;
        private bool p2;

        public DcxmlSerializerWriteImplement(DcxmlBinder dcxmlBinder, System.Xml.XmlWriter writer, bool p1, bool p2)
        {
            // TODO: Complete member initialization
            this.dcxmlBinder = dcxmlBinder;
            this.writer = writer;
            this.p1 = p1;
            this.p2 = p2;
        }
    }
}
