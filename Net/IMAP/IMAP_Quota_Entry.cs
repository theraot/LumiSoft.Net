using System;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP QUOTA entry. Defined in RFC 2087 5.1.
    /// </summary>
    public class IMAP_Quota_Entry
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="resourceName">Resource limit name.</param>
        /// <param name="currentUsage">Current resourse usage.</param>
        /// <param name="maxUsage">Maximum allowed resource usage.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>resourceName</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public IMAP_Quota_Entry(string resourceName,long currentUsage,long maxUsage)
        {
            if(resourceName == null){
                throw new ArgumentNullException("resourceName");
            }
            if(resourceName == string.Empty){
                throw new ArgumentException("Argument 'resourceName' value must be specified.","resourceName");
            }

            ResourceName = resourceName;
            CurrentUsage = currentUsage;
            MaxUsage     = maxUsage;
        }


        #region Properties implementation

        /// <summary>
        /// Gets resource limit name.
        /// </summary>
        public string ResourceName { get; } = "";

        /// <summary>
        /// Gets current resource usage.
        /// </summary>
        public long CurrentUsage { get; }

        /// <summary>
        /// Gets maximum allowed resource usage.
        /// </summary>
        public long MaxUsage { get; }

#endregion
    }
}
