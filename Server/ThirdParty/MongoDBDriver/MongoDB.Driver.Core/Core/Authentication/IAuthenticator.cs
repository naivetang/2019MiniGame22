/* Copyright 2013-present MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver.Core.Connections;

namespace MongoDB.Driver.Core.Authentication
{
    /// <summary>
    /// Represents a connection authenticator.
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// Gets the name of the authenticator.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Authenticates the connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="description">The connection description.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        void Authenticate(IConnection connection, ConnectionDescription description, CancellationToken cancellationToken);

        /// <summary>
        /// Authenticates the connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="description">The connection description.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task.</returns>
        Task AuthenticateAsync(IConnection connection, ConnectionDescription description, CancellationToken cancellationToken);
        
        /// <summary>
        /// Optionally customizes isMaster command.
        /// </summary>
        /// <param name="isMasterCommand">Initial isMaster command.</param>
        /// <returns>Optionally mutated isMaster command.</returns>
        BsonDocument CustomizeInitialIsMasterCommand(BsonDocument isMasterCommand);
    }
}