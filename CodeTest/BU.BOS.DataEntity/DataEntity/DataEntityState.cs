using BU.BOS.Orm.Metadata.DataEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BU.BOS.Orm.DataEntity
{
    public abstract class DataEntityState
    {
        #region Fields
        private bool _formDatabase;
        private PkSnapshotSet _pkSnapshotSet;
        #endregion

        #region Properties
        public abstract bool DataEntityDirty { get; }
        public bool FromDatabase
        {
            get
            {
                return this._formDatabase;
            }
            protected internal set
            {
                this._formDatabase = value;
                if (this._formDatabase)
                {
                    this.SetDirty(false);
                }
            }
        }
        protected internal PkSnapshotSet PkSnapshotSet
        {
            get
            {
                return this._pkSnapshotSet;
            }
            set
            {
                this._pkSnapshotSet = value;
            }
        }
        #endregion

        #region Methods
        protected DataEntityState()
        {

        }
        protected DataEntityState(PkSnapshot[] pkSnapshots, bool fromDatabase)
        {
            if((pkSnapshots!=null)&& (pkSnapshots.Length>0))
            {
                PkSnapshotSet set = new PkSnapshotSet(pkSnapshots.Length);
                set.Snapshots.AddRange(pkSnapshots);
                this._pkSnapshotSet = set;
            }
            this._formDatabase = fromDatabase;
        }
        public abstract IEnumerable<IDataEntityProperty> GetDirtyProperties();
        public abstract IEnumerable<IDataEntityProperty> GetDirtyProperties(bool includehasDefault);
        public abstract void SetDirty(bool newValue);
        public abstract void SetPropertyChanged(PropertyChangedEventArgs e);
        #endregion
    }
}
