﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using Hyak.Common;
using Microsoft.Azure.Commands.Sql.InstanceActiveDirectoryAdministrator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Microsoft.Azure.Commands.Sql.InstanceActiveDirectoryAdministrator.Cmdlet
{
    /// <summary>
    /// Cmdlet to create a new Azure SQL Instance Active Directory administrator
    /// </summary>
    [Cmdlet("Set", ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "SqlInstanceActiveDirectoryAdministrator", DefaultParameterSetName = UseResourceGroupAndInstanceNameParameterSet, ConfirmImpact = ConfirmImpact.Medium, SupportsShouldProcess = true), OutputType(typeof(AzureSqlInstanceActiveDirectoryAdministratorModel))]
    public class SetAzureSqlInstanceActiveDirectoryAdministrator : AzureSqlInstanceActiveDirectoryAdministratorCmdletBase
    {
        /// <summary>
        /// Azure Active Directory display name for a user or group
        /// </summary>
        [Parameter(Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            Position = 2,
            HelpMessage = "Specifies the display name of the user or group for whom to grant permissions. This display name must exist in the active directory associated with the current subscription.")]
        [ValidateNotNullOrEmpty()]
        public string DisplayName { get; set; }

        /// <summary>
        /// Azure Active Directory object id for a user or group
        /// </summary>
        [Parameter(Mandatory = true,
            ValueFromPipelineByPropertyName = true,
            Position = 3,
            HelpMessage = "Specifies the object ID of the user or group in Azure Active Directory for which to grant permissions.")]
        [ValidateNotNullOrEmpty()]
        public Guid ObjectId { get; set; }

        /// <summary>
        /// Get the entities from the service
        /// </summary>
        /// <returns>The list of entities</returns>
        protected override IEnumerable<AzureSqlInstanceActiveDirectoryAdministratorModel> GetEntity()
        {
            List<AzureSqlInstanceActiveDirectoryAdministratorModel> currentActiveDirectoryAdmins = null;
            try
            {
                currentActiveDirectoryAdmins = new List<AzureSqlInstanceActiveDirectoryAdministratorModel>()
                {
                    ModelAdapter.GetInstanceActiveDirectoryAdministrator(GetResourceGroupName(), GetInstanceName()),
                };
            }
            catch (Rest.Azure.CloudException ex)
            {
                if (ex.Response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    // Unexpected exception encountered
                    throw;
                }
            }

            return currentActiveDirectoryAdmins;
        }

        /// <summary>
        /// Create the list of models from a list of user input
        /// </summary>
        /// <param name="model">A IEnumerable of models retrieved from service</param>
        /// <returns>A list of models that was passed in</returns>
        protected override IEnumerable<AzureSqlInstanceActiveDirectoryAdministratorModel> ApplyUserInputToModel(IEnumerable<AzureSqlInstanceActiveDirectoryAdministratorModel> model)
        {
            List<Model.AzureSqlInstanceActiveDirectoryAdministratorModel> newEntity = new List<AzureSqlInstanceActiveDirectoryAdministratorModel>();
            newEntity.Add(new AzureSqlInstanceActiveDirectoryAdministratorModel()
            {
                ResourceGroupName = GetResourceGroupName(),
                InstanceName = GetInstanceName(),
                DisplayName = DisplayName,
                ObjectId = ObjectId,
            });
            return newEntity;
        }

        /// <summary>
        /// Update the Azure SQL Instance Active Directory administrator
        /// </summary>
        /// <param name="entity">A list of models to update the list</param>
        /// <returns>A list of the persisted entities</returns>
        protected override IEnumerable<AzureSqlInstanceActiveDirectoryAdministratorModel> PersistChanges(IEnumerable<AzureSqlInstanceActiveDirectoryAdministratorModel> entity)
        {
            return new List<AzureSqlInstanceActiveDirectoryAdministratorModel>() {
                ModelAdapter.UpsertInstanceActiveDirectoryAdministrator(GetResourceGroupName(), GetInstanceName(), entity.First())
            };
        }
    }
}
