using System;

namespace LumiSoft.Net.SIP.Message
{
    /// <summary>
    /// Implements SIP "directive" value. Defined in RFC 3841.
    /// </summary>
    /// <remarks>
    /// <code>
    /// RFC 3841 Syntax:
    ///     directive          = proxy-directive / cancel-directive / fork-directive / recurse-directive /
    ///                          parallel-directive / queue-directive
    ///     proxy-directive    = "proxy" / "redirect"
    ///     cancel-directive   = "cancel" / "no-cancel"
    ///     fork-directive     = "fork" / "no-fork"
    ///     recurse-directive  = "recurse" / "no-recurse"
    ///     parallel-directive = "parallel" / "sequential"
    ///     queue-directive    = "queue" / "no-queue"
    /// </code>
    /// </remarks>
    public class SIP_t_Directive : SIP_t_Value
    {
        /// <summary>
        /// Proccess directives. Defined in rfc 3841 9.1.
        /// </summary>
        public enum DirectiveType
        {
            /// <summary>
            /// This directive indicates whether the caller would like each server to proxy request. 
            /// </summary>
            Proxy,

            /// <summary>
            /// This directive indicates whether the caller would like each server to redirect request.
            /// </summary>
            Redirect,

            /// <summary>
            /// This directive indicates whether the caller would like each proxy server to send a CANCEL 
            /// request to forked branches.
            /// </summary>
            Cancel,

            /// <summary>
            /// This directive indicates whether the caller would NOT want each proxy server to send a CANCEL 
            /// request to forked branches.
            /// </summary>
            NoCancel,

            /// <summary>
            /// This type of directive indicates whether a proxy should fork a request.
            /// </summary>
            Fork,

            /// <summary>
            /// This type of directive indicates whether a proxy should proxy to only a single address.
            /// The server SHOULD proxy the request to the "best" address (generally the one with the highest q-value).
            /// </summary>
            NoFork,

            /// <summary>
            /// This directive indicates whether a proxy server receiving a 3xx response should send 
            /// requests to the addresses listed in the response.
            /// </summary>
            Recurse,

            /// <summary>
            /// This directive indicates whether a proxy server receiving a 3xx response should forward 
            /// the list of addresses upstream towards the caller.
            /// </summary>
            NoRecurse,

            /// <summary>
            /// This directive indicates whether the caller would like the proxy server to proxy 
            /// the request to all known addresses at once.
            /// </summary>
            Parallel,

            /// <summary>
            /// This directive indicates whether the caller would like the proxy server to go through
            /// all known addresses sequentially, contacting the next address only after it has received 
            /// a non-2xx or non-6xx final response for the previous one.
            /// </summary>
            Sequential,

            /// <summary>
            /// This directive indicates whether if the called party is temporarily unreachable, caller 
            /// wants to have its call queued.
            /// </summary>
            Queue,

            /// <summary>
            /// This directive indicates whether if the called party is temporarily unreachable, caller 
            /// don't want its call to be queued.
            /// </summary>
            NoQueue
        }

        /// <summary>
        /// Parses "directive" from specified value.
        /// </summary>
        /// <param name="value">SIP "directive" value.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>value</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public void Parse(string value)
        {
            if(value == null){
                throw new ArgumentNullException("value");
            }

            Parse(new StringReader(value));
        }

        /// <summary>
        /// Parses "directive" from specified reader.
        /// </summary>
        /// <param name="reader">Reader from where to parse.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public override void Parse(StringReader reader)
        {
            /*
                directive          = proxy-directive / cancel-directive / fork-directive / recurse-directive /
                                     parallel-directive / queue-directive
                proxy-directive    = "proxy" / "redirect"
                cancel-directive   = "cancel" / "no-cancel"
                fork-directive     = "fork" / "no-fork"
                recurse-directive  = "recurse" / "no-recurse"
                parallel-directive = "parallel" / "sequential"
                queue-directive    = "queue" / "no-queue"
            */

            if(reader == null){
                throw new ArgumentNullException("reader");
            }

            // Get Method
            string word = reader.ReadWord();
            if(word == null){
                throw new SIP_ParseException("'directive' value is missing !");
            }
            if(word.ToLower() == "proxy"){
                Directive = DirectiveType.Proxy;
            }
            else if(word.ToLower() == "redirect"){
                Directive = DirectiveType.Redirect;
            }
            else if(word.ToLower() == "cancel"){
                Directive = DirectiveType.Cancel;
            }
            else if(word.ToLower() == "no-cancel"){
                Directive = DirectiveType.NoCancel;
            }
            else if(word.ToLower() == "fork"){
                Directive = DirectiveType.Fork;
            }
            else if(word.ToLower() == "no-fork"){
                Directive = DirectiveType.NoFork;
            }
            else if(word.ToLower() == "recurse"){
                Directive = DirectiveType.Recurse;
            }
            else if(word.ToLower() == "no-recurse"){
                Directive = DirectiveType.NoRecurse;
            }
            else if(word.ToLower() == "parallel"){
                Directive = DirectiveType.Parallel;
            }
            else if(word.ToLower() == "sequential"){
                Directive = DirectiveType.Sequential;
            }
            else if(word.ToLower() == "queue"){
                Directive = DirectiveType.Queue;
            }
            else if(word.ToLower() == "no-queue"){
                Directive = DirectiveType.NoQueue;
            }
            else{
                throw new SIP_ParseException("Invalid 'directive' value !");
            }
        }

        /// <summary>
        /// Converts this to valid "directive" value.
        /// </summary>
        /// <returns>Returns "directive" value.</returns>
        public override string ToStringValue()
        {
            /*
                directive          = proxy-directive / cancel-directive / fork-directive / recurse-directive /
                                     parallel-directive / queue-directive
                proxy-directive    = "proxy" / "redirect"
                cancel-directive   = "cancel" / "no-cancel"
                fork-directive     = "fork" / "no-fork"
                recurse-directive  = "recurse" / "no-recurse"
                parallel-directive = "parallel" / "sequential"
                queue-directive    = "queue" / "no-queue"
            */

            if(Directive == DirectiveType.Proxy){
                return "proxy";
            }

            if(Directive == DirectiveType.Redirect){
                return "redirect";
            }
            if(Directive == DirectiveType.Cancel){
                return "cancel";
            }
            if(Directive == DirectiveType.NoCancel){
                return "no-cancel";
            }
            if(Directive == DirectiveType.Fork){
                return "fork";
            }
            if(Directive == DirectiveType.NoFork){
                return "no-fork";
            }
            if(Directive == DirectiveType.Recurse){
                return "recurse";
            }
            if(Directive == DirectiveType.NoRecurse){
                return "no-recurse";
            }
            if(Directive == DirectiveType.Parallel){
                return "parallel";
            }
            if(Directive == DirectiveType.Sequential){
                return "sequential";
            }
            if(Directive == DirectiveType.Queue){
                return "queue";
            }
            if(Directive == DirectiveType.NoQueue){
                return "no-queue";
            }
            throw new ArgumentException("Invalid property Directive value, this should never happen !");
        }

        /// <summary>
        /// Gets or sets directive.
        /// </summary>
        public DirectiveType Directive { get; set; } = DirectiveType.Fork;
    }
}
