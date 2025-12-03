using Commons.Utils;
using Commons.Utils.UtilitiesClasses;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Commons.Interfaces.Warehouse
{
    public abstract class ExtensionInterfaceListImpl<T, U> : DataExtensionBase<U> where T : Enum, IConvertible where U : ExtensionInterfaceListImpl<T, U>, new()
    {
        public abstract bool AllowGlobal { get; }

        [XmlElement("ListStringData")]
        public List<SimpleEnumerableList<T, string>> m_cachedListString = [];

        public event Action<int, T, string> EventOnValueChanged;



        #region Utils R/W
        protected string SafeGet(int idx, T key)
        {

            if (m_cachedListString.Count <= idx || !m_cachedListString[idx].ContainsKey(key))
            {
                return null;
            }

            return m_cachedListString[idx][key];
        }
        protected int SafeSet(int idx, T key, string value)
        {
            if (m_cachedListString.Count <= idx)
            {
                m_cachedListString.Add([]);
                idx = m_cachedListString.Count - 1;
            }
            if (value == null)
            {
                m_cachedListString[idx].Remove(key);
            }
            else
            {
                m_cachedListString[idx][key] = value;
            }
            EventOnValueChanged?.Invoke(idx, key, value);
            return idx;
        }

        public void SafeCleanEntry(int idx)
        {
            if (idx < m_cachedListString.Count)
            {
                m_cachedListString.RemoveAt(idx);
                EventOnValueChanged?.Invoke(idx, default, null);
            }
        }

        public void SafeCleanProperty(int idx, T key)
        {
            if (idx < m_cachedListString.Count)
            {
                if (m_cachedListString[idx].ContainsKey(key))
                {
                    m_cachedListString[idx].Remove(key);
                    EventOnValueChanged?.Invoke(idx, key, null);
                }
            }
        }
        #endregion
    }
}
