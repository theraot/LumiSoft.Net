using System;
using System.Text;

namespace LumiSoft.Net.RTP
{
    /// <summary>
    /// Implements RTP <b>participant</b>. Defined in RFC 3550.
    /// </summary>
    public class RTP_Participant_Remote : RTP_Participant
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="cname">Canonical name of participant. For example: john.doe@domain.com-randomTag.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>cname</b> is null reference.</exception>
        internal RTP_Participant_Remote(string cname) : base(cname)
        {
        }

        // TODO: PRIV

        /// <summary>
        /// Is raised when participant data changed.
        /// </summary>
        public event EventHandler<RTP_ParticipantEventArgs> Changed;

        /// <summary>
        /// Gets email address. For example "John.Doe@example.com". Value null means not specified.
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Gets location string. It may be geographic address or for example chat room name.
        /// Value null means not specified.
        /// </summary>
        public string Location { get; private set; }

        /// <summary>
        /// Gets the real name, eg. "John Doe". Value null means not specified.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets note text. The NOTE item is intended for transient messages describing the current state
        /// of the source, e.g., "on the phone, can't talk". Value null means not specified.
        /// </summary>
        public string Note { get; private set; }

        /// <summary>
        /// Gets phone number. For example "+1 908 555 1212". Value null means not specified.
        /// </summary>
        public string Phone { get; private set; }

        /// <summary>
        /// Gets streaming application name/version.
        /// Value null means not specified.
        /// </summary>
        public string Tool { get; private set; }

        /// <summary>
        /// Returns participant as string.
        /// </summary>
        /// <returns>Returns participant as string.</returns>
        public override string ToString()
        {
            var retVal = new StringBuilder();

            retVal.AppendLine("CNAME: " + CNAME);
            if (!string.IsNullOrEmpty(Name))
            {
                retVal.AppendLine("Name: " + Name);
            }
            if (!string.IsNullOrEmpty(Email))
            {
                retVal.AppendLine("Email: " + Email);
            }
            if (!string.IsNullOrEmpty(Phone))
            {
                retVal.AppendLine("Phone: " + Phone);
            }
            if (!string.IsNullOrEmpty(Location))
            {
                retVal.AppendLine("Location: " + Location);
            }
            if (!string.IsNullOrEmpty(Tool))
            {
                retVal.AppendLine("Tool: " + Tool);
            }
            if (!string.IsNullOrEmpty(Note))
            {
                retVal.AppendLine("Note: " + Note);
            }

            return retVal.ToString().TrimEnd();
        }

        /// <summary>
        /// Updates participant data from SDES items.
        /// </summary>
        /// <param name="sdes">SDES chunk.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>sdes</b> is null reference value.</exception>
        internal void Update(RTCP_Packet_SDES_Chunk sdes)
        {
            if (sdes == null)
            {
                throw new ArgumentNullException("sdes");
            }

            bool changed = false;
            if (!string.IsNullOrEmpty(sdes.Name) && !string.Equals(Name, sdes.Name))
            {
                Name = sdes.Name;
                changed = true;
            }
            if (!string.IsNullOrEmpty(sdes.Email) && !string.Equals(Email, sdes.Email))
            {
                Email = sdes.Email;
                changed = true;
            }
            if (!string.IsNullOrEmpty(sdes.Phone) && !string.Equals(Phone, sdes.Phone))
            {
                Phone = sdes.Phone;
                changed = true;
            }
            if (!string.IsNullOrEmpty(sdes.Location) && !string.Equals(Location, sdes.Location))
            {
                Location = sdes.Location;
                changed = true;
            }
            if (!string.IsNullOrEmpty(sdes.Tool) && !string.Equals(Tool, sdes.Tool))
            {
                Tool = sdes.Tool;
                changed = true;
            }
            if (!string.IsNullOrEmpty(sdes.Note) && !string.Equals(Note, sdes.Note))
            {
                Note = sdes.Note;
                changed = true;
            }

            if (changed)
            {
                OnChanged();
            }
        }

        /// <summary>
        /// Raises <b>Changed</b> event.
        /// </summary>
        private void OnChanged()
        {
            if (Changed != null)
            {
                Changed(this, new RTP_ParticipantEventArgs(this));
            }
        }
    }
}
