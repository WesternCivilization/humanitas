using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SqlCache
{
    public class SqlCacheDataParameterCollection : IDataParameterCollection
    {

        private Dictionary<string, SqlCacheDataParameter> _parameters = new Dictionary<string, SqlCacheDataParameter>();

        public object this[string parameterName]
        {
            get { return this._parameters[parameterName]; }
            set { this._parameters[parameterName] = (SqlCacheDataParameter)value; }
        }

        public object this[int index]
        {
            get { return _parameters[_parameters.Keys.ElementAtOrDefault(index)]; }
            set { _parameters[_parameters.Keys.ElementAtOrDefault(index)] = (SqlCacheDataParameter)value; }
        }

        public bool IsReadOnly { get; set; }

        public bool IsFixedSize { get; set; }

        public int Count { get; set; }

        public object SyncRoot { get; set; }

        public bool IsSynchronized { get; set; }

        public int Add(object value)
        {
            var par = new SqlCacheDataParameter();
            par.Value = value;
            var name = $"parameter{this._parameters.Count + 1}";
            this[name] = par;
            return this._parameters.Keys.ToList().IndexOf(name);
        }

        public void Clear()
        {
            this._parameters.Clear();
        }

        public bool Contains(string parameterName)
        {
            return this._parameters.ContainsKey(parameterName);
        }

        public bool Contains(object value)
        {
            return this._parameters.Any(f => f.Value.Value == value);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        public IEnumerator GetEnumerator()
        {
            return this._parameters.GetEnumerator();
        }

        public int IndexOf(string parameterName)
        {
            return this._parameters.Keys.ToList().IndexOf(parameterName);
        }

        public int IndexOf(object value)
        {
            return this._parameters.Values.ToList().FindIndex(f => f.Value == value);
        }

        public void Insert(int index, object value)
        {
            this.Add(value);
        }

        public void Remove(object value)
        {
            var key = string.Empty;
            foreach(var par in this._parameters)
            {
                if (par.Value.Value == value) key = par.Key;
            }
            this.RemoveAt(key);
        }

        public void RemoveAt(string parameterName)
        {
            this._parameters.Remove(parameterName);
        }

        public void RemoveAt(int index)
        {
            this.RemoveAt(this._parameters.Keys.ElementAt(index));
        }

    }
}
