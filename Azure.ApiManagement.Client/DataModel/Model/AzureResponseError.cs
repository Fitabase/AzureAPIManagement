using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallStepsLabs.Azure.ApiManagement.DataModel.Model
{
    public class AzureResponseError
    {
        /// <summary>
        /// The link used to get the next page of operations.
        /// </summary>
        public string NextLink { get; set; }
        /// <summary>
        /// The list of operations.
        /// </summary>
        public OperationEntity[] Value { get; set; }
        
    }

    public class OperationEntity
    {
        /// <summary>
        /// Operation name: {provider}/{resource}/{operation}.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The operation supported by Advisor.
        /// </summary>
        public OperationDisplay Display { get; set; }
        
    }

    public class OperationDisplay
    {
        /// <summary>
        /// The description of the operation
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The action that uses can perform, based on their mission level
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// Service provider: Microsoft Advisor
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// Resource on which the operation is perform
        /// </summary>
        public string Resource { get; set; }
    }
}
