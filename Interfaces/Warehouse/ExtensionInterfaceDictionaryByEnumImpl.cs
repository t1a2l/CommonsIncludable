using Commons.Utils.UtilitiesClasses;
using System;
using System.Xml.Serialization;

namespace Commons.Interfaces.Warehouse
{
    public abstract class ExtensionInterfaceDictionaryByEnumImpl<T, U, D> : DataExtensionBase<U> where T : Enum, IConvertible where U : ExtensionInterfaceDictionaryByEnumImpl<T, U, D>, new() where D : class
    {

        [XmlElement("DictData")]
        public SimpleEnumerableList<T, SimpleNonSequentialList<D>> m_cachedDictDataSaved = [];


        public event Action<T, uint, D> EventOnValueChanged;

        #region Utils R/W
        protected D SafeGet(T idx, uint key)
        {

            if (!m_cachedDictDataSaved.ContainsKey(idx) || !m_cachedDictDataSaved[idx].ContainsKey(key))
            {
                return null;
            }

            return m_cachedDictDataSaved[idx][key];
        }
        protected void SafeSet(T idx, uint key, D value)
        {
            if (!m_cachedDictDataSaved.ContainsKey(idx))
            {
                m_cachedDictDataSaved[idx] = [];
            }
            if (value == null)
            {
                m_cachedDictDataSaved[idx].Remove(key);
            }
            else
            {
                m_cachedDictDataSaved[idx][key] = value;
            }
            EventOnValueChanged?.Invoke(idx, key, value);
        }

        public void SafeCleanEntry(T idx)
        {
            if (m_cachedDictDataSaved.ContainsKey(idx))
            {
                m_cachedDictDataSaved.Remove(idx);
            }
            EventOnValueChanged?.Invoke(idx, default, null);
        }

        public void SafeCleanProperty(T idx, uint key)
        {
            if (m_cachedDictDataSaved.ContainsKey(idx))
            {
                if (m_cachedDictDataSaved[idx].ContainsKey(key))
                {
                    m_cachedDictDataSaved[idx].Remove(key);
                    EventOnValueChanged?.Invoke(idx, key, null);
                }
            }
        }
        #endregion
    }
}
