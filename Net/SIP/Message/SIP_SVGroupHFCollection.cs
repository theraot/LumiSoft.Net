using System.Collections.Generic;

namespace LumiSoft.Net.SIP.Message
{
    /// <summary>
    /// Implements same single value header fields group. Group can contain one type header fields only.
    /// This is class is used by Authorization:,Proxy-Authorization:, ... .
    /// </summary>
    public class SIP_SVGroupHFCollection<T> where T : SIP_t_Value
    {
        private readonly SIP_Message                m_pMessage;
        private readonly List<SIP_SingleValueHF<T>> m_pFields;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner message that owns this group.</param>
        /// <param name="fieldName">Header field name what group holds.</param>
        public SIP_SVGroupHFCollection(SIP_Message owner,string fieldName)
        {
            m_pMessage  = owner;
            FieldName = fieldName;

            m_pFields = new List<SIP_SingleValueHF<T>>();

            Refresh();
        }

        /// <summary>
        /// Refreshes header fields in group from actual header.
        /// </summary>
        private void Refresh()
        {
            m_pFields.Clear();
           
            foreach(SIP_HeaderField h in m_pMessage.Header){
                if(h.Name.ToLower() == FieldName.ToLower()){                
                    m_pFields.Add((SIP_SingleValueHF<T>)h);
                }
            }
        }

        /// <summary>
        /// Adds specified header field value to the end of header.
        /// </summary>
        /// <param name="value">Header field value.</param>
        public void Add(string value)
        {
            m_pMessage.Header.Add(FieldName,value);
            Refresh();
        }

        /// <summary>
        /// Removes header field from specified index.
        /// </summary>
        /// <param name="index">Index of the header field what to remove. Index is relative ths group.</param>
        public void Remove(int index)
        {
            m_pMessage.Header.Remove(m_pFields[index]);
            m_pFields.RemoveAt(index);
        }
                                
        /// <summary>
        /// Removes specified header field from header.
        /// </summary>
        /// <param name="field">Header field to remove.</param>
        public void Remove(SIP_SingleValueHF<T> field)
        {
            m_pMessage.Header.Remove(field);
            m_pFields.Remove(field);
        }

        /// <summary>
        /// Removes all this gorup header fields from header.
        /// </summary>
        public void RemoveAll()
        {
            foreach(SIP_SingleValueHF<T> h in m_pFields){
                m_pMessage.Header.Remove(h);
            }
            m_pFields.Clear();
        }

        /// <summary>
        /// Gets the first(Top-Most) header field. Returns null if no header fields.
        /// </summary>
        /// <returns>Returns first header field or null if no header fields.</returns>
        public SIP_SingleValueHF<T> GetFirst()
        {
            if(m_pFields.Count > 0){
                return m_pFields[0];
            }

            return null;
        }

        /// <summary>
        /// Gets all header field values.
        /// </summary>
        /// <returns></returns>
        public T[] GetAllValues()
        {
            List<T> retVal = new List<T>();
            foreach(SIP_SingleValueHF<T> hf in m_pFields){
                retVal.Add(hf.ValueX);
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Gets header field name what this group holds.
        /// </summary>
        public string FieldName { get; } = "";

        /// <summary>
        /// Gets number of header fields in this group.
        /// </summary>
        public int Count
        {
            get{ return m_pFields.Count; }
        }

        /// <summary>
        /// Gets header fields what are in this group.
        /// </summary>
        public SIP_SingleValueHF<T>[] HeaderFields
        {
            get{ return m_pFields.ToArray(); }
        }
    }
}
