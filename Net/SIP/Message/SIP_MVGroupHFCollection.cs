using System.Collections.Generic;

namespace LumiSoft.Net.SIP.Message
{
    /// <summary>
    /// Implements same multi value header fields group. Group can contain one type header fields only.
    /// This is class is used by Via:,Route:, ... .
    /// </summary>
    public class SIP_MVGroupHFCollection<T> where T : SIP_t_Value,new()
    {
        private readonly SIP_Message               m_pMessage;
        private readonly List<SIP_MultiValueHF<T>> m_pFields;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner message that owns this group.</param>
        /// <param name="fieldName">Header field name what group holds.</param>
        public SIP_MVGroupHFCollection(SIP_Message owner,string fieldName)
        {
            m_pMessage  = owner;
            FieldName = fieldName;

            m_pFields = new List<SIP_MultiValueHF<T>>();

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
                    m_pFields.Add((SIP_MultiValueHF<T>)h);
                }
            }
        }

        /// <summary>
        /// Add new header field on the top of the whole header.
        /// </summary>
        /// <param name="value">Header field value.</param>
        public void AddToTop(string value)
        {            
            m_pMessage.Header.Insert(0,FieldName,value);
            Refresh();
        }

        /// <summary>
        /// Add new header field on the bottom of the whole header.
        /// </summary>
        /// <param name="value">Header field value.</param>
        public void Add(string value)
        {            
            m_pMessage.Header.Add(FieldName,value);
            Refresh();
        }

        /// <summary>
        /// Removes all specified header fields with their values.
        /// </summary>
        public void RemoveAll()
        {
            m_pMessage.Header.RemoveAll(FieldName);
            m_pFields.Clear();
        }

        /// <summary>
        /// Gets top most header field first value. 
        /// </summary>
        public T GetTopMostValue()
        {
            if(m_pFields.Count > 0){
                return m_pFields[0].Values[0];
            }

            return null;
        }

        /// <summary>
        /// Removes top most header field first value. If value is the last value, 
        /// the whole header field will be removed.
        /// </summary>
        public void RemoveTopMostValue()
        {
            if(m_pFields.Count > 0){                
                SIP_MultiValueHF<T> h = m_pFields[0];
                if(h.Count > 1){
                    h.Remove(0);
                }
                else{
                    m_pMessage.Header.Remove(m_pFields[0]);
                    m_pFields.Remove(m_pFields[0]);
                }
            }
        }

        /// <summary>
        /// Removes last value. If value is the last value n header field, the whole header field will be removed.
        /// </summary>
        public void RemoveLastValue()
        {
            SIP_MultiValueHF<T> h = m_pFields[m_pFields.Count - 1];
            if(h.Count > 1){
                h.Remove(h.Count - 1);
            }
            else{
                m_pMessage.Header.Remove(m_pFields[0]);
                m_pFields.Remove(h);
            }
        }

        /// <summary>
        /// Gets all header field values.
        /// </summary>
        /// <returns></returns>
        public T[] GetAllValues()
        {
            List<T> retVal = new List<T>();
            foreach(SIP_MultiValueHF<T> h in m_pFields){
                foreach(SIP_t_Value v in h.Values){
                    retVal.Add((T)v);
                }
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
        public SIP_MultiValueHF<T>[] HeaderFields
        {
            get{ return m_pFields.ToArray(); }
        }
    }
}
