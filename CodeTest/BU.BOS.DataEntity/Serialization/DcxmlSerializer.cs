using BU.BOS.Orm.Exceptions;
using BU.BOS.Orm.Metadata.DataEntity;
using BU.BOS.Resource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BU.BOS.Serialization
{
    public class DcxmlSerializer
    {
        // Fields
        private DcxmlBinder _binder;
        private bool _colloctionIgnorePKValue;
        private bool _needDBIgnore;
        private bool _serializeComplexProperty;

        // Methods
        public DcxmlSerializer(DcxmlBinder binder)
        {
            if (binder == null)
            {
                throw new ORMArgInvalidException("??????", ResManager.LoadKDString("设置DcxmlSerializer的binder失败，binder不能为空！", "014009000001765", SubSystemType.SL, new object[0]));
            }
            this._binder = binder;
            this._serializeComplexProperty = true;
        }

        public DcxmlSerializer(IEnumerable<IDataEntityType> dts)
        {
            this._binder = new ListDcxmlBinder(dts);
            this._serializeComplexProperty = true;
        }

        public object Deserialize(Stream stream, object entity = null)
        {
            if (stream == null)
            {
                throw new ORMArgInvalidException("??????", ResManager.LoadKDString("对指定的stream流执行反序列化操作失败,数据流stream不能为空！", "014009000001766", SubSystemType.SL, new object[0]));
            }
            using (XmlTextReader reader = new XmlTextReader(stream))
            {
                return this.ReadElement(reader, null, entity);
            }
        }

        public object Deserialize(string url, object entity = null)
        {
            using (XmlTextReader reader = new XmlTextReader(url))
            {
                return this.ReadElement(reader, null, entity);
            }
        }

        public object Deserialize(XmlTextReader xmlReader, object entity = null)
        {
            if (xmlReader == null)
            {
                throw new ORMArgInvalidException("??????", ResManager.LoadKDString("对指定的xml流执行反序列化操作失败,数据流xmlReader不能为空！", "014009000001767", SubSystemType.SL, new object[0]));
            }
            using (xmlReader)
            {
                return this.ReadElement(xmlReader, null, entity);
            }
        }

        public object DeserializeFromString(string xml, object entity = null)
        {
            object obj2;
            using (StringReader reader = new StringReader(xml))
            {
                using (XmlTextReader reader2 = new XmlTextReader(reader))
                {
                    obj2 = this.ReadElement(reader2, null, entity);
                }
            }
            return obj2;
        }

        private object ReadElement(XmlReader reader, IDataEntityType dt, object entity)
        {
            DcxmlSerializerReadImplement implement = new DcxmlSerializerReadImplement(this._binder, reader, this._colloctionIgnorePKValue)
            {
                OnlyLocaleValue = this.OnlyLocaleVale,
                ResetLoacaleValueBy2052 = this.ResetLoacaleValueBy2052
            };
            object obj2 = implement.ReadElement(dt, entity);
            implement.EndInitialize();
            return obj2;
        }

        public void Serialize(Stream stream, object currentEntity, object baseEntity)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
            {
                this.Serialize(writer, currentEntity, baseEntity);
            }
        }

        public void Serialize(string fileName, object currentEntity, object baseEntity)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
            using (XmlTextWriter writer = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                this.Serialize(writer, currentEntity, baseEntity);
            }
        }

        public void Serialize(XmlWriter writer, object currentEntity, object baseEntity)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (currentEntity == null)
            {
                throw new ArgumentNullException("currentEntity");
            }
            new DcxmlSerializerWriteImplement(this._binder, writer, this._serializeComplexProperty, this._needDBIgnore) { OnlyLocaleValue = this.OnlyLocaleVale }.Serialize(null, currentEntity, baseEntity);
            writer.Flush();
        }

        public string SerializeToString(object currentEntity, object baseEntity)
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {
                using (XmlTextWriter writer2 = new XmlTextWriter(writer))
                {
                    this.Serialize(writer2, currentEntity, baseEntity);
                }
            }
            return sb.ToString();
        }

        // Properties
        public DcxmlBinder Binder
        {
            get
            {
                return this._binder;
            }
            set
            {
                this._binder = value;
            }
        }

        public bool ColloctionIgnorePKValue
        {
            get
            {
                return this._colloctionIgnorePKValue;
            }
            set
            {
                this._colloctionIgnorePKValue = value;
            }
        }

        public bool NeedDBIgnore
        {
            get
            {
                return this._needDBIgnore;
            }
            set
            {
                this._needDBIgnore = value;
            }
        }

        public bool OnlyLocaleVale { get; set; }

        public bool ResetLoacaleValueBy2052 { get; set; }

        public bool SerializeComplexProperty
        {
            get
            {
                return this._serializeComplexProperty;
            }
            set
            {
                this._serializeComplexProperty = value;
            }
        }
    }
}
