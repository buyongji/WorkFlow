using BU.BOS.Orm.Metadata.DataEntity;
using BU.BOS.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BU.BOS.Orm.DataEntity
{
   [Serializable, DataContract(Namespace = "http://schemas.kingdee.com/dataEntity/")]
    public abstract class DataEntityBase : INotifyPropertyChanged,ISupportInitializeNotification,ISupportInitialize, IObjectWithParent
    {
        #region Fields
        [DataMember(Name = "DirtyFlags", EmitDefaultValue = false)]
        internal string _dirtyFlags;
        [NonSerialized]
        private EventHandlerList _events;
        [DataMember(Name = "FromDatabase", EmitDefaultValue = false)]
        internal bool _fromDatabase;
        [NonSerialized]
        private bool _initializing;
        [NonSerialized]
        private object _parent;
        [DataMember(Name = "Snapshots", EmitDefaultValue = false)]
        internal PkSnapshot[] _pkSnapshots;
        [NonSerialized]
        private BoolDataEntityState _state;
        private static readonly object InitializedEventKey;
        private static readonly object PropertyChangedEventKey;
        private static readonly object PropertyChangingEventKey;
        #endregion

        #region Properties
        [Browsable(false)]
        public DataEntityState DataEntityState
        {
            get
            {
                if (this._state == null)
                {
                    lock (this)
                    {
                        if (this._state == null)
                        {
                            BitArray array;
                            IDataEntityPropertyCollection properties = this.GetDataEntityType().Properties;
                            if (string.IsNullOrEmpty(this._dirtyFlags))
                            {
                                array = new BitArray(properties.Count);
                            }
                            else
                            {
                                array = new BitArray(SerializationUtils.StringToIntArray(this._dirtyFlags))
                                {
                                    Length = properties.Count
                                };
                            }
                            this._state = new BoolDataEntityState(properties, array, this._pkSnapshots, this._fromDatabase);
                            this._pkSnapshots = null;
                            this._dirtyFlags = null;
                        }
                    }
                }
                return this._state;
            }
        }

        protected EventHandlerList Events
        {
            get
            {
                if (this._events == null)
                {
                    this._events = new EventHandlerList();
                }
                return this._events;
            }
        }

        internal bool Initializing
        {
            get
            {
                return this._initializing;
            }
        }

        object IObjectWithParent.Parent
        {
            get
            {
                return this._parent;
            }
            set
            {
                this._parent = value;
            }
        }

        public object Parent
        {
            get
            {
                return this._parent;
            }
        }

        bool ISupportInitializeNotification.IsInitialized
        {
            get
            {
                return !this._initializing;
            }
        }

        #endregion

        #region Methods
        protected DataEntityBase()
        {
        }

        public abstract IDataEntityType GetDataEntityType();
        protected virtual void OnInitialized()
        {
            if (this._events != null)
            {
                EventHandler handler = (EventHandler)this._events[InitializedEventKey];
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }
        protected internal virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (!this._initializing)
            {
                this.DataEntityState.SetPropertyChanged(e);
                if (this._events != null)
                {
                    PropertyChangedEventHandler handler = (PropertyChangedEventHandler)this._events[PropertyChangedEventKey];
                    if (handler != null)
                    {
                        handler(this, e);
                    }
                }
            }
        }

        protected internal virtual void OnPropertyChanging(PropertyChangingEventArgs e)
        {
            if (!this._initializing && (this._events != null))
            {
                PropertyChangingEventHandler handler = (PropertyChangingEventHandler)this._events[PropertyChangingEventKey];
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }

        [OnSerializing]
        internal void OnSerializing(StreamingContext context)
        {
            if (this._state != null)
            {
                PkSnapshotSet pkSnapshotSet = this._state.PkSnapshotSet;
                if ((pkSnapshotSet != null) && (pkSnapshotSet.Snapshots.Count > 0))
                {
                    this._pkSnapshots = pkSnapshotSet.Snapshots.ToArray();
                }
                this._fromDatabase = this._state.FromDatabase;
                this._dirtyFlags = SerializationUtils.IntArrayToString(this._state.GetDirtyFlags());
            }
        }

        void ISupportInitialize.BeginInit()
        {
            this._initializing = true;
        }

        void ISupportInitialize.EndInit()
        {
            if (this._initializing)
            {
                this._initializing = false;
                this.OnInitialized();
            }
        }

        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                this.Events.AddHandler(PropertyChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PropertyChangedEventKey, value);
            }
        }

        public event PropertyChangingEventHandler PropertyChanging
        {
            add
            {
                this.Events.AddHandler(PropertyChangingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PropertyChangingEventKey, value);
            }
        }

        event EventHandler ISupportInitializeNotification.Initialized
        {
            add
            {
                this.Events.AddHandler(InitializedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(InitializedEventKey, value);
            }
        }

        #endregion
    }
}
