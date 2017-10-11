using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace GDMWebServiceTSP
{    
    [ServiceContract]
    public interface IService1
    {
        #region Getter Functions

        /// <summary>
        /// Get the XML Graph of the experiment marked as "Current"
        /// </summary>
        /// <returns>Current Experiment Graph</returns>
        [OperationContract]
        string GetExperimentGraph();

        /// <summary>
        /// Get the id of the current user session
        /// </summary>
        /// <returns>Current user id</returns>
        [OperationContract]
        string GetSessionVariable();

        /// <summary>
        /// Get Solution Graph of the experiment marked as "Current"
        /// </summary>
        /// <returns>Current Solution Graph</returns>
        [OperationContract]
        byte[] GetSolutionGraph();

        /// <summary>
        /// Checks if this experiment is defined to show a solution window for the experiment marked as "Current"
        /// </summary>
        /// <returns>Time before show solution</returns>
        [OperationContract]
        string GetUseSolutionGraph();

        /// <summary>
        /// Get the Id of the experiment marked as "Current"
        /// </summary>
        /// <returns>Experiment ID</returns>
        [OperationContract]
        int GetCurrentExperimentID();

        #endregion


        #region Setter Functions

        /// <summary>       
        /// Insert the actions the Participant performed to the DB
        /// </summary>  
        [OperationContract]
        bool InsertIntoExperimentActions(int ExperimentID, int PersonID, string VertexID, string Color, DateTime EventTime);

        /// <summary>
        /// Add a new particepant to the DB
        /// </summary>
        /// <returns>ID assigned to participant</returns>
        [OperationContract]
        int AddParticipant(Object turkId);

        [OperationContract]
        object setSessionVariable(string key, string value);

        /// <summary>
        /// Insert new Experiment graph to DB
        /// </summary>
        /// <returns>ID assigned to graph</returns>
        [OperationContract]
        int InsertExperimentGraph(string graph_xml);

        /// <summary>
        /// Update Experiment Results for the experiment marked as "Current" with the participant's response 
        /// </summary>  
        [OperationContract]
        bool SolvedProblemGraph(int ParticipantID, bool Solved, DateTime SolvedTime);

        #endregion


        #region Query/boolean Functions

        /// <summary>
        /// Inserts partial Experiment Results
        /// </summary>  
        [OperationContract]
        bool SolutionAcceptancePartial(int ParticipantID, int ExperimentID, DateTime Date, bool AcceptedSolution);

        [OperationContract]
        bool PerformBulkCopy();

        #endregion                                           
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
