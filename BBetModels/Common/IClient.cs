using BBetModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;

namespace BBetModels
{
    public interface IClient0 : IDisposable
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        Guid Guid { get; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        ClientStates State { get; }

        /// <summary>
        /// EndPoint
        /// </summary>
        IPEndPoint EndPoint { get; }

        /// <summary>
        /// IsAuthenticated
        /// </summary>
        bool IsAuthenticated { get; set; }

    }
}
